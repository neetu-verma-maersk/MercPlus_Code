using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MercPlusClient.Areas.ManageWorkOrder.Models;
using MercPlusClient.ManageWorkOrderServiceReference;
using MercPlusClient.ManageMasterDataServiceReference;
using MercPlusClient;
using System.Web.UI.WebControls;
using System.Collections.ObjectModel;
using System.Collections;
using MercPlusClient.UtilityClass;
//using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;
using System.ServiceModel;

namespace MercPlusClient.Areas.ManageWorkOrder.Controllers
{
    public class ReviewEstimatesController : Controller, IDisposable
    {
        ReviewEstimatesModel model_ReviewEstimate = new ReviewEstimatesModel();
        ManageWorkOrderServiceReference.ManageWorkOrderClient obj_Sercice = new ManageWorkOrderServiceReference.ManageWorkOrderClient();
        ManageMasterDataServiceReference.ManageMasterDataClient mmdc = new ManageMasterDataServiceReference.ManageMasterDataClient();

        /// <summary>
        /// Review Estimate on page load 
        /// </summary>
        /// <param Name>Afroz Khan></param>
        /// <param Created On>01-Sep-2015</param>
        /// <param Topic>Review Estimate</param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult ReviewEstimates(FormCollection collection)
        {
            // Logger.Write("Review Estimate -- Review Estimate Load Started.");
            try
            {
                model_ReviewEstimate = PopulateAllDropDowns();
                model_ReviewEstimate.ErrorMsg = "";
                Session["WO_LIST"] = null;
            }
            catch (Exception ex)
            {

                //Logger.Write("Review Estimate -- ERROR - Review Estimate Load." + ex.Message + "");
                throw;
            }
            // Logger.Write("Review Estimate -- Review Estimate Load End.");
            return View("ReviewEstimates", model_ReviewEstimate);
        }

        /// <summary>
        /// Search for WO on search button 
        /// </summary>
        /// <param Name>Afroz Khan></param>
        /// <param Created On>02-Sep-2015</param>
        /// <param Topic>Review Estimate</param>
        /// <returns>ActionResult</returns>
        public ActionResult WOSearch(string ShopCode, string FromDate, string ToDate, string CustomerCD, string EqpSize, string EqpType, string EqSubType, string mode, string EquipmentNo, string VenRefNo, string Cocl, string country, string Location, string QueryType, int SortBy, ReviewEstimatesModel model)
        {            
            List<ReviewEstimatesModel> y = null;
            Session["QueryType"] = QueryType.ToString();
            var qq = y;


            try
            {

                MercPlusClient.UtilityClass.UtilMethods.ApprovalAmt = obj_Sercice.RSUserByUserId(((UserSec)Session["UserSec"]).UserId);
                if (((UserSec)Session["UserSec"]).UserType == "ADMIN" || ((UserSec)Session["UserSec"]).UserType == "AREA" || ((UserSec)Session["UserSec"]).UserType == "COUNTRY" || ((UserSec)Session["UserSec"]).UserType == "CPH" || ((UserSec)Session["UserSec"]).UserType == "EMR_SPECIALIST_COUNTRY" || ((UserSec)Session["UserSec"]).UserType == "EMR_APPROVER_COUNTRY" || ((UserSec)Session["UserSec"]).UserType == "EMR_SPECIALIST_SHOP" || ((UserSec)Session["UserSec"]).UserType == "EMR_APPROVER_SHOP" || ((UserSec)Session["UserSec"]).UserType == "MPRO_CLUSTER" || ((UserSec)Session["UserSec"]).UserType == "MPRO_SHOP" || ((UserSec)Session["UserSec"]).UserType == "READONLY")
                {
                    var WOList = obj_Sercice.GetWorkOrderByCountryOrHigher(ShopCode, FromDate, ToDate, CustomerCD, EqpSize, EqpType, EqSubType, mode, EquipmentNo, VenRefNo, Cocl, country, Location, QueryType, SortBy, ((UserSec)Session["UserSec"]).UserId).ToList();
                    qq = (from e in WOList
                          select new ReviewEstimatesModel
                          {

                              WO_ID = Convert.ToString(e.WorkOrderID),
                              LOC_CD = e.AreaCode,
                              SHOP_CD = Convert.ToString(e.Shop.ShopCode),
                              STATUS_CD = e.StatusCode,
                              STATUS_DESC = e.Status,
                              TOTAL_COST_LOCAL = Math.Round(Convert.ToDecimal(e.TotalCostLocal), 2).ToString(),
                              TOTAL_COST_LOCAL_USD = Math.Round(Convert.ToDecimal(e.TotalCostLocalUSD), 2).ToString(),
                              TOTOL_COST_REPAIR_CPH = Math.Round(Convert.ToDecimal(e.TotalCostOfRepairCPH), 2).ToString(),
                              EQPNO = e.EquipmentList[0].EquipmentNo,
                              VENDOR_REF_NO = e.EquipmentList[0].VendorRefNo,
                              MODE = e.Mode,
                              SHOP_WORKING_SW = e.ShopWorkingSW,
                              REPAIR_DTE = (string.IsNullOrEmpty(e.RepairDate.ToString())) ? string.Empty : (Convert.ToDateTime(e.RepairDate.ToString())).ToString("yyyy-MM-dd"),
                              WO_RECV_DTE = (string.IsNullOrEmpty(e.WorkOrderReceiveDate.ToString())) ? string.Empty : (Convert.ToDateTime(e.WorkOrderReceiveDate.ToString())).ToString("yyyy-MM-dd"),
                              CRTS = (string.IsNullOrEmpty(e.ChangeTime.ToString())) ? string.Empty : (Convert.ToDateTime(e.ChangeTime.ToString())).ToString("yyyy-MM-dd"),
                              VoucherNo = e.VoucherNumber,
                              PayAgent_CD = e.PayAgentCode,
                              AgentVouchNo = (e.PayAgentCode == null ? string.Empty : e.PayAgentCode) + "/" + (e.VoucherNumber == null ? string.Empty : e.VoucherNumber),
                              WorkOrderIDList = string.Join(",", WOList.Select(id => { return id.WorkOrderID; }).ToList().ToArray())
                          }).ToList();

                }
                else
                {
                    var WOList = obj_Sercice.GetWorkOrder(ShopCode, FromDate, ToDate, CustomerCD, EqpSize, EqpType, EqSubType, mode, EquipmentNo, VenRefNo, Cocl, country, Location, QueryType, SortBy, ((UserSec)Session["UserSec"]).UserId).ToList();
                    qq = (from e in WOList
                          select new ReviewEstimatesModel
                          {
                              WO_ID = Convert.ToString(e.WorkOrderID),
                              intSerialNo = obj_Sercice.GetSerialNo(e.WorkOrderID.ToString()) == 1 ? 1 : 0,
                              LOC_CD = e.AreaCode,
                              SHOP_CD = Convert.ToString(e.Shop.ShopCode),
                              STATUS_CD = e.StatusCode,
                              STATUS_DESC = e.Status,
                              TOTAL_COST_LOCAL = Math.Round(Convert.ToDecimal(e.TotalCostLocal), 2).ToString(),
                              TOTAL_COST_LOCAL_USD = Math.Round(Convert.ToDecimal(e.TotalCostLocalUSD), 2).ToString(),
                              TOTOL_COST_REPAIR_CPH = Math.Round(Convert.ToDecimal(e.TotalCostOfRepairCPH), 2).ToString(),
                              EQPNO = e.EquipmentList[0].EquipmentNo,
                              VENDOR_REF_NO = e.EquipmentList[0].VendorRefNo,
                              MODE = e.Mode,
                              SHOP_WORKING_SW = e.ShopWorkingSW,
                              REPAIR_DTE = (string.IsNullOrEmpty(e.RepairDate.ToString())) ? string.Empty : (Convert.ToDateTime(e.RepairDate.ToString())).ToString("yyyy-MM-dd"),
                              WO_RECV_DTE = (string.IsNullOrEmpty(e.WorkOrderReceiveDate.ToString())) ? string.Empty : (Convert.ToDateTime(e.WorkOrderReceiveDate.ToString())).ToString("yyyy-MM-dd"),
                              CRTS = (string.IsNullOrEmpty(e.ChangeTime.ToString())) ? string.Empty : (Convert.ToDateTime(e.ChangeTime.ToString())).ToString("yyyy-MM-dd"),
                              VoucherNo = e.VoucherNumber,
                              PayAgent_CD = e.PayAgentCode,
                              AgentVouchNo = (e.PayAgentCode == null ? string.Empty : e.PayAgentCode) + "/" + (e.VoucherNumber == null ? string.Empty : e.VoucherNumber),
                              WorkOrderIDList = string.Join(",", WOList.Select(id => { return id.WorkOrderID; }).ToList().ToArray())
                          }).ToList();

                }

                if (qq != null && qq.Count > 0 && qq[0].WorkOrderIDList != null)
                    Session["WO_LIST"] = qq[0].WorkOrderIDList;

                qq.OrderBy(li => li.EQPNO);
                for (int j = 0; j < qq.Count(); j++)
                {
                    if (((UserSec)Session["UserSec"]).isEMRApproverCountry || ((UserSec)Session["UserSec"]).isEMRApproverShop)
                    {
                        if (qq[j].STATUS_CD == "200")
                        {
                            model.initPendingflag = 1;
                            break;
                        }
                    }
                    if (((UserSec)Session["UserSec"]).isAnyShop)
                    {
                        if (qq[j].STATUS_CD == "390")
                        {
                            model.intWorkingflag = 1;
                            break;
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                model_ReviewEstimate.ErrorMsg = ex.Message;                
            }            
            model.SearchResults = qq;
            return PartialView("ShowWO_List", model);
        }



        /// <summary>
        /// Fill Dropdown on page load 
        /// </summary>
        /// <param Name>Afroz Khan></param>
        /// <param Created On>03-Sep-2015</param>
        /// <param Topic>Review Estimate</param>
        /// <returns>JReviewEstimatesModel</returns>
        /// 
        private ReviewEstimatesModel PopulateAllDropDowns()
        {            
            ReviewEstimatesModel _model = new ReviewEstimatesModel();

            try
            {
                #region ShopDropDown
                _model.ShopList = new List<SelectListItem>();

                // Logger.Write("Review Estimate -- Fill Shop List Started.");
                var Shop = obj_Sercice.GetShopCode(Convert.ToInt32(((UserSec)Session["UserSec"]).UserId)).ToList();

                _model.ShopList = (from d in Shop
                                   select new SelectListItem
                                   {
                                       Value = d.ShopCode.ToString(),
                                       Text = d.ShopCode + "-" + d.ShopDescription
                                   }).ToList();

                _model.ShopList.OrderBy(li => li.Text);
                _model.ShopList.Insert(0, new SelectListItem
                {
                    Value = "0",
                    Text = "ALL"

                });
                // Logger.Write("Review Estimate -- Fill Shop List End.");

                #endregion ShopDropDown
                #region CustomerDropDown

                // Logger.Write("Review Estimate -- Fill Customer List Started.");
                string s = Request.Form["lstShopList"];
                var Cust = obj_Sercice.GetCustomerCodeByShopCode(Request.Form["lstShopList"], Convert.ToInt32(((UserSec)Session["UserSec"]).UserId)).ToList();

                _model.CustomerList = (from d in Cust
                                       select new SelectListItem
                                       {
                                           Value = d.CustomerCode.ToString(),
                                           Text = d.CustomerDesc
                                       }).ToList();

                _model.CustomerList.OrderBy(li => li.Text);
                _model.CustomerList.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "ALL"

                });
                // Logger.Write("Review Estimate -- Fill Shop List End.");

                #endregion CustomerDropDown
                #region EquipmentSize

                // Logger.Write("Review Estimate -- Fill Equipment Size Started.");


                var list = new SelectList(new[] 
                {
                    new { ID = "0", Name = "ALL" },
                    new { ID = "20", Name = "20" },
                    new { ID = "40", Name = "40" },
                    new { ID = "45", Name = "45" },
                    new { ID = "48", Name = "48" },
                },
                    "ID", "Name", 0);

                _model.EqpSizeList = list.ToList();
                // Logger.Write("Review Estimate -- Fill Equipment Size End.");

                #endregion EquipmentSize
                #region EquipmentType

                // Logger.Write("Review Estimate -- Fill Equipment Type List Started.");
                var EqpType = obj_Sercice.GetEquipmentType().ToList();
                _model.EqpTypeList = (from d in EqpType
                                      select new SelectListItem
                                      {
                                          Value = d.EqpType.ToString(),
                                          Text = d.EqTypeDesc
                                      }).ToList();
                _model.EqpTypeList.OrderBy(li => li.Text);
                _model.EqpTypeList.Insert(0, new SelectListItem
                {
                    Value = "0",
                    Text = "ALL",
                    Selected = true
                });
                // Logger.Write("Review Estimate -- Fill Equipment Type List End.");

                #endregion EquipmentType
                #region EquipmentSubType

                // Logger.Write("Review Estimate -- Fill Equipment SubType List Started.");
                string Eqp_Type = "-1";
                var EqpsType = obj_Sercice.GetEquipmentSubType(Eqp_Type).ToList();
                _model.EqpSubTypeList = (from d in EqpsType
                                         select new SelectListItem
                                         {
                                             Value = "0",
                                             Text = "ALL"
                                         }).ToList();
                // Logger.Write("Review Estimate -- Fill Equipment SubType List End.");

                #endregion EquipmentSubType
                #region ModeDropDown

                // Logger.Write("Review Estimate -- Fill Mode List Started.");
                var Mode = mmdc.Get_ModeList().ToList();

                _model.ModeList = (from d in Mode
                                   select new SelectListItem
                                   {
                                       Value = d.ModeCode.ToString(),
                                       Text = d.ModeCode.ToString() + "-" + d.ModeDescription
                                   }).ToList();

                if (_model.ModeList != null)
                {
                    _model.ModeList.OrderBy(li => li.Text);
                    _model.ModeList.Insert(0, new SelectListItem
                    {
                        Value = "",
                        Text = "ALL"
                    });
                }
                //  Logger.Write("Review Estimate -- Fill Mode List End.");

                #endregion ModeDropDown
                #region QueryType
                Dictionary<string, string> QueryTypeLists = new Dictionary<string, string>();

                if (((UserSec)Session["UserSec"]).isEMRApproverCountry)
                {
                    QueryTypeLists = MercPlusClient.UtilityClass.MercApplicationContant.GetSTATUSByReviewEstimate("EMR_APPROVER_COUNTRY");
                }
                else if (((UserSec)Session["UserSec"]).isEMRApproverShop)
                {
                    QueryTypeLists = MercPlusClient.UtilityClass.MercApplicationContant.GetSTATUSByReviewEstimate("EMR_APPROVER_SHOP");
                }
                else if (((UserSec)Session["UserSec"]).isCPH)
                {
                    QueryTypeLists = MercPlusClient.UtilityClass.MercApplicationContant.GetSTATUSByReviewEstimate("CPH");
                }
                else if (((UserSec)Session["UserSec"]).isEMRSpecialistCountry)
                {
                    QueryTypeLists = MercPlusClient.UtilityClass.MercApplicationContant.GetSTATUSByReviewEstimate("EMR_SPECIALIST_COUNTRY");
                }
                else if (((UserSec)Session["UserSec"]).isEMRSpecialistShop)
                {
                    QueryTypeLists = MercPlusClient.UtilityClass.MercApplicationContant.GetSTATUSByReviewEstimate("EMR_SPECIALIST_SHOP");
                }
                else if (((UserSec)Session["UserSec"]).isAdmin)
                {
                    QueryTypeLists = MercPlusClient.UtilityClass.MercApplicationContant.GetSTATUSByReviewEstimate("ADMIN");
                }
                else if (((UserSec)Session["UserSec"]).isShop)
                {
                    QueryTypeLists = MercPlusClient.UtilityClass.MercApplicationContant.GetSTATUSByReviewEstimate("SHOP");
                }
                else if (((UserSec)Session["UserSec"]).isMPROCluster)
                {
                    QueryTypeLists = MercPlusClient.UtilityClass.MercApplicationContant.GetSTATUSByReviewEstimate("MPRO_CLUSTER");
                }
                else if (((UserSec)Session["UserSec"]).isMPROShop)
                {
                    QueryTypeLists = MercPlusClient.UtilityClass.MercApplicationContant.GetSTATUSByReviewEstimate("MPRO_SHOP");
                }
                else if (((UserSec)Session["UserSec"]).isReadOnly)
                {
                    QueryTypeLists = MercPlusClient.UtilityClass.MercApplicationContant.GetSTATUSByReviewEstimate("READONLY");
                }

                _model.QueryTypeList = (from d in QueryTypeLists
                                        select new SelectListItem
                                        {
                                            Value = d.Key.ToString(),
                                            Text = d.Value.ToString()
                                        }).ToList();
                #endregion QueryType
                #region SortType

                //  Logger.Write("Review Estimate -- Fill Sorting Type List Started.");
                var SortTypeLists = (from MercPlusClient.UtilityClass.MercApplicationContant.SortType n in Enum.GetValues(typeof(MercPlusClient.UtilityClass.MercApplicationContant.SortType))
                                     select new { ID = (int)n, Name = MercPlusClient.UtilityClass.MercApplicationContant.GetEnumDescription(n) }).ToList();
                _model.SortList = (from d in SortTypeLists
                                   select new SelectListItem
                                   {
                                       Value = d.ID.ToString(),
                                       Text = d.Name.ToString()
                                   }).ToList();

                // Logger.Write("Review Estimate -- Fill Sorting Type List End.");

                #endregion SortType
            }
             catch (Exception ex)
            {                
                throw;
            }

            return _model;

        }

        /// <summary>
        /// Fill Equipment SubType by Eqipment Type
        /// </summary>
        /// <param Name>Afroz Khan></param>
        /// <param Created On>04-Sep-2015</param>
        /// <param Topic>Review Estimate</param>
        /// <returns>JSON Result</returns>
        /// 
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult FillEquipmentSubType(string EqpType)
        {
            //Logger.Write("Review Estimate -- Fill Equpment SubType List Started.");
            List<SelectListItem> SubType = new List<SelectListItem>();
            try
            {
                var reader = obj_Sercice.GetEquipmentSubType(EqpType).ToList();


                foreach (var q in reader)
                {
                    SubType.Add(new SelectListItem { Text = q.EqSType.ToString(), Value = q.EqSType.ToString() });
                }

            }
            catch (Exception ex)
            {

                // Logger.Write("WORK ORDERReview Estimate -- ERROR - Fill Equipment SubType List " + ex.Message + "");
                throw;
            }
            //Logger.Write("Review Estimate -- Fill Equipment SubType List End."); 
            return Json(SubType, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fill CustomerList by ShopCode
        /// </summary>
        /// <param Name>Afroz Khan></param>
        /// <param Created On>04-Sep-2015</param>
        /// <param Topic>Review Estimate</param>
        /// <returns>JSON Result</returns>
        /// 
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult FillCustomerList(string ShopCode)
        {
            List<SelectListItem> CustList = new List<SelectListItem>();
            try
            {
                // Logger.Write("Review Estimate -- Fill Customer List Started."); 

                var reader = obj_Sercice.GetCustomerCodeByShopCode(ShopCode, Convert.ToInt32(((UserSec)Session["UserSec"]).UserId)).ToList();

                if (reader.Count > 0)
                {
                    foreach (var q in reader)
                    {
                        CustList.Add(new SelectListItem { Text = q.CustomerCode.ToString(), Value = q.CustomerCode.ToString() });
                    }
                }
                else
                {
                    CustList.Add(new SelectListItem { Text = "ALL", Value = "" });
                }

            }
            catch (Exception ex)
            {
                // Logger.Write("Review Estimate -- ERROR - Fill Customer List " + ex.Message + ""); 
                throw;
            }
            //Logger.Write("Review Estimate -- Fill Customer List End."); 
            return Json(CustList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update Work Order by Work ID
        /// </summary>
        /// <param Name>Afroz Khan></param>
        /// <param Created On>05-Sep-2015</param>
        /// <param Topic>Review Estimate</param>
        /// <returns>Action Result</returns>
        /// 
        public JsonResult ApprovePending(string gridData)
        {

            string[] WOIDs = gridData.Split(',');

            List<MercPlusClient.ManageWorkOrderServiceReference.ErrMessage> ErrorList = new List<ManageWorkOrderServiceReference.ErrMessage>();
            string outStatus = string.Empty, outMsg = string.Empty, failedMsg = string.Empty; int index = 0;
            try
            {

                string venRefNo = "";
                string sApproveRemark = "";

                if (((UserSec)Session["UserSec"]).isEMRApproverCountry || ((UserSec)Session["UserSec"]).isEMRApproverShop)
                    sApproveRemark = "Approved by EMR Approver user " + GetUserDtl();
                foreach (string Woid in WOIDs)
                {
                    venRefNo = obj_Sercice.GetVenRefNoByWOID(Convert.ToInt32(Woid));
                    outMsg = obj_Sercice.UpdateApproveWorkOrderByReview(Convert.ToInt32(Woid), GetUserDtl(), sApproveRemark, venRefNo);
                    if (outMsg != "Success")
                    {
                        failedMsg = failedMsg + Woid + ",";
                    }

                }
                if (failedMsg == "" || outMsg == "Success")
                {
                    outMsg = "Success";
                }
                else
                {
                    outMsg = "Not all selected estimates were approved; please try again, or approve them manually.";

                }

            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Update");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Update");
            }
            //model_ReviewEstimate.ErrorMsg = outMsg;
            return Json(outMsg);

        }
        public JsonResult SetWorkingSwitchByID(string gridData, string sSwitch)
        {
            List<MercPlusClient.ManageWorkOrderServiceReference.ErrMessage> ErrorList = new List<ManageWorkOrderServiceReference.ErrMessage>();
            string outStatus = string.Empty, outMsg = string.Empty, failedMsg = string.Empty; int index = 0;
            try
            {
                string[] WOIDs = gridData.Split(',');
                foreach (string Woid in WOIDs)
                {
                    int WID = Convert.ToInt32(Woid);
                    outMsg = obj_Sercice.SetWorkingSwitchByWOIDByReview(WID, sSwitch, GetUserDtl());
                    if (outMsg != "Success")
                    {
                        failedMsg = failedMsg + Woid + ",";
                    }
                }
                if (failedMsg == "" || outMsg == "Success")
                {
                    outMsg = "Success";
                }
                else
                {
                    outMsg = "Not all selected estimates were set to working; please try again, or set them to working manually.";

                }

            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Delete");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Delete");

            }
            //model_ReviewEstimate.ErrorMsg = outMsg;
            return Json(outMsg);
        }
        public JsonResult CompleteApprovedWO(string gridData, short New_Status_Code, short Old_Status_Code, string gridDate)
        {

            string[] WOIDs = gridData.Split(',');
            string[] DateList = gridDate.Split(',');
            List<MercPlusClient.ManageWorkOrderServiceReference.ErrMessage> ErrorList = new List<ManageWorkOrderServiceReference.ErrMessage>();

            string outStatus = string.Empty, outMsg = string.Empty, failedMsg = string.Empty; int index = 0;
            try
            {


                for (int i = 0; i < WOIDs.Count(); i++)
                {

                    outMsg = obj_Sercice.UpdateCompleteApprovedWOByReview(Convert.ToInt32(WOIDs[i]), Convert.ToDateTime(DateList[i]), GetUserDtl());
                    if (outMsg != "Success")
                    {
                        failedMsg = failedMsg + WOIDs[i] + ",";
                    }

                }
                if (failedMsg == "" || outMsg == "Success")
                {
                    outMsg = "Success";
                }
                else
                {
                    outMsg = "Not all selected estimates were completed; please try again, or complete them manually.";

                }

            }
            catch (Exception ex)
            {
                outStatus = Validation.MESSAGETYPE.ERROR.ToString();
                outMsg = ResourceMerc.sERROR + UtilMethods.ReplaceInMsg(ResourceMerc.ACTIONFAILED, "Delete");
                outMsg += "</br>" + UtilMethods.ReplaceInMsg(ResourceMerc.NETWORK_PROBLEM, "Delete");

            }
            //model_ReviewEstimate.ErrorMsg = outMsg;
            return Json(outMsg);
        }

        public ActionResult AuditTrail()
        {
            string[] request = { };
            string RecordId = "";
            if (Request.QueryString["TableName"] == "WorkOrder")
            {
                string parameters = Request.QueryString["val"];
                request = parameters.Split(',');
            }
            else if (Request.QueryString["TableName"] == "Country")
            {
                RecordId = Request.QueryString["val"];
            }
            else if (Request.QueryString["TableName"] == "LabourRates")
            {
                string parameters = Request.QueryString["val"];
                request = parameters.Split(',');
                RecordId = parameters.Split(',')[0];
            }
            else
            {
                RecordId = Request.QueryString["val"];
            }

            List<AuditTrailModel> y = null;

            var qq = y;

            try
            {
                if (Request.QueryString["TableName"] == "WorkOrder")
                {
                    var WOList = obj_Sercice.GetAuditRecord(request[0].ToString());
                    qq = (from e in WOList
                          select new AuditTrailModel
                          {
                              WOID = e.WorkOrderID.ToString(),
                              Description = e.Description,
                              ChangeUser = e.ChangeUser,
                              Timestamp = e.ChangeTime

                          }).ToList();
                }
                else if (Request.QueryString["TableName"] == "Damage" || Request.QueryString["TableName"] == "Repair Codes" || Request.QueryString["TableName"] == "TPI")
                {
                    string tableName = Request.QueryString["TableName"].ToString();
                    if (tableName == "Damage")
                        tableName = "MESC1TS_DAMAGE";
                    else if (tableName == "TPI")
                        tableName = "MESC1TS_TPI";
                    else if (tableName == "Repair Codes")
                        tableName = "MESC1TS_REPAIR_CODE";
                    string uniqueKey = Request.QueryString["val"].ToString();
                    List<AuditTrail> auditRecords = mmdc.GetAuditTrailData(tableName, uniqueKey).ToList();
                    List<AuditTrailModel> models = new List<AuditTrailModel>();
                    foreach (AuditTrail at in auditRecords)
                    {
                        AuditTrailModel model = new AuditTrailModel();
                        model.ChangeTime = at.ChangeTime;
                        model.ChangeUser = at.ChangeUser;
                        model.ColName = at.ColName;
                        model.FirstName = at.FirstName;
                        model.LastName = at.LastName;
                        model.NewValue = at.NewValue;
                        model.OldValue = at.OldValue;
                        models.Add(model);
                    }
                    qq = models;
                }
                else if (Request.QueryString["TableName"] == "MESC1TS_MASTER_PART")
                {
                    var MRec = mmdc.GetAuditTrail_MasterPart(RecordId, "MESC1TS_MASTER_PART");
                    qq = (from e in MRec
                          select new AuditTrailModel
                          {
                              ColName = e.ColName,
                              OldValue = e.OldValue,
                              NewValue = e.NewValue,
                              ChangeUser = e.ChangeUser,
                              LastName = e.LastName,
                              FirstName = e.FirstName,
                              ChangeTime = Convert.ToDateTime(e.ChangeTime).ToString("yyyy-MM-dd hh:mm tt")


                          }).ToList();
                }
                else if (Request.QueryString["TableName"] == "Country")
                {

                    var CountryList = mmdc.GetAuditTrailCountry(RecordId, Request.QueryString["TableName"]);
                    qq = (from e in CountryList
                          select new AuditTrailModel
                          {
                              ColName = e.ColName,
                              OldValue = e.OldValue,
                              NewValue = e.NewValue,
                              ChangeUser = e.ChangeUser,
                              LastName = e.LastName,
                              FirstName = e.FirstName,
                              ChangeTime = Convert.ToDateTime(e.ChangeTime).ToString("yyyy-MM-dd hh:mm tt")

                          }).ToList();
                }
                else if (Request.QueryString["TableName"] == "ManufacturerDiscount")
                {

                    var MDiscount = mmdc.GetAuditTrailMDiscount(RecordId, Request.QueryString["TableName"]);
                    qq = (from e in MDiscount
                          select new AuditTrailModel
                          {
                              ColName = e.ColName,
                              OldValue = e.OldValue,
                              NewValue = e.NewValue,
                              ChangeUser = e.ChangeUser,
                              LastName = e.LastName,
                              FirstName = e.FirstName,
                              ChangeTime = Convert.ToDateTime(e.ChangeTime).ToString("yyyy-MM-dd hh:mm tt")

                          }).ToList();
                }
                else if (Request.QueryString["TableName"] == "LabourRates")
                {
                    /*MISC_RT
                    DOUBLETIME_RT
                    OVERTIME_RT
                    REGULAR_RT
                    Ordinary Rate
                    OT3
                     */

                    var MDiscount = mmdc.GetAuditTrailLabourRate(RecordId, "MESC1TS_LABOR_RATE");
                    qq = (from e in MDiscount
                          select new AuditTrailModel
                          {
                              ColName =
                              (e.ColName == "MISC_RT" ? "OT3" :
                              e.ColName == "DOUBLETIME_RT" ? "OT2" :
                              e.ColName == "OVERTIME_RT" ? "OT1" :
                              e.ColName == "REGULAR_RT" ? "Ordinary Rate" : e.ColName),

                              OldValue = e.OldValue,
                              NewValue = e.NewValue,
                              ChangeUser = e.ChangeUser,
                              LastName = e.LastName,
                              FirstName = e.FirstName,
                              ChangeTime = Convert.ToDateTime(e.ChangeTime).ToString("yyyy-MM-dd hh:mm tt")

                          }).ToList();
                }
                else if (Request.QueryString["TableName"] == "ShopLimits")
                {
                    var ShopList = mmdc.GetAuditTrailShop(RecordId, Request.QueryString["TableName"]);
                    qq = (from e in ShopList
                          select new AuditTrailModel
                          {
                              ColName = (e.ColName == "AUTO_APPROVE_LIMIT" ? "Auto Approval Limit" :
                                          e.ColName == "REPAIR_AMT_LIMIT" ? "Suspend Limit" : "Shop Material Limit"),
                              OldValue = e.OldValue,
                              NewValue = e.NewValue,
                              ChangeUser = e.ChangeUser,
                              LastName = e.LastName,
                              FirstName = e.FirstName,
                              ChangeTime = Convert.ToDateTime(e.ChangeTime).ToString("yyyy-MM-dd hh:mm tt")

                          }).ToList();
                }
                else
                {
                    var ShopList = mmdc.GetAuditTrailShop(RecordId, Request.QueryString["TableName"]);
                    qq = (from e in ShopList
                          select new AuditTrailModel
                          {
                              ColName = e.ColName,
                              OldValue = e.OldValue,
                              NewValue = e.NewValue,
                              ChangeUser = e.ChangeUser,
                              LastName = e.LastName,
                              FirstName = e.FirstName,
                              ChangeTime = Convert.ToDateTime(e.ChangeTime).ToString("yyyy-MM-dd hh:mm tt")

                          }).ToList();
                } 


            }
            catch (Exception ex)
            {
                model_ReviewEstimate.ErrorMsg = ex.Message;
                // Logger.Write("Review Estimate -- ERROR - Search for Work Order." + ex.Message + "");

            }
            //Logger.Write("Review Estimate -- Search for Work Order End.");
            return PartialView("AuditTrail", qq);
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
                    if (obj_Sercice.State != CommunicationState.Faulted)
                    {
                        obj_Sercice.Close();
                    }
                }
                finally
                {
                    if (obj_Sercice.State != CommunicationState.Closed)
                    {
                        obj_Sercice.Abort();
                    }
                }
            }
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~ReviewEstimatesController()
        {
            Dispose(false);
        }

        #endregion

    }
}