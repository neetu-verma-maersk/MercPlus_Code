using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ManageUserService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IManageUser
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        bool isUserExistInDb(Int32 userId);

        [OperationContract]
        bool AddUser(UserInfo UserInfoFromClient, out string Msg);


        [OperationContract]
        List<Country> GetCountryList();

        [OperationContract]
        bool UpdateUser(UserInfo UserInfoFromClient);

        [OperationContract]
        List<WorkOrderReport> GetDataListForWorkOrder();

        [OperationContract]
        bool DeleteUser(int UserToBeDeleted);
    }

    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public Int32 UserId { get; set; }

        [DataMember]
        public string Login { get; set; }

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
        public decimal ApproveAmount { get; set; }
    }

    [DataContract]
    public class Location
    {
        [DataMember]
        public string locCode { get; set; }

        [DataMember]
        public string locDesc { get; set; }

    }

    [DataContract]
    public class Country
    {
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string CountryDescription { get; set; }



    }
    [DataContract]
    public class WorkOrderReport
    {
        [DataMember]
        public string Mode { get; set; }
        [DataMember]
        public string RepairCode { get; set; }
        [DataMember]
        public string RepairDesc { get; set; }
        [DataMember]
        public decimal? ContractAmount { get; set; }
        [DataMember]
        public DateTime EffDate { get; set; }
        [DataMember]
        public DateTime ExpDate { get; set; }

    } 
}
