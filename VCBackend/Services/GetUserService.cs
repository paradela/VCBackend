using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Repositories;
using VCBackend.Models.Dto;

namespace VCBackend.Services
{
    class GetUserService : IService
    {
        private UserDto dto;

        public UserDto UserDto
        {
            get
            {
                return dto;
            }
        }

        public GetUserService(UnitOfWork UnitOfWork, Device AuthDevice) : base(UnitOfWork, AuthDevice) { }

        public bool Execute()
        {
            if (AuthDevice == null)
            {
                dto = null;
                return false;
            }
            else
            {
                dto = new UserDto();
                dto.Serialize(AuthDevice.Owner);
                return true;
            }
        }
    }
}
