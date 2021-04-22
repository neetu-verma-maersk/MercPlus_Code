using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MercPlusServiceLibrary;

namespace MercWOApprovalDAL
{
    class MercWOApprovalDAL
    {
        MESC2DSEntities context;
        string text = "";
        public static LogEntry logEntry = new LogEntry();
        public List<MESC1TS_AUDIT_HISTORY> FetchNewRecord()
        {
            List<MESC1TS_AUDIT_HISTORY> auditlist = new List<MESC1TS_AUDIT_HISTORY>();
            context = new MESC2DSEntities();
            try
            {
                var auditData = (from con in context.MESC1TS_AUDIT_HISTORY
                                 where con.MERC_STATUS.ToUpper() == "NEW"
                                 orderby con.AUDIT_DATE
                                 select con).Take(50).ToList();
                foreach (var item in auditData)
                {
                    MESC1TS_AUDIT_HISTORY audit = new MESC1TS_AUDIT_HISTORY();
                    audit.WO_ID = item.WO_ID;
                    audit.SHOP_CD = item.SHOP_CD;
                    audit.VISIT_ID = item.VISIT_ID;
                    audit.MERC_STATUS = item.MERC_STATUS;
                    audit.MERC_COMMENTS = item.MERC_COMMENTS;
                    audit.COMMENT = item.COMMENT;
                    audit.CERTIFICATE_ID = item.CERTIFICATE_ID;
                    audit.AUDIT_RESULT = item.AUDIT_RESULT;
                    audit.AUDITOR = item.AUDITOR;
                    audit.AUDIT_DATE = item.AUDIT_DATE;
                    auditlist.Add(audit);


                }


            }
            catch (Exception ex)
            {
                MercWOAppRej.MercWoApproval.logEntry.Message = "MercWorkOrderUpdate ERROR:" + ex.Message;
                Logger.Write(MercWOAppRej.MercWoApproval.logEntry);
            }
            return auditlist;
        }
        public Boolean UpdateInProgress(int WO_ID, string VisitID, string auditor)
        {

            context = new MESC2DSEntities();


            try
            {
                var AuditUpdate = (from audit in context.MESC1TS_AUDIT_HISTORY
                                   where audit.WO_ID == WO_ID && audit.AUDITOR == auditor && audit.VISIT_ID == VisitID
                                   select audit).ToList();
                if (AuditUpdate.Count > 0)
                {
                    AuditUpdate[0].MERC_STATUS = "INPROGRESS";
                    int Status = context.SaveChanges();
                    if (Status == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                logEntry.Message = "Unable to Update As Inprogress for WorkOrder ID -" + WO_ID+  " - " + ex.Message;
                Logger.Write(logEntry);
                return false;

            }

        }
        public void UpdateAsFailed(int WO_ID, string VisitID, string auditor, string text,string Code)
        {

            context = new MESC2DSEntities();


            try
            {
                var AuditUpdate = (from audit in context.MESC1TS_AUDIT_HISTORY
                                   where audit.WO_ID == WO_ID && audit.AUDITOR == auditor && audit.VISIT_ID == VisitID
                                   select audit).ToList();
                if (AuditUpdate.Count > 0)
                {
                    AuditUpdate[0].MERC_STATUS = "FAILED";
                    AuditUpdate[0].MERC_COMMENTS = Code + " : WO_ID : " + WO_ID + " is not processed  ("+DateTime.Now+")" + text ;
                    int Status = context.SaveChanges();
                }


            }
            catch (Exception ex)
            {
                logEntry.Message = "Unable to Update As Failed for Workorder ID -" + WO_ID + " - " + ex.Message;
                Logger.Write(logEntry);


            }

        }
        public Boolean ValidateUser(string CertificateID, int WO_ID, string VisitID, string auditor)
        {

            context = new MESC2DSEntities();


            try
            {
                var UserData = (from U in context.SEC_USER
                                from UG in context.SEC_AUTHGROUP_USER
                                .Where(s => s.USER_ID == U.USER_ID).DefaultIfEmpty()
                                where U.LOGIN == CertificateID
                                from G in context.SEC_AUTHGROUP
                               .Where(sc => sc.AUTHGROUP_ID == UG.AUTHGROUP_ID).DefaultIfEmpty()
                                select new
                                  {
                                      UG.AUTHGROUP_ID,
                                      G.AUTHGROUP_NAME,
                                      //COLUMN_VALUE = UG.COLUMN_VALUE,
                                      //G.TABLE_NAME,
                                      //COLUMN_NAME = G.COLUMN_NAME
                                  }).OrderBy(id => id.AUTHGROUP_ID).Distinct().ToList();
                if (UserData.Count > 0)
                {
                    // if (UserData[0].AUTHGROUP_NAME == "CPH" || UserData[0].AUTHGROUP_NAME == "AREA" || UserData[0].AUTHGROUP_NAME == "COUNTRY" || UserData[0].AUTHGROUP_NAME == "MSL (LOCAL EMR)")
                    if (UserData[0].AUTHGROUP_NAME == "CPH" || UserData[0].AUTHGROUP_NAME == "EMR_SPECIALIST_COUNTRY" || UserData[0].AUTHGROUP_NAME == "EMR_APPROVER_COUNTRY" || UserData[0].AUTHGROUP_NAME == "EMR_SPECIALIST_SHOP" || UserData[0].AUTHGROUP_NAME == "EMR_APPROVER_SHOP")
                    {
                        return true;
                    }
                    else
                    {
                        //logEntry.Message = "User not in MSL,CPH,AREA,COUNTRY for WorkOrder ID -" + WO_ID;
                        //Logger.Write(logEntry);
                        text = " - User does not have access like CPH,EMR_SPECIALIST_COUNTRY,EMR_APPROVER_COUNTRY,EMR_SPECIALIST_SHOP,EMR_APPROVER_SHOP...";
                        UpdateAsFailed(WO_ID, VisitID, auditor, text,"104");
                        return false;
                    }
                }

                else
                {
                    //logEntry.Message = "User not exist in Merc Plus for WorkOrder ID -" + WO_ID;
                    //Logger.Write(logEntry);
                    text = " - User does not exist in Merc Plus...";
                    UpdateAsFailed(WO_ID, VisitID, auditor, text,"102");
                    return false;
                }

            }
            catch (Exception ex)
            {
                logEntry.Message = "Unable to Validate User in Merc Plus for WorkOrder ID -" + WO_ID + " - " + ex.Message;
                Logger.Write(logEntry);
                return false;

            }

        }
        public Boolean ValidateShops(int WO_ID, string CertificateID,string VisitID,string Auditor)
        {
            context = new MESC2DSEntities();



            //////////////Shop List/////////////////////////
            List<MercPlusLibrary.Shop> ShopList = new List<MercPlusLibrary.Shop>();
            List<string> ShopFinal = new List<string>();
            List<MercPlusLibrary.WorkOrderDetail> WOList = null;
            try
            {
                var ShopOnAuth = (from U in context.SEC_USER
                                  from UG in context.SEC_AUTHGROUP_USER
                                  .Where(s => s.USER_ID == U.USER_ID).DefaultIfEmpty()
                                  where U.LOGIN == CertificateID
                                  from G in context.SEC_AUTHGROUP
                                 .Where(sc => sc.AUTHGROUP_ID == UG.AUTHGROUP_ID).DefaultIfEmpty()
                                  select new
                                  {
                                      UG.AUTHGROUP_ID,
                                      UG.COLUMN_VALUE,
                                      G.TABLE_NAME,
                                      G.COLUMN_NAME
                                  }).OrderBy(id => id.AUTHGROUP_ID).ToList();

                List<string> VendorCodeList = new List<string>();
                List<string> LocCodeList = new List<string>();
                List<string> CountryCodeList = new List<string>();
                List<string> ShopCodeList = new List<string>();
                List<string> AreaCodeList = new List<string>();
                foreach (var item in ShopOnAuth)
                {
                    //if (item.COLUMN_NAME == "VENDOR_CD")
                   // {
                       // string VendorCode = item.COLUMN_VALUE;
                       // VendorCodeList.Add(VendorCode);
                    //}
                   // if (item.COLUMN_NAME == "LOC_CD")
                   // {
                        //string LocCode = item.COLUMN_VALUE;
                        //LocCodeList.Add(LocCode);
                   // }
                    if (item.COLUMN_NAME == "COUNTRY_CD")
                    {
                        string CountryCode = item.COLUMN_VALUE;
                        CountryCodeList.Add(CountryCode);
                    }
                    if (item.COLUMN_NAME == "AREA_CD")
                    {
                        string AreaCode = item.COLUMN_VALUE;
                        AreaCodeList.Add(AreaCode);
                    }
                    if (item.COLUMN_NAME == "SHOP_CD")
                    {
                        string ShopCd = item.COLUMN_VALUE;
                        ShopCodeList.Add(ShopCd);
                    }
                }

                var ShopListFromDBOnAuth = (from shop in context.MESC1VS_SHOP_LOCATION
                                            where
                                                shop.SHOP_ACTIVE_SW == "Y" &&
                                            (VendorCodeList.Contains(shop.VENDOR_CD) ||
                                            LocCodeList.Contains(shop.LOC_CD) ||
                                            CountryCodeList.Contains(shop.COUNTRY_CD) ||
                                            AreaCodeList.Contains(shop.AREA_CD) ||
                                            ShopCodeList.Contains(shop.SHOP_CD))
                                            select new
                                            {
                                                shop.SHOP_CD,
                                                shop.CUCDN,
                                                shop.SHOP_DESC
                                            }).OrderBy(code => code.SHOP_CD).ToList();
                if (ShopListFromDBOnAuth != null && ShopListFromDBOnAuth.Count > 0)
                {
                    var ShopListFromDB = (from S in context.MESC1TS_SHOP
                                          from SM in context.MESC1TS_CUST_SHOP_MODE
                                          from CY in context.MESC1TS_CURRENCY
                                          from CU in context.MESC1TS_CUSTOMER
                                          where S.SHOP_CD == SM.SHOP_CD &&
                                              S.SHOP_ACTIVE_SW == "Y" &&
                                                S.CUCDN == CY.CUCDN &&
                                                CU.CUSTOMER_CD == SM.CUSTOMER_CD
                                          orderby S.SHOP_CD
                                          select new
                                          {
                                              S.SHOP_CD
                                          }).ToList();


                    var ShopListFinal = ShopListFromDBOnAuth.FindAll(a => ShopListFromDB.Any(ab => ab.SHOP_CD == a.SHOP_CD));
                   
                    foreach (var rItem in ShopListFinal)
                    {

                        ShopFinal.Add(rItem.SHOP_CD.Trim());

                    }
                }
                 ////////////End Shop List///////////////////////
                    var WOValidation=(from wo in context.MESC1TS_WO
                                     where wo.WO_ID==WO_ID &&  ShopFinal.Contains(wo.SHOP_CD)
                                     select new
                                          {
                                              wo.WO_ID,
                                              wo.SHOP_CD
                                          }).ToList();
                    if (WOValidation.Count>0)
                    {
                        return true;
                    }
                    else
                    {
                        //logEntry.Message = "User doesnot have acces to respective shop  in Merc Plus for WorkOrder ID -" + WO_ID;
                        //Logger.Write(logEntry);
                        text = " - User does not have access to respective shop  in Merc Plus for WorkOrder ID -" + WO_ID;
                        
                        UpdateAsFailed(WO_ID, VisitID, Auditor, text,"103");
                        return false;
                    }


            }
            catch (Exception ex)
            {
                logEntry.Message = "Unable to Validate User in Merc Plus for WorkOrder ID -" + WO_ID + " - " + ex.Message;
                Logger.Write(logEntry);
                return false;

            }

        }


          private void UpdateApproverDetails(int workorderid, int status, string userdata)
        {
            try
            {
                if (status == 100 || status == 130 || status == 390)
                {
                    int approverLevel = 0;
                    DateTime currentDate = DateTime.UtcNow;

                    var user = context.SEC_USER.FirstOrDefault(x => x.LOGIN == userdata);
                    if (user != null)
                    {
                        var authGroupUser = context.SEC_AUTHGROUP_USER.FirstOrDefault(x => x.USER_ID == user.USER_ID);
                        if (authGroupUser != null)
                        {
                            var authGroup = context.SEC_AUTHGROUP.FirstOrDefault(x => x.AUTHGROUP_ID == authGroupUser.AUTHGROUP_ID);
                            string authGroupName = authGroup.AUTHGROUP_NAME;

                            if (!string.IsNullOrEmpty(authGroupName) && authGroupName.ToUpper().Contains("EMR_APPROVER"))
                            {
                                approverLevel = 1;
                            }
                            else if (!string.IsNullOrEmpty(authGroupName) && authGroupName.ToUpper().Contains("EMR_SPECIALIST"))
                            {
                                approverLevel = 2;
                            }
                            else
                            {
                                approverLevel = 3;
                            }
                        }
                    }

                    if (approverLevel != 0)
                    {
                        var approverDetail = context.MESC1TS_APPROVER_DETAILS.FirstOrDefault(x => x.WO_ID == workorderid);
                        if (approverDetail == null)
                        {
                            MESC1TS_APPROVER_DETAILS newApproverDetail = new MESC1TS_APPROVER_DETAILS();

                            newApproverDetail.WO_ID = workorderid;
                            newApproverDetail.STATUS = status;
                            newApproverDetail.APPROVAL_LEVEL = approverLevel;
                            newApproverDetail.CHUSER = userdata;
                            newApproverDetail.CHTS = currentDate;

                            context.MESC1TS_APPROVER_DETAILS.Add(newApproverDetail);
                        }
                        else
                        {
                            approverDetail.STATUS = status;
                            approverDetail.APPROVAL_LEVEL = approverLevel;
                            approverDetail.CHUSER = userdata;
                            approverDetail.CHTS = currentDate;
                        }

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }



          public Boolean StartAcceptReject(int WO_ID, string CERTIFICATE_ID, string AUDITOR, string COMMENT, string VISIT_ID, int? AUDIT_RESULT)
          {



              try
              {
                  var UserData = (from U in context.SEC_USER
                                  from UG in context.SEC_AUTHGROUP_USER
                                  .Where(s => s.USER_ID == U.USER_ID).DefaultIfEmpty()
                                  where U.LOGIN == CERTIFICATE_ID
                                  from G in context.SEC_AUTHGROUP
                                 .Where(sc => sc.AUTHGROUP_ID == UG.AUTHGROUP_ID).DefaultIfEmpty()
                                  select new
                                  {
                                      UG.AUTHGROUP_ID,
                                      G.AUTHGROUP_NAME,
                                      //COLUMN_VALUE = UG.COLUMN_VALUE,
                                      //G.TABLE_NAME,
                                      //COLUMN_NAME = G.COLUMN_NAME
                                  }).OrderBy(id => id.AUTHGROUP_ID).Distinct().ToList();
                  var MainWoData = (from wo in context.MESC1TS_WO
                                    where wo.WO_ID == WO_ID
                                    select wo).ToList();
                  if (UserData.Count > 0)
                  {

                      //if (UserData[0].AUTHGROUP_NAME == "CPH" || UserData[0].AUTHGROUP_NAME == "AREA" || UserData[0].AUTHGROUP_NAME == "COUNTRY" || UserData[0].AUTHGROUP_NAME == "MSL (LOCAL EMR)")
                      if (UserData[0].AUTHGROUP_NAME == "CPH" || UserData[0].AUTHGROUP_NAME == "EMR_SPECIALIST_COUNTRY" || UserData[0].AUTHGROUP_NAME == "EMR_APPROVER_COUNTRY" || UserData[0].AUTHGROUP_NAME == "EMR_SPECIALIST_SHOP" || UserData[0].AUTHGROUP_NAME == "EMR_APPROVER_SHOP")
                      {

                          if (MainWoData[0].STATUS_CODE == 390)
                          {
                              int STATUS_CODE = Convert.ToInt32(MainWoData[0].STATUS_CODE);
                              string AuthName = UserData[0].AUTHGROUP_NAME.ToString();


                              if (AUDIT_RESULT == 0) // asked to reject
                              {
                                  if (UserData[0].AUTHGROUP_NAME == "EMR_SPECIALIST_SHOP" || UserData[0].AUTHGROUP_NAME == "EMR_SPECIALIST_COUNTRY") // Check if current user is 2ND LEVEL USER
                                  {
                                      var WoData = (from wo in context.MESC1TS_APPROVER_DETAILS
                                                    where wo.WO_ID == WO_ID
                                                    select wo).ToList();
                                      if (WoData == null || WoData.Count == 0)
                                      {
                                          MESC1TS_APPROVER_DETAILS newApproverDetail = new MESC1TS_APPROVER_DETAILS();

                                          newApproverDetail.WO_ID = WO_ID;
                                          newApproverDetail.STATUS = MainWoData[0].STATUS_CODE;
                                          newApproverDetail.APPROVAL_LEVEL = 1;
                                          newApproverDetail.CHUSER = MainWoData[0].CHUSER;
                                          newApproverDetail.CHTS = MainWoData[0].CHTS;

                                          context.MESC1TS_APPROVER_DETAILS.Add(newApproverDetail);
                                          context.SaveChanges();
                                      }
                                      WoData = (from wo in context.MESC1TS_APPROVER_DETAILS
                                                where wo.WO_ID == WO_ID
                                                select wo).ToList();

                                      if (WoData != null && WoData.Count > 0)
                                      {
                                          if (WoData[0].APPROVAL_LEVEL != null)
                                          {
                                              if (WoData[0].APPROVAL_LEVEL == 1)
                                              {

                                                  WoData[0].STATUS = 100;
                                                  WoData[0].APPROVAL_LEVEL = 2;
                                                  WoData[0].CHUSER = CERTIFICATE_ID;
                                                  MainWoData[0].CHTS = DateTime.Now;
                                                  context.SaveChanges();
                                                  MainWoData[0].STATUS_CODE = 100;
                                                  MainWoData[0].CHTS = DateTime.Now;
                                                  MainWoData[0].CHUSER = CERTIFICATE_ID + "[Audit Tool]";
                                                  context.SaveChanges();

                                                  MESC1TS_WOREMARK WORemark = new MESC1TS_WOREMARK();
                                                  string Remark = "Rejected By Audit Tool User[" + CERTIFICATE_ID + "]";
                                                  WORemark.WO_ID = WO_ID;
                                                  WORemark.CRTS = DateTime.Now;
                                                  WORemark.REMARK_TYPE = "S";
                                                  WORemark.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                                  WORemark.REMARK = Remark;
                                                  context.MESC1TS_WOREMARK.Add(WORemark);
                                                  context.SaveChanges();
                                                  if (!string.IsNullOrEmpty(COMMENT))
                                                  {
                                                      MESC1TS_WOREMARK WORemark1 = new MESC1TS_WOREMARK();
                                                      string Remark1 = COMMENT;
                                                      WORemark.WO_ID = WO_ID;
                                                      WORemark.CRTS = DateTime.Now;
                                                      WORemark.REMARK_TYPE = "E";  //Pinaki
                                                      WORemark.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                                      WORemark.REMARK = COMMENT;
                                                      context.MESC1TS_WOREMARK.Add(WORemark);
                                                      context.SaveChanges();
                                                  }
                                                  UpdateAsSuccess(WO_ID, VISIT_ID, AUDITOR, "Successfully");
                                                  return true;
                                              }

                                              else
                                              {
                                                  text = " - Can not reject, already approved by second level user..  ";
                                                  UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                                  return false;

                                              }
                                          }
                                          else
                                          {
                                              text = " - Can not reject, approval level is missing ..  ";
                                              UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                              return false;
                                          }
                                      }
                                      else
                                      {
                                          text = " - Unable to find any past approve/reject information..  ";
                                          UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                          return false;
                                      }

                                  }
                                  //cph user//
                                  if (UserData[0].AUTHGROUP_NAME == "CPH") // Check if current user is CPH USER
                                  {
                                      var WoData = (from wo in context.MESC1TS_APPROVER_DETAILS
                                                    where wo.WO_ID == WO_ID
                                                    select wo).ToList();
                                      if (WoData == null || WoData.Count == 0)
                                      {
                                          MESC1TS_APPROVER_DETAILS newApproverDetail = new MESC1TS_APPROVER_DETAILS();

                                          newApproverDetail.WO_ID = WO_ID;
                                          newApproverDetail.STATUS = MainWoData[0].STATUS_CODE;
                                          newApproverDetail.APPROVAL_LEVEL = 1;
                                          newApproverDetail.CHUSER = MainWoData[0].CHUSER;
                                          newApproverDetail.CHTS = MainWoData[0].CHTS;

                                          context.MESC1TS_APPROVER_DETAILS.Add(newApproverDetail);
                                               context.SaveChanges();
                                      }
                                      WoData = (from wo in context.MESC1TS_APPROVER_DETAILS
                                                where wo.WO_ID == WO_ID
                                                select wo).ToList();

                                      if (WoData != null && WoData.Count > 0)
                                      {
                                          if (WoData[0].APPROVAL_LEVEL != null)
                                          {
                                              if (WoData[0].APPROVAL_LEVEL == 1 || WoData[0].APPROVAL_LEVEL == 2)
                                              {

                                                  WoData[0].STATUS = 100;
                                                  WoData[0].APPROVAL_LEVEL = 3;
                                                  WoData[0].CHUSER = CERTIFICATE_ID;
                                                  MainWoData[0].CHTS = DateTime.Now;
                                                  context.SaveChanges();
                                                  MainWoData[0].STATUS_CODE = 100;
                                                  MainWoData[0].CHTS = DateTime.Now;
                                                  MainWoData[0].CHUSER = CERTIFICATE_ID + "[Audit Tool]";
                                                  context.SaveChanges();

                                                  MESC1TS_WOREMARK WORemark = new MESC1TS_WOREMARK();
                                                  string Remark = "Rejected By Audit Tool User[" + CERTIFICATE_ID + "]";
                                                  WORemark.WO_ID = WO_ID;
                                                  WORemark.CRTS = DateTime.Now;
                                                  WORemark.REMARK_TYPE = "S";
                                                  WORemark.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                                  WORemark.REMARK = Remark;
                                                  context.MESC1TS_WOREMARK.Add(WORemark);
                                                  context.SaveChanges();
                                                  if (!string.IsNullOrEmpty(COMMENT))
                                                  {
                                                      MESC1TS_WOREMARK WORemark1 = new MESC1TS_WOREMARK();
                                                      string Remark1 = COMMENT;
                                                      WORemark.WO_ID = WO_ID;
                                                      WORemark.CRTS = DateTime.Now;
                                                      WORemark.REMARK_TYPE = "E";  //Pinaki
                                                      WORemark.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                                      WORemark.REMARK = COMMENT;
                                                      context.MESC1TS_WOREMARK.Add(WORemark);
                                                      context.SaveChanges();
                                                  }
                                                  UpdateAsSuccess(WO_ID, VISIT_ID, AUDITOR, "Successfully");
                                                  return true;
                                              }

                                              else
                                              {
                                                  text = " - Can not reject, already approved by CPH user..  ";
                                                  UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                                  return false;

                                              }
                                          }
                                          else
                                          {
                                              text = " - Can not reject, approval level is missing ..  ";
                                              UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                              return false;
                                          }
                                      }
                                      else
                                      {
                                          text = " - Unable to find any past approve/reject information..  ";
                                          UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                          return false;
                                      }

                                  }

                                  else
                                  {
                                      text = " The user is not belong to second level user or CPH user..  ";
                                      UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                      return false;
                                  }
                              }
                              else
                              {
                                  text = " The Work Order is already Approved..  ";
                                  UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                  return false;
                              }
                          }

                          if (MainWoData[0].STATUS_CODE == 100)
                          {
                              int STATUS_CODE = Convert.ToInt32(MainWoData[0].STATUS_CODE);
                              string AuthName = UserData[0].AUTHGROUP_NAME.ToString();


                              if (AUDIT_RESULT == 1) //asked to approve
                              {
                                  if (UserData[0].AUTHGROUP_NAME == "EMR_SPECIALIST_SHOP" || UserData[0].AUTHGROUP_NAME == "EMR_SPECIALIST_COUNTRY") // Check if current user is 2ND LEVEL USER
                                  {
                                      var WoData = (from wo in context.MESC1TS_APPROVER_DETAILS
                                                    where wo.WO_ID == WO_ID
                                                    select wo).ToList();
                                      if (WoData == null || WoData.Count == 0)
                                      {
                                          MESC1TS_APPROVER_DETAILS newApproverDetail = new MESC1TS_APPROVER_DETAILS();

                                          newApproverDetail.WO_ID = WO_ID;
                                          newApproverDetail.STATUS = MainWoData[0].STATUS_CODE;
                                          newApproverDetail.APPROVAL_LEVEL = 1;
                                          newApproverDetail.CHUSER = MainWoData[0].CHUSER;
                                          newApproverDetail.CHTS = MainWoData[0].CHTS;

                                          context.MESC1TS_APPROVER_DETAILS.Add(newApproverDetail);
                                           context.SaveChanges();
                                      }
                                      WoData = (from wo in context.MESC1TS_APPROVER_DETAILS
                                                where wo.WO_ID == WO_ID
                                                select wo).ToList();

                                      if (WoData != null && WoData.Count > 0)
                                      {
                                          if (WoData[0].APPROVAL_LEVEL != null)
                                          {
                                              if (WoData[0].APPROVAL_LEVEL == 1)
                                              {
                                                  WoData[0].STATUS = 390;
                                                  WoData[0].APPROVAL_LEVEL = 2;
                                                  WoData[0].CHUSER = CERTIFICATE_ID;
                                                  MainWoData[0].CHTS = DateTime.Now;
                                                  context.SaveChanges();
                                                  MainWoData[0].STATUS_CODE = 390;
                                                  MainWoData[0].CHTS = DateTime.Now;
                                                  MainWoData[0].CHUSER = CERTIFICATE_ID + "[Audit Tool]";
                                                  context.SaveChanges();

                                                  MESC1TS_WOREMARK WORemark = new MESC1TS_WOREMARK();
                                                  string Remark = "Approved By Audit Tool User[" + CERTIFICATE_ID + "]";
                                                  WORemark.WO_ID = WO_ID;
                                                  WORemark.CRTS = DateTime.Now;
                                                  WORemark.REMARK_TYPE = "S";
                                                  WORemark.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                                  WORemark.REMARK = Remark;
                                                  context.MESC1TS_WOREMARK.Add(WORemark);
                                                  context.SaveChanges();
                                                  if (!string.IsNullOrEmpty(COMMENT))
                                                  {
                                                      MESC1TS_WOREMARK WORemark1 = new MESC1TS_WOREMARK();
                                                      string Remark1 = COMMENT;
                                                      WORemark.WO_ID = WO_ID;
                                                      WORemark.CRTS = DateTime.Now;
                                                      WORemark.REMARK_TYPE = "E";  //Pinaki
                                                      WORemark.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                                      WORemark.REMARK = COMMENT;
                                                      context.MESC1TS_WOREMARK.Add(WORemark);
                                                      context.SaveChanges();
                                                  }
                                                  UpdateAsSuccess(WO_ID, VISIT_ID, AUDITOR, "Successfully");
                                                  return true;
                                              }
                                              else
                                              {
                                                  text = " - Can not approve, already approved by second level user..  ";
                                                  UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                                  return false;

                                              }
                                          }
                                          else
                                          {
                                              text = " - Can not approve, approval level is missing ..  ";
                                              UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                              return false;
                                          }
                                      }

                                      else
                                      {
                                          text = " - Unable to find any past approve/reject information..  ";
                                          UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                          return false;
                                      }
                                  }
                                  //CPH user//

                                  if (UserData[0].AUTHGROUP_NAME == "CPH") // Check if current user is CPH USER
                                  {
                                      var WoData = (from wo in context.MESC1TS_APPROVER_DETAILS
                                                    where wo.WO_ID == WO_ID
                                                    select wo).ToList();

                                      if (WoData == null || WoData.Count == 0)
                                      {
                                          MESC1TS_APPROVER_DETAILS newApproverDetail = new MESC1TS_APPROVER_DETAILS();

                                          newApproverDetail.WO_ID = WO_ID;
                                          newApproverDetail.STATUS = MainWoData[0].STATUS_CODE;
                                          newApproverDetail.APPROVAL_LEVEL = 1;
                                          newApproverDetail.CHUSER = MainWoData[0].CHUSER;
                                          newApproverDetail.CHTS = MainWoData[0].CHTS;

                                          context.MESC1TS_APPROVER_DETAILS.Add(newApproverDetail);
                                           context.SaveChanges();
                                      }
                                      WoData = (from wo in context.MESC1TS_APPROVER_DETAILS
                                                where wo.WO_ID == WO_ID
                                                select wo).ToList();

                                      if (WoData != null && WoData.Count > 0)
                                      {
                                          if (WoData[0].APPROVAL_LEVEL != null)
                                          {
                                              if (WoData[0].APPROVAL_LEVEL == 1 || WoData[0].APPROVAL_LEVEL == 2)
                                              {
                                                  WoData[0].STATUS = 390;
                                                  WoData[0].APPROVAL_LEVEL = 3;
                                                  WoData[0].CHUSER = CERTIFICATE_ID;
                                                  MainWoData[0].CHTS = DateTime.Now;
                                                  context.SaveChanges();
                                                  MainWoData[0].STATUS_CODE = 390;
                                                  MainWoData[0].CHTS = DateTime.Now;
                                                  MainWoData[0].CHUSER = CERTIFICATE_ID + "[Audit Tool]";
                                                  context.SaveChanges();

                                                  MESC1TS_WOREMARK WORemark = new MESC1TS_WOREMARK();
                                                  string Remark = "Approved By Audit Tool User[" + CERTIFICATE_ID + "]";
                                                  WORemark.WO_ID = WO_ID;
                                                  WORemark.CRTS = DateTime.Now;
                                                  WORemark.REMARK_TYPE = "S";
                                                  WORemark.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                                  WORemark.REMARK = Remark;
                                                  context.MESC1TS_WOREMARK.Add(WORemark);
                                                  context.SaveChanges();
                                                  if (!string.IsNullOrEmpty(COMMENT))
                                                  {
                                                      MESC1TS_WOREMARK WORemark1 = new MESC1TS_WOREMARK();
                                                      string Remark1 = COMMENT;
                                                      WORemark.WO_ID = WO_ID;
                                                      WORemark.CRTS = DateTime.Now;
                                                      WORemark.REMARK_TYPE = "E";  //Pinaki
                                                      WORemark.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                                      WORemark.REMARK = COMMENT;
                                                      context.MESC1TS_WOREMARK.Add(WORemark);
                                                      context.SaveChanges();
                                                  }
                                                  UpdateAsSuccess(WO_ID, VISIT_ID, AUDITOR, "Successfully");
                                                  return true;
                                              }
                                              else
                                              {
                                                  text = " - Can not approve, already approved by CPH user..  ";
                                                  UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                                  return false;

                                              }
                                          }
                                          else
                                          {
                                              text = " - Can not approve, approval level is missing ..  ";
                                              UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                              return false;
                                          }
                                      }

                                      else
                                      {
                                          text = " - Unable to find any past  approval/reject information..  ";
                                          UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                          return false;
                                      }
                                  }


                                  else
                                  {
                                      text = " The user is not belong to second level user or CPH user..  ";
                                      UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                      return false;
                                  }


                              }
                              else
                              {
                                  text = " The Work order is already in Rejected status..  ";
                                  UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "110");
                                  return false;
                              }

                          }

                          if (MainWoData[0].STATUS_CODE > 100 && MainWoData[0].STATUS_CODE < 390)
                          {
                              int STATUS_CODE = Convert.ToInt32(MainWoData[0].STATUS_CODE);
                              string AuthName = UserData[0].AUTHGROUP_NAME.ToString();


                              if (AUDIT_RESULT == 0) //asked to reject
                              {
                                  var appdata = (from app in context.WO_VALIDATION
                                                 where app.ROLES == AuthName && app.REJECT == 1 && app.WOSTATUS == STATUS_CODE
                                                 select app).ToList();
                                  if (appdata.Count > 0)
                                  {

                                      MainWoData[0].STATUS_CODE = 100;
                                      MainWoData[0].CHTS = DateTime.Now;
                                      MainWoData[0].CHUSER = CERTIFICATE_ID + "[Audit Tool]";
                                      int Status = context.SaveChanges();
                                      if (Status == 1)
                                      {
                                          MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
                                          string auditComment = "Work Order: " + WO_ID + " update on " + DateTime.Now + " by " + CERTIFICATE_ID + "_AuditTool as follows : WorkOrder has been rejected";
                                          WOAudit.WO_ID = WO_ID;
                                          WOAudit.CHTS = DateTime.Now;
                                          WOAudit.AUDIT_TEXT = auditComment;
                                          WOAudit.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                          context.MESC1TS_WOAUDIT.Add(WOAudit);
                                          context.SaveChanges();

                                          MESC1TS_WOREMARK WORemark = new MESC1TS_WOREMARK();
                                          string Remark = "Rejected By Audit Tool User[" + CERTIFICATE_ID + "]";
                                          WORemark.WO_ID = WO_ID;
                                          WORemark.CRTS = DateTime.Now;
                                          WORemark.REMARK_TYPE = "S";
                                          WORemark.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                          WORemark.REMARK = Remark;
                                          context.MESC1TS_WOREMARK.Add(WORemark);
                                          context.SaveChanges();
                                          if (!string.IsNullOrEmpty(COMMENT))
                                          {
                                              MESC1TS_WOREMARK WORemark1 = new MESC1TS_WOREMARK();
                                              string Remark1 = COMMENT;
                                              WORemark.WO_ID = WO_ID;
                                              WORemark.CRTS = DateTime.Now;
                                              WORemark.REMARK_TYPE = "E";  //Pinaki
                                              WORemark.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                              WORemark.REMARK = COMMENT;
                                              context.MESC1TS_WOREMARK.Add(WORemark);
                                              context.SaveChanges();
                                          }
                                          UpdateApproverDetails(WO_ID, 100, CERTIFICATE_ID);
                                          UpdateAsSuccess(WO_ID, VISIT_ID, AUDITOR, "Successfully");

                                          return true;
                                      }
                                      else
                                      {
                                          return false;
                                      }
                                  }
                                  else
                                  {
                                      //logEntry.Message = "Unable to reject in Merc Plus because the User - " + AuthName + " does not have permission to reject for WorkOrder status_code -   " + MainWoData[0].STATUS_CODE;
                                      //Logger.Write(logEntry);
                                      text = " - Unable to reject in Merc Plus because the User - " + AuthName + " does not have permission to reject for WorkOrder status_code -   " + MainWoData[0].STATUS_CODE;
                                      UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "105");
                                      return false;
                                  }
                              }
                              else if (AUDIT_RESULT == 1) //asked to approve
                              {
                                  var appdata = (from app in context.WO_VALIDATION
                                                 where app.ROLES == AuthName && app.ACCEPT == 1 && app.WOSTATUS == STATUS_CODE
                                                 select app).ToList();
                                  if (appdata.Count > 0)
                                  {
                                      // total loss validation...............
                                      String TL = "";
                                      String EQPNO = MainWoData[0].EQPNO;
                                      TL = CheckTotalLosscontainer(EQPNO, MainWoData[0].SHOP_CD, MainWoData[0].MODE, MainWoData[0].PRESENTLOC);

                                      if (TL.Length == 0)
                                      {
                                          //....................................

                                          MainWoData[0].STATUS_CODE = 390;
                                          MainWoData[0].CHTS = DateTime.Now;
                                          MainWoData[0].CHUSER = CERTIFICATE_ID + "[Audit Tool]";
                                          int Status = context.SaveChanges();
                                          if (Status == 1)
                                          {
                                              MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
                                              string auditComment = "Work Order: " + WO_ID + " update on " + DateTime.Now + " by " + CERTIFICATE_ID + "_AuditTool as follows : WorkOrder has been approved";
                                              WOAudit.WO_ID = WO_ID;
                                              WOAudit.CHTS = DateTime.Now;
                                              WOAudit.AUDIT_TEXT = auditComment;
                                              WOAudit.CHUSER = CERTIFICATE_ID + "[Audit Tool]";
                                              context.MESC1TS_WOAUDIT.Add(WOAudit);
                                              context.SaveChanges();

                                              MESC1TS_WOREMARK WORemark = new MESC1TS_WOREMARK();
                                              string Remark = "Approved By Audit Tool User[" + CERTIFICATE_ID + "]";
                                              WORemark.WO_ID = WO_ID;
                                              WORemark.CRTS = DateTime.Now;
                                              WORemark.REMARK_TYPE = "S";
                                              WORemark.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                              WORemark.REMARK = Remark;
                                              context.MESC1TS_WOREMARK.Add(WORemark);
                                              context.SaveChanges();
                                              if (!string.IsNullOrEmpty(COMMENT))
                                              {
                                                  MESC1TS_WOREMARK WORemark1 = new MESC1TS_WOREMARK();
                                                  string Remark1 = COMMENT;
                                                  WORemark.WO_ID = WO_ID;
                                                  WORemark.CRTS = DateTime.Now;
                                                  WORemark.REMARK_TYPE = "E";  //Pinaki
                                                  WORemark.CHUSER = CERTIFICATE_ID + "[AuditTool]";
                                                  WORemark.REMARK = COMMENT;
                                                  context.MESC1TS_WOREMARK.Add(WORemark);
                                                  context.SaveChanges();
                                              }
                                              UpdateApproverDetails(WO_ID, 390, CERTIFICATE_ID);
                                              UpdateAsSuccess(WO_ID, VISIT_ID, AUDITOR, "Successfully");
                                              return true;
                                          }
                                          else
                                          {
                                              return false;
                                          }
                                      } // TL

                                      //---------------------
                                      else
                                      {

                                          text = " - Unable to accept in Merc Plus because the Equipment - " + EQPNO + " have Some WorkOrder " + TL + " with status as Total Loss ";
                                          UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "106");
                                          return false;
                                      }
                                      //-----------------
                                  }

                                  else
                                  {
                                      // logEntry.Message = "Unable to accept in Merc Plus because the User - " + AuthName + " does not have permission to accept for WorkOrder status_code -   " + MainWoData[0].STATUS_CODE;
                                      // Logger.Write(logEntry);
                                      text = " - Unable to accept in Merc Plus because the User - " + AuthName + " does not have permission to accept for WorkOrder status_code -   " + MainWoData[0].STATUS_CODE;
                                      UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "106");
                                      return false;
                                  }
                              }
                              else
                              {
                                  text = " - the AUDIT_RESULT is  - " + AUDIT_RESULT;
                                  UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "107");
                                  return false;
                              }



                          }




                          else
                          {
                              if (MainWoData[0].STATUS_CODE > 390)
                              {
                                  text = " - Unable to process in Merc Plus because the current status is  " + MainWoData[0].STATUS_CODE;
                                  UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "108");
                                  return false;
                              }


                          }
                      }

                      else
                      {
                          return false;
                      }
                  }
                  else
                  {
                      return false;
                  }
              }
              catch (Exception ex)
              {
                  logEntry.Message = "Unable to process due to exception for WorkOrder ID -" + WO_ID + " - " + ex.Message;
                  Logger.Write(logEntry);
                  text = " - Unable to process due to exception  ";
                  UpdateAsFailed(WO_ID, VISIT_ID, AUDITOR, text, "112");
                  return false;

              }
              return true;
          }
        public void UpdateAsSuccess(int WO_ID, string VisitID, string auditor, string text)
        {

            context = new MESC2DSEntities();


            try
            {
                var AuditUpdate = (from audit in context.MESC1TS_AUDIT_HISTORY
                                   where audit.WO_ID == WO_ID && audit.AUDITOR == auditor && audit.VISIT_ID == VisitID
                                   select audit).ToList();
                if (AuditUpdate.Count > 0)
                {
                    AuditUpdate[0].MERC_STATUS = "SUCCESS";
                    AuditUpdate[0].MERC_COMMENTS = "WO_ID : " + WO_ID + " is processed  " + text;
                    context.SaveChanges();
                  
                }


            }
            catch (Exception ex)
            {
                logEntry.Message = "Unable to Update As success for Workorder ID -" + WO_ID + " - " + ex.Message;
                Logger.Write(logEntry);

            }

        }
        public string CheckTotalLosscontainer(string EqpNo, string shop, string mode, string loc)
        {
            String WO = "";
            var TLWO = (from wo in context.MESC1TS_WO
                        where wo.EQPNO == EqpNo && wo.STATUS_CODE == 150 && wo.PRESENTLOC == loc && (wo.MODE == "02" || wo.MODE == "44" || wo.MODE == "43" || wo.MODE == "45")
                        select wo).ToList();
            if (TLWO.Count > 0)
            {
                WO = TLWO[0].WO_ID.ToString();
            }
            //&& wo.CHTS> DateTime.Now.AddYears(-1)
            return WO;
        }
    }
}
        
    

