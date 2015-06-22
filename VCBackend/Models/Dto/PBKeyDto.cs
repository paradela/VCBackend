using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class PBKeyDto : IDto<PBKey>
    {
        public String Algo { get; set; }
        public int HashSize { get; set; }
        public int Cicles { get; set; }
        public String Salt { get; set; }

        public void Serialize(PBKey entity)
        {
            this.Algo = entity.Algo;
            this.HashSize = entity.HashSize;
            this.Cicles = entity.Cicles;
            this.Salt = entity.B64Salt;
        }
    }
}