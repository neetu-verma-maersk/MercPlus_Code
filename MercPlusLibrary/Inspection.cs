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
    public class Inspection
    {
        [DataMember]
        public System.DateTime InspDte { get; set; }
        [DataMember]
        public string S_InspDate { get; set; }
        [DataMember]
        public string NextInspDate { get; set; }
        [DataMember]
        public string ChasEqpNo { get; set; }
        [DataMember]
        public string InspBy { get; set; }
        [DataMember]
        public string RKEMLoc { get; set; }
        [DataMember]
        public string XmitRc { get; set; }
        [DataMember]
        public Nullable<System.DateTime> XmitDate { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ChangeTime { get; set; }
    }
}
