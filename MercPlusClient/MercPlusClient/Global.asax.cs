using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace MercPlusClient
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();
            // clear error on server
            Server.ClearError();
            Response.Redirect("/ManageSecurity/ManageSecurity/SessionExpire");

        }

        protected void Session_Start()
        {
            HttpContext.Current.Session.Add("UserSec", "");
            HttpContext.Current.Session.Add("UserCertId", "");
            HttpContext.Current.Session.Add("EnvironMent", "");                      
            try
            {
                //  RCMLogHandler.LogInfo("===============Session Start==============", RCMLogHandler.LogType.General);
                string userName = string.Empty;
                //Get internal user from SPENGO header
                userName = GetInternalUserByHeader();
                System.Web.HttpContext.Current.Session["EnvironMent"] = ReadAppSettings.ReadSetting("ServerEnvironment");
                //   if (userName == "") { userName = "ADMIN"; }
                System.Web.HttpContext.Current.Session["UserCertId"] = userName;
            }
            catch (Exception ex)
            {
                //RCMLogHandler.LogError("Login Error", ex);
                // Response.Redirect("~/CustomErrors/LoginError.aspx?Type=3", false);
            }
            finally
            {
                //RCMDispose.ProxyDispose(objAlarmsProxy);
                //RCMDispose.ProxyDispose(objUserProxy);
                //RCMDispose.ProxyDispose(objCommonFunctionsClient);
            }
        }



        #region User_Authentication
        private string GetInternalUserByHeader()
        {
            string userName = string.Empty;
            //if ((Request.Headers.Count > 0) && (Request.Headers["SSO_AD_USER"] != null))
            //{
            //    userName = Request.Headers["SSO_AD_USER"].ToString();
            //    //  RCMLogHandler.LogInfo("===============Internal user by SSO Header is:" + userName, RCMLogHandler.LogType.General);
            //}
            
            var headers = String.Empty;
            if ((Request.Headers.Count > 0) && (Request.Headers["USI_LOP"] != null))
            {
                userName = Request.Headers["USI_LOP"].ToString();
            }
            else
            {
                //    RCMLogHandler.LogInfo("===============Request header is null so didn't found user in SSO Header==============", RCMLogHandler.LogType.General);
            }
            return userName.Trim();
        }
        //private string GetExternalUserByCookie()
        //{
        //    string userName = string.Empty;
        //    if ((Request.Cookies.Count > 0) && (Request.Cookies["x509ClientCert"] != null))
        //    {
        //        string certificateInBase64 = Request.Cookies["x509ClientCert"].Value;
        //        //   RCMLogHandler.LogInfo("Base64 String from Cookie is: " + certificateInBase64, RCMLogHandler.LogType.General);
        //        certificateInBase64 = certificateInBase64.Replace('_', '+');
        //        //  RCMLogHandler.LogInfo("After Conversion Base64 String from Cookie is: " + certificateInBase64, RCMLogHandler.LogType.General);
        //        userName = GetUserNameFromCertificate(certificateInBase64);
        //        //  RCMLogHandler.LogInfo("===============External user from request cookie is:" + userName + " ==============", RCMLogHandler.LogType.General);
        //    }
        //    else
        //    {
        //        //   RCMLogHandler.LogInfo("===============Request cookie is null so didn't found user certificate==============", RCMLogHandler.LogType.General);
        //    }
        //    return userName;
        //}

        //private string GetExternalUserByHeader()
        //{

        //    string userName = string.Empty;
        //    if ((Request.Headers.Count > 0) && (Request.Headers["x509ClientCert"] != null))
        //    {
        //        string certificateInBase64 = Request.Headers["x509ClientCert"].ToString();
        //        //   RCMLogHandler.LogInfo("Base64 String from Header is: " + certificateInBase64, RCMLogHandler.LogType.General);
        //        userName = GetUserNameFromCertificate(certificateInBase64);
        //        //    RCMLogHandler.LogInfo("===============External user from Request header is:" + userName + " ==============", RCMLogHandler.LogType.General);
        //    }
        //    else
        //    {
        //        //   RCMLogHandler.LogInfo("===============Request header is null so didn't found user certificate==============", RCMLogHandler.LogType.General);
        //    }
        //    return userName;
        //}

       

        //private static string GetUserNameFromCertificate(string certificateInBase64)
        //{
        //    byte[] certificatesInBytes = Convert.FromBase64String(certificateInBase64);
        //    X509Certificate2 x509 = new X509Certificate2(certificatesInBytes);

        //    /* CN=Bala VerisignIE2 Dasoji (Maersk Line ID:2363799), 
        //     * OU=Organization - Maersk Line, 
        //     * OU="www.verisign.com/repository/CPS Incorp. by Ref.,LIAB.LTD(c)99", 
        //     * OU=Maersk Line, O=A.P. Moller - Maersk A/S, S=U.S.A. only, 
        //     * C=DK
        //     * As per the certificate for maersk line, shared by Bala Dasoi, the implementation is done
        //     */
        //    var arrSubjectValues = x509.Subject.Split(',');

        //    int startIndex = arrSubjectValues[0].ToString().IndexOf(':') + 1;
        //    int lastIndex = arrSubjectValues[0].ToString().IndexOf(')');

        //    var userName = arrSubjectValues[0].Substring(startIndex, lastIndex - startIndex);
        //    return userName.Trim();
        //}
     
        #endregion
        protected void Session_End()
        {


        }
    }
}