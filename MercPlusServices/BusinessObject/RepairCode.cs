using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    class RepairCode
    {
        
        public string RepairCd { get; set; }
        
        public string ModeCd { get; set; }
        
        public string ManualCd { get; set; }
        
        public string RepairDesc { get; set; }
        
        public string RkrpRepairCd { get; set; }
        
        public string ChUser { get; set; }
        
        public Nullable<short> MaxQuantity { get; set; }
        
        public Nullable<decimal> ShopMaterialCeiling { get; set; }
        
        public System.DateTime Chts { get; set; }
        
        public string RepairInd { get; set; }
         
        public Nullable<double> ManHour { get; set; }
         
        public string RepairActiveSW { get; set; }
         
        public string MultipleUpdateSW { get; set; }
         
        public Nullable<double> WarrantyPeriod { get; set; }
         
        public string TaxAppliedSW { get; set; }
         
        public Nullable<short> RepairPriority { get; set; }
         
        public Nullable<int> IndexID { get; set; }
         
        public string AllowPartsSW { get; set; }

        public virtual Manual Manual { get; set; }
        public virtual Mode Mode { get; set; }
    }
}
