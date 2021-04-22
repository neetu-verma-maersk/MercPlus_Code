using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataCustomerModel
    {
        #region Customer Properties

        public bool IsUpdate { get; set; }
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
        public string changeFUserCust { get; set; }
        public string changeLUserCust { get; set; }

        [Display(Name = "Changed Time")]
        public string changeTimeCust { get; set; }

        #endregion Customer Properties
    }
}