using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using MercPlusClient.ManageWorkOrderServiceReference;
using System.ComponentModel;

namespace MercPlusClient.Areas.ManageWorkOrder.Models
{
    public class ManagerApprovalModel
    {
        public enum REPAIRSPARE { REPAIR = 1, SPARE = 2 }
        public enum DLGTYPE { DAMAGE = 1, REPAIR = 2, REPAIRLOCCODE = 3, TPI = 4 }

        public List<SelectListItem> DDLShop;
        public List<SelectListItem> DDLCustomer;
        public List<SelectListItem> DDLCause;
        public List<SelectListItem> DDLMode;
        public List<SelectListItem> DDLNEWTPI;//Debadrita_TPI_Indicator-19-07-19

        public WorkOrderDetail dbWOD;
        public List<Shop> ShopList;
        public List<Equipment> EquipmentList;

        public bool IsSingle { get; set; }
        public bool IsCreateNew { get; set; }
        public bool IsSaveAsDraft { get; set; }
        public bool ShowComments { get; set; }
        public bool IsCountryOrAbove { get; set; }
        public bool IsApprover { get; set; }
        public bool CreateRemarksTA { get; set; }
        public bool CreateApprovalBtn { get; set; }

        [Display(Name = "Shop Code")]
        public string ShopCode { get; set; }

        [Display(Name = "Customer")]
        public string CustomerCode { get; set; }

        [Display(Name = "Cause")]
        public string Cause { get; set; }

        public string CauseCode { get; set; }

        [Display(Name = "3rd Party Port")]
        public string ThirdPartyPort { get; set; }

        [Display(Name = "Currency")]
        public string CurrencyName { get; set; }

        public string CurrCode { get; set; }

        [Display(Name = "Equipment No")]
        public string EquipmentNo { get; set; }

        [Display(Name = "Mode")]
        public string Mode { get; set; }

        [Display(Name = "External Remarks")]
        public string ERemarks { get; set; }

        [Display(Name = "Shop Remarks")]
        public string SRemarks { get; set; }

        [Display(Name = "Internal Remarks")]
        public string IRemarks { get; set; }

        [Display(Name = "Vendor Remarks")]
        public string VRemarks { get; set; }

        [Display(Name = "Equipment Information")]
        public string EquipmentInformation { get; set; }

        [Display(Name = "RKEM Damage")]                                                                             //MERC-RKEM Damage Code
        public string Damage { get; set; }

        [Display(Name = "Merc Damage")]                                                                             //MERC-RKEM Damage Code
        public string MercDamage { get; set; }

        [Display(Name = "Curr Location")]                                                                             //MERC-RKEM Damage Code
        public string CurrLoc { get; set; }

        [Display(Name = "Curr Move")]                                                                             //MERC-RKEM Damage Code
        public string CurrMove { get; set; }

        [Display(Name = "RKEM Grade")]                                                                             //MERC-RKEM Damage Code
        public string RGrade { get; set; }

        [Display(Name = "Merc Grade")]                                                                             //MERC-RKEM Damage Code
        public string MercGrade { get; set; }

        [Display(Name = "Damage Code")]
        public string DamageCode { get; set; }

        [Display(Name = "Repair Code")]
        public string RepairCode { get; set; }

        [Display(Name = "Repair Loc Code")]
        public string RepairLocationCode { get; set; }

        private int _pcs = 0;
        [Display(Name = "Pcs")]
        [DefaultValue(0)]
        public int Pcs
        {
            get { return _pcs; }
            set { _pcs = value; }
        }

        private float _mcpp = 0.00f;
        [Display(Name = "Material Cost Per Piece")]
        [DefaultValue(0.00f)]
        public float MaterialCostPerPiece
        {
            get { return _mcpp; }
            set { _mcpp = value; }
        }

        private float _mhpp = 0.00f;
        [Display(Name = "Man Hours Per Piece")]
        [DefaultValue(0.00f)]
        public float ManHoursPerPiece
        {
            get { return _mhpp; }
            set { _mhpp = value; }
        }

        [Display(Name = "TPI Indicator")]
        public string TPI { get; set; }

        [Display(Name = "New TPI")] //Debadrita_TPI_Indicator-19-07-19
        public List<SelectListItem> drpTPIList = new List<SelectListItem>();
        public string NewTPICode { get; set; }


        [Display(Name = "Unit Identifier Digit")]
        public string UnitIdentifierDigit { get; set; }

        [Display(Name = "Vendor Ref#")]
        public string VendorRefNo { get; set; }

        private double? _oh = 0.00;
        [Display(Name = "Ordinary Hours:")]
        [DefaultValue(0.00)]
        public double? TotalManHourReg
        {
            get { return _oh; }
            set { _oh = value; }
        }

        private double? _ot1 = 0.00;
        [Display(Name = "OT1:")]
        [DefaultValue(0.00)]
        public double? TotalManHourOverTime
        {
            get { return _ot1; }
            set { _ot1 = value; }
        }

        private double? _ot2 = 0.00;
        [Display(Name = "OT2:")]
        [DefaultValue(0.00)]
        public double? TotalManHourDoubleTime
        {
            get { return _ot2; }
            set { _ot2 = value; }
        }

        private double? _ot3 = 0.00;
        [Display(Name = "OT3:")]
        [DefaultValue(0.00)]
        public double? TotalManHourMisc
        {
            get { return _ot3; }
            set { _ot3 = value; }
        }

        [Display(Name = "Parts No.")]
        public string OwnerSuppliedPartsNumber { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual List<Damage> LstDamage { get; set; }

        public virtual List<RepairCode> LstRepairCode { get; set; }

        public virtual List<RepairLoc> LstRepLocCode { get; set; }

        public virtual List<Tpi> LstTPI { get; set; }

        public int RAdditionalRows { get; set; }
        public int SAdditionalRows { get; set; }
        public int RMaxQuantity { get; set; }
        public string ManualCode { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "ReeferMakeModel")]
        public string ReeferMakeModel { get; set; }

        [Display(Name = "PTI Date")]
        public string Deldatsh { get; set; }

        [Display(Name = "Profile")]
        public string Profile { get; set; }

        [Display(Name = "Material")]
        public string Material { get; set; }

        [Display(Name = "Leasing Company")]
        public string LeasingCompany { get; set; }

        [Display(Name = "In Service")]
        public string InService { get; set; }

        [Display(Name = "GenSet Make/Model")]
        public string GenSetMakeModel { get; set; }

        [Display(Name = "Extension Date")]
        public string ExtensionDate { get; set; }

        public string EqpNotFound { get; set; }

        [Display(Name = "Box Mfg")]
        public string BoxMfg { get; set; }

        [Display(Name = "Leasing Contract")]
        public string LeasingContract { get; set; }

        public int RepairDlgType { get; set; }

        [Display(Name = "Completion Date")]
        public DateTime CompletionDate { get; set; }

        [Display(Name = "Sales Tax Parts Cost:")]
        public Decimal SalesTaxParts { get; set; }

        [Display(Name = "Sales Tax Labour Cost:")]
        public Decimal SalesTaxLabour { get; set; }

        [Display(Name = "Import Tax Cost:")]
        public Decimal ImportTax { get; set; }

        [Display(Name = "Status:")]
        public string WOStatus { get; set; }

        [Display(Name = "Previous Status:")]
        public string PrevStatus { get; set; }

        [Display(Name = "Present Location:")]
        public string PresentLoc { get; set; }

        [Display(Name = "Gate In Date:")]
        public string GateInDate { get; set; }

        [Display(Name = "For Sale:")]
        public string StSelCR { get; set; }

        [Display(Name = "Owner:")]
        public string Owner { get; set; }

        [Display(Name = "At off Hire Loc:")]
        public string AtOfcHrLoc { get; set; }

        [Display(Name = "On Global Hunt:")]
        public string OnGlobalHunt { get; set; }

        [Display(Name = "Empty:")]
        public string Empty { get; set; }

        [Display(Name = "Fixed Cover:")]
        public string FixedCover { get; set; }

        [Display(Name = "For Refurb:")]
        public string ForRefurb { get; set; }

        [Display(Name = "DPP:")]
        public string DPP { get; set; }

        [Display(Name = "Refurb Date:")]
        public string RefurbDate { get; set; }



        public string StatusDesc { get; set; }

        [Display(Name = "Previous Date:")]
        public string PrevDate { get; set; }

        [Display(Name = "Previous Location:")]
        public string PrevLocation { get; set; }

        [Display(Name = "Revision No:")]
        public string RevisionNo { get; set; }

        [Display(Name = "Estimate No:")]
        public string EstimateNo { get; set; }

        [Display(Name = "Core s/n or Tag No.")]
        public string TagNo { get; set; }

        [Display(Name = "Part Cost")]
        public string PartCost { get; set; }

        [Display(Name = "Total per Code")]
        public string TotalPerCode { get; set; }

        public string CONTACT_EQSAL_SW { get; set; }
        public string SHOP_WORKING_SW { get; set; }

        /*User Role Properties*/
        public bool ADMIN { get; set; }
        public bool CPH { get; set; }
        public bool EMR_SPECIALIST_COUNTRY { get; set; }
        public bool EMR_SPECIALIST_SHOP { get; set; }
        public bool EMR_APPROVER_COUNTRY { get; set; }
        public bool EMR_APPROVER_SHOP { get; set; }
        public bool SHOP { get; set; }
        public bool MPRO_CLUSTER { get; set; }
        public bool MPRO_SHOP { get; set; }
        public bool READONLY { get; set; }    
        
        public bool ISANYCPH { get; set; }
        public bool ISANYSHOP { get; set; }

        public bool REGION { get; set; }
        public bool CENEQULOS { get; set; }        

        public int RepCodeCount { get; set; }
        public int SpareCodeCount { get; set; }
        public bool DISPLAY_STATUS_CODE { get; set; }
        public string RemarksDetails { get; set; }
        public string LoadButtons { get; set; }
        public string ErrorMsg { get; set; }

    }
}