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
        private UserDto dto;

        public UserDto UserDto
        {
            get
            {
                return UserDto;
            }
        }

        public UserUpdateService(UnitOfWork UnitOfWork, Device AuthDevice)
            : base(UnitOfWork, AuthDevice) { }

        public override bool Execute()
        {
            if (name == null && email == null && password == null)
                return false;

            User user = AuthDevice.Owner;
            /*
             * Validate if the new user details are well formed
             */
            if (!ValidateUserData(name, email, password)) throw new InvalidDataFormat("One or more of the details passed have an incorrect format.");

            if (name != null)
                user.Name = name;
            if (email != null)
            {
                if (ExistUserWithEmail(email))
                    throw new EmailAlreadyRegistered("Email already in use.");
                user.Email = email;
            }
            if (password != null)
                user.Password = Pbkdf2.DeriveKey(password);

            UnitOfWork.UserRepository.Update(user);
            UnitOfWork.Save();

            this.dto = new UserDto();
            this.dto.Serialize(user);

            return true;
        }
    }
}