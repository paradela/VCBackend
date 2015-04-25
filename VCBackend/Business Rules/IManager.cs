using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;

namespace VCBackend.Business_Rules
{
    /// <summary>
    /// Abstract Manager which all manager's should extend. 
    /// </summary>
    public abstract class IManager
    {
        internal UnitOfWork UnitOfWork;

        public IManager(UnitOfWork UnitOfWork)
        {
            this.UnitOfWork = UnitOfWork;
        }
    }
}