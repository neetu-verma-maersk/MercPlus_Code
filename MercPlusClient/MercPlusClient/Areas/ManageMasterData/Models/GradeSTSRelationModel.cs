using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class GradeSTSRelationModel
    {
        public int GradeSTSId { get; set; }
        public int GradeId { get; set; }
        public string GradeCode { get; set; }
        public bool IsApplicale { get; set; }
    }
}