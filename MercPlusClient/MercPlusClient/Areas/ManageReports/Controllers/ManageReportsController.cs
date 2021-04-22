using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MercPlusClient.ManageReportsServiceReference;
using MercPlusClient;
using System.IO;
using System.Web.UI;
using MercPlusClient.Areas.ManageReports.Models;
using MercPlusClient.UtilityClass;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Text;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace MercPlusClient.Areas.ManageReports.Controllers
{
    public class ManageReportsController : Controller
    {
        LogEntry logEntry = new LogEntry();
        ManageReportsModel ManageReportsModel = new ManageReportsModel();
        ManageReportsClient ManageReportsClient = new ManageReportsClient();
        List<SelectListItem> ReportsDropDownList = new List<SelectListItem>();
        List<Shop> Shop = new List<Shop>();
        string UTCDate = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm");
        string shop = string.Empty;
        string vendor = string.Empty;
        string dateFrom = string.Empty;
        string dateTo = string.Empty;
        string stsCode = string.Empty;
        string currency = string.Empty;
        string exchangeRate = string.Empty;
        string mode = string.Empty;
        string days = string.Empty;
        
        const string ANY = "***Any***";

        string Role = string.Empty;
        const string ADMIN = "ADMIN";
        const string CPH = "CPH";
        const string EMR_SPECIALIST_COUNTRY = "EMR_SPECIALIST_COUNTRY";
        const string EMR_SPECIALIST_SHOP = "EMR_SPECIALIST_SHOP";
        const string EMR_APPROVER_COUNTRY = "EMR_APPROVER_COUNTRY";
        const string EMR_APPROVER_SHOP = "EMR_APPROVER_SHOP";
        const string SHOP = "SHOP";
        const string MPRO_CLUSTER = "MPRO_CLUSTER";
        const string MPRO_SHOP = "MPRO_SHOP";
        const string READONLY = "READONLY";

        bool IsVoucherReqd = false;

        private void SetUserAccess()
        {
            ManageReportsModel.isAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            ManageReportsModel.isCPH = ((UserSec)Session["UserSec"]).isCPH;
            ManageReportsModel.isEMRSpecialistCountry = ((UserSec)Session["UserSec"]).isEMRSpecialistCountry;
            ManageReportsModel.isEMRSpecialistShop = ((UserSec)Session["UserSec"]).isEMRSpecialistShop;
            ManageReportsModel.isEMRApproverCountry = ((UserSec)Session["UserSec"]).isEMRApproverCountry;
            ManageReportsModel.isEMRApproverShop = ((UserSec)Session["UserSec"]).isEMRApproverShop;                     
            ManageReportsModel.isShop = ((UserSec)Session["UserSec"]).isShop;
            ManageReportsModel.isMPROCluster = ((UserSec)Session["UserSec"]).isMPROCluster;
            ManageReportsModel.isMPROShop = ((UserSec)Session["UserSec"]).isMPROShop;
            ManageReportsModel.isAnyCPH = ((UserSec)Session["UserSec"]).isAnyCPH;
            ManageReportsModel.isReadOnly = ((UserSec)Session["UserSec"]).isReadOnly;
                       
            if (ManageReportsModel.isAdmin) Role = ADMIN;
            if (ManageReportsModel.isAnyCPH || ManageReportsModel.isCPH) Role = CPH;
            if (ManageReportsModel.isEMRSpecialistCountry) Role = EMR_SPECIALIST_COUNTRY;
            if (ManageReportsModel.isEMRSpecialistShop) Role = EMR_SPECIALIST_SHOP;
            if (ManageReportsModel.isEMRApproverCountry) Role = EMR_APPROVER_COUNTRY;
            if (ManageReportsModel.isEMRApproverShop) Role = EMR_APPROVER_SHOP;
            if (ManageReportsModel.isShop) Role = SHOP;
            if (ManageReportsModel.isMPROCluster) Role = MPRO_CLUSTER;
            if (ManageReportsModel.isMPROShop) Role = MPRO_SHOP;
            if (ManageReportsModel.isReadOnly) Role = READONLY;
        }

        public ActionResult ManageReports()
        {
            //set the user access
            SetUserAccess();
            //Populate the Reports dropdown
            ReportsDropDownList = PopulateReportsDropDown(ReportsDropDownList);
            //ManageReportsModel.drpDays = PopulateDaysDropDown();
            ManageReportsModel.drpReports = ReportsDropDownList;
            return View(ManageReportsModel);
        }

        [HttpPost]
        public ActionResult ManageReports([Bind] ManageReportsModel ManageReportsModel)//, int? ReportsId)
        {
            List<Reports> ReportsList = new List<Reports>();
            List<ErrMessage> MercPlusErrorMessageList = new List<ErrMessage>();
            try
            {
                if (ManageReportsModel.Mode != null && ManageReportsModel.Mode.Equals(ANY, StringComparison.CurrentCultureIgnoreCase))
                {
                    ManageReportsModel.Mode = null;
                }
                if (ManageReportsModel.STSCode != null && ManageReportsModel.STSCode.Equals(ANY, StringComparison.CurrentCultureIgnoreCase))
                {
                    ManageReportsModel.STSCode = null;
                }
                if (ManageReportsModel.ReportsID == 1)
                {
                    ReportsList = ManageReportsClient.GetReportsDetailsMercA01(out MercPlusErrorMessageList, ManageReportsModel.DateFrom, ManageReportsModel.DateTo, ManageReportsModel.Shop, ManageReportsModel.Customer
                                                                        , ManageReportsModel.Mode, ManageReportsModel.Manual).ToList();
                    if (MercPlusErrorMessageList.Count > 0)
                    {
                        ManageReportsModel.ReportsList = new List<Reports>();
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                        return View(ManageReportsModel);
                    }
                    else if (ReportsList != null)
                    {
                        ExportClientsListToExcelMERCA01(ReportsList, ManageReportsModel);
                    }
                }
                else if (ManageReportsModel.ReportsID == 2)
                {
                    ReportsList = ManageReportsClient.GetReportsDetailsMercA02(out MercPlusErrorMessageList, ManageReportsModel.DateFrom, ManageReportsModel.DateTo, ManageReportsModel.Shop, ManageReportsModel.Customer
                                                                        , ManageReportsModel.Mode, ManageReportsModel.Manual).ToList();
                    if (MercPlusErrorMessageList.Count > 0)
                    {
                        ManageReportsModel.ReportsList = new List<Reports>();
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                        return View(ManageReportsModel);
                    }
                    else if (ReportsList != null)
                    {
                        ExportClientsListToExcelMERCA02(ReportsList, ManageReportsModel);
                    }
                }
                else if (ManageReportsModel.ReportsID == 3)
                {
                    ReportsList = ManageReportsClient.GetReportsDetailsMercA03(out MercPlusErrorMessageList, ManageReportsModel.DateFrom, ManageReportsModel.DateTo, ManageReportsModel.Shop, ManageReportsModel.Customer
                                                                        , ManageReportsModel.Mode, ManageReportsModel.Manual).ToList();
                    if (MercPlusErrorMessageList.Count > 0)
                    {
                        ManageReportsModel.ReportsList = new List<Reports>();
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                        return View(ManageReportsModel);
                    }
                    else if (ReportsList != null)
                    {
                        ExportClientsListToExcelMERCA03(ReportsList, ManageReportsModel);
                    }
                }
                else if (ManageReportsModel.ReportsID == 4)
                {
                    ReportsList = ManageReportsClient.GetReportsDetailsMercB01(out MercPlusErrorMessageList, ManageReportsModel.DateFrom, ManageReportsModel.DateTo, ManageReportsModel.Shop, ManageReportsModel.Customer
                                                  , ManageReportsModel.Mode, ManageReportsModel.Manual, ManageReportsModel.STSCode).ToList();
                    if (MercPlusErrorMessageList.Count > 0)
                    {
                        ManageReportsModel.ReportsList = new List<Reports>();
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                        return View(ManageReportsModel);
                    }
                    else if (ReportsList != null)
                    {
                        ExportClientsListToExcelMERCBO1(ReportsList, ManageReportsModel);
                    }
                }
                else if (ManageReportsModel.ReportsID == 5) // PDF to Excel
                {
                    ReportsList = ManageReportsClient.GetReportsDetailsMercC01(out MercPlusErrorMessageList).ToList();
                    if (MercPlusErrorMessageList.Count > 0)
                    {
                        ManageReportsModel.ReportsList = new List<Reports>();
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                        return View(ManageReportsModel);
                    }
                    else if (ReportsList != null)
                    {
                        ExportClientsListToExcelMERCDO5_1(ReportsList, ManageReportsModel);
                    }
                }
                //else if (ManageReportsModel.ReportsID == 5)
                //{
                //    ReportsList = ManageReportsClient.GetReportsDetailsMercC01(out MercPlusErrorMessageList).ToList();
                //    if (MercPlusErrorMessageList.Count > 0)
                //    {
                //        ManageReportsModel.ReportsList = new List<Reports>();
                //        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                //        return View(ManageReportsModel);
                //    }
                //    else
                //    {
                //        ManageReportsModel.ReportsList = ReportsList;
                //    }
                //    return new Rotativa.ViewAsPdf("PDFMercC01", ManageReportsModel) { FileName = "MERCC01.pdf" };
                //}
                else if (ManageReportsModel.ReportsID == 6) // PDF to excel
                {
                    ReportsList = ManageReportsClient.GetReportsDetailsMercC02(out MercPlusErrorMessageList).ToList();
                    if (MercPlusErrorMessageList.Count > 0)
                    {
                        ManageReportsModel.ReportsList = new List<Reports>();
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                        return View(ManageReportsModel);
                    }
                    else if (ReportsList != null)
                    {
                        ExportClientsListToExcelMERCDO6_1(ReportsList, ManageReportsModel);
                    }
                }
                //else if (ManageReportsModel.ReportsID == 6)
                //{
                //    ReportsList = ManageReportsClient.GetReportsDetailsMercC02(out MercPlusErrorMessageList).ToList();
                //    if (MercPlusErrorMessageList.Count > 0)
                //    {
                //        ManageReportsModel.ReportsList = new List<Reports>();
                //        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                //        return View(ManageReportsModel);
                //    }
                //    else
                //    {
                //        ManageReportsModel.ReportsList = ReportsList;
                //    }
                //    return new Rotativa.ViewAsPdf("PDFMercC02", ManageReportsModel) { FileName = "MERCC02.pdf" };
                //}
                else if (ManageReportsModel.ReportsID == 7)
                {
                    ReportsList = ManageReportsClient.GetReportsDetailsMercD03(out MercPlusErrorMessageList, ManageReportsModel.Shop, ManageReportsModel.Customer
                                                                        , ManageReportsModel.Manual, ManageReportsModel.Mode).ToList();
                    if (MercPlusErrorMessageList.Count > 0)
                    {
                        ManageReportsModel.ReportsList = new List<Reports>();
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                        return View(ManageReportsModel);
                    }
                    else if (ReportsList != null)
                    {
                        ExportClientsListToExcelMERCDO3(ReportsList, ManageReportsModel);
                    }
                }
                else if (ManageReportsModel.ReportsID == 8)
                {
                    ReportsList = ManageReportsClient.GetReportsDetailsMercD05(out MercPlusErrorMessageList, ManageReportsModel.Shop, ManageReportsModel.Customer
                                                                        , ManageReportsModel.Mode, ManageReportsModel.Manual, ManageReportsModel.Country).ToList();
                    if (MercPlusErrorMessageList.Count > 0)
                    {
                        ManageReportsModel.ReportsList = new List<Reports>();
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                        return View(ManageReportsModel);
                    }
                    else if (ReportsList != null)
                    {
                        ExportClientsListToExcelMERCDO5(ReportsList, ManageReportsModel);
                    }
                }
                else if (ManageReportsModel.ReportsID == 9)
                {
                    ReportsList = ManageReportsClient.GetReportsDetailsMercE01(out MercPlusErrorMessageList, ManageReportsModel.Shop, ManageReportsModel.Customer
                                                                        , ManageReportsModel.Mode, ManageReportsModel.Manual, ManageReportsModel.Country, ManageReportsModel.Days, ManageReportsModel.Area).ToList();
                    if (MercPlusErrorMessageList.Count > 0)
                    {
                        ManageReportsModel.ReportsList = new List<Reports>();
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                        return View(ManageReportsModel);
                    }
                    else if (ReportsList != null)
                    {
                        ExportClientsListToExcelMERCE01(ReportsList, ManageReportsModel);
                    }
                }
                else if (ManageReportsModel.ReportsID == 10)
                {
                    ReportsList = ManageReportsClient.GetReportsDetailsMercE02(out MercPlusErrorMessageList, ManageReportsModel.Shop, ManageReportsModel.Customer
                                                                        , ManageReportsModel.Mode, ManageReportsModel.Manual, ManageReportsModel.Country, ManageReportsModel.Days, ManageReportsModel.Area).ToList();
                    if (MercPlusErrorMessageList.Count > 0)
                    {
                        ManageReportsModel.ReportsList = new List<Reports>();
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                        return View(ManageReportsModel);
                    }
                    else if (ReportsList != null)
                    {
                        ExportClientsListToExcelMERCE02(ReportsList, ManageReportsModel);
                    }
                }
                else if (ManageReportsModel.ReportsID == 11)
                {
                    ReportsList = ManageReportsClient.GetReportsDetailsMercE03(out MercPlusErrorMessageList, ManageReportsModel.Shop, ManageReportsModel.Customer
                                                                        , ManageReportsModel.Mode, ManageReportsModel.Manual, ManageReportsModel.Country, ManageReportsModel.Days, ManageReportsModel.Area).ToList();
                    if (MercPlusErrorMessageList.Count > 0)
                    {
                        ManageReportsModel.ReportsList = new List<Reports>();
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(MercPlusErrorMessageList[0].Message, "Warning");
                        return View(ManageReportsModel);
                    }
                    else if (ReportsList != null)
                    {
                        ExportClientsListToExcelMERCE03(ReportsList, ManageReportsModel);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return View(ManageReportsModel);

        }

        public void ExportClientsListToExcelMERCA01(List<Reports> ReportsList, ManageReportsModel ManageReportsModel)
        {
            System.Web.UI.WebControls.GridView grid = new System.Web.UI.WebControls.GridView();
            DataTable dt = new DataTable();
            if ((ReportsList != null && ReportsList.Count > 0) && !string.IsNullOrEmpty(ReportsList[0].VoucherNumber))
            {
                IsVoucherReqd = true;
                dt.Columns.Add("Mode");
                dt.Columns.Add("Actual Completion Date");
                dt.Columns.Add("Equipment No");
                dt.Columns.Add("WorkOrder No");
                dt.Columns.Add("VendorRef No");
                dt.Columns.Add("Voucher Number");
                dt.Columns.Add("Agent Code");
                dt.Columns.Add("Ordinary Man Hours");
                dt.Columns.Add("Overtime1 Man Hours");
                dt.Columns.Add("Overtime2 Man Hours");
                dt.Columns.Add("Overtime3 Man Hours");
                dt.Columns.Add("Total Hours");
                dt.Columns.Add("Total Labour Cost");
                dt.Columns.Add("Total Cost Of Shop Supplied Numbered Part");
                dt.Columns.Add("Total Cost Of Shop Supplied Materials");
                dt.Columns.Add("Import Tax");
                dt.Columns.Add("Sales Tax Parts");
                dt.Columns.Add("Sales Tax Labour");
                dt.Columns.Add("Total To Be Paid To Shop/Agent");
                dt.Columns.Add("Total Cost Of CPH Supplied Parts");
                dt.Columns.Add("Total Cost Of Repair (incl. CPH supplied parts)");

                for (int i = 0; i < ReportsList.Count; i++)
                {
                    object[] dtRow = new object[21];
                    //dtRow = dt.Rows[0];
                    dtRow[0] = ReportsList[i].Mode;
                    dtRow[1] = ReportsList[i].ActualCompletionDate;
                    dtRow[2] = ReportsList[i].EquipmentNo;
                    dtRow[3] = ReportsList[i].WorkOrderNo;
                    dtRow[4] = ReportsList[i].VendorRefNo;
                    dtRow[5] = ReportsList[i].VoucherNumber;
                    dtRow[6] = ReportsList[i].AgentCode;
                    dtRow[7] = ReportsList[i].OrdinaryManHours;
                    dtRow[8] = ReportsList[i].Overtime1ManHours;
                    dtRow[9] = ReportsList[i].Overtime2ManHours;
                    dtRow[10] = ReportsList[i].Overtime3ManHours;
                    dtRow[11] = ReportsList[i].TotalHours;
                    dtRow[12] = ReportsList[i].TotalLabourCostCPH;
                    dtRow[13] = ReportsList[i].TotalCostOfShopSuppliedNumberedParts;
                    dtRow[14] = ReportsList[i].TotalCostOfShopSuppliedMaterials;
                    dtRow[15] = ReportsList[i].ImportTax;
                    dtRow[16] = ReportsList[i].SalesTaxParts;
                    dtRow[17] = ReportsList[i].SalesTaxLabour;
                    dtRow[18] = ReportsList[i].TotalToBePaidToShop;
                    dtRow[19] = ReportsList[i].TotalCostOfCPHSuppliedParts;
                    dtRow[20] = ReportsList[i].TotalCostOfRepairCPH;
                    dt.Rows.Add(dtRow);
                }
            }
            else
            {
                IsVoucherReqd = false;

                dt.Columns.Add("Mode");
                dt.Columns.Add("Actual Completion Date");
                dt.Columns.Add("Equipment No");
                dt.Columns.Add("WorkOrder No");
                dt.Columns.Add("VendorRef No");
                dt.Columns.Add("Ordinary Man Hours");
                dt.Columns.Add("Overtime1 Man Hours");
                dt.Columns.Add("Overtime2 Man Hours");
                dt.Columns.Add("Overtime3 Man Hours");
                dt.Columns.Add("Total Hours");
                dt.Columns.Add("Total Labour Cost");
                dt.Columns.Add("Total Cost Of Shop Supplied Numbered Part");
                dt.Columns.Add("Total Cost Of Shop Supplied Materials");
                dt.Columns.Add("Import Tax");
                dt.Columns.Add("Sales Tax Parts");
                dt.Columns.Add("Sales Tax Labour");
                dt.Columns.Add("Total To Be Paid To Shop/Agent");
                dt.Columns.Add("Total Cost Of CPH Supplied Parts");
                dt.Columns.Add("Total Cost Of Repair (incl. CPH supplied parts)");

                for (int i = 0; i < ReportsList.Count; i++)
                {
                    object[] dtRow = new object[19];
                    dtRow[0] = ReportsList[i].Mode;
                    dtRow[1] = ReportsList[i].ActualCompletionDate;
                    dtRow[2] = ReportsList[i].EquipmentNo;
                    dtRow[3] = ReportsList[i].WorkOrderNo;
                    dtRow[4] = ReportsList[i].VendorRefNo;
                    dtRow[5] = ReportsList[i].OrdinaryManHours;
                    dtRow[6] = ReportsList[i].Overtime1ManHours;
                    dtRow[7] = ReportsList[i].Overtime2ManHours;
                    dtRow[8] = ReportsList[i].Overtime3ManHours;
                    dtRow[9] = ReportsList[i].TotalHours;
                    dtRow[10] = ReportsList[i].TotalLabourCostCPH;
                    dtRow[11] = ReportsList[i].TotalCostOfShopSuppliedNumberedParts;
                    dtRow[12] = ReportsList[i].TotalCostOfShopSuppliedMaterials;
                    dtRow[13] = ReportsList[i].ImportTax;
                    dtRow[14] = ReportsList[i].SalesTaxParts;
                    dtRow[15] = ReportsList[i].SalesTaxLabour;
                    dtRow[16] = ReportsList[i].TotalToBePaidToShop;
                    dtRow[17] = ReportsList[i].TotalCostOfCPHSuppliedParts;
                    dtRow[18] = ReportsList[i].TotalCostOfRepairCPH;
                    dt.Rows.Add(dtRow);
                }
            }

            WriteExcelWithNPOI(dt, "xlsx", ManageReportsModel, ReportsList);
        }

        public void ExportClientsListToExcelMERCA02(List<Reports> ReportsList, ManageReportsModel ManageReportsModel)
        {
            System.Web.UI.WebControls.GridView grid = new System.Web.UI.WebControls.GridView();
            DataTable dt = new DataTable();

            dt.Columns.Add("Mode");
            dt.Columns.Add("Actual Completion Date");
            dt.Columns.Add("Equipment No");
            dt.Columns.Add("WorkOrder No");
            dt.Columns.Add("VendorRef No");
            dt.Columns.Add("Voucher Number");
            dt.Columns.Add("Agent Code");
            dt.Columns.Add("Exchange Rate");
            dt.Columns.Add("Ordinary Man Hours");
            dt.Columns.Add("Overtime 1 Man Hours");
            dt.Columns.Add("Overtime 2 Man Hours");
            dt.Columns.Add("Overtime 3 Man Hours");
            dt.Columns.Add("Total Hours");
            dt.Columns.Add("Total Labour Cost");
            dt.Columns.Add("Part Supplier");
            dt.Columns.Add("Total Cost of Numbered Parts");
            dt.Columns.Add("Total Cost Of Supplied Materials");
            dt.Columns.Add("Import Tax");
            dt.Columns.Add("Sales Tax Parts");
            dt.Columns.Add("Sales Tax Labour");
            dt.Columns.Add("Total To Be Paid To Shop");
            dt.Columns.Add("Total To Be Paid To Shop In USD");
            dt.Columns.Add("Total To Be Paid To Agent From CPH In Local Currency");
            dt.Columns.Add("Total To Be Paid To Agent From CPH In USD");
            dt.Columns.Add("Total Cost Of CPH Supplied Parts In USD");
            dt.Columns.Add("Total Cost Of Repair CPH (incl. CPH supplied parts) in USD");

            for (int i = 0; i < ReportsList.Count; i++)
            {
                object[] dtRow = new object[26];
                //dtRow = dt.Rows[0];
                dtRow[0] = ReportsList[i].Mode;
                dtRow[1] = ReportsList[i].ActualCompletionDate;
                dtRow[2] = ReportsList[i].EquipmentNo;
                dtRow[3] = ReportsList[i].WorkOrderNo;
                dtRow[4] = ReportsList[i].VendorRefNo;
                dtRow[5] = ReportsList[i].VoucherNumber;
                dtRow[6] = ReportsList[i].AgentCode;
                dtRow[7] = ReportsList[i].ExchangeRate;
                dtRow[8] = ReportsList[i].OrdinaryManHours;
                dtRow[9] = ReportsList[i].Overtime1ManHours;
                dtRow[10] = ReportsList[i].Overtime2ManHours;
                dtRow[11] = ReportsList[i].Overtime3ManHours;
                dtRow[12] = ReportsList[i].TotalHours;
                dtRow[13] = ReportsList[i].TotalLabourCost;
                dtRow[14] = ReportsList[i].PartSupplier;
                dtRow[15] = ReportsList[i].TotalCostOfNumberedParts;
                dtRow[16] = ReportsList[i].TotalCostOfSuppliedMaterials;
                dtRow[17] = ReportsList[i].ImportTax;
                dtRow[18] = ReportsList[i].SalesTaxParts;
                dtRow[19] = ReportsList[i].SalesTaxLabour;
                dtRow[20] = ReportsList[i].TotalToBePaidToShop;
                dtRow[21] = ReportsList[i].TotalToBePaidToShopInUSD;
                dtRow[22] = ReportsList[i].TotalToBePaidToAgentFromCPHInLocalCurrency;
                dtRow[23] = ReportsList[i].TotalToBePaidToAgentFromCPHInUSD;
                dtRow[24] = ReportsList[i].TotalCostOfCPHSuppliedPartsInUSD;
                dtRow[25] = ReportsList[i].TotalCostOfRepairCPH;
                dt.Rows.Add(dtRow);
            }

            WriteExcelWithNPOI(dt, "xlsx", ManageReportsModel, ReportsList);
        }

        public void ExportClientsListToExcelMERCA03(List<Reports> ReportsList, ManageReportsModel ManageReportsModel)
        {
            System.Web.UI.WebControls.GridView grid = new System.Web.UI.WebControls.GridView();

            DataTable dt = new DataTable();
            dt.Columns.Add("Mode");
            dt.Columns.Add("Actual Completion Date");
            dt.Columns.Add("Equipment No");
            dt.Columns.Add("WorkOrder No");
            dt.Columns.Add("VendorRef No");
            dt.Columns.Add("Ordinary Man Hours");
            dt.Columns.Add("Overtime1 Man Hours");
            dt.Columns.Add("Overtime2 Man Hours");
            dt.Columns.Add("Overtime3 Man Hours");
            dt.Columns.Add("Total Hours");
            dt.Columns.Add("Total Labour Cost");
            dt.Columns.Add("PartSupplier");
            dt.Columns.Add("TotalCostOfNumberedParts");
            dt.Columns.Add("TotalCostOfShopSuppliedMaterials");
            dt.Columns.Add("Import Tax");
            dt.Columns.Add("Sales Tax Parts");
            dt.Columns.Add("Sales Tax Labour");
            dt.Columns.Add("Total To Be Paid To Shop");

            for (int i = 0; i < ReportsList.Count; i++)
            {
                object[] dtRow = new object[18];
                //dtRow = dt.Rows[0];
                dtRow[0] = ReportsList[i].Mode;
                dtRow[1] = ReportsList[i].ActualCompletionDate;
                dtRow[2] = ReportsList[i].EquipmentNo;
                dtRow[3] = ReportsList[i].WorkOrderNo;
                dtRow[4] = ReportsList[i].VendorRefNo;
                dtRow[5] = ReportsList[i].OrdinaryManHours;
                dtRow[6] = ReportsList[i].Overtime1ManHours;
                dtRow[7] = ReportsList[i].Overtime2ManHours;
                dtRow[8] = ReportsList[i].Overtime3ManHours;
                dtRow[9] = ReportsList[i].TotalHours;
                dtRow[10] = ReportsList[i].TotalLabourCost;
                dtRow[11] = ReportsList[i].PartSupplier;
                dtRow[12] = ReportsList[i].TotalCostOfNumberedParts;
                dtRow[13] = ReportsList[i].TotalCostOfSuppliedMaterials;
                dtRow[14] = ReportsList[i].ImportTax;
                dtRow[15] = ReportsList[i].SalesTaxParts;
                dtRow[16] = ReportsList[i].SalesTaxLabour;
                dtRow[17] = ReportsList[i].TotalToBePaidToShop;
                dt.Rows.Add(dtRow);
            }

            WriteExcelWithNPOI(dt, "xlsx", ManageReportsModel, ReportsList);
        }

        public void ExportClientsListToExcelMERCBO1(List<Reports> ReportsList, ManageReportsModel ManageReportsModel)
        {
            System.Web.UI.WebControls.GridView grid = new System.Web.UI.WebControls.GridView();

            DataTable dt = new DataTable();
            dt.Columns.Add("Mode");
            dt.Columns.Add("Equipment No");
            dt.Columns.Add("Actual Completion Date");
            dt.Columns.Add("WorkOrder No");
            dt.Columns.Add("VendorRef No");
            dt.Columns.Add("Repair Code");
            dt.Columns.Add("Repair Code Description");
            dt.Columns.Add("Part Number");
            dt.Columns.Add("Part Numbe rDescription");
            dt.Columns.Add("Manufacturer");
            dt.Columns.Add("Part Quantity");
            dt.Columns.Add("Total Cost Of Parts");
            dt.Columns.Add("Maersk Sealand CPH SuppliedPart");
            dt.Columns.Add("Core Part");
            dt.Columns.Add("Core Serial Number Tag Number");

            for (int i = 0; i < ReportsList.Count; i++)
            {
                object[] dtRow = new object[15];
                dtRow[0] = ReportsList[i].Mode;
                dtRow[1] = ReportsList[i].EquipmentNo;
                dtRow[2] = ReportsList[i].ActualCompletionDate;
                dtRow[3] = ReportsList[i].WorkOrderNo;
                dtRow[4] = ReportsList[i].VendorRefNo;
                dtRow[5] = ReportsList[i].RepairCod;
                dtRow[6] = ReportsList[i].RepairCodeDesc;
                dtRow[7] = ReportsList[i].PART_CD;
                dtRow[8] = ReportsList[i].PartDesc;
                dtRow[9] = ReportsList[i].ManufacturerName;
                dtRow[10] = ReportsList[i].QuantityParts;
                dtRow[11] = ReportsList[i].CostLocal;
                dtRow[12] = ReportsList[i].MSLPartSW;
                dtRow[13] = ReportsList[i].CorePartSW;
                dtRow[14] = ReportsList[i].SerialNumber;
                dt.Rows.Add(dtRow);
            }

            WriteExcelWithNPOI(dt, "xlsx", ManageReportsModel, ReportsList);
        }

        public void ExportClientsListToExcelMERCDO3(List<Reports> ReportsList, ManageReportsModel ManageReportsModel)
        {
            System.Web.UI.WebControls.GridView grid = new System.Web.UI.WebControls.GridView();
            DataTable dt = new DataTable();
            dt.Columns.Add("Mode");
            dt.Columns.Add("Repair Code");
            dt.Columns.Add("Repair Description");
            dt.Columns.Add("Max Mat Amt");
            dt.Columns.Add("Rate Eff Date");
            dt.Columns.Add("Rate Exp Date");

            for (int i = 0; i < ReportsList.Count; i++)
            {
                object[] dtRow = new object[6];
                //dtRow = dt.Rows[0];
                dtRow[0] = ReportsList[i].Mode;
                dtRow[1] = ReportsList[i].RepairCod;
                dtRow[2] = ReportsList[i].RepairCodeDesc;
                dtRow[3] = ReportsList[i].MaxMaterialAmountPrPiece;
                dtRow[4] = ReportsList[i].RateEffectiveDate;
                dtRow[5] = ReportsList[i].RateExpiryDate;
                dt.Rows.Add(dtRow);
            }

            WriteExcelWithNPOI(dt, "xlsx", ManageReportsModel, ReportsList);
        }

        public void ExportClientsListToExcelMERCDO5(List<Reports> ReportsList, ManageReportsModel ManageReportsModel)
        {
            System.Web.UI.WebControls.GridView grid = new System.Web.UI.WebControls.GridView();
            DataTable dt = new DataTable();
            dt.Columns.Add("Mode");
            dt.Columns.Add("Repair Code");
            dt.Columns.Add("Repair Description");
            dt.Columns.Add("Max Mat Amt");
            dt.Columns.Add("Max Mat Amt USD");
            dt.Columns.Add("Rate Eff Date");
            dt.Columns.Add("Rate Exp Date");

            for (int i = 0; i < ReportsList.Count; i++)
            {
                object[] dtRow = new object[7];
                //dtRow = dt.Rows[0];
                dtRow[0] = ReportsList[i].Mode;
                dtRow[1] = ReportsList[i].RepairCod;
                dtRow[2] = ReportsList[i].RepairDesc;
                dtRow[3] = ReportsList[i].ContractAmount;
                dtRow[4] = (Math.Round(Convert.ToInt32(ReportsList[i].ContractAmount) * (ReportsList[i].ExratUSD) * .01, 2));
                dtRow[5] = ReportsList[i].RateEffectiveDate;
                dtRow[6] = ReportsList[i].RateExpiryDate;
                dt.Rows.Add(dtRow);
            }

            WriteExcelWithNPOI(dt, "xlsx", ManageReportsModel, ReportsList);
        }

        public void ExportClientsListToExcelMERCE01(List<Reports> ReportsList, ManageReportsModel ManageReportsModel)
        {
            var grid = new System.Web.UI.WebControls.GridView();
            DateTime date = DateTime.Now;
            DataTable dt = new DataTable();

            dt.Columns.Add("COCL");
            dt.Columns.Add("Country");
            dt.Columns.Add("Repair Shop");
            dt.Columns.Add("Mode");
            dt.Columns.Add("Status");
            dt.Columns.Add("Equipment No");
            dt.Columns.Add("Estimate Creation Date");
            dt.Columns.Add("Days Since Creation");
            dt.Columns.Add("Estimate Approval Date");
            dt.Columns.Add("Days Since Approval");
            dt.Columns.Add("Last Change To Estimate");
            dt.Columns.Add("Days Since Last Change");
            dt.Columns.Add("EstimatedTotalHours");
            dt.Columns.Add("Estimated Total Cost Of Repair In USD");
            dt.Columns.Add("Vendor Reference No");

            for (int i = 0; i < ReportsList.Count; i++)
            {
                DateTime tempCrtsDate = DateTime.MinValue;
                DateTime tempEstAppDate = DateTime.MinValue;
                DateTime tempChangeEstDate = DateTime.MinValue;

                object[] dtRow = new object[15];
                //dtRow = dt.Rows[0];
                dtRow[0] = ReportsList[i].AreaCode;
                dtRow[1] = ReportsList[i].CountryCode;
                dtRow[2] = ReportsList[i].ShopCode;
                dtRow[3] = ReportsList[i].Mode;
                dtRow[4] = ReportsList[i].Status;
                dtRow[5] = ReportsList[i].EquipmentNo;
                dtRow[6] = ReportsList[i].EstimateCreationDate;
                if (!string.IsNullOrEmpty(ReportsList[i].EstimateCreationDate))
                {
                    tempCrtsDate = Convert.ToDateTime(ReportsList[i].EstimateCreationDate);
                    dtRow[7] = (date).Subtract(tempCrtsDate).Days;
                }
                else
                {
                    dtRow[7] = ReportsList[i].EstimateCreationDate;
                }
                dtRow[8] = ReportsList[i].EstimateApprovalDate;
                if (!string.IsNullOrEmpty(ReportsList[i].EstimateApprovalDate))
                {
                    tempEstAppDate = Convert.ToDateTime(ReportsList[i].EstimateApprovalDate);
                    dtRow[9] = (date).Subtract(tempEstAppDate).Days;
                }
                else
                {
                    dtRow[9] = ReportsList[i].EstimateApprovalDate;
                }
                dtRow[10] = ReportsList[i].LastChangeToEstimate;
                if (!string.IsNullOrEmpty(ReportsList[i].LastChangeToEstimate))
                {
                    tempChangeEstDate = Convert.ToDateTime(ReportsList[i].LastChangeToEstimate);
                    dtRow[11] = (date).Subtract(tempEstAppDate).Days;
                }
                else
                {
                    dtRow[11] = ReportsList[i].LastChangeToEstimate;
                }
                dtRow[12] = ReportsList[i].EstimatedTotalHours;
                dtRow[13] = ReportsList[i].EstimatedTotalCostOfRepairInUSD;
                dtRow[14] = ReportsList[i].VendorRefNo;
                dt.Rows.Add(dtRow);
            }

            WriteExcelWithNPOI(dt, "xlsx", ManageReportsModel, ReportsList);
        }

        public void ExportClientsListToExcelMERCE02(List<Reports> ReportsList, ManageReportsModel ManageReportsModel)
        {
            var grid = new System.Web.UI.WebControls.GridView();
            DateTime date = DateTime.Now;
            DataTable dt = new DataTable();
            days = ManageReportsModel.Days;

            dt.Columns.Add("Country");
            dt.Columns.Add("Repair Shop");
            dt.Columns.Add("Mode");
            dt.Columns.Add("Status");
            dt.Columns.Add("Equipment No");
            dt.Columns.Add("Estimate Creation Date");
            dt.Columns.Add("Days Since Creation");
            dt.Columns.Add("Estimate Approval Date");
            dt.Columns.Add("Days Since Approval");
            dt.Columns.Add("Last Change To Estimate");
            dt.Columns.Add("Days Since Last Change");
            dt.Columns.Add("EstimatedTotalHours");
            dt.Columns.Add("Estimated Total Cost Of Repair In USD");
            dt.Columns.Add("Vendor Reference No");

            for (int i = 0; i < ReportsList.Count; i++)
            {
                DateTime tempCrtsDate = DateTime.MinValue;
                DateTime tempEstAppDate = DateTime.MinValue;
                DateTime tempChangeEstDate = DateTime.MinValue;
                object[] dtRow = new object[14];
                //dtRow = dt.Rows[0];
                dtRow[0] = ReportsList[i].CountryCode;
                dtRow[1] = ReportsList[i].ShopCode;
                dtRow[2] = ReportsList[i].Mode;
                dtRow[3] = ReportsList[i].Status;
                dtRow[4] = ReportsList[i].EquipmentNo;
                dtRow[5] = ReportsList[i].EstimateCreationDate;
                if (!string.IsNullOrEmpty(ReportsList[i].EstimateCreationDate))
                {
                    tempCrtsDate = Convert.ToDateTime(ReportsList[i].EstimateCreationDate);
                    dtRow[6] = (date).Subtract(tempCrtsDate).Days;
                }
                else
                {
                    dtRow[6] = ReportsList[i].EstimateCreationDate;
                }
                dtRow[7] = ReportsList[i].EstimateApprovalDate;
                if (!string.IsNullOrEmpty(ReportsList[i].EstimateApprovalDate))
                {
                    tempEstAppDate = Convert.ToDateTime(ReportsList[i].EstimateApprovalDate);
                    dtRow[8] = (date).Subtract(tempEstAppDate).Days;
                }
                else
                {
                    dtRow[8] = ReportsList[i].EstimateApprovalDate;
                }
                dtRow[9] = ReportsList[i].LastChangeToEstimate;
                if (!string.IsNullOrEmpty(ReportsList[i].LastChangeToEstimate))
                {
                    tempChangeEstDate = Convert.ToDateTime(ReportsList[i].LastChangeToEstimate);
                    dtRow[10] = (date).Subtract(tempEstAppDate).Days;
                }
                else
                {
                    dtRow[10] = ReportsList[i].LastChangeToEstimate;
                }
                dtRow[11] = ReportsList[i].EstimatedTotalHours;
                dtRow[12] = ReportsList[i].EstimatedTotalCostOfRepairInUSD;
                dtRow[13] = ReportsList[i].VendorRefNo;
                dt.Rows.Add(dtRow);
            }

            WriteExcelWithNPOI(dt, "xlsx", ManageReportsModel, ReportsList);
        }

        public void ExportClientsListToExcelMERCE03(List<Reports> ReportsList, ManageReportsModel ManageReportsModel)
        {
            System.Web.UI.WebControls.GridView grid = new System.Web.UI.WebControls.GridView();
            DateTime date = DateTime.Now;
            DataTable dt = new DataTable();
            days = ManageReportsModel.Days;

            dt.Columns.Add("Mode");
            dt.Columns.Add("Status");
            dt.Columns.Add("Equipment No");
            dt.Columns.Add("Estimate Creation Date");
            dt.Columns.Add("Days Since Creation");
            dt.Columns.Add("Estimate Approval Date");
            dt.Columns.Add("Days Since Approval");
            dt.Columns.Add("Last Change To Estimate");
            dt.Columns.Add("Days Since Last Change");
            dt.Columns.Add("EstimatedTotalHours");
            dt.Columns.Add("Estimated Total Cost Of Repair In USD");
            dt.Columns.Add("Vendor Reference No");

            for (int i = 0; i < ReportsList.Count; i++)
            {
                DateTime tempCrtsDate = DateTime.MinValue;
                DateTime tempEstAppDate = DateTime.MinValue;
                DateTime tempChangeEstDate = DateTime.MinValue;
                object[] dtRow = new object[12];
                //dtRow = dt.Rows[0];
                dtRow[0] = ReportsList[i].Mode;
                dtRow[1] = ReportsList[i].Status;
                dtRow[2] = ReportsList[i].EquipmentNo;
                dtRow[3] = ReportsList[i].EstimateCreationDate;
                if (!string.IsNullOrEmpty(ReportsList[i].EstimateCreationDate))
                {
                    tempCrtsDate = Convert.ToDateTime(ReportsList[i].EstimateCreationDate);
                    dtRow[4] = (date).Subtract(tempCrtsDate).Days;
                }
                else
                {
                    dtRow[4] = ReportsList[i].EstimateCreationDate;
                }
                dtRow[5] = ReportsList[i].EstimateApprovalDate;
                if (!string.IsNullOrEmpty(ReportsList[i].EstimateApprovalDate))
                {
                    tempEstAppDate = Convert.ToDateTime(ReportsList[i].EstimateApprovalDate);
                    dtRow[6] = (date).Subtract(tempEstAppDate).Days;
                }
                else
                {
                    dtRow[6] = ReportsList[i].EstimateApprovalDate;
                }
                dtRow[7] = ReportsList[i].LastChangeToEstimate;
                if (!string.IsNullOrEmpty(ReportsList[i].LastChangeToEstimate))
                {
                    tempChangeEstDate = Convert.ToDateTime(ReportsList[i].LastChangeToEstimate);
                    dtRow[8] = (date).Subtract(tempEstAppDate).Days;
                }
                else
                {
                    dtRow[8] = ReportsList[i].LastChangeToEstimate;
                }
                dtRow[9] = ReportsList[i].EstimatedTotalHours;
                dtRow[10] = ReportsList[i].EstimatedTotalCostOfRepairInUSD;
                dtRow[11] = ReportsList[i].VendorRefNo;
                dt.Rows.Add(dtRow);
            }

            WriteExcelWithNPOI(dt, "xlsx", ManageReportsModel, ReportsList);
        }

        public void WriteExcelWithNPOI(DataTable dt, String extension, ManageReportsModel ManageReportsModel, List<Reports> ReportsList)
        {
            IWorkbook workbook;
            string CurrentModeCode = string.Empty;
            string OldModeCode = string.Empty;
            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else
            {
                throw new Exception("This format is not supported");
            }

            ISheet sheet1 = workbook.CreateSheet("Sheet 1");

            //make a header row
            if (ManageReportsModel.ReportsID == 1)
                MercA01(dt, extension, ManageReportsModel, ReportsList, workbook, ref CurrentModeCode, ref OldModeCode, sheet1);
            else if (ManageReportsModel.ReportsID == 2)
                MercA02(dt, extension, ManageReportsModel, ReportsList, workbook, ref CurrentModeCode, ref OldModeCode, sheet1);
            else if (ManageReportsModel.ReportsID == 3)
                MercA03(dt, extension, ManageReportsModel, ReportsList, workbook, ref CurrentModeCode, ref OldModeCode, sheet1);
            else if (ManageReportsModel.ReportsID == 5) //PDF to Excel

            //  MercA05(dt, extension, ManageReportsModel, ReportsList, workbook, ref CurrentModeCode, ref OldModeCode, sheet1);
            {
                // if (ReportsList.Count > 0 && ReportsList[0].HeaderCurrencyCode != null)
                // {
                // currency = ReportsList[0].HeaderCurrencyCode + " " + ReportsList[0].HeaderCurrencyName;
                // }
                // else
                // {
                // currency = "-";
                // }
                // if (ReportsList.Count > 0 && ReportsList[0].HeaderVendorDesc != null)
                // {
                //vendor = ReportsList[0].HeaderVendorDesc;
                //}
                // else
                //  {
                // vendor = "-";
                // }
                // if (ReportsList.Count > 0 && ReportsList[0].HeaderShopDesc != null)
                //  {
                //shop = ManageReportsModel.Shop + ReportsList[0].HeaderShopDesc;
                // }
                //  else
                // {
                // shop = "-";
                //  }
                //shop = ManageReportsModel.Shop;
                sheet1.CreateRow(0).CreateCell(0).SetCellValue("MERCC01 – Exclusionary Code");
                sheet1.CreateRow(1).CreateCell(0).SetCellValue("Report Date " + " " + UTCDate + " UTC");
                sheet1.CreateRow(2).CreateCell(0).SetCellValue("Exclusionary repair codes (exclusionary repair codes cannot be used on same estimate)");
                // sheet1.CreateRow(3).CreateCell(0).SetCellValue("Currency " + currency);
                IRow row1 = sheet1.CreateRow(4);

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + 5);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {

                        ICell cell = row.CreateCell(j);
                        String columnName = dt.Columns[j].ToString();
                        cell.SetCellValue(dt.Rows[i][columnName].ToString());
                    }
                }
                using (var exportData = new MemoryStream())
                {
                    Response.Clear();
                    workbook.Write(exportData);
                    if (extension == "xlsx") //xlsx file format
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "MERCC01.xlsx"));
                        Response.BinaryWrite(exportData.ToArray());
                        Response.Flush();
                        Response.End();
                    }
                }

            }

            else if (ManageReportsModel.ReportsID == 6) //PDF to Excel

            //  MercA05(dt, extension, ManageReportsModel, ReportsList, workbook, ref CurrentModeCode, ref OldModeCode, sheet1);
            {
                // if (ReportsList.Count > 0 && ReportsList[0].HeaderCurrencyCode != null)
                // {
                // currency = ReportsList[0].HeaderCurrencyCode + " " + ReportsList[0].HeaderCurrencyName;
                // }
                // else
                // {
                // currency = "-";
                // }
                // if (ReportsList.Count > 0 && ReportsList[0].HeaderVendorDesc != null)
                // {
                //vendor = ReportsList[0].HeaderVendorDesc;
                //}
                // else
                //  {
                // vendor = "-";
                // }
                // if (ReportsList.Count > 0 && ReportsList[0].HeaderShopDesc != null)
                //  {
                //shop = ManageReportsModel.Shop + ReportsList[0].HeaderShopDesc;
                // }
                //  else
                // {
                // shop = "-";
                //  }
                //shop = ManageReportsModel.Shop;
                sheet1.CreateRow(0).CreateCell(0).SetCellValue("MERCC02- Mode/Code/Part number association");
                sheet1.CreateRow(1).CreateCell(0).SetCellValue("Report Date " + " " + UTCDate + " UTC");
                sheet1.CreateRow(2).CreateCell(0).SetCellValue("Manual/Mode/Repair code/ Part number association");
                // sheet1.CreateRow(3).CreateCell(0).SetCellValue("Currency " + currency);
                IRow row1 = sheet1.CreateRow(4);

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + 5);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {

                        ICell cell = row.CreateCell(j);
                        String columnName = dt.Columns[j].ToString();
                        cell.SetCellValue(dt.Rows[i][columnName].ToString());
                    }
                }
                using (var exportData = new MemoryStream())
                {
                    Response.Clear();
                    workbook.Write(exportData);
                    if (extension == "xlsx") //xlsx file format
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "MERCC02.xlsx"));
                        Response.BinaryWrite(exportData.ToArray());
                        Response.Flush();
                        Response.End();
                    }
                }

            }

            else if (ManageReportsModel.ReportsID == 4)
            {

                //shop = ManageReportsModel.Shop;
                dateFrom = ManageReportsModel.DateFrom;
                dateTo = ManageReportsModel.DateTo;
                mode = ManageReportsModel.Mode;
                stsCode = ManageReportsModel.STSCode;
                double CostOfParts = 0.0;
                double Tot_CostOfParts = 0.0;

                if (ReportsList.Count > 0 && ReportsList[0].HeaderCurrencyCode != null)
                {
                    currency = ReportsList[0].HeaderCurrencyCode; // +" " + ReportsList[0].HeaderCurrencyName;
                }
                else
                {
                    currency = "-";
                }
                if (ReportsList.Count > 0 && ReportsList[0].HeaderVendorDesc != null)
                {
                    vendor = ReportsList[0].HeaderVendorDesc;
                }
                else
                {
                    vendor = "-";
                }
                if (ReportsList.Count > 0 && ReportsList[0].HeaderShopDesc != null)
                {
                    shop = ManageReportsModel.Shop + ReportsList[0].HeaderShopDesc;
                }
                else
                {
                    shop = "-";
                }

                string footerTable = @"<Table><tr><b>Total number of Spare Parts " + ReportsList.Count.ToString() + "</b></tr></Table>";
                sheet1.CreateRow(0).CreateCell(0).SetCellValue("MERCB01; Spare parts usage  pr. container");
                sheet1.CreateRow(1).CreateCell(0).SetCellValue("Report Date " + " " + UTCDate + " UTC");
                sheet1.CreateRow(2).CreateCell(0).SetCellValue("Vendor " + vendor);
                sheet1.CreateRow(3).CreateCell(0).SetCellValue("Shop " + shop);
                sheet1.CreateRow(4).CreateCell(0).SetCellValue("Mode " + mode);
                sheet1.CreateRow(5).CreateCell(0).SetCellValue("STS Codes " + stsCode);
                sheet1.CreateRow(6).CreateCell(0).SetCellValue("Period from" + " " + dateFrom + " " + "to" + " " + dateTo);
                IRow row1 = sheet1.CreateRow(7);

                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row1.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                }
                int i = 0;
                int RowNum = 0;
                int ModeSpecCountB01 = 0;
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    CurrentModeCode = dt.Rows[i]["Mode"].ToString();
                    if (i == 0)
                    {
                        OldModeCode = CurrentModeCode;
                        ModeSpecCountB01++;
                        SetCellValuesMercB01(dt, ref CurrentModeCode, ref sheet1, ref CostOfParts, i);
                        if (i == dt.Rows.Count - 1)
                        {
                            RowNum = (sheet1.LastRowNum);
                            IRow I = PrintModeSummaryB01(CurrentModeCode, ref sheet1, CostOfParts, ModeSpecCountB01, ref RowNum);
                            Tot_CostOfParts = CostOfParts;
                            CostOfParts = 0.0;
                            RowNum = FinalSummaryB01(ReportsList, ref sheet1, Tot_CostOfParts, RowNum++);
                        }
                    }
                    else
                    {
                        if (string.Equals(CurrentModeCode, OldModeCode, StringComparison.OrdinalIgnoreCase))
                        {
                            SetCellValuesMercB01(dt, ref CurrentModeCode, ref sheet1, ref CostOfParts, i);
                            ModeSpecCountB01++;
                            if (i == dt.Rows.Count - 1)
                            {

                                RowNum = (sheet1.LastRowNum);
                                RowNum++;
                                IRow I = PrintModeSummaryB01(CurrentModeCode, ref sheet1, CostOfParts, ModeSpecCountB01, ref RowNum);
                                Tot_CostOfParts = CostOfParts;
                                CostOfParts = 0.0;
                                RowNum = FinalSummaryB01(ReportsList, ref sheet1, Tot_CostOfParts, RowNum++);
                            }
                            //SetCellValuesMercB01(dt, ref CurrentModeCode, ref sheet1, ref CostOfParts, i);

                        }
                        else
                        {
                            //write the comment code after that
                            RowNum = sheet1.LastRowNum;
                            IRow Row5 = PrintModeSummaryB01(OldModeCode, ref sheet1, CostOfParts, ModeSpecCountB01, ref RowNum);
                            Tot_CostOfParts = CostOfParts;
                            CostOfParts = 0.0;
                            OldModeCode = CurrentModeCode;
                            ModeSpecCountB01 = 0;

                            if (i == dt.Rows.Count - 1)
                            {
                                ModeSpecCountB01++;
                                SetCellValuesMercB01(dt, ref CurrentModeCode, ref sheet1, ref CostOfParts, i);
                                RowNum = sheet1.LastRowNum;
                                IRow Row4 = PrintModeSummaryB01(CurrentModeCode, ref sheet1, CostOfParts, ModeSpecCountB01, ref RowNum);
                                Tot_CostOfParts = CostOfParts;
                                CostOfParts = 0.0;
                                RowNum = FinalSummaryB01(ReportsList, ref sheet1, Tot_CostOfParts, RowNum++);
                            }
                            else
                            {
                                ModeSpecCountB01++;
                                SetCellValuesMercB01(dt, ref CurrentModeCode, ref sheet1, ref CostOfParts, i);
                            }
                        }
                    }

                }
                using (var exportData = new MemoryStream())
                {
                    Response.Clear();
                    workbook.Write(exportData);
                    if (extension == "xlsx") //xlsx file format
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "MERCB01.xlsx"));
                        Response.BinaryWrite(exportData.ToArray());
                        Response.Flush();
                        Response.End();
                    }
                }

            }
            else if (ManageReportsModel.ReportsID == 7)
            {
                if (ReportsList.Count > 0 && ReportsList[0].HeaderCurrencyCode != null)
                {
                    currency = ReportsList[0].HeaderCurrencyCode + " " + ReportsList[0].HeaderCurrencyName;
                }
                else
                {
                    currency = "-";
                }
                if (ReportsList.Count > 0 && ReportsList[0].HeaderVendorDesc != null)
                {
                    vendor = ReportsList[0].HeaderVendorDesc;
                }
                else
                {
                    vendor = "-";
                }
                if (ReportsList.Count > 0 && ReportsList[0].HeaderShopDesc != null)
                {
                    shop = ManageReportsModel.Shop + ReportsList[0].HeaderShopDesc;
                }
                else
                {
                    shop = "-";
                }
                //shop = ManageReportsModel.Shop;
                sheet1.CreateRow(0).CreateCell(0).SetCellValue("MERCD03 Shop Material Contract for shop " + shop);
                sheet1.CreateRow(1).CreateCell(0).SetCellValue("Report Date " + " " + UTCDate + " UTC");
                sheet1.CreateRow(2).CreateCell(0).SetCellValue("Vendor " + vendor);
                sheet1.CreateRow(3).CreateCell(0).SetCellValue("Currency " + currency);
                IRow row1 = sheet1.CreateRow(4);

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + 5);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {

                        ICell cell = row.CreateCell(j);
                        String columnName = dt.Columns[j].ToString();
                        cell.SetCellValue(dt.Rows[i][columnName].ToString());
                    }
                }
                using (var exportData = new MemoryStream())
                {
                    Response.Clear();
                    workbook.Write(exportData);
                    if (extension == "xlsx") //xlsx file format
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "MERCD03.xlsx"));
                        Response.BinaryWrite(exportData.ToArray());
                        Response.Flush();
                        Response.End();
                    }
                }

            }
            else if (ManageReportsModel.ReportsID == 8)
            {
                if (ReportsList.Count > 0 && ReportsList[0].HeaderCurrencyCode != null)
                {
                    currency = ReportsList[0].HeaderCurrencyCode + " " + ReportsList[0].HeaderCurrencyName;
                    exchangeRate = ReportsList[0].ExchangeRate;
                }
                else
                {
                    currency = "-";
                }
                if (ReportsList.Count > 0 && ReportsList[0].HeaderShopDesc != null)
                {
                    shop = ManageReportsModel.Shop + ReportsList[0].HeaderShopDesc;
                }
                else
                {
                    shop = "-";
                }
                //shop = ManageReportsModel.Shop;
                sheet1.CreateRow(0).CreateCell(0).SetCellValue("MERCD05 Country Contract between Maersk Sealand Copenhagen and " + shop);
                sheet1.CreateRow(1).CreateCell(0).SetCellValue("Report Date " + " " + UTCDate + " UTC");
                sheet1.CreateRow(2).CreateCell(0).SetCellValue("All Values in currency " + currency + " unless stated otherwise");
                sheet1.CreateRow(3).CreateCell(0).SetCellValue("ExchangeRate " + exchangeRate);
                IRow row1 = sheet1.CreateRow(4);

                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row1.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + 5);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {

                        ICell cell = row.CreateCell(j);
                        String columnName = dt.Columns[j].ToString();
                        cell.SetCellValue(dt.Rows[i][columnName].ToString());
                    }
                }
                using (var exportData = new MemoryStream())
                {
                    Response.Clear();
                    workbook.Write(exportData);
                    if (extension == "xlsx") //xlsx file format
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "MERCD05.xlsx"));
                        Response.BinaryWrite(exportData.ToArray());
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            else if (ManageReportsModel.ReportsID == 9)
            {
                days = ManageReportsModel.Days;
                //string headerTable = @"<Table><tr><td><b>MERCE01 Equipment repair status/longstanding report. More than " + days + " days since estimate creation</b></td></tr>
                //    <tr><td><b>Report Date " + UTCDate + "</b></td></tr><tr><td><b>Repair Status: Not Completed</b></td></tr><tr><td><b>All values in USD</b></td></tr></Table>";
                sheet1.CreateRow(0).CreateCell(0).SetCellValue("MERCE01 Equipment repair status/longstanding report. More than " + days + " days since estimate creation");
                sheet1.CreateRow(1).CreateCell(0).SetCellValue("Report Date " + UTCDate + " UTC");
                sheet1.CreateRow(2).CreateCell(0).SetCellValue("Repair Status: Not Completed");
                sheet1.CreateRow(3).CreateCell(0).SetCellValue("All Values in USD");
                IRow row1 = sheet1.CreateRow(4);

                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row1.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                }
                int i = 0;
                int ModeSpecCountE01 = 0;
                int RowNum = 0;

                for (i = 0; i < dt.Rows.Count; i++)
                {
                    CurrentModeCode = dt.Rows[i]["Mode"].ToString();
                    if (i == 0)
                    {
                        OldModeCode = CurrentModeCode;
                        ModeSpecCountE01++;
                        SetCellValuesMercE01(dt, ref CurrentModeCode, ref sheet1, i);
                        if (i == dt.Rows.Count - 1)
                        {
                            RowNum = (sheet1.LastRowNum);
                            IRow I = PrintModeSummaryE(CurrentModeCode, ref sheet1, ModeSpecCountE01, ref RowNum);

                            RowNum = FinalSummaryE(ReportsList, ref sheet1, RowNum++);
                        }
                    }
                    else
                    {
                        if (string.Equals(CurrentModeCode, OldModeCode, StringComparison.OrdinalIgnoreCase))
                        {
                            SetCellValuesMercE01(dt, ref CurrentModeCode, ref sheet1, i);
                            ModeSpecCountE01++;
                            if (i == dt.Rows.Count - 1)
                            {

                                RowNum = (sheet1.LastRowNum);
                                RowNum++;
                                IRow I = PrintModeSummaryE(CurrentModeCode, ref sheet1, ModeSpecCountE01, ref RowNum);

                                RowNum = FinalSummaryE(ReportsList, ref sheet1, RowNum++);
                            }
                            //SetCellValuesMercE01(dt, ref CurrentModeCode, ref sheet1, i);

                        }
                        else
                        {
                            //write the comment code after that
                            RowNum = sheet1.LastRowNum;
                            IRow Row5 = PrintModeSummaryE(OldModeCode, ref sheet1, ModeSpecCountE01, ref RowNum);

                            OldModeCode = CurrentModeCode;
                            ModeSpecCountE01 = 0;

                            if (i == dt.Rows.Count - 1)
                            {
                                ModeSpecCountE01++;
                                SetCellValuesMercE01(dt, ref CurrentModeCode, ref sheet1, i);
                                RowNum = sheet1.LastRowNum;
                                IRow Row4 = PrintModeSummaryE(CurrentModeCode, ref sheet1, ModeSpecCountE01, ref RowNum);

                                RowNum = FinalSummaryE(ReportsList, ref sheet1, RowNum++);
                            }
                            else
                            {
                                ModeSpecCountE01++;
                                SetCellValuesMercE01(dt, ref CurrentModeCode, ref sheet1, i);
                            }
                        }
                    }

                }
                using (var exportData = new MemoryStream())
                {
                    Response.Clear();
                    workbook.Write(exportData);
                    if (extension == "xlsx") //xlsx file format
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "MERCE01.xlsx"));
                        Response.BinaryWrite(exportData.ToArray());
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            else if (ManageReportsModel.ReportsID == 10)
            {
                days = ManageReportsModel.Days;
                sheet1.CreateRow(0).CreateCell(0).SetCellValue("MERCE02 Equipment repair status/longstanding report. More than " + days + " days since estimate creation");
                sheet1.CreateRow(1).CreateCell(0).SetCellValue("Report Date " + " " + UTCDate + " UTC");
                sheet1.CreateRow(2).CreateCell(0).SetCellValue("Repair Status: Not Completed");
                sheet1.CreateRow(3).CreateCell(0).SetCellValue("All Values in USD");
                IRow row1 = sheet1.CreateRow(4);

                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row1.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                }
                int i = 0;
                int ModeSpecCountE02 = 0;
                int RowNum = 0;
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    CurrentModeCode = dt.Rows[i]["Mode"].ToString();
                    if (i == 0)
                    {
                        OldModeCode = CurrentModeCode;
                        ModeSpecCountE02++;
                        SetCellValuesMercE02(dt, ref CurrentModeCode, ref sheet1, i);
                        if (i == dt.Rows.Count - 1)
                        {
                            RowNum = (sheet1.LastRowNum);
                            IRow I = PrintModeSummaryE(CurrentModeCode, ref sheet1, ModeSpecCountE02, ref RowNum);

                            RowNum = FinalSummaryE(ReportsList, ref sheet1, RowNum++);
                        }
                    }
                    else
                    {
                        if (string.Equals(CurrentModeCode, OldModeCode, StringComparison.OrdinalIgnoreCase))
                        {
                            SetCellValuesMercE02(dt, ref CurrentModeCode, ref sheet1, i);
                            ModeSpecCountE02++;
                            if (i == dt.Rows.Count - 1)
                            {
                                RowNum = (sheet1.LastRowNum);
                                RowNum++;
                                IRow I = PrintModeSummaryE(CurrentModeCode, ref sheet1, ModeSpecCountE02, ref RowNum);

                                RowNum = FinalSummaryE(ReportsList, ref sheet1, RowNum++);
                            }
                            //SetCellValuesMercE02(dt, ref CurrentModeCode, ref sheet1, i);

                        }
                        else
                        {
                            //write the comment code after that
                            RowNum = sheet1.LastRowNum;
                            IRow Row5 = PrintModeSummaryE(OldModeCode, ref sheet1, ModeSpecCountE02, ref RowNum);

                            OldModeCode = CurrentModeCode;
                            ModeSpecCountE02 = 0;

                            if (i == dt.Rows.Count - 1)
                            {
                                ModeSpecCountE02++;
                                SetCellValuesMercE02(dt, ref CurrentModeCode, ref sheet1, i);
                                RowNum = sheet1.LastRowNum;
                                IRow Row4 = PrintModeSummaryE(CurrentModeCode, ref sheet1, ModeSpecCountE02, ref RowNum);

                                RowNum = FinalSummaryE(ReportsList, ref sheet1, RowNum++);
                            }
                            else
                            {
                                ModeSpecCountE02++;
                                SetCellValuesMercE02(dt, ref CurrentModeCode, ref sheet1, i);
                            }
                        }
                    }

                }
                using (var exportData = new MemoryStream())
                {
                    Response.Clear();
                    workbook.Write(exportData);
                    if (extension == "xlsx") //xlsx file format
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "MERCE02.xlsx"));
                        Response.BinaryWrite(exportData.ToArray());
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            else if (ManageReportsModel.ReportsID == 11)
            {
                days = ManageReportsModel.Days;
                sheet1.CreateRow(0).CreateCell(0).SetCellValue("MERCE03 Equipment repair status/longstanding report. More than " + days + " days since estimate creation");
                sheet1.CreateRow(1).CreateCell(0).SetCellValue("Report Date " + " " + UTCDate + " UTC");
                sheet1.CreateRow(2).CreateCell(0).SetCellValue("Repair Status: Not Completed");
                sheet1.CreateRow(3).CreateCell(0).SetCellValue("All Values in USD");
                IRow row1 = sheet1.CreateRow(4);

                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row1.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                }
                int k = 0;
                int RowNum = 0;
                int ModeSpecCountE03 = 0;
                for (k = 0; k < dt.Rows.Count; k++)
                {
                    CurrentModeCode = dt.Rows[k]["Mode"].ToString();
                    if (k == 0)
                    {
                        ModeSpecCountE03++;
                        SetCellValuesMercE03(dt, ref CurrentModeCode, ref sheet1, k);
                        if (k == dt.Rows.Count - 1)
                        {
                            RowNum = (sheet1.LastRowNum);
                            IRow I = PrintModeSummaryE(CurrentModeCode, ref sheet1, ModeSpecCountE03, ref RowNum);

                            RowNum = FinalSummaryE(ReportsList, ref sheet1, RowNum++);
                        }
                    }
                    else
                    {
                        if (string.Equals(CurrentModeCode, OldModeCode, StringComparison.OrdinalIgnoreCase))
                        {
                            SetCellValuesMercE03(dt, ref CurrentModeCode, ref sheet1, k);
                            ModeSpecCountE03++;
                            if (k == dt.Rows.Count - 1)
                            {

                                RowNum = (sheet1.LastRowNum);
                                RowNum++;
                                IRow I = PrintModeSummaryE(CurrentModeCode, ref sheet1, ModeSpecCountE03, ref RowNum);

                                RowNum = FinalSummaryE(ReportsList, ref sheet1, RowNum++);
                            }
                            //SetCellValuesMercE03(dt, ref CurrentModeCode, ref sheet1, k);     //Kasturee_Duplicate_Repair_09.05.2018

                        }
                        else
                        {
                            //write the comment code after that
                            RowNum = sheet1.LastRowNum;
                            IRow Row5 = PrintModeSummaryE(OldModeCode, ref sheet1, ModeSpecCountE03, ref RowNum);

                            OldModeCode = CurrentModeCode;
                            ModeSpecCountE03 = 0;

                            if (k == dt.Rows.Count - 1)
                            {
                                ModeSpecCountE03++;
                                SetCellValuesMercE03(dt, ref CurrentModeCode, ref sheet1, k);  //Kasturee_Duplicate_Repair_09.05.2018
                                RowNum = sheet1.LastRowNum;
                                IRow Row4 = PrintModeSummaryE(CurrentModeCode, ref sheet1, ModeSpecCountE03, ref RowNum);

                                RowNum = FinalSummaryE(ReportsList, ref sheet1, RowNum++);
                            }
                            else
                            {
                                ModeSpecCountE03++;
                                SetCellValuesMercE03(dt, ref CurrentModeCode, ref sheet1, k);
                            }
                        }
                    }

                }
                using (var exportData = new MemoryStream())
                {
                    Response.Clear();
                    workbook.Write(exportData);
                    if (extension == "xlsx") //xlsx file format
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "MERCE03.xlsx"));
                        Response.BinaryWrite(exportData.ToArray());
                        Response.Flush();
                        Response.End();
                    }
                }
            }

        }


        //REPORT BUG FIX -Debadrita..//


        public void ExportClientsListToExcelMERCDO6_1(List<Reports> ReportsList, ManageReportsModel ManageReportsModel) //PDF to Excel
        {
            System.Web.UI.WebControls.GridView grid = new System.Web.UI.WebControls.GridView();
            DataTable dt = new DataTable();
            dt.Columns.Add("Manual");
            dt.Columns.Add("Mode");
            dt.Columns.Add("Repair Code");
            dt.Columns.Add("Repair Code Description");
            dt.Columns.Add("Associated PartNumber");
            dt.Columns.Add("Part Description");
            // dt.Columns.Add("Rate Exp Date");

            for (int i = 0; i < ReportsList.Count; i++)
            {
                object[] dtRow = new object[6];
                //dtRow = dt.Rows[0];
                dtRow[0] = ReportsList[i].Manual;
                dtRow[1] = ReportsList[i].Mode;
                dtRow[2] = ReportsList[i].RepairCod;
                dtRow[3] = ReportsList[i].RepairCodeDesc;
                dtRow[4] = ReportsList[i].PART_CD;
                dtRow[5] = ReportsList[i].PartDesc;
                dt.Rows.Add(dtRow);
            }

            WriteExcelWithNPOI(dt, "xlsx", ManageReportsModel, ReportsList);
        }



        public void ExportClientsListToExcelMERCDO5_1(List<Reports> ReportsList, ManageReportsModel ManageReportsModel) //PDF to Excel
        {
            System.Web.UI.WebControls.GridView grid = new System.Web.UI.WebControls.GridView();
            DataTable dt = new DataTable();
            dt.Columns.Add("Manual");
            dt.Columns.Add("Mode");
            dt.Columns.Add("Repair Code");
            dt.Columns.Add("Repair Code Description");
            dt.Columns.Add("Exclusionary Repair Code");
            dt.Columns.Add("Exclusionary Repair Code Description");
            // dt.Columns.Add("Rate Exp Date");

            for (int i = 0; i < ReportsList.Count; i++)
            {
                object[] dtRow = new object[6];
                //dtRow = dt.Rows[0];
                dtRow[0] = ReportsList[i].Manual;
                dtRow[1] = ReportsList[i].Mode;
                dtRow[2] = ReportsList[i].RepairCod;
                dtRow[3] = ReportsList[i].RepairCodeDesc;
                dtRow[4] = ReportsList[i].ExclusionaryRepairCode;
                dtRow[5] = ReportsList[i].ExclusionaryRepairCodeDescription;
                dt.Rows.Add(dtRow);
            }

            WriteExcelWithNPOI(dt, "xlsx", ManageReportsModel, ReportsList);
        }


        //REPORT BUG FIX -DEBADRITA- END//

        private void SetCellValuesMercE03(DataTable dt, ref string CurrentModeCode, ref ISheet sheet1, int i)
        {
            int RowN = sheet1.LastRowNum;
            //RowN++;
            IRow row = sheet1.CreateRow(RowN + 1);
            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(dt.Rows[i][columnName].ToString());
                if (columnName == "Mode")
                {
                    CurrentModeCode = dt.Rows[i][columnName].ToString();
                    //OldModeCode = CurrentModeCode;
                }
            }
        }

        private void SetCellValuesMercE02(DataTable dt, ref string CurrentModeCode, ref ISheet sheet1, int i)
        {
            int RowN = sheet1.LastRowNum;
            //RowN++;
            IRow row = sheet1.CreateRow(RowN + 1);
            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(dt.Rows[i][columnName].ToString());
                if (columnName == "Mode")
                {
                    CurrentModeCode = dt.Rows[i][columnName].ToString();
                    //OldModeCode = CurrentModeCode;
                }
            }
        }

        private void SetCellValuesMercE01(DataTable dt, ref string CurrentModeCode, ref ISheet sheet1, int i)
        {
            int RowN = sheet1.LastRowNum;
            //RowN++;
            IRow row = sheet1.CreateRow(RowN + 1);
            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(dt.Rows[i][columnName].ToString());
                if (columnName == "Mode")
                {
                    CurrentModeCode = dt.Rows[i][columnName].ToString();
                    //OldModeCode = CurrentModeCode;
                }
            }
        }

        private IRow PrintModeSummaryE(string ModeCode, ref ISheet sheet1, int ModeSpecCount, ref int RowNum)
        {
            int RowCt = RowNum;
            RowCt = RowCt + 1;
            IRow Row2 = sheet1.CreateRow(RowCt++);
            Row2.CreateCell(0).SetCellValue(ModeSpecCount + " Mode " + ModeCode + " Work order(s)");
            RowNum = RowCt;
            return Row2;

        }

        private void SetCellValuesMercB01(DataTable dt, ref string CurrentModeCode, ref ISheet sheet1, ref double CostOfParts, int i)
        {
            int RowN = sheet1.LastRowNum;
            //RowN++;
            IRow row = sheet1.CreateRow(RowN + 1);
            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(dt.Rows[i][columnName].ToString());
                if (columnName == "Mode")
                {
                    CurrentModeCode = dt.Rows[i][columnName].ToString();
                    //OldModeCode = CurrentModeCode;
                }
                if (columnName == "Total Cost Of Parts")
                {
                    CostOfParts += Convert.ToDouble(dt.Rows[i][columnName]);
                }
            }


        }

        #region MercA03

        private void MercA03(DataTable dt, String extension, ManageReportsModel ManageReportsModel, List<Reports> ReportsList, IWorkbook workbook, ref string CurrentModeCode, ref string OldModeCode, ISheet sheet1)
        {
            shop = ManageReportsModel.Shop;
            dateFrom = ManageReportsModel.DateFrom;
            dateTo = ManageReportsModel.DateTo;
            //rohit
            Report03 repV = new Report03();
            if (ReportsList.Count > 0 && ReportsList.Count > 0 && ReportsList[0].HeaderCurrencyCode != null)
            {
                currency = ReportsList[0].HeaderCurrencyName; // +" " + ReportsList[0].HeaderCurrencyName;
            }
            else
            {
                currency = "-";
            }
            if (ReportsList.Count > 0 && ReportsList.Count > 0 && ReportsList[0].HeaderVendorDesc != null)
            {
                vendor = ReportsList[0].HeaderVendorDesc;
            }
            else
            {
                vendor = "-";
            }
            if (ReportsList.Count > 0 && ReportsList[0].HeaderShopDesc != null)
            {
                shop = ManageReportsModel.Shop + ReportsList[0].HeaderShopDesc;
            }
            else
            {
                shop = "-";
            }
            sheet1.CreateRow(0).CreateCell(0).SetCellValue("MERCA03; Summary costs pr shop pr container (Shop)");
            sheet1.CreateRow(1).CreateCell(0).SetCellValue("Report Date " + " " + UTCDate + " UTC");
            sheet1.CreateRow(2).CreateCell(0).SetCellValue("Vendor " + vendor);
            sheet1.CreateRow(3).CreateCell(0).SetCellValue("Maersk Sealand Shop " + shop);
            sheet1.CreateRow(4).CreateCell(0).SetCellValue("Period from" + " " + dateFrom + " " + "to" + " " + dateTo);
            sheet1.CreateRow(5).CreateCell(0).SetCellValue("Currency" + " " + currency);
            IRow row1 = sheet1.CreateRow(6);

            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row1.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
            }
            int i = 0;
            int ModeSpecCount = 0;
            int RowNum = 0;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                CurrentModeCode = dt.Rows[i]["Mode"].ToString();
                if (i == 0)
                {
                    OldModeCode = CurrentModeCode;
                    ModeSpecCount++;
                    SetCellValuesMercA03(dt, ref CurrentModeCode, ref sheet1, ref repV, i);
                    if (i == dt.Rows.Count - 1)
                    {
                        RowNum = (sheet1.LastRowNum);
                        IRow I = PrintModeSummaryA03(CurrentModeCode, ref sheet1, repV, ModeSpecCount, ref RowNum);
                        UpdateTotalA03(repV);
                        RowNum = FinalSummaryA03(ReportsList, ref sheet1, repV, RowNum++);
                    }
                }
                else
                {
                    if (string.Equals(CurrentModeCode, OldModeCode, StringComparison.OrdinalIgnoreCase))
                    {
                        ModeSpecCount++;
                        SetCellValuesMercA03(dt, ref CurrentModeCode, ref sheet1, ref repV, i);
                        if (i == dt.Rows.Count - 1)
                        {

                            RowNum = (sheet1.LastRowNum);
                            IRow I = PrintModeSummaryA03(CurrentModeCode, ref sheet1, repV, ModeSpecCount, ref RowNum);
                            UpdateTotalA03(repV);
                            RowNum = FinalSummaryA03(ReportsList, ref sheet1, repV, RowNum++);
                        }
                    }
                    else
                    {
                        RowNum = sheet1.LastRowNum;
                        IRow Row5 = PrintModeSummaryA03(OldModeCode, ref sheet1, repV, ModeSpecCount, ref RowNum);
                        UpdateTotalA03(repV);
                        OldModeCode = CurrentModeCode;
                        ModeSpecCount = 0;

                        if (i == dt.Rows.Count - 1)
                        {
                            ModeSpecCount++;
                            SetCellValuesMercA03(dt, ref CurrentModeCode, ref sheet1, ref repV, i);
                            RowNum = sheet1.LastRowNum;
                            IRow Row4 = PrintModeSummaryA03(CurrentModeCode, ref sheet1, repV, ModeSpecCount, ref RowNum);
                            UpdateTotalA03(repV);
                            RowNum = FinalSummaryA03(ReportsList, ref sheet1, repV, RowNum++);
                        }
                        else
                        {
                            ModeSpecCount++;
                            SetCellValuesMercA03(dt, ref CurrentModeCode, ref sheet1, ref repV, i);
                        }
                    }
                }
            }
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                workbook.Write(exportData);
                if (extension == "xlsx") //xlsx file format
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "MERCA03.xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.Flush();
                    Response.End();
                }
            }
        }

        private void SetFooterTotalA03(string OldModeCode, ISheet sheet1, Report03 repV, int i, int ModeSpecCount)
        {
            int RowNum = sheet1.LastRowNum;
            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue("Total for Mode " + OldModeCode);
            IRow Row2 = sheet1.CreateRow(RowNum++);
            Row2.CreateCell(5).SetCellValue(repV.OrdinaryManHours);
            Row2.CreateCell(6).SetCellValue(repV.Overtime1ManHours);
            Row2.CreateCell(7).SetCellValue(repV.Overtime2ManHours);
            Row2.CreateCell(8).SetCellValue(repV.Overtime3ManHours);
            Row2.CreateCell(9).SetCellValue(repV.TotalHours);
            Row2.CreateCell(10).SetCellValue(repV.TotalLabourCost);
            Row2.CreateCell(12).SetCellValue(repV.TotalCostOfNumberedParts);
            Row2.CreateCell(13).SetCellValue(repV.TotalCostOfShopSuppliedMaterials);
            Row2.CreateCell(14).SetCellValue(repV.ImportTax);
            Row2.CreateCell(15).SetCellValue(repV.SalesTaxParts);
            Row2.CreateCell(16).SetCellValue(repV.SalesTaxLabour);
            Row2.CreateCell(17).SetCellValue(repV.TotalToBePaidToShop);
            //RowNum = sheet1.LastRowNum;
            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(ModeSpecCount + " Mode " + OldModeCode + " Work order(s)");

            UpdateTotalA03(repV);
        }

        private int FinalSummaryA03(List<Reports> ReportsList, ref ISheet sheet1, Report03 repV, int RowNum)
        {
            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(" Total for Shop ");
            IRow Row6 = sheet1.CreateRow(RowNum++);
            Row6.CreateCell(5).SetCellValue(repV.Tot_OrdinaryManHours);
            Row6.CreateCell(6).SetCellValue(repV.Tot_Overtime1ManHours);
            Row6.CreateCell(7).SetCellValue(repV.Tot_Overtime2ManHours);
            Row6.CreateCell(8).SetCellValue(repV.Tot_Overtime3ManHours);
            Row6.CreateCell(9).SetCellValue(repV.Tot_TotalHours);
            Row6.CreateCell(10).SetCellValue(repV.Tot_TotalLabourCost);
            Row6.CreateCell(12).SetCellValue(repV.Tot_TotalCostOfNumberedParts);
            Row6.CreateCell(13).SetCellValue(repV.Tot_TotalCostOfShopSuppliedMaterials);
            Row6.CreateCell(14).SetCellValue(repV.Tot_ImportTax);
            Row6.CreateCell(15).SetCellValue(repV.Tot_SalesTaxParts);
            Row6.CreateCell(16).SetCellValue(repV.Tot_SalesTaxLabour);
            Row6.CreateCell(17).SetCellValue(repV.Tot_TotalToBePaidToShop);
            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(" Total number of work orders " + ReportsList.Count());
            return RowNum;
        }

        private void UpdateTotalA03(Report03 repV)
        {

            repV.Tot_OrdinaryManHours += repV.OrdinaryManHours;
            repV.Tot_Overtime1ManHours += repV.Overtime1ManHours;
            repV.Tot_Overtime2ManHours += repV.Overtime2ManHours;
            repV.Tot_Overtime3ManHours += repV.Overtime3ManHours;
            repV.Tot_TotalHours += repV.TotalHours;
            repV.Tot_TotalLabourCost += repV.TotalLabourCost;
            repV.Tot_TotalCostOfNumberedParts += repV.TotalCostOfNumberedParts;
            repV.Tot_TotalCostOfShopSuppliedMaterials += repV.TotalCostOfShopSuppliedMaterials;
            repV.Tot_ImportTax += repV.ImportTax;
            repV.Tot_SalesTaxParts += repV.SalesTaxParts;
            repV.Tot_SalesTaxLabour += repV.SalesTaxLabour;
            repV.Tot_TotalToBePaidToShop += repV.TotalToBePaidToShop;

            repV.OrdinaryManHours = 0;
            repV.Overtime1ManHours = 0;
            repV.Overtime2ManHours = 0;
            repV.Overtime3ManHours = 0;
            repV.TotalHours = 0;
            repV.TotalLabourCost = 0;
            repV.TotalCostOfNumberedParts = 0;
            repV.TotalCostOfShopSuppliedMaterials = 0;
            repV.ImportTax = 0;
            repV.SalesTaxParts = 0;
            repV.SalesTaxLabour = 0;
            repV.TotalToBePaidToShop = 0;
        }

        private void SetCellValuesMercA03(DataTable dt, ref string CurrentModeCode, ref ISheet sheet1, ref Report03 repV, int i)
        {
            try
            {
                int RowN = sheet1.LastRowNum;
                //RowN = RowN+1;
                IRow row = sheet1.CreateRow(RowN + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                    if (columnName == "Mode")
                    {
                        CurrentModeCode = dt.Rows[i][columnName].ToString();
                        //OldModeCode = CurrentModeCode;
                    }
                    if (columnName == "Ordinary Man Hours")
                    {
                        repV.OrdinaryManHours += Convert.ToDouble(dt.Rows[i][columnName]);
                    }
                    if (columnName == "Overtime1 Man Hours")
                    {
                        repV.Overtime1ManHours += Convert.ToDouble(dt.Rows[i][columnName]);
                    }
                    if (columnName == "Overtime2 Man Hours")
                    {
                        repV.Overtime2ManHours += Convert.ToDouble(dt.Rows[i][columnName]);
                    }
                    if (columnName == "Overtime3 Man Hours")
                    {
                        repV.Overtime3ManHours += Convert.ToDouble(dt.Rows[i][columnName]);
                    }
                    if (columnName == "Total Hours")
                    {
                        repV.TotalHours += Convert.ToDouble(dt.Rows[i][columnName]);
                    }
                    if (columnName == "Total Labour Cost")
                    {
                        repV.TotalLabourCost += Convert.ToDouble(dt.Rows[i][columnName]);
                    }
                    if (columnName == "TotalCostOfNumberedParts")
                    {
                        repV.TotalCostOfNumberedParts += Convert.ToDouble(dt.Rows[i][columnName]);
                    }
                    if (columnName == "TotalCostOfShopSuppliedMaterials")
                    {
                        if (dt.Rows[i][columnName] != null)
                            repV.TotalCostOfShopSuppliedMaterials += Convert.ToDouble(dt.Rows[i][columnName]);
                    }
                    if (columnName == "Import Tax")
                    {
                        repV.ImportTax += Convert.ToDouble(dt.Rows[i][columnName]);
                    }
                    if (columnName == "Sales Tax Parts")
                    {
                        repV.SalesTaxParts += Convert.ToDouble(dt.Rows[i][columnName]);
                    }
                    if (columnName == "Sales Tax Labour")
                    {
                        repV.SalesTaxLabour += Convert.ToDouble(dt.Rows[i][columnName]);
                    }
                    if (columnName == "Total To Be Paid To Shop")
                    {
                        repV.TotalToBePaidToShop += Convert.ToDouble(dt.Rows[i][columnName]);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private IRow PrintModeSummaryA03(string ModeCode, ref ISheet sheet1, Report03 repV, int ModeSpecCount, ref int RowNum)
        {
            int RowCt = RowNum;
            RowCt = RowCt + 1;
            sheet1.CreateRow(RowCt++).CreateCell(0).SetCellValue("Total for Mode " + ModeCode);
            IRow Row2 = sheet1.CreateRow(RowCt++);
            Row2.CreateCell(5).SetCellValue(repV.OrdinaryManHours);
            Row2.CreateCell(6).SetCellValue(repV.Overtime1ManHours);
            Row2.CreateCell(7).SetCellValue(repV.Overtime2ManHours);
            Row2.CreateCell(8).SetCellValue(repV.Overtime3ManHours);
            Row2.CreateCell(9).SetCellValue(repV.TotalHours);
            Row2.CreateCell(10).SetCellValue(repV.TotalLabourCost);
            Row2.CreateCell(12).SetCellValue(repV.TotalCostOfNumberedParts);
            Row2.CreateCell(13).SetCellValue(repV.TotalCostOfShopSuppliedMaterials);
            Row2.CreateCell(14).SetCellValue(repV.ImportTax);
            Row2.CreateCell(15).SetCellValue(repV.SalesTaxParts);
            Row2.CreateCell(16).SetCellValue(repV.SalesTaxLabour);
            Row2.CreateCell(17).SetCellValue(repV.TotalToBePaidToShop);

            sheet1.CreateRow(RowCt++).CreateCell(0).SetCellValue(ModeSpecCount + " Mode " + ModeCode + " Work order(s)");
            RowNum = RowCt;
            return Row2;
        }
        #endregion MercA03

        private IRow PrintModeSummaryB01(string ModeCode, ref ISheet sheet1, double CostOfPart, int ModeSpecCount, ref int RowNum)
        {

            int RowCt = RowNum;
            RowCt = RowCt + 1;
            sheet1.CreateRow(RowCt++).CreateCell(0).SetCellValue("Total for Mode " + ModeCode);
            IRow Row2 = sheet1.CreateRow(RowCt++);
            Row2.CreateCell(11).SetCellValue(CostOfPart);

            sheet1.CreateRow(RowCt++).CreateCell(0).SetCellValue(ModeSpecCount + " Mode " + ModeCode + " Work order(s)");
            RowNum = RowCt;
            return Row2;
        }

        #region MercA02
        private void MercA02(DataTable dt, String extension, ManageReportsModel ManageReportsModel, List<Reports> ReportsList, IWorkbook workbook, ref string CurrentModeCode, ref string OldModeCode, ISheet sheet1)
        {
            //shop = ManageReportsModel.Shop;
            dateFrom = ManageReportsModel.DateFrom;
            dateTo = ManageReportsModel.DateTo;
            Report02 repV = new Report02();
            if (ReportsList.Count > 0 && ReportsList[0].HeaderCurrencyName != null)
            {
                currency = ReportsList[0].HeaderCurrencyName; // +" " + ReportsList[0].HeaderCurrencyName;
            }
            else
            {
                currency = "-";
            }
            if (ReportsList.Count > 0 && ReportsList[0].HeaderVendorDesc != null)
            {
                vendor = ReportsList[0].HeaderVendorDesc;
            }
            else
            {
                vendor = "-";
            }
            if (ReportsList.Count > 0 && ReportsList[0].HeaderShopDesc != null)
            {
                shop = ManageReportsModel.Shop + ReportsList[0].HeaderShopDesc;
            }
            else
            {
                shop = "-";
            }
            sheet1.CreateRow(0).CreateCell(0).SetCellValue("MERCA02; Summary costs pr shop pr container (MSL and CPH costs)");
            sheet1.CreateRow(1).CreateCell(0).SetCellValue("Report Date " + " " + UTCDate + " UTC");
            sheet1.CreateRow(2).CreateCell(0).SetCellValue("Vendor " + vendor);
            sheet1.CreateRow(3).CreateCell(0).SetCellValue("Shop " + shop);
            sheet1.CreateRow(4).CreateCell(0).SetCellValue("Period from" + " " + dateFrom + " " + "to" + " " + dateTo);
            sheet1.CreateRow(5).CreateCell(0).SetCellValue("All figures in currency" + " " + currency + " " + " . unless otherwise stated");
            IRow row1 = sheet1.CreateRow(6);

            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row1.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
            }
            int i = 0;
            int ModeSpecCount = 0;
            int RowNum = 0;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                CurrentModeCode = dt.Rows[i]["Mode"].ToString();
                if (i == 0)
                {
                    OldModeCode = CurrentModeCode;
                    ModeSpecCount++;
                    SetCellValuesMercA02(dt, ref CurrentModeCode, ref sheet1, ref repV, i);
                    if (i == dt.Rows.Count - 1)
                    {
                        RowNum = (sheet1.LastRowNum);
                        IRow I = PrintModeSummaryA02(CurrentModeCode, ref sheet1, repV, ModeSpecCount, ref RowNum);
                        UpdateTotalA02(repV);
                        RowNum = FinalSummaryA02(ReportsList, ref sheet1, repV, RowNum++);
                    }
                }
                else
                {
                    if (string.Equals(CurrentModeCode, OldModeCode, StringComparison.OrdinalIgnoreCase))
                    {
                        ModeSpecCount++;
                        SetCellValuesMercA02(dt, ref CurrentModeCode, ref sheet1, ref repV, i);
                        if (i == dt.Rows.Count - 1)
                        {

                            RowNum = (sheet1.LastRowNum);
                            IRow I = PrintModeSummaryA02(CurrentModeCode, ref sheet1, repV, ModeSpecCount, ref RowNum);
                            UpdateTotalA02(repV);
                            RowNum = FinalSummaryA02(ReportsList, ref sheet1, repV, RowNum++);
                        }
                    }
                    else
                    {
                        //write the comment code after that
                        //SetFooterTotalA01(OldModeCode, sheet1, repV, i, ModeSpecCount);'

                        // UpdateTotalA01(repV);
                        // IRow Row2 = sheet1.CreateRow(RowNum++);
                        // Row2 = PrintModeSummaryA01(OldModeCode, sheet1, repV, ModeSpecCount, ref RowNum);
                        //sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(ModeSpecCount + " Mode " + OldModeCode + " Work order(s)");


                        RowNum = sheet1.LastRowNum;
                        IRow Row5 = PrintModeSummaryA02(OldModeCode, ref sheet1, repV, ModeSpecCount, ref RowNum);
                        UpdateTotalA02(repV);
                        OldModeCode = CurrentModeCode;
                        ModeSpecCount = 0;

                        if (i == dt.Rows.Count - 1)
                        {
                            ModeSpecCount++;
                            SetCellValuesMercA02(dt, ref CurrentModeCode, ref sheet1, ref repV, i);
                            RowNum = sheet1.LastRowNum;
                            IRow Row4 = PrintModeSummaryA02(CurrentModeCode, ref sheet1, repV, ModeSpecCount, ref RowNum);
                            UpdateTotalA02(repV);
                            RowNum = FinalSummaryA02(ReportsList, ref sheet1, repV, RowNum++);
                        }
                        else
                        {
                            ModeSpecCount++;
                            SetCellValuesMercA02(dt, ref CurrentModeCode, ref sheet1, ref repV, i);
                        }
                    }
                }
            }

            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                workbook.Write(exportData);
                if (extension == "xlsx") //xlsx file format
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "MERCA02.xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.Flush();
                    Response.End();
                }
            }
        }

        private IRow PrintModeSummaryA02(string ModeCode, ref ISheet sheet1, Report02 repV, int ModeSpecCount, ref int RowNum)
        {
            int RowCt = RowNum;
            RowCt = RowCt + 1;
            sheet1.CreateRow(RowCt++).CreateCell(0).SetCellValue("Total for Mode " + ModeCode);
            IRow Row2 = sheet1.CreateRow(RowCt++);
            Row2.CreateCell(8).SetCellValue(repV.OrdinaryManHours);
            Row2.CreateCell(9).SetCellValue(repV.Overtime1ManHours);
            Row2.CreateCell(10).SetCellValue(repV.Overtime2ManHours);
            Row2.CreateCell(11).SetCellValue(repV.Overtime3ManHours);
            Row2.CreateCell(12).SetCellValue(repV.TotalHours);
            Row2.CreateCell(13).SetCellValue(repV.TotalLabourCost);
            Row2.CreateCell(15).SetCellValue(repV.TotalCostOfNumberedPart);
            Row2.CreateCell(16).SetCellValue(repV.TotalCostSuppMat);
            Row2.CreateCell(17).SetCellValue(repV.ImportTax);
            Row2.CreateCell(18).SetCellValue(repV.SalesTaxParts);
            Row2.CreateCell(19).SetCellValue(repV.SalesTaxLabour);
            Row2.CreateCell(20).SetCellValue(repV.TotalToBePaidToShop);
            Row2.CreateCell(21).SetCellValue(repV.TotalToBePaidToShopInUSD);
            Row2.CreateCell(22).SetCellValue(repV.TotalToBePaidToAgentFromCPHInLocalCurrency);
            Row2.CreateCell(23).SetCellValue(repV.TotalToBePaidToAgentFromCPHInUSD);
            Row2.CreateCell(24).SetCellValue(repV.TotalCostOfCPHSuppliedPartsInUSD);
            Row2.CreateCell(25).SetCellValue(repV.TotalCostOfRepairCPHinclCPHsuppliedpartsInUSD);
            sheet1.CreateRow(RowCt++).CreateCell(0).SetCellValue(ModeSpecCount + " Mode " + ModeCode + " Work order(s)");
            RowNum = RowCt;
            return Row2;
        }

        private int FinalSummaryA02(List<Reports> ReportsList, ref ISheet sheet1, Report02 repV, int RowNum)
        {
            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(" Total for Shop ");
            IRow Row6 = sheet1.CreateRow(RowNum++);
            Row6.CreateCell(8).SetCellValue(repV.Tot_OrdinaryManHours);
            Row6.CreateCell(9).SetCellValue(repV.Tot_Overtime1ManHours);
            Row6.CreateCell(10).SetCellValue(repV.Tot_Overtime2ManHours);
            Row6.CreateCell(11).SetCellValue(repV.Tot_Overtime3ManHours);
            Row6.CreateCell(12).SetCellValue(repV.Tot_TotalHours);
            Row6.CreateCell(13).SetCellValue(repV.Tot_TotalLabourCost);
            Row6.CreateCell(15).SetCellValue(repV.Tot_TotalCostOfNumberedPart);
            Row6.CreateCell(16).SetCellValue(repV.Tot_TotalCostSuppMat);
            Row6.CreateCell(17).SetCellValue(repV.Tot_ImportTax);
            Row6.CreateCell(18).SetCellValue(repV.Tot_SalesTaxParts);
            Row6.CreateCell(19).SetCellValue(repV.Tot_SalesTaxLabour);
            Row6.CreateCell(20).SetCellValue(repV.Tot_TotalToBePaidToShop);
            Row6.CreateCell(21).SetCellValue(repV.Tot_TotalToBePaidToShopInUSD);
            Row6.CreateCell(22).SetCellValue(repV.Tot_TotalToBePaidToAgentFromCPHInLocalCurrency);
            Row6.CreateCell(23).SetCellValue(repV.Tot_TotalToBePaidToAgentFromCPHInUSD);
            Row6.CreateCell(24).SetCellValue(repV.Tot_TotalCostOfCPHSuppliedPartsInUSD);
            Row6.CreateCell(25).SetCellValue(repV.Tot_TotalCostOfRepairCPHinclCPHsuppliedpartsInUSD);
            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(" Total number of work orders " + ReportsList.Count());
            return RowNum;
        }

        private void SetFooterTotalA02(string OldModeCode, ISheet sheet1, Report02 repV, int i, int ModeSpecCount)
        {
            int RowNum = sheet1.LastRowNum;
            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue("Total for Mode " + OldModeCode);
            IRow Row2 = sheet1.CreateRow(RowNum++);
            Row2.CreateCell(8).SetCellValue(repV.OrdinaryManHours);
            Row2.CreateCell(9).SetCellValue(repV.Overtime1ManHours);
            Row2.CreateCell(10).SetCellValue(repV.Overtime2ManHours);
            Row2.CreateCell(11).SetCellValue(repV.Overtime3ManHours);
            Row2.CreateCell(12).SetCellValue(repV.TotalHours);
            Row2.CreateCell(13).SetCellValue(repV.TotalLabourCost);
            Row2.CreateCell(15).SetCellValue(repV.TotalCostOfNumberedPart);
            Row2.CreateCell(16).SetCellValue(repV.TotalCostSuppMat);
            Row2.CreateCell(17).SetCellValue(repV.ImportTax);
            Row2.CreateCell(18).SetCellValue(repV.SalesTaxParts);
            Row2.CreateCell(19).SetCellValue(repV.SalesTaxLabour);
            Row2.CreateCell(20).SetCellValue(repV.TotalToBePaidToShop);
            Row2.CreateCell(21).SetCellValue(repV.TotalToBePaidToShopInUSD);
            Row2.CreateCell(22).SetCellValue(repV.TotalToBePaidToAgentFromCPHInLocalCurrency);
            Row2.CreateCell(23).SetCellValue(repV.TotalToBePaidToAgentFromCPHInUSD);
            Row2.CreateCell(24).SetCellValue(repV.TotalCostOfCPHSuppliedPartsInUSD);
            Row2.CreateCell(25).SetCellValue(repV.TotalCostOfRepairCPHinclCPHsuppliedpartsInUSD);
            RowNum = sheet1.LastRowNum;
            sheet1.CreateRow(RowNum + 1).CreateCell(0).SetCellValue(ModeSpecCount + " Mode " + OldModeCode + " Work order(s)");

            UpdateTotalA02(repV);
        }

        private void SetCellValuesMercA02(DataTable dt, ref string CurrentModeCode, ref ISheet sheet1, ref Report02 repV, int i)
        {
            int RowN = sheet1.LastRowNum;
            //RowN++;
            IRow row = sheet1.CreateRow(RowN + 1);
            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(dt.Rows[i][columnName].ToString());

                if (columnName == "Mode")
                {
                    CurrentModeCode = dt.Rows[i][columnName].ToString();
                    //OldModeCode = CurrentModeCode;
                }
                if (columnName == "Ordinary Man Hours")
                {
                    repV.OrdinaryManHours += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Overtime1 Man Hours")
                {
                    repV.Overtime1ManHours += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Overtime2 Man Hours")
                {
                    repV.Overtime2ManHours += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Overtime3 Man Hours")
                {
                    repV.Overtime3ManHours += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total Hours")
                {
                    repV.TotalHours += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total Labour Cost")
                {
                    repV.TotalLabourCost += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total Cost of Numbered Parts")
                {
                    repV.TotalCostOfNumberedPart += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total Cost Of Supplied Materials")
                {
                    repV.TotalCostSuppMat += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Import Tax")
                {
                    repV.ImportTax += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Sales Tax Parts")
                {
                    repV.SalesTaxParts += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Sales Tax Labour")
                {
                    repV.SalesTaxLabour += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total To Be Paid To Shop")
                {
                    repV.TotalToBePaidToShop += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total To Be Paid To Shop In USD")
                {
                    repV.TotalToBePaidToShopInUSD += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total To Be Paid To Agent From CPH In Local Currency")
                {
                    repV.TotalToBePaidToAgentFromCPHInLocalCurrency += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total To Be Paid To Agent From CPH In USD")
                {
                    repV.TotalToBePaidToAgentFromCPHInUSD += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total Cost Of CPH Supplied Parts In USD")
                {
                    repV.TotalCostOfCPHSuppliedPartsInUSD += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total Cost Of Repair CPH (incl. CPH supplied parts) in USD")
                {
                    repV.TotalCostOfRepairCPHinclCPHsuppliedpartsInUSD += Convert.ToDouble(dt.Rows[i][columnName]);
                }
            }
        }

        private void UpdateTotalA02(Report02 repV)
        {
            repV.Tot_OrdinaryManHours += repV.OrdinaryManHours;
            repV.Tot_Overtime1ManHours += repV.Overtime1ManHours;
            repV.Tot_Overtime2ManHours += repV.Overtime2ManHours;
            repV.Tot_Overtime3ManHours += repV.Overtime3ManHours;
            repV.Tot_TotalHours += repV.TotalHours;
            repV.Tot_TotalLabourCost += repV.TotalLabourCost;
            repV.Tot_TotalCostOfNumberedPart += repV.TotalCostOfNumberedPart;
            repV.Tot_TotalCostSuppMat += repV.TotalCostOfNumberedPart;
            repV.Tot_ImportTax += repV.ImportTax;
            repV.Tot_SalesTaxParts += repV.SalesTaxParts;
            repV.Tot_SalesTaxLabour += repV.SalesTaxLabour;
            repV.Tot_TotalToBePaidToShop += repV.TotalToBePaidToShop;
            repV.Tot_TotalToBePaidToShopInUSD += repV.TotalToBePaidToShopInUSD;
            repV.Tot_TotalToBePaidToAgentFromCPHInUSD += repV.TotalToBePaidToAgentFromCPHInUSD;
            repV.Tot_TotalCostOfCPHSuppliedPartsInUSD += repV.TotalCostOfCPHSuppliedPartsInUSD;
            repV.Tot_TotalToBePaidToAgentFromCPHInLocalCurrency += repV.TotalToBePaidToAgentFromCPHInLocalCurrency;
            repV.Tot_TotalCostOfRepairCPHinclCPHsuppliedpartsInUSD += repV.TotalCostOfRepairCPHinclCPHsuppliedpartsInUSD;

            repV.OrdinaryManHours = 0;
            repV.Overtime1ManHours = 0;
            repV.Overtime2ManHours = 0;
            repV.Overtime3ManHours = 0;
            repV.TotalHours = 0;
            repV.TotalLabourCost = 0;
            repV.TotalCostOfNumberedPart = 0;
            repV.TotalCostOfNumberedPart = 0;
            repV.ImportTax = 0;
            repV.SalesTaxParts = 0;
            repV.SalesTaxLabour = 0;
            repV.TotalToBePaidToShop = 0;
            repV.TotalToBePaidToShopInUSD = 0;
            repV.TotalToBePaidToAgentFromCPHInUSD = 0;
            repV.TotalCostOfCPHSuppliedPartsInUSD = 0;
            repV.TotalToBePaidToAgentFromCPHInLocalCurrency = 0;
            repV.TotalCostOfRepairCPHinclCPHsuppliedpartsInUSD = 0;
        }
        #endregion MercA02

        #region MercA01
        private void MercA01(DataTable dt, String extension, ManageReportsModel ManageReportsModel, List<Reports> ReportsList, IWorkbook workbook, ref string CurrentModeCode, ref string OldModeCode, ISheet sheet1)
        {
            dateFrom = ManageReportsModel.DateFrom;
            dateTo = ManageReportsModel.DateTo;
            Report01 repV = new Report01();
            int Count = 0;
            if (ReportsList.Count > 0 && ReportsList[0].HeaderShopDesc != null)
            {
                shop = ManageReportsModel.Shop + ReportsList[0].HeaderShopDesc;
            }
            else
            {
                shop = "-";
            }

            sheet1.CreateRow(0).CreateCell(0).SetCellValue("MERCA01; Summary costs pr shop pr container (CPH costs)");
            sheet1.CreateRow(1).CreateCell(0).SetCellValue("Report Date " + " " + UTCDate + " UTC");
            sheet1.CreateRow(2).CreateCell(0).SetCellValue("Shop " + shop);
            sheet1.CreateRow(3).CreateCell(0).SetCellValue("Period from" + " " + dateFrom + " " + "to" + " " + dateTo);
            sheet1.CreateRow(4).CreateCell(0).SetCellValue("All values in US Dollars");
            IRow row1 = sheet1.CreateRow(5);


            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row1.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
            }
            int i = 0;
            int ModeSpecCount = 0;
            int RowNum = 0;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                CurrentModeCode = dt.Rows[i]["Mode"].ToString();
                if (i == 0)
                {
                    OldModeCode = CurrentModeCode;
                    ModeSpecCount++;
                    SetCellValuesMercA01(dt, ref CurrentModeCode, ref sheet1, ref repV, i);
                    if (i == dt.Rows.Count - 1)
                    {
                        RowNum = (sheet1.LastRowNum);
                        IRow I = PrintModeSummaryA01(CurrentModeCode, ref sheet1, repV, ModeSpecCount, ref RowNum);
                        UpdateTotalA01(repV);
                        RowNum = FinalSummaryA01(ReportsList, ref sheet1, repV, RowNum++);
                    }
                }
                else
                {
                    if (string.Equals(CurrentModeCode, OldModeCode, StringComparison.OrdinalIgnoreCase))
                    {
                        ModeSpecCount++;
                        SetCellValuesMercA01(dt, ref CurrentModeCode, ref sheet1, ref repV, i);
                        if (i == dt.Rows.Count - 1)
                        {

                            RowNum = (sheet1.LastRowNum);
                            IRow I = PrintModeSummaryA01(CurrentModeCode, ref sheet1, repV, ModeSpecCount, ref RowNum);
                            UpdateTotalA01(repV);
                            RowNum = FinalSummaryA01(ReportsList, ref sheet1, repV, RowNum++);
                        }
                    }
                    else
                    {
                        //write the comment code after that
                        //SetFooterTotalA01(OldModeCode, sheet1, repV, i, ModeSpecCount);'

                        // UpdateTotalA01(repV);
                        // IRow Row2 = sheet1.CreateRow(RowNum++);
                        // Row2 = PrintModeSummaryA01(OldModeCode, sheet1, repV, ModeSpecCount, ref RowNum);
                        //sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(ModeSpecCount + " Mode " + OldModeCode + " Work order(s)");


                        RowNum = sheet1.LastRowNum;
                        IRow Row5 = PrintModeSummaryA01(OldModeCode, ref sheet1, repV, ModeSpecCount, ref RowNum);
                        UpdateTotalA01(repV);
                        OldModeCode = CurrentModeCode;
                        ModeSpecCount = 0;

                        if (i == dt.Rows.Count - 1)
                        {
                            ModeSpecCount++;
                            SetCellValuesMercA01(dt, ref CurrentModeCode, ref sheet1, ref repV, i);
                            RowNum = sheet1.LastRowNum;
                            IRow Row4 = PrintModeSummaryA01(CurrentModeCode, ref sheet1, repV, ModeSpecCount, ref RowNum);
                            UpdateTotalA01(repV);
                            RowNum = FinalSummaryA01(ReportsList, ref sheet1, repV, RowNum++);
                        }
                        else
                        {
                            ModeSpecCount++;
                            SetCellValuesMercA01(dt, ref CurrentModeCode, ref sheet1, ref repV, i);
                        }
                    }
                }
            }
            //sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(ModeSpecCount + " Mode " + OldModeCode + " Work order(s)");
            // RowNum = FinalSummary(ReportsList, sheet1, repV, RowNum);

            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                workbook.Write(exportData);
                if (extension == "xlsx") //xlsx file format
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "MERCA01.xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.Flush();
                    Response.End();
                }
            }
        }

        private int FinalSummaryA01(List<Reports> ReportsList, ref ISheet sheet1, Report01 repV, int RowNum)
        {
            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(" Total for Shop ");
            IRow Row6 = sheet1.CreateRow(RowNum++);
            if (IsVoucherReqd)
            {
                Row6.CreateCell(7).SetCellValue(repV.Tot_OrdinaryManHours);
                Row6.CreateCell(8).SetCellValue(repV.Tot_Overtime1ManHours);
                Row6.CreateCell(9).SetCellValue(repV.Tot_Overtime2ManHours);
                Row6.CreateCell(10).SetCellValue(repV.Tot_Overtime3ManHours);
                Row6.CreateCell(11).SetCellValue(repV.Tot_TotalHours);
                Row6.CreateCell(12).SetCellValue(repV.Tot_TotalLabourCost);
                Row6.CreateCell(13).SetCellValue(repV.Tot_TotalCostOfShopSuppliedNumberedPart);
                Row6.CreateCell(14).SetCellValue(repV.Tot_TotalCostShopSuppMat);
                Row6.CreateCell(15).SetCellValue(repV.Tot_ImportTax);
                Row6.CreateCell(16).SetCellValue(repV.Tot_SalesTaxParts);
                Row6.CreateCell(17).SetCellValue(repV.Tot_SalesTaxLabour);
                Row6.CreateCell(18).SetCellValue(repV.Tot_TotalToBePaidToShopAgent);
                Row6.CreateCell(19).SetCellValue(repV.Tot_TotalCostOfCPHSuppliedParts);
                Row6.CreateCell(20).SetCellValue(repV.Tot_TotalCostOfRepairinclCPHsuppliedparts);
                sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(" Total number of work orders " + ReportsList.Count());
            }
            else
            {
                Row6.CreateCell(5).SetCellValue(repV.Tot_OrdinaryManHours);
                Row6.CreateCell(6).SetCellValue(repV.Tot_Overtime1ManHours);
                Row6.CreateCell(7).SetCellValue(repV.Tot_Overtime2ManHours);
                Row6.CreateCell(8).SetCellValue(repV.Tot_Overtime3ManHours);
                Row6.CreateCell(9).SetCellValue(repV.Tot_TotalHours);
                Row6.CreateCell(10).SetCellValue(repV.Tot_TotalLabourCost);
                Row6.CreateCell(11).SetCellValue(repV.Tot_TotalCostOfShopSuppliedNumberedPart);
                Row6.CreateCell(12).SetCellValue(repV.Tot_TotalCostShopSuppMat);
                Row6.CreateCell(13).SetCellValue(repV.Tot_ImportTax);
                Row6.CreateCell(14).SetCellValue(repV.Tot_SalesTaxParts);
                Row6.CreateCell(15).SetCellValue(repV.Tot_SalesTaxLabour);
                Row6.CreateCell(16).SetCellValue(repV.Tot_TotalToBePaidToShopAgent);
                Row6.CreateCell(17).SetCellValue(repV.Tot_TotalCostOfCPHSuppliedParts);
                Row6.CreateCell(18).SetCellValue(repV.Tot_TotalCostOfRepairinclCPHsuppliedparts);
                sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(" Total number of work orders " + ReportsList.Count());
            }
            return RowNum;
        }

        private IRow PrintModeSummaryA01(string ModeCode, ref ISheet sheet1, Report01 repV, int ModeSpecCount, ref int RowNum)
        {
            int RowCt = RowNum;
            RowCt = RowCt + 1;
            sheet1.CreateRow(RowCt++).CreateCell(0).SetCellValue("Total for Mode " + ModeCode);
            IRow Row2 = sheet1.CreateRow(RowCt++);
            if (IsVoucherReqd)
            {
                Row2.CreateCell(7).SetCellValue(repV.OrdinaryManHours);
                Row2.CreateCell(8).SetCellValue(repV.Overtime1ManHours);
                Row2.CreateCell(9).SetCellValue(repV.Overtime2ManHours);
                Row2.CreateCell(10).SetCellValue(repV.Overtime3ManHours);
                Row2.CreateCell(11).SetCellValue(repV.TotalHours);
                Row2.CreateCell(12).SetCellValue(repV.TotalLabourCost);
                Row2.CreateCell(13).SetCellValue(repV.TotalCostOfShopSuppliedNumberedPart);
                Row2.CreateCell(14).SetCellValue(repV.TotalCostShopSuppMat);
                Row2.CreateCell(15).SetCellValue(repV.ImportTax);
                Row2.CreateCell(16).SetCellValue(repV.SalesTaxParts);
                Row2.CreateCell(17).SetCellValue(repV.SalesTaxLabour);
                Row2.CreateCell(18).SetCellValue(repV.TotalToBePaidToShopAgent);
                Row2.CreateCell(19).SetCellValue(repV.TotalCostOfCPHSuppliedParts);
                Row2.CreateCell(20).SetCellValue(repV.TotalCostOfRepairinclCPHsuppliedparts);
                sheet1.CreateRow(RowCt++).CreateCell(0).SetCellValue(ModeSpecCount + " Mode " + ModeCode + " Work order(s)");
            }
            else
            {
                Row2.CreateCell(5).SetCellValue(repV.OrdinaryManHours);
                Row2.CreateCell(6).SetCellValue(repV.Overtime1ManHours);
                Row2.CreateCell(7).SetCellValue(repV.Overtime2ManHours);
                Row2.CreateCell(8).SetCellValue(repV.Overtime3ManHours);
                Row2.CreateCell(9).SetCellValue(repV.TotalHours);
                Row2.CreateCell(10).SetCellValue(repV.TotalLabourCost);
                Row2.CreateCell(11).SetCellValue(repV.TotalCostOfShopSuppliedNumberedPart);
                Row2.CreateCell(12).SetCellValue(repV.TotalCostShopSuppMat);
                Row2.CreateCell(13).SetCellValue(repV.ImportTax);
                Row2.CreateCell(14).SetCellValue(repV.SalesTaxParts);
                Row2.CreateCell(15).SetCellValue(repV.SalesTaxLabour);
                Row2.CreateCell(16).SetCellValue(repV.TotalToBePaidToShopAgent);
                Row2.CreateCell(17).SetCellValue(repV.TotalCostOfCPHSuppliedParts);
                Row2.CreateCell(18).SetCellValue(repV.TotalCostOfRepairinclCPHsuppliedparts);
                sheet1.CreateRow(RowCt++).CreateCell(0).SetCellValue(ModeSpecCount + " Mode " + ModeCode + " Work order(s)");
            }
            RowNum = RowCt;
            return Row2;
        }

        private void SetFooterTotalA01(string OldModeCode, ISheet sheet1, Report01 repV, int i, int ModeSpecCount)
        {
            int RowNum = sheet1.LastRowNum;
            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue("Total for Mode " + OldModeCode);
            IRow Row2 = sheet1.CreateRow(RowNum++);
            Row2.CreateCell(7).SetCellValue(repV.OrdinaryManHours);
            Row2.CreateCell(8).SetCellValue(repV.Overtime1ManHours);
            Row2.CreateCell(9).SetCellValue(repV.Overtime2ManHours);
            Row2.CreateCell(10).SetCellValue(repV.Overtime3ManHours);
            Row2.CreateCell(11).SetCellValue(repV.TotalHours);
            Row2.CreateCell(12).SetCellValue(repV.TotalLabourCost);
            Row2.CreateCell(13).SetCellValue(repV.TotalCostOfShopSuppliedNumberedPart);
            Row2.CreateCell(14).SetCellValue(repV.TotalCostShopSuppMat);
            Row2.CreateCell(15).SetCellValue(repV.ImportTax);
            Row2.CreateCell(16).SetCellValue(repV.SalesTaxParts);
            Row2.CreateCell(17).SetCellValue(repV.SalesTaxLabour);
            Row2.CreateCell(18).SetCellValue(repV.TotalToBePaidToShopAgent);
            Row2.CreateCell(19).SetCellValue(repV.TotalCostOfCPHSuppliedParts);
            Row2.CreateCell(20).SetCellValue(repV.TotalCostOfRepairinclCPHsuppliedparts);
            RowNum = sheet1.LastRowNum;
            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(ModeSpecCount + " Mode " + OldModeCode + " Work order(s)");

            UpdateTotalA01(repV);
        }

        private void SetCellValuesMercA01(DataTable dt, ref string CurrentModeCode, ref ISheet sheet1, ref Report01 repV, int i)
        {
            int RowN = sheet1.LastRowNum;
            //RowN++;
            IRow row = sheet1.CreateRow(RowN + 1);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(dt.Rows[i][columnName].ToString());
                if (columnName == "Mode")
                {
                    CurrentModeCode = dt.Rows[i][columnName].ToString();
                    //OldModeCode = CurrentModeCode;
                }
                if (columnName == "Ordinary Man Hours")
                {
                    repV.OrdinaryManHours += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Overtime1 Man Hours")
                {
                    repV.Overtime1ManHours += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Overtime2 Man Hours")
                {
                    repV.Overtime2ManHours += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Overtime3 Man Hours")
                {
                    repV.Overtime3ManHours += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total Hours")
                {
                    repV.TotalHours += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total Labour Cost")
                {
                    repV.TotalLabourCost += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total Cost Of Shop Supplied Numbered Part")
                {
                    repV.TotalCostOfShopSuppliedNumberedPart += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total Cost Of Shop Supplied Materials")
                {
                    repV.TotalCostShopSuppMat += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Import Tax")
                {
                    repV.ImportTax += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Sales Tax Parts")
                {
                    repV.SalesTaxParts += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Sales Tax Labour")
                {
                    repV.SalesTaxLabour += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total To Be Paid To Shop/Agent")
                {
                    repV.TotalToBePaidToShopAgent += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total Cost Of CPH Supplied Parts")
                {
                    repV.Tot_TotalCostOfCPHSuppliedParts += Convert.ToDouble(dt.Rows[i][columnName]);
                }
                if (columnName == "Total Cost Of Repair (incl. CPH supplied parts)")
                {
                    repV.TotalCostOfRepairinclCPHsuppliedparts += Convert.ToDouble(dt.Rows[i][columnName]);
                }
            }
        }

        private void UpdateTotalA01(Report01 repV)
        {
            repV.Tot_OrdinaryManHours += repV.OrdinaryManHours;
            repV.Tot_Overtime1ManHours += repV.Overtime1ManHours;
            repV.Tot_Overtime2ManHours += repV.Overtime2ManHours;
            repV.Tot_Overtime3ManHours += repV.Overtime3ManHours;
            repV.Tot_TotalHours += repV.TotalHours;
            repV.Tot_TotalLabourCost += repV.TotalLabourCost;
            repV.Tot_TotalCostOfShopSuppliedNumberedPart += repV.TotalCostOfShopSuppliedNumberedPart;
            repV.Tot_TotalCostShopSuppMat += repV.TotalCostShopSuppMat;
            repV.Tot_ImportTax += repV.ImportTax;
            repV.Tot_SalesTaxParts += repV.SalesTaxParts;
            repV.Tot_SalesTaxLabour += repV.SalesTaxLabour;
            repV.Tot_TotalToBePaidToShopAgent += repV.TotalToBePaidToShopAgent;
            repV.Tot_TotalCostOfCPHSuppliedParts += repV.TotalCostOfCPHSuppliedParts;
            repV.Tot_TotalCostOfRepairinclCPHsuppliedparts += repV.TotalCostOfRepairinclCPHsuppliedparts;

            repV.OrdinaryManHours = 0;
            repV.Overtime1ManHours = 0;
            repV.Overtime2ManHours = 0;
            repV.Overtime3ManHours = 0;
            repV.TotalHours = 0;
            repV.TotalLabourCost = 0;
            repV.TotalCostOfShopSuppliedNumberedPart = 0;
            repV.TotalCostShopSuppMat = 0;
            repV.ImportTax = 0;
            repV.SalesTaxParts = 0;
            repV.SalesTaxLabour = 0;
            repV.TotalToBePaidToShopAgent = 0;
            repV.TotalCostOfCPHSuppliedParts = 0;
            repV.TotalCostOfRepairinclCPHsuppliedparts = 0;
        }

        #endregion MercA01

        private int FinalSummaryB01(List<Reports> ReportsList, ref ISheet sheet1, double TotalCost, int RowNum)
        {
            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(" Total for Shop ");
            IRow Row6 = sheet1.CreateRow(RowNum++);
            Row6.CreateCell(11).SetCellValue(TotalCost);

            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(" Total number of work orders " + ReportsList.Count());
            return RowNum;
        }

        private int FinalSummaryE(List<Reports> ReportsList, ref ISheet sheet1, int RowNum)
        {
            //sheet1.CreateRow(RowNo++).CreateCell(0).SetCellValue("Total No of Work order(s) " + ReportsList.Count());
            //sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(" Total for Shop ");
            //IRow Row6 = sheet1.CreateRow(RowNum++);
            //Row6.CreateCell(11).SetCellValue(TotalCost);

            sheet1.CreateRow(RowNum++).CreateCell(0).SetCellValue(" Total number of work orders " + ReportsList.Count());
            return RowNum;
        }


        private List<SelectListItem> PopulateReportsDropDown(List<SelectListItem> ReportsList) //debadrita leo report
        {
            if (ManageReportsModel.isAdmin || ManageReportsModel.isAnyCPH || ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop || ManageReportsModel.isEMRSpecialistCountry || ManageReportsModel.isEMRSpecialistShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCA01 - Billing report for CPH",
                    Value = "1"
                });
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCA02 - Billing report for agent",
                    Value = "2"
                });
            }
            if (ManageReportsModel.isAdmin || ManageReportsModel.isAnyCPH || ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop || ManageReportsModel.isShop || ManageReportsModel.isEMRSpecialistCountry || ManageReportsModel.isEMRSpecialistShop || ManageReportsModel.isMPROCluster || ManageReportsModel.isMPROShop || ManageReportsModel.isReadOnly)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCA03 - Billing report for shop",
                    Value = "3"
                });
            }
            if (ManageReportsModel.isAdmin || ManageReportsModel.isAnyCPH || ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop || ManageReportsModel.isShop || ManageReportsModel.isEMRSpecialistCountry || ManageReportsModel.isEMRSpecialistShop || ManageReportsModel.isMPROCluster || ManageReportsModel.isMPROShop || ManageReportsModel.isReadOnly)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCB01 - Spare part usage pr container",
                    Value = "4"
                });
            }
            if (ManageReportsModel.isAdmin || ManageReportsModel.isAnyCPH || ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop || ManageReportsModel.isShop || ManageReportsModel.isEMRSpecialistCountry || ManageReportsModel.isEMRSpecialistShop || ManageReportsModel.isMPROCluster || ManageReportsModel.isMPROShop || ManageReportsModel.isReadOnly)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCC01 - Exclusionary Codes",
                    Value = "5"
                });
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCC02 - Mode/Code/part number associations",
                    Value = "6"
                });
            }
            if (ManageReportsModel.isAdmin || ManageReportsModel.isAnyCPH || ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop || ManageReportsModel.isShop || ManageReportsModel.isEMRSpecialistCountry || ManageReportsModel.isEMRSpecialistShop || ManageReportsModel.isMPROCluster || ManageReportsModel.isMPROShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCD03 - Shop Contract",
                    Value = "7"
                });
            }
            if (ManageReportsModel.isAdmin || ManageReportsModel.isAnyCPH || ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop || ManageReportsModel.isEMRSpecialistCountry || ManageReportsModel.isEMRSpecialistShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCD05 - Country Contract",
                    Value = "8"
                });
            }
            if (ManageReportsModel.isAdmin || ManageReportsModel.isCPH || ManageReportsModel.isEMRSpecialistCountry || ManageReportsModel.isEMRSpecialistShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCE01 - Equipment Repair Status",
                    Value = "9"
                });
            }
            if (ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCE02 - Equipment Repair Status",
                    Value = "10"
                });
            }
            if (ManageReportsModel.isShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCE03 - Equipment Repair Status",
                    Value = "11"
                });
            }
            return ReportsList;
        }


        private List<SelectListItem> PopulateReportsDropDown_old(List<SelectListItem> ReportsList)
        {
            if (ManageReportsModel.isAdmin || ManageReportsModel.isAnyCPH || ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCA01 - Billing report for CPH",
                    Value = "1"
                });
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCA02 - Billing report for agent",
                    Value = "2"
                });
            }
            if (ManageReportsModel.isAdmin || ManageReportsModel.isAnyCPH || ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop   || ManageReportsModel.isShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCA03 - Billing report for shop",
                    Value = "3"
                });
            }
            if (ManageReportsModel.isAdmin || ManageReportsModel.isAnyCPH || ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop || ManageReportsModel.isShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCB01 - Spare part usage pr container",
                    Value = "4"
                });
            }
            if (ManageReportsModel.isAdmin || ManageReportsModel.isAnyCPH || ManageReportsModel.isEMRApproverShop || ManageReportsModel.isShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCC01 - Exclusionary Codes",
                    Value = "5"
                });
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCC02 - Mode/Code/part number associations",
                    Value = "6"
                });
            }
            if (ManageReportsModel.isAdmin || ManageReportsModel.isCPH || ManageReportsModel.isEMRSpecialistCountry 
                || ManageReportsModel.isEMRSpecialistShop || ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop || ManageReportsModel.isShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCD03 - Shop Contract",
                    Value = "7"
                });
            }
            if (ManageReportsModel.isAdmin || ManageReportsModel.isAnyCPH || ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCD05 - Country Contract",
                    Value = "8"
                });
            }
            if (ManageReportsModel.isAdmin || ManageReportsModel.isCPH || ManageReportsModel.isEMRSpecialistCountry || ManageReportsModel.isEMRSpecialistShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCE01 - Equipment Repair Status",
                    Value = "9"
                });
            }
            if (ManageReportsModel.isEMRApproverCountry || ManageReportsModel.isEMRApproverShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCE02 - Equipment Repair Status",
                    Value = "10"
                });
            }
            if (ManageReportsModel.isShop)
            {
                ReportsList.Add(new SelectListItem
                {
                    Text = "MERCE03 - Equipment Repair Status",
                    Value = "11"
                });
            }
            return ReportsList;
        }

        private ManageReportsModel PopulateAllDropDowns(ManageReportsModel ManageReportsModel, int ReportsID)
        {
            string customerCode = string.Empty;
            string manualCode = string.Empty;
            List<ErrMessage> MercPlusErrorMessageList = new List<ErrMessage>();
            int UserID = Convert.ToInt32(((UserSec)Session["UserSec"]).UserId);

            #region CountryDropDown
            List<SelectListItem> CountryDropDown = new List<SelectListItem>();
            List<Country> CountryList = new List<Country>();
            //Llist = ManageReportsModel.GetLocation();

            CountryList = ManageReportsClient.GetCountryList(out MercPlusErrorMessageList, UserID, Role).ToList();

            foreach (var country in CountryList)
            {
                CountryDropDown.Add(new SelectListItem
                {
                    Text = country.CountryCode + "-" + country.CountryDescription,
                    Value = country.CountryCode
                });
            }
            if (CountryDropDown != null && CountryDropDown.Count > 0 && ReportsID == 8)
            {
                CountryDropDown[0].Selected = true;
            }
            ManageReportsModel.drpCountry = CountryDropDown;
            #endregion CountryDropDown

            #region ShopDropDown
            List<SelectListItem> ShopDropDown = new List<SelectListItem>();
            List<Shop> ShopList = new List<Shop>();

            ShopList = ManageReportsClient.GetShopList(out MercPlusErrorMessageList, UserID).ToList();

            foreach (var Shop in ShopList)
            {
                ShopDropDown.Add(new SelectListItem
                {
                    Text = Shop.ShopCode + "-" + Shop.ShopDescription,
                    Value = Shop.ShopCode
                });
            }
            if (ShopDropDown != null && ShopDropDown.Count > 0)
            {
                ShopDropDown[0].Selected = true;
            }
            string shopCode = ShopDropDown[0].Value;
            ManageReportsModel.drpShop = ShopDropDown;
            ViewBag.ShopList = new SelectList(ShopList, "ShopCode", "ShopDescription");
            #endregion ShopDropDown

            #region CustomerDropDown
            List<SelectListItem> CustomerDropDown = new List<SelectListItem>();
            List<Customer> CustomerList = new List<Customer>();
            CustomerList = ManageReportsClient.GetCustomerList(out MercPlusErrorMessageList).ToList();

            foreach (var customer in CustomerList)
            {
                CustomerDropDown.Add(new SelectListItem
                {
                    Text = customer.CustomerCode + "-" + customer.CustomerDesc,
                    Value = customer.CustomerCode
                });
            }
            foreach (var i in CustomerDropDown)
            {
                if (i.Value == "MAER")
                {
                    i.Selected = true;
                    customerCode = i.Value;
                }
            }
            ManageReportsModel.drpCustomer = CustomerDropDown;

            #endregion CustomerDropDown

            #region ManualDropDown
            List<SelectListItem> ManualDropDown = new List<SelectListItem>();
            List<Manual> ManualList = new List<Manual>();
            ManualList = ManageReportsClient.GetManualList(out MercPlusErrorMessageList).ToList();

            foreach (var manual in ManualList)
            {
                ManualDropDown.Add(new SelectListItem
                {
                    Text = manual.ManualCode + "-" + manual.ManualDesc,
                    Value = manual.ManualCode
                });
            }
            foreach (var i in ManualDropDown)
            {
                if (i.Value == "MAER")
                {
                    i.Selected = true;
                    manualCode = i.Value;
                }
            }
            ManageReportsModel.drpManual = ManualDropDown;

            #endregion ManualDropDown

            #region ModeDropDown
            List<SelectListItem> ModeDropDown = new List<SelectListItem>();
            List<Mode> ModeList = new List<Mode>();
            //ModeList = ManageReportsClient.GetModeList().ToList();
            ModeList = ManageReportsClient.GetModeListOnConditions(shopCode, customerCode, manualCode).ToList();

            foreach (var mode in ModeList)
            {
                ModeDropDown.Add(new SelectListItem
                {
                    Text = mode.ModeCode + "-" + mode.ModeDescription,
                    Value = mode.ModeCode
                });
            }
            ManageReportsModel.drpMode = ModeDropDown;
            #endregion ModeDropDown

            #region AreaDropDown
            List<SelectListItem> AreaDropDown = new List<SelectListItem>();
            List<Area> AreaList = new List<Area>();
           // AreaList = ManageReportsClient.GetAreaList(out MercPlusErrorMessageList, UserID).ToList();
            AreaList = ManageReportsClient.GetAreaList(out MercPlusErrorMessageList, UserID, Role).ToList(); //Area_drop_down_bug_fix_Debadrita

            foreach (var Area in AreaList)
            {
                AreaDropDown.Add(new SelectListItem
                {
                    Text = Area.AreaCode + "-" + Area.AreaDescription,
                    Value = Area.AreaCode
                });
            }
            if (AreaDropDown != null && AreaDropDown.Count > 0)
            {
                AreaDropDown[0].Selected = true;
            }
            ManageReportsModel.drpArea = AreaDropDown;
            #endregion AreaDropDown

            #region STSDropDown
            List<SelectListItem> RepairDropDown = new List<SelectListItem>();
            List<RepairCode> RepairList = new List<RepairCode>();
            RepairList = ManageReportsClient.GetRepairCodeList(out MercPlusErrorMessageList, shopCode, customerCode, manualCode, string.Empty).ToList();

            foreach (var Repair in RepairList)
            {
                RepairDropDown.Add(new SelectListItem
                {
                    Text = Repair.RepairCod + "-" + Repair.RepairDesc,
                    Value = Repair.RepairCod
                });
            }
            if (RepairDropDown != null && RepairDropDown.Count > 0)
            {
                RepairDropDown[0].Selected = true;
            }
            ManageReportsModel.drpSTSCode = RepairDropDown;
            #endregion STSDropDown

            #region DaysDropDown
            List<SelectListItem> DaysList = new List<SelectListItem>();
            DaysList.Add(new SelectListItem
            {
                Text = "10",
                Value = "10"
            });
            DaysList.Add(new SelectListItem
            {
                Text = "20",
                Value = "20"
            });
            DaysList.Add(new SelectListItem
            {
                Text = "30",
                Value = "30"
            });
            DaysList.Add(new SelectListItem
            {
                Text = "40",
                Value = "40"
            });
            DaysList.Add(new SelectListItem
            {
                Text = "50",
                Value = "50"
            });
            DaysList.Add(new SelectListItem
            {
                Text = "60",
                Value = "60"
            });

            DaysList[0].Selected = true;
            ManageReportsModel.drpDays = DaysList;
            #endregion DaysDropDown

            return ManageReportsModel;
        }

        [HttpPost]
        public JsonResult GetShopList(string CountryCode, string CustomerCode, string ManualCode)
        {
            List<ErrMessage> MercPlusErrorMessageList = new List<ErrMessage>();
            List<Shop> ShopList = new List<Shop>();
            int UserID = Convert.ToInt32(((UserSec)Session["UserSec"]).UserId);
            try
            {
                if (!string.IsNullOrEmpty(CountryCode))
                {
                    ShopList = ManageReportsClient.GetShopListOnCountryCode(out MercPlusErrorMessageList, CountryCode).ToList();
                }
                else
                {
                    ShopList = ManageReportsClient.GetShopList(out MercPlusErrorMessageList, UserID).ToList();
                }

                #region ShopDropDown
                List<SelectListItem> ShopDropDown = new List<SelectListItem>();
                List<SelectListItem> ModeDropDown = new List<SelectListItem>();
                //ShopList = ManageReportsClient.GetShopList().ToList();

                foreach (var Shop in ShopList)
                {
                    ShopDropDown.Add(new SelectListItem
                    {
                        Text = Shop.ShopCode + "-" + Shop.ShopDescription,
                        Value = Shop.ShopCode
                    });
                }
                if (ShopList != null && ShopList.Count != 0)
                {
                    ShopDropDown[0].Selected = true;
                    if (string.IsNullOrEmpty(CustomerCode))
                    {
                        CustomerCode = null;
                    }
                    if (string.IsNullOrEmpty(ManualCode))
                    {
                        ManualCode = null;
                    }
                    List<Mode> ModeList = ManageReportsClient.GetModeListOnConditions(ShopList[0].ShopCode, CustomerCode, ManualCode).ToList();
                    ManageReportsModel = PopulateModeDropDown(ModeList);
                }
                ManageReportsModel.drpShop = ShopDropDown;
                //ManageReportsModel.drpMode = ModeDropDown;
                //ViewBag.ShopList = new SelectList(ShopList, "ShopCode", "ShopDescription");
                #endregion ShopDropDown


            }
            catch (Exception ex)
            {
                string error = ex.ToString();
            }

            return Json(ManageReportsModel);
        }

        public JsonResult GetModeList(string ShopCode, string CustomerCode, string ManualCode)
        {
            List<ErrMessage> MercPlusErrorMessageList = new List<ErrMessage>();
            if (string.IsNullOrEmpty(CustomerCode))
            {
                CustomerCode = null;
            }
            if (string.IsNullOrEmpty(ManualCode))
            {
                ManualCode = null;
            }
            if (string.IsNullOrEmpty(ShopCode))
            {
                ShopCode = null;
            }
            List<Mode> ModeList = ManageReportsClient.GetModeListOnConditions(ShopCode, CustomerCode, ManualCode).ToList();
            if (ModeList != null)
            {
                ManageReportsModel = PopulateModeDropDown(ModeList);
            }
            return Json(ManageReportsModel);
        }

        public JsonResult GetRepairCodeList(string ShopCode, string CustomerCode, string ManualCode, string ModeCode)
        {
            if (ModeCode != null && ModeCode.Equals(ANY, StringComparison.CurrentCultureIgnoreCase))
            {
                ModeCode = null;
            }
            List<ErrMessage> MercPlusErrorMessageList = new List<ErrMessage>();
            List<RepairCode> RepairCodeList = new List<RepairCode>();
            RepairCodeList = ManageReportsClient.GetRepairCodeList(out MercPlusErrorMessageList, ShopCode, CustomerCode, ManualCode, ModeCode).ToList();
            if (RepairCodeList != null)
            {
                ManageReportsModel = PopulateSTSCodeDropDown(RepairCodeList);
            }
            return Json(ManageReportsModel);
        }

        private ManageReportsModel PopulateModeDropDown(List<Mode> ModeList)
        {


            #region ModeDropDown
            List<SelectListItem> ModeDropDown = new List<SelectListItem>();
            //ModeList = ManageReportsClient.GetModeList().ToList();

            foreach (var Mode in ModeList)
            {
                ModeDropDown.Add(new SelectListItem
                {
                    Text = Mode.ModeCode + "-" + Mode.ModeDescription,
                    Value = Mode.ModeCode
                });
            }
            ManageReportsModel.drpMode = ModeDropDown;
            #endregion ModeDropDown

            return ManageReportsModel;
        }

        private ManageReportsModel PopulateSTSCodeDropDown(List<RepairCode> RepairCodeList)
        {


            #region RepairDropDown
            List<SelectListItem> STSDropDown = new List<SelectListItem>();
            //ModeList = ManageReportsClient.GetModeList().ToList();

            foreach (var Repair in RepairCodeList)
            {
                STSDropDown.Add(new SelectListItem
                {
                    Text = Repair.RepairCod + "-" + Repair.RepairDesc,
                    Value = Repair.RepairCod
                });
            }
            ManageReportsModel.drpSTSCode = STSDropDown;
            #endregion RepairDropDown

            return ManageReportsModel;
        }

        public ActionResult DisplayPartialView(int Reports_ID)
        {
            //set the user access
            SetUserAccess();
            if (Reports_ID != 5 && Reports_ID != 6)
            {
                List<SelectListItem> CountryDropDown = new List<SelectListItem>();
                List<Country> CountryList = new List<Country>();
                //Llist = ManageReportsModel.GetLocation();
                //CountryList = ManageReportsClient.GetCountryList().ToList();

                //ManageReportsModel.drpCountry = CountryDropDown; 
                ManageReportsModel = PopulateAllDropDowns(ManageReportsModel, Reports_ID);
                //ManageReportsModel.drpDays = PopulateDaysDropDown();
            }
            ManageReportsModel.ReportsID = Reports_ID;
            return PartialView("ReportsPartialView", ManageReportsModel);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session.IsNewSession || Session["UserSec"] == null || Session["UserSec"] == "")
            {
                filterContext.Result = new RedirectResult("/ManageSecurity/ManageSecurity/SessionExpire");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class Report03
    {
        public double OrdinaryManHours { get; set; }
        public double Overtime1ManHours { get; set; }
        public double Overtime2ManHours { get; set; }
        public double Overtime3ManHours { get; set; }
        public double TotalHours { get; set; }
        public double TotalLabourCost { get; set; }
        public double TotalCostOfNumberedParts { get; set; }
        public double TotalCostOfShopSuppliedMaterials { get; set; }
        public double ImportTax { get; set; }
        public double SalesTaxParts { get; set; }
        public double SalesTaxLabour { get; set; }
        public double TotalToBePaidToShop { get; set; }

        public double Tot_OrdinaryManHours { get; set; }
        public double Tot_Overtime1ManHours { get; set; }
        public double Tot_Overtime2ManHours { get; set; }
        public double Tot_Overtime3ManHours { get; set; }
        public double Tot_TotalHours { get; set; }
        public double Tot_TotalLabourCost { get; set; }
        public double Tot_TotalCostOfNumberedParts { get; set; }
        public double Tot_TotalCostOfShopSuppliedMaterials { get; set; }
        public double Tot_ImportTax { get; set; }
        public double Tot_SalesTaxParts { get; set; }
        public double Tot_SalesTaxLabour { get; set; }
        public double Tot_TotalToBePaidToShop { get; set; }
    }


    public class Report01
    {
        public double OrdinaryManHours { get; set; }
        public double Overtime1ManHours { get; set; }
        public double Overtime2ManHours { get; set; }
        public double Overtime3ManHours { get; set; }
        public double TotalHours { get; set; }
        public double TotalLabourCost { get; set; }
        public double TotalCostOfShopSuppliedNumberedPart { get; set; }
        public double TotalCostShopSuppMat { get; set; }
        public double ImportTax { get; set; }
        public double SalesTaxParts { get; set; }
        public double SalesTaxLabour { get; set; }
        public double TotalToBePaidToShopAgent { get; set; }
        public double TotalCostOfCPHSuppliedParts { get; set; }
        public double TotalCostOfRepairinclCPHsuppliedparts { get; set; }

        public double Tot_OrdinaryManHours { get; set; }
        public double Tot_Overtime1ManHours { get; set; }
        public double Tot_Overtime2ManHours { get; set; }
        public double Tot_Overtime3ManHours { get; set; }
        public double Tot_TotalHours { get; set; }
        public double Tot_TotalLabourCost { get; set; }
        public double Tot_TotalCostOfShopSuppliedNumberedPart { get; set; }
        public double Tot_TotalCostShopSuppMat { get; set; }
        public double Tot_ImportTax { get; set; }
        public double Tot_SalesTaxParts { get; set; }
        public double Tot_SalesTaxLabour { get; set; }
        public double Tot_TotalToBePaidToShopAgent { get; set; }
        public double Tot_TotalCostOfCPHSuppliedParts { get; set; }
        public double Tot_TotalCostOfRepairinclCPHsuppliedparts { get; set; }
    }

    public class Report02
    {
        public double OrdinaryManHours { get; set; }
        public double Overtime1ManHours { get; set; }
        public double Overtime2ManHours { get; set; }
        public double Overtime3ManHours { get; set; }
        public double TotalHours { get; set; }
        public double TotalLabourCost { get; set; }
        public double TotalCostOfNumberedPart { get; set; }
        public double TotalCostSuppMat { get; set; }
        public double ImportTax { get; set; }
        public double SalesTaxParts { get; set; }
        public double SalesTaxLabour { get; set; }
        public double TotalToBePaidToShop { get; set; }
        public double TotalToBePaidToShopInUSD { get; set; }
        public double TotalToBePaidToAgentFromCPHInLocalCurrency { get; set; }
        public double TotalToBePaidToAgentFromCPHInUSD { get; set; }
        public double TotalCostOfCPHSuppliedPartsInUSD { get; set; }
        public double TotalCostOfRepairCPHinclCPHsuppliedpartsInUSD { get; set; }

        public double Tot_OrdinaryManHours { get; set; }
        public double Tot_Overtime1ManHours { get; set; }
        public double Tot_Overtime2ManHours { get; set; }
        public double Tot_Overtime3ManHours { get; set; }
        public double Tot_TotalHours { get; set; }
        public double Tot_TotalLabourCost { get; set; }
        public double Tot_TotalCostOfNumberedPart { get; set; }
        public double Tot_TotalCostSuppMat { get; set; }
        public double Tot_ImportTax { get; set; }
        public double Tot_SalesTaxParts { get; set; }
        public double Tot_SalesTaxLabour { get; set; }
        public double Tot_TotalToBePaidToShop { get; set; }
        public double Tot_TotalToBePaidToShopInUSD { get; set; }
        public double Tot_TotalToBePaidToAgentFromCPHInLocalCurrency { get; set; }
        public double Tot_TotalToBePaidToAgentFromCPHInUSD { get; set; }
        public double Tot_TotalCostOfCPHSuppliedPartsInUSD { get; set; }
        public double Tot_TotalCostOfRepairCPHinclCPHsuppliedpartsInUSD { get; set; }
    }
}
