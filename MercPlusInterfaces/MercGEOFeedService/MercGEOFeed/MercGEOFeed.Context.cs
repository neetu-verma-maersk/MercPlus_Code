﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MercGEOFeed
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MESC2DSEntities : DbContext
    {
        public MESC2DSEntities()
            : base("name=MESC2DSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<MESC1TS_AREA> MESC1TS_AREA { get; set; }
        public DbSet<MESC1TS_COUNTRY> MESC1TS_COUNTRY { get; set; }
        public DbSet<MESC1TS_LOCATION> MESC1TS_LOCATION { get; set; }
        public DbSet<MESC1TS_REFAUDIT> MESC1TS_REFAUDIT { get; set; }
        public DbSet<MESC1TS_SHOP> MESC1TS_SHOP { get; set; }
    }
}