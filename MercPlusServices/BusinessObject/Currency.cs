using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{

    public class Currency
    {
        
        public string Curcd { get; set; }
        
        public string Cucdn { get; set; }
        
        public string CurrNamc { get; set; }
        
        public Nullable<decimal> ExtraTdkk { get; set; }
        
        public Nullable<decimal> ExtratUsd { get; set; }
        
        public Nullable<decimal> ExtraTyen{ get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }
        
        public Nullable<decimal> ExtraTeur{ get; set; }
        
        public Nullable<System.DateTime> QuoteDat { get; set; }

        public virtual ICollection<CountryCont> CountryCont { get; set; }
        public virtual ICollection<CountryLabor> CountryLabor { get; set; }
        public virtual ICollection<Shop> SHop { get; set; }
        public virtual ICollection<WorkOrder> WorkOrder { get; set; }
    }
}
