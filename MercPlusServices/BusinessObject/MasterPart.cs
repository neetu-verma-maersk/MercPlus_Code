using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class MasterPart
    {
        
        public string PartsGroupCd { get; set; }
        
        public string PartCd { get; set; }
        
        public string PartDesc { get; set; }
        
        public Nullable<decimal> PartPrice { get; set; }
        
        public string ChUser { get; set; }
        
        public Nullable<int> Quantity { get; set; }
        
        public string PartDesignation1 { get; set; }
        
        public System.DateTime Chts { get; set; }
        
        public string PartDesignation2 { get; set; }
        
        public string PartDesignation3 { get; set; }
        
        public string PartActiveSW { get; set; }
        
        public Nullable<decimal> CoreValue { get; set; }
        
        public string DeductCore { get; set; }
        
        public string Remarks { get; set; }
        
        public string Manufactur { get; set; }
        
        public string CorePartSW { get; set; }
        
        public string MslPartSW { get; set; }

        public virtual Manufactur MANUFACTUR { get; set; }
        public virtual PartsGroup PartsGroup { get; set; }
        //public virtual ICollection<RprCodePart> RprCodePart { get; set; }
        //public virtual ICollection<WOPart> WOPart { get; set; }
    }
}
