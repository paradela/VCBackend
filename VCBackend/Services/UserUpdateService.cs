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
    public class UserUpdateService : IUserService
    {
        public UserDto UserDto
        {
            get;
            private set;
        }

        public UserUpdateService(UnitOfWork UnitOfWork, Device AuthDevice)
            : base(UnitOfWork, AuthDevice) { }

        protected override bool ExecuteService()
        {
            if (Name == null && Email == null && Password == null && Pin == null)
                return false;

            User user = AuthDevice.Owner;
            /*
             * Validate if the new user details are well formed
             */
            if (!ValidateUserData(Name, Email, Password, Pin)) throw new InvalidDataFormat("One or more of the details passed have an incorrect format.");

            if (Name != null)
                user.Name = Name;
            if (Email != null)
            {
                if (ExistUserWithEmail(Email))
                    throw new EmailAlreadyRegistered("Email already in use.");
                user.Email = Email;
            }
            if (Password != null)
                user.Password = PasswordSecurity.GetSHA512(Password);
            if (Pin != null)
                user.PBKey.Initialize(Pin);

            UnitOfWork.UserRepository.Update(user);
            UnitOfWork.Save();

            this.UserDto = new UserDto();
            this.UserDto.Serialize(user);

            return true;
        }
    }
}