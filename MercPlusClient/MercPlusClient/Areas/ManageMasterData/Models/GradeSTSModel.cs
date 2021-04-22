using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class GradeSTSModel
    {
        public int GradeSTSRowId { get; set; }        
        public string STSCode { get; set; }
        public string STSDescription { get; set; }
        public string Mode { get; set; }
        public string ManualCD { get; set; }
        public List<GradeSTSRelationModel> GradeSTSRelationModel { get; set; }        
    }
}