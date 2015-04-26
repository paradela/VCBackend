using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Exceptions;
using System.Net.Http;
using System.Net;

namespace VCBackend.Errors
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