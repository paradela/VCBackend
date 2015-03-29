using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;

namespace VCBackend.Repositories
{
    public class UserRepository : IRepository<User>
    {
        VCardContext usersCtx;

        public UserRepository ()
        {
            usersCtx = new VCardContext();
        }

        IEnumerable<User> IRepository<User>.List
        {
            get 
            {
                return usersCtx.Users.AsEnumerable<User>();
            }
        }

        void IRepository<User>.Add(User entity)
        {
            usersCtx.Users.Add(entity);
            usersCtx.SaveChanges();
        }

        void IRepository<User>.Delete(User entity)
        {
            usersCtx.Users.Remove(entity);
            usersCtx.SaveChanges();
        }

        void IRepository<User>.Update(User entity)
        {
            usersCtx.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            usersCtx.SaveChanges();   
        }

        User IRepository<User>.FindById(int Id)
        {
            var result = (from r in usersCtx.Users where r.Id == Id select r).FirstOrDefault();
            return result;   
        }
    }
}