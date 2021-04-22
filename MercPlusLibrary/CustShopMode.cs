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
    public class CustShopMode
    {
        [DataMember]
        public string CSMCode { get; set; }
        [DataMember]
        public string ModeCode { get; set; }
        [DataMember]
        public string RRISFormat { get; set; }
        [DataMember]
        public string ShopCode { get; set; }
        [DataMember]
        public string ProfitCenter { get; set; }
        [DataMember]
        public string PayAgentCode { get; set; }
        [DataMember]
        public string SubProfitCenter { get; set; }
        [DataMember]
        public string AccountCode { get; set; }
        [DataMember]
        public string CustomerCode { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public string CorpPayAgentCode { get; set; }

        [DataMember]
        public Customer Customer { get; set; }
        [DataMember]
        public PayAgent PayAgent { get; set; }
        [DataMember]
        public Shop Shop { get; set; }
        [DataMember]
        public Mode Mode { get; set; }
    }
}
