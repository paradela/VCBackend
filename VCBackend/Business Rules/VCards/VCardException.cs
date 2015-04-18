using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Business_Rules.VCards
{
    public class VCardException : Exception
    {
        public VCardException() { }
        public VCardException(string Message) : base(Message) { }
    }
}