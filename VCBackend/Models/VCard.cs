using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models
{
    public class VCard : IEntity
    {
        //Specific for Cards with CTS512B model
        public byte[] Data { get; set; }

        public VCard(byte[] Data)
        {
            this.Data = Data;
        }

        public byte[] Read(uint Offset, uint Length)
        {
            return null;
        }

        public bool Write(uint OffSet, byte[] Data, uint Length)
        {
            return false;
        }
    }
}