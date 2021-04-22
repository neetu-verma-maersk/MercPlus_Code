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
    public class Index
    {
        [DataMember]
        public int IndexID { get; set; }
        [DataMember]
        public string Mode { get; set; }
        [DataMember]
        public string ManualCode { get; set; }
        [DataMember]
        public string IndexDesc { get; set; }
        [DataMember]
        public Nullable<short> IndexPriority { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
    }
}
