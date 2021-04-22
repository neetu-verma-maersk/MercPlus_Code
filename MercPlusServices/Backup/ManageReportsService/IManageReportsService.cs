using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ManageReportsService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IManageReports
    {
        [OperationContract]
        List<ReportsDetails> GetReportsDetails();

        [OperationContract]
        List<Country> GetCountryList();

        [OperationContract]
        List<Shop> GetShopList();

        [OperationContract]
        List<Mode> GetModeList();

        [OperationContract]
        List<Manual> GetManualList();

        [OperationContract]
        List<Customer> GetCustomerList();

        [OperationContract]
        List<Area> GetAreaList();

        [OperationContract]
        List<Shop> GetShopListOnCountryCode(string CountryCode);

        //[OperationContract]
        //List<Location> GetLocationDetails();

        [OperationContract]
        List<WorkOrder> GetReportsDetailsMercA01(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual);

        [OperationContract]
        List<WorkOrder> GetReportsDetailsMercA02(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual);

        [OperationContract]
        List<WorkOrder> GetReportsDetailsMercA03(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual);

        [OperationContract]
        List<WorkOrder> GetReportsDetailsMercC01();

        [OperationContract]
        List<WorkOrder> GetReportsDetailsMercC02();
        
        [OperationContract]
        List<WorkOrder> GetReportsDetailsMercB01(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, string RepairCode);

        [OperationContract]
        List<WorkOrder> GetReportsDetailsMercD03(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string ManualCode, string Mode);

        [OperationContract]
        List<WorkOrder> GetReportsDetailsMercD05(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, string Country);

        [OperationContract]
        List<WorkOrder> GetReportsDetailsMercE01(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, string CountryCode, string Days, string AreaCode);

        // TODO: Add your service operations here
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations
    [DataContract]
    public class ReportsDetails
    {
        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Manual { get; set; }

        [DataMember]
        public string Customer { get; set; }

        [DataMember]
        public string Shop { get; set; }

        [DataMember]
        public string Mode { get; set; }
    }

    [DataContract]
    public class Country
    {
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string CountryDescription { get; set; }
    }

    [DataContract]
    public class Shop
    {
        [DataMember]
        public string ShopCode { get; set; }
        [DataMember]
        public string ShopDescription { get; set; }
    }

    [DataContract]
    public class Manual
    {
        [DataMember]
        public string ManualCode { get; set; }
        [DataMember]
        public string ManualDescription { get; set; }
    }

    [DataContract]
    public class Customer
    {
        [DataMember]
        public string CustomerCode { get; set; }
        [DataMember]
        public string CustomerDescription { get; set; }
    }

    [DataContract]
    public class Area
    {
        [DataMember]
        public string AreaCode { get; set; }
        [DataMember]
        public string AreaDescription { get; set; }
    }

    [DataContract]
    public class Mode
    {
        [DataMember]
        public string ModeCode { get; set; }
        [DataMember]
        public string ModeDescription { get; set; }
    }

    [DataContract]
    public class Location
    {
        [DataMember]
        public string locCode { get; set; }

        [DataMember]
        public string locDesc { get; set; }

    }

    [DataContract]
    public class WorkOrder
    {
        //Header
        [DataMember]
        public string HeaderVendorCode { get; set; }
        [DataMember]
        public string HeaderVendorDesc { get; set; }
        [DataMember]
        public string HeaderShopCode { get; set; }
        [DataMember]
        public string HeaderShopDesc { get; set; }
        [DataMember]
        public string HeaderCurrencyName { get; set; }
        [DataMember]
        public string HeaderCurrencyCode { get; set; }
        [DataMember]
        public string Mode { get; set; }
        [DataMember]
        public string Manual { get; set; }
        [DataMember]
        public string ActualCompletionDate { get; set; }
        [DataMember]
        public string EquipmentNo { get; set; }
        [DataMember]
        public string WorkOrderNo { get; set; }
        [DataMember]
        public string VendorRefNo { get; set; }
        [DataMember]
        public string VoucherNumber { get; set; }
        [DataMember]
        public string AgentCode { get; set; }
        [DataMember]
        public string OrdinaryManHours { get; set; }
        [DataMember]
        public string Overtime1ManHours { get; set; }
        [DataMember]
        public string Overtime2ManHours { get; set; }
        [DataMember]
        public string Overtime3ManHours { get; set; }
        [DataMember]
        public string TotalHours { get; set; }
        [DataMember]
        public string TotalLabourCostCPH { get; set; }
        [DataMember]
        public string TotalLabourCost { get; set; }
        [DataMember]
        public string TotalCostOfShopSuppliedNumberedParts { get; set; }
        [DataMember]
        public string TotalCostOfShopSuppliedMaterials { get; set; }
        [DataMember]
        public string TotalCostOfNumberedParts { get; set; }
        [DataMember]
        public string TotalCostOfSuppliedMaterials { get; set; }
        [DataMember]
        public string ImportTax { get; set; }
        [DataMember]
        public string SalesTaxParts { get; set; }
        [DataMember]
        public string SalesTaxLabour { get; set; }
        [DataMember]
        public string TotalToBePaidToShop { get; set; }
        [DataMember]
        public string TotalToBePaidToShopInUSD { get; set; }
        [DataMember]
        public string TotalCostOfCPHSuppliedParts { get; set; }
        [DataMember]
        public string TotalCostOfRepairCPH { get; set; }
        [DataMember]
        public string TotalCostOfCPHSuppliedPartsInUSD { get; set; }
        [DataMember]
        public string TotalToBePaidToAgentFromCPHInUSD { get; set; }
        [DataMember]
        public string PartSupplier { get; set; }
        [DataMember]
        public string TotalToBePaidToAgentFromCPHInLocalCurrency { get; set; }
        [DataMember]
        public string ExchangeRate { get; set; }
        [DataMember]
        public string RepairCode { get; set; }
        [DataMember]
        public string RepairCodeDesc { get; set; }
        [DataMember]
        public string ExclusionaryRepairCode { get; set; }
        [DataMember]
        public string ExclusionaryRepairCodeDescription { get; set; }
        [DataMember]
        public string ContractAmount { get; set; }
        [DataMember]
        public string RateEffectiveDate { get; set; }
        [DataMember]
        public string RateExpiryDate { get; set; }
        [DataMember]
        public string MaxMaterialAmountPrPiece { get; set; }
        [DataMember]
        public string MaxMaterialAmountPrPieceConvertedToUSD { get; set; }
        [DataMember]
        public string AreaCode { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string RepairShop { get; set; }
        [DataMember]
        public string EstimateCreationDate { get; set; }
        [DataMember]
        public string DaysSinceCreation { get; set; }
        [DataMember]
        public string EstimateApprovalDate { get; set; }
        [DataMember]
        public string DaysSinceApproval { get; set; }
        [DataMember]
        public string LastChangeToEstimate { get; set; }
        [DataMember]
        public string DaysSinceLastChange { get; set; }
        [DataMember]
        public string EstimatedTotalHours { get; set; }
        [DataMember]
        public string EstimatedTotalCostOfRepairInUSD { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string PartNumber { get; set; }
        [DataMember]
        public string SHOP_CD { get; set; }
        [DataMember]
        public DateTime REPAIR_DTE { get; set; }
        [DataMember]
        public string REPAIR_CD { get; set; }
        [DataMember]
        public string REPAIR_DESC { get; set; }
        [DataMember]
        public string PART_CD { get; set; }
        [DataMember]
        public string QTY_PARTS { get; set; }
        [DataMember]
        public string COST_LOCAL { get; set; }
        [DataMember]
        public string SERIAL_NUMBER { get; set; }
        [DataMember]
        public string PART_DESC { get; set; }
        [DataMember]
        public string MSL_PART_SW { get; set; }
        [DataMember]
        public string CORE_PART_SW { get; set; }
        [DataMember]
        public string ManufacturerName { get; set; }
        [DataMember]
        public string RepairDesc { get; set; }
        [DataMember]
        public string MODE { get; set; }
        //[DataMember]
        //public string RepairCode { get; set; }
        //[DataMember]
        //public Int32 ContractAmount { get; set; }
        [DataMember]
        public DateTime EffectiveDate { get; set; }
        [DataMember]
        public DateTime ExpiryDate { get; set; }
        [DataMember]
        public Int32 ExratUSD { get; set; }
        //[DataMember]
        //public string AreaCode { get; set; }
        //[DataMember]
        //public string CountryCode { get; set; }
        [DataMember]
        public string RepairShopCode { get; set; }
        [DataMember]
        public Int16 StatusCode { get; set; }
        [DataMember]
        public string EqpmntNo { get; set; }
        //[DataMember]
        //public DateTime EstimateCreationDate { get; set; }
        [DataMember]
        public DateTime ApprovalDate { get; set; }
        [DataMember]
        public DateTime ChangeToEstimateDate { get; set; }
        //[DataMember]
        //public DateTime EstimatedTotalHours { get; set; }
        [DataMember]
        public DateTime EstimatedTotalCost { get; set; }
        [DataMember]
        public DateTime EstiamtedTotalCostRepair { get; set; }
        [DataMember]
        public string VendorReferenceNo { get; set; }
    }
}
