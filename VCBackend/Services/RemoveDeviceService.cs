using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Repositories;
using VCBackend.Exceptions;

namespace VCBackend.Services
{
    public class RemoveDeviceService : IDeviceService
    {
        public RemoveDeviceService(UnitOfWork UnitOfWork, Device AuthDevice)
            : base(UnitOfWork, AuthDevice) { }

        protected override bool ExecuteService()
        {
            if (DeviceId == null)
                return false;
            User user = AuthDevice.Owner;

            if (!ValidateDeviceData(null, DeviceId))
                throw new InvalidDataFormat("Invalid id format");

            var dev = (from d in user.Devices
                       where d.DeviceId == DeviceId
                       select d).First();
            if (dev == null)
                throw new DeviceNotFound("Device with given Id does not exist");

            user.Devices.Remove(dev);

            UnitOfWork.DeviceRepository.Delete(dev);

            UnitOfWork.UserRepository.Update(user);
            UnitOfWork.Save();
            return true;
        }
    }
}
