using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class PayAgentVendor
    {
        [DataMember]
        public string PayAgentCode { get; set; }
        [DataMember]
        public string VendorCode { get; set; }
        [DataMember]
        public string LocalAccountCode { get; set; }
        [DataMember]
        public string SupplierCode { get; set; }
        [DataMember]
        public string PaymentMethod { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }

        [DataMember]
        public PayAgent PayAgent { get; set; }
        //public virtual Vendor Vendor { get; set; }
    }
}
