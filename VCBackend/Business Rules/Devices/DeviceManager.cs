using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;
using VCBackend.Models;
using VCBackend.Utility.Security;

namespace VCBackend.Business_Rules.Devices
{
    public class DeviceManager : Manager
    {
        public DeviceManager(UnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {
        }

        /// <summary>
        /// This method creates a new device with the given name and device id. 
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <param name="Name">The new device name</param>
        /// <param name="DeviceId">The new device identifier. If it is @null a default without
        /// full access to the API will be created!</param>
        /// <returns>The created Device.</returns>
        public Device CreateDeviceToUser(User user, String Name = null, String DeviceId = null)
        {
            Device device;

            if (DeviceId == null)
                device = new Default();
            else device = new Mobile(DeviceId, Name);

            //Generates a token that should be used next to register a device.
            device.Token = AuthToken.GetAPIAccessJwt(user, device);

            user.AddDevice(device);

            UnitOfWork.DeviceRepository.Add(device); // store device on Database
            UnitOfWork.Save();

            return device;
        }

        public void RemoveDevice(int EntityId)
        {
            Device dev = UnitOfWork.DeviceRepository.GetByID(EntityId);

            if (dev != null)
            {
                UnitOfWork.DeviceRepository.Delete(dev);
                UnitOfWork.Save();
            }
        }

    }
}