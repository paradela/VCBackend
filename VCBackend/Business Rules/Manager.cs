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
    public abstract class Manager
    {
        internal UnitOfWork UnitOfWork;

        public Manager(UnitOfWork UnitOfWork)
        {
            this.UnitOfWork = UnitOfWork;
        }
    }
}