using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;

namespace VCBackend.Services
{
    public abstract class IService
    {

        public UnitOfWork UnitOfWork
        {
            get;
            private set;
        }

        public Device AuthDevice
        {
            get;
            private set;
        }

        public IService(UnitOfWork UnitOfWork, Device AuthDevice = null)
        {
            this.UnitOfWork = UnitOfWork;
            this.AuthDevice = AuthDevice;
        }


        public bool Execute()
        {
            bool result = false;

            using (var trx = UnitOfWork.TransactionBegin())
            {
                try
                {
                    result = ExecuteService();
                    trx.Commit();
                }
                catch (Exception ex)
                {
                    trx.Rollback();
                    throw ex;
                }
            }

            return result;
        }


        protected abstract bool ExecuteService();
    }
}