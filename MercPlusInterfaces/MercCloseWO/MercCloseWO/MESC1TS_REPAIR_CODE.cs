//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MercCloseWO
{
    using System;
    using System.Collections.Generic;
    
    public partial class MESC1TS_REPAIR_CODE
    {
        public string REPAIR_CD { get; set; }
        public string MODE { get; set; }
        public string MANUAL_CD { get; set; }
        public string REPAIR_DESC { get; set; }
        public string RKRP_REPAIR_CD { get; set; }
        public string CHUSER { get; set; }
        public Nullable<short> MAX_QUANTITY { get; set; }
        public Nullable<decimal> SHOP_MATERIAL_CEILING { get; set; }
        public System.DateTime CHTS { get; set; }
        public string REPAIR_IND { get; set; }
        public Nullable<double> MAN_HOUR { get; set; }
        public string REPAIR_ACTIVE_SW { get; set; }
        public string MULTIPLE_UPDATE_SW { get; set; }
        public Nullable<double> WARRANTY_PERIOD { get; set; }
        public string TAX_APPLIED_SW { get; set; }
        public Nullable<short> REPAIR_PRIORITY { get; set; }
        public Nullable<int> INDEX_ID { get; set; }
        public string ALLOW_PARTS_SW { get; set; }
    
        public virtual MESC1TS_MANUAL MESC1TS_MANUAL { get; set; }
        public virtual MESC1TS_MODE MESC1TS_MODE { get; set; }
    }
}
