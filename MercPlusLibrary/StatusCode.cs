using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class StatusCode
    {
        [DataMember]
        public short StatusCod { get; set; }
        [DataMember]
        public string StatusDesc { get; set; }

        //[DataMember]
        //public virtual List<> MESC1TS_WO { get; set; }
    }
}
