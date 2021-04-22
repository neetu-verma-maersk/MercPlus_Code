using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataModel
    {


        public bool IsUpdate { get; set; }
         


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

     
         public List<SelectListItem> drpManufacturerList { get; set; }
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
         [Display(Name = "Manufacturer Code")]
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

        
        
       
        /*#region PrepTime properties

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
        */
         //public List<SelectListItem> drpManufacturerList { get; set; }
         //public string txtManufacturerList { get; set; }
         //public int SelectedManufacturerList { get; set; }
         //public int ControllerStatus { get; set; }


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

        // Created by Afroz
         #region Pay Agent Vendor Properties

         //public List<SelectListItem> ddlPayAgent_CDList { get; set; }
         [Display(Name = "Corporate Pay Agent Code")]
         public string VPayAgent_CD { get; set; }
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
        //End Afroz

        #region CountryLabourRate Properties

        public List<ManageMasterDataModel> SearchResult { get; set; }

        #region Control Propeties

        [Display(Name = "Country")]
        public string Country { get; set; }
        public List<SelectListItem> drpCountry = new List<SelectListItem>();

        [Display(Name = "Equipment Type")]
     //   public string Equipment_Type { get; set; }
        public List<SelectListItem> drpEquipmentType = new List<SelectListItem>();


      //  public List<SelectListItem> drpCountryList = new List<SelectListItem>();

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

        [Display(Name = "Mode List")]
        public string Mode_List { get; set; }


        public string txtEqSize { get; set; }
        public string txtModeList { get; set; }

        public List<SelectListItem> drpEqSize = new List<SelectListItem>();
        public List<SelectListItem> drpMode_List = new List<SelectListItem>();
        public List<SelectListItem> ddlEqSize = new List<SelectListItem>();
        public List<SelectListItem> ddlMode_List = new List<SelectListItem>();

        [Display(Name = "Eq Size")]
        public string Eq_Size { get; set; }

        //[Display(Name = "Mode")]
        //public string Mode { get; set; }

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