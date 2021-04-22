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
    public class CphEqpLimit
    {
        [DataMember]
        public string ModeCode { get; set; }
        [DataMember]
        public short AgeFrom{ get; set; }
        [DataMember]
        public Nullable<decimal> LimitAmount { get; set; }
        [DataMember]
        public string EqSize { get; set; }
        [DataMember]
        public short AgeTo { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChTime { get; set; }

        [DataMember]
        public Mode Mode { get; set; }
    }
}
