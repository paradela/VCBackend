using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class BalanceDto : IDto<Account>
    {
        public Double AccountBalance { get; set; }

        public void Serialize(Account entity)
        {
            AccountBalance = entity.Balance;
        }
    }
}