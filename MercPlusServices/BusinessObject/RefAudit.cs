using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class RefAudit
    {
        
        public int AuditID { get; set; }
        
        public string TabName { get; set; }
        
        public string UniqueID { get; set; }
        
        public string ColName { get; set; }
        
        public string OldValue { get; set; }
        
        public string NewValue { get; set; }
        
        public string ChUser { get; set; }
        
        public Nullable<System.DateTime> Chts { get; set; }
    }
}
