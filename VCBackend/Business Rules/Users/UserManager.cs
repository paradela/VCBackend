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
            rep = UserRepository.getRepositorySingleton();
        }

        public static UserManager getUserManagerSingleton()
        {
            if (userManager == null)
                userManager = new UserManager();

            return userManager;
        }

        /*
         * This method creates a new device with the given name and device id. 
         */
        private Device CreateDeviceToUser(User user, String Name = "Default", String DeviceId = null)
        {
            Device device;

            if (DeviceId == null)
                device = new Device();
            else device = new Device(DeviceId);

            device.Name = Name;
            //Generates a token that should be used next to register a device.
            device.Token = AuthToken.GenerateToken(user, device);
            user.AddDevice(device);
            return device;
        }

        //http://stackoverflow.com/questions/5859632/regular-expression-for-password-validation
        private static bool ValidatePassword(string password)
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
            bool validEmail = (Email != null) ? emailRgx.IsMatch(Email) : true;
            return validName && validEmail && (Password != null) ? ValidatePassword(Password) : true;
        }

        private bool ValidateDeviceData(String Name, String DevId)
        {
            Regex nameRgx = new Regex(@"[A-zÀ-ú-]{2}[A-zÀ-ú-\s]*");
            Regex idRgx = new Regex(@"[A-z1-9]{4}[A-z1-9]*");
            bool validName = (Name != null) ? nameRgx.IsMatch(Name) : true;
            bool validId = (DevId != null) ? idRgx.IsMatch(DevId) : true;
            return validName && validId;
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

        public String CreateUser(String Name, String Email, String Password)
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

            //rep.Add(newUser);

            String token = CreateDeviceToUser(newUser).Token;

            rep.Add(newUser);

            return token;
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

        /*
         * This method creates a new @MOBILE_DEVICE with name @DevName and id @DevId.
         * Then it will be added to the user @User.
         * It @returns a new authentication token to authenticate in this device.
         */
        public String AddDeviceToUser(User User, String DevName, String DevId)
        {
            if (!ValidateDeviceData(DevName, DevId))
                throw new ManagingDeviceException("Invalid device name or Id.");

            //Check if the user already have a Device with id @DevId
            var devCount = (from d in User.Devices
                           where d.DeviceId == DevId
                           select d).Count();

            if (devCount == 0)
            {
                Device dev = CreateDeviceToUser(User, DevName, DevId);
                rep.Update(User);
                return dev.Token;
            }
            else throw new ManagingDeviceException("Device Id already registered.");
        }

        public void RemoveDevice(User User, String DevId)
        {
            if (!ValidateDeviceData(null, DevId))
                throw new ManagingDeviceException("Invalid id format");

            var dev = (from d in User.Devices
                      where d.DeviceId == DevId
                      select d).First();
            if (dev == null)
                throw new ManagingDeviceException("Device with given Id does not exist");

            User.Devices.Remove(dev);
            rep.Update(User);
        }

        public ICollection<Device> GetUserDevices(User user)
        {
            return user.Devices;
        }
    }
}