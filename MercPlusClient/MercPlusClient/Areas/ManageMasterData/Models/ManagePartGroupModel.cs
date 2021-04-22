using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManagePartGroupModel
    {

        #region Amlan -  Part Group

        public List<SelectListItem> drpPartGroupList { get; set; }
        public string PartGroupCode { get; set; }
        public string PartGroupCodeList { get; set; }
        public string PartGroupDescription { get; set; }
        public string PartGroupComment { get; set; }
        public string IsPartGroupActive { get; set; }
        public string PartGroupCodeCreate { get; set; }
        public string IsPartGruopAddMode { get; set; }
        public string DateChanged { get; set; }
        public string UserChanged { get; set; }
        public List<SelectListItem> drpPartGroupActive { get; set; }
        public string Message { get; set; }
        public string IsPartGruopHideDetail { get; set; }
        public string ChangeUserName { get; set; }
        public string ChangeTime { get; set; }
        public bool showQueryResult { get; set; }
        public bool showResult { get; set; }
        public bool showAdd { get; set; }
        #endregion Amlan -  Part Group

    }
}