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
    public class ModeEntry
    {
        [DataMember]
        public string EqType { get; set; }
        [DataMember]
        public string SubType { get; set; }
        [DataMember]
        public string Size { get; set; }
        [DataMember]
        public string Aluminum { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChTime { get; set; }
    }
}
