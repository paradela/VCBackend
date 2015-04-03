using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VCBackend.Filters;

namespace VCBackend.Controllers
{
    
    [AuthenticationFilter]
    public class UsersController : ApiController
    {
        String[] products = new String[] 
        { 
            "one",
            "two",
            "three"
        };

        public IEnumerable<String> GetAllUsers()
        {
            return products;
        }
    }
}
