//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RKRP_Service
{
    using System;
    using System.Collections.Generic;
    
    public partial class MESC1TS_SUSPEND
    {
        public string SHOP_CD { get; set; }
        public string REPAIR_CD { get; set; }
        public string MODE { get; set; }
        public string MANUAL_CD { get; set; }
        public int SUSPCAT_ID { get; set; }
        public string CHUSER { get; set; }
        public System.DateTime CHTS { get; set; }
    
        public virtual MESC1TS_SHOP MESC1TS_SHOP { get; set; }
        public virtual MESC1TS_SUSPEND_CAT MESC1TS_SUSPEND_CAT { get; set; }
    }
}
