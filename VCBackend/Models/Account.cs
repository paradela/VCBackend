using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models
{
    public enum ValidationType {
        ONLINE = 1,
        TOKENIZED = 2
    }

    public partial class Account : IEntity
    {
        public ValidationType Type { get; set; }
        public virtual VCard Card { get; set; }
        public virtual ICollection<VCard> Tokens { get; set; }
        public float Balance { get; set; }

        public Account()
        {
            Type = ValidationType.TOKENIZED;
            Balance = 0.0F;
        }

    }
}