using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercFACTUpload
{
    class RepairEntities
    {
        public string REPAIR_CD { get; set; }
        public long QTY_REPAIRS { get; set; }
        public double SHOP_MATERIAL_AMT { get; set; }
        public double CPH_MATERIAL_AMT { get; set; }
        public double ACTUAL_MANH { get; set; }
        public string REPAIR_DESC { get; set; }
        public string TAX_APPLIED_SW { get; set; }
        public string REPAIR_LOC_CD { get; set; } //Kasturee XML-12-06-18

        public List<RepairPartsEntities> pPart = null;
    }
}
