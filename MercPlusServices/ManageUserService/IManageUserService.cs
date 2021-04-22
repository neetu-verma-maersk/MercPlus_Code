using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MercPlusLibrary;

namespace ManageUserService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IManageUser
    {
        #region User Management
        [OperationContract]
        bool isUserExistInDb(string Login);

        [OperationContract]
        bool AddUser(UserInfo UserInfoFromClient, out string Msg);

        [OperationContract]
        List<Country> GetCountryList();

        [OperationContract]
        bool UpdateUser(UserInfo UserInfoFromClient, out string Msg);

        [OperationContract]
        bool UpdateUserActiveStatus(int day, out string Msg);

        [OperationContract]
        bool DeleteUser(int UserToBeDeleted, out string Msg);

        [OperationContract]
        bool DeleteUserDataAccessByUserId(int UserToBeDeleted, out string Msg);

        [OperationContract]
        List<UserInfo> GetUserListOfACountry(string CountryId);

        [OperationContract]
        List<UserInfo> GetUserByUserId(Int32 UserId);

        [OperationContract]
        List<UserInfo> GetUserByEmailId(string EmailId);

        [OperationContract]
        List<UserInfo> GetUserByLoginId(string LoginId);

        [OperationContract]
        bool IsLocationCodeExist(string LocCode);

        [OperationContract]
        List<AssignAuthGroup> AvailablePermissions(Int32 UserId, Int32 AuthGroupId);

        //[OperationContract]
        //List<SecWebSite> GetAuthGroupWebsiteAccessByAuthGroupId(int AuthorisationGroupId);


        [OperationContract]
        List<AvailableAssignAuthGroup> AvailablePermissionsByFilter(Int32 UserId, Int32 AuthGroupId, string SD, out int AvailablePermissionCount);
        #endregion

        #region AuthGroup
        [OperationContract]
        List<SecAuthGroup> GetAuthGroupList();
        [OperationContract]
        List<SecAuthGroup> GetAuthGroupByAuthgroupId(Int32 AuthGroupId);
        [OperationContract]
        List<SecAuthGroupUserInfo> GetAuthGroupByUserID(Int32 UserId);
        [OperationContract]
        bool InsertUserDataAccess(string SelectedActivePermission, int UserId, int AuthorisationGroupId, out string Msg);
        #endregion

        [OperationContract]
        List<SecWebSite> GetWebPageList();

        [OperationContract]
        List<SecWebSite> GetWebpageListByWebsiteId(int WebSiteId);

        [OperationContract]
        List<SecWebPage> GetAuthGroupWebpageAccessById(int WebSiteId, int AuthId);

        //[OperationContract]
        //bool UpdateWebSitePermissions(string SelectedWebSitePermissions, int AuthorisationGroupId, out string Msg);

        [OperationContract]
        bool UpdateWebPagePermissions(string SelectedWebSitePermissions, int AuthorisationGroupId, int WebpageId, out string Msg);

        [OperationContract]
        bool AddAllCluster(int UserId, int AuthorisationGroupId, out string Msg);
    }
}
