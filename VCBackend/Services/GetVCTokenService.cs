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
        public String DateInitial { private get; set; }

        public VCardTokenDto VCardTokenDto { get; private set; }

        public GetVCTokenService(UnitOfWork UnitOfWork, Device AuthDevice)
            : base(UnitOfWork, AuthDevice) { }

        protected override bool ExecuteService()
        {
            if (DateInitial == null)
                return false;

            //DateTime date = DateTime.ParseExact(DateInitial, "yyyy-MM-dd HH:mm",
              //                         System.Globalization.CultureInfo.InvariantCulture);
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddSeconds(long.Parse(DateInitial)).ToLocalTime();

            User u = AuthDevice.Owner;

            VCardToken token;
            VCard card = u.Account.VCard;
            Card4BTicketingKernelProxy tk = new Card4BTicketingKernelProxy();

            token = card.CreateVCardToken(u.Account);

            token.DateFinal = date;
            token.DateInitial = date;

            var load = new LoadRequest(token);
            load.DateInitial = date;
            token.LoadRequest = load;

            if (!tk.ApproveLoadTokenRequest(load))
                throw new InvalidLoadRequest("Request to load no valid.");

            u.Account.Withdraw(load.Price);

            UnitOfWork.Save(); // just to guarantee that loadToken has an ID.

            if (!tk.LoadToken(load))
                throw new InvalidLoadRequest(String.Format("The token loading request failed with result: {0}", load.LoadResult));

            UnitOfWork.Save();

            token = UnitOfWork.VCardTokenRepository.GetByID(token.Id);

            VCardTokenDto = u.PBKey.RijndaelEncrypt(token);

            return true;
        }
    }
}
