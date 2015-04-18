using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend
{
    public partial class Default
    {
        public Default()
        {
            this.Name = "Default";
            this.DeviceId = null;
        }

        override public bool IsDefault()
        {
            return true;
        }
    }
}