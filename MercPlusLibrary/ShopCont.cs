using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class ShopCont
    {
        [DataMember]
        public int ShopContID { get; set; }
        [DataMember]
        public string ShopCode { get; set; }
        [DataMember]
        public string Mode { get; set; }
        [DataMember]
        public string RepairCode { get; set; }
        [DataMember]
        public Nullable<decimal> ContractAmount { get; set; }
        [DataMember]
        public System.DateTime EffDate { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ExpDate { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public string ManualCode { get; set; }

        [DataMember]
        public Shop Shop { get; set; }
    }
}
