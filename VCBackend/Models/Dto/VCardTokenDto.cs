using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class VCardTokenDto
    {
        String EncryptedToken { get; set; }

        public VCardTokenDto(String CardData)
        {
            EncryptedToken = CardData;
        }
    }
}