using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;

namespace VCBackend.Services
{
    public abstract class IService
    {
        private readonly UnitOfWork unitofwork;
        private readonly Device authDevice;

        public UnitOfWork UnitOfWork
        {
            get
            {
                return unitofwork;
            }
        }

        public Device AuthDevice
        {
            get
            {
                return authDevice;
            }
        }

        public IService(UnitOfWork UnitOfWork, Device AuthDevice = null)
        {
            this.unitofwork = UnitOfWork;
            this.authDevice = AuthDevice;
        }

        bool Execute();
    }
}