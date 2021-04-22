using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterLocationModel
    {

        public bool IsUpdateLoc { get; set; }

        [Display(Name = "Location Code Query")]
        public string LocCodeQuery { get; set; }

        [Display(Name = "Location Code")]
        public string LocCode { get; set; }

        public bool Flag { get; set; }
        

        [Display(Name = "Location Description")]
        public string LocDesc { get; set; }


        [Display(Name = "Country Code")]
        public string CountryLocCode { get; set; }

        [Display(Name = "Contact CENEQULOS")]
        public string ContactEqsalSW { get; set; }
        public List<SelectListItem> drpContactEqsalSW = new List<SelectListItem>();

        [Display(Name = "Region Code")]
        public string RegionCode { get; set; }
        public List<SelectListItem> RegionList { get; set; }
        public List<SelectListItem> drpRegionCode = new List<SelectListItem>();

        [Display(Name = "Change User Name")]
        public string ChangeUserLoc { get; set; }

        [Display(Name = "Changed Time")]
        public string ChangeTimeLoc { get; set; }

       
    }
}