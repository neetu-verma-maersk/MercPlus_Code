using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercPlusLibrary;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace RKRP_Service
{
    public class RKRP_Operations
    {        
        MESC2DSEntities objContext = null;
        public List<WorkOrderDetail> GetElligibleWOs()
        {
            objContext = new MESC2DSEntities();
            List<WorkOrderDetail> WOD = new List<WorkOrderDetail>();
            try
            {
                List<SqlDataClass> cls = new List<SqlDataClass>();
                cls = objContext.Database.SqlQuery<SqlDataClass>("SELECT TOP 2000 W.WO_ID, W.CUSTOMER_CD,W.REPAIR_DTE, W.EQPNO, W.SHOP_CD, W.EXCHANGE_RATE,W.MODE,W.MANH_RATE,W.CAUSE, W.AGENT_PARTS_TAX_CPH,W.THIRD_PARTY,W.IMPORT_TAX_CPH,W.TOT_MANH_REG,W.TOT_LABOR_COST,W.TOT_LABOR_COST_CPH,W.TOT_REPAIR_MANH, W.SALES_TAX_PARTS_CPH,W.SALES_TAX_LABOR_CPH,W.TOT_MANH_OT,W.TOT_MANH_DT,W.TOT_MANH_MISC,W.RKRP_XMIT_SW AS WO_RKRP_XMIT_SW, S.CUCDN, S.RKRPLOC, S.ACEP_SW, S.SHOP_TYPE_CD, T.RKRP_XMIT_SW FROM MESC1TS_WO W with (index(XIP3MESC1TS_WO)), MESC1TS_SHOP S, MESC1TS_TRANSMIT T WHERE W.STATUS_CODE BETWEEN 400 and 1000 AND W.SHOP_CD = S.SHOP_CD AND W.MODE = T.MODE AND W.CUSTOMER_CD = T.CUSTOMER_CD AND (W.RKRP_XMIT_SW = '0' OR W.RKRP_XMIT_SW = '9')").ToList();

                if (cls != null)
                {
                    RKRP_WinService.logEntry.Message = "GetElligibleWos: found " + cls.Count + " Wos.";
                   Logger.Write(RKRP_WinService.logEntry);
                    foreach (SqlDataClass item in cls)
                    {
                        WorkOrderDetail WO = new WorkOrderDetail();
                        WO.WorkOrderID = item.WO_ID;
                        Shop s = new Shop();
                        s.ShopCode = item.SHOP_CD;
                        s.RKRPloc = item.RKRPLOC;
                        s.AcepSW = item.ACEP_SW;
                        s.ShopTypeCode = item.SHOP_TYPE_CD;

                        Customer c = new Customer();
                        c.CustomerCode = item.CUSTOMER_CD;
                        s.Customer = new List<Customer>();
                        s.Customer.Add(c);

                        Currency cu = new Currency();
                        cu.Cucdn = item.CUCDN;
                        s.Currency = cu;

                        WO.Shop = s;
                        WO.RepairDate = item.REPAIR_DTE;
                        WO.RKRPRepairDate = item.REPAIR_DTE;
                        WO.RKRPXMITSW = item.WO_RKRP_XMIT_SW;

                        Equipment Equipment = new Equipment();
                        Equipment.EquipmentNo = item.EQPNO;
                        WO.EquipmentList = new List<MercPlusLibrary.Equipment>();
                        WO.EquipmentList.Add(Equipment);

                        WO.ExchangeRate = item.EXCHANGE_RATE;
                        WO.Mode = item.MODE;
                        WO.ManHourRate = item.MANH_RATE;
                        WO.Cause = item.CAUSE;
                        WO.AgentPartsTaxCPH = item.AGENT_PARTS_TAX_CPH;
                        WO.ThirdPartyPort = item.THIRD_PARTY;
                        WO.ImportTaxCPH = item.IMPORT_TAX_CPH;
                        WO.TotalManHourReg = item.TOT_MANH_REG;
                        WO.TotalLabourCost = item.TOT_LABOR_COST;
                        WO.TotalLabourCostCPH = item.TOT_LABOR_COST_CPH;
                        WO.TotalRepairManHour = item.TOT_REPAIR_MANH;
                        WO.SalesTaxPartsCPH = item.SALES_TAX_PARTS_CPH;
                        WO.SalesTaxLabourCPH = item.SALES_TAX_LABOR_CPH;
                        WO.TotalManHourOverTime = item.TOT_MANH_OT;
                        WO.TotalManHourDoubleTime = item.TOT_MANH_DT;
                        WO.TotalManHourMisc = item.TOT_MANH_MISC;
                        WO.RKRPXMITSW = item.WO_RKRP_XMIT_SW;
                        WOD.Add(WO);
                    }
                }
            }
            catch (Exception ex)
            {
                RKRP_WinService.logEntry.Message = "ERROR: GetElligibleWos: " + ex.InnerException;
                Logger.Write(RKRP_WinService.logEntry);
            }
            return WOD;
        }

        public bool UpdateTransmission(WorkOrderDetail _wo, bool noDate)
        {
            try
            {
                objContext = new MESC2DSEntities();
                List<MESC1TS_WO> ToBeUpdateWO = new List<MESC1TS_WO>();
                ToBeUpdateWO = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == _wo.WorkOrderID
                                select wo).ToList();

                if (ToBeUpdateWO != null && ToBeUpdateWO.Count > 0)
                {
                    foreach (var item in ToBeUpdateWO)
                    {
                        item.RKRP_XMIT_SW = "Y";
                        item.RKRP_REPAIR_DTE = _wo.RKRPRepairDate;
                        if (!noDate)
                            item.RKRP_XMIT_DTE = DateTime.Now;
                        objContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                RKRP_WinService.logEntry.Message = "ERROR: UpdateTransmission: " + ex.Message;
                Logger.Write(RKRP_WinService.logEntry);
                return false;
            }
            return true;
        }

        public List<string> GetRepairDates(string shopCode, string eqpNo, string shopRkrpLocCode)
        {
            objContext = new MESC2DSEntities();
            List<string> rprDates = new List<string>();
            var rprDL = (from W in objContext.MESC1TS_WO
                         from S in objContext.MESC1TS_SHOP
                         where S.SHOP_CD == W.SHOP_CD &&
                         W.EQPNO == eqpNo &&
                         S.RKRPLOC == shopRkrpLocCode &&
                         W.RKRP_REPAIR_DTE != null
                         select new
                         {
                             W.RKRP_REPAIR_DTE
                             // W.RKRP_REPAIR_DTE
                         }).ToList();

            foreach (var item in rprDL)
            {                
                if (item != null)
                    rprDates.Add(Convert.ToDateTime(item.RKRP_REPAIR_DTE).ToString("yyyy-MM-dd"));
            }

            return rprDates;
        }


        public bool EventLog(string EVENT_NAME, string UNIQUE_ID, string TABLE_NAME, string EVENT_DESC, string CHUSER, DateTime CHTS)
        {
            try
            {
                MESC1TS_EVENT_LOG EventLog = new MESC1TS_EVENT_LOG();
                objContext = new MESC2DSEntities();
                EventLog.TABLE_NAME = TABLE_NAME;
                EventLog.UNIQUE_ID = UNIQUE_ID;
                EventLog.EVENT_DESC = EVENT_DESC;
                EventLog.EVENT_NAME = EVENT_NAME;
                EventLog.CHUSER = CHUSER;
                EventLog.CHTS = DateTime.Now;
                objContext.MESC1TS_EVENT_LOG.Add(EventLog);
                objContext.SaveChanges();
            }
            catch (Exception ex)
            {
                RKRP_WinService.logEntry.Message = "ERROR: EventLog: " + ex.Message;
                Logger.Write(RKRP_WinService.logEntry);
            }

            return true;
        }

        public bool AuditLog(int WO_ID, string AUDIT_TEXT, string CHUSER, DateTime CHTS)
        {
            MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
            objContext = new MESC2DSEntities();
            WOAudit.WO_ID = WO_ID;
            WOAudit.AUDIT_TEXT = AUDIT_TEXT;
            WOAudit.CHUSER = CHUSER;
            WOAudit.CHTS = DateTime.Now;
            objContext.MESC1TS_WOAUDIT.Add(WOAudit);
            objContext.SaveChanges();
            return true;
        }
    }

    public class SqlDataClass
    {
        public int WO_ID { get; set; }
        public string CUSTOMER_CD { get; set; }
        public DateTime? REPAIR_DTE { get; set; }
        public string EQPNO { get; set; }
        public string SHOP_CD { get; set; }
        public decimal? EXCHANGE_RATE { get; set; }
        public string MODE { get; set; }
        public decimal? MANH_RATE { get; set; }
        public string CAUSE { get; set; }
        public decimal? AGENT_PARTS_TAX_CPH { get; set; }
        public string THIRD_PARTY { get; set; }
        public decimal? IMPORT_TAX_CPH { get; set; }
        public double? TOT_MANH_REG { get; set; }
        public decimal? TOT_LABOR_COST { get; set; }
        public decimal? TOT_LABOR_COST_CPH { get; set; }
        public double? TOT_REPAIR_MANH { get; set; }
        public decimal? SALES_TAX_PARTS_CPH { get; set; }
        public decimal? SALES_TAX_LABOR_CPH { get; set; }
        public double? TOT_MANH_OT { get; set; }
        public double? TOT_MANH_DT { get; set; }
        public double? TOT_MANH_MISC { get; set; }
        public string RKRP_XMIT_SW { get; set; }
        public string CUCDN { get; set; }
        public string RKRPLOC { get; set; }
        public string ACEP_SW { get; set; }
        public string SHOP_TYPE_CD { get; set; }
        public string WO_RKRP_XMIT_SW { get; set; }
    }
}
