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
    public class EDIError
    {
        [DataMember]
        public int EDIId { get; set; }
        [DataMember]
        public Nullable<int> LineNo { get; set; }
        [DataMember]
        public string LineDetail { get; set; }
    }
}
