using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace VCBackend.Models
{
    [ComplexType]
    public class InvoiceHeader
    {
        public String Name { get; set; }
        public String NIF { get; set; }
        public String Address { get; set; }
        public String ZipCode { get; set; }
        public String Locality { get; set; }
        public String Country { get; set; }

        public InvoiceHeader() { }

        public InvoiceHeader(String Name, String NIF, String Address, String ZipCode, String Locality, String Country)
        {
            this.Name = Name;
            this.NIF = NIF;
            this.Address = Address;
            this.ZipCode = ZipCode;
            this.Locality = Locality;
            this.Country = Country;
        }
    }
}
