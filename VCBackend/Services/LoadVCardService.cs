using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;
using VCBackend.Models.Dto;
using VCBackend.ExternalServices.Ticketing;
using VCBackend.Exceptions;

namespace VCBackend.Services
{
    public class LoadVCardService : IService
    {
        public double Amount { private get; set; }

        public CardBalanceDto CardBalanceDto { get; private set; }

        public LoadVCardService(UnitOfWork UnitOfWork, Device AuthDevice)
        : base(UnitOfWork, AuthDevice) { }

        protected override bool ExecuteService()
        {
            User u = AuthDevice.Owner;

            VCard card = u.Account.VCard;
            Card4BTicketingKernelProxy tk = new Card4BTicketingKernelProxy();
            var load = new LoadRequest(card);
            load.Price = Amount;
            card.LoadRequest.Add(load);

            if (!tk.ApproveLoadCardRequest(load))
                throw new InvalidLoadRequest("Request to load no valid.");

            u.Account.Withdraw(load.Price);

            UnitOfWork.Save(); // just to guarantee that loadToken has an ID.

            if (!tk.LoadCard(load))
                throw new InvalidLoadRequest(String.Format("The token loading request failed with result: {0}", load.LoadResult));

            CardBalanceDto = new CardBalanceDto();
            CardBalanceDto.Serialize(load);

            return true;
        }
    }
}