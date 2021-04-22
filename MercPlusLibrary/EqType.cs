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
    public class EqType
    {
        [DataMember]
        public string EqpType { get; set; }
        [DataMember]
        public string EqTypeDesc { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }

        [DataMember]
        public List<CountryLabor> CountryLabor { get; set; }
        [DataMember]
        public List<LaborRate> LaborRate { get; set; }
        //public virtual ICollection<WorkOrder> WorkOrder { get; set; }
    }
}
