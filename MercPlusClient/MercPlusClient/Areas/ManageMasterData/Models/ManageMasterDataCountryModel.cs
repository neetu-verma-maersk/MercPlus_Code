using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataCountryModel
    {
        #region Country Properties

        public bool IsShow { get; set; }

        [Display(Name = "Country Code List")]
        public string Country { get; set; }

        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }
        public List<SelectListItem> drpCountryList = new List<SelectListItem>();

        [Display(Name = "Country Description")]
        public string CountryDescription { get; set; }


        [Display(Name = "COCL Code")]
        public string AreaCode { get; set; }

        [Display(Name = "CPH Limit Adjustment Factor")]
        public Nullable<double> RepairLimitAdjFactor { get; set; }

        [Display(Name = "Change User")]
        public string ChangeUserCon { get; set; }

        [Display(Name = "Change Time")]
        public string ChangeTimeCon { get; set; }

        #endregion Country Properties
    }
}