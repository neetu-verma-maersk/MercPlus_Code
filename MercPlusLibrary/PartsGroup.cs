using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class PartsGroup
    {
        [DataMember]
        public string PartsGroupCd { get; set; }
        [DataMember]
        public string PartsGroupDesc { get; set; }
        [DataMember]
        public string Remarks { get; set; }
        [DataMember]
        public string PartsGroupActiveSW { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public string ChangeTime { get; set; }

        [DataMember]
        public List<MasterPart> MasterPart { get; set; }

        [DataMember]
        public bool IsPartsGroupCodeExists { get; set; }
        [DataMember]
        public bool IsPartsGroupAddUpdateSuccess { get; set; }
    }
}
