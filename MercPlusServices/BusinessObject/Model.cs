using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class Model
    {
        
        public string ModelNo { get; set; }
        
        public string IndicatorCd { get; set; }
        
        public string ManufacturCd { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual Manufactur Manufactur { get; set; }
    }
}
