using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace VCBackend.Models
{
    class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}
