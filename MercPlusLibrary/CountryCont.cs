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
    public class CountryCont
    {
        [DataMember]
        public int CountryContID { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public System.DateTime EffectiveDate { get; set; }
        [DataMember]
        public string Mode { get; set; }
        [DataMember]
        public System.DateTime ExpiryDate { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public decimal ContractAmount { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public string RepairCod { get; set; }
        [DataMember]
        public string CUCDN { get; set; }
        [DataMember]
        public string ManualCode { get; set; }

        [DataMember]
        public Country Country { get; set; }
        [DataMember]
        public Currency Currency { get; set; }
    }
}
