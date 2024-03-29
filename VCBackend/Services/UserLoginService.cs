﻿using System;
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
        public AccessTokensDto TokensDto
        {
            get;
            private set;
        }

        public UserLoginService(UnitOfWork UnitOfWork)
            : base(UnitOfWork) { }

        protected override bool ExecuteService()
        {
            AccessTokens token = null;

            if (Email == null || Password == null)
                return false;

            String HashedPwd = PasswordSecurity.GetSHA512(Password);

            if (!ValidateUserData(email: Email))
                throw new InvalidDataFormat("The user email has an invalid format.");
            if(!ValidateDeviceData(DevId: DeviceId))
                throw new InvalidDataFormat("The device identifier has an invalid format.");

            IEnumerable<User> users = UnitOfWork.UserRepository.Get(filter: q => (q.Email == Email && q.Password == HashedPwd));

            if (users.Count() == 1)
            {
                User user = users.First<User>();
                ICollection<Device> devices = user.Devices;

                if (DeviceId != null)
                {
                    var q = UnitOfWork.DeviceRepository.Get(filter: d => (d.Owner.Id == user.Id && d.DeviceId == DeviceId)).FirstOrDefault();
                    if (q != null)
                    {
                        UnitOfWork.AccessTokensRepository.Delete(q.AccessTokens);
                        token = new AccessTokens();
                        token.AuthToken = AuthToken.GetAPIAuthJwt(user, q);
                        token.RefreshToken = AuthToken.GetAPIRefreshJwt(user, q);
                        q.AccessTokens = token;
                        UnitOfWork.AccessTokensRepository.Add(token);
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
                        UnitOfWork.AccessTokensRepository.Delete(q.AccessTokens);
                        token = new AccessTokens();
                        token.AuthToken = AuthToken.GetAPIAuthJwt(user, q);
                        token.RefreshToken = AuthToken.GetAPIRefreshJwt(user, q);
                        q.AccessTokens = token;
                        UnitOfWork.AccessTokensRepository.Add(token);
                    }
                    else throw new DeviceNotFound("Internal error, no devices to authenticate");
                }

                TokensDto = new AccessTokensDto();
                TokensDto.Serialize(token);
                
                UnitOfWork.UserRepository.Update(user);
                UnitOfWork.Save();
                return true;
            }
            else throw new InvalidCredentials("The email and/or password are not correct.");
        }
    }
}
