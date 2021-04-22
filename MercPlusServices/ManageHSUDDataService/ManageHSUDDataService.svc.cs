using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging;
namespace ManageHSUDDataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class ManageHSUDDataService : IManageHSUDDataService
    {
        LogEntry logEntry = new LogEntry();
        HSUDDataEntities objcontext = new HSUDDataEntities();

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }



        public List<EstLifeCycle_ApprovalCanceled> HSUDDataSearch(String EquipmentNo)
        {

            List<EstLifeCycle_ApprovalCanceled> APPCanceled = new List<EstLifeCycle_ApprovalCanceled>();
            APPCanceled = (from ac in objcontext.EstLifeCycle_ApprovalCanceled
                           where ac.EquimentID == EquipmentNo
                           orderby ac.Estimate_Original_Date descending //Kasturee_HSUD_Before_PROD_02-07-19 
                           select ac).ToList();
            // List<HSUDDataModel> XX = null;


            return APPCanceled;
        }
        #region Get Details
        public List<EstLifeCycle_ApprovalCanceled> GetEstLifeCycle_ApprovalCanceledData(String EquipmentNo, String EstimateNo)
        {

            List<EstLifeCycle_ApprovalCanceled> APPCanceled = new List<EstLifeCycle_ApprovalCanceled>();
            try
            {
                APPCanceled = (from ac in objcontext.EstLifeCycle_ApprovalCanceled
                               where ac.EquimentID == EquipmentNo && ac.EstimateNumber == EstimateNo
                               orderby ac.Approved_By descending //Kasturee_HSUD_Before_PROD_02-07-19 
                               select ac).ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                
            }

            return APPCanceled;
        }

        public List<EstLifeCycleAnalysi> GetEstLifeCycleAnalysisData(String EquipmentNo, String EstimateNo)
        {

            List<EstLifeCycleAnalysi> Analysis = new List<EstLifeCycleAnalysi>();
            try
            {
               
                Analysis = (from ac in objcontext.EstLifeCycleAnalysis
                            where ac.EquimentID == EquipmentNo && ac.EstimateNumber == EstimateNo
                            orderby ac.Approved_By descending //Kasturee_HSUD_Before_PROD_02-07-19 
                            select ac).ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Analysis;
        }

        public List<EstLineItemAnalysi> GetEstLineItemAnalysisData(String EquipmentNo, String EstimateNo) //Kasturee_HSUD_Before_PROD_02-07-19 
        {

            List<EstLineItemAnalysi> LineItem = new List<EstLineItemAnalysi>();
            try
            {

                LineItem = (from ac in objcontext.EstLineItemAnalysis
                            where ac.EQUIPMENTID == EquipmentNo && ac.SENDERESTIMATEID == EstimateNo
                            orderby ac.APPROVALDATE descending //Kasturee_HSUD_Before_PROD_02-07-19 
                            select ac ).ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return LineItem;
        }

        # endregion


    }
}
