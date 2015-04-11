using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;

namespace VCBackend.Repositories
{
    public class VCardRepository : IRepository<VCard>
    {
        private VCardContext Ctx;
        private static VCardRepository rep = null;


        private VCardRepository()
        {
            Ctx = new VCardContext();
        }

        public static VCardRepository getRepositorySingleton()
        {
            if (rep == null)
                rep = new VCardRepository();

            return rep;
        }

        IEnumerable<VCard> IRepository<VCard>.List
        {
            get
            {
                return Ctx.Cards.AsEnumerable<VCard>();
            }
        }

        void IRepository<VCard>.Add(VCard entity)
        {
            Ctx.Cards.Add(entity);
            Ctx.SaveChanges();
        }

        void IRepository<VCard>.Delete(VCard entity)
        {
            Ctx.Cards.Remove(entity);
            Ctx.SaveChanges();
        }

        void IRepository<VCard>.Update(VCard entity)
        {
            Ctx.SaveChanges();   
        }

        VCard IRepository<VCard>.FindById(uint Id)
        {
            var result = (from r in Ctx.Cards where r.Id == Id select r).FirstOrDefault();
            return result;  
        }
    }
}