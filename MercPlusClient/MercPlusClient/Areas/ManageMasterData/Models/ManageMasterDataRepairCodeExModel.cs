using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataRepairCodeExModel
    {
        #region ExclusivrRepairCodes Properties

        public bool IsManual { get; set; }
        public bool IsMode { get; set; }
        public bool IsRepair { get; set; }
        public bool IsDelete { get; set; }
        public bool IsCreate { get; set; }
        public bool IsAdd { get; set; }
        public bool FlagMode { get; set; }
        public bool FlagRepair { get; set; }
      

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
        public string ChangeFUserRpr { get; set; }
        public string ChangeLUserRpr { get; set; }

        [Display(Name = "Changed Time")]
        public string ChangeTimeRpr { get; set; }

        #endregion ExclusiveRepairCodes Properties


    }
}