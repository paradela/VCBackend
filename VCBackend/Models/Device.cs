using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCBackend.Models
{
    public enum DeviceType
    {
        DEFAULT_DEVICE,
        MOBILE_DEVICE
    }

    public class Device
    {
        public int Id { get; set; }
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
