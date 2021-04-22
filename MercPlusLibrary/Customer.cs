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
    public class Customer
    {
        [DataMember]
        public string CustomerCode { get; set; }
        [DataMember]
        public string CustomerDesc { get; set; }
        [DataMember]
        public string ManualCode { get; set; }
        [DataMember]
        public string MaerskSw { get; set; }
        [DataMember]
        public string CustomerActiveSw { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public string CustomerNotFound { get; set; }

        [DataMember]
        public List<CustShopMode> CustShopMode { get; set; }
        [DataMember]
        public Manual Manual { get; set; }
        [DataMember]
        public List<LaborRate> LaborRate { get; set; }
        //public virtual ICollection<Transmit> Transmit { get; set; }
        //public virtual ICollection<WorkOrder> WorkOrder { get; set; }
    }
}
