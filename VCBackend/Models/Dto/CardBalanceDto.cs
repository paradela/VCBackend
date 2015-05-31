using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class CardBalanceDto : IDto<LoadCard>
    {
        public Double TotalBalance { get; set; }
        public Double LoadAmmount { get; set; }



        public void Serialize(LoadCard entity)
        {
            TotalBalance = entity.ResultantBalance.HasValue ? entity.ResultantBalance.Value : entity.Price;
            LoadAmmount = entity.Price;
        }
    }
}