﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Filters; 
using System.Security.Claims;
using System.Security.Principal;
using VCBackend.Utility.Security;
using System.Web.Http.Results;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VCBackend.Models;
using VCBackend.Repositories;
using System.Web.Http.Controllers;

namespace VCBackend.Filters
{
    public class VCAuthenticate : Attribute, IAuthenticationFilter 
    {
        public static String AUTH_DEVICE = "AuthDevice";

        public static Device GetAuthenticatedDevice(HttpActionContext context)
        {
            return (Device)context.Request.Properties[VCAuthenticate.AUTH_DEVICE];
        }

        public System.Threading.Tasks.Task AuthenticateAsync(HttpAuthenticationContext context, System.Threading.CancellationToken cancellationToken)
        {
            IRepository<Device> rep = DeviceRepository.getRepositorySingleton();

            //1. Get query key/value map to get the authentication data
            var query = context.ActionContext.Request.RequestUri.Query;
            var queryStringCollection = System.Web.HttpUtility.ParseQueryString(query);

            var token = queryStringCollection["t"];


            //2. If the token is not available, nothing to do
            if (token == null || token == String.Empty)
                return null;

            var payload = AuthToken.ValidateToken(token);
            //3. Validate the token
            if (payload == null)
                return null;

            var uid = payload["user_id"];
            var did = payload["device_id"];

            Device dev = rep.FindById((int)did);
            
            if (dev == null || dev.Owner.Id != (int)uid || dev.Token != token)
                return null;

            context.Request.Properties.Add(AUTH_DEVICE, dev);
            
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