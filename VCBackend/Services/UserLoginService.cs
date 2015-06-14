using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Repositories;
using VCBackend.Utility.Security;
using VCBackend.Exceptions;
using VCBackend.Models.Dto;

namespace VCBackend.Services
{
    public class UserLoginService : IUserService
    {
        public AccessTokensDto AccessTokenDto
        {
            get;
            private set;
        }

        public UserLoginService(UnitOfWork UnitOfWork)
            : base(UnitOfWork) { }

        public override bool ExecuteService()
        {
            AccessTokens token = null;

            if (email == null || password == null)
                return false;

            String HashedPwd = Pbkdf2.DeriveKey(password);

            if (!ValidateUserData(Email: email))
                throw new InvalidDataFormat("The user email has an invalid format.");
            if(!ValidateDeviceData(DevId: DeviceId))
                throw new InvalidDataFormat("The device identifier has an invalid format.");

            IEnumerable<User> users = UnitOfWork.UserRepository.Get(filter: q => (q.Email == email && q.Password == HashedPwd));

            if (users.Count() == 1)
            {
                User user = users.First<User>();
                ICollection<Device> devices = user.Devices;

                if (DeviceId != null)
                {
                    var q = UnitOfWork.DeviceRepository.Get(filter: d => (d.Owner.Id == user.Id && d.DeviceId == DeviceId)).FirstOrDefault();
                    if (q != null)
                    {
                        token = new AccessTokens();
                        token.AuthToken = AuthToken.GetAPIAuthJwt(user, q);
                        token.RefreshToken = AuthToken.GetAPIRefreshJwt(user, q);
                        q.AccessTokens = token;
                    }
                    else
                    {
                        q = user.CreateDevice(DeviceId);
                        token = new AccessTokens();
                        token.AuthToken = AuthToken.GetAPIAuthJwt(user, q);
                        token.RefreshToken = AuthToken.GetAPIRefreshJwt(user, q);
                        q.AccessTokens = token;
                    }

                }
                else
                {
                    var q = UnitOfWork.DeviceRepository.Get(filter: d => (d.Owner.Id == user.Id && d.DeviceId == null)).FirstOrDefault();
                    if (q != null)
                    {
                        token = new AccessTokens();
                        token.AuthToken = AuthToken.GetAPIAuthJwt(user, q);
                        token.RefreshToken = AuthToken.GetAPIRefreshJwt(user, q);
                        q.AccessTokens = token;
                    }
                    else throw new DeviceNotFound("Internal error, no devices to authenticate");
                }

                AccessTokenDto = new AccessTokensDto();
                AccessTokenDto.Serialize(token);
                
                UnitOfWork.UserRepository.Update(user);
                UnitOfWork.Save();
                return true;
            }
            else throw new InvalidCredentials("The email and/or password are not correct.");
        }
    }
}
