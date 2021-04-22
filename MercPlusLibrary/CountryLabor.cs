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
    public class CountryLabor
    {
        [DataMember]
        public int CountryLaborID { get; set; }
        [DataMember]
        public string EqType { get; set; }
        [DataMember]
        public System.DateTime ExpDate { get; set; }
        [DataMember]
        public System.DateTime EffDate { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public Nullable<decimal> RegularRT { get; set; }
        [DataMember]
        public Nullable<decimal> OvertimeRT { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public Nullable<decimal> DoubleTimeRT { get; set; }
        [DataMember]
        public Nullable<decimal> MiscRT { get; set; }
        [DataMember]
        public string Cucdn { get; set; }

        [DataMember]
        public Country Country { get; set; }
        [DataMember]
        public Currency Currency { get; set; }
        [DataMember]
        public EqType EqpType { get; set; }
    }
}
