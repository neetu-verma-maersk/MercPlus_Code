
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;
namespace MercPlusLibrary
{
 public   class PTIPeriod
    {
        [DataMember]
        public string EqpNoFrom { get; set; }
        [DataMember]
        public string EqpNoTo { get; set; }
        [DataMember]
        public Int64 ExceptionDays { get; set; }
        [DataMember]
        public System.DateTime PTIChTime { get; set; }
        [DataMember]
        public string PTIChUser { get; set; }
    }
}
