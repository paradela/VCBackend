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

        public Double AddBalance(String NewAmount)
        {
            Double amount = 0.0;

            String str = NewAmount.Replace(",", "");
            str = str.Replace(".", ",");

            amount = Double.Parse(str);

            this.Balance += amount;
            return this.Balance;
        }

    }
}