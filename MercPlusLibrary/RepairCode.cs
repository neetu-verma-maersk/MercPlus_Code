using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class RepairCode
    {
        [DataMember]
        public string RepairCod { get; set; }
        [DataMember]
        public string ModeCode { get; set; }
        [DataMember]
        public string ManualCode { get; set; }
        [DataMember]
        public string RepairDesc { get; set; }
        [DataMember]
        public string RkrpRepairCode { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public Nullable<short> MaxQuantity { get; set; }
        [DataMember]
        public Nullable<decimal> ShopMaterialCeiling { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public string RepairInd { get; set; }
        [DataMember] 
        public Nullable<double> ManHour { get; set; }
        [DataMember] 
        public string RepairActiveSW { get; set; }
        [DataMember] 
        public string MultipleUpdateSW { get; set; }
        [DataMember] 
        public Nullable<double> WarrantyPeriod { get; set; }
        [DataMember] 
        public string TaxAppliedSW { get; set; }
        [DataMember] 
        public Nullable<short> RepairPriority { get; set; }
        [DataMember] 
        public Nullable<int> IndexID { get; set; }
        [DataMember] 
        public string AllowPartsSW { get; set; }

        [DataMember]
        public Manual Manual { get; set; }
        [DataMember]
        public Mode Mode { get; set; }
        [DataMember]
        public string RepairCodeNotFound { get; set; }
        [DataMember]
        public bool PrepTime { get; set; }
        //[DataMember]
        //public int rState { get; set; } 
    }
}
