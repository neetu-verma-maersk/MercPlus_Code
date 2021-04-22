using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataIndexModel
    {
        public bool IsInsertMode { get; set; }
        public IEnumerable<SelectListItem> drpIndexManual { get; set; }
        public IEnumerable<SelectListItem> drpIndexMode { get; set; }
        public IEnumerable<SelectListItem> drpIndex { get; set; }
        public string IndexDesc { get; set; }
        public string IndexPriority { get; set; }
        public string IndexManualCode { get; set; }
        public string IndexMode { get; set; }
        public string IndexID { get; set; }
        public string IndexChangeUserName { get; set; }
        public string IndexChangeTime { get; set; }
        public enum IndexSelectionModes {IndexManualCode,IndexMode,Index,IndexRecord }
        public IndexSelectionModes IndexSelectionMode { get; set; }
        public string IndexChangeUserFName { get; set; }
        public string IndexChangeUserLName { get; set; }
    }
}