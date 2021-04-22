using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataModeModel
    {
        public List<SelectListItem> drpMode { get; set; }

        public bool IsModeUpdate { get; set; }
        public List<SelectListItem> drpModeActiveSwitch { get; set; }
        public List<SelectListItem> drpModeIndicator { get; set; }
        public string ModeFullDescription { get; set; }
        public string ModeDescription { get; set; }
        public string ModeInd { get; set; }
        public string ModeActiveSwitch { get; set; }
        public string ModeChangeTime { get; set; }

        public string Mode { get; set; }

        public string txtMode { get; set; }
        public string ModeChangeUser { get; set; }
        public string ModeChangeUserFName { get; set; }
        public string ModeChangeUserLName { get; set; }
        public bool IsModeAdded { get; set; }
    }
}