using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCBackend.Models
{
    class User
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
        public String MobilePhone { get; set; }
        public bool AllowMarketing { get; set; }
        public InvoiceHeader InvoiceData { get; set; }
        public String Password { get; set; }

        public User ( String Name, String Email, String Password, String Phone, String MobilePhone, bool AllowMarketing, InvoiceHeader InvoiceData )
        {
            this.Name = Name;
            this.Email = Email;
            this.Phone = Phone;
            this.MobilePhone = MobilePhone;
            this.AllowMarketing = AllowMarketing;
            this.InvoiceData = InvoiceData;
            this.Password = Password;
        }



    }
}
