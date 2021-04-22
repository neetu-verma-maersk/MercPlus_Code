using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace MercUserActiveChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.SetLogWriter(new LogWriterFactory().Create());
            UserManager userManager = new UserManager();
            userManager.UpdateUserActiveStatus();
        }        
    }
}
