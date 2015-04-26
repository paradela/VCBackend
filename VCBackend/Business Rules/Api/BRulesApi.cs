using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Models;
using VCBackend.Models.Dto;
using VCBackend.Business_Rules.Users;
using VCBackend.Business_Rules.Accounts;
using VCBackend.Repositories;
using VCBackend.Business_Rules.Payments;

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
        public static TokenDto CreateUser (String Name, String Email, String Password) 
        {
            UnitOfWork uw = new UnitOfWork();
            TokenDto dto;

            try
            {
               
                UserManager um = new UserManager(uw);

                String token = um.CreateUser(Name, Email, Password);

                dto = new TokenDto(token);

            }
            finally
            {
                uw.Dispose();
            }

            //returns a token for authentication!
            //It returns the First device, because the account was just created and it only has one device that is the default one!
            return dto;
        }

        /// <summary>
        /// Updates the user data, like the name, email or password.
        /// </summary>
        /// <param name="AuthDevice">The authenticated device.</param>
        /// <param name="Name">The new name.</param>
        /// <param name="Email">The new email.</param>
        /// <param name="Password">The new password.</param>
        /// <returns>A UserDto with the actual user data.</returns>
        /// <exception cref="MalformedUserDetailsException">One or more of the details passed have an incorrect format.</exception>
        /// <exception cref="UserAlreadyExistException">Email already in use.</exception>
        public static UserDto UpdateUser (int AuthDevice, String Name, String Email, String Password)
        {
            UnitOfWork uw = new UnitOfWork();
            UserDto dto;

            try
            {
                UserManager um = new UserManager(uw);

                User User = uw.DeviceRepository.GetByID(AuthDevice).Owner;

                dto = um.UpdateUser(User, Name, Email, Password);
            }
            finally
            {

                uw.Dispose();
            }

            return dto;
        }

        /// <summary>
        /// Returns a authenticated user info.
        /// </summary>
        /// <param name="AuthDevice">The authenticated device id.</param>
        /// <returns>A Dto with the user info.</returns>
        public static UserDto GetUser(int AuthDevice)
        {
            UnitOfWork uw = new UnitOfWork();
            UserDto dto;

            try
            {
                UserManager um = new UserManager(uw);
                User User = uw.DeviceRepository.GetByID(AuthDevice).Owner;

                dto = um.GetUser(User);
            }
            finally
            {
                uw.Dispose();
            }

            return dto;
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
        public static TokenDto Login(String Username, String Password, String DeviceId)
        {
            UnitOfWork uw = new UnitOfWork();
            TokenDto dto;

            try
            {
                UserManager um = new UserManager(uw);

                String token = um.UserLogin(Username, Password, DeviceId);
                dto = new TokenDto(token);
            }
            finally
            {
                uw.Dispose();
            }

            return dto;
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
        public static TokenDto AddDevice(int AuthDevice, String DeviceName, String DeviceId)
        {
            UnitOfWork uw = new UnitOfWork();
            TokenDto dto;

            try
            {
                UserManager um = new UserManager(uw);
                User user = uw.DeviceRepository.GetByID(AuthDevice).Owner;

                String token = um.AddDeviceToUser(user, DeviceName, DeviceId);
                dto = new TokenDto(token);
            }
            finally
            {
                uw.Dispose();
            }

            return dto;
        }

        /// <summary>
        /// This method removes a device from the User's devices. After the removal, the device 
        /// won't be able to access the private API methods.
        /// </summary>
        /// <param name="AuthDevice">The authenticated device id.</param>
        /// <param name="DevId">The device Id.</param>
        /// <exception cref="ManagingDeviceException"></exception>
        public static void RemoveDevice(int AuthDevice, String DeviceId)
        {
            UnitOfWork uw = new UnitOfWork();
            try
            {
                UserManager um = new UserManager(uw);
                User user = uw.DeviceRepository.GetByID(AuthDevice).Owner;

                um.RemoveDeviceFromUser(user, DeviceId);
            }
            finally
            {
                uw.Dispose();
            }
        }

        /// <summary>
        /// List all the devices that the user has registered.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <returns>A list of devices.</returns>
        public static ICollection<DeviceDto> GetAllDevices(int AuthDevice)
        {
            UnitOfWork uw = new UnitOfWork();
            ICollection<DeviceDto> devDto;

            try
            {
                UserManager um = new UserManager(uw);
                User user = uw.DeviceRepository.GetByID(AuthDevice).Owner;

                ICollection<Device> devices = um.GetUserDevices(user);
                devDto = new List<DeviceDto>();
                foreach (Device d in devices)
                {
                    DeviceDto dto = new DeviceDto();
                    dto.Serialize(d);
                    devDto.Add(dto);
                }

            }
            finally
            {
                uw.Dispose();
            }

            return devDto;
        }

        /// <summary>
        /// Initiates the loading of the specified amount in the user account.
        /// </summary>
        /// <param name="AuthDevice">The authenticated device Id.</param>
        /// <param name="PayMethod">The payment method choosed.</param>
        /// <param name="Currency">The client currency</param>
        /// <param name="Amount">The amount to load in the account.</param>
        /// <returns>A PaymentDto with the information for the client proceed with the payment.</returns>
        public static PaymentDto PaymentBegin(int AuthDevice, String PayMethod, String Currency, String Amount)
        {
            UnitOfWork uw = new UnitOfWork();
            PaymentDto dto;

            try
            {
                AccountManager am = new AccountManager(uw);

                Account account = uw.DeviceRepository.GetByID(AuthDevice).Owner.Account;

                PaymentRequest payment = am.PaymentBegin(account, PayMethod, Currency, Amount);
                dto = new PaymentDto();

                dto.Serialize(payment);
            }
            finally
            {
                uw.Dispose();
            }

            return dto;
        }

        /// <summary>
        /// Concludes one payment initiated. The client is supposed to call this method after
        /// authorized the payment.
        /// </summary>
        /// <param name="AuthDevice">The authenticated device.</param>
        /// <param name="PayMethod">The payment method choosed.</param>
        /// <param name="PayerId">The ID of the client in the payment system.</param>
        /// <param name="PaymentId">The transaction ID.</param>
        /// <returns>The new balance after the concluded payment.</returns>
        public static BalanceDto PaymentEnd(int AuthDevice, String PayMethod, String PayerId, String PaymentId)
        {
            UnitOfWork uw = new UnitOfWork();
            BalanceDto dto;

            try
            {
                AccountManager am = new AccountManager(uw);

                Account account = uw.DeviceRepository.GetByID(AuthDevice).Owner.Account;

                PaymentRequest payment = am.PaymentEnd(account, PayMethod, PayerId, PaymentId);

                Account updatedAccount = uw.AccountRepository.GetByID(account.Id);

                dto = new BalanceDto();
                dto.Serialize(updatedAccount);
            }
            finally
            {
                uw.Dispose();
            }

            return dto;
        }

        public static void PaymentCancel(int AuthDevice, String PayMethod, String PaymentId)
        {
            UnitOfWork uw = new UnitOfWork();
            try
            {
                AccountManager am = new AccountManager(uw);

                Account account = uw.DeviceRepository.GetByID(AuthDevice).Owner.Account;

                am.PaymentCancel(account, PayMethod, PaymentId);
            }
            finally
            {
                uw.Dispose();
            }

        }

        /// <summary>
        /// Lists the available payment methods.
        /// </summary>
        /// <returns>List of methods.</returns>
        public static IList<PaymentMethodDto> GetPaymentMethods()
        {
            PaymentGateway gateway = new PaymentGateway();

            IList<PaymentMethodDto> list = new List<PaymentMethodDto>();

            foreach (String s in gateway.PaymentMethods)
            {
                PaymentMethodDto dto = new PaymentMethodDto();
                dto.Serialize(s);
                list.Add(dto);
            }

            return list;
        }
    }
}
