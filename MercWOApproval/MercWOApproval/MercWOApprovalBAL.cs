using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MercWOApprovalDAL;


namespace MercWOAppRej
{
    class MercWOApprovalBAL
    {
        public void StartProcessing()
        {
            string text="";
           
            List <MESC1TS_WO> WOList = new List <MESC1TS_WO>();
            List <MESC1TS_AUDIT_HISTORY>  Wlist = new    List <MESC1TS_AUDIT_HISTORY>();
            MercWOApprovalDAL.MercWOApprovalDAL objDAL = new MercWOApprovalDAL.MercWOApprovalDAL();
            Wlist = objDAL.FetchNewRecord();
            try
            {
                if (Wlist.Count > 0)
                {
                    foreach (var obj in Wlist)
                    {
                        MercWOAppRej.MercWoApproval.logEntry.Message = "MercWorkOrderUpdate start for WorkOrderId:" + obj.WO_ID;
                        Logger.Write(MercWOAppRej.MercWoApproval.logEntry);
                       
                            if (objDAL.UpdateInProgress(obj.WO_ID,obj.VISIT_ID,obj.AUDITOR))
                            {
                                if (!string.IsNullOrEmpty(obj.CERTIFICATE_ID))
                                {
                                    if (objDAL.ValidateUser(obj.CERTIFICATE_ID, obj.WO_ID,obj.VISIT_ID,obj.AUDITOR))
                                    {
                                       if(objDAL.ValidateShops(obj.WO_ID,obj.CERTIFICATE_ID,obj.VISIT_ID,obj.AUDITOR))
                                        {
                                            bool Status = objDAL.StartAcceptReject(obj.WO_ID, obj.CERTIFICATE_ID, obj.AUDITOR, obj.COMMENT, obj.VISIT_ID, obj.AUDIT_RESULT);
                                        }
                                    }
                                  
                                }
                                else
                                {
                                   // MercWOAppRej.MercWoApproval.logEntry.Message = "Certificate is blank for WorkOrderId:" + obj.WO_ID;
                                    //Logger.Write(MercWOAppRej.MercWoApproval.logEntry);
                                    text=" - CERTIFICATE_ID is blank";
                                    objDAL.UpdateAsFailed(obj.WO_ID,obj.VISIT_ID,obj.AUDITOR, text,"101");
                                }

                            }
                            //else
                            //{
                            //    MercWOAppRej.MercWoApproval.logEntry.Message = "Unable to update as InProgress for WorkOrderId:" + obj.WO_ID;
                            //    Logger.Write(MercWOAppRej.MercWoApproval.logEntry);
                            //}
                        
                        

                    }
                }
                //else
                //{
                //    MercWOAppRej.MercWoApproval.logEntry.Message = "No data availabe to update audit tool";
                //    Logger.Write(MercWOAppRej.MercWoApproval.logEntry);
                //}
               
            }
            catch (Exception ex)
            {
                MercWOAppRej.MercWoApproval.logEntry.Message = "MercWorkOrderUpdate ERROR:" + ex.Message;
                Logger.Write(MercWOAppRej.MercWoApproval.logEntry);
            }
        }
    }
}
