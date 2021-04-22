using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Web.Security;
using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageEqtypeModeEntry
    {

        public List<ManageEqtypeModeEntry> SearchData { get; set; }

        #region Control Propeties

        [Display(Name = "EqType")]
        public string EqType { get; set; }
        public List<SelectListItem> drpEqType = new List<SelectListItem>();

        [Display(Name = "SubType")]
        public string SubType { get; set; }
        public List<SelectListItem> drpSubType = new List<SelectListItem>();

        [Display(Name = "Size")]
        public string Size { get; set; }
        public List<SelectListItem> drpSize = new List<SelectListItem>();

        [Display(Name = "Aluminum")]
        public string Aluminum { get; set; }
        public List<SelectListItem> drpAluminum = new List<SelectListItem>();

        #endregion

        #region Grid Properties


        public string EQ_EQUIPMENT { get; set; }
        public string EQ_SUB_TYPE { get; set; }
        public string EQ_SIZE { get; set; }
        public string EQ_MATERIAL { get; set; }
        public string EQ_MODE { get; set; }
        public string EQ_MODE_ID { get; set; }
        public string EQ_EDIT { get; set; }

        #endregion Grid Properties

        #region Add/Edit Properties

        public bool IsAdd { get; set; }

        [Display(Name = "EqId")]
        public string txtEquipmentId { get; set; }

        [Display(Name = "EqType")]
        public string txtEqType { get; set; }
        public List<SelectListItem> ddlEqType = new List<SelectListItem>();

        [Display(Name = "SubType")]
        public string txtSubType { get; set; }
        public List<SelectListItem> ddlSubType = new List<SelectListItem>();

        [Display(Name = "Size")]
        public string txtSize { get; set; }
        public List<SelectListItem> ddlSize = new List<SelectListItem>();

        [Display(Name = "Aluminum")]
        public string txtAluminum { get; set; }
        public List<SelectListItem> ddlAluminum = new List<SelectListItem>();

        [Display(Name = "Aluminum")]
        public string txtMode { get; set; }
        public List<SelectListItem> ddlMode = new List<SelectListItem>();

        [Display(Name = "ChangeUserName")]
        public string txtChangeUserName { get; set; }

        [Display(Name = "ChangedTime")]
        public string txtChangedTime { get; set; }



        #endregion
    }
}