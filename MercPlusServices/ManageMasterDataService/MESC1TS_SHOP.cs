//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class MESC1TS_SHOP
    {
        public MESC1TS_SHOP()
        {
            this.MESC1TS_CUST_SHOP_MODE = new HashSet<MESC1TS_CUST_SHOP_MODE>();
            this.MESC1TS_LABOR_RATE = new HashSet<MESC1TS_LABOR_RATE>();
            this.MESC1TS_SHOP_CONT = new HashSet<MESC1TS_SHOP_CONT>();
            this.MESC1TS_SHOP_LIMITS = new HashSet<MESC1TS_SHOP_LIMITS>();
            this.MESC1TS_SUSPEND = new HashSet<MESC1TS_SUSPEND>();
            this.MESC1TS_WO = new HashSet<MESC1TS_WO>();
        }
    
        public string SHOP_CD { get; set; }
        public string VENDOR_CD { get; set; }
        public string SHOP_DESC { get; set; }
        public string RKRPLOC { get; set; }
        public string LOC_CD { get; set; }
        public string CHUSER { get; set; }
        public System.DateTime CHTS { get; set; }
        public string EMAIL_ADR { get; set; }
        public string PHONE { get; set; }
        public string EDI_PARTNER { get; set; }
        public string RRIS70_SUFFIX_CD { get; set; }
        public string PREPTIME_SW { get; set; }
        public string SHOP_ACTIVE_SW { get; set; }
        public Nullable<double> PCT_MATERIAL_FACTOR { get; set; }
        public string RRIS_XMIT_SW { get; set; }
        public string OVERTIME_SUSP_SW { get; set; }
        public Nullable<double> IMPORT_TAX { get; set; }
        public string SHOP_TYPE_CD { get; set; }
        public Nullable<double> SALES_TAX_PART_CONT { get; set; }
        public Nullable<double> SALES_TAX_PART_GEN { get; set; }
        public Nullable<double> SALES_TAX_LABOR_CON { get; set; }
        public string CUCDN { get; set; }
        public Nullable<double> SALES_TAX_LABOR_GEN { get; set; }
        public string ACEP_SW { get; set; }
        public string DECENTRALIZED { get; set; }
        public string AUTO_COMPLETE_SW { get; set; }
        public string BYPASS_LEASE_RULES { get; set; }
    
        public virtual MESC1TS_CURRENCY MESC1TS_CURRENCY { get; set; }
        public virtual ICollection<MESC1TS_CUST_SHOP_MODE> MESC1TS_CUST_SHOP_MODE { get; set; }
        public virtual ICollection<MESC1TS_LABOR_RATE> MESC1TS_LABOR_RATE { get; set; }
        public virtual MESC1TS_LOCATION MESC1TS_LOCATION { get; set; }
        public virtual ICollection<MESC1TS_SHOP_CONT> MESC1TS_SHOP_CONT { get; set; }
        public virtual ICollection<MESC1TS_SHOP_LIMITS> MESC1TS_SHOP_LIMITS { get; set; }
        public virtual ICollection<MESC1TS_SUSPEND> MESC1TS_SUSPEND { get; set; }
        public virtual MESC1TS_VENDOR MESC1TS_VENDOR { get; set; }
        public virtual ICollection<MESC1TS_WO> MESC1TS_WO { get; set; }
    }
}