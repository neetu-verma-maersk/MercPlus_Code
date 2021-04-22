using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    [DataContract]
    public class WorkOrder
    {
        //Header
        
        public string HeaderVendorCode { get; set; }
        
        public string HeaderVendorDesc { get; set; }
        
        public string HeaderShopCode { get; set; }
        
        public string HeaderShopDesc { get; set; }
        
        public string HeaderCurrencyName { get; set; }
        
        public string HeaderCurrencyCode { get; set; }
        
        public string Mode { get; set; }
        
        public string Manual { get; set; }
        
        public string ActualCompletionDate { get; set; }
        
        public string EquipmentNo { get; set; }
        
        public string WorkOrderNo { get; set; }
        
        public string VendorRefNo { get; set; }
        
        public string VoucherNumber { get; set; }
        
        public string AgentCode { get; set; }
        
        public string OrdinaryManHours { get; set; }
        
        public string Overtime1ManHours { get; set; }
        
        public string Overtime2ManHours { get; set; }
        
        public string Overtime3ManHours { get; set; }
        
        public string TotalHours { get; set; }
        
        public string TotalLabourCostCPH { get; set; }
        
        public string TotalLabourCost { get; set; }
        
        public string TotalCostOfShopSuppliedNumberedParts { get; set; }
        
        public string TotalCostOfShopSuppliedMaterials { get; set; }
        
        public string TotalCostOfNumberedParts { get; set; }
        
        public string TotalCostOfSuppliedMaterials { get; set; }
        
        public string ImportTax { get; set; }
        
        public string SalesTaxParts { get; set; }
        
        public string SalesTaxLabour { get; set; }
        
        public string TotalToBePaidToShop { get; set; }
        
        public string TotalToBePaidToShopInUSD { get; set; }
        
        public string TotalCostOfCPHSuppliedParts { get; set; }
        
        public string TotalCostOfRepairCPH { get; set; }
        
        public string TotalCostOfCPHSuppliedPartsInUSD { get; set; }
        
        public string TotalToBePaidToAgentFromCPHInUSD { get; set; }
        
        public string PartSupplier { get; set; }
        
        public string TotalToBePaidToAgentFromCPHInLocalCurrency { get; set; }
        
        public string ExchangeRate { get; set; }
        
        public string RepairCode { get; set; }
        
        public string RepairCodeDesc { get; set; }
        
        public string ExclusionaryRepairCode { get; set; }
        
        public string ExclusionaryRepairCodeDescription { get; set; }
        
        public string ContractAmount { get; set; }
        
        public string RateEffectiveDate { get; set; }
        
        public string RateExpiryDate { get; set; }
        
        public string MaxMaterialAmountPrPiece { get; set; }
        
        public string MaxMaterialAmountPrPieceConvertedToUSD { get; set; }
        
        public string AreaCode { get; set; }
        
        public string CountryCode { get; set; }
        
        public string RepairShop { get; set; }
        
        public string EstimateCreationDate { get; set; }
        
        public string DaysSinceCreation { get; set; }
        
        public string EstimateApprovalDate { get; set; }
        
        public string DaysSinceApproval { get; set; }
        
        public string LastChangeToEstimate { get; set; }
        
        public string DaysSinceLastChange { get; set; }
        
        public string EstimatedTotalHours { get; set; }
        
        public string EstimatedTotalCostOfRepairInUSD { get; set; }
        
        public string Status { get; set; }
        
        public string PartNumber { get; set; }
        
        public string SHOP_CD { get; set; }
        
        public DateTime REPAIR_DTE { get; set; }
        
        public string REPAIR_CD { get; set; }
        
        public string REPAIR_DESC { get; set; }
        
        public string PART_CD { get; set; }
        
        public string QTY_PARTS { get; set; }
        
        public string COST_LOCAL { get; set; }
        
        public string SERIAL_NUMBER { get; set; }
        
        public string PART_DESC { get; set; }
        
        public string MSL_PART_SW { get; set; }
        
        public string CORE_PART_SW { get; set; }
        
        public string ManufacturerName { get; set; }
        
        public string RepairDesc { get; set; }
        
        public string MODE { get; set; }
        //
        //public string RepairCode { get; set; }
        //
        //public Int32 ContractAmount { get; set; }
        
        public DateTime EffectiveDate { get; set; }
        
        public DateTime ExpiryDate { get; set; }
        
        public Int32 ExratUSD { get; set; }
        
        public string RepairShopCode { get; set; }
        
        public Int16 StatusCode { get; set; }
        
        public string EqpmntNo { get; set; }
        //
        //public DateTime EstimateCreationDate { get; set; }
        
        public DateTime ApprovalDate { get; set; }
        
        public DateTime ChangeToEstimateDate { get; set; }
        //
        //public DateTime EstimatedTotalHours { get; set; }
        
        public DateTime EstimatedTotalCost { get; set; }
        
        public DateTime EstiamtedTotalCostRepair { get; set; }
        
        public string VendorReferenceNo { get; set; }

       //Created By Afroz

        public double? TOTREPAIRMANH { get; set; }
        
        public string EQPSize { get; set; }

        // End
         
        public string ChUser { get; set; }
        
        public System.DateTime ChTime { get; set; }
    }
}
