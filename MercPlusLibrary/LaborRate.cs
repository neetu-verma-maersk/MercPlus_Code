using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class LaborRate
    {
        [DataMember]
        public int LaborRateID { get; set; }
        [DataMember]
        public string EqpType { get; set; }
        [DataMember]
        public System.DateTime EffDate { get; set; }
        [DataMember]
        public System.DateTime ExpDate { get; set; }
        [DataMember]
        public string ShopCode { get; set; }
        [DataMember]
        public string CustomerCode { get; set; }
        [DataMember]
        public Nullable<decimal> RegularRT { get; set; }
        [DataMember]
        public Nullable<decimal> RegularRTCPH { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public Nullable<decimal> OvertimeRT { get; set; }
        [DataMember]
        public Nullable<decimal> DoubleTimeRT { get; set; }
        [DataMember]
        public Nullable<decimal> MiscRT { get; set; }
        [DataMember]
        public decimal? MiscRTCPH { get; set; }
        [DataMember]
        public decimal? OvertimeRTCPH { get; set; }
        [DataMember]
        public decimal? DoubleTimeRTCPH { get; set; }

        [DataMember]
        public Customer Customer { get; set; }
        [DataMember]
        public EqType EqType { get; set; }
        [DataMember]
        public Shop Shop { get; set; }
    }
}
