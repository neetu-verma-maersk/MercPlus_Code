using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MercPlusClient.ManageReportsServiceReference;

namespace MercPlusClient.Areas.ManageReports.Models
{
    public class ManageReportsModel
    {
        private int _rId;
        private string hiddenManual = string.Empty;
        public string radio { get; set; }
        public enum ReportsEnum
        {
            //[EnumMember(Value = "MERCA01 - Billing report for CPH")]
            //MERCA01 = 1,
            //[EnumMember(Value = "MERCA02 - Billing report for agent")]
            //MERCA02 = 2,
            //[EnumMember(Value = "MERCA03 - Billing report for shop")]
            //MERCA03 = 3,
            //[EnumMember(Value = "MERCB01 - Spare part usage pr container")]
            //MERCB01 = 4,
            //[EnumMember(Value = "MERCC01 - Exclusionary Codes")]
            //MERCC01 = 5,
            //[EnumMember(Value = "MERCC02 - Mode/Code/part number associations")]
            //MERCC02 = 6,
            //[EnumMember(Value = "MERCD03 - Shop Contract")]
            //MERCD03 = 7,
            //[EnumMember(Value = "MERCD05 - Country Contract")]
            //MERCD05 = 8,
            //[EnumMember(Value = "MERCE01 - Equipment Repair Status")]
            //MERCE01 = 9

            //[Description("MERCA01 - Billing report for CPH")]
            //MERCA01 = 1,
            //[Description("MERCA02 - Billing report for agent")]
            //MERCA02 = 2,
            //[Description("MERCA03 - Billing report for shop")]
            //MERCA03 = 3,
            //[Description("MERCB01 - Spare part usage pr container")]
            //MERCB01 = 4,
            //[Description("MERCC01 - Exclusionary Codes")]
            //MERCC01 = 5,
            //[Description("MERCC02 - Mode/Code/part number associations")]
            //MERCC02 = 6,
            //[Description("MERCD03 - Shop Contract")]
            //MERCD03 = 7,
            //[Description("MERCD05 - Country Contract")]
            //MERCD05 = 8,
            //[Description("MERCE01 - Equipment Repair Status")]
            //MERCE01 = 9
        }
        public string HiddenManual 
        {
            get;
            set;
        }
        public string HiddenMode { get; set; }
        public string HiddenRepairCode { get; set; }
        public int ReportsID
        {
            get { return _rId; }
            set { _rId = value; }
        }

        [Required(ErrorMessage = "Date From cannot be left blank")]
        [Display(Name = "Date From")]
        public string DateFrom { get; set; }

        [Required(ErrorMessage = "Date To cannot be left blank")]
        [Display(Name = "Date To")]
        public string DateTo { get; set; }

        [Display(Name = "Customer")]
        public string Customer { get; set; }
        public List<SelectListItem> drpCustomer = new List<SelectListItem>();

        [Display(Name = "Country")]
        public string Country { get; set; }
        public List<SelectListItem> drpCountry = new List<SelectListItem>();

        [Display(Name = "Manual")]
        public string Manual { get; set; }
        public List<SelectListItem> drpManual = new List<SelectListItem>();

        [Display(Name = "Shop")]
        public string Shop { get; set; }
        public List<SelectListItem> drpShop = new List<SelectListItem>();

        [Display(Name = "Mode")]
        public string Mode { get; set; }
        public List<SelectListItem> drpMode = new List<SelectListItem>();

        [Display(Name = "Reports")]
        public string Reports { get; set; }
        public List<SelectListItem> drpReports = new List<SelectListItem>();

        [Display(Name = "STSCode")]
        public string STSCode { get; set; }
        public List<SelectListItem> drpSTSCode = new List<SelectListItem>();

        [Display(Name = "Area")]
        public string Area { get; set; }
        public List<SelectListItem> drpArea = new List<SelectListItem>();

        [Display(Name = "Type")]
        public string txtType { get; set; }

        [Display(Name = "Days")]
        public string Days { get; set; }
        public List<SelectListItem> drpDays = new List<SelectListItem>();

        [Display(Name = "Repair Code")]
        public string RepairCode { get; set; }

        [Display(Name = "Repair Code Description")]
        public string RepairCodeDescription { get; set; }

        [Display(Name = "Exclusionary Repair Code")]
        public string ExclusionaryRepairCode { get; set; }

        [Display(Name = "Exclusionary Repair Code Description")]
        public string ExclusionaryRepairCodeDescription { get; set; }

        [Display(Name = "Associated PartNumber")]
        public string AssociatedPartNumber { get; set; }

        [Display(Name = "Part Description")]
        public string PartDescription { get; set; }

        public List<Reports> ReportsList { get; set; }

        public bool isAdmin { get; set; }
        public bool isCPH { get; set; }
        public bool isEMRSpecialistCountry { get; set; }
        public bool isEMRSpecialistShop { get; set; }
        public bool isEMRApproverCountry { get; set; }
        public bool isEMRApproverShop { get; set; }
        public bool isShop { get; set; }
        public bool isMPROCluster { get; set; }
        public bool isMPROShop { get; set; }
        public bool isReadOnly { get; set; }

        public bool isAnyCPH { get; set; }
    }
}