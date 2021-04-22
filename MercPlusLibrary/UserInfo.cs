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
    public class UserInfo
    {
        [DataMember]
        public Int32 UserId { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string AuthGroupName { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public string Loccd { get; set; }

        [DataMember]
        public string ActiveStatus { get; set; }

        [DataMember]
        public decimal? ApproveAmount { get; set; }

        [DataMember]
        public string LoginFirstAndLastName { get; set; }

        [DataMember]
        public string ChangeUser { get; set; }
        
        [DataMember]
        public System.DateTime ChangeTime { get; set; }

        [DataMember]
        public DateTime? LastLogInDateTime { get; set; }

        [DataMember]
        public bool? IsUserActive { get; set; }

        [DataMember]
        public string EmailId { get; set; }


    }
}
