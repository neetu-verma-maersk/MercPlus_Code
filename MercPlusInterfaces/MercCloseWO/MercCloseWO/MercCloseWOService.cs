using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MercCloseWO
{
    public partial class MercCloseWOService : ServiceBase
    {
        private AutoResetEvent AutoEventInstance { get; set; }
        private MercCloseWOService TimerInstance { get; set; }
        private Timer StateTimer { get; set; }
        public int TimerInterval { get; set; }
        Thread Worker;
        AutoResetEvent StopRequest = new AutoResetEvent(false);
        public static LogEntry logEntry = new LogEntry();
        public MercCloseWOService()
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
            TimerInterval = Convert.ToInt32(ConfigurationManager.AppSettings["TimeInterval"]); //This is the time interval between each run value will come from config file
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
                if (StopRequest.WaitOne(10000))
                    return;
                //Wait until the job is done
                AutoEventInstance.WaitOne();
                //Wait for 10 second before starting the job again.
                StateTimer.Change(TimerInterval, Timeout.Infinite);
            }

        }

        public void StartOperation(Object stateInfo)//this is function (delegate) where the actuall operation starts and executed after each time interval
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;

            try
            {
                logEntry.Message = "MercCloseWOService started";
                Logger.Write(logEntry);
                MercCloseWOUpdate objWOUpdate = new MercCloseWOUpdate();
                objWOUpdate.MercWorkOrderUpdate();
                  
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }



            autoEvent.Set();
        }
    }
}
