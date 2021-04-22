using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataManualModeModel
    {
        public string ModeCode { get; set; }
        public string ModeDescription { get; set; }
        public string ManualCode { get; set; }
        public string ManualDescription { get; set; }
        public string ActiveSw { get; set; }
        public string ChangeUser { get; set; }
        public string ChangeFUser { get; set; }
        public string ChangeLUser { get; set; }
        public string ChangeTime { get; set; }
        public bool IsAddMode { get; set; }
        public bool IsShowMode { get; set; }
        public bool IsShowManual { get; set; }
        public bool IsAdded { get; set; }
        public bool IsAddfailed { get; set; }
        public IEnumerable<SelectListItem> drpManual { get; set; }
        public IEnumerable<SelectListItem> drpMode { get; set; }
        public List<SelectListItem> drpYesNo { get; set; }
        public string Msg { get; set; }
    }
}