using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary
{
    
    public partial class EventLog
    {
        
        public int EventID { get; set; }
        
        public string Eventname { get; set; }
        
        public string UniqueID { get; set; }
        
        public string TableName { get; set; }
        
        public string EventDesc { get; set; }
        
        public string ChUser { get; set; }
        
        public Nullable<System.DateTime> Chts { get; set; }
    }
}
