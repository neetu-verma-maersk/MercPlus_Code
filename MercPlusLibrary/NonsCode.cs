using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class NonsCode
    {
        [DataMember]
        public string ModeCode { get; set; }
        [DataMember]
        public string NonsCd { get; set; }
        [DataMember]
        public string NonsDesc { get; set; }
        [DataMember]
        public string NonsActiveSW { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }

        [DataMember]
        public Mode Mode { get; set; }
    }
}
