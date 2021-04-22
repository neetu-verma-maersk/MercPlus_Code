using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.IO;

namespace Merc_INLC
{
    class Program
    {
        public static LogEntry logEntry = new LogEntry();
        public static string sFileName = string.Empty;
        public static Merc_INLC.INLCService Mserv = new INLCService();

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                logEntry.Message = "MercINLC called without filename passed in parm - unable to continue.";
                Logger.Write(logEntry);
            }
            else
            {	// get file name to extract from.
                sFileName = args[0];
                logEntry.Message = "MercINLC called with filename " + sFileName + ".";
                Logger.Write(logEntry);
                Mserv.doServiceWork(sFileName);
            }
        }
    }
}
