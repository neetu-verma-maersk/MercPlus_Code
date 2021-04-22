using MercPlusLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Threading;
using System.Configuration;
namespace FixPartRepairs
{
    public partial class FixPartRepairs : ServiceBase
    {
        static int flag = 0;
        public static List<RepairCodePart> PartList = new List<RepairCodePart>();
        public static List<RepairCodePart> RprPartList = new List<RepairCodePart>();
        public static List<MasterPart> SupercededPartList = new List<MasterPart>();
        public static FixPartRepairsEntities objFixPartRepairsEntities = new FixPartRepairsEntities();
        private AutoResetEvent AutoEventInstance { get; set; }
        public static LogEntry logEntry = new LogEntry();
        private Timer StateTimer { get; set; }
        public int TimerInterval { get; set; }
        Thread Worker;
        AutoResetEvent StopRequest = new AutoResetEvent(false);
        public FixPartRepairs()
        {
            InitializeComponent();
           // TimerInterval = 10000;//
            TimerInterval = Convert.ToInt32(Properties.Settings.Default["TimeInterval"].ToString()); 
        }

        protected override void OnStart(string[] args)
        {
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
            int nRetCode = 0;
            int i = 0, j = 0;
            try
            {
                logEntry.Message = "Fix Repairs Part Job started at :" + DateTime.Now;
                Logger.Write(logEntry);
                // Get list of Parts with repair code associations.
                GetPartCodeList();
                if (PartList.Count > 0)
                {
                    ProcessParts();
                }
                GetPartCodeList();
                if (PartList.Count() > 0)
                {
                    ProcessSupercededParts();
                }
                logEntry.Message = "Fix Repairs Part Job End at :" + DateTime.Now;
                Logger.Write(logEntry);
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            autoEvent.Set();
        }

        // return distinct list of part codes in the part/repair association table: (mesc1ts_rprcode_part)
        // this list will be traversed to check for superceded parts etc.
        public static void GetPartCodeList()
        {
            try
            {
                // FixPartRepairsEntities obj1FixPartRepairsEntities = new FixPartRepairsEntities();
                PartList.Clear();
                var objResults = (from RC in objFixPartRepairsEntities.MESC1TS_RPRCODE_PART
                                  select new { RC.PART_CD }).Distinct();

                foreach (var col in objResults)
                {
                    RepairCodePart objRepairCodePart = new RepairCodePart();
                    objRepairCodePart.PartCd = col.PART_CD;
                    PartList.Add(objRepairCodePart);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }
        // PHASE 1
        // check if part (in part/code list) is superceded by new master parts
        // if so, load part/code associations and create same associations for superceding part
        // in the master part table
        public static void ProcessParts()
        {
            try
            {
                string sPart;

                // iterate through part code collecton 
                // Check if superceded by new part, ensure new part has same associations
                for (int i = 0; i < PartList.Count(); i++)
                {
                    sPart = PartList[i].PartCd.Trim();

                    // check for part(s) superceding this part
                    CheckIfSuperceded(sPart);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }
        public static bool CheckIfSuperceded(string sPart)
        {
            bool bDone = false;
            try
            {
                string sCheckPart = sPart;
                string sNewPartCode = "";
                long nCnt = 0;

                while (!bDone)
                {
                    // check if superceded - amount = 0 and get the description(may contain superceding part code)
                    // if not, then bDone=true;
                    if (!IsSuperceded(sCheckPart, ref sNewPartCode))
                        bDone = true;
                    else // is superceded
                    {

                        // else get list of repair/code associations. Load into the RprPartList collection
                        LoadAssociations(sCheckPart);


                        // get count of associations for new part code superceding original sCheckPart
                        nCnt = GetAssociationCount(sNewPartCode);
                        if (nCnt != RprPartList.Count())
                        {
                            // then create associations for newPart Code
                            CreateNewAssociations(sNewPartCode);
                        }

                        // Set sCheckPart to newpartcode and repeat process until done.
                        sCheckPart = sNewPartCode;
                        sNewPartCode = "";
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return (bDone);
        }
        // Check if a part code has been superceded with a new part number (part_price = 0 and Part_desc will contain new part code)
        public static bool IsSuperceded(string sPart, ref string sNewPart)
        {
            bool isSuperceded = false;
            double partPriceValue = 0;
            try
            {

                var objResults = (from RP in objFixPartRepairsEntities.MESC1TS_MASTER_PART
                                  where RP.PART_CD == sPart
                                  select new { RP.PART_PRICE, RP.PART_DESC });

                List<MasterPart> RepairCodePartList = new List<MasterPart>();
                foreach (var col in objResults)
                {
                    partPriceValue = Convert.ToDouble(col.PART_PRICE);
                    sNewPart = col.PART_DESC.Trim();//--
                    // if price set to zero
                    // and superceding part is different, then this is superceded
                    if (partPriceValue == 0 || partPriceValue == 0.0)
                    {
                        if (!sNewPart.Equals(sPart))
                            isSuperceded = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = "Error reading record\n" + ex.ToString();
                Logger.Write(logEntry);

            }
            return isSuperceded;
        }
        public static bool LoadAssociations(string sPart)
        {
            try
            {
                var objResults = (from RC in objFixPartRepairsEntities.MESC1TS_RPRCODE_PART
                                  where RC.PART_CD == sPart
                                  select new { RC.REPAIR_CD, RC.MODE, RC.MANUAL_CD, RC.PART_CD, RC.MAX_PART_QTY });

                if (RprPartList.Count() > 0) RprPartList.Clear();

                foreach (var col in objResults)
                {
                    RepairCodePart objRepairCodePart = new RepairCodePart();
                    objRepairCodePart.RepairCod = col.REPAIR_CD;
                    objRepairCodePart.ModeCode = col.MODE;
                    objRepairCodePart.ManualCode = col.MANUAL_CD;
                    objRepairCodePart.PartCd = col.PART_CD;
                    objRepairCodePart.MaxPartQty = col.MAX_PART_QTY.ToString();
                    RprPartList.Add(objRepairCodePart);
                }
                return true;
            }
            catch (Exception ex)
            {
                logEntry.Message = "Error while getting part association qty. \n" + ex.ToString();
                Logger.Write(logEntry);
            }
            return false;
        }
        public static long GetAssociationCount(string sPart)
        {
            long nCnt = 0;
            try
            {
                nCnt = (from RC in objFixPartRepairsEntities.MESC1TS_RPRCODE_PART
                        where RC.PART_CD == sPart
                        select new { RC.PART_CD }).Count();
            }
            catch (Exception ex)
            {
                nCnt = 0;
                logEntry.Message = "Error while getting part association qty. \n" + ex.ToString();
                Logger.Write(logEntry);

            }
            return nCnt;
        }
        public static void CreateNewAssociations(string sNewPartCode)
        {
            try
            {
                foreach (var col in RprPartList)
                {
                    var objRprOcdePartResult = (from RC in objFixPartRepairsEntities.MESC1TS_RPRCODE_PART
                                                where RC.MANUAL_CD == col.ManualCode &&
                                                      RC.MODE == col.ModeCode &&
                                                      RC.REPAIR_CD == col.RepairCod &&
                                                      RC.PART_CD == sNewPartCode
                                                select RC).ToList();

                    if (objRprOcdePartResult.Count() <= 0)
                    {
                        var objNewPartCodeResult = (from NRC in objFixPartRepairsEntities.MESC1TS_MASTER_PART
                                                    where NRC.PART_CD == sNewPartCode
                                                    select NRC).ToList();
                        if (objNewPartCodeResult.Count() > 0)
                        {

                            MESC1TS_RPRCODE_PART rprCodePartToBeInserted = new MESC1TS_RPRCODE_PART();
                            rprCodePartToBeInserted.MANUAL_CD = col.ManualCode;
                            rprCodePartToBeInserted.MODE = col.ModeCode;
                            rprCodePartToBeInserted.REPAIR_CD = col.RepairCod;
                            rprCodePartToBeInserted.PART_CD = sNewPartCode;
                            rprCodePartToBeInserted.CHUSER = "Batch Assoc Module";
                            rprCodePartToBeInserted.CHTS = DateTime.Now;
                            try
                            {
                                rprCodePartToBeInserted.MAX_PART_QTY = Convert.ToInt16(col.MaxPartQty);
                            }
                            catch
                            {
                                rprCodePartToBeInserted.MAX_PART_QTY = null;
                            }
                            try
                            {
                                objFixPartRepairsEntities.MESC1TS_RPRCODE_PART.Add(rprCodePartToBeInserted);
                                objFixPartRepairsEntities.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                logEntry.Message = "Error while inserting part association table. \n" + ex.ToString();
                                Logger.Write(logEntry);
                            }

                        }

                    }


                }



            }
            catch (Exception Ex)
            {

                logEntry.Message = "Some Error has occured while inserting part association table. \n" + Ex.ToString();
                Logger.Write(logEntry);

            }
        }
        public static void ProcessSupercededParts()
        {
            string sPart;
            try
            {
                // iterate through part code collecton 
                // Check if superceded by new part, ensure new part has same associations

                for (int i = 0; i < PartList.Count(); i++)
                {
                    sPart = PartList[0].PartCd;
                    // check for part(s) superceding this part
                    CheckIfSupercedingOthers(sPart);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }
        public static bool CheckIfSupercedingOthers(string sPart)
        {
            bool bDone = false;
            try
            {
                string sCheckPart = sPart;
                string sNewPartCode;
                long nCnt = 0;

                // check if superceded - amount = 0 and get the description(may contain superceding part code)
                // if not, then bDone=true;
                if (!IsSuperceding(sCheckPart))
                    bDone = true;
                else // is superceded
                {
                    // else get list of repair/code associations. Load into the RprPartList collection
                    LoadAssociations(sCheckPart);

                    for (int i = 0; i < SupercededPartList.Count(); i++)
                    {
                        sNewPartCode = SupercededPartList[i].PartCd;

                        // get count of associations for new part code superceding original sCheckPart
                        nCnt = GetAssociationCount(sNewPartCode);
                        if (nCnt != RprPartList.Count())
                        {
                            // then create associations for newPart Code
                            CreateNewAssociations(sNewPartCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return (bDone);
        }
        public static bool IsSuperceding(string sPart)
        {
            bool bSuperceding = false;
            double partPriceValue = 0.0;
            string sPartDesc;

            // empty global collection of master parts superceded by part in part/code table.
            SupercededPartList.Clear();
            try
            {
                var objResults = (from RP in objFixPartRepairsEntities.MESC1TS_MASTER_PART
                                  where RP.PART_DESC == sPart
                                  select new { RP.PART_CD, RP.PART_PRICE, RP.PART_DESC });


                // load superceded part codes from master part into list.
                foreach (var col in objResults)
                {
                    partPriceValue = Convert.ToDouble(col.PART_PRICE);
                    sPartDesc = col.PART_DESC;

                    // if price set to zero
                    // and superceding part is different, then this is superceded
                    if (partPriceValue == 0)
                    {
                        if (!sPartDesc.Equals(sPart))
                        {
                            MasterPart objMasterPart = new MasterPart();
                            objMasterPart.PartCd = col.PART_CD;
                            SupercededPartList.Add(objMasterPart);
                            bSuperceding = true;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                logEntry.Message = "Error reading record. \n" + Ex.ToString();
                Logger.Write(logEntry);
            }

            return bSuperceding;
        }


    }
}
