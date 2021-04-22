using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MercPlusLibrary
{
    [DataContract]
    public class SecWebPage
    {
        [DataMember]
        public int WebSiteId { get; set; }
        [DataMember]
        public int WebPageId { get; set; }
        [DataMember]
        public string WebPageName { get; set; }
    }
}
