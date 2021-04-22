using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercDERTService
{
    public class EllWO
    {
        public decimal? sum_partcost { get; set; }
        public decimal? sum_repair_material_amt { get; set; }
        public string VENDOR_REF_NO { get; set; }
        public string MODE { get; set; }
        public string MODE_DESC { get; set; }
        public short? STATUS_CODE { get; set; }
        public string THIRD_PARTY { get; set; }
        public DateTime? REPAIR_DTE { get; set; }
        public string CAUSE { get; set; }
        public int WO_ID { get; set; }
        public string CURCD { get; set; }
        public string LOC_CD { get; set; }
        public string SHOP_CD { get; set; }
        public string SHOP_DESC { get; set; }
        public string EMAIL_ADR { get; set; }
        public string PHONE { get; set; }
        public decimal? TOT_W_MATERIAL_AMT_CPH { get; set; }
        public decimal? TOT_T_MATERIAL_AMT_CPH { get; set; }
        public decimal? IMPORT_TAX_CPH { get; set; }
        public decimal? SALES_TAX_LABOR_CPH { get; set; }
        public decimal? SALES_TAX_PARTS_CPH { get; set; }
        public decimal? AGENT_PARTS_TAX_CPH { get; set; }
        public decimal? TOT_COST_CPH { get; set; }
        public decimal? TOT_SHOP_AMT_CPH { get; set; }

        public double? TOT_REPAIR_MANH { get; set; }
        public decimal? TOT_COST_REPAIR { get; set; }
        public decimal? MANH_RATE_CPH { get; set; }
        public string TPI_CD { get; set; }
        public string DAMAGE_CD { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public short? QTY_REPAIRS { get; set; }
        public string REPAIR_CD { get; set; }
        public string REPAIR_DESC { get; set; }
        public string REPAIR_LOC_CD { get; set; }
        //public string description { get; set; }
        public double? ACTUAL_MANH { get; set; }
        public decimal? CPH_MATERIAL_AMT { get; set; }
        public decimal? COST_CPH { get; set; }
        public double? QTY_PARTS { get; set; }
    }
}
