using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class CardBalanceDto : IDto<LoadRequest>
    {
        public Double TotalBalance { get; set; }
        public Double LoadAmmount { get; set; }



        public void Serialize(LoadRequest entity)
        {
            TotalBalance = entity.ResultantBalance;
            LoadAmmount = entity.Price;
        }
    }
}