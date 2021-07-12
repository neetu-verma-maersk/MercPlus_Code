using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MercPlusClient.Areas.ManageMasterData.Models;
using MercPlusClient.ManageMasterDataServiceReference;
using MercPlusClient;
using System.Configuration;
using MercPlusClient.UtilityClass;
using System.Web.UI;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace MercPlusClient.Areas.ManageMasterData.Controllers
{
    public class ManageMasterDataController : Controller
    {
        ManageMasterDataModel ManageMasterDataModel = new ManageMasterDataModel();
        ManufacturerDiscountModel ManufDiscountModel = new ManufacturerDiscountModel();
        ManufacturerModel ManufModel = new ManufacturerModel();
        PrepTimeModel PModel = new PrepTimeModel();
        LaborRateModel LabModel = new LaborRateModel();
        ManageMasterDataClient mmdc = new ManageMasterDataClient();
        ManageEqtypeModeEntry ManageEqtypeModeEntry = new ManageEqtypeModeEntry();
        ManageCustomerShopMode ManageCustomerShopMode = new ManageCustomerShopMode();
        ManageMasterCountryLabourRateModel ManageMasterCountryLabourRateModel = new ManageMasterCountryLabourRateModel();
        ManageMasterDataCPHModel ManageMasterDataCPHModel = new ManageMasterDataCPHModel();

        LogEntry logEntry = new LogEntry();
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

        string ErrorMessage = string.Empty;
        public ActionResult ManageMasterData()
        {
            return View();
        }


        #region Pay Agent

        [HttpPost]
        public ActionResult GetAllDetails(string id)
        {
            var request = HttpContext.Request;
            PayAgent data = new PayAgent();
            List<PayAgent> payAgentList = mmdc.GetPayAgent().ToList();
            data = payAgentList.Find(Pid => Pid.PayAgentCode == id);
            string userName = mmdc.GetUserName(data.ChangeUser);
            data.ChangeUser = userName;
            return Json(data);
        }

        #region EditPayAgent

        #region EditPayAgent HTTPGet

        public ActionResult ViewPayAgentDetails([Bind] ManageMasterDataPayAgentModel PayAgentModel)
        {

            PayAgentModel.IsUpdate = true;

            List<SelectListItem> RRISPayAGentCode = new List<SelectListItem>();
            List<SelectListItem> RRISFormat = new List<SelectListItem>();

            RRISPayAGentCode = PopulatePayAgentCodeDropDown();
            RRISFormat = PopulateRRISFormatDropDown(string.Empty);

            PayAgentModel.drpRRISCodesList = RRISPayAGentCode;
            PayAgentModel.drpRRISFormat = RRISFormat;
            return View("ViewPayAgentDetails", PayAgentModel);
        }

        public ActionResult ReturnPayAgentDetails()
        {
            ManageMasterDataPayAgentModel PayAgentModel = new ManageMasterDataPayAgentModel();
            PayAgentModel.IsUpdate = true;

            List<SelectListItem> RRISPayAGentCode = new List<SelectListItem>();
            List<SelectListItem> RRISFormat = new List<SelectListItem>();

            RRISPayAGentCode = PopulatePayAgentCodeDropDown();
            RRISFormat = PopulateRRISFormatDropDown(string.Empty);

            PayAgentModel.drpRRISCodesList = RRISPayAGentCode;
            PayAgentModel.drpRRISFormat = RRISFormat;
            return View("ViewPayAgentDetails", PayAgentModel);
        }


        #endregion EditPayAgent HTTPGet

        #region EditPayAgent HTTPPost

        public ActionResult ViewAllDetails([Bind] ManageMasterDataPayAgentModel PayAgentModel)
        {

            List<PayAgent> payAgentList = mmdc.GetPayAgent().ToList();
            PayAgent data = payAgentList.Find(Pid => Pid.PayAgentCode == PayAgentModel.RRISCode);
            List<SelectListItem> RRISPayAGentCode = new List<SelectListItem>();
            List<SelectListItem> RRISFormat = new List<SelectListItem>();
            RRISPayAGentCode = PopulatePayAgentCodeDropDown();
            RRISFormat = PopulateRRISFormatDropDown(string.Empty);
            PayAgentModel.drpRRISCodesList = RRISPayAGentCode;
            PayAgentModel.drpRRISFormat = RRISFormat;
            PayAgentModel.RRISCode = data.PayAgentCode;
            PayAgentModel.RRISFormat = data.RRISFormat;

            string userName = mmdc.GetUserName(PayAgentModel.ChangeUserName);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    PayAgentModel.ChangeFUser = userName.Split('|')[0];
                    PayAgentModel.ChangeLUser = userName.Split('|')[1];
                }
                else
                {
                    PayAgentModel.ChangeFUser = userName;
                    PayAgentModel.ChangeLUser = "";
                }
            }
            else
            {
                PayAgentModel.ChangeFUser = "";
                PayAgentModel.ChangeLUser = "";
            }
            PayAgentModel.ChangeTime = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");
            return View("ViewPayAgentDetails", PayAgentModel);
        }

        [HttpPost]
        public ActionResult UpdatePayAgent([Bind] ManageMasterDataPayAgentModel PayAgentModelL)
        {
            PayAgentModelL.IsUpdate = true;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            PayAgent payAgentListToBeUpdated = new PayAgent();
            payAgentListToBeUpdated.PayAgentCode = PayAgentModelL.RRISCode;
            payAgentListToBeUpdated.CorpPayAgentCode = PayAgentModelL.CorporatePayAgentCenter;
            payAgentListToBeUpdated.ProfitCenter = PayAgentModelL.CorporateProfitCenter;
            payAgentListToBeUpdated.SubProfitCenter = PayAgentModelL.PayAgentProfitCenter;
            payAgentListToBeUpdated.RRISFormat = PayAgentModelL.RRISFormat;
            payAgentListToBeUpdated.ChangeUser = userId;
            //Call the model method to send for update
            bool result = mmdc.UpdatePayAgent(payAgentListToBeUpdated);
            if (result)
            {
                ErrorMessage = "PayAgent " + PayAgentModelL.RRISCode + " Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
            }
            List<SelectListItem> RRISPayAGentCode = new List<SelectListItem>();
            List<SelectListItem> RRISFormat = new List<SelectListItem>();
            RRISPayAGentCode = PopulatePayAgentCodeDropDown();
            RRISFormat = PopulateRRISFormatDropDown(string.Empty);
            PayAgentModelL.drpRRISCodesList = RRISPayAGentCode;
            PayAgentModelL.drpRRISFormat = RRISFormat;
            PayAgentModelL.ChangeUserName = userId;
            PayAgentModelL.ChangeFUser = "";
            PayAgentModelL.ChangeLUser = "";
            PayAgentModelL.ChangeTime = "";

            return RedirectToAction("ViewAllDetails", PayAgentModelL);
        }
        #endregion EditPayAgent HTTPPost
        #endregion EditPayAgent

        #region DeletePayAgent


        [HttpPost]
        public ActionResult DeletePayAgent([Bind] ManageMasterDataPayAgentModel PayAgentModel)
        {
            PayAgentModel.IsUpdate = true;
            bool result = mmdc.DeletePayAgent(PayAgentModel.RRISCode, ref ErrorMessage);
            if (result)
            {
                ErrorMessage = "PayAgent " + PayAgentModel.RRISCode + " Deleted";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");

                PayAgentModel.RRISCode = null;
                PayAgentModel.RRISFormat = null;
                PayAgentModel.CorporateProfitCenter = null;
                PayAgentModel.CorporatePayAgentCenter = null;
                PayAgentModel.PayAgentProfitCenter = null;
                PayAgentModel.ChangeFUser = null;
                PayAgentModel.ChangeLUser = null;
                PayAgentModel.ChangeTime = null;
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
            }
            List<SelectListItem> RRISPayAGentCode = new List<SelectListItem>();
            List<SelectListItem> RRISFormat = new List<SelectListItem>();
            RRISPayAGentCode = PopulatePayAgentCodeDropDown();
            RRISFormat = PopulateRRISFormatDropDown(string.Empty);
            PayAgentModel.drpRRISCodesList = RRISPayAGentCode;
            PayAgentModel.drpRRISFormat = RRISFormat;
            return RedirectToAction("ViewPayAgentDetails");

        }
        #endregion DeletePayAgent

        #region CreatePayAgent
        #region CreatePayAgent HTTPGet

        public ActionResult PayAgentCreate()
        {
            ManageMasterDataPayAgentModel ManageMasterDataModel = new ManageMasterDataPayAgentModel();
            List<SelectListItem> RRISFormat = new List<SelectListItem>();
            RRISFormat = PopulateRRISFormatDropDown(string.Empty);

            ManageMasterDataModel.drpRRISFormat = RRISFormat;

            return View("ViewPayAgentDetails", ManageMasterDataModel);
        }
        #endregion CreatePayAgent HTTPGet

        #region CreatePayAgent HTTPPost

        [HttpPost]
        public ActionResult CreatePayAgent([Bind] ManageMasterDataPayAgentModel ManageMasterDataModel)
        {

            bool success = false;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            PayAgent PayAgentToBeCreated = new PayAgent();
            PayAgentToBeCreated.CorpPayAgentCode = ManageMasterDataModel.CorporatePayAgentCenter;
            PayAgentToBeCreated.PayAgentCode = ManageMasterDataModel.RRISCode;
            PayAgentToBeCreated.ProfitCenter = ManageMasterDataModel.CorporateProfitCenter;
            PayAgentToBeCreated.RRISFormat = ManageMasterDataModel.RRISFormat;
            PayAgentToBeCreated.ChangeUser = userId;

            success = mmdc.CreatePayAgent(PayAgentToBeCreated, ref ErrorMessage);
            if (success)
            {
                ErrorMessage = "PayAgent " + ManageMasterDataModel.RRISCode + " Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
                ManageMasterDataModel.IsUpdate = true;
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
            }
            string userName = mmdc.GetUserName(PayAgentToBeCreated.ChangeUser);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    ManageMasterDataModel.ChangeFUser = userName.Split('|')[0];
                    ManageMasterDataModel.ChangeLUser = userName.Split('|')[1];
                }
                else
                {
                    ManageMasterDataModel.ChangeFUser = userName;
                    ManageMasterDataModel.ChangeLUser = "";
                }
            }
            else
            {
                ManageMasterDataModel.ChangeFUser = "";
                ManageMasterDataModel.ChangeLUser = "";
            }

            List<SelectListItem> RRISPayAGentCode = new List<SelectListItem>();
            List<SelectListItem> RRISFormat = new List<SelectListItem>();
            RRISPayAGentCode = PopulatePayAgentCodeDropDown();
            RRISFormat = PopulateRRISFormatDropDown(string.Empty);
            ManageMasterDataModel.drpRRISCodesList = RRISPayAGentCode;
            ManageMasterDataModel.drpRRISFormat = RRISFormat;
            ManageMasterDataModel.RRISCode = PayAgentToBeCreated.PayAgentCode;
            ManageMasterDataModel.ChangeUserName = userName;
            ManageMasterDataModel.ChangeTime = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");

            return View("ViewPayAgentDetails", ManageMasterDataModel);
        }
        #endregion CreatePayAgent HTTPPost
        #endregion CreatePayAgent




        #endregion Pay Agent

        #region EquipmentTypeEntry

        [HttpPost]
        public ActionResult GetAllDetailsForEquipmentType(string id)
        {
            var request = HttpContext.Request;
            List<EqType> equipmenttypelist = mmdc.GetEquipmentTypeList().ToList();
            EqType data = equipmenttypelist.Find(Eid => Eid.EqpType == id);
            string userName = mmdc.GetUserName(data.ChangeUser);
            data.ChangeUser = userName;
            return Json(data);

        }

        #region EditEquipmentTypeEntry

        #region EditEquipmentTypeEntry HTTPGet


        public ActionResult ViewEquipmentTypeEntryDetails([Bind] ManageMasterDataEquipmentTypeModel ManageMasterDataModel)
        {
            ManageMasterDataModel.IsUpdate = true;
            List<SelectListItem> EquipmentType = new List<SelectListItem>();
            EquipmentType = PopulateEquipmentTypeDropDown();
            ManageMasterDataModel.drpEquipmentTypeList = EquipmentType;

            return View("ViewEquipmentTypeEntryDetails", ManageMasterDataModel);

        }

        public ActionResult ViewAllDetailsForEquipmentType([Bind] ManageMasterDataEquipmentTypeModel manageMasterDataModel)
        {


            manageMasterDataModel.IsUpdate = true;
            List<SelectListItem> EquipmentType = new List<SelectListItem>();
            EquipmentType = PopulateEquipmentTypeDropDown();
            manageMasterDataModel.drpEquipmentTypeList = EquipmentType;

            List<EqType> equipmenttypelist = mmdc.GetEquipmentTypeList().ToList();
            EqType data = equipmenttypelist.Find(Eid => Eid.EqpType == manageMasterDataModel.EquipmentType);
            manageMasterDataModel.EquipmentType = data.EqpType;
            manageMasterDataModel.EqDesc = data.EqTypeDesc;
            manageMasterDataModel.ChTime = (data.ChangeTime).ToString("yyyy-MM-dd hh:mm:ss tt");
            string userName = mmdc.GetUserName(data.ChangeUser);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    manageMasterDataModel.ChangeFUser = userName.Split('|')[0];
                    manageMasterDataModel.ChangeLUser = userName.Split('|')[1];
                }
                else
                {
                    manageMasterDataModel.ChangeFUser = userName;
                    manageMasterDataModel.ChangeLUser = "";
                }
            }
            else
            {
                manageMasterDataModel.ChangeFUser = "";
                manageMasterDataModel.ChangeLUser = "";
            }

            return View("ViewEquipmentTypeEntryDetails", manageMasterDataModel);
        }

        public ActionResult ViewReturnEquipmentTypeEntryDetails()
        {
            ManageMasterDataEquipmentTypeModel ManageMasterDataModel = new ManageMasterDataEquipmentTypeModel();
            ManageMasterDataModel.IsUpdate = true;
            List<SelectListItem> EquipmentType = new List<SelectListItem>();
            EquipmentType = PopulateEquipmentTypeDropDown();
            ManageMasterDataModel.drpEquipmentTypeList = EquipmentType;

            return View("ViewEquipmentTypeEntryDetails", ManageMasterDataModel);

        }

        #endregion EditEquipmentTypeEntry HTTPGet

        #region EditEquipmentTypeEntry HTTPPost

        [HttpPost]
        public ActionResult EditEquipmentTypeEntry([Bind] ManageMasterDataEquipmentTypeModel manageMasterDataModel)
        {
            manageMasterDataModel.IsUpdate = true;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            EqType EquipmentTypeListToBeUpdated = new EqType();
            EquipmentTypeListToBeUpdated.EqpType = manageMasterDataModel.EquipmentType;
            EquipmentTypeListToBeUpdated.EqTypeDesc = manageMasterDataModel.EqDesc;
            EquipmentTypeListToBeUpdated.ChangeUser = userId;

            bool result = mmdc.UpdateEquipmentType(EquipmentTypeListToBeUpdated, ref ErrorMessage);
            if (result)
            {
                ErrorMessage = "Equipment Type " + manageMasterDataModel.EquipmentType.ToUpper() + " Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
            }
            else
            {

                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
            }

            List<SelectListItem> EquipmentType = new List<SelectListItem>();
            EquipmentType = PopulateEquipmentTypeDropDown();
            manageMasterDataModel.drpEquipmentTypeList = EquipmentType;
            manageMasterDataModel.ChangeFUser = "";
            manageMasterDataModel.ChangeLUser = "";
            manageMasterDataModel.ChTime = "";

            return RedirectToAction("ViewAllDetailsForEquipmentType", manageMasterDataModel);

        }
        #endregion EditEquipmentTypeEntry HTTPPost

        #endregion EditEquipmentTypeEntry

        #region CreateEquipmentTypeEntry

        public ActionResult EqTypeCreate()
        {
            ManageMasterDataEquipmentTypeModel ManageMasterDataModel = new ManageMasterDataEquipmentTypeModel();
            ManageMasterDataModel.IsUpdate = false;
            return View("ViewEquipmentTypeEntryDetails", ManageMasterDataModel);

        }


        #region CreateEquipmentTypeEntry HTTPPost
        [HttpPost]
        public ActionResult CreateEquipmentTypeEntry([Bind] ManageMasterDataEquipmentTypeModel ManageMasterDataModel)
        {
            bool success = false;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            EqType EquipmentTypeToBeCreated = new EqType();
            EquipmentTypeToBeCreated.EqpType = ManageMasterDataModel.EquipmentType;
            EquipmentTypeToBeCreated.EqTypeDesc = ManageMasterDataModel.EqDesc;
            EquipmentTypeToBeCreated.ChangeUser = userId;
            success = mmdc.CreateEquipmentTypeEntry(EquipmentTypeToBeCreated, ref ErrorMessage);
            if (success)
            {
                ErrorMessage = "EquipmentType " + ManageMasterDataModel.EquipmentType + " Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
                ManageMasterDataModel.IsUpdate = true;
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                ManageMasterDataModel.IsUpdate = false;
            }
            string userName = mmdc.GetUserName(EquipmentTypeToBeCreated.ChangeUser);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    ManageMasterDataModel.ChangeFUser = userName.Split('|')[0];
                    ManageMasterDataModel.ChangeLUser = userName.Split('|')[1];
                }
                else
                {
                    ManageMasterDataModel.ChangeFUser = userName;
                    ManageMasterDataModel.ChangeLUser = "";
                }
            }
            else
            {
                ManageMasterDataModel.ChangeFUser = "";
                ManageMasterDataModel.ChangeLUser = "";
            }
            List<SelectListItem> EquipmentType = new List<SelectListItem>();
            EquipmentType = PopulateEquipmentTypeDropDown();
            ManageMasterDataModel.EquipmentType = EquipmentTypeToBeCreated.EqpType;
            ManageMasterDataModel.drpEquipmentTypeList = EquipmentType;
            ManageMasterDataModel.ChTime = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");
            return View("ViewEquipmentTypeEntryDetails", ManageMasterDataModel);

        }
        #endregion CreateEquipmentTypeEntry HTTPPost

        #endregion CreateEquipmentTypeEntry

        #endregion EquipmentTypeEntry

        #region RepairLocationCode
        public ActionResult RepairLocationCode()
        {
            List<RepairLocationCode> RepairLocationCodeList = mmdc.GetRepairLocationCodes().ToList();
            ViewBag.Description = new SelectList(RepairLocationCodeList, "RepairCod", "Description");
            return View();
        }

        public ActionResult RepairLocationCode_View() //Leo_new_change_Usermapping_Debadrita
        {
            List<RepairLocationCode> RepairLocationCodeList = mmdc.GetRepairLocationCodes().ToList();
            ViewBag.Description = new SelectList(RepairLocationCodeList, "RepairCod", "Description");
            return View();
        }

        [HttpPost]
        public ActionResult GetAllDetailsForRepairLocationCode(string id)
        {
            var request = HttpContext.Request;

            ManageMasterDataModel model = new ManageMasterDataModel();

            List<RepairLocationCode> RepairLocationCodelist = mmdc.GetRepairLocationCode(id).ToList();
            RepairLocationCode data = RepairLocationCodelist.Find(Pid => Pid.RepairCod == id);


            List<SelectListItem> drp = new List<SelectListItem>();
            return Json(data);
        }
        #endregion ViewRepairLocationCode



        #region ModeEntry
        public ActionResult ViewModeEntry()
        {
            return View();
        }
        public ActionResult AddModeEntry()
        {
            return View();
        }
        #endregion ModeEntry

        #region Master Discount



        #region EditManufacturerDiscount HTTPGet
        public ActionResult ManufacturerDiscount(List<Manufactur> MDiscountList = null)
        {
            TempData["Msg"] = "";
            ManufDiscountModel.IsUpdate = true;
            ManufDiscountModel.ControllerStatus = 1; //update            
            MDiscountList = mmdc.GetManufacturerDiscountList().ToList();
            List<SelectListItem> MDiscountCode = new List<SelectListItem>();
            List<SelectListItem> MDiscountName = new List<SelectListItem>();
            int count = 1;
            //Populating the PayAgent code fromthe list of payagent we got from db
            foreach (var code in MDiscountList)
            {
                MDiscountCode.Add(new SelectListItem
                {
                    Text = code.ManufacturCd + '-' + code.ManufacturName,
                    Value = code.ManufacturCd
                });
                count++;
            }


            //Assigning the list of selecteditems to the model 
            ManufDiscountModel.drpManufacturerList = MDiscountCode;
            ManufDiscountModel.SelectedManufacturerList = 1;

            return View("ManufacturerDiscount", ManufDiscountModel);
        }


        public ActionResult ManufacturerDiscount_View(List<Manufactur> MDiscountList = null) //Debadrita_User_Remapping
        {
            TempData["Msg"] = "";
            ManufDiscountModel.IsUpdate = true;
            ManufDiscountModel.ControllerStatus = 1; //update            
            MDiscountList = mmdc.GetManufacturerDiscountList().ToList();
            List<SelectListItem> MDiscountCode = new List<SelectListItem>();
            List<SelectListItem> MDiscountName = new List<SelectListItem>();
            int count = 1;
            //Populating the PayAgent code fromthe list of payagent we got from db
            foreach (var code in MDiscountList)
            {
                MDiscountCode.Add(new SelectListItem
                {
                    Text = code.ManufacturCd + '-' + code.ManufacturName,
                    Value = code.ManufacturCd
                });
                count++;
            }


            //Assigning the list of selecteditems to the model 
            ManufDiscountModel.drpManufacturerList = MDiscountCode;
            ManufDiscountModel.SelectedManufacturerList = 1;

            return View("ManufacturerDiscount_View", ManufDiscountModel);
        }


        [HttpPost]
        public ActionResult GetManufacturerDiscount(string id)
        {
            TempData["Msg"] = "";
            var request = HttpContext.Request;
            ManufacturerDiscountModel model = new ManufacturerDiscountModel();
            ManufDiscountModel.ControllerStatus = 1; //update
            List<Manufactur> MDiscountList = mmdc.GetManufacturerDiscountList().ToList();
            Manufactur data = MDiscountList.Find(Pid => Pid.ManufacturCd.Trim() == id.Trim());
            List<SelectListItem> drp = new List<SelectListItem>();

            return Json(data);
        }

        [HttpPost]
        public ActionResult CreateManufacturerDiscounts(string CodeID, string NameID, string PercentageID)
        {


            Manufactur MDiscountToBeCreated = new Manufactur();
            string rtn = "";

            MDiscountToBeCreated.ManufacturCd = CodeID.Trim();
            MDiscountToBeCreated.ManufacturName = NameID;
            MDiscountToBeCreated.DiscountPercent = Convert.ToDouble(PercentageID);
            MDiscountToBeCreated.ChangeUser = ((UserSec)Session["UserSec"]).UserId.ToString();
            MDiscountToBeCreated.ChangeTime = DateTime.Now;

            rtn = mmdc.CreateManufacturerDiscount(MDiscountToBeCreated);
            ManufDiscountModel.IsUpdate = true;

            List<Manufactur> MDiscountList = mmdc.GetManufacturerDiscountList().ToList();
            List<SelectListItem> MDiscountCode = new List<SelectListItem>();
            List<SelectListItem> MDiscountName = new List<SelectListItem>();
            int count = 1;
            //Populating the PayAgent code fromthe list of payagent we got from db
            foreach (var cc in MDiscountList)
            {
                MDiscountCode.Add(new SelectListItem
                {
                    Text = cc.ManufacturCd + '-' + cc.ManufacturName,
                    Value = cc.ManufacturCd
                });
                count++;
            }


            //Assigning the list of selecteditems to the model 
            ManufDiscountModel.drpManufacturerList = MDiscountCode;
            ManufDiscountModel.SelectedManufacturerList = 1;
            //ManufDiscountModel.drpRRISFormat = RRISFormat;
            //ViewData["RRISCodesList"] = RRISPayAGentCode;
            TempData["Msg"] = "";
            if (rtn == "Success")
            {
                TempData["Msg"] = "Manufacturer " + MDiscountCode + " Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Manufacturer " + MDiscountCode + " Added", "Success");

            }
            else if (rtn == "Duplicate")
            {
                TempData["Msg"] = "Manufacturer  " + MDiscountCode + " Already Exists - Not Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Manufacturer " + MDiscountCode + "Already Exists - Not Added", "Warning");

            }
            else if (rtn == "Failure" || rtn == "Failed")
            {
                TempData["Msg"] = "Addition Failed";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Manufacturer " + MDiscountCode + " Addition Failed", "Warning");

            }

            //Manufactur data = MDiscountList.Find(Pid => Pid.ManufacturCd == CodeID);
            //List<SelectListItem> drp = new List<SelectListItem>();
            var data = rtn;
            return Json(data);

            //return View("ManufacturerDiscount", ManufDiscountModel);
        }
        public ActionResult UpdateManufacturerDiscount([Bind] ManufacturerDiscountModel ManufDiscountModel)
        {
            double num;

            if (double.TryParse(ManufDiscountModel.DiscountPercentage, out num) == false)
            {
                TempData["Msg"] = "";
                TempData["Msg"] = "Please enter Numbers only - Decimals OK";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Please enter Numbers only - Decimals OK", "Warning");



                ManufDiscountModel.IsUpdate = true;
                ManufDiscountModel.ControllerStatus = 1; //update
                //    TempData["Msg1"] = "Update";
                List<Manufactur> MDiscountList = mmdc.GetManufacturerDiscountList().ToList();
                List<SelectListItem> MDiscountCode = new List<SelectListItem>();
                List<SelectListItem> MDiscountName = new List<SelectListItem>();
                int count = 1;
                //Populating the PayAgent code fromthe list of payagent we got from db
                foreach (var code in MDiscountList)
                {
                    MDiscountCode.Add(new SelectListItem
                    {
                        Text = code.ManufacturCd + '-' + code.ManufacturName,
                        Value = code.ManufacturCd
                    });
                    count++;
                }


                //Assigning the list of selecteditems to the model 
                ManufDiscountModel.drpManufacturerList = MDiscountCode;
                // ManufDiscountModel.SelectedManufacturerList = 1;
            }
            else
            {


                ManufDiscountModel.IsUpdate = true;
                ManufDiscountModel.ControllerStatus = 1; //update
                // TempData["Msg1"] = "Update";
                Manufactur MDiscountToBeUpdated = new Manufactur();


                MDiscountToBeUpdated.ManufacturCd = ManufDiscountModel.ManufacturerCode;
                MDiscountToBeUpdated.ManufacturName = ManufDiscountModel.ManufacturerName;
                MDiscountToBeUpdated.DiscountPercent = Convert.ToDouble(ManufDiscountModel.DiscountPercentage.ToString());
                MDiscountToBeUpdated.ChangeUser = ((UserSec)Session["UserSec"]).UserId.ToString();
                MDiscountToBeUpdated.ChangeTime = DateTime.Now; ;

                mmdc.UpdateManufacturerDiscount(MDiscountToBeUpdated);
                ManufDiscountModel.IsUpdate = true;

                List<Manufactur> MDiscountList = mmdc.GetManufacturerDiscountList().ToList();
                List<SelectListItem> MDiscountCode = new List<SelectListItem>();
                List<SelectListItem> MDiscountName = new List<SelectListItem>();
                int count = 1;
                //Populating the PayAgent code fromthe list of payagent we got from db
                foreach (var code in MDiscountList)
                {
                    MDiscountCode.Add(new SelectListItem
                    {
                        Text = code.ManufacturCd + '-' + code.ManufacturName,
                        Value = code.ManufacturCd
                    });
                    count++;
                }


                //Assigning the list of selecteditems to the model 
                ManufDiscountModel.drpManufacturerList = MDiscountCode;
                ManufDiscountModel.SelectedManufacturerList = 1;
                //ManufDiscountModel.drpRRISFormat = RRISFormat;
                //ViewData["RRISCodesList"] = RRISPayAGentCode;
                TempData["Msg"] = "";
                TempData["Msg"] = "Manufacturer " + ManufDiscountModel.ManufacturerCode.ToString() + " Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Manufacturer " + ManufDiscountModel.ManufacturerCode.ToString() + " Updated", "Success");
            }
            return View("ManufacturerDiscount", ManufDiscountModel);
        }

        [HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "Delete")]
        public ActionResult DeleteManufacturerDiscount([Bind] ManufacturerDiscountModel ManufDiscountModel)
        {
            bool ret = false;
            TempData["Msg"] = "";
            ret = mmdc.DeleteManufacturerDiscount(ManufDiscountModel.ManufacturerCode);
            ManufDiscountModel.ControllerStatus = 1; //update

            List<Manufactur> MDiscountList = mmdc.GetManufacturerDiscountList().ToList();
            List<SelectListItem> MDiscountCode = new List<SelectListItem>();
            List<SelectListItem> MDiscountName = new List<SelectListItem>();
            int count = 1;
            //Populating the PayAgent code fromthe list of payagent we got from db
            foreach (var code in MDiscountList)
            {
                MDiscountCode.Add(new SelectListItem
                {
                    Text = code.ManufacturCd + '-' + code.ManufacturName,
                    Value = code.ManufacturCd
                });
                count++;
            }


            //Assigning the list of selecteditems to the model 
            ManufDiscountModel.drpManufacturerList = MDiscountCode;
            ManufDiscountModel.SelectedManufacturerList = 1;

            if (ret == true)
            {
                TempData["Msg"] = "Manufacturer " + ManufDiscountModel.ManufacturerCode.ToString() + " Deleted";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Manufacturer " + ManufDiscountModel.ManufacturerCode.ToString() + " Deleted", "Success");
            }
            else
            {
                TempData["Msg"] = "Manufacturer " + ManufDiscountModel.ManufacturerCode.ToString() + " Could not Deleted";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Manufacturer " + ManufDiscountModel.ManufacturerCode.ToString() + " Could not deleted", "Warning");
            }
            //return RedirectToAction("ManufacturerDiscount");
            return View("ManufacturerDiscount", ManufDiscountModel);

        }
        #endregion EditManufacturerDiscount
        #endregion master Discount

        #region Manufaturer Model
        public ActionResult ManufacturerModel(List<Manufactur> MDiscountList = null)
        {
            TempData["Msg"] = "";
            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "");
            ManufModel.IsUpdate = true;
            ManufModel.ControllerStatus = 1; //update

            MDiscountList = mmdc.GetManufacturerDiscountList().ToList();
            List<SelectListItem> MDiscountCode = new List<SelectListItem>();
            List<SelectListItem> MDiscountName = new List<SelectListItem>();
            int count = 1;
            //Populating the PayAgent code fromthe list of payagent we got from db
            foreach (var code in MDiscountList)
            {
                MDiscountCode.Add(new SelectListItem
                {
                    Text = code.ManufacturCd + '-' + code.ManufacturName,
                    Value = code.ManufacturCd
                });
                count++;
            }


            //Assigning the list of selecteditems to the model 
            ManufModel.drpManufacturerList = MDiscountCode;
            ManufModel.SelectedManufacturerList = 1;



            List<SelectListItem> MDiscountCodeC = new List<SelectListItem>();

            int countC = 1;
            //Populating the PayAgent code fromthe list of payagent we got from db
            foreach (var code in MDiscountList)
            {
                MDiscountCodeC.Add(new SelectListItem
                {
                    Text = code.ManufacturCd,
                    Value = code.ManufacturCd
                });
                countC++;
            }
            ManufModel.drpManufacturerListC = MDiscountCodeC;
            // ManufModel.SelectedManufacturerList = 1;


            //ManufModel.drpRRISFormat = RRISFormat;
            //ViewData["RRISCodesList"] = RRISPayAGentCode;

            return View("ManufacturerModel", ManufModel);
        }

        public ActionResult ManufacturerModel_View(List<Manufactur> MDiscountList = null)
        {
            TempData["Msg"] = "";
            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "");
            ManufModel.IsUpdate = true;
            ManufModel.ControllerStatus = 1; //update

            MDiscountList = mmdc.GetManufacturerDiscountList().ToList();
            List<SelectListItem> MDiscountCode = new List<SelectListItem>();
            List<SelectListItem> MDiscountName = new List<SelectListItem>();
            int count = 1;
            //Populating the PayAgent code fromthe list of payagent we got from db
            foreach (var code in MDiscountList)
            {
                MDiscountCode.Add(new SelectListItem
                {
                    Text = code.ManufacturCd + '-' + code.ManufacturName,
                    Value = code.ManufacturCd
                });
                count++;
            }


            //Assigning the list of selecteditems to the model 
            ManufModel.drpManufacturerList = MDiscountCode;
            ManufModel.SelectedManufacturerList = 1;



            List<SelectListItem> MDiscountCodeC = new List<SelectListItem>();

            int countC = 1;
            //Populating the PayAgent code fromthe list of payagent we got from db
            foreach (var code in MDiscountList)
            {
                MDiscountCodeC.Add(new SelectListItem
                {
                    Text = code.ManufacturCd,
                    Value = code.ManufacturCd
                });
                countC++;
            }
            ManufModel.drpManufacturerListC = MDiscountCodeC;
            // ManufModel.SelectedManufacturerList = 1;


            //ManufModel.drpRRISFormat = RRISFormat;
            //ViewData["RRISCodesList"] = RRISPayAGentCode;

            return View("ManufacturerModel_View", ManufModel);
        }

        [HttpPost]
        public ActionResult SearchModel(string ManufactureCode)
        {

            ManufacturerModel model = new ManufacturerModel();
            List<Model> ModelList = null;
            //Session["QueryType"] = QueryType.ToString();
            var data = ModelList;
            TempData["Msg"] = "";
            try
            {
                ModelList = mmdc.GetManufacturerModelList(ManufactureCode).ToList();
                data = (from e in ModelList
                        select new Model
                        {
                            ManufacturCd = e.ManufacturCd,
                            ModelNo = e.ModelNo,
                            IndicatorCd = e.IndicatorCd,
                            ChangeUser = e.ChangeUser,
                            ChangeTime = e.ChangeTime,


                        }).ToList();

                data.OrderBy(li => li.ModelNo);

            }
            catch (Exception ex)
            {
                TempData["Msg"] = "FAILED";

            }
            if (data.Count == 0)
            {
                TempData["Msg"] = "No Results Found Matching Search Criteria";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "Warning");
                //return View("ManufacturerModel", data);
            }
            else
                TempData["Msg"] = "";

            return View("MModelPartial", data);
        }


        public ActionResult SearchModel_View(string ManufactureCode)
        {

            ManufacturerModel model = new ManufacturerModel();
            List<Model> ModelList = null;
            //Session["QueryType"] = QueryType.ToString();
            var data = ModelList;
            TempData["Msg"] = "";
            try
            {
                ModelList = mmdc.GetManufacturerModelList(ManufactureCode).ToList();
                data = (from e in ModelList
                        select new Model
                        {
                            ManufacturCd = e.ManufacturCd,
                            ModelNo = e.ModelNo,
                            IndicatorCd = e.IndicatorCd,
                            ChangeUser = e.ChangeUser,
                            ChangeTime = e.ChangeTime,


                        }).ToList();

                data.OrderBy(li => li.ModelNo);

            }
            catch (Exception ex)
            {
                TempData["Msg"] = "FAILED";

            }
            if (data.Count == 0)
            {
                TempData["Msg"] = "No Results Found Matching Search Criteria";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "Warning");
                //return View("ManufacturerModel", data);
            }
            else
                TempData["Msg"] = "";

            return View("MModelPartial_View", data);
        }



        public ActionResult UpdateManufacturerModel(string CodeID, string ModelNameID, string RepaireCodeID, string UserID)
        {
            Model ModelToBeUpdated = new Model();


            ModelToBeUpdated.ManufacturCd = CodeID;
            ModelToBeUpdated.ModelNo = ModelNameID;
            ModelToBeUpdated.IndicatorCd = RepaireCodeID;
            ModelToBeUpdated.ChangeUser = ((UserSec)Session["UserSec"]).UserId.ToString();
            ModelToBeUpdated.ChangeTime = DateTime.Now;

            mmdc.UpdateManufacturerModel(ModelToBeUpdated);

            ModelToBeUpdated.ManufacturCd = "";
            ModelToBeUpdated.ModelNo = "";
            ModelToBeUpdated.IndicatorCd = "";
            ModelToBeUpdated.ChangeUser = ((UserSec)Session["UserSec"]).UserId.ToString();
            return Json(RedirectToAction("ManufacturerModel"));
        }
        public ActionResult DeleteManufacturerModel(string CodeID, string ModelNameID)
        {
            bool result = false;
            TempData["Msg"] = "";
            result = mmdc.DeleteManufacturerModel(CodeID, ModelNameID);
            if (result == true)
            {
                TempData["Msg"] = "**Model: " + ModelNameID + " / " + CodeID + " Deleted**";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "Success");
            }
            else
            {
                TempData["Msg"] = "Deletion Failed";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "Warning");
            }
            return Json(RedirectToAction("ManufacturerModel"));
        }
        public ActionResult CreateManufacturerModel(string CodeID, string ModelNameID, string RepaireCodeID)
        {
            Model MModelToBeCreated = new Model();
            TempData["Msg"] = "";
            bool success = false;
            MModelToBeCreated.ManufacturCd = CodeID;
            MModelToBeCreated.ModelNo = ModelNameID;
            MModelToBeCreated.IndicatorCd = RepaireCodeID;
            MModelToBeCreated.ChangeUser = ((UserSec)Session["UserSec"]).UserId.ToString();
            MModelToBeCreated.ChangeTime = DateTime.Now;

            success = mmdc.CreateManufacturerModel(MModelToBeCreated);

            if (success == true)
            {
                TempData["Msg"] = "**Model: " + ModelNameID + " / " + CodeID + " Added**";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "Success");
            }
            else
            {
                TempData["Msg"] = "Addition Failed";

                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "Success");
            }
            //   return Json(RedirectToAction("ManufacturerModel"));
            return Json(success);
        }
        [HttpPost]
        public ActionResult AddNewModels(List<Manufactur> MDiscountList = null)
        {
            TempData["Msg"] = "";
            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "");
            ManufModel.IsUpdate = true;
            ManufModel.ControllerStatus = 1; //update

            MDiscountList = mmdc.GetManufacturerDiscountList().ToList();
            List<SelectListItem> MDiscountCode = new List<SelectListItem>();
            List<SelectListItem> MDiscountName = new List<SelectListItem>();
            int count = 1;
            //Populating the PayAgent code fromthe list of payagent we got from db
            foreach (var code in MDiscountList)
            {
                MDiscountCode.Add(new SelectListItem
                {
                    Text = code.ManufacturCd + '-' + code.ManufacturName,
                    Value = code.ManufacturCd
                });
                count++;
            }


            //Assigning the list of selecteditems to the model 
            ManufModel.drpManufacturerList = MDiscountCode;
            ManufModel.SelectedManufacturerList = 1;



            List<SelectListItem> MDiscountCodeC = new List<SelectListItem>();

            int countC = 1;
            //Populating the PayAgent code fromthe list of payagent we got from db
            foreach (var code in MDiscountList)
            {
                MDiscountCodeC.Add(new SelectListItem
                {
                    Text = code.ManufacturCd,
                    Value = code.ManufacturCd
                });
                countC++;
            }
            ManufModel.drpManufacturerListC = MDiscountCodeC;
            // ManufModel.SelectedManufacturerList = 1;


            //ManufModel.drpRRISFormat = RRISFormat;
            //ViewData["RRISCodesList"] = RRISPayAGentCode;

            return View("ManufacturerModelAdd", ManufModel);
        }
        #endregion

        #region PrepTime
        public ActionResult PrepTime(List<Mode> ModenList = null)
        {
            TempData["Msg"] = "";
            ModenList = mmdc.GetPrepModes(true).ToList();

            List<SelectListItem> ModeCode = new List<SelectListItem>();
            List<SelectListItem> ModeDescription = new List<SelectListItem>();
            int count = 1;

            foreach (var code in ModenList)
            {
                ModeCode.Add(new SelectListItem
                {
                    //Text = code.ModeCode + '-' + code.ModeDescription,
                    Text = code.ModeDescription,
                    Value = code.ModeCode
                });
                count++;
            }


            //Assigning the list of selecteditems to the model 
            PModel.drpModenList = ModeCode;
            PModel.SelectedModenList = 1;
            //---------------------------------------------------
            List<Mode> ModenListC = null;
            ModenListC = mmdc.GetPrepModes(false).ToList();

            List<SelectListItem> ModeCodeC = new List<SelectListItem>();
            List<SelectListItem> ModeDescriptionC = new List<SelectListItem>();
            int countC = 1;

            foreach (var code in ModenListC)
            {
                ModeCodeC.Add(new SelectListItem
                {
                    //Text = code.ModeCode + '-' + code.ModeDescription,
                    Text = code.ModeDescription,
                    Value = code.ModeCode
                });
                countC++;
            }


            //Assigning the list of selecteditems to the model 
            PModel.drpModenListC = ModeCodeC;
            PModel.SelectedModenListC = 1;



            //---------------------------------------------

            List<SelectListItem> PreptimeMax = new List<SelectListItem>();
            PModel.drpModePList = PreptimeMax;
            return View("PrepTime", PModel);
        }
        public ActionResult GetPrepTimemaxList(string Modecode)
        {
            TempData["Msg"] = "";
            var request = HttpContext.Request;

            PrepTimeModel model = new PrepTimeModel();
            List<PrepTime> ModePList = mmdc.GetPrepTimeDetails(Modecode).ToList();
            // PrepTime data = ModePList.Find(Pid => Pid.ModeCode == Modecode);
            List<SelectListItem> PreptimeMax = new List<SelectListItem>();
            int count = 1;

            foreach (var code in ModePList)
            {
                PreptimeMax.Add(new SelectListItem
                {
                    Text = code.PrepTimeMax.ToString(),
                    Value = code.PrepTimeMax.ToString()
                });
                count++;
            }


            //Assigning the list of selecteditems to the model 
            PModel.drpModePList = PreptimeMax;
            PModel.SelectedModePList = 1;

            List<SelectListItem> data = PreptimeMax;

            return Json(data);
        }

        public ActionResult GetPrepTimemaxDetial(string Modecode, string pmax)
        {
            TempData["Msg"] = "";
            var request = HttpContext.Request;

            double _pmax = Convert.ToDouble(pmax);
            PrepTimeModel model = new PrepTimeModel();
            List<PrepTime> ModePList = mmdc.GetPrepTimeDetails(Modecode).ToList();
            PrepTime data = ModePList.Find(Pid => Pid.PrepTimeMax == _pmax);

            return Json(data);
        }

        public ActionResult UpdatePrepTime(string Modecode, string pmax, string PrepCd, string prephrs)
        {
            PrepTime PrepToBeUpdated = new PrepTime();
            string data = "";
            TempData["Msg"] = "";
            PrepToBeUpdated.ModeCode = Modecode;
            PrepToBeUpdated.PrepTimeMax = Convert.ToDouble(pmax);
            PrepToBeUpdated.PrepHrs = Convert.ToDouble(prephrs);
            PrepToBeUpdated.PrepCd = PrepCd;
            PrepToBeUpdated.ChangeUser = ((UserSec)Session["UserSec"]).UserId.ToString();

            PrepToBeUpdated.ChangeTime = DateTime.Now;

            TempData["Msg"] = mmdc.UpdatePrepTime(PrepToBeUpdated);
            //TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "Success");
            // return Json(RedirectToAction("ManageMasterDataModel"));
            if (TempData["Msg"].ToString().Contains("Modification Completed"))
                data = "Success";
            else
            {
                data = TempData["Msg"].ToString();


            }

            return Json(data);
        }
        public ActionResult AddPrepRecord(string Modecode, string pmax, string PrepCd, string prephrs)
        {
            PrepTime preplToBeCreated = new PrepTime();
            TempData["Msg"] = "";
            string data = "";

            preplToBeCreated.ModeCode = Modecode;
            preplToBeCreated.PrepTimeMax = Convert.ToDouble(pmax);
            preplToBeCreated.PrepHrs = Convert.ToDouble(prephrs);
            preplToBeCreated.PrepCd = PrepCd;
            preplToBeCreated.ChangeUser = ((UserSec)Session["UserSec"]).UserId.ToString();

            preplToBeCreated.ChangeTime = DateTime.Now;

            TempData["Msg"] = mmdc.CreatePrepTime(preplToBeCreated);
            if (TempData["Msg"].ToString().Contains("**PrepTimeMax:"))
                data = "Success";
            else
            {
                data = TempData["Msg"].ToString();

                // TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "Warning");
            }

            return Json(data);
        }
        public ActionResult DeletePrepTime(string Modecode, string pmax, string PrepCd, string prephrs)
        {
            TempData["Msg"] = "";
            bool data = mmdc.DeletePrepTime(Modecode, Convert.ToDouble(pmax));
            if (data == true)
                TempData["Msg"] = "**PrepTimeMax: " + pmax + " / " + Modecode + " Deleted**";

            else
                TempData["Msg"] = "Deletion Failed";

            return Json(data);
        }
        public ActionResult GetPrepTimeDetails(string Modecode = null)
        {
            TempData["Msg"] = "";
            List<PrepTime> ModePList = mmdc.GetPrepTimeDetails(Modecode).ToList();
            List<SelectListItem> PreptimeMax = new List<SelectListItem>();
            List<SelectListItem> ModeDescription = new List<SelectListItem>();
            int count = 1;

            foreach (var code in ModePList)
            {
                PreptimeMax.Add(new SelectListItem
                {
                    Text = code.PrepTimeMax.ToString(),
                    Value = code.PrepTimeMax.ToString()
                });
                count++;
            }


            //Assigning the list of selecteditems to the model 
            PModel.drpModePList = PreptimeMax;
            PModel.SelectedModePList = 1;

            List<SelectListItem> ModeCode1 = new List<SelectListItem>();

            PModel.drpModenList = ModeCode1;


            return View("PrepTime", PModel);



        }
        #endregion

        #region LaborRate

        public ActionResult LaborRate(List<Shop> LshopList = null, List<Customer> LCustomer = null, List<EqType> LEqpType = null)
        {
            LabModel.IsUpdate = true;
            TempData["Msg"] = "";

            List<Shop> LshopListA = null;
            LshopList = mmdc.GetShopCodeByUserId(((UserSec)Session["UserSec"]).UserId).ToList();
            List<SelectListItem> MShopCode = new List<SelectListItem>();
            List<SelectListItem> MShopName = new List<SelectListItem>();
            int count = 1;

            foreach (var code in LshopList)
            {
                MShopCode.Add(new SelectListItem
                {
                    //  Text = code.ShopCode + '-' + code.ShopDescription,
                    Text = code.ShopCode,
                    Value = code.ShopCode
                });
                count++;
            }

            LabModel.drpShopList = MShopCode;
            LshopListA = mmdc.GetInactiveShopByUserId(((UserSec)Session["UserSec"]).UserId).ToList();
            List<SelectListItem> MShopCodeA = new List<SelectListItem>();
            List<SelectListItem> MShopNameA = new List<SelectListItem>();
            int countA = 1;

            foreach (var codeA in LshopListA)
            {
                MShopCodeA.Add(new SelectListItem
                {
                    //  Text = code.ShopCode + '-' + code.ShopDescription,
                    Text = codeA.ShopCode,
                    Value = codeA.ShopCode
                });
                countA++;
            }

            LabModel.drpShopListA = MShopCodeA;

            //LCustomer = mmdc.GetCustomerList().ToList();
            List<SelectListItem> MCustomerCode = new List<SelectListItem>();
            List<SelectListItem> MCustomerName = new List<SelectListItem>();
            int countC = 1;

            //foreach (var code in LCustomer)
            //{
            //    MCustomerCode.Add(new SelectListItem
            //    {
            //        Text = code.CustomerCode + '-' + code.CustomerDesc,
            //        Value = code.CustomerCode
            //    });
            //    countC++;
            //}

            MCustomerCode.Add(new SelectListItem
            {
                Text = "MAER" + '-' + "MAERSK LINE",
                Value = "MAER"
            });


            LabModel.drpCustomerList = MCustomerCode; //change pinaki


            LEqpType = mmdc.GetEquipmentList().ToList();
            List<SelectListItem> MEqpTypeCode = new List<SelectListItem>();
            List<SelectListItem> MEqpTypeName = new List<SelectListItem>();
            int countE = 1;

            foreach (var code in LEqpType)
            {
                MEqpTypeCode.Add(new SelectListItem
                {
                    Text = code.EqpType + '-' + code.EqTypeDesc,
                    Value = code.EqpType
                });
                countE++;
            }

            LabModel.drpEqupList = MEqpTypeCode;


            return View("LaborRate", LabModel);
        }



        public ActionResult LaborRate_View(List<Shop> LshopList = null, List<Customer> LCustomer = null, List<EqType> LEqpType = null)//Debadrita_User_Remapping
        {
            LabModel.IsUpdate = true;
            TempData["Msg"] = "";

            List<Shop> LshopListA = null;
            LshopList = mmdc.GetShopCodeByUserId(((UserSec)Session["UserSec"]).UserId).ToList();
            List<SelectListItem> MShopCode = new List<SelectListItem>();
            List<SelectListItem> MShopName = new List<SelectListItem>();
            int count = 1;

            foreach (var code in LshopList)
            {
                MShopCode.Add(new SelectListItem
                {
                    //  Text = code.ShopCode + '-' + code.ShopDescription,
                    Text = code.ShopCode,
                    Value = code.ShopCode
                });
                count++;
            }

            LabModel.drpShopList = MShopCode;
            LshopListA = mmdc.GetInactiveShopByUserId(((UserSec)Session["UserSec"]).UserId).ToList();
            List<SelectListItem> MShopCodeA = new List<SelectListItem>();
            List<SelectListItem> MShopNameA = new List<SelectListItem>();
            int countA = 1;

            foreach (var codeA in LshopListA)
            {
                MShopCodeA.Add(new SelectListItem
                {
                    //  Text = code.ShopCode + '-' + code.ShopDescription,
                    Text = codeA.ShopCode,
                    Value = codeA.ShopCode
                });
                countA++;
            }

            LabModel.drpShopListA = MShopCodeA;

            //LCustomer = mmdc.GetCustomerList().ToList();
            List<SelectListItem> MCustomerCode = new List<SelectListItem>();
            List<SelectListItem> MCustomerName = new List<SelectListItem>();
            int countC = 1;

            //foreach (var code in LCustomer)
            //{
            //    MCustomerCode.Add(new SelectListItem
            //    {
            //        Text = code.CustomerCode + '-' + code.CustomerDesc,
            //        Value = code.CustomerCode
            //    });
            //    countC++;
            //}

            MCustomerCode.Add(new SelectListItem
            {
                Text = "MAER" + '-' + "MAERSK LINE",
                Value = "MAER"
            });


            LabModel.drpCustomerList = MCustomerCode; //change pinaki


            LEqpType = mmdc.GetEquipmentList().ToList();
            List<SelectListItem> MEqpTypeCode = new List<SelectListItem>();
            List<SelectListItem> MEqpTypeName = new List<SelectListItem>();
            int countE = 1;

            foreach (var code in LEqpType)
            {
                MEqpTypeCode.Add(new SelectListItem
                {
                    Text = code.EqpType + '-' + code.EqTypeDesc,
                    Value = code.EqpType
                });
                countE++;
            }

            LabModel.drpEqupList = MEqpTypeCode;


            return View("LaborRate_View", LabModel);
        }



        public ActionResult GetCurrCode(string ShopCode)
        {

            List<Shop> Slist = mmdc.GetShopCodeByUserId(((UserSec)Session["UserSec"]).UserId).ToList();
            Shop sdata = Slist.Find(Pid => Pid.ShopCode == ShopCode);
            var data = sdata.CUCDN;
            return Json(data);
        }

        public ActionResult CreateLaborRate(string ShopCode, string CustCode, string eqtypeCode, string EFFDate, string EXPDate, string OrdRate, string OT1, string OT2, string OT3)
        {
            TempData["Msg"] = "";
            LaborRate LrateToBeCreated = new LaborRate();
            List<Shop> Slist = mmdc.GetShopCodeByUserId(((UserSec)Session["UserSec"]).UserId).ToList();
            Shop sdata = Slist.Find(Pid => Pid.ShopCode == ShopCode);


            string data1 = "NO";

            LrateToBeCreated.ShopCode = ShopCode;
            LrateToBeCreated.CustomerCode = CustCode;
            LrateToBeCreated.EqpType = eqtypeCode;
            LrateToBeCreated.EffDate = Convert.ToDateTime(EFFDate);
            LrateToBeCreated.ExpDate = Convert.ToDateTime(EXPDate);
            LrateToBeCreated.RegularRT = Convert.ToDecimal(OrdRate);
            LrateToBeCreated.OvertimeRT = Convert.ToDecimal(OT1);
            LrateToBeCreated.DoubleTimeRT = Convert.ToDecimal(OT2);
            LrateToBeCreated.MiscRT = Convert.ToDecimal(OT3);
            LrateToBeCreated.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;
            LrateToBeCreated.ChangeTime = DateTime.Now;


            data1 = mmdc.CreateLaborRate(LrateToBeCreated);
            var Curr = sdata.CUCDN;
            var Result = new { data = data1, id = Curr };
            return Json(Result, JsonRequestBehavior.AllowGet);
            // return Json(data);
        }
        [HttpPost]
        public ActionResult SearchLaborRate(string ShopCode, String CustCode, String eqtypeCode)
        {

            List<Shop> LshopListE = null;
            // LshopListA = mmdc.GetNonActiveShopList().ToList();
            LshopListE = mmdc.GetShopCodeByUserId(((UserSec)Session["UserSec"]).UserId).ToList();

            var edit = false;
            LaborRateModel model = new LaborRateModel();
            List<LaborRate> LRATEList = null;
            //Session["QueryType"] = QueryType.ToString();
            var data = LRATEList;
            TempData["Msg"] = "";
            if (ShopCode == "")
                ShopCode = null;
            if (CustCode == "")
                CustCode = null;
            if (eqtypeCode == "")
                eqtypeCode = null;

            Shop sdata = LshopListE.Find(Pid => Pid.ShopCode == ShopCode);
            if (sdata != null && sdata.ShopActiveSW == "N")
                edit = true;


            try
            {
                LRATEList = mmdc.GetLaborRateDetail(ShopCode, CustCode, eqtypeCode).ToList();
                data = (from e in LRATEList
                        select new LaborRate
                        {

                            LaborRateID = e.LaborRateID,
                            Shop = sdata,
                            ShopCode = e.ShopCode,
                            CustomerCode = e.CustomerCode,
                            EqpType = e.EqpType,
                            EffDate = e.EffDate,
                            ExpDate = e.ExpDate,
                            RegularRT = e.RegularRT,
                            OvertimeRT = e.OvertimeRT,
                            DoubleTimeRT = e.DoubleTimeRT,
                            MiscRT = e.MiscRT,
                            ChangeUser = e.ChangeUser,
                            ChangeTime = e.ChangeTime
                        }).ToList();

                data.OrderBy(li => li.LaborRateID);

            }
            catch (Exception ex)
            {
                TempData["Msg"] = "FAILED";
            }

            if (data.Count == 0)
            {
                TempData["Msg"] = "**** NO RECORDS FOUND MATCHING SEARCH CRITERIA ****";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "Success");
            }
            else
                TempData["Msg"] = "";

            return View("LaborRatePartial", data);
        }



        public ActionResult SearchLaborRate_View(string ShopCode, String CustCode, String eqtypeCode)
        {

            List<Shop> LshopListE = null;
            // LshopListA = mmdc.GetNonActiveShopList().ToList();
            LshopListE = mmdc.GetShopCodeByUserId(((UserSec)Session["UserSec"]).UserId).ToList();

            var edit = false;
            LaborRateModel model = new LaborRateModel();
            List<LaborRate> LRATEList = null;
            //Session["QueryType"] = QueryType.ToString();
            var data = LRATEList;
            TempData["Msg"] = "";
            if (ShopCode == "")
                ShopCode = null;
            if (CustCode == "")
                CustCode = null;
            if (eqtypeCode == "")
                eqtypeCode = null;

            Shop sdata = LshopListE.Find(Pid => Pid.ShopCode == ShopCode);
            if (sdata != null && sdata.ShopActiveSW == "N")
                edit = true;


            try
            {
                LRATEList = mmdc.GetLaborRateDetail(ShopCode, CustCode, eqtypeCode).ToList();
                data = (from e in LRATEList
                        select new LaborRate
                        {

                            LaborRateID = e.LaborRateID,
                            Shop = sdata,
                            ShopCode = e.ShopCode,
                            CustomerCode = e.CustomerCode,
                            EqpType = e.EqpType,
                            EffDate = e.EffDate,
                            ExpDate = e.ExpDate,
                            RegularRT = e.RegularRT,
                            OvertimeRT = e.OvertimeRT,
                            DoubleTimeRT = e.DoubleTimeRT,
                            MiscRT = e.MiscRT,
                            ChangeUser = e.ChangeUser,
                            ChangeTime = e.ChangeTime
                        }).ToList();

                data.OrderBy(li => li.LaborRateID);

            }
            catch (Exception ex)
            {
                TempData["Msg"] = "FAILED";
            }

            if (data.Count == 0)
            {
                TempData["Msg"] = "**** NO RECORDS FOUND MATCHING SEARCH CRITERIA ****";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(TempData["Msg"].ToString(), "Success");
            }
            else
                TempData["Msg"] = "";

            return View("LaborRatePartial_View", data);
        }



        public ActionResult LaborRateEdit(int ContID)
        {
            LaborRateModel LabModel = new LaborRateModel();
            List<Shop> LshopListA = null;
            List<Customer> LCustomer = null;
            List<EqType> LEqpType = null;
            LshopListA = mmdc.GetInactiveShopByUserId(((UserSec)Session["UserSec"]).UserId).ToList();
            List<SelectListItem> MShopCodeA = new List<SelectListItem>();
            List<SelectListItem> MShopNameA = new List<SelectListItem>();
            int countA = 1;

            foreach (var codeA in LshopListA)
            {
                MShopCodeA.Add(new SelectListItem
                {
                    //  Text = code.ShopCode + '-' + code.ShopDescription,
                    Text = codeA.ShopCode,
                    Value = codeA.ShopCode
                });
                countA++;
            }

            LabModel.drpShopListA = MShopCodeA;

            //LCustomer = mmdc.GetCustomerList().ToList();
            List<SelectListItem> MCustomerCode = new List<SelectListItem>();
            List<SelectListItem> MCustomerName = new List<SelectListItem>();
            int countC = 1;

            //foreach (var code in LCustomer)
            //{
            //    MCustomerCode.Add(new SelectListItem
            //    {
            //        Text = code.CustomerCode + '-' + code.CustomerDesc,
            //        Value = code.CustomerCode
            //    });
            //    countC++;
            //}

            MCustomerCode.Add(new SelectListItem
            {
                Text = "MAER" + '-' + "MAERSK LINE",
                Value = "MAER"
            });

            LabModel.drpCustomerList = MCustomerCode; //change pinaki


            LEqpType = mmdc.GetEquipmentList().ToList();
            List<SelectListItem> MEqpTypeCode = new List<SelectListItem>();
            List<SelectListItem> MEqpTypeName = new List<SelectListItem>();
            int countE = 1;

            foreach (var code in LEqpType)
            {
                MEqpTypeCode.Add(new SelectListItem
                {
                    Text = code.EqpType + '-' + code.EqTypeDesc,
                    Value = code.EqpType
                });
                countE++;
            }

            LabModel.drpEqupList = MEqpTypeCode;


            List<LaborRate> LRATEList = null;
            var data = LRATEList;
            LRATEList = mmdc.GetLaborRateEditDetail(ContID).ToList();
            data = (from e in LRATEList
                    select new LaborRate
                    {

                        LaborRateID = e.LaborRateID,

                        ShopCode = e.ShopCode,
                        CustomerCode = e.CustomerCode,
                        EqpType = e.EqpType,
                        EffDate = e.EffDate,
                        ExpDate = e.ExpDate,
                        RegularRT = e.RegularRT,
                        OvertimeRT = e.OvertimeRT,
                        DoubleTimeRT = e.DoubleTimeRT,
                        MiscRT = e.MiscRT,
                        ChangeUser = e.ChangeUser,
                        ChangeTime = e.ChangeTime
                    }).ToList();

            data.OrderBy(li => li.LaborRateID);

            if (data.Count > 0)
            {
                Shop sdata = LshopListA.Find(Pid => Pid.ShopCode == data[0].ShopCode);
                //data[0].Shop.CUCDN = sdata.CUCDN;

                LabModel.LaborRateID = data[0].LaborRateID;
                LabModel.CurrCode = sdata.CUCDN;
                LabModel.ShopListA = data[0].ShopCode;
                LabModel.CustomerCode = data[0].CustomerCode;
                LabModel.EqupList = data[0].EqpType;
                LabModel._sEffdate = data[0].EffDate.ToString("yyyy-MM-dd");
                LabModel._sExpdate = data[0].ExpDate.ToString("yyyy-MM-dd");
                LabModel.RegularRate = data[0].RegularRT.Value.ToString("0.##");
                LabModel.DoubletimeRate = data[0].DoubleTimeRT.Value.ToString("0.##");
                LabModel.OvertimeRate = data[0].OvertimeRT.Value.ToString("0.##");
                LabModel.MiscRate = data[0].MiscRT.Value.ToString("0.##");
                LabModel.ChangeUser = data[0].ChangeUser;
                LabModel.ChangeTime = data[0].ChangeTime.ToString("yyyy-MM-dd hh:mm:ss tt");

            }

            return View("LaborRateEdit", LabModel);
        }

        public ActionResult EditLaborRate(int LaborID, string ShopID, string CustomerID, string EqType, string EFFDate, string EXPDate, string OrdRate, string OT1, string OT2, string OT3)
        {
            TempData["Msg"] = "";
            LaborRate LrateToBeCreated = new LaborRate();
            //List<Shop> Slist = mmdc.GetUserShopList().ToList();
            //Shop sdata = Slist.Find(Pid => Pid.ShopCode == ShopCode);


            string data1 = "NO";

            LrateToBeCreated.ShopCode = ShopID;
            LrateToBeCreated.CustomerCode = CustomerID;
            LrateToBeCreated.EqpType = EqType;
            LrateToBeCreated.LaborRateID = LaborID;
            LrateToBeCreated.EffDate = Convert.ToDateTime(EFFDate);
            LrateToBeCreated.ExpDate = Convert.ToDateTime(EXPDate);
            LrateToBeCreated.RegularRT = Convert.ToDecimal(OrdRate);
            LrateToBeCreated.OvertimeRT = Convert.ToDecimal(OT1);
            LrateToBeCreated.DoubleTimeRT = Convert.ToDecimal(OT2);
            LrateToBeCreated.MiscRT = Convert.ToDecimal(OT3);
            LrateToBeCreated.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;
            LrateToBeCreated.ChangeTime = DateTime.Now;


            data1 = mmdc.ModifyLaborRate(LrateToBeCreated);
            var Curr = "";
            var Result = new { data = data1, id = Curr };
            return Json(Result, JsonRequestBehavior.AllowGet);
            // return Json(data);
        }
        #endregion

        #region Customer

        [HttpPost]
        public ActionResult GetAllDetailsForCustomer(string id)
        {
            var request = HttpContext.Request;

            ManageMasterDataCustomerModel model = new ManageMasterDataCustomerModel();
            List<Customer> customerList = mmdc.GetCustomerDetailsList().ToList();
            Customer data = customerList.Find(Cid => Cid.CustomerCode == id);
            string userName = mmdc.GetUserName(data.ChangeUser);
            data.ChangeUser = userName;
            return Json(data);
        }

        #region EditCustomer

        #region EditCustomer HTTPGet

        public ActionResult ViewCustomerDetails([Bind] ManageMasterDataCustomerModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsUpdate = true;
            List<SelectListItem> CustomerCode = new List<SelectListItem>();
            List<SelectListItem> ManualCode = new List<SelectListItem>();
            List<SelectListItem> CustActvSwtch = new List<SelectListItem>();

            ManualCode = PopulateManualCodeDropDown();
            CustomerCode = PopulateCustomerDropDown();
            CustActvSwtch = PopulateSwitchDropDown(CustActvSwtch, string.Empty);


            ManageMasterDataModel.drpCustomerList = CustomerCode;
            ManageMasterDataModel.drpManualList = ManualCode;
            ManageMasterDataModel.drpCustActvSwtchList = CustActvSwtch;

            return View("ViewCustomerDetails", ManageMasterDataModel);
        }
        public ActionResult ViewReturCustomerDetails()
        {
            ManageMasterDataCustomerModel ManageMasterDataModel = new ManageMasterDataCustomerModel();
            ManageMasterDataModel.IsUpdate = true;
            List<SelectListItem> CustomerCode = new List<SelectListItem>();
            List<SelectListItem> ManualCode = new List<SelectListItem>();
            List<SelectListItem> CustActvSwtch = new List<SelectListItem>();

            ManualCode = PopulateManualCodeDropDown();
            CustomerCode = PopulateCustomerDropDown();
            CustActvSwtch = PopulateSwitchDropDown(CustActvSwtch, string.Empty);


            ManageMasterDataModel.drpCustomerList = CustomerCode;
            ManageMasterDataModel.drpManualList = ManualCode;
            ManageMasterDataModel.drpCustActvSwtchList = CustActvSwtch;

            return View("ViewCustomerDetails", ManageMasterDataModel);

        }

        public ActionResult ViewAllDetailsForCustomer([Bind] ManageMasterDataCustomerModel manageMasterDataModel)
        {


            manageMasterDataModel.IsUpdate = true;
            List<SelectListItem> CustomerCode = new List<SelectListItem>();
            List<SelectListItem> ManualCode = new List<SelectListItem>();
            List<SelectListItem> CustActvSwtch = new List<SelectListItem>();

            ManualCode = PopulateManualCodeDropDown();
            CustomerCode = PopulateCustomerDropDown();
            CustActvSwtch = PopulateSwitchDropDown(CustActvSwtch, string.Empty);


            manageMasterDataModel.drpCustomerList = CustomerCode;
            manageMasterDataModel.drpManualList = ManualCode;
            manageMasterDataModel.drpCustActvSwtchList = CustActvSwtch;
            List<Customer> customerList = mmdc.GetCustomerDetailsList().ToList();
            Customer data = customerList.Find(Cid => Cid.CustomerCode == manageMasterDataModel.CustomerCode);
            manageMasterDataModel.CustomerCode = data.CustomerCode;
            manageMasterDataModel.CustomerName = data.CustomerDesc;
            manageMasterDataModel.ManualCode = data.ManualCode;
            manageMasterDataModel.CustomerActiveSw = data.CustomerActiveSw;
            manageMasterDataModel.changeTimeCust = (data.ChangeTime).ToString("yyyy-MM-dd hh:mm:ss tt");
            string userName = mmdc.GetUserName(data.ChangeUser);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    manageMasterDataModel.changeFUserCust = userName.Split('|')[0];
                    manageMasterDataModel.changeLUserCust = userName.Split('|')[1];
                }
                else
                {
                    manageMasterDataModel.changeFUserCust = userName;
                    manageMasterDataModel.changeLUserCust = "";
                }
            }
            else
            {
                manageMasterDataModel.changeFUserCust = "";
                manageMasterDataModel.changeLUserCust = "";
            }

            return View("ViewCustomerDetails", manageMasterDataModel);
        }


        #endregion EditCustomer HTTPGet

        #region EditCustomer HTTPPost

        [HttpPost]
        public ActionResult UpdateCustomer([Bind] ManageMasterDataCustomerModel manageMasterDataModel)
        {
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            manageMasterDataModel.IsUpdate = true;
            Customer customerListToBeUpdated = new Customer();
            customerListToBeUpdated.CustomerCode = manageMasterDataModel.CustomerCode;
            customerListToBeUpdated.CustomerDesc = manageMasterDataModel.CustomerName;
            customerListToBeUpdated.ManualCode = manageMasterDataModel.ManualCode;
            customerListToBeUpdated.CustomerActiveSw = manageMasterDataModel.CustomerActiveSw;
            customerListToBeUpdated.ChangeUser = userId.ToString();
            manageMasterDataModel.changeTimeCust = "";
            bool result = mmdc.UpdateCustomer(customerListToBeUpdated);
            if (result)
            {
                ErrorMessage = "Customer " + manageMasterDataModel.CustomerCode.ToUpper() + " Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
            }
            else
            {
                ErrorMessage = "Customer Data Updation failed";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
            }
            string userName = mmdc.GetUserName(customerListToBeUpdated.ChangeUser);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    manageMasterDataModel.changeFUserCust = userName.Split('|')[0];
                    manageMasterDataModel.changeLUserCust = userName.Split('|')[1];
                }
                else
                {
                    manageMasterDataModel.changeFUserCust = userName;
                    manageMasterDataModel.changeLUserCust = "";
                }
            }
            else
            {
                manageMasterDataModel.changeFUserCust = "";
                manageMasterDataModel.changeLUserCust = "";
            }
            List<SelectListItem> CustomerCode = new List<SelectListItem>();
            List<SelectListItem> ManualCode = new List<SelectListItem>();
            List<SelectListItem> CustActvSwtch = new List<SelectListItem>();
            ManualCode = PopulateManualCodeDropDown();
            CustomerCode = PopulateCustomerDropDown();
            CustActvSwtch = PopulateSwitchDropDown(CustActvSwtch, string.Empty);

            manageMasterDataModel.drpCustomerList = CustomerCode;
            manageMasterDataModel.drpManualList = ManualCode;
            manageMasterDataModel.drpCustActvSwtchList = CustActvSwtch;
            manageMasterDataModel.changeUserCust = "";
            manageMasterDataModel.changeTimeCust = "";


            manageMasterDataModel.IsUpdate = true;

            return RedirectToAction("ViewAllDetailsForCustomer", manageMasterDataModel);
        }
        #endregion EditCustomer HTTPPost
        #endregion EditCustomer

        #region CreateCustomer
        #region CreateCustomer HTTPGet

        //[HttpPost]
        public ActionResult CustomerCreate()
        {
            ManageMasterDataCustomerModel ManageMasterDataModel = new ManageMasterDataCustomerModel();
            ManageMasterDataModel.IsUpdate = false;
            List<SelectListItem> ManualCode = new List<SelectListItem>();
            List<SelectListItem> CustActvSwtch = new List<SelectListItem>();

            ManualCode = PopulateManualCodeDropDown();
            ManageMasterDataModel.drpManualList = ManualCode;

            ViewData["drpCustActvSwtch"] = PopulateSwitchDropDown(CustActvSwtch, string.Empty);
            ManageMasterDataModel.drpCustActvSwtchList = CustActvSwtch;

            return View("ViewCustomerDetails", ManageMasterDataModel);
        }
        #endregion CreateCustomer HTTPGet

        #region CreateCustomer HTTPPost

        [HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "Add")]
        public ActionResult CreateCustomer([Bind] ManageMasterDataCustomerModel manageMasterDataModel)
        {
            bool success = false;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            Customer customerListToBeCreated = new Customer();
            customerListToBeCreated.CustomerCode = manageMasterDataModel.CustomerCode;
            customerListToBeCreated.CustomerDesc = manageMasterDataModel.CustomerName;
            customerListToBeCreated.ManualCode = manageMasterDataModel.ManualCode;
            customerListToBeCreated.CustomerActiveSw = manageMasterDataModel.CustomerActiveSw;
            customerListToBeCreated.ChangeUser = userId;
            success = mmdc.CreateCustomer(customerListToBeCreated, ref ErrorMessage);
            if (success)
            {
                ErrorMessage = "Customer " + manageMasterDataModel.CustomerCode.ToUpper() + " Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
                manageMasterDataModel.IsUpdate = true;
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                manageMasterDataModel.IsUpdate = false;
            }
            string userName = mmdc.GetUserName(customerListToBeCreated.ChangeUser);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    manageMasterDataModel.changeFUserCust = userName.Split('|')[0];
                    manageMasterDataModel.changeLUserCust = userName.Split('|')[1];
                }
                else
                {
                    manageMasterDataModel.changeFUserCust = userName;
                    manageMasterDataModel.changeLUserCust = "";
                }
            }
            else
            {
                manageMasterDataModel.changeFUserCust = "";
                manageMasterDataModel.changeLUserCust = "";
            }
            List<SelectListItem> CustomerCode = new List<SelectListItem>();
            List<SelectListItem> ManualCode = new List<SelectListItem>();
            List<SelectListItem> CustActvSwtch = new List<SelectListItem>();
            ManualCode = PopulateManualCodeDropDown();
            CustomerCode = PopulateCustomerDropDown();
            CustActvSwtch = PopulateSwitchDropDown(CustActvSwtch, string.Empty);


            manageMasterDataModel.drpCustomerList = CustomerCode;
            manageMasterDataModel.drpManualList = ManualCode;
            manageMasterDataModel.drpCustActvSwtchList = CustActvSwtch;
            manageMasterDataModel.CustomerCode = customerListToBeCreated.CustomerCode;
            manageMasterDataModel.changeUserCust = userName;
            manageMasterDataModel.changeTimeCust = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");

            return View("ViewCustomerDetails", manageMasterDataModel);
        }
        #endregion CreateCustomer HTTPPost
        #endregion CreateCustomer




        #endregion Customer

        #region Location

        public ActionResult ViewLocationdetails([Bind] ManageMasterLocationModel ManageMasterDataModel)
        {
            List<SelectListItem> RegionCodeList = new List<SelectListItem>();
            List<SelectListItem> ContactEqsalSW = new List<SelectListItem>();
            RegionCodeList = PopulateRegionCodeDropDown();
            ContactEqsalSW = PopulateSwitchDropDown(ContactEqsalSW, string.Empty);

            ManageMasterDataModel.drpRegionCode = RegionCodeList;
            ManageMasterDataModel.drpContactEqsalSW = ContactEqsalSW;

            ViewData["RegionCodesList"] = RegionCodeList;
            ViewData["ContactEqsalSW"] = ContactEqsalSW;
            return View("ViewLocationdetails", ManageMasterDataModel);
        }

        public ActionResult ViewLocationdetails_View([Bind] ManageMasterLocationModel ManageMasterDataModel) //Debadrita_User_Remapping
        {
            List<SelectListItem> RegionCodeList = new List<SelectListItem>();
            List<SelectListItem> ContactEqsalSW = new List<SelectListItem>();
            RegionCodeList = PopulateRegionCodeDropDown();
            ContactEqsalSW = PopulateSwitchDropDown(ContactEqsalSW, string.Empty);

            ManageMasterDataModel.drpRegionCode = RegionCodeList;
            ManageMasterDataModel.drpContactEqsalSW = ContactEqsalSW;

            ViewData["RegionCodesList"] = RegionCodeList;
            ViewData["ContactEqsalSW"] = ContactEqsalSW;
            return View("ViewLocationdetails_View", ManageMasterDataModel);
        }


        public ActionResult ViewAllDetailsLocation([Bind] ManageMasterLocationModel manageMasterDataModel)
        {
            List<SelectListItem> RegionCodeList = new List<SelectListItem>();
            List<SelectListItem> ContactEqsalSW = new List<SelectListItem>();
            RegionCodeList = PopulateRegionCodeDropDown();
            ContactEqsalSW = PopulateSwitchDropDown(ContactEqsalSW, string.Empty);

            manageMasterDataModel.drpRegionCode = RegionCodeList;
            manageMasterDataModel.drpContactEqsalSW = ContactEqsalSW;

            List<Location> locationList = mmdc.GetLocationListByLocationCode(manageMasterDataModel.LocCode.Trim()).ToList();
            Location data = locationList.Find(Lid => Lid.LocCode.Trim().ToLower() == manageMasterDataModel.LocCode.Trim().ToLower());
            if (data != null)
            {
                manageMasterDataModel.LocCodeQuery = data.LocCode.Trim();
                manageMasterDataModel.LocCode = data.LocCode.Trim();
                manageMasterDataModel.LocDesc = data.LocDesc;
                manageMasterDataModel.CountryLocCode = data.CountryCode;
                manageMasterDataModel.ContactEqsalSW = data.ContactEqsalSW;
                manageMasterDataModel.RegionCode = data.RegionCode;
                manageMasterDataModel.ChangeTimeLoc = (data.ChangeTimeLoc).ToString("yyyy-MM-dd hh:mm:ss tt");
                string userName = mmdc.GetUserName(data.ChangeUserLoc);
                if ((userName).Contains('|'))
                {
                    manageMasterDataModel.ChangeUserLoc = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                }
                else if (userName == "0")
                {

                    manageMasterDataModel.ChangeUserLoc = "";
                }
                else
                {
                    manageMasterDataModel.ChangeUserLoc = data.ChangeUserLoc;
                }

            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                manageMasterDataModel.Flag = false;
            }


            return View("ViewLocationdetails", manageMasterDataModel);
        }

        public ActionResult ViewAllDetailsLocation_View([Bind] ManageMasterLocationModel manageMasterDataModel)
        {
            List<SelectListItem> RegionCodeList = new List<SelectListItem>();
            List<SelectListItem> ContactEqsalSW = new List<SelectListItem>();
            RegionCodeList = PopulateRegionCodeDropDown();
            ContactEqsalSW = PopulateSwitchDropDown(ContactEqsalSW, string.Empty);

            manageMasterDataModel.drpRegionCode = RegionCodeList;
            manageMasterDataModel.drpContactEqsalSW = ContactEqsalSW;

            List<Location> locationList = mmdc.GetLocationListByLocationCode(manageMasterDataModel.LocCode.Trim()).ToList();
            Location data = locationList.Find(Lid => Lid.LocCode.Trim().ToLower() == manageMasterDataModel.LocCode.Trim().ToLower());
            if (data != null)
            {
                manageMasterDataModel.LocCodeQuery = data.LocCode.Trim();
                manageMasterDataModel.LocCode = data.LocCode.Trim();
                manageMasterDataModel.LocDesc = data.LocDesc;
                manageMasterDataModel.CountryLocCode = data.CountryCode;
                manageMasterDataModel.ContactEqsalSW = data.ContactEqsalSW;
                manageMasterDataModel.RegionCode = data.RegionCode;
                manageMasterDataModel.ChangeTimeLoc = (data.ChangeTimeLoc).ToString("yyyy-MM-dd hh:mm:ss tt");
                string userName = mmdc.GetUserName(data.ChangeUserLoc);
                if ((userName).Contains('|'))
                {
                    manageMasterDataModel.ChangeUserLoc = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                }
                else if (userName == "0")
                {

                    manageMasterDataModel.ChangeUserLoc = "";
                }
                else
                {
                    manageMasterDataModel.ChangeUserLoc = data.ChangeUserLoc;
                }

            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                manageMasterDataModel.Flag = false;
            }


            return View("ViewLocationdetails_View", manageMasterDataModel);
        }


        #region EditLocation

        #region EditLocation HTTPGet


        public ActionResult LocationUpdate([Bind] ManageMasterLocationModel mmdl)
        {
            List<Location> locationList = mmdc.GetLocationListByLocationCode(mmdl.LocCode.Trim()).ToList();
            Location data = locationList.Find(Lid => Lid.LocCode.Trim().ToLower() == mmdl.LocCode.Trim().ToLower());
            if (data != null)
            {
                mmdl.Flag = true;
                mmdl.LocCode = data.LocCode.Trim();
                mmdl.LocDesc = "";
                mmdl.CountryLocCode = "";
                mmdl.ContactEqsalSW = "";
                mmdl.RegionCode = "";
                mmdl.ChangeUserLoc = "";
                mmdl.ChangeTimeLoc = "";
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                mmdl.Flag = false;
            }
            List<SelectListItem> RegionCodeList = new List<SelectListItem>();
            List<SelectListItem> ContactEqsalSW = new List<SelectListItem>();
            RegionCodeList = PopulateRegionCodeDropDown();
            ContactEqsalSW = PopulateSwitchDropDown(ContactEqsalSW, string.Empty);

            mmdl.drpRegionCode = RegionCodeList;
            mmdl.drpContactEqsalSW = ContactEqsalSW;

            mmdl.IsUpdateLoc = true;
            if (mmdl.Flag)
            {
                return RedirectToAction("ViewAllDetailsLocation", mmdl);
            }
            else
            {
                return View("ViewLocationdetails", mmdl);
            }
        }

        public ActionResult LocationUpdate_View([Bind] ManageMasterLocationModel mmdl)
        {
            List<Location> locationList = mmdc.GetLocationListByLocationCode(mmdl.LocCode.Trim()).ToList();
            Location data = locationList.Find(Lid => Lid.LocCode.Trim().ToLower() == mmdl.LocCode.Trim().ToLower());
            if (data != null)
            {
                mmdl.Flag = true;
                mmdl.LocCode = data.LocCode.Trim();
                mmdl.LocDesc = "";
                mmdl.CountryLocCode = "";
                mmdl.ContactEqsalSW = "";
                mmdl.RegionCode = "";
                mmdl.ChangeUserLoc = "";
                mmdl.ChangeTimeLoc = "";
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                mmdl.Flag = false;
            }
            List<SelectListItem> RegionCodeList = new List<SelectListItem>();
            List<SelectListItem> ContactEqsalSW = new List<SelectListItem>();
            RegionCodeList = PopulateRegionCodeDropDown();
            ContactEqsalSW = PopulateSwitchDropDown(ContactEqsalSW, string.Empty);

            mmdl.drpRegionCode = RegionCodeList;
            mmdl.drpContactEqsalSW = ContactEqsalSW;

            mmdl.IsUpdateLoc = true;
            if (mmdl.Flag)
            {
                return RedirectToAction("ViewAllDetailsLocation_View", mmdl);
            }
            else
            {
                return View("ViewLocationdetails_View", mmdl);
            }
        }


        #endregion EditLocation HTTPGet

        #region EditLocation HTTPPost

        [HttpPost]
        public ActionResult UpdateLocation([Bind] ManageMasterLocationModel manageMasterDataModel)
        {
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            Location LocationListToBeUpdated = new Location();
            LocationListToBeUpdated.LocCode = manageMasterDataModel.LocCode;
            LocationListToBeUpdated.RegionCode = manageMasterDataModel.RegionCode;
            LocationListToBeUpdated.ContactEqsalSW = manageMasterDataModel.ContactEqsalSW;
            LocationListToBeUpdated.ChangeUserLoc = userId;
            bool result = mmdc.UpdateLocation(LocationListToBeUpdated);
            if (result)
            {
                ErrorMessage = "Location " + manageMasterDataModel.LocCode.ToUpper() + " Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
            }
            else
            {
                ErrorMessage = "Location Data Updation failed";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
            }
            List<SelectListItem> RegionCodeList = new List<SelectListItem>();
            List<SelectListItem> ContactEqsalSW = new List<SelectListItem>();
            RegionCodeList = PopulateRegionCodeDropDown();
            ContactEqsalSW = PopulateSwitchDropDown(ContactEqsalSW, string.Empty);

            manageMasterDataModel.drpRegionCode = RegionCodeList;
            manageMasterDataModel.drpContactEqsalSW = ContactEqsalSW;
            manageMasterDataModel.LocDesc = "";
            manageMasterDataModel.RegionCode = "";
            manageMasterDataModel.ContactEqsalSW = "";
            manageMasterDataModel.CountryLocCode = "";
            manageMasterDataModel.ChangeUserLoc = "";
            manageMasterDataModel.ChangeTimeLoc = "";
            manageMasterDataModel.IsUpdateLoc = true;
            return RedirectToAction("LocationUpdate", manageMasterDataModel);
        }
        #endregion EditLocation HTTPPost
        #endregion EditLocation




        #endregion Location

        #region Country


        #region EditCountry



        public ActionResult ViewCountrydetails(List<Country> CountryList = null)
        {
            ManageMasterDataCountryModel ManageMasterDataModel = new ManageMasterDataCountryModel();
            List<SelectListItem> CountryCode = new List<SelectListItem>();
            CountryCode = PopulateCountryCodeDropDown();
            ManageMasterDataModel.drpCountryList = CountryCode;
            ViewData["CountryCodeList"] = CountryCode;

            return View("ViewCountrydetails", ManageMasterDataModel);
        }

        public ActionResult ViewCountrydetails_View(List<Country> CountryList = null)
        {
            ManageMasterDataCountryModel ManageMasterDataModel = new ManageMasterDataCountryModel();
            List<SelectListItem> CountryCode = new List<SelectListItem>();
            CountryCode = PopulateCountryCodeDropDown();
            ManageMasterDataModel.drpCountryList = CountryCode;
            ViewData["CountryCodeList"] = CountryCode;

            return View("ViewCountrydetails_View", ManageMasterDataModel);
        }

        #region EditCountry HTTPGet

        public ActionResult ViewAllCountrydetails([Bind] ManageMasterDataCountryModel ManageMasterDataModel)
        {
            List<SelectListItem> CountryCode = new List<SelectListItem>();
            CountryCode = PopulateCountryCodeDropDown();
            ManageMasterDataModel.drpCountryList = CountryCode;
            List<Country> CountryList = mmdc.GetRegionByCountryCode(ManageMasterDataModel.CountryCode).ToList();
            Country data = CountryList.Find(Cnid => Cnid.CountryCode == ManageMasterDataModel.CountryCode);
            ManageMasterDataModel.CountryCode = data.CountryCode.ToUpper();
            ManageMasterDataModel.CountryDescription = data.CountryDescription;
            ManageMasterDataModel.AreaCode = data.AreaCode;
            ManageMasterDataModel.RepairLimitAdjFactor = data.RepairLimitAdjFactor;
            ManageMasterDataModel.ChangeTimeCon = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");
            string userName = mmdc.GetUserName(data.ChangeUserCon);
            if ((userName) != null)
            {
                if ((userName).Contains('|'))
                    ManageMasterDataModel.ChangeUserCon = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                else if (userName == "0")
                {
                    ManageMasterDataModel.ChangeUserCon = "";
                }
                else
                {
                    ManageMasterDataModel.ChangeUserCon = data.ChangeUserCon;
                }
            }
            else
            {
                ManageMasterDataModel.ChangeUserCon = "";
            }

            ManageMasterDataModel.IsShow = true;

            return View("ViewCountrydetails", ManageMasterDataModel);
        }


        public ActionResult ViewAllCountrydetails_View([Bind] ManageMasterDataCountryModel ManageMasterDataModel)
        {
            List<SelectListItem> CountryCode = new List<SelectListItem>();
            CountryCode = PopulateCountryCodeDropDown();
            ManageMasterDataModel.drpCountryList = CountryCode;
            List<Country> CountryList = mmdc.GetRegionByCountryCode(ManageMasterDataModel.CountryCode).ToList();
            Country data = CountryList.Find(Cnid => Cnid.CountryCode == ManageMasterDataModel.CountryCode);
            ManageMasterDataModel.CountryCode = data.CountryCode.ToUpper();
            ManageMasterDataModel.CountryDescription = data.CountryDescription;
            ManageMasterDataModel.AreaCode = data.AreaCode;
            ManageMasterDataModel.RepairLimitAdjFactor = data.RepairLimitAdjFactor;
            ManageMasterDataModel.ChangeTimeCon = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");
            string userName = mmdc.GetUserName(data.ChangeUserCon);
            if ((userName) != null)
            {
                if ((userName).Contains('|'))
                    ManageMasterDataModel.ChangeUserCon = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                else if (userName == "0")
                {
                    ManageMasterDataModel.ChangeUserCon = "";
                }
                else
                {
                    ManageMasterDataModel.ChangeUserCon = data.ChangeUserCon;
                }
            }
            else
            {
                ManageMasterDataModel.ChangeUserCon = "";
            }

            ManageMasterDataModel.IsShow = true;

            return View("ViewCountrydetails_View", ManageMasterDataModel);
        }

        public ActionResult CountryUpdate([Bind] ManageMasterDataCountryModel ManageMasterDataModel)
        {

            List<Country> CountryList = mmdc.GetRegionByCountryCode(ManageMasterDataModel.CountryCode).ToList();
            Country data = CountryList.Find(Cnid => Cnid.CountryCode == ManageMasterDataModel.CountryCode);
            ManageMasterDataModel.CountryCode = data.CountryCode.ToUpper();
            ManageMasterDataModel.CountryDescription = data.CountryDescription;
            ManageMasterDataModel.AreaCode = data.AreaCode;
            ManageMasterDataModel.RepairLimitAdjFactor = data.RepairLimitAdjFactor;
            ManageMasterDataModel.ChangeTimeCon = (data.ChangeTimeCon).ToString("yyyy-MM-dd hh:mm:ss tt");
            string userName = mmdc.GetUserName(data.ChangeUserCon);
            if ((userName) != null)
            {
                if ((userName).Contains('|'))
                    ManageMasterDataModel.ChangeUserCon = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                else if (userName == "0")
                {
                    ManageMasterDataModel.ChangeUserCon = "";
                }
                else
                {
                    ManageMasterDataModel.ChangeUserCon = data.ChangeUserCon;
                }
            }
            else
            {
                ManageMasterDataModel.ChangeUserCon = "";
            }
            List<SelectListItem> CountryCode = new List<SelectListItem>();
            CountryCode = PopulateCountryCodeDropDown();
            ManageMasterDataModel.drpCountryList = CountryCode;

            ManageMasterDataModel.IsShow = true;
            return RedirectToAction("ViewAllCountrydetails", ManageMasterDataModel);
        }

        public ActionResult CountryUpdate_View([Bind] ManageMasterDataCountryModel ManageMasterDataModel)
        {

            List<Country> CountryList = mmdc.GetRegionByCountryCode(ManageMasterDataModel.CountryCode).ToList();
            Country data = CountryList.Find(Cnid => Cnid.CountryCode == ManageMasterDataModel.CountryCode);
            ManageMasterDataModel.CountryCode = data.CountryCode.ToUpper();
            ManageMasterDataModel.CountryDescription = data.CountryDescription;
            ManageMasterDataModel.AreaCode = data.AreaCode;
            ManageMasterDataModel.RepairLimitAdjFactor = data.RepairLimitAdjFactor;
            ManageMasterDataModel.ChangeTimeCon = (data.ChangeTimeCon).ToString("yyyy-MM-dd hh:mm:ss tt");
            string userName = mmdc.GetUserName(data.ChangeUserCon);
            if ((userName) != null)
            {
                if ((userName).Contains('|'))
                    ManageMasterDataModel.ChangeUserCon = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                else if (userName == "0")
                {
                    ManageMasterDataModel.ChangeUserCon = "";
                }
                else
                {
                    ManageMasterDataModel.ChangeUserCon = data.ChangeUserCon;
                }
            }
            else
            {
                ManageMasterDataModel.ChangeUserCon = "";
            }
            List<SelectListItem> CountryCode = new List<SelectListItem>();
            CountryCode = PopulateCountryCodeDropDown();
            ManageMasterDataModel.drpCountryList = CountryCode;

            ManageMasterDataModel.IsShow = true;
            return RedirectToAction("ViewAllCountrydetails_View", ManageMasterDataModel);
        }

        #endregion EditCountry HTTPGet


        #region EditCountry HTTPPost

        [HttpPost]
        public ActionResult UpdateCountry([Bind] ManageMasterDataCountryModel manageMasterDataModel)
        {
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            Country CountryListToBeUpdated = new Country();
            CountryListToBeUpdated.CountryCode = manageMasterDataModel.CountryCode;
            CountryListToBeUpdated.RepairLimitAdjFactor = manageMasterDataModel.RepairLimitAdjFactor;
            CountryListToBeUpdated.ChangeUserCon = userId;
            bool result = mmdc.UpdateCountry(CountryListToBeUpdated);
            if (result)
            {
                ErrorMessage = "Country " + manageMasterDataModel.CountryCode.ToUpper() + " Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
            }
            else
            {
                ErrorMessage = "Country Data Updation failed";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
            }

            List<SelectListItem> CountryCode = new List<SelectListItem>();
            CountryCode = PopulateCountryCodeDropDown();
            manageMasterDataModel.drpCountryList = CountryCode;
            manageMasterDataModel.ChangeUserCon = "";
            manageMasterDataModel.ChangeTimeCon = "";
            manageMasterDataModel.IsShow = true;

            return RedirectToAction("ViewAllCountrydetails", manageMasterDataModel);
        }

        #endregion EditCountry HTTPPost
        #endregion EditCountry
        #endregion Country


        #region Transmit


        public ActionResult ViewTransmitDetails([Bind] ManageMasterDataTransmitModel ManageMasterDataModel)
        {
            ManageMasterDataModel.IsCust = true;
            ManageMasterDataModel.IsMode = false;
            ManageMasterDataModel.IsUpdate = false;
            ManageMasterDataModel.IsCreate = false;

            List<Transmit> TransmitList = mmdc.GetTransmitDetails().ToList();

            List<SelectListItem> CustomerCodeList = new List<SelectListItem>();
            CustomerCodeList = PopulateCustomerCodeDropDown();
            ManageMasterDataModel.drpCustomerCodeList = CustomerCodeList;
            ViewData["CustomerCodeList"] = CustomerCodeList;

            return View("ViewTransmitDetails", ManageMasterDataModel);

        }
        public ActionResult ViewReturnTransmitDetails()
        {
            ManageMasterDataTransmitModel ManageMasterDataModel = new ManageMasterDataTransmitModel();
            ManageMasterDataModel.IsCust = true;
            ManageMasterDataModel.IsMode = false;
            ManageMasterDataModel.IsUpdate = false;
            ManageMasterDataModel.IsCreate = false;

            List<Transmit> TransmitList = mmdc.GetTransmitDetails().ToList();

            List<SelectListItem> CustomerCodeList = new List<SelectListItem>();
            CustomerCodeList = PopulateCustomerCodeDropDown();
            ManageMasterDataModel.drpCustomerCodeList = CustomerCodeList;
            ViewData["CustomerCodeList"] = CustomerCodeList;

            return View("ViewTransmitDetails", ManageMasterDataModel);

        }
        [HttpPost]
        public ActionResult ViewTransmitModeDetails([Bind] ManageMasterDataTransmitModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsMode = true;
            ManageMasterDataModel.IsCust = false;
            ManageMasterDataModel.IsUpdate = false;
            ManageMasterDataModel.IsCreate = false;

            List<SelectListItem> ModeCodeList = new List<SelectListItem>();
            ModeCodeList = PopulateModeCodeDropDownByCustomerCode(ManageMasterDataModel.Customer);
            ManageMasterDataModel.drpModeLCodeList = ModeCodeList;
            ViewData["ModeCodeList"] = ModeCodeList;

            return View("ViewTransmitDetails", ManageMasterDataModel);


        }

        #region EditTransmit

        #region EditTransmit HTTPGet

        public ActionResult ViewTransmitEditDetails([Bind] ManageMasterDataTransmitModel ManageMasterDataModel)
        {

            List<Transmit> TransmitList = new List<Transmit>();
            ManageMasterDataModel.IsUpdate = true;
            ManageMasterDataModel.IsCust = false;
            ManageMasterDataModel.IsMode = false;
            ManageMasterDataModel.IsCreate = false;


            TransmitList = mmdc.GetTransmitDetailsFromCustomerMode(ManageMasterDataModel.Customer, ManageMasterDataModel.ModeCode).ToList();

            List<SelectListItem> RRISSwitchList = new List<SelectListItem>();
            RRISSwitchList = PopulateSwitchDropDown(RRISSwitchList, string.Empty);
            ManageMasterDataModel.drpRRISList = RRISSwitchList;
            ManageMasterDataModel.Customer = TransmitList[0].CustomerCode;
            ManageMasterDataModel.ModeCode = TransmitList[0].ModeCode;
            ManageMasterDataModel.AccountCode = TransmitList[0].AccountCode;
            ManageMasterDataModel.RRISXMITSwitch = TransmitList[0].RRISXMITSwitch;
            string userName = mmdc.GetUserName(TransmitList[0].ChangeUserTransmit);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    ManageMasterDataModel.ChangeFUserTransmit = userName.Split('|')[0];
                    ManageMasterDataModel.ChangeLUserTransmit = userName.Split('|')[1];
                }
                else
                {
                    ManageMasterDataModel.ChangeFUserTransmit = userName;
                    ManageMasterDataModel.ChangeLUserTransmit = "";
                }
            }
            else
            {
                ManageMasterDataModel.ChangeFUserTransmit = "";
                ManageMasterDataModel.ChangeLUserTransmit = "";
            }
            ManageMasterDataModel.ChangeTimeTransmit = TransmitList[0].ChangeTimeTransmit.ToString("yyyy-MM-dd hh:mm:ss tt");
            ViewData["RRISList"] = RRISSwitchList;
            ManageMasterDataModel.IsUpdate = true;
            return View("ViewTransmitDetails", ManageMasterDataModel);


        }

        #endregion EditTransmit HTTPGet

        #region EditTransmit HTTPPost

        [HttpPost]
        public ActionResult UpdateTransmit([Bind] ManageMasterDataTransmitModel manageMasterDataModel)
        {
            manageMasterDataModel.IsUpdate = true;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            Transmit transmitListToBeUpdated = new Transmit();
            transmitListToBeUpdated.CustomerCode = manageMasterDataModel.Customer;
            transmitListToBeUpdated.ModeCode = manageMasterDataModel.ModeCode;
            transmitListToBeUpdated.RRISXMITSwitch = manageMasterDataModel.RRISXMITSwitch;
            transmitListToBeUpdated.AccountCode = manageMasterDataModel.AccountCode;
            transmitListToBeUpdated.ChangeUserTransmit = userId;

            bool result = mmdc.UpdateTransmit(transmitListToBeUpdated);
            if (result)
            {
                ErrorMessage = "Transmit  " + manageMasterDataModel.Customer.ToUpper() + "/" + manageMasterDataModel.ModeCode + " Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
            }
            else
            {
                ErrorMessage = "Customer Data Updation failed";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
            }
            string userName = mmdc.GetUserName(transmitListToBeUpdated.ChangeUserTransmit);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    manageMasterDataModel.ChangeFUserTransmit = userName.Split('|')[0];
                    manageMasterDataModel.ChangeLUserTransmit = userName.Split('|')[1];
                }
                else
                {
                    manageMasterDataModel.ChangeFUserTransmit = userName;
                    manageMasterDataModel.ChangeLUserTransmit = "";
                }
            }
            else
            {
                manageMasterDataModel.ChangeFUserTransmit = "";
                manageMasterDataModel.ChangeLUserTransmit = "";
            }

            List<SelectListItem> RRISSwitchList = new List<SelectListItem>();
            RRISSwitchList = PopulateSwitchDropDown(RRISSwitchList, string.Empty);
            manageMasterDataModel.drpRRISList = RRISSwitchList;
            manageMasterDataModel.ChangeUserTransmit = userName;
            manageMasterDataModel.ChangeTimeTransmit = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");
            manageMasterDataModel.IsUpdate = true;

            return View("ViewTransmitDetails", manageMasterDataModel);
        }
        #endregion EditTransmit HTTPPost

        #endregion EditTransmit

        #region CreateTransmit


        #region CreateTransmit HTTPGet

        //[HttpPost]
        public ActionResult TransmitCreate()
        {
            ManageMasterDataTransmitModel ManageMasterDataModel = new ManageMasterDataTransmitModel();
            ManageMasterDataModel.IsCreate = true;
            ManageMasterDataModel.IsUpdate = false;
            ManageMasterDataModel.IsCust = false;
            ManageMasterDataModel.IsMode = false;

            List<SelectListItem> CustomerCodeList = new List<SelectListItem>();
            List<SelectListItem> ModeCodeList = new List<SelectListItem>();
            List<SelectListItem> RRISSwitchList = new List<SelectListItem>();

            CustomerCodeList = PopulateCustomerCodeDropDown();
            ModeCodeList = PopulateModeCodeDropDown();
            RRISSwitchList = PopulateSwitchDropDown(RRISSwitchList, string.Empty);


            ManageMasterDataModel.drpCustomerCodeList = CustomerCodeList;
            ManageMasterDataModel.drpModeLCodeList = ModeCodeList;
            ManageMasterDataModel.drpRRISList = RRISSwitchList;

            return View("ViewTransmitDetails", ManageMasterDataModel);
        }
        #endregion CreateTransmit HTTPGet

        #region CreateTransmit HTTPPost

        [HttpPost]

        public ActionResult CreateTransmit([Bind] ManageMasterDataTransmitModel manageMasterDataModel)
        {

            bool success = false;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            Transmit transmitListToBeCreated = new Transmit();
            transmitListToBeCreated.CustomerCode = manageMasterDataModel.Customer;
            transmitListToBeCreated.ModeCode = manageMasterDataModel.ModeCode;
            transmitListToBeCreated.RRISXMITSwitch = manageMasterDataModel.RRISXMITSwitch;
            transmitListToBeCreated.AccountCode = manageMasterDataModel.AccountCode;
            transmitListToBeCreated.ChangeUserTransmit = userId;
            success = mmdc.CreateTransmit(transmitListToBeCreated, ref ErrorMessage);
            if (success)
            {
                ErrorMessage = "Transmit " + manageMasterDataModel.Customer + "/" + manageMasterDataModel.ModeCode + " Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
                string userName = mmdc.GetUserName(transmitListToBeCreated.ChangeUserTransmit);
                if (userName != null)
                {
                    if (userName.Contains('|'))
                    {
                        manageMasterDataModel.ChangeFUserTransmit = userName.Split('|')[0];
                        manageMasterDataModel.ChangeLUserTransmit = userName.Split('|')[1];
                    }
                    else
                    {
                        manageMasterDataModel.ChangeFUserTransmit = userName;
                        manageMasterDataModel.ChangeLUserTransmit = "";
                    }
                }
                else
                {
                    manageMasterDataModel.ChangeFUserTransmit = "";
                    manageMasterDataModel.ChangeLUserTransmit = "";
                }
                manageMasterDataModel.IsUpdate = true;
                List<SelectListItem> CustomerCodeList = new List<SelectListItem>();
                List<SelectListItem> ModeCodeList = new List<SelectListItem>();
                List<SelectListItem> RRISSwitchList = new List<SelectListItem>();

                CustomerCodeList = PopulateCustomerCodeDropDown();
                ModeCodeList = PopulateModeCodeDropDown();
                RRISSwitchList = PopulateSwitchDropDown(RRISSwitchList, string.Empty);


                manageMasterDataModel.drpCustomerCodeList = CustomerCodeList;
                manageMasterDataModel.drpModeLCodeList = ModeCodeList;
                manageMasterDataModel.drpRRISList = RRISSwitchList;
                manageMasterDataModel.ChangeUserTransmit = userName;
                manageMasterDataModel.ChangeTimeTransmit = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");

            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");

                manageMasterDataModel.IsCreate = true;
                List<SelectListItem> CustomerCodeList = new List<SelectListItem>();
                List<SelectListItem> ModeCodeList = new List<SelectListItem>();
                List<SelectListItem> RRISSwitchList = new List<SelectListItem>();

                CustomerCodeList = PopulateCustomerCodeDropDown();
                ModeCodeList = PopulateModeCodeDropDown();
                RRISSwitchList = PopulateSwitchDropDown(RRISSwitchList, string.Empty);

                manageMasterDataModel.Customer = transmitListToBeCreated.CustomerCode;
                manageMasterDataModel.ModeCode = transmitListToBeCreated.ModeCode;
                manageMasterDataModel.RRISXMITSwitch = transmitListToBeCreated.RRISXMITSwitch;
                manageMasterDataModel.AccountCode = transmitListToBeCreated.AccountCode;
                manageMasterDataModel.drpCustomerCodeList = CustomerCodeList;
                manageMasterDataModel.drpModeLCodeList = ModeCodeList;
                manageMasterDataModel.drpRRISList = RRISSwitchList;
                manageMasterDataModel.ChangeTimeTransmit = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");
            }

            return View("ViewTransmitDetails", manageMasterDataModel);

        }
        #endregion CreateTransmit HTTPPost



        #endregion CreateTransmit



        #endregion Transmit

        #region Vendor


        [HttpPost]
        public ActionResult GetAllDetailsForVendor(string id)
        {
            var request = HttpContext.Request;

            List<Vendor> vendorList = mmdc.GetVendorList().ToList();
            Vendor data = vendorList.Find(Cid => Cid.VendorCode == id);
            string userName = mmdc.GetUserName(data.ChangeUserVendor);
            data.ChangeUserVendor = userName;
            return Json(data);
        }

        public ActionResult ViewVendorDetails([Bind] ManageMasterDataVendorModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsUpdate = true;
            ManageMasterDataModel.IsChange = true;
            ManageMasterDataModel.IsAdd = false;

            List<SelectListItem> venList = new List<SelectListItem>();
            List<SelectListItem> CountryCodeList = new List<SelectListItem>();
            List<SelectListItem> VenActiveswitch = new List<SelectListItem>();
            venList = PopulateVendorCodeDropDown();
            CountryCodeList = PopulateCountryDropDown();
            VenActiveswitch = PopulateSwitchDropDown(VenActiveswitch, string.Empty);

            ManageMasterDataModel.drpVendorList = venList;
            ManageMasterDataModel.drpVenCountryList = CountryCodeList;
            ManageMasterDataModel.drpVendorSwitchList = VenActiveswitch;

            ViewData["CountryCodeList"] = CountryCodeList;
            ViewData["VendorActiveSwitchList"] = VenActiveswitch;

            return View("ViewVendorDetails", ManageMasterDataModel);

        }

        public ActionResult ViewVendorDetails_View([Bind] ManageMasterDataVendorModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsUpdate = true;
            ManageMasterDataModel.IsChange = true;
            ManageMasterDataModel.IsAdd = false;

            List<SelectListItem> venList = new List<SelectListItem>();
            List<SelectListItem> CountryCodeList = new List<SelectListItem>();
            List<SelectListItem> VenActiveswitch = new List<SelectListItem>();
            venList = PopulateVendorCodeDropDown();
            CountryCodeList = PopulateCountryDropDown();
            VenActiveswitch = PopulateSwitchDropDown(VenActiveswitch, string.Empty);

            ManageMasterDataModel.drpVendorList = venList;
            ManageMasterDataModel.drpVenCountryList = CountryCodeList;
            ManageMasterDataModel.drpVendorSwitchList = VenActiveswitch;

            ViewData["CountryCodeList"] = CountryCodeList;
            ViewData["VendorActiveSwitchList"] = VenActiveswitch;

            return View("ViewVendorDetails_View", ManageMasterDataModel);

        }


        public ActionResult ViewReturnVendorDetails()
        {
            ManageMasterDataVendorModel ManageMasterDataModel = new ManageMasterDataVendorModel();
            ManageMasterDataModel.IsUpdate = true;
            ManageMasterDataModel.IsChange = true;
            ManageMasterDataModel.IsAdd = false;

            List<SelectListItem> venList = new List<SelectListItem>();
            List<SelectListItem> CountryCodeList = new List<SelectListItem>();
            List<SelectListItem> VenActiveswitch = new List<SelectListItem>();
            venList = PopulateVendorCodeDropDown();
            CountryCodeList = PopulateCountryDropDown();
            VenActiveswitch = PopulateSwitchDropDown(VenActiveswitch, string.Empty);

            ManageMasterDataModel.drpVendorList = venList;
            ManageMasterDataModel.drpVenCountryList = CountryCodeList;
            ManageMasterDataModel.drpVendorSwitchList = VenActiveswitch;

            ViewData["CountryCodeList"] = CountryCodeList;
            ViewData["VendorActiveSwitchList"] = VenActiveswitch;

            return View("ViewVendorDetails", ManageMasterDataModel);

        }

        public ActionResult ViewAllDetailsForVendor([Bind] ManageMasterDataVendorModel manageMasterDataModel)
        {

            List<Vendor> vendorList = mmdc.GetVendorList().ToList();
            Vendor data = vendorList.Find(Cid => Cid.VendorCode == manageMasterDataModel.VendorCode);
            string userName = mmdc.GetUserName(manageMasterDataModel.ChangeUserVendor);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    manageMasterDataModel.ChangeFUserVendor = userName.Split('|')[0];
                    manageMasterDataModel.ChangeLUserVendor = userName.Split('|')[1];
                }
                else
                {
                    manageMasterDataModel.ChangeFUserVendor = userName;
                    manageMasterDataModel.ChangeLUserVendor = "";
                }
            }
            else
            {
                manageMasterDataModel.ChangeFUserVendor = "";
                manageMasterDataModel.ChangeLUserVendor = "";
            }
            List<SelectListItem> VendorList = new List<SelectListItem>();
            List<SelectListItem> CountryCodeList = new List<SelectListItem>();
            List<SelectListItem> VenActiveswitch = new List<SelectListItem>();
            VendorList = PopulateVendorCodeDropDown().ToList();
            CountryCodeList = PopulateCountryDropDown();
            VenActiveswitch = PopulateSwitchDropDown(VenActiveswitch, string.Empty);
            manageMasterDataModel.drpVendorList = VendorList;
            manageMasterDataModel.drpVenCountryList = CountryCodeList;
            manageMasterDataModel.drpVendorSwitchList = VenActiveswitch;
            manageMasterDataModel.VendorCode = data.VendorCode;
            manageMasterDataModel.VenCountryCode = data.VenCountryCode;
            manageMasterDataModel.VendorActiveSw = data.VendorActiveSw;
            manageMasterDataModel.ChangeTimeVendor = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");

            manageMasterDataModel.IsUpdate = true;
            manageMasterDataModel.IsFlag = false;

            return View("ViewVendorDetails", manageMasterDataModel);
        }

        [HttpPost]
        public ActionResult UpdateVendor([Bind] ManageMasterDataVendorModel manageMasterDataModel)
        {
            manageMasterDataModel.IsUpdate = true;
            manageMasterDataModel.IsChange = true;
            manageMasterDataModel.IsAdd = false;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            Vendor vendorListToBeUpdated = new Vendor();

            vendorListToBeUpdated.VendorCode = manageMasterDataModel.VendorCode;
            vendorListToBeUpdated.VendorDesc = manageMasterDataModel.VendorDesc;
            vendorListToBeUpdated.VenCountryCode = manageMasterDataModel.VenCountryCode;
            vendorListToBeUpdated.VendorActiveSw = manageMasterDataModel.VendorActiveSw;
            vendorListToBeUpdated.ChangeUserVendor = userId;
            bool success = mmdc.UpdateVendor(vendorListToBeUpdated);
            if (success)
            {
                ErrorMessage = "Vendor " + manageMasterDataModel.VendorCode + " Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
            }
            else
            {
                ErrorMessage = "Vendor Updation failed";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
            }

            List<SelectListItem> VendorList = new List<SelectListItem>();
            List<SelectListItem> CountryCodeList = new List<SelectListItem>();
            List<SelectListItem> VenActiveswitch = new List<SelectListItem>();
            VendorList = PopulateVendorCodeDropDown().ToList();
            CountryCodeList = PopulateCountryDropDown();
            VenActiveswitch = PopulateSwitchDropDown(VenActiveswitch, string.Empty);
            manageMasterDataModel.drpVendorList = VendorList;
            manageMasterDataModel.drpVenCountryList = CountryCodeList;
            manageMasterDataModel.drpVendorSwitchList = VenActiveswitch;
            manageMasterDataModel.VendorCode = vendorListToBeUpdated.VendorCode;
            manageMasterDataModel.VenCountryCode = vendorListToBeUpdated.VenCountryCode;
            manageMasterDataModel.VendorActiveSw = vendorListToBeUpdated.VendorActiveSw;
            manageMasterDataModel.ChangeFUserVendor = "";
            manageMasterDataModel.ChangeLUserVendor = "";
            manageMasterDataModel.ChangeTimeVendor = "";
            manageMasterDataModel.ChangeUserVendor = userId;

            manageMasterDataModel.IsUpdate = true;
            manageMasterDataModel.IsFlag = false;

            return RedirectToAction("ViewAllDetailsForVendor", manageMasterDataModel);
        }

        public ActionResult VendorCreate()
        {
            ManageMasterDataVendorModel ManageMasterDataModel = new ManageMasterDataVendorModel();

            ManageMasterDataModel.IsUpdate = false;
            ManageMasterDataModel.IsAdd = true;
            ManageMasterDataModel.IsChange = false;
            ManageMasterDataModel.IsFlag = true;
            List<SelectListItem> CountryCodeList = new List<SelectListItem>();
            List<SelectListItem> VenActiveswitch = new List<SelectListItem>();
            CountryCodeList = PopulateCountryDropDown();
            VenActiveswitch = PopulateSwitchDropDown(VenActiveswitch, string.Empty);

            ManageMasterDataModel.drpVenCountryList = CountryCodeList;
            ManageMasterDataModel.drpVendorSwitchList = VenActiveswitch;

            ViewData["CountryCodeList"] = CountryCodeList;
            ViewData["VendorActiveSwitchList"] = VenActiveswitch;

            return View("ViewVendorDetails", ManageMasterDataModel);
        }

        [HttpPost]
        public ActionResult CreateVendor([Bind] ManageMasterDataVendorModel manageMasterDataModel)
        {

            bool success = false;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            Vendor vendorListToBeCreated = new Vendor();

            vendorListToBeCreated.VendorCode = manageMasterDataModel.VendorCode;
            vendorListToBeCreated.VendorDesc = manageMasterDataModel.VendorDesc;
            vendorListToBeCreated.VenCountryCode = manageMasterDataModel.VenCountryCode;
            vendorListToBeCreated.VendorActiveSw = manageMasterDataModel.VendorActiveSw;
            vendorListToBeCreated.ChangeUserVendor = userId;
            success = mmdc.CreateVendor(vendorListToBeCreated, ref ErrorMessage);
            if (success)
            {
                ErrorMessage = "Vendor " + manageMasterDataModel.VendorCode + " Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
                manageMasterDataModel.IsUpdate = true;
            }
            else
            {

                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                manageMasterDataModel.IsUpdate = false;
            }
            string userName = mmdc.GetUserName(vendorListToBeCreated.ChangeUserVendor);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    manageMasterDataModel.ChangeFUserVendor = userName.Split('|')[0];
                    manageMasterDataModel.ChangeLUserVendor = userName.Split('|')[1];
                }
                else
                {
                    manageMasterDataModel.ChangeFUserVendor = userName;
                    manageMasterDataModel.ChangeLUserVendor = "";
                }
            }
            else
            {
                manageMasterDataModel.ChangeFUserVendor = "";
                manageMasterDataModel.ChangeLUserVendor = "";
            }
            List<SelectListItem> VendorList = new List<SelectListItem>();
            List<SelectListItem> CountryCodeList = new List<SelectListItem>();
            List<SelectListItem> VenActiveswitch = new List<SelectListItem>();
            VendorList = PopulateVendorCodeDropDown().ToList();
            CountryCodeList = PopulateCountryDropDown();
            VenActiveswitch = PopulateSwitchDropDown(VenActiveswitch, string.Empty);
            manageMasterDataModel.drpVendorList = VendorList;
            manageMasterDataModel.drpVenCountryList = CountryCodeList;
            manageMasterDataModel.drpVendorSwitchList = VenActiveswitch;
            manageMasterDataModel.VendorCode = vendorListToBeCreated.VendorCode;
            manageMasterDataModel.VenCountryCode = vendorListToBeCreated.VenCountryCode;
            manageMasterDataModel.VendorActiveSw = vendorListToBeCreated.VendorActiveSw;
            manageMasterDataModel.ChangeTimeVendor = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");

            manageMasterDataModel.IsAdd = true;
            manageMasterDataModel.IsChange = false;
            manageMasterDataModel.IsFlag = true;

            return View("ViewVendorDetails", manageMasterDataModel);
        }

        #endregion Vendor

        #region Suspend

        #region Delete Suspend
        public ActionResult ViewSuspendDetails()
        {
            ManageMasterDataSuspendModel ManageMasterDataModel = new ManageMasterDataSuspendModel();

            ManageMasterDataModel.IsDelShop = true;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;

            List<SelectListItem> ShopList = new List<SelectListItem>();
            ShopList = PopulateShopCodeDropDown();
            ManageMasterDataModel.drpShopCodeList = ShopList;
            ViewData["ShopCodeList"] = ShopList;

            return View("ViewSuspendDetails", ManageMasterDataModel);

        }



        public ActionResult ViewSuspendDetails_View()//Debadrita_User_Remapping
        {
            ManageMasterDataSuspendModel ManageMasterDataModel = new ManageMasterDataSuspendModel();

            ManageMasterDataModel.IsDelShop = true;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;

            List<SelectListItem> ShopList = new List<SelectListItem>();
            ShopList = PopulateShopCodeDropDown();
            ManageMasterDataModel.drpShopCodeList = ShopList;
            ViewData["ShopCodeList"] = ShopList;

            return View("ViewSuspendDetails_View", ManageMasterDataModel);

        }



        public ActionResult ViewSuspendManualDetails([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = true;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;

            List<SelectListItem> ManualCodeSusList = new List<SelectListItem>();
            ManualCodeSusList = PopulateManualCodeDropDownByShopCode(ManageMasterDataModel.ShopCode);
            if (ManualCodeSusList.Count > 0)
            {
                ManageMasterDataModel.drpManualCodeList = ManualCodeSusList;
                ViewData["ManualCodeList"] = ManualCodeSusList;
                ManageMasterDataModel.FlagManual = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                ManageMasterDataModel.FlagManual = false;
            }

            return View("ViewSuspendDetails", ManageMasterDataModel);


        }


        public ActionResult ViewSuspendManualDetails_View([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = true;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;

            List<SelectListItem> ManualCodeSusList = new List<SelectListItem>();
            ManualCodeSusList = PopulateManualCodeDropDownByShopCode(ManageMasterDataModel.ShopCode);
            if (ManualCodeSusList.Count > 0)
            {
                ManageMasterDataModel.drpManualCodeList = ManualCodeSusList;
                ViewData["ManualCodeList"] = ManualCodeSusList;
                ManageMasterDataModel.FlagManual = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                ManageMasterDataModel.FlagManual = false;
            }

            return View("ViewSuspendDetails_View", ManageMasterDataModel);


        }

        [HttpPost]
        public ActionResult ViewSuspendModeDetails([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = true;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;


            List<SelectListItem> ModeCodeList = new List<SelectListItem>();
            ModeCodeList = PopulateModeCodeDropDownByShopCodeManualCode(ManageMasterDataModel.ShopCode, ManageMasterDataModel.Manual);
            if (ModeCodeList.Count > 0)
            {
                ManageMasterDataModel.drpModeSusList = ModeCodeList;
                ViewData["ModeCodeList"] = ModeCodeList;
                ManageMasterDataModel.FlagMode = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                ManageMasterDataModel.FlagMode = false;
            }

            return View("ViewSuspendDetails", ManageMasterDataModel);


        }



        public ActionResult ViewSuspendModeDetails_View([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = true;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;


            List<SelectListItem> ModeCodeList = new List<SelectListItem>();
            ModeCodeList = PopulateModeCodeDropDownByShopCodeManualCode(ManageMasterDataModel.ShopCode, ManageMasterDataModel.Manual);
            if (ModeCodeList.Count > 0)
            {
                ManageMasterDataModel.drpModeSusList = ModeCodeList;
                ViewData["ModeCodeList"] = ModeCodeList;
                ManageMasterDataModel.FlagMode = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                ManageMasterDataModel.FlagMode = false;
            }

            return View("ViewSuspendDetails_View", ManageMasterDataModel);


        }

        [HttpPost]
        public ActionResult ViewSuspendRepairDetails([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = true;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;

            List<SelectListItem> RepairCodeList = new List<SelectListItem>();
            RepairCodeList = PopulateRepairCodeDropDownByShopCodeManualCodeModeCode(ManageMasterDataModel.ShopCode, ManageMasterDataModel.Manual, ManageMasterDataModel.Mode);
            if (RepairCodeList.Count > 0)
            {
                ManageMasterDataModel.drpRepairCodeList = RepairCodeList;
                ViewData["RepairCodeList"] = RepairCodeList;
                ManageMasterDataModel.FlagRepairCode = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                ManageMasterDataModel.FlagRepairCode = false;
            }


            return View("ViewSuspendDetails", ManageMasterDataModel);


        }



        public ActionResult ViewSuspendRepairDetails_View([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = true;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;

            List<SelectListItem> RepairCodeList = new List<SelectListItem>();
            RepairCodeList = PopulateRepairCodeDropDownByShopCodeManualCodeModeCode(ManageMasterDataModel.ShopCode, ManageMasterDataModel.Manual, ManageMasterDataModel.Mode);
            if (RepairCodeList.Count > 0)
            {
                ManageMasterDataModel.drpRepairCodeList = RepairCodeList;
                ViewData["RepairCodeList"] = RepairCodeList;
                ManageMasterDataModel.FlagRepairCode = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                ManageMasterDataModel.FlagRepairCode = false;
            }


            return View("ViewSuspendDetails_View", ManageMasterDataModel);


        }


        public ActionResult ViewSuspendDetailsForDelete([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {
            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = true;
            ManageMasterDataModel.IsCreate = false;

            List<Suspend> SuspendList = new List<Suspend>();
            SuspendList = mmdc.GetSuspendList(ManageMasterDataModel.ShopCode, ManageMasterDataModel.Manual, ManageMasterDataModel.Mode, ManageMasterDataModel.RepairCod).ToList();
            ManageMasterDataModel.ShopCode = SuspendList[0].ShopCode;
            ManageMasterDataModel.Manual = SuspendList[0].ManualCode;
            ManageMasterDataModel.Mode = SuspendList[0].Mode;
            ManageMasterDataModel.RepairCod = SuspendList[0].RepairCode;
            ManageMasterDataModel.SuspendCatID = SuspendList[0].SuspcatID;
            string userName = SuspendList[0].ChangeUserSp;
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    ManageMasterDataModel.ChangeFUserSp = userName.Split('|')[0];
                    ManageMasterDataModel.ChangeLUserSp = userName.Split('|')[1];
                }
                else
                {
                    ManageMasterDataModel.ChangeFUserSp = userName;
                    ManageMasterDataModel.ChangeLUserSp = "";
                }
            }
            else
            {
                ManageMasterDataModel.ChangeFUserSp = "";
                ManageMasterDataModel.ChangeLUserSp = "";
            }
            ManageMasterDataModel.ChangeTimeSp = (SuspendList[0].ChangeTimeSp).ToString("yyyy-MM-dd hh:mm:ss tt");

            return View("ViewSuspendDetails", ManageMasterDataModel);
        }

        public ActionResult ViewSuspendDetailsForDelete_View([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {
            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = true;
            ManageMasterDataModel.IsCreate = false;

            List<Suspend> SuspendList = new List<Suspend>();
            SuspendList = mmdc.GetSuspendList(ManageMasterDataModel.ShopCode, ManageMasterDataModel.Manual, ManageMasterDataModel.Mode, ManageMasterDataModel.RepairCod).ToList();
            ManageMasterDataModel.ShopCode = SuspendList[0].ShopCode;
            ManageMasterDataModel.Manual = SuspendList[0].ManualCode;
            ManageMasterDataModel.Mode = SuspendList[0].Mode;
            ManageMasterDataModel.RepairCod = SuspendList[0].RepairCode;
            ManageMasterDataModel.SuspendCatID = SuspendList[0].SuspcatID;
            string userName = SuspendList[0].ChangeUserSp;
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    ManageMasterDataModel.ChangeFUserSp = userName.Split('|')[0];
                    ManageMasterDataModel.ChangeLUserSp = userName.Split('|')[1];
                }
                else
                {
                    ManageMasterDataModel.ChangeFUserSp = userName;
                    ManageMasterDataModel.ChangeLUserSp = "";
                }
            }
            else
            {
                ManageMasterDataModel.ChangeFUserSp = "";
                ManageMasterDataModel.ChangeLUserSp = "";
            }
            ManageMasterDataModel.ChangeTimeSp = (SuspendList[0].ChangeTimeSp).ToString("yyyy-MM-dd hh:mm:ss tt");

            return View("ViewSuspendDetails_View", ManageMasterDataModel);
        }



        [HttpPost]
        public ActionResult DeleteSuspend([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {
            Suspend SuspndToBeDeleted = new Suspend();
            SuspndToBeDeleted.ShopCode = ManageMasterDataModel.ShopCode;
            SuspndToBeDeleted.Mode = ManageMasterDataModel.Mode;
            SuspndToBeDeleted.ManualCode = ManageMasterDataModel.Manual;
            SuspndToBeDeleted.RepairCode = ManageMasterDataModel.RepairCod;
            bool result = mmdc.DeleteSuspend(SuspndToBeDeleted);
            if (result)
            {
                ErrorMessage = "Suspend Record Deleted";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
                ManageMasterDataModel.IsFlag = true;
            }
            else
            {
                ErrorMessage = "Suspend Record failed";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                ManageMasterDataModel.IsFlag = false;
            }
            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = true;
            ManageMasterDataModel.IsCreate = false;
            ManageMasterDataModel.ChangeTimeSp = Convert.ToString(DateTime.Now);

            return RedirectToAction("ViewSuspendDetails");
        }
        #endregion Delete Suspend


        #region Add Suspend

        public ActionResult SuspendAdd()
        {
            ManageMasterDataSuspendModel ManageMasterDataModel = new ManageMasterDataSuspendModel();
            int userId = ((UserSec)Session["UserSec"]).UserId;

            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = true;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;

            List<SelectListItem> ShopList = new List<SelectListItem>();
            ShopList = PopulateShopCodeByIdDropDown(userId);
            ManageMasterDataModel.drpShopCodeList = ShopList;
            ViewData["ShopCodeList"] = ShopList;
            return View("ViewSuspendDetails", ManageMasterDataModel);

        }

        public ActionResult AddSuspendManualDetails([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = true;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;

            List<SelectListItem> ManualCodeSusList = new List<SelectListItem>();
            ManualCodeSusList = PopulateManualCodeForSuspedDropDown();
            if (ManualCodeSusList.Count > 0)
            {
                ManageMasterDataModel.drpManualCodeList = ManualCodeSusList;
                ViewData["ManualCodeList"] = ManualCodeSusList;
                ManageMasterDataModel.FlagManual = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                ManageMasterDataModel.FlagManual = false;
            }

            return View("ViewSuspendDetails", ManageMasterDataModel);


        }

        [HttpPost]
        public ActionResult AddSuspendModeDetails([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = true;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;


            List<SelectListItem> ModeCodeList = new List<SelectListItem>();
            ModeCodeList = PopulateModeCodeForSuspendDropDown(ManageMasterDataModel.Manual);
            if (ModeCodeList.Count > 0)
            {
                ManageMasterDataModel.drpModeSusList = ModeCodeList;
                ViewData["ModeCodeList"] = ModeCodeList;
                ManageMasterDataModel.FlagMode = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                ManageMasterDataModel.FlagMode = false;
            }
            return View("ViewSuspendDetails", ManageMasterDataModel);


        }


        public ActionResult AddSuspendModeDetails_View([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = true;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;


            List<SelectListItem> ModeCodeList = new List<SelectListItem>();
            ModeCodeList = PopulateModeCodeForSuspendDropDown(ManageMasterDataModel.Manual);
            if (ModeCodeList.Count > 0)
            {
                ManageMasterDataModel.drpModeSusList = ModeCodeList;
                ViewData["ModeCodeList"] = ModeCodeList;
                ManageMasterDataModel.FlagMode = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                ManageMasterDataModel.FlagMode = false;
            }
            return View("ViewSuspendDetails_View", ManageMasterDataModel);


        }


        [HttpPost]
        public ActionResult AddSuspendRepairDetails([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = true;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;
            ManageMasterDataModel.FlagRepairCode = true;

            List<SelectListItem> RepairCodeList = new List<SelectListItem>();
            List<SelectListItem> SuspendCategoryList = new List<SelectListItem>();
            RepairCodeList = PopulateRepairCodeForSuspendDropDown(ManageMasterDataModel.Manual, ManageMasterDataModel.Mode);
            SuspendCategoryList = PopulateSuspendCatDropDown();
            ManageMasterDataModel.drpRepairCodeList = RepairCodeList;
            ManageMasterDataModel.drpSuspendList = SuspendCategoryList;
            ViewData["RepairCodeList"] = RepairCodeList;
            ViewData["SuspendCategoryList"] = SuspendCategoryList;


            return View("ViewSuspendDetails", ManageMasterDataModel);


        }


        public ActionResult AddSuspendRepairDetails_View([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {

            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = true;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;
            ManageMasterDataModel.FlagRepairCode = true;

            List<SelectListItem> RepairCodeList = new List<SelectListItem>();
            List<SelectListItem> SuspendCategoryList = new List<SelectListItem>();
            RepairCodeList = PopulateRepairCodeForSuspendDropDown(ManageMasterDataModel.Manual, ManageMasterDataModel.Mode);
            SuspendCategoryList = PopulateSuspendCatDropDown();
            ManageMasterDataModel.drpRepairCodeList = RepairCodeList;
            ManageMasterDataModel.drpSuspendList = SuspendCategoryList;
            ViewData["RepairCodeList"] = RepairCodeList;
            ViewData["SuspendCategoryList"] = SuspendCategoryList;


            return View("ViewSuspendDetails_View", ManageMasterDataModel);


        }


        //[HttpPost]
        public ActionResult SuspendCreate([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {
            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = true;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;

            List<SelectListItem> RepairCodeList = new List<SelectListItem>();
            List<SelectListItem> SuspendCatList = new List<SelectListItem>();

            ManageMasterDataModel.drpModeSusList = RepairCodeList;
            ViewData["RepairCodeList"] = RepairCodeList;

            return View("ViewSuspendDetails", ManageMasterDataModel);
        }

        [HttpPost]
        public ActionResult CreateSuspend([Bind] ManageMasterDataSuspendModel ManageMasterDataModel)
        {
            ManageMasterDataModel.IsDelShop = false;
            ManageMasterDataModel.IsAddShop = false;
            ManageMasterDataModel.IsDelManual = false;
            ManageMasterDataModel.IsAddManual = false;
            ManageMasterDataModel.IsDelMode = false;
            ManageMasterDataModel.IsAddMode = false;
            ManageMasterDataModel.IsDelRepair = false;
            ManageMasterDataModel.IsAddRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = true;

            bool success = false;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            Suspend suspendToBeCreated = new Suspend();
            suspendToBeCreated.RepairCode = ManageMasterDataModel.RepairCod;
            suspendToBeCreated.SuspcatID = ManageMasterDataModel.SuspendCatID.ToString();
            suspendToBeCreated.ChangeUserSp = userId;
            success = mmdc.CreateSuspend(suspendToBeCreated, ManageMasterDataModel.ShopCode, ManageMasterDataModel.Manual, ManageMasterDataModel.Mode, ref ErrorMessage);
            if (success)
            {
                ErrorMessage = "Suspend Record Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
                ManageMasterDataModel.IsFlag = true;
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                ManageMasterDataModel.IsFlag = false;
            }
            string userName = mmdc.GetUserName(suspendToBeCreated.ChangeUserSp);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    ManageMasterDataModel.ChangeFUserSp = userName.Split('|')[0];
                    ManageMasterDataModel.ChangeLUserSp = userName.Split('|')[1];
                }
                else
                {
                    ManageMasterDataModel.ChangeFUserSp = userName;
                    ManageMasterDataModel.ChangeLUserSp = "";
                }
            }
            else
            {
                ManageMasterDataModel.ChangeFUserSp = "";
                ManageMasterDataModel.ChangeLUserSp = "";
            }

            ManageMasterDataModel.ChangeTimeSp = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");
            return View("ViewSuspendDetails", ManageMasterDataModel);

        }




        #endregion Add Suspend

        #endregion Suspend


        #region SuspendCategory

        [HttpPost]
        public ActionResult GetAllDetailsForSuspendCategory(string id)
        {
            var request = HttpContext.Request;
            List<SuspendCat> SuspendCatList = mmdc.GetSuspendCatList().ToList();
            SuspendCat data = SuspendCatList.Find(Scid => Scid.SuspcatID.ToString() == id);
            string userName = mmdc.GetUserName(data.ChangeUserSus);
            data.ChangeUserSus = userName;
            return Json(data);
        }

        public ActionResult ViewAllSuspendCategoryDetails([Bind] ManageMasterDataSuspendCategoryModel ManageMasterDataModel)
        {
            ManageMasterDataModel.IsUpdate = true;
            ManageMasterDataModel.IsCreate = false;
            ManageMasterDataModel.IsAdd = false;
            List<SuspendCat> SuspendCatList = new List<SuspendCat>();
            SuspendCatList = mmdc.GetSuspendCatList().ToList();
            SuspendCat data = SuspendCatList.Find(Scid => Scid.SuspcatID == ManageMasterDataModel.SuspcatID);
            ManageMasterDataModel.SuspcatID = data.SuspcatID;
            ManageMasterDataModel.SuspcatDesc = data.SuspcatDesc;
            ManageMasterDataModel.ChangeTimeSus = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");
            string userName = mmdc.GetUserName(data.ChangeUserSus);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    ManageMasterDataModel.ChangeFUserSus = userName.Split('|')[0];
                    ManageMasterDataModel.ChangeLUserSus = userName.Split('|')[1];
                }
                else
                {
                    ManageMasterDataModel.ChangeFUserSus = userName;
                    ManageMasterDataModel.ChangeLUserSus = "";
                }
            }
            else
            {
                ManageMasterDataModel.ChangeFUserSus = "";
                ManageMasterDataModel.ChangeLUserSus = "";
            }

            List<SelectListItem> SusCatList = new List<SelectListItem>();
            SusCatList = PopulateSuspendCatDropDown();
            ManageMasterDataModel.drpSuspendCatList = SusCatList;
            ViewData["SuspendCategoryList"] = SuspendCatList;

            return View("ViewSuspendCategoryDetails", ManageMasterDataModel);

        }

        public ActionResult ViewSuspendCategoryDetails([Bind] ManageMasterDataSuspendCategoryModel ManageMasterDataModel)
        {
            ManageMasterDataModel.IsUpdate = true;
            ManageMasterDataModel.IsCreate = false;
            ManageMasterDataModel.IsAdd = false;
            //List<SuspendCat> SuspendCatList = new List<SuspendCat>();
            //SuspendCatList = mmdc.GetSuspendCatList().ToList();
            List<SelectListItem> SuspendCatList = new List<SelectListItem>();

            SuspendCatList = PopulateSuspendCatDropDown();
            ManageMasterDataModel.drpSuspendCatList = SuspendCatList;
            ViewData["SuspendCategoryList"] = SuspendCatList;

            return View("ViewSuspendCategoryDetails", ManageMasterDataModel);

        }
        public ActionResult ReturnSuspendCategoryDetails()
        {
            ManageMasterDataSuspendCategoryModel ManageMasterDataModel = new ManageMasterDataSuspendCategoryModel();
            ManageMasterDataModel.IsUpdate = true;
            ManageMasterDataModel.IsCreate = false;
            ManageMasterDataModel.IsAdd = false;
            //List<SuspendCat> SuspendCatList = new List<SuspendCat>();
            //SuspendCatList = mmdc.GetSuspendCatList().ToList();
            List<SelectListItem> SuspendCatList = new List<SelectListItem>();

            SuspendCatList = PopulateSuspendCatDropDown();
            ManageMasterDataModel.drpSuspendCatList = SuspendCatList;
            ViewData["SuspendCategoryList"] = SuspendCatList;
            ManageMasterDataModel.ChangeFUserSus = "";
            ManageMasterDataModel.ChangeLUserSus = "";
            ManageMasterDataModel.ChangeTimeSus = "";
            ManageMasterDataModel.SuspcatDesc = "";
            return RedirectToAction("ReturnToUpdate", ManageMasterDataModel);

        }
        public ActionResult ReturnToUpdate()
        {
            ManageMasterDataSuspendCategoryModel ManageMasterDataModel = new ManageMasterDataSuspendCategoryModel();
            ManageMasterDataModel.IsUpdate = true;
            ManageMasterDataModel.IsCreate = false;
            ManageMasterDataModel.IsAdd = false;
            //List<SuspendCat> SuspendCatList = new List<SuspendCat>();
            //SuspendCatList = mmdc.GetSuspendCatList().ToList();
            List<SelectListItem> SuspendCatList = new List<SelectListItem>();

            SuspendCatList = PopulateSuspendCatDropDown();
            ManageMasterDataModel.drpSuspendCatList = SuspendCatList;
            ViewData["SuspendCategoryList"] = SuspendCatList;
            ManageMasterDataModel.ChangeFUserSus = "";
            ManageMasterDataModel.ChangeLUserSus = "";
            ManageMasterDataModel.ChangeTimeSus = "";
            ManageMasterDataModel.SuspcatDesc = "";
            return View("ViewSuspendCategoryDetails", ManageMasterDataModel);

        }

        [HttpPost]
        public ActionResult UpdateSuspendCategory([Bind] ManageMasterDataSuspendCategoryModel manageMasterDataModel)
        {

            manageMasterDataModel.IsUpdate = true;
            manageMasterDataModel.IsCreate = false;
            manageMasterDataModel.IsAdd = false;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            SuspendCat SuspendCatToBeUpdated = new SuspendCat();
            SuspendCatToBeUpdated.SuspcatID = manageMasterDataModel.SuspcatID;
            SuspendCatToBeUpdated.SuspcatDesc = manageMasterDataModel.SuspcatDesc;
            SuspendCatToBeUpdated.ChangeUserSus = userId;
            bool result = mmdc.UpdateSuspendCat(SuspendCatToBeUpdated, ref ErrorMessage);
            if (result)
            {
                ErrorMessage = "Suspend Category ID" + SuspendCatToBeUpdated.SuspcatID + " Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
            }
            else
            {

                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
            }
            //string userName = mmdc.GetUserName(SuspendCatToBeUpdated.ChangeUserSus);
            //if (userName != null)
            //{
            //    if (userName.Contains('|'))
            //    {
            //        manageMasterDataModel.ChangeFUserSus= userName.Split('|')[0];
            //        manageMasterDataModel.ChangeLUserSus= userName.Split('|')[1];
            //    }
            //    else
            //    {
            //        manageMasterDataModel.ChangeFUserSus = userName;
            //        manageMasterDataModel.ChangeLUserSus = "";
            //    }
            //}
            //else
            //{
            //    manageMasterDataModel.ChangeFUserSus = "";
            //    manageMasterDataModel.ChangeLUserSus = "";
            //}

            List<SelectListItem> SuspendCatList = new List<SelectListItem>();
            SuspendCatList = PopulateSuspendCatDropDown();
            manageMasterDataModel.drpSuspendCatList = SuspendCatList;
            manageMasterDataModel.ChangeUserSus = userId;
            manageMasterDataModel.ChangeFUserSus = "";
            manageMasterDataModel.ChangeLUserSus = "";
            manageMasterDataModel.ChangeTimeSus = "";
            manageMasterDataModel.IsUpdate = true;

            return RedirectToAction("ViewAllSuspendCategoryDetails", manageMasterDataModel);
        }

        public ActionResult SuspendCategoryCreate()
        {
            ManageMasterDataSuspendCategoryModel ManageMasterDataModel = new ManageMasterDataSuspendCategoryModel();
            ManageMasterDataModel.IsUpdate = false;
            ManageMasterDataModel.IsCreate = true;
            ManageMasterDataModel.IsAdd = false;

            return View("ViewSuspendCategoryDetails", ManageMasterDataModel);
        }

        [HttpPost]
        public ActionResult CreateSuspendCategory([Bind] ManageMasterDataSuspendCategoryModel manageMasterDataModel)
        {

            bool success = false;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            List<SuspendCat> SuspendCatList = new List<SuspendCat>();
            SuspendCat SuspendCatToBecreated = new SuspendCat();
            SuspendCatToBecreated.SuspcatID = manageMasterDataModel.SuspcatID;
            SuspendCatToBecreated.SuspcatDesc = manageMasterDataModel.SuspcatDesc;
            SuspendCatToBecreated.ChangeUserSus = userId;
            success = mmdc.CreateSuspendCat(SuspendCatToBecreated, ref ErrorMessage);


            List<SuspendCat> SuspendCatListFromDB = mmdc.GetSuspendCategoryID(SuspendCatToBecreated).ToList();


            if (success)
            {
                ErrorMessage = "Suspend Category ID " + SuspendCatListFromDB[0].SuspcatID.ToString() + " Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
                manageMasterDataModel.IsUpdate = false;
                manageMasterDataModel.IsCreate = false;
                manageMasterDataModel.IsAdd = true;
                manageMasterDataModel.ChangeTimeSus = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");

            }
            else
            {

                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                manageMasterDataModel.IsUpdate = false;
                manageMasterDataModel.IsCreate = true;
                manageMasterDataModel.IsAdd = false;
            }
            string userName = mmdc.GetUserName(SuspendCatToBecreated.ChangeUserSus);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    manageMasterDataModel.ChangeFUserSus = userName.Split('|')[0];
                    manageMasterDataModel.ChangeLUserSus = userName.Split('|')[1];
                }
                else
                {
                    manageMasterDataModel.ChangeFUserSus = userName;
                    manageMasterDataModel.ChangeLUserSus = "";
                }
            }
            else
            {
                manageMasterDataModel.ChangeFUserSus = "";
                manageMasterDataModel.ChangeLUserSus = "";
            }
            manageMasterDataModel.ChangeTimeSus = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");
            return View("ViewSuspendCategoryDetails", manageMasterDataModel);
        }

        [HttpPost]
        public ActionResult DeleteSuspendCategory([Bind] ManageMasterDataSuspendCategoryModel manageMasterDataModel)
        {
            bool success = false;

            SuspendCat SuspendCatToBeDeleted = new SuspendCat();
            SuspendCatToBeDeleted.SuspcatID = manageMasterDataModel.SuspcatID;

            success = mmdc.DeleteSuspendCat(SuspendCatToBeDeleted.SuspcatID.ToString());
            if (success)
            {
                ErrorMessage = "Suspend Category ID" + manageMasterDataModel.SuspcatID + " Deleted";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
            }
            else
            {
                ErrorMessage = "Suspend Category ID deletion failed";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
            }
            List<SelectListItem> SuspendCatList = new List<SelectListItem>();
            SuspendCatList = PopulateSuspendCatDropDown();
            manageMasterDataModel.drpSuspendCatList = SuspendCatList;
            manageMasterDataModel.IsUpdate = true;
            manageMasterDataModel.IsCreate = false;
            manageMasterDataModel.IsAdd = false;


            return RedirectToAction("ViewSuspendCategoryDetails");
        }


        #endregion SuspendCategory

        #region ExclusiveRepairCode

        public ActionResult ViewExclusiveRepairCodeDetails()
        {
            ManageMasterDataRepairCodeExModel ManageMasterDataModel = new ManageMasterDataRepairCodeExModel();

            ManageMasterDataModel.IsManual = true;
            ManageMasterDataModel.IsMode = false;
            ManageMasterDataModel.IsRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;
            ManageMasterDataModel.IsAdd = false;

            List<SelectListItem> ManualList = new List<SelectListItem>();
            ManualList = PopulateManualRepairCodeDropDown();
            ManageMasterDataModel.drpManCodeList = ManualList;
            ViewData["ManualList"] = ManualList;
            return View("ViewExclusiveRepairCodeDetails", ManageMasterDataModel);
        }

        public ActionResult ViewExclusiveRepairCodeDetails_View()
        {
            ManageMasterDataRepairCodeExModel ManageMasterDataModel = new ManageMasterDataRepairCodeExModel();

            ManageMasterDataModel.IsManual = true;
            ManageMasterDataModel.IsMode = false;
            ManageMasterDataModel.IsRepair = false;
            ManageMasterDataModel.IsDelete = false;
            ManageMasterDataModel.IsCreate = false;
            ManageMasterDataModel.IsAdd = false;

            List<SelectListItem> ManualList = new List<SelectListItem>();
            ManualList = PopulateManualRepairCodeDropDown();
            ManageMasterDataModel.drpManCodeList = ManualList;
            ViewData["ManualList"] = ManualList;
            return View("ViewExclusiveRepairCodeDetails_View", ManageMasterDataModel);
        }

        public ActionResult ViewExclusiveRepairCodeModeDetails([Bind] ManageMasterDataRepairCodeExModel managemasterdatamodel)
        {
            managemasterdatamodel.IsManual = false;
            managemasterdatamodel.IsMode = true;
            managemasterdatamodel.IsRepair = false;
            managemasterdatamodel.IsDelete = false;
            managemasterdatamodel.IsCreate = false;
            managemasterdatamodel.IsAdd = false;

            List<SelectListItem> ModeList = new List<SelectListItem>();
            ModeList = PopulateModeCodeFromManualCode(managemasterdatamodel.ManCode);
            if (ModeList.Count > 0)
            {
                managemasterdatamodel.drpModCodeList = ModeList;
                ViewData["ModeList"] = ModeList;
                managemasterdatamodel.FlagMode = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                managemasterdatamodel.FlagMode = false;
            }
            return View("ViewExclusiveRepairCodeDetails", managemasterdatamodel);

        }

        public ActionResult ViewExclusiveRepairCodeModeDetails_View([Bind] ManageMasterDataRepairCodeExModel managemasterdatamodel)
        {
            managemasterdatamodel.IsManual = false;
            managemasterdatamodel.IsMode = true;
            managemasterdatamodel.IsRepair = false;
            managemasterdatamodel.IsDelete = false;
            managemasterdatamodel.IsCreate = false;
            managemasterdatamodel.IsAdd = false;

            List<SelectListItem> ModeList = new List<SelectListItem>();
            ModeList = PopulateModeCodeFromManualCode(managemasterdatamodel.ManCode);
            if (ModeList.Count > 0)
            {
                managemasterdatamodel.drpModCodeList = ModeList;
                ViewData["ModeList"] = ModeList;
                managemasterdatamodel.FlagMode = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                managemasterdatamodel.FlagMode = false;
            }
            return View("ViewExclusiveRepairCodeDetails_View", managemasterdatamodel);

        }

        public ActionResult ViewRepairDetails([Bind] ManageMasterDataRepairCodeExModel managemasterdatamodel)
        {
            managemasterdatamodel.IsManual = false;
            managemasterdatamodel.IsMode = false;
            managemasterdatamodel.IsRepair = true;
            managemasterdatamodel.IsDelete = false;
            managemasterdatamodel.IsCreate = false;
            managemasterdatamodel.IsAdd = false;

            List<SelectListItem> RepairList = new List<SelectListItem>();
            RepairList = PopulateRepairCodeFromManualMode(managemasterdatamodel.ManCode, managemasterdatamodel.ModCode);
            if (RepairList.Count > 0)
            {
                managemasterdatamodel.drpRepCodeList = RepairList;
                ViewData["RepairList"] = RepairList;
                managemasterdatamodel.FlagRepair = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                managemasterdatamodel.FlagRepair = false;
            }
            return View("ViewExclusiveRepairCodeDetails", managemasterdatamodel);
        }


        public ActionResult ViewRepairDetails_View([Bind] ManageMasterDataRepairCodeExModel managemasterdatamodel)
        {
            managemasterdatamodel.IsManual = false;
            managemasterdatamodel.IsMode = false;
            managemasterdatamodel.IsRepair = true;
            managemasterdatamodel.IsDelete = false;
            managemasterdatamodel.IsCreate = false;
            managemasterdatamodel.IsAdd = false;

            List<SelectListItem> RepairList = new List<SelectListItem>();
            RepairList = PopulateRepairCodeFromManualMode(managemasterdatamodel.ManCode, managemasterdatamodel.ModCode);
            if (RepairList.Count > 0)
            {
                managemasterdatamodel.drpRepCodeList = RepairList;
                ViewData["RepairList"] = RepairList;
                managemasterdatamodel.FlagRepair = true;
            }
            else
            {
                ErrorMessage = "There were no results meeting your query parameters ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                managemasterdatamodel.FlagRepair = false;
            }
            return View("ViewExclusiveRepairCodeDetails_View", managemasterdatamodel);
        }


        public ActionResult ExclusveRprAdd([Bind] ManageMasterDataRepairCodeExModel managemasterdatamodel)
        {
            ManageMasterDataRepairCodeExModel ManageMasterDataModel = new ManageMasterDataRepairCodeExModel();

            managemasterdatamodel.IsManual = false;
            managemasterdatamodel.IsMode = false;
            managemasterdatamodel.IsRepair = false;
            managemasterdatamodel.IsDelete = false;
            managemasterdatamodel.IsCreate = true;
            managemasterdatamodel.IsAdd = false;

            ManageMasterDataModel.ManCode = managemasterdatamodel.ManCode;
            ManageMasterDataModel.ModCode = managemasterdatamodel.ModCode;
            ManageMasterDataModel.RepCode = managemasterdatamodel.RepCode;

            return View("ViewExclusiveRepairCodeDetails", managemasterdatamodel);
        }

        public ActionResult ExclusveRprAdd_View([Bind] ManageMasterDataRepairCodeExModel managemasterdatamodel)
        {
            ManageMasterDataRepairCodeExModel ManageMasterDataModel = new ManageMasterDataRepairCodeExModel();

            managemasterdatamodel.IsManual = false;
            managemasterdatamodel.IsMode = false;
            managemasterdatamodel.IsRepair = false;
            managemasterdatamodel.IsDelete = false;
            managemasterdatamodel.IsCreate = true;
            managemasterdatamodel.IsAdd = false;

            ManageMasterDataModel.ManCode = managemasterdatamodel.ManCode;
            ManageMasterDataModel.ModCode = managemasterdatamodel.ModCode;
            ManageMasterDataModel.RepCode = managemasterdatamodel.RepCode;

            return View("ViewExclusiveRepairCodeDetails_View", managemasterdatamodel);
        }

        [HttpPost]
        public ActionResult AddExclusveRpr([Bind] ManageMasterDataRepairCodeExModel managemasterdatamodel)
        {

            bool success = false;
            string userId = ((UserSec)Session["UserSec"]).LoginId;
            RprcodeExclu ExRprToBeCreated = new RprcodeExclu();
            ExRprToBeCreated.ManualCode = managemasterdatamodel.ManCode;
            ExRprToBeCreated.Mode = managemasterdatamodel.ModCode;
            ExRprToBeCreated.RepairCod = managemasterdatamodel.RepCode;
            ExRprToBeCreated.ExcluRepairCode = managemasterdatamodel.ExcluRepairCode;
            ExRprToBeCreated.ChangeUser = userId;
            success = mmdc.AddExRepairCode(ExRprToBeCreated, managemasterdatamodel.RepCode, managemasterdatamodel.ModCode, managemasterdatamodel.ManCode, ref ErrorMessage);
            if (success)
            {
                ErrorMessage = "Exclusive Repair Code " + ExRprToBeCreated.ExcluRepairCode + " successfully added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
                managemasterdatamodel.IsManual = false;
                managemasterdatamodel.IsMode = false;
                managemasterdatamodel.IsRepair = false;
                managemasterdatamodel.IsDelete = false;
                managemasterdatamodel.IsCreate = false;
                managemasterdatamodel.IsAdd = true;
            }
            else
            {

                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                managemasterdatamodel.IsManual = false;
                managemasterdatamodel.IsMode = false;
                managemasterdatamodel.IsRepair = false;
                managemasterdatamodel.IsDelete = false;
                managemasterdatamodel.IsCreate = true;
                managemasterdatamodel.IsAdd = false;
            }

            string userName = mmdc.GetUserName(ExRprToBeCreated.ChangeUser);
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    managemasterdatamodel.ChangeFUserRpr = userName.Split('|')[0];
                    managemasterdatamodel.ChangeLUserRpr = userName.Split('|')[1];
                }
                else
                {
                    managemasterdatamodel.ChangeFUserRpr = userName;
                    managemasterdatamodel.ChangeLUserRpr = "";
                }
            }
            else
            {
                managemasterdatamodel.ChangeFUserRpr = "";
                managemasterdatamodel.ChangeLUserRpr = "";
            }

            managemasterdatamodel.ChangeTimeRpr = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");
            return View("ViewExclusiveRepairCodeDetails", managemasterdatamodel);
        }

        [HttpPost]
        public ActionResult DeleteExclusveRpr([Bind] ManageMasterDataRepairCodeExModel managemasterdatamodel)
        {

            managemasterdatamodel.IsManual = false;
            managemasterdatamodel.IsMode = false;
            managemasterdatamodel.IsRepair = false;
            managemasterdatamodel.IsDelete = false;
            managemasterdatamodel.IsCreate = true;
            managemasterdatamodel.IsAdd = false;

            bool success = false;

            RprcodeExclu ExRprToBeDeleted = new RprcodeExclu();
            ExRprToBeDeleted.ManualCode = managemasterdatamodel.ManCode;
            ExRprToBeDeleted.Mode = managemasterdatamodel.ModCode;
            ExRprToBeDeleted.RepairCod = managemasterdatamodel.RepCode;
            ExRprToBeDeleted.ExcluRepairCode = managemasterdatamodel.ExcluRepairCode;

            success = mmdc.DeleteExRprCode(managemasterdatamodel.ManCode, managemasterdatamodel.ModCode, managemasterdatamodel.RepCode, managemasterdatamodel.ExcluRepairCode, ref ErrorMessage);
            if (success)
            {
                ErrorMessage = "Exclusive Repair Code " + managemasterdatamodel.ExcluRepairCode + " successfully Deleted";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
            }

            return View("ViewExclusiveRepairCodeDetails", managemasterdatamodel);
        }






        #endregion ExclusiveRepairCode



        #region RepairTimeProductivity

        private void SetUserAccess(ManageMasterDataRepairTimeProductivityModel manageMasterDataModel)
        {
            manageMasterDataModel.isAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            manageMasterDataModel.isCPH = ((UserSec)Session["UserSec"]).isCPH;
            manageMasterDataModel.isEMRSpecialistCountry = ((UserSec)Session["UserSec"]).isEMRSpecialistCountry;
            manageMasterDataModel.isEMRSpecialistShop = ((UserSec)Session["UserSec"]).isEMRSpecialistShop;
            manageMasterDataModel.isEMRApproverCountry = ((UserSec)Session["UserSec"]).isEMRApproverCountry;
            manageMasterDataModel.isEMRApproverShop = ((UserSec)Session["UserSec"]).isEMRApproverShop;
            manageMasterDataModel.isShop = ((UserSec)Session["UserSec"]).isShop;
            manageMasterDataModel.isMPROCluster = ((UserSec)Session["UserSec"]).isMPROCluster;
            manageMasterDataModel.isMPROShop = ((UserSec)Session["UserSec"]).isMPROShop;
            manageMasterDataModel.isReadOnly = ((UserSec)Session["UserSec"]).isReadOnly;

            manageMasterDataModel.isAnyCPH = ((UserSec)Session["UserSec"]).isAnyCPH;

            if (manageMasterDataModel.isAdmin) Role = ADMIN;
            if (manageMasterDataModel.isAnyCPH || manageMasterDataModel.isCPH) Role = CPH;
            if (manageMasterDataModel.isEMRSpecialistCountry) Role = EMR_SPECIALIST_COUNTRY;
            if (manageMasterDataModel.isEMRSpecialistShop) Role = EMR_SPECIALIST_SHOP;
            if (manageMasterDataModel.isEMRApproverCountry) Role = EMR_APPROVER_COUNTRY;
            if (manageMasterDataModel.isEMRApproverShop) Role = EMR_APPROVER_SHOP;
            if (manageMasterDataModel.isShop) Role = SHOP;
            if (manageMasterDataModel.isMPROCluster) Role = MPRO_CLUSTER;
            if (manageMasterDataModel.isMPROShop) Role = MPRO_SHOP;
            if (manageMasterDataModel.isReadOnly) Role = READONLY;
        }

        public ActionResult GetAllDetailsAccToLocation()
        {
            var request = HttpContext.Request;
            ManageMasterDataRepairTimeProductivityModel ManageMasterDataModel = new ManageMasterDataRepairTimeProductivityModel();
            List<SelectListItem> LocationList = new List<SelectListItem>();
            try
            {
                ManageMasterDataModel.drpLocation = LocationList;

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return View("ViewRepairTimeProductivity", ManageMasterDataModel);
        }

        [HttpPost]
        public ActionResult GetAllDetailsAccToShop()
        {
            var request = HttpContext.Request;
            ManageMasterDataRepairTimeProductivityModel ManageMasterDataModel = new ManageMasterDataRepairTimeProductivityModel();

            int userId = ((UserSec)Session["UserSec"]).UserId;
            List<SelectListItem> ShopList = new List<SelectListItem>();
            try
            {
                ShopList = PopulateShopCodeByIdDropDown(userId);
                ManageMasterDataModel.drpShop = ShopList;
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            ManageMasterDataModel.Flag = false;
            return Json(ShopList);
        }

        public ActionResult ViewAllDetailsRepairTimeProductivity()
        {
            ManageMasterDataRepairTimeProductivityModel manageMasterDataModel = new ManageMasterDataRepairTimeProductivityModel();
            int userId = ((UserSec)Session["UserSec"]).UserId;
            List<SelectListItem> CountryList = new List<SelectListItem>();
            CountryList = PopulateCountryCodeByIdDropDown(userId);
            manageMasterDataModel.drpCountry = CountryList;
            manageMasterDataModel.Flag = false;
            List<SelectListItem> ShopList = new List<SelectListItem>();
            ShopList = PopulateShopCodeByIdDropDown(userId);
            manageMasterDataModel.drpShop = ShopList;
            List<SelectListItem> LocationList = new List<SelectListItem>();
            LocationList = PopulateLocationCodeDropDown();
            manageMasterDataModel.drpLocation = LocationList;
            manageMasterDataModel.IsLocation = true;
            manageMasterDataModel.isEMRApproverCountry = ((UserSec)Session["UserSec"]).isEMRApproverCountry;
            manageMasterDataModel.isEMRApproverShop = ((UserSec)Session["UserSec"]).isEMRApproverShop;
            manageMasterDataModel.isShop = ((UserSec)Session["UserSec"]).isShop;
            manageMasterDataModel.isShop = ((UserSec)Session["UserSec"]).isShop;
            manageMasterDataModel.isEMRSpecialistCountry = ((UserSec)Session["UserSec"]).isEMRSpecialistCountry;

            manageMasterDataModel.isAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            manageMasterDataModel.isCPH = ((UserSec)Session["UserSec"]).isCPH;

            return View("ViewRepairTimeProductivity", manageMasterDataModel);
        }

        public ActionResult ViewRepairTimeProductivity()
        {
            ManageMasterDataRepairTimeProductivityModel manageMasterDataModel = new ManageMasterDataRepairTimeProductivityModel();
            int userId = ((UserSec)Session["UserSec"]).UserId;
            List<SelectListItem> CountryList = new List<SelectListItem>();
            CountryList = PopulateCountryCodeByIdDropDown(userId);
            manageMasterDataModel.drpCountry = CountryList;
            manageMasterDataModel.Flag = false;
            List<SelectListItem> ShopList = new List<SelectListItem>();
            ShopList = PopulateShopCodeByIdDropDown(userId);
            manageMasterDataModel.drpShop = ShopList;
            manageMasterDataModel.IsLocation = false;
            manageMasterDataModel.isEMRApproverCountry = ((UserSec)Session["UserSec"]).isEMRApproverCountry;
            manageMasterDataModel.isEMRApproverShop = ((UserSec)Session["UserSec"]).isEMRApproverShop;
            manageMasterDataModel.isShop = ((UserSec)Session["UserSec"]).isShop;
            manageMasterDataModel.isEMRSpecialistCountry = ((UserSec)Session["UserSec"]).isEMRSpecialistCountry;

            manageMasterDataModel.isEMRSpecialistShop = ((UserSec)Session["UserSec"]).isEMRSpecialistShop;

            manageMasterDataModel.isAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            manageMasterDataModel.isCPH = ((UserSec)Session["UserSec"]).isCPH;

            return View("ViewRepairTimeProductivity", manageMasterDataModel);
        }




        [HttpPost]
        public ActionResult GetRepairTimeList(ManageMasterDataRepairTimeProductivityModel ManageMasterDataModel, FormCollection frm)
        {
            List<ManageMasterDataRepairTimeProductivityModel> manageRTP = null;
            var SearchData = manageRTP;
            DateTime DateFrm = Convert.ToDateTime(ManageMasterDataModel.DateFrom);
            DateTime DateTo = Convert.ToDateTime(ManageMasterDataModel.DateTo);
            string RadValue = Convert.ToString(frm["radList"]);
            int userId = ((UserSec)Session["UserSec"]).UserId;
            if (RadValue == "Country")
            {
                ManageMasterDataModel.txtLocation = "";
                ManageMasterDataModel.txtShop = "";
            }
            else if (RadValue == "Location")
            {
                ManageMasterDataModel.txtCountry = "";
                ManageMasterDataModel.txtShop = "";
            }
            else if (RadValue == "Shop")
            {
                ManageMasterDataModel.txtCountry = "";
                ManageMasterDataModel.txtLocation = "";
            }
            try
            {
                if (!string.IsNullOrEmpty(ManageMasterDataModel.txtCountry))
                {
                    string Countrycode = ManageMasterDataModel.txtCountry.Trim();
                    var WOList = mmdc.GetWOListOnCountry(DateTo, DateFrm, Countrycode).ToList();
                    if (WOList.Count > 0)
                    {
                        SearchData = (from e in WOList
                                      select new ManageMasterDataRepairTimeProductivityModel
                                      {
                                          Location = e.LocCode.ToString(),
                                          Shop = e.ShopCode.ToString(),
                                          AvgEstimateTime = Convert.ToString(e.AvgtimeToEstimate),
                                          AvgAuthoriseTime = Convert.ToString(e.AvgTimeToAuthorise),
                                          AvgRepairTime = Convert.ToString(e.AvgTimeToRepair)

                                      }).ToList();

                        ManageMasterDataModel.SearchResult = SearchData;
                        ManageMasterDataModel.Flag = true;
                        ManageMasterDataModel.IsLocation = false;
                        ManageMasterDataModel.isEMRApproverCountry = true;
                        ManageMasterDataModel.isShop = false;
                    }
                    else
                    {
                        ErrorMessage = "There were no results meeting your query parameters ";
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                        ManageMasterDataModel.Flag = false;
                        ManageMasterDataModel.IsLocation = false;
                        ManageMasterDataModel.isEMRApproverCountry = true;
                        ManageMasterDataModel.isShop = false;
                    }


                }
                else if (!string.IsNullOrEmpty(ManageMasterDataModel.txtLocation))
                {
                    string LocationCode = ManageMasterDataModel.txtLocation.Trim();
                    var WOList = mmdc.GetWOListOnLocation(DateTo, DateFrm, LocationCode).ToList();
                    if (WOList.Count > 0)
                    {
                        SearchData = (from e in WOList
                                      select new ManageMasterDataRepairTimeProductivityModel
                                      {
                                          Location = e.LocCode.ToString(),
                                          Shop = e.ShopCode.ToString(),
                                          AvgEstimateTime = Convert.ToString(e.AvgtimeToEstimate),
                                          AvgAuthoriseTime = Convert.ToString(e.AvgTimeToAuthorise),
                                          AvgRepairTime = Convert.ToString(e.AvgTimeToRepair)

                                      }).ToList();
                        ManageMasterDataModel.SearchResult = SearchData;
                        ManageMasterDataModel.Flag = true;
                        ManageMasterDataModel.IsLocation = true;
                        ManageMasterDataModel.isEMRApproverCountry = false;
                        ManageMasterDataModel.isShop = false;
                    }
                    else
                    {
                        ErrorMessage = "There were no results meeting your query parameters ";
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                        ManageMasterDataModel.Flag = false;
                        ManageMasterDataModel.IsLocation = true;
                        ManageMasterDataModel.isEMRApproverCountry = false;
                        ManageMasterDataModel.isShop = false;
                    }

                }
                else if (!string.IsNullOrEmpty(ManageMasterDataModel.txtShop))
                {
                    string ShopCode = ManageMasterDataModel.txtShop.Trim();
                    var WOList = mmdc.GetWOListOnShop(DateTo, DateFrm, ShopCode).ToList();
                    if (WOList.Count > 0)
                    {
                        SearchData = (from e in WOList
                                      select new ManageMasterDataRepairTimeProductivityModel
                                      {
                                          Location = e.LocCode.ToString(),
                                          Shop = e.ShopCode.ToString(),
                                          AvgEstimateTime = Convert.ToString(e.AvgtimeToEstimate),
                                          AvgAuthoriseTime = Convert.ToString(e.AvgTimeToAuthorise),
                                          AvgRepairTime = Convert.ToString(e.AvgTimeToRepair)

                                      }).ToList();

                        ManageMasterDataModel.SearchResult = SearchData;
                        ManageMasterDataModel.Flag = true;
                        ManageMasterDataModel.IsLocation = false;
                        ManageMasterDataModel.isEMRApproverCountry = false;
                        ManageMasterDataModel.isShop = true;
                    }
                    else
                    {
                        ErrorMessage = "There were no results meeting your query parameters ";
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                        ManageMasterDataModel.Flag = false;
                        ManageMasterDataModel.IsLocation = false;
                        ManageMasterDataModel.isEMRApproverCountry = false;
                        ManageMasterDataModel.isShop = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            List<SelectListItem> CountryList = new List<SelectListItem>();
            List<SelectListItem> LocationList = new List<SelectListItem>();
            List<SelectListItem> ShopList = new List<SelectListItem>();
            //CountryList = PopulateCountryCodeDropDown();
            CountryList = PopulateCountryCodeByIdDropDown(userId);
            //LocationList = PopulateLocationCodeDropDown();
            //ShopList = PopulateShopCodeDropDown();
            ShopList = PopulateShopCodeByIdDropDown(userId);
            ManageMasterDataModel.drpCountry = CountryList;
            //ManageMasterDataModel.drpLocation = LocationList;
            ManageMasterDataModel.drpShop = ShopList;
            //ManageMasterDataModel.txtCountry = string.Empty;
            //ManageMasterDataModel.txtLocation = string.Empty;
            ManageMasterDataModel.txtShop = string.Empty;
            return View("ViewRepairTimeProductivity", ManageMasterDataModel);
        }


        #endregion RepairTimeProductivity



        #region DropDown Populations

        private List<SelectListItem> PopulatePayAgentCodeDropDown()
        {
            List<PayAgent> payAgentList = new List<PayAgent>();
            payAgentList = mmdc.GetPayAgent().ToList();
            int count = 1;
            List<SelectListItem> RRISPayAgentCode = new List<SelectListItem>();
            foreach (var code in payAgentList)
            {
                RRISPayAgentCode.Add(new SelectListItem
                {
                    Text = code.PayAgentCode,
                    Value = code.PayAgentCode
                });
                count++;
            }
            return RRISPayAgentCode;

        }

        private List<SelectListItem> PopulateRRISFormatDropDown(string RRISFormatstring)
        {
            List<SelectListItem> RRISFormat = new List<SelectListItem>();

            if (string.IsNullOrEmpty(RRISFormatstring))
            {
                RRISFormat.Add(new SelectListItem
                {
                    Text = "RRIS52 - Maersk Line Books",
                    Value = "52"
                });
                RRISFormat.Add(new SelectListItem
                {
                    Text = "RRIS70 - Local Books",
                    Value = "70"
                });
                RRISFormat.Add(new SelectListItem
                {
                    Text = "RRIS72 - Interoffice Books",
                    Value = "72"
                });
            }
            else
            {
                if (RRISFormatstring == "52")
                {
                    RRISFormat.Add(new SelectListItem
                    {
                        Text = "RRIS52 - Maersk Line Books",
                        Value = "52"
                    });
                }
                else if (RRISFormatstring == "70")
                {
                    RRISFormat.Add(new SelectListItem
                    {
                        Text = "RRIS70 - Local Books",
                        Value = "70"
                    });
                }
                else if (RRISFormatstring == "72")
                {
                    RRISFormat.Add(new SelectListItem
                    {
                        Text = "RRIS72 - Interoffice Books",
                        Value = "72"
                        //payAgentListToBeUpdated.PayAgentCode = manageMasterDataModel.CorporatePayAgentCode;
                    });
                }
            }
            return RRISFormat;
        }

        private List<SelectListItem> PopulateEquipmentTypeDropDown(List<EqType> EqTypeList = null)
        {
            List<SelectListItem> EqType = new List<SelectListItem>();
            EqTypeList = mmdc.GetEquipmentTypeList().ToList();
            int count = 1;
            foreach (var code in EqTypeList)
            {
                EqType.Add(new SelectListItem
                {
                    Text = code.EqpType,
                    Value = code.EqpType
                });
                count++;
            }
            return EqType;
        }

        private List<SelectListItem> PopulateManualCodeDropDown(List<Manual> ManualCodeList = null)
        {
            List<SelectListItem> ManualCodeCust = new List<SelectListItem>();
            ManualCodeList = mmdc.GetManualList().ToList();
            int count = 1;
            foreach (var code in ManualCodeList)
            {
                ManualCodeCust.Add(new SelectListItem
                {
                    Text = code.ManualCode + "-" + code.ManualDesc,
                    Value = code.ManualCode

                });
                count++;
            }
            return ManualCodeCust;
        }

        private List<SelectListItem> PopulateManualCodeForSuspedDropDown(List<Manual> ManualCodeList = null)
        {
            List<SelectListItem> ManualCodeCust = new List<SelectListItem>();
            ManualCodeList = mmdc.GetManualCodeList().ToList();
            int count = 1;
            foreach (var code in ManualCodeList)
            {
                ManualCodeCust.Add(new SelectListItem
                {
                    Text = code.ManualCode + "-" + code.ManualDesc,
                    Value = code.ManualCode

                });
                count++;
            }
            return ManualCodeCust;
        }


        private List<SelectListItem> PopulateRepairCodeDropDown(List<RepairCode> RepairCodeList = null)
        {
            List<SelectListItem> RprCode = new List<SelectListItem>();
            RepairCodeList = mmdc.GetRepairCode().ToList();
            int count = 1;
            foreach (var code in RepairCodeList)
            {
                RprCode.Add(new SelectListItem
                {
                    Text = code.RepairCod + "-" + code.RepairDesc,
                    Value = code.RepairCod
                });
                count++;
            }
            return RprCode;
        }

        private List<SelectListItem> PopulateRepairCodeForSuspendDropDown(string Manual, string Mode)
        {
            List<RepairCode> RepairCodeList = null;
            List<SelectListItem> RprCode = new List<SelectListItem>();
            RepairCodeList = mmdc.GetRepairCodeFromManualMode(Manual, Mode).ToList();
            int count = 1;
            foreach (var code in RepairCodeList)
            {
                RprCode.Add(new SelectListItem
                {
                    Text = code.RepairCod + "-" + code.RepairDesc,
                    Value = code.RepairCod
                });
                count++;
            }
            return RprCode;
        }


        private List<SelectListItem> PopulateSuspendCatDropDown(List<SuspendCat> SuspendCatList = null)
        {
            List<SelectListItem> SuspendCat = new List<SelectListItem>();
            SuspendCatList = mmdc.GetSuspendCategory().ToList();
            int count = 1;
            foreach (var code in SuspendCatList)
            {
                SuspendCat.Add(new SelectListItem
                {
                    Text = code.SuspcatID + "-" + code.SuspcatDesc,
                    Value = Convert.ToString(code.SuspcatID)
                });
                count++;
            }
            return SuspendCat;
        }

        private List<SelectListItem> PopulateManualRepairCodeDropDown(List<Manual> ManualCodeList = null)
        {
            List<SelectListItem> ManualCodeRpr = new List<SelectListItem>();
            ManualCodeList = mmdc.GetManualDetails().ToList();
            int count = 1;
            foreach (var code in ManualCodeList)
            {
                ManualCodeRpr.Add(new SelectListItem
                {
                    Text = code.ManualCode + "-" + code.ManualDesc,
                    Value = code.ManualCode

                });
                count++;
            }
            return ManualCodeRpr;
        }


        private List<SelectListItem> PopulateSwitchDropDown(List<SelectListItem> Switch, string SwitchString)
        {

            if (string.IsNullOrEmpty(SwitchString))
            {
                Switch.Add(new SelectListItem
                {
                    Text = "Y",
                    Value = "Y"
                });
                Switch.Add(new SelectListItem
                {
                    Text = "N",
                    Value = "N"
                });

            }
            else
            {
                if (SwitchString == "Y")
                {
                    Switch.Add(new SelectListItem
                    {
                        Text = "Y",
                        Value = "Y"
                    });
                }
                else if (SwitchString == "N")
                {
                    Switch.Add(new SelectListItem
                    {
                        Text = "N",
                        Value = "N"
                    });
                }

            }
            return Switch;
        }

        private List<SelectListItem> PopulateRegionCodeDropDown(List<Region> RegionCodeList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            RegionCodeList = mmdc.GetRegionList().ToList();
            List<SelectListItem> RegionCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in RegionCodeList)
            {
                RegionCode.Add(new SelectListItem
                {
                    Text = code.RegionCd + "-" + code.RegionDesc,
                    Value = code.RegionCd
                });

                count++;
            }

            return RegionCode;

        }

        private List<SelectListItem> PopulateCountryCodeDropDown(List<Country> CountryCodeList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            CountryCodeList = mmdc.GetCountryList().ToList();
            List<SelectListItem> CountryCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in CountryCodeList)
            {
                CountryCode.Add(new SelectListItem
                {
                    Text = code.CountryCode + "-" + code.CountryDescription,
                    Value = code.CountryCode
                });

                count++;
            }

            return CountryCode;

        }

        private List<SelectListItem> PopulateCountryDropDown(List<Country> CountryCodeList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            CountryCodeList = mmdc.GetCountryList().ToList();
            List<SelectListItem> CountryCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in CountryCodeList)
            {
                CountryCode.Add(new SelectListItem
                {
                    Text = code.CountryDescription,
                    Value = code.CountryCode
                });

                count++;
            }

            return CountryCode;

        }


        private List<SelectListItem> PopulateLocationCodeDropDown(List<Location> LocationCodeList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            LocationCodeList = mmdc.GetLocationList().ToList();
            List<SelectListItem> LocationCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in LocationCodeList)
            {
                LocationCode.Add(new SelectListItem
                {
                    Text = code.LocCode + "-" + code.LocDesc,
                    Value = code.LocCode
                });

                count++;
            }

            return LocationCode;
        }

        private List<SelectListItem> PopulateShopCodeDropDown(List<Shop> ShopCodeList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            int userId = ((UserSec)Session["UserSec"]).UserId;
            ShopCodeList = mmdc.GetShop(userId).ToList();
            List<SelectListItem> ShopCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in ShopCodeList)
            {
                ShopCode.Add(new SelectListItem
                {
                    Text = code.ShopCode + "-" + code.ShopDescription,
                    Value = code.ShopCode
                });

                count++;
            }

            return ShopCode;

        }

        private List<SelectListItem> PopulateShopCodeByIdDropDown(int userId)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            //List<Shop> ShopCodeList = null;
            // ManageMasterDataClient mmdc = new ManageMasterDataClient();

            List<Shop> ShopList = mmdc.GetShopCodeForSuspend(userId).ToList();
            //List<> ShopCodeList = WorkOrderClient.GetShopCode(userId).ToList();
            List<SelectListItem> ShopCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in ShopList)
            {
                ShopCode.Add(new SelectListItem
                {
                    Text = code.ShopCode + "-" + code.ShopDescription,
                    Value = code.ShopCode
                });

                count++;
            }

            return ShopCode;

        }

        private List<SelectListItem> PopulateCustomerCodeDropDown(List<Customer> CustomerCodeList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            CustomerCodeList = mmdc.GetCustomerCode().ToList();
            List<SelectListItem> CustomerCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in CustomerCodeList)
            {
                CustomerCode.Add(new SelectListItem
                {
                    Text = code.CustomerCode + "-" + code.CustomerDesc,
                    Value = code.CustomerCode
                });

                count++;
            }

            return CustomerCode;

        }

        private List<SelectListItem> PopulateCountryCodeByIdDropDown(int userId)
        {
            ManageMasterDataRepairTimeProductivityModel managemasterdatamodel = new ManageMasterDataRepairTimeProductivityModel();
            //List<Shop> ShopCodeList = null;
            // ManageMasterDataClient mmdc = new ManageMasterDataClient();
            SetUserAccess(managemasterdatamodel);
            List<Country> CountryList = mmdc.GetCountryByID(userId, Role).ToList();
            //List<> ShopCodeList = WorkOrderClient.GetShopCode(userId).ToList();
            List<SelectListItem> CountryCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in CountryList)
            {
                CountryCode.Add(new SelectListItem
                {
                    Text = code.CountryCode + "-" + code.CountryDescription,
                    Value = code.CountryCode
                });

                count++;
            }

            return CountryCode;

        }

        private List<SelectListItem> PopulateCustomerDropDown(List<Customer> CustomerCodeList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            CustomerCodeList = mmdc.GetCustomerCode().ToList();
            List<SelectListItem> CustomerCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in CustomerCodeList)
            {
                CustomerCode.Add(new SelectListItem
                {
                    Text = code.CustomerCode,
                    Value = code.CustomerCode
                });

                count++;
            }

            return CustomerCode;

        }

        private List<SelectListItem> PopulateModeCodeDropDown(List<Mode> ModeCodeList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            ModeCodeList = mmdc.GetModeList().ToList();
            List<SelectListItem> ModeCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in ModeCodeList)
            {
                ModeCode.Add(new SelectListItem
                {
                    Text = code.ModeCode + "-" + code.ModeDescription,
                    Value = code.ModeCode
                });

                count++;
            }

            return ModeCode;

        }

        private List<SelectListItem> PopulateModeCodeForSuspendDropDown(string ManualCode)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            List<Mode> ModeCodeList = null;
            ModeCodeList = mmdc.GetModeForSuspend(ManualCode).ToList();
            List<SelectListItem> ModeCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in ModeCodeList)
            {
                ModeCode.Add(new SelectListItem
                {
                    Text = code.ModeCode + "-" + code.ModeDescription,
                    Value = code.ModeCode
                });

                count++;
            }

            return ModeCode;

        }

        private List<SelectListItem> PopulateModeCodeDropDownByCustomerCode(string CustomerCode, List<Mode> ModeCodeList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            ModeCodeList = mmdc.GetModeByCustomerCode(CustomerCode).ToList();
            List<SelectListItem> ModeCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in ModeCodeList)
            {
                ModeCode.Add(new SelectListItem
                {
                    Text = code.ModeCode + "-" + code.ModeDescription,
                    Value = code.ModeCode
                });

                count++;
            }

            return ModeCode;

        }

        private List<SelectListItem> PopulateManualCodeDropDownByShopCode(string ShopCode, List<Manual> ManualList = null)
        {
            ManageMasterDataSuspendModel managemasterdatamodel = new ManageMasterDataSuspendModel();
            ManualList = mmdc.GetManualCodeFromShopCode(ShopCode).ToList();
            List<SelectListItem> ManualCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in ManualList)
            {
                ManualCode.Add(new SelectListItem
                {
                    Text = code.ManualCode + "-" + code.ManualDesc,
                    Value = code.ManualCode
                });

                count++;
            }

            return ManualCode;
        }

        private List<SelectListItem> PopulateModeCodeDropDownByShopCodeManualCode(string ShopCode, string ManualCode, List<Mode> ModeCodeList = null)
        {
            ManageMasterDataSuspendModel managemasterdatamodel = new ManageMasterDataSuspendModel();
            ModeCodeList = mmdc.GetModeFromShopManual(ShopCode, ManualCode).ToList();
            List<SelectListItem> ModeCode = new List<SelectListItem>();
            int count = 1;
            foreach (var code in ModeCodeList)
            {
                ModeCode.Add(new SelectListItem
                {
                    Text = code.ModeCode + "-" + code.ModeDescription,
                    Value = code.ModeCode
                });

                count++;
            }

            return ModeCode;

        }

        private List<SelectListItem> PopulateRepairCodeDropDownByShopCodeManualCodeModeCode(string ShopCode, string ManualCode, string ModeCode, List<RepairCode> RepairCodeList = null)
        {
            ManageMasterDataSuspendModel managemasterdatamodel = new ManageMasterDataSuspendModel();
            RepairCodeList = mmdc.GetRepairCodeFromShopManualMode(ShopCode, ManualCode, ModeCode).ToList();
            List<SelectListItem> RepairCode = new List<SelectListItem>();
            int count = 1;
            foreach (var code in RepairCodeList)
            {
                RepairCode.Add(new SelectListItem
                {
                    Text = code.RepairCod + "-" + code.RepairDesc,
                    Value = code.RepairCod
                });

                count++;
            }

            return RepairCode;

        }

        private List<SelectListItem> PopulateModeCodeFromManualCode(string ManualCode, List<Mode> ModeList = null)
        {

            ModeList = mmdc.GetModeFromManual(ManualCode).ToList();
            List<SelectListItem> ModeCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in ModeList)
            {
                ModeCode.Add(new SelectListItem
                {
                    Text = code.ModeCode + "-" + code.ModeDescription,
                    Value = code.ModeCode
                });

                count++;
            }

            return ModeCode;
        }

        private List<SelectListItem> PopulateRepairCodeFromManualMode(string ManualCode, string ModeCode, List<RepairCode> RprCodeList = null)
        {

            RprCodeList = mmdc.GetRepairCodeFromManualMode(ManualCode, ModeCode).ToList();
            List<SelectListItem> RepairCode = new List<SelectListItem>();
            int count = 1;
            foreach (var code in RprCodeList)
            {
                RepairCode.Add(new SelectListItem
                {
                    Text = code.RepairCod + "-" + code.RepairDesc,
                    Value = code.RepairCod
                });

                count++;
            }

            return RepairCode;
        }

        private List<SelectListItem> PopulateVendorCodeDropDown(List<Vendor> VendorList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            VendorList = mmdc.GetVendorList().ToList();
            List<SelectListItem> VendorCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in VendorList)
            {
                VendorCode.Add(new SelectListItem
                {
                    Text = code.VendorCode,
                    Value = code.VendorCode
                });

                count++;
            }

            return VendorCode;
        }



        #endregion DropDown Populations


        #region Amlan


        #region MasterPart


        public ActionResult ManageMasterPart()
        {
            //List<PartsGroup> partsGroupList = new List<PartsGroup>();
            ManageMasterPartModel partModel = new ManageMasterPartModel();


            LoadAllPartsDropDowns(partModel);

            LoadSearchFilterDropDowns(partModel);

            List<GridMasterPartModel> lstGridMasterPartModel = new List<GridMasterPartModel>();


            ViewBag.GridData = lstGridMasterPartModel;

            partModel.showQuery = true;

            return View("ManageMasterPart", partModel);

        }


        public ActionResult ManageMasterPart_View() //Debadrita_User_Remapping
        {
            //List<PartsGroup> partsGroupList = new List<PartsGroup>();
            ManageMasterPartModel partModel = new ManageMasterPartModel();


            LoadAllPartsDropDowns(partModel);

            LoadSearchFilterDropDowns(partModel);

            List<GridMasterPartModel> lstGridMasterPartModel = new List<GridMasterPartModel>();


            ViewBag.GridData = lstGridMasterPartModel;

            partModel.showQuery = true;

            return View("ManageMasterPart_View", partModel);

        }

        private void LoadSearchFilterDropDowns(ManageMasterPartModel partModel)
        {

            string msg = "";
            //try
            //{
            if (partModel.drpPartGroupCodeList == null || partModel.drpPartGroupCodeList.Count == 0)
            {
                LoadAllPartsDropDowns(partModel);
            }

            SearchMasterPartModel searchMasterPartModel = new SearchMasterPartModel();

            //List<SelectListItem> selList = partModel.drpActiveList;
            SelectListItem selListItem = new SelectListItem();
            selListItem.Text = "";
            selListItem.Value = "";
            //partModel.drpActiveList.Insert(0, selListItem);
            //partModel.drpDeductCoreValueList.Insert(0, selListItem);
            //partModel.drpManufacturerCodeList.Insert(0, selListItem);
            //partModel.drpPartGroupCodeList.Insert(0, selListItem);

            searchMasterPartModel.drpActive = partModel.drpActiveList;
            searchMasterPartModel.drpCore = partModel.drpDeductCoreValueList;
            searchMasterPartModel.drpDeductCoreValue = partModel.drpDeductCoreValueList;
            searchMasterPartModel.drpManufacturerCode = partModel.drpManufacturerCodeList;
            searchMasterPartModel.drpPartGroupCode = partModel.drpPartGroupCodeList;
            partModel.SearchMasterPartModel = searchMasterPartModel;

            //}
            //catch (Exception ex)
            //{
            //    msg = "Server Error";
            //    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString());

            //}

        }

        private void LoadAllPartsDropDowns(ManageMasterPartModel partModel)
        {
            List<PartsGroup> partsGroupList = new List<PartsGroup>();
            //ManageMasterPartModel partModel = new ManageMasterPartModel();

            // populating PartGroup Code
            ManagePartGroupModel pGroupModel = new ManagePartGroupModel();
            PopulatePartGroupCodeDropDown(pGroupModel);

            //ViewData["drpPartGroupCodeList"] = pGroupModel.drpPartGroupList;

            partModel.drpPartGroupCodeList = pGroupModel.drpPartGroupList;

            if (partModel.drpPartGroupCodeList != null && partModel.drpPartGroupCodeList.Count > 0)
            {
                partModel.PartGroupCodeList = partModel.drpPartGroupCodeList[0].Value;
            }

            // populating Manufacturer code
            PopulateManufacturerCodeDropDown(partModel);

            PopulateMasterPartActiveDropDown(partModel);

            PopulateSerialTAGDropDown(partModel); //

            PopulateDeductCoreDropDown(partModel); //

            PopulateMaerskDropDown(partModel); //


        }
        public void PopulatePartGroupCodeDropDown(ManagePartGroupModel partGroupModel)
        {
            string msg = "";
            List<SelectListItem> selPartGroupCode = new List<SelectListItem>();
            List<PartsGroup> partGroupList = new List<PartsGroup>();
            try
            {
                partGroupList = mmdc.GetAllPartsGroups().ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }


            int count = 1;
            //Populating the PartGroup from the list we got from db
            foreach (var code in partGroupList)
            {
                selPartGroupCode.Add(new SelectListItem
                {
                    Text = code.PartsGroupCd + " - " + code.PartsGroupDesc,
                    Value = code.PartsGroupCd
                });
                count++;
            }


            partGroupModel.drpPartGroupList = selPartGroupCode;
        }



        public ActionResult PopulateMasterPartResultGrid(ManageMasterPartModel model)
        {

            string partCode = model.SearchMasterPartModel.PartCode;
            string manuCode = model.SearchMasterPartModel.ManufacturerCode;
            string parDesc = model.SearchMasterPartModel.PartDescription;
            string partGroupCode = model.SearchMasterPartModel.PartGroupCode;
            string partDesig = model.SearchMasterPartModel.Designation;
            string active = model.SearchMasterPartModel.Active;
            string core = model.SearchMasterPartModel.Core;
            string deductCoreVal = model.SearchMasterPartModel.DeductCoreValue;
            string msg = "";


            List<GridMasterPartModel> lstGridMasterPartModel = new List<GridMasterPartModel>();
            ManageMasterPartModel partModel = new ManageMasterPartModel();
            partModel.drpPartGroupCodeList = new List<SelectListItem>();
            partModel.drpManufacturerCodeList = new List<SelectListItem>();
            partModel.drpTAGList = new List<SelectListItem>();
            partModel.drpMaerskList = new List<SelectListItem>();
            partModel.drpActiveList = new List<SelectListItem>();
            partModel.drpDeductCoreValueList = new List<SelectListItem>();

            LoadSearchFilterDropDowns(partModel);

            try
            {

                lstGridMasterPartModel = GetResultForMasterPartGrid(partGroupCode, manuCode, partDesig, partCode, parDesc, active, core, deductCoreVal);
            }
            catch (Exception ex)
            {
                msg = "Error while fetching search result.";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            ViewBag.GridData = lstGridMasterPartModel;

            if (lstGridMasterPartModel.Count == 0)
            {
                partModel.Message = "There were no results meeting your query parameters";
                if (String.IsNullOrEmpty(msg))
                {
                    msg = partModel.Message;
                }
            }
            else if (lstGridMasterPartModel.Count >= 100)
            {
                partModel.Message = "Note: Please limit your query. The maximum 100 records have been returned.";
                TempData["Msg"] = "</br><div class=\"alert alert-info\" style=\"width: 750px; vertical-align: text-top;\"> <strong>Info!</strong> " + partModel.Message + "</div>";
                partModel.showQueryResult = true;
            }
            else
            {
                partModel.showQueryResult = true;
            }

            partModel.showQuery = true;

            if (!String.IsNullOrEmpty(msg))
            {
                string msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, msgType);
            }

            return View("ManageMasterPart", partModel);
        }



        public ActionResult PopulateMasterPartResultGrid_View(ManageMasterPartModel model)
        {

            string partCode = model.SearchMasterPartModel.PartCode;
            string manuCode = model.SearchMasterPartModel.ManufacturerCode;
            string parDesc = model.SearchMasterPartModel.PartDescription;
            string partGroupCode = model.SearchMasterPartModel.PartGroupCode;
            string partDesig = model.SearchMasterPartModel.Designation;
            string active = model.SearchMasterPartModel.Active;
            string core = model.SearchMasterPartModel.Core;
            string deductCoreVal = model.SearchMasterPartModel.DeductCoreValue;
            string msg = "";


            List<GridMasterPartModel> lstGridMasterPartModel = new List<GridMasterPartModel>();
            ManageMasterPartModel partModel = new ManageMasterPartModel();
            partModel.drpPartGroupCodeList = new List<SelectListItem>();
            partModel.drpManufacturerCodeList = new List<SelectListItem>();
            partModel.drpTAGList = new List<SelectListItem>();
            partModel.drpMaerskList = new List<SelectListItem>();
            partModel.drpActiveList = new List<SelectListItem>();
            partModel.drpDeductCoreValueList = new List<SelectListItem>();

            LoadSearchFilterDropDowns(partModel);

            try
            {

                lstGridMasterPartModel = GetResultForMasterPartGrid(partGroupCode, manuCode, partDesig, partCode, parDesc, active, core, deductCoreVal);
            }
            catch (Exception ex)
            {
                msg = "Error while fetching search result.";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            ViewBag.GridData = lstGridMasterPartModel;

            if (lstGridMasterPartModel.Count == 0)
            {
                partModel.Message = "There were no results meeting your query parameters";
                if (String.IsNullOrEmpty(msg))
                {
                    msg = partModel.Message;
                }
            }
            else if (lstGridMasterPartModel.Count >= 100)
            {
                partModel.Message = "Note: Please limit your query. The maximum 100 records have been returned.";
                TempData["Msg"] = "</br><div class=\"alert alert-info\" style=\"width: 750px; vertical-align: text-top;\"> <strong>Info!</strong> " + partModel.Message + "</div>";
                partModel.showQueryResult = true;
            }
            else
            {
                partModel.showQueryResult = true;
            }

            partModel.showQuery = true;

            if (!String.IsNullOrEmpty(msg))
            {
                string msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, msgType);
            }

            return View("ManageMasterPart_View", partModel);
        }

        private List<GridMasterPartModel> GetResultForMasterPartGrid(string partGroupCode, string manufacturerCode,
            string designation, string partNumber, string description, string isActive, string isCore,
            string isDeductCoreValue)
        {
            List<GridMasterPartModel> lstGridMasterPartModel = new List<GridMasterPartModel>();
            //string partGroupCode = "CHAS"; string manufacturerCode = "MSL"; string designation = "";
            //string partNumber = ""; string description = ""; string isActive = "Y"; string isCore = "";
            //string isDeductCoreValue = "";


            List<MasterPart> lstData = mmdc.GetMasterPartsByQuery(partGroupCode, manufacturerCode, designation,
            partNumber, description, isActive, isCore, isDeductCoreValue).ToList();

            GridMasterPartModel gridMasterPartModel = null;
            foreach (MasterPart mp in lstData)
            {
                gridMasterPartModel = new GridMasterPartModel();
                gridMasterPartModel.PartCode = mp.PartCd;
                gridMasterPartModel.PartGroupCode = mp.PartsGroupCd;
                gridMasterPartModel.PartDescription = String.IsNullOrEmpty(mp.PartDesc) ? "" : mp.PartDesc.Trim();
                gridMasterPartModel.Quantity = mp.Quantity == null ? "" : mp.Quantity.Value.ToString();
                gridMasterPartModel.ManufacturerCode = mp.Manufactur;

                gridMasterPartModel.Designation = (String.IsNullOrEmpty(mp.PartDesignation1) ? "" : mp.PartDesignation1.Trim())
                    + "-" +
                    (String.IsNullOrEmpty(mp.PartDesignation2) ? "" : mp.PartDesignation2.Trim())
                    + "-" +
                    (String.IsNullOrEmpty(mp.PartDesignation3) ? "" : mp.PartDesignation3.Trim());
                gridMasterPartModel.PartPrice = (mp.PartPrice == null) ? "" : String.Format("{0:0.00}", mp.PartPrice.Value); ;
                gridMasterPartModel.Active = mp.PartActiveSW;
                gridMasterPartModel.Core = mp.CoreValueSW;
                gridMasterPartModel.CoreValue = (mp.CoreValue == null) ? "" : String.Format("{0:0.00}", mp.CoreValue.Value);
                gridMasterPartModel.DeductCore = mp.DeductCoreSW;


                lstGridMasterPartModel.Add(gridMasterPartModel);

            }


            return lstGridMasterPartModel;


        }


        public ActionResult AddMode()
        {

            List<PartsGroup> partsGroupList = new List<PartsGroup>();
            ManageMasterPartModel partModel = new ManageMasterPartModel();

            LoadAllPartsDropDowns(partModel);

            List<GridMasterPartModel> lstGridMasterPartModel = new List<GridMasterPartModel>();
            ViewBag.GridData = lstGridMasterPartModel;

            partModel.showAdd = true;

            partModel.drpDeductCoreValueList.Reverse();
            partModel.drpMaerskList.Reverse();
            partModel.drpTAGList.Reverse();

            partModel.Quantity = 1;


            return View("ManageMasterPart", partModel);

        }

        public ActionResult EditMode()
        {
            string msg = ""; string msgType = "";

            ManageMasterPartModel partModel = new ManageMasterPartModel();

            LoadAllPartsDropDowns(partModel);

            try
            {

                string partCode = string.Empty;
                //partCode = RouteData.Values["id"].ToString();

                partCode = Request.QueryString["partNumber"];
                List<MasterPart> lstData = mmdc.GetMasterPartByPartCode(partCode.Trim()).ToList();


                if (lstData.Count > 0)
                {

                    // set the values from the DB in the boxes / drop downs for that part

                    partModel.Active = lstData[0].PartActiveSW;
                    partModel.Maersk = lstData[0].MslPartSW;
                    partModel.TAG = lstData[0].SerialTagSW;
                    partModel.DeductCoreValueList = lstData[0].DeductCoreSW;

                    partModel.Quantity = lstData[0].Quantity.Value;
                    partModel.PartNumber = lstData[0].PartCd;
                    partModel.PartGroupCodeList = lstData[0].PartsGroupCd;
                    partModel.PartDescription = lstData[0].PartDesc;
                    partModel.ManufacturerCodeList = lstData[0].Manufactur;
                    partModel.Comment = lstData[0].Remarks;
                    partModel.Amount = decimal.Round(lstData[0].PartPrice.Value, 2);
                    partModel.CoreValue = decimal.Round(lstData[0].CoreValue.Value, 2);
                    partModel.PartDesignation1 = lstData[0].PartDesignation1;
                    partModel.PartDesignation2 = lstData[0].PartDesignation2;
                    partModel.PartDesignation3 = lstData[0].PartDesignation3;
                    partModel.ChangeTime = String.Format("{0:yyyy-MM-dd hh:mm:ss tt}", lstData[0].ChangeTime);
                    partModel.ChangeUserName = lstData[0].ChangeUser;
                }
                else
                {
                    // no data found message -- deleted by another user
                    partModel.Message = "Edit - PartNumber- " + partCode + " Not found";
                    msg = partModel.Message;
                }

            }
            catch (Exception ex)
            {
                msg = "Error while loading Mater Part data.";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            if (!String.IsNullOrEmpty(msg))
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString());
            }


            List<GridMasterPartModel> lstGridMasterPartModel = new List<GridMasterPartModel>();
            ViewBag.GridData = lstGridMasterPartModel;


            partModel.showEdit = true;
            partModel.isEditMode = true;
            partModel.isEdit = "1";

            return View("ManageMasterPart", partModel);

        }

        private ManageMasterPartModel PopulateMaerskDropDown(ManageMasterPartModel PartModel)
        {
            List<SelectListItem> selList = new List<SelectListItem>();
            selList = PopulateYNDropDown();
            PartModel.drpMaerskList = selList;
            //ViewData["drpMaerskList"] = selList;
            return PartModel;
        }

        private ManageMasterPartModel PopulateMasterPartActiveDropDown(ManageMasterPartModel PartModel)
        {
            List<SelectListItem> selList = new List<SelectListItem>();
            selList = PopulateYNDropDown();
            PartModel.drpActiveList = selList;
            //ViewData["drpActiveList"] = selList;
            return PartModel;
        }

        private ManageMasterPartModel PopulateSerialTAGDropDown(ManageMasterPartModel PartModel)
        {
            List<SelectListItem> selList = new List<SelectListItem>();
            selList = PopulateYNDropDown();
            PartModel.drpTAGList = selList;
            //ViewData["drpTAGList"] = selList;
            return PartModel;
        }

        private ManageMasterPartModel PopulateDeductCoreDropDown(ManageMasterPartModel PartModel)
        {
            List<SelectListItem> selList = new List<SelectListItem>();
            selList = PopulateYNDropDown();
            PartModel.drpDeductCoreValueList = selList;
            //ViewData["drpDeductCoreValueList"] = selList;
            return PartModel;
        }

        private List<SelectListItem> PopulateYNDropDown()
        {
            List<SelectListItem> selList = new List<SelectListItem>();

            SelectListItem selListItem = new SelectListItem();
            selListItem.Text = "Y";
            selListItem.Value = "Y";
            selList.Add(selListItem);

            selListItem = new SelectListItem();
            selListItem.Text = "N";
            selListItem.Value = "N";
            selList.Add(selListItem);

            return selList;

        }


        private ManageMasterPartModel PopulateManufacturerCodeDropDown(ManageMasterPartModel PartModel)
        {
            List<SelectListItem> selManufacturersCode = new List<SelectListItem>();
            List<Manufactur> manufacturersList = new List<Manufactur>();
            //PartModel.drpManufacturerCodeList = selManufacturersCode;
            try
            {
                manufacturersList = mmdc.GetAllManufacturers().ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            int count = 1;
            //Populating the Manufacturers code from the list we got from db
            foreach (var man in manufacturersList)
            {
                selManufacturersCode.Add(new SelectListItem
                {
                    Text = man.ManufacturCd + " - " + man.ManufacturName,
                    Value = man.ManufacturCd
                });
                count++;
            }

            PartModel.drpManufacturerCodeList = selManufacturersCode;
            return PartModel;
        }


        public ActionResult MasterPartInsert([Bind]ManageMasterPartModel PartModel)
        {

            string msg = "";
            string msgType = "";
            bool isSuccess = false;

            if (String.IsNullOrEmpty(PartModel.PartNumber))
            {
                return View("ManageMasterPart", PartModel);
            }

            MasterPart masterPartResult = new MasterPart();
            MasterPart masterPartToInsert = new MasterPart();
            masterPartToInsert.PartsGroupCd = PartModel.PartGroupCodeList;
            masterPartToInsert.Manufactur = PartModel.ManufacturerCodeList;
            masterPartToInsert.PartCd = String.IsNullOrEmpty(PartModel.PartNumber) ? "" : PartModel.PartNumber.Trim();
            masterPartToInsert.PartDesignation1 = String.IsNullOrEmpty(PartModel.PartDesignation1) ? "" : PartModel.PartDesignation1.Trim();
            masterPartToInsert.PartDesignation2 = String.IsNullOrEmpty(PartModel.PartDesignation2) ? "" : PartModel.PartDesignation2.Trim();
            masterPartToInsert.PartDesignation3 = String.IsNullOrEmpty(PartModel.PartDesignation3) ? "" : PartModel.PartDesignation3.Trim();
            masterPartToInsert.Quantity = PartModel.Quantity;
            masterPartToInsert.PartDesc = String.IsNullOrEmpty(PartModel.PartDescription) ? "" : PartModel.PartDescription.Trim();
            masterPartToInsert.PartPrice = PartModel.Amount;
            masterPartToInsert.CoreValue = PartModel.CoreValue;

            masterPartToInsert.DeductCoreSW = PartModel.DeductCoreValueList;
            masterPartToInsert.SerialTagSW = PartModel.TAG;
            masterPartToInsert.MslPartSW = PartModel.Maersk;
            masterPartToInsert.PartActiveSW = PartModel.Active;

            masterPartToInsert.Remarks = String.IsNullOrEmpty(PartModel.Comment) ? "" : PartModel.Comment.Trim();
            masterPartToInsert.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;

            //try
            //{
            if (PartModel.isEdit == "1")
            {
                // Update
                try
                {
                    masterPartResult = mmdc.UpdateMasterPart(masterPartToInsert);
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    msg = "Error while updating data.";
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }

                if (masterPartResult.IsMasterPartEditSuccess)
                {
                    masterPartResult.Message = "Master Part Record- " + masterPartResult.PartCd + " Updated";
                    isSuccess = true;
                }
                else if (masterPartResult.IsMasterPartExist == false)
                {
                    masterPartResult.Message = "Master Part Record- " + masterPartResult.PartCd + " Not found. Update Not Performed";
                }
                else if (masterPartResult.IsMasterPartEditSuccess == false)
                {
                    masterPartResult.Message = "Master Part Record- " + masterPartResult.PartCd + ". Update Not Performed";
                }

                /////////////////////////////////

                SearchMasterPartModel searchMasterPartModel = new SearchMasterPartModel();
                SelectListItem selListItem = new SelectListItem();
                selListItem.Text = "";
                selListItem.Value = "";
                LoadAllPartsDropDowns(PartModel);
                //PartModel.drpActiveList.Insert(0, selListItem);
                //PartModel.drpDeductCoreValueList.Insert(0, selListItem);
                //PartModel.drpManufacturerCodeList.Insert(0, selListItem);
                //PartModel.drpPartGroupCodeList.Insert(0, selListItem);
                searchMasterPartModel.drpActive = PartModel.drpActiveList;
                searchMasterPartModel.drpCore = PartModel.drpDeductCoreValueList;
                searchMasterPartModel.drpDeductCoreValue = PartModel.drpDeductCoreValueList;
                searchMasterPartModel.drpManufacturerCode = PartModel.drpManufacturerCodeList;
                searchMasterPartModel.drpPartGroupCode = PartModel.drpPartGroupCodeList;
                PartModel.SearchMasterPartModel = searchMasterPartModel;

                /////////////////////////////////

                PartModel.showQuery = true;
            }

            else
            {
                // Insert

                try
                {
                    masterPartResult = mmdc.AddMasterPart(masterPartToInsert);
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    msg = "Error while saving data.";
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }


                if (masterPartResult.IsMasterPartExist)
                {
                    masterPartResult.Message = "Master Part Record- " + masterPartResult.PartCd + " Already Exists, Not Added";

                }
                else if (masterPartResult.IsMasterPartAddSuccess)
                {
                    masterPartResult.Message = "Master Part Record- " + masterPartResult.PartCd + " Added";
                    isSuccess = true;

                }
                else if (masterPartResult.IsMasterPartAddSuccess == false)
                {
                    masterPartResult.Message = "Error - Master Part Record- " + masterPartResult.PartCd + " Not Added";

                }

                LoadAllPartsDropDowns(PartModel);
                PartModel.showAdd = true;
                PartModel.isAddMode = "Y";

            }

            //}
            //catch
            //{
            //    isSuccess = false;
            //}

            PartModel.Message = masterPartResult.Message;

            if (String.IsNullOrEmpty(msg))
            {
                msg = PartModel.Message;
            }

            msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
            if (isSuccess) msgType = UtilityClass.UtilMethods.ERRORTYPE.Success.ToString();

            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, msgType);

            return View("ManageMasterPart", PartModel);
        }


        #endregion MasterPart


        #region CountryContract

        //****************************** START - COUNTRY CONTRACT *************************\\


        private void PopulateCountryDropDown(ManageCountryContractModel countryContractModel)
        {
            List<SelectListItem> selCountryList = new List<SelectListItem>();
            List<Country> countryList = new List<Country>();

            try
            {
                countryList = mmdc.GetAllCountries().ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }


            int count = 1;
            //Populating the country List from the list we got from db
            foreach (var country in countryList)
            {
                selCountryList.Add(new SelectListItem
                {
                    //Text = country.CountryCode + " - " + country.CountryDescription,
                    Text = country.CountryCode,
                    Value = country.CountryCode
                });
                count++;
            }

            //selCountryList.Insert(0, new SelectListItem
            //{
            //    Text = "",
            //    Value = ""
            //});


            countryContractModel.drpCountryList = selCountryList;

        }

        private void PopulateModeDropDown(ManageCountryContractModel countryContractModel)
        {
            List<SelectListItem> selModeList = new List<SelectListItem>();
            List<Mode> modeList = new List<Mode>();

            try
            {
                modeList = mmdc.GetAllModes().ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }


            int count = 1;
            //Populating the mode List from the list we got from db
            foreach (var mode in modeList)
            {
                selModeList.Add(new SelectListItem
                {
                    Text = mode.ModeCode + " - " + mode.ModeDescription,
                    Value = mode.ModeCode
                });
                count++;
            }

            //selModeList.Insert(0, new SelectListItem
            //{
            //    Text = "",
            //    Value = ""
            //});

            countryContractModel.drpModeList = selModeList;
            //countryContractModel.modeList = selModeList[0].Text;
        }

        private List<GridCountryContractModel> GetResultForCountryContractGrid(string countryCode, string repairCode, string mode)
        {
            List<GridCountryContractModel> lstCountryContractModel = new List<GridCountryContractModel>();
            List<CountryCont> lstCountryCont = new List<CountryCont>();
            GridCountryContractModel gridCountryContractModel = null;
            try
            {
                lstCountryCont = mmdc.GetCountryContractByQuery(countryCode, repairCode, mode).ToList();

                foreach (CountryCont cc in lstCountryCont)
                {
                    gridCountryContractModel = new GridCountryContractModel();
                    gridCountryContractModel.CountryCode = cc.CountryCode;
                    gridCountryContractModel.RepairCode = cc.RepairCod;
                    gridCountryContractModel.ContractAmount = String.Format("{0:0.00}", cc.ContractAmount);
                    gridCountryContractModel.CUCDN = cc.CUCDN;
                    gridCountryContractModel.ManualCode = cc.ManualCode;
                    gridCountryContractModel.Mode = cc.Mode;
                    gridCountryContractModel.EffectiveDate = String.Format("{0:yyyy-MM-dd}", cc.EffectiveDate);
                    gridCountryContractModel.ExpiryDate = String.Format("{0:yyyy-MM-dd}", cc.ExpiryDate);

                    lstCountryContractModel.Add(gridCountryContractModel);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstCountryContractModel;
        }

        public ActionResult ManageCountryContract(ManageCountryContractModel model)
        {
            try
            {
                PopulateCountryDropDown(model);
                PopulateModeDropDown(model);

            }
            catch (Exception ex)
            {
                string msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Error while load", msgType);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return View("ManageCountryContract", model);

        }

        public ActionResult ManageCountryContract_View(ManageCountryContractModel model)
        {
            try
            {
                PopulateCountryDropDown(model);
                PopulateModeDropDown(model);

            }
            catch (Exception ex)
            {
                string msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Error while load", msgType);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return View("ManageCountryContract_View", model);

        }

        public ActionResult PopulateCountryContractResultGrid(ManageCountryContractModel ccModel)
        {

            List<GridCountryContractModel> lstGridCountryContractModel = new List<GridCountryContractModel>();
            ManageCountryContractModel model = new ManageCountryContractModel();

            //ccModel.countryList = String.IsNullOrEmpty(ccModel.countryList) ? "" : ccModel.countryList.Trim();
            ccModel.RepairCode = String.IsNullOrEmpty(ccModel.RepairCode) ? "" : ccModel.RepairCode.Trim();
            //ccModel.modeList = String.IsNullOrEmpty(ccModel.modeList) ? "" : ccModel.modeList.Trim();

            string msg = "";

            try
            {
                PopulateCountryDropDown(model);
                PopulateModeDropDown(model);

                lstGridCountryContractModel = GetResultForCountryContractGrid(ccModel.countryList, ccModel.RepairCode, ccModel.modeList);
            }
            catch (Exception ex)
            {
                msg = "Error while fetching search result.";
                string msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
                //TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, msgType);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            ViewBag.GridData = lstGridCountryContractModel;

            if (lstGridCountryContractModel.Count > 0)
            {
                model.IsShowGrid = true;
            }
            else
            {
                model.Message = "There were no results meeting your query parameters";
                if (String.IsNullOrEmpty(msg))
                {
                    msg = model.Message;
                }
            }


            if (!String.IsNullOrEmpty(msg))
            {
                string msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, msgType);
            }

            return View("ManageCountryContract", model);
        }


        //****************************** END - COUNTRY CONTRACT *****************************\\

        #endregion CountryContract


        #region PartGroup



        private void PopulatePartGroupActiveDropDown(ManagePartGroupModel partGroupModel)
        {
            List<SelectListItem> selActiveCode = new List<SelectListItem>();
            selActiveCode = PopulateYNDropDown();

            partGroupModel.drpPartGroupActive = selActiveCode;
            partGroupModel.IsPartGroupActive = selActiveCode[0].Text;

            partGroupModel.showQueryResult = true;

            //ViewData["drpPartGroupActive"] = selActiveCode;
        }


        public JsonResult JsonGetPartGroupDetail(string id)
        {

            PartsGroup data = new PartsGroup();
            string msg = ""; string msgType = "";
            try
            {
                data = mmdc.GetPartsGroupByQuery(id);
                if (data.PartsGroupCd == null)
                {
                    msg = "No data found";
                }
            }
            catch (Exception ex)
            {
                msg = "Error while fetching data.";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
            if (!String.IsNullOrEmpty(msg))
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, msgType);
            }

            return Json(data);

        }


        public ActionResult ManagePartGroup()
        {
            string msg = ""; string msgType = "";
            List<PartsGroup> partsGroupList = new List<PartsGroup>();
            ManagePartGroupModel partGroupModel = new ManagePartGroupModel();

            try
            {
                PopulatePartGroupCodeDropDown(partGroupModel);
                PopulatePartGroupActiveDropDown(partGroupModel);
                partGroupModel.Message = String.Empty;
            }
            catch (Exception ex)
            {
                msg = "Error while load.";
            }

            msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
            if (!String.IsNullOrEmpty(msg))
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, msgType);
            }

            return View("ManagePartGroup", partGroupModel);
        }


        [HttpPost]
        public ActionResult PartGroupInsert([Bind]ManagePartGroupModel partGroupModel)
        {
            bool isSuccess = false;
            string msgType = "";
            string msg = "";

            //try
            //{
            PartsGroup partGroupResult = new PartsGroup();
            PartsGroup partGroupToInsert = new PartsGroup();
            partGroupToInsert.PartsGroupCd = partGroupModel.PartGroupCode.Trim();
            partGroupToInsert.PartsGroupDesc = partGroupModel.PartGroupDescription.Trim();
            partGroupToInsert.PartsGroupActiveSW = partGroupModel.IsPartGroupActive.Trim();
            partGroupToInsert.Remarks = partGroupModel.PartGroupComment == null ?
            partGroupModel.PartGroupComment : partGroupModel.PartGroupComment.Trim();
            partGroupToInsert.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;


            if (partGroupModel.IsPartGruopAddMode == "TRUE") // Insert
            {
                try
                {
                    partGroupResult = mmdc.AddPartsGroup(partGroupToInsert);
                }
                catch (Exception ex)
                {
                    msg = "Error while adding data.";
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }

                if (partGroupResult.IsPartsGroupCodeExists)
                {
                    partGroupModel.Message = "Parts Group " + partGroupModel.PartGroupCode.Trim() + " Already Exists, Not Added";
                    isSuccess = false;
                }
                else if (partGroupResult.IsPartsGroupAddUpdateSuccess == true)
                {
                    partGroupModel.Message = "Parts Group " + partGroupModel.PartGroupCode.Trim() + " Added";
                    isSuccess = true;
                }
                else if (partGroupResult.IsPartsGroupAddUpdateSuccess == false)
                {
                    partGroupModel.Message = "Error - Parts Group " + partGroupModel.PartGroupCode.Trim() + " Not Added";
                    isSuccess = false;
                }

                if (isSuccess) // Add success
                {
                    //partGroupModel.IsPartGruopHideDetail = "N";
                    //partGroupModel.IsPartGruopAddMode = "FALSE";
                    //JsonResult js = new JsonResult();
                    //js = JsonGetPartGroupDetail(partGroupModel.PartGroupCode.Trim());

                    // Get a ClientScriptManager reference from the Page class.
                    //ClientScriptManager cs = Page.ClientScript;
                    //System.Web.UI.ClientScriptManager sm = ClientScriptManager();
                    //string txt = "<script> $('#btnQuery').click(function () { $('#div_GroupDetails').show(); $('#div_UserName_Time').show();});</script>";
                    //sm.RegisterStartupScript(this.GetType(), "PopupScript", txt);

                    //return JavaScript(txt);
                    //this.AddJavaScriptFunction("addTableRow", "My Message");

                }
            }
            else if (partGroupModel.IsPartGruopAddMode == "FALSE") // Update
            {

                try
                {
                    isSuccess = mmdc.UpdatePartsGroup(partGroupToInsert);
                }
                catch (Exception ex)
                {
                    msg = "Error while updating data.";
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }

                if (isSuccess)
                {
                    partGroupModel.Message = "Parts Group " + partGroupModel.PartGroupCode.Trim() + " Updated";
                    isSuccess = true;
                }
                else
                {
                    partGroupModel.Message = "Error - Parts Group " + partGroupModel.PartGroupCode.Trim() + " Not Updated";
                }
            }
            else // something wrong as hiden fld is blank
            {
                // do nothing
                partGroupModel.Message = "";
            }

            PopulatePartGroupCodeDropDown(partGroupModel);
            PopulatePartGroupActiveDropDown(partGroupModel);
            partGroupModel.PartGroupCodeList = partGroupModel.PartGroupCode;



            //}
            //catch (Exception e)
            //{
            //    partGroupModel.Message = "Server error";

            //}

            if (isSuccess)
            {
                msgType = UtilityClass.UtilMethods.ERRORTYPE.Success.ToString();
            }
            else
            {
                msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
            }


            if (String.IsNullOrEmpty(msg))
            {
                msg = partGroupModel.Message;
            }


            if (!String.IsNullOrEmpty(msg))
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, msgType);
            }

            return View("ManagePartGroup", partGroupModel);
        }

        #endregion  PartGroup


        #region Repair Code/Part Association

        private void PopulateActiveManuals(ManageRepairPartAssnModel mod)
        {
            List<SelectListItem> selManuals = new List<SelectListItem>();
            List<Manual> lstManual = new List<Manual>();

            try
            {
                lstManual = mmdc.GetAllActiveManuals().ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }


            foreach (var manual in lstManual)
            {
                selManuals.Add(new SelectListItem
                {
                    Text = manual.ManualCode + " - " + manual.ManualDesc,
                    Value = manual.ManualCode
                });

            }


            mod.drpManualCode = selManuals;
            if (selManuals.Count > 0) mod.ManualCode = selManuals[0].Value;
        }

        private void PopulateActiveModes(ManageRepairPartAssnModel mod)
        {
            List<SelectListItem> selModes = new List<SelectListItem>();
            List<Mode> lstModes = new List<Mode>();

            try
            {
                lstModes = mmdc.GetAllActiveModes().ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            int count = 1;

            foreach (var mode in lstModes)
            {
                selModes.Add(new SelectListItem
                {
                    Text = mode.ModeCode + " - " + mode.ModeDescription,
                    Value = mode.ModeCode
                });
                count++;
            }

            //selModes.Insert(0, new SelectListItem
            //{
            //    Text = "",
            //    Value = ""
            //});

            mod.drpModeCode = selModes;
            if (selModes.Count > 0) mod.ModeCode = selModes[0].Value;
        }



        public ActionResult ManageRCPAssociation(ManageRepairPartAssnModel model)
        {
            ManageRepairPartAssnModel rpcModel = new ManageRepairPartAssnModel();

            try
            {
                if (Request.QueryString["Msg"] != null && Request.QueryString["MsgType"] != null)
                // && Request.QueryString["Msg"].Contains("Association"))
                {
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(Request.QueryString["Msg"], Request.QueryString["MsgType"]);
                }
                else
                {
                    PopulateActiveManuals(rpcModel);
                    PopulateActiveModes(rpcModel);
                }

                LoadRPASearchFilterDropDowns(rpcModel);

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                UtilityClass.UtilMethods.GenErrorMessage("Exception during Load.", UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString());
            }


            List<GridRepairPartAssnModel> lstGridRepairPartAssnModel = new List<GridRepairPartAssnModel>();

            ViewBag.GridData = lstGridRepairPartAssnModel;

            rpcModel.showQuery = true;

            return View("ManageRprCodePartAssn", rpcModel);

        }


        public ActionResult ManageRCPAssociation_View(ManageRepairPartAssnModel model) //Debadrita_User_Remapping
        {
            ManageRepairPartAssnModel rpcModel = new ManageRepairPartAssnModel();

            try
            {
                if (Request.QueryString["Msg"] != null && Request.QueryString["MsgType"] != null)
                // && Request.QueryString["Msg"].Contains("Association"))
                {
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(Request.QueryString["Msg"], Request.QueryString["MsgType"]);
                }
                else
                {
                    PopulateActiveManuals(rpcModel);
                    PopulateActiveModes(rpcModel);
                }

                LoadRPASearchFilterDropDowns(rpcModel);

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                UtilityClass.UtilMethods.GenErrorMessage("Exception during Load.", UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString());
            }


            List<GridRepairPartAssnModel> lstGridRepairPartAssnModel = new List<GridRepairPartAssnModel>();

            ViewBag.GridData = lstGridRepairPartAssnModel;

            rpcModel.showQuery = true;

            return View("ManageRprCodePartAssn_View", rpcModel);

        }

        public ActionResult PopulateRprCode_PartAssResultGrid(ManageRepairPartAssnModel model)
        {

            //ManageRepairPartAssnModel model = new ManageRepairPartAssnModel();
            string msg = "";
            string msgType = "";
            ManageRepairPartAssnModel rpaModel = new ManageRepairPartAssnModel();



            //try
            //{

            List<RepairCodePart> lstRepairCodePart = new List<RepairCodePart>();

            try
            {
                lstRepairCodePart = mmdc.GetRepairCode_PartAssociation(model.SearchRepairPartAssnModel.ScrhModeCode, model.SearchRepairPartAssnModel.ScrhManualCode,
          model.SearchRepairPartAssnModel.ScrhRepairCod, model.SearchRepairPartAssnModel.ScrhPartNumber).ToList();

            }
            catch (Exception ex)
            {
                msg = "Error while fetching data.";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }



            GridRepairPartAssnModel gridRepairPartAssnModel;
            List<GridRepairPartAssnModel> lstGridRepairPartAssnModel = new List<GridRepairPartAssnModel>();
            if (lstRepairCodePart != null && lstRepairCodePart.Count > 0)
            {
                foreach (RepairCodePart rpc in lstRepairCodePart)
                {
                    gridRepairPartAssnModel = new GridRepairPartAssnModel();

                    gridRepairPartAssnModel.GridManualCode = rpc.ManualCode;
                    gridRepairPartAssnModel.GridModeCode = rpc.ModeCode;
                    gridRepairPartAssnModel.GridPartNumber = rpc.PartNumber;
                    gridRepairPartAssnModel.GridRepairCod = rpc.RepairCod;
                    gridRepairPartAssnModel.GridRepairDesc = rpc.RepairDesc;
                    gridRepairPartAssnModel.GridPartDesc = rpc.PartDesc;
                    gridRepairPartAssnModel.GridMaxPartQty = rpc.MaxPartQty;

                    lstGridRepairPartAssnModel.Add(gridRepairPartAssnModel);

                }
            }

            ViewBag.GridData = lstGridRepairPartAssnModel;


            rpaModel = model;
            rpaModel.showQuery = true;

            msgType = UtilityClass.UtilMethods.ERRORTYPE.Success.ToString();

            if (lstGridRepairPartAssnModel.Count > 0)
            {
                rpaModel.showQueryResult = true;
            }
            else
            {
                rpaModel.Message = "There were no results meeting your query parameters";
                if (String.IsNullOrEmpty(msg))
                {
                    msg = rpaModel.Message;
                }

                msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
            }

            LoadRPASearchFilterDropDowns(rpaModel);

            //}
            //catch (Exception ex)
            //{
            //    msg = "Service Exception";
            //    msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
            //    logEntry.Message = ex.ToString();
            //    Logger.Write(logEntry);
            //}

            if (!String.IsNullOrEmpty(msg))
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, msgType);
            }

            return View("ManageRprCodePartAssn", rpaModel);
        }

        public ActionResult PopulateRprCode_PartAssResultGrid_View(ManageRepairPartAssnModel model)
        {

            //ManageRepairPartAssnModel model = new ManageRepairPartAssnModel();
            string msg = "";
            string msgType = "";
            ManageRepairPartAssnModel rpaModel = new ManageRepairPartAssnModel();



            //try
            //{

            List<RepairCodePart> lstRepairCodePart = new List<RepairCodePart>();

            try
            {
                lstRepairCodePart = mmdc.GetRepairCode_PartAssociation(model.SearchRepairPartAssnModel.ScrhModeCode, model.SearchRepairPartAssnModel.ScrhManualCode,
          model.SearchRepairPartAssnModel.ScrhRepairCod, model.SearchRepairPartAssnModel.ScrhPartNumber).ToList();

            }
            catch (Exception ex)
            {
                msg = "Error while fetching data.";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }



            GridRepairPartAssnModel gridRepairPartAssnModel;
            List<GridRepairPartAssnModel> lstGridRepairPartAssnModel = new List<GridRepairPartAssnModel>();
            if (lstRepairCodePart != null && lstRepairCodePart.Count > 0)
            {
                foreach (RepairCodePart rpc in lstRepairCodePart)
                {
                    gridRepairPartAssnModel = new GridRepairPartAssnModel();

                    gridRepairPartAssnModel.GridManualCode = rpc.ManualCode;
                    gridRepairPartAssnModel.GridModeCode = rpc.ModeCode;
                    gridRepairPartAssnModel.GridPartNumber = rpc.PartNumber;
                    gridRepairPartAssnModel.GridRepairCod = rpc.RepairCod;
                    gridRepairPartAssnModel.GridRepairDesc = rpc.RepairDesc;
                    gridRepairPartAssnModel.GridPartDesc = rpc.PartDesc;
                    gridRepairPartAssnModel.GridMaxPartQty = rpc.MaxPartQty;

                    lstGridRepairPartAssnModel.Add(gridRepairPartAssnModel);

                }
            }

            ViewBag.GridData = lstGridRepairPartAssnModel;


            rpaModel = model;
            rpaModel.showQuery = true;

            msgType = UtilityClass.UtilMethods.ERRORTYPE.Success.ToString();

            if (lstGridRepairPartAssnModel.Count > 0)
            {
                rpaModel.showQueryResult = true;
            }
            else
            {
                rpaModel.Message = "There were no results meeting your query parameters";
                if (String.IsNullOrEmpty(msg))
                {
                    msg = rpaModel.Message;
                }

                msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
            }

            LoadRPASearchFilterDropDowns(rpaModel);

            //}
            //catch (Exception ex)
            //{
            //    msg = "Service Exception";
            //    msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
            //    logEntry.Message = ex.ToString();
            //    Logger.Write(logEntry);
            //}

            if (!String.IsNullOrEmpty(msg))
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, msgType);
            }

            return View("ManageRprCodePartAssn_View", rpaModel);
        }


        private void LoadRPASearchFilterDropDowns(ManageRepairPartAssnModel model)
        {
            if (model.drpManualCode == null
                || model.drpManualCode.Count == 0)
            {
                PopulateActiveManuals(model);
                //PopulateActiveModes(model);
            }

            JsonResult res;
            if (model.SearchRepairPartAssnModel == null) // 1st time load
            {
                res = JsonGetModeByManual(model.ManualCode);

            }
            else // post back
            {
                res = JsonGetModeByManual(model.SearchRepairPartAssnModel.ScrhManualCode);
            }

            SearchRepairPartAssnModel searchRpcModel = new SearchRepairPartAssnModel();
            searchRpcModel.drpManualCode = model.drpManualCode;

            List<Mode> lstMod = new List<Mode>();
            lstMod = (List<Mode>)res.Data;

            List<SelectListItem> selItem = new List<SelectListItem>();

            foreach (var mod in lstMod)
            {
                selItem.Add(new SelectListItem
                {
                    Text = mod.ModeFullDescription,
                    Value = mod.ModeCode
                });
            }

            searchRpcModel.drpModeCode = selItem;
            model.SearchRepairPartAssnModel = searchRpcModel;

        }

        public ActionResult DelMode_RPAssn()
        {

            ManageRepairPartAssnModel rpaModel = new ManageRepairPartAssnModel();

            string msg = string.Empty;
            //string code = string.Empty;
            //code = RouteData.Values["id"].ToString();

            //string[] allCode = code.Split('-');
            //if (allCode.Count() < 4)
            //{

            //    return null;
            //}
            string manCode = string.Empty;
            string modeCode = string.Empty;
            string repCode = string.Empty;
            string partCode = string.Empty;
            //manCode = allCode[0]; modeCode = allCode[1]; repCode = allCode[2]; partCode = allCode[3];
            string msgType = "";

            try
            {
                LoadRPASearchFilterDropDowns(rpaModel);
                rpaModel.showQuery = true;

                manCode = Request.QueryString[0];
                modeCode = Request.QueryString[1];
                repCode = Request.QueryString[2];
                partCode = Request.QueryString[3];

                ServiceResult result = new ServiceResult();
                try
                {
                    result = mmdc.DeleteRPRCODE_PART(repCode.Trim(), partCode.Trim(), modeCode.Trim(), manCode.Trim());
                }
                catch (Exception ex)
                {
                    msg = "Errro while deleting record.";
                    msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }

                if (String.IsNullOrEmpty(msg))
                {
                    msg = result.Message;
                }

                if (result.IsSuccess)
                {
                    msgType = UtilityClass.UtilMethods.ERRORTYPE.Success.ToString();
                }
                else
                {
                    msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
                }

            }
            catch (Exception ex)
            {
                msg = "Error in Delete action.";
                msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }

            rpaModel.Message = msg;
            rpaModel.ManualCode = manCode;
            rpaModel.ModeCode = modeCode;
            rpaModel.RepairCod = repCode;
            rpaModel.PartNumber = partCode;
            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(rpaModel.Message, msgType);


            string toUrl = "";
            string rq = Request.UrlReferrer.ToString();
            int ind = rq.LastIndexOf('/');
            toUrl = rq.Substring(0, ind) + "/ManageRCPAssociation?Msg=" + msg + "&MsgType=" + msgType;
            Response.Redirect(toUrl);

            return View("ManageRprCodePartAssn", rpaModel);

        }

        public ActionResult AddMode_RPAssn()
        {

            ManageRepairPartAssnModel rpaModel = new ManageRepairPartAssnModel();

            try
            {
                PopulateActiveManuals(rpaModel);
                PopulateActiveModes(rpaModel);
                rpaModel.showAdd = true;
                //rpaModel.isAMode = true;
                rpaModel.isAdd = "Y";
                return View("ManageRprCodePartAssn", rpaModel);
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }

            return View("ManageRprCodePartAssn", rpaModel);

        }

        public ActionResult EditMode_RPAssn()
        {
            string code = string.Empty;
            string msgType = string.Empty;
            string manCode = string.Empty;
            string modeCode = string.Empty;
            string repCode = string.Empty;
            string partCode = string.Empty;
            ManageRepairPartAssnModel rpaModel = new ManageRepairPartAssnModel();


            try
            {
                PopulateActiveManuals(rpaModel);
                PopulateActiveModes(rpaModel);

                manCode = Request.QueryString[0];
                modeCode = Request.QueryString[1];
                repCode = Request.QueryString[2];
                partCode = Request.QueryString[3];

                rpaModel.OrgManualCode = manCode.Trim();
                rpaModel.OrgModeCode = modeCode.Trim();
                rpaModel.OrgPartNumber = partCode.Trim();
                rpaModel.OrgRepairCod = repCode.Trim();

                List<RepairCodePart> lstData = new List<RepairCodePart>();
                try
                {
                    lstData = mmdc.GetRepairCode_PartAssociation(modeCode, manCode, repCode, partCode).ToList();
                }
                catch (Exception ex)
                {
                    rpaModel.Message = "Error while fetching part code.";
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }



                if (lstData.Count > 0)
                {
                    //set the values from the DB in the boxes / drop downs for that part
                    rpaModel.ManualCode = lstData[0].ManualCode;
                    rpaModel.ModeCode = lstData[0].ModeCode;
                    rpaModel.PartNumber = lstData[0].PartNumber;
                    rpaModel.RepairCod = lstData[0].RepairCod;
                    rpaModel.MaxPartQty = lstData[0].MaxPartQty;
                    rpaModel.ChangeTime = lstData[0].ChangeTime;
                    rpaModel.ChangeUser = lstData[0].ChangeUser;
                }
                else
                {
                    //no data found message -- deleted by another user
                    if (String.IsNullOrEmpty(rpaModel.Message))
                    {
                        rpaModel.Message = "Edit - RepairCode/PartNumber- " + repCode + " / " + partCode + " Not found";
                    }
                    msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
                }

                List<GridRepairPartAssnModel> lstGridRepairPartAssnModel = new List<GridRepairPartAssnModel>();
                ViewBag.GridData = lstGridRepairPartAssnModel;

                rpaModel.showEdit = true;
                rpaModel.isEditMode = true;
                rpaModel.isEdit = "Y";

            }
            catch (Exception ex)
            {
                msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
                rpaModel.Message = "Error in edit mode.";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            if (!String.IsNullOrEmpty(rpaModel.Message))
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(rpaModel.Message, msgType);
            }

            return View("ManageRprCodePartAssn", rpaModel);

        }

        public ActionResult RepairCode_Part_Submit(ManageRepairPartAssnModel rpaModel)
        {

            string msg = string.Empty;
            RepairCodePart repairCodePart = new RepairCodePart();
            repairCodePart.ManualCode = rpaModel.ManualCode.Trim();
            repairCodePart.ModeCode = rpaModel.ModeCode.Trim();
            repairCodePart.MaxPartQty = rpaModel.MaxPartQty;
            repairCodePart.PartNumber = rpaModel.PartNumber.Trim();
            repairCodePart.RepairCod = rpaModel.RepairCod.Trim();


            repairCodePart.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;

            ServiceResult result = new ServiceResult();
            string msgType = "";

            try
            {
                if (rpaModel.isEdit == "Y")
                {
                    // Update

                    repairCodePart.OrgManualCode = rpaModel.OrgManualCode.Trim();
                    repairCodePart.OrgModeCode = rpaModel.OrgModeCode.Trim();
                    //repairCodePart.OrgMaxPartQty = rpaModel.OrgMaxPartQty.Trim();
                    repairCodePart.OrgPartNumber = rpaModel.OrgPartNumber.Trim();
                    repairCodePart.OrgRepairCod = rpaModel.OrgRepairCod.Trim();

                    rpaModel.showEdit = true;
                    rpaModel.isEditMode = true;
                    rpaModel.isEdit = "Y";

                    //check if the data is changed.
                    bool isDataChanged = false;
                    if (rpaModel.OrgManualCode.Trim() == rpaModel.ManualCode.Trim() &&
                        rpaModel.OrgModeCode.Trim() == rpaModel.ModeCode.Trim() &&
                        rpaModel.OrgPartNumber.Trim() == rpaModel.PartNumber.Trim() &&
                        rpaModel.OrgRepairCod.Trim() == rpaModel.RepairCod.Trim())
                    {
                        isDataChanged = false;
                        rpaModel.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;
                        // update only MaxPartQty, UserName, Time.           
                        try
                        {
                            result = mmdc.UpdateRPA(rpaModel.OrgManualCode, rpaModel.OrgModeCode, rpaModel.OrgPartNumber, rpaModel.OrgRepairCod, rpaModel.MaxPartQty, rpaModel.ChangeUser);
                        }
                        catch (Exception ex)
                        {
                            result.Message = "Error while updating part record.";
                            logEntry.Message = ex.ToString();
                            Logger.Write(logEntry);
                        }
                    }
                    else
                    {
                        try
                        {
                            result = mmdc.Add_Edit_RPA(repairCodePart, "EDIT");
                        }
                        catch (Exception ex)
                        {
                            result.Message = "Error while updating record.";
                            logEntry.Message = ex.ToString();
                            Logger.Write(logEntry);
                        }
                    }
                }
                else if (rpaModel.isAdd == "Y")
                {
                    //insert the new record.
                    rpaModel.showAdd = true;
                    rpaModel.isAdd = "Y";
                    try
                    {
                        result = mmdc.Add_Edit_RPA(repairCodePart, "ADD");
                    }
                    catch (Exception ex)
                    {
                        result.Message = "Error while adding record.";
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                    }

                }
                else
                {
                    //do nothing
                }

                PopulateActiveManuals(rpaModel);
                PopulateActiveModes(rpaModel);

            }
            catch (Exception ex)
            {
                result.Message = "Error while Update / Add ";
                result.IsSuccess = false;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            if (result.IsSuccess)
            {
                msgType = UtilityClass.UtilMethods.ERRORTYPE.Success.ToString();
            }
            else
            {
                msgType = UtilityClass.UtilMethods.ERRORTYPE.Warning.ToString();
            }

            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(result.Message, msgType);

            rpaModel.Message = result.Message;

            return View("ManageRprCodePartAssn", rpaModel);
        }


        public JsonResult JsonGetModeByManual(string id)
        {
            List<Mode> lstMode = new List<Mode>();
            try
            {
                lstMode = mmdc.GetModesByManuals(id).ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(lstMode);
        }


        #endregion Repair Code/Part Association

        #endregion Amlan



        #region Index
        public ActionResult Index()
        {
            ManageMasterDataIndexModel ManageMasterDataModel = new ManageMasterDataIndexModel();
            List<Manual> indexManuals = mmdc.GetIndexManual().ToList();
            ManageMasterDataModel.drpIndexManual = new SelectList(indexManuals, "ManualCode", "ManualDesc");
            ManageMasterDataModel.IndexSelectionMode = ManageMasterDataIndexModel.IndexSelectionModes.IndexManualCode;
            return View(ManageMasterDataModel);
        }

        public ActionResult IndexNew()
        {
            ManageMasterDataIndexModel ManageMasterDataModel = new ManageMasterDataIndexModel();
            List<Manual> indexManuals = mmdc.GetIndexAllActiveManual().ToList();
            ManageMasterDataModel.drpIndexManual = new SelectList(indexManuals, "ManualCode", "ManualDesc");
            ManageMasterDataModel.IndexSelectionMode = ManageMasterDataIndexModel.IndexSelectionModes.IndexManualCode;
            ManageMasterDataModel.IsInsertMode = true;
            return View("Index", ManageMasterDataModel);
        }

        //[HttpPost]
        public ActionResult GetIndexMode([Bind] ManageMasterDataIndexModel manageMasterDataModel)
        {
            List<Mode> indexModes = mmdc.GetIndexMode(manageMasterDataModel.IndexManualCode).ToList();
            manageMasterDataModel.drpIndexMode = new SelectList(indexModes, "ModeCode", "ModeDescription");
            //List<Manual> indexManuals = mmdc.GetIndexManual().ToList();
            //ManageMasterDataModel.drpIndexManual = new SelectList(indexManuals, "ManualCode", "ManualDesc",id);
            manageMasterDataModel.IndexSelectionMode = ManageMasterDataIndexModel.IndexSelectionModes.IndexMode;
            //ViewBag.ModifyButtonValue = "Update-Get Index";
            //ViewBag.AddButtonValue = "";
            return View("Index", manageMasterDataModel);
        }

        public ActionResult GetIndexModeAdd([Bind] ManageMasterDataIndexModel manageMasterDataModel)
        {
            List<Mode> indexModes = mmdc.GetIndexAllActiveMode(manageMasterDataModel.IndexManualCode).ToList();
            manageMasterDataModel.drpIndexMode = new SelectList(indexModes, "ModeCode", "ModeDescription");
            //List<Manual> indexManuals = mmdc.GetIndexManual().ToList();
            //ManageMasterDataModel.drpIndexManual = new SelectList(indexManuals, "ManualCode", "ManualDesc",id);
            manageMasterDataModel.IndexSelectionMode = ManageMasterDataIndexModel.IndexSelectionModes.IndexMode;
            manageMasterDataModel.IsInsertMode = true;
            //ViewBag.ModifyButtonValue = "Update-Get Index";
            //ViewBag.AddButtonValue = "";
            return View("Index", manageMasterDataModel);
        }

        public ActionResult GetIndexDropDown([Bind] ManageMasterDataIndexModel manageMasterDataModel, FormCollection frm)
        {
            string manualCode = frm["hddIndexManualCode"];
            List<Index> indexes = mmdc.GetIndexesForDropDown(manageMasterDataModel.IndexManualCode, manageMasterDataModel.IndexMode).ToList();
            manageMasterDataModel.drpIndex = new SelectList(indexes, "IndexID", "IndexDesc");
            //List<Manual> indexManuals = mmdc.GetIndexManual().ToList();
            //ManageMasterDataModel.drpIndexManual = new SelectList(indexManuals, "ManualCode", "ManualDesc",id);
            manageMasterDataModel.IndexSelectionMode = ManageMasterDataIndexModel.IndexSelectionModes.Index;
            //ViewBag.ModifyButtonValue = "Update-Get Index";
            //ViewBag.AddButtonValue = "";
            return View("Index", manageMasterDataModel);

        }

        public ActionResult GetIndexDropDownAdd([Bind] ManageMasterDataIndexModel manageMasterDataModel, FormCollection frm)
        {
            string manualCode = frm["hddIndexManualCode"];
            List<Index> indexes = mmdc.GetIndexesForDropDown(manageMasterDataModel.IndexManualCode, manageMasterDataModel.IndexMode).ToList();
            manageMasterDataModel.drpIndex = new SelectList(indexes, "IndexID", "IndexDesc");
            //List<Manual> indexManuals = mmdc.GetIndexManual().ToList();
            //ManageMasterDataModel.drpIndexManual = new SelectList(indexManuals, "ManualCode", "ManualDesc",id);
            manageMasterDataModel.IndexSelectionMode = ManageMasterDataIndexModel.IndexSelectionModes.IndexRecord;
            manageMasterDataModel.IsInsertMode = true;
            return View("Index", manageMasterDataModel);

        }

        public ActionResult GetIndexResult([Bind] ManageMasterDataIndexModel manageMasterDataModel)
        {
            ModelState.Clear();
            List<Index> indexes = mmdc.GetIndex(Convert.ToInt32(manageMasterDataModel.IndexID), manageMasterDataModel.IndexManualCode, manageMasterDataModel.IndexMode).ToList();
            manageMasterDataModel.IndexID = indexes[0].IndexID.ToString();
            manageMasterDataModel.IndexDesc = indexes[0].IndexDesc;
            manageMasterDataModel.IndexChangeTime = indexes[0].ChangeTime.ToString("yyyy-MM-dd hh:mm:ss tt");
            string username = indexes[0].ChangeUser;
            if (username.Contains("|"))
            {
                manageMasterDataModel.IndexChangeUserFName = username.Split('|')[0];
                manageMasterDataModel.IndexChangeUserLName = username.Split('|')[1];
            }
            else
            {
                manageMasterDataModel.IndexChangeUserFName = username;
                manageMasterDataModel.IndexChangeUserLName = "";
            }
            manageMasterDataModel.IndexChangeUserName = indexes[0].ChangeUser;
            manageMasterDataModel.IndexManualCode = indexes[0].ManualCode;
            manageMasterDataModel.IndexMode = indexes[0].Mode;
            manageMasterDataModel.IndexPriority = indexes[0].IndexPriority.ToString();
            manageMasterDataModel.IndexSelectionMode = ManageMasterDataIndexModel.IndexSelectionModes.IndexRecord;

            return View("Index", manageMasterDataModel);
        }

        public ActionResult GetIndexResultAdd([Bind] ManageMasterDataIndexModel manageMasterDataModel)
        {
            //List<Index> indexes = mmdc.GetIndex(Convert.ToInt32(manageMasterDataModel.IndexID), manageMasterDataModel.IndexManualCode, manageMasterDataModel.IndexMode).ToList();
            //manageMasterDataModel.IndexID = indexes[0].IndexID.ToString();
            //manageMasterDataModel.IndexDesc = indexes[0].IndexDesc;
            //manageMasterDataModel.IndexChangeTime = indexes[0].ChangeTime.ToString("yyyy-MM-dd hh:mm");
            //manageMasterDataModel.IndexChangeUserName = indexes[0].ChangeUser;
            //manageMasterDataModel.IndexManualCode = indexes[0].ManualCode;
            //manageMasterDataModel.IndexMode = indexes[0].Mode;
            //manageMasterDataModel.IndexPriority = indexes[0].IndexPriority.ToString();
            manageMasterDataModel.IndexSelectionMode = ManageMasterDataIndexModel.IndexSelectionModes.IndexRecord;
            manageMasterDataModel.IsInsertMode = true;
            return View("Index", manageMasterDataModel);
        }
        public ActionResult DeleteIndex([Bind] ManageMasterDataIndexModel manageMasterDataModel)
        {
            bool sucess = false;
            string msg = "";
            sucess = mmdc.DeleteIndex(Convert.ToInt32(manageMasterDataModel.IndexID), manageMasterDataModel.IndexManualCode, manageMasterDataModel.IndexMode, ref msg);// (period, ref msg);
            //ManageMasterDataPTIPeriodModel manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
            //manageMasterDataModel.PTIMessage = msg;
            if (sucess)
            {
                msg = "Index ID " + manageMasterDataModel.IndexID + " deleted";
                manageMasterDataModel.IndexSelectionMode = ManageMasterDataIndexModel.IndexSelectionModes.IndexManualCode;
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Success");
            }
            else
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
            //return View("Index", manageMasterDataModel);
            return RedirectToAction("Index");
        }

        public ActionResult InsertIndex([Bind] ManageMasterDataIndexModel manageMasterDataModel)
        {
            bool sucess = false;
            string msg = "";
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            Index index = new ManageMasterDataServiceReference.Index();
            index.ChangeUser = userId;
            index.IndexDesc = manageMasterDataModel.IndexDesc;
            index.IndexID = Convert.ToInt32(manageMasterDataModel.IndexID);
            index.ManualCode = manageMasterDataModel.IndexManualCode;
            index.Mode = manageMasterDataModel.IndexMode;
            sucess = mmdc.CreateIndex(index, ref msg);
            //manageMasterDataModel.PTIMessage = msg;
            if (sucess)
            {
                msg = "Index ID " + manageMasterDataModel.IndexID + " Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Success");
                //manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
                //ModelState.Clear();
                //manageMasterDataModel.IsEditPTIPeriod = true;
                //manageMasterDataModel.PTIMessage = "PTI Exception Period " + msg + " Updated";
            }
            else
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
            return RedirectToAction("GetIndexResultAdd", manageMasterDataModel); //View("Index", manageMasterDataModel);
        }

        public ActionResult ModifyIndex([Bind] ManageMasterDataIndexModel manageMasterDataModel)
        {
            bool sucess = false;
            string msg = "";
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            Index index = new ManageMasterDataServiceReference.Index();
            index.ChangeUser = userId;
            index.IndexDesc = manageMasterDataModel.IndexDesc;
            index.IndexID = Convert.ToInt32(manageMasterDataModel.IndexID);
            index.ManualCode = manageMasterDataModel.IndexManualCode;
            index.Mode = manageMasterDataModel.IndexMode;
            sucess = mmdc.UpdateIndex(index, ref msg);
            //manageMasterDataModel.PTIMessage = msg;
            if (sucess)
            {
                msg = "Index ID " + manageMasterDataModel.IndexID + " updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Success");
            }
            else
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
            //return View("Index", manageMasterDataModel);
            return RedirectToAction("GetIndexResult", manageMasterDataModel);
        }
        #endregion Index

        #region PTIPeriod
        public ActionResult CreatePTIPeriod([Bind] ManageMasterDataPTIPeriodModel manageMasterDataModel)
        {
            bool sucess = false;
            string msg = "";
            PTIPeriod period = new PTIPeriod();
            period.EqpNoFrom = manageMasterDataModel.PTIPeriodFrom;
            period.EqpNoTo = manageMasterDataModel.PTIPeriodTo;
            period.ExceptionDays = manageMasterDataModel.PTIPeriodNumber;
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            period.PTIChUser = userId;
            sucess = mmdc.CreatePTIPeriod(period, ref msg);
            manageMasterDataModel.PTIMessage = msg;
            if (sucess)
            {
                msg = manageMasterDataModel.PTIPeriodFrom + " / " + manageMasterDataModel.PTIPeriodTo;
                manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
                ModelState.Clear();
                manageMasterDataModel.IsEditPTIPeriod = false;
                //manageMasterDataModel.PTIMessage = "PTI Exception Period " + msg + " Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("PTI Exception Period " + msg + " Added", "Success");
                return View("PTIPeriod", manageMasterDataModel);
            }
            else
            {
                manageMasterDataModel.IsEditPTIPeriod = false;
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
                return View("PTIPeriodEdit", manageMasterDataModel);
            }
        }

        public ActionResult ModifyPTIPeriod([Bind] ManageMasterDataPTIPeriodModel manageMasterDataModel)
        {
            bool sucess = false;
            string msg = "";
            PTIPeriod period = new PTIPeriod();
            period.EqpNoFrom = manageMasterDataModel.PTIPeriodFrom;
            period.EqpNoTo = manageMasterDataModel.PTIPeriodTo;
            period.ExceptionDays = manageMasterDataModel.PTIPeriodNumber;
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            period.PTIChUser = userId;
            sucess = mmdc.ModifyPTIPeriod(period, ref msg);
            manageMasterDataModel.PTIMessage = msg;
            if (sucess)
            {
                msg = manageMasterDataModel.PTIPeriodFrom + " / " + manageMasterDataModel.PTIPeriodTo;
                //manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
                //ModelState.Clear();
                manageMasterDataModel.IsEditPTIPeriod = true;
                //manageMasterDataModel.PTIMessage = "PTI Exception Period " + msg + " Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("PTI Exception Period " + msg + " Updated", "Success");
                return View("PTIPeriod", manageMasterDataModel);
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
                return View("PTIPeriodEdit", manageMasterDataModel);
            }
            //return View("PTIPeriodEdit");
        }

        public ActionResult DeletePTIPeriod(string PTIFrom, string PTITo)
        {
            bool sucess = false;
            string msg = "";
            PTIPeriod period = new PTIPeriod();
            period.EqpNoFrom = PTIFrom;
            period.EqpNoTo = PTITo;
            //period.ExceptionDays = manageMasterDataModel.PTIPeriodNumber;
            //string userId = ((List<MercPlusClient.ManageUserServiceReference.UserInfo>)System.Web.HttpContext.Current.Session["UserSec"])[0].UserId.ToString();
            //period.PTIChUser = userId;
            sucess = mmdc.DeletePTIPeriod(period, ref msg);
            ManageMasterDataPTIPeriodModel manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
            manageMasterDataModel.PTIMessage = msg;
            if (sucess)
            {
                msg = PTIFrom + " / " + PTITo;
                //manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
                //ModelState.Clear();
                manageMasterDataModel.IsEditPTIPeriod = true;
                //manageMasterDataModel.PTIMessage = "PTI Exception Period " + msg + " Deleted";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("PTI Exception Period " + msg + " Deleted", "Success");
            }
            else
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
            return View("PTIPeriod", manageMasterDataModel);
        }

        public ActionResult PTIPeriodNew()
        {
            ManageMasterDataPTIPeriodModel model = new ManageMasterDataPTIPeriodModel();
            model.IsEditPTIPeriod = false;
            model.PTIMessage = " ";
            return View("PTIPeriodEdit", model);
        }
        public ActionResult PTIPeriodView(string PTIFrom, string PTITo)
        {
            ManageMasterDataPTIPeriodModel model = new ManageMasterDataPTIPeriodModel();
            List<PTIPeriod> ptiPeriod = new List<PTIPeriod>();
            PTIPeriod period = mmdc.GetPTIPeriod(PTIFrom, PTITo);
            ptiPeriod.Add(period);
            model.IsEditPTIPeriod = true;
            model.PTIPeriodFrom = ptiPeriod[0].EqpNoFrom.Trim();
            model.PTIPeriodTo = ptiPeriod[0].EqpNoTo.Trim();
            model.PTIPeriodNumber = (int)ptiPeriod[0].ExceptionDays;
            //string userId = ((List<MercPlusClient.ManageUserServiceReference.UserInfo>)System.Web.HttpContext.Current.Session["UserSec"])[0].UserId.ToString();
            model.PTIChangedUser = ptiPeriod[0].PTIChUser.Replace("|", " ");
            model.PTIUpdateDate = ptiPeriod[0].PTIChTime.ToString("yyyy-MM-dd hh:mm:ss tt");

            return View("PTIPeriodEdit", model);
        }

        public ActionResult GetPTIPeriod([Bind] ManageMasterDataPTIPeriodModel model)
        {
            ModelState.Clear();
            //ManageMasterDataPTIPeriodModel model = new ManageMasterDataPTIPeriodModel();
            List<PTIPeriod> ptiPeriod = mmdc.GetPTIPeriods(model.PTIPeriodFrom).ToList();
            model.FilterPTIPeriods = ptiPeriod;
            model.ShowGrid = true;
            model.MaxPTIPeriod = mmdc.GetPTIDefaultPeriod();
            if (ptiPeriod.Count < 1)
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("There were no results meeting your query parameters", "Warning");
            return View("PTIPeriod", model);
        }

        public ActionResult PTIPeriod()
        {
            ManageMasterDataPTIPeriodModel model = new ManageMasterDataPTIPeriodModel();
            //model.ShowGrid = true;
            model.FilterPTIPeriods = new List<PTIPeriod>();
            model.FilterPTIPeriods.Add(new PTIPeriod() { EqpNoFrom = "1001", EqpNoTo = "1001", ExceptionDays = 10 });
            return View(model);
        }

        public ActionResult DefaultPTIPeriodView()
        {
            ManageMasterDataPTIPeriodModel model = new ManageMasterDataPTIPeriodModel();
            //List<PTIPeriod> ptiPeriod = new List<PTIPeriod>();
            PTIPeriod period = mmdc.GetPTIDefaultPeriodRecord();
            // ptiPeriod.Add(period);
            model.EditDefaultPTIPeriod = true;
            model.MaxPTIPeriod = Convert.ToInt32(period.ExceptionDays);
            // model.PTIPeriodTo = ptiPeriod[0].EqpNoTo.Trim();
            //model.PTIPeriodNumber = (int)ptiPeriod[0].ExceptionDays;
            //string userId = ((List<MercPlusClient.ManageUserServiceReference.UserInfo>)System.Web.HttpContext.Current.Session["UserSec"])[0].UserId.ToString();
            model.PTIChangedUser = period.PTIChUser.Replace("|", " ");
            model.PTIUpdateDate = period.PTIChTime.ToString("yyyy-MM-dd hh:mm:ss tt");

            return View("PTIPeriodEdit", model);
        }

        public ActionResult ModifiyDefaultPTIPeriod([Bind] ManageMasterDataPTIPeriodModel model)
        {
            bool sucess = false;
            string msg = "";
            //PTIPeriod period = new PTIPeriod();
            //period.EqpNoFrom = PTIFrom;
            //period.EqpNoTo = PTITo;
            //period.ExceptionDays = manageMasterDataModel.PTIPeriodNumber;
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            //period.PTIChUser = userId;
            sucess = mmdc.ModifyPTIDefaultPeriod(model.MaxPTIPeriod, userId, ref msg);
            ManageMasterDataPTIPeriodModel manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
            manageMasterDataModel.PTIMessage = msg;
            if (sucess)
            {
                msg = manageMasterDataModel.PTIPeriodFrom + " / " + manageMasterDataModel.PTIPeriodTo;
                //manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
                ModelState.Clear();
                manageMasterDataModel.IsEditPTIPeriod = false;
                //manageMasterDataModel.PTIMessage = "Default PTI Period Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Default PTI Period Updated", "Success");
                return View("PTIPeriod", manageMasterDataModel);
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
                return RedirectToAction("DefaultPTIPeriodView");
            }
        }
        #endregion PTIPeriod

        #region Special Remarks
        public ActionResult SpecialRemarks()
        {
            ManageMasterDataSpecialRemarksModel ManageMasterDataModel = new ManageMasterDataSpecialRemarksModel();
            PopulateSpecialRemarksCombo(ManageMasterDataModel);
            ManageMasterDataModel.PageTitle = "Special Remarks View";
            ManageMasterDataModel.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            ManageMasterDataModel.IsCPH = ((UserSec)Session["UserSec"]).isCPH;
            return View(ManageMasterDataModel);
        }

        public ActionResult NewSpecialRemarks()
        {
            ManageMasterDataSpecialRemarksModel manageMasterDataModel = new ManageMasterDataSpecialRemarksModel();
            ModelState.Clear();
            PopulateSpecialRemarksCombo(manageMasterDataModel);
            manageMasterDataModel.PageTitle = "Special Remarks Add";
            manageMasterDataModel.IsInsertMode = true;
            manageMasterDataModel.ShowDetails = true;
            return View("SpecialRemarks", manageMasterDataModel);
        }

        public ActionResult NewSpecialRemarksFailed(ManageMasterDataSpecialRemarksModel manageMasterDataModel)
        {
            ModelState.Clear();
            PopulateSpecialRemarksCombo(manageMasterDataModel);
            manageMasterDataModel.PageTitle = "Special Remarks Add";
            manageMasterDataModel.IsInsertMode = true;
            manageMasterDataModel.ShowDetails = true;
            manageMasterDataModel.RKEMProfile = "";
            manageMasterDataModel.SerialNoFrom = "";
            manageMasterDataModel.SerialNoTo = "";
            return View("SpecialRemarks", manageMasterDataModel);
        }

        private void PopulateSpecialRemarksCombo(ManageMasterDataSpecialRemarksModel ManageMasterDataModel)
        {
            SpecialRemarks[][] profile = mmdc.GetSpecialRemarksComboValue();
            ManageMasterDataModel.drpProfile = new SelectList(profile[0], "RemarksID", "RKEMProfile");
            ManageMasterDataModel.drpRange = new SelectList(profile[1], "RemarksID", "RKEMProfile");
            List<SelectListItem> display = new List<SelectListItem>() { new SelectListItem() { Text = "", Value = "", Selected = true }, new SelectListItem() { Text = "N", Value = "N" }, new SelectListItem() { Text = "Y", Value = "Y" } };
            ManageMasterDataModel.drpDisplay = display;
        }

        public ActionResult GetSpecialRemarks([Bind] ManageMasterDataSpecialRemarksModel manageMasterDataModel)
        {
            string id = "";
            ModelState.Clear();
            PopulateSpecialRemarksCombo(manageMasterDataModel);
            manageMasterDataModel.PageTitle = "Special Remarks Edit";
            manageMasterDataModel.IsInsertMode = false;
            manageMasterDataModel.ShowDetails = true;
            manageMasterDataModel.IsProfileSelected = (manageMasterDataModel.RemarksID != null && manageMasterDataModel.RemarksID.Trim() != "") ? true : false;
            if (manageMasterDataModel.IsProfileSelected)
            {
                manageMasterDataModel.RemarksID1 = "";
                id = manageMasterDataModel.RemarksID;
            }
            else
            {
                manageMasterDataModel.RemarksID = "";
                id = manageMasterDataModel.RemarksID1;
            }
            SpecialRemarks rkm = mmdc.GetSpecialRemarks(Convert.ToInt32(id.Trim()));
            manageMasterDataModel.ChangeTime = rkm.ChangeTime.ToString("yyyy-MM-dd hh:mm:ss tt");
            manageMasterDataModel.ChangeUser = rkm.ChangeUser.Replace("|", " ");
            manageMasterDataModel.DisplaySW = rkm.DisplaySW;
            manageMasterDataModel.Remarks = rkm.Remarks;
            manageMasterDataModel.RepairCeiling = rkm.RepairCeiling.ToString();
            manageMasterDataModel.RKEMProfile = rkm.RKEMProfile;
            manageMasterDataModel.SerialNoFrom = rkm.SerialNoFrom;
            manageMasterDataModel.SerialNoTo = rkm.SerialNoTo;
            manageMasterDataModel.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            manageMasterDataModel.IsCPH = ((UserSec)Session["UserSec"]).isCPH;
            return View("SpecialRemarks", manageMasterDataModel);
        }

        public ActionResult InsertSpecialRemarks([Bind] ManageMasterDataSpecialRemarksModel manageMasterDataModel)
        {
            bool sucess = false;
            string msg = "";
            //string userId = ((List<MercPlusClient.ManageUserServiceReference.UserInfo>)System.Web.HttpContext.Current.Session["UserSec"])[0].UserId.ToString();
            SpecialRemarks sr = PrepareSpecialRemarksDataSet(manageMasterDataModel);
            //sr.RemarksID = Convert.ToInt32(manageMasterDataModel.RemarksID);
            //sr.ChangeUser = userId;
            //sr.DisplaySW = manageMasterDataModel.DisplaySW;
            //sr.Remarks = manageMasterDataModel.Remarks;
            //sr.RepairCeiling = Convert.ToDecimal(manageMasterDataModel.RepairCeiling);
            //sr.RKEMProfile = manageMasterDataModel.RKEMProfile;
            //sr.SerialNoFrom = manageMasterDataModel.SerialNoFrom;
            //sr.SerialNoTo = manageMasterDataModel.SerialNoTo;
            int id = 0;
            sucess = mmdc.InsertSpecialRemarks(sr, ref id, ref msg);
            manageMasterDataModel.RemarksMessages = msg;
            if (sucess)
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Special Remarks Added", "Success");
                if (manageMasterDataModel.RKEMProfile != null && manageMasterDataModel.RKEMProfile.Trim() != "")
                    manageMasterDataModel.RemarksID = id.ToString();
                else
                    manageMasterDataModel.RemarksID1 = id.ToString();
                return RedirectToAction("GetSpecialRemarks", manageMasterDataModel);
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
                return RedirectToAction("NewSpecialRemarksFailed", manageMasterDataModel);
            }
            //return NewSpecialRemarks();
        }

        public ActionResult ModifySpecialRemarksFailed([Bind] ManageMasterDataSpecialRemarksModel manageMasterDataModel)
        {
            string id = "";
            ModelState.Clear();
            PopulateSpecialRemarksCombo(manageMasterDataModel);
            manageMasterDataModel.PageTitle = "Special Remarks Edit";
            manageMasterDataModel.IsInsertMode = false;
            manageMasterDataModel.ShowDetails = true;
            manageMasterDataModel.IsProfileSelected = (manageMasterDataModel.RemarksID != null && manageMasterDataModel.RemarksID.Trim() != "") ? true : false;
            if (manageMasterDataModel.IsProfileSelected)
            {
                manageMasterDataModel.RemarksID1 = "";
                id = manageMasterDataModel.RemarksID;
            }
            else
            {
                manageMasterDataModel.RemarksID = "";
                id = manageMasterDataModel.RemarksID1;
            }
            return View("SpecialRemarks", manageMasterDataModel);
        }

        public ActionResult ModifySpecialRemarks([Bind] ManageMasterDataSpecialRemarksModel manageMasterDataModel)
        {
            bool sucess = false;
            string msg = "";
            //string userId = ((List<MercPlusClient.ManageUserServiceReference.UserInfo>)System.Web.HttpContext.Current.Session["UserSec"])[0].UserId.ToString();
            SpecialRemarks sr = PrepareSpecialRemarksDataSet(manageMasterDataModel);

            sucess = mmdc.ModifySpecialRemarks(sr, ref msg);

            if (sucess)
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Special Remarks Updated", "Success");
                return RedirectToAction("GetSpecialRemarks", manageMasterDataModel);
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
                //return View("PTIPeriod", manageMasterDataModel);
                return RedirectToAction("ModifySpecialRemarksFailed", manageMasterDataModel);
            }
        }

        public ActionResult SpecialRemarksAfterDelete([Bind] ManageMasterDataSpecialRemarksModel manageMasterDataModel)
        {
            string id = "";
            PopulateSpecialRemarksCombo(manageMasterDataModel);
            manageMasterDataModel.PageTitle = "Special Remarks Edit";
            manageMasterDataModel.IsInsertMode = false;
            manageMasterDataModel.ShowDetails = true;
            manageMasterDataModel.IsProfileSelected = (manageMasterDataModel.RemarksID != null && manageMasterDataModel.RemarksID.Trim() != "") ? true : false;
            if (manageMasterDataModel.IsProfileSelected)
            {
                manageMasterDataModel.RemarksID1 = "";
                id = manageMasterDataModel.RemarksID;
            }
            else
            {
                manageMasterDataModel.RemarksID = "";
                id = manageMasterDataModel.RemarksID1;
            }
            manageMasterDataModel.IsDeleted = true;
            return View("SpecialRemarks", manageMasterDataModel);
        }

        public ActionResult DeleteSpecialRemarks([Bind] ManageMasterDataSpecialRemarksModel manageMasterDataModel)
        {
            bool sucess = false;
            string msg = "";
            SpecialRemarks sr = PrepareSpecialRemarksDataSet(manageMasterDataModel);
            sucess = mmdc.DeleteSpecialRemarks(sr, ref msg);
            manageMasterDataModel.RemarksMessages = msg;
            if (sucess)
            {
                //msg = manageMasterDataModel.PTIPeriodFrom + " / " + manageMasterDataModel.PTIPeriodTo;
                //manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
                //ModelState.Clear();
                //manageMasterDataModel.IsEditPTIPeriod = false;
                //manageMasterDataModel.RemarksMessages = "Special Remarks is deleted";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Special Remarks Deleted", "Success");
                return RedirectToAction("SpecialRemarksAfterDelete", manageMasterDataModel);
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
                return RedirectToAction("ModifySpecialRemarksFailed", manageMasterDataModel);
            }
        }

        private SpecialRemarks PrepareSpecialRemarksDataSet(ManageMasterDataSpecialRemarksModel manageMasterDataModel)
        {
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            SpecialRemarks sr = new ManageMasterDataServiceReference.SpecialRemarks();
            sr.RemarksID = Convert.ToInt32(manageMasterDataModel.RemarksID == null ? manageMasterDataModel.RemarksID1 : manageMasterDataModel.RemarksID);
            sr.ChangeUser = userId;
            sr.DisplaySW = manageMasterDataModel.DisplaySW;
            sr.Remarks = manageMasterDataModel.Remarks;
            sr.RepairCeiling = Convert.ToDecimal(manageMasterDataModel.RepairCeiling);
            sr.RKEMProfile = manageMasterDataModel.RKEMProfile;
            sr.SerialNoFrom = manageMasterDataModel.SerialNoFrom;
            sr.SerialNoTo = manageMasterDataModel.SerialNoTo;
            return sr;
        }
        #endregion Special Remarks

        #region Repair STS Code
        private void PopulateRePairSTSCombo(ManageMasterDataRepairSTSCodeModel ManageMasterDataModel, bool PopulateIndex, bool PopulateRepairCode, bool PopulateRepairCodeEdit, bool IsAddMode)
        {
            List<Manual> manual;
            if (IsAddMode)
                manual = mmdc.GetRepairCodeActiveIndexManual().ToList();
            else
                manual = mmdc.GetRepairCodeManual().ToList();
            List<Mode> mode = new List<Mode>();
            if (IsAddMode)
            {
                if (ManageMasterDataModel.ManualCode != null)
                    mode = mmdc.GetRepairCodeIndexMode(ManageMasterDataModel.ManualCode).ToList();
                else
                    mode = mmdc.GetRepairCodeIndexMode(manual[0].ManualCode).ToList();
            }
            else
            {
                if (ManageMasterDataModel.ManualCode != null)
                    mode = mmdc.GetRepairCodeMode(ManageMasterDataModel.ManualCode).ToList();
                else
                    mode = mmdc.GetRepairCodeMode(manual[0].ManualCode).ToList();
            }
            ManageMasterDataModel.drpManual = new SelectList(manual, "ManualCode", "ManualDesc");
            ManageMasterDataModel.drpMode = new SelectList(mode, "ModeCode", "ModeDescription");
            //List<SelectListItem> display = new List<SelectListItem>() { new SelectListItem() { Text = "N", Value = "N" }, new SelectListItem() { Text = "Y", Value = "Y" } };
            //ManageMasterDataModel.drpYesNo = display;
            List<SelectListItem> display = new List<SelectListItem>() { new SelectListItem() { Text = "N", Value = "N" }, new SelectListItem() { Text = "Y", Value = "Y" }, new SelectListItem() { Text = "M", Value = "M" } };
            ManageMasterDataModel.drpYesNo = display;
            if (PopulateIndex)
            {
                List<Index> index = mmdc.GetIndexesForDropDown(ManageMasterDataModel.ManualCode, ManageMasterDataModel.ModeCode).ToList();
                ManageMasterDataModel.drpIndex = new SelectList(index, "IndexID", "IndexDesc");
            }
            else
            {
                List<SelectListItem> items = new List<SelectListItem>();
                SelectListItem item = new SelectListItem() { Text = "", Value = "" };
                items.Add(item);
                ManageMasterDataModel.drpIndex = items;
            }
            if (PopulateRepairCode)
            {
                List<RepairCode> repair = mmdc.GetRepairCodeByMode(ManageMasterDataModel.ManualCode, ManageMasterDataModel.ModeCode).ToList();
                repair = repair.OrderBy(p => p.RepairDesc).ToList();
                ManageMasterDataModel.drpRepairCode = new SelectList(repair, "RepairCod", "RepairDesc");
            }
            else
            {
                List<SelectListItem> items = new List<SelectListItem>();
                SelectListItem item = new SelectListItem() { Text = "", Value = "" };
                items.Add(item);
                ManageMasterDataModel.drpRepairCode = items;
            }
            if (PopulateRepairCodeEdit)
            {
                List<RepairCode> repair = mmdc.GetRepairCodeByMode(ManageMasterDataModel.ManualCode, ManageMasterDataModel.ModeCode).ToList();
                repair = repair.OrderBy(p => p.RepairDesc).ToList();
                ManageMasterDataModel.drpRepairCodeEdit = new SelectList(repair, "RepairCod", "RepairDesc");
            }
            else
            {
                List<SelectListItem> items = new List<SelectListItem>();
                SelectListItem item = new SelectListItem() { Text = "", Value = "" };
                items.Add(item);
                ManageMasterDataModel.drpRepairCodeEdit = items;
            }
        }

        public ActionResult RepairSTSCode()
        {
            ManageMasterDataRepairSTSCodeModel model = new ManageMasterDataRepairSTSCodeModel();
            ModelState.Clear();
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.RCManualCode;
            PopulateRePairSTSCombo(model, false, false, false, false);
            return View(model);
        }

        public ActionResult RepairSTSCode_View()
        {
            ManageMasterDataRepairSTSCodeModel model = new ManageMasterDataRepairSTSCodeModel();
            ModelState.Clear();
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.RCManualCode;
            PopulateRePairSTSCombo(model, false, false, false, false);
            return View(model);
        }

        public ActionResult RepairSTSCodeManual(string manualCode, bool isAddMode)
        {
            ManageMasterDataRepairSTSCodeModel model = new ManageMasterDataRepairSTSCodeModel();
            model.ManualCode = manualCode;
            ModelState.Clear();
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.RCManualCode;
            if (isAddMode)
                PopulateRePairSTSCombo(model, false, false, false, true);
            else
                PopulateRePairSTSCombo(model, false, false, false, false);
            model.IsAddMode = isAddMode;
            return View("RepairSTSCode", model);
        }

        public ActionResult RepairSTSCodeManual_View(string manualCode, bool isAddMode)
        {
            ManageMasterDataRepairSTSCodeModel model = new ManageMasterDataRepairSTSCodeModel();
            model.ManualCode = manualCode;
            ModelState.Clear();
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.RCManualCode;
            if (isAddMode)
                PopulateRePairSTSCombo(model, false, false, false, true);
            else
                PopulateRePairSTSCombo(model, false, false, false, false);
            model.IsAddMode = isAddMode;
            return View("RepairSTSCode_View", model);
        }

        public ActionResult RepairSTSCodeNew()
        {
            ManageMasterDataRepairSTSCodeModel model = new ManageMasterDataRepairSTSCodeModel();
            ModelState.Clear();
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.RCManualCode;
            model.IsAddMode = true;
            PopulateRePairSTSCombo(model, false, false, false, true);
            return View("RepairSTSCode", model);
        }

        public ActionResult RepairSTSCodeNewIndex([Bind] ManageMasterDataRepairSTSCodeModel model)
        {
            ModelState.Clear();
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.RepairCode;
            model.IsAddMode = true;
            PopulateRePairSTSCombo(model, true, false, false, true);
            return View("RepairSTSCode", model);
        }

        public ActionResult RepairSTSCodeNewRecord([Bind] ManageMasterDataRepairSTSCodeModel model)
        {
            ModelState.Clear();
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.Record;
            model.IsAddMode = true;
            PopulateRePairSTSCombo(model, true, false, true, true);
            model.MultipleUpdateSW = "N";
            model.TaxAppliedSW = "Y";
            model.RepairActiveSW = "Y";
            model.AllowPartsSW = "Y";
            model.ShopMaterialCeiling = "9999.00";
            return View("RepairSTSCode", model);
        }

        public ActionResult RepairSTSCodeGetIndex([Bind] ManageMasterDataRepairSTSCodeModel model)
        {
            ModelState.Clear();
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.RepairCode;
            model.IsAddMode = false;
            PopulateRePairSTSCombo(model, true, true, false, false);
            return View("RepairSTSCode", model);
        }

        public ActionResult RepairSTSCodeGetIndex_View([Bind] ManageMasterDataRepairSTSCodeModel model)
        {
            ModelState.Clear();
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.RepairCode;
            model.IsAddMode = false;
            PopulateRePairSTSCombo(model, true, true, false, false);
            return View("RepairSTSCode_View", model);
        }


        public ActionResult RepairSTSCodeGetIndexModeChange(string ManualCode, string ModeCode, bool IsAddMode)
        {
            ModelState.Clear();
            ManageMasterDataRepairSTSCodeModel model = new ManageMasterDataRepairSTSCodeModel();
            if (ModeCode == "")
                model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.RCManualCode;
            else
                model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.RepairCode;
            model.IsAddMode = IsAddMode;
            model.ManualCode = ManualCode;
            model.ModeCode = ModeCode;
            if (IsAddMode)
                PopulateRePairSTSCombo(model, true, true, false, true);
            else
                PopulateRePairSTSCombo(model, true, true, false, false);
            return View("RepairSTSCode", model);
        }

        public ActionResult RepairSTSCodeGetIndexModeChange_View(string ManualCode, string ModeCode, bool IsAddMode)
        {
            ModelState.Clear();
            ManageMasterDataRepairSTSCodeModel model = new ManageMasterDataRepairSTSCodeModel();
            if (ModeCode == "")
                model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.RCManualCode;
            else
                model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.RepairCode;
            model.IsAddMode = IsAddMode;
            model.ManualCode = ManualCode;
            model.ModeCode = ModeCode;
            if (IsAddMode)
                PopulateRePairSTSCombo(model, true, true, false, true);
            else
                PopulateRePairSTSCombo(model, true, true, false, false);
            return View("RepairSTSCode_View", model);
        }





        public ActionResult RepairSTSCodeAfterAddDelete([Bind] ManageMasterDataRepairSTSCodeModel model)
        {
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.Record;
            model.IsAddMode = false;
            PopulateRePairSTSCombo(model, true, false, false, false);
            model.IsAfterAddDeleteMode = true;
            model.ChangeUserFName = ((UserSec)Session["UserSec"]).UserFirstName;
            model.ChangeUserLName = ((UserSec)Session["UserSec"]).UserLastName;
            model.ChangeTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
            return View("RepairSTSCode", model);
        }

        public ActionResult RepairSTSCodeGetRecord(ManageMasterDataRepairSTSCodeModel model)
        {
            ModelState.Clear();
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.Record;
            model.IsAddMode = false;
            PopulateRePairSTSCombo(model, true, false, false, false);
            List<RepairCode> rc = new List<RepairCode>();
            if (model.IndexID > 0)
            {
                rc = mmdc.GetRepairCodeByIndex(model.ManualCode, model.ModeCode, Convert.ToInt32(model.IndexID)).ToList();
            }
            else if (model.RepairCode.Trim() != "")
            {
                rc = mmdc.GetRepairCodeByRepairCode(model.ManualCode, model.ModeCode, model.RepairCode).ToList();
            }
            if (rc.Count > 0)
            {
                Session.Add("RCSTSRecords", rc);
                model.AllowPartsSW = rc[0].AllowPartsSW;
                model.ChangeTime = rc[0].ChangeTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                model.ChangeUser = rc[0].ChangeUser;
                if (model.ChangeUser.Contains("|"))
                {
                    model.ChangeUserFName = model.ChangeUser.Split('|')[0];
                    model.ChangeUserLName = model.ChangeUser.Split('|')[1];
                }
                else
                {
                    model.ChangeUserFName = model.ChangeUser;
                    model.ChangeUserLName = model.ChangeUser;
                }
                model.IndexID = rc[0].IndexID;
                model.ManHour = rc[0].ManHour;
                model.MaxQuantity = rc[0].MaxQuantity;
                model.MultipleUpdateSW = rc[0].MultipleUpdateSW;
                model.PrepTime = rc[0].RepairInd;
                model.RepairActiveSW = rc[0].RepairActiveSW;
                model.RepairCode = rc[0].RepairCod;
                model.RepairDesc = rc[0].RepairDesc;
                model.RepairInd = rc[0].RepairInd;
                model.RepairPriority = rc[0].RepairPriority;
                model.RkrpRepairCode = rc[0].RkrpRepairCode;
                model.ShopMaterialCeiling = Convert.ToDecimal(rc[0].ShopMaterialCeiling).ToString("0.00");
                model.TaxAppliedSW = rc[0].TaxAppliedSW;
                model.WarrantyPeriod = rc[0].WarrantyPeriod;
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (RepairCode repair in rc)
                {
                    SelectListItem item = new SelectListItem() { Text = repair.RepairCod, Value = repair.RepairCod };
                    items.Add(item);
                }
                model.drpRepairCodeEdit = items;
            }
            return View("RepairSTSCode", model);
        }


        public ActionResult RepairSTSCodeGetRecord_View(ManageMasterDataRepairSTSCodeModel model)
        {
            ModelState.Clear();
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.Record;
            model.IsAddMode = false;
            PopulateRePairSTSCombo(model, true, false, false, false);
            List<RepairCode> rc = new List<RepairCode>();
            if (model.IndexID > 0)
            {
                rc = mmdc.GetRepairCodeByIndex(model.ManualCode, model.ModeCode, Convert.ToInt32(model.IndexID)).ToList();
            }
            else if (model.RepairCode.Trim() != "")
            {
                rc = mmdc.GetRepairCodeByRepairCode(model.ManualCode, model.ModeCode, model.RepairCode).ToList();
            }
            if (rc.Count > 0)
            {
                Session.Add("RCSTSRecords", rc);
                model.AllowPartsSW = rc[0].AllowPartsSW;
                model.ChangeTime = rc[0].ChangeTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                model.ChangeUser = rc[0].ChangeUser;
                if (model.ChangeUser.Contains("|"))
                {
                    model.ChangeUserFName = model.ChangeUser.Split('|')[0];
                    model.ChangeUserLName = model.ChangeUser.Split('|')[1];
                }
                else
                {
                    model.ChangeUserFName = model.ChangeUser;
                    model.ChangeUserLName = model.ChangeUser;
                }
                model.IndexID = rc[0].IndexID;
                model.ManHour = rc[0].ManHour;
                model.MaxQuantity = rc[0].MaxQuantity;
                model.MultipleUpdateSW = rc[0].MultipleUpdateSW;
                model.PrepTime = rc[0].RepairInd;
                model.RepairActiveSW = rc[0].RepairActiveSW;
                model.RepairCode = rc[0].RepairCod;
                model.RepairDesc = rc[0].RepairDesc;
                model.RepairInd = rc[0].RepairInd;
                model.RepairPriority = rc[0].RepairPriority;
                model.RkrpRepairCode = rc[0].RkrpRepairCode;
                model.ShopMaterialCeiling = Convert.ToDecimal(rc[0].ShopMaterialCeiling).ToString("0.00");
                model.TaxAppliedSW = rc[0].TaxAppliedSW;
                model.WarrantyPeriod = rc[0].WarrantyPeriod;
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (RepairCode repair in rc)
                {
                    SelectListItem item = new SelectListItem() { Text = repair.RepairCod, Value = repair.RepairCod };
                    items.Add(item);
                }
                model.drpRepairCodeEdit = items;
            }
            return View("RepairSTSCode_View", model);
        }

        public ActionResult RepairSTSCodeGetRecordByRepairCode(string Manual, string Mode, string RepairCode)
        {
            ModelState.Clear();
            ManageMasterDataRepairSTSCodeModel model = new ManageMasterDataRepairSTSCodeModel();
            model.ManualCode = Manual;
            model.ModeCode = Mode;
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.Record;
            model.IsAddMode = false;
            PopulateRePairSTSCombo(model, true, false, false, false);
            List<RepairCode> rcs = new List<RepairCode>();
            //if (model.IndexID > 0)
            //{
            //    rc = mmdc.GetRepairCodeByIndex(model.ManualCode, model.ModeCode, Convert.ToInt32(model.IndexID)).ToList();
            //}
            //else if (model.RepairCode.Trim() != "")
            //{
            //    rc = mmdc.GetRepairCodeByRepairCode(model.ManualCode, model.ModeCode, model.RepairCode).ToList();
            //}
            rcs = ((List<RepairCode>)Session["RCSTSRecords"]);
            if (rcs.Count > 0)
            {
                RepairCode rc = rcs.Find(Pid => Pid.RepairCod == RepairCode);
                return Json(rc);
                //model.AllowPartsSW = rc.AllowPartsSW;
                //model.ChangeTime = rc.ChangeTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                //model.ChangeUser = rc.ChangeUser;
                //if (model.ChangeUser.Contains("|"))
                //{
                //    model.ChangeUserFName = model.ChangeUser.Split('|')[0];
                //    model.ChangeUserLName = model.ChangeUser.Split('|')[1];
                //}
                //else
                //{
                //    model.ChangeUserFName = model.ChangeUser;
                //    model.ChangeUserLName = model.ChangeUser;
                //}
                //model.IndexID = rc.IndexID;
                //model.ManHour = rc.ManHour;
                //model.MaxQuantity = rc.MaxQuantity;
                //model.MultipleUpdateSW = rc.MultipleUpdateSW;
                ////model.PrepTime = rc[0].PrepTime;
                //model.RepairActiveSW = rc.RepairActiveSW;
                //model.RepairCode = rc.RepairCod;
                //model.RepairDesc = rc.RepairDesc;
                //model.RepairInd = rc.RepairInd;
                //model.RepairPriority = rc.RepairPriority;
                //model.RkrpRepairCode = rc.RkrpRepairCode;
                //model.ShopMaterialCeiling = rc.ShopMaterialCeiling;
                //model.TaxAppliedSW = rc.TaxAppliedSW;
                //model.WarrantyPeriod = rc.WarrantyPeriod;
                //List<SelectListItem> items = new List<SelectListItem>();
                //foreach (RepairCode repair in rcs)
                //{
                //    SelectListItem item = new SelectListItem() { Text = repair.RepairCod, Value = repair.RepairCod };
                //    items.Add(item);
                //}
                //model.drpRepairCodeEdit = items;
            }
            return View("RepairSTSCode", model);
        }

        public ActionResult InsertRepairSTSCode([Bind] ManageMasterDataRepairSTSCodeModel model)
        {
            bool sucess = false;
            string msg = "";
            //string userId = ((List<MercPlusClient.ManageUserServiceReference.UserInfo>)System.Web.HttpContext.Current.Session["UserSec"])[0].UserId.ToString();
            RepairCode rc = PrepareRepairSTSCodeDataSet(model);

            sucess = mmdc.InsertRepairCode(rc, ref msg);
            model.Msg = msg;
            if (sucess)
            {
                msg = "Repair Code " + model.RepairCode + " Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Success");
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
                return RedirectToAction("InsertRepairSTSCodeFailed", model);
            }
            return RedirectToAction("RepairSTSCodeAfterAddDelete", model);
        }

        public ActionResult ModifyRepairSTSCode([Bind] ManageMasterDataRepairSTSCodeModel model)
        {
            bool sucess = false;
            string msg = "";
            RepairCode rc = PrepareRepairSTSCodeDataSet(model);

            sucess = mmdc.ModifyRepairCode(rc, ref msg);
            model.Msg = msg;
            if (sucess)
            {
                msg = "Repair Code " + model.RepairCode + " Updated";
                //manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
                //ModelState.Clear();
                //manageMasterDataModel.IsEditPTIPeriod = false;
                //model.Msg = "Repair Code " + model.RepairCode + " Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Success");

            }
            else
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
            //return View("PTIPeriod", manageMasterDataModel);
            model.IndexID = 0;
            return RedirectToAction("RepairSTSCodeGetRecord", model);
        }

        public ActionResult DeleteRepairSTSCode([Bind] ManageMasterDataRepairSTSCodeModel model)
        {
            bool sucess = false;
            string msg = "";
            RepairCode rc = PrepareRepairSTSCodeDataSet(model);
            sucess = mmdc.DeleteRepairCode(rc, ref msg);
            model.Msg = msg;
            if (sucess)
            {
                msg = "Repair Code " + model.RepairCode + " Deleted";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Success");
            }
            else
            {
                msg = "Repair Code " + model.RepairCode + " Exists in Another Table - Not Deleted";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
            }

            return RedirectToAction("RepairSTSCodeAfterAddDelete", model);
        }

        private RepairCode PrepareRepairSTSCodeDataSet(ManageMasterDataRepairSTSCodeModel model)
        {
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            RepairCode rCode = new RepairCode();
            rCode.AllowPartsSW = model.AllowPartsSW;
            rCode.ChangeUser = userId;
            rCode.IndexID = model.IndexID;
            rCode.ManHour = model.ManHour;
            rCode.ManualCode = model.ManualCode;
            rCode.MaxQuantity = model.MaxQuantity;
            rCode.ManualCode = model.ManualCode;
            rCode.ModeCode = model.ModeCode;
            rCode.MaxQuantity = model.MaxQuantity;
            rCode.ModeCode = model.ModeCode;
            rCode.MultipleUpdateSW = model.MultipleUpdateSW;
            rCode.RepairActiveSW = model.RepairActiveSW;
            rCode.RepairCod = model.RepairCode;
            rCode.RepairDesc = model.RepairDesc;
            rCode.RepairInd = model.RepairInd;
            rCode.RepairPriority = model.RepairPriority;
            rCode.RkrpRepairCode = model.RkrpRepairCode;
            rCode.ShopMaterialCeiling = Convert.ToDecimal(model.ShopMaterialCeiling);
            rCode.TaxAppliedSW = model.TaxAppliedSW;
            rCode.WarrantyPeriod = model.WarrantyPeriod;
            return rCode;
        }

        public ActionResult InsertRepairSTSCodeFailed(ManageMasterDataRepairSTSCodeModel model)
        {
            //ModelState.Clear();
            model.RepairCodeSelectionMode = ManageMasterDataRepairSTSCodeModel.RepairCodeSelectionModes.Record;
            model.IsAddMode = true;
            PopulateRePairSTSCombo(model, true, false, true, true);
            //model.MultipleUpdateSW = "N";
            //model.TaxAppliedSW = "Y";
            //model.RepairActiveSW = "Y";
            //model.AllowPartsSW = "Y";
            //model.ShopMaterialCeiling = "9999.00";
            return View("RepairSTSCode", model);
        }
        #endregion Repair STS Code

        #region ManualMode
        private void PopulateManualModeCombos(ManageMasterDataManualModeModel ManageMasterDataModel, bool PopulateMode, bool PopulateManual, bool IsAddMode, string ManualCode)
        {
            if (PopulateManual)
            {
                List<Manual> manual = new List<Manual>();
                if (IsAddMode)
                    manual = mmdc.GetAllManual().ToList();
                else
                    manual = mmdc.GetManualModeManual().ToList();
                ManageMasterDataModel.drpManual = new SelectList(manual, "ManualCode", "ManualDesc");
            }
            else
            {
                List<SelectListItem> items = new List<SelectListItem>();
                SelectListItem item = new SelectListItem() { Text = "", Value = "" };
                items.Add(item);
                ManageMasterDataModel.drpManual = items;
            }
            if (PopulateMode)
            {
                List<Mode> mode = new List<Mode>();
                if (IsAddMode)
                    mode = mmdc.GetAllMode().ToList();
                else
                    mode = mmdc.GetManualModeMode(ManualCode).ToList();
                ManageMasterDataModel.drpMode = new SelectList(mode, "ModeCode", "ModeDescription");
            }
            else
            {
                List<SelectListItem> items = new List<SelectListItem>();
                SelectListItem item = new SelectListItem() { Text = "", Value = "" };
                items.Add(item);
                ManageMasterDataModel.drpMode = items;
            }
            List<SelectListItem> display = new List<SelectListItem>() { new SelectListItem() { Text = "", Value = "", Selected = true }, new SelectListItem() { Text = "N", Value = "N" }, new SelectListItem() { Text = "Y", Value = "Y" } };
            ManageMasterDataModel.drpYesNo = display;
        }

        public ActionResult ManualMode()
        {
            ManageMasterDataManualModeModel model = new ManageMasterDataManualModeModel();
            model.IsAddMode = false;
            model.IsShowManual = true;
            model.IsShowMode = false;
            PopulateManualModeCombos(model, false, true, false, "");
            return View(model);
        }

        public ActionResult ManualModeNew()
        {
            ManageMasterDataManualModeModel model = new ManageMasterDataManualModeModel();
            model.IsAddMode = true;
            model.IsShowManual = false;
            model.IsShowMode = false;
            PopulateManualModeCombos(model, true, true, true, "");
            return View("ManualMode", model);
        }

        public ActionResult ManulModeAfterNew(ManageMasterDataManualModeModel model)
        {
            model.IsAddMode = true;
            model.IsShowManual = false;
            model.IsShowMode = false;
            if (!model.IsAddfailed)
                model.IsAdded = true;
            PopulateManualModeCombos(model, true, true, true, "");
            List<ManualMode> mms = mmdc.GetManualMode(model.ManualCode, model.ModeCode).ToList();
            model.ActiveSw = mms[0].ActiveSw;
            model.ChangeTime = mms[0].ChangeTime.ToString("yyyy-MM-dd hh:mm:ss tt");
            model.ChangeUser = mms[0].ChangeUser;
            model.ChangeFUser = ((UserSec)Session["UserSec"]).UserFirstName;
            model.ChangeLUser = ((UserSec)Session["UserSec"]).UserLastName;
            return View("ManualMode", model);
        }

        public ActionResult InsertManualMode([Bind] ManageMasterDataManualModeModel model)
        {
            bool sucess = false;
            string msg = "";
            //string userId = ((List<MercPlusClient.ManageUserServiceReference.UserInfo>)System.Web.HttpContext.Current.Session["UserSec"])[0].UserId.ToString();
            ManualMode mm = PrepareManualModeDataSet(model);
            try
            {
                sucess = mmdc.InsertManualMode(mm, ref msg);
            }
            catch (Exception ex)
            {
                msg = "Unable to add Manual Mode";
            }
            model.Msg = msg;
            if (sucess)
            {
                msg = model.ManualCode + " / " + model.ModeCode;
                //manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
                ModelState.Clear();
                //manageMasterDataModel.IsEditPTIPeriod = false;
                model.Msg = "Manual Mode " + msg + " Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(model.Msg, "Success");
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(model.Msg, "Warning");
                model.IsAddfailed = true;
            }
            //return View("ManualMode", model);
            return RedirectToAction("ManulModeAfterNew", model);
        }

        public ActionResult ModifyManualMode([Bind] ManageMasterDataManualModeModel model)
        {
            bool sucess = false;
            string msg = "";
            ManualMode mm = PrepareManualModeDataSet(model);
            try
            {
                sucess = mmdc.ModifyManualMode(mm, ref msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            model.Msg = msg;
            if (sucess)
            {
                msg = model.ManualCode + " / " + model.ModeCode;
                //manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
                ModelState.Clear();
                //manageMasterDataModel.IsEditPTIPeriod = false;
                model.Msg = "Manual Mode " + msg + " Updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(model.Msg, "Success");
            }
            else
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(model.Msg, "Warning");
            //return View("ManualMode", model);
            return RedirectToAction("ManualModeGetMaualMode", model);
        }
        //public ActionResult DeleteManualMode([Bind] ManageMasterDataManualModeModel model)
        //{
        //    bool sucess = false;
        //    string msg = "";
        //    ManualMode mm = PrepareManualModeDataSet(model);
        //    sucess = mmdc.DeleteManualMode(mm, ref msg);
        //    model.Msg = msg;
        //    if (sucess)
        //    {
        //        //msg = manageMasterDataModel.PTIPeriodFrom + " / " + manageMasterDataModel.PTIPeriodTo;
        //        //manageMasterDataModel = new ManageMasterDataPTIPeriodModel();
        //        ModelState.Clear();
        //        //manageMasterDataModel.IsEditPTIPeriod = false;
        //        model.Msg = "Manual Mode deleted";
        //        return SpecialRemarks();
        //    }
        //    return View();
        //}

        public ActionResult ManualModeGetMode([Bind] ManageMasterDataManualModeModel model)
        {
            model.IsAddMode = false;
            model.IsShowManual = false;
            model.IsShowMode = true;
            PopulateManualModeCombos(model, true, false, false, model.ManualCode);
            return View("ManualMode", model);
        }

        public ActionResult ManualModeGetMaualMode([Bind] ManageMasterDataManualModeModel model)
        {
            ModelState.Clear();
            model.IsAddMode = false;
            model.IsShowManual = false;
            model.IsShowMode = false;
            PopulateManualModeCombos(model, false, false, false, "");
            List<ManualMode> mms = mmdc.GetManualMode(model.ManualCode, model.ModeCode).ToList();
            model.ActiveSw = mms[0].ActiveSw;
            model.ChangeTime = mms[0].ChangeTime.ToString("yyyy-MM-dd hh:mm:ss tt");
            string userName = mms[0].ChangeUser;
            if (userName != null)
            {
                if (userName.Contains('|'))
                {
                    model.ChangeFUser = userName.Split('|')[0];
                    model.ChangeLUser = userName.Split('|')[1];
                }
                else
                {
                    model.ChangeFUser = userName;
                    model.ChangeLUser = "";
                }
            }
            else
            {
                model.ChangeFUser = "";
                model.ChangeLUser = "";
            }
            model.ChangeUser = mms[0].ChangeUser;
            return View("ManualMode", model);
        }

        private ManualMode PrepareManualModeDataSet(ManageMasterDataManualModeModel model)
        {
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            ManualMode mm = new ManualMode();
            mm.ActiveSw = model.ActiveSw;
            mm.ChangeUser = userId;
            mm.ManualCode = model.ManualCode;
            mm.ModeCode = model.ModeCode;

            return mm;
        }
        #endregion ManualMode

        #region Mode
        public ActionResult EditModeAdmin(List<Mode> modeList = null)
        {
            ManageMasterDataModeModel ManageMasterDataModel = new ManageMasterDataModeModel();
            ManageMasterDataModel.IsModeUpdate = true;
            modeList = mmdc.GetAllActiveInActiveModes().ToList();
            List<SelectListItem> ModeList = new List<SelectListItem>();
            List<SelectListItem> ModeActiveSwitchList = new List<SelectListItem>();
            List<SelectListItem> ModeIndicatorList = new List<SelectListItem>();
            int count = 1;
            //Populating the Mode fromthe list of Mode we got from db
            foreach (var code in modeList)
            {
                ModeList.Add(new SelectListItem
                {
                    Text = code.ModeFullDescription,
                    Value = code.ModeCode
                });
                count++;
            }

            //popuplating the active switch with hard code here since that's how its done currently.
            ModeActiveSwitchList = PopulateModeActiveSwitchDropDown(ModeActiveSwitchList, string.Empty);
            ModeIndicatorList = PopulateModeIndicatorDropDown(ModeIndicatorList, string.Empty);
            //Assigning the list of selecteditems to the model 
            ManageMasterDataModel.drpMode = ModeList;

            ManageMasterDataModel.drpModeActiveSwitch = ModeActiveSwitchList;
            ManageMasterDataModel.drpModeIndicator = ModeIndicatorList;
            ViewData["ModeList"] = ModeList;
            return View("EditMode", ManageMasterDataModel);
        }


        public ActionResult EditModeAdmin_View(List<Mode> modeList = null) // new requirement Leo Usermapping_Debadrira
        {
            ManageMasterDataModeModel ManageMasterDataModel = new ManageMasterDataModeModel();
            ManageMasterDataModel.IsModeUpdate = true;
            modeList = mmdc.GetAllActiveInActiveModes().ToList();
            List<SelectListItem> ModeList = new List<SelectListItem>();
            List<SelectListItem> ModeActiveSwitchList = new List<SelectListItem>();
            List<SelectListItem> ModeIndicatorList = new List<SelectListItem>();
            int count = 1;
            //Populating the Mode fromthe list of Mode we got from db
            foreach (var code in modeList)
            {
                ModeList.Add(new SelectListItem
                {
                    Text = code.ModeFullDescription,
                    Value = code.ModeCode
                });
                count++;
            }

            //popuplating the active switch with hard code here since that's how its done currently.
            ModeActiveSwitchList = PopulateModeActiveSwitchDropDown(ModeActiveSwitchList, string.Empty);
            ModeIndicatorList = PopulateModeIndicatorDropDown(ModeIndicatorList, string.Empty);
            //Assigning the list of selecteditems to the model 
            ManageMasterDataModel.drpMode = ModeList;

            ManageMasterDataModel.drpModeActiveSwitch = ModeActiveSwitchList;
            ManageMasterDataModel.drpModeIndicator = ModeIndicatorList;
            ViewData["ModeList"] = ModeList;
            return View("EditMode_View", ManageMasterDataModel);
        }

        [HttpPost]
        public ActionResult UpdateMode([Bind] ManageMasterDataModeModel manageMasterDataModel)
        {
            string msg = "";
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            ModelState.Clear();
            manageMasterDataModel.IsModeUpdate = true;
            Mode ModeListToBeUpdated = new Mode();
            ModeListToBeUpdated.ModeCode = manageMasterDataModel.ModeFullDescription;
            ModeListToBeUpdated.ModeDescription = manageMasterDataModel.ModeDescription;
            ModeListToBeUpdated.ModeActiveSW = manageMasterDataModel.ModeActiveSwitch;
            ModeListToBeUpdated.ModeInd = manageMasterDataModel.ModeInd;
            ModeListToBeUpdated.ChangeUser = userId;
            ModeListToBeUpdated.ChTime = DateTime.Now;
            //Call the model method to send for update
            bool sucess = mmdc.UpdateMode(ModeListToBeUpdated, ref msg);
            if (sucess)
            {
                msg = "Mode " + manageMasterDataModel.ModeFullDescription + " updated";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Success");
                manageMasterDataModel.ModeChangeTime = ModeListToBeUpdated.ChTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                manageMasterDataModel.ModeChangeUserFName = ((UserSec)Session["UserSec"]).UserFirstName;
                manageMasterDataModel.ModeChangeUserLName = ((UserSec)Session["UserSec"]).UserLastName;
            }
            else
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
            List<Mode> modeList = mmdc.GetAllActiveInActiveModes().ToList();
            List<SelectListItem> ModeList = new List<SelectListItem>();
            foreach (var code in modeList)
            {
                ModeList.Add(new SelectListItem
                {
                    Text = code.ModeFullDescription,
                    Value = code.ModeCode
                });
            }
            List<SelectListItem> ModeActiveSwitchList = new List<SelectListItem>();
            List<SelectListItem> ModeIndicatorList = new List<SelectListItem>();
            ModeActiveSwitchList = PopulateModeActiveSwitchDropDown(ModeActiveSwitchList, string.Empty);
            ModeIndicatorList = PopulateModeIndicatorDropDown(ModeIndicatorList, string.Empty);
            manageMasterDataModel.drpModeActiveSwitch = ModeActiveSwitchList;
            manageMasterDataModel.drpModeIndicator = ModeIndicatorList;
            manageMasterDataModel.drpMode = ModeList;
            manageMasterDataModel.IsModeUpdate = true;
            return View("EditMode", manageMasterDataModel);
        }

        //[HttpPost]
        public ActionResult ModeCreate()
        {
            ManageMasterDataModeModel ManageMasterDataModel = new ManageMasterDataModeModel();
            ManageMasterDataModel.IsModeUpdate = false;
            List<SelectListItem> ModeActiveSwitchList = new List<SelectListItem>();
            ModeActiveSwitchList = PopulateModeActiveSwitchDropDown(ModeActiveSwitchList, string.Empty);
            ManageMasterDataModel.drpModeActiveSwitch = ModeActiveSwitchList;
            List<SelectListItem> ModeIndList = new List<SelectListItem>();
            ModeIndList = PopulateModeIndicatorDropDown(ModeIndList, string.Empty);
            ManageMasterDataModel.drpModeIndicator = ModeIndList;
            return View("EditMode", ManageMasterDataModel);
        }
        public ActionResult ModeCreateError([Bind] ManageMasterDataModeModel ManageMasterDataModel)
        {
            // ManageMasterDataModeModel ManageMasterDataModel = new ManageMasterDataModeModel();
            ManageMasterDataModel.IsModeUpdate = false;
            List<SelectListItem> ModeActiveSwitchList = new List<SelectListItem>();
            ModeActiveSwitchList = PopulateModeActiveSwitchDropDown(ModeActiveSwitchList, string.Empty);
            ManageMasterDataModel.drpModeActiveSwitch = ModeActiveSwitchList;
            List<SelectListItem> ModeIndList = new List<SelectListItem>();
            ModeIndList = PopulateModeIndicatorDropDown(ModeIndList, string.Empty);
            ManageMasterDataModel.drpModeIndicator = ModeIndList;
            return View("EditMode", ManageMasterDataModel);
        }
        [HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "Add")]
        public ActionResult CreateMode([Bind] ManageMasterDataModeModel ManageMasterDataModel)
        {
            bool success = false;
            string msg = "";
            ManageMasterDataModel.IsModeUpdate = false;
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            Mode ModeToBeCreated = new Mode();
            ModeToBeCreated.ModeCode = ManageMasterDataModel.txtMode;
            ModeToBeCreated.ModeDescription = ManageMasterDataModel.ModeDescription;
            ModeToBeCreated.ModeActiveSW = ManageMasterDataModel.ModeActiveSwitch;
            ModeToBeCreated.ModeInd = ManageMasterDataModel.ModeInd;
            ModeToBeCreated.ChTime = DateTime.Now;
            ModeToBeCreated.ChangeUser = userId;
            success = mmdc.CreateMode(ModeToBeCreated, ref msg);
            if (success)
            {
                msg = "Mode " + ManageMasterDataModel.txtMode + " Added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Success");
                ManageMasterDataModel.ModeChangeTime = ModeToBeCreated.ChTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                return RedirectToAction("ModeAfterAdd", ManageMasterDataModel);
            }
            else
            {
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
                return RedirectToAction("ModeCreateError", ManageMasterDataModel);
            }

        }

        public ActionResult ModeAfterAdd(ManageMasterDataModeModel model)
        {
            model.IsModeAdded = true;
            model.IsModeUpdate = false;
            model.ModeChangeUserFName = ((UserSec)Session["UserSec"]).UserFirstName;
            model.ModeChangeUserLName = ((UserSec)Session["UserSec"]).UserLastName;
            List<SelectListItem> ModeActiveSwitchList = new List<SelectListItem>();
            List<SelectListItem> ModeIndicatorList = new List<SelectListItem>();
            ModeActiveSwitchList = PopulateModeActiveSwitchDropDown(ModeActiveSwitchList, string.Empty);
            ModeIndicatorList = PopulateModeIndicatorDropDown(ModeIndicatorList, string.Empty);
            model.drpModeActiveSwitch = ModeActiveSwitchList;
            model.drpModeIndicator = ModeIndicatorList;
            return View("EditMode", model);
        }

        private List<SelectListItem> PopulateModeActiveSwitchDropDown(List<SelectListItem> ModeActiveSWitchList, string ActiveSwitchstring)
        {


            ModeActiveSWitchList.Add(new SelectListItem
            {
                Text = "Y",
                Value = "Y"
            });
            ModeActiveSWitchList.Add(new SelectListItem
            {
                Text = "N",
                Value = "N"
            });



            return ModeActiveSWitchList;
        }
        private List<SelectListItem> PopulateModeIndicatorDropDown(List<SelectListItem> ModeIndicatorList, string ModeIndicatorstring)
        {


            ModeIndicatorList.Add(new SelectListItem
            {
                Text = "Standard",
                Value = "0"
            });
            ModeIndicatorList.Add(new SelectListItem
            {
                Text = "Offhire",
                Value = "1"
            });



            return ModeIndicatorList;
        }
        [HttpPost]
        public ActionResult GetAllDetailsForMode(string id)
        {
            var request = HttpContext.Request;

            ManageMasterDataModel model = new ManageMasterDataModel();

            List<Mode> ModeCodelist = mmdc.GetMode(id).ToList();
            Mode data = ModeCodelist.Find(Pid => Pid.ModeCode == id);
            List<SelectListItem> drp = new List<SelectListItem>();

            return Json(data);

        }

        #endregion mode

        #region Manual
        public ActionResult EditManual(List<Manual> manualList = null)
        {
            ManageMasterDataManualModel ManageMasterDataModel = new ManageMasterDataManualModel();
            ManageMasterDataModel.IsManualUpdate = true;
            manualList = mmdc.GetManual().ToList();
            List<SelectListItem> ManualList = new List<SelectListItem>();
            List<SelectListItem> ManualActiveSwitchList = new List<SelectListItem>();

            int count = 1;
            //Populating the Mode fromthe list of Mode we got from db
            foreach (var code in manualList)
            {
                ManualList.Add(new SelectListItem
                {
                    Text = code.ManualCode + " - " + code.ManualDesc,
                    Value = code.ManualCode
                });
                count++;
            }

            //popuplating the active switch with hard code here since that's how its done currently.
            ManualActiveSwitchList = PopulateManualActiveSwitchDropDown(ManualActiveSwitchList, string.Empty);

            //Assigning the list of selecteditems to the model 
            ManageMasterDataModel.drpManual = ManualList;

            ManageMasterDataModel.drpManualActiveSwitch = ManualActiveSwitchList;

            ViewData["ManualList"] = ManualList;
            return View("EditManual", ManageMasterDataModel);
        }
        [HttpPost]
        public ActionResult UpdateManualDescription([Bind] ManageMasterDataManualModel manageMasterDataModel)
        {
            ModelState.Clear();
            //ManageMasterDataManualModel ManageMasterDataModel = new ManageMasterDataManualModel();
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            manageMasterDataModel.IsManualUpdate = true;
            Manual ManualDescriptionToBeUpdated = new Manual();
            ManualDescriptionToBeUpdated.ManualDesc = manageMasterDataModel.ManualDesc;
            ManualDescriptionToBeUpdated.ManualCode = manageMasterDataModel.ManualFullDesc;
            ManualDescriptionToBeUpdated.ChangeUser = userId;
            ManualDescriptionToBeUpdated.ChTime = DateTime.Now;
            string message = "";
            //Call the model method to send for update
            bool success = mmdc.UpdateManualDescription(ManualDescriptionToBeUpdated, ref message); ;
            if (success)
            {
                message = "Manual " + manageMasterDataModel.ManualFullDesc + " updated.";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(message, "Success");
                manageMasterDataModel.ManualChTime = ManualDescriptionToBeUpdated.ChTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                manageMasterDataModel.ManualChangeUserFName = ((UserSec)Session["UserSec"]).UserFirstName;
                manageMasterDataModel.ManualChangeUserLName = ((UserSec)Session["UserSec"]).UserLastName;
            }
            else
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(message, "Warning");
            List<Manual> manualList = mmdc.GetManual().ToList();
            List<SelectListItem> ManualList = new List<SelectListItem>();
            List<SelectListItem> ManualActiveSwitchList = new List<SelectListItem>();

            //Populating the Mode fromthe list of Mode we got from db
            foreach (var code in manualList)
            {
                ManualList.Add(new SelectListItem
                {
                    Text = code.ManualCode + " - " + code.ManualDesc,
                    Value = code.ManualCode
                });
            }

            //popuplating the active switch with hard code here since that's how its done currently.
            ManualActiveSwitchList = PopulateManualActiveSwitchDropDown(ManualActiveSwitchList, string.Empty);
            manageMasterDataModel.drpManual = ManualList;
            manageMasterDataModel.drpManualActiveSwitch = ManualActiveSwitchList;
            manageMasterDataModel.IsManualUpdate = true;
            //ModelState.Clear();

            return View("EditManual", manageMasterDataModel);
        }
        [HttpPost]
        public ActionResult UpdateManualActiveSwitch([Bind] ManageMasterDataManualModel manageMasterDataModel)
        {
            ModelState.Clear();
            //ManageMasterDataManualModel ManageMasterDataModel = new ManageMasterDataManualModel();
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            manageMasterDataModel.IsManualUpdate = true;
            Manual ManualActiveSwitchToBeUpdated = new Manual();
            ManualActiveSwitchToBeUpdated.ManualActiveSW = manageMasterDataModel.ManualActiveSW;
            ManualActiveSwitchToBeUpdated.ManualCode = manageMasterDataModel.ManualFullDesc;
            ManualActiveSwitchToBeUpdated.ChangeUser = userId;
            ManualActiveSwitchToBeUpdated.ChTime = DateTime.Now;
            string message = "";
            //Call the model method to send for update
            bool success = mmdc.UpdateManualActiveSwitch(ManualActiveSwitchToBeUpdated, ref message); ;
            if (success)
            {
                message = "Manual " + manageMasterDataModel.ManualFullDesc + " updated.";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(message, "Success");
                manageMasterDataModel.ManualChTime = ManualActiveSwitchToBeUpdated.ChTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                manageMasterDataModel.ManualChangeUserFName = ((UserSec)Session["UserSec"]).UserFirstName;
                manageMasterDataModel.ManualChangeUserLName = ((UserSec)Session["UserSec"]).UserLastName;
            }
            else
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(message, "Warning");

            List<Manual> manualList = mmdc.GetManual().ToList();
            List<SelectListItem> ManualList = new List<SelectListItem>();
            List<SelectListItem> ManualActiveSwitchList = new List<SelectListItem>();

            //Populating the Mode fromthe list of Mode we got from db
            foreach (var code in manualList)
            {
                ManualList.Add(new SelectListItem
                {
                    Text = code.ManualCode + " - " + code.ManualDesc,
                    Value = code.ManualCode
                });
            }

            //popuplating the active switch with hard code here since that's how its done currently.
            ManualActiveSwitchList = PopulateManualActiveSwitchDropDown(ManualActiveSwitchList, string.Empty);
            manageMasterDataModel.drpManual = ManualList;
            manageMasterDataModel.drpManualActiveSwitch = ManualActiveSwitchList;
            //ModelState.Clear();
            return View("EditManual", manageMasterDataModel);
        }
        [HttpPost]
        public ActionResult ManualCreate()
        {
            ManageMasterDataManualModel ManageMasterDataModel = new ManageMasterDataManualModel();
            ManageMasterDataModel.IsManualUpdate = false;
            List<SelectListItem> ManualActiveSwitchList = new List<SelectListItem>();
            ManualActiveSwitchList = PopulateManualActiveSwitchDropDown(ManualActiveSwitchList, string.Empty);
            ManageMasterDataModel.drpManualActiveSwitch = ManualActiveSwitchList;


            return View("EditManual", ManageMasterDataModel);
        }
        [HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "Add")]
        public ActionResult CreateManual([Bind] ManageMasterDataManualModel ManageMasterDataModel)
        {
            bool success = false;
            ManageMasterDataModel.IsManualUpdate = false;
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            Manual ManualToBeCreated = new Manual();
            ManualToBeCreated.ManualCode = ManageMasterDataModel.txtManual;
            ManualToBeCreated.ManualDesc = ManageMasterDataModel.ManualDesc;
            ManualToBeCreated.ManualActiveSW = ManageMasterDataModel.ManualActiveSW;
            ManualToBeCreated.ChangeUser = userId;
            ManualToBeCreated.ChTime = DateTime.Now;
            string message = "";
            success = mmdc.CreateManual(ManualToBeCreated, ref message);
            if (success)
            {
                message = "Manual " + ManageMasterDataModel.txtManual + " added.";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(message, "Success");
            }
            else
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(message, "Warning");
            List<SelectListItem> ManualActiveSwitchList = new List<SelectListItem>();
            ManualActiveSwitchList = PopulateManualActiveSwitchDropDown(ManualActiveSwitchList, string.Empty);
            ManageMasterDataModel.drpManualActiveSwitch = ManualActiveSwitchList;
            ModelState.Clear();
            return View("EditManual", ManageMasterDataModel);
        }
        private List<SelectListItem> PopulateManualActiveSwitchDropDown(List<SelectListItem> ManualActiveSWitchList, string ActiveSwitchstring)
        {


            ManualActiveSWitchList.Add(new SelectListItem
            {
                Text = "Y",
                Value = "Y"
            });
            ManualActiveSWitchList.Add(new SelectListItem
            {
                Text = "N",
                Value = "N"
            });



            return ManualActiveSWitchList;
        }
        //[HttpPost]
        public ActionResult GetAllDetailsForManual(string id)
        {
            //var request = HttpContext.Request;

            ManageMasterDataModel model = new ManageMasterDataModel();

            List<Manual> ManualCodelist = mmdc.GetSingleManual(id).ToList();
            Manual data = ManualCodelist.Find(Pid => Pid.ManualCode == id);
            //List<SelectListItem> drp = new List<SelectListItem>();

            return Json(data);

        }
        #endregion Manual

        #region DamageCode
        //[HttpPost]
        public ActionResult DamageCodeCreate()
        {
            ManageMasterDataDamageCodeModel ManageMasterDataModel = new ManageMasterDataDamageCodeModel();
            ManageMasterDataModel.IsDamgeUpdate = false;
            List<Damage> DamageCodeList = mmdc.GetDamageCodes().ToList();
            ViewBag.DamageFullDescription = new SelectList(DamageCodeList, "DamageCedexCode", "DamageFullDescription");
            ManageMasterDataModel.DamageCHTS = "N/A";
            ManageMasterDataModel.DamageCHUser = "N/A";
            ManageMasterDataModel.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            ManageMasterDataModel.IsAddMode = true;
            return View("EditDamageCode", ManageMasterDataModel);
        }

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "Submit")]
        public ActionResult CreateDamageCode([Bind] ManageMasterDataDamageCodeModel ManageMasterDataModel)
        {
            bool success = false;
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            ManageMasterDataModel.IsDamgeUpdate = false;
            Damage DamageToBeCreated = new Damage();
            DamageToBeCreated.DamageCedexCode = ManageMasterDataModel.DamageCedexCode;
            DamageToBeCreated.DamageName = ManageMasterDataModel.DamageName;
            DamageToBeCreated.DamageDescription = ManageMasterDataModel.DamageDescription;
            DamageToBeCreated.DamageNumericalCode = ManageMasterDataModel.DamageNumericalCode;
            DamageToBeCreated.ChangeUser = userId;//ManageMasterDataModel.DamageCHUser;
            DamageToBeCreated.ChangeTime = DateTime.Now; //Convert.ToDateTime(ManageMasterDataModel.DamageCHTS);
            string message = "";
            try
            {
                success = mmdc.CreateDamageCode(DamageToBeCreated, ref message);
            }
            catch (Exception ex)
            {
                message = "Unable to add Damage Code. Error was " + ex.Message;
            }
            List<Damage> DamageCodeList = mmdc.GetDamageCodes().ToList();
            ViewBag.DamageFullDescription = new SelectList(DamageCodeList, "DamageCedexCode", "DamageFullDescription");

            ManageMasterDataModel.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;

            if (success)
            {
                message = "Damage Code " + ManageMasterDataModel.DamageCedexCode + " added";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(message, "Success");
                ManageMasterDataModel.IsDamgeUpdate = true;
                ManageMasterDataModel.DamageCode = ManageMasterDataModel.DamageCedexCode;
                return RedirectToAction("GetAllDetailsForDamageCode", ManageMasterDataModel);
            }
            else
            {
                //message = "Unable to add";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(message, "Warning");
                ManageMasterDataModel.DamageCHTS = "N/A";
                ManageMasterDataModel.DamageCHUser = "N/A";
                ManageMasterDataModel.IsAddMode = true;
                return View("EditDamageCode", ManageMasterDataModel);
            }

        }

        public ActionResult EditDamageCode()
        {
            ManageMasterDataDamageCodeModel ManageMasterDataModel = new ManageMasterDataDamageCodeModel();
            ManageMasterDataModel.IsDamgeUpdate = false;
            List<Damage> DamageCodeList = mmdc.GetDamageCodes().ToList();
            ViewBag.DamageFullDescription = new SelectList(DamageCodeList, "DamageCedexCode", "DamageFullDescription");
            ManageMasterDataModel.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            ManageMasterDataModel.IsViewMode = true;
            return View("EditDamageCode", ManageMasterDataModel);
        }

        public ActionResult EditDamageCode_View()
        {
            ManageMasterDataDamageCodeModel ManageMasterDataModel = new ManageMasterDataDamageCodeModel();
            ManageMasterDataModel.IsDamgeUpdate = false;
            List<Damage> DamageCodeList = mmdc.GetDamageCodes().ToList();
            ViewBag.DamageFullDescription = new SelectList(DamageCodeList, "DamageCedexCode", "DamageFullDescription");
            ManageMasterDataModel.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            ManageMasterDataModel.IsViewMode = true;
            return View("EditDamageCode_View", ManageMasterDataModel);
        }

        //[HttpPost]
        public ActionResult UpdateDamageCode([Bind] ManageMasterDataDamageCodeModel manageMasterDataModel)
        {
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            ManageMasterDataDamageCodeModel ManageMasterDataModel = new ManageMasterDataDamageCodeModel();
            ManageMasterDataModel.IsDamgeUpdate = true;
            Damage damageListToBeUpdated = new Damage();
            damageListToBeUpdated.DamageCedexCode = manageMasterDataModel.DamageCedexCode;
            damageListToBeUpdated.DamageName = manageMasterDataModel.DamageName;
            damageListToBeUpdated.DamageDescription = manageMasterDataModel.DamageDescription;
            damageListToBeUpdated.DamageNumericalCode = manageMasterDataModel.DamageNumericalCode;
            damageListToBeUpdated.ChangeUser = userId;
            //Call the model method to send for update
            string message = "";
            try
            {
                mmdc.UpdateDamageCode(damageListToBeUpdated); ;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            //List<Damage> DamageCodeList = mmdc.GetDamageCodes().ToList();
            //ViewBag.DamageFullDescription = new SelectList(DamageCodeList, "DamageCedexCode", "DamageFullDescription");
            manageMasterDataModel.IsDamgeUpdate = true;
            if (message.Trim() == "")
            {
                message = "Damage Code " + manageMasterDataModel.DamageCedexCode + " updated";
            }
            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(message, "Success");
            manageMasterDataModel.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            manageMasterDataModel.DamageCode = manageMasterDataModel.DamageCedexCode;
            return RedirectToAction("GetAllDetailsForDamageCode", manageMasterDataModel);
            //return View("EditDamageCode", manageMasterDataModel);
        }

        public ActionResult GetAllDetailsForDamageCode1(string id)
        {
            ManageMasterDataDamageCodeModel model = new ManageMasterDataDamageCodeModel();

            List<Damage> DamageCodelist = mmdc.GetDamageCode(id).ToList();
            Damage data = DamageCodelist.Find(Pid => Pid.DamageCedexCode == id);
            List<SelectListItem> drp = new List<SelectListItem>();

            return Json(data);

        }

        public ActionResult GetAllDetailsForDamageCode([Bind] ManageMasterDataDamageCodeModel model)
        {
            //ManageMasterDataDamageCodeModel model = new ManageMasterDataDamageCodeModel();
            ModelState.Clear();
            List<Damage> DamageCodelist = mmdc.GetDamageCode(model.DamageCode).ToList();
            Damage data = DamageCodelist.Find(Pid => Pid.DamageCedexCode == model.DamageCode);
            List<Damage> DamageCodeList = mmdc.GetDamageCodes().ToList();
            ViewBag.DamageFullDescription = new SelectList(DamageCodeList, "DamageCedexCode", "DamageFullDescription");
            model.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            model.IsViewMode = false;
            model.IsDamgeUpdate = true;
            model.DamageCedexCode = DamageCodelist[0].DamageCedexCode;
            model.DamageCHTS = DamageCodelist[0].ChangeTime.ToString("dd-MM-yyyy hh:mm:ss tt");
            string userName = DamageCodelist[0].ChangeUser;
            if (userName.Contains('|'))
                userName = userName.Replace("|", " ");
            model.DamageCHUser = userName;
            model.DamageDescription = DamageCodelist[0].DamageDescription;
            model.DamageName = DamageCodelist[0].DamageName;
            model.DamageNumericalCode = DamageCodelist[0].DamageNumericalCode;
            return View("EditDamageCode", model);

        }

        public ActionResult GetAllDetailsForDamageCode_View([Bind] ManageMasterDataDamageCodeModel model)
        {
            //ManageMasterDataDamageCodeModel model = new ManageMasterDataDamageCodeModel();
            ModelState.Clear();
            List<Damage> DamageCodelist = mmdc.GetDamageCode(model.DamageCode).ToList();
            Damage data = DamageCodelist.Find(Pid => Pid.DamageCedexCode == model.DamageCode);
            List<Damage> DamageCodeList = mmdc.GetDamageCodes().ToList();
            ViewBag.DamageFullDescription = new SelectList(DamageCodeList, "DamageCedexCode", "DamageFullDescription");
            model.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            model.IsViewMode = false;
            model.IsDamgeUpdate = true;
            model.DamageCedexCode = DamageCodelist[0].DamageCedexCode;
            model.DamageCHTS = DamageCodelist[0].ChangeTime.ToString("dd-MM-yyyy hh:mm:ss tt");
            string userName = DamageCodelist[0].ChangeUser;
            if (userName.Contains('|'))
                userName = userName.Replace("|", " ");
            model.DamageCHUser = userName;
            model.DamageDescription = DamageCodelist[0].DamageDescription;
            model.DamageName = DamageCodelist[0].DamageName;
            model.DamageNumericalCode = DamageCodelist[0].DamageNumericalCode;
            return View("EditDamageCode_View", model);

        }

        // [HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "Delete")]
        public ActionResult DeleteDamageCode([Bind] ManageMasterDataDamageCodeModel manageMasterDataModel)
        {
            bool sucess = mmdc.DeleteDamageCode(manageMasterDataModel.DamageCedexCode);
            manageMasterDataModel.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            if (sucess)
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Damage Code " + manageMasterDataModel.DamageCedexCode + " deleted", "Success");
            else
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage("Damage Code unable to delete", "Warning");
            return RedirectToAction("EditDamageCode");

        }
        #endregion DamageCode

        #region TPI

        public ActionResult EditTPI()
        {
            ManageMasterDataTPIIndicatorModel ManageMasterDataModel = new ManageMasterDataTPIIndicatorModel();
            ManageMasterDataModel.IsTPIUpdate = true;
            List<TPIIndicator> TPIIndicatorList = mmdc.GetTPIIndicators().ToList();
            List<SelectListItem> TPICategoryList = new List<SelectListItem>();
            TPICategoryList = PopulateTPICategoryDropDown(TPICategoryList, string.Empty);

            ViewBag.TPIFullDescription = new SelectList(TPIIndicatorList, "TPICedexCode", "TPIFullDescription");
            ManageMasterDataModel.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;

            ManageMasterDataModel.drpCategory = TPICategoryList;
            ManageMasterDataModel.IsTPIView = true;
            return View("EditTPI", ManageMasterDataModel);
        }
        public ActionResult EditTPI_View()
        {
            ManageMasterDataTPIIndicatorModel ManageMasterDataModel = new ManageMasterDataTPIIndicatorModel();
            ManageMasterDataModel.IsTPIUpdate = true;
            List<TPIIndicator> TPIIndicatorList = mmdc.GetTPIIndicators().ToList();
            List<SelectListItem> TPICategoryList = new List<SelectListItem>();
            TPICategoryList = PopulateTPICategoryDropDown(TPICategoryList, string.Empty);

            ViewBag.TPIFullDescription = new SelectList(TPIIndicatorList, "TPICedexCode", "TPIFullDescription");
            ManageMasterDataModel.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;

            ManageMasterDataModel.drpCategory = TPICategoryList;
            ManageMasterDataModel.IsTPIView = true;
            return View("EditTPI_View", ManageMasterDataModel);
        }
        //[HttpPost]
        public ActionResult UpdateTPI([Bind] ManageMasterDataTPIIndicatorModel manageMasterDataModel)
        {
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            manageMasterDataModel.IsTPIUpdate = true;
            manageMasterDataModel.TPISelectedCedexCode = manageMasterDataModel.TPICedexCode;
            TPIIndicator tpiListToBeUpdated = new TPIIndicator();
            //List<SelectListItem> TPICategoryList = new List<SelectListItem>();
            tpiListToBeUpdated.TPICedexCode = manageMasterDataModel.TPICedexCode;
            tpiListToBeUpdated.TPIName = manageMasterDataModel.TPIName;
            tpiListToBeUpdated.TPIDescription = manageMasterDataModel.TPIDescription;
            tpiListToBeUpdated.TPINumericalCode = manageMasterDataModel.TPINumericalCode;
            //TPICategoryList = PopulateTPICategoryDropDown(TPICategoryList, string.Empty);
            tpiListToBeUpdated.Category = manageMasterDataModel.Category;
            tpiListToBeUpdated.TPICHUser = userId;
            tpiListToBeUpdated.TPICHTS = DateTime.Now;
            //Call the model method to send for update
            string msg = "";
            try
            {
                bool success = mmdc.UpdateTPIIndicator(tpiListToBeUpdated, ref msg);
                if (success)
                {
                    msg = "TPI " + manageMasterDataModel.TPICedexCode + " updated";
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Success");
                }
                else
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
                //List<TPIIndicator> TPIIndicatorList = mmdc.GetTPIIndicators().ToList();
                //List<SelectListItem> TPICategoryList = new List<SelectListItem>();
                //TPICategoryList = PopulateTPICategoryDropDown(TPICategoryList, string.Empty);

                //ViewBag.TPIFullDescription = new SelectList(TPIIndicatorList, "TPICedexCode", "TPIFullDescription");


                //manageMasterDataModel.drpCategory = TPICategoryList;
            }
            catch (Exception ex) { }
            //return View("EditTPI", manageMasterDataModel);
            return RedirectToAction("GetAllDetailsForTPI", manageMasterDataModel);
        }

        [HttpPost]
        public ActionResult TPICreate([Bind] ManageMasterDataTPIIndicatorModel ManageMasterDataModel)
        {
            //ManageMasterDataTPIIndicatorModel ManageMasterDataModel = new ManageMasterDataTPIIndicatorModel();
            ManageMasterDataModel.IsTPIUpdate = false;
            List<TPIIndicator> tpiList = mmdc.GetTPIIndicators().ToList();
            ViewBag.TPIFullDescription = new SelectList(tpiList, "TPICedexCode", "TPIFullDescription");
            List<SelectListItem> TPICategoryList = new List<SelectListItem>();
            TPICategoryList = PopulateTPICategoryDropDown(TPICategoryList, string.Empty);
            ManageMasterDataModel.drpCategory = TPICategoryList;
            ManageMasterDataModel.ShowData = true;
            ManageMasterDataModel.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            ManageMasterDataModel.IsTPIAdd = true;
            ManageMasterDataModel.IsTPIUpdate = false;
            ManageMasterDataModel.TPICHUser = "N/A";
            ManageMasterDataModel.TPICHTS = "N/A";
            return View("EditTPI", ManageMasterDataModel);
        }

        public ActionResult TPICreateFail([Bind] ManageMasterDataTPIIndicatorModel ManageMasterDataModel)
        {
            //ManageMasterDataTPIIndicatorModel ManageMasterDataModel = new ManageMasterDataTPIIndicatorModel();
            ManageMasterDataModel.IsTPIUpdate = false;
            List<TPIIndicator> tpiList = mmdc.GetTPIIndicators().ToList();
            ViewBag.TPIFullDescription = new SelectList(tpiList, "TPICedexCode", "TPIFullDescription");
            List<SelectListItem> TPICategoryList = new List<SelectListItem>();
            TPICategoryList = PopulateTPICategoryDropDown(TPICategoryList, string.Empty);
            ManageMasterDataModel.drpCategory = TPICategoryList;
            ManageMasterDataModel.ShowData = true;
            ManageMasterDataModel.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            ManageMasterDataModel.IsTPIAdd = true;
            return View("EditTPI", ManageMasterDataModel);
        }

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "Add")]
        public ActionResult CreateTPI([Bind] ManageMasterDataTPIIndicatorModel ManageMasterDataModel)
        {
            bool success = false;
            string userId = ((UserSec)Session["UserSec"]).UserId.ToString();
            ManageMasterDataModel.IsTPIUpdate = false;
            TPIIndicator tpiListToBeCreated = new TPIIndicator();
            tpiListToBeCreated.TPICedexCode = ManageMasterDataModel.TPICedexCode;
            tpiListToBeCreated.TPIName = ManageMasterDataModel.TPIName;
            tpiListToBeCreated.TPIDescription = ManageMasterDataModel.TPIDescription;
            tpiListToBeCreated.TPINumericalCode = ManageMasterDataModel.TPINumericalCode;
            tpiListToBeCreated.Category = ManageMasterDataModel.Category;
            tpiListToBeCreated.TPICHUser = userId;
            tpiListToBeCreated.TPICHTS = DateTime.Now;
            string msg = "";
            try
            {
                success = mmdc.CreateTPIIndicator(tpiListToBeCreated, ref msg);
                if (success)
                {
                    msg = "TPI " + ManageMasterDataModel.TPICedexCode + " added";
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Success");
                }
                else
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
            }
            catch (Exception ex) { }
            ManageMasterDataModel.ShowData = true;
            if (success)
            {
                ManageMasterDataModel.TPISelectedCedexCode = ManageMasterDataModel.TPICedexCode;
                ManageMasterDataModel.IsTPIAdded = true;
                return RedirectToAction("GetAllDetailsForTPI", ManageMasterDataModel);
            }
            else
                return RedirectToAction("TPICreateFail", ManageMasterDataModel);
        }

        public ActionResult TPIDeleted([Bind] ManageMasterDataTPIIndicatorModel model)
        {
            model.IsTPIDeleted = true;
            model.ShowData = true;
            //ManageMasterDataTPIIndicatorModel ManageMasterDataModel = new ManageMasterDataTPIIndicatorModel();
            //ManageMasterDataModel.IsTPIUpdate = true;
            List<TPIIndicator> TPIIndicatorList = mmdc.GetTPIIndicators().ToList();
            List<SelectListItem> TPICategoryList = new List<SelectListItem>();
            TPICategoryList = PopulateTPICategoryDropDown(TPICategoryList, string.Empty);

            ViewBag.TPIFullDescription = new SelectList(TPIIndicatorList, "TPICedexCode", "TPIFullDescription");
            model.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;

            model.drpCategory = TPICategoryList;
            return View("EditTPI", model);
        }

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "Delete")]
        public ActionResult DeleteTPI([Bind] ManageMasterDataTPIIndicatorModel manageMasterDataModel)
        {
            string msg = "";
            bool success = false;
            try
            {
                success = mmdc.DeleteTPIIndicator(manageMasterDataModel.TPICedexCode, ref msg);
                if (success)
                {
                    msg = "TPI " + manageMasterDataModel.TPICedexCode + " Deleted";
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Success");
                }
                else
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(msg, "Warning");
            }
            catch (Exception ex) { }
            if (success)
            {
                return RedirectToAction("TPIDeleted", manageMasterDataModel);
            }
            else
            {
                manageMasterDataModel.TPISelectedCedexCode = manageMasterDataModel.TPICedexCode;
                return RedirectToAction("GetAllDetailsForTPI", manageMasterDataModel);
            }

        }

        private List<SelectListItem> PopulateTPICategoryDropDown(List<SelectListItem> TPICategoryList, string TPICategoryString)
        {


            TPICategoryList.Add(new SelectListItem
            {
                Text = "W- Wear and Tear",
                Value = "W"
            });
            TPICategoryList.Add(new SelectListItem
            {
                Text = "T- Third Party",
                Value = "T"
            });



            return TPICategoryList;
        }

        public ActionResult GetAllDetailsForTPI([Bind] ManageMasterDataTPIIndicatorModel model)
        {
            ModelState.Clear();

            //ManageMasterDataTPIIndicatorModel model = new ManageMasterDataTPIIndicatorModel();
            List<TPIIndicator> TPIIndicatorList = mmdc.GetTPIIndicators().ToList();
            List<SelectListItem> TPICategoryList = new List<SelectListItem>();
            TPICategoryList = PopulateTPICategoryDropDown(TPICategoryList, string.Empty);
            List<TPIIndicator> tpiList = mmdc.GetTPIIndicator(model.TPISelectedCedexCode).ToList();
            ViewBag.TPIFullDescription = new SelectList(TPIIndicatorList, "TPICedexCode", "TPIFullDescription");
            model.drpCategory = TPICategoryList;
            // List<EquipmentType> equipmenttypelist = mmdc.GetEquipmentTypeList().ToList();
            TPIIndicator data = tpiList.Find(Pid => Pid.TPICedexCode == model.TPISelectedCedexCode);
            //List<SelectListItem> drp = new List<SelectListItem>();
            model.TPICedexCode = data.TPICedexCode;
            model.TPIName = data.TPIName;
            model.TPIDescription = data.TPIDescription;
            model.TPINumericalCode = data.TPINumericalCode;
            string userName = data.TPICHUser;
            if (userName.Contains('|'))
                userName = userName.Replace("|", " ");
            model.TPICHUser = userName;
            model.TPICHTS = data.TPICHTS.ToString("yyyy-MM-dd hh:mm:ss tt");
            model.Category = data.Category;
            model.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            model.ShowData = true;
            //model.TPISelectedCedexCode = manageMasterDataModel.TPISelectedCedexCode;
            model.IsTPIUpdate = true;
            model.IsTPIAdded = false;
            return View("EditTPI", model);
        }

        public ActionResult GetAllDetailsForTPI_View([Bind] ManageMasterDataTPIIndicatorModel model)
        {
            ModelState.Clear();

            //ManageMasterDataTPIIndicatorModel model = new ManageMasterDataTPIIndicatorModel();
            List<TPIIndicator> TPIIndicatorList = mmdc.GetTPIIndicators().ToList();
            List<SelectListItem> TPICategoryList = new List<SelectListItem>();
            TPICategoryList = PopulateTPICategoryDropDown(TPICategoryList, string.Empty);
            List<TPIIndicator> tpiList = mmdc.GetTPIIndicator(model.TPISelectedCedexCode).ToList();
            ViewBag.TPIFullDescription = new SelectList(TPIIndicatorList, "TPICedexCode", "TPIFullDescription");
            model.drpCategory = TPICategoryList;
            // List<EquipmentType> equipmenttypelist = mmdc.GetEquipmentTypeList().ToList();
            TPIIndicator data = tpiList.Find(Pid => Pid.TPICedexCode == model.TPISelectedCedexCode);
            //List<SelectListItem> drp = new List<SelectListItem>();
            model.TPICedexCode = data.TPICedexCode;
            model.TPIName = data.TPIName;
            model.TPIDescription = data.TPIDescription;
            model.TPINumericalCode = data.TPINumericalCode;
            string userName = data.TPICHUser;
            if (userName.Contains('|'))
                userName = userName.Replace("|", " ");
            model.TPICHUser = userName;
            model.TPICHTS = data.TPICHTS.ToString("yyyy-MM-dd hh:mm:ss tt");
            model.Category = data.Category;
            model.IsAdmin = ((UserSec)Session["UserSec"]).isAdmin;
            model.ShowData = true;
            //model.TPISelectedCedexCode = manageMasterDataModel.TPISelectedCedexCode;
            model.IsTPIUpdate = true;
            model.IsTPIAdded = false;
            return View("EditTPI_View", model);
        }
        #endregion TPI

        #region  Country Labour Rate


        #region CountryLabourRate HTTPGet

        public ActionResult ViewCountryLabourRateDetails(List<Country> CountryList = null, List<EqType> EqTypeList = null)
        {

            // Populate Country Drop Down  //

            CountryList = mmdc.GetCountryLabourList().ToList();
            List<SelectListItem> CountryCode = new List<SelectListItem>();
            int count = 1;

            CountryCode.Add(new SelectListItem
            {
                Text = "Select Country",
                Value = "Select Country"
            });


            foreach (var code in CountryList)
            {
                CountryCode.Add(new SelectListItem
                {
                    Text = code.CountryCode,
                    Value = code.CountryCode
                });
                count++;
            }


            ManageMasterCountryLabourRateModel.drpCountryList = CountryCode;

            // Populate Equipment Type  //

            EqTypeList = mmdc.GetEquipmentTypeList().ToList();
            List<SelectListItem> EquipmentType = new List<SelectListItem>();

            EquipmentType.Add(new SelectListItem
            {
                Text = "Select Equipment",
                Value = "Select Equipment"
            });


            int counts = 1;
            foreach (var eqTyp in EqTypeList)
            {
                EquipmentType.Add(new SelectListItem
                {
                    Text = eqTyp.EqpType,
                    Value = eqTyp.EqpType
                });
                counts++;
            }


            ManageMasterCountryLabourRateModel.drpEquipmentType = EquipmentType;

            return View("ViewCountryLabourRateDetails", ManageMasterCountryLabourRateModel);
        }


        public ActionResult ViewCountryLabourRateDetails_View(List<Country> CountryList = null, List<EqType> EqTypeList = null)
        {

            // Populate Country Drop Down  //

            CountryList = mmdc.GetCountryLabourList().ToList();
            List<SelectListItem> CountryCode = new List<SelectListItem>();
            int count = 1;

            CountryCode.Add(new SelectListItem
            {
                Text = "Select Country",
                Value = "Select Country"
            });


            foreach (var code in CountryList)
            {
                CountryCode.Add(new SelectListItem
                {
                    Text = code.CountryCode,
                    Value = code.CountryCode
                });
                count++;
            }


            ManageMasterCountryLabourRateModel.drpCountryList = CountryCode;

            // Populate Equipment Type  //

            EqTypeList = mmdc.GetEquipmentTypeList().ToList();
            List<SelectListItem> EquipmentType = new List<SelectListItem>();

            EquipmentType.Add(new SelectListItem
            {
                Text = "Select Equipment",
                Value = "Select Equipment"
            });


            int counts = 1;
            foreach (var eqTyp in EqTypeList)
            {
                EquipmentType.Add(new SelectListItem
                {
                    Text = eqTyp.EqpType,
                    Value = eqTyp.EqpType
                });
                counts++;
            }


            ManageMasterCountryLabourRateModel.drpEquipmentType = EquipmentType;

            return View("ViewCountryLabourRateDetails_View", ManageMasterCountryLabourRateModel);
        }




        [HttpPost]
        public ActionResult GetCountryLabourRateDetails(ManageMasterCountryLabourRateModel ManageMasterCountryLabourRateModel, FormCollection frm)
        {

            string strCountryCode = ManageMasterCountryLabourRateModel.CountryCode;
            string strEquipmentType = ManageMasterCountryLabourRateModel.EquipmentType;
            string errorMessage = string.Empty;

            try
            {

                List<Country> CountryList = null;
                List<EqType> EqTypeList = null;
                if (strCountryCode == "Select Country")
                {
                    strCountryCode = "";
                }

                if (strEquipmentType == "Select Equipment")
                {
                    strEquipmentType = "";
                }
                var result = mmdc.GetLabourRateDetails(strCountryCode, strEquipmentType).ToList();
                TempData["Msg"] = string.Empty;
                errorMessage = string.Empty;
                if (result.Count < 1)
                {
                    errorMessage = "There were no results meeting your query parameters.";
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(errorMessage, "Warning");
                }


                var SearchData = (from e in result
                                  select new ManageMasterDataModel
                                  {

                                      COUNTRYNAME = Convert.ToString(e.CountryCode),
                                      EQUIPMENT_TYPE = Convert.ToString(e.EqType),
                                      EFFECTIVE_DATE = (e.EffDate).ToString("yyyy-MM-dd"),
                                      EXPIRATION_DATE = (e.ExpDate).ToString("yyyy-MM-dd"),
                                      ORDINARY_RATE = Convert.ToInt32(e.RegularRT).ToString(),
                                      OT1_RATE = Convert.ToInt32(e.DoubleTimeRT).ToString(),
                                      OT2_RATE = Convert.ToInt32(e.OvertimeRT).ToString(),
                                      OT3_RATE = Convert.ToInt32(e.MiscRT).ToString(),


                                  }).ToList();

                if (string.IsNullOrEmpty(errorMessage))
                {
                    ManageMasterCountryLabourRateModel.SearchResult = SearchData;
                }
                else
                {

                }

                // Populate Country Drop Down  //

                CountryList = mmdc.GetCountryLabourList().ToList();
                List<SelectListItem> CountryCode = new List<SelectListItem>();
                int count = 1;

                CountryCode.Add(new SelectListItem
                {
                    Text = "Select Country",
                    Value = "Select Country"
                });


                foreach (var code in CountryList)
                {
                    CountryCode.Add(new SelectListItem
                    {
                        Text = code.CountryCode,
                        Value = code.CountryCode
                    });
                    count++;
                }


                ManageMasterCountryLabourRateModel.drpCountryList = CountryCode;

                // Populate Equipment Type  //

                EqTypeList = mmdc.GetEquipmentTypeList().ToList();
                List<SelectListItem> EquipmentType = new List<SelectListItem>();
                int counts = 1;

                EquipmentType.Add(new SelectListItem
                {
                    Text = "Select Equipment",
                    Value = "Select Equipment"
                });



                foreach (var eqTyp in EqTypeList)
                {
                    EquipmentType.Add(new SelectListItem
                    {
                        Text = eqTyp.EqpType,
                        Value = eqTyp.EqpType
                    });
                    counts++;
                }



                ManageMasterCountryLabourRateModel.drpEquipmentType = EquipmentType;
            }
            catch (Exception ex)
            {

            }




            return View("ViewCountryLabourRateDetails", ManageMasterCountryLabourRateModel);
        }




        private List<SelectListItem> PopulateEquipmentDropDown(List<EqType> EqTypeList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            EqTypeList = mmdc.GetEquipmentTypeList().ToList();
            List<SelectListItem> EqTypeCode = new List<SelectListItem>();

            int count = 1;
            foreach (var eqtype in EqTypeList)
            {
                EqTypeCode.Add(new SelectListItem
                {
                    Text = eqtype.EqpType,
                    Value = eqtype.EqpType
                });

                count++;
            }

            return EqTypeCode;

        }

        private List<SelectListItem> PopulateCountryLabourRateDropDown(List<Country> CountryList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            CountryList = mmdc.GetCountryLabourList().ToList();
            List<SelectListItem> CountryCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in CountryList)
            {
                CountryCode.Add(new SelectListItem
                {
                    Text = code.CountryCode,
                    Value = code.CountryCode
                });

                count++;
            }

            return CountryCode;

        }

        #endregion




        #endregion


        #region CPH Approval Level


        public ActionResult GetAllDetailsForCPHApproval([Bind] ManageMasterDataCPHModel ManageMasterDataCPHModel)
        {
            var request = HttpContext.Request;
            ManageMasterDataCPHModel.View = false;
            if (ManageMasterDataCPHModel.Header)
            {
                ManageMasterDataCPHModel.Header = true;
            }
            ManageMasterDataModel model = new ManageMasterDataModel();
            ManageMasterDataCPHModel.IsFlag = false;
            ManageMasterDataCPHModel.IsSubmit = false;


            CphEqpLimit data = new CphEqpLimit();
            List<CphEqpLimit> dataList = new List<CphEqpLimit>();

            if (!string.IsNullOrEmpty(ManageMasterDataCPHModel.Eq_Size) && !string.IsNullOrEmpty(ManageMasterDataCPHModel.Mode_List))
            {

                dataList = mmdc.GetRSAllLimits(ManageMasterDataCPHModel.Eq_Size, ManageMasterDataCPHModel.Mode_List).ToList();
                if (dataList.Count == 0)
                {
                    ErrorMessage = "There were no results meeting your query parameters ";
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                    ManageMasterDataCPHModel.IsAdd = false;
                }
                else
                {
                    ManageMasterDataCPHModel.IsAdd = true;

                    for (int count = 0; count < dataList.Count; count++)
                    {
                        string userName = dataList[count].ChangeUser;
                        if (userName != null || userName != "0")
                        {
                            if ((userName).Contains('|'))
                            {
                                if (count == 0)
                                {

                                    ManageMasterDataCPHModel.txtLimitAmtAdd1 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser1 = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                                    ManageMasterDataCPHModel.txtChangeTime1 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                                if (count == 1)
                                {
                                    ManageMasterDataCPHModel.txtLimitAmtAdd2 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser2 = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                                    ManageMasterDataCPHModel.txtChangeTime2 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                                if (count == 2)
                                {
                                    ManageMasterDataCPHModel.txtLimitAmtAdd3 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser3 = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                                    ManageMasterDataCPHModel.txtChangeTime3 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                                if (count == 3)
                                {
                                    ManageMasterDataCPHModel.txtLimitAmtAdd4 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser4 = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                                    ManageMasterDataCPHModel.txtChangeTime4 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                                if (count == 4)
                                {
                                    ManageMasterDataCPHModel.txtLimitAmtAdd5 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser5 = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                                    ManageMasterDataCPHModel.txtChangeTime5 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                                if (count == 5)
                                {
                                    ManageMasterDataCPHModel.txtLimitAmtAdd6 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser6 = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                                    ManageMasterDataCPHModel.txtChangeTime6 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                                if (count == 6)
                                {
                                    ManageMasterDataCPHModel.txtLimitAmtAdd7 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser7 = (userName).Split('|')[0] + " " + (userName).Split('|')[1];
                                    ManageMasterDataCPHModel.txtChangeTime7 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }

                            }
                            else
                            {
                                if (count == 0)
                                {

                                    ManageMasterDataCPHModel.txtLimitAmtAdd1 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser1 = userName;
                                    ManageMasterDataCPHModel.txtChangeTime1 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                                if (count == 1)
                                {
                                    ManageMasterDataCPHModel.txtLimitAmtAdd2 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser2 = userName;
                                    ManageMasterDataCPHModel.txtChangeTime2 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                                if (count == 2)
                                {
                                    ManageMasterDataCPHModel.txtLimitAmtAdd3 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser3 = userName;
                                    ManageMasterDataCPHModel.txtChangeTime3 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                                if (count == 3)
                                {
                                    ManageMasterDataCPHModel.txtLimitAmtAdd4 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser4 = userName;
                                    ManageMasterDataCPHModel.txtChangeTime4 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                                if (count == 4)
                                {
                                    ManageMasterDataCPHModel.txtLimitAmtAdd5 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser5 = userName;
                                    ManageMasterDataCPHModel.txtChangeTime5 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                                if (count == 5)
                                {
                                    ManageMasterDataCPHModel.txtLimitAmtAdd6 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser6 = userName;
                                    ManageMasterDataCPHModel.txtChangeTime6 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                                if (count == 6)
                                {
                                    ManageMasterDataCPHModel.txtLimitAmtAdd7 = Convert.ToString(dataList[count].LimitAmount);
                                    ManageMasterDataCPHModel.txtChangeUser7 = userName;
                                    ManageMasterDataCPHModel.txtChangeTime7 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                                }
                            }
                        }
                        else
                        {
                            if (count == 0)
                            {
                                ManageMasterDataCPHModel.txtLimitAmtAdd1 = Convert.ToString(dataList[count].LimitAmount);
                                ManageMasterDataCPHModel.txtChangeUser1 = "";
                                ManageMasterDataCPHModel.txtChangeTime1 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                            }
                            if (count == 1)
                            {
                                ManageMasterDataCPHModel.txtLimitAmtAdd2 = Convert.ToString(dataList[count].LimitAmount);
                                ManageMasterDataCPHModel.txtChangeUser2 = "";
                                ManageMasterDataCPHModel.txtChangeTime2 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                            }
                            if (count == 2)
                            {
                                ManageMasterDataCPHModel.txtLimitAmtAdd3 = Convert.ToString(dataList[count].LimitAmount);
                                ManageMasterDataCPHModel.txtChangeUser3 = "";
                                ManageMasterDataCPHModel.txtChangeTime3 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                            }
                            if (count == 3)
                            {
                                ManageMasterDataCPHModel.txtLimitAmtAdd4 = Convert.ToString(dataList[count].LimitAmount);
                                ManageMasterDataCPHModel.txtChangeUser4 = "";
                                ManageMasterDataCPHModel.txtChangeTime4 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                            }
                            if (count == 4)
                            {
                                ManageMasterDataCPHModel.txtLimitAmtAdd5 = Convert.ToString(dataList[count].LimitAmount);
                                ManageMasterDataCPHModel.txtChangeUser5 = "";
                                ManageMasterDataCPHModel.txtChangeTime5 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                            }
                            if (count == 5)
                            {
                                ManageMasterDataCPHModel.txtLimitAmtAdd6 = Convert.ToString(dataList[count].LimitAmount);
                                ManageMasterDataCPHModel.txtChangeUser6 = "";
                                ManageMasterDataCPHModel.txtChangeTime6 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                            }
                            if (count == 6)
                            {
                                ManageMasterDataCPHModel.txtLimitAmtAdd7 = Convert.ToString(dataList[count].LimitAmount);
                                ManageMasterDataCPHModel.txtChangeUser7 = "";
                                ManageMasterDataCPHModel.txtChangeTime7 = (dataList[count].ChTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                            }
                        }
                    }
                }
                ManageMasterDataCPHModel.ddlEqSize = PopulateEqSizeDropDown();
                ManageMasterDataCPHModel.ddlMode_List = PopulateModeListDropDown();
                ManageMasterDataCPHModel.drpEqSize = PopulateEqSizeDropDown();
                ManageMasterDataCPHModel.drpMode_List = PopulateModeListDropDown();
            }

            return View("ViewCPHApprovalLevel", ManageMasterDataCPHModel);
        }

        private List<SelectListItem> PopulateEqSizeDropDown()
        {
            List<SelectListItem> EqSizeList = new List<SelectListItem>();

            EqSizeList.Add(new SelectListItem
            {
                Text = "20",
                Value = "20"
            });
            EqSizeList.Add(new SelectListItem
            {
                Text = "40",
                Value = "40"
            });
            EqSizeList.Add(new SelectListItem
            {
                Text = "45",
                Value = "45"
            });
            EqSizeList.Add(new SelectListItem
            {
                Text = "48",
                Value = "48"
            });
            EqSizeList.Add(new SelectListItem
            {
                Text = "NA",
                Value = "NA"
            });
            return EqSizeList;


        }

        private List<SelectListItem> PopulateModeListDropDown(List<Mode> CphEqpLimitList = null)
        {

            List<SelectListItem> ModeList = new List<SelectListItem>();
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            CphEqpLimitList = mmdc.GetCphEquLimitModeList().ToList();


            int count = 1;




            foreach (var EqpLimit in CphEqpLimitList)
            {


                ModeList.Add(new SelectListItem
                {
                    Text = EqpLimit.ModeCode + "-" + EqpLimit.ModeDescription,
                    Value = EqpLimit.ModeCode
                });

                count++;
            }

            return ModeList;


        }


        #region CPHApproval Level HTTP GET

        public ActionResult ViewCPHApprovalLevel()
        {

            ManageMasterDataCPHModel.UsrId = ((UserSec)Session["UserSec"]).LoginId;
            ManageMasterDataCPHModel.UsrRole = ((UserSec)Session["UserSec"]).UserType;
            List<SelectListItem> EqSize = new List<SelectListItem>();
            List<SelectListItem> ModeList = new List<SelectListItem>();
            EqSize = PopulateEqSizeDropDown();
            ModeList = PopulateModeListDropDown();
            ManageMasterDataCPHModel.drpEqSize = EqSize;
            ManageMasterDataCPHModel.drpMode_List = ModeList;
            ManageMasterDataCPHModel.IsFlag = false;
            ManageMasterDataCPHModel.IsSubmit = false;
            ManageMasterDataCPHModel.IsAdd = false;
            ManageMasterDataCPHModel.Header = false;
            ManageMasterDataCPHModel.View = true;

            return View("ViewCPHApprovalLevel", ManageMasterDataCPHModel);

        }

        public ActionResult AddCPHApproval()
        {

            ManageMasterDataCPHModel ManageMasterDataCPHModel = new ManageMasterDataCPHModel();
            ManageMasterDataCPHModel.UsrId = ((UserSec)Session["UserSec"]).LoginId;
            ManageMasterDataCPHModel.UsrRole = ((UserSec)Session["UserSec"]).UserType;
            List<SelectListItem> EqSize = new List<SelectListItem>();
            List<SelectListItem> ModeList = new List<SelectListItem>();
            EqSize = PopulateEqSizeDropDown();
            ModeList = PopulateModeListDropDown();
            ManageMasterDataCPHModel.drpEqSize = EqSize;
            ManageMasterDataCPHModel.drpMode_List = ModeList;
            ManageMasterDataCPHModel.IsFlag = true;
            ManageMasterDataCPHModel.IsSubmit = true;
            ManageMasterDataCPHModel.IsAdd = false;
            ManageMasterDataCPHModel.Header = true;
            ManageMasterDataCPHModel.IsEdit = true;
            return View("ViewCPHApprovalLevel", ManageMasterDataCPHModel);
        }
        #endregion


        #region CPHApproval Level HTTP POST


        [HttpPost]
        public ActionResult SubmitCPHApprovalDetails(FormCollection frm, ManageMasterDataCPHModel ManageMasterDataCPHModel)
        {
            string ErrMsg;
            ManageMasterDataCPHModel.IsFlag = false;
            try
            {
                CphEqpLimit cphEqp = new CphEqpLimit();
                List<SelectListItem> EqSize = new List<SelectListItem>();
                List<SelectListItem> ModeList = new List<SelectListItem>();
                cphEqp.EqSize = Convert.ToString(frm["ddlEqSize"]);
                cphEqp.ModeCode = Convert.ToString(frm["ddlMode_List"]);
                decimal cphAmt = Convert.ToDecimal(cphEqp.LimitAmount);
                string UserId = ((UserSec)Session["UserSec"]).LoginId;
                bool IsValid = false;
                IsValid = mmdc.IsCheckDuplicate(cphEqp.EqSize, cphEqp.ModeCode);
                if (IsValid == true)
                {
                    //TempData["SubmitMsg"] = "Approval Limits" + cphEqp.EqSize + "/" + cphEqp.ModeCode + "Already Exists, not Added.";
                    ErrMsg = "Approval Limits " + cphEqp.EqSize + "/" + cphEqp.ModeCode + " already exists, not Added.";
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrMsg, "Warning");
                    ManageMasterDataCPHModel.IsAdd = false;
                    ManageMasterDataCPHModel.IsFlag = false;
                    ManageMasterDataCPHModel.IsSubmit = false;
                }
                else
                {
                    List<CphEqpLimit> cphEquList = mmdc.SubmitCPHApprovalDetails(cphEqp, UserId, Convert.ToString(frm["txtLimitAmt1"]), Convert.ToString(frm["txtLimitAmt2"]), Convert.ToString(frm["txtLimitAmt3"]), Convert.ToString(frm["txtLimitAmt4"]), Convert.ToString(frm["txtLimitAmt5"]), Convert.ToString(frm["txtLimitAmt6"]), Convert.ToString(frm["txtLimitAmt7"])).ToList();
                    //TempData["SubmitMsg"] = "Approval Limits" + cphEqp.EqSize + "/" + cphEqp.ModeCode + " Added.";
                    ErrMsg = "Approval Limits " + cphEqp.EqSize + "/" + cphEqp.ModeCode + " Added.";
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrMsg, "Success");
                    // List<CphEqpLimit> CphList = mmdc.GetRSAllLimits(cphEqp.EqSize, cphEqp.ModeCode).ToList();
                    ManageMasterDataCPHModel.IsAdd = true;
                    ManageMasterDataCPHModel.IsFlag = false;
                    ManageMasterDataCPHModel.IsSubmit = true;
                    ManageMasterDataCPHModel.Header = true;
                    ManageMasterDataCPHModel.IsEdit = false;
                }


                EqSize = PopulateEqSizeDropDown();
                ModeList = PopulateModeListDropDown();
                ManageMasterDataCPHModel.drpEqSize = EqSize;
                ManageMasterDataCPHModel.drpMode_List = ModeList;
                ManageMasterDataCPHModel.UsrRole = ((UserSec)Session["UserSec"]).UserType;
                ManageMasterDataCPHModel.Eq_Size = cphEqp.EqSize;
                ManageMasterDataCPHModel.Mode_List = cphEqp.ModeCode;
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("GetAllDetailsForCPHApproval", ManageMasterDataCPHModel);
            //return View("ViewCPHApprovalLevel", ManageMasterDataCPHModel);
        }


        [HttpPost]
        public ActionResult EditCPHApprovalDetails(ManageMasterDataCPHModel ManageMasterDataModelCPH)
        {

            CphEqpLimit cphEqp = new CphEqpLimit();
            string UserId = ((UserSec)Session["UserSec"]).LoginId;
            string UserLogin = UserId;
            bool bUpdateSW = false;

            //      cphEqp.EqSize = Convert.ToString(frm["Eq_Size"]);
            //      cphEqp.ModeCode = Convert.ToString(frm["Mode_List"]);

            cphEqp.EqSize = ManageMasterDataModelCPH.Eq_Size;
            cphEqp.ModeCode = ManageMasterDataModelCPH.Mode_List;

            ManageMasterDataModelCPH.EqSize = cphEqp.EqSize;
            ManageMasterDataModelCPH.Mode_List = cphEqp.ModeCode;

            List<CphEqpLimit> cphList = mmdc.GetRSAllLimits(cphEqp.EqSize, cphEqp.ModeCode).ToList();
            if (cphList.Count == 0)
            {
                //// Display Message ///  Approval Limit Data is Unavailable  //
            }
            else
            {

                for (int i = 0; i < cphList.Count; i++)
                {

                    string age = Convert.ToString(cphList[i].AgeFrom);
                    if (age == "0")
                    {
                        //ManageMasterDataModelCPH.txtChangeTime1 = Convert.ToString(cphList[i].ChTime);
                        //ManageMasterDataModelCPH.txtChangeUser1 = Convert.ToString(cphList[i].ChangeUser);

                        if (Convert.ToString(cphList[i].LimitAmount) != ManageMasterDataModelCPH.txtLimitAmtAdd1)
                        {
                            List<CphEqpLimit> cphEquList = mmdc.UpdateCPHApprovalDetails(cphEqp.EqSize, cphEqp.ModeCode, age, ManageMasterDataModelCPH.txtLimitAmtAdd1, UserLogin).ToList();
                            bUpdateSW = true;

                        }


                    }

                    if (age == "3")
                    {
                        //ManageMasterDataModelCPH.txtChangeTime2 = Convert.ToString(cphList[i].ChTime);
                        //ManageMasterDataModelCPH.txtChangeUser2 = Convert.ToString(cphList[i].ChangeUser);

                        if (Convert.ToString(cphList[i].LimitAmount) != ManageMasterDataModelCPH.txtLimitAmtAdd2)
                        {
                            List<CphEqpLimit> cphEquList = mmdc.UpdateCPHApprovalDetails(cphEqp.EqSize, cphEqp.ModeCode, age, ManageMasterDataModelCPH.txtLimitAmtAdd2, UserLogin).ToList();
                            bUpdateSW = true;

                        }

                    }


                    if (age == "5")
                    {
                        //ManageMasterDataModelCPH.txtChangeTime3 = Convert.ToString(cphList[i].ChTime);
                        //ManageMasterDataModelCPH.txtChangeUser3 = Convert.ToString(cphList[i].ChangeUser);

                        if (Convert.ToString(cphList[i].LimitAmount) != ManageMasterDataModelCPH.txtLimitAmtAdd3)
                        {
                            List<CphEqpLimit> cphEquList = mmdc.UpdateCPHApprovalDetails(cphEqp.EqSize, cphEqp.ModeCode, age, ManageMasterDataModelCPH.txtLimitAmtAdd3, UserLogin).ToList();
                            bUpdateSW = true;

                        }

                    }


                    if (age == "7")
                    {
                        //ManageMasterDataModelCPH.txtChangeTime4 = Convert.ToString(cphList[i].ChTime);
                        //ManageMasterDataModelCPH.txtChangeUser4 = Convert.ToString(cphList[i].ChangeUser);

                        if (Convert.ToString(cphList[i].LimitAmount) != ManageMasterDataModelCPH.txtLimitAmtAdd4)
                        {
                            List<CphEqpLimit> cphEquList = mmdc.UpdateCPHApprovalDetails(cphEqp.EqSize, cphEqp.ModeCode, age, ManageMasterDataModelCPH.txtLimitAmtAdd4, UserLogin).ToList();
                            bUpdateSW = true;

                        }

                    }


                    if (age == "9")
                    {
                        //ManageMasterDataModelCPH.txtChangeTime5 = Convert.ToString(cphList[i].ChTime);
                        //ManageMasterDataModelCPH.txtChangeUser5 = Convert.ToString(cphList[i].ChangeUser);

                        if (Convert.ToString(cphList[i].LimitAmount) != ManageMasterDataModelCPH.txtLimitAmtAdd5)
                        {
                            List<CphEqpLimit> cphEquList = mmdc.UpdateCPHApprovalDetails(cphEqp.EqSize, cphEqp.ModeCode, age, ManageMasterDataModelCPH.txtLimitAmtAdd5, UserLogin).ToList();
                            bUpdateSW = true;

                        }

                    }

                    if (age == "11")
                    {
                        //ManageMasterDataModelCPH.txtChangeTime6 = Convert.ToString(cphList[i].ChTime);
                        //ManageMasterDataModelCPH.txtChangeUser6 = Convert.ToString(cphList[i].ChangeUser);

                        if (Convert.ToString(cphList[i].LimitAmount) != ManageMasterDataModelCPH.txtLimitAmtAdd6)
                        {
                            List<CphEqpLimit> cphEquList = mmdc.UpdateCPHApprovalDetails(cphEqp.EqSize, cphEqp.ModeCode, age, ManageMasterDataModelCPH.txtLimitAmtAdd6, UserLogin).ToList();
                            bUpdateSW = true;

                        }

                    }


                    if (age == "12" || age == "13")
                    {
                        //ManageMasterDataModelCPH.txtChangeTime7 = Convert.ToString(cphList[i].ChTime);
                        //ManageMasterDataModelCPH.txtChangeUser7 = Convert.ToString(cphList[i].ChangeUser);

                        if (Convert.ToString(cphList[i].LimitAmount) != ManageMasterDataModelCPH.txtLimitAmtAdd7)
                        {
                            List<CphEqpLimit> cphEquList = mmdc.UpdateCPHApprovalDetails(cphEqp.EqSize, cphEqp.ModeCode, age, ManageMasterDataModelCPH.txtLimitAmtAdd7, UserLogin).ToList();
                            bUpdateSW = true;

                        }

                    }

                    ////  If Age is invalid then  "Invalid Case Age From " ////


                }
            }



            if (bUpdateSW == true)
            {
                //ManageMasterDataModelCPH.strMsg = " Approval Limits " + cphEqp.EqSize + "/" + cphEqp.ModeCode + " Updated. ";
                string ErrMsg = " Approval Limits " + cphEqp.EqSize + "/" + cphEqp.ModeCode + " Updated. ";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrMsg, "Success");
                ManageMasterDataCPHModel.IsAdd = true;
                ManageMasterDataCPHModel.IsFlag = false;
                ManageMasterDataCPHModel.IsSubmit = false;

            }
            else
            {

                //ManageMasterDataModelCPH.strMsg = "No Information Has Changed";
                string ErrMsg = "No information has changed";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrMsg, "Warning");
                ManageMasterDataCPHModel.IsAdd = false;
                ManageMasterDataCPHModel.IsFlag = false;
                ManageMasterDataCPHModel.IsSubmit = false;

            }

            List<SelectListItem> EqSize = new List<SelectListItem>();
            List<SelectListItem> ModeList = new List<SelectListItem>();

            EqSize = PopulateEqSizeDropDown();
            ModeList = PopulateModeListDropDown();
            ManageMasterDataModelCPH.drpEqSize = EqSize;
            ManageMasterDataModelCPH.drpMode_List = ModeList;

            TempData["SubmitMsg"] = Convert.ToString(ManageMasterDataModelCPH.strMsg);

            //   ManageMasterDataModelCPH.UsrId = ((List<MercPlusClient.ManageUserServiceReference.UserInfo>)System.Web.HttpContext.Current.Session["UserSec"])[0].UserId.ToString();
            //   ManageMasterDataModelCPH.UsrRole = System.Web.HttpContext.Current.Session["UserType"].ToString();  


            ManageMasterDataModelCPH.UsrRole = ((UserSec)Session["UserSec"]).UserType;

            // Re-Populating Values

            //     ManageMasterDataModel.Eq_Size = Convert.ToString(frm["Eq_Size"]);
            //     ManageMasterDataModel.Mode_List = Convert.ToString(frm["Mode_List"]);

            ManageMasterDataModelCPH.txtLimitAmtAdd1 = "";
            ManageMasterDataModelCPH.txtChangeUser1 = "";
            ManageMasterDataModelCPH.txtChangeTime1 = string.Empty;
            ManageMasterDataModelCPH.txtLimitAmtAdd2 = "";
            ManageMasterDataModelCPH.txtChangeUser2 = "";
            ManageMasterDataModelCPH.txtChangeTime2 = "";
            ManageMasterDataModelCPH.txtLimitAmtAdd3 = "";
            ManageMasterDataModelCPH.txtChangeUser3 = "";
            ManageMasterDataModelCPH.txtChangeTime3 = "";
            ManageMasterDataModelCPH.txtLimitAmtAdd4 = "";
            ManageMasterDataModelCPH.txtChangeUser4 = "";
            ManageMasterDataModelCPH.txtChangeTime4 = "";
            ManageMasterDataModelCPH.txtLimitAmtAdd5 = "";
            ManageMasterDataModelCPH.txtChangeUser5 = "";
            ManageMasterDataModelCPH.txtChangeTime5 = "";
            ManageMasterDataModelCPH.txtLimitAmtAdd6 = "";
            ManageMasterDataModelCPH.txtChangeUser6 = "";
            ManageMasterDataModelCPH.txtChangeTime6 = "";
            ManageMasterDataModelCPH.txtLimitAmtAdd7 = "";
            ManageMasterDataModelCPH.txtChangeUser7 = "";
            ManageMasterDataModelCPH.txtChangeTime7 = "";

            ManageMasterDataCPHModel.Header = false;

            return RedirectToAction("GetAllDetailsForCPHApproval", ManageMasterDataModelCPH);
        }

        #endregion

        #endregion


        #region EqType/Mode Entry

        public string EquipmentId = string.Empty;

        public ActionResult SubmitEqTypeEntry(string EQ_ID, string getEqType, string getSubType, string getSize, string getAluminium, string getMode)
        {

            Session["DisplayMsg"] = "";
            Session["Msg"] = "";

            if (string.IsNullOrEmpty(EQ_ID))
            {
                ManageEqtypeModeEntry.IsAdd = true;


                // Populate Sub Type Drop Down  //

                List<EqsType> EqTypeList = null;

                EqTypeList = mmdc.GetSubType().ToList();
                List<SelectListItem> SubType = new List<SelectListItem>();
                int count = 1;

                foreach (var code in EqTypeList)
                {
                    SubType.Add(new SelectListItem
                    {
                        Text = code.EqSType,
                        Value = code.EqSType
                    });
                    count++;
                }

                ManageEqtypeModeEntry.ddlSubType = SubType;

                // Populate Equipment Type  //


                EqTypeList = null;
                EqTypeList = mmdc.GetEqType().ToList();
                List<SelectListItem> EquipmentType = new List<SelectListItem>();

                //EquipmentType.Add(new SelectListItem
                //{
                //    Text = "",
                //    Value = ""
                //});

                int counts = 1;
                foreach (var eqTyp in EqTypeList)
                {
                    EquipmentType.Add(new SelectListItem
                    {
                        Text = eqTyp.CoType,
                        Value = eqTyp.CoType
                    });
                    counts++;
                }

                ManageEqtypeModeEntry.ddlEqType = EquipmentType;

                ////// Populate Size Dropdown


                List<SelectListItem> Size = new List<SelectListItem>();
                Size = PopulateSizeDropDown();
                ManageEqtypeModeEntry.ddlSize = PopulateSizeDropDown();

                ////// Populate Aluminium Dropdown

                List<SelectListItem> Aluminium = new List<SelectListItem>();
                Aluminium = PopulateSizeDropDown();
                ManageEqtypeModeEntry.ddlAluminum = PopulateAluminiumDropDown();

                ////// Populate Mode Dropdown

                List<Mode> ModeList = null;

                ModeList = mmdc.GetRSAllModes().ToList();
                List<SelectListItem> Mode = new List<SelectListItem>();
                count = 1;

                Mode.Add(new SelectListItem
                {
                    Text = "",
                    Value = "",

                });


                foreach (var code in ModeList)
                {
                    Mode.Add(new SelectListItem
                    {
                        Text = code.ModeCode + "--" + code.ModeDescription,
                        Value = code.ModeCode
                    });
                    count++;
                }

                ManageEqtypeModeEntry.ddlMode = Mode;


            }
            else
            {


                ManageEqtypeModeEntry.IsAdd = false;

                if (!string.IsNullOrEmpty(getEqType))
                {

                    List<SelectListItem> EquipmentType = new List<SelectListItem>();
                    EquipmentType.Add(new SelectListItem
                    {
                        Text = getEqType,
                        Value = getEqType,
                        Selected = true
                    });

                    ManageEqtypeModeEntry.ddlEqType = EquipmentType;

                }

                if (!string.IsNullOrEmpty(getSubType))
                {
                    List<SelectListItem> SubType = new List<SelectListItem>();
                    SubType.Add(new SelectListItem
                    {
                        Text = getSubType,
                        Value = getSubType,
                        Selected = true
                    });

                    ManageEqtypeModeEntry.ddlSubType = SubType;
                }


                if (!string.IsNullOrEmpty(getSize))
                {
                    List<SelectListItem> Size = new List<SelectListItem>();
                    Size.Add(new SelectListItem
                    {
                        Text = getSize,
                        Value = getSize,
                        Selected = true
                    });

                    ManageEqtypeModeEntry.ddlSize = Size;
                }


                if (!string.IsNullOrEmpty(getAluminium))
                {
                    List<SelectListItem> Aluminium = new List<SelectListItem>();

                    int result = getAluminium.IndexOf("N", 0, 1);

                    if (result == 0)
                    {
                        Aluminium.Add(new SelectListItem
                        {
                            Text = "N",
                            Value = "N",
                            Selected = true
                        });
                    }

                    if (result == -1)
                    {
                        Aluminium.Add(new SelectListItem
                        {
                            Text = "Y",
                            Value = "Y",
                            Selected = true
                        });
                    }
                    ManageEqtypeModeEntry.ddlAluminum = Aluminium;
                }

                ////// Populate Mode Dropdown

                List<Mode> ModeList = null;

                ModeList = mmdc.GetRSAllModes().ToList();
                List<SelectListItem> Mode = new List<SelectListItem>();
                int count = 1;

                Mode.Add(new SelectListItem
                {
                    Text = "",
                    Value = "",

                });


                foreach (var code in ModeList)
                {
                    Mode.Add(new SelectListItem
                    {
                        Text = code.ModeCode + "--" + code.ModeDescription,
                        Value = code.ModeCode
                    });
                    count++;
                }

                ManageEqtypeModeEntry.ddlMode = Mode;
                ManageEqtypeModeEntry.txtMode = getMode;

                /// Change DateTime
                /// 

                List<EqMode> objEqMode = mmdc.GetRSByEqMode(EQ_ID).ToList();
                string userName = mmdc.GetUserName(objEqMode[0].ChangeUser);
                if (userName != null)
                {
                    if (userName.Contains('|'))
                    {
                        ManageEqtypeModeEntry.txtChangeUserName = userName.Split('|')[0] + " " + userName.Split('|')[1];

                    }
                    else if (userName == "0")
                    {
                        ManageEqtypeModeEntry.txtChangeUserName = "";
                    }
                    else
                    {
                        ManageEqtypeModeEntry.txtChangeUserName = userName;
                    }
                }
                else
                {
                    ManageEqtypeModeEntry.txtChangeUserName = "";
                }
                //ManageEqtypeModeEntry.txtChangeUserName = objEqMode[0].ChangeUser;


                //string date = Convert.ToString(objEqMode[0].ChangeTime);
                //DateTime dt = DateTime.Parse(date);
                //string year = ((objEqMode[0].ChangeTime).ToString("u")).Substring(0, 10);
                //string time = String.Format("{0:T}", dt);
                //string datetime = year + " " + time;
                //ManageEqtypeModeEntry.txtChangedTime = datetime;

                //// Change UserName
                ManageEqtypeModeEntry.txtChangedTime = (objEqMode[0].ChangeTime).ToString("yyyy-MM-dd hh:mm:ss tt");

                ManageEqtypeModeEntry.txtEquipmentId = EQ_ID;

            }

            ManageEqtypeModeEntry.txtEqType = getEqType;
            ManageEqtypeModeEntry.txtSubType = getSubType;
            ManageEqtypeModeEntry.txtSize = getSize;
            if (getAluminium != null)
            {
                if (getAluminium.Trim() == "ALUMINIUM")
                    ManageEqtypeModeEntry.txtAluminum = "Y";
                if (getAluminium.Trim() == "NON-ALUMINIUM")
                    ManageEqtypeModeEntry.txtAluminum = "N";
            }

            ManageEqtypeModeEntry.txtMode = getMode;




            return View("SubmitEqTypeEntry", ManageEqtypeModeEntry);
        }

        [HttpPost]
        public ActionResult InsertEqTypeEntry(FormCollection frm)
        {
            string errorMessage = string.Empty;
            string strEqType = Convert.ToString(frm["txtEqType"]);
            string strSubType = Convert.ToString(frm["txtSubType"]);
            string strSize = Convert.ToString(frm["txtSize"]);
            string strAluminium = Convert.ToString(frm["txtAluminum"]);
            string strMode = Convert.ToString(frm["txtMode"]);
            //    string strChangeUserName = ((List<MercPlusClient.ManageUserServiceReference.UserInfo>)System.Web.HttpContext.Current.Session["UserSec"])[0].UserId.ToString();

            string strChangeUserName = ((UserSec)Session["UserSec"]).LoginId;
            string strChangeTime = Convert.ToString(frm["txtChangedTime"]);

            //Session["DisplayMsg"] = string.Empty;
            //Session["Msg"] = string.Empty;
            ///// Insert into database

            bool RecordExistsFlag = true;


            List<EqMode> EqModeList = new List<EqMode>();
            EqModeList = mmdc.GetRSByAltKey(strEqType, strSubType, strSize, strMode, strAluminium).ToList();
            if (EqModeList.Count == 0)
            {
                RecordExistsFlag = false;
            }
            else
            {
                //Session["DisplayMsg"] = "Association " + strEqType + "/" + strSubType + "/" + strMode + ". Already Exists.";
                //Session["Msg"] = "True";

                errorMessage = "Association " + strEqType + " / " + strSubType + " / " + strMode + " already exists - Not added.";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(errorMessage, "Warning");

            }

            if (RecordExistsFlag == false)
            {
                bool IsSucess = mmdc.CreateEqTypeModeEntry(strEqType, strSubType, strSize, strMode, strAluminium, strChangeUserName);
                //Session["DisplayMsg"] = "Association " + strEqType + "/" + strSubType + "/" + strMode + " Added.";
                //Session["Msg"] = "True";

                errorMessage = "Association " + strEqType + " / " + strSubType + " / " + strMode + " added.";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(errorMessage, "Success");
            }
            // Populate Sub Type Drop Down  //

            List<EqsType> EqSubTypeList = null;

            EqSubTypeList = mmdc.GetSubType().ToList();
            List<SelectListItem> SubType = new List<SelectListItem>();
            int count = 1;
            foreach (var code in EqSubTypeList)
            {
                SubType.Add(new SelectListItem
                {
                    Text = code.EqSType,
                    Value = code.EqSType
                });
                count++;
            }

            ManageEqtypeModeEntry.ddlSubType = SubType;

            // Populate Equipment Type  //


            EqSubTypeList = null;
            EqSubTypeList = mmdc.GetEqType().ToList();
            List<SelectListItem> EquipmentType = new List<SelectListItem>();

            EquipmentType.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });


            int counts = 1;
            foreach (var eqTyp in EqSubTypeList)
            {
                EquipmentType.Add(new SelectListItem
                {
                    Text = eqTyp.CoType,
                    Value = eqTyp.CoType
                });
                counts++;
            }

            ManageEqtypeModeEntry.ddlEqType = EquipmentType;

            ////// Populate Size Dropdown


            List<SelectListItem> Size = new List<SelectListItem>();
            Size = PopulateSizeDropDown();
            ManageEqtypeModeEntry.ddlSize = PopulateSizeDropDown();

            ////// Populate Aluminium Dropdown

            List<SelectListItem> Aluminium = new List<SelectListItem>();
            Aluminium = PopulateSizeDropDown();
            ManageEqtypeModeEntry.ddlAluminum = PopulateAluminiumDropDown();

            ////// Populate Mode Dropdown

            List<Mode> ModeList = null;

            ModeList = mmdc.GetRSAllModes().ToList();
            List<SelectListItem> Mode = new List<SelectListItem>();
            count = 1;

            Mode.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });


            foreach (var code in ModeList)
            {
                Mode.Add(new SelectListItem
                {
                    Text = code.ModeCode + '-' + code.ModeDescription,
                    Value = code.ModeCode
                });
                count++;
            }

            ManageEqtypeModeEntry.ddlMode = Mode;

            ManageEqtypeModeEntry.txtEqType = strEqType;
            ManageEqtypeModeEntry.txtSubType = strSubType;
            ManageEqtypeModeEntry.txtSize = strSize;
            ManageEqtypeModeEntry.txtAluminum = strAluminium;
            ManageEqtypeModeEntry.txtMode = strMode;


            ManageEqtypeModeEntry.IsAdd = true;
            return View("SubmitEqTypeEntry", ManageEqtypeModeEntry);
        }

        [HttpPost]
        public ActionResult EditEqTypeEntry(FormCollection frm)
        {


            // GET EQUIPMENT ID //

            string EquipmentId = string.Empty;
            EquipmentId = Convert.ToString(frm["txtEquipmentId"]);

            /////////////////////

            string strEqType = Convert.ToString(frm["txtEqType"]);
            string strSubType = Convert.ToString(frm["txtSubType"]);
            string strSize = Convert.ToString(frm["txtSize"]);
            string strAluminium = Convert.ToString(frm["txtAluminum"]);
            string strMode = Convert.ToString(frm["txtMode"]);
            //   string strChangeUserName = ((List<MercPlusClient.ManageUserServiceReference.UserInfo>)System.Web.HttpContext.Current.Session["UserSec"])[0].UserId.ToString();

            string strChangeUserName = ((UserSec)Session["UserSec"]).UserType;
            string strChangeTime = Convert.ToString(frm["txtChangedTime"]);
            Session["DisplayMsg"] = string.Empty;
            Session["Msg"] = string.Empty;

            bool IsSuccess = false;
            try
            {
                bool RecordExistsFlag = true;

                List<EqMode> EqModeList = new List<EqMode>();
                EqModeList = mmdc.GetRSByAltKey(strSubType, strEqType, strSize, strMode, strAluminium).ToList();
                if (EqModeList.Count == 0)
                {
                    RecordExistsFlag = false;
                }

                if (RecordExistsFlag == false)
                {
                    //  if (!mmdc.EQCheckDuplicate(strSubType, strEqType, strSize, strMode, strAluminium) && (mmdc.EQCheckDuplicateByType(strSubType, strEqType)) && (mmdc.EQCheckDuplicateByMode(strMode)) && (mmdc.CheckDuplicateEqId(EquipmentId)))
                    //  {

                    IsSuccess = mmdc.UpdateEqTypeModeEntry(EquipmentId, strMode, strChangeUserName);
                    if (IsSuccess == true)
                    {
                        Session["DisplayMsg"] = "Association " + strEqType + " / " + strSubType + " / " + strMode + " updated.";
                        Session["Msg"] = "True";

                    }

                    // }
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            ManageEqtypeModeEntry.IsAdd = false;

            return RedirectToAction("ViewEqTypeModeEntry", new { PageName = "Edit" });



        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SetSubTypeDetails(string EqType)
        {
            List<SelectListItem> ModeItems = new List<SelectListItem>();
            try
            {

                var reader = mmdc.GetSubTypeDetail(EqType).ToList();
                if (reader.Count > 0)
                {
                    foreach (var q in reader)
                    {
                        ModeItems.Add(new SelectListItem { Text = q.EqSType.ToString(), Value = q.EqSType.ToString() });
                    }
                }
                else
                {
                    ModeItems.Add(new SelectListItem { Text = "ALL", Value = "" });
                }


            }
            catch (Exception ex)
            {

            }

            return Json(ModeItems, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetEqtypeModeEntryDetails(FormCollection frm)
        {

            string strEqType = Convert.ToString(frm["EqType"]);
            string strSubType = Convert.ToString(frm["SubType"]).Trim();
            string strSize = Convert.ToString(frm["Size"]);
            string strAluminium = Convert.ToString(frm["Aluminum"]);
            string errorMessage = string.Empty;

            List<EqsType> EqTypeList = null;

            var result = mmdc.GetRSAllEquModes(strEqType, strSubType, strSize, strAluminium).ToList();



            var SearchData = (from e in result
                              select new ManageEqtypeModeEntry
                              {
                                  EQ_EQUIPMENT = e.CoType,
                                  EQ_SUB_TYPE = e.EqsType,
                                  EQ_SIZE = e.EqSize,
                                  EQ_MATERIAL = e.AluminiumSW,
                                  EQ_MODE = e.ModeCode,
                                  EQ_MODE_ID = Convert.ToString(e.EqModeID),
                                  EQ_EDIT = "edit"

                              }).ToList();




            if (result.Count < 1)
            {
                errorMessage = "There were no results meeting your query parameters.";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(errorMessage, "Warning");
            }
            else
            {
                ManageEqtypeModeEntry.SearchData = SearchData;
            }

            // Populate Sub Type Drop Down  //

            EqTypeList = mmdc.GetSubTypeDetail(strEqType).ToList();
            List<SelectListItem> SubType = new List<SelectListItem>();
            int count = 1;




            foreach (var code in EqTypeList)
            {
                SubType.Add(new SelectListItem
                {
                    Text = code.EqSType.Trim(),
                    Value = code.EqSType.Trim()
                });
                count++;
            }

            ManageEqtypeModeEntry.drpSubType = SubType;

            // Populate Equipment Type  //


            EqTypeList = null;
            EqTypeList = mmdc.GetEqType().ToList();
            List<SelectListItem> EquipmentType = new List<SelectListItem>();

            EquipmentType.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            int counts = 1;
            foreach (var eqTyp in EqTypeList)
            {
                EquipmentType.Add(new SelectListItem
                {
                    Text = eqTyp.CoType,
                    Value = eqTyp.CoType
                });
                counts++;
            }

            ManageEqtypeModeEntry.drpEqType = EquipmentType;

            ////// Populate Size Dropdown


            List<SelectListItem> Size = new List<SelectListItem>();
            Size = PopulateSizeDropDown();
            ManageEqtypeModeEntry.drpSize = PopulateSizeDropDown();

            ////// Populate Aluminium Dropdown

            List<SelectListItem> Aluminium = new List<SelectListItem>();
            Aluminium = PopulateSizeDropDown();
            ManageEqtypeModeEntry.drpAluminum = PopulateAluminiumDropDown();

            ManageEqtypeModeEntry.EqType = strEqType;
            ManageEqtypeModeEntry.SubType = strSubType;
            ManageEqtypeModeEntry.Size = strSize;
            ManageEqtypeModeEntry.Aluminum = strAluminium;


            return View("ViewEqTypeModeEntry", ManageEqtypeModeEntry);

        }

        public ActionResult ViewEqtypeModeEntry(string PageName)
        {
            if (string.IsNullOrEmpty(PageName))
            {
                Session["Msg"] = "";
                Session["DisplayMsg"] = "";
            }
            else
            {

                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(Convert.ToString(Session["DisplayMsg"]), "Success");
            }
            // Populate Sub Type Drop Down  //
            List<EqsType> EqTypeList = null;
            EqTypeList = mmdc.GetSubType().ToList();


            List<SelectListItem> SubType = new List<SelectListItem>();
            int count = 1;


            foreach (var code in EqTypeList)
            {
                SubType.Add(new SelectListItem
                {
                    Text = code.EqSType,
                    Value = code.EqSType
                });
                count++;
            }

            ManageEqtypeModeEntry.drpSubType = SubType;

            // Populate Equipment Type  //


            EqTypeList = null;
            EqTypeList = mmdc.GetEqType().ToList();
            List<SelectListItem> EquipmentType = new List<SelectListItem>();

            //EquipmentType.Add(new SelectListItem
            //{
            //    Text = "",
            //    Value = ""
            //});

            int counts = 1;
            foreach (var eqTyp in EqTypeList)
            {
                EquipmentType.Add(new SelectListItem
                {
                    Text = eqTyp.CoType,
                    Value = eqTyp.CoType
                });
                counts++;
            }

            ManageEqtypeModeEntry.drpEqType = EquipmentType;

            ////// Populate Size Dropdown


            List<SelectListItem> Size = new List<SelectListItem>();
            Size = PopulateSizeDropDown();
            ManageEqtypeModeEntry.drpSize = PopulateSizeDropDown();

            ////// Populate Aluminium Dropdown

            List<SelectListItem> Aluminium = new List<SelectListItem>();
            Aluminium = PopulateSizeDropDown();
            ManageEqtypeModeEntry.drpAluminum = PopulateAluminiumDropDown();
            return View("ViewEqTypeModeEntry", ManageEqtypeModeEntry);

        }

        private List<SelectListItem> PopulateSizeDropDown()
        {
            List<SelectListItem> EqSizeList = new List<SelectListItem>();
            EqSizeList.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            EqSizeList.Add(new SelectListItem
            {
                Text = "20",
                Value = "20"
            });
            EqSizeList.Add(new SelectListItem
            {
                Text = "40",
                Value = "40"
            });
            EqSizeList.Add(new SelectListItem
            {
                Text = "45",
                Value = "45"
            });
            EqSizeList.Add(new SelectListItem
            {
                Text = "48",
                Value = "48"
            });

            return EqSizeList;


        }

        private List<SelectListItem> PopulateAluminiumDropDown()
        {
            List<SelectListItem> EqSizeList = new List<SelectListItem>();
            EqSizeList.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            EqSizeList.Add(new SelectListItem
            {
                Text = "Y",
                Value = "Y"
            });
            EqSizeList.Add(new SelectListItem
            {
                Text = "N",
                Value = "N"
            });

            return EqSizeList;


        }

        #endregion


        #region Customer Shop Mode

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SetShopCodeDetails(string Cust)
        {
            List<SelectListItem> ModeItems = new List<SelectListItem>();
            try
            {

                var ShopList = mmdc.GetShopCodeByCustomer(Cust).ToList();

                if (ShopList.Count > 0)
                {
                    foreach (var q in ShopList)
                    {
                        ModeItems.Add(new SelectListItem
                        {
                            Text = q.ShopCode + "-" + q.ShopDescription,
                            Value = q.ShopCode
                        });
                    }
                }
                else
                {
                    ModeItems.Add(new SelectListItem { Text = "ALL", Value = "" });
                }


            }
            catch (Exception ex)
            {

            }

            return Json(ModeItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewCustShopModeDetails(FormCollection collection, string CSM_CD, string PageName)
        {

            try
            {

                bool IsDelete = false;
                string CO_CSM_CD = CSM_CD;


                if (string.IsNullOrEmpty(PageName))
                {
                    //Session["DisplayMsg"] = "";
                    //Session["Msg"] = "";    
                }

                if (!string.IsNullOrEmpty(CO_CSM_CD))
                {
                    List<CustShopMode> objCustShopMode = mmdc.GetRsByCSM(CO_CSM_CD).ToList();

                    if (objCustShopMode.Count > 0)
                    {

                        IsDelete = mmdc.DeleteCsmCode(CO_CSM_CD);
                        //    ManageCustomerShopMode = PopulateAllCSMDropDowns();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CO_CSM_CD))
                        {
                            //    ManageCustomerShopMode = PopulateAllCSMDropDowns();
                            string Message = "Customer/SHop/Mode Data is Unavailable for Delete";
                            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(Message, "Success");
                        }
                    }


                }
                else
                {

                    if (PageName == "Edit")
                    { }
                    else
                    {

                        Session["DisplayMsg"] = string.Empty;
                        Session["Msg"] = string.Empty;
                    }

                }


                //      ManageCustomerShopMode = PopulateAllCSMDropDowns();


                ManageCustomerShopMode.ShopList = PopulateShopDropDown();
                ManageCustomerShopMode.ModeList = PopulateModeDropDown();
                ManageCustomerShopMode.CustomerList = PopulateCustomerDropDown();


            }
            catch (Exception ex)
            {


            }
            ManageCustomerShopMode.Flag = true;
            return View("ViewCustShopModeDetails", ManageCustomerShopMode);

        }


        public ActionResult DeleteCustShopModeDetails(string CSM_CD)
        {
            try
            {

                bool IsDelete = false;
                string CO_CSM_CD = CSM_CD;



                Session["DisplayMsg"] = "";
                Session["Msg"] = "";


                if (!string.IsNullOrEmpty(CO_CSM_CD))
                {
                    List<CustShopMode> objCustShopMode = mmdc.GetRsByCSM(CO_CSM_CD).ToList();

                    if (objCustShopMode.Count > 0)
                    {

                        IsDelete = mmdc.DeleteCsmCode(CO_CSM_CD);
                        if (IsDelete)
                        {
                            string Message = "Association " + objCustShopMode[0].CustomerCode + " / " + objCustShopMode[0].ShopCode + " / " + objCustShopMode[0].ModeCode + " Deleted.";
                            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(Message, "Success");
                        }
                        else
                        {
                            string Message = "Association " + objCustShopMode[0].CustomerCode + " / " + objCustShopMode[0].ShopCode + " / " + objCustShopMode[0].ModeCode + " is unavailable for Delete";
                            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(Message, "Warning");
                        }
                        //    ManageCustomerShopMode = PopulateAllCSMDropDowns();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CO_CSM_CD))
                        {
                            //    ManageCustomerShopMode = PopulateAllCSMDropDowns();
                            string Message = "Customer/SHop/Mode Data is Unavailable for Delete";
                            TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(Message, "Warning");
                        }
                    }


                }
                else
                {

                }


                //      ManageCustomerShopMode = PopulateAllCSMDropDowns();


                ManageCustomerShopMode.ShopList = PopulateShopDropDown();
                ManageCustomerShopMode.ModeList = PopulateModeDropDown();
                ManageCustomerShopMode.CustomerList = PopulateCustomerDropDown();


            }
            catch (Exception ex)
            {


            }
            ManageCustomerShopMode.Flag = true;
            return View("ViewCustShopModeDetails", ManageCustomerShopMode);

        }

        public ActionResult GetCustShopModeList(ManageCustomerShopMode ManageCustomerShopMode)
        {
            List<ManageCustomerShopMode> manageCSM = null;
            var SearchData = manageCSM;
            string CustomerCode = ManageCustomerShopMode.lstCustomerList;
            string ShopCode = ManageCustomerShopMode.lstShopList;
            string Mode = ManageCustomerShopMode.lstModeList;
            //Session["DisplayMsg"] = "";
            //Session["Msg"] = "";    

            try
            {
                string errorMessage = string.Empty;
                var CSMList = mmdc.GetCSMList(CustomerCode, ShopCode, Mode).ToList();
                SearchData = (from e in CSMList
                              select new ManageCustomerShopMode
                              {
                                  COSHOP = e.ShopCode.ToString(),
                                  COCUSTOMER = e.CustomerCode,
                                  COMODE = e.ModeCode,
                                  COPAYAGENT = e.PayAgentCode,
                                  COCORPPAYAGENT = e.CorpPayAgentCode,
                                  CORRIS = e.RRISFormat,
                                  COCPC = e.SubProfitCenter,
                                  COPROFITCENTER = e.ProfitCenter,
                                  COEXPCODE = e.AccountCode,
                                  CO_CSM_CD = e.CSMCode


                              }).ToList();

                if (SearchData.Count > 0)
                {

                    ManageCustomerShopMode = PopulateAllCSMDropDowns();
                    ManageCustomerShopMode.lstCustomerList = CustomerCode;
                    ManageCustomerShopMode.lstShopList = ShopCode;
                    ManageCustomerShopMode.lstModeList = Mode;
                    ManageCustomerShopMode.SearchData = SearchData;
                }
                else
                {

                    ManageCustomerShopMode = PopulateAllCSMDropDowns();
                    ManageCustomerShopMode.lstCustomerList = CustomerCode;
                    ManageCustomerShopMode.lstShopList = ShopCode;
                    ManageCustomerShopMode.lstModeList = Mode;
                    //   ManageCustomerShopMode.SearchData = SearchData;

                    errorMessage = "There were no results meeting your query parameters.";
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(errorMessage, "Warning");

                }
                ManageCustomerShopMode.IsAdd = true;
                ManageCustomerShopMode.Flag = true;
            }
            catch (Exception ex)
            {

            }

            //Logger.Write("Review Estimate -- Search for Work Order End.");

            return View("ViewCustShopModeDetails", ManageCustomerShopMode);
        }

        public ManageCustomerShopMode PopulateAllCSMDropDowns()
        {
            ManageCustomerShopMode ManageCustomerShopMode = new ManageCustomerShopMode();
            ManageMasterDataServiceReference.ManageMasterDataClient obj_Service = new ManageMasterDataServiceReference.ManageMasterDataClient();
            string errorMessage = string.Empty;

            try
            {

                #region ShopDropDown


                var Shop = mmdc.GetShopCode().ToList();

                ManageCustomerShopMode.ShopList = (from d in Shop
                                                   select new SelectListItem
                                                   {
                                                       Value = d.ShopCode.ToString(),
                                                       Text = d.ShopCode.ToString()
                                                   }).ToList();


                if (ManageCustomerShopMode.ShopList.Count == 0)
                {
                    errorMessage = "Customer Shop Data is unavailable.";
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(errorMessage, "Warning");
                }

                #endregion ShopDropDown

                #region ModeDropDown


                var Mode = obj_Service.GetModeList().ToList();

                ManageCustomerShopMode.ModeList = (from d in Mode
                                                   select new SelectListItem
                                                   {
                                                       Value = d.ModeCode.ToString(),
                                                       Text = d.ModeCode.ToString() + "-" + d.ModeDescription
                                                   }).ToList();

                ManageCustomerShopMode.ModeList.OrderBy(li => li.Text);
                ManageCustomerShopMode.ModeList.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = ""

                });

                if (ManageCustomerShopMode.ModeList.Count == 0)
                {
                    errorMessage = "Mode Data is unavailable.";
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(errorMessage, "Warning");
                }


                #endregion ModeDropDown

                #region CustomerDropDown

                var Cust = mmdc.GetCustomerCode().ToList();

                ManageCustomerShopMode.CustomerList = (from d in Cust
                                                       select new SelectListItem
                                                       {
                                                           Value = d.CustomerCode.ToString(),
                                                           Text = d.CustomerCode + "-" + d.CustomerDesc
                                                       }).ToList();

                ManageCustomerShopMode.CustomerList.OrderBy(li => li.Text);

                if (ManageCustomerShopMode.CustomerList.Count == 0)
                {
                    errorMessage = "Customer Data is unavailable.";
                    TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(errorMessage, "Warning");
                }


                #endregion CustomerDropDown
            }
            catch (Exception ex)
            {
            }

            return ManageCustomerShopMode;

        }

        public List<SelectListItem> PopulateCustomerDropDown()
        {
            ManageCustomerShopMode ManageCustomerShopMode = new ManageCustomerShopMode();
            ManageMasterDataServiceReference.ManageMasterDataClient obj_Service = new ManageMasterDataServiceReference.ManageMasterDataClient();
            string errorMessage = string.Empty;
            List<SelectListItem> CustomerList = new List<SelectListItem>();

            try
            {
                var Cust = mmdc.GetCustomerCode().ToList();
                foreach (var d in Cust)
                {
                    CustomerList.Add(new SelectListItem
                    {
                        Text = d.CustomerCode,
                        Value = d.CustomerCode
                    });
                }

                CustomerList.OrderBy(li => li.Text);

            }
            catch (Exception ex)
            {


            }

            return CustomerList;

        }

        public List<SelectListItem> PopulateShopDropDown()
        {
            ManageCustomerShopMode ManageCustomerShopMode = new ManageCustomerShopMode();
            ManageMasterDataServiceReference.ManageMasterDataClient obj_Service = new ManageMasterDataServiceReference.ManageMasterDataClient();
            string errorMessage = string.Empty;
            List<SelectListItem> ShopList = new List<SelectListItem>();

            try
            {
                var Shop = mmdc.GetShopCode().ToList();

                if (Shop.Count > 0)
                {
                    foreach (var q in Shop)
                    {
                        ShopList.Add(new SelectListItem
                        {
                            Text = q.ShopCode + "-" + q.ShopDescription,
                            Value = q.ShopCode
                        });
                    }
                }
                else
                {
                    //ModeItems.Add(new SelectListItem { Text = "ALL", Value = "" });
                }

            }
            catch (Exception ex)
            {


            }

            return ShopList;

        }

        public List<SelectListItem> PopulateModeDropDown()
        {
            ManageCustomerShopMode ManageCustomerShopMode = new ManageCustomerShopMode();
            ManageMasterDataServiceReference.ManageMasterDataClient obj_Service = new ManageMasterDataServiceReference.ManageMasterDataClient();
            string errorMessage = string.Empty;
            List<SelectListItem> ModeList = new List<SelectListItem>();

            try
            {

                var Mode = obj_Service.GetModeList().ToList();
                foreach (var d in Mode)
                {
                    ModeList.Add(new SelectListItem
                    {
                        Text = d.ModeCode + "-" + d.ModeDescription,
                        Value = d.ModeCode
                    });
                }
            }
            catch (Exception ex)
            {


            }

            return ModeList;

        }

        private List<SelectListItem> PopulatePayAgent()
        {


            ManageCustomerShopMode ManageCustomerShopMode = new ManageCustomerShopMode();
            ManageMasterDataServiceReference.ManageMasterDataClient obj_Service = new ManageMasterDataServiceReference.ManageMasterDataClient();
            List<SelectListItem> PayAgentList = new List<SelectListItem>();

            try
            {
                #region PayAgent DropDown


                var PayAgent = mmdc.GetAllPayAgents().ToList();
                foreach (var d in PayAgent)
                {
                    PayAgentList.Add(new SelectListItem
                    {
                        Text = d.PayAgentCode,
                        Value = d.PayAgentCode
                    });
                }
                #endregion PayAgent DropDown

            }
            catch (Exception ex)
            {
                throw;
            }

            return PayAgentList;


        }

        private List<SelectListItem> PopulateRRISDropDown()
        {
            List<SelectListItem> RRISList = new List<SelectListItem>();
            RRISList.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            RRISList.Add(new SelectListItem
            {
                Text = "RRIS52 - Maersk Line Books ",
                Value = "52"
            });
            RRISList.Add(new SelectListItem
            {
                Text = "RRIS70 - Local Books",
                Value = "70"
            });
            RRISList.Add(new SelectListItem
            {
                Text = "RRIT72 - Interoffice Books",
                Value = "72"
            });

            return RRISList;


        }

        private List<SelectListItem> PopulateCustShopDropDown(List<Shop> ShopCodeList = null)
        {
            ManageMasterDataModel managemasterdatamodel = new ManageMasterDataModel();
            int userId = ((UserSec)Session["UserSec"]).UserId;
            ShopCodeList = mmdc.GetShopCodeByUserId(((UserSec)Session["UserSec"]).UserId).ToList();
            List<SelectListItem> ShopCode = new List<SelectListItem>();

            int count = 1;
            foreach (var code in ShopCodeList)
            {
                ShopCode.Add(new SelectListItem
                {
                    Text = code.ShopCode + "-" + code.ShopDescription,
                    Value = code.ShopCode
                });

                count++;
            }

            return ShopCode;

        }


        public ActionResult CustShopMode(string CSM_CD)
        {
            ManageCustomerShopMode.Flag = false;
            string CsmCode = CSM_CD;
            List<SelectListItem> CustomerList = new List<SelectListItem>();
            List<SelectListItem> ShopList = new List<SelectListItem>();
            List<SelectListItem> ModeList = new List<SelectListItem>();
            if (string.IsNullOrEmpty(CsmCode))
            {


                ManageCustomerShopMode.ddlPayAgent = PopulatePayAgent();
                ManageCustomerShopMode.ddlRRIS = PopulateRRISDropDown();
                ManageCustomerShopMode.IsAdd = false;
                //ManageCustomerShopMode.IsView = true;
                //   ManageCustomerShopMode = PopulateAllCSMDropDowns();


                ManageCustomerShopMode.ShopList = PopulateCustShopDropDown();
                ManageCustomerShopMode.ModeList = PopulateModeDropDown();
                ManageCustomerShopMode.CustomerList = PopulateCustomerDropDown();
            }
            else
            {

                //       ManageCustomerShopMode = PopulateAllCSMDropDowns();
                ManageCustomerShopMode.IsView = true;
                ManageCustomerShopMode.IsAdd = true;
                ManageCustomerShopMode.ddlPayAgent = PopulatePayAgent();
                ManageCustomerShopMode.ddlRRIS = PopulateRRISDropDown();

                List<CustShopMode> objCustShop = mmdc.GetRsByCSM(CsmCode).ToList();
                if (objCustShop[0].CustomerCode != null)
                {
                    List<SelectListItem> CustList = new List<SelectListItem>();
                    CustList.Add(new SelectListItem
                    {
                        Text = objCustShop[0].CustomerCode,
                        Value = objCustShop[0].CustomerCode,
                        Selected = true
                    });

                    ManageCustomerShopMode.CustomerList = CustList;
                }
                if (objCustShop[0].ShopCode != null)
                {
                    List<SelectListItem> ShopList1 = new List<SelectListItem>();
                    ShopList1.Add(new SelectListItem
                    {
                        Text = objCustShop[0].ShopCode,
                        Value = objCustShop[0].ShopCode,
                        Selected = true
                    });

                    ManageCustomerShopMode.ShopList = ShopList1;
                }
                if (objCustShop[0].ModeCode != null)
                {
                    List<SelectListItem> ModeList1 = new List<SelectListItem>();
                    ModeList1.Add(new SelectListItem
                    {
                        Text = objCustShop[0].ModeCode,
                        Value = objCustShop[0].ModeCode,
                        Selected = true
                    });
                    ManageCustomerShopMode.ModeList = ModeList1;
                }
                if (objCustShop[0].PayAgentCode != null)
                {
                    ManageCustomerShopMode.txtPayAgent = objCustShop[0].PayAgentCode;
                }
                if (objCustShop[0].ProfitCenter != null)
                {
                    ManageCustomerShopMode.txtCPC = objCustShop[0].ProfitCenter.Trim();
                }
                if (objCustShop[0].AccountCode != null)
                {
                    ManageCustomerShopMode.txtExpenseCode = objCustShop[0].AccountCode.Trim();
                }
                if (objCustShop[0].RRISFormat != null)
                {
                    ManageCustomerShopMode.txtRRIS = objCustShop[0].RRISFormat;
                }
                if (objCustShop[0].CorpPayAgentCode != null)
                {
                    ManageCustomerShopMode.txtCorpPayagent = objCustShop[0].CorpPayAgentCode;
                }
                if (objCustShop[0].SubProfitCenter != null)
                {
                    ManageCustomerShopMode.txtProfitCenter = objCustShop[0].SubProfitCenter;
                }
                if (objCustShop[0].ChangeUser != null)
                {
                    string userName = mmdc.GetUserName(objCustShop[0].ChangeUser);
                    if (userName != null)
                    {
                        if (userName.Contains('|'))
                        {
                            ManageCustomerShopMode.txtChangedUserName = userName.Split('|')[0] + " " + userName.Split('|')[1];

                        }
                        else
                        {
                            ManageCustomerShopMode.txtChangedUserName = userName;
                        }
                    }
                    else
                    {
                        ManageCustomerShopMode.txtChangedUserName = "";
                    }
                }
                if (objCustShop[0].ChangeTime != null)
                {
                    ManageCustomerShopMode.txtChangeTime = (objCustShop[0].ChangeTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                }


                ManageCustomerShopMode.txtCSMCode = CsmCode;

            }
            // ManageCustomerShopMode.Flag = false;
            return View("CustShopMode", ManageCustomerShopMode);


        }
        public ActionResult UpdateCustShopMode(string CSM_CD)
        {
            ManageCustomerShopMode.Flag = false;
            string CsmCode = CSM_CD;
            List<SelectListItem> CustomerList = new List<SelectListItem>();
            List<SelectListItem> ShopList = new List<SelectListItem>();
            List<SelectListItem> ModeList = new List<SelectListItem>();
            if (string.IsNullOrEmpty(CsmCode))
            {


                ManageCustomerShopMode.ddlPayAgent = PopulatePayAgent();
                ManageCustomerShopMode.ddlRRIS = PopulateRRISDropDown();
                ManageCustomerShopMode.IsAdd = false;
                //ManageCustomerShopMode.IsView = true;
                //   ManageCustomerShopMode = PopulateAllCSMDropDowns();


                ManageCustomerShopMode.ShopList = PopulateShopDropDown();
                ManageCustomerShopMode.ModeList = PopulateModeDropDown();
                ManageCustomerShopMode.CustomerList = PopulateCustomerDropDown();
            }
            else
            {

                //       ManageCustomerShopMode = PopulateAllCSMDropDowns();
                ManageCustomerShopMode.IsView = true;
                ManageCustomerShopMode.IsAdd = true;
                ManageCustomerShopMode.ddlPayAgent = PopulatePayAgent();
                ManageCustomerShopMode.ddlRRIS = PopulateRRISDropDown();

                List<CustShopMode> objCustShop = mmdc.GetRsByCSM(CsmCode).ToList();
                if (objCustShop[0].CustomerCode != null)
                {
                    List<SelectListItem> CustList = new List<SelectListItem>();
                    CustList.Add(new SelectListItem
                    {
                        Text = objCustShop[0].CustomerCode,
                        Value = objCustShop[0].CustomerCode,
                        Selected = true
                    });

                    ManageCustomerShopMode.CustomerList = CustList;
                }
                if (objCustShop[0].ShopCode != null)
                {
                    List<SelectListItem> ShopList1 = new List<SelectListItem>();
                    ShopList1.Add(new SelectListItem
                    {
                        Text = objCustShop[0].ShopCode,
                        Value = objCustShop[0].ShopCode,
                        Selected = true
                    });

                    ManageCustomerShopMode.ShopList = ShopList1;
                }
                if (objCustShop[0].ModeCode != null)
                {
                    List<SelectListItem> ModeList1 = new List<SelectListItem>();
                    ModeList1.Add(new SelectListItem
                    {
                        Text = objCustShop[0].ModeCode,
                        Value = objCustShop[0].ModeCode,
                        Selected = true
                    });
                    ManageCustomerShopMode.ModeList = ModeList1;
                }
                if (objCustShop[0].PayAgentCode != null)
                {
                    ManageCustomerShopMode.txtPayAgent = objCustShop[0].PayAgentCode;
                }
                if (objCustShop[0].ProfitCenter != null)
                {
                    ManageCustomerShopMode.txtCPC = objCustShop[0].ProfitCenter.Trim();
                }
                if (objCustShop[0].AccountCode != null)
                {
                    ManageCustomerShopMode.txtExpenseCode = objCustShop[0].AccountCode.Trim();
                }
                if (objCustShop[0].RRISFormat != null)
                {
                    ManageCustomerShopMode.txtRRIS = objCustShop[0].RRISFormat;
                }
                if (objCustShop[0].CorpPayAgentCode != null)
                {
                    ManageCustomerShopMode.txtCorpPayagent = objCustShop[0].CorpPayAgentCode;
                }
                if (objCustShop[0].SubProfitCenter != null)
                {
                    ManageCustomerShopMode.txtProfitCenter = objCustShop[0].SubProfitCenter;
                }
                if (objCustShop[0].ChangeUser != null)
                {
                    string userName = mmdc.GetUserName(objCustShop[0].ChangeUser);
                    if (userName != null)
                    {
                        if (userName.Contains('|'))
                        {
                            ManageCustomerShopMode.txtChangedUserName = userName.Split('|')[0] + " " + userName.Split('|')[1];

                        }
                        else
                        {
                            ManageCustomerShopMode.txtChangedUserName = userName;
                        }
                    }
                    else
                    {
                        ManageCustomerShopMode.txtChangedUserName = "";
                    }
                }
                if (objCustShop[0].ChangeTime != null)
                {
                    ManageCustomerShopMode.txtChangeTime = (objCustShop[0].ChangeTime).ToString("yyyy-MM-dd hh:mm:ss tt");
                }

                ManageCustomerShopMode.txtCSMCode = CsmCode;

            }
            ManageCustomerShopMode.Flag = false;
            return View("ViewCustShopModeDetails", ManageCustomerShopMode);


        }

        public ActionResult InsertCustShopMode(FormCollection frm)
        {

            ManageCustomerShopMode ViewManageCustomerShopMode = new ManageCustomerShopMode();
            string sCSMCd = Convert.ToString(frm["txtCSMCode"]);
            string sCustomerCd = Convert.ToString(frm["lstCustomerList"]);
            string sShopCd = Convert.ToString(frm["lstShopList"]);
            string sMode = Convert.ToString(frm["lstModeList"]);
            string sPayagentCd = Convert.ToString(frm["txtPayAgent"]);
            string sCorpPayagentCd = Convert.ToString(frm["txtCorpPayagent"]);
            string sRRISFormat = Convert.ToString(frm["txtRRIS"]);
            string sProfitCenter = Convert.ToString(frm["txtCPC"]);
            string sSubProfitCenter = Convert.ToString(frm["txtProfitCenter"]);
            string sAccountCd = Convert.ToString(frm["txtExpenseCode"]);
            string sUser = ((UserSec)Session["UserSec"]).UserType;
            string sUserId = ((UserSec)Session["UserSec"]).LoginId;
            bool res;

            bool RecordExistsFlag = true;
            bool IsValid = false;
            Session["DisplayMsg"] = string.Empty;
            Session["Msg"] = string.Empty;

            var result = mmdc.GetRsByCSM(sCSMCd);
            if (result.Count() < 1)
            {
                RecordExistsFlag = false;
            }

            if (RecordExistsFlag == false)
            {


                if (IsValid == false)
                {
                    sCSMCd = sCustomerCd.Trim() + sShopCd.Trim() + sMode.Trim();
                    bool IsSucess = mmdc.InsertCustShopMode(sCSMCd, sCustomerCd, sShopCd, sMode, sPayagentCd, sCorpPayagentCd, sRRISFormat, sProfitCenter, sSubProfitCenter, sAccountCd, sUser, ref ErrorMessage);
                    if (IsSucess)
                    {
                        ErrorMessage = "Association " + sCustomerCd.Trim() + "/" + sShopCd.Trim() + "/" + sMode.Trim() + " " + " Added.";
                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Success");
                        res = true;
                        string userName = mmdc.GetUserName(sUserId);
                        if (userName != null)
                        {
                            if (userName.Contains('|'))
                            {
                                ViewManageCustomerShopMode.txtChangedUserName = userName.Split('|')[0] + " " + userName.Split('|')[1];

                            }
                            else
                            {
                                ViewManageCustomerShopMode.txtChangedUserName = userName;
                            }
                        }
                        else
                        {
                            ViewManageCustomerShopMode.txtChangedUserName = "";
                        }

                        ViewManageCustomerShopMode.txtChangeTime = (DateTime.Now).ToString("yyyy-MM-dd hh:mm:ss tt");
                    }
                    else
                    {

                        TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(ErrorMessage, "Warning");
                        res = false;
                    }
                    if (res)
                    {
                        ViewManageCustomerShopMode.IsView = true;

                    }
                    ViewManageCustomerShopMode.ddlPayAgent = PopulatePayAgent();
                    ViewManageCustomerShopMode.ddlRRIS = PopulateRRISDropDown();
                    ViewManageCustomerShopMode.ShopList = PopulateShopDropDown();
                    ViewManageCustomerShopMode.ModeList = PopulateModeDropDown();
                    ViewManageCustomerShopMode.CustomerList = PopulateCustomerDropDown();
                    ViewManageCustomerShopMode.ddlRRIS = PopulateRRISDropDown();

                    ViewManageCustomerShopMode.txtCorpPayagent = sCorpPayagentCd;
                    ViewManageCustomerShopMode.txtProfitCenter = sSubProfitCenter;
                    ViewManageCustomerShopMode.txtExpenseCode = sAccountCd;
                    ViewManageCustomerShopMode.txtCPC = sProfitCenter;
                    ViewManageCustomerShopMode.txtPayAgent = sPayagentCd;
                    ViewManageCustomerShopMode.lstCustomerList = sCustomerCd;
                    ViewManageCustomerShopMode.lstModeList = sMode;
                    ViewManageCustomerShopMode.lstShopList = sShopCd;
                    ViewManageCustomerShopMode.txtRRIS = sRRISFormat;


                }
            }


            return View("CustShopMode", ViewManageCustomerShopMode);

        }

        [HttpPost]
        public ActionResult EditCustShopMode(ManageCustomerShopMode ManageCustomerShopMode, FormCollection frm)
        {
            bool IsValid = false;
            string sUser = ((UserSec)Session["UserSec"]).LoginId;
            string sCSMCd = Convert.ToString(frm["txtCSMCode"]);
            string sCustomerCd = Convert.ToString(frm["lstCustomerList"]);
            Session["DisplayMsg"] = string.Empty;
            Session["Msg"] = string.Empty;

            string sShopCd = Convert.ToString(frm["lstShopList"]);

            string sMode = Convert.ToString(frm["lstModeList"]);
            string sPayagentCd = Convert.ToString(frm["txtPayAgent"]);
            string sCorpPayagentCd = Convert.ToString(frm["txtCorpPayagent"]);
            string sRRISFormat = Convert.ToString(frm["txtRRIS"]);
            string sProfitCenter = Convert.ToString(frm["txtCPC"]);
            string sSubProfitCenter = Convert.ToString(frm["txtProfitCenter"]);
            string sAccountCd = ManageCustomerShopMode.txtExpenseCode;
            sCSMCd = sCustomerCd + sShopCd + sMode;
            bool IsSucess = mmdc.UpdateCustShopMode(sCSMCd, sCustomerCd, sShopCd, sMode, sPayagentCd, sCorpPayagentCd, sRRISFormat, sProfitCenter, sSubProfitCenter, sAccountCd, sUser);
            if (IsSucess)
            {
                string errMsg = "Association " + sCustomerCd + "/" + sShopCd + "/" + sMode + " updated.";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(errMsg, "Success");
            }
            else
            {
                string errMsg = "No information has changed";
                TempData["Msg"] = UtilityClass.UtilMethods.GenErrorMessage(errMsg, "Warning");
            }
            ManageCustomerShopMode.Flag = true;
            return RedirectToAction("ViewCustShopModeDetails", new { PageName = "Edit" });
        }

        #endregion

        // Created By Afroz
        #region PayAgent Vendor
        #region LoadPayAgentVendor HTTPGet
        /// <summary>
        /// This is the HTTPGet method, which is called when we first open the page PayAgent vendor 

        /// </summary>
        /// <param name="payAgentVendor"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PayAgentVendor()
        {

            ManageMasterDataModel model = new ManageMasterDataModel();
            try
            {
                model = RSByPayAgentVendor(model);
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return View("PayAgentVendor", model);
        }
        #endregion LoadPayAgentVendor HTTPGet

        public ManageMasterDataModel RSByPayAgentVendor(ManageMasterDataModel model)
        {
            try
            {

                List<SelectListItem> AgentCodeItems = new List<SelectListItem>();
                List<PayAgentVendor> AgentCodeList;

                AgentCodeList = mmdc.RSAllCorpPayAgents().ToList();

                foreach (var item in AgentCodeList)
                {
                    AgentCodeItems.Add(new SelectListItem
                    {
                        Text = item.PayAgentCode,
                        Value = item.PayAgentCode
                    });
                }
                AgentCodeItems.Add(new SelectListItem { Text = "--Select--", Value = "-1", Selected = true });
                model.ddlPayAgent_CD = AgentCodeItems;


            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return model;

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RSVendorsByPayAgent(string AgentCode)
        {
            List<SelectListItem> AgentVendorCodeItems = new List<SelectListItem>();
            List<Vendor> AgentVendorCodeList;
            try
            {
                AgentVendorCodeList = mmdc.RSVendorsByPayAgent(AgentCode).ToList();

                if (AgentVendorCodeList.Count > 0)
                {
                    foreach (var q in AgentVendorCodeList)
                    {
                        AgentVendorCodeItems.Add(new SelectListItem { Text = q.VendorCode.ToString(), Value = q.VendorCode.ToString() });
                    }
                }
                else
                {
                    AgentVendorCodeItems.Add(new SelectListItem { Text = "", Value = "" });
                }



            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(AgentVendorCodeItems, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RSAllVendors()
        {
            List<SelectListItem> AgentVendorCodeItems = new List<SelectListItem>();
            List<Vendor> AgentVendorCodeList = new List<Vendor>();

            try
            {

                AgentVendorCodeList = mmdc.GetRSAllVendors().ToList();

                if (AgentVendorCodeList.Count > 0)
                {
                    foreach (var q in AgentVendorCodeList)
                    {
                        AgentVendorCodeItems.Add(new SelectListItem { Text = q.VendorCode.ToString() + " - " + q.VendorDesc.ToString(), Value = q.VendorCode.ToString() });
                    }
                }
                else
                {
                    AgentVendorCodeItems.Add(new SelectListItem { Text = "", Value = "" });
                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Json(AgentVendorCodeItems, JsonRequestBehavior.AllowGet);


        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RSByPayAgentCodeList()
        {
            List<SelectListItem> AgentVendorCodeItems = new List<SelectListItem>();
            List<PayAgentVendor> AgentVendorCodeList;
            try
            {
                AgentVendorCodeList = mmdc.RSAllCorpPayAgents().ToList();

                if (AgentVendorCodeList.Count > 0)
                {
                    foreach (var q in AgentVendorCodeList)
                    {
                        AgentVendorCodeItems.Add(new SelectListItem { Text = q.PayAgentCode.ToString(), Value = q.PayAgentCode.ToString() });
                    }
                }
                else
                {
                    AgentVendorCodeItems.Add(new SelectListItem { Text = "", Value = "" });
                }



            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(AgentVendorCodeItems, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RSByPayAgentVendor(string AgentCode, string VendorCode)
        {
            List<PayAgentVendor> AgentVendorCodeList = new List<PayAgentVendor>();
            try
            {


                AgentVendorCodeList = mmdc.RSByPayAgentVendor(AgentCode, VendorCode).ToList();


            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(AgentVendorCodeList, JsonRequestBehavior.AllowGet);

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeletePayAgentVendor(string AgentCode, string VendorCode)
        {
            string result = "";
            try
            {


                result = mmdc.DeletePayAgentVendor(AgentCode, VendorCode);


            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdatePayAgentVendor(string AgentCode, string VendorCode, string AcctCode, string SupplierCode)
        {
            string result = "";
            try
            {
                PayAgentVendor payAgentListToBeUpdated = new PayAgentVendor();
                payAgentListToBeUpdated.PayAgentCode = AgentCode;
                payAgentListToBeUpdated.VendorCode = VendorCode;
                payAgentListToBeUpdated.LocalAccountCode = AcctCode;
                payAgentListToBeUpdated.SupplierCode = SupplierCode;
                payAgentListToBeUpdated.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;
                result = mmdc.UpdatePayAgentVendor(payAgentListToBeUpdated);


            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Json(result, JsonRequestBehavior.AllowGet);


        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult CreatePayAgentVendor(string AgentCode, string VendorCode, string AcctCode, string SupplierCode)
        {
            string result = "";
            try
            {
                PayAgentVendor payAgentListToBeUpdated = new PayAgentVendor();
                payAgentListToBeUpdated.PayAgentCode = AgentCode;
                payAgentListToBeUpdated.VendorCode = VendorCode;
                payAgentListToBeUpdated.LocalAccountCode = AcctCode;
                payAgentListToBeUpdated.SupplierCode = SupplierCode;
                payAgentListToBeUpdated.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;
                result = mmdc.CreatePayAgentVendor(payAgentListToBeUpdated);


            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Json(result, JsonRequestBehavior.AllowGet);


        }
        #endregion PayAgentVendor
        // End Afroz
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


        public ActionResult ContainerGrade()
        {
            ContainerGrade model = new ContainerGrade();
            try
            {

                model = fillShopsWithDescription(model);
                model = fillGrade(model);

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View(model);


            //ContainerGrade containerGrade = GetContainerGradeData();
            //return View(containerGrade);
        }
        public ContainerGrade fillShopsWithDescription(ContainerGrade model)
        {
            #region ShopDropDown
            List<SelectListItem> ShopItems = new List<SelectListItem>();
            List<ManageMasterDataServiceReference.Shop> ShopList;
            try
            {
                //ShopList = mmdc.GetShopByUserId(((UserSec)Session["UserSec"]).UserId).ToList();
                ShopList = mmdc.GetShopCode().Take(100).ToList();
                foreach (var item in ShopList)
                {
                    ShopItems.Add(new SelectListItem
                    {
                        Text = item.ShopCode + "-" + item.ShopDescription,
                        Value = item.ShopCode
                    });
                    //if (item.ShopCode == "040")
                    //    break;
                }

                model.DDLShop = ShopItems;

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return model;
            #endregion ShopDropDown


        }
        //public List<MercPlusClient.Areas.ManageMasterData.Models.Grade> GetGradeLists()
        //{
        //    // Test Grade List
        //    MercPlusClient.Areas.ManageMasterData.Models.Grade gradeQ = new MercPlusClient.Areas.ManageMasterData.Models.Grade { GradeId = 1, GradeCode = "Q", GradeDescription = "Desc Q" };
        //    MercPlusClient.Areas.ManageMasterData.Models.Grade gradeE = new MercPlusClient.Areas.ManageMasterData.Models.Grade { GradeId = 2, GradeCode = "E", GradeDescription = "Desc E" };
        //    MercPlusClient.Areas.ManageMasterData.Models.Grade gradeK = new MercPlusClient.Areas.ManageMasterData.Models.Grade { GradeId = 3, GradeCode = "K", GradeDescription = "Desc K" };
        //    MercPlusClient.Areas.ManageMasterData.Models.Grade gradeM = new MercPlusClient.Areas.ManageMasterData.Models.Grade { GradeId = 4, GradeCode = "M", GradeDescription = "Desc M" };
        //    MercPlusClient.Areas.ManageMasterData.Models.Grade gradeS = new MercPlusClient.Areas.ManageMasterData.Models.Grade { GradeId = 5, GradeCode = "S", GradeDescription = "Desc S" };

        //    return new List<MercPlusClient.Areas.ManageMasterData.Models.Grade>
        //    {
        //        gradeQ,
        //        gradeE,
        //        gradeK,
        //        gradeM,
        //        gradeS
        //    };
        //}

        public ContainerGrade fillGrade(ContainerGrade model)
        {


            ManageMasterDataServiceReference.ManageMasterDataClient obj_Service = new ManageMasterDataServiceReference.ManageMasterDataClient();
            List<SelectListItem> GradeItems = new List<SelectListItem>();

            try
            {



                var GradeList = obj_Service.GetAllGradeCode().ToList();
                foreach (ManageMasterDataServiceReference.Grade item in GradeList)
                {
                    GradeItems.Add(new SelectListItem
                    {
                        Text = item.GradeCode,
                        Value = item.GradeCode.ToString().Trim().ToUpper() //item.GradeId.ToString()
                    });
                }





                model.DDLGrade = GradeItems;
            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return model;



        }

        //public JsonResult ContainerGradeDetails(string Shop_CD, string EQPNO)
        //{


        //    ContainerGrade containerGrade = GetContainerGradeData();
        //    // return View(containerGrade);
        //    return Json(containerGrade);
        //}

        public JsonResult AddContainerGrade(string ConNo, string CurrLoc, string GrCode, string GrCodeNew)
        {
            //TempData["Msg"] = "";
            // ContainerGrade Cgrade = new ContainerGrade();
            bool GC = true;
            string Result;
            //-------------------------Grade validation
            if (GrCode.ToUpper() == "N")
            {
                GC = true;
            }

            else if (GrCodeNew.ToUpper() == "N")
            {
                GC = true;
            }
            else
            {
                GC = GradeCodeValidation(GrCode, GrCodeNew);
            }
            if (GC == false)
            {
                Result = "Selected new Grade: " + GrCodeNew + " is not mapped with " + GrCode + "";
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            //------------------------
            GradeContainer Cgrade = new GradeContainer();
            int Wo_id = 0;
            Cgrade.EQPNO = ConNo;
            if (Wo_id != 0)
            {
                Cgrade.WO_ID = Wo_id;
            }
            Cgrade.CURRENTLOC = CurrLoc;
            Cgrade.GRADECODE = GrCode;
            Cgrade.GRADECODE_NEW = GrCodeNew;
            Cgrade.MODIFIEDBY = ((UserSec)Session["UserSec"]).LoginId;
            Cgrade.MODIFIEDON = DateTime.Now;


            bool Rs = mmdc.UpdateGradeContainer(Cgrade);



            //
            MercPlusClient.ManageWorkOrderServiceReference.ManageWorkOrderClient WC = new MercPlusClient.ManageWorkOrderServiceReference.ManageWorkOrderClient();
            bool Sr = WC.SendGradeCodeToRKEM(ConNo, CurrLoc, GrCodeNew);

            if (Rs == true && Sr == true)
                Result = "Success";
            else
                Result = "Failed";
            return Json(Result, JsonRequestBehavior.AllowGet);
            // return Json(data);
        }

        private bool GradeCodeValidation(string GrCode, string GrCodeNew)
        {
            bool OK = false;
            List<GradeRelationModel> gradeRelations = GetGradeRelations();
            foreach (var item in gradeRelations)
            {
                if (item.GradeCode == GrCode)
                {
                    if (item.UpgradedGrades != null)
                    {
                        foreach (var uitem in item.UpgradedGrades)
                        {
                            if (uitem.ToUpper() == GrCodeNew.ToUpper())
                                OK = true;
                        }
                    }

                    if (item.DowngradedGrades != null)
                    {
                        foreach (var ditem in item.DowngradedGrades)
                        {
                            if (ditem.ToUpper() == GrCodeNew.ToUpper())
                                OK = true;
                        }
                    }
                    //if ((item.UpgradedGrades == null) && (item.DowngradedGrades == null))
                    //{
                    //    OK = true;
                    //}
                }
            }

            return OK;

        }

        public ActionResult GradeRelation()
        {
            List<GradeRelationModel> gradeRelations = GetGradeRelations();
            ViewBag.GradeNames = GetAllGradeNames();
            return View(gradeRelations);
        }

        private List<string> GetAllGradeNames()
        {
            List<string> GradeNames = null;
            try
            {
                GradeNames = mmdc.GetAllGradeNames().ToList();
                GradeNames.Remove("N");
            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return GradeNames;
        }

        private List<GradeRelationModel> GetGradeRelations()
        {
            List<GradeRelation> GradeRelationList = null;
            List<GradeRelationModel> GradeRelationModelList = null;
            try
            {
                GradeRelationList = mmdc.GetAllGradeRelations().ToList();
                if (GradeRelationList != null && GradeRelationList.Count > 0)
                {
                    GradeRelationModelList = new List<GradeRelationModel>();
                    foreach (var item in GradeRelationList)
                    {
                        GradeRelationModel gradeRelationModel = new GradeRelationModel();

                        gradeRelationModel.GradeRelationId = item.GradeRelationId;
                        gradeRelationModel.GradeId = item.GradeId;
                        gradeRelationModel.GradeCode = item.GradeCode;
                        gradeRelationModel.GradeDescription = item.GradeDescription;

                        gradeRelationModel.UpgradedGrades = !string.IsNullOrWhiteSpace(item.UpgradedGrade)
                                                            ? item.UpgradedGrade.Split(',').ToList()
                                                            : null;
                        gradeRelationModel.DowngradedGrades = !string.IsNullOrWhiteSpace(item.DowngradedGrade)
                                                              ? item.DowngradedGrade.Split(',').ToList()
                                                              : null;

                        gradeRelationModel.CreatedBy = item.CreatedBy;
                        gradeRelationModel.CreatedOn = item.CreatedOn;
                        gradeRelationModel.ModifiedBy = item.ModifiedBy;
                        gradeRelationModel.ModifiedOn = item.ModifiedOn;

                        GradeRelationModelList.Add(gradeRelationModel);
                    }

                }
            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return GradeRelationModelList;
        }

        public JsonResult JsonUpdateGradeRelation(string gradeRelationId, string upgradedGrades, string downgradedGrades)
        {
            bool isUpdated = false;
            GradeRelation gradeRelation = null;
            int graderelationid = 0;
            try
            {
                if (!int.TryParse(gradeRelationId, out graderelationid))
                {
                    graderelationid = 0;
                }

                if (graderelationid != 0)
                {
                    gradeRelation = mmdc.GetGradeRelationById(graderelationid);
                    if (gradeRelation != null)
                    {
                        gradeRelation.UpgradedGrade = upgradedGrades;
                        gradeRelation.DowngradedGrade = downgradedGrades;
                        gradeRelation.ModifiedBy = ((UserSec)Session["UserSec"]).LoginId;

                        isUpdated = mmdc.UpdateGradeRelation(gradeRelation);
                    }
                }

                return Json(isUpdated);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsElligibleforDelete(int gradeRelationid)
        {
            bool IsEligible = true;
            GradeRelation gradeRelation = null;
            string GC_CODE = "";


            gradeRelation = mmdc.GetGradeRelationById(gradeRelationid);
            if (gradeRelation != null)
            {
                GC_CODE = gradeRelation.GradeCode;
                List<GradeRelation> GradeRelationListd = null;
                GradeRelationListd = mmdc.GetAllGradeRelations().ToList();

                foreach (var item in GradeRelationListd)
                {
                    if (item.DowngradedGrade.Contains(GC_CODE) || item.UpgradedGrade.Contains(GC_CODE))
                    {

                        IsEligible = false;
                        break;
                    }


                }
            }


            return IsEligible;

        }

        public JsonResult JsonDeleteGradeRelation(string gradeRelationId)
        {
            bool isDeleted = false;
            int graderelationid = 0;
            try
            {
                if (!int.TryParse(gradeRelationId, out graderelationid))
                {
                    graderelationid = 0;
                }

                if (graderelationid != 0)
                {
                    if (IsElligibleforDelete(graderelationid) == true)
                    {
                        isDeleted = mmdc.DeleteGradeRelation(graderelationid);
                    }
                    else
                    {
                        isDeleted = false;
                    }
                }

                return Json(isDeleted);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult AddGradeRelation(string gradecode, string gradedescription, string upgradedGrades, string downgradedGrades)
        {
            bool isAdded = false;
            GradeRelation gradeRelation = new GradeRelation();
            try
            {
                gradeRelation.GradeCode = gradecode;
                gradeRelation.GradeDescription = gradedescription;
                gradeRelation.UpgradedGrade = upgradedGrades;
                gradeRelation.DowngradedGrade = downgradedGrades;
                gradeRelation.CreatedBy = ((UserSec)Session["UserSec"]).LoginId;

                isAdded = mmdc.AddGradeRelation(gradeRelation);

                if (isAdded)
                {
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("GradeRelation", "ManageMasterData");
                    return Json(new { Url = redirectUrl });
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult STSGradeMapping()
        {
            List<GradeSTSModel> gradeSTSList = GetGradeSTSList();
            ViewBag.GradeNames = GetAllGradeNames();
            return View("GradeSTSMapping", gradeSTSList);
        }

        private List<GradeSTSModel> GetGradeSTSList()
        {
            List<GradeSTS> GradeSTSList = null;
            List<GradeSTSModel> GradeSTSModelList = null;
            string[] modes = { "03", "04", "05" };
            string manualcd = "MAER";
            List<string> stsCodeList = null;
            List<string> GradeNames = GetAllGradeNames();
            try
            {
                GradeSTSList = mmdc.GetAllGradeSTSByMode(modes, manualcd).ToList();

                if (GradeSTSList != null && GradeSTSList.Count > 0)
                {
                    stsCodeList = GradeSTSList.Select(x => x.STSCode).Distinct().ToList();
                }

                if (stsCodeList != null && stsCodeList.Count > 0)
                {
                    GradeSTSModelList = new List<GradeSTSModel>();
                    foreach (var stsCode in stsCodeList)
                    {
                        foreach (string mode in modes)
                        {
                            GradeSTSModel gradeSTSModel = null;
                            List<GradeSTS> GradeSTSListTemp = GradeSTSList
                                .Where(x => x.STSCode == stsCode && x.Mode == mode).ToList();

                            if (GradeSTSListTemp != null && GradeSTSListTemp.Count > 0)
                            {
                                gradeSTSModel = new GradeSTSModel();

                                gradeSTSModel.GradeSTSRowId = GetGradeSTSRowId();
                                gradeSTSModel.STSCode = stsCode;
                                gradeSTSModel.STSDescription = GradeSTSListTemp.First().STSDescription;
                                gradeSTSModel.Mode = mode;
                                gradeSTSModel.ManualCD = GradeSTSListTemp.First().ManualCD;
                                gradeSTSModel.Flag = GradeSTSListTemp.First().FLAG;

                                List<GradeSTSRelationModel> GradeSTSRelationModelListForGrade = new List<GradeSTSRelationModel>();

                                foreach (string gradeName in GradeNames)
                                {
                                    GradeSTS GradeSTSListForGrade = GradeSTSListTemp.FirstOrDefault(x => x.GradeCode == gradeName);

                                    GradeSTSRelationModel gradeSTSRelationModel = new GradeSTSRelationModel();
                                    gradeSTSRelationModel.GradeCode = gradeName;
                                    gradeSTSRelationModel.IsApplicale = false;

                                    if (GradeSTSListForGrade != null)
                                    {
                                        gradeSTSRelationModel.GradeSTSId = GradeSTSListForGrade.GradeSTSId;
                                        gradeSTSRelationModel.GradeId = GradeSTSListForGrade.GradeId;
                                        gradeSTSRelationModel.IsApplicale = GradeSTSListForGrade.IsApplicable;
                                    }

                                    GradeSTSRelationModelListForGrade.Add(gradeSTSRelationModel);
                                }

                                gradeSTSModel.GradeSTSRelationModel = GradeSTSRelationModelListForGrade;
                                GradeSTSModelList.Add(gradeSTSModel);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return GradeSTSModelList;
        }

        private readonly Random _random = new Random();

        private int GetGradeSTSRowId()
        {
            return _random.Next();
        }

        public JsonResult JsonDeleteGradeSTSMapping(string stscode, string mode)
        {
            bool isDeleted = false;
            string manualcd = "MAER";
            try
            {
                if (!string.IsNullOrWhiteSpace(stscode) && !string.IsNullOrWhiteSpace(mode))
                {
                    isDeleted = mmdc.DeleteGradeSTSMapping(stscode.Trim(), mode.Trim(), manualcd);
                }

                return Json(isDeleted);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult JsonUpdateGradeSTSMapping(string stscode, string mode, string FLAG, string gradeapplicablevalues)
        {
            bool isUpdated = false;
            string manualcd = "MAER";
            List<GradeSTS> gradeSTSList = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(stscode) && !string.IsNullOrWhiteSpace(mode))
                {
                    gradeSTSList = new List<GradeSTS>();
                    List<string> grades = GetAllGradeNames();
                    string[] applicableValues = gradeapplicablevalues.Split(',');
                    int i = 0;
                    foreach (string grade in grades)
                    {
                        GradeSTS gradeSTS = new GradeSTS();

                        gradeSTS.STSCode = stscode.Trim();
                        gradeSTS.Mode = mode.Trim();
                        gradeSTS.FLAG = string.IsNullOrWhiteSpace(FLAG) ? false :
                                                (
                                                    FLAG.Trim().ToUpper() == "YES" ? true : false
                                                );
                        gradeSTS.ManualCD = manualcd;
                        gradeSTS.GradeCode = grade;
                        gradeSTS.IsApplicable = string.IsNullOrWhiteSpace(applicableValues[i]) ? false :
                                                (
                                                    applicableValues[i].Trim().ToUpper() == "APPLICABLE" ? true : false
                                                );
                        gradeSTS.ModifiedBy = ((UserSec)Session["UserSec"]).LoginId;

                        gradeSTSList.Add(gradeSTS);

                        i++;
                    }

                    isUpdated = mmdc.UpdateGradeSTSMapping(gradeSTSList.ToArray());
                }

                 return Json(isUpdated);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult JsonAddGradeSTSMapping(string stscode, string mode, string FLAG, string gradeapplicablevalues)
        {
            bool isAdded = false;
            string manualcd = "MAER";
            List<GradeSTS> gradeSTSList = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(stscode) && !string.IsNullOrWhiteSpace(mode))
                {
                    gradeSTSList = new List<GradeSTS>();
                    List<string> grades = GetAllGradeNames();
                    string[] applicableValues = gradeapplicablevalues.Split(',');
                    int i = 0;
                    foreach (string grade in grades)
                    {
                        GradeSTS gradeSTS = new GradeSTS();

                        gradeSTS.STSCode = stscode.Trim();
                        gradeSTS.Mode = mode.Trim();
                        gradeSTS.ManualCD = manualcd;
                        gradeSTS.FLAG = string.IsNullOrWhiteSpace(FLAG) ? false :
                                                  (
                                                      FLAG.Trim().ToUpper() == "YES" ? true : false
                                                  );
                        gradeSTS.GradeCode = grade;
                        gradeSTS.IsApplicable = string.IsNullOrWhiteSpace(applicableValues[i]) ? false :
                                                (
                                                    applicableValues[i].Trim().ToUpper() == "APPLICABLE" ? true : false
                                                );
                        gradeSTS.CreatedBy = ((UserSec)Session["UserSec"]).LoginId;

                        gradeSTSList.Add(gradeSTS);

                        i++;
                    }

                    isAdded = mmdc.AddGradeSTSMapping(gradeSTSList.ToArray());
                }

                if (isAdded)
                {
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("STSGradeMapping", "ManageMasterData");
                    return Json(new { Url = redirectUrl });
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult JsonGetSTSDescription(string stscode, string mode)
        {
            string Result = string.Empty;
            string manualcd = "MAER";
            try
            {
                if (!string.IsNullOrWhiteSpace(stscode) && !string.IsNullOrWhiteSpace(mode))
                {
                    Result = mmdc.GetSTSDescription(stscode.Trim(), mode.Trim(), manualcd);
                }

                //return Json(stsDescription);
                //return Result;
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}





