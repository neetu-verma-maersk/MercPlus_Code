using MercPlusClient.Areas.ManageSecurity.Models;
using MercPlusClient.ManageUserServiceReference;
using MercPlusClient.UtilityClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace MercPlusClient.Areas.ManageSecurity.Controllers
{
    public class ManageSecurityController : Controller
    {
        ManageSecurityModel objLoginModel = new ManageSecurityModel();
        LogEntry logEntry = new LogEntry();
        public ActionResult Login()
        {
            try
            {
                ViewData["UserRoleList"] = new SelectList(objLoginModel.UserRoleList, "Value", "Text");
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View();
        }
        public ActionResult LogOut()
        {
            try
            {
                Session["UserSec"] = null;
                Session["UserCertId"] = null;
                Session.Clear();
                Session.Abandon();
                //Disable back button In all browsers.
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
                Response.Cache.SetNoStore();
                FormsAuthentication.SignOut();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View();

        }
        public ActionResult SessionExpire()
        {
            try
            {
                Session["UserSec"] = null;
                Session["UserCertId"] = null;
                Session.Clear();
                Session.Abandon();
                FormsAuthentication.SignOut();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View();

        }

        public ActionResult UserInactive()
        {
            try
            {
                Session["UserSec"] = null;
                Session["UserCertId"] = null;
                Session.Clear();
                Session.Abandon();
                FormsAuthentication.SignOut();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View();

        }
        public ActionResult DefaultLogin()
        {
            try
            {
                if (ReadAppSettings.ReadSetting("ServerEnvironment").ToString().ToUpper() == "TEST/DEMO")
                {
                    return RedirectToAction("Login", "ManageSecurity");
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View();
        }
        public ActionResult CertificateLogin()
        {
            try
            {
                GetHomePageForaAuthId("PROD", System.Web.HttpContext.Current.Session["UserCertId"].ToString());
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            if (Session["UserSec"] == null || Session["UserSec"] == "")
            {
                return RedirectToAction("/SessionExpire");
            }
            return Redirect(((UserSec)Session["UserSec"]).HomePage);
        }

        private bool CheckDuplicateEmailIdinfo(string emailid, int userid)
        {
            bool isDuplicate = true;
            ManageUserClient objUserClient1 = new ManageUserClient();
            List<UserInfo> objUserInfo = objUserClient1.GetUserByEmailId(emailid).ToList();

            //if (userid != 0)
            //{
            //    UserInfo userInfoToRemove = objUserInfo.Where(x => x.UserId == userid).FirstOrDefault();
            //    if (userInfoToRemove != null)
            //    {
            //        objUserInfo.Remove(userInfoToRemove);
            //    }
            //}

            if (objUserInfo == null || objUserInfo.Count == 0 || objUserInfo.Count == 1)
            {
                isDuplicate = false;
                  
            }

            foreach (UserInfo userInfo in objUserInfo)
            {
                List<SecAuthGroupUserInfo> objUserAuth = objUserClient1.GetAuthGroupByUserID(userInfo.UserId).ToList();
                if (objUserAuth != null && objUserAuth.Count > 0)
                {
                    string authGroupName = objUserAuth[0].AuthGroupName;
                    if (authGroupName == "SYSTEM ADMINISTRATOR")
                    {
                        isDuplicate = false;
                        break;
                    }
                }
            }

            return isDuplicate;
        }

        [HttpPost]
        public ActionResult GetHomePageForaAuthId(string EnvironMent, string UserLoginId)
        {
            try
            {

                UserSec objUser = new UserSec();
                UserInfo UserList = new UserInfo(); //pinaki

                ManageUserClient objUserClient = new ManageUserClient();
                objUser.SessionId = Session.SessionID;

                objUser.isAdmin = false;
                objUser.isCPH = false;
                objUser.isEMRSpecialistCountry = false;
                objUser.isEMRSpecialistShop = false;
                objUser.isEMRApproverCountry = false;
                objUser.isEMRApproverShop = false;
                objUser.isShop = false;
                objUser.isMPROCluster = false;
                objUser.isMPROShop = false;
                objUser.isReadOnly = false;

                objUser.isAnyCPH = false;
                objUser.isAnyShop = false;

                List<UserInfo> obUserInfo = objUserClient.GetUserByLoginId(UserLoginId).ToList();
                if (obUserInfo.Count() <= 0)
                {
                    logEntry.Message = "ManageSecurity : GetUserByLoginId : No user found with User Id : " + UserLoginId;
                    Logger.Write(logEntry);
                    return SendToExpire(objUser);
                }
                else
                {
                    objUser.UserId = obUserInfo[0].UserId;
                    objUser.LoginId = obUserInfo[0].Login;
                    objUser.ApprovalAmount = obUserInfo[0].ApproveAmount;
                    objUser.UserFirstName = obUserInfo[0].FirstName;
                    objUser.UserLastName = obUserInfo[0].LastName;

                    objUser.LastLogInDateTime = obUserInfo[0].LastLogInDateTime;
                    objUser.IsUserActive = obUserInfo[0].IsUserActive;
                    objUser.EmailId = obUserInfo[0].EmailId;

                    if (objUser.IsUserActive == null || objUser.IsUserActive == false)
                    {
                        return SendToUserInactive(objUser);
                    }
                }
                //Pinaki added
                objUser.DuplicateEmail = false;
                if (ReadAppSettings.ReadSetting("CheckDUPEmail").ToString().ToUpper() == "Y")
                //if (Session["DUPEmail"]=="Y")
                {
                    if (objUser.EmailId != null || objUser.EmailId != string.Empty)
                    {
                        if (CheckDuplicateEmailIdinfo(objUser.EmailId, objUser.UserId) == true)
                        {
                            objUser.DuplicateEmail = true;

                        }

                    }
                }

                List<SecAuthGroupUserInfo> objUserAuth = objUserClient.GetAuthGroupByUserID(obUserInfo[0].UserId).ToList();
                if (objUserAuth == null || objUserAuth.Count <= 0)
                {
                    logEntry.Message = "ManageSecurity : GetAuthGroupByUserID : No SecAuthGroupUserInfo found with user Id : " + obUserInfo[0].UserId;
                    Logger.Write(logEntry);
                    return SendToExpire(objUser);
                }

                switch (objUserAuth[0].TableName)
                {
                    case "MESC1TS_SHOP":
                        objUser.GetUserTable = 1; break;
                    case "MESC1TS_VENDOR":
                        objUser.GetUserTable = 2; break;
                    case "MESC1TS_LOCATION":
                        objUser.GetUserTable = 3; break;
                    case "MESC1TS_REGION":
                        objUser.GetUserTable = 4; break;
                    case "MESC1TS_COUNTRY":
                        objUser.GetUserTable = 5; break;
                    case "MESC1TS_AREA":
                        objUser.GetUserTable = 6; break;
                    default:
                        objUser.GetUserTable = 0; break;
                }
                objUser.AuthGroupName = objUserAuth[0].AuthGroupName;
                objUser.AuthGroupID = objUserAuth[0].AuthGroupId.ToString();
                objUser.WorkOrderStatusDesc = GetWorkOrderStatusDesc();

                if (objUserAuth[0].AuthGroupName == "SYSTEM ADMINISTRATOR")
                {
                    objUser.UserType = UtilMethods.USERROLE.ADMIN.ToString();
                }
                else if (objUserAuth[0].AuthGroupName == "CPH")
                {
                    objUser.UserType = UtilMethods.USERROLE.CPH.ToString();
                }
                else if (objUserAuth[0].AuthGroupName == "EMR_SPECIALIST_COUNTRY")
                {
                    objUser.UserType = UtilMethods.USERROLE.EMR_SPECIALIST_COUNTRY.ToString();
                }
                else if (objUserAuth[0].AuthGroupName == "EMR_SPECIALIST_SHOP")
                {
                    objUser.UserType = UtilMethods.USERROLE.EMR_SPECIALIST_SHOP.ToString();
                }
                else if (objUserAuth[0].AuthGroupName == "EMR_APPROVER_COUNTRY")
                {
                    objUser.UserType = UtilMethods.USERROLE.EMR_APPROVER_COUNTRY.ToString();
                }
                else if (objUserAuth[0].AuthGroupName == "EMR_APPROVER_SHOP")
                {
                    objUser.UserType = UtilMethods.USERROLE.EMR_APPROVER_SHOP.ToString();
                }
                else if (objUserAuth[0].AuthGroupName == "SHOP")
                {
                    objUser.UserType = UtilMethods.USERROLE.SHOP.ToString();
                }
                else if (objUserAuth[0].AuthGroupName == "MPRO_CLUSTER")
                {
                    objUser.UserType = UtilMethods.USERROLE.MPRO_CLUSTER.ToString();
                }
                else if (objUserAuth[0].AuthGroupName == "MPRO_SHOP")
                {
                    objUser.UserType = UtilMethods.USERROLE.MPRO_SHOP.ToString();
                }
                else if (objUserAuth[0].AuthGroupName == "READ_ONLY")
                {
                    objUser.UserType = UtilMethods.USERROLE.READONLY.ToString();
                }

                if (objUser.UserType == UtilMethods.USERROLE.ADMIN.ToString()
                   || objUser.UserType == UtilMethods.USERROLE.CPH.ToString()
                   || objUser.UserType == UtilMethods.USERROLE.EMR_SPECIALIST_COUNTRY.ToString()
                   || objUser.UserType == UtilMethods.USERROLE.EMR_SPECIALIST_SHOP.ToString()
                   || objUser.UserType == UtilMethods.USERROLE.EMR_APPROVER_COUNTRY.ToString()
                   || objUser.UserType == UtilMethods.USERROLE.EMR_APPROVER_SHOP.ToString()
                   || objUser.UserType == UtilMethods.USERROLE.SHOP.ToString()
                   || objUser.UserType == UtilMethods.USERROLE.MPRO_CLUSTER.ToString()
                   || objUser.UserType == UtilMethods.USERROLE.MPRO_SHOP.ToString()
                   || objUser.UserType == UtilMethods.USERROLE.READONLY.ToString()
                   )
                {
                    UpdateLastLogInDateTime(obUserInfo[0]);
                }

                if (objUser.UserType == UtilMethods.USERROLE.ADMIN.ToString())
                {
                    objUser.isAdmin = true;
                    objUser.HomePage = Url.Action("TablemodLaunch", "ManageSecurity", new { area = "ManageSecurity" });
                    Session["UserSec"] = objUser;
                    return Json(new { redirectUrl = Url.Action("TablemodLaunch", "ManageSecurity", new { area = "ManageSecurity" }), isRedirect = true });
                }
                else if (objUser.UserType == UtilMethods.USERROLE.CPH.ToString())
                {
                    objUser.isCPH = true;
                    objUser.isAnyCPH = true;
                    objUser.HomePage = Url.Action("TablemodLaunch", "ManageSecurity", new { area = "ManageSecurity" });
                    Session["UserSec"] = objUser;
                    return Json(new { redirectUrl = Url.Action("TablemodLaunch", "ManageSecurity", new { area = "ManageSecurity" }), isRedirect = true });
                }
                else if (objUser.UserType == UtilMethods.USERROLE.EMR_SPECIALIST_COUNTRY.ToString())
                {
                    objUser.isEMRSpecialistCountry = true;
                    objUser.isAnyCPH = true;
                    objUser.HomePage = Url.Action("TablemodLaunch", "ManageSecurity", new { area = "ManageSecurity" });
                    Session["UserSec"] = objUser;
                    return Json(new { redirectUrl = Url.Action("TablemodLaunch", "ManageSecurity", new { area = "ManageSecurity" }), isRedirect = true });
                }
                else if (objUser.UserType == UtilMethods.USERROLE.EMR_SPECIALIST_SHOP.ToString())
                {
                    objUser.isEMRSpecialistShop = true;
                    objUser.isAnyCPH = true;
                    objUser.HomePage = Url.Action("TablemodLaunch", "ManageSecurity", new { area = "ManageSecurity" });
                    Session["UserSec"] = objUser;
                    return Json(new { redirectUrl = Url.Action("TablemodLaunch", "ManageSecurity", new { area = "ManageSecurity" }), isRedirect = true });
                }
                else if (objUser.UserType == UtilMethods.USERROLE.EMR_APPROVER_COUNTRY.ToString())
                {
                    objUser.isEMRApproverCountry = true;
                    objUser.HomePage = Url.Action("TablemodLaunch", "ManageSecurity", new { area = "ManageSecurity" });
                    Session["UserSec"] = objUser;
                    return Json(new { redirectUrl = Url.Action("TablemodLaunch", "ManageSecurity", new { area = "ManageSecurity" }), isRedirect = true });
                }
                else if (objUser.UserType == UtilMethods.USERROLE.EMR_APPROVER_SHOP.ToString())
                {
                    objUser.isEMRApproverShop = true;
                    objUser.HomePage = Url.Action("TablemodLaunch", "ManageSecurity", new { area = "ManageSecurity" });
                    Session["UserSec"] = objUser;
                    return Json(new { redirectUrl = Url.Action("TablemodLaunch", "ManageSecurity", new { area = "ManageSecurity" }), isRedirect = true });
                }
                else if (objUser.UserType == UtilMethods.USERROLE.SHOP.ToString())
                {
                    objUser.isShop = true;
                    objUser.isAnyShop = true;
                    objUser.HomePage = Url.Action("ManageWorkOrder", "ManageWorkOrder", new { area = "ManageWorkOrder", IsMulti = false });
                    Session["UserSec"] = objUser;
                    return Json(new { redirectUrl = Url.Action("ManageWorkOrder", "ManageWorkOrder", new { area = "ManageWorkOrder", IsMulti = false }), isRedirect = true });
                }
                else if (objUser.UserType == UtilMethods.USERROLE.MPRO_CLUSTER.ToString())
                {
                    //objUser.isMPROCluster = true;

                    //objUser.HomePage = Url.Action("ManageReports", "ManageReports", new { area = "ManageReports", IsMulti = false });
                    //Session["UserSec"] = objUser;
                    //return Json(new { redirectUrl = Url.Action("ManageReports", "ManageReports", new { area = "ManageReports" }), isRedirect = true });

                    objUser.isMPROCluster = true;
                    //objUser.isReadOnly = true;
                    objUser.HomePage = Url.Action("ReviewEstimates", "ReviewEstimates", new { area = "ManageWorkOrder", IsMulti = false });
                    Session["UserSec"] = objUser;
                    return Json(new { redirectUrl = Url.Action("ReviewEstimates", "ReviewEstimates", new { area = "ManageWorkOrder" }), isRedirect = true });
                }
                else if (objUser.UserType == UtilMethods.USERROLE.MPRO_SHOP.ToString())
                {
                    objUser.isMPROShop = true;
                    //objUserisReadOnly = true;.
                    //objUser.HomePage = Url.Action("ManageReports", "ManageReports", new { area = "ManageReports", IsMulti = false });
                    //Session["UserSec"] = objUser;
                    //return Json(new { redirectUrl = Url.Action("ManageReports", "ManageReports", new { area = "ManageReports" }), isRedirect = true });

                    objUser.HomePage = Url.Action("ReviewEstimates", "ReviewEstimates", new { area = "ManageWorkOrder", IsMulti = false });
                    Session["UserSec"] = objUser;
                    return Json(new { redirectUrl = Url.Action("ReviewEstimates", "ReviewEstimates", new { area = "ManageWorkOrder" }), isRedirect = true });
                }
                else if (objUser.UserType == UtilMethods.USERROLE.READONLY.ToString())
                {
                    objUser.isReadOnly = true;
                    objUser.HomePage = Url.Action("ReviewEstimates", "ReviewEstimates", new { area = "ManageWorkOrder", IsMulti = false });
                    Session["UserSec"] = objUser;
                    return Json(new { redirectUrl = Url.Action("ReviewEstimates", "ReviewEstimates", new { area = "ManageWorkOrder" }), isRedirect = true });
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return Json("");
            }
        }


        private void UpdateLastLogInDateTime(UserInfo userinfo)
        {
            try
            {
                ManageUserClient objUserClient = new ManageUserClient();
                string Message = "";

                userinfo.LastLogInDateTime = DateTime.UtcNow;
                objUserClient.UpdateUser(out Message, userinfo);
            }
            catch (Exception ex)
            {

            }
        }

        private ActionResult SendToExpire(UserSec objUser)
        {
            objUser.HomePage = Url.Action("SessionExpire", "ManageSecurity", new { area = "ManageSecurity" });
            Session["UserSec"] = objUser;
            return Json(new { redirectUrl = Url.Action("SessionExpire", "ManageSecurity", new { area = "ManageSecurity" }), isRedirect = true });
        }

        private ActionResult SendToUserInactive(UserSec objUser)
        {
            objUser.HomePage = Url.Action("UserInactive", "ManageSecurity", new { area = "ManageSecurity" });
            Session["UserSec"] = objUser;
            return Json(new { redirectUrl = Url.Action("UserInactive", "ManageSecurity", new { area = "ManageSecurity" }), isRedirect = true });
        }


        public ActionResult TablemodLaunch()
        {
            try
            {
                if (Session["UserSec"] == null || Session["UserSec"] == "")
                {
                    return RedirectToAction("/SessionExpire");
                }

                string usertype = ((UserSec)Session["UserSec"]).UserType;


                // System.Web.HttpContext.Current.Session["UserType"].ToString();
                if (((UserSec)Session["UserSec"]).UserType != null && ((UserSec)Session["UserSec"]).UserType != "")
                {
                    string xmlPath = HttpContext.Server.MapPath("~/Content/LoginXml/ConfigureLoginUI.xml");
                    DataTable dtUserTable = new DataTable();
                    dtUserTable = UtilityClass.UtilMethods.BuildDataTableFromXml("UserTable", xmlPath);

                    DataTable dtFilteredByUserType = dtUserTable.Clone();
                    String Utype = string.Empty;
                    if (((UserSec)Session["UserSec"]).UserType == "SHOP")
                        Utype = "SHOPS";
                    else
                        Utype = ((UserSec)Session["UserSec"]).UserType;
                    //get only the rows you want
                    //DataRow[] results = dtUserTable.Select("ROLES LIKE '%" + ((UserSec)Session["UserSec"]).UserType + "%' AND ACCESS LIKE '%" + ((UserSec)Session["UserSec"]).UserType + "%'", "ID");
                    DataRow[] results = dtUserTable.Select("ROLES LIKE '%" + Utype + "%' AND ACCESS LIKE '%" + ((UserSec)Session["UserSec"]).UserType + "%'", "ID");

                    //populate new destination table
                    foreach (DataRow dr in results)
                        dtFilteredByUserType.ImportRow(dr);

                    int RowLimit = 15, tempCount = 0;
                    DataTable dtUniqRecords = new DataTable();

                    dtUniqRecords = dtFilteredByUserType.DefaultView.ToTable(true, "ITEM");
                    dtUniqRecords.DefaultView.Sort = "ITEM";
                    StringBuilder strUserMenu = new StringBuilder();
                    strUserMenu.AppendLine("<table border=\"0\"><tr><td valign=\"top\">");
                    for (int iCtr = 0; iCtr < dtUniqRecords.Rows.Count; iCtr++)
                    {
                        strUserMenu.AppendLine("<b><font color=\"blue\">" + dtUniqRecords.Rows[iCtr]["ITEM"].ToString() + "</font></b><br>");
                        tempCount = tempCount + 1;
                        for (int jCtr = 0; jCtr < dtFilteredByUserType.Rows.Count; jCtr++)
                        {
                            if (dtUniqRecords.Rows[iCtr]["ITEM"].ToString().Trim() == dtFilteredByUserType.Rows[jCtr]["ITEM"].ToString().Trim())
                            {
                                tempCount = tempCount + 1;

                                string actionmethod = string.Empty;
                                string[] accesslevels = dtFilteredByUserType.Rows[jCtr]["ACCESS"].ToString().Split(new char[] { ',' });
                                string accesslevel = accesslevels.First(x => x.Contains(((UserSec)Session["UserSec"]).UserType));
                                string useraccess = accesslevel.Substring(accesslevel.IndexOf("-") + 1).ToUpper() == "VIEW" ? "_View" : string.Empty;

                                strUserMenu.AppendLine("<img src=\"/Images/i.gif\"  width=\"10\">&nbsp;&nbsp;<a href=\"/" + dtFilteredByUserType.Rows[jCtr]["CONTROLLER"].ToString() + "/"
                                    + dtFilteredByUserType.Rows[jCtr]["ACTION"].ToString() + useraccess
                                    + "\">" + dtFilteredByUserType.Rows[jCtr]["TEXT"].ToString() + "</a><br>");
                            }
                        }
                        strUserMenu.AppendLine("<br>");
                        if (tempCount >= RowLimit)
                        {
                            tempCount = 0;
                            strUserMenu.AppendLine("</td><td><img src=\"/Images/i.gif\" width=\"10\" height=\"1\"></td><td valign=\"top\">");
                        }
                    }
                    strUserMenu.AppendLine("</td></tr></table>");


                    // TempData["UserId"] = "User Id is :" + System.Web.HttpContext.Current.Session["UserCertId"].ToString();
                    TempData["TableModeData"] = strUserMenu.ToString();
                }
                else
                {
                    TempData["TableModeData"] = "<br><br><b>The URL that you used may be incorrect.</b><br><b>Try using: <a href='/ManageSecurity/ManageSecurity/DefaultLogin'>" + HttpContext.Request.Url + "</a></b><br>";
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                TempData["TableModeData"] = "<br><br><b>The URL that you used may be incorrect.</b><br><b>Try using: <a href='/ManageSecurity/ManageSecurity/DefaultLogin'>" + HttpContext.Request.Url.AbsoluteUri + "</a></b><br>";
            }

            return View();
        }

        private Dictionary<string, string> GetWorkOrderStatusDesc()
        {
            Dictionary<string, string> CodeDescPair = new Dictionary<string, string>();

            CodeDescPair.Add("0", "Draft");
            CodeDescPair.Add("000", "Draft");
            CodeDescPair.Add("100", "Rejected");
            // CodeDescPair.Add("130", "Rejected for review by MSL");
            CodeDescPair.Add("130", "Rejected to Approver");
            //CodeDescPair.Add("130", "Rejected");
            //' Rejected to CPH
            //  CodeDescPair.Add("140", "Rejected for review by EMR mng");
            CodeDescPair.Add("140", "Rejected to Specialist");
            //'RQ7407
            //' Total Loss
            CodeDescPair.Add("150", "Total Loss");
            //'End
            //' Pending MSL Approval
            CodeDescPair.Add("200", "Pending Approver Approval");
            //' Suspended to MSL
            // CodeDescPair.Add("310", "Suspended for review by MSL");
            CodeDescPair.Add("310", "Suspended to Approver");
            //' Suspended to MSL/CPH
            // CodeDescPair.Add("320", "Suspended for review by MSL final forwarding to EMR mng");
            CodeDescPair.Add("320", "Suspended to Approver  then forward to Specialist");
            //' Suspended to CPH	
            //CodeDescPair.Add("330", "Suspended for review by EMR mng");
            CodeDescPair.Add("330", "Suspend to EMR Specialist");

            //' Suspended to CENEQULOS
            //CodeDescPair.Add("340", "Suspended for review by CENEQULOS");	//'CENEQUSAL changed to CENEQULOS for FP7409
            CodeDescPair.Add("340", "Suspend to CENEQULOS /CPH");
            //' SHOP  --------------------------------------------
            CodeDescPair.Add("s130", "Submitted");
            CodeDescPair.Add("s140", "Submitted");
            CodeDescPair.Add("s130,140", "Submitted");
            //' Pending MSL Approval
            CodeDescPair.Add("s200", "Submitted");
            //' Suspended to MSL
            CodeDescPair.Add("s310", "Submitted");
            //' Suspended to MSL/CPH
            CodeDescPair.Add("s320", "Submitted");
            //' Suspended to CPH	
            CodeDescPair.Add("s330", "Submitted");
            //' Suspended to CENEQULOS	//'CENEQUSAL changed to CENEQULOS for FP7409
            CodeDescPair.Add("s340", "Submitted");

            //' Approved Estimate 
            CodeDescPair.Add("390", "Approved Estimate");
            CodeDescPair.Add("-390", "Working");

            //' VJP FP Ticket 4004	
            CodeDescPair.Add("400,500,550,600,900", "Completed");

            CodeDescPair.Add("400", "Completed");
            CodeDescPair.Add("500", "RRIS Transmitted");
            CodeDescPair.Add("550", "RRIS Rejected");
            CodeDescPair.Add("600", "RRIS Accepted");
            CodeDescPair.Add("800", "Paid");
            CodeDescPair.Add("900", "Processed");
            CodeDescPair.Add("9999", "Deleted");

            CodeDescPair.Add("130,140,200,310,320,330,340", "Submitted");
            CodeDescPair.Add("130,200,310,320", "Daily");
            CodeDescPair.Add("000,100,390", "Daily");
            CodeDescPair.Add("130,140", "Rejected");
            CodeDescPair.Add("100,130,140", "Rejected");
            CodeDescPair.Add("100,130", "Rejected");
            CodeDescPair.Add("310,320,330,340", "Suspended");
            CodeDescPair.Add("310,320,340", "Suspended");
            CodeDescPair.Add("330,340", "Suspended");
            CodeDescPair.Add("310,320", "Suspended");
            CodeDescPair.Add("330,340,-1", "ToCPH");

            //' //'All//' Query Type selection:
            //'RQ7407 ( 150 has been added to AllCPH, AllArea, AllAdminCountry,AllShopVendor );
            CodeDescPair.Add("All,100,130,140,150,200,310,320,330,340,390,400,500,550,600,900,9999,-9000", "AllCPH");
            CodeDescPair.Add("All,100,130,140,150,200,310,320,330,340,390,400,500,550,600,900,9999,-9001", "AllArea");
            CodeDescPair.Add("All,100,130,140,150,200,310,320,330,340,390,400,500,550,600,900,9999", "AllAdminCountry");
            CodeDescPair.Add("All,100,130,140,200,310,320,330,340,390,400,500,550,600,900,9999", "AllMSLRegion");
            CodeDescPair.Add("All,000,100,130,140,150,200,310,320,330,340,390,400,500,550,600,900,9999", "AllShopVendor");
            CodeDescPair.Add("All,000,100,130,140,200,310,320,330,340,390,400,500,550,600,900,9999", "AllThirdParty");

            return CodeDescPair;
        }
    }
}
