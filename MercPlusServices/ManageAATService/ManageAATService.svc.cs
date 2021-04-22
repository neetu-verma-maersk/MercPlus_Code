using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MercPlusLibrary;
using System.Data.SqlClient;
using System.Configuration;
using MercPlusServiceLibrary.BusinessObjects;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Data;
//using IBM.WMQ;
using System.Data.Entity.Validation;
using System.Threading;
//using System.Transactions;
using System.ServiceModel.Description;





namespace ManageAATService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ManageAATService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ManageAATService.svc or ManageAATService.svc.cs at the Solution Explorer and start debugging.
    public class ManageAATService : IManageAATService
    {

        ManageAATServiceEntities objContext;

        ManageAATService()
        {
            objContext = new ManageAATServiceEntities();
        }
        public void DoWork()
        {
            ManageAATService MAT = new ManageAATService();
        }


        ErrMessage Message = new ErrMessage();

        LogEntry logEntry = new LogEntry();
        //MESC2DSEntities objContext;
        public List<ErrMessage> ApproveWorkOrderOLD(int WOID, string User, string OldStatusOrRemark, string VendorRefNo)
        {
            MESC1TS_WO PrevWODataFromDB = new MESC1TS_WO();
            string LocCode = string.Empty;
            string EqpNO = string.Empty;
            string Present_Loc = string.Empty;
            int? PrevID = 0;
            string Date = string.Empty;
            short? StatusCodeTemp = 0;
            short? Status = 390;
            //var PrevWOID;
            MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
            MESC1TS_WOREMARK WORemark = new MESC1TS_WOREMARK();
            List<ErrMessage> errorMessageList = new List<ErrMessage>();
            try
            {
                //if (Status == APPROVEDSTATUS)
                //{

                string chUser = GetFormattedCHUSER(User);
                //pinaki Added

                string authGroupName = "EMR_APPROVER_COUNTRY";
                var user = objContext.SEC_USER.FirstOrDefault(x => x.LOGIN == chUser);
                if (user != null)
                {
                    var authGroupUser = objContext.SEC_AUTHGROUP_USER.FirstOrDefault(x => x.USER_ID == user.USER_ID);
                    if (authGroupUser != null)
                    {
                        var authGroup = objContext.SEC_AUTHGROUP.FirstOrDefault(x => x.AUTHGROUP_ID == authGroupUser.AUTHGROUP_ID);
                        authGroupName = authGroup.AUTHGROUP_NAME;
                    }
                }
                else
                {
                    Message = new ErrMessage();
                    Message.Message = "User " + chUser + " Not exist in MERC PLUS";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    return errorMessageList;
                }



                var PrevWOData = (from W in objContext.MESC1TS_WO
                                  where W.WO_ID == WOID
                                  select new
                                  {
                                      W.prev_wo_id,
                                      W.EQPNO,
                                      W.PRESENTLOC
                                  }).FirstOrDefault();

                if (PrevWOData != null)
                {
                    PrevID = PrevWOData.prev_wo_id;
                    EqpNO = PrevWOData.EQPNO;
                    Present_Loc = PrevWOData.PRESENTLOC;
                }
                else
                {
                    Message = new ErrMessage();
                    Message.Message = "WorkOrder  " + WOID + " Not exist in MERC PLUS";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    return errorMessageList;
                }


                if (PrevID == null || PrevID == 0)
                {
                    var PrevWOID = (from w1 in objContext.MESC1TS_WO
                                    join w2 in objContext.MESC1TS_WO on w1.EQPNO equals w2.EQPNO
                                    where w1.WO_ID == WOID &&
                                    w1.CRTS > w2.CRTS
                                    select new
                                    {
                                        w2.WO_ID,
                                        w2.CRTS
                                    }).OrderByDescending(w2 => w2.CRTS).FirstOrDefault();

                    if (PrevWOID != null) //if previous wo_id found, buffer it else change it to -1
                    {
                        PrevID = PrevWOID.WO_ID;
                    }
                    else
                    {
                        PrevID = -1;
                    }
                }
                if (PrevID == -1) //-1 for work orders without prev data
                {
                    LocCode = string.Empty;
                    Date = "9999-12-31";
                    StatusCodeTemp = null;
                }
                else
                {
                    var WOData = (from wo in objContext.MESC1TS_WO
                                  from s in objContext.MESC1TS_SHOP
                                  where wo.WO_ID == PrevID &&
                                  wo.SHOP_CD == s.SHOP_CD
                                  select new
                                  {
                                      wo.STATUS_CODE,
                                      wo.CHTS,
                                      s.LOC_CD
                                  }).ToList();

                    if (WOData != null && WOData.Count > 0)
                    {
                        LocCode = WOData[0].LOC_CD;
                        Date = WOData[0].CHTS.ToString();
                        StatusCodeTemp = WOData[0].STATUS_CODE;
                    }
                }

                var WOFromDB = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == WOID
                                select wo).FirstOrDefault();

                if (WOFromDB != null)
                {
                    if (390 > WOFromDB.STATUS_CODE)
                    {

                        int WorkOrderId = CheckWordOrdersTotalLosscontainer(EqpNO, Present_Loc);                                                                           //Total Loss-Udita
                        //"'"; rhsRecord.m_sPREV_STATUS += _bstr_t(cStatus); rhsRecord.m_sPREV_STATUS += "'";
                        if (WorkOrderId != 0)                                                                                                        //Total Loss-Udita
                        {
                            if (WOFromDB.MODE == "49" && authGroupName == "CPH")
                            {
                                WOFromDB.STATUS_CODE = 390;
                                WOFromDB.CHUSER = chUser;
                                WOFromDB.CHTS = DateTime.Now;
                                WOFromDB.APPROVAL_DTE = DateTime.Now;
                                WOFromDB.PREV_STATUS = Status;
                                if (!string.IsNullOrEmpty(Date))
                                    WOFromDB.PREV_DATE = Convert.ToDateTime(Date);
                                WOFromDB.PREV_LOC_CD = LocCode;
                                objContext.SaveChanges();

                                WOAudit.WO_ID = WOID;
                                WOAudit.CHUSER = chUser;
                                WOAudit.CHTS = DateTime.Now;
                                WOAudit.AUDIT_TEXT = "Approved by AAT Tool" + User; //Kasturee_Approve_workorder-13-08-19
                                objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                                objContext.SaveChanges();

                                WORemark.WO_ID = WOID;
                                WORemark.REMARK_TYPE = "S";
                                WORemark.SUSPCAT_ID = null;
                                WORemark.REMARK = OldStatusOrRemark;
                                WORemark.CHUSER = chUser;
                                WORemark.CRTS = DateTime.Now;

                                objContext.MESC1TS_WOREMARK.Add(WORemark);
                                objContext.SaveChanges();
                                UpdateApproverDetails(WOID, 390, chUser);
                            }
                            else
                            {
                                Message = new ErrMessage();
                                Message.Message = "WO " + WorkOrderId + " for this Equipment number is already in state of Total Loss:";                 //Total Loss-Udita
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                errorMessageList.Add(Message);
                            }

                        }
                        else
                        {
                            if (authGroupName == "CPH" || authGroupName == "EMR_SPECIALIST_COUNTRY" || authGroupName == "EMR_APPROVER_COUNTRY" || authGroupName == "EMR_SPECIALIST_SHOP" || authGroupName == "EMR_APPROVER_SHOP")
                            {
                                bool Access = false;
                                if (authGroupName == "CPH" && (WOFromDB.STATUS_CODE == 100 || WOFromDB.STATUS_CODE == 340))
                                    Access = true;
                                else if ((authGroupName == "EMR_SPECIALIST_COUNTRY" || authGroupName == "EMR_SPECIALIST_SHOP") && (WOFromDB.STATUS_CODE == 100 || WOFromDB.STATUS_CODE == 200 || WOFromDB.STATUS_CODE == 310 || WOFromDB.STATUS_CODE == 320 || WOFromDB.STATUS_CODE == 330))
                                    Access = true;

                                else if ((authGroupName == "EMR_APPROVER_COUNTRY" || authGroupName == "EMR_APPROVER_COUNTRY") && (WOFromDB.STATUS_CODE == 200 || WOFromDB.STATUS_CODE == 310))
                                    Access = true;
                                else
                                    Access = false;

                                if (Access == true)
                                {
                                    WOFromDB.STATUS_CODE = 390;
                                    WOFromDB.CHUSER = chUser;
                                    WOFromDB.CHTS = DateTime.Now;
                                    WOFromDB.APPROVAL_DTE = DateTime.Now;
                                    WOFromDB.PREV_STATUS = Status;
                                    if (!string.IsNullOrEmpty(Date))
                                        WOFromDB.PREV_DATE = Convert.ToDateTime(Date);
                                    WOFromDB.PREV_LOC_CD = LocCode;
                                    objContext.SaveChanges();

                                    WOAudit.WO_ID = WOID;
                                    WOAudit.CHUSER = chUser;
                                    WOAudit.CHTS = DateTime.Now;
                                    WOAudit.AUDIT_TEXT = "Approved by AAT Tool" + User;
                                    objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                                    objContext.SaveChanges();

                                    WORemark.WO_ID = WOID;
                                    WORemark.REMARK_TYPE = "S";
                                    WORemark.SUSPCAT_ID = null;
                                    WORemark.REMARK = OldStatusOrRemark;
                                    WORemark.CHUSER = chUser;
                                    WORemark.CRTS = DateTime.Now;
                                    objContext.MESC1TS_WOREMARK.Add(WORemark);
                                    objContext.SaveChanges();
                                    UpdateApproverDetails(WOID, 390, chUser);
                                }
                                else
                                {
                                    Message = new ErrMessage();
                                    Message.Message = "User Role :" + authGroupName + "doesnot have access to approve the Work orders Status ( " + WOFromDB.STATUS_CODE + " )";

                                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                    errorMessageList.Add(Message);
                                }
                            }
                            else
                            {
                                Message = new ErrMessage();
                                Message.Message = "User doesnot have access to approve the Work orders";                 //Total Loss-Udita
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                errorMessageList.Add(Message);
                            }

                        }
                    }
                    else
                    {
                        Message = new ErrMessage();
                        Message.Message = "Canot approve WorkOrder: " + WOID + " as it is already Approved";                 //Total Loss-Udita
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        errorMessageList.Add(Message);
                    }
                }
            }

            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "ERROR OCURED DURING APPROVAL TIME";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);//kasturee
                //success = false;
            }

            return errorMessageList;
        }
        private void UpdateApproverDetails(int workorderid, int status, string userdata)
        {
            try
            {
                int approverLevel = 0;
                DateTime currentDate = DateTime.Now;
                if (status == 100 || status == 130 || status == 390)
                {
                    approverLevel = 1;  // default level 1 user


                    var user = objContext.SEC_USER.FirstOrDefault(x => x.LOGIN == userdata);
                    if (user != null)
                    {
                        var authGroupUser = objContext.SEC_AUTHGROUP_USER.FirstOrDefault(x => x.USER_ID == user.USER_ID);
                        if (authGroupUser != null)
                        {
                            var authGroup = objContext.SEC_AUTHGROUP.FirstOrDefault(x => x.AUTHGROUP_ID == authGroupUser.AUTHGROUP_ID);
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
                        var approverDetail = objContext.MESC1TS_APPROVER_DETAILS.FirstOrDefault(x => x.WO_ID == workorderid);
                        if (approverDetail == null)
                        {
                            MESC1TS_APPROVER_DETAILS newApproverDetail = new MESC1TS_APPROVER_DETAILS();

                            newApproverDetail.WO_ID = workorderid;
                            newApproverDetail.STATUS = status;
                            newApproverDetail.APPROVAL_LEVEL = approverLevel;
                            newApproverDetail.CHUSER = userdata;
                            newApproverDetail.CHTS = currentDate;

                            objContext.MESC1TS_APPROVER_DETAILS.Add(newApproverDetail);
                        }
                        else
                        {
                            approverDetail.STATUS = status;
                            approverDetail.APPROVAL_LEVEL = approverLevel;
                            approverDetail.CHUSER = userdata;
                            approverDetail.CHTS = currentDate;
                        }

                        objContext.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }
        private string GetFormattedCHUSER(string user)
        {
            /*
             user="FIRSTNAME LASTNAME [CERTIFICATE#]"
             checks if the string is more than 31 characters. If it is not, save the string as it is. If it is more than 31 characters,                 save only the Last name and the Certificate Number in the CHUSER field like so "LASTNAME[CERTIFICATE#]". 
             If the last name itself contains a space, only the name after the space will be considered as lastname.             
           */

            string chUser = "";
            string lastName = "";
            string certificate = "";

            try
            {
                if (!String.IsNullOrEmpty(user))
                {
                    user = user.Trim();
                    if (user.Length > 31)
                    {

                        /*
                        * It is confirmed that this part - [Certificate#] will never have any spaces in the string.
                        * Based on this assumption, the below .Split(' ') works fine.
                        */
                        var arr = user.Split(' ');
                        if (arr.Length > 2)
                        {
                            lastName = arr[arr.Length - 2];
                            certificate = arr[arr.Length - 1];
                            chUser = lastName + certificate;
                            //in case the client validation of lastname max length limit fails. Then only take certificate.
                            chUser = (chUser.Length > 31 ? certificate : chUser);
                        }
                        else
                        /*in case there is no space between firstname and lastname and certificate#, 
                        *then only cerficate number will be taken.
                        *i.e. FIRSTNAMELASTNAME [3268534] OR FIRSTNAMELASTNAME[3268534] OR FIRSTNAME LASTNAME[3268534] then [3268534] is taken.
                        */
                        {
                            chUser = user.Substring(user.LastIndexOf('['), user.Length - user.LastIndexOf('['));
                        }

                    }
                    else
                    {
                        chUser = user;
                    }
                }
                else
                {
                    chUser = "";
                }
            }
            catch (Exception ex)
            {
                chUser = "";
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return chUser;

        }
        public int CheckWordOrdersTotalLosscontainer(string EqpNo, string Present_Loc)
        {
            //bool success = false;
            int WorkOrderId = 0;
            var TotalLossWOs = (from wo in objContext.MESC1TS_WO
                                where wo.EQPNO == EqpNo && wo.STATUS_CODE == 150
                                && (wo.MODE == "43" || wo.MODE == "44" || wo.MODE == "45" || wo.MODE == "02")
                                && wo.PRESENTLOC == Present_Loc
                                select wo).ToList();
            if (TotalLossWOs.Count > 0)
            {
                WorkOrderId = TotalLossWOs[0].WO_ID;
            }

            return WorkOrderId;
        }

        public string WorkOrderDeletion(int WOID, string User) //Debadrita Work order deletion from SWAT
        {
            string message = "";
            string chUser = "";
            bool bValid;

            string Deletion_Log = string.Empty;
            Deletion_Log = ConfigurationManager.AppSettings["Deletion_Log"].ToString();


            chUser = GetFormattedCHUSER(User);

            // WOID = 11138739;
            MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
            try
            {


                var wofromDB = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == WOID
                                select wo).FirstOrDefault();

                if (wofromDB != null)
                {

                    //if (wofromDB.STATUS_CODE > 390)
                    if (wofromDB.STATUS_CODE == 400 || wofromDB.STATUS_CODE == 600 || wofromDB.STATUS_CODE == 900 || wofromDB.STATUS_CODE == 9999 || wofromDB.STATUS_CODE == 390)
                    {

                        if (wofromDB.STATUS_CODE == 9999)
                        {
                            message = "Work Order:" + WOID + " is already deleted";
                            if (Deletion_Log == "Y")
                            {
                                logEntry.Message = message;
                                Logger.Write(logEntry);
                            }
                        }
                        else
                            switch (wofromDB.STATUS_CODE)
                            {
                                case 400: bValid = true; message = "Work Order:" + WOID + " could not be deleted due to WO status is Completed " + wofromDB.STATUS_CODE + "."; break;
                                case 600: bValid = true; message = "Work Order:" + WOID + " could not be deleted due to WO status is RRIS Accepted" + wofromDB.STATUS_CODE + "."; break;
                                case 900: bValid = true; message = "Work Order:" + WOID + " could not be deleted due to WO status is Processed" + wofromDB.STATUS_CODE + "."; break;
                                // case 500: bValid = true; message = "Work Order:" + WOID + " could not be deleted due to WO status is RRIS Transmitted" + wofromDB.STATUS_CODE + "."; break;
                                //case 800: bValid = true; message = "Work Order:" + WOID + " could not be deleted due to WO status is Paid" + wofromDB.STATUS_CODE + "."; break;
                                // case 550: bValid = true; message = "Work Order:" + WOID + " could not be deleted due to WO status is RRIS Rejected" + wofromDB.STATUS_CODE + "."; break;

                                default: bValid = false; message = "Work Order:" + WOID + " is having unknown status code" + wofromDB.STATUS_CODE; break;

                            }
                        //message = "Work Order:" + WOID + " could not be deleted due to WO status is either Completed/Working/RRIS Accepted/PROCESSED " + wofromDB.STATUS_CODE + ".";
                        //===========

                        if (wofromDB.STATUS_CODE == 390) //added based on new requirement of WORKING--if the status_code is 390, it will check for the SW, if it is 'Y' , estimate will not be deleted
                        {
                            //if (wofromDB.SHOP_WORKING_SW.Equals("Y"))

                            if (wofromDB.SHOP_WORKING_SW == null)
                            {

                                string AuditComment = "";

                                wofromDB.STATUS_CODE = 9999;  //STATUS CODE will be changed to 9999 in WO table
                                wofromDB.CHUSER = chUser;
                                wofromDB.CHTS = DateTime.Now;
                                objContext.SaveChanges();

                                WOAudit.WO_ID = WOID;
                                WOAudit.CHUSER = chUser;
                                WOAudit.CHTS = DateTime.Now;
                                AuditComment = "Work Order: " + WOID + " is deleted by " + chUser + " from SWAT ";
                                WOAudit.AUDIT_TEXT = AuditComment;
                                objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                                objContext.SaveChanges();
                                message = "Work Order:" + WOID + " is successfully deleted";
                                if (Deletion_Log == "Y")
                                {
                                    logEntry.Message = message;
                                    Logger.Write(logEntry);
                                }

                            }
                            else
                            {
                                if (wofromDB.SHOP_WORKING_SW.Equals("Y")) //Kasturee_Working_SW
                                    message = "Work Order: " + WOID + "could not be deleted due to WO status is Working" + wofromDB.STATUS_CODE + ".";
                                if (Deletion_Log == "Y")
                                {
                                    logEntry.Message = message;
                                    Logger.Write(logEntry);
                                }
                            }


                        }
                        //=========
                    }

                    else
                    {


                        string AuditComment = "";



                        wofromDB.STATUS_CODE = 9999;  //STATUS CODE will be changed to 9999 in WO table
                        wofromDB.CHUSER = chUser;
                        wofromDB.CHTS = DateTime.Now;
                        objContext.SaveChanges();

                        WOAudit.WO_ID = WOID;
                        WOAudit.CHUSER = chUser;
                        WOAudit.CHTS = DateTime.Now;
                        AuditComment = "Work Order: " + WOID + " is deleted by " + chUser + " from SWAT ";
                        WOAudit.AUDIT_TEXT = AuditComment;
                        objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                        objContext.SaveChanges();
                        message = "Work Order:" + WOID + " is successfully deleted";

                        if (Deletion_Log == "Y")
                        {
                            logEntry.Message = message;
                            Logger.Write(logEntry);
                        }
                    }

                }
                else
                {
                    message = "Work Order:" + WOID + " is invalid!!!";
                    if (Deletion_Log == "Y")
                    {
                        logEntry.Message = message;
                        Logger.Write(logEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                // Message = new ErrMessage();
                //Message.Message = "Not all selected estimates were approved; please try again, or approve them manually: <br>";
                //Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                //errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                //success = false;
            }
            return message;

        }

        // Warranty comments update from SWAT

        public string UpdateWarrantyComment(int wo_id, string comment)
        {
            string message = "";

            try
            {
                var wofromDB = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == wo_id
                                select wo).FirstOrDefault();


                if (wofromDB != null)
                {
                    if (comment != null || comment != "" && comment.Length <= 255)
                    {
                        if (wofromDB.STATUS_CODE == 200 || wofromDB.STATUS_CODE == 310 || wofromDB.STATUS_CODE == 320 || wofromDB.STATUS_CODE == 330)
                        {
                            //Insert in Remarks table
                            MESC1TS_WOREMARK WORemark = new MESC1TS_WOREMARK();
                            WORemark.WO_ID = wo_id;
                            WORemark.CHUSER = "Comment-SWAT";
                            // Remarks = "Work Order: " + wo_id + " is updated by " + "SWAT Warranty Commenter" + " from SWAT ";

                            WORemark.REMARK = comment;

                            WORemark.REMARK_TYPE = "E";
                            WORemark.CRTS = DateTime.Now;
                            WORemark.SUSPCAT_ID = null;
                            objContext.MESC1TS_WOREMARK.Add(WORemark);
                            objContext.SaveChanges();
                            message = "Work Order:" + wo_id + " is successfully  updated with warrenty comments from SWAT";

                            //Inset in Audit log  

                            MESC1TS_WOAUDIT WOAudit1 = new MESC1TS_WOAUDIT();
                            WOAudit1.WO_ID = wo_id;
                            WOAudit1.CHUSER = "Comment(SWAT)";
                            WOAudit1.CHTS = DateTime.Now;
                            WOAudit1.AUDIT_TEXT = "Warranty Comments Addes by SWAT";
                            objContext.MESC1TS_WOAUDIT.Add(WOAudit1);
                            objContext.SaveChanges();
                            message = "Warranty Comments added Successfully";
                        }

                        else
                        {
                            message = "WO Status  " + wofromDB.STATUS_CODE + " doesnot match the criteria";
                        }

                    }
                    else
                    {
                        message = "Length of comment is greater than 255 char or comment is null!!!";
                    }
                }
                else
                {
                    message = "Invalid work order: " + wo_id + " !";
                }

            }
            catch (Exception ex)
            {
                // Message = new ErrMessage();
                //Message.Message = "Not all selected estimates were approved; please try again, or approve them manually: <br>";
                //Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                //errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                //success = false;
            }
            return message;
        }

        // US 6334 Impementation - Start

        public string ApproveWorkOrder(int wo_id, string User_ID, string Remarks, string Remarks_Type, decimal totalcost)
        {
            string message = string.Empty;
            string exceptionMessage = string.Empty;
            short approveStatus = 390;
            string auditText = string.Empty;

            try
            {
                if (!string.IsNullOrWhiteSpace(Remarks) && Remarks.Length > 255)
                {
                    message = "Failure : Invalid Remark : Length of Remarks is greater than 255 characters";
                    throw new Exception(message);
                }

                var wofromDB = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == wo_id
                                select wo).FirstOrDefault();

                if (wofromDB == null)
                {
                    message = "Failure : Invalid work order : " + wo_id;
                    throw new Exception(message);
                }

                if (wofromDB.STATUS_CODE == null)
                {
                    message = "Failure : Invalid work order status code : empty";
                    throw new Exception(message);
                }

                if (wofromDB.STATUS_CODE > 390)
                {
                    message = "Failure : WO with Status  " + wofromDB.STATUS_CODE + " can not be modifed";
                    throw new Exception(message);
                }


                // if (wofromDB.TOT_COST_CPH != totalcost)
                if (Math.Round(wofromDB.TOT_COST_CPH == null ? 0 : wofromDB.TOT_COST_CPH.Value, 2) != Math.Round(totalcost, 2))
                {
                    message = "Failure : Total cost value is not matched with the work order : " + wo_id;
                    throw new Exception(message);
                }

                if (!IsUserEligilbleForWOUpdate(User_ID, wofromDB.STATUS_CODE.Value, WorkOrderAction.ACCEPT))
                {
                    message = "Failure : User is not eligible to approve the work order : user id : " + User_ID;
                    throw new Exception(message);
                }
                int WorkOrderId = CheckWordOrdersTotalLosscontainer(wofromDB.EQPNO, wofromDB.PRESENTLOC);   // Pinaki                                                                 

                if (WorkOrderId != 0)
                {
                    message = "Failure : WO " + wo_id + " for this Equipment number is already in state of Total Loss:";

                    throw new Exception(message);
                }
                // Update in Work Order table
                wofromDB.STATUS_CODE = approveStatus;
                wofromDB.CHUSER = User_ID;
                wofromDB.CHTS = DateTime.Now;
                wofromDB.APPROVAL_DTE = DateTime.Now;
                objContext.SaveChanges();

                // Add new entry in Remark table   
                if (!string.IsNullOrWhiteSpace(Remarks))
                {
                    string defaultRemarkType = "E";
                    string[] remarkTypeValues = { "E", "I", "B" };
                    string remarktype = string.IsNullOrWhiteSpace(Remarks_Type) ? defaultRemarkType :
                                        (remarkTypeValues.Contains(Remarks_Type.ToUpper()) ? Remarks_Type.ToUpper() : defaultRemarkType);
                    AddWorkOrderRemark(wo_id, User_ID, Remarks, Remarks_Type);
                }
                string systemRemark = "Approved By : " + User_ID;
                string systemRemarkType = "S";
                AddWorkOrderRemark(wo_id, User_ID, systemRemark, systemRemarkType);

                // Add new entry in Audit table
                auditText = "Wordk Order : " + wo_id + " is approved by SWAT user : " + User_ID;
                AddWorkOrderAudit(wo_id, User_ID, auditText);
                UpdateApproverDetails(wo_id, 390, User_ID);  //pinaki
                message = "Success : Work Order : " + wo_id + " is successfully approved by SWAT user : " + User_ID;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return message;
        }

        public string RejectWorkOrder(int wo_id, string User_ID, string Remarks, string Remarks_Type)
        {
            string message = string.Empty;
            string exceptionMessage = string.Empty;
            short rejectStatus = 100;
            string auditText = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(Remarks))
                {
                    message = "Failure : Invalid Remark : blank or null";
                    throw new Exception(message);
                }

                if (Remarks.Length > 255)
                {
                    message = "Failure : Invalid Remark : Length of Remarks is greater than 255 characters";
                    throw new Exception(message);
                }

                if (string.IsNullOrWhiteSpace(Remarks_Type))
                {
                    message = "Failure : Invalid Remark Type : blank or null";
                    throw new Exception(message);
                }

                if (Remarks_Type.ToUpper() != "E")
                {
                    message = "Failure : Invalid Remark Type : Remark type must be E";
                    throw new Exception(message);
                }

                var wofromDB = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == wo_id
                                select wo).FirstOrDefault();

                if (wofromDB == null)
                {
                    message = "Failure : Invalid work order : " + wo_id;
                    throw new Exception(message);
                }

                if (wofromDB.STATUS_CODE == null)
                {
                    message = "Failure : Invalid work order status code : empty";
                    throw new Exception(message);
                }

                if (wofromDB.STATUS_CODE > 390)
                {
                    message = "Failure : WO with Status  " + wofromDB.STATUS_CODE + " can not be modifed";
                    throw new Exception(message);
                }

                if (!IsUserEligilbleForWOUpdate(User_ID, wofromDB.STATUS_CODE.Value, WorkOrderAction.REJECT))
                {
                    message = "Failure : User is not eligible to reject the work order : user id : " + User_ID;
                    throw new Exception(message);
                }

                // Update in Work Order table
                wofromDB.STATUS_CODE = rejectStatus;
                wofromDB.CHUSER = User_ID;
                wofromDB.CHTS = DateTime.Now;
                objContext.SaveChanges();

                // Add new entry in Remark table                
                AddWorkOrderRemark(wo_id, User_ID, Remarks, Remarks_Type);
                string systemRemark = "Rejected By : " + User_ID;
                string systemRemarkType = "S";
                AddWorkOrderRemark(wo_id, User_ID, systemRemark, systemRemarkType);

                // Add new entry in Audit table
                auditText = "Work Order : " + wo_id + " is rejected by SWAT user : " + User_ID;
                AddWorkOrderAudit(wo_id, User_ID, auditText);

                message = "Success : Work Order : " + wo_id + " is successfully rejected by SWAT user : " + User_ID;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return message;
        }

        public string ForwardWorkOrder(int wo_id, string User_ID, string Remarks, string Remarks_Type, int StatusCode)
        {

            string message = string.Empty;
            string exceptionMessage = string.Empty;
            short forwardStatus = 0;
            short[] forwardStatusValues = { 200, 310, 320, 330, 340 };
            string auditText = string.Empty;

            try
            {
                if (!short.TryParse(StatusCode.ToString(), out forwardStatus))
                {
                    forwardStatus = 0;
                }

                if (!forwardStatusValues.Contains(forwardStatus))
                {
                    message = "Failure : Invalid status code : " + StatusCode;
                    throw new Exception(message);
                }

                if (string.IsNullOrWhiteSpace(Remarks))
                {
                    message = "Failure : Invalid Remark : blank or null";
                    throw new Exception(message);
                }

                if (Remarks.Length > 255)
                {
                    message = "Failure : Invalid Remark : Length of Remarks is greater than 255 characters";
                    throw new Exception(message);
                }

                if (string.IsNullOrWhiteSpace(Remarks_Type))
                {
                    message = "Failure : Invalid Remark Type : blank or null";
                    throw new Exception(message);
                }

                if (Remarks_Type.ToUpper() != "I")
                {
                    message = "Failure : Invalid Remark Type : Remark type must be I";
                    throw new Exception(message);
                }

                var wofromDB = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == wo_id
                                select wo).FirstOrDefault();

                if (wofromDB == null)
                {
                    message = "Failure : Invalid work order : " + wo_id;
                    throw new Exception(message);
                }

                if (wofromDB.STATUS_CODE == null)
                {
                    message = "Failure : Invalid work order status code : empty";
                    throw new Exception(message);
                }

                if (wofromDB.STATUS_CODE > 390)
                {
                    message = "Failure : WO with Status  " + wofromDB.STATUS_CODE + " can not be modifed";
                    throw new Exception(message);
                }

                if (!IsUserEligilbleForWOUpdate(User_ID, wofromDB.STATUS_CODE.Value, WorkOrderAction.FORWARD, forwardStatus))
                {
                    message = "Failure : User is not eligible to forward the work order : user id : " + User_ID;
                    throw new Exception(message);
                }

                // Update in Work Order table
                wofromDB.STATUS_CODE = forwardStatus;
                wofromDB.CHUSER = User_ID;
                wofromDB.CHTS = DateTime.Now;
                objContext.SaveChanges();

                // Add new entry in Remark table                
                AddWorkOrderRemark(wo_id, User_ID, Remarks, Remarks_Type);
                string systemRemark = "Forwarded By : " + User_ID;
                string systemRemarkType = "S";
                AddWorkOrderRemark(wo_id, User_ID, systemRemark, systemRemarkType);

                // Add new entry in Audit table
                auditText = "Work Order : " + wo_id + " is forwarded to " + forwardStatus + "by SWAT user : " + User_ID;
                AddWorkOrderAudit(wo_id, User_ID, auditText);

                message = "Success : Work Order : " + wo_id + " is successfully forwarded to " + forwardStatus + " by SWAT user : " + User_ID;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return message;
        }

        public string TotalLossWorkOrder(int wo_id, string User_ID, string Remarks, string Remarks_Type)
        {
            string message = string.Empty;
            string exceptionMessage = string.Empty;
            short totalLossStatus = 150;
            string auditText = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(Remarks))
                {
                    message = "Failure : Invalid Remark : blank or null";
                    throw new Exception(message);
                }

                if (Remarks.Length > 255)
                {
                    message = "Failure : Invalid Remark : Length of Remarks is greater than 255 characters";
                    throw new Exception(message);
                }

                if (string.IsNullOrWhiteSpace(Remarks_Type))
                {
                    message = "Failure : Invalid Remark Type : blank or null";
                    throw new Exception(message);
                }

                if (Remarks_Type.ToUpper() != "B")
                {
                    message = "Failure : Invalid Remark Type : Remark type must be B";
                    throw new Exception(message);
                }

                var wofromDB = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == wo_id
                                select wo).FirstOrDefault();

                if (wofromDB == null)
                {
                    message = "Failure : Invalid work order : " + wo_id;
                    throw new Exception(message);
                }

                if (wofromDB.STATUS_CODE == null)
                {
                    message = "Failure : Invalid work order status code : empty";
                    throw new Exception(message);
                }

                if (wofromDB.STATUS_CODE > 390)
                {
                    message = "Failure : WO with Status  " + wofromDB.STATUS_CODE + " can not be modifed";
                    throw new Exception(message);
                }

                if (!IsUserEligilbleForWOUpdate(User_ID, wofromDB.STATUS_CODE.Value, WorkOrderAction.TOTALLOSS))
                {
                    message = "Failure : User is not eligible to perform total loss for the work order : user id : " + User_ID;
                    throw new Exception(message);
                }

                // Update in Work Order table
                wofromDB.STATUS_CODE = totalLossStatus;
                wofromDB.CHUSER = User_ID;
                wofromDB.CHTS = DateTime.Now;
                objContext.SaveChanges();

                // Add new entry in Remark table               
                AddWorkOrderRemark(wo_id, User_ID, Remarks, Remarks_Type);
                string systemRemark = "Total loss done By : " + User_ID;
                string systemRemarkType = "S";
                AddWorkOrderRemark(wo_id, User_ID, systemRemark, systemRemarkType);

                // Add new entry in Audit table
                auditText = "Work Order : " + wo_id + " : Status is changed to total loss by SWAT user : " + User_ID;
                AddWorkOrderAudit(wo_id, User_ID, auditText);

                message = "Success : Work Order : " + wo_id + " : Status is successfully changed to total loss by SWAT user : " + User_ID;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return message;
        }
        public string UpdateWORemarks(int wo_id, string User_ID, string Remarks, string Remarks_Type)
        {
            string message = string.Empty;
             
            try
            {
                var wofromDB = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == wo_id
                                select wo).FirstOrDefault();


                if (wofromDB != null)
                {
                    if (Remarks != null || Remarks != "" && Remarks.Length <= 255)
                    {
                        if (Remarks_Type.ToUpper() == "E" || Remarks_Type.ToUpper() == "B" || Remarks_Type.ToUpper() == "I")
                        {

                            if (wofromDB.STATUS_CODE < 410)
                            {
                                //Insert in Remarks table
                                MESC1TS_WOREMARK WORemark = new MESC1TS_WOREMARK();
                                WORemark.WO_ID = wo_id;
                                WORemark.CHUSER = User_ID;
                                // Remarks = "Work Order: " + wo_id + " is updated by " + "SWAT Warranty Commenter" + " from SWAT ";

                                WORemark.REMARK = Remarks;
                                if (Remarks_Type == "E" || Remarks_Type == "e")
                                {
                                    WORemark.REMARK_TYPE = "E";
                                }
                                else if (Remarks_Type == "I" || Remarks_Type == "i")
                                {
                                    WORemark.REMARK_TYPE = "I";
                                }
                                else if (Remarks_Type == "B" || Remarks_Type == "b")
                                {
                                    WORemark.REMARK_TYPE = "I";
                                }


                                WORemark.CRTS = DateTime.Now;
                                WORemark.SUSPCAT_ID = null;
                                objContext.MESC1TS_WOREMARK.Add(WORemark);
                                objContext.SaveChanges();
                                message = "Success : Work Order:" + wo_id + " is successfully added with remarks from SWAT";


                                if (Remarks_Type == "B" || Remarks_Type == "b")
                                {
                                    WORemark.REMARK_TYPE = "E";
                                    objContext.MESC1TS_WOREMARK.Add(WORemark);
                                    objContext.SaveChanges();
                                    message = "Success : Work Order:" + wo_id + " is successfully added with remarks from SWAT";
                                }
                                //Inset in Audit log  

                                MESC1TS_WOAUDIT WOAudit1 = new MESC1TS_WOAUDIT();
                                WOAudit1.WO_ID = wo_id;
                                WOAudit1.CHUSER = User_ID  ;
                                WOAudit1.CHTS = DateTime.Now;
                                WOAudit1.AUDIT_TEXT = "Remarks Added by SWAT";
                                objContext.MESC1TS_WOAUDIT.Add(WOAudit1);
                                objContext.SaveChanges();
                                message = "Success : Remarks is successfully added in Work Order:" + wo_id + " by SWAT";
                            }


                            else
                            {
                                message = "Failure : WO Status  " + wofromDB.STATUS_CODE + " doesnot match the criteria";
                            }
                        }
                        else 
                        {
                             message = "Failure : Invalid Remark Type : Remark type must be E/I/B";
                            //throw new Exception(message);
                        }

                    }
                    else
                    {
                        message = "Failure : Length of Remarks is greater than 255 char or comment is null!!!";
                    }
                }
                else
                {
                    message = "Failure : Invalid work order: " + wo_id + " !";
                }

            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return message;
                //success = false;
            }

            return message;
        }


        #region Private Methods

        private string GetUserRoleById(string userid)
        {
            string userRole = string.Empty;

            try
            {
                var user = objContext.SEC_USER.FirstOrDefault(x => x.LOGIN == userid);

                if (user == null)
                {
                    throw new Exception("Failure : Invalid user id");
                }

                var authGroupUser = objContext.SEC_AUTHGROUP_USER.FirstOrDefault(x => x.USER_ID == user.USER_ID);
                if (authGroupUser != null)
                {
                    var authGroup = objContext.SEC_AUTHGROUP.FirstOrDefault(x => x.AUTHGROUP_ID == authGroupUser.AUTHGROUP_ID);
                    if (!string.IsNullOrEmpty(authGroup.AUTHGROUP_NAME))
                    {
                        userRole = authGroup.AUTHGROUP_NAME.ToUpper();
                    }
                    else
                    {
                        throw new Exception("Failure : User role not found");
                    }
                }
                else
                {
                    throw new Exception("Failure : User role not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return userRole;
        }

        private bool IsUserEligilbleForWOUpdate(string userid, short wocurrentstatus, WorkOrderAction workOrderAction, short statuscodeforupdate = 0)
        {
            bool isEligle = false;
            string userRole = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(userid))
                {
                    throw new Exception("Failure : User id is blank");
                }

                userRole = GetUserRoleById(userid);

                switch (userRole.ToUpper())
                {
                    case "CPH":
                        if (workOrderAction == WorkOrderAction.ACCEPT)
                        {
                            if (wocurrentstatus == 340)
                            {
                                isEligle = true;
                            }
                        }
                        else if (workOrderAction == WorkOrderAction.REJECT)
                        {
                            if (wocurrentstatus == 390)
                            {
                                isEligle = true;
                            }
                        }
                        else if (workOrderAction == WorkOrderAction.FORWARD)
                        {
                            if ((
                                wocurrentstatus == 200 &&
                                (statuscodeforupdate == 310 || statuscodeforupdate == 320 || statuscodeforupdate == 330 || statuscodeforupdate == 340)
                                )
                                ||
                                (
                                wocurrentstatus == 310 &&
                                (statuscodeforupdate == 200 || statuscodeforupdate == 320 || statuscodeforupdate == 330 || statuscodeforupdate == 340)
                                )
                                ||
                                (
                                wocurrentstatus == 320 &&
                                (statuscodeforupdate == 200 || statuscodeforupdate == 310 || statuscodeforupdate == 330 || statuscodeforupdate == 340)
                                )
                                ||
                                (
                                wocurrentstatus == 330 &&
                                (statuscodeforupdate == 200 || statuscodeforupdate == 310 || statuscodeforupdate == 320 || statuscodeforupdate == 340)
                                )
                                ||
                                (
                                wocurrentstatus == 340 &&
                                statuscodeforupdate == 200
                                )
                               )
                            {
                                isEligle = true;
                            }
                        }
                        else if (workOrderAction == WorkOrderAction.TOTALLOSS)
                        {
                            if (wocurrentstatus == 340)
                            {
                                isEligle = true;
                            }
                        }
                        break;
                    case "EMR_APPROVER_COUNTRY":
                        if (workOrderAction == WorkOrderAction.ACCEPT)
                        {
                            if (wocurrentstatus == 200 || wocurrentstatus == 310)
                            {
                                isEligle = true;
                            }
                        }
                        else if (workOrderAction == WorkOrderAction.REJECT)
                        {
                            if (wocurrentstatus == 200 || wocurrentstatus == 310 || wocurrentstatus == 320)
                            {
                                isEligle = true;
                            }
                        }
                        else if (workOrderAction == WorkOrderAction.FORWARD)
                        {
                            if ((
                                wocurrentstatus == 200 &&
                                (  statuscodeforupdate == 330 || statuscodeforupdate == 340)
                                )
                                ||
                                (
                                wocurrentstatus == 310 &&
                                (  statuscodeforupdate == 330 || statuscodeforupdate == 340)
                                )
                                ||
                                (
                                wocurrentstatus == 320 &&
                                ( statuscodeforupdate == 330 || statuscodeforupdate == 340)
                                )
                               )
                            {
                                isEligle = true;
                            }
                        }
                        break;
                    case "EMR_APPROVER_SHOP":
                        if (workOrderAction == WorkOrderAction.ACCEPT)
                        {
                            if (wocurrentstatus == 200 || wocurrentstatus == 310)
                            {
                                isEligle = true;
                            }
                        }
                        else if (workOrderAction == WorkOrderAction.REJECT)
                        {
                            if (wocurrentstatus == 200 || wocurrentstatus == 310 || wocurrentstatus == 320)
                            {
                                isEligle = true;
                            }
                        }
                        else if (workOrderAction == WorkOrderAction.FORWARD)
                        {
                            if ((
                                 wocurrentstatus == 200 &&
                                 (  statuscodeforupdate == 330 || statuscodeforupdate == 340)
                                 )
                                 ||
                                 (
                                 wocurrentstatus == 310 &&
                                 (  statuscodeforupdate == 330 || statuscodeforupdate == 340)
                                 )
                                 ||
                                (
                                wocurrentstatus == 320 &&
                                (statuscodeforupdate == 330 || statuscodeforupdate == 340)
                                )
                                )
                            {
                                isEligle = true;
                            }
                        }
                        break;
                    case "EMR_SPECIALIST_COUNTRY":
                        if (workOrderAction == WorkOrderAction.ACCEPT)
                        {
                            if (wocurrentstatus == 200 || wocurrentstatus == 310 || wocurrentstatus == 320
                                || wocurrentstatus == 330
                                )
                            {
                                isEligle = true;
                            }
                        }
                        else if (workOrderAction == WorkOrderAction.REJECT)
                        {
                            if (wocurrentstatus == 200 || wocurrentstatus == 310 || wocurrentstatus == 320
                                || wocurrentstatus == 330 || wocurrentstatus == 390
                                )
                            {
                                isEligle = true;
                            }
                        }
                        else if (workOrderAction == WorkOrderAction.FORWARD)
                        {
                            if ((
                                 wocurrentstatus == 200 &&
                                 (  statuscodeforupdate == 340)
                                 )
                                 ||
                                 (
                                 wocurrentstatus == 310 &&
                                 (  statuscodeforupdate == 340)
                                 )
                                 ||
                                 (
                                 wocurrentstatus == 320 &&
                                 (  statuscodeforupdate == 340)
                                 )
                                 ||
                                 (
                                 wocurrentstatus == 330 &&
                                 (statuscodeforupdate == 340)
                                 )
                                )
                            {
                                isEligle = true;
                            }
                        }
                        break;
                    case "EMR_SPECIALIST_SHOP":
                        if (workOrderAction == WorkOrderAction.ACCEPT)
                        {
                            if (wocurrentstatus == 200 || wocurrentstatus == 310
                                || wocurrentstatus == 320 || wocurrentstatus == 330
                                )
                            {
                                isEligle = true;
                            }
                        }
                        else if (workOrderAction == WorkOrderAction.REJECT)
                        {
                            if (wocurrentstatus == 200 || wocurrentstatus == 310
                                || wocurrentstatus == 320 || wocurrentstatus == 330 || wocurrentstatus == 390
                                )
                            {
                                isEligle = true;
                            }
                        }
                        else if (workOrderAction == WorkOrderAction.FORWARD)
                        {
                            if ((
                                 wocurrentstatus == 200 &&
                                 ( statuscodeforupdate == 340)
                                 )
                                 ||
                                 (
                                 wocurrentstatus == 310 &&
                                 (  statuscodeforupdate == 340)
                                 )
                                 ||
                                 (
                                 wocurrentstatus == 320 &&
                                 (  statuscodeforupdate == 340)
                                 )
                                 ||
                                 (
                                 wocurrentstatus == 330 &&
                                 (statuscodeforupdate == 340)
                                 )
                                )
                            {
                                isEligle = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isEligle;
        }

        private void AddWorkOrderRemark(int workorderid, string userid, string remark, string remarktype)
        {
            try
            {
                MESC1TS_WOREMARK WORemark = new MESC1TS_WOREMARK();
                WORemark.WO_ID = workorderid;
                WORemark.CHUSER = userid  ;
                WORemark.REMARK = remark;
                WORemark.CRTS = DateTime.Now;
                WORemark.SUSPCAT_ID = null;

                remarktype = remarktype.ToUpper();

                WORemark.REMARK_TYPE = remarktype;

                if (remarktype == "B")
                {
                    WORemark.REMARK_TYPE = "I";
                }

                objContext.MESC1TS_WOREMARK.Add(WORemark);
                objContext.SaveChanges();

                if (remarktype == "B")
                {
                    WORemark.REMARK_TYPE = "E";
                    objContext.MESC1TS_WOREMARK.Add(WORemark);
                    objContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                throw new Exception("Failure : Error while saving data for Remark");
            }
        }

        private void AddWorkOrderAudit(int workorderid, string userid, string audittext)
        {
            try
            {
                MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
                WOAudit.WO_ID = workorderid;
                WOAudit.CHUSER = userid + " (SWAT)";
                WOAudit.CHTS = DateTime.Now;
                WOAudit.AUDIT_TEXT = audittext;
                objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                objContext.SaveChanges();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                throw new Exception("Failure : Error while saving data for Audit");
            }
        }

        #endregion

    }

    enum WorkOrderAction
    {
        ACCEPT,
        REJECT,
        FORWARD,
        TOTALLOSS
    }
}
