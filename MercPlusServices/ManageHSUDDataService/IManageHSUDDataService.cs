using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ManageHSUDDataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IManageHSUDDataService
    {

        [OperationContract]
        string GetData(int value);
             

        // TODO: Add your service operations here

        [OperationContract]
        List<EstLifeCycle_ApprovalCanceled> HSUDDataSearch(String EquipmentNo);

        [OperationContract]
        List<EstLifeCycle_ApprovalCanceled> GetEstLifeCycle_ApprovalCanceledData(String EquipmentNo, String EstimateNo);


        [OperationContract]
        List<EstLifeCycleAnalysi> GetEstLifeCycleAnalysisData(String EquipmentNo, String EstimateNo);

        [OperationContract]
        List<EstLineItemAnalysi> GetEstLineItemAnalysisData(String EquipmentNo, String EstimateNo);
    }
       

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
 
}
