using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace VCBackend.Errors
{
    public class JsonErrorContent : ObjectContent<object>
    {
        public JsonErrorContent(String ErrorCode = "Unkown", String ErrorDescription = "No error description available.")
            : base(new { Error = ErrorCode, Description = ErrorDescription }, new JsonMediaTypeFormatter())
        {
        }
    }
}