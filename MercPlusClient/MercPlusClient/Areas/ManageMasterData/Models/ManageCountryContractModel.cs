using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageCountryContractModel
    {
        public string countryList { get; set; }
        public List<SelectListItem> drpCountryList { get; set; }

        public string modeList { get; set; }
        public List<SelectListItem> drpModeList { get; set; }

        public string RepairCode { get; set; }

        public string Message { get; set; }

        public List<GridCountryContractModel> GridCountryContractModelList { get; set; }

        public bool IsShowGrid { get; set; }

    }

    public class GridCountryContractModel
    {
        public string CountryCode { get; set; }
        public string RepairCode { get; set; }
        public string ContractAmount { get; set; }
        public string CUCDN { get; set; }
        public string ManualCode { get; set; }
        public string Mode { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpiryDate { get; set; }
    }


}