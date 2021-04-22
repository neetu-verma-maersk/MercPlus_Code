using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class Area
    {
        [DataMember]
        public string AreaCode { get; set; }
        [DataMember]
        public string AreaDescription { get; set; }
        [DataMember]
        public string AreaGeoID { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public string AreaNotFound { get; set; }

        [DataMember]
        public List<Country> Country { get; set; }
    }
}
