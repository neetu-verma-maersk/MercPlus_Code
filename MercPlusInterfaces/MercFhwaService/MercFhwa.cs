using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using MercPlusLibrary;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using IBM.WMQ;
using System.Data.Entity.Validation;
using System.Threading;
using System.Configuration;

namespace MercFhwaService
{
    public partial class MercFhwa : ServiceBase
    {
        #region declare Variables
        public static LogEntry logEntry = new LogEntry();
        public static MQManager MQManager = new MQManager();
        //char szQueueManager = [MQ_Q_MGR_NAME_LENGTH] ;
        //char	szQueueName[icIpParamSize];
        public static List<Inspection> InspList = new List<Inspection>();
        public static MESC2DSEntities objContext = new MESC2DSEntities();
        private const string sMsgStart = "MERCRKEM05000001";
        private const string sHeadStart = "HDR";
        private const string sDetStart = "DET";
        private const int iMaxDetInMsg = 300;
        public static List<string> FHWAList = new List<string>();
        public static List<string> FHWAQueueList = new List<string>();
        public static MQManager mqManager = new MQManager();
        //////////////////////////////////
        private static bool bIntRecoverFlag = false;
        private static bool bMQCommit = false;
        private static bool bCoInitialize = false;
        private const string MQ_WRITE_MODE = "W";
        private static System.IO.StreamReader oRead = null;
        private static string[] FirstLine = new string[500];
        // process counters
        public static int nXmitCtr = 0;

        private AutoResetEvent AutoEventInstance { get; set; }
        private MercFhwa TimerInstance { get; set; }
        private Timer StateTimer { get; set; }
        public int TimerInterval { get; set; }
        Thread Worker;
        AutoResetEvent StopRequest = new AutoResetEvent(false);
        #endregion

        public MercFhwa()
        {
            InitializeComponent();
            //         TimerInterval = Convert.ToInt32(ConfigurationSettings.AppSettings["TimeInterval"]);
            //TimerInterval = 10000; //This is the time interval between each run value will come from config file
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
            TimerInterval = Convert.ToInt32(ConfigurationSettings.AppSettings["TimeInterval"]);
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
            int nRetCode = 0;
            int i = 0, j = 0;
            try
            {
                logEntry.Message = "Start sending FHWA messages";
                Logger.Write(logEntry);
                int lineNo = 0;
                // oRead = System.IO.File.OpenText(@"D:\Logs\test.txt");
                oRead = System.IO.File.OpenText(ConfigurationSettings.AppSettings["FilePath"]);
                lineNo = 0;
                while (!oRead.EndOfStream)
                {
                    FirstLine[lineNo] = oRead.ReadLine();
                    lineNo += 1;
                }
                for (int cnt = 0; cnt <= (lineNo - 1); cnt++)
                {
                    FHWAQueueList.Add(FirstLine[cnt].ToString());
                    //Console.WriteLine(FirstLine[i].ToString());
                }
                oRead.Dispose();
                //RecoverFile.LoadFromFile(RECOVERYFILENAME.c_str());
                //for (j = 0; j < RecoverFile.GetSize(); j++)
                //{ 	// transfer these unsent values to the MQList for send
                //    FHWAQueueList.Insert((LPCTSTR)RecoverFile.GetAt(j));
                //}
                // Empty recover file. (data has been transfered to the collection.

                //RecoverFile.RemoveAll();

                // validation of input parameters


                //if (argv.Length == 0 || envp.Length == 0)
                //{
                //    logEntry.Message = "Queue Manager / Queue name not specified correctly";
                //    Logger.Write(logEntry);

                //   return (nRetCode);
                //}


                // Obtain a list of Inspection records for send
                ProcessInspectionRecord();
                // Send the records fetched to RKEM
                SendRKEM();
                //Write results to MercEvent table
                SendEventTableAudit();
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            autoEvent.Set();
        }

        //protected override void OnStop()
        //{
        //    StateTimer.Dispose();
        //}

        public static void ProcessInspectionRecord()
        {
            List<MESC1TS_INSPECTION> InspectionListFromDB = new List<MESC1TS_INSPECTION>();
            Inspection Inspection = new Inspection();
            try
            {
                InspectionListFromDB = (from ins in objContext.MESC1TS_INSPECTION
                                        where ins.XMIT_DTE == null
                                        select ins).ToList();

                if (InspectionListFromDB != null && InspectionListFromDB.Count > 0)
                {
                    foreach (var item in InspectionListFromDB)
                    {
                        //    sInspSQL = "SELECT CHAS_EQPNO, CONVERT(CHAR,INSP_DTE,12)INSP_DTE, ";
                        //    sInspSQL+= "CONVERT(CHAR,DATEADD(year, 1, INSP_DTE),12)NEXT_INSP_DTE, ";
                        ////	sInspSQL+= "ISNULL(INSP_BY,' ')INSP_BY, ISNULL(RKEMLOC,' ')RKEMLOC FROM MESC1TS_INSPECTION1 ";
                        //    sInspSQL+= "ISNULL(INSP_BY,' ')INSP_BY, ISNULL(RKEMLOC,' ')RKEMLOC FROM MESC1TS_INSPECTION ";
                        //    sInspSQL+= "WHERE XMIT_DTE IS NULL";
                        Inspection = new Inspection();
                        Inspection.ChasEqpNo = item.CHAS_EQPNO;
                        Inspection.InspDte = item.INSP_DTE;
                        Inspection.S_InspDate = item.INSP_DTE.ToString("yyMMdd");
                        DateTime temp = item.INSP_DTE.AddYears(1);
                        Inspection.NextInspDate = temp.ToString("yyMMdd");
                        Inspection.InspBy = item.INSP_BY;
                        Inspection.RKEMLoc = item.RKEMLOC;
                        InspList.Add(Inspection);
                    }
                }
                else
                {
                    return;
                }

                if (InspList != null && InspList.Count > 0)
                {
                    foreach (var item in InspList)
                    {
                        FHWAList.Add(FormatFHWA(item));
                    }
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        public static string FormatFHWA(Inspection Inspection)
        {
            //DET002NEZAD   100302110302MAEC423378
            string sDetRec = string.Empty;
            string sRKEMLOC = string.Empty;
            string sNextInspDate = string.Empty;
            string InspDate = string.Empty;
            string nextInspDate = string.Empty;

            try
            {
                InspDate = Inspection.S_InspDate;
                //nextInspDate = Inspection
                sDetRec += (sDetStart);
                sDetRec += Inspection.InspBy.PadRight(3);
                sRKEMLOC += Inspection.RKEMLoc.PadRight(8);
                sDetRec += sRKEMLOC.Substring(0, 2);
                sDetRec += sRKEMLOC.Substring(2, 6);
                sDetRec += InspDate.PadRight(6);
                sDetRec += Inspection.NextInspDate.PadRight(6);
                sDetRec += Inspection.ChasEqpNo.PadRight(11);
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return (sDetRec);
        }

        public static void SendEventTableAudit()
        {
            MESC1TS_EVENT_LOG EventLog = new MESC1TS_EVENT_LOG();
            string msg = string.Empty;
            try
            {
                msg = "RKEM FHWA transmission process - Records selected: ";
                msg += FHWAList.Count();
                msg += (" Messages sent to  RKEM (in batches of 300 or less): ");
                msg += nXmitCtr;

                //pAudit->InsertEvent("MESC1TS_INSPECTION","Not Applicable",msg.c_str(),"RKEM_TRANSMISSION","MercFHWA Process");

                EventLog.TABLE_NAME = "MESC1TS_INSPECTION";
                EventLog.UNIQUE_ID = "Not Applicable";
                EventLog.EVENT_DESC = msg;
                EventLog.EVENT_NAME = "RKEM_TRANSMISSION";
                EventLog.CHUSER = "MercFHWA Process";
                EventLog.CHTS = DateTime.Now;
                objContext.MESC1TS_EVENT_LOG.Add(EventLog);
                objContext.SaveChanges();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        public static bool FHWAUpdateInspection(int iLoopStart, int iLoopEnd)
        {
            MESC1TS_INSPECTION Inspection = new MESC1TS_INSPECTION();

            bool bOK = true;

            try
            {
                for (int i = iLoopStart; i < iLoopEnd; i++)
                {
                    string chas = InspList[i].ChasEqpNo;
                    DateTime insDate = InspList[i].InspDte;
                    //string tempInspDate = InspList[i].S_InspDate.PadRight(6);
                    //DateTime? dt = (DateTime)tempInspDate;
                    var InspFromDB = (from insp in objContext.MESC1TS_INSPECTION
                                      where insp.CHAS_EQPNO == chas &&
                                      insp.INSP_DTE == insDate
                                      //sUpdSql.append("',21)")
                                      select insp).ToList();

                    if (InspFromDB != null && InspFromDB.Count > 0)
                    {
                        foreach (var item in InspFromDB)
                        {
                            item.XMIT_DTE = DateTime.Now;
                            item.CHUSER = "99999";
                            item.CHTS = DateTime.Now;
                            item.XMIT_RC = "0";
                            try
                            {
                                //objContext.MESC1TS_INSPECTION.Add(Inspection);
                                objContext.SaveChanges();
                            }
                            catch (DbEntityValidationException ex)
                            {
                                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                                throw new DbEntityValidationException(errorMessages);
                            }
                            catch (Exception ex)
                            {
                                bOK = false;
                            }
                        }
                    }
                }
                if (!bOK)
                {
                    // Log error.
                    //pRecordStart = (CInspRecord*)InspList.GetAt( iLoopStart );
                    //pRecordEnd = (CInspRecord*)InspList.GetAt( iLoopEnd );
                    //sprintf(msg,"Unable to update Inspection table for FHWA Transmit to RKEM for CHASSIS id: %s  on Date %s through CHASSIS id: %s on Date %s",pRecordStart->m_sCHAS_EQPNO.c_str(), pRecordStart->m_sINSP_DTE.c_str(),pRecordEnd->m_sCHAS_EQPNO.c_str(), pRecordEnd->m_sINSP_DTE.c_str()  );
                    //// db->pError->WriteApplicationLog("MercFHWA", msg );
                    //// Commented SNA 04/28/05 to replace with WriteErrorWithID 
                    ////   - This logs a unque id that helps tracking failures.
                    //db->pError->WriteErrorWithID("MercFHWA", msg, 7882);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return (bOK);
        }

        public static void SendRKEM()
        {
            MQManager mMQ = new MQManager();
            MQQueueManager QMgr = null;
            MQQueue Q = null;
            System.IO.StreamWriter oWrite = null;
            bool success = true;
            MQMessage mqMsg = new MQMessage();
            try
            {
                QMgr = mMQ.OpenQueueManager(ConfigurationSettings.AppSettings["MQManagerQueueName"]);
                Q = mMQ.OpenQ(MQ_WRITE_MODE, QMgr, ConfigurationSettings.AppSettings["MQManagerRequestName"]);
                //QMgr = mMQ.OpenQueueManager("MERCDEV");
                //Q = mMQ.OpenQ(MQ_WRITE_MODE, QMgr, "RKEM.RKEMMP.REQUEST");
                int n = 1;
                // Open the MQ connection for write access.

                // if queue opened successfully, load messages into the queue
                bool bMQWriteError = false;
                //if (iMQConRet == 1)
                //{

                /************************************************************************/
                //Transmitting records read from the backup file
                string sRetransMsg = "";
                for (int iRetransCtr = 0; iRetransCtr < FHWAQueueList.Count(); iRetransCtr++)
                {

                    sRetransMsg = FHWAQueueList[iRetransCtr];
                    nXmitCtr++;
                    success = mMQ.PutMessage(Q, sRetransMsg);
                    if (!success)
                    {
                        System.IO.File.WriteAllText(ConfigurationSettings.AppSettings["FilePath"], sRetransMsg + "\r\n");
                        //				cout<<"Adding to recovery file"<<endl;
                        //RecoverFile.Add(sRetransMsg.c_str());
                        bMQWriteError = true;
                        bIntRecoverFlag = true;
                    }
                    sRetransMsg = "";
                }

                /************************************************************************
                // Send FHWAList to MQ
                Creating the beginning of the output string
                ************************************************************************/
                int iTotalDetRecs = FHWAList.Count();
                if (iTotalDetRecs > 0)
                {
                    string sHdr = string.Empty;
                    string sDet = string.Empty; ;

                    string sRecCount;
                    char[] cHdrRecCount = new char[10];
                    int iFileCount = 0, iDetWritten = 0, iDetToWrite = 0, iDetInHdr = 0;
                    // Total detail record count 

                    iFileCount = (int)(Math.Ceiling((double)iTotalDetRecs / (double)iMaxDetInMsg));
                    //cout << "The file count is "<<iFileCount<<endl; // number of messages, given this detail record count

                    // For each output file 
                    for (int i = 1; i <= iFileCount; i++)
                    {
                        //////////////////////////////////////////////////////////
                        //setting the start of the message
                        //////////////////////////////////////////////////////////
                        sHdr = sMsgStart;
                        sHdr += DateTime.Now.ToString();
                        //////////////////////////////////////////////////////////
                        // start header section
                        //////////////////////////////////////////////////////////
                        sHdr += (sHeadStart);					// 'HDR'
                        // Detail Record count to be added in the header
                        iDetToWrite = iTotalDetRecs - iDetWritten;
                        iDetInHdr = (iDetToWrite > iMaxDetInMsg) ? iMaxDetInMsg : iDetToWrite;
                        //_itoa( iDetInHdr, cHdrRecCount, 10 );
                        string sHdrRecCount = iDetInHdr.ToString();
                        sHdr += sHdrRecCount.PadRight(3);

                        //////////////////////////////////////////////////////////
                        //Extracting detail records and appending them to string
                        //////////////////////////////////////////////////////////
                        int x = iDetWritten;
                        for (int j = iDetWritten; j < i * iMaxDetInMsg && j < iTotalDetRecs; j++)
                        {
                            sDet += FHWAList[j];
                            x++;

                        }
                        sHdr += (sDet);
                        //////////////////////////////////////////////////////////
                        // go for batch database update of detail records 
                        // to be written to queue.if db update fails, attempt the 
                        // same for the next batch of records.
                        //////////////////////////////////////////////////////////
                        if (!(FHWAUpdateInspection(iDetWritten, x)))
                        {
                            logEntry.Message = "Error updating inspection records in the database MESC2DS";
                            Logger.Write(logEntry);
                        }
                        else
                        {

                            ///////////////////////////////////////////////
                            //writing to queue. if write successful,commit queue.
                            // else write to recovery file
                            ///////////////////////////////////////////
                            // Added for testing recovery - to be removed
                            //RecoverFile.Add( sHdr.c_str() );
                            //bMQWriteError=true;
                            ///////////////////////////////////////////
                            //n = MQManager.WriteQueue( (char*)sHdr.c_str());
                            success = mMQ.PutMessage(Q, sHdr);
                            if (!success)
                            {
                                System.IO.File.AppendAllText(ConfigurationSettings.AppSettings["FilePath"], sHdr);
                                //RecoverFile.Add( sHdr.c_str() );
                                bIntRecoverFlag = true;
                                bMQCommit = false;
                                bMQWriteError = true;
                                logEntry.Message = "Error putting FHWA inspection information to  remote Queue: %s on Manager: %s.";                          Logger.Write(logEntry);
                                //db->pError->WriteErrorWithID("MercFHWA", msg, 7882);


                            }
                            else
                            {
                                nXmitCtr++;

                                //MQManager.Commit();
                                mMQ.CloseQ(Q);
                                bMQCommit = true;
                            }
                        }
                        //////////////////////////////////////////////////////////
                        // Empty header and detail strings, set DetWritten
                        //////////////////////////////////////////////////////////
                        iDetWritten = x;
                        sHdr = ""; sDet = "";
                        bMQCommit = false;

                    }
                }
                // Close the MQ Connection
                mMQ.DisconnectQueueManager(QMgr);
                // if no errors...
                // Delete recovery file if no MQ errors. we do not want to 
                // send same messages again.
                //if (! bMQWriteError)
                //    unlink( RECOVERYFILENAME.c_str() );
                //else 
                //{
                //    // save to disk for next attempt 
                //    RecoverFile.SaveToFile( RECOVERYFILENAME.c_str() );
                //    bIntRecoverFlag=false;
                //}
                bIntRecoverFlag = false;
                //		cout<<"disconnected from queue manager "<< endl;
                //}
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

        }
    }
}
