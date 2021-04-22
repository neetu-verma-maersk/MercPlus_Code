using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class SparePartsView
    {
        [DataMember]
        public RepairCode RepairCode { get; set; }
        [DataMember]
        public double? Pieces { get; set; }
        [DataMember]
        public string OwnerSuppliedPartsNumber { get; set; }
        //Added by bishnu (check PartRecord.cpp. It seems the below fields are necessary in a PartRecord.h class)
        //[DataMember]
        //public string sCostLocal { get; set; }
        //[DataMember]
        //public string sCostLocalCPH { get; set; }
        [DataMember]
        public decimal? CoreValue { get; set; } //
        [DataMember]
        public string SerialNumber { get; set; }
        [DataMember]
        public int pState { get; set; }
        [DataMember]
        public int PartCode { get; set; }
        [DataMember]
        public string PartDescription { get; set; }
        [DataMember]
        public int WorkOrderID { get; set; }
        //end of table columns

        //for calculations
        [DataMember]
        public string MslPartSW { get; set; }
        [DataMember]
        public decimal? fCostNoMarkup { get; set; } //
        [DataMember]
        public decimal? fCostCPHNoMarkup { get; set; } //
        [DataMember]
        public decimal? CostLocal { get; set; } //
        [DataMember]
        public decimal? CostLocalCPH { get; set; } //
        /*added by rohit for checkpart local cost issues*/
        [DataMember]
        public char PART_ACTIVE_SW { get; set; } //
        [DataMember]
        public string MANUFCTR { get; set; } //
        [DataMember]
        public decimal? PART_PRICE { get; set; } //
        [DataMember]
        public char DEDUCT_CORE { get; set; } //        
        [DataMember]
        public char CORE_PART_SW { get; set; } //

    }
}
