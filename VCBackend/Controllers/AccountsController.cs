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
        [Route("{mode}")]
        [VCAuthenticate]
        public void PutValidationMode([FromUri] String mode)
        {
            //Change validation mode mode = online || mode = token
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
        public void /*PaypalPaymentDto*/ PostLoadCardBegin([FromUri] String method, [FromUri] String c,[FromUri] String a)
        {
        }

        //POST api/account/load/paypal/end?t=asd32&u=PayerID&p=PaymentID
        [Route("load/{method}/end")]
        [VCAuthenticate]
        public void PostLoadCardEnd([FromUri] String method, [FromUri] String u, [FromUri] String p)
        {
        }

        [Route("load/{method}/cancel")]
        [VCAuthenticate]
        public void DeleteLoadCard()
        {
        }

    }
}