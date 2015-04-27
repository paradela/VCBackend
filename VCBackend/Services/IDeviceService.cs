using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Repositories;
using System.Text.RegularExpressions;

namespace VCBackend.Services
{
    public abstract class IDeviceService : IService
    {
        internal String deviceid;

        public String DeviceId
        {
            set
            {
                deviceid = value;
            }
        }

        public IDeviceService(UnitOfWork UnitOfWork, Device AuthDevice = null) : base(UnitOfWork, AuthDevice) { }

        internal bool ValidateDeviceData(String Name = null, String DevId = null)
        {
            Regex nameRgx = new Regex(@"[A-zÀ-ú-]{2}[A-zÀ-ú-\s]*");
            Regex idRgx = new Regex(@"[A-z1-9]{4}[A-z1-9]*");
            bool validName = (Name != null) ? nameRgx.IsMatch(Name) : true;
            bool validId = (DevId != null) ? idRgx.IsMatch(DevId) : true;
            return validName && validId;
        }
    }
}
