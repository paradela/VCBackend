using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Repositories;
using VCBackend.Exceptions;
using VCBackend.Models.Dto;
using VCBackend.Utility.Security;

namespace VCBackend.Services
{
    public class CreateVCTokenService : IService
    {
        private String productid, dateinitial;
        private VCardTokenDto dto;

        public String ProductId
        {
            set
            {
                productid = value;
            }
        }

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

        public CreateVCTokenService(UnitOfWork UnitOfWork, Device AuthDevice)
            : base(UnitOfWork, AuthDevice) { }

        public override bool ExecuteService()
        {
            if (productid == null || productid == String.Empty)
                return false;
            if (dateinitial == null)
                return false;

            DateTime date = DateTime.ParseExact(dateinitial, "dd-MM-yyyy HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture);

            if(date.AddMinutes(10) < DateTime.Now) 
                throw new VCException("Invalid initial date.");
            User u = AuthDevice.Owner;

            VCardToken token;

            using (var transaction = UnitOfWork.TransactionBegin())
            {
                VCard card = u.Account.VCard;

                token = card.CreateVCardToken(u.Account);

                LoadRequest req = new LoadRequest();
                req.CardAuth = u.Account.GetAuthToLoadCard(token.Id);
                req.DateInitial = date;
                req.ProdId = productid;
                req.Quantity = 1;
                //ProdManager


            }

            VCardEncryptor secure = new VCardEncryptor(u);

            token = UnitOfWork.VCardTokenRepository.GetByID(token.Id);

            String secureToken = secure.RijndaelEncrypt(token);

            dto = new VCardTokenDto(secureToken);

            return true;
        }
    }
}
