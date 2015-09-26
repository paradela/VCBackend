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
using System.Text;
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

        [Route("balance")]
        [VCAuthenticate]
        public BalanceDto GetAccountBalance()
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(AuthDev);
                GetAccountBalanceService service = new GetAccountBalanceService(uw, dev);
                if (service.Execute())
                    return service.Balance;
                return null;
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

        [Route("vcard/balance")]
        [VCAuthenticate]
        public CardBalanceDto GetVCardBalance()
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(AuthDev);
                ReadVCardBalanceService service = new ReadVCardBalanceService(uw, dev);
                service.Execute();
                return service.BalanceDto;
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


        [Route("vcard/load/{Amount}")]
        [VCAuthenticate]
        public LoadResultDto PostLoadProduct([FromUri] String Amount)
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(AuthDev);
                LoadVCardService service = new LoadVCardService(uw, dev);
                int am;
                if (!Int32.TryParse(Amount, out am))
                    throw new InvalidLoadRequest(String.Format("Ammount {0} is not a valid ammount.", Amount));
                double amount = am * 0.01;
                service.Amount = amount;
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

        [Route("vcard/run/apdu")]
        [VCAuthenticate]
        public ApduResponseDto PostCardApdu([FromUri] String hexapdu)
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                int AuthDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(AuthDev);
                RunApduService service = new RunApduService(uw, dev);
                service.HexApdu = hexapdu;
                service.Execute();
                return service.ResponseDto;
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

        //[Route("vcard/ws")]
        //public HttpResponseMessage GetWebSocketValidation()
        //{
        //    if (HttpRuntime.UsingIntegratedPipeline && HttpContext.Current.IsWebSocketRequest)
        //    {
        //        HttpContext.Current.AcceptWebSocketRequest(VCardTxRxAPDU);
        //    }
        //    return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        //}

        private const byte OK = 0x00;
        private const byte MSG_APDU = 0x01;
        private const byte MSG_AUTH_TOKEN = 0x02;
        private const byte MSG_REFRESG_TOKEN = 0x03;
        private const byte ERROR_EXPIRED_TOKEN = 0xA1;
        private const byte ERROR_INVALID_TOKEN = 0xA2;
        private const byte ERROR_NOT_AUTHENTICATED = 0xA3;
        private const byte ERROR_UNKNOWN = 0xA4;

        /*
         * 0        1                     n
         * |type_msg|------msg-----------|
         */
        private async Task VCardTxRxAPDU(AspNetWebSocketContext context)
        {
            WebSocket socket = context.WebSocket;
            
            UnitOfWork uw = new UnitOfWork();
            bool authenticated = false;

            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
            VCard card = null;

            while (true)
            {
                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (socket.State == WebSocketState.Open)
                {
                    byte[] res = null;
                    byte[] msg = new byte[result.Count - 1];
                    System.Array.Copy(buffer.Array, 1, msg, 0, result.Count);
                    byte type = buffer.Array[0];

                    switch (type)
                    {
                        case MSG_APDU:
                            if (authenticated && card != null)
                            {
                                byte[] tmp = card.IsoATxRxAPDU(msg);
                                res = new byte[tmp.Length + 1];
                                res[0] = OK;
                                System.Array.Copy(tmp, 0, res, 1, tmp.Length);
                            }
                            else res = new byte[] { ERROR_NOT_AUTHENTICATED };
                            break;
                        case MSG_AUTH_TOKEN:
                            {
                                authenticated = false;
                                string token = Encoding.UTF8.GetString(msg, 0, msg.Length);
                                var payload = AuthToken.ValidateToken(token);
                                var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                                var now = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);

                                if (payload != null && payload["type"] == AuthToken.API_ACCESS_TOKEN_TYPE_AUTH)
                                {
                                    if ((long)payload["exp"] < now)
                                    {
                                        Device dev = uw.DeviceRepository.Get(filter: d => (d.AccessTokens.RefreshToken == token)).FirstOrDefault();

                                        if (dev == null)
                                        {
                                            res = new byte[] { ERROR_INVALID_TOKEN };
                                            break;
                                        }

                                        card = dev.Owner.Account.VCard;

                                        authenticated = true;
                                        res = new byte[] { OK };
                                    }
                                    else
                                    {
                                        res = new byte[] { ERROR_EXPIRED_TOKEN };
                                    }
                                }
                                else
                                {
                                    res = new byte[] { ERROR_INVALID_TOKEN };
                                }
                            }
                            break;
                        case MSG_REFRESG_TOKEN:
                            {
                                authenticated = false;
                                string token = Encoding.UTF8.GetString(msg, 0, msg.Length);
                                var payload = AuthToken.ValidateToken(token);
                                var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                                var now = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);

                                if (payload != null && payload["type"] == AuthToken.API_ACCESS_TOKEN_TYPE_REFRESH)
                                {
                                    if ((long)payload["exp"] < now)
                                    {
                                        Device dev = uw.DeviceRepository.Get(filter: d => (d.AccessTokens.RefreshToken == token)).FirstOrDefault();

                                        if (dev == null)
                                        {
                                            res = new byte[] { ERROR_INVALID_TOKEN };
                                            break;
                                        }

                                        User user = dev.Owner;

                                        dev.AccessTokens.AuthToken = Utility.Security.AuthToken.GetAPIAuthJwt(user, dev);

                                        uw.AccessTokensRepository.Update(dev.AccessTokens);

                                        authenticated = true;
                                        byte[] tmp = Encoding.UTF8.GetBytes(dev.AccessTokens.AuthToken);

                                        res = new byte[tmp.Length + 1];
                                        res[0] = OK;
                                        System.Array.Copy(res, 1, tmp, 0, tmp.Length);
                                    }
                                    else
                                    {
                                        res = new byte[] { ERROR_EXPIRED_TOKEN };
                                    }
                                }
                                else
                                {
                                    res = new byte[] { ERROR_INVALID_TOKEN };
                                }
                            }
                            break;
                    }

                    uw.Save();

                    buffer = new ArraySegment<byte>(res);
                    
                    await socket.SendAsync(
                        buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
                }
                else
                {
                    break;
                }
                
            }

            uw.Dispose();
        }

    }
}