using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class Model
    {
        [DataMember]
        public string ModelNo { get; set; }
        [DataMember]
        public string IndicatorCd { get; set; }
        [DataMember]
        public string ManufacturCd { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }

        [DataMember]
        public Manufactur Manufactur { get; set; }
    }
}
