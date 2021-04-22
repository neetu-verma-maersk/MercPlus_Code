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
  public   class TPIIndicator
    {
         [DataMember]
         public string TPICedexCode { get; set; }
         [DataMember]
         public string TPIFullDescription { get; set; }
         [DataMember]
         public string TPIName { get; set; }
         [DataMember]
         public string TPIDescription { get; set; }
         [DataMember]
         public string TPINumericalCode { get; set; }
         [DataMember]
         public string TPICHUser { get; set; }
         [DataMember]
         public System.DateTime TPICHTS { get; set; }
         [DataMember]
         public string Category { get; set; }

    }
}
