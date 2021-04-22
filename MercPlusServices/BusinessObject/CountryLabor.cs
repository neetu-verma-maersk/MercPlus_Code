using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class CountryLabor
    {
        
        public int CountryLaborID { get; set; }
        
        public string EqType { get; set; }
        
        public System.DateTime ExpDate { get; set; }
        
        public System.DateTime EffDate { get; set; }
        
        public string CountryCode { get; set; }
        
        public Nullable<decimal> RegularRT { get; set; }
        
        public Nullable<decimal> OvertimeRT { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }
        
        public Nullable<decimal> DoubleTimeRT { get; set; }
        
        public Nullable<decimal> MiscRT { get; set; }
        
        public string Cucdn { get; set; }

        public virtual Country Country { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual EqType EqpType { get; set; }
    }
}
