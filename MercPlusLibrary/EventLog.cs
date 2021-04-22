using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MercPlusServiceLibrary
{
    [DataContract]
    public partial class EventLog
    {
        [DataMember]
        public int EventID { get; set; }
        [DataMember]
        public string Eventname { get; set; }
        [DataMember]
        public string UniqueID { get; set; }
        [DataMember]
        public string TableName { get; set; }
        [DataMember]
        public string EventDesc { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ChangeTime { get; set; }
    }
}
