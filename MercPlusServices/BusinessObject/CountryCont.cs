using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class CountryCont
    {
        
        public int CountryContID { get; set; }
        
        public string CountryCode { get; set; }
        
        public System.DateTime EffectiveDate { get; set; }
        
        public string Mode { get; set; }
        
        public System.DateTime ExpiryDate { get; set; }
        
        public string ChUser { get; set; }
        
        public decimal ContractAmount { get; set; }
        
        public System.DateTime Chts { get; set; }
        
        public string RepairCOde { get; set; }
        
        public string CUCDN { get; set; }
        
        public string ManualCode { get; set; }

        public virtual Country Country { get; set; }
        public virtual Currency Currency { get; set; }
    }
}
