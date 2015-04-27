using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Repositories;
using VCBackend.Models.Dto;

namespace VCBackend.Services
{
    public class GetUserDevicesService : IService
    {
        private ICollection<DeviceDto> dtoList;

        public ICollection<DeviceDto> DeviceDtoList
        {
            get
            {
                return dtoList;
            }
        }

        public GetUserDevicesService(UnitOfWork UnitOfWork, Device AuthDevice)
            : base(UnitOfWork, AuthDevice)
        {
            dtoList = new List<DeviceDto>();
        }

        public override bool ExecuteService()
        {
            User user = AuthDevice.Owner;

            foreach (Device d in user.Devices)
            {
                DeviceDto dto = new DeviceDto();
                dto.Serialize(d);
                dtoList.Add(dto);
            }
            return true;
        }
    }
}
