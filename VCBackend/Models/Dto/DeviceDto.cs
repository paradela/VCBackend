﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public enum DeviceTypeDto
    {
        DTO_DEFAULT_DEVICE,
        DTO_MOBILE_DEVICE
    }

    public class DeviceDto : IDto<Device>
    {
        public DeviceTypeDto Type { get; set; }
        public String Name { get; set; }
        public String Id { get; set; }

        public void Serialize(Device entity)
        {
            Device dev = (Device)entity;

            Name = dev.Name;
            Type = (dev.DeviceId == null) ?
                DeviceTypeDto.DTO_DEFAULT_DEVICE : DeviceTypeDto.DTO_MOBILE_DEVICE;
            Id = dev.DeviceId;
        }
    }
}