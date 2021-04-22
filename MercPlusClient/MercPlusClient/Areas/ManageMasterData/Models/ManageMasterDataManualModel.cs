using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataManualModel
    {
        public List<SelectListItem> drpManual { get; set; }

        public bool IsManualUpdate { get; set; }
        public List<SelectListItem> drpManualActiveSwitch { get; set; }
        public List<SelectListItem> drpManualIndicator { get; set; }
        public string ManualFullDesc { get; set; }
        public string ManualDesc { get; set; }

        public string ManualActiveSW { get; set; }
        public string ManualChTime { get; set; }

        public string Manual { get; set; }

        public string txtManual { get; set; }
        public string ManualChangeUser { get; set; }
        public string ManualChangeUserFName { get; set; }
        public string ManualChangeUserLName { get; set; }
    }
}