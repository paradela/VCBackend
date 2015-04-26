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
        private User user;
        private UserDto dto;

        public User User
        {
            set
            {
                user = value;
            }
        }

        public UserDto UserDto
        {
            get
            {
                return UserDto;
            }
        }

        public UserUpdateService(UnitOfWork UnitOfWork)
            : base(UnitOfWork) { }

        public void doExecute()
        {
            if (user == null)
                throw new VCException("Invalid User");
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
        }
    }
}