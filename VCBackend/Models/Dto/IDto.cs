using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public interface IDto <TEntity> where TEntity : class
    {
        void Serialize(TEntity entity);
    }
}