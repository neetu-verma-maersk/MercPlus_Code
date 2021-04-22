using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using System.Data.Objects;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MercPlusLibrary;

namespace ManageReportsService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class ManageReportsService : IManageReports
    {
        LogEntry logEntry = new LogEntry();
        //ManageReportsDAL ManageReportsDAL = new ManageReportsDAL();
        ManageReportsServiceEntities objContext = new ManageReportsServiceEntities();
        ErrMessage ErrorMessages = new ErrMessage();

        #region GetReportsDetailsMercA01
        public List<Reports> GetReportsDetailsMercA01(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, out List<ErrMessage> ErrorMessageList)
        {
            List<Reports> ReportsList = new List<Reports>();
            Reports Reports = new Reports();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            DateTime startDate = Convert.ToDateTime(DateFrom);
            string dateTo = DateTo + " " + "23:59:59";
            DateTime endDate = Convert.ToDateTime(dateTo);
            ErrorMessageList = new List<ErrMessage>();
            short startStatusCode = 600;
            short endStatusCode = 9000;
            try
            {
                #region MercA01
                //string whereCond = "wo.CUSTOMER_CD = csm.CUSTOMER_CD and wo.MODE = csm.MODE and wo.SHOP_CD = csm.SHOP_CD";
                //if (MercA01 != null || !string.IsNullOrEmpty(MercA01))
                //{
                string a = string.Empty;
                var WOrderMercA01 = (from wo in objContext.MESC1TS_WO
                                     from csm in objContext.MESC1TS_CUST_SHOP_MODE
                                     where wo.CUSTOMER_CD == csm.CUSTOMER_CD &&
                                         wo.MODE == csm.MODE &&
                                         wo.SHOP_CD == csm.SHOP_CD &&
                                         wo.RKRP_XMIT_DTE >= startDate &&
                                         wo.RKRP_XMIT_DTE <= endDate &&
                                         wo.STATUS_CODE >= startStatusCode &&
                                         wo.STATUS_CODE < endStatusCode &&
                                         wo.SHOP_CD == ShopCode &&
                                         (CustomerCode == null ? a == a : wo.CUSTOMER_CD == CustomerCode) &&
                                         (Manual == null ? a == a : wo.MANUAL_CD == Manual) &&
                                         (Mode == null ? a == a : wo.MODE == Mode)
                                     //orderby wo.MODE, wo.EQPNO//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                                     select new
                                     {
                                         wo.MODE,
                                         wo.REPAIR_DTE,
                                         csm.PAYAGENT_CD,
                                         wo.EQPNO,
                                         wo.WO_ID,
                                         wo.VENDOR_REF_NO,
                                         wo.VOUCHER_NO,
                                         wo.TOT_MANH_REG,
                                         wo.TOT_MANH_OT,
                                         wo.TOT_MANH_DT,
                                         wo.TOT_MANH_MISC,
                                         wo.TOT_REPAIR_MANH,
                                         wo.TOT_LABOR_COST_CPH,
                                         wo.TOT_MAN_PARTS_CPH,
                                         wo.TOT_SHOP_AMT_CPH,
                                         wo.IMPORT_TAX_CPH,
                                         wo.SALES_TAX_PARTS_CPH,
                                         wo.SALES_TAX_LABOR_CPH,
                                         wo.TOT_COST_CPH,
                                         wo.TOT_MAERSK_PARTS_CPH,
                                         wo.TOT_COST_REPAIR_CPH,
                                     }).Distinct().OrderBy(mode => mode.MODE).ThenBy(dt => dt.REPAIR_DTE).ThenBy(eqpno => eqpno.EQPNO);

                //Get the shop detail to check the rris_xmit_sw value
                List<MESC1TS_SHOP> shop = (from s in objContext.MESC1TS_SHOP
                                           where s.SHOP_CD == ShopCode
                                           select s).ToList();
                //List<Shop> shopList = shop.ToList();

                if (WOrderMercA01 != null && WOrderMercA01.Count() != 0)
                {
                    //lambda exp
                    foreach (var obj in WOrderMercA01)
                    {
                        Reports = new Reports();
                        Reports.ActualCompletionDate = obj.REPAIR_DTE.ToString();
                        Reports.Mode = obj.MODE;
                        Reports.EquipmentNo = obj.EQPNO;
                        Reports.AgentCode = obj.PAYAGENT_CD;
                        Reports.WorkOrderNo = obj.WO_ID.ToString();
                        Reports.VendorRefNo = obj.VENDOR_REF_NO;
                        //If “rris_xmit_sw’=‘y’ in shop table
                        if (shop[0].RRIS_XMIT_SW == "Y")
                        {
                            Reports.VoucherNumber = obj.VOUCHER_NO;
                            Reports.AgentCode = obj.PAYAGENT_CD;
                        }
                        if (obj.TOT_MANH_REG != null)
                        {
                            Reports.OrdinaryManHours = Math.Round(obj.TOT_MANH_REG.Value, 2).ToString();
                        }
                        if (obj.TOT_MANH_OT != null)
                        {
                            Reports.Overtime1ManHours = Math.Round(obj.TOT_MANH_OT.Value, 2).ToString();
                        }
                        if (obj.TOT_MANH_DT != null)
                        {
                            Reports.Overtime2ManHours = Math.Round(obj.TOT_MANH_DT.Value, 2).ToString();
                        }
                        if (obj.TOT_MANH_MISC != null)
                        {
                            Reports.Overtime3ManHours = Math.Round(obj.TOT_MANH_MISC.Value, 2).ToString();
                        }
                        if (obj.TOT_REPAIR_MANH != null)
                        {
                            Reports.TotalHours = Math.Round(obj.TOT_REPAIR_MANH.Value, 2).ToString();
                        }
                        if (obj.TOT_LABOR_COST_CPH != null)
                        {
                            Reports.TotalLabourCostCPH = Math.Round(obj.TOT_LABOR_COST_CPH.Value, 2).ToString();
                        }
                        if (shop[0].SHOP_TYPE_CD == "4")
                        {
                            Reports.TotalCostOfAgentSuppliedNumberedParts = "";
                        }
                        else
                        {
                            Reports.TotalCostOfShopSuppliedNumberedParts = Math.Round(obj.TOT_MAN_PARTS_CPH.Value, 2).ToString();
                        }
                        if (obj.TOT_SHOP_AMT_CPH != null)
                        {
                            Reports.TotalCostOfShopSuppliedMaterials = Math.Round(obj.TOT_SHOP_AMT_CPH.Value, 2).ToString();
                        }
                        if (obj.IMPORT_TAX_CPH != null)
                        {
                            Reports.ImportTax = Math.Round(obj.IMPORT_TAX_CPH.Value, 2).ToString();
                        }
                        if (obj.SALES_TAX_PARTS_CPH != null)
                        {
                            Reports.SalesTaxParts = Math.Round(obj.SALES_TAX_PARTS_CPH.Value, 2).ToString();
                        }
                        if (obj.SALES_TAX_LABOR_CPH != null)
                        {
                            Reports.SalesTaxLabour = Math.Round(obj.SALES_TAX_LABOR_CPH.Value, 2).ToString();
                        }
                        if (obj.TOT_COST_CPH != null)
                        {
                            Reports.TotalToBePaidToShop = Math.Round(obj.TOT_COST_CPH.Value, 2).ToString();
                        }
                        if (obj.TOT_MAERSK_PARTS_CPH != null)
                        {
                            Reports.TotalCostOfCPHSuppliedParts = Math.Round(obj.TOT_MAERSK_PARTS_CPH.Value, 2).ToString();
                        }
                        if (obj.TOT_COST_REPAIR_CPH != null)
                        {
                            Reports.TotalCostOfRepairCPH = Math.Round(obj.TOT_COST_REPAIR_CPH.Value, 2).ToString();
                        }
                        Reports.HeaderShopDesc = shop[0].SHOP_DESC;
                        ReportsList.Add(Reports);
                    }
                }
                //}
                #endregion MercA01                #endregion MercA01
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity";
                ErrorMessageList.Add(ErrorMessages);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                ReportsList.Add(Reports);
            }

            return ReportsList;
        }
        #endregion GetReportsDetailsMercA01

        #region GetReportsDetailsMercA02
        public List<Reports> GetReportsDetailsMercA02(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, out List<ErrMessage> ErrorMessageList)
        {
            List<Reports> ReportsList = new List<Reports>();
            Reports Reports = new Reports();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            //string shopCode = string.Empty;
            //ShopCode = "134";
            DateTime startDate = Convert.ToDateTime(DateFrom);
            string dateTo = DateTo + " " + "23:59:59";
            DateTime endDate = Convert.ToDateTime(dateTo);
            string a = string.Empty;
            ErrorMessageList = new List<ErrMessage>();
            short startStatusCode = 600;
            short endStatusCode = 9000;

            try
            {
                #region MercA02
                var WOrderMERCA02 = (from wo in objContext.MESC1TS_WO
                                     from shop in objContext.MESC1TS_SHOP
                                     from csm in objContext.MESC1TS_CUST_SHOP_MODE
                                     where wo.SHOP_CD == shop.SHOP_CD &&
                                         wo.CUSTOMER_CD == csm.CUSTOMER_CD &&
                                         wo.MODE == csm.MODE &&
                                         wo.SHOP_CD == csm.SHOP_CD &&
                                         wo.SHOP_CD == ShopCode &&
                                         wo.RKRP_XMIT_DTE >= startDate &&
                                         wo.RKRP_XMIT_DTE <= endDate &&
                                         wo.STATUS_CODE >= startStatusCode &&
                                         wo.STATUS_CODE < endStatusCode &&
                                         wo.SHOP_CD == ShopCode &&
                                        (CustomerCode == null ? a == a : wo.CUSTOMER_CD == CustomerCode) &&
                                        (Manual == null ? a == a : wo.MANUAL_CD == Manual) &&
                                        (Mode == null ? a == a : wo.MODE == Mode)
                                     //orderby wo.MODE //, (wo.REPAIR_DTE), month(wo.REPAIR_DTE), day(wo.REPAIR_DTE), EQPNO
                                     select new
                                     {
                                         wo.MODE,
                                         wo.REPAIR_DTE,
                                         wo.EQPNO,
                                         wo.WO_ID,
                                         wo.VENDOR_REF_NO,
                                         wo.VOUCHER_NO,
                                         csm.PAYAGENT_CD,
                                         wo.EXCHANGE_RATE,
                                         wo.TOT_MANH_REG,
                                         wo.TOT_MANH_OT,
                                         wo.TOT_MANH_DT,
                                         wo.TOT_MANH_MISC,
                                         wo.TOT_LABOR_COST,
                                         wo.TOT_REPAIR_MANH,
                                         shop.SHOP_TYPE_CD,
                                         wo.TOT_MAN_PARTS, //Total cost of numbered parts
                                         wo.TOT_SHOP_AMT, //Total cost of supplied materials
                                         wo.IMPORT_TAX,
                                         wo.SALES_TAX_PARTS,
                                         wo.SALES_TAX_LABOR,
                                         wo.TOT_COST_LOCAL, //Total to be paid to shop
                                         wo.TOTAL_COST_LOCAL_USD, // Total to be paid to shop in USD
                                         //CAST(( tot_cost_cph * 100 )/wo.Exchange_rate AS NUMERIC(12,2)) 'Total to be paid to agent from CPH in local currency'
                                         wo.TOT_COST_REPAIR_CPH, //Total cost of repair (incl. CPH supplied parts)in USD
                                         wo.TOT_MAERSK_PARTS_CPH, //Total cost of CPH supplied parts in USD
                                         wo.TOT_COST_CPH, //Total to be paid to agent from CPH in USD

                                     }).Distinct().OrderBy(mode => mode.MODE).ThenBy(dt => dt.REPAIR_DTE).ThenBy(eqp => eqp.EQPNO);

                var header = (from s in objContext.MESC1TS_SHOP
                              from v in objContext.MESC1TS_VENDOR
                              from c in objContext.MESC1TS_CURRENCY
                              where v.VENDOR_CD == s.VENDOR_CD &&
                                    s.CUCDN == c.CUCDN &&
                                    s.SHOP_CD == ShopCode
                              select new
                              {
                                  v.VENDOR_CD,
                                  v.VENDOR_DESC,
                                  s.SHOP_CD,
                                  s.SHOP_DESC,
                                  c.CUCDN,
                                  c.CURRNAMC
                              });

                foreach (var obj in WOrderMERCA02)
                {
                    Reports = new Reports();
                    Reports.Mode = obj.MODE;
                    Reports.ActualCompletionDate = obj.REPAIR_DTE.ToString();
                    Reports.EquipmentNo = obj.EQPNO;
                    Reports.AgentCode = obj.PAYAGENT_CD;
                    Reports.WorkOrderNo = obj.WO_ID.ToString();
                    Reports.VendorRefNo = obj.VENDOR_REF_NO;
                    Reports.ExchangeRate = obj.EXCHANGE_RATE.ToString();
                    Reports.VoucherNumber = obj.VOUCHER_NO;
                    if (obj.TOT_MANH_REG != null)
                    {
                        Reports.OrdinaryManHours = Math.Round(obj.TOT_MANH_REG.Value, 2).ToString();
                    }
                    if (obj.TOT_COST_REPAIR_CPH != null)
                    {
                        Reports.Overtime1ManHours = Math.Round(obj.TOT_MANH_OT.Value, 2).ToString();
                    }
                    if (obj.TOT_COST_REPAIR_CPH != null)
                    {
                        Reports.Overtime2ManHours = Math.Round(obj.TOT_MANH_DT.Value, 2).ToString();
                    }
                    if (obj.TOT_MANH_MISC != null)
                    {
                        Reports.Overtime3ManHours = Math.Round(obj.TOT_MANH_MISC.Value, 2).ToString();
                    }
                    if (obj.TOT_REPAIR_MANH != null)
                    {
                        Reports.TotalHours = Math.Round(obj.TOT_REPAIR_MANH.Value, 2).ToString();
                    }
                    if (obj.TOT_LABOR_COST != null)
                    {
                        Reports.TotalLabourCost = Math.Round(obj.TOT_LABOR_COST.Value, 2).ToString();
                    }
                    if (obj.TOT_MAN_PARTS != null)
                    {
                        Reports.TotalCostOfNumberedParts = Math.Round(obj.TOT_MAN_PARTS.Value, 2).ToString();
                    }
                    if (obj.TOT_SHOP_AMT != null)
                    {
                        Reports.TotalCostOfSuppliedMaterials = Math.Round(obj.TOT_SHOP_AMT.Value, 2).ToString();
                    }
                    if (obj.IMPORT_TAX != null)
                    {
                        Reports.ImportTax = Math.Round(obj.IMPORT_TAX.Value, 2).ToString();
                    }
                    if (obj.SALES_TAX_PARTS != null)
                    {
                        Reports.SalesTaxParts = Math.Round(obj.SALES_TAX_PARTS.Value, 2).ToString();
                    }
                    if (obj.TOT_COST_REPAIR_CPH != null)
                    {
                        Reports.SalesTaxLabour = Math.Round(obj.TOT_COST_REPAIR_CPH.Value, 2).ToString();
                    }
                    if (obj.SALES_TAX_LABOR != null)
                    {
                        Reports.TotalToBePaidToShop = Math.Round(obj.SALES_TAX_LABOR.Value, 2).ToString();
                    }
                    if (obj.TOTAL_COST_LOCAL_USD != null)
                    {
                        Reports.TotalToBePaidToShopInUSD = Math.Round(obj.TOTAL_COST_LOCAL_USD.Value, 2).ToString();
                    }
                    if (obj.TOT_COST_REPAIR_CPH != null)
                    {
                        Reports.TotalToBePaidToAgentFromCPHInLocalCurrency = Math.Round(obj.TOT_COST_REPAIR_CPH.Value, 2).ToString();
                    }
                    if (obj.TOT_COST_REPAIR_CPH != null)
                    {
                        Reports.TotalCostOfRepairCPH = Math.Round(obj.TOT_COST_REPAIR_CPH.Value, 2).ToString();
                    }
                    if (obj.TOT_MAERSK_PARTS_CPH != null)
                    {
                        Reports.TotalCostOfCPHSuppliedPartsInUSD = Math.Round(obj.TOT_MAERSK_PARTS_CPH.Value, 2).ToString();
                    }
                    if (obj.TOT_COST_CPH != null)
                    {
                        Reports.TotalToBePaidToAgentFromCPHInUSD = Math.Round(obj.TOT_COST_CPH.Value, 2).ToString();
                    }
                    Reports.ShopCode = obj.SHOP_TYPE_CD;
                    double? exrt = (100 / ((double)obj.EXCHANGE_RATE * .01));
                    Reports.ExchangeRate = Math.Round(exrt.Value, 2).ToString();
                    if (obj.SHOP_TYPE_CD == "4")
                    {
                        Reports.PartSupplier = "Agent";
                    }
                    else
                    {
                        Reports.PartSupplier = "Shop";
                    }
                    decimal? TotalToAgent = ((obj.TOT_COST_CPH * 100) / obj.EXCHANGE_RATE);
                    Reports.TotalToBePaidToAgentFromCPHInLocalCurrency = Math.Round(TotalToAgent.Value, 2).ToString();
                    foreach (var item in header)
                    {
                        Reports.HeaderCurrencyCode = item.CUCDN;
                        Reports.HeaderCurrencyName = item.CURRNAMC;
                        Reports.HeaderVendorCode = item.VENDOR_CD;
                        Reports.HeaderVendorDesc = item.VENDOR_DESC;
                        Reports.HeaderShopCode = item.SHOP_CD;
                        Reports.HeaderShopDesc = item.SHOP_DESC;
                    }

                    ReportsList.Add(Reports);
                }

                #endregion MercA02
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                ReportsList.Add(Reports);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ReportsList;
        }
        #endregion GetReportsDetailsMercA02

        #region GetReportsDetailsMercA03
        public List<Reports> GetReportsDetailsMercA03(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, out List<ErrMessage> ErrorMessageList)
        {
            List<Reports> ReportsList = new List<Reports>();
            Reports Reports = new Reports();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            DateTime startDate = Convert.ToDateTime(DateFrom);
            string dateTo = DateTo + " " + "23:59:59";
            DateTime endDate = Convert.ToDateTime(dateTo);
            string a = string.Empty;
            short startStatusCode = 600;
            short endStatusCode = 9000;
            ErrorMessageList = new List<ErrMessage>();

            try
            {
                #region MercA03

                var WOrderMERCA03 = (from WO in objContext.MESC1TS_WO
                                     from S in objContext.MESC1TS_SHOP
                                     from C in objContext.MESC1TS_CURRENCY
                                     //from V in objContext.MESC1TS_VENDOR
                                     where WO.SHOP_CD == S.SHOP_CD &&
                                         //WO.VENDOR_CD == V.VENDOR_CD &&
                                         WO.CUCDN == C.CUCDN &&
                                         WO.RKRP_XMIT_DTE >= startDate &&
                                         WO.RKRP_XMIT_DTE <= endDate &&
                                         WO.STATUS_CODE >= startStatusCode &&
                                         WO.STATUS_CODE < endStatusCode &&
                                         WO.SHOP_CD == ShopCode &&
                                         (CustomerCode == null ? a == a : WO.CUSTOMER_CD == CustomerCode) &&
                                         (Manual == null ? a == a : WO.MANUAL_CD == Manual) &&
                                         (Mode == null ? a == a : WO.MODE == Mode)
                                     //orderby WO.MODE//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                                     select new
                                     {
                                         WO.MODE,
                                         WO.REPAIR_DTE,
                                         WO.EQPNO,
                                         WO.WO_ID,
                                         WO.VENDOR_REF_NO,
                                         WO.TOT_MANH_REG,
                                         WO.TOT_MANH_OT,
                                         WO.TOT_MANH_DT,
                                         WO.TOT_MANH_MISC,
                                         WO.TOT_LABOR_COST,
                                         WO.TOT_REPAIR_MANH,
                                         S.SHOP_TYPE_CD,
                                         WO.TOT_MAN_PARTS,
                                         WO.TOT_SHOP_AMT,
                                         WO.IMPORT_TAX,
                                         WO.SALES_TAX_PARTS,
                                         WO.SALES_TAX_LABOR,
                                         WO.TOT_COST_LOCAL
                                     }).Distinct().OrderBy(mode => mode.MODE).ThenBy(dt => dt.REPAIR_DTE).ThenBy(eqp => eqp.EQPNO);

                //Header
                //"select convert(char(16),getutcdate(),120) date, v.Vendor_cd 'Vendor_cd', v.Vendor_desc 'Vendor_desc'," & _ 
                //   "s.shop_cd 'SHOP_CD', s.shop_desc, c.cucdn, c.currnamc 'Currency'" & _ 
                //   "from mesc1ts_shop s, mesc1ts_vendor v, mesc1ts_currency c " & _  
                //   "where v.vendor_cd = s.vendor_cd " & _  
                //   "and s.cucdn = c.cucdn and s.shop_cd ='"&Shop&"'" 

                var header = (from s in objContext.MESC1TS_SHOP
                              from v in objContext.MESC1TS_VENDOR
                              from c in objContext.MESC1TS_CURRENCY
                              where v.VENDOR_CD == s.VENDOR_CD &&
                                    s.CUCDN == c.CUCDN &&
                                    s.SHOP_CD == ShopCode
                              select new
                              {
                                  v.VENDOR_CD,
                                  v.VENDOR_DESC,
                                  s.SHOP_CD,
                                  s.SHOP_DESC,
                                  c.CUCDN,
                                  c.CURRNAMC
                              });


                foreach (var obj in WOrderMERCA03)
                {
                    Reports = new Reports();
                    Reports.Mode = obj.MODE;
                    Reports.ActualCompletionDate = obj.REPAIR_DTE.ToString();
                    Reports.EquipmentNo = obj.EQPNO;
                    //Reports.AgentCode = obj.PAYAGENT_CD;
                    Reports.WorkOrderNo = obj.WO_ID.ToString();
                    Reports.VendorRefNo = obj.VENDOR_REF_NO;
                    if (obj.TOT_MANH_REG != null)
                    {
                        Reports.OrdinaryManHours = Math.Round(obj.TOT_MANH_REG.Value, 2).ToString();
                    }
                    if (obj.TOT_MANH_OT != null)
                    {
                        Reports.Overtime1ManHours = Math.Round(obj.TOT_MANH_OT.Value, 2).ToString();
                    }
                    if (obj.TOT_MANH_DT != null)
                    {
                        Reports.Overtime2ManHours = Math.Round(obj.TOT_MANH_DT.Value, 2).ToString();
                    }
                    if (obj.TOT_MANH_MISC != null)
                    {
                        Reports.Overtime3ManHours = Math.Round(obj.TOT_MANH_MISC.Value, 2).ToString();
                    }
                    if (obj.TOT_REPAIR_MANH != null)
                    {
                        Reports.TotalHours = Math.Round(obj.TOT_REPAIR_MANH.Value, 2).ToString();
                    }
                    if (obj.TOT_LABOR_COST != null)
                    {
                        Reports.TotalLabourCost = Math.Round(obj.TOT_LABOR_COST.Value, 2).ToString();
                    }
                    if (obj.TOT_MAN_PARTS != null)
                    {
                        Reports.TotalCostOfNumberedParts = Math.Round(obj.TOT_MAN_PARTS.Value, 2).ToString();
                    }
                    if (obj.IMPORT_TAX != null)
                    {
                        Reports.ImportTax = Math.Round(obj.IMPORT_TAX.Value, 2).ToString();
                    }
                    if (obj.TOT_SHOP_AMT != null)
                    {
                        Reports.TotalCostOfSuppliedMaterials = Math.Round(obj.TOT_SHOP_AMT.Value, 2).ToString();
                    }
                    if (obj.SALES_TAX_PARTS != null)
                    {
                        Reports.SalesTaxParts = Math.Round(obj.SALES_TAX_PARTS.Value, 2).ToString();
                    }
                    if (obj.SALES_TAX_LABOR != null)
                    {
                        Reports.SalesTaxLabour = Math.Round(obj.SALES_TAX_LABOR.Value, 2).ToString();
                    }
                    if (obj.TOT_COST_LOCAL != null)
                    {
                        Reports.TotalToBePaidToShop = Math.Round(obj.TOT_COST_LOCAL.Value, 2).ToString();
                    }
                    
                    if (obj.SHOP_TYPE_CD == "4")
                    {
                        Reports.PartSupplier = "Agent";
                    }
                    else
                    {
                        Reports.PartSupplier = "Shop";
                    }
                    foreach (var item in header)
                    {
                        Reports.HeaderCurrencyCode = item.CUCDN;
                        Reports.HeaderCurrencyName = item.CURRNAMC;
                        Reports.HeaderVendorCode = item.VENDOR_CD;
                        Reports.HeaderVendorDesc = item.VENDOR_DESC;
                        Reports.HeaderShopCode = item.SHOP_CD;
                        Reports.HeaderShopDesc = item.SHOP_DESC;
                    }
                    ReportsList.Add(Reports);
                }
                #endregion MercA03
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                ReportsList.Add(Reports);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ReportsList;
        }
        #endregion GetReportsDetailsMercA03

        #region GetReportsDetailsMercB01
        public List<Reports> GetReportsDetailsMercB01(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, string RepairCode, out List<ErrMessage> ErrorMessageList)//, string MercA01, string MERCB01, string MERCC01,
        //string MERCD03, string MERCD05, string MERCE01)
        {
            List<Reports> ReportsList = new List<Reports>();
            Reports Reports = new Reports();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            string shopCode = string.Empty;
            DateTime startDate = Convert.ToDateTime(DateFrom);
            string dateTo = DateTo + " " + "23:59:59";
            DateTime endDate = Convert.ToDateTime(dateTo);
            short startStatusCode = 600;
            short endStatusCode = 9000;
            string a = string.Empty;
            ErrorMessageList = new List<ErrMessage>();

            try
            {
                #region MercB01

                //"Select wo.SHOP_CD  'Shop Code'," & _
                //"wo.MODE 'Mode'," & _
                //"wo.eqpno 'Equipment No'," & _
                //"" & _
                //"wo.REPAIR_DTE  'Completion Date'," & _
                //"wo.wo_id 'Work order no.'," & _
                //"wo.vendor_ref_no 'Vendor Ref No'," & _
                //"wor.REPAIR_CD 'Repair Code'," & _
                //"REPAIR_DESC  'Repair code description'," & _
                //"wop.PART_CD 'Part Number'," & _
                //"PART_DESC  'Part number description'," & _
                //"MANUFACTUR_NAME 'Manufacturer'," & _
                //"QTY_PARTS  'Part Quantity'," & _
                //"COST_LOCAL 'Total cost of parts'," & _
                //"MSL_PART_SW  'Maersk Sealand CPH,supplied part'," & _
                //"CORE_PART_SW  'Core part'," & _
                //"SERIAL_NUMBER  'Core serial no/,TAG number' " & _
                //"from mesc1ts_wo wo, mesc1ts_worepair wor, mesc1ts_wopart wop, mesc1ts_manufactur mfr, " & _
                //"mesc1ts_shop sh, mesc1ts_vendor vr, mesc1ts_repair_code wrc,mesc1ts_master_part mp " & _
                //"where wo.WO_ID=wor.WO_ID " & _
                //"and wo.WO_ID=wop.WO_ID " & _
                //"and wor.REPAIR_CD=wop.REPAIR_CD " & _
                //"and wop.PART_CD=mp.PART_CD " & _
                //"and mp.MANUFCTR=mfr.MANUFCTR " & _
                //"and wo.MANUAL_CD=wrc.MANUAL_CD " & _
                //"and wo.MODE=wrc.MODE " & _
                //"and wor.REPAIR_CD=wrc.REPAIR_CD " & _
                //"and wo.SHOP_CD=sh.SHOP_CD " & _
                //"and sh.VENDOR_CD=vr.VENDOR_CD " & _
                //"and wop.PART_CD!='' " & _
                //"and RKRP_XMIT_DTE> '"&DateFrom&"' and RKRP_XMIT_DTE<'"&DateEnd&"' " & _
                //"and wo.SHOP_CD='"&Shop&"' " & _
                //"and status_code>=600 and status_code<9000 "



                var WOrderMercB01 = (from wo in objContext.MESC1TS_WO
                                     from wor in objContext.MESC1TS_WOREPAIR
                                     from wop in objContext.MESC1TS_WOPART
                                     from mfr in objContext.MESC1TS_MANUFACTUR
                                     from sh in objContext.MESC1TS_SHOP
                                     from vr in objContext.MESC1TS_VENDOR
                                     from wrc in objContext.MESC1TS_REPAIR_CODE
                                     from mp in objContext.MESC1TS_MASTER_PART
                                     where wo.WO_ID == wor.WO_ID &&
                                         wo.WO_ID == wop.WO_ID &&
                                         wor.REPAIR_CD == wop.REPAIR_CD &&
                                         wop.PART_CD == mp.PART_CD &&
                                         mp.MANUFCTR == mfr.MANUFCTR &&
                                         wo.MANUAL_CD == wrc.MANUAL_CD &&
                                         wo.MODE == wrc.MODE &&
                                         wor.REPAIR_CD == wrc.REPAIR_CD &&
                                         wo.SHOP_CD == sh.SHOP_CD &&
                                         sh.VENDOR_CD == vr.VENDOR_CD &&
                                         wop.PART_CD != string.Empty &&
                                         wo.RKRP_XMIT_DTE >= startDate &&
                                         wo.RKRP_XMIT_DTE <= endDate &&
                                         wo.STATUS_CODE >= startStatusCode &&
                                         wo.STATUS_CODE < endStatusCode &&
                                         wo.SHOP_CD == ShopCode &&
                                         (RepairCode == null ? a == a : wor.REPAIR_CD == RepairCode) &&
                                         (Mode == null ? a == a : wo.MODE == Mode)
                                     orderby wo.SHOP_CD, wo.MODE, wo.EQPNO //, year(wo.RKRP_XMIT_DTE),month(wo.RKRP_XMIT_DTE),day(wo.RKRP_XMIT_DTE), year(wo.REPAIR_DTE),month(wo.REPAIR_DTE),day(wo.REPAIR_DTE), wo.wo_id, wo.vendor_ref_no, wor.REPAIR_CD, REPAIR_DESC,wop.PART_CD  " 
                                     select new
                                     {
                                         wo.SHOP_CD,  //'Shop Code',
                                         wo.MODE, //'Mode,
                                         wo.EQPNO, // 'Equipment No'
                                         wo.REPAIR_DTE,  //'Completion Date'
                                         wo.WO_ID, // 'Work order no
                                         wo.VENDOR_REF_NO,
                                         wor.REPAIR_CD, //'Repair Code' 
                                         wrc.REPAIR_DESC, //'Repair code description'
                                         wop.PART_CD, // 'Part Number',
                                         mp.PART_DESC, //Part number description'
                                         mfr.MANUFACTUR_NAME, //Manufacturer
                                         wop.QTY_PARTS, //Part Quantity
                                         wop.COST_LOCAL, //Total cost of parts
                                         mp.MSL_PART_SW, //'Maersk Sealand CPH,supplied part'
                                         mp.CORE_PART_SW, //'Core part'
                                         wop.SERIAL_NUMBER, //SERIAL_NUMBER
                                         wo.RKRP_XMIT_DTE
                                     }).Distinct().OrderBy(shop => shop.SHOP_CD).ThenBy(mode => mode.MODE).ThenBy(eqpno => eqpno.EQPNO).ThenBy(date => date.RKRP_XMIT_DTE).ThenBy(dt => dt.REPAIR_DTE).ThenBy(dt => dt.WO_ID).ThenBy(eqp => eqp.VENDOR_REF_NO).ThenBy(dt => dt.REPAIR_CD).ThenBy(eqp => eqp.REPAIR_DESC).ThenBy(p => p.PART_CD);

                var header = (from s in objContext.MESC1TS_SHOP
                              from v in objContext.MESC1TS_VENDOR
                              from c in objContext.MESC1TS_CURRENCY
                              where v.VENDOR_CD == s.VENDOR_CD &&
                                    s.CUCDN == c.CUCDN &&
                                    s.SHOP_CD == ShopCode
                              select new
                              {
                                  v.VENDOR_CD,
                                  v.VENDOR_DESC,
                                  s.SHOP_CD,
                                  s.SHOP_DESC,
                                  c.CUCDN,
                                  c.CURRNAMC
                              });

                foreach (var obj in WOrderMercB01)
                {
                    Reports = new Reports();
                    Reports.Mode = obj.MODE;
                    Reports.ActualCompletionDate = obj.REPAIR_DTE.ToString();
                    Reports.EquipmentNo = obj.EQPNO;
                    Reports.WorkOrderNo = obj.WO_ID.ToString();
                    Reports.VendorRefNo = obj.VENDOR_REF_NO;
                    Reports.ShopCode = obj.SHOP_CD;
                    Reports.RepairCod = obj.REPAIR_CD;
                    Reports.RepairCodeDesc = obj.REPAIR_DESC;
                    Reports.PART_CD = obj.PART_CD;
                    Reports.PartDesc = obj.PART_DESC;
                    Reports.ManufacturerName = obj.MANUFACTUR_NAME;
                    Reports.QuantityParts = obj.QTY_PARTS.ToString();
                    if (obj.COST_LOCAL != null)
                    {
                        Reports.CostLocal = Math.Round(obj.COST_LOCAL.Value, 2).ToString();
                    }
                    Reports.MSLPartSW = obj.MSL_PART_SW;
                    Reports.CorePartSW = obj.CORE_PART_SW;
                    Reports.SerialNumber = obj.SERIAL_NUMBER;
                    foreach (var item in header)
                    {
                        Reports.HeaderVendorCode = item.VENDOR_CD;
                        Reports.HeaderVendorDesc = item.VENDOR_DESC;
                        Reports.HeaderShopCode = item.SHOP_CD;
                        Reports.HeaderShopDesc = item.SHOP_DESC;
                    }
                    ReportsList.Add(Reports);
                }
                #endregion MercB01
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                ReportsList.Add(Reports);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ReportsList;
        }
        #endregion GetReportsDetailsMercB01

        #region GetReportsDetailsMercC01
        public List<Reports> GetReportsDetailsMercC01(out List<ErrMessage> ErrorMessageList)
        {
            List<Reports> ReportsList = new List<Reports>();
            Reports Reports = new Reports();
            ErrorMessageList = new List<ErrMessage>();

            try
            {
                #region MercA03
                //select a.manual_cd manual,a.mode mode ,a.repair_cd rpr,a.repair_desc rpr_desc,b.exclu_repair_cd ex_rpr,c.repair_desc ex_rpr_desc " &_
                //"from mesc1ts_repair_code a , mesc1ts_rprcode_exclu b,mesc1ts_repair_code c " &_
                //"where a.manual_cd = b.manual_cd " &_
                //"and a.repair_cd=b.repair_cd " &_
                //"and a.mode=b.mode " &_
                //"and b.manual_cd = c.manual_cd " &_
                //"and b.mode=c.mode " &_
                //"and b.exclu_repair_cd = c.repair_cd " &_
                //"order by a.manual_cd,a.mode,a.repair_Cd,b.exclu_repair_cd
                var WOrderMERCC01 = (from A in objContext.MESC1TS_REPAIR_CODE
                                     from B in objContext.MESC1TS_RPRCODE_EXCLU
                                     from C in objContext.MESC1TS_REPAIR_CODE
                                     where A.MANUAL_CD == B.MANUAL_CD &&
                                         A.REPAIR_CD == B.REPAIR_CD &&
                                         A.MODE == B.MODE &&
                                         B.MANUAL_CD == C.MANUAL_CD &&
                                         B.MODE == C.MODE &&
                                         B.EXCLU_REPAIR_CD == C.REPAIR_CD
                                     //orderby A.MANUAL_CD, A.MODE, A.REPAIR_CD, B.EXCLU_REPAIR_CD
                                     select new
                                     {
                                         A.MANUAL_CD, //manual
                                         A.MODE, //mode
                                         A.REPAIR_CD, //rpr
                                         A.REPAIR_DESC, //rpr_desc
                                         B.EXCLU_REPAIR_CD,//ex_rpr
                                         //Confusion
                                         ExclusionaryRepairCode = C.REPAIR_CD,  //ex_rpr_desc
                                         ExclusionaryRepairCodeDesc = C.REPAIR_DESC
                                     }).Distinct().OrderBy(manual => manual.MANUAL_CD).ThenBy(mode => mode.MODE).ThenBy(rep => rep.REPAIR_CD).ThenBy(exclu => exclu.EXCLU_REPAIR_CD);


                foreach (var obj in WOrderMERCC01)
                {
                    Reports = new Reports();
                    Reports.Mode = obj.MODE;
                    Reports.Manual = obj.MANUAL_CD;
                    Reports.RepairCod = obj.REPAIR_CD;
                    Reports.ExclusionaryRepairCode = obj.ExclusionaryRepairCode;
                    Reports.ExclusionaryRepairCodeDescription = obj.ExclusionaryRepairCodeDesc;
                    Reports.RepairCodeDesc = obj.REPAIR_DESC;
                    ReportsList.Add(Reports);
                }
                #endregion MercA03
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                ReportsList.Add(Reports);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ReportsList;
        }
        #endregion GetReportsDetailsMercC01

        #region GetReportsDetailsMercC02
        public List<Reports> GetReportsDetailsMercC02(out List<ErrMessage> ErrorMessageList)
        {
            List<Reports> ReportsList = new List<Reports>();
            Reports Reports = new Reports();
            ErrorMessageList = new List<ErrMessage>();

            try
            {
                #region MercA03
                //"	select a.manual_cd, a.mode, a.repair_cd, b.repair_desc, a.part_cd, c.part_desc " &_
                //"from mesc1ts_rprcode_part a, mesc1ts_repair_code b, mesc1ts_master_part c " &_
                //"where a.manual_cd = b.manual_cd and a.mode = b.mode and " &_
                //"a.repair_cd = b.repair_cd and a.PART_CD = c.part_cd order by a.mode, a.repair_cd, a.part_cd "

                var WOrderMERCC02 = (from A in objContext.MESC1TS_RPRCODE_PART
                                     from B in objContext.MESC1TS_REPAIR_CODE
                                     from C in objContext.MESC1TS_MASTER_PART
                                     where A.MANUAL_CD == B.MANUAL_CD &&
                                         A.MODE == B.MODE &&
                                         A.REPAIR_CD == B.REPAIR_CD &&
                                         A.PART_CD == C.PART_CD
                                     //orderby A.MODE, A.REPAIR_CD, A.PART_CD
                                     select new
                                     {
                                         A.MANUAL_CD, //manual
                                         A.MODE, //mode
                                         A.REPAIR_CD, //rpr
                                         B.REPAIR_DESC, //rpr_desc
                                         A.PART_CD,//ex_rpr
                                         C.PART_DESC //ex_rpr_desc
                                     }).Distinct().OrderBy(mode => mode.MODE).ThenBy(rep => rep.REPAIR_CD).ThenBy(part => part.PART_CD);


                foreach (var obj in WOrderMERCC02)
                {
                    Reports = new Reports();
                    Reports.Mode = obj.MODE;
                    Reports.Manual = obj.MANUAL_CD;
                    Reports.RepairCod = obj.REPAIR_CD;
                    Reports.RepairCodeDesc = obj.REPAIR_DESC;
                    Reports.PART_CD = obj.PART_CD;
                    Reports.PartDesc = obj.PART_DESC;
                    ReportsList.Add(Reports);
                }
                #endregion MercA03
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                ReportsList.Add(Reports);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ReportsList;
        }
        #endregion GetReportsDetailsMercC01

        #region GetReportsDetailsMercD03
        public List<Reports> GetReportsDetailsMercD03(string ShopCode, string CustomerCode, string ManualCode, string Mode, out List<ErrMessage> ErrorMessageList)
        {
            List<Reports> ReportsList = new List<Reports>();
            Reports Reports = new Reports();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            DateTime CurrentDate = DateTime.Now;
            string a = string.Empty;
            ErrorMessageList = new List<ErrMessage>();

            try
            {
                #region MercD03

                //var sqlMode = (from S in objContext.MESC1TS_SHOP
                //               from ST in objContext.MESC1TS_SHOP_CONT
                //               from CR in objContext.MESC1TS_CURRENCY
                //               from V in objContext.MESC1TS_VENDOR
                //               from RP in objContext.MESC1TS_REPAIR_CODE
                //               where S.SHOP_CD == ST.SHOP_CD &&
                //                   S.VENDOR_CD == V.VENDOR_CD &&
                //                   S.CUCDN == CR.CUCDN &&
                //                   ST.MANUAL_CD == RP.MANUAL_CD &&
                //                   ST.MODE == RP.MODE &&
                //                   ST.REPAIR_CD == RP.REPAIR_CD &&
                //                   ST.SHOP_CD == shopCode &&
                //                   ST.MANUAL_CD == ManualCode &&
                //                   ST.EFF_DTE <= CurrentDate &&
                //                   ST.EXP_DTE >= CurrentDate &&
                //                   (Mode == null ? a == a : ST.MODE == Mode)
                //               orderby ST.MODE //asc
                //               select new
                //               {
                //                   ST.MODE
                //               }).Distinct();
                //if (Mode != null)
                //{
                //    //If strMode<>"***Any***" then
                //    //    sSQL=sSQL & " and  ST.Mode = '" & strMode & "' "	
                //    //End if
                //    //sSQL=sSQL & "order by ST.MODE asc"	
                //}

                //if (Mode != null)
                //{
                var WOrderMERCD03 = (from S in objContext.MESC1TS_SHOP
                                     from ST in objContext.MESC1TS_SHOP_CONT
                                     from CR in objContext.MESC1TS_CURRENCY
                                     from V in objContext.MESC1TS_VENDOR
                                     from RP in objContext.MESC1TS_REPAIR_CODE
                                     where S.SHOP_CD == ST.SHOP_CD &&
                                         S.VENDOR_CD == V.VENDOR_CD &&
                                         S.CUCDN == CR.CUCDN &&
                                         ST.MANUAL_CD == RP.MANUAL_CD &&
                                         ST.MODE == RP.MODE &&
                                         ST.REPAIR_CD == RP.REPAIR_CD &&
                                         ST.SHOP_CD == ShopCode &&
                                         ST.MANUAL_CD == ManualCode &&
                                         ST.EFF_DTE <= CurrentDate &&
                                         ST.EXP_DTE >= CurrentDate &&
                                         (Mode == null ? a == a : ST.MODE == Mode)
                                     //orderby ST.MODE, ST.REPAIR_CD, ST.EFF_DTE
                                     select new
                                     {
                                         ST.MODE,
                                         ST.REPAIR_CD, //'Repair Code'
                                         RP.REPAIR_DESC, //Repair Code Description
                                         ST.CONTRACT_AMOUNT, //Max material amount pr piece
                                         ST.EFF_DTE, //Rate Effective Date
                                         ST.EXP_DTE //Rate expiry date
                                     }).Distinct().OrderBy(st => st.MODE).ThenBy(st => st.REPAIR_CD).ThenBy(st => st.EFF_DTE);

                var header = (from s in objContext.MESC1TS_SHOP
                              from v in objContext.MESC1TS_VENDOR
                              from c in objContext.MESC1TS_CURRENCY
                              where v.VENDOR_CD == s.VENDOR_CD &&
                                    s.CUCDN == c.CUCDN &&
                                    s.SHOP_CD == ShopCode
                              select new
                              {
                                  v.VENDOR_CD,
                                  v.VENDOR_DESC,
                                  s.SHOP_CD,
                                  s.SHOP_DESC,
                                  c.CUCDN,
                                  c.CURRNAMC
                              });

                foreach (var obj in WOrderMERCD03)
                {
                    Reports = new Reports();
                    //Reports.Mode = obj.MODE;
                    Reports.Mode = obj.MODE;
                    Reports.RepairCod = obj.REPAIR_CD;
                    Reports.RepairCodeDesc = obj.REPAIR_DESC;
                    Reports.MaxMaterialAmountPrPiece = obj.CONTRACT_AMOUNT.ToString();
                    Reports.RateEffectiveDate = obj.EFF_DTE.ToString();
                    Reports.RateExpiryDate = obj.EXP_DTE.ToString();
                    foreach (var item in header)
                    {
                        Reports.HeaderCurrencyCode = item.CUCDN;
                        Reports.HeaderCurrencyName = item.CURRNAMC;
                        Reports.HeaderVendorCode = item.VENDOR_CD;
                        Reports.HeaderVendorDesc = item.VENDOR_DESC;
                        Reports.HeaderShopCode = item.SHOP_CD;
                        Reports.HeaderShopDesc = item.SHOP_DESC;
                    }
                    ReportsList.Add(Reports);
                }
                //}
                #endregion MercD03
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                ReportsList.Add(Reports);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ReportsList;
        }
        #endregion GetReportsDetailsMercD03

        #region GetReportsDetailsMercD05
        public List<Reports> GetReportsDetailsMercD05(string ShopCode, string CustomerCode, string Mode, string Manual, string Country, out List<ErrMessage> ErrorMessageList)
        {
            List<Reports> ReportsList = new List<Reports>();
            Reports Reports = new Reports();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            string shopCode = string.Empty;
            DateTime CurrentDate = DateTime.Now;
            string a = string.Empty;
            ErrorMessageList = new List<ErrMessage>();

            try
            {
                //Setting the Header part, we need to know the country, date and the currency type
                List<MESC1TS_COUNTRY> CountryList = new List<MESC1TS_COUNTRY>();
                //CountryList = (from country in objContext.MESC1TS_COUNTRY select country).ToList();
                var country = from ctr in objContext.MESC1TS_COUNTRY
                              where ctr.COUNTRY_CD == Country
                              select new
                              {
                                  ctr.COUNTRY_DESC,
                              };
                //"select CUCDN,CURRNAMC,ROUND(100/EXRATUSD,6) 'Exchange Rate' from mesc1ts_currency 
                //where cucdn in (select cucdn from mesc1ts_country_cont where country_cd='" & strCountry & "')"
                //var currency = from curr in objContext.MESC1TS_CURRENCY
                //               where cucdn in (select cucdn from objContext.MESC1TS_COUNTRY_CONT where cou


                #region MercD05

                //var sqlMode = (from C in objContext.MESC1TS_COUNTRY
                //               from CT in objContext.MESC1TS_COUNTRY_CONT
                //               from CR in objContext.MESC1TS_CURRENCY
                //               from RC in objContext.MESC1TS_REPAIR_CODE
                //               where C.COUNTRY_CD == CT.COUNTRY_CD &&
                //                     CT.CUCDN == CR.CUCDN &&
                //                     CT.MANUAL_CD == RC.MANUAL_CD &&
                //                     CT.MODE == RC.MODE &&
                //                     CT.REPAIR_CD == RC.REPAIR_CD &&
                //                     CT.MANUAL_CD == Manual &&
                //                     C.COUNTRY_CD == Country &&
                //                     CT.EFF_DTE <= CurrentDate &&
                //                     CT.EXP_DTE >= CurrentDate &&
                //                     (Mode == null ? a == a : CT.MODE == Mode)
                //               orderby CT.MODE//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                //               select new
                //               {
                //                   CT.MODE
                //               }).Distinct();

                //mainSQL="select CT.Mode 'Mode', CT.Repair_CD 'Repair Code', REPAIR_DESC 'Repair Code Description',ROUND(CONTRACT_AMOUNT,2) 'Max material amount pr piece', 
                //(ROUND(CONTRACT_AMOUNT * EXRATUSD * .01,2)) 'Max material amount pr piece converted to USD', convert(char(10),EFF_DTE,120) 'Rate Effective Date',
                //convert(char(10),EXP_DTE,120) 'Rate expiry date' from mesc1ts_country C, mesc1ts_country_cont CT, mesc1ts_currency CR, mesc1ts_repair_code RC 
                //where C.COUNTRY_CD=CT.COUNTRY_CD and CT.CUCDN=CR.CUCDN and CT.MANUAL_CD=RC.MANUAL_CD and CT.MODE=RC.MODE and CT.REPAIR_CD=RC.REPAIR_CD and CT.MANUAL_CD='" & strManual & "' 
                //and C.COUNTRY_CD='" & strCountry & "' and EFF_DTE <= getdate() and EXP_DTE>=getdate() "

                var WOrderMercD05 = (from C in objContext.MESC1TS_COUNTRY
                                     from CT in objContext.MESC1TS_COUNTRY_CONT
                                     from CR in objContext.MESC1TS_CURRENCY
                                     from RC in objContext.MESC1TS_REPAIR_CODE
                                     where C.COUNTRY_CD == CT.COUNTRY_CD &&
                                         CT.CUCDN == CR.CUCDN &&
                                         CT.MANUAL_CD == RC.MANUAL_CD &&
                                         CT.MODE == RC.MODE &&
                                         CT.REPAIR_CD == RC.REPAIR_CD &&
                                         CT.MANUAL_CD == Manual &&
                                         C.COUNTRY_CD == Country &&
                                         CT.EFF_DTE <= CurrentDate &&
                                         CT.EXP_DTE >= CurrentDate &&
                                         (Mode == null ? a == a : CT.MODE == Mode)
                                     //CT.MODE == Mode
                                     //orderby CT.MODE, CT.REPAIR_CD, CT.EFF_DTE //asc
                                     select new
                                     {
                                         CT.MODE,
                                         CT.REPAIR_CD, //'Repair Code'
                                         RC.REPAIR_DESC, //Repair Code Description
                                         CT.CONTRACT_AMOUNT, //Max material amount pr piece
                                         CT.EFF_DTE, //Rate Effective Date
                                         CT.EXP_DTE, //Rate expiry date
                                         CR.EXRATUSD
                                     }).Distinct().OrderBy(ct => ct.MODE).ThenBy(ct => ct.REPAIR_CD).ThenBy(ct => ct.EFF_DTE);

                var header = (from s in objContext.MESC1TS_SHOP
                              from v in objContext.MESC1TS_VENDOR
                              from c in objContext.MESC1TS_CURRENCY
                              where v.VENDOR_CD == s.VENDOR_CD &&
                                    s.CUCDN == c.CUCDN &&
                                    s.SHOP_CD == ShopCode
                              select new
                              {
                                  v.VENDOR_CD,
                                  v.VENDOR_DESC,
                                  s.SHOP_CD,
                                  s.SHOP_DESC,
                                  c.CUCDN,
                                  c.CURRNAMC
                              });

                if (WOrderMercD05 != null && WOrderMercD05.Count() != 0)
                {
                    //lambda exp
                    foreach (var obj in WOrderMercD05)
                    {
                        Reports = new Reports();
                        //Reports.Mode = obj.MODE;
                        Reports.Mode = obj.MODE;
                        Reports.RepairCod = obj.REPAIR_CD;
                        Reports.RepairCodeDesc = obj.REPAIR_DESC;
                        Reports.MaxMaterialAmountPrPiece = Math.Round(obj.CONTRACT_AMOUNT, 2).ToString();  //'Max material amount pr piece'
                        Reports.MaxMaterialAmountPrPieceConvertedToUSD = (obj.EXRATUSD * obj.CONTRACT_AMOUNT * (int).01).ToString();
                        Reports.RateEffectiveDate = obj.EFF_DTE.ToString();
                        Reports.RateExpiryDate = obj.EXP_DTE.ToString();
                        Reports.ExchangeRate = obj.EXRATUSD.ToString();
                        foreach (var item in header)
                        {
                            Reports.HeaderCurrencyCode = item.CUCDN;
                            Reports.HeaderCurrencyName = item.CURRNAMC;
                        }
                        ReportsList.Add(Reports);
                    }
                }
                //}
                #endregion MercD05
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                ReportsList.Add(Reports);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ReportsList;
        }
        #endregion GetReportsDetailsMercD05

        #region GetReportsDetailsMercE01
        public List<Reports> GetReportsDetailsMercE01(string ShopCode, string CustomerCode, string Mode, string Manual, string CountryCode,
                                                        string Days, string AreaCode, out List<ErrMessage> ErrorMessageList)
        {
            //Get the List of countries
            List<Reports> ReportsList = new List<Reports>();
            Reports Reports = new Reports();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            DateTime CurrentDate = DateTime.Now;
            string a = string.Empty;
            ErrorMessageList = new List<ErrMessage>();
            bool isShop = false;
            bool isArea = false;
            bool isCountry = false;
            //string ModeCode = string.Empty;
            short startStatusCode = 100;
            short endStatusCode = 400;

            try
            {
                if (!string.IsNullOrEmpty(ShopCode))
                {
                    isShop = true;
                }
                else if (!string.IsNullOrEmpty(CountryCode))
                {
                    isCountry = true;
                }
                else
                {
                    isArea = true;
                }
                //"select distinct mode from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd 
                //and S.shop_cd = '" & strShop & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" and W.status_code >= 100 and W.status_code < 400 
                //and ST.status_code = W.status_code and W.manual_cd ='" & strManual &"' "
                //if (Mode == null || string.IsNullOrEmpty(Mode))
                //{
                //    if (isShop)
                //    {
                //        var sqlShop = (from W in objContext.MESC1TS_WO
                //                       from C in objContext.MESC1TS_COUNTRY
                //                       from L in objContext.MESC1TS_LOCATION
                //                       from S in objContext.MESC1TS_SHOP
                //                       from ST in objContext.MESC1TS_STATUS_CODE
                //                       where L.LOC_CD == S.LOC_CD &&
                //                             S.SHOP_CD == W.SHOP_CD &&
                //                             S.SHOP_CD == ShopCode &&
                //                             L.COUNTRY_CD == C.COUNTRY_CD &&
                //                             W.CRTS < DateTime.Now && //Pending
                //                             W.STATUS_CODE >= startStatusCode &&
                //                             W.STATUS_CODE < endStatusCode &&
                //                             ST.STATUS_CODE == W.STATUS_CODE &&
                //                             W.MANUAL_CD == Manual &&
                //                             (Mode == null ? a == a : W.MODE == Mode)
                //                       //orderby Mode//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                //                       select new
                //                       {
                //                           W.MODE,
                //                           W.REPAIR_DTE,
                //                           W.EQPNO
                //                       }).Distinct().OrderBy(m => m.MODE).ThenBy(dt => dt.REPAIR_DTE).ThenBy(eqp => eqp.EQPNO);

                //        foreach (var obj in sqlShop)
                //        {
                //            Mode = obj.MODE;
                //        }

                //        var header = (from s in objContext.MESC1TS_SHOP
                //                      from v in objContext.MESC1TS_VENDOR
                //                      from c in objContext.MESC1TS_CURRENCY
                //                      where v.VENDOR_CD == s.VENDOR_CD &&
                //                            s.CUCDN == c.CUCDN &&
                //                            s.SHOP_CD == ShopCode
                //                      select new
                //                      {
                //                          v.VENDOR_CD,
                //                          v.VENDOR_DESC,
                //                          s.SHOP_CD,
                //                          s.SHOP_DESC,
                //                          c.CUCDN,
                //                          c.CURRNAMC
                //                      });

                //    }
                //    else if (isCountry)
                //    {
                //        //"select distinct mode from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                //        //where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd and L.country_cd = '" & strCountry & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" 
                //        //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual &"' "
                //        var sqlCountry = (from W in objContext.MESC1TS_WO
                //                          from C in objContext.MESC1TS_COUNTRY
                //                          from L in objContext.MESC1TS_LOCATION
                //                          from S in objContext.MESC1TS_SHOP
                //                          from ST in objContext.MESC1TS_STATUS_CODE
                //                          where L.LOC_CD == S.LOC_CD &&
                //                                S.SHOP_CD == W.SHOP_CD &&
                //                                L.COUNTRY_CD == CountryCode &&
                //                                L.COUNTRY_CD == C.COUNTRY_CD &&
                //                                W.CRTS < DateTime.Now && //Pending
                //                                W.STATUS_CODE >= startStatusCode &&
                //                                W.STATUS_CODE < endStatusCode &&
                //                                ST.STATUS_CODE == W.STATUS_CODE &&
                //                                W.MANUAL_CD == Manual &&
                //                                (Mode == null ? a == a : W.MODE == Mode)
                //                          orderby Mode//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                //                          select new
                //                          {
                //                              W.MODE,
                //                              W.REPAIR_DTE,
                //                              W.EQPNO
                //                          }).Distinct().OrderBy(m => m.MODE).ThenBy(dt => dt.REPAIR_DTE).ThenBy(eqp => eqp.EQPNO);

                //        foreach (var obj in sqlCountry)
                //        {
                //            Mode = obj.MODE;
                //        }
                //    }
                //    else if (isArea)
                //    {
                //        //select distinct mode from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                //        //where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd and C.area_cd ='" & strArea & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" 
                //        //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual &"' "
                //        var sqlArea = (from W in objContext.MESC1TS_WO
                //                       from C in objContext.MESC1TS_COUNTRY
                //                       from L in objContext.MESC1TS_LOCATION
                //                       from S in objContext.MESC1TS_SHOP
                //                       from ST in objContext.MESC1TS_STATUS_CODE
                //                       where L.LOC_CD == S.LOC_CD &&
                //                             S.SHOP_CD == W.SHOP_CD &&
                //                             C.AREA_CD == AreaCode &&
                //                             L.COUNTRY_CD == C.COUNTRY_CD &&
                //                             W.CRTS < DateTime.Now && //Pending
                //                             W.STATUS_CODE >= startStatusCode &&
                //                             W.STATUS_CODE < endStatusCode &&
                //                             ST.STATUS_CODE == W.STATUS_CODE &&
                //                             W.MANUAL_CD == Manual &&
                //                             (Mode == null ? a == a : W.MODE == Mode)
                //                       //orderby Mode//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                //                       select new
                //                       {
                //                           W.MODE,
                //                           W.REPAIR_DTE,
                //                           W.EQPNO
                //                       }).Distinct().OrderBy(m => m.MODE).ThenBy(dt => dt.REPAIR_DTE).ThenBy(eqp => eqp.EQPNO); ;

                //        foreach (var obj in sqlArea)
                //        {
                //            Mode = obj.MODE;
                //        }
                //    }
                //}

                //If isShop then
                //mainSQL="select C.area_cd 'Area',C.country_cd 'Country',W.shop_cd 'Repair shop',W.mode as 'Mode',ST.status_dsc 'Status',w.eqpno 'Equipment No.'
                //,replace(convert(char(10),w.crts,120),'-','/') 'Estimate creation date',datediff(DAY,CRTS,getdate()) 'Days since creation',
                //replace(convert(char(10),W.approval_dte,120),'-','/') 'Estimate approval date',datediff(DAY,APPROVAL_DTE,getdate()) 'Days since approval',
                //replace(convert(char(10),W.chts,120),'-','/') 'Last change to estimate',datediff(DAY,W.CHTS,GetDate()) 'Days since last change',
                //ROUND(W.tot_repair_manh,2)'Estimated total hours',ROUND(W.TOT_COST_REPAIR_CPH,2) 'Estimated total cost of repair in USD(incl. CPH supplied parts)'
                //,W.vendor_ref_no 'Vendor Ref. No' 
                //from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                //where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd and S.shop_cd = '" & strShop & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" 
                //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual & "' and mode='" & mode & "' 
                //order by mode, year(crts),month(crts),day(crts),eqpno asc"

                #region MercEO1
                if (isShop)
                {
                    var WOrderMercEO1 = (from W in objContext.MESC1TS_WO
                                         from C in objContext.MESC1TS_COUNTRY
                                         from L in objContext.MESC1TS_LOCATION
                                         from S in objContext.MESC1TS_SHOP
                                         from ST in objContext.MESC1TS_STATUS_CODE
                                         where L.LOC_CD == S.LOC_CD &&
                                               S.SHOP_CD == W.SHOP_CD &&
                                               S.SHOP_CD == ShopCode &&
                                               L.COUNTRY_CD == C.COUNTRY_CD &&
                                             //W.CRTS < DateTime.Now &&
                                               W.STATUS_CODE >= startStatusCode &&
                                               W.STATUS_CODE < endStatusCode &&
                                               ST.STATUS_CODE == W.STATUS_CODE &&
                                               W.MANUAL_CD == Manual &&
                                               (Mode == null ? a == a : W.MODE == Mode)
                                         //orderby W.MODE//,  year(crts),month(crts),day(crts), EQPNO asc
                                         select new
                                         {
                                             C.AREA_CD, //Area
                                             C.COUNTRY_CD, //Country
                                             W.SHOP_CD, //Repair shop
                                             W.MODE, //as 'Mode'
                                             ST.STATUS_DSC, //'Status'
                                             W.EQPNO, //'Equipment No.
                                             W.CRTS, //Estimate creation date
                                             W.APPROVAL_DTE,
                                             W.CHTS,
                                             W.TOT_REPAIR_MANH,
                                             W.TOT_COST_REPAIR_CPH,
                                             W.VENDOR_REF_NO //Vendor Ref. No
                                         }).Distinct().OrderBy(m => m.MODE).ThenBy(dt => dt.CRTS).ThenBy(eqp => eqp.EQPNO);

                    if (WOrderMercEO1 != null && WOrderMercEO1.Count() != 0)
                    {
                        foreach (var obj in WOrderMercEO1)
                        {
                            DateTime tempCRTSDate = (DateTime)obj.CRTS;
                            DateTime date = tempCRTSDate.AddDays(Convert.ToInt32(Days));
                            if (date.Date < DateTime.Now.Date)
                            {
                                Reports = new Reports();
                                //Reports.Mode = obj.MODE;
                                Reports.Mode = obj.MODE;
                                Reports.AreaCode = obj.AREA_CD;
                                Reports.CountryCode = obj.COUNTRY_CD;
                                Reports.ShopCode = obj.SHOP_CD;
                                Reports.Status = obj.STATUS_DSC;
                                Reports.EquipmentNo = obj.EQPNO;
                                Reports.EstimateCreationDate = obj.CRTS.ToString();
                                //Reports.DaysSinceCreation = ;
                                Reports.EstimateApprovalDate = obj.APPROVAL_DTE.ToString();
                                //Reports.DaysSinceApproval = ;
                                Reports.LastChangeToEstimate = obj.CHTS.ToString();
                                //Reports.DaysSinceLastChange = ;
                                Reports.EstimatedTotalHours = Math.Round((decimal)obj.TOT_REPAIR_MANH, 2).ToString();
                                Reports.EstimatedTotalCostOfRepairInUSD = Math.Round((decimal)obj.TOT_COST_REPAIR_CPH, 2).ToString();
                                Reports.VendorRefNo = obj.VENDOR_REF_NO;
                                ReportsList.Add(Reports);
                            }

                        }
                    }
                }
                else if (isCountry)
                {
                    //select C.area_cd 'Area',C.country_cd 'Country',W.shop_cd 'Repair shop',W.mode as 'Mode',ST.status_dsc 'Status',w.eqpno 'Equipment No.'
                    //,replace(convert(char(10),w.crts,120),'-','/') 'Estimate creation date',datediff(DAY,CRTS,getdate()) 'Days since creation',
                    //replace(convert(char(10),W.approval_dte,120),'-','/') 'Estimate approval date',datediff(DAY,APPROVAL_DTE,getdate()) 'Days since approval',
                    //replace(convert(char(10),W.chts,120),'-','/') 'Last change to estimate',datediff(DAY,W.CHTS,GetDate()) 'Days since last change',
                    //ROUND(W.tot_repair_manh,2)'Estimated total hours',ROUND(W.TOT_COST_REPAIR_CPH,2) 'Estimated total cost of repair in USD(incl. CPH supplied parts)'
                    //,W.vendor_ref_no 'Vendor Ref. No' from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                    //where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd and L.country_cd = '" & strCountry & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" 
                    //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual & "' and mode='" & mode & "' 
                    //order by mode, year(crts),month(crts),day(crts),eqpno asc"
                    var WOrderMercEO1 = (from W in objContext.MESC1TS_WO
                                         from C in objContext.MESC1TS_COUNTRY
                                         from L in objContext.MESC1TS_LOCATION
                                         from S in objContext.MESC1TS_SHOP
                                         from ST in objContext.MESC1TS_STATUS_CODE
                                         where L.LOC_CD == S.LOC_CD &&
                                               S.SHOP_CD == W.SHOP_CD &&
                                               L.COUNTRY_CD == CountryCode &&
                                               L.COUNTRY_CD == C.COUNTRY_CD &&
                                               W.CRTS < DateTime.Now &&
                                               W.STATUS_CODE >= startStatusCode &&
                                               W.STATUS_CODE < endStatusCode &&
                                               ST.STATUS_CODE == W.STATUS_CODE &&
                                               W.MANUAL_CD == Manual &&
                                               (Mode == null ? a == a : W.MODE == Mode)
                                         //orderby W.MODE//,  year(crts),month(crts),day(crts), EQPNO asc
                                         select new
                                         {
                                             C.AREA_CD, //Area
                                             C.COUNTRY_CD, //Country
                                             W.SHOP_CD, //Repair shop
                                             W.MODE, //as 'Mode'
                                             ST.STATUS_DSC, //'Status'
                                             W.EQPNO, //'Equipment No.
                                             W.CRTS, //Estimate creation date
                                             W.APPROVAL_DTE,
                                             W.CHTS,
                                             W.TOT_REPAIR_MANH,
                                             W.TOT_COST_REPAIR_CPH,
                                             W.VENDOR_REF_NO //Vendor Ref. No
                                         }).Distinct().OrderBy(m => m.MODE);

                    if (WOrderMercEO1 != null && WOrderMercEO1.Count() != 0)
                    {
                        //lambda exp
                        foreach (var obj in WOrderMercEO1)
                        {
                            Reports = new Reports();
                            //Reports.Mode = obj.MODE;
                            Reports.Mode = obj.MODE;
                            Reports.AreaCode = obj.AREA_CD;
                            Reports.CountryCode = obj.COUNTRY_CD;
                            Reports.ShopCode = obj.SHOP_CD;
                            Reports.Status = obj.STATUS_DSC;
                            Reports.EquipmentNo = obj.EQPNO;
                            Reports.EstimateCreationDate = obj.CRTS.ToString();
                            //Reports.DaysSinceCreation = ;
                            Reports.EstimateApprovalDate = obj.APPROVAL_DTE.ToString();
                            //Reports.DaysSinceApproval = ;
                            Reports.LastChangeToEstimate = obj.CHTS.ToString();
                            //Reports.DaysSinceLastChange = ;
                            Reports.EstimatedTotalHours = Math.Round((decimal)obj.TOT_REPAIR_MANH, 2).ToString();
                            Reports.EstimatedTotalCostOfRepairInUSD = Math.Round((decimal)obj.TOT_COST_REPAIR_CPH, 2).ToString();
                            Reports.VendorRefNo = obj.VENDOR_REF_NO;
                            ReportsList.Add(Reports);
                        }
                    }
                }
                else if (isArea)
                {
                    //select C.area_cd 'Area',C.country_cd 'Country',W.shop_cd 'Repair shop',W.mode as 'Mode',ST.status_dsc 'Status',w.eqpno 'Equipment No.'
                    //,replace(convert(char(10),w.crts,120),'-','/') 'Estimate creation date',datediff(DAY,CRTS,getdate()) 'Days since creation',
                    //replace(convert(char(10),W.approval_dte,120),'-','/') 'Estimate approval date',datediff(DAY,APPROVAL_DTE,getdate()) 'Days since approval'
                    //,replace(convert(char(10),W.chts,120),'-','/') 'Last change to estimate',datediff(DAY,W.CHTS,GetDate()) 'Days since last change',
                    //ROUND(W.tot_repair_manh,2)'Estimated total hours',ROUND(W.TOT_COST_REPAIR_CPH,2) 'Estimated total cost of repair in USD(incl. CPH supplied parts)'
                    //,W.vendor_ref_no 'Vendor Ref. No' from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                    //where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd and   C.area_cd = '" & strArea & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" 
                    //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual & "' and mode='" & mode & "' 
                    //order by mode, year(crts),month(crts),day(crts),eqpno asc"
                    var WOrderMercEO1 = (from W in objContext.MESC1TS_WO
                                         from C in objContext.MESC1TS_COUNTRY
                                         from L in objContext.MESC1TS_LOCATION
                                         from S in objContext.MESC1TS_SHOP
                                         from ST in objContext.MESC1TS_STATUS_CODE
                                         where L.LOC_CD == S.LOC_CD &&
                                               S.SHOP_CD == W.SHOP_CD &&
                                               C.AREA_CD == AreaCode &&
                                               L.COUNTRY_CD == C.COUNTRY_CD &&
                                               W.CRTS < DateTime.Now &&
                                               W.STATUS_CODE >= startStatusCode &&
                                               W.STATUS_CODE < endStatusCode &&
                                               ST.STATUS_CODE == W.STATUS_CODE &&
                                               W.MANUAL_CD == Manual &&
                                               (Mode == null ? a == a : W.MODE == Mode)
                                         //orderby W.MODE//,  year(crts),month(crts),day(crts), EQPNO asc
                                         select new
                                         {
                                             C.AREA_CD, //Area
                                             C.COUNTRY_CD, //Country
                                             W.SHOP_CD, //Repair shop
                                             W.MODE, //as 'Mode'
                                             ST.STATUS_DSC, //'Status'
                                             W.EQPNO, //'Equipment No.
                                             W.CRTS, //Estimate creation date
                                             //datediff(DAY,CRTS,getdate()) 'Days since creation'
                                             W.APPROVAL_DTE, //replace(convert(char(10),w.crts,120),'-','/') Estimate approval date
                                             //datediff(DAY,APPROVAL_DTE,getdate()) 'Days since approval'
                                             W.CHTS, //Last change to estimate
                                             //datediff(DAY,W.CHTS,GetDate()) 'Days since last change'
                                             W.TOT_REPAIR_MANH, //'Estimated total hours
                                             W.TOT_COST_REPAIR_CPH, //Estimated total cost of repair in USD(incl. CPH supplied parts)
                                             W.VENDOR_REF_NO //Vendor Ref. No
                                         }).Distinct().OrderBy(m => m.MODE);

                    if (WOrderMercEO1 != null && WOrderMercEO1.Count() != 0)
                    {
                        //lambda exp
                        foreach (var obj in WOrderMercEO1)
                        {
                            Reports = new Reports();
                            //Reports.Mode = obj.MODE;
                            Reports.Mode = obj.MODE;
                            Reports.AreaCode = obj.AREA_CD;
                            Reports.CountryCode = obj.COUNTRY_CD;
                            Reports.ShopCode = obj.SHOP_CD;
                            Reports.Status = obj.STATUS_DSC;
                            Reports.EquipmentNo = obj.EQPNO;
                            Reports.EstimateCreationDate = obj.CRTS.ToString();
                            //Reports.DaysSinceCreation = ;
                            Reports.EstimateApprovalDate = obj.APPROVAL_DTE.ToString();
                            //Reports.DaysSinceApproval = ;
                            Reports.LastChangeToEstimate = obj.CHTS.ToString();
                            //Reports.DaysSinceLastChange = ;
                            Reports.EstimatedTotalHours = Math.Round((decimal)obj.TOT_REPAIR_MANH, 2).ToString();
                            Reports.EstimatedTotalCostOfRepairInUSD = Math.Round((decimal)obj.TOT_COST_REPAIR_CPH, 2).ToString();
                            Reports.VendorRefNo = obj.VENDOR_REF_NO;
                            ReportsList.Add(Reports);
                        }
                    }
                }

                //}
                #endregion MercE01
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity";
                ErrorMessageList.Add(ErrorMessages);
                ReportsList.Add(Reports);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ReportsList;
        }
        #endregion GetReportsDetailsMercE01

        #region GetReportsDetailsMercE02
        public List<Reports> GetReportsDetailsMercE02(string ShopCode, string CustomerCode, string Mode, string Manual, string CountryCode,
                                                        string Days, string AreaCode, out List<ErrMessage> ErrorMessageList)
        {
            List<Reports> ReportsList = new List<Reports>();
            Reports Reports = new Reports();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            DateTime CurrentDate = DateTime.Now;
            string a = string.Empty;
            bool isShop = false;
            bool isCountry = false;
            //DateTime CRTS = DateTime.v + (Days);
            short startStatusCode = 100;
            short endStatusCode = 400;
            //string ModeCode = string.Empty;
            ErrorMessageList = new List<ErrMessage>();

            try
            {
                if (!string.IsNullOrEmpty(ShopCode))
                {
                    isShop = true;
                }
                else if (!string.IsNullOrEmpty(CountryCode))
                {
                    isCountry = true;
                }

                //"select distinct mode from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd 
                //and S.shop_cd = '" & strShop & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" and W.status_code >= 100 and W.status_code < 400 
                //and ST.status_code = W.status_code and W.manual_cd ='" & strManual &"' "
                //if (Mode == null || string.IsNullOrEmpty(Mode))
                //{
                //    if (isShop)
                //    {
                //        var sqlShop = (from W in objContext.MESC1TS_WO
                //                       from C in objContext.MESC1TS_COUNTRY
                //                       from L in objContext.MESC1TS_LOCATION
                //                       from S in objContext.MESC1TS_SHOP
                //                       from ST in objContext.MESC1TS_STATUS_CODE
                //                       where L.LOC_CD == S.LOC_CD &&
                //                             S.SHOP_CD == W.SHOP_CD &&
                //                             S.SHOP_CD == ShopCode &&
                //                             L.COUNTRY_CD == C.COUNTRY_CD &&
                //                             W.CRTS < DateTime.Now && //Pending
                //                             W.STATUS_CODE >= startStatusCode &&
                //                             W.STATUS_CODE < endStatusCode &&
                //                             ST.STATUS_CODE == W.STATUS_CODE &&
                //                             W.MANUAL_CD == Manual &&
                //                             (Mode == null ? a == a : W.MODE == Mode)
                //                       //orderby Mode//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                //                       select new
                //                       {
                //                           W.MODE,
                //                           W.REPAIR_DTE,
                //                           W.EQPNO
                //                       }).Distinct().OrderBy(m => m.MODE).ThenBy(dt => dt.REPAIR_DTE).ThenBy(eqp => eqp.EQPNO);

                //        foreach (var obj in sqlShop)
                //        {
                //            Mode = obj.MODE;
                //        }

                //        var header = (from s in objContext.MESC1TS_SHOP
                //                      from v in objContext.MESC1TS_VENDOR
                //                      from c in objContext.MESC1TS_CURRENCY
                //                      where v.VENDOR_CD == s.VENDOR_CD &&
                //                            s.CUCDN == c.CUCDN &&
                //                            s.SHOP_CD == ShopCode
                //                      select new
                //                      {
                //                          v.VENDOR_CD,
                //                          v.VENDOR_DESC,
                //                          s.SHOP_CD,
                //                          s.SHOP_DESC,
                //                          c.CUCDN,
                //                          c.CURRNAMC
                //                      });

                //    }
                //    else if (isCountry)
                //    {
                //        //"select distinct mode from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                //        //where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd and L.country_cd = '" & strCountry & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" 
                //        //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual &"' "
                //        var sqlCountry = (from W in objContext.MESC1TS_WO
                //                          from C in objContext.MESC1TS_COUNTRY
                //                          from L in objContext.MESC1TS_LOCATION
                //                          from S in objContext.MESC1TS_SHOP
                //                          from ST in objContext.MESC1TS_STATUS_CODE
                //                          where L.LOC_CD == S.LOC_CD &&
                //                                S.SHOP_CD == W.SHOP_CD &&
                //                                L.COUNTRY_CD == CountryCode &&
                //                                L.COUNTRY_CD == C.COUNTRY_CD &&
                //                                W.CRTS < DateTime.Now && //Pending
                //                                W.STATUS_CODE >= startStatusCode &&
                //                                W.STATUS_CODE < endStatusCode &&
                //                                ST.STATUS_CODE == W.STATUS_CODE &&
                //                                W.MANUAL_CD == Manual &&
                //                                (Mode == null ? a == a : W.MODE == Mode)
                //                          //orderby Mode//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                //                          select new
                //                          {
                //                              W.MODE,
                //                              W.REPAIR_DTE,
                //                              W.EQPNO
                //                          }).Distinct().OrderBy(m => m.MODE).ThenBy(dt => dt.REPAIR_DTE).ThenBy(eqp => eqp.EQPNO);

                //        foreach (var obj in sqlCountry)
                //        {
                //            Mode = obj.MODE;
                //        }
                //    }
                //}

                //If isShop then
                //mainSQL="select C.country_cd 'Country',W.shop_cd 'Repair shop',W.mode 'Mode',ST.status_dsc 'Status',w.eqpno 'Equipment', 
                //replace(convert(char(10),w.crts,120),'-','/') 'Estimate creation date', datediff(DAY,CRTS,getdate())'Days since creation', 
                //replace(convert(char(10),W.approval_dte,120),'-','/') 'Estimate approval date', datediff(DAY,APPROVAL_DTE,getdate()) 'Days since approval', 
                //replace(convert(char(10),W.chts,120),'-','/') 'Last change to estimate', datediff(DAY,w.CHTS,GetDate()) 'Days since last change', 
                //round(W.tot_repair_manh,2) 'Estimated total hours', ROUND(TOTAL_COST_LOCAL_USD,2) 'Estimated total cost to be paid to shop', 
                //ROUND(W.TOT_COST_REPAIR_CPH,2) 'Estimated total cost of repair ,(incl. CPH supplied parts)', W.vendor_ref_no 'Vendor Ref. No' 
                //from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                //where L.Loc_cd = S.loc_cd and L.country_cd = C.country_cd and S.Shop_cd = W.shop_cd and S.shop_cd = '" & strShop & "' and crts < getdate() - "& strDays &" 
                //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual & "' and mode='" & mode & "' 
                //order by mode, year(crts),month(crts),day(crts),eqpno asc"

                #region MercEO2
                if (isShop)
                {
                    var WorkOrderMercEO2 = (from W in objContext.MESC1TS_WO
                                            from C in objContext.MESC1TS_COUNTRY
                                            from L in objContext.MESC1TS_LOCATION
                                            from S in objContext.MESC1TS_SHOP
                                            from ST in objContext.MESC1TS_STATUS_CODE
                                            where L.LOC_CD == S.LOC_CD &&
                                                  S.SHOP_CD == W.SHOP_CD &&
                                                  S.SHOP_CD == ShopCode &&
                                                  L.COUNTRY_CD == C.COUNTRY_CD &&
                                                  W.CRTS < DateTime.Now &&
                                                  W.STATUS_CODE >= startStatusCode &&
                                                  W.STATUS_CODE < endStatusCode &&
                                                  ST.STATUS_CODE == W.STATUS_CODE &&
                                                  W.MANUAL_CD == Manual &&
                                                  (Mode == null ? a == a : W.MODE == Mode)
                                            //orderby W.MODE//,  year(crts),month(crts),day(crts), EQPNO asc
                                            select new
                                            {
                                                C.AREA_CD, //Area
                                                C.COUNTRY_CD, //Country
                                                W.SHOP_CD, //Repair shop
                                                W.MODE, //as 'Mode'
                                                ST.STATUS_DSC, //'Status'
                                                W.EQPNO, //'Equipment No.
                                                W.CRTS, //Estimate creation date
                                                W.APPROVAL_DTE,
                                                W.CHTS,
                                                W.TOT_REPAIR_MANH,
                                                W.TOT_COST_REPAIR_CPH,
                                                W.VENDOR_REF_NO //Vendor Ref. No
                                            }).Distinct().OrderBy(m => m.MODE).ThenBy(dt => dt.CRTS).ThenBy(eqp => eqp.EQPNO);

                    if (WorkOrderMercEO2 != null && WorkOrderMercEO2.Count() != 0)
                    {
                        //lambda exp
                        foreach (var obj in WorkOrderMercEO2)
                        {
                            Reports = new Reports();
                            //Reports.Mode = obj.MODE;
                            Reports.Mode = obj.MODE;
                            Reports.AreaCode = obj.AREA_CD;
                            Reports.CountryCode = obj.COUNTRY_CD;
                            Reports.ShopCode = obj.SHOP_CD;
                            Reports.Status = obj.STATUS_DSC;
                            Reports.EquipmentNo = obj.EQPNO;
                            Reports.EstimateCreationDate = obj.CRTS.ToString();
                            //Reports.DaysSinceCreation = 
                            Reports.EstimateApprovalDate = obj.APPROVAL_DTE.ToString();
                            //Reports.DaysSinceApproval = ;
                            Reports.LastChangeToEstimate = obj.CHTS.ToString();
                            //Reports.DaysSinceLastChange = ;
                            Reports.EstimatedTotalHours = Math.Round((decimal)obj.TOT_REPAIR_MANH, 2).ToString();
                            Reports.EstimatedTotalCostOfRepairInUSD = Math.Round((decimal)obj.TOT_COST_REPAIR_CPH, 2).ToString();
                            Reports.VendorRefNo = obj.VENDOR_REF_NO;
                            ReportsList.Add(Reports);
                        }
                    }
                }
                else if (isCountry)
                {
                    //select C.area_cd 'Area',C.country_cd 'Country',W.shop_cd 'Repair shop',W.mode as 'Mode',ST.status_dsc 'Status',w.eqpno 'Equipment No.'
                    //,replace(convert(char(10),w.crts,120),'-','/') 'Estimate creation date',datediff(DAY,CRTS,getdate()) 'Days since creation',
                    //replace(convert(char(10),W.approval_dte,120),'-','/') 'Estimate approval date',datediff(DAY,APPROVAL_DTE,getdate()) 'Days since approval',
                    //replace(convert(char(10),W.chts,120),'-','/') 'Last change to estimate',datediff(DAY,W.CHTS,GetDate()) 'Days since last change',
                    //ROUND(W.tot_repair_manh,2)'Estimated total hours',ROUND(W.TOT_COST_REPAIR_CPH,2) 'Estimated total cost of repair in USD(incl. CPH supplied parts)'
                    //,W.vendor_ref_no 'Vendor Ref. No' from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                    //where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd and L.country_cd = '" & strCountry & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" 
                    //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual & "' and mode='" & mode & "' 
                    //order by mode, year(crts),month(crts),day(crts),eqpno asc"
                    var WorkOrderMercEO2 = (from W in objContext.MESC1TS_WO
                                            from C in objContext.MESC1TS_COUNTRY
                                            from L in objContext.MESC1TS_LOCATION
                                            from S in objContext.MESC1TS_SHOP
                                            from ST in objContext.MESC1TS_STATUS_CODE
                                            where L.LOC_CD == S.LOC_CD &&
                                                  S.SHOP_CD == W.SHOP_CD &&
                                                  L.COUNTRY_CD == CountryCode &&
                                                  L.COUNTRY_CD == C.COUNTRY_CD &&
                                                  W.CRTS < DateTime.Now &&
                                                  W.STATUS_CODE >= startStatusCode &&
                                                  W.STATUS_CODE < endStatusCode &&
                                                  ST.STATUS_CODE == W.STATUS_CODE &&
                                                  W.MANUAL_CD == Manual &&
                                                  (Mode == null ? a == a : W.MODE == Mode)
                                            //orderby W.MODE//,  year(crts),month(crts),day(crts), EQPNO asc
                                            select new
                                            {
                                                C.AREA_CD, //Area
                                                C.COUNTRY_CD, //Country
                                                W.SHOP_CD, //Repair shop
                                                W.MODE, //as 'Mode'
                                                ST.STATUS_DSC, //'Status'
                                                W.EQPNO, //'Equipment No.
                                                W.CRTS, //Estimate creation date
                                                W.APPROVAL_DTE,
                                                W.CHTS,
                                                W.TOT_REPAIR_MANH,
                                                W.TOT_COST_REPAIR_CPH,
                                                W.VENDOR_REF_NO //Vendor Ref. No
                                            }).Distinct().OrderBy(m => m.MODE).ThenBy(dt => dt.CRTS).ThenBy(eqp => eqp.EQPNO);

                    if (WorkOrderMercEO2 != null && WorkOrderMercEO2.Count() != 0)
                    {
                        //lambda exp
                        foreach (var obj in WorkOrderMercEO2)
                        {
                            DateTime tempCRTSDate = (DateTime)obj.CRTS;
                            DateTime date = tempCRTSDate.AddDays(Convert.ToInt32(Days));
                            {
                                Reports = new Reports();
                                //Reports.Mode = obj.MODE;
                                Reports.Mode = obj.MODE;
                                Reports.AreaCode = obj.AREA_CD;
                                Reports.CountryCode = obj.COUNTRY_CD;
                                Reports.ShopCode = obj.SHOP_CD;
                                Reports.Status = obj.STATUS_DSC;
                                Reports.EquipmentNo = obj.EQPNO;
                                Reports.EstimateCreationDate = obj.CRTS.ToString();
                                //Reports.DaysSinceCreation = ;
                                Reports.EstimateApprovalDate = obj.APPROVAL_DTE.ToString();
                                //Reports.DaysSinceApproval = ;
                                Reports.LastChangeToEstimate = obj.CHTS.ToString();
                                //Reports.DaysSinceLastChange = ;
                                Reports.EstimatedTotalHours = Math.Round((decimal)obj.TOT_REPAIR_MANH, 2).ToString();
                                Reports.EstimatedTotalCostOfRepairInUSD = Math.Round((decimal)obj.TOT_COST_REPAIR_CPH, 2).ToString();
                                Reports.VendorRefNo = obj.VENDOR_REF_NO;
                                ReportsList.Add(Reports);
                            }
                        }
                    }
                }

                #endregion MercE02
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                ReportsList.Add(Reports);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ReportsList;
        }
        #endregion GetReportsDetailsMercE02

        #region GetReportsDetailsMercE03
        public List<Reports> GetReportsDetailsMercE03(string ShopCode, string CustomerCode, string Mode, string Manual, string CountryCode,
                                                        string Days, string AreaCode, out List<ErrMessage> ErrorMessageList)
        {
            List<Reports> ReportsList = new List<Reports>();
            Reports Reports = new Reports();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            DateTime CurrentDate = DateTime.Now;
            string a = string.Empty;
            //DateTime CRTS = DateTime.v + (Days);
            short startStatusCode = 100;
            short endStatusCode = 400;
            //string ModeCode = string.Empty;
            ErrorMessageList = new List<ErrMessage>();

            try
            {
                //"select distinct mode from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd 
                //and S.shop_cd = '" & strShop & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" and W.status_code >= 100 and W.status_code < 400 
                //and ST.status_code = W.status_code and W.manual_cd ='" & strManual &"' "
                //if (Mode == null || string.IsNullOrEmpty(Mode))
                //{
                //    var sqlShop = (from W in objContext.MESC1TS_WO
                //                   from C in objContext.MESC1TS_COUNTRY
                //                   from L in objContext.MESC1TS_LOCATION
                //                   from S in objContext.MESC1TS_SHOP
                //                   from ST in objContext.MESC1TS_STATUS_CODE
                //                   where L.LOC_CD == S.LOC_CD &&
                //                           S.SHOP_CD == W.SHOP_CD &&
                //                           S.SHOP_CD == ShopCode &&
                //                           L.COUNTRY_CD == C.COUNTRY_CD &&
                //                           W.CRTS < DateTime.Now && //Pending
                //                           W.STATUS_CODE >= startStatusCode &&
                //                           W.STATUS_CODE < endStatusCode &&
                //                           ST.STATUS_CODE == W.STATUS_CODE &&
                //                           W.MANUAL_CD == Manual &&
                //                           (Mode == null ? a == a : W.MODE == Mode)
                //                   //orderby Mode//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                //                   select new
                //                   {
                //                       W.MODE,
                //                       W.REPAIR_DTE,
                //                       W.EQPNO
                //                   }).Distinct().OrderBy(m => m.MODE).ThenBy(dt => dt.REPAIR_DTE).ThenBy(eqp => eqp.EQPNO);

                //    foreach (var obj in sqlShop)
                //    {
                //        Mode = obj.MODE;
                //    }
                //}

                //var header = (from s in objContext.MESC1TS_SHOP
                //              from v in objContext.MESC1TS_VENDOR
                //              from c in objContext.MESC1TS_CURRENCY
                //              where v.VENDOR_CD == s.VENDOR_CD &&
                //                  s.CUCDN == c.CUCDN &&
                //                  s.SHOP_CD == ShopCode
                //              select new
                //              {
                //                  v.VENDOR_CD,
                //                  v.VENDOR_DESC,
                //                  s.SHOP_CD,
                //                  s.SHOP_DESC,
                //                  c.CUCDN,
                //                  c.CURRNAMC
                //              });




                //If isShop then
                //mainSQL="select C.country_cd 'Country',W.shop_cd 'Repair shop',W.mode 'Mode',ST.status_dsc 'Status',w.eqpno 'Equipment', 
                //replace(convert(char(10),w.crts,120),'-','/') 'Estimate creation date', datediff(DAY,CRTS,getdate())'Days since creation', 
                //replace(convert(char(10),W.approval_dte,120),'-','/') 'Estimate approval date', datediff(DAY,APPROVAL_DTE,getdate()) 'Days since approval', 
                //replace(convert(char(10),W.chts,120),'-','/') 'Last change to estimate', datediff(DAY,w.CHTS,GetDate()) 'Days since last change', 
                //round(W.tot_repair_manh,2) 'Estimated total hours', ROUND(TOTAL_COST_LOCAL_USD,2) 'Estimated total cost to be paid to shop', 
                //ROUND(W.TOT_COST_REPAIR_CPH,2) 'Estimated total cost of repair ,(incl. CPH supplied parts)', W.vendor_ref_no 'Vendor Ref. No' 
                //from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                //where L.Loc_cd = S.loc_cd and L.country_cd = C.country_cd and S.Shop_cd = W.shop_cd and S.shop_cd = '" & strShop & "' and crts < getdate() - "& strDays &" 
                //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual & "' and mode='" & mode & "' 
                //order by mode, year(crts),month(crts),day(crts),eqpno asc"

                #region MercEO3

                var WorkOrderMercEO3 = (from W in objContext.MESC1TS_WO
                                        from C in objContext.MESC1TS_COUNTRY
                                        from L in objContext.MESC1TS_LOCATION
                                        from S in objContext.MESC1TS_SHOP
                                        from ST in objContext.MESC1TS_STATUS_CODE
                                        where L.LOC_CD == S.LOC_CD &&
                                                S.SHOP_CD == W.SHOP_CD &&
                                                S.SHOP_CD == ShopCode &&
                                                L.COUNTRY_CD == C.COUNTRY_CD &&
                                                W.CRTS < DateTime.Now &&
                                                W.STATUS_CODE >= startStatusCode &&
                                                W.STATUS_CODE < endStatusCode &&
                                                ST.STATUS_CODE == W.STATUS_CODE &&
                                                W.MANUAL_CD == Manual &&
                                                (Mode == null ? a == a : W.MODE == Mode)
                                        //orderby W.MODE//,  year(crts),month(crts),day(crts), EQPNO asc
                                        select new
                                        {
                                            C.AREA_CD, //Area
                                            C.COUNTRY_CD, //Country
                                            W.SHOP_CD, //Repair shop
                                            W.MODE, //as 'Mode'
                                            ST.STATUS_DSC, //'Status'
                                            W.EQPNO, //'Equipment No.
                                            W.CRTS, //Estimate creation date
                                            W.APPROVAL_DTE,
                                            W.CHTS,
                                            W.TOT_REPAIR_MANH,
                                            W.TOT_COST_REPAIR_CPH,
                                            W.VENDOR_REF_NO //Vendor Ref. No
                                        }).Distinct().OrderBy(m => m.MODE).ThenBy(dt => dt.CRTS).ThenBy(eqp => eqp.EQPNO);

                if (WorkOrderMercEO3 != null && WorkOrderMercEO3.Count() != 0)
                {
                    //lambda exp
                    foreach (var obj in WorkOrderMercEO3)
                    {
                        DateTime tempCRTSDate = (DateTime)obj.CRTS;
                        DateTime date = tempCRTSDate.AddDays(Convert.ToInt32(Days));
                        if (date.Date < DateTime.Now.Date)
                        {
                            Reports = new Reports();
                            //Reports.Mode = obj.MODE;
                            Reports.Mode = obj.MODE;
                            Reports.AreaCode = obj.AREA_CD;
                            Reports.CountryCode = obj.COUNTRY_CD;
                            Reports.ShopCode = obj.SHOP_CD;
                            Reports.Status = obj.STATUS_DSC;
                            Reports.EquipmentNo = obj.EQPNO;
                            Reports.EstimateCreationDate = obj.CRTS.ToString();
                            //Reports.DaysSinceCreation = ;
                            Reports.EstimateApprovalDate = obj.APPROVAL_DTE.ToString();
                            //Reports.DaysSinceApproval = ;
                            Reports.LastChangeToEstimate = obj.CHTS.ToString();
                            //Reports.DaysSinceLastChange = ;
                            Reports.EstimatedTotalHours = Math.Round((decimal)obj.TOT_REPAIR_MANH, 2).ToString();
                            Reports.EstimatedTotalCostOfRepairInUSD = Math.Round((decimal)obj.TOT_COST_REPAIR_CPH, 2).ToString();
                            Reports.VendorRefNo = obj.VENDOR_REF_NO;
                            ReportsList.Add(Reports);
                        }
                    }
                }



                #endregion MercE03
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                ReportsList.Add(Reports);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ReportsList;
        }
        #endregion GetReportsDetailsMercE03



        #region GetShopList
        public List<Shop> GetShopList(out List<ErrMessage> ErrorMessageList, int UserID)
        {
            List<Shop> ShopList = new List<Shop>();
            List<MESC1TS_SHOP> ShopListFromDB = new List<MESC1TS_SHOP>();
            ErrorMessageList = new List<ErrMessage>();
            List<string> VendorCodeList = new List<string>();
            List<string> LocCodeList = new List<string>();
            List<string> CountryCodeList = new List<string>();
            List<string> ShopCodeList = new List<string>();
            List<string> AreaCodeList = new List<string>();
            List<MESC1VS_SHOP_LOCATION> ShopListFromDBOnAuth = null;
            try
            {
                //ShopListFromDB = (from S in objContext.MESC1TS_SHOP
                //                  join G in objContext.SEC_AUTHGROUP_USER on S.SHOP_CD equals G.COLUMN_VALUE
                //                  join A in objContext.SEC_AUTHGROUP on G.AUTHGROUP_ID equals A.AUTHGROUP_ID
                //                  where G.USER_ID == UserID &&
                //                  S.SHOP_ACTIVE_SW == "Y"
                //                  orderby S.SHOP_CD
                //                  select S).ToList();

                var ShopOnAuth = (from U in objContext.SEC_AUTHGROUP_USER
                                  from G in objContext.SEC_AUTHGROUP
                                  where U.USER_ID == UserID &&
                                  U.AUTHGROUP_ID == G.AUTHGROUP_ID
                                  select new
                                  {
                                      U.AUTHGROUP_ID,
                                      COLUMN_VALUE = U.COLUMN_VALUE,
                                      G.TABLE_NAME,
                                      COLUMN_NAME = G.COLUMN_NAME
                                  }).OrderBy(id => id.AUTHGROUP_ID).ToList();



                foreach (var item in ShopOnAuth)
                {
                    if (item.COLUMN_NAME == "VENDOR_CD")
                    {
                        string VendorCode = item.COLUMN_VALUE;
                        VendorCodeList.Add(VendorCode);
                    }
                    if (item.COLUMN_NAME == "LOC_CD")
                    {
                        string LocCode = item.COLUMN_VALUE;
                        LocCodeList.Add(LocCode);
                    }
                    if (item.COLUMN_NAME == "COUNTRY_CD")
                    {
                        string CountryCode = item.COLUMN_VALUE;
                        CountryCodeList.Add(CountryCode);
                    }
                    if (item.COLUMN_NAME == "AREA_CD")
                    {
                        string AreaCode = item.COLUMN_VALUE;
                        AreaCodeList.Add(AreaCode);
                    }
                    if (item.COLUMN_NAME == "SHOP_CD")
                    {
                        string ShopCode = item.COLUMN_VALUE;
                        ShopCodeList.Add(ShopCode);
                    }
                }
                ShopListFromDBOnAuth = new List<MESC1VS_SHOP_LOCATION>();
                ShopListFromDBOnAuth = (from shop in objContext.MESC1VS_SHOP_LOCATION
                                        where shop.SHOP_ACTIVE_SW == "Y" &&
                                        (VendorCodeList.Contains(shop.VENDOR_CD) ||
                                        LocCodeList.Contains(shop.LOC_CD) ||
                                        CountryCodeList.Contains(shop.COUNTRY_CD) ||
                                        AreaCodeList.Contains(shop.AREA_CD) ||
                                        ShopCodeList.Contains(shop.SHOP_CD))
                                        select shop).OrderBy(s => s.SHOP_CD).ToList();


                for (int count = 0; count < ShopListFromDBOnAuth.Count; count++)
                {
                    Shop Shop = new Shop();
                    Shop.ShopCode = ShopListFromDBOnAuth[count].SHOP_CD;
                    Shop.ShopDescription = ShopListFromDBOnAuth[count].SHOP_DESC;
                    ShopList.Add(Shop);
                }
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity. Error \n" + ex.Message;
                ErrorMessageList.Add(ErrorMessages);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ShopList;
        }
        #endregion GetShopList

        //#region GetModeList
        //public List<Mode> GetModeList()
        //{
        //    List<Mode> ModeList = new List<Mode>();
        //    List<ErrMessage> ErrorMessageList = new List<ErrMessage>();

        //    try
        //    {
        //        ModeList = ManageReportsDAL.GetModeList("", "", "");
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessages.Message = "Some Error has occurred while performing your activity.";
        //        ErrorMessageList.Add(ErrorMessages);
        //        logEntry.Message = ex.ToString();
        //        Logger.Write(logEntry);
        //    }
        //    return ModeList;
        //}
        //#endregion GetModeList

        #region GetManualList
        public List<Manual> GetManualList(out List<ErrMessage> ErrorMessageList)
        {
            List<Manual> ManualList = new List<Manual>();
            List<MESC1TS_MANUAL> ManualListFromDB = new List<MESC1TS_MANUAL>();
            ErrorMessageList = new List<ErrMessage>();

            try
            {
                ManualListFromDB = (from m in objContext.MESC1TS_MANUAL
                                    where m.MANUAL_ACTIVE_SW == "Y"
                                    //orderby m.MANUAL_CD
                                    select m).OrderBy(m => m.MANUAL_CD).ToList();

                for (int count = 0; count < ManualListFromDB.Count; count++)
                {
                    Manual Manual = new Manual();
                    Manual.ManualCode = ManualListFromDB[count].MANUAL_CD;
                    Manual.ManualDesc = ManualListFromDB[count].MANUAL_DESC;
                    ManualList.Add(Manual);
                }
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ManualList;
        }
        #endregion GetManualList

        #region GetCustomerList
        public List<Customer> GetCustomerList(out List<ErrMessage> ErrorMessageList)
        {
            List<Customer> CustomerList = new List<Customer>();
            List<MESC1TS_CUSTOMER> CustomerListFromDB = new List<MESC1TS_CUSTOMER>();
            ErrorMessageList = new List<ErrMessage>();
            try
            {
                CustomerListFromDB = (from customer in objContext.MESC1TS_CUSTOMER
                                      where customer.CUSTOMER_ACTIVE_SW == "Y"
                                      //orderby customer.CUSTOMER_CD
                                      select customer).OrderBy(m => m.CUSTOMER_CD).ToList();

                for (int count = 0; count < CustomerListFromDB.Count; count++)
                {
                    Customer Customer = new Customer();
                    Customer.CustomerCode = CustomerListFromDB[count].CUSTOMER_CD;
                    Customer.CustomerDesc = CustomerListFromDB[count].CUSTOMER_DESC;
                    CustomerList.Add(Customer);
                }
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return CustomerList;
        }
        #endregion GetCustomerList
        //Area_Bug_Fix_Debadrita
        #region GetAreaList
        public List<Area> GetAreaList(out List<ErrMessage> ErrorMessageList, int UserID, string Role)
        {
            List<Area> AreaList = new List<Area>();
            //List<MESC1TS_AREA> AreaListFromDB = new List<MESC1TS_AREA>();
            ErrorMessageList = new List<ErrMessage>();


            try
            {
                if (Role == "ADMIN" || Role == "CPH")
                {
                    var AreaListFromDB = (from A in objContext.MESC1TS_AREA
                                          join C in objContext.MESC1TS_COUNTRY on A.AREA_CD equals C.AREA_CD
                                          join L in objContext.MESC1TS_LOCATION on C.COUNTRY_CD equals L.COUNTRY_CD
                                          join U in objContext.SEC_AUTHGROUP_USER on A.AREA_CD equals U.COLUMN_VALUE
                                          join SA in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals SA.AUTHGROUP_ID
                                          where U.USER_ID == UserID
                                          orderby A.AREA_CD
                                          select new { A.AREA_CD, A.AREA_DESC }).Distinct().ToList();

                    for (int count = 0; count < AreaListFromDB.Count; count++)
                    {
                        Area Area = new Area();
                        Area.AreaCode = AreaListFromDB[count].AREA_CD;
                        Area.AreaDescription = AreaListFromDB[count].AREA_DESC;
                        AreaList.Add(Area);
                    }
                }
                else if (Role == "EMR_SPECIALIST_SHOP" || Role == "EMR_APPROVER_SHOP" || Role== "SHOP")
                {
                    var AreaListFromDB = (from A in objContext.MESC1TS_AREA
                                          join C in objContext.MESC1TS_COUNTRY on A.AREA_CD equals C.AREA_CD
                                          join L in objContext.MESC1TS_LOCATION on C.COUNTRY_CD equals L.COUNTRY_CD
                                          join S in objContext.MESC1TS_SHOP on L.LOC_CD equals S.LOC_CD
                                          join U in objContext.SEC_AUTHGROUP_USER on S.SHOP_CD equals U.COLUMN_VALUE
                                          join SA in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals SA.AUTHGROUP_ID
                                          where U.USER_ID == UserID
                                          orderby A.AREA_CD
                                          select new { A.AREA_CD, A.AREA_DESC }).Distinct().ToList();
                    for (int count = 0; count < AreaListFromDB.Count; count++)
                    {
                        Area Area = new Area();
                        Area.AreaCode = AreaListFromDB[count].AREA_CD;
                        Area.AreaDescription = AreaListFromDB[count].AREA_DESC;
                        AreaList.Add(Area);
                    }
                }

                else if (Role == "EMR_SPECIALIST_COUNTRY" || Role == "EMR_SPECIALIST_COUNTRY")
                {
                    var AreaListFromDB = (from A in objContext.MESC1TS_AREA
                                          join C in objContext.MESC1TS_COUNTRY on A.AREA_CD equals C.AREA_CD
                                          join U in objContext.SEC_AUTHGROUP_USER on C.COUNTRY_CD equals U.COLUMN_VALUE
                                          join SA in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals SA.AUTHGROUP_ID
                                          where U.USER_ID == UserID
                                          orderby A.AREA_CD
                                          select new { A.AREA_CD, A.AREA_DESC }).Distinct().ToList();
                    for (int count = 0; count < AreaListFromDB.Count; count++)
                    {
                        Area Area = new Area();
                        Area.AreaCode = AreaListFromDB[count].AREA_CD;
                        Area.AreaDescription = AreaListFromDB[count].AREA_DESC;
                        AreaList.Add(Area);
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return AreaList;
        }
        #endregion GetAreaList


        #region GetRepairList
        public List<RepairCode> GetRepairCodeList(string ShopCode, string CustomerCode, string ManualCode, string modeCode, out List<ErrMessage> ErrorMessageList)
        {
            int UserID = 10173;
            List<RepairCode> RepairList = new List<RepairCode>();
            List<MESC1TS_REPAIR_CODE> RepairListFromDB = new List<MESC1TS_REPAIR_CODE>();
            ErrorMessageList = new List<ErrMessage>();
            string a = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(modeCode))
                {
                    List<MESC1TS_MODE> Mode = (from m in objContext.MESC1TS_MODE
                                               from mm in objContext.MESC1TS_MANUAL_MODE
                                               from man in objContext.MESC1TS_MANUAL
                                               from cus in objContext.MESC1TS_CUSTOMER
                                               from cs in objContext.MESC1VS_CUST_SHOP
                                               from s in objContext.MESC1TS_SHOP
                                               where m.MODE == mm.MODE &&
                                                     mm.MANUAL_CD == man.MANUAL_CD &&
                                                     man.MANUAL_CD == cus.MANUAL_CD &&
                                                     cus.CUSTOMER_CD == cs.CUSTOMER_CD &&
                                                     cs.SHOP_CD == s.SHOP_CD &&
                                                     (ShopCode == null ? a == a : s.SHOP_CD == ShopCode) &&
                                                     (CustomerCode == null ? a == a : cus.CUSTOMER_CD == CustomerCode) &&
                                                     (ManualCode == null ? a == a : man.MANUAL_CD == ManualCode)
                                               //orderby m.MODE
                                               select m).Distinct().OrderBy(x => x.MODE).ToList();

                    if (Mode != null && Mode.Count > 0)
                    {
                        modeCode = Mode[0].MODE;
                    }
                }

                RepairListFromDB = (from R in objContext.MESC1TS_REPAIR_CODE
                                    from M in objContext.MESC1TS_MODE
                                    where R.MODE == M.MODE &&
                                    M.MODE == modeCode
                                    orderby R.REPAIR_CD
                                    select R).Distinct().OrderBy(r => r.REPAIR_CD).ToList();

                for (int count = 0; count < RepairListFromDB.Count; count++)
                {
                    RepairCode Repair = new RepairCode();
                    Repair.RepairCod = RepairListFromDB[count].REPAIR_CD;
                    Repair.RepairDesc = RepairListFromDB[count].REPAIR_DESC;
                    RepairList.Add(Repair);
                }
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return RepairList;
        }
        #endregion GetRepairList

        #region GetShopListOnCountryCode
        public List<Shop> GetShopListOnCountryCode(string CountryCode, out List<ErrMessage> ErrorMessageList)
        {
            List<Shop> ShopList = new List<Shop>();
            List<MESC1TS_SHOP> ShopListFromDB = new List<MESC1TS_SHOP>();
            ErrorMessageList = new List<ErrMessage>();

            try
            {
                ShopListFromDB = (from s in objContext.MESC1TS_SHOP
                                  from l in objContext.MESC1TS_LOCATION
                                  where s.LOC_CD == l.LOC_CD &&
                                      l.COUNTRY_CD == CountryCode
                                  select s).ToList();


                for (int count = 0; count < ShopListFromDB.Count; count++)
                {
                    Shop Shop = new Shop();
                    Shop.ShopCode = ShopListFromDB[count].SHOP_CD;
                    Shop.ShopDescription = ShopListFromDB[count].SHOP_DESC;
                    ShopList.Add(Shop);
                }
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ShopList;
        }
        #endregion GetShopListOnCountryCode

        #region GetModeListOnConditions
        public List<Mode> GetModeListOnConditions(string ShopCode, string CustomerCode, string ManualCode)
        {
            List<Mode> ModeList = new List<Mode>();
            List<MESC1TS_MODE> ModeListFromDB = new List<MESC1TS_MODE>();
            string a = string.Empty;
            List<ErrMessage> ErrorMessageList = new List<ErrMessage>();

            try
            {
                int UserID = 4;
                //"SELECT DISTINCT MESC1TS_MODE.MODE, MESC1TS_MODE.MODE + ' - ' + MODE_DESC AS DESCRIPTION"
                //strTables = " FROM MESC1TS_MODE, MESC1TS_MANUAL_MODE, MESC1TS_MANUAL, MESC1TS_CUSTOMER, MESC1VS_CUST_SHOP, MESC1TS_SHOP "
                //strWhere = " WHERE (MESC1TS_MODE.MODE = MESC1TS_MANUAL_MODE.MODE)" _ 
                //    & " AND (MESC1TS_MANUAL_MODE.MANUAL_CD = MESC1TS_MANUAL.MANUAL_CD)" _ 
                //    & " AND (MESC1TS_MANUAL.MANUAL_CD = MESC1TS_CUSTOMER.MANUAL_CD)" _ 
                //    & " AND (MESC1TS_CUSTOMER.CUSTOMER_CD = MESC1VS_CUST_SHOP.CUSTOMER_CD)" _ 
                //    & " AND (MESC1VS_CUST_SHOP.SHOP_CD = MESC1TS_SHOP.SHOP_CD)"


                ModeListFromDB = (from m in objContext.MESC1TS_MODE
                                  from mm in objContext.MESC1TS_MANUAL_MODE
                                  from man in objContext.MESC1TS_MANUAL
                                  from cus in objContext.MESC1TS_CUSTOMER
                                  from cs in objContext.MESC1VS_CUST_SHOP
                                  from s in objContext.MESC1TS_SHOP
                                  where m.MODE == mm.MODE &&
                                        mm.MANUAL_CD == man.MANUAL_CD &&
                                        man.MANUAL_CD == cus.MANUAL_CD &&
                                        cus.CUSTOMER_CD == cs.CUSTOMER_CD &&
                                        cs.SHOP_CD == s.SHOP_CD &&
                                        (ShopCode == null ? a == a : s.SHOP_CD == ShopCode) &&
                                        (CustomerCode == null ? a == a : cus.CUSTOMER_CD == CustomerCode) &&
                                        (ManualCode == null ? a == a : man.MANUAL_CD == ManualCode)
                                  //orderby m.MODE
                                  select m).Distinct().OrderBy(x => x.MODE).ToList();

                foreach (var item in ModeListFromDB)
                {
                    Mode Mode = new Mode();
                    Mode.ModeCode = item.MODE;
                    Mode.ModeDescription = item.MODE_DESC;
                    ModeList.Add(Mode);
                }
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ModeList;
        }
        #endregion GetModeListOnConditions

        //Country_populate_bug_fix_Debadrita
        #region GetCountryList
        public List<Country> GetCountryList(out List<ErrMessage> ErrorMessageList, int UserID, string Role)
        {
            List<Country> CountryList = new List<Country>();
            List<MESC1TS_COUNTRY> CountryListFromDB = new List<MESC1TS_COUNTRY>();
            ErrorMessageList = new List<ErrMessage>();

            try
            {
                //var RSAuthGroupByUID = (from u in objContext.SEC_AUTHGROUP_USER
                //                                where u.USER_ID == UserID
                //                                select new
                //                                {
                //                                    u.AUTHGROUP_ID
                //                                }).FirstOrDefault();

                //if (RSAuthGroupByUID != null)
                //{
                //    var RSAuthgroupByAuthgroupId = (from 
                //}

                //    SELECT DISTINCT C.COUNTRY_CD, C.COUNTRY_CD + ' - ' + C.COUNTRY_DESC AS DESCRIPTION"
                //strSQL = strSQL & " FROM MESC1TS_COUNTRY C INNER JOIN MESC1TS_LOCATION L"
                //strSQL = strSQL & " ON C.COUNTRY_CD = L.COUNTRY_CD"
                //strSQL = strSQL & " INNER JOIN SEC_AUTHGROUP_USER U"
                //strSQL = strSQL & " ON U.COLUMN_VALUE = L.LOC_CD"
                //strSQL = strSQL & " INNER JOIN SEC_AUTHGROUP A"
                //strSQL = strSQL & " ON A.AUTHGROUP_ID = U.AUTHGROUP_ID"
                //strSQL = strSQL & " WHERE USER_ID = " & sUserID
                //strSQL = strSQL & " ORDER BY C.COUNTRY_CD"

                if (Role == "MSL")
                {
                    CountryListFromDB = (from C in objContext.MESC1TS_COUNTRY
                                         join L in objContext.MESC1TS_LOCATION on C.COUNTRY_CD equals L.COUNTRY_CD
                                         join U in objContext.SEC_AUTHGROUP_USER on L.LOC_CD equals U.COLUMN_VALUE
                                         join A in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals A.AUTHGROUP_ID
                                         where U.USER_ID == UserID
                                         //orderby C.COUNTRY_CD
                                         select C).OrderBy(c => c.COUNTRY_CD).ToList();
                }
                // else if (Role == "AREA" || Role == "CPH" || Role == "ADMIN")
                else if (Role == "CPH" || Role == "ADMIN" || Role == "MPRO_CLUSTER" || Role == "READ_ONLY")
                {
                    CountryListFromDB = (from C in objContext.MESC1TS_COUNTRY
                                         join L in objContext.MESC1TS_AREA on C.AREA_CD equals L.AREA_CD
                                         join U in objContext.SEC_AUTHGROUP_USER on L.AREA_CD equals U.COLUMN_VALUE
                                         join A in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals A.AUTHGROUP_ID
                                         where U.USER_ID == UserID
                                         //orderby C.COUNTRY_CD
                                         select C).OrderBy(c => c.COUNTRY_CD).ToList();
                }
                else if (Role == "EMR_SPECIALIST_SHOP" || Role == "EMR_APPROVER_SHOP" || Role == "MPRO_SHOP")
                {
                    CountryListFromDB = (from C in objContext.MESC1TS_COUNTRY
                                         join L in objContext.MESC1TS_LOCATION on C.COUNTRY_CD equals L.COUNTRY_CD
                                         join S in objContext.MESC1TS_SHOP on L.LOC_CD equals S.LOC_CD
                                         join U in objContext.SEC_AUTHGROUP_USER on S.SHOP_CD equals U.COLUMN_VALUE
                                         join A in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals A.AUTHGROUP_ID
                                         where U.USER_ID == UserID
                                         //orderby C.COUNTRY_CD
                                         select C).OrderBy(c => c.COUNTRY_CD).ToList();
                }
                //else if (Role == "COUNTRY")
                else if (Role == "EMR_SPECIALIST_COUNTRY" || Role == "EMR_APPROVER_COUNTRY")
                {
                    CountryListFromDB = (from C in objContext.MESC1TS_COUNTRY
                                         join U in objContext.SEC_AUTHGROUP_USER on C.COUNTRY_CD equals U.COLUMN_VALUE
                                         join A in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals A.AUTHGROUP_ID
                                         where U.USER_ID == UserID
                                         //orderby C.COUNTRY_CD
                                         select C).OrderBy(c => c.COUNTRY_CD).ToList();
                }

                for (int count = 0; count < CountryListFromDB.Count; count++)
                {
                    Country Country = new Country();
                    Country.CountryCode = CountryListFromDB[count].COUNTRY_CD;
                    Country.CountryDescription = CountryListFromDB[count].COUNTRY_DESC;
                    CountryList.Add(Country);
                }
            }
            catch (Exception ex)
            {
                ErrorMessages.Message = "Some Error has occurred while performing your activity.";
                ErrorMessageList.Add(ErrorMessages);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return CountryList;
        }
        #endregion GetCountryList
    }
}
