using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
     [DataContract]
    public class RepairCodePart
    {

        [DataMember]
        public string PartCd { get; set; }
        [DataMember]
        public string RepairCod { get; set; }
        [DataMember]
        public string ModeCode { get; set; }
        [DataMember]
        public string ManualCode { get; set; }
        [DataMember]
        public string RepairDesc { get; set; }
        [DataMember]
        public string PartNumber { get; set; }

        [DataMember]
        public string OrgRepairCod { get; set; }
        [DataMember]
        public string OrgModeCode { get; set; }
        [DataMember]
        public string OrgManualCode { get; set; }
        [DataMember]
        public string OrgPartNumber { get; set; }

        [DataMember]
        public string PartDesc { get; set; }
        [DataMember]
        public string MaxPartQty { get; set; }
        [DataMember]
        public string OrgMaxPartQty { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public string ChangeTime { get; set; }


    }
}
