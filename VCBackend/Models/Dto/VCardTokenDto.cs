using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class VCardTokenDto : IDto<VCardToken>
    {
        public String EncryptedToken { get; set; }
        public DateTime DateInitial { get; set; }
        public DateTime DateFinal { get; set; }
        public Double Ammount { get; set; }

        public VCardTokenDto(String CardData)
        {
            EncryptedToken = CardData;
        }

        public void Serialize(VCardToken entity)
        {
            DateInitial = entity.DateInitial;
            DateFinal = entity.DateFinal;
            Ammount = entity.Ammount.HasValue? entity.Ammount.Value : 0.0;
        }
    }
}