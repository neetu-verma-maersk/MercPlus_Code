//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MercWOApprovalDAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class MESC1TS_CPH_EQP_LIMIT
    {
        public string MODE { get; set; }
        public short AGE_FROM { get; set; }
        public Nullable<decimal> LIMIT_AMOUNT { get; set; }
        public string EQSIZE { get; set; }
        public short AGE_TO { get; set; }
        public string CHUSER { get; set; }
        public Nullable<System.DateTime> CHTS { get; set; }
    
        public virtual MESC1TS_MODE MESC1TS_MODE { get; set; }
    }
}