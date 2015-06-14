using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net;
using VCBackend.Exceptions;

namespace VCBackend.Errors
{
    public class BadTokenResponse : HttpResponseMessage
    {
         public BadTokenResponse(VCException ex)
            : base(HttpStatusCode.Unauthorized)
        {
            Content = new JsonErrorContent(ex.Error, ex.Description);
        }

         public BadTokenResponse(Exception ex)
            : base(HttpStatusCode.Unauthorized)
        {
            Content = new JsonErrorContent();
        }
    }
}