using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class ManualMode
    {
        
        public string ModeCd { get; set; }
        
        public string ManualCd { get; set; }
        
        public string ActiveSw { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual Manual Manual { get; set; }
        public virtual Mode Mode { get; set; }
    }
}
