using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class Suspend
    {
        [DataMember]
        public string ShopCode { get; set; }
        [DataMember]
        public string RepairCode { get; set; }
        [DataMember]
        public string Mode { get; set; }
        [DataMember]
        public string ManualCode { get; set; }
        [DataMember]
        public string SuspcatID { get; set; }
        [DataMember]
        public string ChangeUserSp { get; set; }
        [DataMember]
        public System.DateTime ChangeTimeSp { get; set; }
        [DataMember]
        public Shop MESC1TS_SHOP { get; set; }
        //public virtual MESC1TS_SUSPEND_CAT MESC1TS_SUSPEND_CAT { get; set; }
    }
}
