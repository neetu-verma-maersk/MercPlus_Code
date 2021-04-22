using System;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class GradeContainer
    {
        [DataMember]
        public int GradeId { get; set; }

        [DataMember]
        public string GradeCode { get; set; }

        [DataMember]
        public string GradeDescription { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime? CreatedOn { get; set; }

        [DataMember]
        public string ModifiedBy { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }


        [DataMember]
        public string EQPNO { get; set; }

        [DataMember]
        public Nullable<int> WO_ID { get; set; }

        [DataMember]
        public string CURRENTLOC { get; set; }


        [DataMember]
        public string GRADECODE { get; set; }

        [DataMember]
        public string GRADECODE_NEW { get; set; }

        [DataMember]
        public string MODIFIEDBY { get; set; }

        [DataMember]
        public Nullable<System.DateTime> MODIFIEDON { get; set; }
    }
}
