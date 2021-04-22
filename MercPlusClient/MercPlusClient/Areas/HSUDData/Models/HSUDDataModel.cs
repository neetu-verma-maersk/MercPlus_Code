using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.HSUDData.Models
{
    public class HSUDDataModel
    {
        #region Control Properties
        public string EquipmentNo { get; set; }
          

        #endregion Control Properties
        #region Grid Properties

        public string Equipment_ID { get; set; }
        public string Equipment_Type { get; set; }
        public string Estimate_Number { get; set; }
        public string Estimate_Status { get; set; }
        public string Summary_Size_Type { get; set; }
        public string Estimate_Original_Date { get; set; }
        public string Approved_date { get; set; }
        public string Cancelled_date { get; set; }
        public int Count { get; set; }
        public List<HSUDDataModel> SearchResults { get; set; }

        #endregion Grid Properties

        public string ErrorMsg { get; set; } /*I want to dynamically change the value of the error message*/


        //public List<EstLifeCycle_ApprovalCanceledModel> Approve_Canceled { get; set; }
        //public List<EstLifeCycleAnalysisModel> Lifecycle { get; set; }
        //public List<EstLineItemAnalysisModel> EstLineItem { get; set; }
    }
}