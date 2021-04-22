using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class Reports
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
        public string TotalCostOfAgentSuppliedNumberedParts { get; set; }
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
        public string RepairCod { get; set; }
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
        public string ShopCode { get; set; }
        [DataMember]
        public DateTime RepairDate { get; set; }
        [DataMember]
        public string PART_CD { get; set; }
        [DataMember]
        public string QuantityParts { get; set; }
        [DataMember]
        public string CostLocal { get; set; }
        [DataMember]
        public string SerialNumber { get; set; }
        [DataMember]
        public string PartDesc { get; set; }
        [DataMember]
        public string MSLPartSW { get; set; }
        [DataMember]
        public string CorePartSW { get; set; }
        [DataMember]
        public string ManufacturerName { get; set; }
        [DataMember]
        public string RepairDesc { get; set; }
        [DataMember]
        public DateTime EffectiveDate { get; set; }
        [DataMember]
        public DateTime ExpiryDate { get; set; }
        [DataMember]
        public Int32 ExratUSD { get; set; }
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
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChTime { get; set; }
        [DataMember]
        public ErrMessage ErrorMessage { get; set; }

    }
}
