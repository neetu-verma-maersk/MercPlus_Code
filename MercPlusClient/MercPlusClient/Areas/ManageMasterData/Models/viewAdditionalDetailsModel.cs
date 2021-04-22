using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class viewAdditionalDetailsModel
    {
        public string WOID { get; set; }
        public string SHOP_CD { get; set; }
        public string Vender_Ref_No { get; set; }
        public string SHOP_TYPE_CODE { get; set; }
        [DisplayFormat(DataFormatString = "{0:n4}")]
        public decimal? EXCHANGE_RATE { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public double? TOT_PREP_HRS { get; set; }
        public string Currency { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? MANH_RATE_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? MANH_RATE { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? TOT_MANH_REG { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_EMRCost_USD { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_ShopCost_USD { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_ShopCost_Currency { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? OT_RATE_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? OT_RATE { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? TOT_MANH_OT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_EMRCost_USD_OT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_ShopCost_USD_OT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_ShopCost_Currency_OT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? DT_RATE_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? DT_RATE { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? TOT_MANH_DT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_EMRCost_USD_DT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_ShopCost_USD_DT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_ShopCost_Currency_DT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? MISC_RATE_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? MISC_RATE { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? TOT_MANH_MISC { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_EMRCost_USD_MISC { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_ShopCost_USD_MISC { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_ShopCost_Currency_MISC { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? TOT_LABOR_COST_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? TOT_LABOR_COST { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public double? TOT_REPAIR_MANH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? Cal_TOT_LABOR_COST_Currency { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? AGENT_PARTS_TAX_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? AGENT_PARTS_TAX { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? TOT_MAN_PARTS_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? TOT_MAN_PARTS { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? TOT_SHOP_AMT_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? TOT_SHOP_AMT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? TOT_MAERSK_PARTS_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? TOT_MAERSK_PARTS { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? CAL_SHOP_AMT_AND_MAN_PARTS { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? CAL_SHOP_AMT_CPH_AND_MAN_PARTS_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? CAL_SHOP_AMT_AND_MAN_PARTS_USD { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? CAL_SHOP_AMT_USD { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? CAL_TOT_MAN_PARTS_USD { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? SALES_TAX_LABOR_PCT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? SALES_TAX_LABOR_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? SALES_TAX_LABOR { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? SALES_TAX_PARTS_PCT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? SALES_TAX_PARTS_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? SALES_TAX_PARTS { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? IMPORT_TAX_PCT { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? IMPORT_TAX_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? IMPORT_TAX { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? CAL_IMPORT_TAX { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? CAL_SALES_TAX_PARTS { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? CAL_SALES_TAX_LABOR { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? CAL_SALES_AGENT_CPH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? CAL_SALES_AGENT_PARTS { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal? CAL_SALES_AGENT { get; set; }
    }
}