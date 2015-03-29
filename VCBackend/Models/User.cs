using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VCBackend.Models
{
    public partial class User : IEntity
    {
        [Key]
        public int Id { get; set; }
        public String Uid { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
        public bool AllowMarketing { get; set; }
        public InvoiceHeader InvoiceData { get; set; }
        public String Password { get; set; }

        public User() { }

        public User ( String Uid, String Name, String Email, String Password, String Phone, bool AllowMarketing, InvoiceHeader InvoiceData )
        {
            this.Name = Name;
            this.Email = Email;
            this.Phone = Phone;
            this.AllowMarketing = AllowMarketing;
            this.InvoiceData = InvoiceData;
            this.Password = Password;
        }

    }
}
