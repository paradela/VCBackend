using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VCBackend.Models
{
    public class PartialDevice : Device
    {
        public int Id;
        private String DeviceID;
        public String Token;

        int Device.Id
        {
            get { return Id; }
        }

        string Device.DeviceId
        {
            get { return DeviceID; }
        }

        string Device.AccessToken
        {
            get
            {
                return Token;
            }
            set
            {
                this.Token = value;
            }
        }

        bool Device.HasFullAccess()
        {
            return false;
        }
    }
}
