using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class SpecialRemarks
    {
        [DataMember]
        public int RemarksID { get; set; }
        [DataMember]
        public string RKEMProfile { get; set; }
        [DataMember]
        public string SerialNoFrom { get; set; }
        [DataMember]
        public string SerialNoTo { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public string DisplaySW { get; set; }
        [DataMember]
        public string Remarks { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public Nullable<decimal> RepairCeiling { get; set; }
    }
}
