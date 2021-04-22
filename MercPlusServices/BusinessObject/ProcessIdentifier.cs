using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public partial class ProcessIdentifier
    {
        
        public int ProcessID { get; set; }
        
        public string ProcessDesc { get; set; }
        
        public string ParamName { get; set; }
        
        public string ParamValue { get; set; }
    }
}
