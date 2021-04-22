using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class RefAudit
    {
        [DataMember]
        public int AuditID { get; set; }
        [DataMember]
        public string TabName { get; set; }
        [DataMember]
        public string UniqueID { get; set; }
        [DataMember]
        public string ColName { get; set; }
        [DataMember]
        public string OldValue { get; set; }
        [DataMember]
        public string NewValue { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ChangeTime { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string FirstName { get; set; }
    }
}
