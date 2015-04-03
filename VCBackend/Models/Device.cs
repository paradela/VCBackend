using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace VCBackend.Models
{
    public enum DeviceType
    {
        DEFAULT_DEVICE,
        MOBILE_DEVICE
    }

    public class Device : IEntity
    {
        
        //[ForeignKey("OwnerId")]
        public virtual User Owner { get; set; }
        public DeviceType Type { get; set; }
        public String Name { get; set; }
        public String DeviceId { get; set; }
        public String Token { get; set; }

        public Device() 
        {
            Type = DeviceType.DEFAULT_DEVICE;
        }

        public Device(String devId)
        {
            Type = DeviceType.MOBILE_DEVICE;
            DeviceId = devId;
        }
    }
}
