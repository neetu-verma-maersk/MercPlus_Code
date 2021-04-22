using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercFACTUpload
{
    class MercFactUploadEntity
    {
        public string WO_ID { get; set; }
        public string CUSTOMER_CD { get; set; }
        public string MODE { get; set; }
        public string SHOP_CD { get; set; }
        public string REPAIR_DTE { get; set; }
        public double EXCHANGE_RATE { get; set; }
        public double TOT_SHOP_AMT { get; set; }
        public double TOT_COST_LOCAL { get; set; }
        public string VENDOR_REF_NO { get; set; }
        public string VOUCHER_NO { get; set; }
        public string APPROVAL_DTE { get; set; }
        public double SALES_TAX_LABOR { get; set; }
        public string SALES_TAX_LABOR_PCT { get; set; }
        public double SALES_TAX_PARTS { get; set; }
        public double TOT_LABOR_COST { get; set; }
        public double TOT_MAN_PARTS { get; set; }
        public double IMPORT_TAX_CPH { get; set; }
        public double IMPORT_TAX { get; set; }
        public string EQPNO { get; set; }
        public string CUCDN { get; set; }
        public string CAUSE { get; set; }
        public double MANH_RATE { get; set; }
        public double MANH_RATE_CPH { get; set; }
        public double OT_RATE { get; set; }
        public double OT_RATE_CPH { get; set; }
        public double DT_RATE { get; set; }
        public double DT_RATE_CPH { get; set; }
        public double MISC_RATE { get; set; }
        public double MISC_RATE_CPH { get; set; }
        public double TOT_MANH_OT { get; set; }
        public double TOT_MANH_DT { get; set; }
        public double TOT_MANH_MISC { get; set; }
        public string COUNTRY_CUCDN { get; set; }
        public double COUNTRY_EXCHANGE_RATE { get; set; }
        public string COUNTRY_EXCHANGE_DTE { get; set; }
        public string XACCOUNT_CD { get; set; }
        public string XRRIS_XMIT_SW { get; set; }
        public string CSM_PAYAGENT_CD { get; set; }
        public string CSM_CORP_PAYAGENT_CD { get; set; }
        public string CSM_RRIS_FORMAT { get; set; }
        public string CSM_PROFIT_CENTER { get; set; }
        public string CSM_SUB_PROFIT_CENTER { get; set; }
        public string CSM_ACCOUNT_CD { get; set; }
        public string PA_PAYAGENT_CD { get; set; }
        public string PA_CORP_PAYAGENT_CD { get; set; }
        public string PA_RRIS_FORMAT { get; set; }
        public string PA_PROFIT_CENTER { get; set; }
        public string PA_SUB_PROFIT_CENTER { get; set; }
        public string SH_SHOP_TYPE_CD { get; set; }
        public string SH_VENDOR_CD { get; set; }
        public string SH_SHOP_DESC { get; set; }
        public string SH_RKRPLOC { get; set; }
        public string DECENTRALIZED { get; set; }
        public string LOC_CD { get; set; }
        public double PCT_MATERIAL_FACTOR { get; set; }
        public string SH_RRIS70_SUFFIX_CD { get; set; }
        public string SH_RRIS_XMIT_SW { get; set; }
        public double SH_IMPORT_TAX_PCT { get; set; }
        public string SUPPLIER_CD { get; set; }
        public string LOCAL_ACCOUNT_CD { get; set; }
    }
}
