using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using MercPlusLibrary;
using MercPlusServiceLibrary.BusinessObjects;

namespace ManageWorkOrderService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IManageWorkOrder
    {
        //[OperationContract]
        //List<WorkOrderDetail> GetElligibleWOs(string EqpNo);
        //[OperationContract]
        //List<Mode> GetEquipmentModeDropDownFiltered(string CoType, string EqSType, string Material, string Size, string STREFURB, string STREDEL, string EQIOFLT);
        [OperationContract]
        bool SendGradeCodeToRKEM(string EqpNo, string CurrLoc, string GradeCode);
 
        [OperationContract]
        bool CallSaveMethod(WorkOrderDetail WorkOrder, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        bool CallValidateMethod(ref WorkOrderDetail WorkOrderDetail, Equipment Equipment, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        WorkOrderDetail GetWorkOrderDetails(int WorkOrderID);

        [OperationContract]
        List<RepairsView> GetHours(WorkOrderDetail WorkOrder, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Damage> GetDamageCodeAll(string Code);

        [OperationContract]
        List<Shop> GetShopCode(int UserID);

        [OperationContract]
        Shop GetShopDetailsOnShopCode(string ShopCode);

        //[OperationContract]
        //List<Mode> GetModeList();

        [OperationContract]
        List<Customer> GetCustomerCode(string ShopCode);

        [OperationContract]
        string GetCurrency(string ShopCode);

        [OperationContract]
        List<RepairLoc> GetRepairLocCode(string Code);

        [OperationContract]
        List<RepairCode> GetRepairCode(string ModeCode);

        [OperationContract]
        List<Tpi> GetTpiCode(string Code);

        [OperationContract]
        Equipment GetEquipmentDetailsFromRKEM(string EqpNo, string ShopCode, string VendorRefNo);

        [OperationContract]
        string RSByMfgAndModel(string eqpRUType);

        [OperationContract]
        List<ErrMessage> ApproveWorkOrder(int WOID, string User, string OldStatusOrRemark, string VendorRefNo);

        // Created by Afroz
        [OperationContract]
        List<Customer> GetCustomerCodeByShopCode(string ShopCode, int UserId);

        [OperationContract]
        List<ErrMessage> UpdateNewTPI(int WOID, string Repaircd, string NewTPI, string ChangeUser);//Debadrita_TPI_Indicator-19-07-19



        [OperationContract]
        List<EqType> GetEquipmentType();

        [OperationContract]
        List<EqsType> GetEquipmentSubType(string EqpType);

        [OperationContract]
        List<WorkOrderDetail> GetWorkOrder(string ShopCode, string FromDate, string ToDate, string CustomerCD, string EqpSize, string EqpType, string EqpSType, string Mode, string EquipmentNo, string VenRefNo, string Cocl, string Country, string Location, string QueryType, int SortType, int UserId);

        [OperationContract]
        int GetSerialNo(string WOID);

        [OperationContract]
        List<WorkOrderDetail> GetWorkOrderByCountryOrHigher(string ShopCode, string FromDate, string ToDate, string CustomerCD, string EqpSize, string EqpType, string EqpSType, string Mode, string EquipmentNo, string VenRefNo, string Cocl, string Country, string Location, string QueryType, int SortType, int UserId);

        [OperationContract]
        decimal? RSUserByUserId(int UserId);

        [OperationContract]
        string UpdateWorkOrder(string WO_ID, int Status_Code);

        [OperationContract]
        List<ErrMessage> SetWorkingSwitchByWOID(int WOID, string Switch, string ChangeUser);

        [OperationContract]
        List<ErrMessage> UpdateCompleteApprovedWO(int WorkIDs, DateTime? NewRepairDate, string ChangeUser);
        [OperationContract]
        string UpdateApproveWorkOrder(int WOID, string User, string OldStatusOrRemark, string VendorRefNo);
        //By Afroz
        [OperationContract]
        string UpdateApproveWorkOrderByReview(int WOID, string User, string OldStatusOrRemark, string VendorRefNo);

        [OperationContract]
        string SetWorkingSwitchByWOIDByReview(int WOID, string Switch, string ChangeUser);

        [OperationContract]
        string UpdateCompleteApprovedWOByReview(int WorkIDs, DateTime? NewRepairDate, string ChangeUser);
        // End Afroz
        [OperationContract]
        string GetVenRefNoByWOID(int WOID);

        [OperationContract]
        List<ErrMessage> Review(ref WorkOrderDetail WorkOrderDetail, List<Equipment> EquipmentList, bool ClientCall = true);

        [OperationContract]
        List<ErrMessage> SubmitWorkOrder(ref WorkOrderDetail WorkOrderDetail, List<Equipment> EquipmentList);

        [OperationContract]
        List<WorkOrderDetail> GetAuditRecord(string WOID);

        [OperationContract]
        WorkOrderDetail GetWOAdditionalDetails(string orderNo);

        [OperationContract]
        List<ErrMessage> SaveAsDraft(ref WorkOrderDetail WorkOrderDetail, List<Equipment> EquipmentList);

        [OperationContract]
        List<ErrMessage> ChangeStatus(int WOID, short? WOStatus, string ChangeUser);
        //End
        [OperationContract]
        List<RemarkEntry> LoadRemarksDetails(int WOID);

        [OperationContract]
        List<ErrMessage> AddRemarkByTypeAndWOID(int WOID, string Remarks, string RemarksType, string ChangeUser);

        [OperationContract]
        List<ErrMessage> UpdateThirdPartyCause(int WOID, string NewThirdParty, string NewCause, string ChangeUser);

        [OperationContract]
        List<ErrMessage> UpdateRepairDateByWOID(int WOID, DateTime? NewRepairDate, string ChangeUser);

        [OperationContract]
        List<ErrMessage> UpdateSerialNumber(int WOID, string RepairCode, string PartNumber, string SerialNumber, string ChangeUser);

        [OperationContract]
        List<ErrMessage> UpdateRevNo(int WOID);

        [OperationContract]
        List<ErrMessage> UpdateShopWorkingSwitch(int WOID, string Switch, string ChangeUser);

        [OperationContract]
        Dictionary<string, object> GetPrevStatusDateLoc(int WOID, string WOIndicator);

        [OperationContract]
        bool AuthenticateShopCodeByUserID(string ShopCode, int UserID);

        [OperationContract]
        List<ErrMessage> CompleteWorkOrderByID(int WOID, DateTime RepairDate, string ChangeUser);

        [OperationContract]
        List<ErrMessage> RevertTotalLoss(int WOID, string chUser);

        [OperationContract]
        List<ErrMessage> ChangeStatusForOtherWOs(int WOID, string ChangeUser);

        [OperationContract]
        int CheckWordOrdersTotalLosscontainer(string EqpNo, string Present_Loc);

        //[OperationContract]
        //bool CheckDamageCode(ref WorkOrderDetail WorkOrderDetail, ref Equipment eqp,out);

        //[OperationContract]
        //bool SendDamageCodeToRKEM(string EqpNo, string CurrLoc, string DamageCode);

        [OperationContract]
        List<string> GetMercDamageCode(int WOID);

        [OperationContract]
        List<string> GetMercGradeCode(int WOID, string EQP);

        [OperationContract]
        bool GetServiceStatus();


    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.


}
