//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ManageAATService
{
    using System;
    using System.Collections.Generic;
    
    public partial class MESC1TS_MODE
    {
        public MESC1TS_MODE()
        {
            this.MESC1TS_WO = new HashSet<MESC1TS_WO>();
        }
    
        public string MODE { get; set; }
        public string MODE_DESC { get; set; }
        public string STANDARD_TIME_SW { get; set; }
        public string VALIDATION_SW { get; set; }
        public string MODE_ACTIVE_SW { get; set; }
        public string CHUSER { get; set; }
        public string MODE_IND { get; set; }
        public System.DateTime CHTS { get; set; }
    
        public virtual ICollection<MESC1TS_WO> MESC1TS_WO { get; set; }
    }
}
