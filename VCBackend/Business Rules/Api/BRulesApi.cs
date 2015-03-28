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
    class BRulesApi
    {
        /********************************
         * User Creation and Update
         ********************************/
        public static void CreateUser ( String Name, String Email, String Password, String Phone, bool AllowMarketing, String NIF, String Address, String ZipCode, String Locality, String Country ) 
        {
            UserManager um = UserManager.getUserManagerSingleton();
            String[] AddrLines = Address.Split('\n');
            Address addr = new Address(AddrLines, ZipCode, Locality, Country);
            InvoiceHeader invoice = new InvoiceHeader(Name, NIF, addr);
            
            um.CreateUser(Name, Email, Password, Phone, AllowMarketing, invoice);
        }

        public static bool UpdateUser ( String Name, String Email, String Password, String Phone, bool AllowMarketing, InvoiceHeader InvoiceData )
        {
            //TODO: implementation
            return true;
        }

        /********************************
         * User Login
         ********************************/
        public static String Login ( String Username, String TransformedPwd )
        {
            //TODO: implementation
            return "token";
        }

        /********************************
         * 
         ********************************/
            
    }
}
