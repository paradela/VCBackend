using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Models;
using VCBackend.Models.Dto;
using VCBackend.Business_Rules.Users;

namespace VCBackend.Business_Rules
{
    
    /*
     * This class implements the façade design pattern. http://en.wikipedia.org/wiki/Facade_pattern
     * This class will have a public interface which will be used by the Controllers layer.
     */
    public class BRulesApi
    {
        /********************************
         * User Creation and Update
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
        public static String CreateUser (String Name, String Email, String Password) 
        {
            UserManager um = UserManager.getUserManagerSingleton();

            String token = um.CreateUser(Name, Email, Password);

            //returns a token for authentication!
            //It returns the First device, because the account was just created and it only has one device that is the default one!
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
        public static void UpdateUser (User User, String Name, String Email, String Password)
        {
            UserManager um = UserManager.getUserManagerSingleton();

            um.UpdateUser(User, Name, Email, Password);
        }

        /// <summary>
        /// Returns a authenticated user info.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <returns>A Dto with the user info.</returns>
        public static UserDto GetUser(User user)
        {
            UserManager um = UserManager.getUserManagerSingleton();

            return um.GetUser(user);
        }

        /********************************
         * User Login
         ********************************/
        /// <summary>
        /// Authenticates a user with the given email and password. The device id is optional, although
        /// if it is not passed, the given token doesn't have access to the whole API.
        /// </summary>
        /// <param name="Email">The user email.</param>
        /// <param name="Password">The user password.</param>
        /// <param name="DeviceId">The device id.</param>
        /// <returns>JWT Authentication token for the default device, or for the device with the given Id.</returns>
        /// <exception cref="InvalidCredentialsException"></exception>
        public static String Login (String Username, String Password, String DeviceId)
        {
            UserManager um = UserManager.getUserManagerSingleton();
            return um.UserLogin(Username, Password, DeviceId);
        }

        /********************************
         * Device Management
         ********************************/
        /// <summary>
        /// This method creates a new mobile device for access the API. This device will allow the 
        /// access of the whole API.
        /// </summary>
        /// <param name="User">The authenticated user.</param>
        /// <param name="DevName">The device name. Just used for identification.</param>
        /// <param name="DevId">The device id.</param>
        /// <returns>JWT Authentication token for the created device.</returns>
        /// <exception cref="ManagingDeviceException"></exception>
        public static String AddDevice(User User, String DeviceName, String DeviceId)
        {
            UserManager um = UserManager.getUserManagerSingleton();
            return um.AddDeviceToUser(User, DeviceName, DeviceId);
        }

        /// <summary>
        /// This method removes a device from the User's devices. After the removal, the device 
        /// won't be able to access the private API methods.
        /// </summary>
        /// <param name="User">The authenticated user.</param>
        /// <param name="DevId">The device Id.</param>
        /// <exception cref="ManagingDeviceException"></exception>
        public static void RemoveDevice(User User, String DeviceId)
        {
            UserManager um = UserManager.getUserManagerSingleton();
            um.RemoveDeviceFromUser(User, DeviceId);
        }

        /// <summary>
        /// List all the devices that the user has registered.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <returns>A list of devices.</returns>
        public static ICollection<DeviceDto> GetAllDevices(User User)
        {
            UserManager um = UserManager.getUserManagerSingleton();
            ICollection<Device> devices = um.GetUserDevices(User);
            ICollection<DeviceDto> devDto = new List<DeviceDto>();
            foreach (Device d in devices)
            {
                DeviceDto dto = new DeviceDto();
                dto.Serialize(d);
                devDto.Add(dto);
            }
            return devDto;
        }
    }
}
