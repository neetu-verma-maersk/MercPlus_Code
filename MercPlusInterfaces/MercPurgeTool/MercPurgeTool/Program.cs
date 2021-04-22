using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.IO;

namespace MercPurgeTool
{
    class Program
    {
        public static LogEntry logEntry = new LogEntry();
        public static MercPurgeTool.InIOperations iniOp = new InIOperations();
        public static PurgeModel pModel = null;
        public string FactPath { get; set; }
        public string EdiPath { get; set; }
        public string Facttoken { get; set; }
        public string Editoken { get; set; }
        public string Factdate { get; set; }
        public string Edidate { get; set; }
        public string Path { get; set; }

        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    logEntry.Message = "MercPurgeTool called without parameters passed in param - unable to continue.";
                    Logger.Write(logEntry);
                    return;
                }
                else
                {
                    logEntry.Message = "MercPurgeTool called with parameter passed in parm - " + args[0];
                    Logger.Write(logEntry);

                    if (!GetRegistrySettings())
                    {
                        logEntry.Message = "MercPurgeTool : GetRegistrySettings failed.";
                        Logger.Write(logEntry);
                        return;
                    }

                    if (string.Equals(args[0], "fact", StringComparison.OrdinalIgnoreCase))
                    {
                        pModel.Path = pModel.FactPath;
                        if (string.Equals(pModel.Facttoken, "y", StringComparison.OrdinalIgnoreCase))
                        {
                            DirToken(pModel.Path, true);
                        }
                        else
                        {
                            if (string.Equals(pModel.Factdate, "y", StringComparison.OrdinalIgnoreCase))
                            {
                                DirDate(pModel.Path, true);
                            }
                        }
                    }
                    else if (string.Equals(args[0], "edi", StringComparison.OrdinalIgnoreCase))
                    {
                        pModel.Path = pModel.EdiPath;
                        if (string.Equals(pModel.Editoken, "y", StringComparison.OrdinalIgnoreCase))
                        {
                            DirToken(pModel.Path, true);
                        }
                        else
                        {
                            if (string.Equals(pModel.Edidate, "y", StringComparison.OrdinalIgnoreCase))
                            {
                                DirDate(pModel.Path, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = "MercPurgeTool : Main : Exception :" + ex.Message;
                Logger.Write(logEntry);
            }

            logEntry.Message = "MercPurgeTool : Completed.";
            Logger.Write(logEntry);
        }

        private static void DirToken(string sPath, bool recurse)
        {
            //  throw new NotImplementedException();
        }

        private static void DirDate(string sPath, bool recurse)
        {
            logEntry.Message = "MercPurgeTool : DirDate : " + sPath;
            Logger.Write(logEntry);

            string[] subfolders = Directory.GetDirectories(sPath);
            foreach (string s in subfolders)
            {
                string dName = new DirectoryInfo(System.IO.Path.GetFileNameWithoutExtension(s)).Name;
                DateTime folderDt = DateTime.Parse(GetDDMMYYYY(dName));
                if (folderDt != null)
                {
                    if ((DateTime.Now - folderDt).Days >= 90)
                    {
                        DeleteDirectory(s, recurse);
                        DirDate(sPath, recurse);
                    }
                }
            }
        }

        private static string GetDDMMYYYY(string dtDDMMYYYY)
        {
            DateTime lclDateTime = new DateTime();
            //lclDateTime = DateTime.ParseExact(dtDDMMYYYY, "MM-dd-yyyy", null);
            //return lclDateTime.ToString("dd-MM-yyyy"); 
            lclDateTime = DateTime.ParseExact(dtDDMMYYYY, "dd-MM-yyyy", null);
            return lclDateTime.ToString();

        }

        private static void DeleteDirectory(string path, bool recursive)
        {
            try
            {
                // Delete all files and sub-folders?
                if (recursive)
                {
                    // Yep... Let's do this
                    var subfolders = Directory.GetDirectories(path);
                    foreach (var s in subfolders)
                    {
                        DeleteDirectory(s, recursive);
                    }
                }

                // Get all files of the folder
                var files = Directory.GetFiles(path);
                foreach (var f in files)
                {
                    // Get the attributes of the file
                    var attr = File.GetAttributes(f);

                    // Is this file marked as 'read-only'?
                    if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {

                        // Yes... Remove the 'read-only' attribute, then
                        File.SetAttributes(f, attr ^ FileAttributes.ReadOnly);
                    }

                    // Delete the file
                    File.Delete(f);
                }

                // When we get here, all the files of the folder were
                // already deleted, so we just delete the empty folder
                Directory.Delete(path, true);
            }
            catch
            { }
        }

        private static bool GetRegistrySettings()
        {
            if (pModel == null)
                pModel = new PurgeModel();
            else
                pModel = pModel.Empty();

            pModel.FactPath = ReadAppSettings.ReadSetting("fact_path");
            if (string.IsNullOrEmpty(pModel.FactPath))
                return false;
            pModel.EdiPath = ReadAppSettings.ReadSetting("edi_path");
            if (string.IsNullOrEmpty(pModel.EdiPath))
                return false;
            pModel.Facttoken = ReadAppSettings.ReadSetting("fact_token");
            if (string.IsNullOrEmpty(pModel.Facttoken))
                return false;
            pModel.Editoken = ReadAppSettings.ReadSetting("edi_token");
            if (string.IsNullOrEmpty(pModel.Editoken))
                return false;
            pModel.Factdate = ReadAppSettings.ReadSetting("fact_date");
            if (string.IsNullOrEmpty(pModel.Factdate))
                return false;
            pModel.Edidate = ReadAppSettings.ReadSetting("edi_date");
            if (string.IsNullOrEmpty(pModel.Edidate))
                return false;

            return true;
        }
    }
}
