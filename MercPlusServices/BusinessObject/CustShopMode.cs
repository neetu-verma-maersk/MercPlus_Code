using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{

    public class CustShopMode
    {
        
        public string CsmCd { get; set; }
        
        public string ModeCd { get; set; }
        
        public string RRISFormat { get; set; }
        
        public string ShopCd { get; set; }
        
        public string ProfitCenter { get; set; }
        
        public string PayAgentCd { get; set; }
        
        public string SubProfitCenter { get; set; }
        
        public string AccountCd { get; set; }
        
        public string CustomerCd { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }
        
        public string CorpPayAgentCd { get; set; }

        public virtual Customer Custome { get; set; }
        public virtual PayAgent PayAgent { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual Mode Mode { get; set; }
    }
}
