using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class PayAgent
    {
        
        public string PayAgentCd { get; set; }
        
        public string CorpPayAgentCd { get; set; }
        
        public string RRISFormat { get; set; }
        
        public string ProfitCenter { get; set; }
        
        public string SubProfitCenter { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual ICollection<CustShopMode> CustShopMode { get; set; }
        public virtual ICollection<PayAgentVendor> PayAgentVendor { get; set; }
    }
}
