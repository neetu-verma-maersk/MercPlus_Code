using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MercPlusClient.ManageWorkOrderServiceReference;
using MercPlusClient.Areas.ManageWorkOrder.Models;
using System.Text;
using MercPlusClient.UtilityClass;
using MWOM = MercPlusClient.Areas.ManageWorkOrder.Models.ManageWorkOrderModel;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.ServiceModel;

namespace MercPlusClient.Areas.ManageWorkOrder.Controllers
{
    public class ManageWorkOrderController : Controller,IDisposable
    {
        #region VARIABLES
        ManageWorkOrderClient WorkOrderClient = new ManageWorkOrderClient();
        LogEntry logEntry = new LogEntry();
        MWOM WOM = new MWOM();
        WorkOrderDetail localWOD;
        enum UIITEMS { CCREATENEW, CISSINGLE, CWOID, CSHOP, CCUSTOMER, CCAUSE, CTHIRDPARTYPORT, CCURRENCY, CCOMPLETIONDATE, CMODE, CUNITIDENDIGIT, CEQPVENDLIST, CSHOWCOMMENTS, CREMARKS, CSALESTAXPARTSCOST, CSALESTAXLABOURCOST, CIMPORTTAXCOST, CREPVIEW, CSPAREVIEW, CHOURS }
        const string CauseVal = "1";
        #endregion

        #region SINGLE EQUIPMENT
        public ActionResult ManageWorkOrder(bool IsMulti, int orderID = 0)
        {
            try
            {
                WOM.IsSingle = (orderID != null && orderID > 0) ? true : !IsMulti;
                WOM.IsCreateNew = (orderID != null && orderID > 0) ? false : true;
                SetRole();
                WOM = PopulateScreen(orderID);
            }
            catch
            {
                throw;
            }

            return View(WOM);
        }

        private ManageWorkOrderModel PopulateScreen(int orderID)
        {
            if (orderID != 0)
            {
                WOM.dbWOD = new WorkOrderDetail();
                WOM.dbWOD = WorkOrderClient.GetWorkOrderDetails(orderID);
                FillScreenWithDetails(WOM.dbWOD);
            }
            else
                WOM = PopulateInitialDDLs(WOM);

            return WOM;
        }

        private void FillScreenWithDetails(WorkOrderDetail dbWOD)
        {
            WOM.woID = dbWOD.WorkOrderID;
            if (dbWOD.WorkOrderStatus == 0)//for saveasdraft
            {
                WOM.IsSaveAsDraft = true;
            }

            SetDDLSelectedItems(dbWOD);
            SetEquipmentsDetails(dbWOD);
            WOM.ShowComments = dbWOD.ReqdRemarkSW;
            WOM.Remarks = dbWOD.Remarks;
            SetOverTimeDetails(dbWOD);
            GetViewsRowsCount((int)MWOM.REPAIRSPARE.REPAIR);
        }

        private void SetOverTimeDetails(WorkOrderDetail dbWOD)
        {
            WOM.TotalManHourReg = (dbWOD.TotalManHourReg != null ? Math.Round(Convert.ToDouble(dbWOD.TotalManHourReg.Value), 2) : 0.0);
            WOM.TotalManHourOverTime = (dbWOD.TotalManHourReg != null ? Math.Round(Convert.ToDouble(dbWOD.TotalManHourOverTime.Value), 2) : 0.0);
            WOM.TotalManHourDoubleTime = (dbWOD.TotalManHourReg != null ? Math.Round(Convert.ToDouble(dbWOD.TotalManHourDoubleTime.Value), 2) : 0.0);
            WOM.TotalManHourMisc = (dbWOD.TotalManHourReg != null ? Math.Round(Convert.ToDouble(dbWOD.TotalManHourMisc.Value), 2) : 0.0);
        }

        private void SetEquipmentsDetails(WorkOrderDetail dbWOD)
        {
            WOM.EquipmentList = dbWOD.EquipmentList.ToList();
            WOM.UnitIdentifierDigit = (dbWOD.EquipmentList != null && dbWOD.EquipmentList.Count() > 0 ? dbWOD.EquipmentList[0].UnitIdentifierDigit : string.Empty);
            WOM.Mode = dbWOD.Mode;
            WOM.ModeDescp = dbWOD.Mode + "-" + dbWOD.ModeDescription;
            WOM.EquipmentNo = WOM.dbWOD.EquipmentList[0].EquipmentNo;
            UpdateRKEMDetail(dbWOD.Mode, dbWOD.EquipmentList[0]);
            /*
            Equipment Eqp = GetRKEM(dbWOD.EquipmentList != null && dbWOD.EquipmentList.Count() > 0 ? dbWOD.EquipmentList[0].EquipmentNo : null, dbWOD.Shop.ShopCode, dbWOD.EquipmentList[0].VendorRefNo);
            UpdateRKEMDetail(dbWOD.Mode, Eqp);
             */
        }

        public Equipment GetRKEM(string eqpNo, string shpCode, string vendRefNo)
        {
            Equipment Eqp = null;
            if (!string.IsNullOrEmpty(eqpNo) && !string.IsNullOrEmpty(shpCode))
            {
                Eqp = WorkOrderClient.GetEquipmentDetailsFromRKEM(eqpNo, shpCode, vendRefNo);
            }
            return Eqp;
        }

        private void UpdateRKEMDetail(string modeCode, Equipment Eqp)
        {
            if (Eqp != null)
            {
                WOM.Type = Eqp.COType + "/" + Eqp.Eqstype;
                WOM.Size = Eqp.Size;
                if (!string.IsNullOrEmpty(Eqp.Eqstype) && (Eqp.COType.Equals("CONT") && Eqp.Eqstype.Equals("REEF")))
                    WOM.ReeferMakeModel = Eqp.EQRuman + "/" + Eqp.EQRutyp;
                else
                    WOM.ReeferMakeModel = "";
                if (!(Eqp.Deldatsh == null || Eqp.Deldatsh.Equals(DateTime.MinValue)))
                    WOM.Deldatsh = Convert.ToDateTime(Eqp.Deldatsh).ToString("yyyy-MM-dd");
                else
                    WOM.Deldatsh = "";

                WOM.Profile = Eqp.EQProfile;
                WOM.Material = Eqp.Eqmatr;
                WOM.LeasingCompany = Eqp.LeasingCompany;
                WOM.InService = Convert.ToDateTime(Eqp.EQInDate) <= DateTime.MinValue ? "" : Convert.ToDateTime(Eqp.EQInDate).ToString("yyyy-MM-dd");
                WOM.GenSetMakeModel = (!Eqp.COType.Equals("GENS") ? "" : (Eqp.EqMancd + "/" + Eqp.EQRutyp));
                WOM.ExtensionDate = Convert.ToDateTime(Eqp.ExtensionDate) <= DateTime.MinValue ? "" : Convert.ToDateTime(Eqp.ExtensionDate).ToString("yyyy-MM-dd");
                WOM.EqpNotFound = Eqp.EquipmentNo;
                WOM.BoxMfg = (Eqp.COType.Equals("CONT") ? Eqp.EqMancd : "");
                WOM.LeasingContract = Eqp.LeasingContract;
                WOM.Size = Eqp.Size + "/" + Eqp.Eqouthgu;
                WOM.Damage = Eqp.Damage;
                WOM.GradeCode = Eqp.GradeCode; //pinaki
                WOM.IndicatorCode = GetIndicator(Eqp.EQRutyp);
                WOM.DDLMode = GetModes(Eqp.ModeList, modeCode);
            }
        }

        private string GetIndicator(string EQRUTYP)
        {
            if (!string.IsNullOrEmpty(EQRUTYP))
                return WorkOrderClient.RSByMfgAndModel(EQRUTYP);

            return string.Empty;
        }

        private void SetDDLSelectedItems(WorkOrderDetail dbWOD)
        {
            WOM.ShopCode = dbWOD.Shop.ShopCode;
            WOM.ShowTax = (dbWOD.Shop.ImportTax != null && dbWOD.Shop.ImportTax > 0) ? false : true;
            WOM.ImportTax = (dbWOD.ImportTax != null ? Math.Round(Convert.ToDecimal(dbWOD.ImportTax.Value), 2) : 0);
            WOM.DDLCustomer = GetCustomersByShop(dbWOD.Shop);
            WOM.CustomerCode = "MAER";
            /**
             Harcoded MAER
            WOM.CustomerCode = ((dbWOD.Shop.Customer != null) ? dbWOD.Shop.Customer[0].CustomerCode : string.Empty);
            **/
            if (WOM.IsSingle)
            {
                WOM.DDLCause = UtilMethods.GetCauseItems();
                WOM.CauseCode = dbWOD.Cause;
            }
            else
            {
                WOM.Cause = (from item in (UtilMethods.GetCauseItems())
                             where item.Value == dbWOD.Cause
                             select item.Text).FirstOrDefault();
                WOM.CauseCode = dbWOD.Cause;
            }
            WOM.ThirdPartyPort = dbWOD.ThirdPartyPort;
            WOM.Currency = dbWOD.Shop.Currency.CurrName;
            WOM.CurrCode = dbWOD.Shop.Currency.Cucdn;
            if (dbWOD.CompletionDate == null || dbWOD.CompletionDate.Equals(DateTime.MinValue))
                WOM.CompletionDate = (DateTime.Now).ToString("yyyy-MM-dd");
            else
                WOM.CompletionDate = (dbWOD.CompletionDate).ToString("yyyy-MM-dd");
        }

        public ActionResult Blank()
        {
            return View();
        }

        #endregion

        #region COMMON METHODS
        private ManageWorkOrderModel PopulateInitialDDLs(ManageWorkOrderModel WOM)
        {
            #region ShopDropDown
            List<SelectListItem> ShopItems = new List<SelectListItem>();
            WOM.DDLShop = ShopItems;
            #endregion ShopDropDown

            #region CustomerDropDown
            WOM.DDLCustomer = new List<SelectListItem>();
            #endregion CustomerDropDown
            WOM.Currency = "Loading...";
            WOM.CurrCode = string.Empty;
            if (WOM.IsSingle)
                WOM.DDLCause = UtilMethods.GetCauseItems();
            else
            {
                WOM.Cause = (from item in (UtilMethods.GetCauseItems())
                             where item.Value == CauseVal
                             select item.Text).FirstOrDefault();
            }
            WOM.CauseCode = CauseVal;

            List<SelectListItem> ModeItems = new List<SelectListItem>();
            ModeItems.Add(new SelectListItem
            {
                Text = "Equipment Info Required",
                Value = "-1"
            });
            WOM.DDLMode = ModeItems;
            WOM.Mode = "";
            WOM.ShowComments = false;
            return WOM;
        }

        private List<SelectListItem> GetCustomersByShop(Shop shop)
        {
            List<SelectListItem> CustomerItems = new List<SelectListItem>();
            /*
             Harcoded MAER
            foreach (Customer cus in shop.Customer)
            {
                CustomerItems.Add(new SelectListItem
                {
                    Text = cus.CustomerCode,
                    Value = cus.CustomerCode
                });
            }
             */

            CustomerItems.Add(new SelectListItem
                {
                    Text = "MAER",
                    Value = "MAER"
                });
            return CustomerItems;
        }

        public ActionResult GetCustomersOnShopChange(string shopCode)
        {
            Shop shopD = WorkOrderClient.GetShopDetailsOnShopCode(shopCode);
            string outStatus; WOM.ShowTax = false;
            if (shopD == null)
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
            else
            {
                outStatus = Validation.MESSAGETYPE.SUCCESS.ToString();
                WOM.ShowTax = (shopD.ImportTax != null && shopD.ImportTax > 0) ? false : true;
            }
            return Json(new { Status = outStatus, UIMsg = ResourceMerc.CUSTOMER_UNAVAILABLE, ItemList = shopD.Customer, CurrCode = shopD.Currency, ShowTax = WOM.ShowTax }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShopDetails()
        {
            Currency cur = new Currency();
            List<Shop> ShopList = WorkOrderClient.GetShopCode(Convert.ToInt32(((UserSec)Session["UserSec"]).UserId)).ToList();
            string _msg = string.Empty;
            string outStatus; WOM.ShowTax = false;
            if (ShopList == null || ShopList.Count == 0)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                _msg = ResourceMerc.SHOP_UNAVAILABLE;
            }
            else
            {
                outStatus = Validation.MESSAGETYPE.SUCCESS.ToString();
                WOM.ShowTax = (ShopList[0].ImportTax != null && ShopList[0].ImportTax > 0) ? false : true;
                cur = ShopList[0].Currency;
            }
            var jsonResult = Json(new { Status = outStatus, UIMsg = _msg, ItemList = ShopList, CurrCode = cur, ShowTax = WOM.ShowTax }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        private List<SelectListItem> GetModes(List<Mode> modeList, string modeCode)
        {
            List<SelectListItem> ModeItems = new List<SelectListItem>();
            if (modeList != null && modeList.Count > 0)
            {
                foreach (var modes in modeList)
                {
                    ModeItems.Add(new SelectListItem
                    {
                        Text = modes.ModeCode + "-" + modes.ModeDescription,
                        Value = modes.ModeCode,
                    });
                }
            }
            else
            {
                ModeItems.Add(new SelectListItem
                {
                    Text = "No Mode(s) Returned for Equipment",
                    Value = "-1",
                });
            }
            return ModeItems;
        }

        #endregion

        public ActionResult RepairDialogues(int dlgType, string modeCode)
        {
            if (dlgType == (int)MWOM.DLGTYPE.DAMAGE)
            {
                WOM.LstDamage = WorkOrderClient.GetDamageCodeAll(null).ToList();
            }
            else if (dlgType == (int)MWOM.DLGTYPE.REPAIR)
            {
                WOM.LstRepairCode = WorkOrderClient.GetRepairCode(modeCode).ToList();
            }
            else if (dlgType == (int)MWOM.DLGTYPE.REPAIRLOCCODE)
            {
                WOM.LstRepLocCode = WorkOrderClient.GetRepairLocCode(null).ToList();
            }
            else
            {
                WOM.LstTPI = WorkOrderClient.GetTpiCode(null).ToList();
            }
            WOM.RepairDlgType = dlgType;
            return PartialView("RepairDlgView", WOM);
        }

        [HttpPost]
        public ActionResult AddUpdateRepairViewRows(int viewType)
        {
            if (WOM != null)
            {
                GetViewsRowsCount(viewType);
            }
            return (viewType == (int)MWOM.REPAIRSPARE.REPAIR ? PartialView("RepairViewAddRows", WOM) : PartialView("SpareViewAddRows", WOM));
        }

        private void GetViewsRowsCount(int viewType)
        {
            int maxRowsPerClick = Convert.ToInt32(ReadAppSettings.ReadSetting("AdditionalRows"));
            int maxRows = Convert.ToInt32(ReadAppSettings.ReadSetting("MaxRepairRows"));

            if (viewType == (int)MWOM.REPAIRSPARE.REPAIR)
            {
                if (WOM.dbWOD == null || WOM.dbWOD.RepairsViewList == null || WOM.dbWOD.RepairsViewList.Count() == 0)
                    WOM.RAdditionalRows = maxRowsPerClick;
                else
                {
                    if (WOM.dbWOD.RepairsViewList.Count() < maxRows)
                    {
                        WOM.RAdditionalRows = ((maxRows - WOM.dbWOD.RepairsViewList.Count()) > maxRowsPerClick ? ((maxRows - WOM.dbWOD.RepairsViewList.Count()) - maxRowsPerClick) : (maxRows - WOM.dbWOD.RepairsViewList.Count()));
                    }
                }
            }
            else
            {
                if (WOM.dbWOD == null || WOM.dbWOD.SparePartsViewList == null || WOM.dbWOD.SparePartsViewList.Count() == 0)
                    WOM.SAdditionalRows = maxRowsPerClick;
                else
                {
                    if (WOM.dbWOD.SparePartsViewList.Count() < maxRows)
                    {
                        WOM.SAdditionalRows = ((maxRows - WOM.dbWOD.SparePartsViewList.Count()) > maxRowsPerClick ? ((maxRows - WOM.dbWOD.SparePartsViewList.Count()) - maxRowsPerClick) : (maxRows - WOM.dbWOD.SparePartsViewList.Count()));
                    }
                }
            }
        }
        public JsonResult GetHours([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            List<RepairsView> rvList = new List<RepairsView>();
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                List<ErrMessage> ErrList = new List<ErrMessage>();
                localWOD = ConstructWOD(UIContent, true);
                if (localWOD != null)
                {
                    rvList = WorkOrderClient.GetHours(out ErrList, localWOD).ToList();
                    if (rvList != null)
                    {
                        int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                        outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                        outMsg = UtilMethods.CreateMessageString(ErrList);
                    }
                }
                else
                {
                    outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                    outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "GetHours");
                    outMsg += "</br>" + ResourceMerc.INVALID_INPUT;
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "GetHours");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "GetHours");
            }
            return Json(new { Status = outStatus, UIMsg = outMsg, RepVList = rvList }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TabbedCallRKEM(string EqpNo, string shpCode, string vendRefNo)
        {
            Equipment eqp = GetRKEM(EqpNo, shpCode, vendRefNo);
            if (!string.IsNullOrEmpty(eqp.EQRutyp) && (eqp.COType.Equals("GENS") || (eqp.COType.Equals("CONT") && eqp.Eqstype.Equals("REEF"))))
                return Json(new { data = eqp, IndCode = GetIndicator(eqp.EQRutyp) });
            else
                return Json(new { data = eqp, IndCode = "" });
        }

        [ValidateInput(false)]
        public JsonResult SaveAsDraft([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            List<ErrMessage> ErrList;
            string outStatus = string.Empty, outMsg = string.Empty, _woSummary = string.Empty;
            Session["ReviewData"] = null; TempData["INFO"] = null; TempData["SHOP"] = null;
            try
            {
                localWOD = ConstructWOD(UIContent);
                if (localWOD != null)
                {
                    ErrList = WorkOrderClient.SaveAsDraft(ref localWOD, localWOD.EquipmentList).ToList();
                    if (ErrList != null)
                    {
                        int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                        outMsg = UtilMethods.CreateMessageString(ErrList);
                        if (index != -1)
                        {
                            outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                        }
                        else
                        {
                            TempData["INFO"] = outMsg;
                            TempData["SHOP"] = localWOD.Shop.ShopCode;
                            //Session["ReviewData"] = localWOD;
                            //_woSummary = LoadWOSummary(localWOD);
                            outStatus = Validation.MESSAGETYPE.SUCCESS.ToString();
                        }
                    }
                }
                else
                {
                    outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                    outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "SaveAsDraft");
                    outMsg += "</br>" + ResourceMerc.INVALID_INPUT;
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "SaveAsDraft");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "SaveAsDraft");
            }
            return Json(new
            {
                Status = outStatus,
                UIMsg = outMsg,
                _woSummary = _woSummary,
                redirectUrl = Url.Action("ManageWorkOrder", "ManageWorkOrder", new { IsMulti = false }),
                isRedirect = true
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteWO(string WOID)
        {
            List<ErrMessage> ErrList;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = WorkOrderClient.ChangeStatus(Int32.Parse(WOID), 9999, GetUserDtl()).ToList();
                if (ErrList != null)
                {
                    int index = ErrList.FindIndex(item =>item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                    outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                    outMsg = UtilMethods.CreateMessageString(ErrList);
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Delete");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Delete");
            }
            return Json(new { Status = outStatus, UIMsg = outMsg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ReviewWO([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            List<ErrMessage> ErrList;
            string outStatus = string.Empty, outMsg = string.Empty, _woSummary = string.Empty; int index = -1;
            Session["ReviewData"] = null;
            try
            {
                localWOD = ConstructWOD(UIContent);
                if (localWOD != null)
                {
                    ErrList = WorkOrderClient.Review(ref localWOD, localWOD.EquipmentList, true).ToList();

                    if (ErrList != null)
                    {
                        index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                        outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                        outMsg = UtilMethods.CreateMessageString(ErrList);
                        // output = new { status = (index > 0 ? "Warning" : "Success"), UIMsg = UtilMethods.CreateMessageString(ErrList) };
                    }

                    if (index < 0)
                    {
                        Session["ReviewData"] = localWOD;
                        _woSummary = LoadWOSummary(localWOD);
                    }
                }
                else
                {
                    outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                    outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Review");
                    outMsg += "</br>" + ResourceMerc.INVALID_INPUT;
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Review");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Review");
            }
            return Json(new { Status = outStatus, UIMsg = outMsg, _woSummary = _woSummary }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public JsonResult SubmitWO([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            List<ErrMessage> ErrList;
            string outStatus = string.Empty, outMsg = string.Empty, _woSummary = string.Empty;
            Session["ReviewData"] = null; TempData["INFO"] = null; TempData["SHOP"] =null;
            try
            {
                localWOD = ConstructWOD(UIContent);
                if (localWOD != null)
                {
                    ErrList = WorkOrderClient.SubmitWorkOrder(ref localWOD, localWOD.EquipmentList).ToList();
                    if (ErrList != null)
                    {
                        int index = ErrList.FindIndex(item =>  item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                        outMsg = UtilMethods.CreateMessageString(ErrList);
                        if (index != -1)
                        {
                            outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                        }
                        else
                        {
                            TempData["INFO"] = outMsg;
                            TempData["SHOP"] = localWOD.Shop.ShopCode;
                            //Session["ReviewData"] = localWOD;
                            //_woSummary = LoadWOSummary(localWOD);
                            outStatus = Validation.MESSAGETYPE.SUCCESS.ToString();
                        }
                    }
                }
                else
                {
                    outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                    outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Submit");
                    outMsg += "</br>" + ResourceMerc.INVALID_INPUT;
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Submit");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Submit");
            }
            return Json(new
            {
                Status = outStatus,
                UIMsg = outMsg,
                _woSummary = _woSummary,
                redirectUrl = Url.Action("ManageWorkOrder", "ManageWorkOrder", new { IsMulti = false }),
                isRedirect = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClearWO(string isSingle)
        {
            return RedirectToAction("ManageWorkOrder", new { IsMulti = !UtilMethods.ToBool(isSingle) });
        }

        private WorkOrderDetail ConstructWOD(Dictionary<string, object> UIContent, bool gethours = false)
        {
            localWOD = new WorkOrderDetail();
            bool corrupted = false;
            string dicVal = "";

            try
            {
                localWOD.ChangeUser = GetUserDtl();
                if (UIContent.ContainsKey(UIITEMS.CCREATENEW.ToString()))
                {
                    WOM.IsCreateNew = UtilMethods.ToBool(((string[])(UIContent[UIITEMS.CCREATENEW.ToString()]))[0]);
                    localWOD.RepairDate = DateTime.Now;
                }
                else
                    if (!gethours)
                        return null;

                foreach (UIITEMS item in Enum.GetValues(typeof(UIITEMS)))
                {
                    if (!UIContent.ContainsKey(item.ToString()))
                        continue;

                    dicVal = ((string[])(UIContent[item.ToString()]))[0];
                    switch (item)
                    {
                        case UIITEMS.CCREATENEW:
                            WOM.IsCreateNew = UtilMethods.ToBool(dicVal);
                            break;
                        case UIITEMS.CISSINGLE:
                            WOM.IsSingle = UtilMethods.ToBool(dicVal);
                            localWOD.IsSingle = WOM.IsSingle;
                            break;
                        case UIITEMS.CWOID:
                            if (!WOM.IsCreateNew)
                                localWOD.WorkOrderID = WOM.woID = Int32.Parse(dicVal);
                            break;
                        case UIITEMS.CSHOP:
                            localWOD.Shop = new Shop();
                            localWOD.Shop.ShopCode = dicVal;
                            break;
                        case UIITEMS.CCUSTOMER:
                            Customer cus = new Customer();
                            cus.CustomerCode = dicVal;
                            localWOD.Shop.Customer = new List<Customer>();
                            localWOD.Shop.Customer.Add(cus);
                            break;
                        case UIITEMS.CCAUSE:
                            localWOD.Cause = dicVal; break;
                        case UIITEMS.CTHIRDPARTYPORT:
                            localWOD.ThirdPartyPort = dicVal;
                            break;
                        case UIITEMS.CCURRENCY:
                            localWOD.Shop.Currency = new Currency();
                            localWOD.Shop.Currency.Cucdn = dicVal;
                            break;
                        case UIITEMS.CCOMPLETIONDATE:
                            if (!string.IsNullOrEmpty(dicVal))
                            {
                                if (Convert.ToDateTime(dicVal).Equals(DateTime.MinValue))
                                    localWOD.CompletionDate = DateTime.Now;
                                else
                                    localWOD.CompletionDate = DateTime.Parse(dicVal);
                            }
                            else
                            {
                                localWOD.CompletionDate = DateTime.Now;
                            }
                            localWOD.RepairDate = localWOD.CompletionDate;
                            break;
                        case UIITEMS.CMODE:
                            localWOD.Mode = dicVal;
                            break;
                        case UIITEMS.CUNITIDENDIGIT:
                            //if (!string.IsNullOrEmpty(UIContent[item]))
                            //localWOD.EquipmentList[0].UnitIdentifierDigit = UIContent[item]; 
                            break;
                        case UIITEMS.CEQPVENDLIST:
                            if (!string.IsNullOrEmpty(dicVal))
                            {
                                localWOD.EquipmentList = new List<Equipment>();
                                Equipment eqp;
                                string[] cEVList = ((string[])(UIContent[item.ToString()]));
                                foreach (string val in cEVList)
                                {
                                    eqp = new Equipment();
                                    string[] cVal = val.Split(UtilMethods.SPLITBYPIPE, StringSplitOptions.None);
                                    eqp.EquipmentNo = cVal[0];
                                    eqp.VendorRefNo = cVal[1];
                                    eqp.SelectedMode = localWOD.Mode;
                                    localWOD.EquipmentList.Add(eqp);
                                }
                            }
                            break;
                        case UIITEMS.CSHOWCOMMENTS:
                            localWOD.ReqdRemarkSW = bool.Parse(dicVal); break;
                        case UIITEMS.CREMARKS:
                            if (!string.IsNullOrEmpty(dicVal))
                            {
                                localWOD.RemarksList = new List<RemarkEntry>();
                                RemarkEntry Remarks = new RemarkEntry();
                                Remarks.Remark = dicVal;
                                Remarks.RemarkType = "V";
                                localWOD.RemarksList.Add(Remarks);
                            }
                            break;
                        case UIITEMS.CSALESTAXPARTSCOST:
                            localWOD.SalesTaxParts = UtilMethods.ToNullableDecimal(dicVal); break;
                        case UIITEMS.CSALESTAXLABOURCOST:
                            localWOD.SalesTaxLabour = UtilMethods.ToNullableDecimal(dicVal); break;
                        case UIITEMS.CIMPORTTAXCOST:
                            localWOD.ImportTax = UtilMethods.ToNullableDecimal(dicVal); break;
                        case UIITEMS.CREPVIEW:
                            if (!string.IsNullOrEmpty(dicVal))
                            {
                                localWOD.RepairsViewList = new List<RepairsView>();
                                RepairsView repV;
                                string[] cRVList = ((string[])(UIContent[item.ToString()]));
                                foreach (string val in cRVList)
                                {
                                    repV = new RepairsView();
                                    string[] cVal = val.Split(UtilMethods.SPLITBYPIPE, StringSplitOptions.None);
                                    if (!gethours)
                                    {
                                        repV.Damage = new Damage();
                                        repV.Damage.DamageCedexCode = cVal[0];
                                        repV.RepairCode = new RepairCode();
                                        repV.RepairCode.RepairCod = cVal[1];
                                        repV.RepairLocationCode = new RepairLoc();
                                        repV.RepairLocationCode.CedexCode = cVal[2];
                                        repV.Pieces = int.Parse(cVal[3]);
                                        repV.MaterialCost = UtilMethods.ToNullableDecimal(cVal[4]);
                                        repV.ManHoursPerPiece = float.Parse(cVal[5]);
                                        repV.Tpi = new Tpi();
                                        repV.Tpi.CedexCode = cVal[6];
                                    }
                                    else
                                    {
                                        repV.RepairCode = new RepairCode();
                                        repV.RepairCode.RepairCod = cVal[1];
                                    }
                                    localWOD.RepairsViewList.Add(repV);
                                    if (localWOD.RepairsViewList != null && localWOD.RepairsViewList.Count > 0)
                                        localWOD.RepairsViewList.OrderBy(rCd => rCd.RepairCode.RepairCod);
                                }
                            }
                            break;
                        case UIITEMS.CSPAREVIEW:
                            if (WOM.IsSingle && !string.IsNullOrEmpty(dicVal))
                            {
                                localWOD.SparePartsViewList = new List<SparePartsView>();
                                SparePartsView spareV;
                                string[] cSVList = ((string[])(UIContent[item.ToString()]));
                                foreach (string val in cSVList)
                                {
                                    spareV = new SparePartsView();
                                    string[] cVal = val.Split(UtilMethods.SPLITBYPIPE, StringSplitOptions.None);
                                    spareV.RepairCode = new RepairCode();
                                    spareV.RepairCode.RepairCod = cVal[0];
                                    if (string.IsNullOrEmpty(cVal[1]))
                                    {
                                        cVal[1] = "0.0";
                                    }
                                    spareV.Pieces = double.Parse(cVal[1]);
                                    spareV.OwnerSuppliedPartsNumber = cVal[2];
                                    spareV.SerialNumber = cVal[3];
                                    localWOD.SparePartsViewList.Add(spareV);
                                }
                                if (localWOD.SparePartsViewList != null && localWOD.SparePartsViewList.Count > 0)
                                    localWOD.SparePartsViewList.OrderBy(rCd => rCd.RepairCode.RepairCod);
                            }
                            break;
                        case UIITEMS.CHOURS:
                            string[] cHours = (dicVal).Split(UtilMethods.SPLITBYPIPE, StringSplitOptions.None);
                            double? vals = null;
                            vals = UtilMethods.ToNullableDouble(cHours[0]);
                            localWOD.TotalManHourReg = vals == null ? (double?)0.0 : vals;
                            vals = UtilMethods.ToNullableDouble(cHours[1]);
                            localWOD.TotalManHourOverTime = vals == null ? (double?)0.0 : vals;
                            vals = UtilMethods.ToNullableDouble(cHours[2]);
                            localWOD.TotalManHourDoubleTime = vals == null ? (double?)0.0 : vals;
                            vals = UtilMethods.ToNullableDouble(cHours[3]);
                            localWOD.TotalManHourMisc = vals == null ? (double?)0.0 : vals;
                            break;
                        default:
                            //someone malafied the script file
                            corrupted = true;
                            break;
                    }
                    if (corrupted)
                    {
                        localWOD = null;
                        return localWOD;
                    }
                }
            }
            catch (Exception ex)
            { 
            
            }
            return localWOD;
        }

        public JsonResult CreateErrorContent(string[][] UIContent)
        {
            List<ErrMessage> eL = new List<ErrMessage>();
            return Json(UtilMethods.CreateMessageString(eL));
        }

        private void SetRole()
        {
            if (WOM == null)
                WOM = new MWOM();
            WOM.ADMIN = ((UserSec)Session["UserSec"]).isAdmin;
            WOM.CPH = ((UserSec)Session["UserSec"]).isCPH;
            WOM.EMR_SPECIALIST_COUNTRY = ((UserSec)Session["UserSec"]).isEMRSpecialistCountry;
            WOM.EMR_SPECIALIST_SHOP = ((UserSec)Session["UserSec"]).isEMRSpecialistShop;
            WOM.EMR_APPROVER_COUNTRY = ((UserSec)Session["UserSec"]).isEMRApproverCountry;
            WOM.EMR_APPROVER_SHOP = ((UserSec)Session["UserSec"]).isEMRApproverShop;
            WOM.SHOP = ((UserSec)Session["UserSec"]).isShop;
            WOM.MPRO_CLUSTER = ((UserSec)Session["UserSec"]).isMPROCluster;
            WOM.MPRO_SHOP = ((UserSec)Session["UserSec"]).isMPROShop;
            WOM.READONLY = ((UserSec)Session["UserSec"]).isReadOnly;
            
            WOM.ISANYCPH = ((UserSec)Session["UserSec"]).isAnyCPH;
            WOM.ISANYSHOP = ((UserSec)Session["UserSec"]).isAnyShop;

            WOM.CENEQULOS = true;            
        }

        public string LoadWOSummary(WorkOrderDetail _wo)
        {
            WOM = new MWOM();
            WOM.dbWOD = new WorkOrderDetail();
            WOM.dbWOD = _wo;
            WOM.woID = WOM.dbWOD.WorkOrderID;
            SetRole();
            return GetSummaryData(WOM);
        }

        private string GetSummaryData(MWOM WOM)
        {
            /*while returning WOsummary*/
            if (WOM.ISANYCPH)
                WOM.CurrencyName = "U.S. DOLLAR";
            else
                WOM.CurrencyName = WOM.dbWOD.Shop.Currency.CurrName;
            return PartialView("WOSummary", WOM).RenderToString();
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

        private string GetUserDtl()
        {
            string userD = string.Empty;
            if (!string.IsNullOrEmpty(((UserSec)Session["UserSec"]).UserFirstName))
                userD = ((UserSec)Session["UserSec"]).UserFirstName;
            if (!string.IsNullOrEmpty(((UserSec)Session["UserSec"]).UserLastName))
                userD = userD + " " + ((UserSec)Session["UserSec"]).UserLastName;
            if (!string.IsNullOrEmpty(((UserSec)Session["UserSec"]).LoginId))
                userD = userD + "[" + ((UserSec)Session["UserSec"]).LoginId + "]";

            return userD;
        }

        #region IDisposable implementation

        /// <summary>
        /// IDisposable.Dispose implementation, calls Dispose(true).
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose worker method. Handles graceful shutdown of the
        /// client even if it is an faulted state.
        /// </summary>
        /// <param name="disposing">Are we disposing (alternative
        /// is to be finalizing)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (WorkOrderClient.State != CommunicationState.Faulted)
                    {
                        WorkOrderClient.Close();
                    }
                }
                finally
                {
                    if (WorkOrderClient.State != CommunicationState.Closed)
                    {
                        WorkOrderClient.Abort();
                    }
                }
            }
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~ManageWorkOrderController()
        {
            Dispose(false);
        }

        #endregion
    }    
}