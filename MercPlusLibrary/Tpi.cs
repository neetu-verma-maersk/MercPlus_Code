using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class Tpi
    {
        private string _ccode = "O";
        [DataMember]
        public string CedexCode
        {
            get { return _ccode; }
            set { _ccode = value; }
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string NumericalCode { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public string newTPI { get; set; }
    }
}
