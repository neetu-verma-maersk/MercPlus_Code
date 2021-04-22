using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class GradeModel
    {
        public int GradeId { get; set; }
        public string GradeCode { get; set; }
        public string GradeDescription { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}