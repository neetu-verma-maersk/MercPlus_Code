//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Merc_INLC
{
    using System;
    using System.Collections.Generic;
    
    public partial class MESC1TS_COUNTRY
    {
        public MESC1TS_COUNTRY()
        {
            this.MESC1TS_COUNTRY_CONT = new HashSet<MESC1TS_COUNTRY_CONT>();
            this.MESC1TS_COUNTRY_LABOR = new HashSet<MESC1TS_COUNTRY_LABOR>();
            this.MESC1TS_LOCATION = new HashSet<MESC1TS_LOCATION>();
        }
    
        public string COUNTRY_CD { get; set; }
        public string COUNTRY_DESC { get; set; }
        public string AREA_CD { get; set; }
        public Nullable<double> REPAIR_LIMIT_ADJ_FACTOR { get; set; }
        public string COUNTRY_GEO_ID { get; set; }
        public string CHUSER { get; set; }
        public System.DateTime CHTS { get; set; }
    
        public virtual MESC1TS_AREA MESC1TS_AREA { get; set; }
        public virtual ICollection<MESC1TS_COUNTRY_CONT> MESC1TS_COUNTRY_CONT { get; set; }
        public virtual ICollection<MESC1TS_COUNTRY_LABOR> MESC1TS_COUNTRY_LABOR { get; set; }
        public virtual ICollection<MESC1TS_LOCATION> MESC1TS_LOCATION { get; set; }
    }
}