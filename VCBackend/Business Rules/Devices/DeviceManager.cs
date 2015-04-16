using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;
using VCBackend.Models;
using VCBackend.Utility.Security;

namespace VCBackend.Business_Rules.Devices
{
    public class DeviceManager
    {

        private static DeviceManager deviceManager = null;

        private IRepository<Device> rep;

        private DeviceManager()
        {
            rep = DeviceRepository.getRepositorySingleton();
        }

        public static DeviceManager getManagerSingleton()
        {
            if (deviceManager == null)
            {
                deviceManager = new DeviceManager();
            }
            return deviceManager;
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
            device.Token = AuthToken.GetAPIAccessToken(user, device);

            user.AddDevice(device);

            rep.Add(device); // store device on Database

            return device;
        }

        public void RemoveDevice(int EntityId)
        {
            Device dev = rep.FindById(EntityId);

            if (dev != null)
                rep.Delete(dev);
        }

    }
}