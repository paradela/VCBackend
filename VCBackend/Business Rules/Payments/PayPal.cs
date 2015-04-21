using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Caching;
using PayPal.Api;

namespace VCBackend.Business_Rules.Payments
{
    public class PayPal
    {
        private String ClientID, ClientSecret;

        public PayPal()
        {
            ClientID = ConfigurationManager.AppSettings["PP_ClientID"];
            ClientSecret = ConfigurationManager.AppSettings["PP_ClientSecret"];
        }

        ProdPayment PaymentBegin(ProdPayment request)
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
            redirect_urls.return_url = "http://localhost:64390/api/load/paypal/end";
            redirect_urls.cancel_url = "http://localhost:64390/api/load/paypal/cancel";

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
            else throw new PayPalPaymentFailed(request, String.Format("PayPal payment {0}.", request.State));
        }

        ProdPayment PaymentEnd(ProdPayment request)
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
                    //payment.Execute(context,
                    PaymentExecution paymentExecution = new PaymentExecution();
                    paymentExecution.payer_id = request.PayerId;

                    Payment executedPayment = payment.Execute(context, paymentExecution);

                    request.State = executedPayment.state;

                    if (request.State == ProdPayment.STATE_APPROVED)
                        return request;
                }
            }

            throw new PayPalPaymentFailed(request, String.Format("Paypal payment {0}.", request.State));
        }

        ProdPayment PaymentCancel(ProdPayment request)
        {
            if (request.State != ProdPayment.STATE_APPROVED)
                request.State = ProdPayment.STATE_CANCELED;
            return request;
        }
    }

    public class PayPalPaymentFailed : Exception
    {
        public ProdPayment Payment { get; set; }

        public PayPalPaymentFailed(ProdPayment payment, String message)
            : base(message)
        {
            this.Payment = payment;
        }
    }
}