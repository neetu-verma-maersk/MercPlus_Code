using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{

    public class RprcodeExclu
    {
        
        public string RepairCd { get; set; }
        
        public string ExcluRepairCd { get; set; }
        
        public string Mode { get; set; }
        
        public string ManualCd { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }
    }
}
