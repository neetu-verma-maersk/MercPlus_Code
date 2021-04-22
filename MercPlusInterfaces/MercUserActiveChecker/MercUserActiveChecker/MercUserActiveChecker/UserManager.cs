using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercUserActiveChecker.ManageUserServiceReference;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace MercUserActiveChecker
{
    class UserManager
    {
        private LogEntry logEntry = new LogEntry();
        public void UpdateUserActiveStatus()
        {
            try
            {
                logEntry.Message = "Update user's inactive status - started.";
                Logger.Write(logEntry);

                string Message = "";      
                string days = ConfigurationManager.AppSettings["ExpirationDay"] == null ? "0" : ConfigurationManager.AppSettings["ExpirationDay"].ToString();

                int daysvalue = 0;
                if (!int.TryParse(days, out daysvalue))
                { 
                    daysvalue = 0;
                }

                logEntry.Message = "Inactive days count : " + days;
                Logger.Write(logEntry);

                ManageUserClient objUserClient = new ManageUserClient();
                objUserClient.UpdateUserActiveStatus(daysvalue, out Message);

                logEntry.Message = Message;
                Logger.Write(logEntry);

                logEntry.Message = "Update user's inactive status - completed.";
                Logger.Write(logEntry);
            }            
            catch (Exception ex)
            {                
                logEntry.Message = "Update user's inactive status - error - " + ex.Message;
                Logger.Write(logEntry);
            }
        }
    }
}
