using System;
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
        private String productid, dateinitial;
        private VCardTokenDto dto;

        public String DateInitial
        {
            set
            {
                dateinitial = value;
            }
        }

        public VCardTokenDto VCardTokenDto
        {
            get
            {
                return dto;
            }
        }

        public GetVCTokenService(UnitOfWork UnitOfWork, Device AuthDevice)
            : base(UnitOfWork, AuthDevice) { }

        public override bool ExecuteService()
        {
            if (productid == null || productid == String.Empty)
                return false;
            if (dateinitial == null)
                return false;

            DateTime date = DateTime.ParseExact(dateinitial, "yyyy-MM-dd HH:mm",
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

                if (!tk.ApproveLoadTokenRequest(load))
                {
                    transaction.Rollback();
                    throw new InvalidLoadRequest("Initial date needs to be set for today!");
                }

                try
                {
                    u.Account.Withdraw(load.Price);
                }
                catch (VCException ex)
                {
                    transaction.Rollback();
                    throw ex;
                }


                UnitOfWork.Save(); // just to guarantee that loadToken has an ID.

                if (!tk.LoadToken(load))
                {
                    transaction.Rollback();
                    throw new InvalidLoadRequest("The token loading request failed!");
                }

                transaction.Commit();
            }

            VCardEncryptor secure = new VCardEncryptor(u);

            token = UnitOfWork.VCardTokenRepository.GetByID(token.Id);

            String secureToken = secure.RijndaelEncrypt(token);

            dto = new VCardTokenDto(secureToken);

            return true;
        }
    }
}
