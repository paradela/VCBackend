using VCBackend.Models.Dto;
using VCBackend.Repositories;

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

        public override bool ExecuteService()
        {
            dto = new UserDto();
            dto.Serialize(AuthDevice.Owner);
            return true;
        }
    }
}
