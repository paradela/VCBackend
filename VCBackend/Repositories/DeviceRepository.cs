using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;

namespace VCBackend.Repositories
{
    public class DeviceRepository : IRepository<Device>
    {
        private static DeviceRepository rep = null;
        private ModelContainer Ctx;

        private DeviceRepository()
        {
            Ctx = new ModelContainer();
        }

        public static IRepository<Device> getRepositorySingleton()
        {
            if (rep == null)
            {
                rep = new DeviceRepository();
            }
            return rep;
        }

        IEnumerable<Device> IRepository<Device>.List
        {
            get
            {
                return Ctx.DeviceSet.AsEnumerable<Device>();
            }
        }

        void IRepository<Device>.Add(Device entity)
        {
            Ctx.DeviceSet.Add(entity);
            Ctx.SaveChanges();
        }

        void IRepository<Device>.Delete(Device entity)
        {
            Ctx.DeviceSet.Remove(entity);
            Ctx.SaveChanges();
        }

        void IRepository<Device>.Update(Device entity)
        {
            Ctx.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            Ctx.SaveChanges();   
        }

        Device IRepository<Device>.FindById(int Id)
        {
            var result = (from r in Ctx.DeviceSet where r.Id == Id select r).FirstOrDefault();
            return result;   
        }
    }
}