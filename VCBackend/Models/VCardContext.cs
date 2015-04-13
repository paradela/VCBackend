using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace VCBackend.Models
{
    public class VCardContext : DbContext
    {

        public VCardContext()
        {
            Database.SetInitializer<VCardContext>(new DropCreateDatabaseAlways<VCardContext>());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<VCard> Cards { get; set; }
    }
}
