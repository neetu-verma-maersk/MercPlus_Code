using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MercPlusLibrary
{
    [DataContract]
    public class Country
    {
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string CountryDescription { get; set; }
        [DataMember]
        public string AreaCode { get; set; }
        [DataMember]
        public Nullable<double> RepairLimitAdjFactor { get; set; }
        [DataMember]
        public string CountryGeoID { get; set; }
        [DataMember]
        public string ChangeUserCon { get; set; }
        [DataMember]
        public System.DateTime ChangeTimeCon { get; set; }
        [DataMember]
        public string CountryNotFound { get; set; }
        [DataMember]
        public ErrMessage ErrorMessage { get; set; }
        [DataMember]
        public bool ShowDiv { get; set; }

        [DataMember]
        public string CountryCodeAndDescription { get; set; }
    }
}
