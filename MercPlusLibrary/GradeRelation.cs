using System;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class GradeRelation
    {
        [DataMember]
        public int GradeRelationId { get; set; }

        [DataMember]
        public int GradeId { get; set; }

        [DataMember]
        public string GradeCode { get; set; }

        [DataMember]
        public string GradeDescription { get; set; }

        [DataMember]
        public string UpgradedGrade { get; set; }

        [DataMember]
        public string DowngradedGrade { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime? CreatedOn { get; set; }

        [DataMember]
        public string ModifiedBy { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }
    }
}
