using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class SecAuthGroupUserInfo
    {
        
        public Int32 AccessId { get; set; }

        
        public Int32 AuthGroupId { get; set; }

        
        public Int32 UserId { get; set; }

        
        public string ColumnValue { get; set; }
    }
}
