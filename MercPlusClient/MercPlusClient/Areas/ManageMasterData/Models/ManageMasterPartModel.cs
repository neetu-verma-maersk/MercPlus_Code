using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterPartModel
    {

        #region Amlan -  Master Part

        public string PartGroupCodeList { get; set ; }
        public List<SelectListItem> drpPartGroupCodeList{ get; set; }
        public string PartDesignation1 { get; set; }
        public string PartDesignation2 { get; set; }
        public string PartDesignation3 { get; set; }
        public string ManufacturerCodeList { get; set; }
        public List<SelectListItem> drpManufacturerCodeList { get; set; }
        public string PartNumber { get; set; }
        public int Quantity { get; set; }
        public string PartDescription { get; set; }
        public decimal Amount { get; set; }
        public string TAG { get; set; }
        public List<SelectListItem> drpTAGList { get; set; }
        public decimal CoreValue { get; set; }
        public string DeductCoreValueList { get; set; }
        public List<SelectListItem> drpDeductCoreValueList { get; set; }
        public string Maersk { get; set; }
        public List<SelectListItem> drpMaerskList { get; set; }
        public string Active { get; set; }
        public List<SelectListItem> drpActiveList { get; set; }
        public string Comment { get; set; }
        public string ChangeUserName { get; set; }
        public string ChangeTime { get; set; }

        public string Message { get; set; }

        public List<GridMasterPartModel> GridMasterPartModelList { get; set; }
        public bool showQuery { get; set; }
        public bool showQueryResult { get; set; }
        public bool isQuerySuccess { get; set; }
        public bool showEdit { get; set; }
        public bool showAdd { get; set; }
        public bool isAddSuccess { get; set; }
        public bool isEditSuccess{ get; set; }
        public bool isDeleteSuccess{ get; set; }
        public string isAddMode { get; set; }
        public bool isEditMode { get; set; }
        public string isEdit { get; set; }
       
        public SearchMasterPartModel SearchMasterPartModel { get; set; }
      
    }

    public class GridMasterPartModel
    {
        public string PartCode { get; set; }
        public string PartGroupCode { get; set; }
        public string ManufacturerCode { get; set; }
        public string Quantity { get; set; }
        public string PartDescription { get; set; }

        public string PartPrice { get; set; }
        public string Designation { get; set; }
        public string Active { get; set; }
        public string Core { get; set; }
        public string CoreValue { get; set; }
        public string DeductCore { get; set; }
    }

    public class SearchMasterPartModel
    {
        public string PartCode { get; set; }
        public string PartGroupCode { get; set; }
        public List<SelectListItem> drpPartGroupCode { get; set; }
        public string ManufacturerCode { get; set; }
        public List<SelectListItem> drpManufacturerCode { get; set; }
        public string Designation { get; set; }
        public string PartDescription { get; set; }

        public string Active { get; set; }
        public List<SelectListItem> drpActive { get; set; }
        public string Core { get; set; }
        public List<SelectListItem> drpCore { get; set; }
        public string DeductCoreValue { get; set; }
        public List<SelectListItem> drpDeductCoreValue { get; set; }

        public string SearchResultMessage { get; set; }
    }


     #endregion Amlan -  Master Part
}