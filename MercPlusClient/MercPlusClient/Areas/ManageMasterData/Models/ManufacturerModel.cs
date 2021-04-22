using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManufacturerModel
    {
        #region MasterDiscount Properties
        public bool IsUpdate { get; set; }
        public List<SelectListItem> drpManufacturerList { get; set; }
        public List<SelectListItem> drpManufacturerListC { get; set; }
        public string txtManufacturerList { get; set; }
        public int SelectedManufacturerList { get; set; }
        public string txtManufacturerCode { get; set; }
        public string txtManufacturerName { get; set; }
        public string txtDiscountPercentage { get; set; }

        public string txtChangeUserName { get; set; }
        public string txtChangeTime { get; set; }

        [Display(Name = "Manufacturer List")]
        public string ManufacturerList { get; set; }

        [Required(ErrorMessage = "Manufacturer Name is required")]
        [Display(Name = "Manufacturer Name")]
        public string ManufacturerName { get; set; }

        [StringLength(3)]
        [Required(ErrorMessage = "Manufacturer Code is required")]
        [Display(Name = "Manufacturer")]
        public string ManufacturerCode { get; set; }

        [Range(0.01, 100.00,
            ErrorMessage = "Discount Percentage must be between 0.01 and 100.00")]
        [Display(Name = "Discount Percentage")]
        public string DiscountPercentage { get; set; }

        [Display(Name = "Change User Name")]
        public string ChangeUser_Name { get; set; }

        [Display(Name = "Changed Time")]

        [DataType(DataType.DateTime)]
        public DateTime Changed_Time { get; set; }

        public int ControllerStatus { get; set; }

        //------------------Model---------------------------
        [Display(Name = "Model Number")]
        public string ModelNo { get; set; }

        [Range(1, 10,
           ErrorMessage = "Repair Code Digit must be between 1 to 10")]
        [Display(Name = "Repair Code Digit")]
        public int RepaireCode { get; set; }
        //--------------------------------------

        #endregion MasterDiscount Properties

        #region PayAgent Properties

        public List<SelectListItem> drpRRISCodesList { get; set; }
        public string txtRRISCode { get; set; }
        public int SelectedRRISCode { get; set; }
        public List<SelectListItem> drpRRISFormat { get; set; }
        public string txtRRISFormat { get; set; }
        public string CorporateProfitCenter { get; set; }
        public string CorporatePayAgentCode { get; set; }
        public string PayAgentProfitCenter { get; set; }
        public string ChangeUserName { get; set; }
        public string ChangeTime { get; set; }
        //rohit added 
      
        [Display(Name = "RRIS Code")]
        public string RRISCode { get; set; }

        [Display(Name = "RRIS Format")]
        public string RRISFormat { get; set; }

        #endregion PayAgent Properties


        #region EquipmentTypeEntry Properties

        [Required(ErrorMessage = "Please Enter a New Equipment Type")]
        [Display(Name = "Equipment Type")]
        public string EquipmentType { get; set; }
        public List<SelectListItem> drpEquipmentTypeList { get; set; }

        [Required(ErrorMessage = "Please Enter an Equipment Description")]
        [Display(Name = "Equipment Description")]
        public string EquipmentDescription { get; set; }


        public string ChangeUser { get; set; }
        public string ChTime { get; set; }

        #endregion EquipmentTypeEntry Properties


        #region RepairLocation properties
        public List<SelectListItem> drpRepairLocationcode { get; set; }
        public string RepairCode { get; set; }
        public string RepairDescription { get; set; }
        public string Description { get; set; }
        #endregion RepairLocation properties


        #region ModeEntry properties

        public List<SelectListItem> drpEqTypeList { get; set; }
        public string txtEqType { get; set; }
        public List<SelectListItem> drpSubTypeList { get; set; }
        public string txtSubType { get; set; }
        public List<SelectListItem> drpSizeList { get; set; }
        public string txtSize { get; set; }
        public List<SelectListItem> drpAluminumList { get; set; }
        public string txtAluminum { get; set; }
        public List<SelectListItem> drpModeList { get; set; }
        public string txtMode { get; set; }

        #endregion ModeEntry properties


        #region Customer Properties

        //public bool IsUpdateCust { get; set; }
        [Required(ErrorMessage = "Please enter a new Customer Code")]
        [Display(Name = "Customer Code")]
        public string CustomerCode { get; set; }
        public List<SelectListItem> drpCustomerList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please enter a new Customer Name")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        // public List<SelectListItem> drpCustomer = new List<SelectListItem>();

        [Required(ErrorMessage = "Please select a Manual code")]
        [Display(Name = "Manual Code")]
        public string ManualCode { get; set; }
        public List<SelectListItem> drpManualList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Select a Customer Active Switch")]
        [Display(Name = "Customer Active Switch")]
        public string CustomerActiveSw { get; set; }
        public List<SelectListItem> drpCustActvSwtchList = new List<SelectListItem>();

        [Display(Name = "Change User Name")]
        public string changeUserCust { get; set; }
        [Display(Name = "Changed Name")]
        public string changeTimeCust { get; set; }

        #endregion Customer Properties


        #region Location Properties


        [Display(Name = "Location Code Query")]
        public string LocCodeQuery { get; set; }

        [Display(Name = "Location Code")]
        public string LocCode { get; set; }


        [Display(Name = "Location Description")]
        public string LocDesc { get; set; }


        [Display(Name = "Country Code")]
        public string CountryLocCode { get; set; }

        //[Display(Name = "Contact CENEQULOS")]
        public string ContactEqsalSW { get; set; }
        public List<SelectListItem> drpContactEqsalSW = new List<SelectListItem>();

        [Display(Name = "Region Code")]
        public string RegionCode { get; set; }
        List<SelectListItem> RegionList { get; set; }
        public List<SelectListItem> drpRegionCode = new List<SelectListItem>();

        public string ChangeUserLoc { get; set; }

        public string ChangeTimeLoc { get; set; }

        #endregion Location Properties


        #region Country Properties




        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }
        public List<SelectListItem> drpCountryList = new List<SelectListItem>();
        //public List<SelectListItem> drpCountryList = new List<SelectListItem>();


        [Display(Name = "Country Description")]
        public string CountryDescription { get; set; }


        [Display(Name = "COCL Code")]
        public string AreaCode { get; set; }

        [Display(Name = "CPH Limit Adjustment Factor")]
        public Nullable<double> RepairLimitAdjFactor { get; set; }

        public string ChangeUserCon { get; set; }

        public string ChangeTimeCon { get; set; }

        #endregion Country Properties


        #region Customer_Shop_Mode
        #region Control Properties

        public int count { get; set; }
        public List<SelectListItem> ShopLists { get; set; }
        public List<SelectListItem> CustomerList { get; set; }
        public List<SelectListItem> ModeList { get; set; }

        #endregion Control Properties
        #region Grid Properties

        public string CUSTOMER_CD { get; set; }
        public string SHOP_CD { get; set; }
        public string MODE_Grid { get; set; }
        public string PAYAGENT_CD { get; set; }
        public string CORP_PAYAGENT_CD { get; set; }
        public string RRIS_FORMAT { get; set; }
        public string SUB_PROFIT_CENTER { get; set; }
        public string PROFIT_CENTER { get; set; }
        public string ACCOUNT_CD { get; set; }

        #endregion Grid Properties

        #endregion Customer_Shop_Mode


        #region Transmit Properties

        public bool IsCust { get; set; }
        public bool IsMode { get; set; }
        public bool IsCreate { get; set; }

        [Required(ErrorMessage = "Please Select a Customer")]
        [Display(Name = "Customer Code")]
        public string Customer { get; set; }
        public List<SelectListItem> drpCustomerCodeList = new List<SelectListItem>();


        [Required(ErrorMessage = "Please Select a Mode")]
        [Display(Name = "Mode")]
        public string ModeCode { get; set; }
        public List<SelectListItem> drpModeLCodeList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Set your RRIS Transmit Switch")]
        [Display(Name = "RRIS Transmit Switch")]
        public string RRISXMITSwitch { get; set; }
        public List<SelectListItem> drpRRISList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please enter an RRIS Expense Account Code")]
        [StringLength(6, ErrorMessage = "The Account Code must be less than 6 characters")]
        [Display(Name = "RRIS Expense Account Code")]
        public string AccountCode { get; set; }

        [Display(Name = "Change User Name")]
        public string ChangeUserTransmit { get; set; }

        public string ChangeTimeTransmit { get; set; }

        #endregion Transmit Properties


        #region Vendor Properties

        [Required(ErrorMessage = "Please Select a MERC Vendor Code")]
        [Display(Name = "MERC Vendor Code")]
        public string VendorCode { get; set; }
        public List<SelectListItem> drpVendorList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Enter a Vendor Description")]
        [Display(Name = "Vendor Description")]
        public string VendorDesc { get; set; }

        [Required(ErrorMessage = "Please Select a Country")]
        [Display(Name = "Country")]
        public string VenCountryCode { get; set; }
        public List<SelectListItem> drpVenCountryList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Set your Vendor Active Switch")]
        [Display(Name = "Vendor Active Switch")]
        public string VendorActiveSw { get; set; }
        public List<SelectListItem> drpVendorSwitchList = new List<SelectListItem>();

        [Display(Name = "Change User Name")]
        public string ChangeUserVendor { get; set; }

        [Display(Name = "Changed Time")]
        public string ChangeTimeVendor { get; set; }




        #endregion Vendor Properties


        #region Suspend Category Properties

        [Required(ErrorMessage = "Please Select a Suspend Category ID")]
        [Display(Name = "Suspend Category ID")]
        public int SuspcatID { get; set; }
        public List<SelectListItem> drpSuspendCatList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Enter a New Suspend Category Description")]
        [Display(Name = "Suspend Category Description")]
        public string SuspcatDesc { get; set; }

        public string ChangeUserSus { get; set; }
        public string ChangeTimeSus { get; set; }

        #endregion Suspend Category Properties


        #region Suspend Properties

        public bool IsShop { get; set; }
        public bool IsManual { get; set; }
        public bool IsRepair { get; set; }
        public bool IsDelete { get; set; }
        public bool Flag { get; set; }


        [Required(ErrorMessage = "Please Select a Shop Code")]
        [Display(Name = "Shop Code")]
        public string ShopCode { get; set; }
        public List<SelectListItem> drpShopCodeList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Select a Manual Code")]
        [Display(Name = "Manual Code")]
        public string Manual { get; set; }
        public List<SelectListItem> drpManualCodeList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Select a Mode")]
        [Display(Name = "Mode")]
        public string Mode { get; set; }
        public List<SelectListItem> drpModeSusList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Select a Repair Code")]
        [Display(Name = "Repair Code")]
        public string RepairCod { get; set; }
        public List<SelectListItem> drpRepairCodeList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Select a Customer")]
        [Display(Name = "Suspend Category ID")]
        public string SuspendCatID { get; set; }
        public List<SelectListItem> drpSuspendList = new List<SelectListItem>();

        public string ChangeUserSp { get; set; }
        public string ChangeTimeSp { get; set; }


        #endregion Suspend Properties


        #region ExclusivrRepairCodes Properties

        public bool IsAdd { get; set; }

        [Required(ErrorMessage = "Please Select a Manual Code")]
        [Display(Name = "Manual Code")]
        public string ManCode { get; set; }
        public List<SelectListItem> drpManCodeList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Select a Mode")]
        [Display(Name = "Mode")]
        public string ModCode { get; set; }
        public List<SelectListItem> drpModCodeList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Select a Repair Code")]
        [Display(Name = "Repair Code")]
        public string RepCode { get; set; }
        public List<SelectListItem> drpRepCodeList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Enter an Exclusive Repair Code")]
        [Display(Name = "Exclusive Repair Code")]
        public string ExcluRepairCode { get; set; }

        [Display(Name = "Change User Name")]
        public string ChangeUserRpr { get; set; }

        [Display(Name = "Changed Time")]
        public string ChangeTimeRpr { get; set; }

        #endregion ExclusiveRepairCodes Properties
    
        #region PrepTime properties

        //public string ModeCode { get; set; }
        public string ModeDescription { get; set; }
        public string PrepCd { get; set; }

        public double PrepTimeMax { get; set; }

        public Nullable<double> PrepHrs { get; set; }

        public string ChUser { get; set; }


        public List<SelectListItem> drpModenList { get; set; }

        [Display(Name = "Select Mode")]
        public string ModenList { get; set; }
        public int SelectedModenList { get; set; }


        public List<SelectListItem> drpModePList { get; set; }

        [Display(Name = "Select PrepTimeMax")]
        public string ModePList { get; set; }
        public int SelectedModePList { get; set; }


        #endregion
        
        #region LaborRate

        // public string ShopCode { get; set; }
        public List<SelectListItem> drpShopList { get; set; }

        [Display(Name = "Shop")]
        public string ShopList { get; set; }
        public int SelectedShopList { get; set; }


        public string Customercode { get; set; }
        public List<SelectListItem> drpCustomerLists { get; set; }

        [Display(Name = "Customer")]
        public string CustomerLists { get; set; }
        public int SelectedCustomerLists { get; set; }


        public List<SelectListItem> drpEqupList { get; set; }

        [Display(Name = "Equipment Type")]
        public string EqupList { get; set; }
        public int SelectedEqupList { get; set; }


        public string Eqtype { get; set; }
        [Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        public DateTime Effdate { get; set; }

        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime Expdate { get; set; }
        //[Display(Name = "Customer Code")]
        //public string CustomerCode { get; set; }
        [Display(Name = "Ordinary Rate")]
        public double RegularRate { get; set; }
        [Display(Name = "OT1")]
        public double OvertimeRate { get; set; }
        [Display(Name = "OT2")]
        public double DoubletimeRate { get; set; }
        [Display(Name = "OT3")]
        public double MiscRate { get; set; }

        [Display(Name = "Currency Code")]
        public string CurrCode { get; set; }
        [Display(Name = "Effective Date")]

        public string _sEffdate { get; set; }

        [Display(Name = "Expiry Date")]

        public string _sExpdate { get; set; }




        #endregion


        #region Pay Agent Vendor Properties

        //public List<SelectListItem> ddlPayAgent_CDList { get; set; }
        [Display(Name = "Corporate Pay Agent Code")]
        public string PayAgent_CD { get; set; }
        public List<SelectListItem> ddlPayAgent_CD = new List<SelectListItem>();

        [Display(Name = "MERC Vendor Code")]
        public string SMerc_Vendor_CD { get; set; }
        public List<SelectListItem> ddlSMerc_Vendor_CD = new List<SelectListItem>();


        [Display(Name = "Corporate Pay Agent Code")]
        public string NPayAgent_CD { get; set; }
        public List<SelectListItem> ddlNPayAgent_CD = new List<SelectListItem>();

        [Display(Name = "MERC Vendor Code")]
        public string NMerc_Vendor_CD { get; set; }
        public List<SelectListItem> ddlNMerc_Vendor_CD = new List<SelectListItem>();
        [Display(Name = "RRIS Payment Account Code")]
        public string NRRIS_Acctount_CD { get; set; }

        [Display(Name = "RRIS Supplier Code")]
        public string NRRIS_Supplier_CD { get; set; }


        public string Payment_Method { get; set; }

        [Display(Name = "Corporate Pay Agent Code")]
        public string MPayAgent_CD { get; set; }

        [Display(Name = "MERC Vendor Code")]
        public string MMerc_Vendor_CD { get; set; }

        [Display(Name = "RRIS Payment Account Code")]
        public string MRRIS_Acctount_CD { get; set; }

        [Display(Name = "RRIS Supplier Code")]
        public string MRRIS_Supplier_CD { get; set; }

        [Display(Name = "Change User Name")]
        public string MCHUser { get; set; }
        public string MCHUserType { get; set; }
        public string txtUser { get; set; }

        [Display(Name = "Changed Time")]
        [DataType(DataType.DateTime)]
        public DateTime MCHTS { get; set; }




        #endregion Pay Agent Vendor Properties


        #region CountryLabourRate Properties

        public List<ManageMasterDataModel> SearchResult { get; set; }

        #region Control Propeties

        [Display(Name = "Country")]
        public string Country { get; set; }
        public List<SelectListItem> drpCountry = new List<SelectListItem>();

        [Display(Name = "Equipment Type")]
        //   public string Equipment_Type { get; set; }
        public List<SelectListItem> drpEquipmentType = new List<SelectListItem>();

        #endregion

        #region Grid Properties

        public string COUNTRYNAME { get; set; }
        public string EQUIPMENT_TYPE { get; set; }
        public string EFFECTIVE_DATE { get; set; }
        public string EXPIRATION_DATE { get; set; }
        public string ORDINARY_RATE { get; set; }
        public string OT1_RATE { get; set; }
        public string OT2_RATE { get; set; }
        public string OT3_RATE { get; set; }

        #endregion Grid Properties

        #endregion


        #region CPH ApprovalLevel Properties

        public string UsrRole { get; set; }
        public string UsrId { get; set; }

        [Display(Name = "Eq Size")]
        public string EqSize { get; set; }
        public List<SelectListItem> drpEqSize = new List<SelectListItem>();

        [Display(Name = "Mode List")]
        public string Mode_List { get; set; }
        public List<SelectListItem> drpMode = new List<SelectListItem>();

        [Display(Name = "Eq Size")]
        public string Eq_Size { get; set; }

        [Display(Name = "Mode")]
        public string CPHMode { get; set; }

        [Display(Name = "Message")]
        public string strMsg { get; set; }

        public string txtLimitAmt1 { get; set; }

        public string txtLimitAmt2 { get; set; }

        public string txtLimitAmt3 { get; set; }

        public string txtLimitAmt4 { get; set; }

        public string txtLimitAmt5 { get; set; }

        public string txtLimitAmt6 { get; set; }

        public string txtLimitAmt7 { get; set; }

        public string txtChangeUser1 { get; set; }

        public string txtChangeUser2 { get; set; }

        public string txtChangeUser3 { get; set; }

        public string txtChangeUser4 { get; set; }

        public string txtChangeUser5 { get; set; }

        public string txtChangeUser6 { get; set; }

        public string txtChangeUser7 { get; set; }

        public string txtChangeTime1 { get; set; }

        public string txtChangeTime2 { get; set; }

        public string txtChangeTime3 { get; set; }

        public string txtChangeTime4 { get; set; }

        public string txtChangeTime5 { get; set; }

        public string txtChangeTime6 { get; set; }

        public string txtChangeTime7 { get; set; }


        #endregion
    }
}