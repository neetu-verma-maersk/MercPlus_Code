//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MercFACTUpload
{
    using System;
    using System.Collections.Generic;
    
    public partial class MESC1TS_CURRENCY
    {
        public MESC1TS_CURRENCY()
        {
            this.MESC1TS_COUNTRY_CONT = new HashSet<MESC1TS_COUNTRY_CONT>();
            this.MESC1TS_COUNTRY_LABOR = new HashSet<MESC1TS_COUNTRY_LABOR>();
            this.MESC1TS_SHOP = new HashSet<MESC1TS_SHOP>();
            this.MESC1TS_WO = new HashSet<MESC1TS_WO>();
        }
    
        public string CURCD { get; set; }
        public string CUCDN { get; set; }
        public string CURRNAMC { get; set; }
        public Nullable<decimal> EXRATDKK { get; set; }
        public Nullable<decimal> EXRATUSD { get; set; }
        public Nullable<decimal> EXRATYEN { get; set; }
        public string CHUSER { get; set; }
        public System.DateTime CHTS { get; set; }
        public Nullable<decimal> EXRATEUR { get; set; }
        public Nullable<System.DateTime> QUOTEDAT { get; set; }
    
        public virtual ICollection<MESC1TS_COUNTRY_CONT> MESC1TS_COUNTRY_CONT { get; set; }
        public virtual ICollection<MESC1TS_COUNTRY_LABOR> MESC1TS_COUNTRY_LABOR { get; set; }
        public virtual ICollection<MESC1TS_SHOP> MESC1TS_SHOP { get; set; }
        public virtual ICollection<MESC1TS_WO> MESC1TS_WO { get; set; }
    }
}