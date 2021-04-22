using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Web.Security;
using System.Web.Mvc;


namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterShopVndorModel
    {
        public List<SelectListItem> DDLShop = new List<SelectListItem>();
        public List<SelectListItem> DDLMode = new List<SelectListItem>();
        public List<SelectListItem> DDLShopCodeAdd = new List<SelectListItem>();
        public List<SelectListItem> DDLShopModeAdd = new List<SelectListItem>();
        public List<SelectListItem> DDLShopActive = new List<SelectListItem>();
        public List<SelectListItem> DDLVendor = new List<SelectListItem>();
        public List<SelectListItem> DDLCurrency = new List<SelectListItem>();
        public List<SelectListItem> DDLManual = new List<SelectListItem>();
        [Display(Name = "Shop Mode")]
        public string Mode { get; set; }
        [Display(Name = "Shop Code")]
        public string ShopCode { get; set; }
        [Display(Name = "Manual")]
        public string ManualCode { get; set; }
        [Display(Name = "Repair Code")]
        public string RepairCode { get; set; }
        [Display(Name = "Shop Code")]
        public string ShopCodeAdd { get; set; }
        [Display(Name = "Shop Mode")]
        public string ShopModeAdd { get; set; }
        [Display(Name = "Suspend Limit")]
        public string SuspendLimit { get; set; }
        [Display(Name = "ShopMaterialLimit")]
        public string ShopMaterialLimit { get; set; }
        [Display(Name = "AutoApprovalLimit")]
        public string AutoApprovalLimit { get; set; }
        [Display(Name = "Currency")]
        public Nullable<decimal> CurrencyAmount { get; set; }
        [Display(Name = "Shop Code")]
        public string ShopCodeUpdate { get; set; }
        [Display(Name = "Shop Mode")]
        public string ModeCodeUpdate { get; set; }
        [Display(Name = "Mode Description")]
        public string ModeDescUpdate { get; set; }
        [Display(Name = "Suspend Limit")]
        public Nullable<decimal> SuspendLimitUpdate { get; set; }
        [Display(Name = "Shop Material Limit")]
        public Nullable<decimal> ShopMaterialLimitUpdate { get; set; }
        [Display(Name = "Auto Approval Limit")]
        public Nullable<decimal> AutoApprovalLimitUpdate { get; set; }
        [Display(Name = "Change User Name")]
        public string ChangeUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Change Time")]
        public System.DateTime ChangeTime { get; set; }


        public int ShopContID { get; set; }
        public Nullable<decimal> ContractAmount { get; set; }
        //[DataType(DataType.Date)]
        public string EffDate { get; set; }
        //[DataType(DataType.Date)]
        public string ExpDate { get; set; }
        // public string ManualCode { get; set; }
        public string ShopActiveSW { get; set; }
        public string CUCDN { get; set; }
        public string CurCode { get; set; }


        public List<ManageMasterShopVndorModel> SearchResults { get; set; }
        public string SearchButton { get; set; }

        [Display(Name = "Shop Active")]
        public string ShopActive { get; set; }
        [Display(Name = "Shop Description")]
        public string ShopDesc { get; set; }
        [Display(Name = "Shop Type")]
        public string ShopType { get; set; }
        [Display(Name = "Vendor Code")]
        public string VendorCode { get; set; }
        public string VendorDesc { get; set; }
        [Display(Name = "Currency Code")]
        public string CurrencyCode { get; set; }
        public string CurrencyDesc { get; set; }
        [Display(Name = "GEO Location")]
        public string GEOLocation { get; set; }
        [Display(Name = "RKRP Location")]
        public string RKRPLocation { get; set; }
        public string Parts1 { get; set; }
        public string Labor1 { get; set; }
        public string Parts2 { get; set; }
        public string Labor2 { get; set; }
        [Display(Name = "E-mail Address")]
        public string EmailAddress { get; set; }
        [Display(Name = "Import Tax")]
        public string ImportTax { get; set; }
        [Display(Name = "Phone")]
        public string Phone { get; set; }
        [Display(Name = "Discount/Markup")]
        public string Discount { get; set; }
        [Display(Name = "Link To Accounting")]
        public string LinkAccount { get; set; }
        [Display(Name = "ACEP")]
        public string Acep { get; set; }
        [Display(Name = "RRIS70 Suffix Code")]
        public string RRIS70SuffixCode { get; set; }
        [Display(Name = "OT Suspended")]
        public string OTSuspended { get; set; }
        [Display(Name = "Change User Name")]
        public string ChangeUserName { get; set; }
        [Display(Name = "Preparation Time")]
        public string PreparationTime { get; set; }
        [Display(Name = "Changed Time")]
        public string ChangedTime { get; set; }
        [Display(Name = "Decentralized")]
        public string Decentralized { get; set; }
        //[Display(Name = "Bypass Leased Container Validations")]
        [Display(Name = "Bypass All Validations Except CPH Limits")]
        public string BypassLeasedContainerValidations { get; set; }
        [Display(Name = "Auto Complete")]
        public string AutoComplete { get; set; }

        #region "ShopProfile"

        //[Required(ErrorMessage = "Please select a Shop Code to Update")]
        [Display(Name = "Shop Code")]
        public string ShopCode_Profile { get; set; }
        public List<SelectListItem> DDLShop_ShopProfile = new List<SelectListItem>();

        [Display(Name = "Discount/MarkUp - All Manufacturers")]
        public double? PCTMaterialFactor { get; set; }

        [Display(Name = "Manufacturer Code")]
        public string Manufacturer1 { get; set; }
        public List<SelectListItem> DDL_Manufacturer1 = new List<SelectListItem>();
        [Display(Name = "Discount/Markup")]
        public double? Discount1 { get; set; }
        [Display(Name = "Manufacturer Code")]
        public string Manufacturer2 { get; set; }
        public List<SelectListItem> DDL_Manufacturer2 = new List<SelectListItem>();
        [Display(Name = "Discount/Markup")]
        public double? Discount2 { get; set; }
        [Display(Name = "Manufacturer Code")]
        public string Manufacturer3 { get; set; }
        public List<SelectListItem> DDL_Manufacturer3 = new List<SelectListItem>();
        [Display(Name = "Discount/Markup")]
        public double? Discount3 { get; set; }
        [Display(Name = "Manufacturer Code")]
        public string Manufacturer4 { get; set; }
        public List<SelectListItem> DDL_Manufacturer4 = new List<SelectListItem>();
        [Display(Name = "Discount/Markup")]
        public double? Discount4 { get; set; }
        [Display(Name = "Manufacturer Code")]
        public string Manufacturer5 { get; set; }
        public List<SelectListItem> DDL_Manufacturer5 = new List<SelectListItem>();
        [Display(Name = "Discount/Markup")]
        public double? Discount5 { get; set; }
        [Display(Name = "Manufacturer Code")]
        public string Manufacturer6 { get; set; }
        public List<SelectListItem> DDL_Manufacturer6 = new List<SelectListItem>();
        [Display(Name = "Discount/Markup")]
        public double? Discount6 { get; set; }
        [Display(Name = "Manufacturer Code")]
        public string Manufacturer7 { get; set; }
        public List<SelectListItem> DDL_Manufacturer7 = new List<SelectListItem>();
        [Display(Name = "Discount/Markup")]
        public double? Discount7 { get; set; }
        [Display(Name = "Manufacturer Code")]
        public string Manufacturer8 { get; set; }
        public List<SelectListItem> DDL_Manufacturer8 = new List<SelectListItem>();
        [Display(Name = "Discount/Markup")]
        public double? Discount8 { get; set; }
        [Display(Name = "Manufacturer Code")]
        public string Manufacturer9 { get; set; }
        public List<SelectListItem> DDL_Manufacturer9 = new List<SelectListItem>();
        [Display(Name = "Discount/Markup")]
        public double? Discount9 { get; set; }
        [Display(Name = "Manufacturer Code")]
        public string Manufacturer10 { get; set; }
        public List<SelectListItem> DDL_Manufacturer10 = new List<SelectListItem>();
        [Display(Name = "Discount/Markup")]
        public double? Discount10 { get; set; }
        #endregion "ShopProfile"

        #region "ShopContract"
        [Display(Name = "Shop")]
        // [Required(ErrorMessage = "Please select a Shop Code")]
        public string ShopCode_ShopContract { get; set; }
        [Display(Name = "Manual")]
        //[Required(ErrorMessage = "Please select a Manual")]
        public string ManualCode_ShopContract { get; set; }
        [Display(Name = "Mode")]
        //[Required(ErrorMessage = "Please select a Mode")]
        public string Mode_ShopContract { get; set; }

        [Display(Name = "Repair Code")]
        public string RepairCode_ShopContract { get; set; }
        //[Required]
        //[Required(ErrorMessage = "Please enter a Repair Code")]

        [Display(Name = "Amount")]
        //[Required(ErrorMessage = "Please enter a Contract Amount")]
        //[RegularExpression(@"^\d+.\d{0,2}$", ErrorMessage = "Please Enter Numbers with 2 Decimal Places Only")]
        public Nullable<decimal> Amount_ShopContract { get; set; }
        [Display(Name = "Currency")]
        public string Currency_ShopContract { get; set; }
        [Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        //[Required(ErrorMessage = "Please enter an Effective Date")]
        public System.DateTime EffectiveDate_ShopContract { get; set; }
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public System.DateTime ExpireDate_ShopContract { get; set; }
        public int intedit { get; set; }
        public int intflag { get; set; }

        public List<SelectListItem> DDLShop_ShopContract = new List<SelectListItem>();
        public List<SelectListItem> DDLMode_ShopContract = new List<SelectListItem>();
        public List<SelectListItem> DDLManual_ShopContract = new List<SelectListItem>();
        #endregion  ShopContract

        public bool IsUpdate { get; set; }
    }
}