using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MercPlusClient.Areas.ManageWorkOrder.Models
{
    public class AuditTrailModel
    {
        public string WOID { get; set; }
        public string Description { get; set; }
        public string ChangeUser { get; set; }
        public DateTime Timestamp { get; set; }


        public int AuditID { get; set; }
        public string TabName { get; set; }
        public string UniqueID { get; set; }
        public string ColName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ChangeTime { get; set; }
    }
}