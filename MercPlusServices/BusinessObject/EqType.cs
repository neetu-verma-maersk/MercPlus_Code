using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class EqType
    {
        
        public string EqpType { get; set; }
        
        public string EqTypeDesc { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual ICollection<CountryLabor> CountryLabor { get; set; }
        public virtual ICollection<LaborRate> LaborRate { get; set; }
        public virtual ICollection<WorkOrder> WorkOrder { get; set; }
    }
}
