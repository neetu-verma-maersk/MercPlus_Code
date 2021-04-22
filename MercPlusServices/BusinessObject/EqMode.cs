using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class EqMode
    {
        
        public int EqModeID { get; set; }
        
        public string EqsType { get; set; }
        
        public string EqSize { get; set; }
        
        public string ModeCd { get; set; }
        
        public string CoType { get; set; }
        
        public string ChUser { get; set; }
        
        public string AluminiumSW { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual Mode Mode { get; set; }
    }
}
