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
    public class MasterPart
    {
        [DataMember]
        public string PartsGroupCd { get; set; }
        [DataMember]
        public string PartCd { get; set; }
        [DataMember]
        public string PartDesc { get; set; }
        [DataMember]
        public Nullable<decimal> PartPrice { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public Nullable<int> Quantity { get; set; }
        [DataMember]
        public string PartDesignation1 { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public string PartDesignation2 { get; set; }
        [DataMember]
        public string PartDesignation3 { get; set; }
        [DataMember]
        public string PartActiveSW { get; set; }
        [DataMember]
        public Nullable<decimal> CoreValue { get; set; }
        [DataMember]
        public string DeductCore { get; set; }
        [DataMember]
        public string Remarks { get; set; }
        [DataMember]
        public string Manufactur { get; set; }
        [DataMember]
        public string CorePartSW { get; set; }
        [DataMember]
        public string MslPartSW { get; set; }

   /*     [DataMember]
        public Manufactur MANUFACTUR { get; set; } @Soumik */ 
        [DataMember]
        public PartsGroup PartsGroup { get; set; }
        //public virtual ICollection<RprCodePart> RprCodePart { get; set; }
        //public virtual ICollection<WOPart> WOPart { get; set; }


        [DataMember]
        public bool IsMasterPartExist { get; set; }

        [DataMember]
        public bool IsMasterPartAddSuccess { get; set; }

        [DataMember]
        public bool IsMasterPartEditSuccess { get; set; }

        [DataMember]
        public bool IsMasterPartDeleteSuccess { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string SerialTagSW { get; set; }
        [DataMember]
        public string CoreValueSW { get; set; }

        [DataMember]
        public string DeductCoreSW { get; set; }



    }
}
