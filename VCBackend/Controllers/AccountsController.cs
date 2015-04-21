using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VCBackend.Filters;

namespace VCBackend.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountsController : ApiController
    {

        [Route("products")]
        [VCAuthenticate]
        public void /*List<ProductDto>*/ GetProductsList()
        {
        }

        [Route("payments/list")]
        [VCAuthenticate]
        public void /*List<PaymentGatewayDto>*/ GetPaymentMethods()
        {
        }

        //POST api/account/load/paypal/begin?t=asd32&c=EUR&a=10.0
        //https://developer.paypal.com/docs/integration/direct/rest_api_payment_country_currency_support/
        [Route("load/{method}/begin")]
        [VCAuthenticate]
        public void /*PaypalPaymentDto*/ PostLoadBegin([FromUri] String method, [FromUri] String c, [FromUri] String a = null)
        {
        }

        //POST api/account/load/paypal/end?t=asd32&u=PayerID&p=PaymentID
        [Route("load/{method}/end")]
        [VCAuthenticate]
        public void PostLoadEnd([FromUri] String method, [FromUri] String u, [FromUri] String p)
        {
        }

        public void PostLoadProduct([FromUri] String p)
        {
        }

        public void GetToken([FromUri] String p)
        {
        }

        [Route("load/{method}/cancel")]
        [VCAuthenticate]
        public void DeleteLoadCard()
        {
        }

    }
}