using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataTransmitModel
    {
        #region Transmit Properties

        public bool IsCust { get; set; }
        public bool IsMode { get; set; }
        public bool IsCreate { get; set; }
        public bool IsUpdate { get; set; }

        [Display(Name = "Customer Code")]
        public string Customer { get; set; }
        public List<SelectListItem> drpCustomerCodeList = new List<SelectListItem>();


        [Display(Name = "Mode")]
        public string ModeCode { get; set; }
        public List<SelectListItem> drpModeLCodeList = new List<SelectListItem>();

    
        [Display(Name = "RRIS Transmit Switch")]
        public string RRISXMITSwitch { get; set; }
        public List<SelectListItem> drpRRISList = new List<SelectListItem>();

        [Required(ErrorMessage = "Please enter an RRIS Expense Account Code")]
        [StringLength(6, ErrorMessage = "The Account Code must be less than 6 characters")]
        [Display(Name = "RRIS Expense Account Code")]
        public string AccountCode { get; set; }

        [Display(Name = "Change User Name")]
        public string ChangeUserTransmit { get; set; }
        public string ChangeFUserTransmit { get; set; }
        public string ChangeLUserTransmit { get; set; }

        [Display(Name = "Changed Time")]
        public string ChangeTimeTransmit { get; set; }

        #endregion Transmit Properties
    }
}