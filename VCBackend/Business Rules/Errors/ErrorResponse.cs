using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Business_Rules.Exceptions;
using System.Net.Http;
using System.Web.Http;
using System.Net;

namespace VCBackend.Business_Rules.Errors
{
    public class ErrorResponse : HttpResponseException
    {
        public ErrorResponse(VCException ex)
            : base(HttpStatusCode.Forbidden)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new JsonErrorContent(ex.Error, ex.Description)
            };
            throw new HttpResponseException(resp);
        }

        public ErrorResponse(Exception ex)
            : base(HttpStatusCode.Forbidden)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new JsonErrorContent()
            };
            throw new HttpResponseException(resp);
        }
    }
}