using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
//using System.ServiceModel.Web;
using System.Text;
//using System.Data.Objects;

namespace MercPlusLibrary
{
    [DataContract]
    public class Equipment
    {
        //to be added
        public enum CONTAINERTYPE
        {
            REFU,
            REEF,
            CONT,
            GENS,
            RFHV,
            ALU,
            CHAS
        }

        [DataMember]
        public string EquipmentNo { get; set; }
        [DataMember]
        public string VendorRefNo { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string COType { get; set; }
        //[DataMember]
        //public string SubType { get; set; } 
        [DataMember]
        public string EQProfile { get; set; }
        [DataMember]
        public string Size { get; set; }
        //[DataMember]
        //public string Material { get; set; }
        [DataMember]
        public string BoxMfg { get; set; }
        [DataMember]
        public string LeasingCompany { get; set; }

        /// <summary>
        /// Use EqInDate instead of InService
        /// </summary>
        //[DataMember]
        //public string InService { get; set; }
        [DataMember]
        public string ReeferMakeModel { get; set; }
        //[DataMember]
        //public DateTime PTIDate { get; set; }
        [DataMember]
        public string GensetMakeModel { get; set; }
        [DataMember]
        public string Damage { get; set; }
        [DataMember]
        public string UnitIdentifierDigit { get; set; }
        [DataMember]
        public string SelectedMode { get; set; }
        [DataMember]
        public List<Mode> ModeList { get; set; }
        [DataMember]
        public string EqpNotFound { get; set; }
        [DataMember]
        public DateTime ExtensionDate { get; set; }
        [DataMember]
        public string LeasingContract { get; set; }
        //Added by bishnu
        [DataMember]
        public string MaerskSwitch { get; set; }
        [DataMember]
        public DateTime? EQInDate { get; set; }
        [DataMember]
        public string ReqRemarkSW { get; set; }

        [DataMember]
        public string Eqouthgu { get; set; }   /// Ashiqur 

        //USing Eqstype instead of SubType                               
        [DataMember]
        public string Eqstype { get; set; }
        [DataMember]
        public string Eqowntp { get; set; }

        /// <summary>
        /// Use Eqmatr instead of Material
        /// </summary>
        [DataMember]
        public string Eqmatr { get; set; }
        [DataMember]
        public string EqMancd { get; set; }
        [DataMember]
        public string EQRuman { get; set; }
        [DataMember]
        public string EQRutyp { get; set; }
        [DataMember]
        public string EQIoflt { get; set; }
        //to add rohit
        //[DataMember]
        //public string EqRUMan { get; set; }
        //[DataMember]
        //public string EqRUTyp { get; set; }
        //[DataMember]
        //public string EqIOFlt { get; set; }
        //[DataMember]
        //public DateTime EQINDAT { get; set; }
        //set this property value from RKEM call
        [DataMember]
        public string RKEM_EQOWNThirdParty { get; set; }
        [DataMember]
        public string PLAWHO { get; set; }
        // Allow some shops to bypass some rules for leased equipment
        [DataMember]
        public bool BypassLease { get; set; }
        [DataMember]
        public DateTime? GateInDate { get; set; }
        [DataMember]
        public string LeaseComp { get; set; }
        [DataMember]
        public string LeaseContract { get; set; }

        /// <summary>
        /// Use Deldatsh instead of PTI Date
        /// </summary>
        [DataMember]
        public DateTime? Deldatsh { get; set; } //PTI Date
        [DataMember]
        public string StEmptyFullInd { get; set; } //Full/Empty Indicator
        [DataMember]
        public string Strefurb { get; set; } //In eqpmnt
        [DataMember]
        public DateTime? RefurbishmnentDate { get; set; }
        [DataMember]
        public string Stredel { get; set; } //Global hunt
        [DataMember]
        public double? Fixcover { get; set; }
        [DataMember]
        public double? Dpp { get; set; }
        [DataMember]
        public string OffhirLocationSW { get; set; }
        [DataMember]
        public string STSELSCR { get; set; }
        [DataMember]
        public string PresentLoc { get; set; }
        [DataMember]
        public string CurrentMove { get; set; }
        [DataMember]
        public string CurrentLoc { get; set; }
        [DataMember]
        public string GradeCode { get; set; } //added
        [DataMember]
        public string NewGradeCode { get; set; } //added
        [DataMember]
        public string TempGradeCode { get; set; } //added
    }
}
