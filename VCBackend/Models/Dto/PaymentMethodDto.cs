using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class PaymentMethodDto : IDto<String>
    {
        String Method { get; set; }

        public void Serialize(string entity)
        {
            Method = entity;
        }
    }
}