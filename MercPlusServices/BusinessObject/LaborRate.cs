using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class LaborRate
    {
        
        public int LaborRateID { get; set; }
        
        public string EqpType { get; set; }
        
        public System.DateTime EffDte { get; set; }
        
        public System.DateTime ExpDte { get; set; }
        
        public string ShopCd { get; set; }
        
        public string CustomerCd { get; set; }
        
        public Nullable<decimal> RegularRT { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }
        
        public Nullable<decimal> OvertimeRT { get; set; }
        
        public Nullable<decimal> DoubleTimeRT { get; set; }
        
        public Nullable<decimal> MiscRT { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual EqType EqType { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
