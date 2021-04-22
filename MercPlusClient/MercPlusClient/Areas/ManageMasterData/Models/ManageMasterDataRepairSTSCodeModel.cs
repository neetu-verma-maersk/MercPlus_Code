using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataRepairSTSCodeModel
    {
        public string RepairCode { get; set; }
        public string ModeCode { get; set; }
        public string ManualCode { get; set; }
        public string RepairDesc { get; set; }
        public string RkrpRepairCode { get; set; }
        public string ChangeUser { get; set; }
        public Nullable<short> MaxQuantity { get; set; }
        public string ShopMaterialCeiling { get; set; }
        public string ChangeTime { get; set; }
        public string RepairInd { get; set; }
        public Nullable<double> ManHour { get; set; }
        public string RepairActiveSW { get; set; }
        public string MultipleUpdateSW { get; set; }
        public Nullable<double> WarrantyPeriod { get; set; }
        public string TaxAppliedSW { get; set; }
        public Nullable<short> RepairPriority { get; set; }
        public Nullable<int> IndexID { get; set; }
        public string AllowPartsSW { get; set; }
        public IEnumerable<SelectListItem> drpManual { get; set; }
        public IEnumerable<SelectListItem> drpMode { get; set; }
        public IEnumerable<SelectListItem> drpIndex { get; set; }
        public IEnumerable<SelectListItem> drpRepairCode { get; set; }
        public IEnumerable<SelectListItem> drpRepairCodeEdit { get; set; }
        public List<SelectListItem> drpYesNo { get; set; }
        public string PrepTime { get; set; }
        public enum RepairCodeSelectionModes {RCManualCode,RepairCode,Record }
        public RepairCodeSelectionModes RepairCodeSelectionMode { get; set; }
        public bool IsAddMode { get; set; }
        public string Msg { get; set; }
        public string ChangeUserFName { get; set; }
        public string ChangeUserLName { get; set; }
        public bool IsAfterAddDeleteMode { get; set; }
    }
}