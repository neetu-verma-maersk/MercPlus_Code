using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataSpecialRemarksModel
    {
        
        public string RemarksID { get; set; }
        public string RemarksID1 { get; set; }
        public string RKEMProfile { get; set; }
        public string SerialNoFrom { get; set; }
        public string SerialNoTo { get; set; }
        public string ChangeUser { get; set; }
        public string DisplaySW { get; set; }
        public string Remarks { get; set; }
        public string ChangeTime { get; set; }
        public string RepairCeiling { get; set; }
        public bool IsInsertMode { get; set; }
        public IEnumerable<SelectListItem> drpProfile { get; set; }
        public IEnumerable<SelectListItem> drpRange { get; set; }
        public List<SelectListItem> drpDisplay { get; set; }
        public string PageTitle { get; set; }
        public bool IsProfileSelected { get; set; }
        public bool ShowDetails { get; set; }
        public string RemarksMessages { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsCPH { get; set; }
        public bool IsDeleted { get; set; }
    }
}