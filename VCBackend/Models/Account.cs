using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend
{
    public partial class Account : IEntity
    {
        private Account(Double Balance)
        {
            this.Balance = Balance;
            this.IsOnline = true;
        }

        public static Account CreateOnlineAccoun()
        {
            return new Account();
        }
    }
}