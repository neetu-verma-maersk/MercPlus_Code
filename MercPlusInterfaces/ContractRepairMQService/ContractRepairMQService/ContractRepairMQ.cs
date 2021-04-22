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
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Globalization;
using System.IO;

namespace ContractRepairMQService
{
    public partial class ContractRepairMQ : ServiceBase
    {
        #region Declare Variables
        private AutoResetEvent AutoEventInstance { get; set; }
        private ContractRepairMQ TimerInstance { get; set; }
        private Timer StateTimer { get; set; }
        public int TimerInterval { get; set; }
        Thread Worker;
        AutoResetEvent StopRequest = new AutoResetEvent(false);
        enum RecordTypes
        {
            EMPTY = 0,
            SHOP,
            COUNTRY,
            DETAIL,
            ERRORLINE
        };

        enum FileTypes
        {
            SHOPFILE,
            COUNTRYFILE
        };

        enum ValidationRtns
        {
            OK = 0,
            BADSHOP,
            BADMODE,
            BADREPAIR,
            BADMANUAL,
            BADCOUNTRY,
            DBFAILURE,
            BADCURRENCY,
            BADDATERANGE
        };

        MESC2DSEntities objContext = new MESC2DSEntities();
        const string MQ_READ_MODE = "R";
        string QueueManager = null;
        string RequestQueue = null;
        string ReplyQueue = null;
        string Senderid = null;
        string receiverid = null;
        string aprfco = null;
        string aprferr = null;
        string ContractLogFilePath = null;
        string ConRprMQMessageFile = null;
        int count = 0;
        List<string> ContractFileRecList = new List<string>();
        FileStream LogFilefs;
        StreamWriter LogFile;

        bool bCheckData = false;
        string sCheckFileName = string.Empty;
        string sNewFileName = string.Empty;
        string sEMailAddress = string.Empty;
        string sDefaultEMailAddress = "<CENEMRERR@maersk.com> ";
        LogEntry logEntry = new LogEntry();
        bool bFileCheckOnly = true;
        List<string> ErrorMessageList = new List<string>();
        int nFileCount;
        FileTypes nFileType;
        List<string> WorkFile = new List<string>();
        string sError = string.Empty;
        string sDetail = string.Empty;
        string m_sCountryCode;
        string m_sShopCode;
        string m_sMode;
        string m_sRepairCode;
        string m_sManualCode;
        string m_sCurrencyCode;
        string m_sFromDate;
        string m_sToDate;
        string m_sAmount;
        //string sError = string.Empty;
        bool bChangesMade = false;

        string isEnterprise = "TRUE";
        #endregion

        public ContractRepairMQ()
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
            //          ContractRepair Load = new ContractRepair();
            /*  int nRetCode = 0;
              Load.nFileCount = 0;
              Load.bFileCheckOnly = true;
              Load.bChangesMade = false;
              Load.sEMailAddress = string.Empty;
              Load.bCheckData = false;*/
            try
            {
                log.Message = "Contract Repair Service started";
                Logger.Write(log);
                ContractRepairMQ ContractRepair = new ContractRepairMQ();

                ContractRepair.ReadConfig();

                string ContractFileName = ContractRepair.GenerateContractLogFilePathName();
                if (ContractFileName == null)
                {
                    //Write in application log file - Contract log file cannot open
                    //return false
                }

                ContractRepair.LogFilefs = File.Open(ContractFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                ContractRepair.LogFile = new System.IO.StreamWriter(ContractRepair.LogFilefs);

                if (ContractRepair.isEnterprise.ToUpper() == "TRUE")
                {
                    ContractRepair.bFileCheckOnly = false;
                    ContractRepair.bCheckData = true;

                    if (ContractRepair.ExtractContractData())
                    {
                        ContractRepair.ParseAndCreateFiles();
                    }
                }
                else
                {
                    ContractRepair.bCheckData = true; //as per current logic this comes from the command line arg
                    ContractRepair.ParseAndCreateFiles();
                }

                /*    for (int increment = 0; increment < ContractRepair.count; increment++)
                    {
                        if (ContractRepair.ExtractContractData())
                        {
                            ContractRepair.ParseAndCreateFiles();
                        }
                        else
                        {
                            ContractRepair.ParseAndCreateFiles();
                        }
                    }*/
                //@soumik there is no concept of count in contract

                ContractRepair.LogFile.Close();
                ContractRepair.LogFilefs.Close();
            }
            catch (Exception ex)
            {
                //Write in application log file - Unknown error while processing work orders
                log.Message = "Unknown error while processing shop contract";
                Logger.Write(log);
            }

            autoEvent.Set();
        }

        bool ExtractContractData()
        {
            try
            {

                ProcessQueue();
                if (ContractFileRecList.Count == 0)
                {
                    //Write in application logEntry file - No records found in MQ
                    logEntry.Message = "No records found in MQ";
                    Logger.Write(logEntry);
                    return false;
                }
                //else
                //{*/
                //create the WO logEntry file and print the WO records in the file in correct format


                // }

                using (FileStream Filefs = File.Open(ConRprMQMessageFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (System.IO.StreamWriter WOfile = new System.IO.StreamWriter(Filefs))
                    {
                        for (int i = 0; i < ContractFileRecList.Count; i++)
                        {
                            WOfile.WriteLine(ContractFileRecList[i]);
                        }
                        WOfile.Close();
                    }
                    Filefs.Close();
                }


                LogFile.WriteLine("File found");

                LogFile.WriteLine("extract started");
                for (int i = 0; i < ContractFileRecList.Count; i++)
                {
                    LogFile.WriteLine(ContractFileRecList[i]);
                }

                LogFile.WriteLine("Extract Work order completed.");

                if (ContractFileRecList.Count == 0)
                {
                    LogFile.WriteLine("Data Not Found");
                }
                LogFile.WriteLine("Extract completed.");


            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return false;
                //Write in application logEntry file - Failed to extract MQ message and write in file
            }


            return true;
        }

        void ParseAndCreateFiles()
        {
            bool success = false;
            string line = string.Empty;
            string RecordType = string.Empty;
            string WORecord = string.Empty;
            bool bHeaderFound = false;
            string sLine = string.Empty;
            string sHeaderHold = string.Empty;


            //LogFilefs = File.Open(ConfigurationManager.AppSettings["ConRprMQMessageFile"], FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            //StreamWriter LogFile = new System.IO.StreamWriter(LogFilefs);
            try
            {
                using (FileStream WOFileFs = File.Open(ConRprMQMessageFile, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (System.IO.StreamReader WOfile = new System.IO.StreamReader(WOFileFs))
                    {
                        while ((sLine = WOfile.ReadLine()) != null)
                        {
                            //if (line.Length < 10)
                            //    continue;

                            //RecordType = line.Substring(0, 3);   //Soumik not reqd as per current code
                            //WORecord = line.Substring(4);//line.Length - 1);

                            switch (GetRecordType(sLine))
                            {
                                case RecordTypes.EMPTY: break;

                                case RecordTypes.SHOP:
                                    if (!bFileCheckOnly)
                                    {
                                        if (!string.IsNullOrEmpty(sEMailAddress))
                                        {
                                            if (ErrorMessageList.Count > 0)
                                            {
                                                LogFile.WriteLine("SHOP::Sending error started.");
                                                //logEntry.Message = "SHOP::Sending error started.";
                                                //Logger.Write(logEntry);
                                                SendEmail(ErrorMessageList, sEMailAddress);
                                                LogFile.WriteLine("SHOP::Sending error complete.");
                                                //logEntry.Message = "SHOP::Sending error complete.";
                                                //Logger.Write(logEntry);
                                            }
                                        }
                                    }
                                    bHeaderFound = true;
                                    sHeaderHold = sLine;
                                    nFileType = FileTypes.SHOPFILE;
                                    ExtractEmailAddress(sLine, nFileType);
                                    WorkFile.Add(sLine);
                                    break;

                                case RecordTypes.COUNTRY:
                                    if (!bFileCheckOnly)
                                    {
                                        if (!string.IsNullOrEmpty(sEMailAddress))
                                        {
                                            if (ErrorMessageList.Count > 0)
                                            {
                                                LogFile.WriteLine("COUNTRY::Sending error started.");
                                                //logEntry.Message = "SHOP::Sending error started.";
                                                //Logger.Write(logEntry);
                                                SendEmail(ErrorMessageList, sEMailAddress);
                                                LogFile.WriteLine("COUNTRY::Sending error complete.");
                                                //logEntry.Message = "SHOP::Sending error complete.";
                                                //Logger.Write(logEntry);
                                            }
                                        }
                                    }
                                    bHeaderFound = true;
                                    sHeaderHold = sLine;
                                    nFileType = FileTypes.COUNTRYFILE;
                                    ExtractEmailAddress(sLine, nFileType);
                                    WorkFile.Add(sLine);
                                    break;

                                case RecordTypes.DETAIL:
                                    sLine = sLine.ToUpper();

                                    //logEntry.Message = "Under detail part";
                                    //Logger.Write(logEntry);
                                    LogFile.WriteLine("Under detail part");
                                    if (nFileType != FileTypes.COUNTRYFILE)
                                    {
                                        if (bHeaderFound)
                                        {
                                            LogFile.WriteLine("DETAIL::check general syntax");
                                            if (ParseDetail(sLine, nFileType))
                                            {
                                                if (bCheckData)
                                                {
                                                    // if validates, add for later Contract load processing
                                                    if (ValidateLine(sDetail, nFileType))
                                                    {
                                                        WorkFile.Add(sDetail);
                                                        // insert data here
                                                        try
                                                        {
                                                            LogFile.WriteLine("DETAIL::insert data here");
                                                            success = InsertData(sDetail, nFileType);
                                                            if (!success)
                                                            {
                                                                LogFile.WriteLine(sDetail); LogFile.WriteLine(sError);
                                                                ErrorMessageList.Add(sDetail + sError);
                                                            }
                                                            else
                                                            {
                                                                LogFile.WriteLine("DETAIL::Successfully inserted into database");
                                                            }

                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            LogFile.WriteLine(sDetail);
                                                            LogFile.WriteLine("DETAIL::Fail insert into contract table");
                                                            ErrorMessageList.Add(sDetail + " - Fail insert into contract table");
                                                            logEntry.Message = ex.ToString();
                                                            Logger.Write(logEntry);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        LogFile.WriteLine(sDetail); LogFile.WriteLine(sError);
                                                        ErrorMessageList.Add(sDetail + sError);
                                                    }
                                                }
                                                else	// just checking syntax, so add ok
                                                {
                                                    WorkFile.Add(sDetail);
                                                }
                                            }
                                            else
                                            {
                                                // non-correctable systax errors found
                                                ErrorMessageList.Add(sLine + sError);
                                            }
                                        }
                                        else
                                        {
                                            sLine += " - no header for detail in file";
                                            ErrorMessageList.Add(sLine);
                                        }
                                    }
                                    break;

                            }
                        }
                        LogFile.WriteLine("Reading file completed.");

                        LogFile.WriteLine("finally if from enterprise - send collected errors");
                        if (!bFileCheckOnly)
                        {
                            // if e-mail return address exists
                            if (!string.IsNullOrEmpty(sEMailAddress))
                            {
                                // if errors exist
                                if (ErrorMessageList.Count > 0)
                                {
                                    // INCOMPLETE - send errors
                                    LogFile.WriteLine("INCOMPLETE - send errors");
                                    SendEmail(ErrorMessageList, sEMailAddress);

                                    ErrorMessageList = new List<string>();
                                }
                            }
                        }


                        // finally at EOF save last file if existing.
                        // save for Contract Load module if good data exists.
                        //SaveFile();

                        LogFile.WriteLine("Parse complete");
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteLine("Fail insert into contract table.");

                ErrorMessageList.Add(sDetail + " - Fail insert into contract table");

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        void SaveFile()
        {

            // User checking format only
            if (bFileCheckOnly)
            {
                if (ErrorMessageList.Count() > 0)
                {
                    SaveToFile("errors.txt");
                    Console.WriteLine("\n\nNOTE: ERRORS FOUND, review file: errors.txt.\n", sCheckFileName);
                    Console.WriteLine("\nThe errors.txt file contains one or more line items which the repair module \nwas not able to correct.\n");
                    Console.WriteLine("Correct the errors in the file: %s\nand run the contractrepair module again.", sCheckFileName);
                    if (WorkFile.Count() > 1)
                    {
                        SaveToFile(sCheckFileName);
                        Console.WriteLine("Errors found - invalid records moved to errors.txt, please review \n");
                        Console.WriteLine("\nNote: file %s contains format corrections, please review. \nUse notepad for viewing.\n", sCheckFileName);
                    }
                }
                else if ((WorkFile.Count() > 1) && (bChangesMade))
                {
                    SaveToFile(sCheckFileName);
                    Console.WriteLine("\nNote: file %s contains format corrections, please review. \nUse notepad for viewing.\n", sCheckFileName);
                }
                else if ((WorkFile.Count > 1) && (!bChangesMade))
                {
                    SaveToFile(sCheckFileName);
                    Console.WriteLine("\nOK: No format errors found in file %s\n", sCheckFileName);
                }
            }
            else // is enterprise - save file for load to DB by MercContractLoad module
            {
                if (WorkFile.Count > 1)
                    SaveToFile(sCheckFileName);
            }
        }

        void SaveToFile(string nOutFile)
        {
            string tmp;
            FileStream fs;
            nOutFile = "MercContractLoad_Email.list";
            //CLogFile	Log( "PsLog.txt" );		// log errors..
            try
            {
                fs = new FileStream(nOutFile, FileMode.Create, FileAccess.Write);
                if (!fs.CanRead)
                {
                    tmp = "Unable to open file: ";
                    tmp += nOutFile;
                    //Log << tmp;
                    return;
                }
                else
                {
                    using (System.IO.StreamWriter WOfile = new System.IO.StreamWriter(fs))
                    {
                        for (int i = 0; i < ErrorMessageList.Count; i++)
                        {
                            WOfile.WriteLine(ErrorMessageList[i]);
                            WOfile.WriteLine(System.Environment.NewLine);
                        }
                        WOfile.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                tmp = "Unable to open file: ";
                tmp += nOutFile;
                //logEntry.Message = ex.ToString();
                //Logger.Write(logEntry);
                //Log << tmp;
                return;
            }

            //for (int i = 0; i < GetSize(); i++)
            //{
            //    tmp = GetAt(i);
            //    if (tmp.GetLength() > 0)
            //        if ((tmp.GetAt(tmp.GetLength() - 1)) != '\n') tmp += '\n';
            //        else
            //            tmp += '\n';
            //    fp << (LPCTSTR)tmp;
            //}

            fs.Close();
        }

        void Instruct()
        {
            Console.WriteLine("<ContractRepair usage:>\n\n");
            Console.WriteLine("To check a shop or country contract file, type: \n");
            Console.WriteLine("ContractRepair filename.cvs   <press enter>\n");
            Console.WriteLine("\nThe program will provide an errors.txt file for any errors found\n");
            Console.WriteLine("or re-write the file with possible minor format corrections.\n");
            Console.WriteLine("Use the newly formatted file to send to Merc+ only.\n");
            Console.WriteLine("\nNote that this module checks data format, it does not check if data is \n");
            Console.WriteLine("valid, example: it will not check if a shop code is valid.\n");

            Console.WriteLine("\n\n<For Merc+ system batch process only: (in-house with db connectivity)>\n\n");
            Console.WriteLine("To force enterprise EAP read of mailbox, use enterprise as first argument.\n");
            Console.WriteLine("The second argument is the  default e-mail address to return errors\n");
            Console.WriteLine("Example:\n");
            Console.WriteLine("ContractRepair enterprise myemail@someplace.com\n");
            Console.WriteLine("\nNote: When using module in-house only to check a file manually,\n");
            Console.WriteLine("Specify second argument as checkdata to force a data check as well as\n");
            Console.WriteLine("a syntax check. Example:\n");
            Console.WriteLine("ContractRepair filename.cvs  checkdata <press enter>\n");

        }

        bool SendEMailError(List<string> errorList, string sAddress)
        {
            string tmp = string.Empty;
            bool bOK = true;


            tmp = "To: <" + sAddress + ">";
            tmp += "\n";
            tmp += "Subject: Contract Load errors";
            tmp += "\n\n";
            tmp += "Error Results from MERC+ Contract Load, see attached file \n.\n";

            //sendFile.Add(tmp);

            // save email header file
            SaveToFile(ConfigurationManager.AppSettings["EMAILLIST"]);

            // save errors to content file for send. (c:\temp\MercContratLoad_ErrorLog.txt)
            SaveToFile(ConfigurationManager.AppSettings["EMAILERRORS"]);

            //try
            //{
            //    if(_spawnl( _P_WAIT, EMAILCOMMAND, EMAILCOMMAND, NULL )!=0)
            //    {
            //        bOK = false;
            //    }
            //}
            //catch(...)
            //{
            //    // dummy
            //}

            return bOK;
        }


        bool ValidateLine(string sLine, FileTypes nFileType)
        {
            bool bOK = true;
            //sError = string.Empty;
            int nRtn = 0;
            try
            {
                if (!ExtractFields(sLine, nFileType))
                {
                    sError = "Unable to parse fields for validation";
                    return (false);
                }

                nRtn = GetValidationResponse(nFileType);

                if (nRtn == (int)ValidationRtns.OK)
                {
                    nRtn = GetDateRange(nFileType);
                    if ((nRtn > 0) && (nRtn < (int)ValidationRtns.DBFAILURE))
                    {
                        nRtn = (int)ValidationRtns.BADDATERANGE;
                    }
                }
            }

            catch (Exception ex)
            {
                nRtn = (int)ValidationRtns.DBFAILURE;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            switch (nRtn)
            {
                case (int)ValidationRtns.OK:
                    break;
                case (int)ValidationRtns.BADSHOP:
                    sError = " - Shop code not found";
                    bOK = false;
                    break;
                case (int)ValidationRtns.BADMODE:
                    sError = " - Invalid Repair code, mode and manual combination";
                    bOK = false;
                    break;
                case (int)ValidationRtns.BADREPAIR:
                    sError = " - Repair code not found";
                    bOK = false;
                    break;
                case (int)ValidationRtns.BADMANUAL:
                    sError = " - Manual code not found";
                    bOK = false;
                    break;
                case (int)ValidationRtns.BADCOUNTRY:
                    sError = " - Country code not found";
                    bOK = false;
                    break;
                case (int)ValidationRtns.DBFAILURE:
                    bOK = false;
                    sError = " - database failure when performing validations";
                    break;
                case (int)ValidationRtns.BADDATERANGE:
                    bOK = false;
                    sError = " - contract dates overlap an existing contract";
                    break;

            }
            return bOK;
        }

        int GetValidationResponse(FileTypes nFileType)
        {
            try
            {
                if (nFileType == FileTypes.SHOPFILE)
                {
                    var shop = (from s in objContext.MESC1TS_SHOP
                                where s.SHOP_CD == m_sShopCode
                                select s).ToList();

                    if (shop != null && shop.Count > 0)
                    {
                        var repCode = (from rc in objContext.MESC1TS_REPAIR_CODE
                                       where rc.REPAIR_CD == m_sRepairCode &&
                                       rc.MODE == m_sMode &&
                                       rc.MANUAL_CD == m_sManualCode
                                       select rc).ToList();

                        if (repCode != null && repCode.Count > 0)
                        {
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    var country = (from c in objContext.MESC1TS_COUNTRY
                                   where c.COUNTRY_CD == m_sCountryCode
                                   select c).ToList();

                    if (country != null && country.Count > 0)
                    {
                        var repCode = (from rc in objContext.MESC1TS_REPAIR_CODE
                                       where rc.REPAIR_CD == m_sRepairCode &&
                                       rc.MODE == m_sMode &&
                                       rc.MANUAL_CD == m_sManualCode
                                       select rc).ToList();

                        if (repCode != null && repCode.Count > 0)
                        {
                            var currency = (from curr in objContext.MESC1TS_CURRENCY
                                            where curr.CUCDN == m_sCurrencyCode
                                            select curr).ToList();

                            if (currency != null && currency.Count > 0)
                            {
                                return 0;
                            }
                            else
                            {
                                return 7;
                            }
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        return 5;
                    }
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return 6;
            }
        }

        int GetDateRange(FileTypes nFileType)
        {
            int count = 0;
            string tempFromDate = m_sFromDate.Substring(0, 4) + "/" + m_sFromDate.Substring(4, 2) + "/" + m_sFromDate.Substring(6, 2);
            string tempToDate = m_sToDate.Substring(0, 4) + "/" + m_sToDate.Substring(4, 2) + "/" + m_sToDate.Substring(6, 2);
            DateTime ToDate = Convert.ToDateTime(tempToDate);
            DateTime FromDate = Convert.ToDateTime(tempFromDate);
            try
            {
                if (nFileType == FileTypes.SHOPFILE)
                {
                    count = (from cont in objContext.MESC1TS_SHOP_CONT
                             where cont.SHOP_CD == m_sShopCode &&
                             cont.MODE == m_sMode &&
                             cont.REPAIR_CD == m_sRepairCode &&
                             cont.EFF_DTE <= ToDate &&
                             cont.EXP_DTE >= FromDate
                             select cont).Count();

                }
                else
                {
                    count = (from cont in objContext.MESC1TS_COUNTRY_CONT
                             where cont.COUNTRY_CD == m_sCountryCode &&
                             cont.MODE == m_sMode &&
                             cont.REPAIR_CD == m_sRepairCode &&
                             cont.EFF_DTE <= ToDate &&
                             cont.EXP_DTE >= FromDate
                             select cont).Count();
                }
            }
            catch (Exception ex)
            {
                count = (int)ValidationRtns.DBFAILURE;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return count;
        }

        bool ExtractFields(string sLine, FileTypes nFileType)
        {
            int i = 0;
            bool bAllColumnsFilled = false;
            int size = 0;
            int idx = 0;
            string szRecord = null;
            bool bOK = true;

            if (nFileType == FileTypes.SHOPFILE)
            {
                //szRecord = strtok( szLine, sep );
                while (sLine.IndexOf(",") >= 0)
                {
                    int Delimidx = sLine.IndexOf(",");
                    size = sLine.Length;
                    szRecord = sLine.Substring(idx, Delimidx);
                    switch (i)
                    {	// Note: 
                        // Shop Code
                        case 0:
                            m_sShopCode = szRecord;
                            break;
                        // From date
                        case 1:
                            m_sFromDate = szRecord;
                            break;
                        // ToDate
                        case 2:
                            m_sToDate = szRecord;
                            break;
                        // Manual Code
                        case 3:
                            m_sManualCode = szRecord;
                            break;
                        // Mode
                        case 4:
                            m_sMode = szRecord;
                            break;
                        // Repair Code
                        case 5:
                            m_sRepairCode = szRecord;
                            break;
                        case 6:
                            m_sAmount = szRecord;
                            break;
                    }

                    // get next string - up index 
                    i++;
                    if (Delimidx + 2 > size)
                        break;
                    else
                    {
                        string tempMessage = sLine.Substring(Delimidx + 1, (size - Delimidx - 1));
                        sLine = tempMessage;
                    }
                }
                if (i == 6) //handle case if no , at the end
                {
                    m_sAmount = sLine;
                }
            }
            else // is a country detail line.
            {
                //szRecord = strtok( szLine, sep );
                while (sLine.IndexOf(",") >= 0)
                {
                    int Delimidx = sLine.IndexOf(",");
                    size = sLine.Length;
                    szRecord = sLine.Substring(idx, Delimidx);
                    switch (i)
                    {	// Note: 
                        // Country Code
                        case 0:
                            m_sCountryCode = szRecord;
                            break;
                        // From date
                        case 1:
                            m_sFromDate = szRecord;
                            break;
                        // ToDate
                        case 2:
                            m_sToDate = szRecord;
                            break;
                        // Manual Code
                        case 3:
                            m_sManualCode = szRecord;
                            break;
                        // Mode
                        case 4:
                            m_sMode = szRecord;
                            break;
                        // Repair Code
                        case 5:
                            m_sRepairCode = szRecord;
                            break;
                        case 6:
                            m_sAmount = szRecord;
                            break;
                        case 7:
                            m_sCurrencyCode = szRecord;
                            break;
                    }

                    // get next string - up index 
                    if (Delimidx + 2 > size)
                        break;
                    else
                    {
                        string tempMessage = sLine.Substring(Delimidx + 1, (size - Delimidx - 1));
                        sLine = tempMessage;
                    }
                    i++;
                }
                if (i == 7) //handle case if no , at the end
                {
                    m_sAmount = sLine;
                }
            }

            return bOK;
        }

        bool ParseDetail(string sLine, FileTypes nFileType)
        {
            bool success = true;
            sLine = sLine.ToUpper();

            if (!CheckForEmpties(sLine, nFileType))
            {
                sError = " - NULL required fields - line incomplete";
                return (false);
            }

            if (nFileType == FileTypes.SHOPFILE)
                success = ValidateShop(sLine);

            return success;
        }

        bool CheckForEmpties(string sLine, FileTypes nFileType)
        {
            bool bOK = true;
            int i = 0;
            bool bAllColumnsFilled = false;
            int size = 0;
            int idx = 0;
            string szRecord = null;

            try
            {
                while (sLine.IndexOf(",") >= 0)
                {
                    int Delimidx = sLine.IndexOf(",");
                    size = sLine.Length;
                    szRecord = sLine.Substring(idx, Delimidx).TrimEnd();

                    if (string.IsNullOrEmpty(szRecord))
                    {
                        if (i < 7)
                        {
                            if (string.IsNullOrEmpty(szRecord))
                            {
                                bOK = false;
                            }
                        }
                        else
                        {
                            if ((nFileType == FileTypes.COUNTRYFILE) && (i == 7))
                            {
                                if (string.IsNullOrEmpty(szRecord))
                                {
                                    bOK = false;
                                }
                            }
                        }
                    }
                    i++;
                    if (Delimidx + 2 > size)
                        break;
                    else
                    {
                        string tempMessage = sLine.Substring(Delimidx + 1, (size - Delimidx - 1));
                        sLine = tempMessage;
                    }
                }

                //handle case if no , at the end of record
                if ((nFileType == FileTypes.COUNTRYFILE && i == 6) || (nFileType == FileTypes.COUNTRYFILE && i == 7))
                {
                    if (string.IsNullOrEmpty(sLine))
                    {
                        if (i < 7)
                        {
                            if (string.IsNullOrEmpty(sLine))
                            {
                                bOK = false;
                            }
                        }
                        else
                        {
                            if ((nFileType == FileTypes.COUNTRYFILE) && (i == 7))
                            {
                                if (string.IsNullOrEmpty(sLine))
                                {
                                    bOK = false;
                                }
                            }
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                bOK = false;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return bOK;

        }

        bool ValidateShop(string sLine)
        {
            bool bAllColumnsFilled = false;
            bool bOK = true;
            bool bErrorFound = false;
            sError = string.Empty;
            sDetail = string.Empty;
            int i = 0;
            int size = 0;
            int idx = 0;
            string szRecord = null;
            string sFromDate = string.Empty;
            string sToDate = string.Empty;

            try
            {
                while (sLine.IndexOf(",") >= 0)
                {
                    int Delimidx = sLine.IndexOf(",");
                    size = sLine.Length;
                    szRecord = sLine.Substring(idx, Delimidx);

                    switch (i)
                    {
                        case 0:
                            sDetail = szRecord;
                            sDetail += ",";
                            break;
                        // From Date
                        case 1:
                            sFromDate = szRecord;

                            sDetail += szRecord;
                            sDetail += ",";
                            break;
                        // To Date
                        case 2:
                            sToDate = szRecord;

                            sDetail += szRecord;
                            sDetail += ",";
                            break;
                        // Manual Code
                        case 3:
                            if (szRecord.Length == 0)
                            {
                                bErrorFound = true;
                            }
                            else
                            {
                                sDetail += szRecord;
                                sDetail += ",";
                            }
                            break;
                        // Mode
                        case 4:
                            if (szRecord.Length > 2)
                            {
                                bErrorFound = true;
                                sError = "- Mode max length is 2 characters";
                            }
                            else
                            {
                                sDetail += szRecord;
                                sDetail += ",";
                            }
                            break;
                        // Repair Code
                        case 5:
                            if ((szRecord.Length >= 1) && (szRecord.Length <= 6))
                            {
                                sDetail += szRecord;
                                sDetail += ",";
                            }
                            else
                            {
                                bErrorFound = true;
                                sError = " - Repair code expected";
                            }
                            break;
                        // Material amount
                        case 6:
                            bAllColumnsFilled = true; // all values entered

                            // now check numerics
                            if (!IsNumeristring(szRecord))
                            {
                                bErrorFound = true;
                                sError = " - Amount must contain 2 decimals and be separated by period (.) not comma (,)";
                            }
                            else
                            {
                                if (!ContainsDecimals(szRecord))
                                {
                                    bErrorFound = true;
                                    sError = " - Amount must contain 2 decimals and be separated by period (.) not comma (,)";
                                }
                                else
                                    sDetail += szRecord;
                            }
                            break;
                    }
                    i++;
                    if (Delimidx + 2 > size)
                        break;
                    else
                    {
                        string tempMessage = sLine.Substring(Delimidx + 1, (size - Delimidx - 1));
                        sLine = tempMessage;
                    }
                }

                if (i == 6)//handle case if no , at the end
                {

                    bAllColumnsFilled = true; // all values entered

                    // now check numerics
                    if (!IsNumeristring(sLine))
                    {
                        bErrorFound = true;
                        sError = " - Amount must contain 2 decimals and be separated by period (.) not comma (,)";
                    }
                    else
                    {
                        if (!ContainsDecimals(sLine))
                        {
                            bErrorFound = true;
                            sError = " - Amount must contain 2 decimals and be separated by period (.) not comma (,)";
                        }
                        else
                            sDetail += sLine;
                    }
                }

                // do date checks.
                if (!ValidateDateRange(sFromDate, sToDate))
                {
                    bErrorFound = true;
                }

                // if not all columns ok or filled, .
                if (!bAllColumnsFilled)
                {
                    bErrorFound = true;
                    sError = " <<< Data incomplete >>>";
                }

                if (bErrorFound) bOK = false;
            }
            catch(Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                bOK = false;
                sError = "Unable to parse string";
            }
            return bOK;
        }

        void ExtractEmailAddress(string sLine, FileTypes nfileType)
        {
            int i = 0;
            sEMailAddress = string.Empty;
            bool bOK = true;
            bool bErrorFound = false;
            string szRecord = null;
            int size = 0;
            int idx = 0;

            while (sLine.IndexOf(",") >= 0)
            {
                int Delimidx = sLine.IndexOf(",");
                size = sLine.Length;
                szRecord = sLine.Substring(idx, Delimidx);

                switch (i)
                {
                    case 7: //as per file @sb validate with real msges
                        if (nfileType == FileTypes.SHOPFILE)
                            sEMailAddress = szRecord;
                        break;

                    case 8://as per file @sb validate with real msges
                        if (nfileType == FileTypes.COUNTRYFILE)
                            sEMailAddress = szRecord;
                        break;
                }

                if (Delimidx + 2 > size)
                    break;
                else
                {
                    string tempMessage = sLine.Substring(Delimidx + 1, (size - Delimidx - 1));
                    sLine = tempMessage;
                }

                i++;
            }

            //hanndle case if delimeter is not present at end of record
            if ((nfileType == FileTypes.SHOPFILE && i == 7) || (nfileType == FileTypes.COUNTRYFILE && i == 8))
                sEMailAddress = sLine;



            if (string.IsNullOrEmpty(sEMailAddress))
            {
                sEMailAddress = sDefaultEMailAddress;
            }

        }

        public void SendEmail(List<string> ErrorMessageList, string EmailAddress)
        {
            try
            {
                CreateErrorFile();
                MailMessage mail = new MailMessage();
                mail.Priority = MailPriority.Normal;
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"]);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["FromEmailAddress"]);
                mail.To.Add(EmailAddress);
                mail.Subject = "Contract Load errors";
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(ConfigurationManager.AppSettings["EMAILERRORS"]);
                mail.Attachments.Add(attachment);
                //foreach (var msg in ErrorMessageList)
                //{
                //    mail.Body += msg;
                //    mail.Body += System.Environment.NewLine;
                //}

                SmtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                // SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
                //  SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                logEntry.Message = "Unknown error while sending mail";
                Logger.Write(logEntry);
            }

        }

        void CreateErrorFile()
        {
            FileStream fs;
            try
            {
                if (System.IO.File.Exists(ConfigurationManager.AppSettings["EMAILERRORS"]))
                {

                    System.IO.File.Delete(ConfigurationManager.AppSettings["EMAILERRORS"]);

                }

                fs = new FileStream(ConfigurationManager.AppSettings["EMAILERRORS"], FileMode.OpenOrCreate, FileAccess.Write);
                using (System.IO.StreamWriter WOfile = new System.IO.StreamWriter(fs))
                {
                    for (int i = 0; i < ErrorMessageList.Count; i++)
                    {
                        WOfile.WriteLine(ErrorMessageList[i]);
                        WOfile.WriteLine(System.Environment.NewLine);
                    }
                    WOfile.Close();
                }



            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return;
            }
        }
 

        void ReadConfig() //to be read from config file
        {
            QueueManager = ConfigurationManager.AppSettings["MQManagerQueueName"];
            ReplyQueue = ConfigurationManager.AppSettings["RequestQueue"];
            RequestQueue = ConfigurationManager.AppSettings["ReplyQueue"];
            Senderid = ConfigurationManager.AppSettings["Senderid"];
            receiverid = ConfigurationManager.AppSettings["receiverid"];
            aprfco = ConfigurationManager.AppSettings["aprfco"];
            aprferr = ConfigurationManager.AppSettings["aprferr"];
            ContractLogFilePath = ConfigurationManager.AppSettings["ConRprLogFilePath"];
            ConRprMQMessageFile = ConfigurationManager.AppSettings["ConRprMQMessageFile"];
            isEnterprise = ConfigurationManager.AppSettings["IsEnterprise"];
            sDefaultEMailAddress = ConfigurationManager.AppSettings["DefaultEmailAddress"];
        }

        string GenerateContractLogFilePathName()
        {
            try
            {

                string ContractLogFilePathName = ContractLogFilePath;
                DateTime CurrentDateTime = DateTime.Now;

                if (ContractLogFilePathName.LastIndexOf("\\") != ContractLogFilePathName.Length - 1)
                    ContractLogFilePathName += "\\";

                ContractLogFilePathName += CurrentDateTime.ToString("dd-MM-yyyy");

                bool Exists = System.IO.Directory.Exists(ContractLogFilePathName);
                if (!Exists)
                    Directory.CreateDirectory(ContractLogFilePathName);

                ContractLogFilePathName += "\\ContractRepair-";
                ContractLogFilePathName += CurrentDateTime.ToString("hh-mm-ss");
                ContractLogFilePathName += ".txt";

                return ContractLogFilePathName;

            }
            catch (Exception ex)
            {
                //Application logEntry -  Error generating the EDILogFile
                logEntry.Message = "Unknown error while processing shop contract";
                Logger.Write(logEntry);
                return null;
            }

        }

        public void ProcessQueue() //read messages from MQ and write in file same as ExtractFile
        {
            MQManager MQmgr = new MQManager();
            MQQueueManager Qmgr = null;
            MQQueue Qqueue = null;
            MQMessage MQMessage = null;

            Qmgr = MQmgr.OpenQueueManager(QueueManager);
            Qqueue = MQmgr.OpenQ(MQ_READ_MODE, Qmgr, RequestQueue);

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
                    logEntry.Message = "A problem occured while retrieving the MQ message: " + mqException.Message.ToString();
                    Logger.Write(logEntry);
                    //Write logEntry - A problem occured while retrieving the MQ message: " + mqException.Message.ToString()  
                    //logEntry.Message = mqException.ToString();
                    // Logger.Write(logEntry);
                }
            }

            MQmgr.CloseQ(Qqueue);
            MQmgr.DisconnectQueueManager(Qmgr);
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
                    ContractFileRecList.Add(szRecord);
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

        RecordTypes GetRecordType(string RecordType)
        {
            string temp = RecordType.ToUpper();
            if (string.IsNullOrEmpty(temp)) return RecordTypes.EMPTY;

            // Check if shop header
            if (temp.Contains("SHOP")) return RecordTypes.SHOP;

            // Check if country header
            if (temp.Contains("COUNTRY")) return RecordTypes.COUNTRY;

            return RecordTypes.DETAIL;
        }

        // weak numeric check...
        private bool IsNumeristring(string s)
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

        // check that value contains decimal with 2 decimal values after eg: (1.00 ok) (1.0 not ok)
        bool ContainsDecimals(string sNumber)
        {
            string s = sNumber;
            s = s.TrimEnd();

            int iPos = s.IndexOf(".");
            if (iPos == -1)
                return (false);
            // ensure 2 decimal places
            string sDecPlaces = s.Substring(iPos + 1);
            if (sDecPlaces.Length != 2)
                return (false);

            return (true);
        }

        bool ValidateDateRange(string sFromDate, string sToDate)
        {
            bool bOK = true;
            DateTime dateFrom = DateTime.MinValue;
            DateTime dateTo = DateTime.MinValue;
            DateTime CurrDate;

            int nYr, nDy, nMt;


            try
            {
                if (!string.IsNullOrEmpty(sFromDate))
                {
                    nYr = Int32.Parse(sFromDate.Substring(0, 4));
                    nMt = Int32.Parse(sFromDate.Substring(4, 2));
                    nDy = Int32.Parse(sFromDate.Substring(6, 2));
                    dateFrom = new DateTime(nYr, nMt, nDy, 0, 0, 0);
                }
                // check if valid date
                //if (dateFrom.GetStatus() != 0)
                //{
                //    sError = " - Invalid from date";
                //    return (false);
                //}
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                sError = " - Invalid from date - general error";
                return (false);
            }

           
            try
            {
                if (!string.IsNullOrEmpty(sToDate))
                {
                    nYr = Int32.Parse(sToDate.Substring(0, 4));
                    nMt = Int32.Parse(sToDate.Substring(4, 2));
                    nDy = Int32.Parse(sToDate.Substring(6, 2));

                    dateTo = new DateTime(nYr, nMt, nDy, 0, 0, 0);
                    // check if valid date
                    //if (dateTo.GetStatus() != 0)
                    //{
                    //    sError = " - Invalid to date";
                    //    return (false);
                    //}
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                sError = " - Invalid to date";
                return (false);
            }
                             

            if (!string.IsNullOrEmpty(sFromDate) && !(string.IsNullOrEmpty(sToDate)))
            {
                if (dateFrom >= dateTo)
                {
                    sError = " - From date must be less than the to date";
                    return (false);
                }   

                // get current date - set time to zeros;
                CurrDate = DateTime.Now;
                nYr = CurrDate.Year;
                nDy = CurrDate.Day;
                nMt = CurrDate.Month;
                try
                {
                    CurrDate = new DateTime(nYr, nMt, nDy, 0, 0, 0);
                }
                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    sError = " - unable to compare date to current date";
                    return (false);
                }
                if (dateFrom < CurrDate)
                {
                    sError = " - From date must be greater or equal to current date";
                    return (false);
                }

            }

            return (true);
        }

        bool InsertData(string sLine, FileTypes nFileType)
        {
            bool success = true;
            MESC1TS_SHOP_CONT ShopCont = new MESC1TS_SHOP_CONT();
            try
            {
                // parse out fields
                ExtractFields(sLine, nFileType);

                if (nFileType == FileTypes.SHOPFILE)
                {
                    string tempEffDate = m_sFromDate.Substring(0, 4) + "/" + m_sFromDate.Substring(4, 2) + "/" + m_sFromDate.Substring(6, 2);
                    string tempExpDate = m_sToDate.Substring(0, 4) + "/" + m_sToDate.Substring(4, 2) + "/" + m_sToDate.Substring(6, 2);
                    ShopCont.SHOP_CD = m_sShopCode;
                    ShopCont.MODE = m_sMode;
                    ShopCont.REPAIR_CD = m_sRepairCode;
                    ShopCont.CONTRACT_AMOUNT = Convert.ToDecimal(m_sAmount);
                    ShopCont.EFF_DTE = Convert.ToDateTime(tempEffDate);
                    ShopCont.CHUSER = "ContractLoad";
                    ShopCont.EXP_DTE = Convert.ToDateTime(tempExpDate).AddHours(23).AddMinutes(59);//Kasturee_shop_cont-29-01-2019
                    ShopCont.MANUAL_CD = m_sManualCode;
                    ShopCont.CHTS = DateTime.Now;
                    objContext.MESC1TS_SHOP_CONT.Add(ShopCont);
                    objContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }
            return success;
        }


    }
}
