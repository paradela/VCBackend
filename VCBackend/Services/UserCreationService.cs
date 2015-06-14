using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;
using VCBackend.Exceptions;
using VCBackend.Utility.Security;
using VCBackend.Models.Dto;

namespace VCBackend.Services
{
    public class UserCreationService : IUserService
    {
        public AccessTokensDto AccessToken
        {
            get;
            private set;
        }

        public UserCreationService(UnitOfWork UnitOfWork)
            : base(UnitOfWork) { }

        public override bool ExecuteService()
        {
            if (name == null || email == null || password == null)
                return false;

            if (!ValidateUserData(name, email, password)) 
                throw new InvalidDataFormat("One or more of the details passed have an incorrect format.");

            // First check wether exists on the Users DB a user with any of the given data.
            if (ExistUserWithEmail(email))
                throw new EmailAlreadyRegistered("Given email address is already in use.");


            // Then if the user doesn't exist, we can create the user
            // A DefaultDevice is used to login with browsers.
            // They will not have full access to the API
            User newUser = new User(name, email, Pbkdf2.DeriveKey(password));

            //UnitOfWork.UserRepository.Add(newUser);
            //UnitOfWork.Save();

            Device device = newUser.CreateDevice();

            if (DeviceId != null &&
                ValidateDeviceData(null, DeviceId))
            {
                device = newUser.CreateDevice(DeviceId);
            }

            Account account = newUser.CreateAccount();

            UnitOfWork.UserRepository.Add(newUser);
            UnitOfWork.Save();

            newUser = UnitOfWork.UserRepository.Get(filter: q => (q.Email == newUser.Email)).FirstOrDefault();

            device = UnitOfWork.DeviceRepository.Get(filter: q => (q.Owner.Id == newUser.Id)).FirstOrDefault();

            if (newUser != null && device != null)
            {
                device.AccessTokens = new AccessTokens();
                AccessToken = new AccessTokensDto();
                device.AccessTokens.AuthToken = AuthToken.GetAPIAuthJwt(newUser, device);
                device.AccessTokens.RefreshToken = AuthToken.GetAPIRefreshJwt(newUser, device);
                AccessToken.Serialize(device.AccessTokens);
            }
            else
            {
                if(newUser != null)
                    UnitOfWork.UserRepository.Delete(newUser.Id);
                if (device != null)
                    UnitOfWork.DeviceRepository.Delete(device.Id);
                UnitOfWork.Save();
                return false;
            }

            account.InitializeAccount();

            UnitOfWork.Save();

            this.AccessToken = new AccessTokensDto();
            this.AccessToken.Serialize(device.AccessTokens);

            return true;
        }
    }
}