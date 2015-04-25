using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Business_Rules.Exceptions;
using System.Net.Http;
using System.Net;

namespace VCBackend.Business_Rules.Errors
{
    public class ErrorResponse : HttpResponseMessage
    {
        public ErrorResponse(VCException ex)
            : base(HttpStatusCode.Forbidden)
        {
            Content = new JsonErrorContent(ex.Error, ex.Description);
        }

        public ErrorResponse(Exception ex)
            : base(HttpStatusCode.Forbidden)
        {
            Content = new JsonErrorContent();
        }
    }
}