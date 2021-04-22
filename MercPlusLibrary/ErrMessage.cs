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
    public class ErrMessage
    {
        [DataMember]
        public int ErrorNo { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string ErrorType { get; set; }
    }
}
