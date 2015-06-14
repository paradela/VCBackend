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
using VCBackend.Exceptions;
using VCBackend.Errors;
using VCBackend.Repositories;
using VCBackend.Services;

namespace VCBackend.Controllers
{
    [RoutePrefix("api/user")]
    public class UsersController : ApiController
    {
        //POST api/user?n="Name"&e="email@mail.pt"&p="passw0rd"
        [Route("")]
        public AccessTokensDto PostNewUser([FromUri] String n, [FromUri] String e, [FromUri] String p, [FromUri] String id = null)
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                UserCreationService service = new UserCreationService(uw);
                service.Name = n;
                service.Email = e;
                service.Password = p;
                service.DeviceId = id;
                if (service.Execute())
                    return service.AccessToken;
                else return null;
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
        public AccessTokensDto PostLogin([FromUri] String u, [FromUri] String p, [FromUri] String id = null)
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                UserLoginService service = new UserLoginService(uw);
                service.Email = u;
                service.Password = p;
                service.DeviceId = id;
                if (service.Execute())
                    return service.TokensDto;
                else return null;
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
        //GET api/user/refresh_token?r=123token
        [Route("refresh_token")]
        public AuthTokenDto GetRefreshedAuthToken([FromUri] String r)
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                RefreshAuthTokenService service = new RefreshAuthTokenService(uw);
                service.RefreshToken = r;
                if (service.Execute())
                    return service.AuthToken;
                else return null;
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
            try
            {
                UnitOfWork uw = new UnitOfWork();
                int authDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(authDev);
                UserUpdateService service = new UserUpdateService(uw, dev);
                service.Name = n;
                service.Email = e;
                service.Password = p;
                if (service.Execute())
                    return service.UserDto;
                return null;
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
            UnitOfWork uw = new UnitOfWork();
            int authDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
            Device dev = uw.DeviceRepository.GetByID(authDev);
            GetUserService service = new GetUserService(uw, dev);
            if (service.Execute())
                return service.UserDto;
            else return null;
        }

        //GET api/user/test
        [Route("test")]
        public UserDto GetUserTest()
        {
            User u = new User("Teste", "test@mail.pt", "Password");
            UserDto dto = new UserDto();
            dto.Serialize(u);
            return dto;
        }

        ////POST api/user/device?t=123token
        //[Route("device")]
        //[VCAuthenticate]
        //public TokenDto PostAddDevice([FromUri] String n, [FromUri] String id)
        //{
        //    try
        //    {
        //        UnitOfWork uw = new UnitOfWork();
        //        int authDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
        //        Device dev = uw.DeviceRepository.GetByID(authDev);
        //        AddDeviceService service = new AddDeviceService(uw, dev);
        //        service.DeviceName = n;
        //        service.DeviceId = id;
        //        if (service.ExecuteService())
        //            return service.TokenDto;
        //        else return null;
        //    }
        //    catch (VCException ex)
        //    {
        //        throw new HttpResponseException(new ErrorResponse(ex));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HttpResponseException(new ErrorResponse(ex));
        //    }
        //}

        //DELETE api/user/device/id?t=123token
        [Route("device/{id}")]
        [VCAuthenticate]
        public void DeleteDevice([FromUri] String id)
        {
            try
            {
                UnitOfWork uw = new UnitOfWork();
                int authDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(authDev);
                RemoveDeviceService service = new RemoveDeviceService(uw, dev);
                service.DeviceId = id;
                service.Execute();
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
            try
            {
                UnitOfWork uw = new UnitOfWork();
                int authDev = VCAuthenticate.GetAuthenticatedDevice(ActionContext);
                Device dev = uw.DeviceRepository.GetByID(authDev);
                GetUserDevicesService service = new GetUserDevicesService(uw, dev);
                if (service.Execute())
                    return service.DeviceDtoList;
                return null;
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
