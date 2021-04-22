using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class Transmit
    {
        [DataMember]
        public string CustomerCode { get; set; }
        [DataMember]
        public string ModeCode { get; set; }
        [DataMember]
        public string RRISXMITSwitch { get; set; }
        [DataMember]
        public string RKRPXMITSwitch { get; set; }
        [DataMember]
        public string AccountCode { get; set; }
        [DataMember]
        public string ChangeUserTransmit { get; set; }
        [DataMember]
        public System.DateTime ChangeTimeTransmit { get; set; }

        [DataMember]
        public virtual Customer Customer { get; set; }
        [DataMember]
        public virtual Mode Mode { get; set; }
    }
}
