using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class ErrMessage
    {
        
        public int ErrorNo { get; set; }
        
        public string ErrorMsg { get; set; }
    }
}
