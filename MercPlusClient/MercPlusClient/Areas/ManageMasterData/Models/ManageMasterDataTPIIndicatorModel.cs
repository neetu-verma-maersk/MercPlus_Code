using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataTPIIndicatorModel
    {
        public List<SelectListItem> drpTPI { get; set; }
        public List<SelectListItem> drpCategory { get; set; }
        public string TPICedexCode { get; set; }
        public string TPIDescription { get; set; }
        public string TPIFullDescription { get; set; }
        public string TPIName { get; set; }
        public string TPINumericalCode { get; set; }
        public string TPICHUser { get; set; }
        public string TPICHTS { get; set; }
        public bool IsTPIUpdate { get; set; }
        public string Category { get; set; }
        public bool IsAdmin { get; set; }
        public bool ShowData { get; set; }
        public bool IsTPIAdd { get; set; }
        public bool IsTPIView { get; set; }
        public string TPISelectedCedexCode { get; set; }
        public bool IsTPIDeleted { get; set; }
        public bool IsTPIAdded { get; set; }
    }
}