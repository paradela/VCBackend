using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCBackend.Models
{
    public class Device
    {
        public int Id { get; set; }
        public String DeviceId { get; set; }
        public String Token { get; set; }
        public Boolean FullDevice { get; set; }

        public Device() { }

        private Device(String DeviceId, String Token, Boolean FullDevice)
        {
            this.DeviceId = DeviceId;
            this.Token = Token;
            this.FullDevice = FullDevice;
        }

        public static Device CreateFullAccessDevice(String DeviceId, String Token)
        {
            return new Device(DeviceId, Token, true);
        }

        public static Device CreatePartialAccessDevice(String Token)
        {
            return new Device(null, Token, false);
        }

        public Boolean HasFullAccess()
        {
            return FullDevice;
        }

    }
}
