using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Web.Security;
using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterCountryLabourRateModel
    {

        #region CountryLabourRate Properties

        public List<ManageMasterDataModel> SearchResult { get; set; }

        #region Control Propeties

        [Display(Name = "Country")]
        public string Country { get; set; }
        public List<SelectListItem> drpCountry = new List<SelectListItem>();

        [Display(Name = "Equipment Type")]
        public string EquipmentType { get; set; }
        public List<SelectListItem> drpEquipmentType = new List<SelectListItem>();

        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }
        public List<SelectListItem> drpCountryList = new List<SelectListItem>();

        #endregion

        #region Grid Properties

        public string COUNTRYNAME { get; set; }
        public string EQUIPMENT_TYPE { get; set; }
        public string EFFECTIVE_DATE { get; set; }
        public string EXPIRATION_DATE { get; set; }
        public string ORDINARY_RATE { get; set; }
        public string OT1_RATE { get; set; }
        public string OT2_RATE { get; set; }
        public string OT3_RATE { get; set; }

        #endregion Grid Properties

        #endregion


    }
}