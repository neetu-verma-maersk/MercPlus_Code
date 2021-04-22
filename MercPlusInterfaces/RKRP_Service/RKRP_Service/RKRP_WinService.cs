using MercPlusLibrary;
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

namespace RKRP_Service
{
    public partial class RKRP_WinService : ServiceBase
    {
        RKRP_Operations rkrpOp = new RKRP_Operations();
        List<RepairDateRecord> WODateList = new List<RepairDateRecord>();
        private AutoResetEvent AutoEventInstance { get; set; }
        private RKRP_WinService TimerInstance { get; set; }
        private Timer StateTimer { get; set; }
        public int TimerInterval { get; set; }
        Thread Worker;
        AutoResetEvent StopRequest = new AutoResetEvent(false);
        public static LogEntry logEntry = new LogEntry();
        List<WorkOrderDetail> lstElgbleWOs = null;

        public RKRP_WinService()
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
            TimerInterval = Convert.ToInt32(ReadAppSettings.ReadSetting("PollTime"));// 1200000 seconds,20min //This is the time interval between each run value will come from config file
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
            // TODO: Insert monitoring activities here.           

            logEntry.Message = "StartOperation : Getting WorkOrders list...load xmit wo's";
            Logger.Write(logEntry);
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            try
            {
                lstElgbleWOs = new List<WorkOrderDetail>();
                lstElgbleWOs = rkrpOp.GetElligibleWOs();
                WODateList.Clear();

                foreach (WorkOrderDetail wo in lstElgbleWOs)
                {
                    if (wo.RKRPXMITSW != null && wo.RKRPXMITSW.Equals("Y"))
                        wo.RKRPRepairDate = wo.RepairDate;

                    // Load date list and check for duplicate RKRP repair dates
                    if (CheckRKRPRepairDate(wo))
                    {
                        //what to do
                    }
                    else
                    {
                        //what to do
                    }
                }
                UpdateDB(lstElgbleWOs);
            }
            catch (Exception ex)
            {
                logEntry.Message = "ERROR: StartOperation: " + ex.Message;
                Logger.Write(logEntry);
            }
            autoEvent.Set();
        }

        private bool CheckRKRPRepairDate(WorkOrderDetail wo)
        {
            try
            {
                string sNewDate = Convert.ToDateTime(wo.RKRPRepairDate).ToString("yyyy-MM-dd");
                List<string> RepairDateList = new List<string>();
                RepairDateList = rkrpOp.GetRepairDates(wo.Shop.ShopCode, wo.EquipmentList[0].EquipmentNo, wo.Shop.RKRPloc);

                if (WODateList.Count > 0)
                {
                    foreach (RepairDateRecord rdr in WODateList)
                    {
                        if (rdr.EQPNO.Equals(wo.EquipmentList[0].EquipmentNo))
                        {
                            if (rdr.RKRPLOC.Equals(wo.Shop.RKRPloc))
                            {
                                RepairDateList.Add(rdr.REPAIR_DTE);
                            }
                        }
                    }
                }

                // Check if date exists already in list of dates from DB and date list, if so, subtract day and retest until date not found
                bool bDone = false;
                while (!bDone)
                {
                    if (RepairDateList.FindIndex(x => x == sNewDate) != -1)
                    {
                        sNewDate = (Convert.ToDateTime(sNewDate).AddDays(-1)).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        bDone = true;
                        // Set RKRP date to NewDate.
                        wo.RKRPRepairDate = Convert.ToDateTime(sNewDate);
                    }

                }
                RepairDateRecord rp = new RepairDateRecord();
                rp.EQPNO = wo.EquipmentList[0].EquipmentNo;
                rp.RKRPLOC = wo.Shop.RKRPloc;
                rp.REPAIR_DTE = sNewDate;

                WODateList.Add(rp);
            }
            catch (Exception ex)
            {
                logEntry.Message = "ERROR: CheckRKRPRepairDate: " + ex.Message;
                Logger.Write(logEntry);
            }
            return true;
        }

        private void UpdateDB(List<WorkOrderDetail> _woList)
        {
            bool bOK = true;

            try
            {                
                foreach (WorkOrderDetail _wo in _woList)
                {

                    if (_wo.RKRPXMITSW != null && _wo.RKRPXMITSW.Equals("9"))
                        bOK = rkrpOp.UpdateTransmission(_wo, true);
                    else
                        bOK = rkrpOp.UpdateTransmission(_wo, false);

                    // if ok, create and send audit trail.
                    if (bOK)
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.Append("Work Order: ");
                        sb.Append(_wo.WorkOrderID);
                        sb.Append(" RKRP Transmitted ");
                        sb.Append(DateTime.Now);
                        sb.Append(" by MercRKRP batch process");

                        // send audit trail
                        rkrpOp.AuditLog(_wo.WorkOrderID, sb.ToString(), "MercRKRP Process", DateTime.Now);
                    }
                    else
                    {
                        rkrpOp.EventLog("RKRP_TRANSMISSION", "Not Applicable", "MESC1TS_WO", "Unable to update WO table for RKRP Transmit for WOID:" + _wo.WorkOrderID, "MercRKRP Process", DateTime.Now);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = "ERROR: UpdateDB: " + ex.Message;
                Logger.Write(logEntry);
            }
        }
    }
}
