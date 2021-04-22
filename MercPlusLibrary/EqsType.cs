using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class EqsType
    {
        [DataMember]
        public string EqSType { get; set; }
        [DataMember]
        public string CoType { get; set; }
        [DataMember]
        public string TypeDesc { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
    }
}
