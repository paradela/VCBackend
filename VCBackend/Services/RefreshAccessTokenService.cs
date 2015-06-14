using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models.Dto;
using VCBackend.Repositories;
using VCBackend.Exceptions;
using VCBackend.Utility.Security;

namespace VCBackend.Services
{
    public class RefreshAuthTokenService : IService
    {
        public String RefreshToken { private get; set; }
        public AuthTokenDto AuthToken { get; private set; }

        public RefreshAuthTokenService(UnitOfWork UnitofWork)
            : base(UnitofWork)
        {}

        protected override bool ExecuteService()
        {
            if (RefreshToken == null || RefreshToken == String.Empty)
                throw new InvalidRefreshToken("Refresh token is empty.");

            var payload = Utility.Security.AuthToken.ValidateToken(RefreshToken);

            if(payload == null)
                throw new InvalidRefreshToken("Refresh token is not valid.");

            if (payload.ContainsKey("type"))
            {
                if ((string)payload["type"] == Utility.Security.AuthToken.API_ACCESS_TOKEN_TYPE_REFRESH)
                {
                    Device dev = UnitOfWork.DeviceRepository.Get(filter: d => (d.AccessTokens.RefreshToken == RefreshToken)).FirstOrDefault();

                    if(dev == null) throw new InvalidRefreshToken("Refresh token is not valid.");

                    User user = dev.Owner;

                    dev.AccessTokens.AuthToken = Utility.Security.AuthToken.GetAPIAuthJwt(user, dev);

                    UnitOfWork.AccessTokensRepository.Update(dev.AccessTokens);
                    UnitOfWork.Save();

                    AuthToken = new AuthTokenDto();
                    AuthToken.Serialize(dev.AccessTokens);

                    return true;
                }
            }

            return false;
        }
    }
}