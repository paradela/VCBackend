using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCBackend.Models
{
    class Address
    {
        String[] AddressLine { get; set; }
        String ZipCode { get; set; }
        String Locality { get; set; }
        String Country { get; set; }

        public Address(String[] Addressline, String ZipCode, String Locality, String Country)
        {
            this.AddressLine = AddressLine;
            this.ZipCode = ZipCode;
            this.Locality = Locality;
            this.Country = Country;
        }
    }
}
