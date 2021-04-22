using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MercPlusClient.ManageWorkOrderServiceReference;
using MercPlusClient.Areas.ManageWorkOrder.Models;
using System.Text;
using MercPlusClient.UtilityClass;
using MAM = MercPlusClient.Areas.ManageWorkOrder.Models.ManagerApprovalModel;
using System.IO;
using System.ServiceModel;

namespace MercPlusClient.Areas.ManageWorkOrder.Controllers
{
    public class ManagerApprovalController : Controller, IDisposable
    {
        #region VARIABLES
        ManageWorkOrderClient WorkOrderClient = new ManageWorkOrderClient();
        MAM MAML = new MAM();
        WorkOrderDetail localWOD;
        #endregion

        public ActionResult ManagerApproval(int WO_ID)
        {
            try
            {
                if (WO_ID != 0)
                {
                    CreateWOSum(WO_ID);
                }
            }
            catch
            {
                throw;
            }
            return View("ManagerApproval", MAML);
        }

        private void CreateWOSum(int WO_ID)
        {
            MAML.IsSingle = true;
            MAML.IsCreateNew = false;//iscreatenew method call            
            SetRoleBasics();

            MAML.dbWOD = new WorkOrderDetail();
            MAML.dbWOD = WorkOrderClient.GetWorkOrderDetails(WO_ID);
            MAML.WOStatus = MAML.dbWOD.WorkOrderStatus.ToString();//convert this status to logical value
            if (MAML.IsSingle)
            {
                MAML.DDLCause = UtilMethods.GetCauseItems();
                MAML.CauseCode = MAML.dbWOD.Cause;
            }
            else
            {
                MAML.Cause = (from item in (UtilMethods.GetCauseItems())
                              where item.Value == MAML.dbWOD.Cause
                              select item.Text).FirstOrDefault();
                MAML.CauseCode = MAML.dbWOD.Cause;
            }
            MAML.ThirdPartyPort = MAML.dbWOD.ThirdPartyPort;
            AllowCreateRemarksField(MAML.dbWOD);
            ValidateForApprovalBtn(MAML.dbWOD);
            MAML.DISPLAY_STATUS_CODE = UtilMethods.ToBool(ReadAppSettings.ReadSetting("DISPLAY_STATUS_CODE"));
            MAML.CONTACT_EQSAL_SW = (MAML.dbWOD.Shop.Location.ContactEqsalSW == null ? "N" : MAML.dbWOD.Shop.Location.ContactEqsalSW);

            MAML.RepCodeCount = MAML.dbWOD.RepairsViewList.Count;
            if (MAML.EMR_SPECIALIST_COUNTRY || MAML.EMR_SPECIALIST_SHOP)
                MAML.CurrencyName = MAML.dbWOD.Shop.Currency.CurrName;
            else if (MAML.ISANYCPH)
                MAML.CurrencyName = "U.S. DOLLAR";
            else
                MAML.CurrencyName = MAML.dbWOD.Shop.Currency.CurrName;


            Dictionary<string, object> PreviousWorkOrderData = new Dictionary<string, object>();
            string cStatus = "";
            if (MAML.dbWOD.WorkOrderStatus >= 390)//For Approved WO's
            {
                if (MAML.dbWOD.PrevStatus != null && !string.IsNullOrEmpty(MAML.dbWOD.PrevLocCode))
                {
                    if (MAML.ISANYSHOP && ((MercPlusClient.UserSec)System.Web.HttpContext.Current.Session["UserSec"]).WorkOrderStatusDesc.ContainsKey("s" + MAML.dbWOD.PrevStatus.ToString()))
                    {
                        cStatus = "s" + MAML.dbWOD.PrevStatus.ToString();
                        var statusCode = ((MercPlusClient.UserSec)System.Web.HttpContext.Current.Session["UserSec"]).WorkOrderStatusDesc.Single(code => { return code.Key == cStatus; });
                        MAML.PrevStatus = statusCode.Value;
                    }
                    else
                    {
                        cStatus = MAML.dbWOD.PrevStatus.ToString();
                        var statusCode = ((MercPlusClient.UserSec)System.Web.HttpContext.Current.Session["UserSec"]).WorkOrderStatusDesc.Single(code => { return code.Key == cStatus; });
                        MAML.PrevStatus = statusCode.Value;
                    }

                    MAML.PrevDate = ((MAML.dbWOD.PrevDate == null || MAML.dbWOD.PrevDate == DateTime.MinValue) ? string.Empty : Convert.ToDateTime(MAML.dbWOD.PrevDate).ToString("yyyy-MM-dd"));
                    MAML.PrevLocation = MAML.dbWOD.PrevLocCode;
                }
                else
                {
                    MAML.PrevStatus = string.Empty;
                    MAML.PrevDate = string.Empty;
                    MAML.PrevLocation = string.Empty;
                }
            }
            else if (MAML.dbWOD.PrevWorkOrderID != null && MAML.dbWOD.PrevWorkOrderID > 1)
            {
                if (MAML.dbWOD.PrevWorkOrderID != -1)
                {
                    PreviousWorkOrderData = WorkOrderClient.GetPrevStatusDateLoc(Convert.ToInt32(MAML.dbWOD.PrevWorkOrderID), "PREV_WO_ID");
                    //if (PreviousWorkOrderData == null || PreviousWorkOrderData.Count == 0 || PreviousWorkOrderData["PREVDATE"] == null)
                    if (PreviousWorkOrderData == null || PreviousWorkOrderData.Count == 0 || PreviousWorkOrderData["PREVDATE"] == null || PreviousWorkOrderData["PREVLOCCODE"] == null || PreviousWorkOrderData["STATUSDESC"] == null || PreviousWorkOrderData["PREVSTATUS"] == null) //Debadrita_Prev_data_bug_fix_09_01/2020
                    {
                        MAML.PrevDate = string.Empty;
                        MAML.PrevLocation = string.Empty;
                        MAML.PrevStatus = string.Empty;
                        MAML.StatusDesc = string.Empty;
                    }
                    else
                    {
                        MAML.PrevDate = (PreviousWorkOrderData["PREVDATE"] == null || Convert.ToDateTime(PreviousWorkOrderData["PREVDATE"]) == DateTime.MinValue) ? string.Empty : (Convert.ToDateTime(PreviousWorkOrderData["PREVDATE"])).ToString("yyyy-MM-dd");
                        MAML.PrevLocation = PreviousWorkOrderData["PREVLOCCODE"].ToString();
                        //MAML.PrevStatus = PreviousWorkOrderData["PREVSTATUS"].ToString();
                        MAML.PrevStatus = PreviousWorkOrderData["STATUSDESC"].ToString();
                    }
                }
                else
                {
                    MAML.PrevStatus = string.Empty;
                    MAML.PrevDate = string.Empty;
                    MAML.PrevLocation = string.Empty;
                }
            }
            else
            {
                PreviousWorkOrderData = WorkOrderClient.GetPrevStatusDateLoc(Convert.ToInt32(MAML.dbWOD.WorkOrderID), "WO_ID");
                //if (PreviousWorkOrderData == null || PreviousWorkOrderData.Count == 0 || PreviousWorkOrderData["PREVDATE"] == null)
                if (PreviousWorkOrderData == null || PreviousWorkOrderData.Count == 0 || PreviousWorkOrderData["PREVDATE"] == null || PreviousWorkOrderData["PREVLOCCODE"] == null || PreviousWorkOrderData["STATUSDESC"] == null || PreviousWorkOrderData["PREVSTATUS"] == null) // //Debadrita_Prev_data_bug_fix_09_01/2020
                {
                    MAML.PrevDate = string.Empty;
                    MAML.PrevLocation = string.Empty;
                    MAML.PrevStatus = string.Empty;
                    MAML.StatusDesc = string.Empty;
                }
                else
                {
                    MAML.PrevDate = (PreviousWorkOrderData["PREVDATE"] == null || Convert.ToDateTime(PreviousWorkOrderData["PREVDATE"]) == DateTime.MinValue) ? string.Empty : (Convert.ToDateTime(PreviousWorkOrderData["PREVDATE"])).ToString("yyyy-MM-dd");
                    MAML.PrevLocation = PreviousWorkOrderData["PREVLOCCODE"].ToString();
                    //MAML.PrevStatus = PreviousWorkOrderData["PREVSTATUS"].ToString();
                    MAML.PrevStatus = PreviousWorkOrderData["STATUSDESC"].ToString();
                }
            }



            //if (MAML.dbWOD.PrevWorkOrderID != null && MAML.dbWOD.PrevWorkOrderID != -1)
            //{
            //    PreviousWorkOrderData = WorkOrderClient.GetPrevStatusDateLoc(WO_ID, "PREV_WO_ID");
            //    if (PreviousWorkOrderData == null || PreviousWorkOrderData.Count == 0 || PreviousWorkOrderData["PREVDATE"] == null)
            //    {
            //        MAML.PrevDate = null;
            //        MAML.PrevLocation = "";
            //        MAML.PrevStatus = null;
            //        MAML.StatusDesc = "";
            //    }
            //    else
            //    {
            //        if (Convert.ToDateTime(PreviousWorkOrderData["PREVDATE"]) == DateTime.MinValue || (Convert.ToDateTime(PreviousWorkOrderData["PREVDATE"])).ToString("yyyy-MM-dd") == "9999-12-31")
            //        {
            //            MAML.PrevDate = null;
            //        }
            //        else
            //            MAML.PrevDate = (Convert.ToDateTime(PreviousWorkOrderData["PREVDATE"])).ToString("yyyy-MM-dd");
            //        MAML.PrevLocation = PreviousWorkOrderData["PREVLOCCODE"].ToString();
            //        MAML.PrevStatus = PreviousWorkOrderData["PREVSTATUS"].ToString();
            //        MAML.StatusDesc = PreviousWorkOrderData["STATUSDESC"].ToString();
            //    }
            //}
            //else if (MAML.dbWOD.PrevWorkOrderID == -1)
            //{
            //    MAML.PrevDate = null;
            //    MAML.PrevLocation = "";
            //    MAML.PrevStatus = null;
            //    MAML.StatusDesc = "";
            //}
            //else
            //{
            //    PreviousWorkOrderData = WorkOrderClient.GetPrevStatusDateLoc(WO_ID, "WO_ID");
            //    if (PreviousWorkOrderData == null || PreviousWorkOrderData.Count == 0 || PreviousWorkOrderData["PREVDATE"] == null)
            //        MAML.PrevDate = null;
            //    else
            //    {
            //        if (Convert.ToDateTime(PreviousWorkOrderData["PREVDATE"]) == DateTime.MinValue || (Convert.ToDateTime(PreviousWorkOrderData["PREVDATE"])).ToString("yyyy-MM-dd") == "9999-12-31")
            //        {
            //            MAML.PrevDate = null;
            //        }
            //        else
            //            MAML.PrevDate = (Convert.ToDateTime(PreviousWorkOrderData["PREVDATE"])).ToString("yyyy-MM-dd");
            //        MAML.PrevLocation = PreviousWorkOrderData["PREVLOCCODE"].ToString();
            //        MAML.PrevStatus = PreviousWorkOrderData["PREVSTATUS"].ToString();
            //        MAML.StatusDesc = PreviousWorkOrderData["STATUSDESC"].ToString();
            //    }
            //}

            MAML.RemarksDetails = GetRemarksData(MAML, WO_ID);
            MAML.LoadButtons = PartialView("WOSummaryButtons", MAML).RenderToString();
            if (MAML.dbWOD.EquipmentList != null && MAML.dbWOD.EquipmentList.Count > 0)
            {
                MAML.EquipmentNo = MAML.dbWOD.EquipmentList[0].EquipmentNo;
                UpdateRKEMDetail(MAML.dbWOD.Mode, MAML.dbWOD.EquipmentList[0]);
            }
            else
                MAML.EquipmentNo = "";
            //Merc-RKEM Damage Code
            MAML.DDLNEWTPI = UtilMethods.GetTPIItems();//Debadrita_TPI_Indicator-19-07-19
            List<string> WODamage = new List<string>();
            WODamage = WorkOrderClient.GetMercDamageCode(Convert.ToInt32(MAML.dbWOD.WorkOrderID));
            if (WODamage.Count() > 0)
            {
                if (WODamage[0] == "" || WODamage[0] == " " || WODamage[0] == "NA")
                {
                    MAML.MercDamage = "N";
                }
                else
                {
                    MAML.MercDamage = "Y";
                }
                MAML.CurrLoc = WODamage[1];
                MAML.CurrMove = WODamage[2];
            }
            //Merc-RKEM Damage Code
        }

        private void ValidateForApprovalBtn(WorkOrderDetail workOrderDetail)
        {
            try
            {
                MAML.CreateApprovalBtn = false;

                if (MAML.EMR_APPROVER_COUNTRY || MAML.EMR_APPROVER_SHOP || MAML.EMR_SPECIALIST_COUNTRY || MAML.EMR_SPECIALIST_SHOP || MAML.ISANYCPH)
                {
                    if (WorkOrderClient.AuthenticateShopCodeByUserID(MAML.dbWOD.Shop.ShopCode, Convert.ToInt32(((UserSec)Session["UserSec"]).UserId)))
                    {
                        VerifyAmount(workOrderDetail);
                    }
                }
                else
                    VerifyAmount(workOrderDetail);
            }
            catch
            { throw; }
        }

        private void VerifyAmount(WorkOrderDetail workOrderDetail)
        {
            if (((UserSec)Session["UserSec"]).ApprovalAmount != null && ((UserSec)Session["UserSec"]).ApprovalAmount > 0 && (((UserSec)Session["UserSec"]).ApprovalAmount >= workOrderDetail.TotalCostLocalUSD))
            {
                MAML.CreateApprovalBtn = true;
            }
        }

        private void SetRoleBasics()
        {
            if (MAML == null)
                MAML = new MAM();
            SetRole();
            if (MAML.EMR_APPROVER_COUNTRY || MAML.EMR_APPROVER_SHOP || MAML.EMR_SPECIALIST_COUNTRY || MAML.EMR_SPECIALIST_SHOP || MAML.CPH || MAML.ADMIN)
                MAML.IsCountryOrAbove = true;
            MAML.IsApprover = CheckIsApprover();
        }

        private bool CheckIsApprover()
        {
            if (MAML.EMR_APPROVER_COUNTRY || MAML.EMR_APPROVER_SHOP || MAML.EMR_SPECIALIST_COUNTRY || MAML.EMR_SPECIALIST_SHOP || MAML.ISANYCPH || MAML.ADMIN)
                return true;
            else
                return false;
        }

        private void AllowCreateRemarksField(WorkOrderDetail dbWOD)
        {
            MAML.CreateRemarksTA = false;
            switch (Convert.ToString(dbWOD.WorkOrderStatus))
            {
                case "300":
                case "200": if (MAML.IsApprover) MAML.CreateRemarksTA = true; break;  //Pinaki
                case "310": if (MAML.IsApprover) MAML.CreateRemarksTA = true; break;  //Pinaki
                case "320": if (MAML.IsApprover) MAML.CreateRemarksTA = true; break;  //Pinaki
                case "330": if (MAML.EMR_SPECIALIST_COUNTRY || MAML.EMR_SPECIALIST_SHOP || MAML.ISANYCPH) MAML.CreateRemarksTA = true; break;  //Pinaki
                case "340": if (MAML.ISANYCPH) MAML.CreateRemarksTA = true; break;  //Pinaki
                case "140": if (MAML.EMR_SPECIALIST_COUNTRY || MAML.EMR_SPECIALIST_SHOP) MAML.CreateRemarksTA = true; break;  //Pinaki
                case "390": MAML.CreateRemarksTA = true; break;
                case "130": if (MAML.EMR_APPROVER_COUNTRY || MAML.EMR_APPROVER_SHOP) MAML.CreateRemarksTA = true; break;
                default: break;
            }
        }

        [HttpPost]
        public void EditMgrApproval(string WOID)
        {
            RedirectToAction("ManageWorkOrder", "ManageWorkOrder", new { IsMulti = false, orderID = Int32.Parse(WOID) });
        }

        public JsonResult SubmitMA([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Submit");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Submit");
            }

            return GetResult(UIContent, outStatus, outMsg);
            //return Json(new { Status = outStatus, UIMsg = outMsg, RemarksData = GetRemarksData(null, Convert.ToInt32(((string[])(UIContent["WOID"]))[0])) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMA([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                ErrList = ErrList.Union(WorkOrderClient.ChangeStatus(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), 9999, GetUserDtl())).ToList();
                if (ErrList != null)
                {
                    int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
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
            return GetResult(UIContent, outStatus, outMsg);
            // return Json(new { Status = outStatus, UIMsg = outMsg, RemarksData = GetRemarksData(null, Convert.ToInt32(((string[])(UIContent["WOID"]))[0])) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateMA([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList = null;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                if (CheckIsApprover() || MAML.ISANYSHOP)
                {
                    ErrList = ErrList.Union(WorkOrderClient.AddRemarkByTypeAndWOID(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), "Updated by " + GetUserDtl(), "S", GetUserDtl())).ToList();
                }
                if (ErrList != null)
                {
                    int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                    outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                    outMsg = UtilMethods.CreateMessageString(ErrList);
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Update");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Update");
            }

            return Json(new
            {
                Status = outStatus,
                UIMsg = outMsg,
                RemarksData = GetRemarksData(null, Convert.ToInt32(((string[])(UIContent["WOID"]))[0])),
                redirectUrl = "",
                isRedirect = false
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Fwd2EMRMA([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList = new List<ErrMessage>();
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                if (CheckIsApprover())
                {
                    ErrList = ErrList.Union(WorkOrderClient.ChangeStatus(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), 330, GetUserDtl())).ToList();
                    if (ErrList != null)
                    {
                        int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                        outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                        outMsg = UtilMethods.CreateMessageString(ErrList);
                    }
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Forward to EMR");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Forward to EMR");
            }
            return GetResult(UIContent, outStatus, outMsg);
            //return Json(new { Status = outStatus, UIMsg = outMsg, RemarksData = GetRemarksData(null, Convert.ToInt32(((string[])(UIContent["WOID"]))[0])) }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AdviseMSLMA([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                if (CheckIsApprover())
                {
                    ErrList = ErrList.Union(WorkOrderClient.AddRemarkByTypeAndWOID(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), "Advisement to EMR Approver by " + GetUserDtl(), "S", GetUserDtl())).ToList();
                    if (((UserSec)Session["UserSec"]).isAnyCPH)
                    {
                        ErrList = ErrList.Union(WorkOrderClient.ChangeStatus(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), 310, GetUserDtl())).ToList();
                        if (ErrList != null)
                        {
                            int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                            outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                            outMsg = UtilMethods.CreateMessageString(ErrList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Advise EMR Approver");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Advise EMR Approver");
            }
            return GetResult(UIContent, outStatus, outMsg);
            //return Json(new { Status = outStatus, UIMsg = outMsg, RemarksData = GetRemarksData(null, Convert.ToInt32(((string[])(UIContent["WOID"]))[0])) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult WorkingMA([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList = null;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                if (MAML.ISANYSHOP)
                {
                    ErrList = ErrList.Union(WorkOrderClient.UpdateShopWorkingSwitch(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), "Y", GetUserDtl())).ToList();
                }
                if (ErrList != null)
                {
                    int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                    outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                    outMsg = UtilMethods.CreateMessageString(ErrList);
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Working");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Working");
            }
            return GetResult(UIContent, outStatus, outMsg);
            //return Json(new { Status = outStatus, UIMsg = outMsg, RemarksData = GetRemarksData(null, Convert.ToInt32(((string[])(UIContent["WOID"]))[0])) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CompleteMA([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList = null;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                if (MAML.ISANYSHOP)
                {
                    ErrList = ErrList.Union(WorkOrderClient.CompleteWorkOrderByID(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), DateTime.Parse(((string[])(UIContent["REPDATE"]))[0]), GetUserDtl())).ToList();
                }
                if (ErrList != null)
                {
                    int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                    outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                    outMsg = UtilMethods.CreateMessageString(ErrList);
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Complete");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Complete");
            }
            return GetResult(UIContent, outStatus, outMsg);
            //return Json(new { Status = outStatus, UIMsg = outMsg, RemarksData = GetRemarksData(null, Convert.ToInt32(((string[])(UIContent["WOID"]))[0])) }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult TOTALLOSSMA([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList;
            List<ErrMessage> ErrListForRejectedWO;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                ErrList = ErrList.Union(WorkOrderClient.ChangeStatus(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), 150, GetUserDtl())).ToList();
                if (ErrList.Count != 0)             // Total Loss-Udita
                {
                    ErrList = ErrList.Union(WorkOrderClient.AddRemarkByTypeAndWOID(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), "Declared Total Loss by " + GetUserDtl(), "S", GetUserDtl())).ToList();
                }
                if (ErrList != null)
                {
                    int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                    outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                    outMsg = UtilMethods.CreateMessageString(ErrList);
                }
                //Total Loss- Udita
                ErrListForRejectedWO = UpdateGeneralValues(UIContent);
                ErrListForRejectedWO = ErrListForRejectedWO.Union(WorkOrderClient.ChangeStatusForOtherWOs(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), GetUserDtl())).ToList();
                //if (ErrListForRejectedWO.Count == 0)
                //{
                //    ErrListForRejectedWO = ErrList.Union(WorkOrderClient.AddRemarkByTypeAndWOID(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), "Rejected due to Total Loss by " + GetUserDtl(), "S", GetUserDtl())).ToList();
                //}
                if (ErrListForRejectedWO != null)
                {
                    int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                    outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                    outMsg = UtilMethods.CreateMessageString(ErrList);
                }
                //Total Loss-Udita
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Total Loss");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Total Loss");
            }
            return GetResult(UIContent, outStatus, outMsg);
            //return Json(new { Status = outStatus, UIMsg = outMsg, RemarksData = GetRemarksData(null, Convert.ToInt32(((string[])(UIContent["WOID"]))[0])) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RevertTotalLoss([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                // List<ErrMessage> RevertTotalLoss(int WOID, string chUser) 
                ErrList = UpdateGeneralValues(UIContent);
                ErrList = ErrList.Union(WorkOrderClient.RevertTotalLoss(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), GetUserDtl())).ToList();
                if (ErrList != null)
                {
                    int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                    outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                    outMsg = UtilMethods.CreateMessageString(ErrList);
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Revert Total Loss");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Revert Total Loss");
            }
            return GetResult(UIContent, outStatus, outMsg);
            //return Json(new { Status = outStatus, UIMsg = outMsg, RemarksData = GetRemarksData(null, Convert.ToInt32(((string[])(UIContent["WOID"]))[0])) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AdviseEMRMGRMA([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                if (CheckIsApprover())
                {
                    ErrList = ErrList.Union(WorkOrderClient.AddRemarkByTypeAndWOID(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), "Advisement to specialist by " + GetUserDtl(), "S", GetUserDtl())).ToList();
                    ErrList = ErrList.Union(WorkOrderClient.ChangeStatus(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), 330, GetUserDtl())).ToList();

                    if (ErrList != null)
                    {
                        int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                        outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                        outMsg = UtilMethods.CreateMessageString(ErrList);
                    }
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Advise EMR MGR");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Advise EMR MGR");
            }
            return GetResult(UIContent, outStatus, outMsg);
            //return Json(new { Status = outStatus, UIMsg = outMsg, RemarksData = GetRemarksData(null, Convert.ToInt32(((string[])(UIContent["WOID"]))[0])) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Fwd2CENEQULOSMA([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList = null;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                if (CheckIsApprover())
                {
                    ErrList = ErrList.Union(WorkOrderClient.ChangeStatus(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), 340, GetUserDtl())).ToList();
                    if (ErrList != null)
                    {
                        int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                        outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                        outMsg = UtilMethods.CreateMessageString(ErrList);
                    }
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Forward to Cenequlos");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Forward to Cenequlos");
            }
            return GetResult(UIContent, outStatus, outMsg);
            //return Json(new { Status = outStatus, UIMsg = outMsg, RemarksData = GetRemarksData(null, Convert.ToInt32(((string[])(UIContent["WOID"]))[0])) }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ApproveMA([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList;
            string outStatus = string.Empty, outMsg = string.Empty; int index = 0;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                if (CheckIsApprover())
                {
                    string sApproveRemark = "";
                    if (((UserSec)Session["UserSec"]).isEMRApproverCountry || ((UserSec)Session["UserSec"]).isEMRApproverShop || ((UserSec)Session["UserSec"]).isAdmin || ((UserSec)Session["UserSec"]).isAnyCPH)
                    {
                        if (((UserSec)Session["UserSec"]).isEMRApproverCountry || ((UserSec)Session["UserSec"]).isEMRApproverShop)
                            sApproveRemark = "Approved by EMR Approver user " + GetUserDtl();
                        else if (((UserSec)Session["UserSec"]).isAdmin)
                            sApproveRemark = "Approved by Admin user " + GetUserDtl();
                        else if (((UserSec)Session["UserSec"]).isCPH)
                            sApproveRemark = "Approved by CPH user " + GetUserDtl(); //Debadrita_Leo_new_requirement
                        else if (((UserSec)Session["UserSec"]).isEMRSpecialistCountry || ((UserSec)Session["UserSec"]).isEMRSpecialistShop)
                            sApproveRemark = "Approved by EMR Specialist user " + GetUserDtl(); //Debadrita_Leo_new_requirement


                        ErrList = ErrList.Union(WorkOrderClient.ApproveWorkOrder(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), GetUserDtl(), sApproveRemark, "")).ToList();

                        if (ErrList != null)
                        {
                            index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                            outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                            outMsg = UtilMethods.CreateMessageString(ErrList);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Approve");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Approve");
            }

            return GetResult(UIContent, outStatus, outMsg);
        }

        private JsonResult GetResult(Dictionary<string, object> UIContent, string outStatus, string outMsg)
        {
            int _WOID = GetWOID(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), "NEXT");
            return Json(new
            {
                Status = outStatus,
                UIMsg = outMsg,
                RemarksData = (_WOID != 0 ? "" : GetRemarksData(null, Convert.ToInt32(((string[])(UIContent["WOID"]))[0]))),
                redirectUrl = Url.Action("ManagerApproval", "ManagerApproval", new { WO_ID = _WOID }),
                isRedirect = (_WOID != 0)
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SuspendMA([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList = null;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                if (CheckIsApprover())
                {
                    ErrList = ErrList.Union(WorkOrderClient.AddRemarkByTypeAndWOID(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), "Suspended by " + GetUserDtl(), "S", GetUserDtl())).ToList();
                    if (((UserSec)Session["UserSec"]).isAnyCPH)
                        ErrList = ErrList.Union(WorkOrderClient.ChangeStatus(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), 330, GetUserDtl())).ToList();
                    else if (((UserSec)Session["UserSec"]).isEMRApproverCountry || ((UserSec)Session["UserSec"]).isEMRApproverShop || ((UserSec)Session["UserSec"]).isEMRSpecialistCountry || ((UserSec)Session["UserSec"]).isEMRSpecialistShop || ((UserSec)Session["UserSec"]).isAdmin)
                        ErrList = ErrList.Union(WorkOrderClient.ChangeStatus(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), 310, GetUserDtl())).ToList();
                    else
                    {
                        //move to next wo
                    }
                }

                if (ErrList != null)
                {
                    int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                    outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                    outMsg = UtilMethods.CreateMessageString(ErrList);
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Suspend");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Suspend");
            }
            return GetResult(UIContent, outStatus, outMsg);
        }

        public JsonResult Reject2MSL([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList = null;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                if (CheckIsApprover())
                {
                    ErrList = ErrList.Union(WorkOrderClient.AddRemarkByTypeAndWOID(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), "Rejected by " + GetUserDtl(), "S", GetUserDtl())).ToList();
                    if (((UserSec)Session["UserSec"]).isEMRApproverCountry || ((UserSec)Session["UserSec"]).isEMRApproverShop || ((UserSec)Session["UserSec"]).isAdmin)
                    {
                        ErrList = ErrList.Union(WorkOrderClient.ChangeStatus(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), 100, GetUserDtl())).ToList();
                        ErrList = ErrList.Union(WorkOrderClient.UpdateRevNo(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]))).ToList();
                    }
                    else if (((UserSec)Session["UserSec"]).isAnyCPH || ((UserSec)Session["UserSec"]).isEMRSpecialistCountry || ((UserSec)Session["UserSec"]).isEMRSpecialistShop)
                    {
                        ErrList = ErrList.Union(WorkOrderClient.ChangeStatus(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), 130, GetUserDtl())).ToList();
                    }
                    if (ErrList != null)
                    {
                        int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                        outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                        outMsg = UtilMethods.CreateMessageString(ErrList);
                    }
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Reject to MSL");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Reject to MSL");
            }
            return GetResult(UIContent, outStatus, outMsg);
        }

        public JsonResult RejectTo100([ModelBinder(typeof(DictionaryModelBinder))]  Dictionary<string, object> UIContent)
        {
            SetRoleBasics();
            List<ErrMessage> ErrList = null;
            string outStatus = string.Empty, outMsg = string.Empty;
            try
            {
                ErrList = UpdateGeneralValues(UIContent);
                if (CheckIsApprover())
                {
                    ErrList = ErrList.Union(WorkOrderClient.AddRemarkByTypeAndWOID(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), "Rejected by " + GetUserDtl(), "S", GetUserDtl())).ToList();
                    if (((UserSec)Session["UserSec"]).isEMRApproverCountry || ((UserSec)Session["UserSec"]).isEMRApproverShop || ((UserSec)Session["UserSec"]).isEMRSpecialistCountry || ((UserSec)Session["UserSec"]).isEMRSpecialistShop || ((UserSec)Session["UserSec"]).isAdmin)
                    {
                        ErrList = ErrList.Union(WorkOrderClient.ChangeStatus(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), 100, GetUserDtl())).ToList();
                        ErrList = ErrList.Union(WorkOrderClient.UpdateRevNo(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]))).ToList();
                    }
                    else if (((UserSec)Session["UserSec"]).isAnyCPH)
                    {
                        ErrList = ErrList.Union(WorkOrderClient.ChangeStatus(Convert.ToInt32(((string[])(UIContent["WOID"]))[0]), 140, GetUserDtl())).ToList();
                    }
                    if (ErrList != null)
                    {
                        int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                        outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                        outMsg = UtilMethods.CreateMessageString(ErrList);
                    }
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Reject to 100");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Reject to 100");
            }
            return GetResult(UIContent, outStatus, outMsg);
        }

        private void SetRole()
        {
            MAML.ADMIN = ((UserSec)Session["UserSec"]).isAdmin;
            MAML.CPH = ((UserSec)Session["UserSec"]).isCPH;
            MAML.EMR_SPECIALIST_COUNTRY = ((UserSec)Session["UserSec"]).isEMRSpecialistCountry;
            MAML.EMR_SPECIALIST_SHOP = ((UserSec)Session["UserSec"]).isEMRSpecialistShop;
            MAML.EMR_APPROVER_COUNTRY = ((UserSec)Session["UserSec"]).isEMRApproverCountry;
            MAML.EMR_APPROVER_SHOP = ((UserSec)Session["UserSec"]).isEMRApproverShop;
            MAML.SHOP = ((UserSec)Session["UserSec"]).isShop;
            MAML.MPRO_CLUSTER = ((UserSec)Session["UserSec"]).isMPROCluster;
            MAML.MPRO_SHOP = ((UserSec)Session["UserSec"]).isMPROShop;
            MAML.READONLY = ((UserSec)Session["UserSec"]).isReadOnly;
            MAML.ISANYCPH = ((UserSec)Session["UserSec"]).isAnyCPH;
            MAML.ISANYSHOP = ((UserSec)Session["UserSec"]).isAnyShop;

            MAML.CENEQULOS = true;
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
                MAML.Type = Eqp.COType + "/" + Eqp.Eqstype;
                MAML.Size = Eqp.Size;

                if (!string.IsNullOrEmpty(Eqp.Eqstype) && (Eqp.COType.Equals("CONT") && Eqp.Eqstype.Equals("REEF")))
                    MAML.ReeferMakeModel = Eqp.EQRuman + "/" + Eqp.EQRutyp;
                else
                    MAML.ReeferMakeModel = "";
                if (!(Eqp.Deldatsh == null || Eqp.Deldatsh.Equals(DateTime.MinValue)))
                    MAML.Deldatsh = Convert.ToDateTime(Eqp.Deldatsh).ToString("yyyy-MM-dd");
                else
                    MAML.Deldatsh = "";
                MAML.Profile = Eqp.EQProfile;
                MAML.Material = Eqp.Eqmatr;
                MAML.LeasingCompany = Eqp.LeasingCompany;
                MAML.InService = Convert.ToDateTime(Eqp.EQInDate) <= DateTime.MinValue ? "" : Convert.ToDateTime(Eqp.EQInDate).ToString("yyyy-MM-dd");
                MAML.GenSetMakeModel = (!Eqp.COType.Equals("GENS") ? "" : (Eqp.EqMancd + "/" + Eqp.EQRutyp));
                MAML.ExtensionDate = Eqp.ExtensionDate.ToString();
                MAML.EqpNotFound = Eqp.EquipmentNo;
                MAML.BoxMfg = (Eqp.COType.Equals("CONT") ? Eqp.EqMancd : "");
                MAML.LeasingContract = Eqp.LeasingContract;
                MAML.Size = Eqp.Size + "/" + Eqp.Eqouthgu;
                switch (Eqp.Damage)
                {
                    case "1": MAML.Damage = "1-Total Loss"; break;
                    case "2": MAML.Damage = "2-Damaged Box"; break;
                    case "3": MAML.Damage = "3-Damaged Reefer Box + Reefer Unit"; break;
                    case "4": MAML.Damage = "4-Damaged Reefer Machinery"; break;
                    default: MAML.Damage = "No Damage/Sound Condition"; break;
                }
                MAML.PresentLoc = Eqp.PresentLoc;
                MAML.GateInDate = Convert.ToDateTime(Eqp.GateInDate) <= DateTime.MinValue ? "" : Convert.ToDateTime(Eqp.GateInDate).ToString("yyyy-MM-dd");
                MAML.StSelCR = Eqp.STSELSCR;
                MAML.Owner = Eqp.Eqowntp;
                MAML.AtOfcHrLoc = Eqp.OffhirLocationSW;
                MAML.OnGlobalHunt = Eqp.Stredel;
                MAML.Empty = Eqp.StEmptyFullInd;
                MAML.ForRefurb = Eqp.Strefurb;
                MAML.FixedCover = Convert.ToString(Eqp.Fixcover);
                MAML.DPP = Convert.ToString(Eqp.Dpp);
                MAML.RefurbDate = Convert.ToDateTime(Eqp.RefurbishmnentDate) <= DateTime.MinValue ? "" : Convert.ToDateTime(Eqp.RefurbishmnentDate).ToString("yyyy-MM-dd");
                //MAML.CurrLoc = Eqp.CurrentLoc;
                //MAML.CurrMove = Eqp.CurrentMove;
            }
        }

        public List<ErrMessage> UpdateGeneralValues(Dictionary<string, object> UIContent)
        {
            List<ErrMessage> ErrList = new List<ErrMessage>();
            string outStatus = string.Empty, outMsg = string.Empty;
            int woid = Convert.ToInt32(((string[])(UIContent["WOID"]))[0]);
            try
            {
                if (!string.IsNullOrEmpty(((string[])(UIContent["EREMARKS"]))[0]))
                {
                    ErrList = ErrList.Union(WorkOrderClient.AddRemarkByTypeAndWOID(woid, ((string[])(UIContent["EREMARKS"]))[0], "E", GetUserDtl())).ToList();
                }

                if (!string.IsNullOrEmpty(((string[])(UIContent["IREMARKS"]))[0]))
                {
                    ErrList = ErrList.Union(WorkOrderClient.AddRemarkByTypeAndWOID(woid, ((string[])(UIContent["IREMARKS"]))[0], "I", GetUserDtl())).ToList();
                }

                if (!string.IsNullOrEmpty(((string[])(UIContent["CAUSE"]))[0]))
                {
                    List<ErrMessage> ErrMsgList = (WorkOrderClient.UpdateThirdPartyCause(woid, ((string[])(UIContent["TPP"]))[0], ((string[])(UIContent["CAUSE"]))[0], GetUserDtl())).ToList();
                    ErrList = ErrList.Union(ErrMsgList).ToList();
                }


                //Debadrita_TPI_Indicator-19-07-19---Start
                if (!string.IsNullOrEmpty(((string[])(UIContent["NEWTPICODE"]))[0]))
                {
                    if (MAML.ADMIN == true || MAML.EMR_SPECIALIST_COUNTRY == true || MAML.EMR_SPECIALIST_SHOP == true
                        || MAML.EMR_APPROVER_COUNTRY == true || MAML.EMR_APPROVER_SHOP == true || MAML.CPH == true)
                    {
                        List<string> RepCode = null;
                        List<string> NewTPICODE = null;
                        RepCode = new List<string>(((string[])(UIContent["REPRCODE"]))[0].Split(UtilMethods.SPLITBYCOMMA));
                        NewTPICODE = new List<string>(((string[])(UIContent["NEWTPICODE"]))[0].Split(UtilMethods.SPLITBYCOMMA));
                        for (int srStart = 0; srStart < RepCode.Count; srStart++)
                        {
                            List<ErrMessage> ErrMsgList = (WorkOrderClient.UpdateNewTPI(woid, RepCode[srStart], NewTPICODE[srStart], GetUserDtl())).ToList();

                            ErrList = ErrList.Union(ErrMsgList).ToList();
                        }
                    }
                }

                //Debadrita_TPI_Indicator-19-07-19---end


                if (!string.IsNullOrEmpty(((string[])(UIContent["SERIALNO"]))[0]))
                {
                    List<string> SerialNo = null;
                    List<string> RepCode = null;
                    List<string> PartCode = null;

                    SerialNo = new List<string>(((string[])(UIContent["SERIALNO"]))[0].Split(UtilMethods.SPLITBYCOMMA));
                    RepCode = new List<string>(((string[])(UIContent["REPCODE"]))[0].Split(UtilMethods.SPLITBYCOMMA));
                    PartCode = new List<string>(((string[])(UIContent["PARTNO"]))[0].Split(UtilMethods.SPLITBYCOMMA));
                    for (int srStart = 0; srStart < SerialNo.Count; srStart++)
                    {
                        //'serial number field format: "SN~(0)" & WorkOrderID(1) & "~" & repairCode(2)  & "~" & partNumber(3) ==> SN~872~5314~XX-1001
                        ErrList = ErrList.Union(WorkOrderClient.UpdateSerialNumber(woid, RepCode[srStart], PartCode[srStart], SerialNo[srStart], GetUserDtl())).ToList();
                    }
                }

                if (UIContent.ContainsKey("REPDATE"))
                {
                    if (!string.IsNullOrEmpty(((string[])(UIContent["REPDATE"]))[0]))
                    {
                        ErrList = ErrList.Union(WorkOrderClient.UpdateRepairDateByWOID(woid, Convert.ToDateTime(((string[])(UIContent["REPDATE"]))[0]), GetUserDtl())).ToList();

                    }
                }

                if (ErrList != null)
                {
                    int index = ErrList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                    outStatus = (index != -1 ? Validation.MESSAGETYPE.ERROR.ToString() : Validation.MESSAGETYPE.SUCCESS.ToString());
                    outMsg = UtilMethods.CreateMessageString(ErrList);
                }
            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Update");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Update");
            }
            return ErrList;
        }

        public ActionResult GetNextWODetail(int WOID, string ACTIONTYPE)
        {
            int _WOID = GetWOID(WOID, ACTIONTYPE);

            if (_WOID != 0)
                CreateWOSum(_WOID);
            else
                CreateWOSum(WOID);
            return View("ManagerApproval", MAML);
        }

        private int GetWOID(int WOID, string ACTIONTYPE)
        {
            int _WOID = 0, index = 0;
            List<string> WO_LIST = new List<string>();

            if (Session["WO_LIST"] != null)
            {
                WO_LIST = new List<string>((Session["WO_LIST"].ToString()).Split(UtilMethods.SPLITBYCOMMA));
                switch (ACTIONTYPE)
                {
                    case "NEXT":
                        index = WO_LIST.IndexOf(WOID.ToString());
                        if ((index + 1) < WO_LIST.Count())
                            _WOID = Convert.ToInt32(WO_LIST.ElementAt(index + 1));
                        break;
                    case "LAST":
                        index = WO_LIST.Count();
                        if (index != -1)
                            _WOID = Convert.ToInt32(WO_LIST.ElementAt(index - 1));
                        break;
                    case "PREVIOUS":
                        index = WO_LIST.IndexOf(WOID.ToString());
                        if (index > 0)
                            _WOID = Convert.ToInt32(WO_LIST.ElementAt(index - 1));
                        break;
                    case "FIRST":
                        _WOID = Convert.ToInt32(WO_LIST.ElementAt(0));
                        break;
                    default: break;
                }
            }
            return _WOID;
        }

        public string GetRemarksData(ManagerApprovalModel _ma, int _woID)
        {
            if (_ma == null)
            {
                SetRoleBasics();
                MAML.dbWOD = new WorkOrderDetail();
                MAML.dbWOD.RemarksList = new List<RemarkEntry>();
                MAML.dbWOD.RemarksList = WorkOrderClient.LoadRemarksDetails(_woID).ToList();
                return PartialView("MgrAprvlRemarks", MAML).RenderToString();
            }
            else
                return PartialView("MgrAprvlRemarks", _ma).RenderToString();
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
        ~ManagerApprovalController()
        {
            Dispose(false);
        }

        #endregion

        //public static string RenderRazorViewToString(this Controller controller, string viewName, object model)
        //{
        //    controller.ViewData.Model = model;
        //    using (var sw = new StringWriter())
        //    {
        //        var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
        //        var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
        //        viewResult.View.Render(viewContext, sw);
        //        viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
        //        return sw.GetStringBuilder().ToString();
        //    }
        //}
    }
}
