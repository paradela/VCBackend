using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;
using VCBackend.Models.Dto;
using VCBackend.Repositories;
using VCBackend.Utility.Security;
using System.Text.RegularExpressions;

using VCBackend.Business_Rules.Devices;
using VCBackend.Business_Rules.Accounts;

namespace VCBackend.Business_Rules.Users
{
    public class UserManager
    {
        private static UserManager userManager = null;

        private IRepository<User> rep;

        private UserManager() 
        {
            rep = UserRepository.getRepositorySingleton();
        }
        /// <summary>
        /// A static method for getting a unique instance of UserManager.
        /// </summary>
        /// <returns>UserManager singleton.</returns>
        public static UserManager getUserManagerSingleton()
        {
            if (userManager == null)
                userManager = new UserManager();

            return userManager;
        }

        
        

        //http://stackoverflow.com/questions/5859632/regular-expression-for-password-validation
        private bool ValidatePassword(string password)
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
            var list =
                (from user in users
                where user.Email == Email
                select user).ToList();

            if (list.Count() > 0)
                return true;
            else return false;
        }

        /********************************
        * User & Device management
        ********************************/
        /// <summary>
        /// Generates a new user in the system. A default device to access the rest of the API will be generated.
        /// </summary>
        /// <param name="Name">The new user full name.</param>
        /// <param name="Email">The new user email. This will be used for user login.</param>
        /// <param name="Password">The new user password.</param>
        /// <returns>JWT Authentication token for the created users.</returns>
        /// <exception cref="MalformedUserDetailsException">One or more of the details passed have an incorrect format.</exception>
        /// <exception cref="UserAlreadyExistException">Email already in use.</exception>
        public String CreateUser(String Name, String Email, String Password)
        {
            DeviceManager deviceManager = DeviceManager.getManagerSingleton();
            AccountManager accountManager = AccountManager.getManagerSingleton();

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

            rep.Add(newUser);

            String token = deviceManager.CreateDeviceToUser(newUser).Token;

            Account account = accountManager.CreateAccount();

            newUser.Account = account;

            rep.Update(newUser);

            accountManager.InitializeAccount(account.Id);

            //newUser.Account = account;

            //rep.Update(newUser);

            return token;
        }
        /// <summary>
        /// Updates the user data, like the name, email or password.
        /// </summary>
        /// <param name="User">The authenticated user.</param>
        /// <param name="Name">The new name.</param>
        /// <param name="Email">The new email.</param>
        /// <param name="Password">The new password.</param>
        /// <exception cref="MalformedUserDetailsException">One or more of the details passed have an incorrect format.</exception>
        /// <exception cref="UserAlreadyExistException">Email already in use.</exception>
        public UserDto UpdateUser(User User, String Name, String Email, String Password)
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

            UserDto dto = new UserDto();
            dto.Serialize(User);
            return dto;
        }

        /// <summary>
        /// Returns a authenticated user info.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <returns>A Dto with the user info.</returns>
        public UserDto GetUser(User User)
        {
            UserDto dto = new UserDto();
            dto.Serialize(User);
            return dto;
        }

        /// <summary>
        /// Authenticates a user with the given email and password. The device id is optional, although
        /// if it is not passed, the given token doesn't have access to the whole API.
        /// </summary>
        /// <param name="Email">The user email.</param>
        /// <param name="Password">The user password.</param>
        /// <param name="DeviceId">The device id.</param>
        /// <returns>JWT Authentication token for the default device, or for the device with the given Id.</returns>
        /// <exception cref="InvalidCredentialsException"></exception>
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
                             select dev).FirstOrDefault();
                    if (q != null)
                    {
                        q.Token = AuthToken.GetAPIAccessToken(user, q);
                        token = q.Token;
                    }
                    else throw new InvalidCredentialsException("Invalid DeviceId");
                    
                }
                else
                {
                    var q = (from dev in devices
                             where dev.Type == Device.DEFAULT_DEVICE
                             select dev).FirstOrDefault();
                    if (q != null)
                    {
                        q.Token = AuthToken.GetAPIAccessToken(user, q);
                        token = q.Token;
                    }
                    else throw new InvalidCredentialsException("Internal error, no devices to authenticate");
                }

                rep.Update(user);

            }
            else throw new InvalidCredentialsException();

            return token;

        }

        /// <summary>
        /// This method creates a new mobile device for access the API. This device will allow the 
        /// access of the whole API.
        /// </summary>
        /// <param name="User">The authenticated user.</param>
        /// <param name="DevName">The device name. Just used for identification.</param>
        /// <param name="DevId">The device id.</param>
        /// <returns>JWT Authentication token for the created device.</returns>
        /// <exception cref="ManagingDeviceException"></exception>
        public String AddDeviceToUser(User User, String DevName, String DevId)
        {
            DeviceManager deviceManager = DeviceManager.getManagerSingleton();

            if (!ValidateDeviceData(DevName, DevId))
                throw new ManagingDeviceException("Invalid device name or Id.");

            //Check if the user already have a Device with id @DevId
            var devCount = (from d in User.Devices
                           where d.DeviceId == DevId
                           select d).Count();

            if (devCount == 0)
            {
                Device dev = deviceManager.CreateDeviceToUser(User, DevName, DevId);
                rep.Update(User);
                return dev.Token;
            }
            else throw new ManagingDeviceException("Device Id already registered.");
        }

        /// <summary>
        /// This method removes a device from the User's devices. After the removal, the device 
        /// won't be able to access the private API methods.
        /// </summary>
        /// <param name="User">The authenticated user.</param>
        /// <param name="DevId">The device Id.</param>
        /// <exception cref="ManagingDeviceException"></exception>
        public void RemoveDeviceFromUser(User User, String DevId)
        {
            DeviceManager devMngr = DeviceManager.getManagerSingleton();

            if (!ValidateDeviceData(null, DevId))
                throw new ManagingDeviceException("Invalid id format");

            var dev = (from d in User.Devices
                      where d.DeviceId == DevId
                      select d).First();
            if (dev == null)
                throw new ManagingDeviceException("Device with given Id does not exist");

            devMngr.RemoveDevice(dev.Id);
            
            User.Devices.Remove(dev);
        }

        /// <summary>
        /// List all the devices that the user has registered.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <returns>A list of devices.</returns>
        public ICollection<Device> GetUserDevices(User user)
        {
            return user.Devices;
        }
    }
}