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
    [RoutePrefix("api/user")]
    public class UsersController : ApiController
    {
        //POST api/user?n="Name"&e="email@mail.pt"&p="passw0rd"
        [Route("")]
        public IHttpActionResult PostNewUser([FromUri] String n, [FromUri] String e, [FromUri] String p)
        {
            try
            {
                String token = BRulesApi.CreateUser(n, e, p);
                return Ok(token);
            }
            catch (MalformedUserDetailsException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UserAlreadyExistException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //POST api/user?u=email@mail.pt&p=Pa$$w0rd
        [Route("")]
        public IHttpActionResult PostLogin([FromUri] String u, [FromUri] String p, [FromUri] String id = null)
        {
            try
            {
                String token = BRulesApi.Login(u, p, id);
                return Ok(token);
            }
            catch (InvalidCredentialsException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("edit")]
        [AuthenticationFilter]
        public IHttpActionResult PostUpdateUser([FromUri] String n = null, [FromUri] String e = null, [FromUri] String p = null)
        {
            //User authUser = AuthenticationFilter.GetAuthenticatedDevice(ActionContext)
            return Ok();
        }
    }
}
