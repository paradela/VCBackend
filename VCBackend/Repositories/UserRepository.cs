using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;

namespace VCBackend.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private VCardContext usersCtx;
        private static UserRepository rep = null;


        private UserRepository ()
        {
            usersCtx = new VCardContext();
        }

        public static UserRepository getRepositorySingleton()
        {
            if (rep == null)
                rep = new UserRepository();

            return rep;
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
            usersCtx.SaveChanges();   
        }

        User IRepository<User>.FindById(int Id)
        {
            var result = (from r in usersCtx.Users where r.Id == Id select r).FirstOrDefault();
            return result;   
        }


        ICollection<User> IRepository<User>.ExecuteSQL(string SqlQuery)
        {
            return usersCtx.Users.SqlQuery(SqlQuery).ToList();
        }
    }
}