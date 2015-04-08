using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using VCBackend.Filters;
using VCBackend.Models;
using VCBackend.Models.Dto;
using VCBackend.Business_Rules;
using VCBackend.Business_Rules.Users;

namespace VCBackend.Controllers
{
    [RoutePrefix("api/user")]
    public class UsersController : ApiController
    {
        //POST api/user?n="Name"&e="email@mail.pt"&p="passw0rd"
        [Route("")]
        public TokenDto PostNewUser([FromUri] String n, [FromUri] String e, [FromUri] String p)
        {
            try
            {
                TokenDto token = BRulesApi.CreateUser(n, e, p);
                return token;
            }
            catch (MalformedUserDetailsException ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(resp);
            }
            catch (UserAlreadyExistException ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(resp);
            }
        }

        //POST api/user?u=email@mail.pt&p=Pa$$w0rd
        [Route("")]
        public TokenDto PostLogin([FromUri] String u, [FromUri] String p, [FromUri] String id = null)
        {
            try
            {
                TokenDto token = BRulesApi.Login(u, p, id);
                return token;
            }
            catch (InvalidCredentialsException ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(resp);
            }
        }

        //POST api/user/edit?t=123token&n=Jon Doe&e=jon.doe@mail.pt&p=Pa$$w0rd
        [Route("edit")]
        [VCAuthenticate]
        public UserDto PostUpdateUser([FromUri] String n = null, [FromUri] String e = null, [FromUri] String p = null)
        {
            User authUser = VCAuthenticate.GetAuthenticatedDevice(ActionContext).Owner;

            try
            {
                UserDto dto = BRulesApi.UpdateUser(authUser, n, e, p);
                return dto;
            }
            catch (MalformedUserDetailsException ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(resp);
            }
            catch (UserAlreadyExistException ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(resp);
            }
        }

        //GET api/user?t=123token
        [Route("")]
        [VCAuthenticate]
        public UserDto GetUser()
        {
            User authUser = VCAuthenticate.GetAuthenticatedDevice(ActionContext).Owner;

            UserDto dto = BRulesApi.GetUser(authUser);

            return dto;
        }

        //POST api/user/device?t=123token
        [Route("device")]
        [VCAuthenticate]
        public TokenDto PostAddDevice([FromUri] String n, [FromUri] String id)
        {
            User authUser = VCAuthenticate.GetAuthenticatedDevice(ActionContext).Owner;

            try
            {
                TokenDto token = BRulesApi.AddDevice(authUser, n, id);
                return token;
            }
            catch (ManagingDeviceException ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(resp);
            }
        }

        //DELETE api/user/device/id?t=123token
        [Route("device/{id}")]
        [VCAuthenticate]
        public IHttpActionResult DeleteDevice([FromUri] String id)
        {
            User authUser = VCAuthenticate.GetAuthenticatedDevice(ActionContext).Owner;

            try
            {
                BRulesApi.RemoveDevice(authUser, id);
                return Ok();
            }
            catch (ManagingDeviceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //GET api/user/device?t=123token
        [Route("device")]
        [VCAuthenticate]
        public IHttpActionResult GetAllDevices() 
        {
            User authUser = VCAuthenticate.GetAuthenticatedDevice(ActionContext).Owner;

            ICollection<DeviceDto> devices = BRulesApi.GetAllDevices(authUser);
            return Ok(devices);
        }

    }
}
