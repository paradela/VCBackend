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
        protected String name;
        protected String email;
        protected String password;

        public IUserService(UnitOfWork UnitOfWork, Device AuthDevice = null)
            : base(UnitOfWork, AuthDevice)
        { }

        public String Name
        {
            set
            {
                name = value;
            }
        }
        public String Email
        {
            set
            {
                email = value;
            }
        }
        public String Password
        {
            set
            {
                password = value;
            }
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

        
         //This method validates if the attributes are wel formed.
         //It will not look for those that are @null!!

        protected bool ValidateUserData(String Name = null, String Email = null, String Password = null)
        {
            Regex nameRgx = new Regex(@"[A-zÀ-ú-]{2}[A-zÀ-ú-\s]*");
            Regex emailRgx = new Regex(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
            bool validName = (Name != null) ? nameRgx.IsMatch(Name) : true;
            bool validEmail = (Email != null) ? emailRgx.IsMatch(Email) : true;
            return validName && validEmail && (Password != null) ? ValidatePassword(Password) : true;
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