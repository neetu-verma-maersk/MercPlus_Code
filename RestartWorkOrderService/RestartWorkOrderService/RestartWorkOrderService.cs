using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Web.Administration;
using System.Threading;
using RestartWorkOrderService.WorkOrderServiceReference;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace RestartWorkOrderService
{
    public partial class WorkOrderRestart : ServiceBase
    {
        private AutoResetEvent AutoEventInstance { get; set; }
        private WorkOrderRestart TimerInstance { get; set; }
        private Timer StateTimer { get; set; }
        public int TimerInterval { get; set; }
        Thread Worker;
        AutoResetEvent StopRequest = new AutoResetEvent(false);
        LogEntry log = new LogEntry();

        public WorkOrderRestart()
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
                if (StopRequest.WaitOne(600000))
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
            var server = new ServerManager();
            log.Message = "Service started..";
            Logger.Write(log);
            var site = server.Sites.FirstOrDefault(s => s.Name == "ManageWorkOrderService");
            try
            {

                ManageWorkOrderClient obj = new ManageWorkOrderClient();
                bool result = obj.GetServiceStatus();
                if (result)
                {
                    log.Message = "ManageWorkOrder Service is running properly";
                    Logger.Write(log);
                }

            }
            catch (Exception ex)
            {
                log.Message = ex.ToString();
                Logger.Write(log);
                if (site != null)
                {
                    if (site.State == ObjectState.Stopped)
                    {
                        site.Start();
                        log.Message = "MangeWorkOrder Service in stopped state. Service started!!!";
                        Logger.Write(log);
                    }
                    else
                    {
                        site.Stop();
                        log.Message = "MangeWorkOrder stopped!!!";
                        Thread.Sleep(3000);
                        log.Message = "Waiting for 3 secs";
                        Logger.Write(log);
                        site.Start();
                        log.Message = "MangeWorkOrder started!!!";
                        Logger.Write(log);
                    }

                }
                else
                {
                    log.Message = "MangeWorkOrder not found!!!";
                    Logger.Write(log);
                }

            }

            autoEvent.Set();
        }


       

    }
}
