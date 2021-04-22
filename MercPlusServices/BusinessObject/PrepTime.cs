using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class PrepTime
    {
        
        public string ModeCd { get; set; }
        
        public string PrepCd { get; set; }
        
        public double PrepTimeMax { get; set; }
        
        public Nullable<double> PrepHrs { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual Mode Mode { get; set; }
    }
}
