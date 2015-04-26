using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class VCardTokenDto : IDto<VCardToken>
    {
        String EncryptedToken { get; set; }
        String Data { get; set; }

        public VCardTokenDto() { }


        public void Serialize(VCardToken entity)
        {
            EncryptedToken = entity.Data;
        }
    }
}