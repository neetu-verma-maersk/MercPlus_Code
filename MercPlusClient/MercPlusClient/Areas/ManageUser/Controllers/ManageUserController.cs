using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MercPlusClient.ManageUserServiceReference;
using MercPlusClient.Areas.ManageUser.Models;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging;
namespace MercPlusClient.Areas.ManageUser.Controllers
{
    public class ManageUserController : Controller
    {       
        ManageUserClient objUserClient = new ManageUserClient();
        ManageUserModel objUserModel = new ManageUserModel();
        LogEntry logEntry = new LogEntry();
        public ActionResult ManageUser()
        {
            return View();
        }
        #region Create New User
        public ActionResult SecNewUser()
        {            
            return View();
        }

        [HttpPost]
        public ActionResult SecNewUser(ManageUserModel objUserModel, string Command)
        {
            try
            {
                if (Command == "Add New")
                {
                    if (ModelState.IsValid)
                    {
                        bool isSuccess = false;
                        string Message = "";
                        UserInfo UserDetails = new UserInfo();
                        UserDetails.Login = objUserModel.Login;
                        UserDetails.FirstName = objUserModel.FirstName;
                        UserDetails.LastName = objUserModel.LastName;
                        UserDetails.Company = objUserModel.Company;
                        UserDetails.Loccd = objUserModel.Loccd;
                        UserDetails.ActiveStatus = objUserModel.ActiveStatus;
                        UserDetails.ApproveAmount = objUserModel.ApproveAmount;
                        UserDetails.EmailId = objUserModel.EmailId;
                        UserDetails.IsUserActive = (objUserModel.Expired != null && objUserModel.Expired.ToUpper() == "Y") ? false : true;
                        if (!CheckDuplicateEmailId(objUserModel.EmailId, 0))
                        {
                            isSuccess = objUserClient.AddUser(out Message, UserDetails);
                            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(Message, (isSuccess == true ? "Success" : "Warning"));
                        }
                        else
                        {                            
                            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Email id exists", "Warning");
                        }
                        
                        if (isSuccess == true)
                        {
                            objUserModel.isConfirmUpdate = true;
                        }
                    }
                    return View(objUserModel);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return View();
            }
        }
        #endregion
        [HttpPost]
        public ActionResult SecUserEdit(ManageUserModel objUserModel, string Command)
        {
            try
            {
                string Message = "";

                
                if (Command == "Delete")
                {
                    List<UserInfo> objUserInfo = objUserClient.GetUserByUserId(objUserModel.UserId).ToList();
                    if (objUserInfo.Count > 0)
                    {
                        objUserModel.Login = objUserInfo[0].Login.ToString();
                        objUserModel.FirstName = objUserInfo[0].FirstName.ToString();
                        objUserModel.LastName = objUserInfo[0].LastName.ToString();
                        objUserModel.Company = objUserInfo[0].Company.ToString();
                        objUserModel.Loccd = objUserInfo[0].Loccd.ToString();
                        objUserModel.ActiveStatus = (objUserInfo[0].ActiveStatus != null ? objUserInfo[0].ActiveStatus.ToString() : "N");     
                        objUserModel.ApproveAmount = Convert.ToDecimal(objUserInfo[0].ApproveAmount);
                        objUserModel.EmailId = objUserInfo[0].EmailId;
                        objUserModel.Expired = (objUserInfo[0].IsUserActive != null && objUserInfo[0].IsUserActive == true) ? "N" : "Y";
                    }
                    objUserModel.isCountrySelection = false;
                    objUserModel.isConfirmUpdate = false;
                    objUserModel.isDeleteMode = true;

                    return View(objUserModel);
                }
                else if (Command == "Update")
                {
                    if (ModelState.IsValid)
                    {
                        bool isSuccess = false;
                        UserInfo UserDetails = new UserInfo();
                        UserDetails.UserId = objUserModel.UserId;
                        UserDetails.Login = objUserModel.Login;
                        UserDetails.FirstName = objUserModel.FirstName;
                        UserDetails.LastName = objUserModel.LastName;
                        UserDetails.Company = objUserModel.Company;
                        UserDetails.Loccd = objUserModel.Loccd;
                        UserDetails.ActiveStatus = objUserModel.ActiveStatus;
                        UserDetails.ApproveAmount = objUserModel.ApproveAmount;
                        UserDetails.EmailId = objUserModel.EmailId;
                        UserDetails.IsUserActive = (objUserModel.Expired != null && objUserModel.Expired.ToUpper() == "Y") ? false : true;
                        UserDetails.ChangeUser = ((UserSec)Session["UserSec"]).LoginId.ToString();
                        UserDetails.ChangeTime = DateTime.Now;

                        if (!CheckDuplicateEmailId(objUserModel.EmailId, UserDetails.UserId))
                        {
                            isSuccess = objUserClient.UpdateUser(out Message, UserDetails);
                            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(Message, (isSuccess == true ? "Success" : "Warning"));
                        }
                        else
                        {
                            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Email id exists", "Warning");
                        }

                        if (isSuccess == true)
                        {
                            objUserModel.isConfirmUpdate = true;
                        }
                        return View(objUserModel);
                    }
                    else
                    {
                        TempData["Msg"] = "Some error has occured ";
                        return RedirectToAction("SecUserEdit");
                    }

                }
                else if (Command == "Confirm Delete")
                {
                    bool isSuccess = false;
                    isSuccess = objUserClient.DeleteUser(out Message, objUserModel.UserId);
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(Message, (isSuccess == true ? "Success" : "Warning"));

                    objUserModel.isConfirmUpdate = true;
                    return View(objUserModel);
                }                
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return View();
            }
        }
        public ActionResult SecUserEdit()
        {
            try
            {
                objUserModel.isCountrySelection = true;
                List<Country> CountryDetails = objUserClient.GetCountryList().ToList();
                if (CountryDetails.Count > 0)
                {
                    foreach (var row in CountryDetails)
                    {
                        objUserModel.CountryList.Add(new SelectListItem
                        {
                            Text = row.CountryCodeAndDescription,
                            Value = row.CountryCode
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View(objUserModel);
        }
        public ActionResult UserSelectionBasedOnCountry()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetUserListOfACountry(string CountryCode)
        {
            try
            {
                List<UserInfo> UserDetails = objUserClient.GetUserListOfACountry(CountryCode).ToList();

                if (((UserSec)Session["UserSec"]).UserType == "CPH") // "SYSTEM ADMINISTRATOR"
                {
                    for (int i = UserDetails.Count - 1; i >= 0; i--)
                    {
                        if (UserDetails[i].UserId == ((UserSec)Session["UserSec"]).UserId)
                        {
                            UserDetails.RemoveAt(i);
                        }
                        if (UserDetails[i].AuthGroupName == "SYSTEM ADMINISTRATOR")
                        {
                            UserDetails.RemoveAt(i);
                        }
                    }
                }
                else
                    if (((UserSec)Session["UserSec"]).UserType == "AREA") // "SYSTEM ADMINISTRATOR"
                    {
                        for (int i = UserDetails.Count - 1; i >= 0; i--)
                        {
                            if (UserDetails[i].UserId == ((UserSec)Session["UserSec"]).UserId)
                            {
                                UserDetails.RemoveAt(i);
                            }
                            if (UserDetails[i].AuthGroupName == "SYSTEM ADMINISTRATOR" || UserDetails[i].AuthGroupName == "CPH")
                            {
                                UserDetails.RemoveAt(i);
                            }
                        }
                    }

                ViewBag.UserList = new SelectList(UserDetails, "UserId", "LoginFirstAndLastName");
                return Json(ViewBag.UserList);
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return Json("");
            }
        }
        [HttpPost]
        public JsonResult GetUserDetailsOfaUserId(int UserId)
        {
            try
            {
                List<UserInfo> objUserInfo = objUserClient.GetUserByUserId(UserId).ToList();
                if (objUserInfo.Count > 0)
                {
                    objUserModel.Login = objUserInfo[0].Login.ToString();
                    objUserModel.FirstName = objUserInfo[0].FirstName.ToString();
                    objUserModel.LastName = objUserInfo[0].LastName.ToString();
                    objUserModel.Company = objUserInfo[0].Company.ToString();
                    objUserModel.Loccd = objUserInfo[0].Loccd.ToString();
                    objUserModel.ActiveStatus = (objUserInfo[0].ActiveStatus != null ? objUserInfo[0].ActiveStatus.ToString() : "N");
                    objUserModel.ApproveAmount = Convert.ToDecimal(objUserInfo[0].ApproveAmount);
                    objUserModel.EmailId = objUserInfo[0].EmailId;
                    objUserModel.Expired = (objUserInfo[0].IsUserActive != null && objUserInfo[0].IsUserActive == true) ? "N" : "Y";
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Json(objUserModel);
        }

        public ActionResult SecSetUserPermissions()
        {
            try
            {
                List<Country> CountryDetails = objUserClient.GetCountryList().ToList();
                if (CountryDetails.Count > 0)
                {
                    foreach (var row in CountryDetails)
                    {
                        objUserModel.CountryList.Add(new SelectListItem
                        {
                            Text = row.CountryCodeAndDescription,
                            Value = row.CountryCode
                        });
                    }
                }
                List<SecAuthGroup> lstSecAuthGroup = objUserClient.GetAuthGroupList().ToList();
                //  ViewBag.lstSecAuthGroup = new SelectList(lstSecAuthGroup, "AuthGroupId", "AuthGroupName");

                string strTmpUserAuthgroup = string.Empty;
                if (lstSecAuthGroup.Count > 0)
                {
                    foreach (var row in lstSecAuthGroup)
                    {
                        strTmpUserAuthgroup = row.AuthGroupName;
                        if (strTmpUserAuthgroup.ToUpper() == "AREA")
                        {
                            strTmpUserAuthgroup = "COUNTRY CLUSTER";
                        }
                        if (strTmpUserAuthgroup.ToUpper() != "3RD PARTY INPUT")
                        {
                            objUserModel.SecAuthGroupList.Add(new SelectListItem
                            {
                                Text = strTmpUserAuthgroup,
                                Value = row.AuthGroupId.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View(objUserModel);
        }

        [HttpPost]
        public JsonResult SecSetUserPermissions(string SelectedActivePermission, string SelectedAvailablePermission, int UserId, int AuthorisationGroupId)
        {
            bool isSuccess = false;
            string Message = "";
            try
            {
                isSuccess = objUserClient.InsertUserDataAccess(out  Message, SelectedActivePermission, UserId, AuthorisationGroupId);
                objUserModel.strMessage = Message;
            }
            catch (Exception ex)
            {
                objUserModel.strMessage = "Some Error has occurred while performing your activity. Please contact System Administrator ";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Json(objUserModel);

        }

        [HttpPost]
        public JsonResult AddAllCluster( int UserId, int AuthorisationGroupId)
        {
          
            string Message = "";
            try
            {               
                objUserModel.isSuccess = objUserClient.AddAllCluster(out  Message, UserId, AuthorisationGroupId);
                objUserModel.strMessage = Message;
            }
            catch (Exception ex)
            {
                objUserModel.isSuccess = false;
                objUserModel.strMessage = "Some Error has occurred while performing your activity. Please contact System Administrator ";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Json(objUserModel);
        }
         [HttpPost]
        public JsonResult IsLocationCodeExist(string LocCode)
        {
            try
            {
                objUserModel.isLocationExist = objUserClient.IsLocationCodeExist(LocCode);
                objUserModel.strMessage = "";
            }
            catch (Exception ex)
            {
                objUserModel.isSuccess = false;
                objUserModel.strMessage = "Some Error has occurred while performing your activity. Please contact System Administrator ";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Json(objUserModel);     
        }
        


        [HttpPost]
        public JsonResult DeleteUserDataAccessByUserId(int UserId)
        {
            
            string Message = "";
            try
            {
                objUserModel.isSuccess  = objUserClient.DeleteUserDataAccessByUserId(out  Message, UserId);
                objUserModel.strMessage = Message;
            }
            catch(Exception ex){
                objUserModel.isSuccess = false;
                objUserModel.strMessage = "Some Error has occurred while performing your activity. Please contact System Administrator ";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Json(objUserModel);
        }

        //[HttpPost]
        //public JsonResult UpdateWebsiteAccess(string SelectedWebSitePermission, int UserId, int AuthorisationGroupId)
        //{
        //    bool isSuccess = false;
        //    string Message = "";
        //    isSuccess = objUserClient.UpdateWebSitePermissions(out  Message, SelectedWebSitePermission, AuthorisationGroupId);
        ////    TempData["Msg"] = Message;
        //    return Json("");
        //}

        [HttpPost]
        public JsonResult UpdateWebPageAccess(string SelectedWebSitePermission, int AuthorisationGroupId, int WebSiteId)
        {
            
            string Message = "";
           
            try
            {
                 objUserModel.isSuccess = objUserClient.UpdateWebPagePermissions(out  Message, SelectedWebSitePermission, WebSiteId, AuthorisationGroupId);
               
                objUserModel.strMessage = Message;
            }
            catch (Exception ex)
            {
                objUserModel.isSuccess = false;
                objUserModel.strMessage = "Some Error has occurred while performing your activity. Please contact System Administrator ";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(objUserModel);          
        }

        //[HttpPost]
        //public JsonResult GetWebsiteUserPermission(int AuthGroupId)
        //{
        ////    List<SelectListItem> AssignedSecWebSitePermission = new List<SelectListItem>();


        //     List<SecWebSite> objGetWebPageList = objUserClient.GetWebPageList().ToList();
        //     if (objGetWebPageList.Count > 0)
        //    {
        //        foreach (var row in objGetWebPageList)
        //        {
        //            objUserModel.ActiveSecWebSitePermission.Add(new SelectListItem
        //            {
        //                Text = row.WebSiteName,
        //                Value = row.WebSiteId.ToString()
        //            });
        //        }
        //    }
        //     List<SecWebSite> objGetAssignedWebPageList = objUserClient.GetAuthGroupWebsiteAccessByAuthGroupId(AuthGroupId).ToList();
             
        //           if (objGetAssignedWebPageList.Count > 0)
        //    {
        //        foreach (var row in objGetAssignedWebPageList)
        //        {
        //            objUserModel.AssignedSecWebSitePermission.Add(new SelectListItem
        //            {
        //                Text = row.WebSiteName,
        //                Value = row.WebSiteId.ToString()
        //            });
        //        }
        //    }
        //    return Json(objUserModel);
        //}
        [HttpPost]
        public JsonResult GetWebsitePageLevelUserPermission(int websiteid, int AuthGroupId)
        {
            try
            {
                List<SecWebPage> objGetWebPageListById = objUserClient.GetAuthGroupWebpageAccessById(websiteid, AuthGroupId).ToList();
                return Json(objGetWebPageListById);
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return Json("");
            }
            
        }
        [HttpPost]
        public JsonResult GetAuthGroupByUserID(int UserId)
        {
            try
            {
                //  ManageUserModel objUserModel = new ManageUserModel();
                StringBuilder strAuthGroup = new StringBuilder();
                objUserModel.AuthGroupId = 0;
                objUserModel.AuthGroupName = "";
                List<SecAuthGroupUserInfo> objAuthGroupUserInfo = objUserClient.GetAuthGroupByUserID(UserId).ToList();
                if (objAuthGroupUserInfo.Count > 0)
                {
                    foreach (var item in objAuthGroupUserInfo)
                    {
                        strAuthGroup.Append(" " + item.ColumnValue + " ");
                    }
                    objUserModel.AuthGroupId = objAuthGroupUserInfo[0].AuthGroupId;
                    objUserModel.AuthGroupName = (objAuthGroupUserInfo[0].AuthGroupName == "AREA" ? "COUNTRY CLUSTER" : objAuthGroupUserInfo[0].AuthGroupName);

                    objUserModel.isAdminUserSelected = (objUserModel.AuthGroupName == "SYSTEM ADMINISTRATOR" ? true : false);
                    objUserModel.isCPHUserSelected = (objUserModel.AuthGroupName == "CPH" ? true : false);
                }
                else
                {
                    strAuthGroup.Append("This user is not yet associated with an Authorisation Group or Data Access Code: ");
                }
                objUserModel.strHtml = strAuthGroup.ToString();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Json(objUserModel);
        }

        [HttpPost]
        public JsonResult GetRSAvailablePermissionsOfaAuthGroupId(int UserId, int AuthorisationGroupId, string PermissionsPrefix)
        {
            try
            {
                int AvailablePermissionCount = 0;
                List<AvailableAssignAuthGroup> objAvailablePermissionsByFilter = objUserClient.AvailablePermissionsByFilter(out AvailablePermissionCount, UserId, AuthorisationGroupId, PermissionsPrefix).ToList();
                if (objAvailablePermissionsByFilter.Count > 0)
                {
                    foreach (var row in objAvailablePermissionsByFilter)
                    {
                        objUserModel.AvailablePermission.Add(new SelectListItem
                        {
                            Text = row.ListItem,
                            Value = row.ValueItem
                        });
                    }
                    objUserModel.AvailablePermissionCount = AvailablePermissionCount;
                }
                List<AssignAuthGroup> objAvailablePermissions = objUserClient.AvailablePermissions(UserId, AuthorisationGroupId).ToList();
                if (objAvailablePermissions.Count > 0)
                {
                    foreach (var row in objAvailablePermissions)
                    {
                        objUserModel.ActivePermission.Add(new SelectListItem
                        {
                            Text = row.ListItem,
                            Value = row.ValueItem
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Json(objUserModel);
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session.IsNewSession ||  Session["UserSec"] == null )            
            {
                filterContext.Result = new RedirectResult("/ManageSecurity/ManageSecurity/SessionExpire");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        private bool CheckDuplicateEmailId(string emailid, int userid)
        {
            bool isDuplicate = true;

            List<UserInfo> objUserInfo = objUserClient.GetUserByEmailId(emailid).ToList();

            if (userid != 0)
            {
                UserInfo userInfoToRemove = objUserInfo.Where(x => x.UserId == userid).FirstOrDefault();
                if (userInfoToRemove != null)
                {
                    objUserInfo.Remove(userInfoToRemove);
                }
            }

            if (objUserInfo == null || objUserInfo.Count == 0)
            {
                return false;
            }

            foreach (UserInfo userInfo in objUserInfo)
            {
                List<SecAuthGroupUserInfo> objUserAuth = objUserClient.GetAuthGroupByUserID(userInfo.UserId).ToList();
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
       
    }
}
