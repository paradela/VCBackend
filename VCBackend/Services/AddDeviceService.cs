using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Repositories;
using VCBackend.Exceptions;
using VCBackend.Models.Dto;

namespace VCBackend.Services
{
    public class AddDeviceService : IDeviceService
    {
        private String name;
        private TokenDto dto;

        public String DeviceName
        {
            set
            {
                this.name = value;
            }
        }

        public TokenDto TokenDto
        {
            get
            {
                return dto;
            }
        }

        public AddDeviceService(UnitOfWork UnitOfWork, Device AuthDevice = null)
            : base(UnitOfWork, AuthDevice) { }

        public override bool Execute()
        {
            User user = AuthDevice.Owner;

            if (name == null || deviceid == null) // Bad service initialization
            {
                dto = null;
                return false;
            }

            if (!ValidateDeviceData(name, deviceid))
                throw new DeviceNotFound(String.Format("Invalid device name: {0} or Id {1}.", name, deviceid));

            //Check if the user already have a Device with id @DevId
            var devCount = (from d in user.Devices
                            where d.DeviceId == deviceid
                            select d).Count();

            if (devCount == 0)
            {
                Device dev = user.CreateDevice(name, deviceid);
                UnitOfWork.UserRepository.Update(user);
                UnitOfWork.Save();
                dto = new TokenDto(dev.Token);
            }
            else throw new DeviceAlreadyRegistered(String.Format("This user already has a device with identifier: {0}", deviceid));

            return true;
        }
    }
}
