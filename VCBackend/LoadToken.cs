//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VCBackend
{
    using System;
    using System.Collections.Generic;
    
    public partial class LoadToken : LoadRequest
    {
        public Nullable<System.DateTime> DateInitial { get; set; }
        public Nullable<System.DateTime> DateFinal { get; set; }
    
        public virtual VCardToken VCardToken { get; private set; }
    }
}
