using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MercPlusLibrary;

namespace ManageReportsService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IManageReports
    {
        //[OperationContract]
        //List<ReportsDetails> GetReportsDetails();

        [OperationContract]
        List<Country> GetCountryList(out List<ErrMessage> ErrorMessageList, int UserID, string Role);

        [OperationContract]
        List<Shop> GetShopList(out List<ErrMessage> ErrorMessageList, int UserID);

        //[OperationContract]
        //List<Mode> GetModeList();

        [OperationContract]
        List<Manual> GetManualList(out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Customer> GetCustomerList(out List<ErrMessage> ErrorMessageList);

        [OperationContract]
      //  List<Area> GetAreaList(out List<ErrMessage> ErrorMessageList, int UserID);
        List<Area> GetAreaList(out List<ErrMessage> ErrorMessageList, int UserID, string Role);

        [OperationContract]
        List<RepairCode> GetRepairCodeList(string shopCode, string customerCode, string manualCode, string modeCode, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Shop> GetShopListOnCountryCode(string CountryCode, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Mode> GetModeListOnConditions(string shopCode, string CustomeCode, string ManualCode);

        [OperationContract]
        List<Reports> GetReportsDetailsMercA01(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Reports> GetReportsDetailsMercA02(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Reports> GetReportsDetailsMercA03(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Reports> GetReportsDetailsMercC01(out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Reports> GetReportsDetailsMercC02(out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Reports> GetReportsDetailsMercB01(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, string RepairCode, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Reports> GetReportsDetailsMercD03(string ShopCode, string CustomerCode, string ManualCode, string Mode, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Reports> GetReportsDetailsMercD05(string ShopCode, string CustomerCode, string Mode, string Manual, string Country, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Reports> GetReportsDetailsMercE01(string ShopCode, string CustomerCode, string Mode, string Manual, string CountryCode, string Days, string AreaCode, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Reports> GetReportsDetailsMercE02(string ShopCode, string CustomerCode, string Mode, string Manual, string CountryCode, string Days, string AreaCode, out List<ErrMessage> ErrorMessageList);

        [OperationContract]
        List<Reports> GetReportsDetailsMercE03(string ShopCode, string CustomerCode, string Mode, string Manual, string CountryCode, string Days, string AreaCode, out List<ErrMessage> ErrorMessageList);
        // TODO: Add your service operations here
    }

    
}
