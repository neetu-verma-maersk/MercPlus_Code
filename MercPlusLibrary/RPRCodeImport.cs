using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class RPRCodeImport
    {
        [DataMember]
        public string RepairCode { get; set; }
        [DataMember]
        public string ExcluRepairCode { get; set; }
        [DataMember]
        public string Mode { get; set; }
        [DataMember]
        public string ManualCode { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
    }
}
