using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercFACTUpload
{
    class RepairPartsEntities
    {
        public string PART_CD { get; set; }
        public string PART_DESC { get; set; }//Kasturee_Part_desc_26-03-19
        public double COST_CPH { get; set; }
        public double COST_LOCAL { get; set; }
        public double QTY_PARTS { get; set; }
        public string MANUFCTR { get; set; }
        public double PART_PRICE { get; set; }
        public long QUANTITY { get; set; }
        public double CORE_VALUE { get; set; }
        public string DEDUCT_CORE { get; set; }
        public string MSL_PART_SW { get; set; }
        public double DISCOUNT_PERCENT { get; set; }

        // Miscellaneous work 
        public double TAX_LOCAL;
        public double TAX_CPH;
        public double COST_COUNTRY;
        
    }
}
