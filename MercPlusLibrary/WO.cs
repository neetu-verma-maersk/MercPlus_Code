using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class WO
    {
        [DataMember]
        public int WorkOrderID { get; set; }
        [DataMember]
        public string CustomerCode { get; set; }
        [DataMember]
        public string EqSize { get; set; }
        [DataMember]
        public string ModeCode { get; set; }
        [DataMember]
        public string WorkOrderType { get; set; }
        [DataMember]
        public string Eqouthgu { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public string ShopCode { get; set; }
        [DataMember]
        public Nullable<System.DateTime> RepairDate { get; set; }
        [DataMember]
        public string Eqowntp { get; set; }
        [DataMember]
        public string EqsType { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public Nullable<short> StatusCod { get; set; }
        [DataMember]
        public string EqMatr { get; set; }
        [DataMember]
        public string EqpType { get; set; }
        [DataMember]
        public string Cause { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DelDatsh { get; set; }
        [DataMember]
        public string ThirdParty { get; set; }
        [DataMember]
        public string StEmpty { get; set; }
        [DataMember]
        public string Strefurb { get; set; }
        [DataMember]
        public Nullable<decimal> ManhRate { get; set; }
        [DataMember]
        public Nullable<decimal> ManhRateCPH { get; set; }
        [DataMember]
        public Nullable<System.DateTime> RefrbDat { get; set; }
        [DataMember]
        public Nullable<decimal> ExchangeRate { get; set; }
        [DataMember]
        public string StreDel { get; set; }
        [DataMember]
        public Nullable<double> TotRepairManH { get; set; }
        [DataMember]
        public Nullable<double> FixCover { get; set; }
        [DataMember]
        public Nullable<double> TotManhReg { get; set; }
        [DataMember]
        public Nullable<double> DPP { get; set; }
        [DataMember]
        public Nullable<double> TotManHOt { get; set; }
        [DataMember]
        public string OffhirLocationSW { get; set; }
        [DataMember]
        public Nullable<decimal> OtRate { get; set; }
        [DataMember]
        public string Stselscr { get; set; }
        [DataMember]
        public Nullable<double> TotManhDate { get; set; }
        [DataMember]
        public string EqProfile { get; set; }
        [DataMember]
        public Nullable<double> TotManHMisc { get; set; }
        [DataMember]
        public Nullable<System.DateTime> EqInDate { get; set; }
        [DataMember]
        public Nullable<double> TotPrepHrs { get; set; }
        [DataMember]
        public string EqRuType { get; set; }
        [DataMember]
        public Nullable<double> TotalLaborHours { get; set; }
        [DataMember]
        public Nullable<decimal> TotalLaborCost { get; set; }
        [DataMember]
        public string EqRuMan { get; set; }
        [DataMember]
        public Nullable<decimal> TotalShopAmount { get; set; }
        [DataMember]
        public Nullable<decimal> TotalLaborCostCPH { get; set; }
        [DataMember]
        public Nullable<decimal> TotalCostLocal { get; set; }
        [DataMember]
        public string EqManCode { get; set; }
        [DataMember]
        public string VendorRefNo { get; set; }
        [DataMember]
        public Nullable<decimal> TotShopAmountCPH { get; set; }
        [DataMember]
        public string EqIOFlt { get; set; }
        [DataMember]
        public Nullable<decimal> TotalMaerskPartCPH { get; set; }
        [DataMember]
        public string VoucherNo { get; set; }
        [DataMember]
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        [DataMember]
        public Nullable<decimal> TotManPartsCPH { get; set; }
        [DataMember]
        public Nullable<decimal> TotalCostCPH { get; set; }
        [DataMember]
        public string CheckNo { get; set; }
        [DataMember]
        public Nullable<System.DateTime> PaidDate { get; set; }
        [DataMember]
        public Nullable<decimal> AmountPaid { get; set; }
        [DataMember]
        public Nullable<System.DateTime> RKRPRepairDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> RKRPXmitDate { get; set; }
        [DataMember]
        public string RKRPXmitSW { get; set; }
        [DataMember]
        public Nullable<System.DateTime> WorkOrderReceiveDate { get; set; }
        [DataMember]
        public string ApprovedBy { get; set; }
        [DataMember]
        public string ShopWorkingSW { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ApprovalDate { get; set; }
        [DataMember]
        public Nullable<decimal> ImportTax { get; set; }
        [DataMember]
        public Nullable<decimal> ImportTaxCPH { get; set; }
        [DataMember]
        public Nullable<decimal> SalesTaxLabor { get; set; }
        [DataMember]
        public Nullable<decimal> SalesTaxLaborCPH { get; set; }
        [DataMember]
        public Nullable<decimal> SalesTaxParts { get; set; }
        [DataMember]
        public Nullable<decimal> SalesTaxPartsCPH { get; set; }
        [DataMember]
        public Nullable<decimal> TotalMaerskParts { get; set; }
        [DataMember]
        public Nullable<decimal> TotalManParts { get; set; }
        [DataMember]
        public string ManualCode { get; set; }
        [DataMember]
        public string EquipmentNo { get; set; }
        [DataMember]
        public string CoType { get; set; }
        [DataMember]
        public string VendorCode { get; set; }
        [DataMember]
        public string ReqRemarkSW { get; set; }
        [DataMember]
        public string CUCDN { get; set; }
        [DataMember]
        public Nullable<decimal> OTRatePCH { get; set; }
        [DataMember]
        public Nullable<decimal> DTRate { get; set; }
        [DataMember]
        public Nullable<decimal> DTRateCPH { get; set; }
        [DataMember]
        public Nullable<decimal> MisRate { get; set; }
        [DataMember]
        public Nullable<decimal> MiscRateCPH { get; set; }
        [DataMember]
        public Nullable<decimal> TotalCostLocalUSD { get; set; }
        [DataMember]
        public Nullable<decimal> TotalCostRepair { get; set; }
        [DataMember]
        public Nullable<decimal> TotalCOstRepairCPH { get; set; }
        [DataMember]
        public Nullable<double> SalesTaxLaborPct { get; set; }
        [DataMember]
        public Nullable<double> SalesTaxPartsPCT { get; set; }
        [DataMember]
        public Nullable<double> ImportTaxPart { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CreateTime { get; set; }
        [DataMember]
        public Nullable<decimal> AgentPartsTax { get; set; }
        [DataMember]
        public Nullable<decimal> AgentTaxPartsCPH { get; set; }
        [DataMember]
        public string CountryCUCDN { get; set; }
        [DataMember]
        public Nullable<decimal> CountryExchangeRate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CountryExchangeDate { get; set; }
        [DataMember]
        public string Damage { get; set; }
        [DataMember]
        public string LsComp { get; set; }
        [DataMember]
        public string LsContr { get; set; }
        [DataMember]
        public Nullable<short> RevisionNo { get; set; }
        [DataMember]
        public Nullable<short> PreviousStatus { get; set; }
        [DataMember]
        public Nullable<System.DateTime> PreviewDate { get; set; }
        [DataMember]
        public string PreviousLocationCode { get; set; }
        [DataMember]
        public Nullable<decimal> TotalWMaterialAmount { get; set; }
        [DataMember]
        public Nullable<decimal> TotalTMaterialAmount { get; set; }
        [DataMember]
        public Nullable<decimal> TotalWMaterialAmountCPH { get; set; }
        [DataMember]
        public Nullable<decimal> TotalTMaterialAmountCPH { get; set; }
        [DataMember]
        public Nullable<decimal> TotalWMaterialAmountCPHUSD { get; set; }
        [DataMember]
        public Nullable<decimal> TotalTMaterialAmountUSD { get; set; }
        [DataMember]
        public Nullable<decimal> TotalTMaterialAmountCPHUSD { get; set; }
        //[DataMember]
        //public Nullable<decimal> TotalWMaterialAmountCPHUSD { get; set; }
        [DataMember]
        public string PresentLocation { get; set; }
        [DataMember]
        public Nullable<System.DateTime> GateInDate { get; set; }
        [DataMember]
        public Nullable<int> PreviousWOrkOrderID { get; set; }

        [DataMember]
        public Currency Currency { get; set; }
        [DataMember]
        public Customer Customer { get; set; }
        [DataMember]
        public EqType EqType { get; set; }
        [DataMember]
        public Manual Manual { get; set; }
        [DataMember]
        public Mode Mode { get; set; }
        [DataMember]
        public Shop Shop { get; set; }
        [DataMember]
        public StatusCode StatusCode { get; set; }
        [DataMember]
        public Vendor Vendor { get; set; }
        [DataMember]
        public string LocCode { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public double AvgtimeToEstimate { get; set; }
        [DataMember]
        public double AvgTimeToAuthorise { get; set; }
        [DataMember]
        public double AvgTimeToRepair { get; set; }
        //[DataMember]
        //public virtual ICollection<MESC1TS_WOREPAIR> MESC1TS_WOREPAIR { get; set; }
    }
}
