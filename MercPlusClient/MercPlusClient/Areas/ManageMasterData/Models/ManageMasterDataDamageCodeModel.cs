using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataDamageCodeModel
    {
        public List<SelectListItem> drpDamagecode { get; set; }
        public string DamageCedexCode { get; set; }
        public string DamageDescription { get; set; }
        public string DamageName { get; set; }
        public string DamageNumericalCode { get; set; }
        public string DamageCHUser { get; set; }
        public string DamageCHTS { get; set; }
        public bool IsDamgeUpdate { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAddMode { get; set; }
        public bool IsViewMode { get; set; }
        public string DamageCode { get; set; }
    }
}