using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class Region
    {
        [DataMember]
        public string RegionCd { get; set; }
        [DataMember]
        public string RegionDesc { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }

        [DataMember]
        public List<Location> Location { get; set; }
    }
}
