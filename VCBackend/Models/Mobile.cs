﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend
{
    public partial class Mobile
    {
        public Mobile() { }
        public Mobile(String DeviceId, String Name)
        {
            this.Name = Name;
            this.DeviceId = DeviceId;
        }

        override public bool IsDefault()
        {
            return false;
        }
    }
}