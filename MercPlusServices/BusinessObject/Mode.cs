using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class Mode
    {
        
        public string ModeCd { get; set; }
        
        public string ModeDescription { get; set; }
        
        public System.DateTime ChTime { get; set; }
        
        public string StandardTimeSW { get; set; }
        
        public string ValidationSW { get; set; }
        
        public string ModeActiveSW { get; set; }
        
        public string ChUser { get; set; }
        
        public string ModeInd { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual ICollection<CphEqpLimit> CphEqpLimit { get; set; }
        public virtual ICollection<CustShopMode> CustShopMode { get; set; }
        public virtual ICollection<EqMode> EqMode { get; set; }
        public virtual ICollection<ManualMode> ManualMode { get; set; }
        public virtual ICollection<NonsCode> NonsCode { get; set; }
        public virtual ICollection<PrepTime> PrepTime { get; set; }
        //public virtual ICollection<RepairCode> RepairCode { get; set; }
        //public virtual ICollection<Transmit> Transmit { get; set; }
        public virtual ICollection<WorkOrder> WorkOrder { get; set; }
    }
}
