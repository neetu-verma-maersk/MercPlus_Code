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
    public class EDITransmission
    {
        [DataMember]
        public int EDIId { get; set; }
        [DataMember]
        public Nullable<System.DateTime> Crts { get; set; }
        [DataMember]
        public Nullable<int> WOQty { get; set; }
        [DataMember]
        public Nullable<int> WOPassQty { get; set; }
        [DataMember]
        public Nullable<int> WOFailQty { get; set; }
    }
}
