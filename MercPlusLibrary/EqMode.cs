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
    public class EqMode
    {
        [DataMember]
        public int EqModeID { get; set; }
        [DataMember]
        public string EqsType { get; set; }
        [DataMember]
        public string EqSize { get; set; }
        [DataMember]
        public string ModeCode { get; set; }
        [DataMember]
        public string CoType { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public string AluminiumSW { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }

        [DataMember]
        public Mode Mode { get; set; }
    }
}
