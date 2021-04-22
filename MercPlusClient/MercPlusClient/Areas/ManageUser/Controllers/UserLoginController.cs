using MercPlusClient.Areas.ManageUser.Models;
using MercPlusClient.ManageUserServiceReference;
using MercPlusClient.UtilityClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageUser.Controllers
{
    public class UserLoginController : Controller
    {
        LoginModel objLoginModel = new LoginModel();
        public ActionResult Login()
        {

            ViewData["UserRoleList"] = new SelectList(objLoginModel.UserRoleList, "Value", "Text");
            return View();
        }
        [HttpPost]
        public JsonResult GetHomePageForaAuthId(string EnvironMent, string UserLoginAuthType)
        {
            ManageUserClient objUserClient = new ManageUserClient();
            System.Web.HttpContext.Current.Session["UserType"] = UserLoginAuthType;
            //   List<UserInfo> objUserInfo = objUserClient.GetUserByLoginId(UserLoginAuthType).ToList();
            System.Web.HttpContext.Current.Session["UserInfo"] = objUserClient.GetUserByLoginId(UserLoginAuthType).ToList();
            string userId = ((List<UserInfo>)System.Web.HttpContext.Current.Session["UserInfo"])[0].UserId.ToString();


            if (UserLoginAuthType == UtilMethods.USERROLE.ADMIN.ToString())
            {
                return Json(new { redirectUrl = Url.Action("TablemodLaunch", "UserLogin"), isRedirect = true });
            }
            else if (UserLoginAuthType == UtilMethods.USERROLE.CPH.ToString())
            {
                return Json(new { redirectUrl = Url.Action("TablemodLaunch", "UserLogin"), isRedirect = true });
            }
            else if (UserLoginAuthType == UtilMethods.USERROLE.AREA.ToString())
            {
                return Json(new { redirectUrl = Url.Action("TablemodLaunch", "UserLogin"), isRedirect = true });
            }
            else if (UserLoginAuthType == UtilMethods.USERROLE.COUNTRY.ToString())
            {
                return Json(new { redirectUrl = Url.Action("TablemodLaunch", "UserLogin"), isRedirect = true });
            }
            else if (UserLoginAuthType == UtilMethods.USERROLE.MSL.ToString())
            {
                return Json(new { redirectUrl = Url.Action("TablemodLaunch", "UserLogin"), isRedirect = true });
            }
            else if (UserLoginAuthType == UtilMethods.USERROLE.VENDOR.ToString())
            {
                return Json(new { redirectUrl = Url.Action("TablemodLaunch", "SingEstEntry"), isRedirect = true });
            }
            else if (UserLoginAuthType == UtilMethods.USERROLE.SHOP.ToString())
            {
                return Json(new { redirectUrl = Url.Action("UserLogin", "SingEstEntry"), isRedirect = true });
            }
            else if (UserLoginAuthType == UtilMethods.USERROLE.THIRDPARTY.ToString())
            {
                return Json(new { redirectUrl = Url.Action("UserLogin", "SingEstEntry"), isRedirect = true });
            }
            else if (UserLoginAuthType == UtilMethods.USERROLE.READONLY.ToString())
            {
                return Json(new { redirectUrl = Url.Action("UserLogin", "ReportsUrl"), isRedirect = true });
            }
            else
            {
                return Json("");
            }

        }
        public ActionResult TablemodLaunch()
        {

            string xmlPath = HttpContext.Server.MapPath("~/Content/LoginXml/ConfigureLoginUI.xml");

//C:\IBM\MERC+\MercPlusSolution\MercPlusSolution_006_MVC_4\Afroz code\MercPlus_Afroz\MercPlus_Afroz\MercPlusClient\MercPlusClient\Content\LoginXml\ConfigureLoginUI.xml
            DataTable dtUserTable = new DataTable();
            dtUserTable = UtilityClass.UtilMethods.BuildDataTableFromXml("UserTable", xmlPath);

            DataTable dtFilteredByUserType = dtUserTable.Clone();
            //get only the rows you want
            DataRow[] results = dtUserTable.Select("ROLES LIKE '%" + System.Web.HttpContext.Current.Session["UserType"].ToString() + "%'", "ID");
            //populate new destination table
            foreach (DataRow dr in results)
                dtFilteredByUserType.ImportRow(dr);

            int RowLimit = 15, tempCount = 0;
            DataTable dtUniqRecords = new DataTable();

            dtUniqRecords = dtFilteredByUserType.DefaultView.ToTable(true, "ITEM", "ID");
            dtUniqRecords.DefaultView.Sort = "ID";
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
                        strUserMenu.AppendLine("<img src=\"/Content/Images/i.gif\"  width=\"10\">&nbsp;&nbsp;<a href=\"/" + dtFilteredByUserType.Rows[jCtr]["CONTROLLER"].ToString() + "/" + dtFilteredByUserType.Rows[jCtr]["ACTION"].ToString() + "\">" + dtFilteredByUserType.Rows[jCtr]["TEXT"].ToString() + "</a><br>");
                    }
                }
                strUserMenu.AppendLine("<br>");
                if (tempCount >= RowLimit)
                {
                    tempCount = 0;
                    strUserMenu.AppendLine("</td><td><img src=\"/Content/Images/i.gif\" width=\"10\" height=\"1\"></td><td valign=\"top\">");
                }
            }
            strUserMenu.AppendLine("</td></tr></table>");
            TempData["TableModeData"] = strUserMenu.ToString();
            return View();
        }

    }
}
