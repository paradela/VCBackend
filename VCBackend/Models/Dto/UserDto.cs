using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class UserDto : IDto<User>
    {
        public String Name { get; set; }
        public String Email { get; set; }

        public void Serialize(User entity)
        {
            User user = (User)entity;
            Name = user.Name;
            Email = user.Email;
        }
    }
}