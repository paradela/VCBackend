using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace VCBackend.Models
{
    public partial class Device : IEntity
    {
        public readonly static int DEFAULT_DEVICE = 1;
        public readonly static int MOBILE_DEVICE = 2;

        public virtual User Owner { get; set; }
        public int Type { get; set; }
        public String Name { get; set; }
        public String DeviceId { get; set; }
        public String Token { get; set; }

        public Device() 
        {
            Type = DEFAULT_DEVICE;
        }

        public Device(String devId)
        {
            Type = MOBILE_DEVICE;
            DeviceId = devId;
        }
    }
}
