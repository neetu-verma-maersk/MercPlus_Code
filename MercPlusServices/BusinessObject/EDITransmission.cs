using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class EDITransmission
    {
        
        public int EDIId { get; set; }
        
        public Nullable<System.DateTime> Crts { get; set; }
        
        public Nullable<int> WOQty { get; set; }
        
        public Nullable<int> WOPassQty { get; set; }
        
        public Nullable<int> WOFailQty { get; set; }
    }
}
