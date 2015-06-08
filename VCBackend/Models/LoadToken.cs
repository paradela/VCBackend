using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend
{
    public partial class LoadToken
    {
        public LoadToken()
        {
        }
        public LoadToken(VCardToken Token)
        {
            VCardToken = Token;
        }
    }
}