using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;
using VCBackend.Repositories;
using VCBackend.Utility.Security;
using System.Text.RegularExpressions;

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

        private void AddDeviceToUser(User user, String Name = "Default", String DeviceId = null)
        {
            Device device;

            if (DeviceId == null)
                device = new Device();
            else device = new Device(DeviceId);

            device.Name = Name;
            //Generates a token that should be used next to register a device.
            device.Token = AuthToken.GenerateToken(user, device);
            user.Devices.Add(device);
        }

        //http://stackoverflow.com/questions/5859632/regular-expression-for-password-validation
        static bool ValidatePassword(string password)
        {
            const int MIN_LENGTH = 6;
            const int MAX_LENGTH = 40;

            if (password == null) throw new ArgumentNullException();

            bool meetsLengthRequirements = password.Length >= MIN_LENGTH && password.Length <= MAX_LENGTH;
            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasDecimalDigit = false;

            if (meetsLengthRequirements)
            {
                foreach (char c in password)
                {
                    if (char.IsUpper(c)) hasUpperCaseLetter = true;
                    else if (char.IsLower(c)) hasLowerCaseLetter = true;
                    else if (char.IsDigit(c)) hasDecimalDigit = true;
                }
            }

            return meetsLengthRequirements && hasUpperCaseLetter && hasLowerCaseLetter && hasDecimalDigit;
        }

        /*
         * This method validates if the attributes are wel formed.
         * It will not look for those that are @null!!
         */
        private bool ValidateUserData(String Name, String Email, String Password)
        {
            Regex nameRgx = new Regex(@"[A-zÀ-ú-]{2}[A-zÀ-ú-\s]*");
            Regex emailRgx = new Regex(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
            bool validName = (Name != null) ? nameRgx.IsMatch(Name) : true;
            bool validEmail = Email != null && emailRgx.IsMatch(Email);
            return validName && validEmail && ValidatePassword(Password);
        }

        private bool ExistUserWithEmail(String Email)
        {
            IEnumerable<User> users = rep.List;
            var query =
                from user in users
                where user.Email == Email
                select user;

            if (query.Count() > 0)
                return true;
            else return false;
        }

        public User CreateUser(String Name, String Email, String Password)
        {
            /*
             * Validate if the new user details are well formed
             */
            if (!ValidateUserData(Name, Email, Password)) throw new MalformedUserDetailsException("One or more of the details passed have an incorrect format.");
            /*
             * First check wether exists on the Users DB a user with any of the given data.
             */
            if (ExistUserWithEmail(Email))
                throw new UserAlreadyExistException("Email already in use.");

            /*
             * Then if the user doesn't exist, we can create the user
             */
            /*
             * A DefaultDevice is used to login with browsers.
             * They will not have full access to the API
             */
            User newUser = new User(Name, Email, Pbkdf2.DeriveKey(Password));

            AddDeviceToUser(newUser);
            
            rep.Add(newUser);

            return newUser;
        }

        public void UpdateUser(User User, String Name, String Email, String Password)
        {
            /*
             * Validate if the new user details are well formed
             */
            if (!ValidateUserData(Name, Email, Password)) throw new MalformedUserDetailsException("One or more of the details passed have an incorrect format.");

            if (Name != null)
                User.Name = Name;
            if (Email != null)
            {
                if (ExistUserWithEmail(Email))
                    throw new UserAlreadyExistException("Email already in use.");
                User.Email = Email;
            }
            if (Password != null)
                User.Password = Pbkdf2.DeriveKey(Password);

            rep.Update(User);
        }

        public String UserLogin(String Email, String Password, String DeviceId = null)
        {
            String token = null;
            IEnumerable<User> users = rep.List;
            var u = from user in users
                    where user.Email == Email
                    && user.Password == Pbkdf2.DeriveKey(Password)
                    select user;

            if (u.Count() == 1)
            {
                User user = u.First<User>();
                ICollection<Device> devices = user.Devices;

                if (DeviceId != null)
                {
                    var q = (from dev in devices
                             where dev.DeviceId == DeviceId
                             select dev).First();
                    if (q != null)
                    {
                        q.Token = AuthToken.GenerateToken(user, q);
                        token = q.Token;
                    }
                    else throw new InvalidCredentialsException("Invalid DeviceId");
                    
                }
                else
                {
                    var q = (from dev in devices
                             where dev.Type == DeviceType.DEFAULT_DEVICE
                             select dev).First();
                    if (q != null)
                    {
                        q.Token = AuthToken.GenerateToken(user, q);
                        token = q.Token;
                    }
                    else throw new InvalidCredentialsException("Internal error, no devices to authenticate");
                }

                rep.Update(user);

            }
            else throw new InvalidCredentialsException();

            return token;

        }
    }
}