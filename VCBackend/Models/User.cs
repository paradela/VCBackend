using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

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

    }
}
