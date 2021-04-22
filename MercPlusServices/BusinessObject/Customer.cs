using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{

    public class Customer
    {
        
        public string CustomerCd { get; set; }
        
        public string CustomerDesc { get; set; }
        
        public string ManualCd { get; set; }
        
        public string MaerskSw { get; set; }
        
        public string CustomerActiveSw { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual ICollection<CustShopMode> CustShopMode { get; set; }
        public virtual Manual Manual { get; set; }
        public virtual ICollection<LaborRate> LaborRate { get; set; }
        //public virtual ICollection<Transmit> Transmit { get; set; }
        public virtual ICollection<WorkOrder> WorkOrder { get; set; }
    }
}
