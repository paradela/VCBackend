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
        private TokenDto dto;

        public TokenDto TokenDto
        {
            get
            {
                return dto;
            }
        }

        public UserLoginService(UnitOfWork UnitOfWork)
            : base(UnitOfWork) { }

        public override bool ExecuteService()
        {
            String token = null;

            if (email == null || password == null)
                return false;

            String HashedPwd = Pbkdf2.DeriveKey(password);

            if (!ValidateUserData(Email: email))
                throw new InvalidDataFormat("The user email has an invalid format.");
            if(!ValidateDeviceData(DevId: deviceid))
                throw new InvalidDataFormat("The device identifier has an invalid format.");

            IEnumerable<User> users = UnitOfWork.UserRepository.Get(filter: q => (q.Email == email && q.Password == HashedPwd));

            if (users.Count() == 1)
            {
                User user = users.First<User>();
                ICollection<Device> devices = user.Devices;

                if (deviceid != null)
                {
                    var q = UnitOfWork.DeviceRepository.Get(filter: d => (d.Owner.Id == user.Id && d.DeviceId == deviceid)).FirstOrDefault();
                    if (q != null)
                    {
                        q.Token = AuthToken.GetAPIAccessJwt(user, q);
                        token = q.Token;
                    }
                    else throw new DeviceNotFound("Invalid Device identifier!");

                }
                else
                {
                    var q = UnitOfWork.DeviceRepository.Get(filter: d => (d.Owner.Id == user.Id && d.DeviceId == null)).FirstOrDefault();
                    if (q != null)
                    {
                        q.Token = AuthToken.GetAPIAccessJwt(user, q);
                        token = q.Token;
                    }
                    else throw new DeviceNotFound("Internal error, no devices to authenticate");
                }

                dto = new TokenDto(token);
                
                UnitOfWork.UserRepository.Update(user);
                UnitOfWork.Save();
                return true;
            }
            else throw new InvalidCredentials("The email and/or password are not correct.");
        }
    }
}
