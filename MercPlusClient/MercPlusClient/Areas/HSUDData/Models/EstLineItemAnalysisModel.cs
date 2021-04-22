using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MercPlusClient.Areas.HSUDData.Models
{
    public class EstLineItemAnalysisModel
    {

        #region EstLineItemAnalysis

        public string FACILITYCODE { get; set; }
        public string FACILITYNAME { get; set; }
        public string EQUIPMENTTYPE { get; set; }
        public string SUMMARYSIZETYPE { get; set; }
        public string EQUIPMENTID { get; set; }
        public string SENDERESTIMATEID { get; set; }
        public Nullable<System.DateTime> ESTIMATEDATE { get; set; }
        public Nullable<System.DateTime> APPROVALDATE { get; set; }
        public string CREATEDBYUSER { get; set; }
        public Nullable<int> ISSURVEYREQUESTED { get; set; }
        public int LINEITEMNUMBER { get; set; }
        public string COMPONENTCODE { get; set; }
        public string LOCATIONCODE { get; set; }
        public string REPAIRCODE { get; set; }
        public string DAMAGECODE { get; set; }
        public string MATERIALCODE { get; set; }
        public string UNITOFMEASURE { get; set; }
        public Nullable<int> QUANTITY { get; set; }
        public Nullable<int> LENGTH { get; set; }
        public Nullable<int> WIDTH { get; set; }
        public string ORGANIZATIONTYPE { get; set; }
        public string BASECURRENCYCODE { get; set; }
        public Nullable<decimal> LABORHOURS { get; set; }
        public Nullable<decimal> LABORRATEBASE { get; set; }
        public Nullable<decimal> LABORCOSTBASE { get; set; }
        public Nullable<decimal> MATERIALCOSTBASE { get; set; }
        public Nullable<decimal> TOTALBASE { get; set; }

       // public List<EstLineItemAnalysisModel> EstLineItem { get; set; }
        #endregion
    }
}