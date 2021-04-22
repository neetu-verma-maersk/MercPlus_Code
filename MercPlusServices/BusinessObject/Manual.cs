using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class Manual
    {
        
        public string ManualCode { get; set; }
        
        public string ManualDesc { get; set; }
        
        public string ManualActiveSW { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime ChTime { get; set; }

        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<ManualMode> ManualMode { get; set; }
        //public virtual ICollection<RepairCode> RepairCode { get; set; }
        public virtual ICollection<WorkOrder> WOrkOrder { get; set; }
    }
}
