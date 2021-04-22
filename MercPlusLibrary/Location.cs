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
    public class Location
    {
        [DataMember]
        public string LocCode { get; set; }
        [DataMember]
        public string RegionCode { get; set; }
        [DataMember]
        public string LocDesc { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string RkrpLoc { get; set; }
        [DataMember]
        public string LocGeoID { get; set; }
        [DataMember]
        public string ChangeUserLoc { get; set; }
        [DataMember]
        public string ContactEqsalSW { get; set; }
        [DataMember]
        public System.DateTime ChangeTimeLoc { get; set; }
        [DataMember]
        public string LocationNotFound { get; set; }
      
        [DataMember]
        public Country Country { get; set; }
        [DataMember]
        public Region Region { get; set; }
        [DataMember]
        public List<Shop> Shop { get; set; }
        //public virtual ICollection<SecUser> SEC_USER { get; set; }
    }
}
