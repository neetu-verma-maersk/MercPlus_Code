using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataSuspendModel
    {
        #region Suspend Properties

        public bool IsDelShop { get; set; }
        public bool IsAddShop { get; set; }
        public bool IsDelMode { get; set; }
        public bool IsAddMode { get; set; }
        public bool IsDelManual { get; set; }
        public bool IsAddManual { get; set; }
        public bool IsDelRepair { get; set; }
        public bool IsAddRepair { get; set; }
        public bool IsDelete { get; set; }
        public bool IsCreate { get; set; }
        public bool FlagManual { get; set; }
        public bool FlagMode { get; set; }
        public bool FlagRepairCode { get; set; }
        public bool IsFlag { get; set; }


       
        [Display(Name = "Shop Code")]
        public string ShopCode { get; set; }
        public List<SelectListItem> drpShopCodeList = new List<SelectListItem>();

       
        [Display(Name = "Manual Code")]
        public string Manual { get; set; }
        public List<SelectListItem> drpManualCodeList = new List<SelectListItem>();

      
        [Display(Name = "Mode")]
        public string Mode { get; set; }
        public List<SelectListItem> drpModeSusList = new List<SelectListItem>();

      
        [Display(Name = "Repair Code")]
        public string RepairCod { get; set; }
        public List<SelectListItem> drpRepairCodeList = new List<SelectListItem>();

        
        [Display(Name = "Suspend Category ID")]
        public string SuspendCatID { get; set; }
        public List<SelectListItem> drpSuspendList = new List<SelectListItem>();

        [Display(Name = "Change User Name")]
        public string ChangeUserSp { get; set; }
        public string ChangeFUserSp { get; set; }
        public string ChangeLUserSp { get; set; }
        

        [Display(Name = "Changed Time")]
        public string ChangeTimeSp { get; set; }


        #endregion Suspend Properties
    }
}