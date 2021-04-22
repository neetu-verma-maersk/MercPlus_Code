using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class EqsType
    {
        
        public string EqSType { get; set; }
        
        public string CoType { get; set; }
        
        public string TypeDesc { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }
    }
}
