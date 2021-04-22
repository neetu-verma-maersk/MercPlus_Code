using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Web.Security;
using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ContainerGrade
    {
        public int ContainerGradeId { get; set; }
        public int WorkOrderId { get; set; }
        public int ContainerId { get; set; }
        public string ContainerType { get; set; }
        public string ContainerSize { get; set; }
        public string ContainerProfile { get; set; }
        public string ContainerInService { get; set; }
        public string ContainerDamageCode { get; set; }       
        public string ContainerCurrentLocation { get; set; }
        public string GradeCode { get; set; }       
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

        public List<SelectListItem> DDLShop = new List<SelectListItem>();
        public List<SelectListItem> DDLMode = new List<SelectListItem>();
        public List<SelectListItem> DDLGrade = new List<SelectListItem>();

        [Display(Name = "Shop Mode")]
        public string Mode { get; set; }
        [Display(Name = "Shop Code")]
        public string ShopCode { get; set; }
        [Display(Name = "Manual")]
        public string ManualCode { get; set; }
        public string EQIoflt { get; set; }
    }
}