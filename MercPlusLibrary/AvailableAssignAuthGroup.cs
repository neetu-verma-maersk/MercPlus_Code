using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MercPlusLibrary
{
    [DataContract]
    public class AvailableAssignAuthGroup
    {
        [DataMember]
        public string ValueItem { get; set; }
        [DataMember]
        public string ListItem { get; set; }
    }
}
