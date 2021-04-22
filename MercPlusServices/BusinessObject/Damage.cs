using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{

    public class Damage
    {
        
        public string DamageCedexCode { get; set; }
        
        public string DamageName { get; set; }
        
        public string DamageDescription { get; set; }
        
        public string DamageNumericalCode { get; set; }
        
        public string DamageFullDescription { get; set; }
        
        public string ChUser { get; set; }
        
        public System.DateTime Chts { get; set; }

    }
}
