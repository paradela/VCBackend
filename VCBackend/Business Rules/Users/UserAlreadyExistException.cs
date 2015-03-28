using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Business_Rules.Users
{
    public class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException()
        {
        }

        public UserAlreadyExistException(string message)
        : base(message)
        {
        }
    }
}