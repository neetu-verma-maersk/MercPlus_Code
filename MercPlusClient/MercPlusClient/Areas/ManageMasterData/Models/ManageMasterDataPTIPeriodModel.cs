using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using MercPlusClient.ManageMasterDataServiceReference;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataPTIPeriodModel
    {
        #region PTIPeriod
        public bool IsEditPTIPeriod { get; set; }
        public string PTIPeriodFrom { get; set; }
        public string PTIPeriodTo { get; set; }
        public int PTIPeriodNumber { get; set; }
        public List<PTIPeriod> FilterPTIPeriods { get; set; }
        public int MaxPTIPeriod { get; set; }
        public string PTIMessage { get; set; }
        public string PTIChangedUser { get; set; }
        public string PTIUpdateDate { get; set; }
        public bool ShowGrid { get; set; }
        public bool EditDefaultPTIPeriod { get; set; }
        #endregion PTIPeriod
    }
}