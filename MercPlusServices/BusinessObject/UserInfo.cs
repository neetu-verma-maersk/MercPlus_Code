using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class UserInfo
    {
        
        public Int32 UserId { get; set; }

        
        public string Login { get; set; }

        
        public string FirstName { get; set; }

        
        public string LastName { get; set; }

        
        public string Company { get; set; }

        
        public string Loccd { get; set; }

        
        public string ActiveStatus { get; set; }

        
        public decimal? ApproveAmount { get; set; }

        
        public string LoginFirstAndLastName { get; set; }
    }
}
