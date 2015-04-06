using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;

namespace VCBackend.Repositories
{
    public class DeviceRepository : IRepository<Device>
    {
        VCardContext Ctx;

        public DeviceRepository()
        {
            Ctx = new VCardContext();
        }

        IEnumerable<Device> IRepository<Device>.List
        {
            get
            {
                return Ctx.Devices.AsEnumerable<Device>();
            }
        }

        void IRepository<Device>.Add(Device entity)
        {
            Ctx.Devices.Add(entity);
            Ctx.SaveChanges();
        }

        void IRepository<Device>.Delete(Device entity)
        {
            Ctx.Devices.Remove(entity);
            Ctx.SaveChanges();
        }

        void IRepository<Device>.Update(Device entity)
        {
            Ctx.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            Ctx.SaveChanges();   
        }

        Device IRepository<Device>.FindById(int Id)
        {
            var result = (from r in Ctx.Devices where r.Id == Id select r).FirstOrDefault();
            return result;   
        }


        ICollection<Device> IRepository<Device>.ExecuteSQL(string SQL)
        {
            throw new NotImplementedException();
        }
    }
}