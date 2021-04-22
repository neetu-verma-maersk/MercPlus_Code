using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MercPlusLibrary
{
    [DataContract]
    public class Discount
    {
        [DataMember]
        public string ShopCode { get; set; }
        [DataMember]
        public string Manufctr { get; set; }
        [DataMember]
        public Nullable<double> MarkDiscount { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
    }
}
