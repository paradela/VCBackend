﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VCBackend.Filters;
using VCBackend.Business_Rules;
using VCBackend.Models.Dto;
using VCBackend.Business_Rules.Exceptions;
using VCBackend.Business_Rules.Errors;

namespace VCBackend.Controllers
{
    /// <summary>
    /// This endpoints provide an API for a user to add money to his Account, load his Virtual Card, get Card Tokens, among others.
    /// </summary>
    [RoutePrefix("api/account")]
    public class AccountsController : ApiController
    {

        [Route("products")]
        public void /*List<ProductDto>*/ GetProductsList()
        {
        }

        [Route("payments/list")]
        [VCAuthenticate]
        public IList<PaymentMethodDto> GetPaymentMethods()
        {
            try
            {
                return BRulesApi.GetPaymentMethods();
            }
            catch (VCException ex)
            {
                throw new ErrorResponse(ex);
            }
            catch (Exception ex)
            {
                throw new ErrorResponse(ex);
            }
        }

        //POST api/account/load/paypal/begin?t=asd32&c=EUR&a=10.0
        //https://developer.paypal.com/docs/integration/direct/rest_api_payment_country_currency_support/
        [Route("pay/{method}/begin")]
        [VCAuthenticate]
        public PaymentDto PostPaymentBegin([FromUri] String method, [FromUri] String c, [FromUri] String a)
        {
            int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);

            try
            {
                PaymentDto dto = BRulesApi.PaymentBegin(AuthDev, method, c, a);
                return dto;
            }
            catch (VCException ex)
            {
                throw new ErrorResponse(ex);
            }
            catch (Exception ex)
            {
                throw new ErrorResponse(ex);
            }
        }

        //POST api/account/load/paypal/end?t=asd32&u=PayerID&p=PaymentID
        [Route("pay/{method}/end")]
        [VCAuthenticate]
        public BalanceDto PostPaymentEnd([FromUri] String method, [FromUri] String u, [FromUri] String p)
        {
            int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);

            try
            {
                BalanceDto dto = BRulesApi.PaymentEnd(AuthDev, method, u, p);
                return dto;
            }
            catch (VCException ex)
            {
                throw new ErrorResponse(ex);
            }
            catch (Exception ex)
            {
                throw new ErrorResponse(ex);
            }
        }

        [Route("pay/{method}/cancel")]
        [VCAuthenticate]
        public void DeletePaymentRequest([FromUri] String AuthDevice, [FromUri] String method, [FromUri] String p)
        {
            int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);

            try
            {
                BRulesApi.PaymentCancel(AuthDev, method, p);
            }
            catch (VCException ex)
            {
                throw new ErrorResponse(ex);
            }
            catch (Exception ex)
            {
                throw new ErrorResponse(ex);
            }
        }

        [Route("vcard/load/{product}")]
        [VCAuthenticate]
        public void PostLoadProduct([FromUri] String p)
        {
        }

        [Route("vcard/token")]
        [VCAuthenticate]
        public void GetToken()
        {
        }

    }
}