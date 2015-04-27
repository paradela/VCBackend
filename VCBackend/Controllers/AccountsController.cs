using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VCBackend.Filters;
using VCBackend.Models.Dto;
using VCBackend.Exceptions;
using VCBackend.Errors;
using VCBackend.Services;
using VCBackend.Repositories;

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
                UnitOfWork uw = new UnitOfWork();
                int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(AuthDev);
                //IService service = new 
                return null;//BRulesApi.GetPaymentMethods();
            }
            catch (VCException ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
        }

        //POST api/account/load/paypal/begin?t=asd32&a=10.0
        [Route("pay/{method}/begin")]
        [VCAuthenticate]
        public PaymentDto PostPaymentBegin([FromUri] String method, [FromUri] String a)
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(AuthDev);
                BeginPaymentService service = new BeginPaymentService(uw, dev);
                service.Amount = a;
                service.Method = method;
                if (service.Execute())
                {
                    return service.PaymentDto;
                }
                else return null;
            }
            catch (VCException ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
        }

        //POST api/account/load/paypal/end?t=asd32&u=PayerID&p=PaymentID
        [Route("pay/{method}/end")]
        [VCAuthenticate]
        public BalanceDto PostPaymentEnd([FromUri] String method, [FromUri] String u, [FromUri] String p)
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(AuthDev);
                EndPaymentService service = new EndPaymentService(uw, dev);
                service.Method = method;
                service.PayerId = u;
                service.PaymentId = p;
                if (service.Execute())
                {
                    return service.BalanceDto;
                }
                else return null;
            }
            catch (VCException ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
        }

        [Route("pay/{method}/cancel")]
        [VCAuthenticate]
        public void DeletePaymentRequest([FromUri] String AuthDevice, [FromUri] String method, [FromUri] String p)
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(AuthDev);
                CancelPaymentService service = new CancelPaymentService(uw, dev);
                service.Method = method;
                service.PaymentId = p;
                service.Execute();
            }
            catch (VCException ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
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