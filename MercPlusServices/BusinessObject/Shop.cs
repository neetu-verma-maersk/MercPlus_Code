using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class Shop
    {
        
        public string ShopCd { get; set; }
        
        public string ShopDescription { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime ChTime { get; set; }

    }
}
