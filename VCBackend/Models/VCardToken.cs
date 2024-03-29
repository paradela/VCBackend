﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend
{
    public partial class VCardToken
    {
        public VCardToken() 
        {
            this.Used = false;
        }
        public VCardToken(byte[] Data)
        {
            this.Data = System.Convert.ToBase64String(Data);
            this.Used = false;
        }

        public byte[] Read(uint Offset, uint Length)
        {
            return null;
        }

        public bool Write(uint OffSet, byte[] Data, uint Length)
        {
            if ((OffSet + Length) <= this.Data.Length || Data.Length >= Length)
            {
                byte[] card = System.Convert.FromBase64String(this.Data);
                System.Array.Copy(Data, 0, card, OffSet, Length);
                this.Data = System.Convert.ToBase64String(card);
                return true;
            }
            else return false;
        }
    }
}