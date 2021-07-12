using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ManageUserService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class ManageUser : IManageUser
    {
        ManageUserServiceEntities objUserServiceEntites;
        ManageUser()
        {
            objUserServiceEntites = new ManageUserServiceEntities();
        }
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }
        public bool isUserExistInDb(Int32 userId)
        {
            List<SEC_USER> ObjUser = (from user in objUserServiceEntites.SEC_USER
                                      where user.USER_ID == userId
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

        public List<Country> GetCountryList()
        {
            var objCountryResult = from country in objUserServiceEntites.MESC1TS_COUNTRY
                                   orderby country.COUNTRY_DESC
                                   select country;
            List<Country> CountryList = new List<Country>();
            foreach (var col in objCountryResult)
            {
                Country objCountry = new Country();
                objCountry.CountryCode = col.COUNTRY_CD;
                objCountry.CountryDescription = col.COUNTRY_DESC;
                CountryList.Add(objCountry);
            }
            return CountryList;
        }

        public bool AddUser(UserInfo UserInfoFromClient, out string Msg)
        {
            bool IsSuccess = false;
            SEC_USER userInfoToBeInserted = new SEC_USER();
            userInfoToBeInserted.LOGIN = UserInfoFromClient.Login;
            userInfoToBeInserted.FIRSTNAME = UserInfoFromClient.FirstName;
            userInfoToBeInserted.LASTNAME = UserInfoFromClient.LastName;
            userInfoToBeInserted.COMPANY = UserInfoFromClient.Company;
            userInfoToBeInserted.LOC_CD = UserInfoFromClient.Loccd;
            userInfoToBeInserted.ACTIVE_STATUS = UserInfoFromClient.ActiveStatus;
            userInfoToBeInserted.APPROVAL_AMOUNT = UserInfoFromClient.ApproveAmount;
            objUserServiceEntites.SEC_USER.AddObject(userInfoToBeInserted);
            try
            {
                objUserServiceEntites.SaveChanges();
                IsSuccess = true;
                Msg = "User created successfull.";
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Msg = "Some Error has occurred while performing your activity. Error \n" + ex.Message;
            }
            return IsSuccess;
        }

        public bool UpdateUser(UserInfo UserInfoFromClient)
        {

            bool IsSuccess = false;

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
            try
            {
                objUserServiceEntites.SaveChanges();
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
            }
            return IsSuccess;

        }

        public bool DeleteUser(int UserToBeDeleted)
        {
            bool IsSuccess = false;
            List<SEC_USER> ObjUser = (from user in objUserServiceEntites.SEC_USER
                                      where user.USER_ID == UserToBeDeleted
                                      select user).ToList();
            objUserServiceEntites.DeleteObject(ObjUser);
            return IsSuccess;
        }

        public List<WorkOrderReport> GetDataListForWorkOrder()
        {
            ManageUserServiceEntities db = new ManageUserServiceEntities();
            var results = (from S in db.MESC1TS_SHOP
                           join ST in db.MESC1TS_SHOP_CONT on S.SHOP_CD equals ST.SHOP_CD
                           join CR in db.MESC1TS_CURRENCY on S.CUCDN equals CR.CUCDN
                           join V in db.MESC1TS_VENDOR on S.VENDOR_CD equals V.VENDOR_CD
                           join RP in db.MESC1TS_REPAIR_CODE on ST.REPAIR_CD equals RP.REPAIR_CD
                           where (ST.MANUAL_CD == RP.MANUAL_CD) && (ST.MODE == RP.MODE)
                           select new { ST.MODE, ST.REPAIR_CD, RP.REPAIR_DESC, ST.CONTRACT_AMOUNT, ST.EFF_DTE, ST.EXP_DTE }).Take(10);


            List<WorkOrderReport> WorkOrderReportList = new List<WorkOrderReport>();
            WorkOrderReport objWorkOrderReport = new WorkOrderReport();
            foreach (var col in results)
            {
                objWorkOrderReport.Mode = col.MODE;
                objWorkOrderReport.RepairCode = col.REPAIR_CD;
                objWorkOrderReport.RepairDesc = col.REPAIR_DESC;
                objWorkOrderReport.ContractAmount = col.CONTRACT_AMOUNT;
                objWorkOrderReport.EffDate = col.EFF_DTE;
                objWorkOrderReport.ExpDate = col.EXP_DTE;
                WorkOrderReportList.Add(objWorkOrderReport);
            }


            return WorkOrderReportList;
            // return string.Format("You entered: {0}", value);
        }
    }
}
