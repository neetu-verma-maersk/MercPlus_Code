//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ManageWorkOrderService
{
    using System;
    using System.Collections.Generic;
    
    public partial class MESC1TS_TRANSMIT
    {
        public string CUSTOMER_CD { get; set; }
        public string MODE { get; set; }
        public string RRIS_XMIT_SW { get; set; }
        public string RKRP_XMIT_SW { get; set; }
        public string ACCOUNT_CD { get; set; }
        public string CHUSER { get; set; }
        public System.DateTime CHTS { get; set; }
    
        public virtual MESC1TS_CUSTOMER MESC1TS_CUSTOMER { get; set; }
        public virtual MESC1TS_MODE MESC1TS_MODE { get; set; }
    }
}
