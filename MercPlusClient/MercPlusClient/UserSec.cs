using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MercPlusClient
{
    public class UserSec 
    {      
        public int UserId;
        public string LoginId;
        public string UserType;
        public string UserFirstName;
        public string UserLastName;
        public string AuthGroupName;
        public decimal? ApprovalAmount;

        public DateTime? LastLogInDateTime;
        public bool? IsUserActive;
        public string EmailId;

        public string AuthGroupID;

        public bool	isAdmin = false;
        public bool	isCPH = false;
        public bool isEMRSpecialistCountry = false;
        public bool isEMRSpecialistShop = false;       
        public bool isEMRApproverCountry = false;
        public bool isEMRApproverShop = false;
        public bool	isShop = false;
        public bool	isMPROCluster = false;
        public bool isMPROShop = false;
        public bool	isReadOnly = false;
        
        public bool	isAnyCPH = false;
        public bool isAnyShop = false;

        public bool isRegion = false;
        public bool isThirdPartyInput = false;

        public string SessionId = "";
        public int GetUserTable = 0;
        public string HomePage;
        public bool DuplicateEmail = false; //Pinaki
        public Dictionary<string, string> WorkOrderStatusDesc = new Dictionary<string,string>(); 
        
        public UserSec() {}        
    }
    
}