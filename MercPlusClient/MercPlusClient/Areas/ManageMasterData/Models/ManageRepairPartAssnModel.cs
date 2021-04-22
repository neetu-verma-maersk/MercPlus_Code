using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageRepairPartAssnModel
    {

        public string RepairCod { get; set; }

        public string ModeCode { get; set; }

        public string ManualCode { get; set; }

        public string RepairDesc { get; set; }

        public string PartNumber { get; set; }

        public string PartDesc { get; set; }

        public string MaxPartQty { get; set; }

        public string ChangeUser { get; set; }

        public string ChangeTime { get; set; }

        public string Message { get; set; }

        public List<SelectListItem> drpManualCode { get; set; }
        public List<SelectListItem> drpModeCode { get; set; }


        public List<GridRepairPartAssnModel> GridRepairPartAssnModelList { get; set; }

        public bool showQuery { get; set; }
        public bool showQueryResult { get; set; }
        public bool isQuerySuccess { get; set; }
        public bool showEdit { get; set; }
        public bool showAdd { get; set; }
        public bool isAddSuccess { get; set; }
        public bool isEditSuccess { get; set; }
        public bool isDeleteSuccess { get; set; }
        //public bool isAddMode { get; set; }
        public bool isEditMode { get; set; }
        public string isEdit { get; set; }
        public string isAdd { get; set; }


        public SearchRepairPartAssnModel SearchRepairPartAssnModel { get; set; }

        public string OrgRepairCod { get; set; }

        public string OrgModeCode { get; set; }

        public string OrgManualCode { get; set; }

        public string OrgPartNumber { get; set; }

        public string OrgMaxPartQty { get; set; }


    }

    public class GridRepairPartAssnModel
    {
        public string GridRepairCod { get; set; }

        public string GridModeCode { get; set; }

        public string GridManualCode { get; set; }

        public string GridRepairDesc { get; set; }

        public string GridPartNumber { get; set; }

        public string GridPartDesc { get; set; }

        public string GridMaxPartQty { get; set; }
    }


    public class SearchRepairPartAssnModel
    {
        public string ScrhRepairCod { get; set; }

        public string ScrhModeCode { get; set; }

        public string ScrhManualCode { get; set; }

        public string ScrhPartNumber { get; set; }

        public List<SelectListItem> drpManualCode { get; set; }
        public List<SelectListItem> drpModeCode { get; set; }
    }
}