using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class ModeEntry
    {
        
        public string EqType { get; set; }
        
        public string SubType { get; set; }
        
        public string Size { get; set; }
        
        public string Aluminum { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime ChTime { get; set; }
    }
}
