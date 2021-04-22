using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class SecAuthGroup
    {
        
        public Int32 AuthGroupId { get; set; }
        
        public string AuthGroupName { get; set; }
        
        public Int32 ParentAuthGroupId { get; set; }
        
        public string TableName { get; set; }
        
        public string ColumnName { get; set; }
        
        public string ColumnDesc { get; set; }
        
        public string ReadOnlySw { get; set; }
    }
}
