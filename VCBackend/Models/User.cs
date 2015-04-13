using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VCBackend.Models
{
    public partial class User : IEntity
    {
        [Required]
        public String Name { get; set; }
        [Required]
        public String Email { get; set; }
        [Required]
        public String Password { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
        public virtual Account Account { get; set; }

        public User() { }

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
