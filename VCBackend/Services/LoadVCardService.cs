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

        public override bool ExecuteService()
        {
            User u = AuthDevice.Owner;

            using (var transaction = UnitOfWork.TransactionBegin())
            {
                VCard card = u.Account.VCard;
                Card4BTicketingKernelProxy tk = new Card4BTicketingKernelProxy();
                var load = new LoadRequest(card);
                load.Price = Amount;
                card.LoadRequest.Add(load);

                try
                {
                    if (!tk.ApproveLoadCardRequest(load))
                    {
                        transaction.Rollback();
                        throw new InvalidLoadRequest("Request to load no valid.");
                    }

                    u.Account.Withdraw(load.Price);

                    UnitOfWork.Save(); // just to guarantee that loadToken has an ID.

                    if (!tk.LoadCard(load))
                    {
                        transaction.Rollback();
                        throw new InvalidLoadRequest(String.Format("The token loading request failed with result: {0}", load.LoadResult));
                    }
                }
                catch (VCException ex)
                {
                    transaction.Rollback();
                    throw ex;
                }

                transaction.Commit();

                CardBalanceDto = new CardBalanceDto();
                CardBalanceDto.Serialize(load);
            }

            return true;
        }
    }
}