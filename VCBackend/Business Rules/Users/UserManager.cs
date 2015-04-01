using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;
using VCBackend.Repositories;
using VCBackend.Utility.Security;

namespace VCBackend.Business_Rules.Users
{
    public class UserManager
    {
        private static UserManager userManager = null;

        private IRepository<User> rep;

        private UserManager() {
            rep = new UserRepository(); 
        }

        public static UserManager getUserManagerSingleton()
        {
            if (userManager == null)
                userManager = new UserManager();

            return userManager;
        }

        private bool ValidateUserData(String Name, String Email, String Password, String Phone, InvoiceHeader InvoiceData)
        {
            //TODO Validate wether the fields are well formed.
            return true;
        }

        public User CreateUser(String Name, String Email, String Password, String Phone, bool AllowMarketing, InvoiceHeader InvoiceData)
        {
            IEnumerable<User> users = rep.List;
            /*
             * Validate if the new user details are well formed
             */
            if (!ValidateUserData(Name, Email, Password, Phone, InvoiceData)) throw new MalformedUserDetails("One or more of the details passed have an incorrect format.");
            /*
             * First check wether exists on the Users DB a user with any of the given data.
             */
            var query =
                from user in users
                where user.Email == Email
                || user.Phone == Phone
                || user.InvoiceData.NIF == InvoiceData.NIF
                select user;

            if (query.Count() != 0) throw new UserAlreadyExistException("There's already a user registered with the given: Email, Phone number or NIF.");

            /*
             * Then if the user doesn't exist, we can create the user
             */
            //TODO call the "Portal Viva" create user
            String uid = "123"; //The user id returned by PV
            /*
             * A PartialAccessDevice is used to login with browsers.
             * They will not have full access to the API
             */
            Device device = new Device();
            device.Name = "Default";
            User newUser = new User(uid, Name, Email, Pbkdf2.DeriveKey(Password), Phone, AllowMarketing, InvoiceData, device);

            //Generates a token that should be used next to register a device.
            device.Token = AuthToken.GenerateToken(newUser, device);

            rep.Add(newUser);

            return newUser;
        }
    }
}