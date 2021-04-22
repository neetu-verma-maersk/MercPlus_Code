using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.WMQ;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MercPlusLibrary;

namespace MQSUURA
{
    class MercMQSUURA
    {
        const string MQ_READ_MODE = "R";
        const string HEADER_ID = "AUMQBMM ";
        string QmanagerName;
        string QName;
        string FileNameWithPath;
        string ProcessNameWithPath;
        LogEntry logEntry = new LogEntry();


        static void Main(string[] args)
        {
            
            LogEntry log = new LogEntry();
            try
            {
                log.Message = "MQSUURA started";
                Logger.Write(log);
                MercMQSUURA MercMQSUURA = new MercMQSUURA();
                MercMQSUURA.ReadConfiguration();
                MercMQSUURA.ProcessQueue();
                MercMQSUURA.StartFileProcessing();
                
            }
            catch (Exception ex)
            {
                log.Message = ex.ToString();
                Logger.Write(log);
            }
        }
 

        //@Soumik
        public bool ReadConfiguration() //read configuration from config file
        {
            QmanagerName = ConfigurationManager.AppSettings["MQManagerName"];   //"MERCDEV1";
            QName = ConfigurationManager.AppSettings["MQueueName"];  // "MERCDEV.EXRT";
            FileNameWithPath = ConfigurationManager.AppSettings["OutputFileName"] ; // "D:\\Mercplus\\Work\\EXRT.txt";
            ProcessNameWithPath = ConfigurationManager.AppSettings["ProcessName"]; //"D:\SBH122\Merc_INLC\Merc_INLC\bin\Debug\Merc_INLC.exe";
            return true;
        }

        public void ProcessQueue() //read messages from MQ and write in file
        {
            MQManager MQmgr = new MQManager();
            MQQueueManager Qmgr = null;
            MQQueue Qqueue = null;
            MQMessage MQMessage = null;
            LogEntry logEntry = new LogEntry();
            Qmgr = MQmgr.OpenQueueManager(QmanagerName);
            Qqueue = MQmgr.OpenQ(MQ_READ_MODE, Qmgr, QName);

            bool blnContinue = true;
            while (blnContinue)
            {
                try
                {
                    MQMessage = new MQMessage();
                    MQmgr.GetMessage(Qqueue, ref MQMessage, false);
                    if (MQMessage.Format.CompareTo(MQC.MQFMT_STRING) == 0)
                    {
                        ParseAndProcessRecords(MQMessage.ReadString(MQMessage.MessageLength));
                    }
                    else
                    {
                        // Error Message - The MQ message is a Non-text message + mqMessage.MessageSequenceNumber.ToString()
                    }
                }
                catch (MQException mqException)
                {
                    if (mqException.Reason != MQC.MQRC_NO_MSG_AVAILABLE)
                    {
                        //Write log - A problem occured while retrieving the MQ message: " + mqException.Message.ToString()  
                        logEntry.Message = mqException.ToString();
                        Logger.Write(logEntry);
                    }
                    blnContinue = false;
                }

            }

            MQmgr.CloseQ(Qqueue);
            MQmgr.DisconnectQueueManager(Qmgr);
        }

        public void ParseAndProcessRecords(string Message)
        {

            int Headeridx = Message.IndexOf(HEADER_ID);
            if (Headeridx >= 0)
                return;    //Header found
            else
            {
                int idx = 0;
                int size = 0;
                string szRecord = string.Empty;
                //LogEntry logEntry = new LogEntry();
                try
                {
                    using (FileStream fs = File.Open(FileNameWithPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(fs))
                        {
                            while (Message.IndexOf("*") >= 0)
                            {
                                int Delimidx = Message.IndexOf("*");
                                size = Message.Length;
                                szRecord = Message.Substring(idx, Delimidx + 1);
                                file.WriteLine(szRecord);
                                if (Delimidx + 2 > size)
                                    break;
                                else
                                {
                                    string tempMessage = Message.Substring(Delimidx + 1, (size - Delimidx - 1));
                                    Message = tempMessage;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }
            }
        }
        void StartFileProcessing()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = ProcessNameWithPath;
                startInfo.Arguments = FileNameWithPath;
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

    }
}
