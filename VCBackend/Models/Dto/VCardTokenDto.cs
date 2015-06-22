using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class VCardTokenDto : IDto<VCardToken>
    {
        public PBKeyDto PublicPBKey { get; set; }
        public String IV { get; set; }
        public String EncryptedToken { get; set; }
        public String HMAC256 { get; set; }

        public long DateInitial { get; set; }
        public long DateFinal { get; set; }
        public int Amount { get; set; }

        public VCardTokenDto(PBKeyDto PBKey, String IV, String EncryptedToken, String HMAC)
        {
            this.PublicPBKey = PBKey;
            this.EncryptedToken = EncryptedToken;
            this.IV = IV;
            this.HMAC256 = HMAC;
        }

        public void Serialize(VCardToken entity)
        {
            DateInitial = (long)entity.DateInitial.ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000; //Millis to Seconds
            DateFinal = (long)entity.DateFinal.ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000; //Millis to Seconds
            Amount = entity.Amount.HasValue? (int)entity.Amount.Value * 100 : 0;
        }
    }
}