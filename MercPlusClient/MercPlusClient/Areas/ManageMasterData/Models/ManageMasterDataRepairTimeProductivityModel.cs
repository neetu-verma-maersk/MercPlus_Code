using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataRepairTimeProductivityModel
    {        
        public bool IsLocation { get; set; }
        
        public bool Flag { get; set; }

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

        public List<ManageMasterDataRepairTimeProductivityModel> SearchResult { get; set; }

        [Display(Name = "Date From:")]
        public string DateFrom { get; set; }

        [Display(Name = "Date To:")]
        public string DateTo { get; set; }

        public List<SelectListItem> RadioButtonListItem = new List<SelectListItem>();
        public string SelectedAnswer { get; set; }

        [Display(Name = "Country")]
        public string txtCountry { get; set; }
        public List<SelectListItem> drpCountry = new List<SelectListItem>();

        [Display(Name = "Location")]
        public string txtLocation { get; set; }
        public List<SelectListItem> drpLocation = new List<SelectListItem>();

        [Display(Name = "Shop")]
        public string txtShop { get; set; }
        public List<SelectListItem> drpShop = new List<SelectListItem>();

        #region Grid Properties
        public string Location { get; set; }
        public string Shop { get; set; }
        public string AvgEstimateTime { get; set; }
        public string AvgAuthoriseTime { get; set; }
        public string AvgRepairTime { get; set; }
        #endregion Grid Properties







    }
}