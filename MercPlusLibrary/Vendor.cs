using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class Vendor
    {
        [DataMember]
        public string VendorCode { get; set; }
        [DataMember]
        public string VendorDesc { get; set; }
        [DataMember]
        public string VendorActiveSw { get; set; }
        [DataMember]
        public string VenCountryCode { get; set; }
        [DataMember]
        public Nullable<double> DIscountPCT { get; set; }
        [DataMember]
        public Nullable<short> DiscountDays { get; set; }
        [DataMember]
        public string ChangeUserVendor { get; set; }
        [DataMember]
        public System.DateTime ChangeTimeVendor { get; set; }
        [DataMember]
        public string CountryCode { get; set; }

        [DataMember]
        public List<PayAgentVendor> PayAgentVendor { get; set; }
        [DataMember]
        public List<Shop> Shop { get; set; }
        //[DataMember]
        //public List<MESC1TS_WO> MESC1TS_WO { get; set; }
    }
}
