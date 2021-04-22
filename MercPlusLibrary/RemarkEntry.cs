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
    public class RemarkEntry
    {
        [DataMember]
        public int RemarkID { get; set; }
        [DataMember]
        public int WorkOrderID { get; set; }
        [DataMember]
        public string RemarkType { get; set; }
        [DataMember]
        public int? SuspendCatID { get; set; }
        [DataMember]
        public string CRTSDate { get; set; }
        [DataMember]
        public DateTime? XMITDate { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public string Remark { get; set; }
        [DataMember]
        public List<string> RemarksList { get; set; }
        [DataMember]
        public bool FatalError { get; set; }
        [DataMember]
        public int rState { get; set; }

        public RemarkEntry()
        {
            Empty();
        }

        public RemarkEntry(string sCRTS, string sType, int? ID, string sRemark)
        {
            this.RemarkType = sType;
            this.SuspendCatID = ID;
            this.Remark = sRemark;
            this.CRTSDate = sCRTS;
        }

        public RemarkEntry(string sCRTS, string sType, int? ID, string sRemark, string sUser)
        {
            this.RemarkType = sType;
            this.SuspendCatID = ID;
            this.Remark = sRemark;
            this.CRTSDate = sCRTS;
            this.ChangeUser = sUser;
        }
        public void Empty()
        {
            //this.RemarkID = "";
            //this.WOID = "";
            this.RemarkType = "";
            //this.SuspendCatID = "";
            this.Remark = "";
            this.CRTSDate = "";
            //this.XmitDte = "";
            this.ChangeUser = "";

            // validation work
            this.FatalError = false;
        }
    }
}
