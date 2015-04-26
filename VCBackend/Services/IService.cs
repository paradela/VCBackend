using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;

namespace VCBackend.Services
{
    public abstract class IService
    {
        internal readonly UnitOfWork unitofwork;

        public UnitOfWork UnitOfWork
        {
            get
            {
                return unitofwork;
            }
        }

        public IService(UnitOfWork UnitOfWork)
        {
            this.unitofwork = UnitOfWork;
        }

        void doExecute();
    }
}