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
    public class Damage
    {
        [DataMember]
        public string DamageCedexCode { get; set; }
        [DataMember]
        public string DamageName { get; set; }
        [DataMember]
        public string DamageDescription { get; set; }
        [DataMember]
        public string DamageNumericalCode { get; set; }
        [DataMember]
        public string DamageFullDescription { get; set; }
        [DataMember]
        public string DamageCodeNotFound { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }

    }
}
