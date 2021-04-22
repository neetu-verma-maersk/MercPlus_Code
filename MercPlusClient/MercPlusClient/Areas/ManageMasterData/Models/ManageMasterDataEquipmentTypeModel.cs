using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataEquipmentTypeModel
    {
        #region EquipmentTypeEntry Properties

        public bool IsUpdate { get; set; }

       
        [Display(Name = "Equipment Type")]
        public string EquipmentType { get; set; }
        public List<SelectListItem> drpEquipmentTypeList { get; set; }

        [Display(Name = "Equipment Description")]
        public string EqDesc { get; set; }

        [Display(Name = "Change User Name")]
        public string ChangeUser { get; set; }
        public string ChangeFUser { get; set; }
        public string ChangeLUser { get; set; }

        [Display(Name = "Changed Time")]
        public string ChTime { get; set; }

        #endregion EquipmentTypeEntry Properties
    }
}