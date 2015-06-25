using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models.Dto;
using VCBackend.Repositories;

namespace VCBackend.Services
{
    public class GetAccountBalanceService : IService
    {
        public BalanceDto Balance { get; private set; }

        public GetAccountBalanceService(UnitOfWork uw, Device device)
            : base(uw, device) {}

        protected override bool ExecuteService()
        {
            Balance = new BalanceDto();
            Balance.Serialize(AuthDevice.Owner.Account);
            return true;
        }
    }
}