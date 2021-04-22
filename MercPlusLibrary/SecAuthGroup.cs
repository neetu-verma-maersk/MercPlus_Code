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
    public class SecAuthGroup
    {
        [DataMember]
        public Int32 AuthGroupId { get; set; }
        [DataMember]
        public string AuthGroupName { get; set; }
        [DataMember]
        public Int32 ParentAuthGroupId { get; set; }
        [DataMember]
        public string TableName { get; set; }
        [DataMember]
        public string ColumnName { get; set; }
        [DataMember]
        public string ColumnDesc { get; set; }
        [DataMember]
        public string ReadOnlySw { get; set; }    
    }
}
