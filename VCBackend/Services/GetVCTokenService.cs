﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Repositories;
using VCBackend.Exceptions;
using VCBackend.Models.Dto;
using VCBackend.Utility.Security;
using VCBackend.ExternalServices.Ticketing;

namespace VCBackend.Services
{
    public class GetVCTokenService : IService
    {
        public String DateInitial { private get; set; }

        public VCardTokenDto VCardTokenDto { get; private set; }

        public GetVCTokenService(UnitOfWork UnitOfWork, Device AuthDevice)
            : base(UnitOfWork, AuthDevice) { }

        public override bool ExecuteService()
        {
            if (DateInitial == null)
                return false;

            DateTime date = DateTime.ParseExact(DateInitial, "yyyy-MM-dd HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture);

            User u = AuthDevice.Owner;

            VCardToken token;

            using (var transaction = UnitOfWork.TransactionBegin())
            {
                VCard card = u.Account.VCard;
                Card4BTicketingKernelProxy tk = new Card4BTicketingKernelProxy();

                token = card.CreateVCardToken(u.Account);

                var load = new LoadToken(token);

                load.AccountId = u.Account.Id;
                load.DateInitial = date;

                try
                {
                    if (!tk.ApproveLoadTokenRequest(load))
                    {
                        transaction.Rollback();
                        throw new InvalidLoadRequest("Request to load no valid.");
                    }

                    u.Account.Withdraw(load.Price);

                    UnitOfWork.Save(); // just to guarantee that loadToken has an ID.

                    if (!tk.LoadToken(load))
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
            }

            VCardEncryptor secure = new VCardEncryptor(u);

            token = UnitOfWork.VCardTokenRepository.GetByID(token.Id);

            String secureToken = secure.RijndaelEncrypt(token);

            VCardTokenDto = new VCardTokenDto(secureToken);
            VCardTokenDto.Serialize(token);

            return true;
        }
    }
}