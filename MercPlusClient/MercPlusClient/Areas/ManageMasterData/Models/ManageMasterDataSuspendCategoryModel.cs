using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataSuspendCategoryModel
    {
        #region Suspend Category Properties

        public bool IsUpdate { get; set; }
        public bool IsAdd { get; set; }
        public bool IsCreate { get; set; }

        [Required(ErrorMessage = "Please Select a Suspend Category ID")]
        [Display(Name = "Suspend Category ID")]
        public int SuspcatID { get; set; }
        public List<SelectListItem> drpSuspendCatList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Enter a New Suspend Category Description")]
        [Display(Name = "Suspend Category Description")]
        public string SuspcatDesc { get; set; }

        [Display(Name = "Change User")]
        public string ChangeUserSus { get; set; }
        public string ChangeFUserSus { get; set; }
        public string ChangeLUserSus { get; set; }

        [Display(Name = "Change Time")]
        public string ChangeTimeSus { get; set; }

        #endregion Suspend Category Properties
    }
}