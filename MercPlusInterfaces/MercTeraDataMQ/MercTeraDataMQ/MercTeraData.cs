using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Collections;
using System.Configuration;
using MercPlusLibrary;
using IBM.WMQ;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace MercTeraDataMQ
{
    class MercTeraData
    {
        public const string MQ_WRITE_MODE = "W";
        public static string queueManager = new string(new char[25]);
        public static string outputQueueName = new string(new char[25]);
        public static string path = ConfigurationManager.AppSettings["DirPath"];
        public static LogEntry logEntry = new LogEntry();
        MQManager mqMgr = new MQManager();
        MQQueueManager qMgr = null;
        MQQueue qQueue  = null;
        
        public static void Main(string[] args)
        {
            try
            {
                MercTeraData mercTeraData = new MercTeraData();
                logEntry.Message = "BI Service started";
                Logger.Write(logEntry);
                string newPath = mercTeraData.FolderConstruction(path);
               
                foreach (string arg in args)
                {
                    Console.WriteLine(arg);
                    mercTeraData.ExtractParm(arg);           
                }
                mercTeraData.DirSearch(newPath);

            }
            catch(Exception ex)
            {                             
                Logger.Write( ex.ToString());
            }
            

          
        }
        public string FolderConstruction(string path)
        {
            //create the file path 
            //The code sets the file path to read the xml from. For the date 30-09-2015,
            //the file path will be ..\\MercFactUploadXMLStore\\29-09-2015.

            DateTime currentDate = DateTime.Now;
            
            DateTime previousDate=DateTime.Today.AddDays(-1);
            string previousDateformatted = previousDate.ToString("dd-MM-yyyy");
            string filePath = path.Trim() + previousDateformatted.Trim();
            return filePath;
        }
 
        public void ExtractParm( string lpzValue)
        {
            //Extract the name of QueueManager Name and Output Queue Name
            string p;

            if (lpzValue.Length < 4)
                return; // if no parm, exit
            
           // p = lpzValue[2].ToString(); // Set p=value
            p = lpzValue.Substring(2, lpzValue.Length-2);
            /* populate appropriate variables */
            switch (char.ToUpper(lpzValue[1]))
            {
                case 'M':
                    queueManager = p;
                    Console.WriteLine("QueueManager Name");
                    Console.WriteLine(queueManager);
                    break; // Queue Manager

           
                case 'O':
                    outputQueueName = p;
                    Console.WriteLine("Output Queue Name");
                    Console.WriteLine(outputQueueName);
                    break; // Output Queue Name
                
                default:
                    break;
            }
        }

        public void DirSearch(string sDir)
        {

            //code reads all the xml files in the location ..\MercFactUploadXMLStore\(yesterday’s date), 
            //stores them  and  send to BI
            try
            {
                bool success = false;
                
                qMgr = mqMgr.OpenQueueManager(queueManager);
                qQueue = mqMgr.OpenQ(MQ_WRITE_MODE, qMgr, outputQueueName);
                if (Directory.Exists(sDir))
                {

                    
                    string[] files = Directory.GetFiles(sDir, "*.xml", SearchOption.AllDirectories);

                    for (int i = 0; i <= files.Length - 1; i++)
                    {
                        using (StreamReader streamReader = new StreamReader(files[i], Encoding.UTF8))
                        {
                            string readContents = streamReader.ReadToEnd();

                            success = mqMgr.PutMessage(qQueue, readContents);
                        }
                    }
                }
                else
                {
                    logEntry.Message = "No Message to sent ";
                    Logger.Write(logEntry);
                }
                if (success)
                {
                    // Do logging queue name, process etc.
                    logEntry.Message = "Messages sent successfully" + "MQ Manager=" + queueManager + "QueName= " + outputQueueName;
                    Logger.Write(logEntry);

                }

                mqMgr.CloseQ(qQueue);
                mqMgr.DisconnectQueueManager(qMgr);

                
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString());
            }
        }
    }
}
