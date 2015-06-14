using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class AccessTokensDto : IDto<AccessTokens>
    {
        public String AuthToken { get; private set; }
        public String RefreshToken { get; private set; }

        public AccessTokensDto() { }

        public void Serialize(AccessTokens entity)
        {
            AuthToken = entity.AuthToken;
            RefreshToken = entity.RefreshToken;
        }
    }
}