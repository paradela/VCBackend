using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCBackend.Models
{
    class InvoiceHeader
    {
        public String Name { get; set; }
        public String NIF { get; set; }
        public Address Addr { get; set; }
    }
}
