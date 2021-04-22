using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class PartsGroup
    {
        
        public string PartsGroupCd { get; set; }
        
        public string PartsGroupDesc { get; set; }
        
        public string Remarks { get; set; }
        
        public string PartsGroupActiveSW { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual ICollection<MasterPart> MasterPart { get; set; }
    }
}
