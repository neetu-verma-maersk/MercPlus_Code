using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class RepairLocationCode
    {
        
        public string RepairCode { get; set; }
        
        public string RepairDescription { get; set; }
        
        public string Description { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime ChTime { get; set; }
    }
}
