using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Business_Rules.Users
{
    public class ManagingDeviceException : Exception
    {
        public ManagingDeviceException() { }
        public ManagingDeviceException(String message) : base(message) { }
    }
}