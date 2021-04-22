using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class RepairsView
    {
        [DataMember]
        public Damage Damage { get; set; }
        [DataMember]
        public RepairCode RepairCode { get; set; }
        [DataMember]
        public RepairLoc RepairLocationCode { get; set; }
        [DataMember]
        public Tpi Tpi { get; set; }
        [DataMember]
        public int Pieces { get; set; }
        [DataMember]
        public decimal? MaterialCostCPH { get; set; }
        [DataMember]
        public decimal? MaterialCost { get; set; }
        [DataMember]
        public double? ManHoursPerPiece { get; set; }
        [DataMember]
        public bool IsRepairTaxCode { get; set; }
        [DataMember]
        public decimal? TotalPerCode { get; set; }


        //added by bishnu
        [DataMember]
        public NonsCode NonsCode { get; set; }
        [DataMember]
        public int WorkOrderID { get; set; }
        [DataMember]
        public int rState { get; set; }
    }
}
