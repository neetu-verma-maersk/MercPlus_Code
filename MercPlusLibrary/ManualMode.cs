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
    public class ManualMode
    {
        [DataMember]
        public string ModeCode { get; set; }
        [DataMember]
        public string ManualCode { get; set; }
        [DataMember]
        public string ActiveSw { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }

        [DataMember]
        public Manual Manual { get; set; }
        [DataMember]
        public Mode Mode { get; set; }
    }
}
