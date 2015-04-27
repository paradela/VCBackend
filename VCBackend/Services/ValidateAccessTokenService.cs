using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Repositories;
using VCBackend.Exceptions;

namespace VCBackend.Services
{
    class ValidateAccessTokenService : IService
    {
        private String accessToken;
        private Device authDevice;

        public String AccessToken
        {
            set
            {
                accessToken = value;
            }
        }

        public Device AuthDevice
        {
            get
            {
                return authDevice;
            }
        }

        public ValidateAccessTokenService(UnitOfWork UnitOfWork)
            : base(UnitOfWork) { }

        public bool Execute()
        {
            if (accessToken == null || accessToken == String.Empty)
                return false;

            Device device = UnitOfWork.DeviceRepository.Get(filter: dev => (dev.Token == accessToken)).FirstOrDefault();

            if (device == null) //the token is not valid anymore! 
                throw new InvalidAuthToken("The authentication token is not valid anymore!");

            else authDevice = device;
            return true;
        }

    }
}
