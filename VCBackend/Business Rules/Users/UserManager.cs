using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;
using VCBackend.Models.Dto;
using VCBackend.Repositories;
using VCBackend.Utility.Security;
using VCBackend.Business_Rules.Accounts;
using VCBackend.Business_Rules.Exceptions;

namespace VCBackend.Business_Rules.Users
{
    public class UserManager : IManager
    {
        public UserManager(UnitOfWork UnitOfWork) 
            : base(UnitOfWork)
        {
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
            String HashedPwd = Pbkdf2.DeriveKey(Password);
            IEnumerable<User> users = UnitOfWork.UserRepository.Get(filter : q => (q.Email == Email && q.Password == HashedPwd));

            if (users.Count() == 1)
            {
                User user = users.First<User>();
                ICollection<Device> devices = user.Devices;

                if (DeviceId != null)
                {
                    var q = UnitOfWork.DeviceRepository.Get(filter: d => (d.Owner.Id == user.Id && d.DeviceId == DeviceId)).FirstOrDefault();
                    if (q != null)
                    {
                        q.Token = AuthToken.GetAPIAccessJwt(user, q);
                        token = q.Token;
                    }
                    else throw new DeviceNotFound("Invalid Device identifier!");
                    
                }
                else
                {
                    var q = (from dev in devices
                             where dev.IsDefault()
                             select dev).FirstOrDefault();
                    if (q != null)
                    {
                        q.Token = AuthToken.GetAPIAccessJwt(user, q);
                        token = q.Token;
                    }
                    else throw new DeviceNotFound("Internal error, no devices to authenticate");
                }

                UnitOfWork.UserRepository.Update(user);
                UnitOfWork.Save();
            }
            else throw new InvalidCredentials("The email and/or password are not correct.");

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
            DeviceManager deviceManager = new DeviceManager(UnitOfWork);

            if (!ValidateDeviceData(DevName, DevId))
                throw new DeviceNotFound(String.Format("Invalid device name: {0} or Id {1}.", DevName, DevId));

            //Check if the user already have a Device with id @DevId
            var devCount = (from d in User.Devices
                           where d.DeviceId == DevId
                           select d).Count();

            if (devCount == 0)
            {
                Device dev = deviceManager.CreateDeviceToUser(User, DevName, DevId);
                UnitOfWork.UserRepository.Update(User);
                UnitOfWork.Save();
                return dev.Token;
            }
            else throw new DeviceAlreadyRegistered(String.Format("This user already has a device with identifier: {0}", DevId));
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
            DeviceManager devMngr = new DeviceManager(UnitOfWork);

            if (!ValidateDeviceData(null, DevId))
                throw new DeviceNotFound("Invalid id format");

            var dev = (from d in User.Devices
                      where d.DeviceId == DevId
                      select d).First();
            if (dev == null)
                throw new DeviceNotFound("Device with given Id does not exist");

            devMngr.RemoveDevice(dev.Id);
            
            User.Devices.Remove(dev);

            UnitOfWork.UserRepository.Update(User);
            UnitOfWork.Save();
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