using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Web.WebSockets;
using System.Net.WebSockets;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using VCBackend.Filters;
using VCBackend.Models.Dto;
using VCBackend.Exceptions;
using VCBackend.Errors;
using VCBackend.Services;
using VCBackend.Repositories;
using VCBackend.Utility.Security;

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
        public void DeletePaymentRequest([FromUri] String method, [FromUri] String p)
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
        //?d="2015-05-31 01:02"
        [Route("vcard/token")]
        [VCAuthenticate]
        public VCardTokenDto GetToken([FromUri] String d)
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(AuthDev);
                GetVCTokenService service = new GetVCTokenService(uw, dev);
                service.DateInitial = d;
                service.Execute();
                return service.VCardTokenDto;
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

        [Route("vcard/load/{ammount}")]
        [VCAuthenticate]
        public CardBalanceDto PostLoadProduct([FromUri] String ammount)
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(AuthDev);
                LoadVCardService service = new LoadVCardService(uw, dev);
                double ammnt;
                if (!Double.TryParse(ammount, out ammnt))
                    throw new InvalidLoadRequest(String.Format("Ammount {} is not a valid ammount.", ammount));
                service.Amount = ammnt;
                service.Execute();
                return service.CardBalanceDto;
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

        [Route("vcard/ws")]
        [VCAuthenticate]
        public HttpResponseMessage GetWebSocketValidation([FromUri] String t)
        {//t[oken] is parsed in the VCardTxRxAPDU callback to get the authenticated user
            if (HttpRuntime.UsingIntegratedPipeline && HttpContext.Current.IsWebSocketRequest)
            {
                HttpContext.Current.AcceptWebSocketRequest(VCardTxRxAPDU);
            }
            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        private async Task VCardTxRxAPDU(AspNetWebSocketContext context)
        {
            WebSocket socket = context.WebSocket;
            UnitOfWork uw = new UnitOfWork();

            Device dev = GetAuthenticatedDevice(context, uw);

            if (dev == null) return;

            using (var trx = uw.TransactionBegin())
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                VCard card = dev.Owner.Account.VCard;

                while (true)
                {
                    WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    if (socket.State == WebSocketState.Open)
                    {
                        byte[] apdu = new byte[result.Count];
                        System.Array.Copy(buffer.Array, apdu, result.Count);
                        byte[] res = card.IsoATxRxAPDU(apdu);
                        buffer = new ArraySegment<byte>(res);

                        await socket.SendAsync(
                            buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
                    }
                    else
                    {
                        break;
                    }
                }

                trx.Commit();
            }
        }

        private static Device GetAuthenticatedDevice(AspNetWebSocketContext context, UnitOfWork uw)
        {
            var query = context.RequestUri.Query;
            var queryCollection = System.Web.HttpUtility.ParseQueryString(query);
            var token = queryCollection["t"];
            var payload = AuthToken.ValidateToken(token);
            var uid = payload["user_id"];
            var did = payload["device_id"];
            Device dev = uw.DeviceRepository.Get(filter: d => (d.Owner.Id == (int)uid && d.AccessTokens.AuthToken != token && d.Id == (int)did)).FirstOrDefault();
            return dev;
        }

    }
}