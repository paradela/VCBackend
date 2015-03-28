using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Business_Rules.Users
{
    public class MalformedUserDetails : Exception
    {
        public MalformedUserDetails() {}
        public MalformedUserDetails(string message) : base(message) { }
    }
}