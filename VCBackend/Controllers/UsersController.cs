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
using VCBackend.Business_Rules.Exceptions;
using VCBackend.Business_Rules.Errors;

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
            catch (VCException ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
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
            catch (VCException ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
        }

        //POST api/user/edit?t=123token&n=Jon Doe&e=jon.doe@mail.pt&p=Pa$$w0rd
        [Route("edit")]
        [VCAuthenticate]
        public UserDto PostUpdateUser([FromUri] String n = null, [FromUri] String e = null, [FromUri] String p = null)
        {
            int authDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);

            try
            {
                UserDto dto = BRulesApi.UpdateUser(authDev, n, e, p);
                return dto;
            }
            catch (VCException ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
        }

        //GET api/user?t=123token
        [Route("")]
        [VCAuthenticate]
        public UserDto GetUser()
        {
            int authDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);

            UserDto dto = BRulesApi.GetUser(authDev);

            return dto;
        }

        //POST api/user/device?t=123token
        [Route("device")]
        [VCAuthenticate]
        public TokenDto PostAddDevice([FromUri] String n, [FromUri] String id)
        {
            int authDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);

            try
            {
                TokenDto token = BRulesApi.AddDevice(authDev, n, id);
                return token;
            }
            catch (VCException ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
        }

        //DELETE api/user/device/id?t=123token
        [Route("device/{id}")]
        [VCAuthenticate]
        public IHttpActionResult DeleteDevice([FromUri] String id)
        {
            int authDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);

            try
            {
                BRulesApi.RemoveDevice(authDev, id);
                return Ok();
            }
            catch (VCException ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
        }

        //GET api/user/device?t=123token
        [Route("device")]
        [VCAuthenticate]
        public ICollection<DeviceDto> GetAllDevices() 
        {
            int authDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
            try
            {
                ICollection<DeviceDto> devices = BRulesApi.GetAllDevices(authDev);
                return devices;
            }
            catch (VCException ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new ErrorResponse(ex));
            }
        }

    }
}
