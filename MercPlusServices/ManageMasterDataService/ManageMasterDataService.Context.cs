﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ManageMasterDataService
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ManageMasterDataServiceEntities : DbContext
    {
        public ManageMasterDataServiceEntities()
            : base("name=ManageMasterDataServiceEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<dtproperty> dtproperties { get; set; }
        public DbSet<MESC1TS_AREA> MESC1TS_AREA { get; set; }
        public DbSet<MESC1TS_COUNTRY> MESC1TS_COUNTRY { get; set; }
        public DbSet<MESC1TS_COUNTRY_CONT> MESC1TS_COUNTRY_CONT { get; set; }
        public DbSet<MESC1TS_COUNTRY_LABOR> MESC1TS_COUNTRY_LABOR { get; set; }
        public DbSet<MESC1TS_CPH_EQP_LIMIT> MESC1TS_CPH_EQP_LIMIT { get; set; }
        public DbSet<MESC1TS_CURRENCY> MESC1TS_CURRENCY { get; set; }
        public DbSet<MESC1TS_CUST_SHOP_MODE> MESC1TS_CUST_SHOP_MODE { get; set; }
        public DbSet<MESC1TS_CUSTOMER> MESC1TS_CUSTOMER { get; set; }
        public DbSet<MESC1TS_DAMAGE> MESC1TS_DAMAGE { get; set; }
        public DbSet<MESC1TS_EDI_ERROR> MESC1TS_EDI_ERROR { get; set; }
        public DbSet<MESC1TS_EDI_LINEITEM> MESC1TS_EDI_LINEITEM { get; set; }
        public DbSet<MESC1TS_EDI_TRANSMISSION> MESC1TS_EDI_TRANSMISSION { get; set; }
        public DbSet<MESC1TS_EQMODE> MESC1TS_EQMODE { get; set; }
        public DbSet<MESC1TS_EQSTYPE> MESC1TS_EQSTYPE { get; set; }
        public DbSet<MESC1TS_EQTYPE> MESC1TS_EQTYPE { get; set; }
        public DbSet<MESC1TS_ERR_MESSAGE> MESC1TS_ERR_MESSAGE { get; set; }
        public DbSet<MESC1TS_EVENT_LOG> MESC1TS_EVENT_LOG { get; set; }
        public DbSet<MESC1TS_INDEX> MESC1TS_INDEX { get; set; }
        public DbSet<MESC1TS_INSPECTION> MESC1TS_INSPECTION { get; set; }
        public DbSet<MESC1TS_LABOR_RATE> MESC1TS_LABOR_RATE { get; set; }
        public DbSet<MESC1TS_LOCATION> MESC1TS_LOCATION { get; set; }
        public DbSet<MESC1TS_MANUAL> MESC1TS_MANUAL { get; set; }
        public DbSet<MESC1TS_MANUAL_MODE> MESC1TS_MANUAL_MODE { get; set; }
        public DbSet<MESC1TS_MANUFACTUR> MESC1TS_MANUFACTUR { get; set; }
        public DbSet<MESC1TS_MASTER_PART> MESC1TS_MASTER_PART { get; set; }
        public DbSet<MESC1TS_MODE> MESC1TS_MODE { get; set; }
        public DbSet<MESC1TS_MODEL> MESC1TS_MODEL { get; set; }
        public DbSet<MESC1TS_NONSCODE> MESC1TS_NONSCODE { get; set; }
        public DbSet<MESC1TS_PARTS_GROUP> MESC1TS_PARTS_GROUP { get; set; }
        public DbSet<MESC1TS_PAYAGENT> MESC1TS_PAYAGENT { get; set; }
        public DbSet<MESC1TS_PAYAGENT_VENDOR> MESC1TS_PAYAGENT_VENDOR { get; set; }
        public DbSet<MESC1TS_PREPTIME> MESC1TS_PREPTIME { get; set; }
        public DbSet<MESC1TS_PROCESS_IDENTIFIER> MESC1TS_PROCESS_IDENTIFIER { get; set; }
        public DbSet<MESC1TS_PTI> MESC1TS_PTI { get; set; }
        public DbSet<MESC1TS_REFAUDIT> MESC1TS_REFAUDIT { get; set; }
        public DbSet<MESC1TS_REGION> MESC1TS_REGION { get; set; }
        public DbSet<MESC1TS_REPAIR_CODE> MESC1TS_REPAIR_CODE { get; set; }
        public DbSet<MESC1TS_REPAIR_LOC> MESC1TS_REPAIR_LOC { get; set; }
        public DbSet<MESC1TS_RPRCODE_EXCLU> MESC1TS_RPRCODE_EXCLU { get; set; }
        public DbSet<MESC1TS_RPRCODE_IMPORT> MESC1TS_RPRCODE_IMPORT { get; set; }
        public DbSet<MESC1TS_RPRCODE_PART> MESC1TS_RPRCODE_PART { get; set; }
        public DbSet<MESC1TS_SHOP> MESC1TS_SHOP { get; set; }
        public DbSet<MESC1TS_SHOP_CONT> MESC1TS_SHOP_CONT { get; set; }
        public DbSet<MESC1TS_SHOP_LIMITS> MESC1TS_SHOP_LIMITS { get; set; }
        public DbSet<MESC1TS_SPECIAL_REMARKS> MESC1TS_SPECIAL_REMARKS { get; set; }
        public DbSet<MESC1TS_STATUS_CODE> MESC1TS_STATUS_CODE { get; set; }
        public DbSet<MESC1TS_SUSPEND> MESC1TS_SUSPEND { get; set; }
        public DbSet<MESC1TS_SUSPEND_CAT> MESC1TS_SUSPEND_CAT { get; set; }
        public DbSet<MESC1TS_TPI> MESC1TS_TPI { get; set; }
        public DbSet<MESC1TS_TRANSMIT> MESC1TS_TRANSMIT { get; set; }
        public DbSet<MESC1TS_VENDOR> MESC1TS_VENDOR { get; set; }
        public DbSet<MESC1TS_WOAUDIT> MESC1TS_WOAUDIT { get; set; }
        public DbSet<MESC1TS_WOPART> MESC1TS_WOPART { get; set; }
        public DbSet<MESC1TS_WOREMARK> MESC1TS_WOREMARK { get; set; }
        public DbSet<MESC1TS_WOREPAIR> MESC1TS_WOREPAIR { get; set; }
        public DbSet<MESC1TS_XML_LOG> MESC1TS_XML_LOG { get; set; }
        public DbSet<SEC_ACTION> SEC_ACTION { get; set; }
        public DbSet<SEC_AUTHGROUP> SEC_AUTHGROUP { get; set; }
        public DbSet<SEC_AUTHGROUP_ACTION> SEC_AUTHGROUP_ACTION { get; set; }
        public DbSet<SEC_AUTHGROUP_FUNCTION> SEC_AUTHGROUP_FUNCTION { get; set; }
        public DbSet<SEC_AUTHGROUP_USER> SEC_AUTHGROUP_USER { get; set; }
        public DbSet<SEC_AUTHGROUP_WEBPAGE> SEC_AUTHGROUP_WEBPAGE { get; set; }
        public DbSet<SEC_FUNCTION> SEC_FUNCTION { get; set; }
        public DbSet<SEC_USER> SEC_USER { get; set; }
        public DbSet<SEC_WEBPAGE> SEC_WEBPAGE { get; set; }
        public DbSet<SEC_WEBSITE> SEC_WEBSITE { get; set; }
        public DbSet<TESTEQUIPNO> TESTEQUIPNOes { get; set; }
        public DbSet<MESC1VS_CUST_SHOP> MESC1VS_CUST_SHOP { get; set; }
        public DbSet<MESC1VS_SHOP_LOCATION> MESC1VS_SHOP_LOCATION { get; set; }
        public DbSet<MESC1TS_DISCOUNT> MESC1TS_DISCOUNT { get; set; }
        public DbSet<MESC1TS_GRADE> MESC1TS_GRADE { get; set; }
        public DbSet<MESC1TS_GRADECONTAINER> MESC1TS_GRADECONTAINER { get; set; }
        public DbSet<MESC1TS_GRADERELATION> MESC1TS_GRADERELATION { get; set; }
        public DbSet<MESC1TS_GRADESTS> MESC1TS_GRADESTS { get; set; }
        public DbSet<MESC1TS_WO> MESC1TS_WO { get; set; }
    }
}
