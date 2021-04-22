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
    public class RepairLocationCode
    {
        [DataMember]
        public string RepairCod { get; set; }
        [DataMember]
        public string RepairDescription { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChTime { get; set; }
    }
}
