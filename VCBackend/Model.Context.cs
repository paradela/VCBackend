﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ModelContainer : DbContext
    {
        public ModelContainer()
            : base("name=ModelContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<User> UserSet { get; set; }
        public virtual DbSet<Account> AccountSet { get; set; }
        public virtual DbSet<Device> DeviceSet { get; set; }
        public virtual DbSet<VCard> VCardSet { get; set; }
        public virtual DbSet<VCardToken> VCardTokenSet { get; set; }
        public virtual DbSet<PaymentRequest> PaymentRequestSet { get; set; }
        public virtual DbSet<LoadRequest> LoadRequestSet { get; set; }
    }
}
