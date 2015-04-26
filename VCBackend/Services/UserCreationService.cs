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
        private TokenDto token;

        public TokenDto Token
        {
            get
            {
                return token;
            }
        }

        public UserCreationService(UnitOfWork UnitOfWork)
            : base(UnitOfWork) { }

        public void doExecute()
        {
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

            String token = device.Token;

            Account account = newUser.CreateAccount();

            newUser.Account = account;

            UnitOfWork.UserRepository.Update(newUser);
            UnitOfWork.Save();

            account.InitializeAccount();

            this.token = new TokenDto(token);
        }
    }
}