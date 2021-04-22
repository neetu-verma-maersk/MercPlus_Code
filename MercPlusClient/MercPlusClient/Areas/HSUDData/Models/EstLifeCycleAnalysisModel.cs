using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MercPlusClient.Areas.HSUDData.Models
{
    public class EstLifeCycleAnalysisModel
    {

        #region EstLifeCycleAnalysis

        public string Facility_Code { get; set; }
        public string Facility_Name { get; set; }
        public string EquipmentType { get; set; }
        public string Summary_Size_Type { get; set; }
        public string EquimentID { get; set; }
        public string EstimateNumber { get; set; }
        public string Estimate_Status { get; set; }
        public Nullable<System.DateTime> Estimate_Original_Date { get; set; }
        public Nullable<System.DateTime> Estimate_Transmission_date { get; set; }
        public string Base_Currency_Original { get; set; }
        public Nullable<decimal> Owner_total_labor_original { get; set; }
        public Nullable<decimal> Owner_total_material_original { get; set; }
        public Nullable<decimal> Owner_total_handling_original { get; set; }
        public Nullable<decimal> Owner_total_tax_original { get; set; }
        public Nullable<decimal> Owner_total_original { get; set; }
        public Nullable<decimal> User_total_labor_original { get; set; }
        public Nullable<decimal> User_Total_material_Original { get; set; }
        public Nullable<decimal> User_Total_handling_Original { get; set; }
        public Nullable<decimal> User_Total_tax_Original { get; set; }
        public Nullable<decimal> User_Total_Original { get; set; }
        public Nullable<decimal> Estimate_Grand_Total_Original { get; set; }
        public string Surveyrequested { get; set; }
        public Nullable<int> Revision_Number { get; set; }
        public Nullable<System.DateTime> Cancelled_Date { get; set; }
        public string Cancelled_By { get; set; }
        public Nullable<System.DateTime> Owner_Approval_Date { get; set; }
        public string Approved_By { get; set; }
        public string Onwer_Approval_Number_Original { get; set; }
        public string Base_Currency_Approved { get; set; }
        public Nullable<decimal> Owner_Labor_Approved { get; set; }
        public Nullable<decimal> Owner_Material_Approved { get; set; }
        public Nullable<decimal> Owner_Handling_Approved { get; set; }
        public Nullable<decimal> Owner_Tax_Approved { get; set; }
        public Nullable<decimal> Owner_Total_Approved { get; set; }
        public Nullable<decimal> User_Total_Approved { get; set; }
        public Nullable<decimal> Estimate_Grand_Total_Approved { get; set; }
        public Nullable<System.DateTime> Repair_Completed_Date { get; set; }
        public Nullable<System.DateTime> Repair_Complete_Reported { get; set; }

        //public List<EstLifeCycleAnalysisModel> Lifecycle { get; set; }

        #endregion
    }
}