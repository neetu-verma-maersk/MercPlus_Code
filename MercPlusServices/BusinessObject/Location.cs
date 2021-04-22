using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class Location
    {
        
        public string LocCd { get; set; }
        
        public string RegionCd { get; set; }
        
        public string LocDesc { get; set; }
        
        public string CountryCd { get; set; }
        
        public string RkrpLoc { get; set; }
        
        public string LocGeoID { get; set; }
        
        public string ChUser { get; set; }
        
        public string ContactEqsalSW { get; set; }
        
        public System.DateTime Chts { get; set; }

        public virtual Country Country { get; set; }
        //public virtual Region Region { get; set; }
        public virtual ICollection<Shop> Shop { get; set; }
        //public virtual ICollection<SecUser> SEC_USER { get; set; }
    }
}
