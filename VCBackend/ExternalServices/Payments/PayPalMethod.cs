using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Caching;
using PayPal.Api;
using VCBackend.Exceptions;
using System.Text.RegularExpressions;

namespace VCBackend.ExternalServices.Payments
{
    public class PayPalMethod : IPaymentMethod
    {
        private String ClientID, ClientSecret;

        public PayPalMethod()
        {
            ClientID = ConfigurationManager.AppSettings["PP_ClientID"];
            ClientSecret = ConfigurationManager.AppSettings["PP_ClientSecret"];
        }

        PaymentRequest IPaymentMethod.PaymentBegin(PaymentRequest request)
        {
            OAuthTokenCredential tokenCredential = null;

            try
            {
                ValidatePaymentRequest(request);

                tokenCredential = new OAuthTokenCredential(ClientID, ClientSecret);

                var accessToken = tokenCredential.GetAccessToken();

                APIContext context = new APIContext(accessToken);

                var payer = new Payer();
                payer.payment_method = "paypal";

                var amount = new Amount();
                amount.currency = request.Currency;
                amount.total = request.Price;

                var transaction = new Transaction();
                transaction.amount = amount;
                transaction.description = "Payment to load the virtual card.";

                var redirect_urls = new RedirectUrls();
                redirect_urls.return_url = "http://vcardcallback/paypal/end";
                redirect_urls.cancel_url = "http://vcardcallback/paypal/cancel";

                var payment = new Payment();
                payment.intent = "sale";
                payment.payer = payer;
                payment.transactions = new List<Transaction>();
                payment.transactions.Add(transaction);
                payment.redirect_urls = redirect_urls;

                var createdPayment = payment.Create(context);


                request.PaymentData = createdPayment.ConvertToJson();
                request.PaymentMethod = "paypal";
                request.State = createdPayment.state;

                if (createdPayment.state == PaymentRequest.STATE_CREATED)
                {
                    request.RedirectURL = createdPayment.GetApprovalUrl();
                    request.PaymentId = createdPayment.id;
                    return request;
                }
                else throw new PayPalPaymentFailed(String.Format("PayPal payment creation failed. State: {0}.", request.State));
            }
            catch (VCException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new PayPalPaymentFailed(String.Format("Error calling PayPal."));
            }
        }

        PaymentRequest IPaymentMethod.PaymentEnd(PaymentRequest request)
        {
            OAuthTokenCredential tokenCredential =
                new OAuthTokenCredential(ClientID, ClientSecret);

            string accessToken = tokenCredential.GetAccessToken();

            APIContext context = new APIContext(accessToken);

            if (request.State == PaymentRequest.STATE_CREATED)
            {
                Payment payment = Payment.Get(context, request.PaymentId);
                
                request.State = payment.state;

                if (request.State == PaymentRequest.STATE_CREATED && request.PayerId != null)
                {
                    PaymentExecution paymentExecution = new PaymentExecution();
                    paymentExecution.payer_id = request.PayerId;

                    Payment executedPayment = payment.Execute(context, paymentExecution);

                    request.State = executedPayment.state;

                    if (request.State == PaymentRequest.STATE_APPROVED)
                        return request;
                }
            }

            throw new PayPalPaymentFailed(String.Format("Paypal payment is in state: {0}.", request.State));
        }

        PaymentRequest IPaymentMethod.PaymentCancel(PaymentRequest request)
        {
            if (request.State != PaymentRequest.STATE_APPROVED)
                request.State = PaymentRequest.STATE_CANCELED;
            return request;
        }

        private bool ValidatePaymentRequest(PaymentRequest request)
        {
            //Validate the request fields. Actually I'm just verifying currency and ammount.
            Regex priceRgx = new Regex(@"[0-9]{1,7}\.[0-9]{2}");
            if (request.Price != null && priceRgx.IsMatch(request.Price))
                return true;
            else throw new PayPalPaymentFailed("Invalid price format!");
        }

    }
}