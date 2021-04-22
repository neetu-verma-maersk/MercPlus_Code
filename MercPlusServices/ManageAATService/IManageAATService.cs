using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MercPlusLibrary;
namespace ManageAATService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IManageAATService" in both code and config file together.
    [ServiceContract]
    public interface IManageAATService
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        List<ErrMessage> ApproveWorkOrderOLD(int WOID, string User, string OldStatusOrRemark, string VendorRefNo);

        [OperationContract]
         string WorkOrderDeletion(int WOID, string User);

        [OperationContract]
        string UpdateWarrantyComment(int wo_id, string comment);

        //US 6334 changes

        [OperationContract]
        string ApproveWorkOrder(int wo_id, string User_ID, string Remarks, string Remarks_Type, decimal totalcost);

        [OperationContract]
        string RejectWorkOrder(int wo_id, string User_ID, string Remarks, string Remarks_Type);

        [OperationContract]
        string ForwardWorkOrder(int wo_id, string User_ID,string Remarks,string Remarks_Type, int StatusCode);

        [OperationContract]
        string TotalLossWorkOrder(int wo_id, string User_ID,string Remarks,string Remarks_Type);

        [OperationContract]
        string UpdateWORemarks(int wo_id, string User_ID, string Remarks,string Remarks_Type);        
    }
}
