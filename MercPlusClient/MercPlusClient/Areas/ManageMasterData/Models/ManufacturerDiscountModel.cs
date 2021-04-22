using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManufacturerDiscountModel
    {
        #region MasterDiscount Properties
        public bool IsUpdate { get; set; }
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
        [StringLength(5)]
        [Display(Name = "Discount Percentage")]
        public string DiscountPercentage { get; set; }

        [Display(Name = "Change User Name")]
        public string ChangeUser_Name { get; set; }

        [Display(Name = "Changed Time")]

        [DataType(DataType.DateTime)]
        public DateTime Changed_Time { get; set; }

        public string ChangeTime { get; set; }
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
    }
}