using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MercPlusLibrary;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using IBM.WMQ;
using System.IO;
using System.Configuration;
using WokOrderLoadService.ManageWorkOrderServiceReference;
using System.ServiceModel;

namespace WokOrderLoadService
{
    public partial class WOLoad : ServiceBase
    {
        #region DeclareVariables
        private AutoResetEvent AutoEventInstance { get; set; }
        private WOLoad TimerInstance { get; set; }
        private Timer StateTimer { get; set; }
        public int TimerInterval { get; set; }
        Thread Worker;
        AutoResetEvent StopRequest = new AutoResetEvent(false);
        List<MercPlusLibrary.ErrMessage> ErrorMessageList = new List<MercPlusLibrary.ErrMessage>();
        List<string> EAPFormatList = new List<string>();
        const string MQ_READ_MODE = "R";
        const string MQ_WRITE_MODE = "W";
        const int HEADERREC = 0;
        const int REPAIRREC = 1;
        const int PARTSREC = 2;
        const int REMARKREC = 3;
        const int RECORDLEN = 83;
        const int CUSTOMERLEN = 4;
        const int MANUALCODELEN = 4;
        const int SHOPCODELEN = 3;
        const int DATELEN = 8;
        const int EQUIPNUMBERLEN = 11;
        const int EQUIPTYPELEN = 4;
        const int EQUIPSTYPELEN = 4;
        const int EQUIPSIZELEN = 2;
        const int REPAIRMODELEN = 2;
        const int CAUSEMODELEN = 1;
        const int LOCATIONCODELEN = 3;
        const int WOTYPELEN = 1;
        const int THIRDPARTYLEN = 3;
        //Header 2
        const int VENDORREFLEN = 36;
        const int STHOURSLEN = 4;
        const int OTHOURSLEN = 4;
        const int DTHOURSLEN = 4;
        const int MISHOURSLEN = 4;
        const int TOTAMOUNTLEN = 12;
        // Remarks
        const int REMARKLEN = 77;
        const int REMARKDBLEN = 255;
        const int REMARKUSERLEN = 20;
        // Repair record
        const int REPAIRCODELEN = 6;
        const int PIECESLEN = 3;
        const int MATERIALAMTLEN = 12;
        const int MANHOURLEN = 3;

        const int DAMAGECODELEN = 2;
        const int REPAIRLOCCODELEN = 4;
        const int TPICODELEN = 1;

        //Parts record
        const int PARTNUMBERLEN = 20;

        // Remarks record
        const int REMARKTYPELEN = 1;

        // Inspection Record
        const int RKEMLOCATIONLEN = 8;

        string QueueManager = null;
        string RequestQueue = null;
        string ReplyQueue = null;
        string Senderid = null;
        string receiverid = null;
        string aprfco = null;
        string aprferr = null;
        string EDILogFilePath = null;
        string WOMQMessageFile = null;
        int count = 0;
        List<string> WOFileRecList = new List<string>();
        FileStream WOLogFilefs;
        StreamWriter WOLogFile;
        static int nID = 0;
        static int nLineNo = 0;
        static int nWOQty = 0;
        static int nWOPassQty = 0;
        static int nWOFailQty = 0;
        static int nErrLineNo = 0;
        WorkOrderDetail WOD = null;
        MESC2DSEntities objContext = new MESC2DSEntities();
        LogEntry logEntry = new LogEntry();
        #endregion DeclareVariables

        public WOLoad()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Start the worker thread
            Worker = new Thread(WorkerThread);
            Worker.Start();
        }

        protected override void OnStop()
        {
            StopRequest.Set();
            Worker.Join();
            StateTimer.Dispose();
        }

        private void WorkerThread(object arg)
        {
            TimerInterval = Convert.ToInt32(ConfigurationManager.AppSettings["TimeInterval"]);
            AutoEventInstance = new AutoResetEvent(false);
            // Create the delegate that invokes methods for the timer.
            TimerCallback timerDelegate =
                new TimerCallback(StartOperation);

            // Create a timer that signals the delegate to invoke 
            // 1.CheckStatus immediately, 
            // 2.Wait until the job is finished,
            // 3.then wait 5 minutes before executing again. 
            // 4.Repeat from point 2.

            //Start Immediately but don't run again.
            StateTimer = new Timer(timerDelegate, AutoEventInstance, 0, Timeout.Infinite);
            while (StateTimer != null)
            {
                if (StopRequest.WaitOne(1000))
                    return;
                //Wait until the job is done
                AutoEventInstance.WaitOne();
                //Wait for 5 minutes before starting the job again.
                StateTimer.Change(TimerInterval, Timeout.Infinite);
            }

        }

        public void StartOperation(Object stateInfo)//this is function (delegate) where the actuall operation starts and executed after each time interval
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            LogEntry log = new LogEntry();
            WOLoad Load = new WOLoad();
            try
            {
                log.Message = "MercWOLoad started...";
                Logger.Write(log);
                WOLoad MercWOLoadEDI = new WOLoad();

                MercWOLoadEDI.ReadConfig();

                for (int increment = 0; increment < MercWOLoadEDI.count; increment++)
                {
                    nID = 0;
                    nWOQty = 0;
                    nWOPassQty = 0;
                    nWOFailQty = 0;
                    nLineNo = 0;
                    nErrLineNo = 0;
                    if (MercWOLoadEDI.ExtractWorkOrders())
                    {
                        MercWOLoadEDI.ProcessWorkorders();
                    }
                    else
                    {
                        if (WOFileRecList.Count == 0)
                        {
                            break;
                        }
                        //Write in application log file - Failed to extract and parse MQ message 
                        log.Message = "Failed to extract and parse MQ message ";
                        Logger.Write(log);
                    }

                    Load.UpdateTransmission(nID, nWOQty, nWOPassQty, nWOFailQty);
                    MercWOLoadEDI.WOLogFile.Close();
                    MercWOLoadEDI.WOLogFilefs.Close();
                }
            }
            catch (Exception ex)
            {
                //Write in application log file - Unknown error while processing work orders
                log.Message = "Unknown error while processing work orders";
                Logger.Write(log);
            }
            autoEvent.Set();
        }

        string GenerateWOLogFilePathName()
        {
            try
            {

                string WOLogFilePathName = EDILogFilePath;
                DateTime CurrentDateTime = DateTime.Now;

                if (WOLogFilePathName.LastIndexOf("\\") != WOLogFilePathName.Length - 1)
                    WOLogFilePathName += "\\";

                WOLogFilePathName += CurrentDateTime.ToString("dd-MM-yyyy");

                bool Exists = System.IO.Directory.Exists(WOLogFilePathName);
                if (!Exists)
                    Directory.CreateDirectory(WOLogFilePathName);

                WOLogFilePathName += "\\WOLoad-";
                WOLogFilePathName += CurrentDateTime.ToString("hh-mm-ss");
                WOLogFilePathName += ".txt";

                return WOLogFilePathName;

            }
            catch (Exception ex)
            {
                //Application logEntry -  Error generating the EDILogFile
                logEntry.Message = "Unknown error while processing work orders";
                Logger.Write(logEntry);
                return null;
            }

        }

        bool ExtractWorkOrders()
        {
            nID = 0;
            nWOQty = 0;
            nWOPassQty = 0;
            nWOFailQty = 0;
            nLineNo = 0;
            nErrLineNo = 0;

            try
            {

                ProcessQueue();
                if (WOFileRecList.Count == 0)
                {
                    logEntry.Message = "No records found in MQ";
                    Logger.Write(logEntry);
                    return false;
                    //Write in application logEntry file - No records found in MQ
                    //return false;
                }
                //else
                //{*/
                //create the WO logEntry file and print the WO records in the file in correct format
                string WOFileName = GenerateWOLogFilePathName();
                WOLogFilefs = File.Open(WOFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                WOLogFile = new System.IO.StreamWriter(WOLogFilefs);
                if (WOFileName == null)
                {
                    return false;
                }

                // }

                using (FileStream WOFilefs = File.Open(WOMQMessageFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (System.IO.StreamWriter WOfile = new System.IO.StreamWriter(WOFilefs))
                    {
                        for (int i = 0; i < WOFileRecList.Count; i++)
                        {
                            WOfile.WriteLine(WOFileRecList[i]);
                        }
                        WOfile.Close();
                    }
                    WOFilefs.Close();
                }

                //WOLogFilefs = File.Open(WOFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                //StreamWriter WOLogFile = new System.IO.StreamWriter(WOLogFilefs);
                WOLogFile.WriteLine("File found");

                for (int i = 0; i < WOFileRecList.Count; i++)
                {
                    WOLogFile.WriteLine(WOFileRecList[i]);
                }

                WOLogFile.WriteLine("Extract Work order completed.");

                if (WOFileRecList.Count > 0)
                {
                    WOLogFile.WriteLine("Start EDI log header");
                    nID = InsertNewTransmission();
                    WOLogFile.WriteLine(nID);
                    WOLogFile.WriteLine("End EDI log header");

                }
                WOLogFile.WriteLine("Extracting workorder completed.");
            }
            catch (Exception ex)
            {
                logEntry.Message = "Failed to extract MQ message and write in file";
                Logger.Write(logEntry);
                return false;
                //Write in application logEntry file - Failed to extract MQ message and write in file
            }


            return true;
        }

        public void ProcessQueue() //read messages from MQ and write in file same as ExtractFile
        {
            MQManager MQmgr = new MQManager();
            MQQueueManager Qmgr = null;
            MQQueue Qqueue = null;
            MQMessage MQMessage = null;

            Qmgr = MQmgr.OpenQueueManager(QueueManager);
            Qqueue = MQmgr.OpenQ(MQ_READ_MODE, Qmgr, ReplyQueue);
            WOFileRecList = new List<string>();

            try
            {
                MQMessage = new MQMessage();
                MQmgr.GetMessage(Qqueue, ref MQMessage, false);
                if (MQMessage.Format.CompareTo(MQC.MQFMT_STRING) == 0)
                {
                    AddLines(MQMessage.ReadString(MQMessage.MessageLength));
                }
                else
                {
                    logEntry.Message = "The MQ message is a Non-text message" + MQMessage.MessageSequenceNumber.ToString();
                    Logger.Write(logEntry);
                    // Error Message - The MQ message is a Non-text message + mqMessage.MessageSequenceNumber.ToString()
                }
            }
            catch (MQException mqException)
            {
                if (mqException.Reason != MQC.MQRC_NO_MSG_AVAILABLE)
                {
                    //logEntry.Message = "A problem occured while retrieving the MQ message: " + mqException.Message.ToString();
                    //Logger.Write(logEntry);
                    //Write logEntry - A problem occured while retrieving the MQ message: " + mqException.Message.ToString()  
                    logEntry.Message = mqException.ToString();
                    Logger.Write(logEntry);
                }
            }

            MQmgr.CloseQ(Qqueue);
            MQmgr.DisconnectQueueManager(Qmgr);
        }

        private void SaveWorkOrder(WorkOrderDetail WODetail)
        {
            bool success = true;
            Equipment Eqp = new Equipment();
            Eqp.EquipmentNo = WOD.EquipmentList[0].EquipmentNo;
            Eqp.VendorRefNo = WOD.EquipmentList[0].VendorRefNo;
            WODetail.ChangeUser = "MercWOLoad";
            ManageWorkOrderClient WorkOrderClient = new ManageWorkOrderClient();

            try
            {
                WOLogFile.WriteLine("Work order validation started.");
                success = WorkOrderClient.CallValidateMethod(ref WOD, Eqp, out ErrorMessageList); //@Fixbysoumik
                WOLogFile.WriteLine("Work order validation completed.");
                Logger.Write(logEntry);
                //ErrorMessageList = WorkOrderClient.Review(ref WOD, WOD.EquipmentList, false).ToList(); //@Fixbysoumik
                ErrorMessageList = ErrorMessageList.FindAll(err => err.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                if (success) // && ErrorMessageList.Count == 0)
                {
                    WOLogFile.WriteLine("Saving Work order started.");
                    success = WorkOrderClient.CallSaveMethod(WOD, out ErrorMessageList); //@Fixbysoumik
                    WOLogFile.WriteLine("Saving Work order completed.");
                    Logger.Write(logEntry);
                    ErrorMessageList = ErrorMessageList.FindAll(err => err.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                    if (success)
                    {
                        nWOPassQty++;
                    }
                    else
                    {
                        nWOFailQty++;
                        SendErrorToEnterprise();
                    }
                }
                else
                {
                    nWOFailQty++;
                    SendErrorToEnterprise();
                    CloseConnection(WorkOrderClient);
                    return;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = "Unknown error while saving work orders " + ex.ToString();
                Logger.Write(logEntry);
            }
            CloseConnection(WorkOrderClient);
            try
            {
                if (WorkOrderClient != null)
                {
                    logEntry.Message = "WorkOrderClient State: " + WorkOrderClient.State.ToString();
                    Logger.Write(logEntry);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.Message.ToString();
                Logger.Write(logEntry);
            }
        }

        private static void CloseConnection(ManageWorkOrderClient WorkOrderClient)
        {
            try
            {
                if (WorkOrderClient.State != CommunicationState.Faulted)
                {
                    WorkOrderClient.Close();
                }
            }
            finally
            {
                if (WorkOrderClient.State != CommunicationState.Closed)
                {
                    WorkOrderClient.Abort();
                }
            }
        }

        void SendErrorToEnterprise()
        {
            bool bFound = false;
            int nEDIRtn = 0;

            try
            {
                WOLogFile.WriteLine("Prepare Enterprise Format started");
                PrepareEnterpriseFormat(WOD, ErrorMessageList); //@Fixbysoumik
                WOLogFile.WriteLine("Prepare Enterprise Format completed");
                WOLogFile.WriteLine("Getting Error");
                Logger.Write(logEntry);

                for (int i = 0; i < EAPFormatList.Count; i++)
                {
                    string tmp = EAPFormatList[i];

                    // log if we have a valid EDI ID.
                    if (nID > 0)
                    {
                        InsertError(nID, nErrLineNo++, tmp);
                    }

                    string tmp1 = tmp.Substring(0, 3);
                    if (tmp1 == "ERR")
                    {
                        if (tmp.Contains("System Error"))
                            bFound = true;
                    }
                }
                EAPFormatList = new List<string>();

                string mqMessage = string.Empty;
                // if System errors found, send back to enterprise. for later re-processing.
                if (bFound)
                {
                    WOLogFile.WriteLine("System errors found, send back to enterprise. for later re-processing.");
                    // mark as sent back to enterprise for reprocess
                    if (nID > 0)
                    {
                        InsertError(nID, nErrLineNo++, "ABOVE ESTIMATE SENT TO QUEUE FOR RE-PROSSESING - contains system error(s)");
                    }

                    //@Soumik  
                    WOLogFile.WriteLine("format again and send to Enterprise");
                    PrepareEnterpriseFormat(WOD, ErrorMessageList);

                    for (int i = 0; i < EAPFormatList.Count; i++)
                    {
                        string tmp = EAPFormatList[i];
                        mqMessage += tmp + "\n";
                    }

                    WOLogFile.WriteLine("System error sending file started");
                    SendFileToMQ(mqMessage);
                    WOLogFile.WriteLine("System error sending file end");
                    System.Threading.Thread.Sleep(500);
                    //InsertError(nID, nErrLineNo++, "ABOVE ESTIMATE SENT TO QUEUE FOR RETURN TO SENDER - contains validation error(s)");
                    return; //check with bishnu if bool needs to return
                }

                WOLogFile.WriteLine("Format again and send to Enterprise.");
                PrepareEnterpriseFormat(WOD, ErrorMessageList);
                // pass formatted records to EAP Link server.
                for (int i = 0; i < EAPFormatList.Count; i++)
                {
                    string tmp = EAPFormatList[i];
                    mqMessage += tmp + "\n";
                }

                // finally, send error file to Enterprise server.
                //printf("Sending errors to Enterprise");
                WOLogFile.WriteLine("Sending file started.");
                SendFileToMQ(mqMessage);
                WOLogFile.WriteLine("Sending file completed.");


                // need to log message sent to enterprise
                WOLogFile.WriteLine("Need to log message sent to enterprise.");
                InsertError(nID, nErrLineNo++, "ABOVE ESTIMATE SENT TO QUEUE FOR RETURN TO SENDER - contains validation error(s)");

                // Found pause necessary in batch mode with EAP link.  
                // Otherwise see failures on sending files to EAP.
                System.Threading.Thread.Sleep(500);


                // Get return acknowledgement

                WOLogFile.WriteLine("Getting return acknowledgement");

                //// Check if transmission failed needs to be implemented or not
                //if (tmp.IsEmpty())
                //{	// Save to error list for later transmission attempts.
                //    pWorkOrder->MoveFirstEAPLine();
                //    for (i = 0; i < iRtn; i++) m_aFailedSend.Add((char*)pWorkOrder->GetNextEAPLine());

                //    // Write possible error to system application log
                //    t = pEAPLink->GetError();
                //    fLogFile << " Write possible error to system application log" << endl;
                //    fLogFile << (char*)t << endl;
                //    if (t.length() > 0)
                //        pLog->WriteApplicationLog(PROGRAMNAME, t);
                //}
            }
            catch (Exception ex)
            {
                logEntry.Message = "Unknown error while saving work orders" + ex.ToString();
                Logger.Write(logEntry);
            }
        }

        void SendFileToMQ(string mqMessage)
        {

            MQManager MQmgr = new MQManager();
            MQQueueManager Qmgr = null;
            MQQueue Qqueue = null;
            MQPutMessageOptions putOptions = new MQPutMessageOptions();
            MQMessage msg = new MQMessage();
            string data = "AP10";

            try
            {
                putOptions.Options = MQC.MQPMO_SET_ALL_CONTEXT;
                msg.MessageId = MQC.MQCI_NONE;
                msg.CorrelationId = MQC.MQCI_NONE;
                msg.Format = MQC.MQFMT_STRING;
                msg.ApplicationOriginData = data;
                msg.MessageType = MQC.MQMT_DATAGRAM;
                msg.Priority = MQC.MQPRI_PRIORITY_AS_Q_DEF;
                msg.Expiry = MQC.MQEI_UNLIMITED;
                msg.Encoding = MQC.MQENC_NATIVE;
                msg.Persistence = MQC.MQPER_PERSISTENT;
                msg.ReplyToQueueName = Senderid;
                msg.ApplicationIdData = receiverid;
                msg.PutApplicationName = aprferr;
                msg.WriteString(mqMessage);
                Qmgr = MQmgr.OpenQueueManager(QueueManager);
                Qqueue = MQmgr.OpenRemoteQueueWithContext(Qmgr, RequestQueue);

                MQmgr.PutMessageWithOptions(Qqueue, msg, putOptions);
                MQmgr.CloseQ(Qqueue);
                MQmgr.DisconnectQueueManager(Qmgr);
            }
            catch (MQException mqException)
            {
                logEntry.Message = "A problem occured while putting the MQ message: " + mqException.Message.ToString();
                Logger.Write(logEntry);
            }
        }

        void PrepareEnterpriseFormat(WorkOrderDetail WorkOrderDetail, List<ErrMessage> ErrormessageList)
        {

            string sLine = string.Empty;
            string s1 = string.Empty;
            string s2 = string.Empty;
            string s3 = string.Empty;
            int k;
            int i = 0;

            // Create ERR records containing detailed error information for 
            // followed immediately by the HD1 record. Note: Prefix for all actual data is 'DAT' which is prefixed to
            // each record header type.
            // Later, there may be additional space added to accomodate larger comments/error records.
            try
            {
                for (i = 0; i < ErrormessageList.Count(); i++)
                {
                    sLine = ErrormessageList[i].Message;		// iterate thru error collection

                    // split message into multiple if too long, i.e. > 77 bytes 
                    k = SplitMessage(sLine, out s1, out s2, out s3);
                    if (s1.Length > 0)
                    {
                        sLine = s1;
                        sLine = sLine.PadRight(RECORDLEN, ' ');			// pad to 80
                        // remove ':' from line and replace with '-', colon is causing parsing issue in Enterprise maps... 
                        sLine.Replace(':', '-');
                        EAPFormatList.Add(sLine);
                    }

                    if (s2.Length > 0)
                    {
                        sLine = s2;
                        sLine = sLine.PadRight(RECORDLEN, ' ');			// pad to 80
                        // remove ':' from line and replace with '-', colon is causing parsing issue in Enterprise maps... 
                        sLine.Replace(':', '-');
                        EAPFormatList.Add(sLine);
                    }

                    if (s3.Length > 0)
                    {
                        sLine = s3;
                        sLine = sLine.PadRight(RECORDLEN, ' ');		// pad to 80
                        // remove ':' from line and replace with '-', colon is causing parsing issue in Enterprise maps... 
                        sLine.Replace(':', '-');
                        EAPFormatList.Add(sLine);
                    }

                    //		sLine = "ERR";
                    //		sLine+= m_ErrorList.GetAt( i ).c_str();			// iterate thru error collection
                    //		sLine= PadString( sLine, RECORDLEN );			// pad to 80
                    // remove ':' from line and replace with '-', colon is causing parsing issue in Enterprise maps... 
                    //		sLine.Replace(':', '-');
                    //		m_EAPFormatList.Insert( (LPCTSTR)sLine );
                }

                // Continue with header record. Must be part of first line!!!  GEISS issue????
                // Maps not working if HD1 record is on its own line. 
                DateTime tempRepCode = Convert.ToDateTime(WorkOrderDetail.RepairDate);
                sLine = "RECHD1";
                sLine += WorkOrderDetail.Shop.Customer[0].CustomerCode.PadRight(CUSTOMERLEN, ' ');
                sLine += WorkOrderDetail.Shop.ShopCode.PadRight(SHOPCODELEN, ' ');
                sLine += tempRepCode.ToString("dd-MM-yy").PadLeft(DATELEN, '0');
                if (WorkOrderDetail.EquipmentList != null && WorkOrderDetail.EquipmentList.Count > 0)
                {
                    sLine += WorkOrderDetail.EquipmentList[0].EquipmentNo.PadRight(EQUIPNUMBERLEN, ' ');
                }
                sLine += WorkOrderDetail.Mode.PadLeft(REPAIRMODELEN, '0');
                sLine += WorkOrderDetail.Cause.PadLeft(CAUSEMODELEN, '0');
                sLine += WorkOrderDetail.ThirdPartyPort.PadRight(THIRDPARTYLEN, ' ');
                sLine += WorkOrderDetail.WorkOrderType.PadLeft(WOTYPELEN, '0');
                sLine = sLine.PadRight(RECORDLEN, ' ');
                EAPFormatList.Add(sLine);
                sLine = "DATHD2";
                // ticket 10931 - return vendor-reference number.
                //	sLine+= ZeroPadLeft( WorkOrderDetail.VENDOR_CD.c_str(), VENDORREFLEN );datet
                if (WorkOrderDetail.EquipmentList != null && WorkOrderDetail.EquipmentList.Count > 0)
                {
                    if (!string.IsNullOrEmpty(WorkOrderDetail.EquipmentList[0].VendorRefNo))
                        sLine += WorkOrderDetail.EquipmentList[0].VendorRefNo.PadRight(VENDORREFLEN, ' ');
                }
                sLine += RemoveDecimal(WorkOrderDetail.TotalManHourReg.ToString().PadLeft(STHOURSLEN, '0'));
                sLine += RemoveDecimal(WorkOrderDetail.TotalManHourOverTime.ToString().PadLeft(OTHOURSLEN, '0'));
                sLine += RemoveDecimal(WorkOrderDetail.TotalManHourDoubleTime.ToString().PadLeft(DTHOURSLEN, '0'));
                sLine += RemoveDecimal(WorkOrderDetail.TotalManHourMisc.ToString().PadLeft(MISHOURSLEN, '0'));
                sLine += RemoveDecimal(WorkOrderDetail.TotalLabourCost.ToString().PadLeft(TOTAMOUNTLEN, '0'));
                sLine = sLine.PadRight(RECORDLEN, ' ');
                EAPFormatList.Add(sLine);

                // Add remarks from remarks recordlist if any (mostly from status settings and EDI batch)

                //====================================Kasturee 21-02-18 start=================================================
                if (WorkOrderDetail.RemarksList != null)
                {
                    for (i = 0; i < WorkOrderDetail.RemarksList.Count(); i++)
                    {
                        sLine = "DATRMK";
                        if (WorkOrderDetail.RemarksList[i].Remark.Length <= 77)
                        {
                            sLine += WorkOrderDetail.RemarksList[i].Remark;
                            sLine = sLine.PadRight(RECORDLEN, ' ');
                            EAPFormatList.Add(sLine);
                        }
                        else
                        {
                            if (WorkOrderDetail.RemarksList[i].Remark.Length <= 154)
                            {
                                string rmk1 = WorkOrderDetail.RemarksList[i].Remark.Substring(0, 77);
                                sLine += rmk1;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);
                                string rmk2 = WorkOrderDetail.RemarksList[i].Remark.Substring(77, 77);   //---------EDIError Kasturee
                                sLine = "DATRMK";
                                sLine += rmk2;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);
                            }
                            else if (WorkOrderDetail.RemarksList[i].Remark.Length <= 231 && WorkOrderDetail.RemarksList[i].Remark.Length > 154)
                            {
                                string rmk1 = WorkOrderDetail.RemarksList[i].Remark.Substring(0, 77);
                                sLine += rmk1;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);
                                string rmk2 = WorkOrderDetail.RemarksList[i].Remark.Substring(77, 77);
                                sLine = "DATRMK";                                           //---------EDIError Kasturee
                                sLine += rmk2;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);
                                string rmk3 = WorkOrderDetail.RemarksList[i].Remark.Substring(154, 77);
                                sLine = "DATRMK";
                                sLine += rmk3;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);
                            }

                            else if (WorkOrderDetail.RemarksList[i].Remark.Length <= 255 && WorkOrderDetail.RemarksList[i].Remark.Length > 231)
                            {
                                string rmk1 = WorkOrderDetail.RemarksList[i].Remark.Substring(0, 77);
                                sLine += rmk1;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);
                                string rmk2 = WorkOrderDetail.RemarksList[i].Remark.Substring(77, 77);//---------EDIError Kasturee
                                sLine = "DATRMK";
                                sLine += rmk2;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);
                                string rmk3 = WorkOrderDetail.RemarksList[i].Remark.Substring(154, 77);
                                sLine = "DATRMK";
                                sLine += rmk3;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);

                                string rmk4 = WorkOrderDetail.RemarksList[i].Remark.Substring(231);
                                sLine = "DATRMK";
                                sLine += rmk4;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);

                            }

                            else
                            {
                                string rmk1 = WorkOrderDetail.RemarksList[i].Remark.Substring(0, 77);
                                sLine += rmk1;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);
                                string rmk2 = WorkOrderDetail.RemarksList[i].Remark.Substring(77, 77);
                                sLine = "DATRMK";
                                sLine += rmk2;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);
                                string rmk3 = WorkOrderDetail.RemarksList[i].Remark.Substring(154, 77);
                                sLine = "DATRMK";
                                sLine += rmk3;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);

                                string rmk4 = " More than 255 char";
                                sLine = "DATRMK";
                                sLine += rmk4;
                                sLine = sLine.PadRight(RECORDLEN, ' ');
                                EAPFormatList.Add(sLine);
                            }



                        }
                    }
                }
                //sLine += WorkOrderDetail.RemarksList[i].Remark;
                //sLine = sLine.PadRight(RECORDLEN, ' ');
                //EAPFormatList.Add(sLine);
                //=========================================Kasturee 20-02-18 End===================================
                //if (WorkOrderDetail.RemarksList != null)
                //{
                //    for (i = 0; i < WorkOrderDetail.RemarksList.Count(); i++)
                //    {
                //        sLine = "DATRMK";
                //        if (WorkOrderDetail.RemarksList[i].Remark.Length <= 77)
                //        {
                //            sLine += WorkOrderDetail.RemarksList[i].Remark;
                //            sLine = sLine.PadRight(RECORDLEN, ' ');
                //            EAPFormatList.Add(sLine);
                //        }
                //        else
                //        {
                //            string rmk1 = WorkOrderDetail.RemarksList[i].Remark.Substring(0, 77);
                //            sLine += rmk1;
                //            sLine = sLine.PadRight(RECORDLEN, ' ');
                //            EAPFormatList.Add(sLine);
                //            string rmk2 = WorkOrderDetail.RemarksList[i].Remark.Substring(77);
                //            sLine = "DATRMK";                                                       //---------EDIError Kasturee
                //            sLine += rmk2;                                                          //---------EDIError Kasturee
                //            //sLine = rmk2;
                //            sLine = sLine.PadRight(RECORDLEN, ' ');
                //            EAPFormatList.Add(sLine);
                //        }
                //        //sLine += WorkOrderDetail.RemarksList[i].Remark;
                //        //sLine = sLine.PadRight(RECORDLEN, ' ');
                //        //EAPFormatList.Add(sLine);
                //    }
                //}

                // iterate through sub-collections.
                for (i = 0; i < WorkOrderDetail.RepairsViewList.Count(); i++)
                {
                    sLine = "DATRPR";
                    //Mangal RQ6342 Damage code
                    if (WorkOrderDetail.RepairsViewList[i].Damage != null && WorkOrderDetail.RepairsViewList[i].Damage.DamageCedexCode != null)
                        sLine += WorkOrderDetail.RepairsViewList[i].Damage.DamageCedexCode.PadLeft(DAMAGECODELEN, '0');
                    //Mangal
                    if (WorkOrderDetail.RepairsViewList[i].RepairCode != null && WorkOrderDetail.RepairsViewList[i].RepairCode.RepairCod != null)
                        sLine += WorkOrderDetail.RepairsViewList[i].RepairCode.RepairCod.PadLeft(REPAIRCODELEN, '0');
                    //Mangal RQ6343 Repair loc code
                    if (WorkOrderDetail.RepairsViewList[i].RepairLocationCode != null && WorkOrderDetail.RepairsViewList[i].RepairLocationCode.CedexCode != null)
                        sLine += WorkOrderDetail.RepairsViewList[i].RepairLocationCode.CedexCode.PadLeft(REPAIRLOCCODELEN, '0');
                    //Mangal
                    sLine += WorkOrderDetail.RepairsViewList[i].Pieces.ToString().PadLeft(PIECESLEN, '0');
                    sLine += RemoveDecimal(WorkOrderDetail.RepairsViewList[i].MaterialCost.ToString().PadLeft(MATERIALAMTLEN, '0'));
                    sLine += RemoveDecimal(WorkOrderDetail.RepairsViewList[i].ManHoursPerPiece.ToString().PadLeft(MANHOURLEN, '0'));
                    //Mangal RQ6344 TPI code
                    if (WorkOrderDetail.RepairsViewList[i].Tpi != null && WorkOrderDetail.RepairsViewList[i].Tpi.CedexCode != null)
                        sLine += WorkOrderDetail.RepairsViewList[i].Tpi.CedexCode.PadLeft(TPICODELEN, '0');
                    //Mangal
                    sLine = sLine.PadRight(RECORDLEN, ' ');
                    EAPFormatList.Add(sLine);
                }

                if (WorkOrderDetail.SparePartsViewList != null)
                {
                    for (i = 0; i < WorkOrderDetail.SparePartsViewList.Count(); i++)
                    {
                        sLine = "DATPRT";

                        // remove decimal from pieces count
                        //tmp = pPart->GetPieces().c_str(); pPart->SetPieces( (LPCTSTR)RemoveDecimal( tmp ) );
                        sLine += RemoveDecimal(WorkOrderDetail.SparePartsViewList[i].Pieces.ToString().PadLeft(PIECESLEN, '0'));

                        sLine += WorkOrderDetail.SparePartsViewList[i].OwnerSuppliedPartsNumber.PadLeft(PARTNUMBERLEN, '0');
                        sLine = sLine.PadRight(RECORDLEN, ' ');
                        EAPFormatList.Add(sLine);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }


            //return( m_EAPFormatList.GetCount() );
        }

        string RemoveDecimal(string sValue)
        {
            string sNew = string.Empty;

            try
            {
                // keep only digits.
                //for (int i=0;i<sValue.Length;i++)
                foreach (var c in sValue)
                    if (c != '.') sNew += c;
                //		if (isdigit( sValue.GetAt( i ) )) sNew+= sValue.GetAt( i );
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return (sNew);
        }

        private int SplitMessage(string inString, out string out1, out string out2, out string out3)
        {
            int i = 1;
            string sWork = string.Empty; ;
            out1 = string.Empty;
            out2 = string.Empty;
            out3 = string.Empty;
            try
            {


                //out1 += inString;
                if (inString.Length <= 77)
                {
                    i = 1;
                    out1 = "ERR";
                    out1 += inString;

                }
                else
                {      //else is greater than 77 bytes.
                    i = 2;
                    out1 = "ERR";
                    out1 += (inString.Substring(0, 77));

                    if (inString.Length >= 154)
                    {

                        out2 = "ERR";
                        out2 += (inString.Substring(77, 77));
                        if (inString.Length > 154)
                        {
                            i = 3;
                            out3 = "ERR";
                            out3 += (inString.Substring(154));
                        }
                    }

                    else // length less than 154
                    {
                        out2 = "ERR";
                        out2 += (inString.Substring(77));
                    }
                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return (i);
        }


        void AddLines(string MQMessage)
        {
            try
            {
                string szRecord = null;
                int idx = 0;
                while (MQMessage.IndexOf('\n') >= 0)
                {
                    int Delimidx = MQMessage.IndexOf('\n');
                    int size = MQMessage.Length;
                    szRecord = MQMessage.Substring(idx, Delimidx + 1);
                    WOFileRecList.Add(szRecord);
                    if (Delimidx + 2 > size)
                        break;
                    else
                    {
                        string tempMessage = MQMessage.Substring(Delimidx + 1, (size - Delimidx - 1));
                        MQMessage = tempMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = "Error while parsing MQ message";
                Logger.Write(logEntry);
                //Error while parsing MQ message
            }

        }

        void ProcessWorkorders()
        {
            //parseHeader(null);//remove this
            bool success = false;
            string line = string.Empty;
            string RecordType = string.Empty;
            string WORecord = string.Empty;
            bool bWOExists = false;
            string CurrentRepCode = string.Empty;

            try
            {
                using (FileStream WOFileFs = File.Open(WOMQMessageFile, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    //WOLogFile = new StreamWriter(WOFileFs);
                    using (System.IO.StreamReader WOfile = new System.IO.StreamReader(WOFileFs))
                    {
                        while ((line = WOfile.ReadLine()) != null)
                        {
                            if (line.Length < 10)
                                continue;

                            // add to EDI detail log if we have an EDI_ID to log against
                            if (nID > 0)
                            {
                                // int i = pEDI->InsertLineItem(nID, nLineNo++, line);
                                InsertLineItem(nID, nLineNo++, line);
                                WOLogFile.WriteLine("Add to EDI detail log if we have an EDI_ID to log against");
                            }

                            RecordType = line.Substring(0, 3);
                            WORecord = line.Substring(4);//line.Length - 1);
                            switch (GetRecordType(RecordType))
                            {
                                case HEADERREC:
                                    if (bWOExists)
                                    {
                                        SaveWorkOrder(WOD);
                                    }
                                    parseHeader(WORecord);
                                    bWOExists = true;
                                    nWOQty++;
                                    WOLogFile.WriteLine("Reading header record complete.");
                                    break;

                                case REPAIRREC:
                                    ParseRepair(WORecord, out CurrentRepCode); //it should be the string with rep info
                                    WOLogFile.WriteLine("Reading repair record complete.");
                                    break;

                                case PARTSREC:
                                    WORecord = CurrentRepCode + WORecord;
                                    ParseEmailSparePart(WORecord);
                                    WOLogFile.WriteLine("Reading part record complete.");
                                    break;

                                case REMARKREC:
                                    RemarkEntry RE = new RemarkEntry();
                                    RE.Remark = WORecord.Trim();
                                    RE.CRTSDate = DateTime.Now.ToString();
                                    RE.ChangeUser = "MercWOLoad";
                                    RE.RemarkType = "V";
                                    RE.SuspendCatID = 3;
                                    if (WOD.RemarksList == null)
                                    {
                                        WOD.RemarksList = new List<RemarkEntry>();
                                        WOD.RemarksList.Add(RE);
                                    }
                                    else
                                        WOD.RemarksList.Add(RE);
                                    WOLogFile.WriteLine("Reading remark record complete.");
                                    break;

                            }
                        }

                        try
                        {
                            if (bWOExists) SaveWorkOrder(WOD);
                        }
                        catch (Exception ex)
                        {
                            logEntry.Message = "Unknown error while saving work orders" + ex.ToString();
                            Logger.Write(logEntry);
                            //SaveEDIData();
                        }
                        WOLogFile.WriteLine("Parse complete");
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        void parseHeader(string WORecord)
        {
            // WORecord = "MAER~                          1512220786~J58~23122015~PONU7968777~03~4~   ~ ~0040~0000~0000~0000~000000001900~                                           ";
            string szRecord = null;
            int size = 0;
            int idx = 0;
            int i = 0;
            WOD = new WorkOrderDetail();
            WOD.Shop = new Shop();
            WOD.Shop.Customer = new List<Customer>();
            Customer cust = new Customer();
            Equipment eqp = new Equipment();
            WOD.EquipmentList = new List<Equipment>();
            string tempRepairDate = string.Empty;
            string tempManHourReg = string.Empty;
            string tempManHourDouble = string.Empty;
            string tempManHourMisc = string.Empty;
            string tempManHourOther = string.Empty;
            string tempEDIAmount = string.Empty;

            try
            {
                while (WORecord.IndexOf("~") >= 0)
                {
                    //i = 0;
                    int Delimidx = WORecord.IndexOf("~");
                    size = WORecord.Length;
                    szRecord = WORecord.Substring(idx, Delimidx);
                    WOD.woState = (int)Validation.STATE.NEW;
                    //do ur handling here szRecord will contain the parsed message
                    switch (i)
                    {
                        case 0: cust.CustomerCode = szRecord; break;
                        case 1: eqp.VendorRefNo = szRecord.Trim(); break;
                        case 2: WOD.Shop.ShopCode = szRecord; break;

                        // Note EDI date will be DDMMYYYY - must be altered.
                        case 3: tempRepairDate = szRecord;
                            if (tempRepairDate.Length == 8)
                                tempRepairDate = FormatEDIDate(tempRepairDate);
                            WOD.RepairDate = Convert.ToDateTime(tempRepairDate);
                            break;

                        case 4: eqp.EquipmentNo = szRecord.Trim(); break;
                        case 5: WOD.Mode = szRecord; break;
                        case 6: WOD.Cause = szRecord; break;
                        case 7: WOD.ThirdPartyPort = szRecord; break;
                        case 8: WOD.WorkOrderType = szRecord; break;

                        case 9: tempManHourReg = szRecord; tempManHourReg = tempManHourReg.Trim();
                            if (tempManHourReg == "") tempManHourReg = "0.0";
                            if (tempManHourReg.Length < 3)
                                if (tempManHourReg.StartsWith(".")) tempManHourReg += ".0";
                            tempManHourReg = AddDecimal(tempManHourReg, 2);
                            WOD.TotalManHourReg = Convert.ToDouble(tempManHourReg);
                            break;
                        case 10: tempManHourDouble = szRecord; tempManHourDouble = tempManHourDouble.Trim();
                            if (tempManHourDouble == "") tempManHourDouble = "0.0";
                            if (tempManHourDouble.Length < 3)
                                if (tempManHourDouble.StartsWith(".")) tempManHourDouble += ".0";
                            tempManHourDouble = AddDecimal(tempManHourDouble, 2);
                            WOD.TotalManHourDoubleTime = Convert.ToDouble(tempManHourDouble);
                            break;
                        case 11: tempManHourOther = szRecord;
                            tempManHourOther = tempManHourOther.Trim();
                            if (tempManHourOther == "") tempManHourOther = "0.0";
                            if (tempManHourOther.Length < 3)
                                if (tempManHourOther.StartsWith(".")) tempManHourOther += ".0";
                            tempManHourOther = AddDecimal(tempManHourOther, 2);
                            WOD.TotalManHourOverTime = Convert.ToDouble(tempManHourOther); break;
                        case 12: tempManHourMisc = szRecord; tempManHourMisc = tempManHourMisc.Trim();
                            if (tempManHourMisc == "") tempManHourMisc = "0.0";
                            if (tempManHourMisc.Length < 3)
                                if (tempManHourMisc.StartsWith(".")) tempManHourMisc += ".0";
                            tempManHourMisc = AddDecimal(tempManHourMisc, 2);
                            WOD.TotalManHourMisc = Convert.ToDouble(tempManHourMisc); break;
                        case 13: tempEDIAmount = szRecord; tempEDIAmount = tempEDIAmount.Trim();
                            tempEDIAmount = AddDecimal(tempEDIAmount, 2);
                            WOD.TotalEDIAmount = Convert.ToDecimal(tempEDIAmount);
                            break;
                        //			case 14: m_sRemarks= token; m_sRemarks.TrimRight(); break;

                    }

                    i++;

                    if (Delimidx + 2 > size)
                        break;
                    else
                    {
                        string tempMessage = WORecord.Substring(Delimidx + 1, (size - Delimidx - 1));
                        WORecord = tempMessage;
                    }
                }
                // Force upper case.
                WOD.Shop.Customer.Add(cust);
                WOD.EquipmentList.Add(eqp);
                WOD.Shop.Customer[0].CustomerCode = WOD.Shop.Customer[0].CustomerCode.ToUpper();
                WOD.EquipmentList[0].VendorRefNo = WOD.EquipmentList[0].VendorRefNo.ToUpper();
                WOD.Shop.ShopCode = WOD.Shop.ShopCode.ToUpper();
                WOD.EquipmentList[0].EquipmentNo = WOD.EquipmentList[0].EquipmentNo.ToUpper();
                WOD.Mode = WOD.Mode.ToUpper();
                WOD.Cause = WOD.Cause.ToUpper();
                WOD.ThirdPartyPort = WOD.ThirdPartyPort.ToUpper();
                WOD.WorkOrderType = WOD.WorkOrderType.ToUpper();
                WOD.IsSingle = true;

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        void ParseRepair(string RepairDetails, out string CurrentRepCode)
        {
            //RepairDetails = "MAER~                          1512220786~J58~23122015~PONU7968777~03~4~   ~ ~0040~0000~0000~0000~000000001900~                                           ";
            string szRecord = null;
            int size = 0;
            int idx = 0;
            int i = 0;
            //WOD.RepairsViewList = new List<RepairsView>();
            RepairsView Rv = new RepairsView();
            Rv.Damage = new Damage();
            Rv.Tpi = new Tpi();
            Rv.RepairLocationCode = new RepairLoc();
            Rv.RepairCode = new RepairCode();
            string tempPieces = string.Empty;
            string tempMaterialAmt = string.Empty;
            string tempManHrs = string.Empty;
            int MATERIALAMTLEN = 12;
            CurrentRepCode = string.Empty;

            try
            {
                while (RepairDetails.IndexOf("~") >= 0)
                {
                    //i = 0;
                    int Delimidx = RepairDetails.IndexOf("~");
                    size = RepairDetails.Length;
                    szRecord = RepairDetails.Substring(idx, Delimidx);
                    Rv.rState = (int)Validation.STATE.NEW;
                    //do ur handling here szRecord will contain the parsed message
                    switch (i)
                    {
                        case 0: Rv.Damage.DamageCedexCode = szRecord.Trim(); break;
                        case 1: Rv.RepairCode.RepairCod = szRecord.Trim();
                            CurrentRepCode = Rv.RepairCode.RepairCod;
                            CurrentRepCode += "~";
                            break;
                        //Mangal Release 3 RQ6343 getting RepairLocation code from EDI, manual or WS mode 
                        case 2: Rv.RepairLocationCode.CedexCode = szRecord.ToUpper().Trim(); break;
                        case 3: tempPieces = szRecord.Trim();
                            Rv.Pieces = Convert.ToInt32(tempPieces);
                            break;
                        case 4: tempMaterialAmt = szRecord.Trim();
                            if (!string.IsNullOrEmpty(tempMaterialAmt))
                            {
                                // if length < 12 assume from Web - if no decimal, assume whole number and append .0
                                if (tempMaterialAmt.Length < MATERIALAMTLEN)
                                    if (tempMaterialAmt.StartsWith(".")) tempMaterialAmt += (".0");
                                if (IsNumericString(tempMaterialAmt))
                                    tempMaterialAmt = AddDecimal(tempMaterialAmt, 2);
                            }
                            Rv.MaterialCost = Convert.ToDecimal(tempMaterialAmt);
                            break;
                        case 5: tempManHrs = szRecord.Trim();
                            if (IsNumericString(tempManHrs))
                            {
                                if (tempManHrs.Length < 3)
                                    if (tempManHrs.StartsWith(".")) tempManHrs += (".0");
                                tempManHrs = AddDecimal(tempManHrs, 2);
                            }
                            Rv.ManHoursPerPiece = Convert.ToDouble(tempManHrs);
                            break;
                        //Mangal Release 3 RQ6344 getting Tpi code from EDI, manual or WS mode 
                        case 6: Rv.Tpi.CedexCode = szRecord.Trim();
                            break;

                    }

                    i++;
                    if (Delimidx + 2 > size)
                        break;
                    else
                    {
                        string tempMessage = RepairDetails.Substring(Delimidx + 1, (size - Delimidx - 1));
                        RepairDetails = tempMessage;
                    }
                }
                if (WOD.RepairsViewList == null)
                {
                    WOD.RepairsViewList = new List<RepairsView>();
                    WOD.RepairsViewList.Add(Rv);
                }
                else
                    WOD.RepairsViewList.Add(Rv); ;

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        void ParseEmailSparePart(string SparePartDetails)
        {
            //SparePartDetails = "MAER~                          1512220786~J58~23122015~PONU7968777~03~4~   ~ ~0040~0000~0000~0000~000000001900~                                           ";
            string szRecord = null;
            int size = 0;
            int idx = 0;
            int i = 0;
            SparePartsView SPV = new SparePartsView();
            SPV.RepairCode = new RepairCode();
            string tempPieces = string.Empty;
            double dPcs = 0;
            try
            {
                while (SparePartDetails.IndexOf("~") >= 0)
                {

                    int Delimidx = SparePartDetails.IndexOf("~");
                    size = SparePartDetails.Length;
                    szRecord = SparePartDetails.Substring(idx, Delimidx);
                    SPV.pState = (int)Validation.STATE.NEW;
                    //do ur handling here szRecord will contain the parsed message
                    switch (i)
                    {
                        case 0: SPV.RepairCode.RepairCod = szRecord.Trim(); break;

                        case 1: tempPieces = szRecord.Trim();
                            tempPieces = AddDecimal(tempPieces, 1);
                            dPcs = Convert.ToDouble(tempPieces);
                            SPV.Pieces = dPcs;
                            break;

                        case 2: SPV.OwnerSuppliedPartsNumber = szRecord.Trim(); break;
                        case 3: SPV.SerialNumber = szRecord.Trim(); break;
                    }

                    i++;

                    if (Delimidx + 2 > size)
                        break;
                    else
                    {
                        string tempMessage = SparePartDetails.Substring(Delimidx + 1, (size - Delimidx - 1));
                        SparePartDetails = tempMessage;
                    }
                    // Force upper case on all entries;

                }
                if (WOD.SparePartsViewList == null)
                {
                    WOD.SparePartsViewList = new List<SparePartsView>();
                    WOD.SparePartsViewList.Add(SPV);
                }
                else
                    WOD.SparePartsViewList.Add(SPV); ;
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        void ParseRemarks(string RemarksDetails)
        {
            //SparePartDetails = "MAER~                          1512220786~J58~23122015~PONU7968777~03~4~   ~ ~0040~0000~0000~0000~000000001900~                                           ";
            string szRecord = null;
            int size = 0;
            int idx = 0;
            int i = 0;
            RemarkEntry RE = new RemarkEntry();
            try
            {
                while (RemarksDetails.IndexOf("~") >= 0)
                {

                    int Delimidx = RemarksDetails.IndexOf("~");
                    size = RemarksDetails.Length;
                    szRecord = RemarksDetails.Substring(idx, Delimidx);
                    //do ur handling here szRecord will contain the parsed message
                    switch (i)
                    {
                        case 0: RE.Remark = szRecord.Trim();
                            RE.CRTSDate = DateTime.Now.ToString();
                            RE.ChangeUser = "MercWOLoad";
                            RE.RemarkType = "V";
                            RE.SuspendCatID = 3;
                            break;
                    }

                    i++;

                    if (Delimidx + 2 > size)
                        break;
                    else
                    {
                        string tempMessage = RemarksDetails.Substring(Delimidx + 1, (size - Delimidx - 1));
                        RemarksDetails = tempMessage;
                    }
                    // Force upper case on all entries;

                }
                if (WOD.RemarksList == null)
                {
                    WOD.RemarksList = new List<RemarkEntry>();
                    WOD.RemarksList.Add(RE);
                }
                else
                    WOD.RemarksList.Add(RE); ;
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        int InsertNewTransmission()
        {
            MESC1TS_EDI_TRANSMISSION EDITransmission = new MESC1TS_EDI_TRANSMISSION();
            DateTime CurrentDate = DateTime.Now;
            int ID = 0;
            try
            {
                //                EDITransmission.CRTS = CurrentDate;
                //                objContext.MESC1TS_EDI_TRANSMISSION.AddObject(EDITransmission);
                //                objContext.SaveChanges();

                //                ID = EDITransmission.EDI_ID;

                //                var Identity = (from edi in objContext.MESC1TS_EDI_TRANSMISSION
                //                                where edi.CRTS == CurrentDate
                //                                select edi).FirstOrDefault();

                //                if (Identity != null)
                //                {
                //                    ID = Identity.EDI_ID;
                //                }

                List<SQLDataClass> cls = new List<SQLDataClass>();
                cls = objContext.Database.SqlQuery<SQLDataClass>("insert into MESC1TS_EDI_TRANSMISSION (CRTS) values ({0})", CurrentDate).ToList();
                var Identity1 = (from edi in objContext.MESC1TS_EDI_TRANSMISSION
                                 select edi).OrderByDescending(e => e.CRTS).FirstOrDefault();

                if (Identity1 != null)
                {
                    ID = Identity1.EDI_ID;
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ID;
        }

        void InsertLineItem(int ID, int LineNo, string LineDetail)
        {
            MESC1TS_EDI_LINEITEM EDILineItem = new MESC1TS_EDI_LINEITEM();

            try
            {
                //EDILineItem.EDI_ID = ID;
                //EDILineItem.LINE_NO = LineNo;
                //EDILineItem.LINE_DETAIL = RepairText(LineDetail);
                //objContext.MESC1TS_EDI_LINEITEM.AddObject(EDILineItem);
                //objContext.SaveChanges();

                LineDetail = RepairText(LineDetail);
                List<SQLDataClass> cls = new List<SQLDataClass>();
                cls = objContext.Database.SqlQuery<SQLDataClass>("insert into MESC1TS_EDI_LINEITEM (EDI_ID, LINE_NO, LINE_DETAIL) values({0},{1},{2})", ID, LineNo, LineDetail).ToList();
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        void InsertError(int ID, int LineNo, string LineDetail)
        {
            MESC1TS_EDI_ERROR EDILineItem = new MESC1TS_EDI_ERROR();

            try
            {
                //EDILineItem.EDI_ID = ID;
                //EDILineItem.LINE_NO = LineNo;
                //EDILineItem.LINE_DETAIL = RepairText(LineDetail);
                //objContext.MESC1TS_EDI_ERROR.AddObject(EDILineItem);
                //objContext.SaveChanges();

                LineDetail = RepairText(LineDetail);
                List<SQLDataClass> cls = new List<SQLDataClass>();
                cls = objContext.Database.SqlQuery<SQLDataClass>("insert into MESC1TS_EDI_ERROR (EDI_ID, LINE_NO, LINE_DETAIL) values({0},{1},{2})", ID, LineNo, LineDetail).ToList();
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        void UpdateTransmission(int ID, int Qty, int Pass, int Fail)
        {
            try
            {
                //EDITransmission = (from edi in objContext.MESC1TS_EDI_TRANSMISSION
                //                   where edi.EDI_ID == ID
                //                   select edi).FirstOrDefault();
                //if (EDITransmission != null)
                //{
                //    EDITransmission.WO_QTY = Qty;
                //    EDITransmission.WO_PASS_QTY = Pass;
                //    EDITransmission.WO_FAIL_QTY = Fail;
                //    objContext.SaveChanges();
                //}

                List<SQLDataClass> cls = new List<SQLDataClass>();
                cls = objContext.Database.SqlQuery<SQLDataClass>("update MESC1TS_EDI_TRANSMISSION set WO_QTY = {0} where EDI_ID = {1}", Qty, ID).ToList();
                cls = objContext.Database.SqlQuery<SQLDataClass>("update MESC1TS_EDI_TRANSMISSION set WO_PASS_QTY = {0} where EDI_ID = {1}", Pass, ID).ToList();
                cls = objContext.Database.SqlQuery<SQLDataClass>("update MESC1TS_EDI_TRANSMISSION set WO_FAIL_QTY = {0} where EDI_ID = {1}", Fail, ID).ToList();
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        private string FormatEDIDate(string sDate)
        {
            // Date expected from EDI as DDMMYYYY
            // reformat as YYYY-MM-DD
            string s = "";

            if (sDate.Length >= 8)
            {
                s += sDate.Substring(4, 4);
                s += "-";
                s += sDate.Substring(2, 2);
                s += "-";
                s += sDate.Substring(0, 2);
            }

            return (s);
        }

        // Insert decimal into whole number with 'assumed' decimal eg: pic 99V9 - 
        private string AddDecimal(string sValue, int nPrecision)
        {
            double f;
            string tmp = (sValue);

            if (tmp.Length == 0)
            {
                switch (nPrecision)
                {
                    case 1: return ("0.0"); break;
                    case 2: return ("0.00"); break;
                    default: return ("0.00"); break;
                }
            }


            // else if decimal found, simply return value as is - could be from DB.
            if (tmp.Contains('.')) return (tmp); // return as is - no changes

            // else string value requires a decimal place.
            double tmpDec = Convert.ToDouble(tmp);
            switch (nPrecision)
            {
                case 1: f = (tmpDec) * 0.1; break;
                case 2: f = (tmpDec) * 0.01; break;
                default: f = (tmpDec) * 0.01; break;
            }

            if (nPrecision == 1)
                return (FormatFloatDecimal(f, true));
            else
                return (FormatFloatDecimal(f, false));
        }

        private string FormatFloatDecimal(double f, bool bSinglePrecision)
        {
            if (f == 0) return ("0");

            // [INCOMPLETE]  - Possibly different formats required.... for now, USA decimal used
            string s;

            if (bSinglePrecision)
                f = Math.Round(f, 1);
            else
                f = Math.Round(f, 2);

            s = f.ToString();

            // NULL fields are returning garbage, so need numeric validation here
            //	if (! IsNumericString( s ) ) return( "0" );	

            return (s);
        }

        // weak numeric check...
        private bool IsNumericString(string s)
        {
            char c;
            if (s.Length == 0) return (false);

            for (int i = 0; i < s.Length; i++)
            {
                c = s[i];
                if ((c != '-') && (c != '+') && (!char.IsDigit(c)) && (c != '.')) return (false);
            }
            return (true);
        }

        private string RepairText(string MSG)
        {
            string newstr = "";
            char ctest;

            // fix apostrophe for SQL server.
            for (int i = 0; i < MSG.Length; i++)
            {
                ctest = MSG[i];
                if (ctest == '\'') newstr += '\'';
                newstr += ctest;
            }

            return (newstr);
        }

        bool SetWorkOrder(string WORecord)
        {
            //parse work order details here
            //pWorkOrder->SetUser(PROGRAMNAME);
            return true;
        }

        //bool AddRepairCost(string WORecord)
        //{
        //    //CString tmp;

        //    //// Prefix repair code to part line.( already tilde delimited ).
        //    //tmp = m_sRepairCd;
        //    //tmp += s;

        //    //return (tmp);
        //    ////add work order repair cost
        //    //return true ;
        //}

        //        void ExtractRepairCode(string s)
        //{

        //    //char	cLine[2048];
        //    //char*   token;
        //    //char	sep[] = "~";
        //    //int		i=0;

        //    //strcpy( cLine, (LPCTSTR)s );

        //    //i = 0; 
        //    ///* get first string */
        //    //token = strtok( cLine, sep );
        //    //token = strtok( NULL, sep );
        //    //if (token != NULL) 
        //    //{
        //    //    m_sRepairCd = token;
        //    //    m_sRepairCd+= "~";
        //    //}
        //    //else
        //    //    m_sRepairCd= "<<*>>~";	 // safety: if Repair record should be empty (unlikely)
        //}

        // called for each WO - validations performed first.
        // if OK, will call save method.
        //void SaveWorkOrder()
        //{

        //    // TEST TEST TEST 
        //    //int i = pWorkOrder->GetWorkOrderCount();
        //    //printf("Count of WO = %d", i);

        //    //printf("Count of Repairs = %d", pWorkOrder->GetRepairCount() );
        //    // END TEST TEST


        //    int iRtn = 0;

        //    // validation routines in WorkOrder
        //    // if failed, send EAP formatted WO back to enterprise.
        //    //printf("Validate\n");
        //    // Must set user value
        //    pWorkOrder->SetUser(PROGRAMNAME);
        //    fLogFile << "Work order validation started." << endl;

        //    iRtn = pWorkOrder->Validate();
        //    fLogFile << "Work order validation completed." << endl;

        //    //printf("Return from Validate = %d", iRtn);

        //    if (iRtn == 1) // failed
        //    {
        //        //printf("Validate failed - send error to enterprise file\n");
        //        nWOFailQty++;

        //        SendErrorToEnterprise();
        //        return;
        //    }

        //    //printf("Saving work order\n");

        //    // Else save WO to DB.
        //    // Assuming validation OK, save.
        //    fLogFile << "Saving Work order started." << endl;
        //    iRtn = pWorkOrder->SaveWorkOrder();
        //    fLogFile << "Saving Work order completed." << endl;
        //    // INCOMPLETE
        //    if (iRtn == 1) // save failed 
        //    {
        //        nWOFailQty++;
        //        SendErrorToEnterprise();
        //        return;
        //    }

        //    nWOPassQty++;
        //}

        int GetRecordType(string RecordType)
        {
            string temp = RecordType.Substring(0, 3);
            if ((temp.ToUpper()).CompareTo("HD1") == 0) return HEADERREC;
            if ((temp.ToUpper()).CompareTo("RPR") == 0) return REPAIRREC;
            if ((temp.ToUpper()).CompareTo("PRT") == 0) return PARTSREC;
            if ((temp.ToUpper()).CompareTo("RMK") == 0) return REMARKREC;

            return (-1);
        }

        void ReadConfig() //to be read from config file
        {
            QueueManager = ConfigurationManager.AppSettings["MQManagerQueueName"];
            RequestQueue = ConfigurationManager.AppSettings["RequestQueue"];
            ReplyQueue = ConfigurationManager.AppSettings["ReplyQueue"];
            Senderid = ConfigurationManager.AppSettings["Senderid"];
            receiverid = ConfigurationManager.AppSettings["receiverid"];
            aprfco = ConfigurationManager.AppSettings["aprfco"];
            aprferr = ConfigurationManager.AppSettings["aprferr"];
            EDILogFilePath = ConfigurationManager.AppSettings["EDILogFilePath"];
            WOMQMessageFile = ConfigurationManager.AppSettings["WOMQMessageFile"];
            count = Int32.Parse(ConfigurationManager.AppSettings["Count"]);
        }
    }
}
