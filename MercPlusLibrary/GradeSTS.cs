using System;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class GradeSTS
    {
        [DataMember]
        public int GradeSTSId { get; set; }

        [DataMember]
        public int GradeId { get; set; }

        [DataMember]
        public string GradeCode { get; set; }

        [DataMember]
        public string STSCode { get; set; }

        [DataMember]
        public string STSDescription { get; set; }

        [DataMember]
        public string Mode { get; set; }

        [DataMember]
        public string ManualCD { get; set; }

        [DataMember]
        public bool IsApplicable { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime? CreatedOn { get; set; }

        [DataMember]
        public string ModifiedBy { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public bool FLAG { get; set; }
    }
}
