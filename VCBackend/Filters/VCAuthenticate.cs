using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Filters; 
using System.Security.Claims;
using System.Security.Principal;
using VCBackend.Utility.Security;
using System.Web.Http.Results;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Threading.Tasks;
using VCBackend.Models;
using VCBackend.Repositories;
using System.Web.Http.Controllers;
using VCBackend.Exceptions;
using VCBackend.Errors;

namespace VCBackend.Filters
{
    public class VCAuthenticate : Attribute, IAuthenticationFilter 
    {
        public static String AUTH_DEVICE = "AuthDevice";

        public static int GetAuthenticatedDevice(HttpActionContext context)
        {
            return (int)context.Request.Properties[VCAuthenticate.AUTH_DEVICE];
        }

        public System.Threading.Tasks.Task AuthenticateAsync(HttpAuthenticationContext context, System.Threading.CancellationToken cancellationToken)
        {
            UnitOfWork uw = new UnitOfWork();
            try
            {
                //1. Get query key/value map to get the authentication data
                var query = context.ActionContext.Request.RequestUri.Query;
                var queryStringCollection = System.Web.HttpUtility.ParseQueryString(query);

                var token = queryStringCollection["t"];


                //2. If the token is not available, nothing to do
                if (token == null || token == String.Empty)
                    throw new HttpResponseException(new BadTokenResponse(new InvalidAuthToken("Missing authentication token.")));

                var payload = AuthToken.ValidateToken(token);
                var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var now = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);
                //3. Validate the token
                if (payload == null || (long)payload["exp"] >= now)
                    throw new HttpResponseException(new BadTokenResponse(new InvalidAuthToken("The supplied token is not valid.")));
                string type = (string)payload["type"];

                if(type != AuthToken.API_ACCESS_TOKEN_TYPE_AUTH)
                    throw new HttpResponseException(new BadTokenResponse(new InvalidAuthToken("The supplied token is not a valid auth token")));

                var uid = payload["user_id"];
                var did = payload["device_id"];

                Device dev = uw.DeviceRepository.GetByID((int)did);

                if (dev == null || dev.Owner.Id != (int)uid || dev.AccessTokens.AuthToken != token)
                    throw new HttpResponseException(new BadTokenResponse(new InvalidAuthToken("The supplied token is no longer valid.")));

                context.Request.Properties.Add(AUTH_DEVICE, did);
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw new HttpResponseException(new ErrorResponse(new Exception()));
            }
            finally
            {
                uw.Dispose();
            }

            return Task.FromResult(0);
        }

        public System.Threading.Tasks.Task ChallengeAsync(HttpAuthenticationChallengeContext context, System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public bool AllowMultiple
        {
            get { return false; }
        }
    }
}