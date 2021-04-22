using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using MercPlusClient.ManageMasterDataServiceReference;
using MercPlusClient.Areas.ManageMasterData.Models;
using System.Data.OleDb;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Data;
using System.Xml.Linq;
using System.Data.Odbc;

namespace MercPlusClient.Areas.ManageMasterData.Controllers
{
    public class ManageMasterReTransmissionToolController : Controller
    {
        ManageMasterDataClient mmdc = new ManageMasterDataClient();
        LogEntry logEntry = new LogEntry();
        public ActionResult ReTransmissionTool()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RetransmitWorkOrderStatus(string WOID)
        {
            string result = "";
            try
            {
                string ChgUser = ((UserSec)Session["UserSec"]).UserFirstName + " " + ((UserSec)Session["UserSec"]).UserLastName + "[" + ((UserSec)Session["UserSec"]).LoginId + "]";
                string[] WOIDs = WOID.Split(',');
                result = mmdc.RetransmitWorkOrderStatus(WOIDs, ChgUser);

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                result = "Failed...";
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
         [HttpPost]
        public ActionResult ReTransmissionTool(HttpPostedFileBase upfile, FormCollection collection)
        {
            TempData["Msg"] = "";
            OleDbConnection OleDbCon = null;
            OleDbCommand OLeDbCmd = null;
            string path1 = "";
            OleDbDataReader OleDbdReader;
            OleDbDataAdapter da = new OleDbDataAdapter();
            DataSet ds = new DataSet();
            string ChgUser = ((UserSec)Session["UserSec"]).UserFirstName + " " + ((UserSec)Session["UserSec"]).UserLastName + "[" + ((UserSec)Session["UserSec"]).LoginId + "]";
            try
            {

                if (upfile != null && upfile.ContentLength > 0)
                {


                    string extension = System.IO.Path.GetExtension(Request.Files["upfile"].FileName);
                    if (extension.ToUpper() == ".XLS" || extension.ToUpper() == ".XLSX")
                    {
                        bool exists = System.IO.Directory.Exists(Server.MapPath("~/Content/UploadFolder"));

                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(Server.MapPath("~/Content/UploadFolder"));
                        }
                        path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/UploadFolder"), System.IO.Path.GetFileName(Request.Files["upfile"].FileName));
                        if (System.IO.File.Exists(path1))
                            System.IO.File.Delete(path1);

                        Request.Files["upfile"].SaveAs(path1);

                        if (extension.ToUpper() == ".XLS")
                        {
                            OleDbCon = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties='Excel 8.0;HDR=Yes'");
                        }
                        else
                        {
                            OleDbCon = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";" + "Extended Properties='Excel 12.0 Xml;HDR=Yes;'");
                        }
                        OleDbCon.Open();

                        string selectSql = @"SELECT * FROM [Sheet1$]";
                        OLeDbCmd = new OleDbCommand(selectSql, OleDbCon);

                        da.SelectCommand = OLeDbCmd;
                        da.Fill(ds);
                        OleDbCon.Close();
                        string WOIDS = "";
                        foreach (DataTable table in ds.Tables)
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                if (!string.IsNullOrEmpty(row[0].ToString().Trim()))
                                {
                                    if (string.IsNullOrEmpty(WOIDS))
                                        WOIDS = row[0].ToString();
                                    else
                                        WOIDS += "," + row[0].ToString();
                                }
                            }
                        } 

                      //  OleDbdReader = OLeDbCmd.ExecuteReader();
                        
                      /*  while (OleDbdReader.Read())
                        {

                            WOIDS = WOIDS + "," + OleDbdReader[0] + "";

                        }*/

                        //dReader.Close();
                        //cmd.Dispose();
                       // WOIDS = WOIDS.Substring(0, WOIDS.Length - 1);
                        string result = "";
                        if (WOIDS != "")
                        {
                            string[] WOIDs = WOIDS.Split(',');
                            result = mmdc.RetransmitWorkOrderStatus(WOIDs, ChgUser);
                        }
                        else
                        {
                            result = "There is no valid record to update";
                        }

                       
                        if (result == "Success")
                        {
                            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Retransmitted successfully", "Success");
                        }
                        else
                        {
                            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(result, "Warning");
                        }


                    }
                    else
                    {
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("File with " + extension + " is invalid. Upload a valid file with xls / xlsx extensions", "Warning");

                    }
                }
                else
                {
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Browse to upload a valid File with xls / xlsx extension", "Warning");

                }
            }
            catch (Exception ex)
            {
                OleDbCon.Close();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Error in saving the data. This could be due to incorrect format of the input file. Please correct the file format and resubmit. If you believe that the input file format is correct, please contact the System Administrator.", "Warning");
                //TempData["Msg"] = ex.ToString();
                return View("ReTransmissionTool");
            }
            finally
            {
                //if (OleDbCon.State == ConnectionState.Open)
                //{
                //    OleDbCon.Close();
                //}
                
                if (System.IO.File.Exists(path1))
                {
                    System.IO.File.Delete(path1);
                }
            }

            return View("ReTransmissionTool");
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
}
