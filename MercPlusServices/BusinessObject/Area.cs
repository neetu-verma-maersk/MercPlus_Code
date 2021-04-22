using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class Area
    {
        
        public string AreaCode { get; set; }
        
        public string AreaDescription { get; set; }
        
        public string AreaGeoID { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual ICollection<Country> Country { get; set; }
    }
}
