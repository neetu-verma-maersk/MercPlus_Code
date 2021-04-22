using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using MercPlusLibrary;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using IBM.WMQ;
using System.Threading;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace MercDERTService
{
    public partial class MercDERTService : ServiceBase
    {
        private AutoResetEvent AutoEventInstance { get; set; }
        private MercDERTService TimerInstance { get; set; }
        private Timer StateTimer { get; set; }
        public int TimerInterval { get; set; }
        public static MESC2DSEntities objcontext = new MESC2DSEntities();
        public static MQManager MQManager = new MQManager();
        private const string MQ_READ_MODE = "R";
        private const string MQ_WRITE_MODE = "W";
        //public static bool success = false;
        public static string EqpNo = string.Empty;
        public static string StartDate;
        public static string EndDate;
        public static string CaseId = string.Empty;
        public static string msg = string.Empty;
        AutoResetEvent StopRequest = new AutoResetEvent(false);
        public static LogEntry logEntry = new LogEntry();
        public static byte[] msgID = null;
        Thread Worker;

        public MercDERTService()
        {
            InitializeComponent();
            TimerInterval = Convert.ToInt32(ConfigurationManager.AppSettings["TimerInterval"]);
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

            try
            {
                logEntry.Message = "DERT Service started";
                Logger.Write(logEntry);
                while (ReceiveDERT() == true)
                {
                    ParseDERTRequest();
                    ProcessDERTRequest();
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            autoEvent.Set();
        }

        private static bool ReceiveDERT()
        {
            MQManager mMQ = new MQManager();
            MQQueueManager QMgr = null;
            MQQueue Q = null;
            MQMessage mqMsg = new MQMessage();

            try
            {
                QMgr = mMQ.OpenQueueManager(ConfigurationManager.AppSettings["MQManagerQueueName"]);
                Q = mMQ.OpenQ(MQ_READ_MODE, QMgr, ConfigurationManager.AppSettings["MQManagerRequestName"]);
                int msgCount = mMQ.GetMessageCount(Q);
                //msg = mMQ.GetMessage(Q, false, ref corrID);
                msg = mMQ.GetMessage(Q, false, out msgID);
                mMQ.CloseQ(Q);
                mMQ.DisconnectQueueManager(QMgr);
                /* if (!success)
                 {
                     // Do logging queue name, process etc.
                     //logEntry.Message = "Unable to connect to MQ: Manager: MERCDEV1, DERT.MERC.REQUEST ";
                     //Logger.Write(logEntry);
                     return (false);
                 }*/
            }
            catch (MQException mqException)
            {
                if (mqException.Reason != MQC.MQRC_NO_MSG_AVAILABLE)
                {
                    //Write log - A problem occured while retrieving the MQ message: " + mqException.Message.ToString()  
                    logEntry.Message = "Error in fetching message from queue : " + mqException.ToString();
                    Logger.Write(logEntry);
                }
                else
                {   
                    logEntry.Message = "No message found in queue";
                    Logger.Write(logEntry);
                }
                mMQ.CloseQ(Q);
                mMQ.DisconnectQueueManager(QMgr);
                return false;
            }
            return (true);
        }

        private static void ProcessDERTRequest()
        {
            List<MESC1TS_WO> WorkOrderListFromDB = new List<MESC1TS_WO>();
            List<WorkOrderDetail> WorkOrderDetailList = new List<WorkOrderDetail>();
            string XMLMessage = string.Empty;
            List<RepairsView> RepairsViewList = new List<RepairsView>();


            try
            {
                //DB Call
                WorkOrderDetailList = GetElligibleWOs(EqpNo, StartDate, EndDate);
                //RepairsViewList = get
                foreach (var ritem in WorkOrderDetailList)
                {
                    if (ritem.RepairsViewList == null)
                        ritem.RepairsViewList = new List<RepairsView>();
                    RepairsViewList = GetElligibleWORepair(ritem.WorkOrderID, ritem.Mode, ritem);
                    ritem.RepairsViewList.AddRange(RepairsViewList);
                }


                XMLMessage = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><MERCPLUS><MessageHeader>";
                XMLMessage += "<MessageID><MessageIDSystem>MERCPLUS</MessageIDSystem><MessageIDType>REPAIRINFO</MessageIDType><CaseId>";
                XMLMessage += CaseId;
                XMLMessage += "</CaseId>";
                XMLMessage += "<Equipment_Number>";
                XMLMessage += EqpNo;
                XMLMessage += "</Equipment_Number>";

                if (WorkOrderDetailList == null || WorkOrderDetailList.Count == 0)
                {
                    XMLMessage += "<Indicator>";
                    XMLMessage += "N";
                    XMLMessage += "</Indicator>";
                }
                else
                {
                    XMLMessage += "<Indicator>";
                    XMLMessage += "Y";
                    XMLMessage += "</Indicator>";
                }

                XMLMessage += "<StartDate>";
                XMLMessage += StartDate;
                XMLMessage += "</StartDate>";
                XMLMessage += "<EndDate>";
                XMLMessage += EndDate;
                XMLMessage += "</EndDate>";
                XMLMessage += "</MessageID>";
                XMLMessage += "<TimingInfo>";
                XMLMessage += "<TimingMessageCreation>";
                XMLMessage += DateTime.Now;
                XMLMessage += "</TimingMessageCreation></TimingInfo></MessageHeader>";

                if (WorkOrderDetailList == null || WorkOrderDetailList.Count == 0)
                {
                    XMLMessage += "</MERCPLUS>";
                }
                else
                {
                    XMLMessage += "<WorkorderDetails>";
                    foreach (var item in WorkOrderDetailList)
                    {
                        decimal? d; // CostLocalexcltaxvat;
                        d = (item.ImportTaxCPH);

                        d += (item.SalesTaxPartsCPH);

                        //m_sSALES_TAX_PARTS= m_StringUtil.FloatToString(d);	
                        d += (item.AgentPartsTaxCPH);
                        //m_AGENT_PARTS_TAX = m_StringUtil.FloatToString(d);
                        d += (item.SalesTaxLabourCPH);
                        item.TotalCostLocalExclTaxVat = (item.TotalCostCPH - d);
                        XMLMessage += "<Workorder>";
                        if (item.EquipmentList != null && item.EquipmentList.Count > 0)
                        {
                            XMLMessage += "<vendorrefno>"; XMLMessage += item.EquipmentList[0].VendorRefNo; XMLMessage += "</vendorrefno>";
                        }
                        else
                        {
                            XMLMessage += "<vendorrefno>"; XMLMessage += string.Empty; XMLMessage += "</vendorrefno>";
                        }
                        XMLMessage += "<mode>"; XMLMessage += item.Mode; XMLMessage += "</mode>";
                        XMLMessage += "<modeDescription>"; XMLMessage += RepairXML(item.ModeDescription); XMLMessage += "</modeDescription>";
                        XMLMessage += "<Status>"; XMLMessage += item.WorkOrderStatus; XMLMessage += "</Status>";
                        XMLMessage += "<Status_Desc>"; XMLMessage += GetStatusDescription(item.WorkOrderStatus); XMLMessage += "</Status_Desc>";
                        XMLMessage += "<thirdpartyport>"; XMLMessage += item.ThirdPartyPort; XMLMessage += "</thirdpartyport>";
                        XMLMessage += "<completiondate>"; XMLMessage += item.RepairDate; XMLMessage += "</completiondate>";
                        XMLMessage += "<cause>"; XMLMessage += GetCauseDescription(item.Cause); XMLMessage += "</cause>";
                        XMLMessage += "<estimateno>"; XMLMessage += item.WorkOrderID; XMLMessage += "</estimateno>";
                        XMLMessage += "<currencycode>"; XMLMessage += item.Shop.Currency.CurCode; XMLMessage += "</currencycode>";
                        XMLMessage += "<geolocation>"; XMLMessage += item.Shop.LocationCode; XMLMessage += "</geolocation>";
                        XMLMessage += "<shopcode>"; XMLMessage += item.Shop.ShopCode; XMLMessage += "</shopcode>";
                        XMLMessage += "<shopdescription>"; XMLMessage += RepairXML(item.Shop.ShopDescription); XMLMessage += "</shopdescription>";
                        XMLMessage += "<shopemail>"; XMLMessage += item.Shop.EmailAdress; XMLMessage += "</shopemail>";
                        XMLMessage += "<shopphone>"; XMLMessage += item.Shop.Phone; XMLMessage += "</shopphone>";
                        XMLMessage += "<totalwearandtear>"; XMLMessage += item.TotalWMaterialAmountUSD; XMLMessage += "</totalwearandtear>";
                        XMLMessage += "<totalthirdparty>"; XMLMessage += item.TotalTMaterialAmountUSD; XMLMessage += "</totalthirdparty>";
                        XMLMessage += "<totalexcltaxvat>"; XMLMessage += item.TotalCostLocalExclTaxVat; XMLMessage += "</totalexcltaxvat>";
                        XMLMessage += "<totalhrs>"; XMLMessage += item.TotalRepairManHour; XMLMessage += "</totalhrs>";
                        XMLMessage += "<totalmaterialcost>"; XMLMessage += item.SumRepairMaterialAmt; XMLMessage += "</totalmaterialcost>";
                        XMLMessage += "<totalPartcost>"; XMLMessage += item.SumPartCost; XMLMessage += "</totalPartcost>";
                        XMLMessage += "<totalrepaircost>"; XMLMessage += item.TotalCostOfRepair; XMLMessage += "</totalrepaircost>";
                        XMLMessage += "<ordinaryLabourrate>"; XMLMessage += item.ManHourRateCPH; XMLMessage += "</ordinaryLabourrate>";

                        XMLMessage += "<Repairdetails>";
                        foreach (var rItem in item.RepairsViewList)
                        {
                            decimal? totalpercode = 0, totcphmaterialamt = 0, totcphpartamt = 0;
                            string costLocal = string.Empty;
                            totcphmaterialamt = rItem.Pieces * rItem.MaterialCostCPH;
                            List<SparePartsView> SparePartsList = new List<SparePartsView>();
                            SparePartsList = item.SparePartsViewList.FindAll(cd => cd.RepairCode.RepairCod == rItem.RepairCode.RepairCod);
                            foreach (var sItem in SparePartsList)
                            {
                                totcphpartamt = sItem.Pieces * sItem.CostLocalCPH;
                            }
                            totalpercode = (totcphmaterialamt + totcphpartamt);
                            rItem.TotalPerCode = totalpercode;
                            XMLMessage += "<Repair>";
                            XMLMessage += "<TPICode>"; XMLMessage += rItem.Tpi.CedexCode; XMLMessage += "</TPICode>";
                            XMLMessage += "<DamageCode>"; XMLMessage += rItem.Damage.DamageCedexCode; XMLMessage += "</DamageCode>";
                            XMLMessage += "<DamageName>"; XMLMessage += RepairXML(rItem.Damage.DamageName); XMLMessage += "</DamageName>";
                            XMLMessage += "<damageDescription>"; XMLMessage += RepairXML(rItem.Damage.DamageDescription);
                            XMLMessage += "</damageDescription>";
                            XMLMessage += "<Pieces>"; XMLMessage += rItem.Pieces; XMLMessage += "</Pieces>";
                            XMLMessage += "<RepairCode>"; XMLMessage += rItem.RepairCode.RepairCod; XMLMessage += "</RepairCode>";
                            XMLMessage += "<repairdescription>"; XMLMessage += RepairXML(rItem.RepairCode.RepairDesc); XMLMessage += "</repairdescription>";
                            XMLMessage += "<RepairLocationCode>"; XMLMessage += rItem.RepairLocationCode.CedexCode; XMLMessage += "</RepairLocationCode>";
                            XMLMessage += "<RepairLocationdescription>"; XMLMessage += RepairXML(rItem.RepairLocationCode.CedexCode); XMLMessage += "</RepairLocationdescription>";
                            XMLMessage += "<hrs>"; XMLMessage += rItem.ManHoursPerPiece; XMLMessage += "</hrs>";
                            XMLMessage += "<materialcost>"; XMLMessage += rItem.MaterialCostCPH; XMLMessage += "</materialcost>";
                            XMLMessage += "<totalpercode>"; XMLMessage += rItem.TotalPerCode; XMLMessage += "</totalpercode>";
                            XMLMessage += "<Part>";
                            if(SparePartsList != null && SparePartsList.Count > 0)
                            {
                                foreach (var sItem in SparePartsList)
                                {
                                    XMLMessage += "<Partcost>"; XMLMessage += sItem.CostLocalCPH; XMLMessage += "</Partcost>";
                                }
                            }
                            else
                            {
                                XMLMessage += "<Partcost>"; XMLMessage += string.Empty;  XMLMessage += "</Partcost>";
                            }
                            XMLMessage += "</Part>";
                            XMLMessage += "</Repair>";
                        }
                        XMLMessage += "</Repairdetails>";
                        XMLMessage += "</Workorder>";
                    }
                    XMLMessage += "</WorkorderDetails></MERCPLUS>";
                }
                SendDERT(XMLMessage);
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        private static string GetStatusDescription(short? StatusCode)
        {
            string StatusDesc = string.Empty;

            switch (StatusCode)
            {
                case 000: StatusDesc = "Draft"; break;
                case 100: StatusDesc = "Rejected"; break;
                case 130: StatusDesc = "Rejected to MSL"; break;
                case 140: StatusDesc = "Rejected to CPH"; break;
                case 200: StatusDesc = "Pending MSL Approval"; break;
                case 310: StatusDesc = "Suspended to MSL"; break;
                case 320: StatusDesc = "Suspended to MSL/CPH"; break;
                case 330: StatusDesc = "Suspended to CPH"; break;
                case 340: StatusDesc = "Suspended to CENEQUSAL"; break;
                case 390: StatusDesc = "Approved Estimate"; break;
                case 400: StatusDesc = "Completed"; break;
                case 500: StatusDesc = "RRIS Transmitted"; break;
                case 550: StatusDesc = "RRIS rejected"; break;
                case 600: StatusDesc = "RRIS accepted"; break;
                case 800: StatusDesc = "Paid"; break;
                case 900: StatusDesc = "Processed"; break;
                case 9998: StatusDesc = "Estimate void"; break;
                case 9999: StatusDesc = "Deleted"; break;
                default: StatusDesc = "("; StatusDesc += StatusCode; StatusDesc += ")"; break;
            }
            return StatusDesc;
        }

        private static string GetCauseDescription(string Cause)
        {
            string cause = string.Empty;
            int CharStat = Convert.ToInt32(Cause);

            switch (CharStat)
            {
                case 1: cause = Cause; cause += "-Wear and Tear"; break;
                case 2: cause = Cause.ToString(); cause += "-Handling Damage"; break;
                case 3: cause = Cause.ToString(); cause += "-Confirmed 3rd Party"; break;
                case 4: cause = Cause.ToString(); cause += "-UnConfirmed 3rd Party"; break;
                default: cause = "("; cause += Cause; cause += ")"; break;
            }
            return cause;
        }

        private static bool SendDERT(string DERTMessage)
        {
            bool success = false;
            MQManager mMQ = new MQManager();
            MQQueueManager QMgr = null;
            MQQueue Q = null;
            string msg = string.Empty;
            MQPutMessageOptions putOptions = new MQPutMessageOptions();
            MQMessage mqMsg = new MQMessage();

            try
            {
                mqMsg.MessageId = MQC.MQCI_NONE;
                mqMsg.CorrelationId = msgID;
                mqMsg.Format = MQC.MQFMT_STRING;
                mqMsg.WriteString(DERTMessage);
                //MQmgr.PutMessageWithOptions(Qqueue, msg, putOptions);
                QMgr = mMQ.OpenQueueManager(ConfigurationManager.AppSettings["MQManagerQueueName"]);
                Q = mMQ.OpenQ(MQ_WRITE_MODE, QMgr, ConfigurationManager.AppSettings["MQManagerResponseName"]);
                success = mMQ.PutMessageWithOptions(Q, mqMsg, putOptions);


                if (!success)
                {
                    // Do logging queue name, process etc.
                    logEntry.Message = "Error in putting message in Queue : " + ConfigurationManager.AppSettings["MQManagerQueueName"];
                    Logger.Write(logEntry);
                    return (false);
                }


                mMQ.CloseQ(Q);
                mMQ.DisconnectQueueManager(QMgr);
            }
            catch (MQException mqException)
            {
                logEntry.Message = mqException.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }

        private static bool ParseDERTRequest()
        {
            bool success = false;
            XmlDataDocument xmldoc = new XmlDataDocument();
            XmlNodeList xmlnode = null;
            int i = 0;
            /*FileStream fs = new FileStream("product.xml", FileMode.Open, FileAccess.Read);
            xmldoc.Load(fs);*/
            xmldoc.LoadXml(msg);//Soumik
            xmlnode = xmldoc.GetElementsByTagName("CaseId");
            for (i = 0; i <= xmlnode.Count - 1; i++)
            {
                xmlnode[i].ChildNodes.Item(0).InnerText.Trim();
                CaseId = xmlnode[i].ChildNodes.Item(0).InnerText.Trim();// +"  " + xmlnode[i].ChildNodes.Item(1).InnerText.Trim() + "  " + xmlnode[i].ChildNodes.Item(2).InnerText.Trim();
                //MessageBox.Show(str);
            }
            xmlnode = xmldoc.GetElementsByTagName("Equipment_Number");
            for (i = 0; i <= xmlnode.Count - 1; i++)
            {
                xmlnode[i].ChildNodes.Item(0).InnerText.Trim();
                EqpNo = xmlnode[i].ChildNodes.Item(0).InnerText.Trim(); // +"  " + xmlnode[i].ChildNodes.Item(1).InnerText.Trim() + "  " + xmlnode[i].ChildNodes.Item(2).InnerText.Trim();
                //MessageBox.Show(str);
            }
            xmlnode = xmldoc.GetElementsByTagName("StartDate");
            for (i = 0; i <= xmlnode.Count - 1; i++)
            {
                xmlnode[i].ChildNodes.Item(0).InnerText.Trim();
                StartDate = xmlnode[i].ChildNodes.Item(0).InnerText.Trim(); // +"  " + xmlnode[i].ChildNodes.Item(1).InnerText.Trim() + "  " + xmlnode[i].ChildNodes.Item(2).InnerText.Trim();
                //MessageBox.Show(str);
            }
            xmlnode = xmldoc.GetElementsByTagName("EndDate");
            for (i = 0; i <= xmlnode.Count - 1; i++)
            {
                xmlnode[i].ChildNodes.Item(0).InnerText.Trim();
                EndDate = xmlnode[i].ChildNodes.Item(0).InnerText.Trim(); // +"  " + xmlnode[i].ChildNodes.Item(1).InnerText.Trim() + "  " + xmlnode[i].ChildNodes.Item(2).InnerText.Trim();
                //MessageBox.Show(str);
            }

            return success;
        }

        private static List<WorkOrderDetail> GetElligibleWOs(string EqpNo, string StartDate, string EndDate)
        {
            List<WorkOrderDetail> WorkOrderList = new List<WorkOrderDetail>();
            string SEndDate = EndDate + " " + "23:59:59";
            //List<MESC1TS_WO> 
            //EqpNo = "MAEU5532364";
            //var temp_mesc1ts_wopart = null;
            DateTime tempSDate = Convert.ToDateTime(StartDate);
            DateTime tempEDate = Convert.ToDateTime(SEndDate);
            string sSQL = string.Empty;
            try
            {

                sSQL = "with temp_mesc1ts_wo";
                sSQL += " as ";
                sSQL += "(select w1.wo_id ";
                sSQL += ",w1.status_code";
                sSQL += ",w1.mode";
                sSQL += ",w1.shop_cd";
                sSQL += ",w1.vendor_ref_no";
                sSQL += ",w1.MANH_RATE_CPH";
                sSQL += ",w1.repair_dte";
                sSQL += ",w1.tot_w_material_amt_cph";
                sSQL += ",w1.tot_t_material_amt_cph";
                sSQL += ",w1.cause";
                sSQL += ",w1.third_party";
                sSQL += ",w1.tot_repair_manh";
                sSQL += ",w1.tot_shop_amt_cph";
                sSQL += ",w1.CUCDN";
                sSQL += ",w1.tot_cost_cph";
                sSQL += ",w1.IMPORT_TAX_CPH";
                sSQL += ",w1.AGENT_PARTS_TAX_CPH";
                sSQL += ",w1.SALES_TAX_PARTS_CPH";
                sSQL += ",w1.SALES_TAX_LABOR_CPH";
                sSQL += " from mesc1ts_wo  w1";
                sSQL += " where eqpno='";
                sSQL += EqpNo;
                sSQL += "' and repair_dte>'";
                sSQL += tempSDate;
                sSQL += "' and repair_dte<'";
                sSQL += tempEDate;
                //sSQL += " 23:59:59'";

                sSQL += "')";
                //--select * from temp_mesc1ts_wo
                sSQL += " ,temp_mesc1ts_wopart";
                sSQL += " as ";
                sSQL += "(";
                sSQL += "select sum(t2.cost_cph) as sum_partcost,t2.wo_id ";
                sSQL += "from temp_mesc1ts_wo t1 ";
                sSQL += "left outer join mesc1ts_wopart t2 ";
                sSQL += "on t1.wo_id=t2.wo_id ";
                sSQL += "group by t2.wo_id";
                sSQL += ") ";
                //--select * from temp_mesc1ts_wopart
                sSQL += ",temp_combine_wo_wopart ";
                sSQL += "as ";
                sSQL += "( select t1.*,t2.sum_partcost ";
                sSQL += "from temp_mesc1ts_wo t1 ";
                sSQL += "left outer join temp_mesc1ts_wopart t2 ";
                sSQL += "on t1.wo_id=t2.wo_id ";
                sSQL += ") ";
                //--select * from temp_combine_wo_wopart
                sSQL += ",temp_combine_wo_wopart_shop ";
                sSQL += "as ";
                sSQL += "( select t1.*,t2.shop_desc, t2.email_adr, t2.phone, t2.loc_cd ";
                sSQL += "from temp_combine_wo_wopart t1 ";
                sSQL += "left outer join mesc1ts_shop t2 ";
                sSQL += "on t1.shop_cd=t2.shop_cd ";
                sSQL += ") ";          //--select * from temp_combine_wo_wopart_shop
                sSQL += ",temp_combine_wo_wopart_shop_mode ";
                sSQL += "as ";
                sSQL += "(select t1.*, t2.mode_desc ";
                sSQL += "from temp_combine_wo_wopart_shop as t1 ";
                sSQL += "left outer join mesc1ts_mode as t2 ";
                sSQL += "on t1.mode=t2.mode ";

                sSQL += ") ";
                sSQL += ",temp_combine_wo_wopart_shop_mode_CURR ";
                sSQL += "as ";
                sSQL += "(select t1.*, t2.CURCD ";
                sSQL += "from temp_combine_wo_wopart_shop_mode as t1 ";
                sSQL += "left outer join mesc1ts_currency as t2 ";
                sSQL += "on t1.CUCDN=t2.CUCDN ";

                sSQL += ") ";

                sSQL += ",temp_mesc1ts_worepair ";
                sSQL += "as ";
                sSQL += "(";
                sSQL += "select sum(t2.cph_material_amt) as sum_repair_material_amt,t2.wo_id ";
                sSQL += "from temp_combine_wo_wopart_shop_mode_CURR t1 ";
                sSQL += "left outer join mesc1ts_worepair t2 ";
                sSQL += "on t1.wo_id=t2.wo_id ";
                sSQL += "group by t2.wo_id ";
                sSQL += ") ";

                sSQL += ",temp_combine_wo_wopart_shop_mode_CURR_worepair ";
                sSQL += "as ";
                sSQL += "( select t1.*,t2.sum_repair_material_amt ";
                sSQL += "from temp_combine_wo_wopart_shop_mode_CURR t1 ";
                sSQL += "left outer join temp_mesc1ts_worepair t2 ";
                sSQL += "on t1.wo_id=t2.wo_id ";
                sSQL += ") ";

                sSQL += "select * from temp_combine_wo_wopart_shop_mode_CURR_worepair";

                List<EllWO> ElligibleWOs = new List<EllWO>();
                ElligibleWOs = objcontext.Database.SqlQuery<EllWO>(sSQL).ToList();

                foreach (var item in ElligibleWOs)
                {
                    WorkOrderDetail wo = new WorkOrderDetail();
                    wo.EquipmentList = new List<Equipment>();
                    wo.Shop = new Shop();
                    wo.Shop.Currency = new Currency();
                    Equipment eqp = new Equipment();
                    eqp.VendorRefNo = item.VENDOR_REF_NO;
                    wo.Mode = item.MODE;
                    wo.ModeDescription = item.MODE_DESC;
                    wo.WorkOrderStatus = item.STATUS_CODE;
                    wo.ThirdPartyPort = item.THIRD_PARTY;
                    wo.RepairDate = item.REPAIR_DTE;
                    wo.Cause = item.CAUSE;
                    wo.WorkOrderID = item.WO_ID;
                    wo.Shop.Currency.CurCode = item.CURCD;
                    wo.Shop.LocationCode = item.LOC_CD;
                    wo.Shop.ShopCode = item.SHOP_CD;
                    wo.Shop.ShopDescription = item.SHOP_DESC;
                    wo.Shop.EmailAdress = item.EMAIL_ADR;
                    wo.Shop.Phone = item.PHONE;
                    wo.TotalWMaterialAmountCPH = item.TOT_W_MATERIAL_AMT_CPH;
                    wo.TotalTMaterialAmountCPH = item.TOT_T_MATERIAL_AMT_CPH;
                    wo.ImportTaxCPH = item.IMPORT_TAX_CPH;
                    wo.SalesTaxLabourCPH = item.SALES_TAX_LABOR_CPH;
                    wo.SalesTaxPartsCPH = item.SALES_TAX_PARTS_CPH;
                    wo.AgentPartsTaxCPH = item.AGENT_PARTS_TAX_CPH;
                    wo.TotalRepairManHour = item.TOT_REPAIR_MANH;
                    wo.SumRepairMaterialAmt = item.sum_repair_material_amt;
                    wo.SumPartCost = item.sum_partcost;
                    wo.TotalCostOfRepair = item.TOT_SHOP_AMT_CPH;
                    wo.ManHourRateCPH = item.MANH_RATE_CPH;
                    wo.TotalCostCPH = item.TOT_COST_CPH;
                    wo.EquipmentList.Add(eqp);
                    WorkOrderList.Add(wo);
                }
            }


            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return WorkOrderList;
        }

        private static List<RepairsView> GetElligibleWORepair(int woid, string Mode, WorkOrderDetail WOD)
        {
            string sSQL = string.Empty;
            string id = woid.ToString();
            WOD.SparePartsViewList = new List<SparePartsView>();

            sSQL = "with temp_mesc1ts_worepair ";
            sSQL += "as ";
            sSQL += "( select r1.repair_cd";
            sSQL += ",r1.qty_repairs";
            sSQL += ",r1.tpi_cd";
            sSQL += ",r1.damage_cd";
            sSQL += ",r1.repair_loc_cd";
            sSQL += ",r1.actual_manh";
            sSQL += ",r1.cph_material_amt";
            sSQL += ",r1.wo_id ";
            sSQL += "from mesc1ts_worepair r1 ";
            sSQL += "where wo_id='";
            sSQL += woid;
            sSQL += "') ";
            //--select * from temp_mesc1ts_worepair
            sSQL += ",temp_combine_worepair_wopart ";
            sSQL += "as ";
            sSQL += "( select t1.*,t2.cost_cph,t2.qty_parts ";
            sSQL += "from temp_mesc1ts_worepair t1 ";
            sSQL += "left outer join mesc1ts_wopart t2 ";
            sSQL += "on t1.wo_id=t2.wo_id  and t1.repair_cd=t2.repair_cd ";
            sSQL += ") ";

            //--select * from temp_combine_worepair_wopart
            sSQL += ",temp_combine_worepair_wopart_damage ";
            sSQL += "as ";
            sSQL += "(select t1.*, t2.name,t2.description ";
            sSQL += "from temp_combine_worepair_wopart t1 ";
            sSQL += "left outer join mesc1ts_damage t2 ";
            sSQL += "on t1.damage_cd=t2.cedex_code ";
            sSQL += ") ";
            //--select * from temp_combine_worepair_wopart_damage
            sSQL += ",temp_combine_worepair_wopart_damage_repair ";
            sSQL += "as ";
            sSQL += "(select  t1.*, t2.repair_desc ";
            sSQL += "from temp_combine_worepair_wopart_damage t1 ";
            sSQL += "left outer join mesc1ts_repair_code t2 ";
            sSQL += "on t1.repair_cd=t2.repair_cd	and t2.mode='";
            sSQL += Mode;
            sSQL += "') ";
            //--select * from temp_combine_worepair_wopart_damage_repair

            sSQL += ",temp_combine_worepair_wopart_damage_repair_repairloc ";
            sSQL += "as ";
            sSQL += "(select t1.*, t2.description as repair_loc_desc ";
            sSQL += "from temp_combine_worepair_wopart_damage_repair t1 ";
            sSQL += "left outer join mesc1ts_repair_loc t2 ";
            sSQL += "on  SUBSTRING(t1.repair_loc_cd,1,2)= t2.cedex_code ";
            sSQL += ") ";

            sSQL += "select * from temp_combine_worepair_wopart_damage_repair_repairloc";
            List<RepairsView> RepairViewList = new List<RepairsView>();

            List<EllWO> ELLWORepair = new List<EllWO>();
            ELLWORepair = objcontext.Database.SqlQuery<EllWO>(sSQL).ToList();




            foreach (var item in ELLWORepair)
            {
                RepairsView rv = new RepairsView();
                SparePartsView spv = new SparePartsView();
                rv.RepairCode = new RepairCode();
                rv.RepairLocationCode = new RepairLoc();
                rv.Damage = new Damage();
                rv.Tpi = new Tpi();
                rv.Tpi.CedexCode = item.TPI_CD;
                rv.Damage.DamageCedexCode = item.DAMAGE_CD;
                rv.Damage.DamageName = item.name;
                rv.Damage.DamageDescription = item.description;
                rv.Pieces = (int)item.QTY_REPAIRS;
                rv.RepairCode.RepairCod = item.REPAIR_CD;
                rv.RepairCode.RepairDesc = item.REPAIR_DESC;
                rv.RepairLocationCode.CedexCode = item.REPAIR_LOC_CD;
                rv.RepairLocationCode.Description = item.description;
                rv.ManHoursPerPiece = item.ACTUAL_MANH;
                rv.MaterialCostCPH = item.CPH_MATERIAL_AMT;
                if (item.COST_CPH != null && item.QTY_PARTS != null)
                {
                    spv.RepairCode = new RepairCode();
                    spv.RepairCode.RepairCod = item.REPAIR_CD;
                    spv.CostLocalCPH = item.COST_CPH;
                    spv.Pieces = (int)item.QTY_PARTS;
                    WOD.SparePartsViewList.Add(spv);
                }
                RepairViewList.Add(rv);
            }
            return RepairViewList;
        }

        private static string RepairXML(string str)
        {
            string newstr = string.Empty;
            char ctest;

            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    ctest = str[i];
                    if (ctest == '>')
                    {
                        newstr += ctest;
                        newstr = newstr.Substring(0, newstr.Length - 1);
                        newstr += "&gt;";
                    }
                    else if (ctest == '<')
                    {
                        newstr += ctest;
                        newstr = newstr.Substring(0, newstr.Length - 1);
                        newstr += "&lt;";
                    }
                    else if (ctest == '&')
                    {
                        newstr += ctest;
                        newstr = newstr.Substring(0, newstr.Length - 1);
                        newstr += "&amp;";
                    }
                    else if (ctest == '"')
                    {
                        newstr += ctest;
                        newstr = newstr.Substring(0, newstr.Length - 1);
                        newstr += "&quot;";
                    }
                    else if (ctest == '\'')
                    {
                        newstr += ctest;
                        newstr = newstr.Substring(0, newstr.Length - 1);
                        newstr += "&apos;";
                    }
                    else
                        newstr += ctest;
                }
            }
            return newstr;
        }
    }
}
