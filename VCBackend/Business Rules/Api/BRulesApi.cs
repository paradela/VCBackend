using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Models;
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
        public static String CreateUser ( String Name, String Email, String Password) 
        {
            UserManager um = UserManager.getUserManagerSingleton();

            User user = um.CreateUser(Name, Email, Password);

            //returns a token for authentication!
            //It returns the First device, because the account was just created and it only has one device that is the default one!
            return user.Devices.First<Device>().Token;
        }

        public static void UpdateUser ( String Name, String Email, String Password)
        {
            UserManager um = UserManager.getUserManagerSingleton();
            
        }

        /********************************
         * User Login
         ********************************/
        public static String Login ( String Username, String Password, String DeviceId)
        {
            UserManager um = UserManager.getUserManagerSingleton();
            return um.UserLogin(Username, Password, DeviceId);
        }

        /********************************
         * 
         ********************************/
            
    }
}
