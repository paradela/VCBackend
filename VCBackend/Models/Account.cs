using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend
{
    public partial class Account
    {
        private Account(Double Balance)
        {
            this.Balance = Balance;
        }

        public static Account CreateOnlineAccoun()
        {
            return new Account();
        }

    }
}