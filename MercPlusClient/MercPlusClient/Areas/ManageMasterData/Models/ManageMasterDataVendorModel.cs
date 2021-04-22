using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataVendorModel
    {
        #region Vendor Properties

        public bool IsUpdate { get; set; }
        public bool IsChange { get; set; }
        public bool IsAdd { get; set; }
        public bool IsFlag { get; set; }

        
        [Display(Name = "MERC Vendor Code")]
        public string VendorCode { get; set; }
        public List<SelectListItem> drpVendorList = new List<SelectListItem>();

        
        [Display(Name = "Vendor Description")]
        public string VendorDesc { get; set; }

        
        [Display(Name = "Country")]
        public string VenCountryCode { get; set; }
        public List<SelectListItem> drpVenCountryList = new List<SelectListItem>();

        
        [Display(Name = "Vendor Active Switch")]
        public string VendorActiveSw { get; set; }
        public List<SelectListItem> drpVendorSwitchList = new List<SelectListItem>();

        [Display(Name = "Change User Name")]
        public string ChangeUserVendor { get; set; }
        public string ChangeFUserVendor { get; set; }
        public string ChangeLUserVendor { get; set; }

        [Display(Name = "Changed Time")]
        public string ChangeTimeVendor { get; set; }




        #endregion Vendor Properties

    }
}