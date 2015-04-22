using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Caching;
using PayPal.Api;
using VCBackend.Business_Rules.Exceptions;

namespace VCBackend.Business_Rules.Payments
{
    public class PayPal : IPaymentMethod
    {
        private String ClientID, ClientSecret;

        public PayPal()
        {
            ClientID = ConfigurationManager.AppSettings["PP_ClientID"];
            ClientSecret = ConfigurationManager.AppSettings["PP_ClientSecret"];
        }

        ProdPayment IPaymentMethod.PaymentBegin(ProdPayment request)
        {   
            OAuthTokenCredential tokenCredential =
                new OAuthTokenCredential(ClientID, ClientSecret);
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
            redirect_urls.return_url = "http://localhost:64390/api/pay/paypal/end";
            redirect_urls.cancel_url = "http://localhost:64390/api/pay/paypal/cancel";

            var payment = new Payment();
            payment.intent = "sale";
            payment.payer = payer;
            payment.transactions.Add(transaction);
            payment.redirect_urls = redirect_urls;

            var createdPayment = payment.Create(context);

            request.PaymentData = createdPayment.ConvertToJson();
            request.PaymentMethod = "paypal";
            request.State = createdPayment.state;

            if (createdPayment.state == ProdPayment.STATE_CREATED)
            {
                request.RedirectURL = createdPayment.GetApprovalUrl();
                request.PaymentId = createdPayment.id;
                return request;
            }
            else throw new PayPalPaymentFailed(String.Format("PayPal payment creation failed. State: {0}.", request.State));
        }

        ProdPayment IPaymentMethod.PaymentEnd(ProdPayment request)
        {
            OAuthTokenCredential tokenCredential =
                new OAuthTokenCredential(ClientID, ClientSecret);

            string accessToken = tokenCredential.GetAccessToken();

            APIContext context = new APIContext(accessToken);

            if (request.State == ProdPayment.STATE_CREATED)
            {
                Payment payment = Payment.Get(context, request.PaymentId);
                
                request.State = payment.state;

                if (request.State == ProdPayment.STATE_APPROVED && request.PayerId != null)
                {
                    PaymentExecution paymentExecution = new PaymentExecution();
                    paymentExecution.payer_id = request.PayerId;

                    Payment executedPayment = payment.Execute(context, paymentExecution);

                    request.State = executedPayment.state;

                    if (request.State == ProdPayment.STATE_APPROVED)
                        return request;
                }
            }

            throw new PayPalPaymentFailed(String.Format("Paypal payment is in state: {0}.", request.State));
        }

        ProdPayment IPaymentMethod.PaymentCancel(ProdPayment request)
        {
            if (request.State != ProdPayment.STATE_APPROVED)
                request.State = ProdPayment.STATE_CANCELED;
            return request;
        }

    }
}