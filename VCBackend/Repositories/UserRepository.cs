using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;

namespace VCBackend.Repositories
{
    public class UserRepository : IRepository<User>
    {
        UsersContext usersCtx;

        public UserRepository ()
        {
            usersCtx = new UsersContext();
        }

        IEnumerable<User> IRepository<User>.List
        {
            get 
            {
                using (var db = usersCtx)
                {
                    return db.Users.AsEnumerable<User>();
                }
            }
        }

        void IRepository<User>.Add(User entity)
        {
            using (var db = usersCtx)
            {
                db.Users.Add(entity);
                db.SaveChanges();
            }
        }

        void IRepository<User>.Delete(User entity)
        {
            using (var db = usersCtx)
            {
                db.Users.Remove(entity);
                db.SaveChanges();
            }
        }

        void IRepository<User>.Update(User entity)
        {
            using (var db = usersCtx)
            {
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        User IRepository<User>.FindById(int Id)
        {
            using (var db = usersCtx)
            {
                var result = (from r in db.Users where r.Id == Id select r).FirstOrDefault();
                return result;
            }
        }
    }
}