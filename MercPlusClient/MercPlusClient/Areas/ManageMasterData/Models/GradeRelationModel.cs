using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class GradeRelationModel
    {
        public int GradeRelationId { get; set; }
        public int GradeId { get; set; }
        public string GradeCode { get; set; }
        public string GradeDescription { get; set; }
        public List<string> UpgradedGrades { get; set; }
        public List<string> DowngradedGrades { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}