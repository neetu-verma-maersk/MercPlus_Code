using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MercPlusLibrary;
//using System.Data.SqlClient;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace ManageUserService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class ManageUser : IManageUser
    {
        //public SqlConnection Conn;
        int flag = 0;
        LogEntry logEntry = new LogEntry();
        ManageUserServiceEntities objUserServiceEntites;
        ManageUser()
        {
            objUserServiceEntites = new ManageUserServiceEntities();
        }
        #region User Management
        public bool isUserExistInDb(string Login)
        {
            try
            {
                List<SEC_USER> ObjUser = (from user in objUserServiceEntites.SEC_USER
                                          where user.LOGIN == Login
                                          select user).ToList();

                if (ObjUser.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return false;
            }

        }
        public List<UserInfo> GetUserByUserId(Int32 UserId)
        {
            List<UserInfo> UserList = new List<UserInfo>();
            try
            {
                var objUserResult = from U in objUserServiceEntites.SEC_USER
                                    where U.USER_ID == UserId
                                    select new { U.USER_ID, U.LOGIN, U.FIRSTNAME, U.LASTNAME, U.COMPANY, U.ACTIVE_STATUS, U.APPROVAL_AMOUNT, U.LOC_CD, U.LastLoginDateTime, U.IsUserActive, U.EmailId };


                foreach (var col in objUserResult)
                {
                    UserInfo objUser = new UserInfo();
                    objUser.UserId = col.USER_ID;
                    objUser.Login = col.LOGIN;
                    objUser.FirstName = col.FIRSTNAME;
                    objUser.LastName = col.LASTNAME;
                    objUser.Company = col.COMPANY;
                    objUser.ActiveStatus = col.ACTIVE_STATUS;
                    objUser.ApproveAmount = col.APPROVAL_AMOUNT;
                    objUser.Loccd = col.LOC_CD;

                    objUser.LastLogInDateTime = col.LastLoginDateTime;
                    objUser.IsUserActive = col.IsUserActive;
                    objUser.EmailId = col.EmailId;

                    UserList.Add(objUser);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return UserList;
        }

        public List<UserInfo> GetUserByEmailId(string EmailId)
        {
            List<UserInfo> UserList = new List<UserInfo>();
            try
            {
                var objUserResult = from U in objUserServiceEntites.SEC_USER
                                    where !string.IsNullOrEmpty(U.EmailId) && U.EmailId.ToLower() == EmailId.ToLower()
                                    select new { U.USER_ID, U.LOGIN, U.FIRSTNAME, U.LASTNAME, U.COMPANY, U.ACTIVE_STATUS,    U.APPROVAL_AMOUNT, U.LOC_CD, U.LastLoginDateTime, U.IsUserActive, U.EmailId };


                foreach (var col in objUserResult)
                {
                    UserInfo objUser = new UserInfo();
                    objUser.UserId = col.USER_ID;
                    objUser.Login = col.LOGIN;
                    objUser.FirstName = col.FIRSTNAME;
                    objUser.LastName = col.LASTNAME;
                    objUser.Company = col.COMPANY;
                    objUser.ActiveStatus = col.ACTIVE_STATUS;
                    objUser.ApproveAmount = col.APPROVAL_AMOUNT;
                    objUser.Loccd = col.LOC_CD;

                    objUser.LastLogInDateTime = col.LastLoginDateTime;
                    objUser.IsUserActive = col.IsUserActive;
                    objUser.EmailId = col.EmailId;

                    UserList.Add(objUser);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return UserList;
        }
        public List<UserInfo> GetUserByLoginId(string LoginId)
        {
            List<UserInfo> UserList = new List<UserInfo>();
            try
            {
                var objUserResult = from U in objUserServiceEntites.SEC_USER
                                    where U.LOGIN == LoginId
                                    select new { U.USER_ID, U.LOGIN, U.FIRSTNAME, U.LASTNAME, U.COMPANY, U.ACTIVE_STATUS, U.APPROVAL_AMOUNT, U.LOC_CD, U.LastLoginDateTime, U.IsUserActive, U.EmailId };
                foreach (var col in objUserResult)
                {
                    UserInfo objUser = new UserInfo();
                    objUser.UserId = col.USER_ID;
                    objUser.Login = col.LOGIN;
                    objUser.FirstName = col.FIRSTNAME;
                    objUser.LastName = col.LASTNAME;
                    objUser.Company = col.COMPANY;
                    objUser.ActiveStatus = col.ACTIVE_STATUS;
                    objUser.ApproveAmount = col.APPROVAL_AMOUNT;
                    objUser.Loccd = col.LOC_CD;

                    objUser.LastLogInDateTime = col.LastLoginDateTime;
                    objUser.IsUserActive = col.IsUserActive;
                    objUser.EmailId = col.EmailId;

                    UserList.Add(objUser);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return UserList;
        }
        public List<Country> GetCountryList()
        {
            List<Country> CountryList = new List<Country>();
            try
            {
                var objCountryResult = from country in objUserServiceEntites.MESC1TS_COUNTRY
                                       orderby country.COUNTRY_CD
                                       select country;

                foreach (var col in objCountryResult)
                {
                    Country objCountry = new Country();
                    objCountry.CountryCode = col.COUNTRY_CD;
                    objCountry.CountryDescription = col.COUNTRY_DESC;
                    objCountry.CountryCodeAndDescription = col.COUNTRY_CD + " - " + col.COUNTRY_DESC;
                    CountryList.Add(objCountry);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return CountryList;
        }
        public List<UserInfo> GetUserListOfACountry(string CountryId)
        {
            List<UserInfo> UserList = new List<UserInfo>();
            try
            {
                //var objUserResult = from U in objUserServiceEntites.SEC_USER
                //                    from L in objUserServiceEntites.MESC1TS_LOCATION
                //                    from AU in objUserServiceEntites.SEC_AUTHGROUP_USER
                //                    from A in objUserServiceEntites.SEC_AUTHGROUP
                //                    where L.COUNTRY_CD == CountryId && U.LOC_CD == L.LOC_CD
                //                    && (U.USER_ID == AU.USER_ID && AU.AUTHGROUP_ID == A.AUTHGROUP_ID)
                //                    orderby U.LOGIN
                //                    select new { U.USER_ID, U.LOGIN, U.FIRSTNAME, U.LASTNAME, A.AUTHGROUP_NAME };


                //var objUserResult = (from U in objUserServiceEntites.SEC_USER
                                     //join L in objUserServiceEntites.MESC1TS_LOCATION on U.LOC_CD equals L.LOC_CD
                                     //join AU in objUserServiceEntites.SEC_AUTHGROUP_USER on U.USER_ID equals AU.USER_ID
                                     //join A in objUserServiceEntites.SEC_AUTHGROUP on AU.AUTHGROUP_ID equals A.AUTHGROUP_ID
                                     //where L.COUNTRY_CD == CountryId
                                     ////orderby U.LOGIN
                                     //select new { U.USER_ID, U.LOGIN, U.FIRSTNAME, U.LASTNAME, A.AUTHGROUP_NAME }
                                     ////   ).Distinct().ToList();
                                     //).Distinct().ToList().OrderBy(x => x.LASTNAME).ThenBy(y => y.FIRSTNAME);

                var objUserResult = (from U in objUserServiceEntites.SEC_USER
                                     from AU in objUserServiceEntites.SEC_AUTHGROUP_USER
                                     .Where(AU => AU.USER_ID == U.USER_ID)
                                     .DefaultIfEmpty()
                                     from A in objUserServiceEntites.SEC_AUTHGROUP
                                     .Where(A => A.AUTHGROUP_ID == AU.AUTHGROUP_ID)
                                     .DefaultIfEmpty()
                                     join L in objUserServiceEntites.MESC1TS_LOCATION on U.LOC_CD equals L.LOC_CD
                                     where L.COUNTRY_CD == CountryId
                                     select new { U.USER_ID, U.LOGIN, U.FIRSTNAME, U.LASTNAME, A.AUTHGROUP_NAME,                U.LastLoginDateTime, U.IsUserActive, U.EmailId }
                                     ).Distinct().ToList().OrderBy(x => x.LASTNAME).ThenBy(y => y.FIRSTNAME).ThenBy(z => z.LOGIN);
                
                foreach (var col in objUserResult)
                {
                    UserInfo objUser = new UserInfo();
                    objUser.UserId = col.USER_ID;
                    objUser.Login = col.LOGIN;
                    objUser.AuthGroupName = col.AUTHGROUP_NAME;
                    objUser.LoginFirstAndLastName = col.LASTNAME + ", " + col.FIRSTNAME + " -- " + col.LOGIN;

                    objUser.LastLogInDateTime = col.LastLoginDateTime;
                    objUser.IsUserActive = col.IsUserActive;
                    objUser.EmailId = col.EmailId;

                    UserList.Add(objUser);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return UserList;
        }
        public bool IsLocationCodeExist(string LocCode)
        {
            try
            {
                var objLocationResult = (from location in objUserServiceEntites.MESC1TS_LOCATION
                                         where location.LOC_CD == LocCode
                                         select location).ToList();
                if (objLocationResult.Count() >= 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return false;
        }
        public bool AddUser(UserInfo UserInfoFromClient, out string Msg)
        {

            bool IsSuccess = false;
            try
            {
                IsSuccess = isUserExistInDb(UserInfoFromClient.Login);
                if (IsSuccess == true)
                {
                    Msg = "User " + UserInfoFromClient.Login + " already exists. Please choose a different Login ID.";
                    return false;
                }
                IsSuccess = false;
                SEC_USER userInfoToBeInserted = new SEC_USER();
                var objLocationResult = (from location in objUserServiceEntites.MESC1TS_LOCATION
                                         where location.LOC_CD == UserInfoFromClient.Loccd
                                         select location).ToList();

                if (objLocationResult.Count() <= 0)
                {
                    IsSuccess = false;
                    Msg = "Location " + UserInfoFromClient.Loccd + " Is invalid. Please enter correct location";
                    return IsSuccess;
                }
                userInfoToBeInserted.LOGIN = UserInfoFromClient.Login;
                userInfoToBeInserted.FIRSTNAME = UserInfoFromClient.FirstName;
                userInfoToBeInserted.LASTNAME = UserInfoFromClient.LastName;
                userInfoToBeInserted.COMPANY = UserInfoFromClient.Company;
                userInfoToBeInserted.LOC_CD = UserInfoFromClient.Loccd;
                userInfoToBeInserted.ACTIVE_STATUS = UserInfoFromClient.ActiveStatus;
                userInfoToBeInserted.APPROVAL_AMOUNT = UserInfoFromClient.ApproveAmount;

                userInfoToBeInserted.IsUserActive = true;
                userInfoToBeInserted.EmailId = UserInfoFromClient.EmailId;                

                objUserServiceEntites.SEC_USER.Add(userInfoToBeInserted);
                objUserServiceEntites.SaveChanges();
                IsSuccess = true;
                Msg = "User " + UserInfoFromClient.Login + " created successfull.";
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Msg = "Some Error has occurred while performing your activity. Please contact System Administrator ";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return IsSuccess;
        }
        public bool UpdateUser(UserInfo UserInfoFromClient, out string Msg)
        {

            bool IsSuccess = false;
            try
            {
                var objLocationResult = (from location in objUserServiceEntites.MESC1TS_LOCATION
                                         where location.LOC_CD == UserInfoFromClient.Loccd
                                         select location).ToList();

                if (objLocationResult.Count() <= 0)
                {
                    IsSuccess = false;
                    Msg = "Location " + UserInfoFromClient.Loccd + " Is invalid. Please enter correct location";
                    return IsSuccess;
                }
                List<UserInfo> oldData = GetUserByUserId(UserInfoFromClient.UserId);
                List<SEC_USER> objUserInfo = (from user in objUserServiceEntites.SEC_USER
                                              where user.USER_ID == UserInfoFromClient.UserId
                                              select user).ToList();


                objUserInfo[0].LOGIN = UserInfoFromClient.Login;
                objUserInfo[0].FIRSTNAME = UserInfoFromClient.FirstName;
                objUserInfo[0].LASTNAME = UserInfoFromClient.LastName;
                objUserInfo[0].COMPANY = UserInfoFromClient.Company;
                objUserInfo[0].LOC_CD = UserInfoFromClient.Loccd;
                objUserInfo[0].ACTIVE_STATUS = UserInfoFromClient.ActiveStatus;
                objUserInfo[0].APPROVAL_AMOUNT = UserInfoFromClient.ApproveAmount;

                objUserInfo[0].IsUserActive = UserInfoFromClient.IsUserActive;
                objUserInfo[0].EmailId = UserInfoFromClient.EmailId;
                objUserInfo[0].LastLoginDateTime = UserInfoFromClient.LastLogInDateTime;

                objUserServiceEntites.SaveChanges();
                Msg = "User " + UserInfoFromClient.Login + " Updated successfully.";
                IsSuccess = true;
                try
                {
                    InsertUserSecAuditTrail(UserInfoFromClient, oldData);
                }
                catch (Exception ex) { }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Msg = "Some Error has occurred while performing your activity. Please contact System Administrator ";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return IsSuccess;
        }

        public bool UpdateUserActiveStatus(int day, out string Msg)
        {
            bool isSuccess = false;
            StringBuilder updateMessage = new StringBuilder();
            DateTime referenceDate = DateTime.Now.AddDays(day * -1);
            try
            {
                List<SEC_USER> objUserInfo = (from user in objUserServiceEntites.SEC_USER
                                              where
                                              user.LastLoginDateTime != null &&
                                              user.LastLoginDateTime.Value < referenceDate &&
                                              user.IsUserActive != null &&
                                              user.IsUserActive == true
                                              select user).ToList();

                if (objUserInfo != null)
                {
                    List<UserInfo> newData = new List<UserInfo>();
                    List<UserInfo> oldData = new List<UserInfo>();
                    foreach (SEC_USER user in objUserInfo)
                    {
                        oldData.AddRange(GetUserByUserId(user.USER_ID));

                        user.IsUserActive = false;
                        UserInfo objUser = new UserInfo();
                        objUser.UserId = user.USER_ID;
                        objUser.Login = user.LOGIN;
                        objUser.FirstName = user.FIRSTNAME;
                        objUser.LastName = user.LASTNAME;
                        objUser.Company = user.COMPANY;
                        objUser.ActiveStatus = user.ACTIVE_STATUS;
                        objUser.ApproveAmount = user.APPROVAL_AMOUNT;
                        objUser.Loccd = user.LOC_CD;

                        objUser.LastLogInDateTime = user.LastLoginDateTime;
                        objUser.IsUserActive = user.IsUserActive;
                        objUser.EmailId = user.EmailId;
                        objUser.ChangeTime = DateTime.Now;
                        objUser.ChangeUser = "UserActiveCheckerService";
                        objUserServiceEntites.SaveChanges();

                        updateMessage.AppendLine(" User " + user.LOGIN + " Updated successfully.");
                        newData.Add(objUser);

                    }
                    try
                    {
                        foreach (var data in newData)
                        {
                            InsertUserSecAuditTrail(data, oldData.Where(s => s.UserId == data.UserId).Select(p => p).ToList());
                        }
                    }
                    catch (Exception ex) { }
                }

                Msg = updateMessage.ToString();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Msg = "Some Error has occurred while performing your activity. Please contact System Administrator ";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return isSuccess;
        }

        public bool DeleteUser(int UserToBeDeleted, out string Msg)
        {
            bool IsSuccess = false;
            try
            {
                string userLoginId = "";
                List<SEC_AUTHGROUP_USER> ObjSecAuthUser = (from user in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                           where user.USER_ID == UserToBeDeleted
                                                           select user).ToList();
                if (ObjSecAuthUser.Count() > 0)
                {
                    foreach (var col in ObjSecAuthUser.ToList())
                    {
                        objUserServiceEntites.SEC_AUTHGROUP_USER.Remove(col);
                    }
                    objUserServiceEntites.SaveChanges();
                }

                List<SEC_USER> ObjUser = (from user in objUserServiceEntites.SEC_USER
                                          where user.USER_ID == UserToBeDeleted
                                          select user).ToList();

                if (ObjUser.Count() > 0)
                {
                    userLoginId = ObjUser[0].LOGIN.ToString();
                }
                objUserServiceEntites.SEC_USER.Remove(ObjUser.First());
                objUserServiceEntites.SaveChanges();
                Msg = "User " + userLoginId + " deleted successfully.";
                IsSuccess = true;

            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Msg = "Some Error has occurred while performing your activity. Please contact System Administrator ";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return IsSuccess;

        }
        public bool DeleteUserDataAccessByUserId(int UserToBeDeleted, out string Msg)
        {
            bool IsSuccess = false;
            try
            {
                List<SEC_AUTHGROUP_USER> ObjUser = (from user in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                    where user.USER_ID == UserToBeDeleted
                                                    select user).ToList();
                foreach (var col in ObjUser.ToList())
                {
                    objUserServiceEntites.SEC_AUTHGROUP_USER.Remove(col);
                }
                objUserServiceEntites.SaveChanges();
                Msg = "User Authorisation group deleted successfully.";
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Msg = "Some Error has occurred while performing your activity. Please contact System Administrator ";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return IsSuccess;

        }

        private bool InsertUserSecAuditTrail(UserInfo NewRecord, List<UserInfo> OldRecord)
        {
            bool success = false;
            try
            {
                int UniqueKey = NewRecord.UserId;
                MESC1TS_REFAUDIT record;
                objUserServiceEntites = new ManageUserServiceEntities();
                if (OldRecord[0].Login.Trim() != NewRecord.Login.Trim())
                {
                    record = BuildDamageCodeAuditRecordSet("SEC_USER", "Login", OldRecord[0].Login, NewRecord.Login, NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey.ToString());
                    objUserServiceEntites.MESC1TS_REFAUDIT.Add(record);
                }
                if (OldRecord[0].FirstName.Trim() != NewRecord.FirstName.Trim())
                {
                    record = BuildDamageCodeAuditRecordSet("SEC_USER", "First Name", OldRecord[0].FirstName, NewRecord.FirstName, NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey.ToString());
                    objUserServiceEntites.MESC1TS_REFAUDIT.Add(record);
                }
                if (OldRecord[0].LastName.Trim() != NewRecord.LastName.Trim())
                {
                    record = BuildDamageCodeAuditRecordSet("SEC_USER", "Last Name", OldRecord[0].LastName, NewRecord.LastName, NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey.ToString());
                    objUserServiceEntites.MESC1TS_REFAUDIT.Add(record);
                }
                if (OldRecord[0].Company.Trim() != NewRecord.Company.Trim())
                {
                    record = BuildDamageCodeAuditRecordSet("SEC_USER", "Company", OldRecord[0].Company, NewRecord.Company, NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey.ToString());
                    objUserServiceEntites.MESC1TS_REFAUDIT.Add(record);
                }
                if (OldRecord[0].Loccd.Trim() != NewRecord.Loccd.Trim())
                {
                    record = BuildDamageCodeAuditRecordSet("SEC_USER", "Location Code", OldRecord[0].Loccd, NewRecord.Loccd, NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey.ToString());
                    objUserServiceEntites.MESC1TS_REFAUDIT.Add(record);
                }
                if (OldRecord[0].ApproveAmount != NewRecord.ApproveAmount)
                {
                    record = BuildDamageCodeAuditRecordSet("SEC_USER", "Approve Amount", OldRecord[0].ApproveAmount.ToString(), NewRecord.ApproveAmount.ToString(), NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey.ToString());
                    objUserServiceEntites.MESC1TS_REFAUDIT.Add(record);
                }
                if (OldRecord[0].ActiveStatus.Trim() != NewRecord.ActiveStatus.Trim())
                {
                    record = BuildDamageCodeAuditRecordSet("SEC_USER", "Active Status", OldRecord[0].ActiveStatus.ToString(), NewRecord.ActiveStatus.ToString(), NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey.ToString());
                    objUserServiceEntites.MESC1TS_REFAUDIT.Add(record);
                }
                if (OldRecord[0].EmailId == null || OldRecord[0].EmailId.Trim() != NewRecord.EmailId.Trim())
                {
                    record = BuildDamageCodeAuditRecordSet("SEC_USER", "Email Id", 
                             OldRecord[0].EmailId == null ? string.Empty : OldRecord[0].EmailId,
                             NewRecord.EmailId == null ? string.Empty : NewRecord.EmailId,
                             NewRecord.ChangeUser, 
                             NewRecord.ChangeTime, 
                             UniqueKey.ToString());
                    objUserServiceEntites.MESC1TS_REFAUDIT.Add(record);
                }
                if (OldRecord[0].IsUserActive == null || OldRecord[0].IsUserActive != NewRecord.IsUserActive)
                {
                    record = BuildDamageCodeAuditRecordSet("SEC_USER", "User Active Status", 
                             OldRecord[0].IsUserActive == null ? string.Empty : OldRecord[0].IsUserActive.ToString(), 
                             NewRecord.IsUserActive == null ? string.Empty : NewRecord.IsUserActive.ToString(),
                             NewRecord.ChangeUser, 
                             NewRecord.ChangeTime, 
                             UniqueKey.ToString());
                    objUserServiceEntites.MESC1TS_REFAUDIT.Add(record);
                }
                if (OldRecord[0].LastLogInDateTime == null || OldRecord[0].LastLogInDateTime != NewRecord.LastLogInDateTime)
                {
                    record = BuildDamageCodeAuditRecordSet("SEC_USER", "Last Login Date Time", 
                             OldRecord[0].LastLogInDateTime == null ? string.Empty : OldRecord[0].LastLogInDateTime.ToString(), 
                             NewRecord.LastLogInDateTime == null ? string.Empty : NewRecord.LastLogInDateTime.ToString(), 
                             NewRecord.ChangeUser, 
                             NewRecord.ChangeTime, 
                             UniqueKey.ToString());
                    objUserServiceEntites.MESC1TS_REFAUDIT.Add(record);
                }
                objUserServiceEntites.SaveChanges();
                success = true;

            }
            catch (Exception ex)
            {
                success = false;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }

        private MESC1TS_REFAUDIT BuildDamageCodeAuditRecordSet(string TableName, string ColName, string OldValue, string NewValue, string ChangeUser, DateTime ChangeTime, string ID)
        {
            MESC1TS_REFAUDIT record = new MESC1TS_REFAUDIT();
            record.UNIQUE_ID = ID;
            record.TAB_NAME = TableName;
            record.COL_NAME = ColName;
            record.OLD_VALUE = OldValue;
            record.NEW_VALUE = NewValue;
            record.CHUSER = ChangeUser;
            record.CHTS = ChangeTime;
            return record;
        }
        #endregion

        #region UserAuthGroup
        public List<SecAuthGroup> GetAuthGroupList()
        {
            List<SecAuthGroup> UserAuthList = new List<SecAuthGroup>();
            try
            {
                var objUserResult = from AG in objUserServiceEntites.SEC_AUTHGROUP
                                    orderby AG.AUTHGROUP_NAME
                                    select new { AG.AUTHGROUP_ID, AG.AUTHGROUP_NAME, AG.PARENT_AUTHGROUP_ID, AG.TABLE_NAME, AG.COLUMN_NAME, AG.COLUMN_DESC };
                foreach (var col in objUserResult)
                {
                    UserAuthList.Add(new SecAuthGroup()
                    {
                        AuthGroupName = col.AUTHGROUP_NAME,
                        AuthGroupId = col.AUTHGROUP_ID,
                        ParentAuthGroupId = col.PARENT_AUTHGROUP_ID,
                        TableName = col.TABLE_NAME,
                        ColumnName = col.COLUMN_NAME,
                        ColumnDesc = col.COLUMN_DESC,
                    });
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return UserAuthList;
        }
        public List<SecAuthGroup> GetAuthGroupByAuthgroupId(Int32 AuthGroupId)
        {
            List<SecAuthGroup> UserAuthList = new List<SecAuthGroup>();
            try
            {
                var objUserResult = from AG in objUserServiceEntites.SEC_AUTHGROUP
                                    where AG.AUTHGROUP_ID == AuthGroupId
                                    select new { AG.AUTHGROUP_NAME, AG.PARENT_AUTHGROUP_ID, AG.TABLE_NAME, AG.COLUMN_NAME, AG.COLUMN_DESC };
                foreach (var col in objUserResult)
                {
                    UserAuthList.Add(new SecAuthGroup()
                    {
                        AuthGroupName = col.AUTHGROUP_NAME,
                        ParentAuthGroupId = col.PARENT_AUTHGROUP_ID,
                        TableName = col.TABLE_NAME,
                        ColumnName = col.COLUMN_NAME,
                        ColumnDesc = col.COLUMN_DESC,
                    });
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return UserAuthList;
        }
        public List<SecAuthGroupUserInfo> GetAuthGroupByUserID(Int32 UserId)
        {
            List<SecAuthGroupUserInfo> UserList = new List<SecAuthGroupUserInfo>();
            try
            {
                var objUserResult = from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                    from AGU in objUserServiceEntites.SEC_AUTHGROUP
                                    where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AGU.AUTHGROUP_ID
                                    select new { AG.ACCESS_ID, AG.AUTHGROUP_ID, AG.USER_ID, AG.COLUMN_VALUE, AGU.AUTHGROUP_NAME, AGU.TABLE_NAME, AGU.COLUMN_DESC, AGU.COLUMN_NAME };
                foreach (var col in objUserResult)
                {
                    UserList.Add(new SecAuthGroupUserInfo()
                    {
                        AccessId = col.ACCESS_ID,
                        AuthGroupId = col.AUTHGROUP_ID,
                        UserId = col.USER_ID,
                        ColumnValue = col.COLUMN_VALUE,
                        AuthGroupName = col.AUTHGROUP_NAME,
                        TableName = col.TABLE_NAME,
                        ColumnDesc = col.COLUMN_DESC,
                        ColumnName = col.COLUMN_NAME,

                    });
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return UserList;
        }
        public List<AssignAuthGroup> AvailablePermissions(Int32 UserId, Int32 AuthGroupId)
        {
            List<AssignAuthGroup> UserList = new List<AssignAuthGroup>();
            try
            {
                var objSecUserResult = (from AG in objUserServiceEntites.SEC_AUTHGROUP
                                        where AG.AUTHGROUP_ID == AuthGroupId
                                        select new { AG.AUTHGROUP_ID, AG.AUTHGROUP_NAME, AG.PARENT_AUTHGROUP_ID, AG.TABLE_NAME, AG.COLUMN_NAME, AG.COLUMN_DESC }).ToList();

                if (objSecUserResult.Count() > 0)
                {

                    if (objSecUserResult[0].TABLE_NAME == "MESC1TS_AREA")
                    {

                        var objQueryResult = from AR in objUserServiceEntites.MESC1TS_AREA
                                             where (from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                    where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                    select AG.COLUMN_VALUE).Contains(AR.AREA_CD)
                                             select new { list = AR.AREA_DESC, value = AR.AREA_CD };

                        foreach (var col in objQueryResult)
                        {
                            UserList.Add(new AssignAuthGroup()
                            {
                                ValueItem = col.value,
                                ListItem = col.list,
                            });
                        }
                    }
                    if (objSecUserResult[0].TABLE_NAME == "MESC1TS_COUNTRY")
                    {

                        var objQueryResult = from AR in objUserServiceEntites.MESC1TS_COUNTRY
                                             where (from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                    where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                    select AG.COLUMN_VALUE).Contains(AR.COUNTRY_CD)
                                             select new { list = AR.COUNTRY_DESC, value = AR.COUNTRY_CD };

                        foreach (var col in objQueryResult)
                        {
                            UserList.Add(new AssignAuthGroup()
                            {
                                ValueItem = col.value,
                                ListItem = col.list,
                            });
                        }
                    }
                    if (objSecUserResult[0].TABLE_NAME == "MESC1TS_LOCATION")
                    {

                        var objQueryResult = from AR in objUserServiceEntites.MESC1TS_LOCATION
                                             where (from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                    where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                    select AG.COLUMN_VALUE).Contains(AR.LOC_CD)
                                             select new { list = AR.LOC_DESC, value = AR.LOC_CD };

                        foreach (var col in objQueryResult)
                        {
                            UserList.Add(new AssignAuthGroup()
                            {
                                ValueItem = col.value,
                                ListItem = col.list,
                            });
                        }
                    }
                    if (objSecUserResult[0].TABLE_NAME == "MESC1TS_VENDOR")
                    {

                        var objQueryResult = from AR in objUserServiceEntites.MESC1TS_VENDOR
                                             where (from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                    where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                    select AG.COLUMN_VALUE).Contains(AR.VENDOR_CD)
                                             select new { list = AR.VENDOR_DESC, value = AR.VENDOR_CD };

                        foreach (var col in objQueryResult)
                        {
                            UserList.Add(new AssignAuthGroup()
                            {
                                ValueItem = col.value,
                                ListItem = col.list,
                            });
                        }
                    }
                    if (objSecUserResult[0].TABLE_NAME == "MESC1TS_SHOP")
                    {

                        var objQueryResult = from AR in objUserServiceEntites.MESC1TS_SHOP
                                             where (from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                    where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                    select AG.COLUMN_VALUE).Contains(AR.SHOP_CD)
                                             select new { list = AR.SHOP_DESC, value = AR.SHOP_CD };

                        foreach (var col in objQueryResult)
                        {
                            UserList.Add(new AssignAuthGroup()
                            {
                                ValueItem = col.value,
                                ListItem = col.list,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return UserList;
        }
        public List<AvailableAssignAuthGroup> AvailablePermissionsByFilter(Int32 UserId, Int32 AuthGroupId, string SD, out int AvailablePermissionCount)
        {
            List<AvailableAssignAuthGroup> UserList = new List<AvailableAssignAuthGroup>();
            AvailablePermissionCount = 0;
            try
            {

                var objSecUserResult = (from AG in objUserServiceEntites.SEC_AUTHGROUP
                                        where AG.AUTHGROUP_ID == AuthGroupId
                                        select new { AG.AUTHGROUP_ID, AG.AUTHGROUP_NAME, AG.PARENT_AUTHGROUP_ID, AG.TABLE_NAME, AG.COLUMN_NAME, AG.COLUMN_DESC }).ToList();

                if (objSecUserResult.Count() > 0)
                {

                    if (objSecUserResult[0].TABLE_NAME == "MESC1TS_AREA")
                    {

                        var objQueryResult = from AR in objUserServiceEntites.MESC1TS_AREA
                                             where !(from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                     where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                     select AG.COLUMN_VALUE).Contains(AR.AREA_CD) && AR.AREA_CD.StartsWith(SD)
                                             select new { list = AR.AREA_DESC, value = AR.AREA_CD };

                        foreach (var col in objQueryResult)
                        {
                            UserList.Add(new AvailableAssignAuthGroup()
                            {
                                ValueItem = col.value,
                                ListItem = col.list,
                            });
                        }

                        objQueryResult = from AR in objUserServiceEntites.MESC1TS_AREA
                                         where !(from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                 where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                 select AG.COLUMN_VALUE).Contains(AR.AREA_CD)
                                         select new { list = AR.AREA_DESC, value = AR.AREA_CD };

                        if (objQueryResult.Count() > 0)
                        {
                            AvailablePermissionCount = objQueryResult.Count();
                        }

                    }
                    if (objSecUserResult[0].TABLE_NAME == "MESC1TS_COUNTRY")
                    {

                        var objQueryResult = from AR in objUserServiceEntites.MESC1TS_COUNTRY
                                             where !(from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                     where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                     select AG.COLUMN_VALUE).Contains(AR.COUNTRY_CD) && AR.COUNTRY_CD.StartsWith(SD)
                                             select new { list = AR.COUNTRY_DESC, value = AR.COUNTRY_CD };

                        foreach (var col in objQueryResult)
                        {
                            UserList.Add(new AvailableAssignAuthGroup()
                            {
                                ValueItem = col.value,
                                ListItem = col.list,
                            });
                        }
                        objQueryResult = from AR in objUserServiceEntites.MESC1TS_COUNTRY
                                         where !(from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                 where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                 select AG.COLUMN_VALUE).Contains(AR.COUNTRY_CD)
                                         select new { list = AR.COUNTRY_DESC, value = AR.COUNTRY_CD };
                        if (objQueryResult.Count() > 0)
                        {
                            AvailablePermissionCount = objQueryResult.Count();
                        }
                    }
                    if (objSecUserResult[0].TABLE_NAME == "MESC1TS_LOCATION")
                    {

                        var objQueryResult = from AR in objUserServiceEntites.MESC1TS_LOCATION
                                             where !(from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                     where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                     select AG.COLUMN_VALUE).Contains(AR.LOC_CD) && AR.LOC_CD.StartsWith(SD)
                                             select new { list = AR.LOC_DESC, value = AR.LOC_CD };

                        foreach (var col in objQueryResult)
                        {
                            UserList.Add(new AvailableAssignAuthGroup()
                            {
                                ValueItem = col.value,
                                ListItem = col.list,
                            });
                        }
                        objQueryResult = from AR in objUserServiceEntites.MESC1TS_LOCATION
                                         where !(from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                 where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                 select AG.COLUMN_VALUE).Contains(AR.LOC_CD)
                                         select new { list = AR.LOC_DESC, value = AR.LOC_CD };
                        if (objQueryResult.Count() > 0)
                        {
                            AvailablePermissionCount = objQueryResult.Count();
                        }
                    }
                    if (objSecUserResult[0].TABLE_NAME == "MESC1TS_VENDOR")
                    {

                        var objQueryResult = from AR in objUserServiceEntites.MESC1TS_VENDOR
                                             where !(from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                     where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                     select AG.COLUMN_VALUE).Contains(AR.VENDOR_CD) && AR.VENDOR_CD.StartsWith(SD)
                                             select new { list = AR.VENDOR_DESC, value = AR.VENDOR_CD };

                        foreach (var col in objQueryResult)
                        {
                            UserList.Add(new AvailableAssignAuthGroup()
                            {
                                ValueItem = col.value,
                                ListItem = col.list,
                            });
                        }
                        objQueryResult = from AR in objUserServiceEntites.MESC1TS_VENDOR
                                         where !(from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                 where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                 select AG.COLUMN_VALUE).Contains(AR.VENDOR_CD)
                                         select new { list = AR.VENDOR_DESC, value = AR.VENDOR_CD };
                        if (objQueryResult.Count() > 0)
                        {
                            AvailablePermissionCount = objQueryResult.Count();
                        }
                    }
                    if (objSecUserResult[0].TABLE_NAME == "MESC1TS_SHOP")
                    {

                        var objQueryResult = from AR in objUserServiceEntites.MESC1TS_SHOP
                                             where !(from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                     where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                     select AG.COLUMN_VALUE).Contains(AR.SHOP_CD) && AR.SHOP_CD.StartsWith(SD)
                                             select new { list = AR.SHOP_DESC, value = AR.SHOP_CD };
                        foreach (var col in objQueryResult)
                        {
                            UserList.Add(new AvailableAssignAuthGroup()
                            {
                                ValueItem = col.value,
                                ListItem = col.list,
                            });
                        }
                        objQueryResult = from AR in objUserServiceEntites.MESC1TS_SHOP
                                         where !(from AG in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                 where AG.USER_ID == UserId && AG.AUTHGROUP_ID == AuthGroupId
                                                 select AG.COLUMN_VALUE).Contains(AR.SHOP_CD)
                                         select new { list = AR.SHOP_DESC, value = AR.SHOP_CD };
                        if (objQueryResult.Count() > 0)
                        {
                            AvailablePermissionCount = objQueryResult.Count();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return UserList;
        }

        public bool InsertUserDataAccess(string SelectedActivePermission, int UserId, int AuthorisationGroupId, out string Msg)
        {


            bool IsSuccess = false;
            Msg = "";
            string[] strActivePermissionArray = null;
            char[] splitchar = { ',' };
            try
            {
                objUserServiceEntites.Configuration.AutoDetectChangesEnabled = false;// Has been introduce due to performence issue
                List<SEC_AUTHGROUP_USER> ObjSecAuthUser = (from user in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                           where user.USER_ID == UserId
                                                           select user).ToList();
                if (ObjSecAuthUser.Count() > 0)
                {

                    foreach (var col in ObjSecAuthUser.ToList())
                    {
                        objUserServiceEntites.SEC_AUTHGROUP_USER.Remove(col);
                    }
                    objUserServiceEntites.SaveChanges();
                }
                objUserServiceEntites.Configuration.AutoDetectChangesEnabled = false;// Has been introduce due to performence issue
                SEC_AUTHGROUP_USER userAuthInfoToBeInserted = new SEC_AUTHGROUP_USER();
                strActivePermissionArray = SelectedActivePermission.Split(splitchar);
                for (int iCount = 0; iCount <= strActivePermissionArray.Length - 1; iCount++)
                {
                    userAuthInfoToBeInserted = new SEC_AUTHGROUP_USER();
                    userAuthInfoToBeInserted.AUTHGROUP_ID = AuthorisationGroupId;
                    userAuthInfoToBeInserted.USER_ID = UserId;
                    userAuthInfoToBeInserted.COLUMN_VALUE = strActivePermissionArray[iCount].Trim();
                    objUserServiceEntites.SEC_AUTHGROUP_USER.Add(userAuthInfoToBeInserted);
                }
                objUserServiceEntites.SaveChanges();
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Msg = "Some Error has occurred while performing your activity. Error \n" + ex.Message;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return IsSuccess;
        }
        public bool AddAllCluster(int UserId, int AuthorisationGroupId, out string Msg)
        {
            bool IsSuccess = false;
            Msg = "";
            try
            {
                objUserServiceEntites.Configuration.AutoDetectChangesEnabled = false;// Has been introduce due to performence issue
                List<SEC_AUTHGROUP_USER> ObjSecAuthUser = (from user in objUserServiceEntites.SEC_AUTHGROUP_USER
                                                           where user.USER_ID == UserId
                                                           select user).ToList();
                if (ObjSecAuthUser.Count() > 0)
                {
                    foreach (var col in ObjSecAuthUser.ToList())
                    {
                        objUserServiceEntites.SEC_AUTHGROUP_USER.Remove(col);
                    }
                    objUserServiceEntites.SaveChanges();
                }


                objUserServiceEntites.Configuration.AutoDetectChangesEnabled = false;// Has been introduce due to performence issue
                var objQueryResult = from AR in objUserServiceEntites.MESC1TS_AREA
                                     select new { list = AR.AREA_DESC, value = AR.AREA_CD };
                foreach (var col in objQueryResult.ToList())
                {
                    SEC_AUTHGROUP_USER userAuthInfoToBeInserted = new SEC_AUTHGROUP_USER();
                    userAuthInfoToBeInserted.AUTHGROUP_ID = AuthorisationGroupId;
                    userAuthInfoToBeInserted.USER_ID = UserId;
                    userAuthInfoToBeInserted.COLUMN_VALUE = col.value;
                    objUserServiceEntites.SEC_AUTHGROUP_USER.Add(userAuthInfoToBeInserted);
                }

                objUserServiceEntites.SaveChanges();
                Msg = "All Cluster Updated successfully";
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Msg = "Some Error has occurred while performing your activity. Error \n" + ex.Message;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return IsSuccess;
        }
        #endregion




        public List<SecWebSite> GetWebPageList()
        {
            List<SecWebSite> SecWebSite = new List<SecWebSite>();
            try
            {
                var objUserResult = from WB in objUserServiceEntites.SEC_WEBSITE
                                    select new { WB.WEBSITE_ID, WB.WEBSITE_NAME };
                foreach (var col in objUserResult)
                {
                    SecWebSite.Add(new SecWebSite()
                    {
                        WebSiteId = col.WEBSITE_ID,
                        WebSiteName = col.WEBSITE_NAME,
                    });
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return SecWebSite;
        }
        //public List<SecWebSite> GetAuthGroupWebsiteAccessByAuthGroupId(int AuthorisationGroupId)
        //{


        //    StringBuilder StrSql = new StringBuilder();
        //    StrSql.AppendLine("SELECT SW.WEBSITE_NAME, SW.WEBSITE_ID ");
        //    StrSql.AppendLine("FROM SEC_AUTHGROUP_WEBSITE AS SAW JOIN SEC_WEBSITE AS SW ");
        //    StrSql.AppendLine("ON SAW.WEBSITE_ID=SW.WEBSITE_ID ");
        //    StrSql.AppendLine("WHERE SAW.AUTHGROUP_ID = " + AuthorisationGroupId  + " ");
        //    StrSql.AppendLine(" ORDER BY SW.WEBSITE_NAME"); 	

        //    SqlCommand cmd = new SqlCommand(StrSql.ToString());
        //    DataTable ObjDataTable = GetDataTable(cmd);


        //    List<SecWebSite> SecWebSite = new List<SecWebSite>();
        //    for (int iCnt = 0; iCnt < ObjDataTable.Rows.Count; iCnt++)
        //    {
        //        SecWebSite.Add(new SecWebSite()
        //        {
        //            WebSiteId =Convert.ToInt32(ObjDataTable.Rows[iCnt]["WEBSITE_ID"].ToString()),
        //            WebSiteName =ObjDataTable.Rows[iCnt]["WEBSITE_NAME"].ToString(),
        //        });

        //    }

        //    return SecWebSite;
        //}


        public List<SecWebSite> GetWebpageListByWebsiteId(int WebSiteId)
        {
            List<SecWebSite> SecWebSiteList = new List<SecWebSite>();
            try
            {
                var objUserResult = from WB in objUserServiceEntites.SEC_WEBSITE
                                    where WB.WEBSITE_ID == WebSiteId
                                    select new { WB.WEBSITE_ID, WB.WEBSITE_NAME };
                foreach (var col in objUserResult)
                {
                    SecWebSiteList.Add(new SecWebSite()
                    {
                        WebSiteId = col.WEBSITE_ID,
                        WebSiteName = col.WEBSITE_NAME,
                    });
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return SecWebSiteList;
        }


        public List<SecWebPage> GetAuthGroupWebpageAccessById(int WebSiteId, int AuthId)
        {
            List<SecWebPage> SecWebPageList = new List<SecWebPage>();
            try
            {
                var objUserResult = from SAW in objUserServiceEntites.SEC_AUTHGROUP_WEBPAGE
                                    from SW in objUserServiceEntites.SEC_WEBPAGE
                                    where SAW.AUTHGROUP_ID == AuthId && SAW.WEBPAGE_ID == SW.WEBPAGE_ID && SW.WEBSITE_ID == WebSiteId
                                    select new { SAW.WEBPAGE_ID, SW.WEBPAGE_NAME };
                foreach (var col in objUserResult)
                {
                    SecWebPageList.Add(new SecWebPage()
                    {
                        WebPageId = col.WEBPAGE_ID,
                        WebPageName = col.WEBPAGE_NAME,
                    });
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return SecWebPageList;
        }

        //public bool UpdateWebSitePermissions(string SelectedWebSitePermissions, int AuthorisationGroupId, out string Msg)
        //{
        //    bool IsSuccess = false;
        //    bool isExist = false, isExecutable = false;
        //    Msg = "";
        //    string[] strActivePermissionArray = null;
        //    char[] splitchar = { ',' };



        //    List<SecWebSite> objGetWebPageListByAuthId = GetAuthGroupWebsiteAccessByAuthGroupId(AuthorisationGroupId).ToList();
        //    StringBuilder StrSql = new StringBuilder();
        //    strActivePermissionArray = SelectedWebSitePermissions.Split(splitchar);
        //    for (int iCount = 0; iCount <= strActivePermissionArray.Length - 2; iCount++)
        //    {
        //        isExist = false;
        //        foreach (var row in objGetWebPageListByAuthId)
        //        {
        //            if (strActivePermissionArray[iCount].Trim() == row.WebSiteId.ToString())
        //            {
        //                isExist = true;
        //            }                        
        //        }
        //        if (isExist == false)
        //        {
        //            isExecutable = true;
        //            StrSql.AppendLine("INSERT INTO SEC_AUTHGROUP_WEBSITE( AUTHGROUP_ID, WEBSITE_ID )   ");
        //            StrSql.AppendLine("VALUES('" + AuthorisationGroupId + "','" + strActivePermissionArray[iCount].Trim() + "') ");
        //        }               
        //    }
        //    if (isExecutable == true)
        //    {
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand(StrSql.ToString());
        //            cmd.CommandType = CommandType.Text;
        //            Ex_Nonquery(cmd);
        //        }
        //        catch (Exception ex)
        //        {
        //            Msg = "Some Error has occurred while performing your activity. Error \n" + ex.Message;
        //            return IsSuccess;
        //        }
        //    }
        //    isExecutable = false;
        //    List<SecWebSite> objGetWebPageList = GetWebPageList().ToList();
        //    foreach (var row in objGetWebPageList)
        //    {
        //        isExist = false;
        //        for (int iCount = 0; iCount <= strActivePermissionArray.Length - 2; iCount++)
        //        {

        //            if (strActivePermissionArray[iCount].Trim() == row.WebSiteId.ToString())
        //            {
        //                isExist = true;
        //            }
        //        }
        //        if (isExist == false)
        //        {
        //            isExecutable = true;
        //            StrSql.AppendLine("DELETE FROM SEC_AUTHGROUP_WEBPAGE  WHERE WEBPAGE_ID IN (SELECT WEBPAGE_ID FROM SEC_WEBPAGE WHERE WEBSITE_ID ='" + row.WebSiteId.ToString() + "') and AUTHGROUP_ID='" + AuthorisationGroupId + "' ");

        //            StrSql.AppendLine("DELETE FROM SEC_AUTHGROUP_WEBSITE WHERE AUTHGROUP_ID='" + AuthorisationGroupId + "' and WEBSITE_ID ='" + row.WebSiteId.ToString() + "' ");

        //        }  
        //    }

        //    if (isExecutable == true)
        //    {
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand(StrSql.ToString());
        //            cmd.CommandType = CommandType.Text;
        //            Ex_Nonquery(cmd);
        //            IsSuccess = true;
        //            Msg = "Website data access updated successfully";
        //        }
        //        catch (Exception ex)
        //        {
        //            IsSuccess = false;
        //            Msg = "Some Error has occurred while performing your activity. Error \n" + ex.Message;
        //        }
        //    }
        //    return IsSuccess;
        //}
        public bool UpdateWebPagePermissions(string SelectedWebSitePermissions, int AuthorisationGroupId, int WebpageId, out string Msg)
        {
            bool IsSuccess = false;
            Msg = "";
            try
            {
                string[] strActivePermissionArray = null;
                char[] splitchar = { ',' };
                List<SEC_AUTHGROUP_WEBPAGE> ObjSecAuthUser = (from user in objUserServiceEntites.SEC_AUTHGROUP_WEBPAGE
                                                              where user.AUTHGROUP_ID == AuthorisationGroupId && user.WEBPAGE_ID == WebpageId
                                                              select user).ToList();
                if (ObjSecAuthUser.Count() > 0)
                {
                    foreach (var col in ObjSecAuthUser.ToList())
                    {
                        objUserServiceEntites.SEC_AUTHGROUP_WEBPAGE.Remove(col);
                        objUserServiceEntites.SaveChanges();
                    }
                }

                SEC_AUTHGROUP_WEBPAGE userAuthInfoToBeInserted = new SEC_AUTHGROUP_WEBPAGE();
                strActivePermissionArray = SelectedWebSitePermissions.Split(splitchar);
                for (int iCount = 0; iCount <= strActivePermissionArray.Length - 1; iCount++)
                {
                    userAuthInfoToBeInserted = new SEC_AUTHGROUP_WEBPAGE();
                    userAuthInfoToBeInserted.AUTHGROUP_ID = AuthorisationGroupId;
                    userAuthInfoToBeInserted.WEBPAGE_ID = Convert.ToInt16(strActivePermissionArray[iCount].Trim());
                    objUserServiceEntites.SEC_AUTHGROUP_WEBPAGE.Add(userAuthInfoToBeInserted);
                    objUserServiceEntites.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Msg = "Some Error has occurred while performing your activity. Error \n" + ex.Message;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return IsSuccess;
        }

    }
}
