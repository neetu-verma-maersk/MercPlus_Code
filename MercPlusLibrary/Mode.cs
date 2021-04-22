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
    public class Mode
    {
        [DataMember]
        public string ModeCode { get; set; }
        [DataMember]
        public string ModeDescription { get; set; }
        [DataMember]
        public System.DateTime ChTime { get; set; }
        [DataMember]
        public string StandardTimeSW { get; set; }
        [DataMember]
        public string ValidationSW { get; set; }
        [DataMember]
        public string ModeActiveSW { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public string ModeInd { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public string ModeNotFound { get; set; }

        [DataMember]
        public List<CphEqpLimit> CphEqpLimit { get; set; }
        [DataMember]
        public List<CustShopMode> CustShopMode { get; set; }
        [DataMember]
        public List<EqMode> EqMode { get; set; }
        [DataMember]
        public List<ManualMode> ManualMode { get; set; }
        [DataMember]
        public List<NonsCode> NonsCode { get; set; }
        [DataMember]
        public List<PrepTime> PrepTime { get; set; }
        [DataMember]
        public List<RepairCode> RepairCode { get; set; }

        [DataMember]
        public string ModeFullDescription { get; set; }
        //public virtual ICollection<Transmit> Transmit { get; set; }
        //public virtual ICollection<WorkOrder> WorkOrder { get; set; }
    }
}
