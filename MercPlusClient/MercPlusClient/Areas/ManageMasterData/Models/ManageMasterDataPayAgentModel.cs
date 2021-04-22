using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataPayAgentModel
    {
        #region PayAgent Properties

     
        public bool IsUpdate { get; set; }

        [Display(Name = "RRIS Pay Agent Code")]
        public string RRISCode { get; set; }
        public List<SelectListItem> drpRRISCodesList { get; set; }

        [Display(Name = "RRIS Format")]
        public string RRISFormat { get; set; }
        public List<SelectListItem> drpRRISFormat { get; set; }

        [Display(Name = "Corporate Profit Center")]
        public string CorporateProfitCenter { get; set; }

        [Display(Name = "Corporate PayAgent Code")]
        public string CorporatePayAgentCenter { get; set; }

        [Display(Name = "Pay Agent Profit Center")]
        public string PayAgentProfitCenter { get; set; }

        [Display(Name = "Change User Name")]
        public string ChangeUserName { get; set; }
        public string ChangeFUser { get; set; }
        public string ChangeLUser { get; set; }

        [Display(Name = "Changed Time")]
        public string ChangeTime { get; set; }


        #endregion PayAgent Properties
    }
}