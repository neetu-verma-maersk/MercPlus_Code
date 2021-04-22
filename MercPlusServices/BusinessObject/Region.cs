using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    class Region
    {
        
        public string RegionCd { get; set; }
        
        public string RegionDesc { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual ICollection<Location> Location { get; set; }
    }
}
