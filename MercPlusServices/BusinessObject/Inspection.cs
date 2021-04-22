using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class Inspection
    {
        
        public System.DateTime InspDte { get; set; }
        
        public string ChasEqpNo { get; set; }
        
        public string InspBy { get; set; }
        
        public string RKEMLoc { get; set; }
        
        public string XmitRc { get; set; }
        
        public Nullable<System.DateTime> XmitDte { get; set; }
        
        public string ChUser { get; set; }
        
        public Nullable<System.DateTime> Chts { get; set; }
    }
}
