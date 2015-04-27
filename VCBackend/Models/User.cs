using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Utility.Security;
using VCBackend.Exceptions;

namespace VCBackend
{
    public partial class User
    {
        public User(String Name, String Email, String Password)
        {
            this.Name = Name;
            this.Email = Email;
            this.Password = Password;
            this.Devices = new List<Device>();
        }
        
        public void AddDevice(Device device)
        {
            device.Owner = this;
            Devices.Add(device);
        }

        public Device CreateDevice(String Name = null, String DeviceId = null)
        {
            Device device;

            if (DeviceId == null)
                device = new Device();
            else device = new Device(DeviceId, Name);

            //Generates a token that should be used next to register a device.
            device.Token = AuthToken.GetAPIAccessJwt(this, device);

            AddDevice(device);

            return device;
        }

        public Device RemoveDevice(String DeviceId)
        {
            var dev = (from d in Devices
                      where d.DeviceId == DeviceId &&
                      d.DeviceId != null
                      select d).FirstOrDefault();

            if (dev == null)
                throw new DeviceNotFound("Device identifier is unknown.");

            Devices.Remove(dev);

            return dev;
        }

        public Account CreateAccount()
        {
            Account account = new Account(0.0);
            Account = account;
            return account;
        }

    }
}
