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
    public partial class PayAgent
    {
        [DataMember]
        public string PayAgentCode { get; set; }
        [DataMember]
        public string CorpPayAgentCode { get; set; }
        [DataMember]
        public string RRISFormat { get; set; }
        [DataMember]
        public string ProfitCenter { get; set; }
        [DataMember]
        public string SubProfitCenter { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }

        [DataMember]
        public List<CustShopMode> CustShopMode { get; set; }
        [DataMember]
        public List<PayAgentVendor> PayAgentVendor { get; set; }
    }
}
