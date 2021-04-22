using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public partial class ProcessIdentifier
    {
        [DataMember]
        public int ProcessID { get; set; }
        [DataMember]
        public string ProcessDesc { get; set; }
        [DataMember]
        public string ParamName { get; set; }
        [DataMember]
        public string ParamValue { get; set; }
    }
}
