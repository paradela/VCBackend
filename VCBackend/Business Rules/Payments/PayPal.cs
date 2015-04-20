using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Caching;
using PayPal.Api;

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

        PaymentRequest IPaymentMethod.PaymentBegin(params string[] args)
        {
            
            OAuthTokenCredential tokenCredential =
                new OAuthTokenCredential(ClientID, ClientSecret);
            var accessToken = tokenCredential.GetAccessToken();

            APIContext context = new APIContext(accessToken);
            

            var payer = new Payer();
            payer.payment_method = "paypal";

            
            var amount = new Amount();
            amount.currency = "EUR"; //passed in the arguments
            amount.total = "10.0";

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

            throw new NotImplementedException();
        }

        bool IPaymentMethod.PaymentEnd(PaymentRequest request)
        {
            throw new NotImplementedException();
        }

        bool IPaymentMethod.PaymentCancel(PaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}