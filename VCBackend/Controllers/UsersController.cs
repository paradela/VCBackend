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

        //POST api/user/edit?t=123token&n=Jon Doe&e=jon.doe@mail.pt&p=Pa$$w0rd
        [Route("edit")]
        [VCAuthenticate]
        public IHttpActionResult PostUpdateUser([FromUri] String n = null, [FromUri] String e = null, [FromUri] String p = null)
        {
            User authUser = VCAuthenticate.GetAuthenticatedDevice(ActionContext).Owner;

            try
            {
                BRulesApi.UpdateUser(authUser, n, e, p);
                return Ok();
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

        //GET api/user?t=123token
        [Route("")]
        [VCAuthenticate]
        public IHttpActionResult GetUser()
        {
            User authUser = VCAuthenticate.GetAuthenticatedDevice(ActionContext).Owner;

            UserDto dto = BRulesApi.GetUser(authUser);

            return Ok(dto);
        }

        //POST api/user/device?t=123token
        [Route("device")]
        [VCAuthenticate]
        public IHttpActionResult PostAddDevice([FromUri] String n, [FromUri] String id)
        {
            User authUser = VCAuthenticate.GetAuthenticatedDevice(ActionContext).Owner;

            try
            {
                String token = BRulesApi.AddDevice(authUser, n, id);
                return Ok(token);
            }
            catch (ManagingDeviceException ex)
            {
                return BadRequest(ex.Message);
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
