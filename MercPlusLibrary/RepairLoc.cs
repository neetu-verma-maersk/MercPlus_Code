using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class RepairLoc
    {
        [DataMember]
        public string CedexCode { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
