using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Business_Rules.Users
{
    public class MalformedUserDetailsException : Exception
    {
        public MalformedUserDetailsException() {}
        public MalformedUserDetailsException(string message) : base(message) { }
    }
}