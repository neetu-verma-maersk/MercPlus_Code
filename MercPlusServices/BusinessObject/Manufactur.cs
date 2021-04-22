using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class Manufactur
    {
        
        public string ManufacturCd { get; set; }
        
        public Nullable<double> DiscountPercent { get; set; }
        
        public string ManufacturName { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual ICollection<MasterPart> MasterPart { get; set; }
        public virtual ICollection<Model> Model { get; set; }
    }
}
