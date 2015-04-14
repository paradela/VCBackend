using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace VCBackend
{
    public partial class Device : IEntity
    {
        abstract public bool IsDefault();
    }
}
