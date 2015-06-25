using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models.Dto;
using VCBackend.ExternalServices.Ticketing;
using VCBackend.Repositories;

namespace VCBackend.Services
{
    public class ReadVCardBalanceService : IService
    {
        public CardBalanceDto BalanceDto { get; private set; }

        public ReadVCardBalanceService(UnitOfWork uw, Device dev) : base(uw, dev) { }

        protected override bool ExecuteService()
        {
            VCard card = AuthDevice.Owner.Account.VCard;
            Card4BTicketingKernelProxy tk = new Card4BTicketingKernelProxy();
            var balance = tk.ReadCardBalance(card);
            if (balance >= 0)
            {
                BalanceDto = new CardBalanceDto();
                BalanceDto.Balance = balance;
                return true;
            }
            else return false;
        }
    }
}