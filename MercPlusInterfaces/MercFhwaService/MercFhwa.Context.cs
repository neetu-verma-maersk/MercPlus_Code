//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MercFhwaService
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
    
        public DbSet<MESC1TS_EVENT_LOG> MESC1TS_EVENT_LOG { get; set; }
        public DbSet<MESC1TS_INSPECTION> MESC1TS_INSPECTION { get; set; }
    }
}
