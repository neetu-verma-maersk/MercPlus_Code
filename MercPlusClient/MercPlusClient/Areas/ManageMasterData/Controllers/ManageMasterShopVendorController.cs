using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MercPlusClient.ManageMasterDataServiceReference;
using MercPlusClient.ManageWorkOrderServiceReference;
using MercPlusClient.Areas.ManageMasterData.Models;
using Microsoft.Practices.EnterpriseLibrary.Logging;


using System.Web.Helpers;


namespace MercPlusClient.Areas.ManageMasterData.Controllers
{
    public class ManageMasterShopVendorController : Controller
    {
        ManageMasterShopVndorModel ManageMasterShopVndorModel = new ManageMasterShopVndorModel();
        ManageMasterDataClient mmdc = new ManageMasterDataClient();
        ManageWorkOrderServiceReference.ManageWorkOrderClient obj_Sercice = new ManageWorkOrderServiceReference.ManageWorkOrderClient();
        LogEntry logEntry = new LogEntry();
        public ActionResult ShopContract(ManageMasterShopVndorModel model, FormCollection collection)
        {


            try
            {
                model = fillShop(model);
                model = fillModeByManualCode(null, model);

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View(model);

        }


        public ActionResult ShopContract_View(ManageMasterShopVndorModel model, FormCollection collection) //Debadrita_User_Remapping
        {


            try
            {
                model = fillShop(model);
                model = fillModeByManualCode(null, model);

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View(model);

        }

        [HttpPost]
        public ActionResult GetRSAllContracts(string ShopCode, string RepairCode, string Mode, ManageMasterShopVndorModel model)
        {

            List<ManageMasterShopVndorModel> y = null;
            var qq = y;

            try
            {
                var ShopContList = mmdc.GetRSAllContracts(ShopCode, RepairCode, Mode).ToList();



                qq = (from e in ShopContList
                      select new ManageMasterShopVndorModel
                      {
                          ShopContID = e.ShopContID,
                          ShopCode = e.ShopCode,
                          EffDate = (string.IsNullOrEmpty(e.EffDate.ToString())) ? string.Empty : (Convert.ToDateTime(e.EffDate.ToString())).ToString("yyyy-MM-dd"),
                          ExpDate = (string.IsNullOrEmpty(e.ExpDate.ToString())) ? string.Empty : (Convert.ToDateTime(e.ExpDate.ToString())).ToString("yyyy-MM-dd"),
                          ManualCode = e.ManualCode,
                          RepairCode = e.RepairCode,
                          ContractAmount = e.ContractAmount,
                          Mode = e.Mode,
                          ShopActiveSW = e.Shop.ShopActiveSW,
                          CUCDN = e.Shop.CUCDN,
                          CurCode = e.Shop.Currency.CurCode,
                          Decentralized = e.Shop.Decentralized,

                          intedit = (e.Shop.ShopActiveSW == "N" || e.Shop.Decentralized == "Y") ? 1 : 0,
                          //intflag = (e.Shop.ShopActiveSW == "N" || e.Shop.Decentralized == "Y") ? 1 : 0
                      }).ToList();
                for (int j = 0; j < qq.Count(); j++)
                {
                    if (Convert.ToDateTime(qq[j].ExpDate) >= System.DateTime.Now)
                    {
                        if (qq[j].ShopActiveSW == "N" || qq[j].Decentralized == "Y")
                        {
                            model.intflag = 1;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            ManageMasterDataServiceReference.Currency objCurrency = new ManageMasterDataServiceReference.Currency();
            objCurrency = mmdc.GetShopContCurrencyCode(ShopCode);
            if (objCurrency != null)
            {
                model.CurCode = objCurrency.Cucdn;
            }
            else
            {
                model.CurCode = "";
            }
            model.SearchResults = qq;
            return PartialView("Partial_ShopContract", model);

        }



        public ActionResult GetRSAllContracts_View(string ShopCode, string RepairCode, string Mode, ManageMasterShopVndorModel model)
        {

            List<ManageMasterShopVndorModel> y = null;
            var qq = y;

            try
            {
                var ShopContList = mmdc.GetRSAllContracts(ShopCode, RepairCode, Mode).ToList();



                qq = (from e in ShopContList
                      select new ManageMasterShopVndorModel
                      {
                          ShopContID = e.ShopContID,
                          ShopCode = e.ShopCode,
                          EffDate = (string.IsNullOrEmpty(e.EffDate.ToString())) ? string.Empty : (Convert.ToDateTime(e.EffDate.ToString())).ToString("yyyy-MM-dd"),
                          ExpDate = (string.IsNullOrEmpty(e.ExpDate.ToString())) ? string.Empty : (Convert.ToDateTime(e.ExpDate.ToString())).ToString("yyyy-MM-dd"),
                          ManualCode = e.ManualCode,
                          RepairCode = e.RepairCode,
                          ContractAmount = e.ContractAmount,
                          Mode = e.Mode,
                          ShopActiveSW = e.Shop.ShopActiveSW,
                          CUCDN = e.Shop.CUCDN,
                          CurCode = e.Shop.Currency.CurCode,
                          Decentralized = e.Shop.Decentralized,

                          intedit = (e.Shop.ShopActiveSW == "N" || e.Shop.Decentralized == "Y") ? 1 : 0,
                          //intflag = (e.Shop.ShopActiveSW == "N" || e.Shop.Decentralized == "Y") ? 1 : 0
                      }).ToList();
                for (int j = 0; j < qq.Count(); j++)
                {
                    if (Convert.ToDateTime(qq[j].ExpDate) >= System.DateTime.Now)
                    {
                        if (qq[j].ShopActiveSW == "N" || qq[j].Decentralized == "Y")
                        {
                            model.intflag = 1;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            ManageMasterDataServiceReference.Currency objCurrency = new ManageMasterDataServiceReference.Currency();
            objCurrency = mmdc.GetShopContCurrencyCode(ShopCode);
            if (objCurrency != null)
            {
                model.CurCode = objCurrency.Cucdn;
            }
            else
            {
                model.CurCode = "";
            }
            model.SearchResults = qq;
            return PartialView("Partial_ShopContract_View", model);

        }


        public JsonResult DeleteShopContract(string gridData)
        {
            string result = "";
            string[] strArray = gridData.Split(',');
            int[] myIntIDs = Array.ConvertAll(strArray, int.Parse);

            try
            {
                //result = mmdc.DeleteShopContract(myIntIDs);
                result = mmdc.DeleteShopContract(gridData);

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                result = "Error while going to delete...";
            }

            return Json(result);
        }
        public JsonResult UpdateExpDateForShopContract(string gridData, string expDate)
        {

            string result = "";
            string[] strArray = gridData.Split(',');
            int[] myIntIDs = Array.ConvertAll(strArray, int.Parse);
            if (expDate == "")
            {
                result = "Please give expiry date to update the record...";
            }
            else
            {
                DateTime dt = Convert.ToDateTime(expDate.ToString());
                try
                {
                   // result = mmdc.UpdateExpDateForShopContract(myIntIDs, dt, ((UserSec)Session["UserSec"]).UserId.ToString());
                    result = mmdc.UpdateExpDateForShopContract(gridData, dt, ((UserSec)Session["UserSec"]).UserId.ToString());
                }
                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    result = "Error in update...";
                }
            }

            return Json(result);
        }

        public ActionResult FillShopContractEdit(int ContID)
        {

            ManageMasterShopVndorModel model = new ManageMasterShopVndorModel();
            var result = mmdc.FillShopContractEdit(ContID);
            ManageMasterDataServiceReference.Currency objCurrency = new ManageMasterDataServiceReference.Currency();
            objCurrency = mmdc.GetShopContCurrencyCode(result.ShopCode);
            try
            {
                model.ShopContID = result.ShopContID;
                model.IsUpdate = true;
                model = fillShop(model);
                model.DDLShop_ShopContract = model.DDLShop_ShopContract.Where(n => n.Value == result.ShopCode).ToList();
                model = GetRSAllManual(model);
                model.DDLManual_ShopContract = model.DDLManual_ShopContract.Where(m => m.Value == result.ManualCode).ToList();
                model = fillModeByManualCode(null, model);
                model.DDLMode_ShopContract = model.DDLMode_ShopContract.Where(o => o.Value == result.Mode).ToList();
                model.RepairCode_ShopContract = result.RepairCode;
                model.Amount_ShopContract = result.ContractAmount;
                model.EffectiveDate_ShopContract = result.EffDate;
                if (result.ExpDate.ToShortDateString() == "1/1/1753")
                {
                }
                else
                {
                    model.ExpireDate_ShopContract = result.ExpDate;
                }

                model.Currency_ShopContract = objCurrency.Cucdn;
                model.ChangeUser = result.ChangeUser;
                model.ChangeTime = result.ChangeTime;



            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return View("ShopContract_Edit", model);
        }
        public ActionResult ShopContract_Edit()
        {

            ManageMasterShopVndorModel model = new ManageMasterShopVndorModel();
            try
            {

                model = fillInactiveShop(model);
                ManageMasterDataServiceReference.Currency objCurrency = new ManageMasterDataServiceReference.Currency();
                objCurrency = mmdc.GetShopContCurrencyCode(model.DDLShop_ShopContract[0].Value.ToString());
                model.Currency_ShopContract = objCurrency.Cucdn;
                model = GetRSAllManual(model);
                model.IsUpdate = false;
                model = fillModeByManualCode(model.DDLManual_ShopContract[0].Value, model);

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View("ShopContract_Edit", model);
        }
        [HttpPost]
        public JsonResult ShopContractAdd(string ShopCode, string ManualCode, string ModeCode, string RepairCode, string ContAmt, string Currency, string EffectiveDate, string ExpireDate)
        {
            string result = "";
            try
            {
                if (ModelState.IsValid)
                {
                    ShopCont ShopContList = new ShopCont();
                    ShopContList.ShopCode = ShopCode;
                    ShopContList.ManualCode = ManualCode;
                    ShopContList.Mode = ModeCode;
                    ShopContList.RepairCode = RepairCode;
                    ShopContList.ContractAmount = Convert.ToDecimal(ContAmt);
                    if (EffectiveDate != "")
                    {
                        ShopContList.EffDate = Convert.ToDateTime(EffectiveDate);
                    }

                    if (ExpireDate != "")
                    {
                        ShopContList.ExpDate = Convert.ToDateTime(ExpireDate);
                    }
                    ShopContList.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;
                    result = mmdc.InsertShopContract(ShopContList);

                }
            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                result = "Failed to insert.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult ShopContractUpdate(string ShopContId, string ShopCode, string ManualCode, string ModeCode, string RepairCode, string ContAmt, string Currency, string EffectiveDate, string ExpireDate)
        {
            string result = "";
            try
            {
                if (ModelState.IsValid)
                {
                    ShopCont ShopContList = new ShopCont();
                    ShopContList.ShopContID = Convert.ToInt32(ShopContId);
                    ShopContList.ShopCode = ShopCode;
                    ShopContList.ManualCode = ManualCode;
                    ShopContList.Mode = ModeCode;
                    ShopContList.RepairCode = RepairCode;
                    ShopContList.ContractAmount = Convert.ToDecimal(ContAmt);
                    if (EffectiveDate != "")
                    {
                        ShopContList.EffDate = Convert.ToDateTime(EffectiveDate);
                    }

                    if (ExpireDate != "")
                    {
                        ShopContList.ExpDate = Convert.ToDateTime(ExpireDate);
                    }
                    ShopContList.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;
                    result = mmdc.UpdateShopContract(ShopContList);

                }
            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(result);
        }

        public ActionResult ShopLimits()
        {
            ManageMasterShopVndorModel model = new ManageMasterShopVndorModel();
            try
            {

                model = fillShopWithDescription(model);

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View(model);
        }

        public ActionResult ShopLimits_View()
        {
            ManageMasterShopVndorModel model = new ManageMasterShopVndorModel();
            try
            {

                model = fillShopWithDescription(model);

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View(model);
        }

        public ActionResult ShopProfiles()
        {
            ManageMasterShopVndorModel model = new ManageMasterShopVndorModel();
            try
            {

                model = fillShopProfileWithDescription(model);
                model = fillVendor(model);
                model = fillCurrency(model);

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View("ShopProfiles", model);
        }

        public ActionResult ShopProfiles_View()
        {
            ManageMasterShopVndorModel model = new ManageMasterShopVndorModel();
            try
            {

                model = fillShopProfileWithDescription(model);
                model = fillVendor(model);
                model = fillCurrency(model);

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View("ShopProfiles_View", model);
        }

        public ActionResult Discount()
        {

            ManageMasterShopVndorModel model = new Models.ManageMasterShopVndorModel();

            string ManufctrCode1 = "";
            string ManufctrCode2 = "";
            string ManufctrCode3 = "";
            string ManufctrCode4 = "";
            string ManufctrCode5 = "";
            string ManufctrCode6 = "";
            string ManufctrCode7 = "";
            string ManufctrCode8 = "";
            string ManufctrCode9 = "";
            string ManufctrCode10 = "";

            List<SelectListItem> Manufacturer1Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer2Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer3Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer4Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer5Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer6Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer7Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer8Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer9Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer10Items = new List<SelectListItem>();
            List<ManageMasterDataServiceReference.Shop> ShopList;
            List<Manufactur> ManufacturList;
            List<Discount> DiscountList;
            try
            {
                ShopList = mmdc.GetRSByShop(Request.QueryString["ShopCD"]).ToList();
                foreach (var item in ShopList)
                {
                    model.ShopCode_Profile = item.ShopCode;
                    model.PCTMaterialFactor = item.PCTMaterialFactor;
                }
                DiscountList = mmdc.GetRSDiscount(Request.QueryString["ShopCD"]).ToList();
                for (int i = 0; i < DiscountList.Count; i++)
                {
                    if (i == 0)
                    {
                        ManufctrCode1 = DiscountList[i].Manufctr;
                        model.Discount1 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 1)
                    {
                        ManufctrCode2 = DiscountList[i].Manufctr;
                        model.Discount2 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 2)
                    {
                        ManufctrCode3 = DiscountList[i].Manufctr;
                        model.Discount3 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 3)
                    {
                        ManufctrCode4 = DiscountList[i].Manufctr;
                        model.Discount4 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 4)
                    {
                        ManufctrCode5 = DiscountList[i].Manufctr;
                        model.Discount5 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 5)
                    {
                        ManufctrCode6 = DiscountList[i].Manufctr;
                        model.Discount6 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 6)
                    {
                        ManufctrCode7 = DiscountList[i].Manufctr;
                        model.Discount7 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 7)
                    {
                        ManufctrCode8 = DiscountList[i].Manufctr;
                        model.Discount8 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 8)
                    {
                        ManufctrCode9 = DiscountList[i].Manufctr;
                        model.Discount9 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 9)
                    {
                        ManufctrCode10 = DiscountList[i].Manufctr;
                        model.Discount10 = DiscountList[i].MarkDiscount;
                    }
                }

                ManufacturList = mmdc.RSAllManufacturers().ToList();

                foreach (var item in ManufacturList)
                {
                    Manufacturer1Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode1 ? true : false)
                    });
                    Manufacturer2Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode2 ? true : false)
                    });
                    Manufacturer3Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode3 ? true : false)
                    });
                    Manufacturer4Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode4 ? true : false)
                    });
                    Manufacturer5Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode5 ? true : false)
                    });
                    Manufacturer6Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode6 ? true : false)
                    });
                    Manufacturer7Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode7 ? true : false)
                    });
                    Manufacturer8Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode8 ? true : false)
                    });
                    Manufacturer9Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode9 ? true : false)
                    });
                    Manufacturer10Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode10 ? true : false)
                    });
                }

                model.DDL_Manufacturer1 = Manufacturer1Items;
                model.DDL_Manufacturer2 = Manufacturer2Items;
                model.DDL_Manufacturer3 = Manufacturer3Items;
                model.DDL_Manufacturer4 = Manufacturer4Items;
                model.DDL_Manufacturer5 = Manufacturer5Items;
                model.DDL_Manufacturer6 = Manufacturer6Items;
                model.DDL_Manufacturer7 = Manufacturer7Items;
                model.DDL_Manufacturer8 = Manufacturer8Items;
                model.DDL_Manufacturer9 = Manufacturer9Items;
                model.DDL_Manufacturer10 = Manufacturer10Items;


            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View("Discount", model);
        }


        public ActionResult Discount_View()
        {

            ManageMasterShopVndorModel model = new Models.ManageMasterShopVndorModel();

            string ManufctrCode1 = "";
            string ManufctrCode2 = "";
            string ManufctrCode3 = "";
            string ManufctrCode4 = "";
            string ManufctrCode5 = "";
            string ManufctrCode6 = "";
            string ManufctrCode7 = "";
            string ManufctrCode8 = "";
            string ManufctrCode9 = "";
            string ManufctrCode10 = "";

            List<SelectListItem> Manufacturer1Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer2Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer3Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer4Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer5Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer6Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer7Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer8Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer9Items = new List<SelectListItem>();
            List<SelectListItem> Manufacturer10Items = new List<SelectListItem>();
            List<ManageMasterDataServiceReference.Shop> ShopList;
            List<Manufactur> ManufacturList;
            List<Discount> DiscountList;
            try
            {
                ShopList = mmdc.GetRSByShop(Request.QueryString["ShopCD"]).ToList();
                foreach (var item in ShopList)
                {
                    model.ShopCode_Profile = item.ShopCode;
                    model.PCTMaterialFactor = item.PCTMaterialFactor;
                }
                DiscountList = mmdc.GetRSDiscount(Request.QueryString["ShopCD"]).ToList();
                for (int i = 0; i < DiscountList.Count; i++)
                {
                    if (i == 0)
                    {
                        ManufctrCode1 = DiscountList[i].Manufctr;
                        model.Discount1 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 1)
                    {
                        ManufctrCode2 = DiscountList[i].Manufctr;
                        model.Discount2 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 2)
                    {
                        ManufctrCode3 = DiscountList[i].Manufctr;
                        model.Discount3 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 3)
                    {
                        ManufctrCode4 = DiscountList[i].Manufctr;
                        model.Discount4 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 4)
                    {
                        ManufctrCode5 = DiscountList[i].Manufctr;
                        model.Discount5 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 5)
                    {
                        ManufctrCode6 = DiscountList[i].Manufctr;
                        model.Discount6 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 6)
                    {
                        ManufctrCode7 = DiscountList[i].Manufctr;
                        model.Discount7 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 7)
                    {
                        ManufctrCode8 = DiscountList[i].Manufctr;
                        model.Discount8 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 8)
                    {
                        ManufctrCode9 = DiscountList[i].Manufctr;
                        model.Discount9 = DiscountList[i].MarkDiscount;
                    }
                    if (i == 9)
                    {
                        ManufctrCode10 = DiscountList[i].Manufctr;
                        model.Discount10 = DiscountList[i].MarkDiscount;
                    }
                }

                ManufacturList = mmdc.RSAllManufacturers().ToList();

                foreach (var item in ManufacturList)
                {
                    Manufacturer1Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode1 ? true : false)
                    });
                    Manufacturer2Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode2 ? true : false)
                    });
                    Manufacturer3Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode3 ? true : false)
                    });
                    Manufacturer4Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode4 ? true : false)
                    });
                    Manufacturer5Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode5 ? true : false)
                    });
                    Manufacturer6Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode6 ? true : false)
                    });
                    Manufacturer7Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode7 ? true : false)
                    });
                    Manufacturer8Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode8 ? true : false)
                    });
                    Manufacturer9Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode9 ? true : false)
                    });
                    Manufacturer10Items.Add(new SelectListItem
                    {
                        Text = item.ManufacturCd,
                        Value = item.ManufacturCd,
                        Selected = (item.ManufacturCd == ManufctrCode10 ? true : false)
                    });
                }

                model.DDL_Manufacturer1 = Manufacturer1Items;
                model.DDL_Manufacturer2 = Manufacturer2Items;
                model.DDL_Manufacturer3 = Manufacturer3Items;
                model.DDL_Manufacturer4 = Manufacturer4Items;
                model.DDL_Manufacturer5 = Manufacturer5Items;
                model.DDL_Manufacturer6 = Manufacturer6Items;
                model.DDL_Manufacturer7 = Manufacturer7Items;
                model.DDL_Manufacturer8 = Manufacturer8Items;
                model.DDL_Manufacturer9 = Manufacturer9Items;
                model.DDL_Manufacturer10 = Manufacturer10Items;


            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View("Discount_View", model);
        }
        [HttpPost]
        public JsonResult InsertUpdateShopDiscount(string ShopCode, string DiscountAll, string ManufactCode, string ManufactDis)
        {
            string result = "";
            try
            {
                result = mmdc.InsertUpdateShopDiscount(ShopCode, DiscountAll, ManufactCode, ManufactDis, ((UserSec)Session["UserSec"]).LoginId);


            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                result = "Failed.";
            }

            return Json(result);
        }


        public ActionResult FillShopModeAdd()
        {
            ManageMasterShopVndorModel model = new ManageMasterShopVndorModel();
            try
            {

                #region ShopDropDown
                List<SelectListItem> ShopItems = new List<SelectListItem>();
                List<ManageWorkOrderServiceReference.Shop> ShopList;

                ShopList = obj_Sercice.GetShopCode(Convert.ToInt32(((UserSec)Session["UserSec"]).UserId.ToString())).ToList();

                foreach (var item in ShopList)
                {
                    ShopItems.Add(new SelectListItem
                    {
                        Text = item.ShopCode + " - " + item.ShopDescription,
                        Value = item.ShopCode
                    });
                }
                model.DDLShopCodeAdd = ShopItems;
                #endregion ShopDropDown

                #region ModeDropDown
                List<SelectListItem> ModeItems = new List<SelectListItem>();
                List<ManageMasterDataServiceReference.Mode> ModeList;

                ModeList = mmdc.GetModes().ToList();

                foreach (var item in ModeList)
                {
                    ModeItems.Add(new SelectListItem
                    {
                        Text = item.ModeDescription,
                        Value = item.ModeCode
                    });
                }
                model.DDLShopModeAdd = ModeItems;

                #endregion ModeDropDown


            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return View("ShopLimits", model);
        }

        public ManageMasterShopVndorModel fillShop(ManageMasterShopVndorModel model)
        {
            List<SelectListItem> ShopItems = new List<SelectListItem>();
            List<ManageMasterDataServiceReference.Shop> ShopList;
            try
            {
                #region ShopDropDown


                ShopList = mmdc.GetShopCodeByUserId(((UserSec)Session["UserSec"]).UserId).ToList();

                foreach (var item in ShopList)
                {
                    ShopItems.Add(new SelectListItem
                    {
                        Text = item.ShopCode,
                        Value = item.ShopCode
                    });
                }
                model.DDLShop_ShopContract = ShopItems;

                #endregion ShopDropDown
            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return model;

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult fillShopAdd()
        {

            List<SelectListItem> ShopItems = new List<SelectListItem>();


            try
            {
                var reader = mmdc.GetShopCodeByUserId(((UserSec)Session["UserSec"]).UserId).ToList();

                if (reader.Count > 0)
                {
                    foreach (var q in reader)
                    {
                        ShopItems.Add(new SelectListItem { Text = q.ShopCode.ToString() + " - " + q.ShopDescription.ToString(), Value = q.ShopCode.ToString() });
                    }
                }


            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(ShopItems, JsonRequestBehavior.AllowGet);


        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult fillModeAdd()
        {
            #region ModeDropDown
            List<SelectListItem> ModeItems = new List<SelectListItem>();
            try
            {
                var reader = mmdc.GetModes().ToList();

                if (reader.Count > 0)
                {
                    foreach (var q in reader)
                    {
                        ModeItems.Add(new SelectListItem { Text = q.ModeFullDescription.ToString(), Value = q.ModeCode.ToString() });
                    }
                }


            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(ModeItems, JsonRequestBehavior.AllowGet);
            #endregion ModeDropDown
        }
        [HttpPost]
        public JsonResult fillRSByShopMode(string ShopCode, string ModeCode)
        {
            List<ShopLimits> reader = new List<ShopLimits>();
            try
            {
                reader = mmdc.GetRSByShopModes(ShopCode, ModeCode).ToList();

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(reader);
        }
        [HttpPost]
        public JsonResult fillRSByShop(string ShopCode)
        {
            List<ManageMasterDataServiceReference.Shop> reader = new List<ManageMasterDataServiceReference.Shop>();
            try
            {
                reader = mmdc.GetRSByShop(ShopCode).ToList();

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Json(reader);
        }

        [HttpPost]
        public JsonResult InsertShopLimit(string ShopCode, string ModeCode, string SuspendLimit, string ShopMatLimit, string AutoAppLimit)
        {
            string result = "";
            try
            {
                ShopLimits ShopLimitList = new ShopLimits();
                ShopLimitList.ShopCode = ShopCode;
                ShopLimitList.Mode = ModeCode;
                ShopLimitList.RepairAmtLimit = Convert.ToDecimal(SuspendLimit);
                ShopLimitList.ShopMaterialLimit = Convert.ToDecimal(ShopMatLimit);
                ShopLimitList.AutoApproveLimit = Convert.ToDecimal(AutoAppLimit);
                ShopLimitList.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;
                result = mmdc.InsertShopLimit(ShopLimitList);

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Json(result);

        }

        [HttpPost]
        public JsonResult UpdateShopLimit(string ShopCode, string ModeCode, string SuspendLimit, string ShopMatLimit, string AutoAppLimit)
        {
            string result = "";
            try
            {
                ShopLimits ShopLimitList = new ShopLimits();
                ShopLimitList.ShopCode = ShopCode;
                ShopLimitList.Mode = ModeCode;
                ShopLimitList.RepairAmtLimit = Convert.ToDecimal(SuspendLimit);
                ShopLimitList.ShopMaterialLimit = Convert.ToDecimal(ShopMatLimit);
                ShopLimitList.AutoApproveLimit = Convert.ToDecimal(AutoAppLimit);
                ShopLimitList.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;
                result = mmdc.UpdateShopLimit(ShopLimitList);

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                result = "Failed";
            }

            return Json(result);
        }
        [HttpPost]
        public JsonResult UpdateShopProfile(string ShopCode, string ShopActive, string ShopDesc, string ShopType, string VendorCode, string GeoLoc, string RKRPLoc,
            string Part1, string Labor1, string CurrencyCode, string Part2, string Labor2, string EmailAddress, string ImportTax, string Phone, string Discount, string LinkAccount,
            string Acep, string RRIS70SuffixCode, string OTSuspend, string PrepTime, string Decentralized, string BypassLeasedContainerValidations, string AutoComplete)
        {
            string result = "";
            try
            {
                ManageMasterDataServiceReference.Shop ShopList = new ManageMasterDataServiceReference.Shop();
                ShopList.ShopCode = ShopCode;
                ShopList.ShopActiveSW = ShopActive;
                ShopList.ShopDescription = ShopDesc;
                ShopList.ShopTypeCode = ShopType;
                ShopList.VendorCode = VendorCode;
                ShopList.LocationCode = GeoLoc;
                ShopList.RKRPloc = RKRPLoc;
                if (Part1 != "")
                {
                    ShopList.SalesTaxPartCont = Convert.ToDouble(Part1);
                }
                if (Labor1 != "")
                {
                    ShopList.SalesTaxLaborCon = Convert.ToDouble(Labor1);
                }
                ShopList.CUCDN = CurrencyCode;
                if (Part2 != "")
                {
                    ShopList.SalesTaxPartGen = Convert.ToDouble(Part2);
                }
                if (Labor2 != "")
                {
                    ShopList.SalesTaxLaborGen = Convert.ToDouble(Labor2);
                }
                ShopList.EmailAdress = EmailAddress;
                if (ImportTax != "")
                {
                    ShopList.ImportTax = Convert.ToDouble(ImportTax);
                }

                ShopList.Phone = Phone;


                if (Discount != "")
                {
                    ShopList.PCTMaterialFactor = Convert.ToDouble(Discount);
                }
                ShopList.RRISXmitSW = LinkAccount;
                if (Acep != "-1")
                {
                    ShopList.AcepSW = Acep;
                }
                ShopList.RRIS70SuffixCode = RRIS70SuffixCode;
                if (OTSuspend != "-1")
                {
                    ShopList.OvertimeSuspSW = OTSuspend;
                }
                if (PrepTime != "-1")
                {
                    ShopList.PreptimeSW = PrepTime;
                }
                ShopList.Decentralized = Decentralized;
                if (BypassLeasedContainerValidations != "-1")
                {
                    ShopList.BypassLeaseRules = BypassLeasedContainerValidations;
                }
                if (AutoComplete != "-1")
                {
                    ShopList.AutoCompleteSW = AutoComplete;
                }
                ShopList.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;
                result = mmdc.UpdateShopProfile(ShopList);

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                result = "Error in updating the data. Please contact the System Administrator.";
            }
            return Json(result);

        }
        [HttpPost]
        public JsonResult InsertShopProfile(string ShopCode, string ShopActive, string ShopDesc, string ShopType, string VendorCode, string GeoLoc, string RKRPLoc,
            string Part1, string Labor1, string CurrencyCode, string Part2, string Labor2, string EmailAddress, string ImportTax, string Phone, string Discount, string LinkAccount,
            string Acep, string RRIS70SuffixCode, string OTSuspend, string PrepTime, string Decentralized, string BypassLeasedContainerValidations, string AutoComplete)
        {
            string result = "";
            try
            {
                ManageMasterDataServiceReference.Shop ShopList = new ManageMasterDataServiceReference.Shop();
                ShopList.ShopCode = ShopCode;
                ShopList.ShopActiveSW = ShopActive;
                ShopList.ShopDescription = ShopDesc;
                ShopList.ShopTypeCode = ShopType;
                ShopList.VendorCode = VendorCode;
                ShopList.LocationCode = GeoLoc;
                
                if (RKRPLoc == "")
                {
                    ShopList.RKRPloc = GeoLoc.Substring(GeoLoc.Length - Math.Min(3, GeoLoc.Length));
                }
                else
                {
                    ShopList.RKRPloc = RKRPLoc;
                }
                if (Part1 != "")
                {
                    ShopList.SalesTaxPartCont = Convert.ToDouble(Part1);
                }
                if (Labor1 != "")
                {
                    ShopList.SalesTaxLaborCon = Convert.ToDouble(Labor1);
                }
                ShopList.CUCDN = CurrencyCode;
                if (Part2 != "")
                {
                    ShopList.SalesTaxPartGen = Convert.ToDouble(Part2);
                }
                if (Labor2 != "")
                {
                    ShopList.SalesTaxLaborGen = Convert.ToDouble(Labor2);
                }

                ShopList.EmailAdress = EmailAddress;
                if (ImportTax != "")
                {
                    ShopList.ImportTax = Convert.ToDouble(ImportTax);
                }
                ShopList.Phone = Phone;
                if (Discount != "")
                {
                    ShopList.PCTMaterialFactor = Convert.ToDouble(Discount);
                }
                ShopList.RRISXmitSW = LinkAccount;
                if (Acep != "-1")
                {
                    ShopList.AcepSW = Acep;
                }
                ShopList.RRIS70SuffixCode = RRIS70SuffixCode;
                if (OTSuspend != "-1")
                {
                    ShopList.OvertimeSuspSW = OTSuspend;
                }
                if (PrepTime != "-1")
                {
                    ShopList.PreptimeSW = PrepTime;
                }
                ShopList.Decentralized = Decentralized;
                if (BypassLeasedContainerValidations != "-1")
                {
                    ShopList.BypassLeaseRules = BypassLeasedContainerValidations;
                }
                if (AutoComplete != "-1")
                {
                    ShopList.AutoCompleteSW = AutoComplete;
                }
                ShopList.ChangeUser = ((UserSec)Session["UserSec"]).LoginId;
                result = mmdc.InsertShopProfile(ShopList);

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                result = "Error in adding the data. Please contact the System Administrator.";
            }
            return Json(result);

        }
        public ManageMasterShopVndorModel fillShopWithDescription(ManageMasterShopVndorModel model)
        {
            #region ShopDropDown
            List<SelectListItem> ShopItems = new List<SelectListItem>();
            List<ManageMasterDataServiceReference.Shop> ShopList;
            try
            {
                ShopList = mmdc.GetShopByUserId(((UserSec)Session["UserSec"]).UserId).ToList();

                foreach (var item in ShopList)
                {
                    ShopItems.Add(new SelectListItem
                    {
                        Text = item.ShopCode + "-" + item.ShopDescription,
                        Value = item.ShopCode
                    });
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
        public ManageMasterShopVndorModel fillShopProfileWithDescription(ManageMasterShopVndorModel model)
        {
            #region ShopDropDown
            List<SelectListItem> ShopItems = new List<SelectListItem>();
            List<ManageMasterDataServiceReference.Shop> ShopList;
            try
            {
                ShopList = mmdc.GetShopProfileByUserId(((UserSec)Session["UserSec"]).UserId).ToList();

                foreach (var item in ShopList)
                {
                    ShopItems.Add(new SelectListItem
                    {
                        Text = item.ShopCode + "-" + item.ShopDescription,
                        Value = item.ShopCode
                    });
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
        public ManageMasterShopVndorModel fillVendor(ManageMasterShopVndorModel model)
        {
            #region ShopDropDown
            List<SelectListItem> VendorItems = new List<SelectListItem>();
            List<ManageMasterDataServiceReference.Vendor> VendorList;
            try
            {
                VendorList = mmdc.GetRSAllVendors().ToList();

                foreach (var item in VendorList)
                {
                    VendorItems.Add(new SelectListItem
                    {
                        Text = item.VendorCode + " - " + item.VendorDesc,
                        Value = item.VendorCode.Trim()
                    });
                }

                model.DDLVendor = VendorItems;

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return model;
            #endregion ShopDropDown


        }
        public ManageMasterShopVndorModel fillCurrency(ManageMasterShopVndorModel model)
        {

            List<SelectListItem> CurrencyItems = new List<SelectListItem>();
            List<ManageMasterDataServiceReference.Currency> CurrencyList;
            try
            {
                CurrencyList = mmdc.GetRSAllCurrencies().ToList();

                foreach (var item in CurrencyList)
                {
                    CurrencyItems.Add(new SelectListItem
                    {
                        Text = item.Cucdn + "-" + item.CurrName,
                        Value = item.Cucdn
                    });
                }

                model.DDLCurrency = CurrencyItems;

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return model;

        }

        public ManageMasterShopVndorModel fillModeByManualCode(string ManualCode, ManageMasterShopVndorModel model)
        {
            #region ModeDropDown
            List<SelectListItem> ModeItems = new List<SelectListItem>();
            List<ManageMasterDataServiceReference.Mode> ModeList;
            try
            {
                if (ManualCode != null)
                {
                    ModeList = mmdc.GetRSAllManualModes(ManualCode).ToList();
                    foreach (var item in ModeList)
                    {
                        ModeItems.Add(new SelectListItem
                        {
                            Text = item.ModeCode + " - " + item.ModeDescription,
                            Value = item.ModeCode
                        });
                    }
                }
                else
                {
                    ModeList = mmdc.GetModes().ToList();
                    foreach (var item in ModeList)
                    {
                        ModeItems.Add(new SelectListItem
                        {
                            Text = item.ModeFullDescription,
                            Value = item.ModeCode
                        });
                    }
                }



                model.DDLMode_ShopContract = ModeItems;

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return model;
            #endregion ModeDropDown
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetRSAllManualModes(string ManualCode)
        {
            #region ModeDropDown
            List<SelectListItem> ModeItems = new List<SelectListItem>();
            List<ManageMasterDataServiceReference.Mode> ModeList;
            try
            {
                ModeList = mmdc.GetRSAllManualModes(ManualCode).ToList();

                foreach (var item in ModeList)
                {
                    ModeItems.Add(new SelectListItem
                    {
                        Text = item.ModeCode + " - " + item.ModeDescription,
                        Value = item.ModeCode
                    });
                }


            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(ModeItems);
            #endregion ModeDropDown
        }
        public ManageMasterShopVndorModel GetRSAllManual(ManageMasterShopVndorModel model)
        {
            #region ModeDropDown
            List<SelectListItem> ManualItems = new List<SelectListItem>();
            List<ManageMasterDataServiceReference.Manual> ManualList;
            try
            {

                ManualList = mmdc.GetRSAllManual().ToList();
                ManualList.OrderBy(m => m.ManualDesc);
                foreach (var item in ManualList)
                {
                    if (item.ManualCode == "MAER")
                    {
                        ManualItems.Add(new SelectListItem
                        {

                            Text = item.ManualCode + " - " + item.ManualDesc,
                            Value = item.ManualCode

                        });
                    }
                }


                model.DDLManual_ShopContract = ManualItems;

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return model;
            #endregion ModeDropDown
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetShopContCurrencyCode(string ShopCode)
        {
            string reader = "";
            try
            {

                ManageMasterDataServiceReference.Currency objCurrency = new ManageMasterDataServiceReference.Currency();
                objCurrency = mmdc.GetShopContCurrencyCode(ShopCode);
                reader = objCurrency.Cucdn;




            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(reader);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult fillModeByShop(string ShopCode)
        {
            List<SelectListItem> ModeItems = new List<SelectListItem>();
            try
            {
                var reader = mmdc.GetModesByShop(ShopCode).ToList();

                if (reader.Count > 0)
                {
                    foreach (var q in reader)
                    {
                        ModeItems.Add(new SelectListItem { Text = q.ModeCode.ToString() + "-" + q.ModeDescription.ToString(), Value = q.ModeCode.ToString() });
                    }
                }
                else
                {
                    ModeItems.Add(new SelectListItem { Text = "ALL", Value = "" });
                }


            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Json(ModeItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewAdditionalDetails()
        {
            viewAdditionalDetailsModel model = new viewAdditionalDetailsModel();
            ManageMasterDataServiceReference.Currency objCurrency = new ManageMasterDataServiceReference.Currency();
            string WOID = Request.QueryString["RecordID"];
            WorkOrderDetail WOList = new WorkOrderDetail();

            if ((Session["ReviewData"] != null && Session["ReviewData"] != "") || string.IsNullOrEmpty(WOID) || WOID == "0")
            {
                WOList = (WorkOrderDetail)(Session["ReviewData"]);
                if (WOList != null)
                {
                    if (WOList.WorkOrderID == 0)
                    {
                        if (!WOList.IsSingle)
                        {
                            model.WOID = "00000MULTI";
                        }
                    }
                    else if (WOList.WorkOrderID > 0)
                    {
                        if (!WOList.IsSingle)
                        {
                            model.WOID = "00000MULTI";
                        }
                        else
                        {
                            model.WOID = "00" + WOList.WorkOrderID;
                        }
                    }
                }
            }
            else
            {
                WOList = obj_Sercice.GetWOAdditionalDetails(WOID);
                //model.WOID = "00" + WOList.WorkOrderID;
                model.WOID = WOList.WorkOrderID.ToString();
            }

            try
            {

                objCurrency = mmdc.GetShopContCurrencyCode(WOList.Shop.ShopCode);


                model.SHOP_CD = WOList.Shop.ShopCode; // Convert.ToInt32(WOList.Shop.ShopCode);
                model.Vender_Ref_No = WOList.EquipmentList[0].VendorRefNo;
                model.EXCHANGE_RATE = WOList.ExchangeRate;
                model.TOT_PREP_HRS = WOList.TotalPrepHours;
                model.Currency = objCurrency.CurrName;
                model.MANH_RATE_CPH = WOList.ManHourRateCPH;
                model.MANH_RATE = WOList.ManHourRate;
                model.TOT_MANH_REG = Convert.ToDecimal(WOList.TotalManHourReg);
                model.Cal_EMRCost_USD = WOList.ManHourRateCPH * Convert.ToDecimal(WOList.TotalManHourReg);
                model.Cal_ShopCost_USD = (WOList.ExchangeRate / 100) * (WOList.ManHourRate * Convert.ToDecimal(WOList.TotalManHourReg));
                model.Cal_ShopCost_Currency = WOList.ManHourRate * Convert.ToDecimal(WOList.TotalManHourReg);
                model.OT_RATE_CPH = WOList.OverTimeRateCPH;
                model.OT_RATE = WOList.OverTimeRate;
                model.TOT_MANH_OT = Convert.ToDecimal(WOList.TotalManHourOverTime);
                model.Cal_EMRCost_USD_OT = WOList.OverTimeRateCPH * Convert.ToDecimal(WOList.TotalManHourOverTime);
                model.Cal_ShopCost_USD_OT = (WOList.ExchangeRate / 100) * (WOList.OverTimeRate * Convert.ToDecimal(WOList.TotalManHourOverTime));
                model.Cal_ShopCost_Currency_OT = WOList.OverTimeRate * Convert.ToDecimal(WOList.TotalManHourOverTime);
                model.DT_RATE_CPH = WOList.DoubleTimeRateCPH;
                model.DT_RATE = WOList.DoubleTimeRate;
                model.TOT_MANH_DT = Convert.ToDecimal(WOList.TotalManHourDoubleTime);
                model.Cal_EMRCost_USD_DT = WOList.DoubleTimeRateCPH * Convert.ToDecimal(WOList.TotalManHourDoubleTime);
                model.Cal_ShopCost_USD_DT = (WOList.ExchangeRate / 100) * (WOList.DoubleTimeRate * Convert.ToDecimal(WOList.TotalManHourDoubleTime));
                model.Cal_ShopCost_Currency_DT = WOList.DoubleTimeRate * Convert.ToDecimal(WOList.TotalManHourDoubleTime);
                model.MISC_RATE_CPH = WOList.MiscRateCPH;
                model.MISC_RATE = WOList.MiscRate;
                model.TOT_MANH_MISC = Convert.ToDecimal(WOList.TotalManHourMisc);
                model.Cal_EMRCost_USD_MISC = WOList.MiscRateCPH * Convert.ToDecimal(WOList.TotalManHourMisc);
                model.Cal_ShopCost_USD_MISC = (WOList.ExchangeRate / 100) * (WOList.MiscRate * Convert.ToDecimal(WOList.TotalManHourMisc));
                model.Cal_ShopCost_Currency_MISC = WOList.MiscRate * Convert.ToDecimal(WOList.TotalManHourMisc);
                model.TOT_REPAIR_MANH = WOList.TotalRepairManHour;
                model.TOT_LABOR_COST_CPH = WOList.TotalLabourCostCPH;
                model.Cal_TOT_LABOR_COST_Currency = (WOList.ExchangeRate / 100) * WOList.TotalLabourCost;
                model.TOT_LABOR_COST = WOList.TotalLabourCost;
                model.AGENT_PARTS_TAX_CPH = WOList.AgentPartsTaxCPH;
                model.AGENT_PARTS_TAX = WOList.AgentPartsTax;
                model.TOT_MAN_PARTS_CPH = WOList.TotalManPartsCPH;
                model.TOT_MAN_PARTS = WOList.TotalManParts;
                model.TOT_SHOP_AMT_CPH = WOList.TotalShopAmountCPH;
                model.TOT_SHOP_AMT = WOList.TotalShopAmount;
                model.TOT_MAERSK_PARTS_CPH = WOList.TotalMaerksPartsCPH;
                model.TOT_MAERSK_PARTS = WOList.TotalMaerksParts;
                model.CAL_SHOP_AMT_AND_MAN_PARTS = WOList.TotalShopAmount + WOList.TotalManParts;
                model.CAL_SHOP_AMT_CPH_AND_MAN_PARTS_CPH = WOList.TotalShopAmountCPH + WOList.TotalManPartsCPH;
                model.CAL_SHOP_AMT_AND_MAN_PARTS_USD = (WOList.ExchangeRate / 100 * WOList.TotalShopAmount) + (WOList.ExchangeRate / 100 * WOList.TotalManParts);
                model.CAL_SHOP_AMT_USD = WOList.ExchangeRate / 100 * WOList.TotalShopAmount;
                model.SHOP_TYPE_CODE = mmdc.GetShopTypeByShopCode(WOList.Shop.ShopCode);
                model.CAL_TOT_MAN_PARTS_USD = (WOList.ExchangeRate / 100) * WOList.TotalManParts;
                model.SALES_TAX_LABOR_PCT = Convert.ToDecimal(WOList.SalesTaxLaborPCT);
                model.SALES_TAX_LABOR_CPH = WOList.SalesTaxLabourCPH;
                model.SALES_TAX_LABOR = WOList.SalesTaxLabour;
                model.SALES_TAX_PARTS_PCT = Convert.ToDecimal(WOList.SalesTaxPartsPCT);
                model.SALES_TAX_PARTS_CPH = WOList.SalesTaxPartsCPH;
                model.SALES_TAX_PARTS = WOList.SalesTaxParts;
                model.IMPORT_TAX_PCT = Convert.ToDecimal(WOList.ImportTaxPCT);
                model.IMPORT_TAX_CPH = WOList.ImportTaxCPH;
                model.IMPORT_TAX = WOList.ImportTax;
                model.CAL_IMPORT_TAX = WOList.ExchangeRate / 100 * WOList.ImportTax;
                model.CAL_SALES_TAX_PARTS = WOList.ExchangeRate / 100 * WOList.SalesTaxParts;
                model.CAL_SALES_TAX_LABOR = WOList.ExchangeRate / 100 * WOList.SalesTaxLabour;
                model.CAL_SALES_AGENT_CPH = WOList.SalesTaxPartsCPH + WOList.SalesTaxLabourCPH + WOList.AgentPartsTaxCPH;
                model.CAL_SALES_AGENT_PARTS = WOList.AgentPartsTax + WOList.SalesTaxParts + WOList.SalesTaxLabour;
                model.CAL_SALES_AGENT = WOList.ExchangeRate / 100 * (WOList.AgentPartsTax + WOList.SalesTaxParts + WOList.SalesTaxLabour);

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            return View("viewAdditionalDetails", model);
        }
        public ActionResult ShopDetailsByShopCode()
        {
            string ShopCede = Request.QueryString["RecordID"].PadLeft(3, '0');
            ManageMasterShopVndorModel model = new ManageMasterShopVndorModel();
            List<ManageMasterDataServiceReference.Shop> ShopDetails = new List<ManageMasterDataServiceReference.Shop>();
            try
            {
                ShopDetails = mmdc.GetRSByShop(ShopCede).ToList();
                foreach (var shop in ShopDetails)
                {
                    model.ShopCode = shop.ShopCode;
                    model.VendorCode = shop.VendorCode;
                    model.ShopDesc = shop.ShopDescription;
                    model.GEOLocation = shop.LocationCode;
                    model.CUCDN = shop.CUCDN;
                    model.PreparationTime = shop.PreptimeSW;
                    model.ShopActiveSW = shop.ShopActiveSW;
                    model.PCTMaterialFactor = shop.PCTMaterialFactor;
                    model.OTSuspended = shop.OvertimeSuspSW;
                    model.ShopType = shop.ShopTypeCode;
                    model.ImportTax = shop.ImportTax.ToString();
                    model.Parts1 = shop.SalesTaxPartCont.ToString();
                    model.Parts2 = shop.SalesTaxPartGen.ToString();
                    model.Labor1 = shop.SalesTaxLaborCon.ToString();
                    model.Labor2 = shop.SalesTaxLaborGen.ToString();

                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            return View("ShopDetails", model);
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

        public ManageMasterShopVndorModel fillInactiveShop(ManageMasterShopVndorModel model)
        {
            List<SelectListItem> ShopItems = new List<SelectListItem>();
            List<ManageMasterDataServiceReference.Shop> ShopList;
            try
            {
                #region ShopDropDown


                ShopList = (mmdc.GetInactiveShopByUserId(((UserSec)Session["UserSec"]).UserId)).ToList();

                foreach (var item in ShopList)
                {
                    ShopItems.Add(new SelectListItem
                    {
                        Text = item.ShopCode,
                        Value = item.ShopCode
                    });
                }
                model.DDLShop_ShopContract = ShopItems;

                #endregion ShopDropDown
            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return model;

        }
    }
}



