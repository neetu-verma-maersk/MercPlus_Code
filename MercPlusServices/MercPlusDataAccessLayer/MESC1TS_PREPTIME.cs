//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MercPlusDataAccessLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class MESC1TS_PREPTIME
    {
        public string MODE { get; set; }
        public string PREP_CD { get; set; }
        public double PREP_TIME_MAX { get; set; }
        public Nullable<double> PREP_HRS { get; set; }
        public string CHUSER { get; set; }
        public System.DateTime CHTS { get; set; }
    
        public virtual MESC1TS_MODE MESC1TS_MODE { get; set; }
    }
}