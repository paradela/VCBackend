using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;
using System.Text.RegularExpressions;

namespace VCBackend.Services
{
    public abstract class IUserService : IDeviceService
    {

        public IUserService(UnitOfWork UnitOfWork, Device AuthDevice = null)
            : base(UnitOfWork, AuthDevice)
        { }

        public String Name
        {
            protected get;
            set;
        }
        public String Email
        {
            protected get;
            set;
        }
        public String Password
        {
            protected get;
            set;
        }

        public String Pin
        {
            protected get;
            set;
        }

        //http://stackoverflow.com/questions/5859632/regular-expression-for-password-validation
        protected bool ValidatePassword(string password)
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

        protected bool ValidatePin(char[] pin)
        {
            const int MIN_LENGTH = 4;

            if (pin == null) throw new ArgumentNullException();

            if (pin.Length == MIN_LENGTH)
            {
                foreach (char c in pin)
                {
                    if (c >= '0' && c <= '9') continue;
                    else return false;
                }
                return true;
            }
            else return false;

        }

        
         //This method validates if the attributes are wel formed.
         //It will not look for those that are @null!!

        protected bool ValidateUserData(String name = null, String email = null, String password = null, String pin = null)
        {
            Regex nameRgx = new Regex(@"[A-zÀ-ú-]{2}[A-zÀ-ú-\s]*");
            Regex emailRgx = new Regex(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
            Regex pinRgx = new Regex(@"[0-9]{4}");
            bool validName = (name != null) ? nameRgx.IsMatch(name) : true;
            bool validEmail = (email != null) ? emailRgx.IsMatch(email) : true;
            bool validPwd = (password != null) ? ValidatePassword(password) : true;
            bool validPin = (pin != null) ? pinRgx.IsMatch(pin) : true;
            return validName && validEmail && validPwd;
        }

        protected bool ExistUserWithEmail(String Email)
        {
            IEnumerable<User> users = UnitOfWork.UserRepository.Get(filter: u => (u.Email == Email));

            if (users.Count() > 0)
                return true;
            else return false;
        }
    }
}