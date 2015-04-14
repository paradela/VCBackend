using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;

namespace VCBackend.Repositories
{
    public class UserRepository : IRepository<User>
    {
        //private VCardContext usersCtx;
        private ModelContainer usersCtx;
        private static UserRepository rep = null;


        private UserRepository ()
        {
            usersCtx = new ModelContainer();
        }

        public static IRepository<User> getRepositorySingleton()
        {
            if (rep == null)
                rep = new UserRepository();

            return rep;
        }

        IEnumerable<User> IRepository<User>.List
        {
            get 
            {
                return usersCtx.UserSet.AsEnumerable<User>();
            }
        }

        void IRepository<User>.Add(User entity)
        {
            usersCtx.UserSet.Add(entity);
            usersCtx.SaveChanges();
        }

        void IRepository<User>.Delete(User entity)
        {
            usersCtx.UserSet.Remove(entity);
            usersCtx.SaveChanges();
        }

        void IRepository<User>.Update(User entity)
        {
            usersCtx.SaveChanges();   
        }

        User IRepository<User>.FindById(int Id)
        {
            var result = (from r in usersCtx.UserSet where r.Id == Id select r).FirstOrDefault();
            return result;   
        }
    }
}