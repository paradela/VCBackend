using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace VCBackend
{
    public partial class Device
    {
        public Device()
        {
            this.Name = "Default";
            this.DeviceId = null;
        }

        public Device(String Name, String DeviceId)
        {
            this.Name = Name;
            this.DeviceId = DeviceId;
        }
    }
}
