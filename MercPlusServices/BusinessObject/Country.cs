using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class Country
    {
        
        public string CountryCode { get; set; }
        
        public string CountryDescription { get; set; }
        
        public string AreaCode { get; set; }
        
        public Nullable<double> RepairLimitAdjFactor { get; set; }
        
        public string CountryGeoID { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }
    }
}
