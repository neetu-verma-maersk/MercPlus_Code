using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class NonsCode
    {
        
        public string ModeCd { get; set; }
        
        public string NonsCd { get; set; }
        
        public string NonsDesc { get; set; }
        
        public string NonsActiveSW { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual Mode Mode { get; set; }
    }
}
