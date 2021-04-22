using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class ShopLimits
    {
        [DataMember]
        public string ShopCode { get; set; }
        [DataMember]
        public string Mode { get; set; }
        [DataMember]
        public string ModeDesc { get; set; } 
        [DataMember]
        public Nullable<decimal> RepairAmtLimit { get; set; }
        [DataMember]
        public Nullable<decimal> AutoApproveLimit { get; set; }
        [DataMember]
        public Nullable<decimal> ShopMaterialLimit { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public string FName { get; set; }
        [DataMember]
        public string LName { get; set; }

        [DataMember]
        public virtual Shop Shop { get; set; }
    }
}
