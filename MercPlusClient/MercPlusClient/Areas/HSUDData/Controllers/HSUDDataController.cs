using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;

using System.Text;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Logging;

using System.Globalization;
using System.Data.SqlClient;
using System.Configuration;

using System.Data;
using System.Web.Mvc;
using MercPlusClient.Areas.HSUDData.Models;

using MercPlusClient.ManageHSUDDataServiceReference;
using MercPlusClient;


using MercPlusClient.UtilityClass;
using System.Web.UI;
using System.Dynamic;
using HSUD = MercPlusClient.Areas.HSUDData.Models.GetHSUDDetailsModel;


namespace MercPlusClient.Areas.HSUDData.Controllers
{
    public class HSUDDataController : Controller
    {
        HSUDDataModel MD = new HSUDDataModel();

        //HSUDEntities objcontext = new HSUDEntities();


        ManageHSUDDataServiceClient HSUDclient = new ManageHSUDDataServiceClient();
        [HttpGet]
        public ActionResult HSUDData()
        {
           
                return View(MD);
          
            
            //C:\Merc+\MercPlusClient\MercPlusClient\Areas\HSUDData\Style\
        }
        //
        // GET: /HSUDData/HSUDData/
        [HttpPost]

        public ActionResult HSUDDataSearch(string EquipmentNo, HSUDDataModel model)
        {

            List<EstLifeCycle_ApprovalCanceled> result = null;
            List<HSUDDataModel> XX = null;
            try
            {
                result = HSUDclient.HSUDDataSearch(EquipmentNo).ToList();
                XX = PopulateData(result);
                model.Count = XX.Count();
                model.SearchResults = XX;

            }
            catch (Exception ex)
            {
                throw;
            }


            return View("HSUDDataPartial", model);
        }
        public List<HSUDDataModel> PopulateData(List<EstLifeCycle_ApprovalCanceled> input)
        {
            List<HSUDDataModel> aa = new List<HSUDDataModel>();
            try
            {
                foreach (var e in input)
                {
                    HSUDDataModel HSModel = new HSUDDataModel();
                    HSModel.Equipment_ID = Convert.ToString(e.EquimentID);
                    HSModel.Equipment_Type = e.EquipmentType;
                    HSModel.Estimate_Number = e.EstimateNumber;
                    HSModel.Estimate_Status = e.Estimate_Status;
                    HSModel.Summary_Size_Type = e.Summary_Size_Type;
                    HSModel.Estimate_Original_Date = (string.IsNullOrEmpty(e.Estimate_Original_Date.ToString())) ? string.Empty : (Convert.ToDateTime(e.Estimate_Original_Date.ToString())).ToString("yyyy-MM-dd");
                    HSModel.Approved_date = (string.IsNullOrEmpty(e.Owner_Approval_Date.ToString())) ? string.Empty : (Convert.ToDateTime(e.Estimate_Original_Date.ToString())).ToString("yyyy-MM-dd");
                    HSModel.Cancelled_date = (string.IsNullOrEmpty(e.Cancelled_Date)) ? string.Empty : (Convert.ToDateTime(e.Cancelled_Date.ToString())).ToString();
                    aa.Add(HSModel);
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            return aa;
        }

        # region GetHSUDDetails
        public ActionResult GetHSUDDetails(string EquipmentNo, string EstimateNumber)
        {
            HSUD HSUDDetails = new HSUD();
            List<EstLifeCycle_ApprovalCanceled> ApproveCanceled = null;
            List<EstLifeCycleAnalysi> LifeCycle = null;
            List<EstLineItemAnalysi> LineItem = null;
            try
            {

                ApproveCanceled = HSUDclient.GetEstLifeCycle_ApprovalCanceledData(EquipmentNo, EstimateNumber).ToList();
                HSUDDetails.EstAppCanclled = new List<EstLifeCycle_ApprovalCanceledModel>();
                HSUDDetails.EstAppCanclled = populateEstLifeCycleApprovalCanceledData(ApproveCanceled);


                LifeCycle = HSUDclient.GetEstLifeCycleAnalysisData(EquipmentNo, EstimateNumber).ToList();
                HSUDDetails.EstAnalysis = new List<EstLifeCycleAnalysisModel>();
                HSUDDetails.EstAnalysis = PopulateEstLifeCycleAnalysiData(LifeCycle);


                LineItem = HSUDclient.GetEstLineItemAnalysisData(EquipmentNo, EstimateNumber).ToList();
                HSUDDetails.EstLineItem = new List<EstLineItemAnalysisModel>();
                HSUDDetails.EstLineItem = PopulateEstLineItemAnalysisData(LineItem);
            }
            catch (Exception ex)
            {
                throw;

            }


            return View(HSUDDetails);
        }
        public List<EstLifeCycle_ApprovalCanceledModel> populateEstLifeCycleApprovalCanceledData(List<EstLifeCycle_ApprovalCanceled> input)
        {
            List<EstLifeCycle_ApprovalCanceledModel> est = new List<EstLifeCycle_ApprovalCanceledModel>();
            
                  try
                {
                    foreach (var e in input)
                {
               
                    EstLifeCycle_ApprovalCanceledModel HSModel = new EstLifeCycle_ApprovalCanceledModel();//Kasturee_HSUD_Before_PROD_02-07-19 
                    #region variabes
                    HSModel.Facility_Code = e.Facility_Code;
                    HSModel.Facility_Name = e.Facility_Name;
                    HSModel.EquipmentType = e.EquipmentType;
                    HSModel.Summary_Size_Type = e.Summary_Size_Type;

                    HSModel.EquimentID = e.EquimentID;
                    HSModel.EstimateNumber = e.EstimateNumber;
                    HSModel.Estimate_Status = e.Estimate_Status;

                    HSModel.Estimate_Original_Date = e.Estimate_Original_Date;
                    HSModel.Estimate_Transmission_date = e.Estimate_Transmission_date;

                    HSModel.Base_Currency_Original = e.Base_Currency_Original;


                    HSModel.Owner_total_labor_original = Convert.ToDecimal(e.Owner_total_labor_original);
                    HSModel.Owner_total_material_original = Convert.ToDecimal(e.Owner_total_material_original);
                    HSModel.Owner_total_handling_original = Convert.ToDecimal(e.Owner_total_handling_original);
                    HSModel.Owner_total_tax_original = Convert.ToDecimal(e.Owner_total_tax_original);

                    HSModel.Owner_total_original = Convert.ToDecimal(e.Owner_total_original);
                    HSModel.User_total_labor_original = Convert.ToDecimal(e.User_total_labor_original);
                    HSModel.User_Total_material_Original = Convert.ToDecimal(e.User_Total_material_Original);
                    HSModel.User_Total_handling_Original = Convert.ToDecimal(e.User_Total_handling_Original);


                    HSModel.User_Total_tax_Original = Convert.ToDecimal(e.User_Total_tax_Original);
                    HSModel.User_Total_Original = Convert.ToDecimal(e.User_Total_Original);
                    HSModel.Estimate_Grand_Total_Original = Convert.ToDecimal(e.Estimate_Grand_Total_Original);

                    HSModel.Surveyrequested = e.Surveyrequested;

                    HSModel.Revision_Number = e.Revision_Number;

                    HSModel.Cancelled_Date = e.Cancelled_Date;
                    HSModel.Cancelled_By = e.Cancelled_By;

                    HSModel.Owner_Approval_Date = e.Owner_Approval_Date;
                    HSModel.Approved_By = e.Approved_By;


                    HSModel.Onwer_Approval_Number_Original = e.Onwer_Approval_Number_Original;
                    HSModel.Base_Currency_Approved = e.Base_Currency_Approved;


                    HSModel.Owner_Labor_Approved = Convert.ToDecimal(e.Owner_Labor_Approved);
                    HSModel.Owner_Material_Approved = Convert.ToDecimal(e.Owner_Material_Approved);
                    HSModel.Owner_Handling_Approved = Convert.ToDecimal(e.Owner_Handling_Approved);
                    HSModel.Owner_Tax_Approved = Convert.ToDecimal(e.Owner_Tax_Approved);

                    HSModel.Owner_Total_Approved = Convert.ToDecimal(e.Owner_Total_Approved);
                    HSModel.User_Total_Approved = Convert.ToDecimal(e.User_Total_Approved);
                    HSModel.Estimate_Grand_Total_Approved = Convert.ToDecimal(e.Estimate_Grand_Total_Approved);
                    #endregion
                    est.Add(HSModel);
                }
                
            }
                    catch (Exception ex)
                    {
                        throw;
                    }
            return est;
        }

        public List<EstLifeCycleAnalysisModel> PopulateEstLifeCycleAnalysiData(List<EstLifeCycleAnalysi> input)
        {
            List<EstLifeCycleAnalysisModel> aa = new List<EstLifeCycleAnalysisModel>();
            

            try
            {
                foreach (var e in input)
                {
                    EstLifeCycleAnalysisModel HSModel = new EstLifeCycleAnalysisModel();//Kasturee_HSUD_Before_PROD_02-07-19 
                    #region Variable
                    HSModel.Facility_Code = e.Facility_Code;
                    HSModel.Facility_Name = e.Facility_Name;
                    HSModel.EquipmentType = e.EquipmentType;
                    HSModel.Summary_Size_Type = e.Summary_Size_Type;
                    HSModel.EquimentID = e.EquimentID;
                    HSModel.EstimateNumber = e.EstimateNumber;
                    HSModel.Estimate_Status = e.Estimate_Status;

                    HSModel.Estimate_Original_Date = e.Estimate_Original_Date;

                    HSModel.Estimate_Transmission_date = e.Estimate_Transmission_date;
                    HSModel.Base_Currency_Original = e.Base_Currency_Original;

                    HSModel.Owner_total_material_original = Convert.ToDecimal(e.Owner_total_material_original);
                    HSModel.Owner_total_labor_original = Convert.ToDecimal(e.Owner_total_labor_original);
                    HSModel.Owner_total_handling_original = Convert.ToDecimal(e.Owner_total_handling_original);
                    HSModel.Owner_total_tax_original = Convert.ToDecimal(e.Owner_total_tax_original);
                    HSModel.Owner_total_original = Convert.ToDecimal(e.Owner_total_original);
                    HSModel.User_total_labor_original = Convert.ToDecimal(e.User_total_labor_original);
                    HSModel.User_Total_material_Original = Convert.ToDecimal(e.User_Total_material_Original);
                    HSModel.User_Total_handling_Original = Convert.ToDecimal(e.User_Total_handling_Original);
                    HSModel.User_Total_tax_Original = Convert.ToDecimal(e.User_Total_tax_Original);
                    HSModel.User_Total_Original = Convert.ToDecimal(e.User_Total_Original);
                    HSModel.Estimate_Grand_Total_Original = Convert.ToDecimal(e.Estimate_Grand_Total_Original);


                    HSModel.Surveyrequested = e.Surveyrequested;
                    HSModel.Revision_Number = e.Revision_Number;
                    HSModel.Cancelled_Date = e.Cancelled_Date;
                    HSModel.Cancelled_By = e.Cancelled_By;
                    HSModel.Owner_Approval_Date = e.Owner_Approval_Date;
                    HSModel.Approved_By = e.Approved_By;
                    HSModel.Onwer_Approval_Number_Original = e.Onwer_Approval_Number_Original;
                    HSModel.Base_Currency_Approved = e.Base_Currency_Approved;

                    HSModel.Owner_Labor_Approved = Convert.ToDecimal(e.Owner_Labor_Approved);
                    HSModel.Owner_Material_Approved = Convert.ToDecimal(e.Owner_Material_Approved);
                    HSModel.Owner_Handling_Approved = Convert.ToDecimal(e.Owner_Handling_Approved);
                    HSModel.Owner_Tax_Approved = Convert.ToDecimal(e.Owner_Tax_Approved);
                    HSModel.Owner_Total_Approved = Convert.ToDecimal(e.Owner_Total_Approved);
                    HSModel.User_Total_Approved = Convert.ToDecimal(e.User_Total_Approved);
                    HSModel.Estimate_Grand_Total_Approved = Convert.ToDecimal(e.Estimate_Grand_Total_Approved);
                    HSModel.Repair_Completed_Date = e.Repair_Completed_Date;
                    HSModel.Repair_Complete_Reported = e.Repair_Complete_Reported;
                    #endregion
                    aa.Add(HSModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return aa;
        }

        public List<EstLineItemAnalysisModel> PopulateEstLineItemAnalysisData(List<EstLineItemAnalysi> input)
        {
            List<EstLineItemAnalysisModel> aa = new List<EstLineItemAnalysisModel>();
            
            try
            {

                foreach (var e in input)
                {
                    EstLineItemAnalysisModel HSModel = new EstLineItemAnalysisModel();//Kasturee_HSUD_Before_PROD_02-07-19 
                    #region Varible
                    
                    HSModel.FACILITYCODE = e.FACILITYCODE;
                    HSModel.FACILITYNAME = e.FACILITYNAME;
                    HSModel.EQUIPMENTTYPE = e.EQUIPMENTTYPE;
                    HSModel.SUMMARYSIZETYPE = e.SUMMARYSIZETYPE;

                    HSModel.EQUIPMENTID = e.EQUIPMENTID;
                    HSModel.SENDERESTIMATEID = e.SENDERESTIMATEID;//Kasturee_HSUD_Before_PROD_02-07-19 
                    HSModel.ESTIMATEDATE = e.ESTIMATEDATE;
                    HSModel.APPROVALDATE = e.APPROVALDATE;
                    HSModel.CREATEDBYUSER = e.CREATEDBYUSER;

                    HSModel.ISSURVEYREQUESTED = e.ISSURVEYREQUESTED;
                    HSModel.LINEITEMNUMBER = e.LINEITEMNUMBER;
                    HSModel.COMPONENTCODE = e.COMPONENTCODE;
                    HSModel.LOCATIONCODE = e.LOCATIONCODE;

                    HSModel.REPAIRCODE = e.REPAIRCODE;
                    HSModel.DAMAGECODE = e.DAMAGECODE;
                    HSModel.MATERIALCODE = e.MATERIALCODE;
                    HSModel.UNITOFMEASURE = e.UNITOFMEASURE;
                    HSModel.QUANTITY = e.QUANTITY;

                    HSModel.LENGTH = e.LENGTH;
                    HSModel.WIDTH = e.WIDTH;
                    HSModel.ORGANIZATIONTYPE = e.ORGANIZATIONTYPE;
                    HSModel.BASECURRENCYCODE = e.BASECURRENCYCODE;

                    HSModel.LABORHOURS = Convert.ToDecimal(e.LABORHOURS);
                    HSModel.LABORRATEBASE = Convert.ToDecimal(e.LABORRATEBASE);
                    HSModel.LABORCOSTBASE = Convert.ToDecimal(e.LABORCOSTBASE);
                    HSModel.MATERIALCOSTBASE = Convert.ToDecimal(e.MATERIALCOSTBASE);
                    HSModel.TOTALBASE = Convert.ToDecimal(e.TOTALBASE);
                    #endregion
                    aa.Add(HSModel);
                }
                
            }
            catch (Exception ex)
            {
                throw;

            }
            return aa;
        }
        #endregion

    }
}
