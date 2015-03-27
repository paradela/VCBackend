using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Models;

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
        public static bool CreateUser 
            (
            String Name,
            String Email,
            String Password,
            String Phone,
            String MobilePhone,
            bool AllowMarketing,
            InvoiceHeader InvoiceData
            ) 
        {
            //TODO: implementation
            return true;
        }

        public static bool UpdateUser
            (
            String Name,
            String Email,
            String Password,
            String Phone,
            String MobilePhone,
            bool AllowMarketing,
            InvoiceHeader InvoiceData
            )
        {
            //TODO: implementation
            return true;
        }

        /********************************
         * User Login
         ********************************/
        public static String Login
            (
            String Username,
            String TransformedPwd
            )
        {
            //TODO: implementation
            return "token";
        }

            
    }
}
