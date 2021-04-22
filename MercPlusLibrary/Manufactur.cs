using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class Manufactur
    {
        [DataMember]
        public string ManufacturCd { get; set; }
        [DataMember]
        public Nullable<double> DiscountPercent { get; set; }
        [DataMember]
        public string ManufacturName { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }

        [DataMember]
        public List<MasterPart> MasterPart { get; set; }
        [DataMember]
        public List<Model> Model { get; set; }
    }
}
