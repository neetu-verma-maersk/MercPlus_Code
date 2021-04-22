using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class PayAgentVendor
    {
        
        public string PayAgentCd { get; set; }
        
        public string VendorCd { get; set; }
        
        public string LocalAccountCd { get; set; }
        
        public string SupplierCd { get; set; }
        
        public string PaymentMethod { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual PayAgent PayAgent { get; set; }
        //public virtual Vendor Vendor { get; set; }
    }
}
