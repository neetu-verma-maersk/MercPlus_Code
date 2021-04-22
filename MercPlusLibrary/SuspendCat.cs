using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    public class SuspendCat
    {
        [DataMember]
        public int SuspcatID { get; set; }
        [DataMember]
        public string SuspcatDesc { get; set; }
        [DataMember]
        public string ChangeUserSus { get; set; }
        [DataMember]
        public System.DateTime ChangeTimeSus { get; set; }


        [DataMember]
        public virtual List<Suspend> Suspend { get; set; }
        //[DataMember]
        //public virtual List<MESC1TS_WOREMARK> MESC1TS_WOREMARK { get; set; }
    }
}
