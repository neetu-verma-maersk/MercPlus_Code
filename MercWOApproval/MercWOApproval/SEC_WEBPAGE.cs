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
    
    public partial class SEC_WEBPAGE
    {
        public int WEBPAGE_ID { get; set; }
        public int WEBSITE_ID { get; set; }
        public string WEBPAGE_NAME { get; set; }
    
        public virtual SEC_WEBSITE SEC_WEBSITE { get; set; }
    }
}