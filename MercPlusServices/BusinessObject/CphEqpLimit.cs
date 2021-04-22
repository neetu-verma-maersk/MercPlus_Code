using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{

    public class CphEqpLimit
    {
        
        public string ModeCd { get; set; }
        
        public short AgeFrom{ get; set; }
        
        public Nullable<decimal> LimitAmount { get; set; }
        
        public string EqSize { get; set; }
        
        public short AgeTo { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime ChTime { get; set; }

        public virtual Mode Mode { get; set; }
    }
}
