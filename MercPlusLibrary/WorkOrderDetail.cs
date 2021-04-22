using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class WorkOrderDetail
    {
        [DataMember]
        public Shop Shop { get; set; }
        [DataMember]
        public List<Equipment> EquipmentList { get; set; }
        [DataMember]
        public List<RepairsView> RepairsViewList { get; set; }
        [DataMember]
        public List<SparePartsView> SparePartsViewList { get; set; }

        [DataMember]
        public int WorkOrderID { get; set; }
        [DataMember]
        public string AreaCode { get; set; }
        [DataMember]
        public string RateEffectiveDate { get; set; }
        [DataMember]
        public double? TotalRepairManHour { get; set; }
        [DataMember]
        public string ThirdPartyPort { get; set; }
        [DataMember]
        public string Cause { get; set; }
        [DataMember]
        public string Mode { get; set; }
        [DataMember]
        public string Remarks { get; set; }
        [DataMember]
        public bool ReqdRemarkSW { get; set; }
        [DataMember]
        public DateTime CompletionDate { get; set; } //Similar to Repair_Dte???
        [DataMember]
        public decimal? SalesTaxParts { get; set; }
        [DataMember]
        public decimal? SalesTaxPartsCPH { get; set; }
        [DataMember]
        public decimal? SalesTaxLabour { get; set; }
        [DataMember]
        public decimal? SalesTaxLabourCPH { get; set; }
        [DataMember]
        public decimal? ImportTax { get; set; }
        [DataMember]
        public decimal? ImportTaxCPH { get; set; }
        [DataMember]
        public string WorkOrderType { get; set; }
        //[DataMember]
        //public bool IsCreateNew { get; set; }
        //New added by bishnu
        [DataMember]
        public short? WorkOrderStatus { get; set; } // Status_Code
        [DataMember]
        public Nullable<decimal> ManHourRate { get; set; }
        [DataMember]
        public Nullable<decimal> ManHourRateCPH { get; set; }
        [DataMember]
        public Nullable<decimal> DoubleTimeRate { get; set; }
        [DataMember]
        public Nullable<decimal> DoubleTimeRateCPH { get; set; }
        [DataMember]
        public Nullable<decimal> MiscRate { get; set; }
        [DataMember]
        public Nullable<decimal> MiscRateCPH { get; set; }
        [DataMember]
        public Nullable<decimal> OverTimeRate { get; set; }
        [DataMember]
        public Nullable<decimal> OverTimeRateCPH { get; set; }
        [DataMember]
        public decimal? CountryExchangeRate { get; set; } //Similar to exchange rate?
        [DataMember]
        public decimal? TotalEDIAmount { get; set; }
        [DataMember]
        public Nullable<double> TotalPrepHours { get; set; }
        [DataMember]
        public Nullable<double> TotalLaborHours { get; set; }
        [DataMember]
        public Nullable<decimal> TotalShopAmount { get; set; }
        [DataMember]
        public decimal? TotalWMaterialAmount { get; set; }
        [DataMember]
        public decimal? TotalTMaterialAmount { get; set; }
        [DataMember]
        public decimal? TotalWMaterialAmountUSD { get; set; }
        [DataMember]
        public decimal? TotalTMaterialAmountUSD { get; set; }
        [DataMember]
        public decimal? TotalWMaterialAmountCPHUSD { get; set; }
        [DataMember]
        public decimal? TotalTMaterialAmountCPHUSD { get; set; }
        [DataMember]
        public decimal? TotalWMaterialAmountCPH { get; set; }
        [DataMember]
        public decimal? TotalTMaterialAmountCPH { get; set; }
        [DataMember]
        public decimal? AmountPaid { get; set; }
        [DataMember]
        public DateTime? InvoiceDate { get; set; }
        [DataMember]
        public DateTime? RKRPRepairDate { get; set; }
        [DataMember]
        public DateTime? RKRPXMITDate { get; set; }
        [DataMember]
        public DateTime? WorkOrderReceiveDate { get; set; }
        [DataMember]
        public string CheckNo { get; set; }
        [DataMember]
        public DateTime? PaidDate { get; set; }
        [DataMember]
        public string RKRPXMITSW { get; set; }
        [DataMember]
        public string ApprovedBy { get; set; }
        [DataMember]
        public DateTime? ApprovalDate { get; set; }
        [DataMember]
        public string ShopWorkingSW { get; set; }
        [DataMember]
        public string VendorCode { get; set; }
        [DataMember]
        public decimal? TotalCostOfRepair { get; set; }
        [DataMember]
        public decimal? TotalCostOfRepairCPH { get; set; }
        //[DataMember]
        //public string STSELSCR { get; set; }
        [DataMember]
        public short? RevisionNo { get; set; }
        [DataMember]
        public int woState { get; set; }

        /// <summary>
        /// Ordinary hours
        /// </summary>
        //[DataMember]
        //public Nullable<double> TotalManHourReg { get; set; }

        /// <summary>
        /// Overtime1
        /// </summary>
        //[DataMember]
        //public Nullable<double> TotalManHourOverTime { get; set; }

        /// <summary>
        /// Overtime2
        /// </summary>
        //[DataMember]
        //public Nullable<double> TotalManHourDoubleTime { get; set; }

        /// <summary>
        /// Overtime3
        /// </summary>
        //[DataMember]
        //public Nullable<double> TotalManHourMisc { get; set; }

        //Create By Afroz
        //Header

        [DataMember]
        public string EquipmentNo { get; set; } //Already declared in equipment class

        [DataMember]
        public string VoucherNumber { get; set; }
        [DataMember]
        public string EQPSize { get; set; } //Already declared in equipment class
        [DataMember]
        public string TotalHours { get; set; } //????
        [DataMember]
        public decimal? TotalLabourCostCPH { get; set; }
        [DataMember]
        public string StatusCode { get; set; } //???
        [DataMember]
        public string Status { get; set; } //???
        [DataMember]
        public DateTime EffectiveDate { get; set; } //??
        [DataMember]
        public DateTime ExpiryDate { get; set; }
        [DataMember]
        public string EqpmntNo { get; set; }  //Already declared in equipment class
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        //End

        //ashiqur
        [DataMember]
        public DateTime? RepairDate { get; set; }
        [DataMember]
        public Decimal? ExchangeRate { get; set; }
        [DataMember]
        public decimal? TotalLabourCost { get; set; }
        [DataMember]
        public decimal? TotalShopAmountCPH { get; set; }
        [DataMember]
        public decimal? TotalCostLocal { get; set; }
        [DataMember]
        public decimal? TotalCostCPH { get; set; }
        [DataMember]
        public decimal? TotalCostLocalUSD { get; set; }
        [DataMember]
        public decimal? TotalMaerksParts { get; set; }
        [DataMember]
        public decimal? TotalMaerksPartsCPH { get; set; }
        [DataMember]
        public decimal? TotalManParts { get; set; }
        [DataMember]
        public decimal? TotalManPartsCPH { get; set; }
        //[DataMember]
        //public DateTime? Deldatsh { get; set; } //PTI Date
        //[DataMember]
        //public string StEmptyFullInd { get; set; } //Full/Empty Indicator
        //[DataMember]
        //public string Strefurb { get; set; } //In eqpmnt
        //[DataMember]
        //public DateTime? RefurbishmnentDate { get; set; }
        //[DataMember]
        //public string Stredel { get; set; } //Global hunt
        //[DataMember]
        //public double? Fixcover { get; set; }
        //[DataMember]
        //public double? Dpp { get; set; }
        //[DataMember]
        //public string OffhirLocationSW { get; set; }
        //[DataMember]
        //public char StEqpForSale { get; set; } //eqpmnt for sale, stselscr
        [DataMember]
        public double? SalesTaxLaborPCT { get; set; }
        [DataMember]
        public double? SalesTaxPartsPCT { get; set; }
        [DataMember]
        public double? ImportTaxPCT { get; set; }
        [DataMember]
        public decimal? AgentPartsTax { get; set; }
        [DataMember]
        public decimal? AgentPartsTaxCPH { get; set; }
        //[DataMember]
        //public string ReqRemarkSW { get; set; }
        [DataMember]
        public string CountryCUCDN { get; set; } //new field in db?
        [DataMember]
        public DateTime? CountryExchangeDate { get; set; } //new field in db?
        //[DataMember]
        //public string LSCompany { get; set; } //new field in db?
        //[DataMember]
        //public string LSCountry { get; set; } //new field in db?
        [DataMember]
        public string XmitSW { get; set; }
        [DataMember]
        public int? PrevWorkOrderID { get; set; }
        [DataMember]
        public short? PrevStatus { get; set; }
        [DataMember]
        public string PrevLocCode { get; set; }
        [DataMember]
        public DateTime? PrevDate { get; set; }
        [DataMember]
        public string PresentLoc { get; set; }
        [DataMember]
        public DateTime? GateInDate { get; set; }
        [DataMember]
        public string LeaseComp { get; set; }
        [DataMember]
        public string LeaseContract { get; set; }
        private double? _reg;
        [DataMember]
        public double? TotalManHourReg
        {
            get { return (_reg == null ? 0.00f : _reg); }
            set { _reg = value; }
        }

        private double? _ot1;
        [DataMember]
        public double? TotalManHourOverTime
        {
            get { return (_ot1 == null ? 0.00f : _ot1); }
            set { _ot1 = value; }
        }

        private double? _ot2;
        [DataMember]
        public double? TotalManHourDoubleTime
        {
            get { return (_ot2 == null ? 0.00f : _ot2); }
            set { _ot2 = value; }
        }

        private double? _ot3;
        [DataMember]
        public double? TotalManHourMisc
        {
            get { return (_ot3 == null ? 0.00f : _ot3); }
            set { _ot3 = value; }
        }

        [DataMember]
        public bool IsSingle { get; set; }

        [DataMember]
        public DateTime SalesTaxPartsCost { get; set; }
        [DataMember]
        public DateTime SalesTaxLabourCost { get; set; }
        [DataMember]
        public DateTime ImportTaxCost { get; set; }
        //rohit add every where
        [DataMember]
        public float TotCostRepairCPH { get; set; }
        [DataMember]
        public List<RemarkEntry> RemarksList { get; set; }
        //rohit addition
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string ModeDescription { get; set; }
        [DataMember]
        public string PayAgentCode { get; set; }
        //For DERT
        [DataMember]
        public decimal? TotalCostLocalExclTaxVat { get; set; }
        [DataMember]
        public decimal? SumRepairMaterialAmt { get; set; }
        [DataMember]
        public decimal? SumPartCost { get; set; }

        //Pinaki Added for Grade Code
        [DataMember]
        public string GradeCode { get; set; }
        [DataMember]
        public string NewGradeCode { get; set; }
    }
}
