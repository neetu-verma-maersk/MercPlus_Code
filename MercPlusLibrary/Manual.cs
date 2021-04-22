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
    public class Manual
    {
        [DataMember]
        public string ManualCode { get; set; }
        [DataMember]
        public string ManualDesc { get; set; }
        [DataMember]
        public string ManualActiveSW { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChTime { get; set; }
        [DataMember]
        public string ManualNotFound { get; set; }

        [DataMember]
        public List<Customer> Customer { get; set; }
        [DataMember]
        public List<ManualMode> ManualMode { get; set; }
        [DataMember]
        public List<RepairCode> RepairCode { get; set; }
        //public virtual ICollection<WorkOrder> WOrkOrder { get; set; }
    }
}
