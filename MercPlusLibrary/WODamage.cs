using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MercPlusLibrary
{
    [DataContract]
    class WODamage
    {
        //RKEM-DamageCode change
        [DataMember]
        public int WorkOrderID { get; set; }
        [DataMember]
        public string EquipmentNo { get; set; }
        [DataMember]
        public string Mode { get; set; }
        [DataMember]
        public string ShopCode { get; set; }
        [DataMember]
        public string ShopLocationCode { get; set; }
        [DataMember]
        public string CurrentMove { get; set; }
        [DataMember]
        public string CurrentLocation { get; set; }
        [DataMember]
        public string MercDamage { get; set; }
        [DataMember]
        public string RKEMDamage { get; set; }
        [DataMember]
        public int RKEMStatus { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
    }

}
