using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class PrepTime
    {
        [DataMember]
        public string ModeCode { get; set; }
        [DataMember]
        public string PrepCd { get; set; }
        [DataMember]
        public double PrepTimeMax { get; set; }
        [DataMember]
        public Nullable<double> PrepHrs { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }

          [DataMember]
        public string ChangeTime_Display { get; set; }

        [DataMember]
        public Mode Mode { get; set; }
    }
}
