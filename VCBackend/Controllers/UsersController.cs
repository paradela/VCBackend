using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using VCBackend.Filters;
using VCBackend.Models;
using VCBackend.Business_Rules;
using VCBackend.Business_Rules.Users;

namespace VCBackend.Controllers
{
    //PUT api/users?t="123token"&n="Name"&e="email@mail.pt"&p="passw0rd"
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        [Route("")]
        public IHttpActionResult PutNewUser([FromUri] String n, [FromUri] String e, [FromUri] String p)
        {
            try
            {
                String token = BRulesApi.CreateUser(n, e, p);
                return Ok(token);
            }
            catch (MalformedUserDetailsException)
            {
                return BadRequest();
            }
            catch (UserAlreadyExistException)
            {
                return BadRequest();
            }
        }

        [Route("devices")]
        [AuthenticationFilter]
        public IEnumerable<String> GetAllUsersDevices()
        {
            String[] products = new String[] 
            { 
                "moto",
                "one plus one",
                "LG g3"
            };
            return products;
        }
    }
}
