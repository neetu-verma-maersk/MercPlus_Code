using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.Objects;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace ManageReportsService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class ManageReportsService : IManageReports
    {
        ManageReportsServiceEntities objContext = new ManageReportsServiceEntities();

        #region GetReportsDetailsMercA01
        public List<WorkOrder> GetReportsDetailsMercA01(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual)//, string MercA01, string MERCB01, string MERCC01,
                                                //string MERCD03, string MERCD05, string MERCE01)
        {
            objContext = new ManageReportsServiceEntities();
            //Get the List of countries
            List<WorkOrder> WorkOrderList = new List<WorkOrder>();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            string shopCode = string.Empty;
            DateTime startDate = Convert.ToDateTime(DateFrom); 
            string dateTo = DateTo + " " +"23:59:59";
            DateTime endDate = Convert.ToDateTime(dateTo);
            short startStatusCode = 600;
            short endStatusCode = 9000;

            var results = (from S in objContext.MESC1TS_SHOP
                           from ST in objContext.MESC1TS_SHOP_CONT
                           from CR in objContext.MESC1TS_CURRENCY
                           from V in objContext.MESC1TS_VENDOR
                           from RP in objContext.MESC1TS_REPAIR_CODE
                           where S.SHOP_CD == ST.SHOP_CD &&
                                 S.VENDOR_CD == V.VENDOR_CD &&
                                 S.CUCDN == CR.CUCDN &&
                                 ST.MANUAL_CD == RP.MANUAL_CD &&
                                 ST.MODE == RP.MODE &&
                                 ST.REPAIR_CD == RP.REPAIR_CD
                           select new { ST.MODE, ST.REPAIR_CD, RP.REPAIR_DESC, ST.CONTRACT_AMOUNT, ST.EFF_DTE, ST.EXP_DTE }).Take(10);

            #region MercA01
            //string whereCond = "wo.CUSTOMER_CD = csm.CUSTOMER_CD and wo.MODE = csm.MODE and wo.SHOP_CD = csm.SHOP_CD";
            //if (MercA01 != null || !string.IsNullOrEmpty(MercA01))
            //{
            string a = string.Empty;
            var WOrderMercA01 = (from wo in objContext.MESC1TS_WO
                                 from csm in objContext.MESC1TS_CUST_SHOP_MODE
                                 where wo.CUSTOMER_CD == csm.CUSTOMER_CD &&
                                       wo.MODE == csm.MODE &&
                                       wo.SHOP_CD == csm.SHOP_CD &&
                                       wo.RKRP_XMIT_DTE >= startDate &&
                                       wo.RKRP_XMIT_DTE <= endDate &&
                                       wo.STATUS_CODE >= startStatusCode &&
                                       wo.STATUS_CODE < endStatusCode &&
                                       wo.SHOP_CD == ShopCode &&
                                       (CustomerCode == null ? a == a : wo.CUSTOMER_CD == CustomerCode) &&
                                       (Manual == null ? a == a : wo.MANUAL_CD == Manual) &&
                                       (Mode == null ? a == a : wo.MODE == Mode)
                                     orderby wo.MODE//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                                 select new
                                 {
                                     wo.MODE,
                                     wo.REPAIR_DTE,
                                     csm.PAYAGENT_CD,
                                     wo.EQPNO,
                                     wo.WO_ID,
                                     wo.VENDOR_REF_NO,
                                     wo.VOUCHER_NO,
                                     wo.TOT_MANH_REG,
                                     wo.TOT_MANH_OT,
                                     wo.TOT_MANH_DT,
                                     wo.TOT_MANH_MISC,
                                     wo.TOT_REPAIR_MANH,
                                     wo.TOT_LABOR_COST_CPH,
                                     wo.TOT_MAN_PARTS_CPH,
                                     wo.TOT_SHOP_AMT_CPH,
                                     wo.IMPORT_TAX_CPH,
                                     wo.SALES_TAX_PARTS_CPH,
                                     wo.SALES_TAX_LABOR_CPH,
                                     wo.TOT_COST_CPH,
                                     wo.TOT_MAERSK_PARTS_CPH,
                                     wo.TOT_COST_REPAIR_CPH
                                 }).Take(10);

                //if (CustomerCode != null)
                //{
                //    WOrderMercA01 = WOrderMercA01 + 
                //}
                if (WOrderMercA01 != null && WOrderMercA01.Count() != 0)
                {
                    //lambda exp
                    foreach (var obj in WOrderMercA01)
                    {
                        WorkOrder WorkOrder = new WorkOrder();
                        WorkOrder.ActualCompletionDate = obj.REPAIR_DTE.ToString();
                        WorkOrder.Mode = obj.MODE;
                        WorkOrder.EquipmentNo = obj.EQPNO;
                        WorkOrder.AgentCode = obj.PAYAGENT_CD;
                        WorkOrder.WorkOrderNo = obj.WO_ID.ToString();
                        WorkOrder.VendorRefNo = obj.VENDOR_REF_NO;
                        //If condition to be added
                        WorkOrder.VoucherNumber = obj.VOUCHER_NO;
                        WorkOrder.OrdinaryManHours = obj.TOT_MANH_REG.ToString();
                        WorkOrder.Overtime1ManHours = obj.TOT_MANH_OT.ToString();
                        WorkOrder.Overtime2ManHours = obj.TOT_MANH_DT.ToString();
                        WorkOrder.Overtime3ManHours = obj.TOT_MANH_MISC.ToString();
                        WorkOrder.TotalHours = obj.TOT_REPAIR_MANH.ToString();
                        WorkOrder.TotalLabourCostCPH = obj.TOT_LABOR_COST_CPH.ToString();
                        WorkOrder.TotalCostOfShopSuppliedNumberedParts = obj.TOT_MAN_PARTS_CPH.ToString();
                        WorkOrder.TotalCostOfShopSuppliedMaterials = obj.TOT_SHOP_AMT_CPH.ToString();
                        WorkOrder.ImportTax = obj.IMPORT_TAX_CPH.ToString();
                        WorkOrder.SalesTaxParts = obj.SALES_TAX_PARTS_CPH.ToString();
                        WorkOrder.SalesTaxLabour = obj.SALES_TAX_LABOR_CPH.ToString();
                        WorkOrder.TotalToBePaidToShop = obj.TOT_COST_CPH.ToString();
                        WorkOrder.TotalCostOfCPHSuppliedParts = obj.TOT_MAERSK_PARTS_CPH.ToString();
                        WorkOrder.TotalCostOfRepairCPH = obj.TOT_COST_REPAIR_CPH.ToString();
                        WorkOrderList.Add(WorkOrder);
                    }
                }
            //}
            #endregion MercA01

            return WorkOrderList;
        }
        #endregion GetReportsDetailsMercA01

        #region GetReportsDetailsMercA02
        public List<WorkOrder> GetReportsDetailsMercA02(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual)//, string MercA01, string MERCB01, string MERCC01,
        //string MERCD03, string MERCD05, string MERCE01)
        {
            objContext = new ManageReportsServiceEntities();
            //Get the List of countries
            List<WorkOrder> WorkOrderList = new List<WorkOrder>();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            //string shopCode = string.Empty;
            ShopCode = "134";
            DateTime startDate = Convert.ToDateTime(DateFrom);
            string dateTo = DateTo + " " + "23:59:59";
            DateTime endDate = Convert.ToDateTime(dateTo);
            short startStatusCode = 600;
            short endStatusCode = 9000;
            string a = string.Empty;
            

            #region MercA02
            var WOrderMERCA02 = (from wo in objContext.MESC1TS_WO
                                    from shop in objContext.MESC1TS_SHOP
                                    from csm in objContext.MESC1TS_CUST_SHOP_MODE
                                    where wo.SHOP_CD == shop.SHOP_CD &&
                                        wo.CUSTOMER_CD == csm.CUSTOMER_CD &&
                                        wo.MODE == csm.MODE &&
                                        wo.SHOP_CD == csm.SHOP_CD &&
                                        wo.SHOP_CD == ShopCode &&
                                        wo.RKRP_XMIT_DTE >= startDate &&
                                        wo.RKRP_XMIT_DTE <= endDate &&
                                        wo.STATUS_CODE >= startStatusCode &&
                                        wo.STATUS_CODE < endStatusCode &&
                                        wo.SHOP_CD == ShopCode &&
                                       (CustomerCode == null ? a == a : wo.CUSTOMER_CD == CustomerCode) &&
                                       (Manual == null ? a == a : wo.MANUAL_CD == Manual) &&
                                       (Mode == null ? a == a : wo.MODE == Mode)
                                       orderby wo.MODE //, (wo.REPAIR_DTE), month(wo.REPAIR_DTE), day(wo.REPAIR_DTE), EQPNO
                                    select new
                                    {
                                        wo.MODE,
                                        wo.REPAIR_DTE,
                                        wo.EQPNO,
                                        wo.WO_ID,
                                        wo.VENDOR_REF_NO,
                                        wo.VOUCHER_NO,
                                        csm.PAYAGENT_CD,
                                        wo.EXCHANGE_RATE,
                                        wo.TOT_MANH_REG,
                                        wo.TOT_MANH_OT,
                                        wo.TOT_MANH_DT,
                                        wo.TOT_MANH_MISC,
                                        wo.TOT_LABOR_COST,
                                        wo.TOT_REPAIR_MANH,
                                        shop.SHOP_TYPE_CD,
                                        wo.TOT_MAN_PARTS, //Total cost of numbered parts
                                        wo.TOT_SHOP_AMT, //Total cost of supplied materials
                                        wo.IMPORT_TAX,
                                        wo.SALES_TAX_PARTS,
                                        wo.SALES_TAX_LABOR,
                                        wo.TOT_COST_LOCAL, //Total to be paid to shop
                                        wo.TOTAL_COST_LOCAL_USD, // Total to be paid to shop in USD
                                        //CAST(( tot_cost_cph * 100 )/wo.Exchange_rate AS NUMERIC(12,2)) 'Total to be paid to agent from CPH in local currency'
                                        wo.TOT_COST_REPAIR_CPH, //Total cost of repair (incl. CPH supplied parts)in USD
                                        wo.TOT_MAERSK_PARTS_CPH, //Total cost of CPH supplied parts in USD
                                        wo.TOT_COST_CPH, //Total to be paid to agent from CPH in USD

                                    });

            int PartSupplierCode = 4;
            foreach (var obj in WOrderMERCA02)
            {
                WorkOrder WorkOrder = new WorkOrder();
                WorkOrder.MODE = obj.MODE;
                WorkOrder.ActualCompletionDate = obj.REPAIR_DTE.ToString();
                WorkOrder.EquipmentNo = obj.EQPNO;
                WorkOrder.AgentCode = obj.PAYAGENT_CD;
                WorkOrder.WorkOrderNo = obj.WO_ID.ToString();
                WorkOrder.VendorRefNo = obj.VENDOR_REF_NO;
                WorkOrder.ExchangeRate = obj.EXCHANGE_RATE.ToString();
                WorkOrder.VoucherNumber = obj.VOUCHER_NO;
                WorkOrder.OrdinaryManHours = obj.TOT_MANH_REG.ToString();
                WorkOrder.Overtime1ManHours = obj.TOT_MANH_OT.ToString();
                WorkOrder.Overtime2ManHours = obj.TOT_MANH_DT.ToString();
                WorkOrder.Overtime3ManHours = obj.TOT_MANH_MISC.ToString();
                WorkOrder.TotalHours = obj.TOT_REPAIR_MANH.ToString();
                WorkOrder.TotalLabourCost = obj.TOT_LABOR_COST.ToString();
                //If condition to be added for shop/agent
                WorkOrder.TotalCostOfNumberedParts = obj.TOT_MAN_PARTS.ToString();
                WorkOrder.TotalCostOfSuppliedMaterials = obj.TOT_SHOP_AMT.ToString();
                WorkOrder.ImportTax = obj.IMPORT_TAX.ToString();
                WorkOrder.SalesTaxParts = obj.SALES_TAX_PARTS.ToString();
                WorkOrder.SalesTaxLabour = obj.SALES_TAX_LABOR.ToString();
                WorkOrder.TotalToBePaidToShop = obj.TOT_COST_LOCAL.ToString();
                WorkOrder.TotalToBePaidToShopInUSD = obj.TOTAL_COST_LOCAL_USD.ToString();
                //WorkOrder.TotalCostOfCPHSuppliedParts = obj.TOT_MAERSK_PARTS_CPH.ToString();
                WorkOrder.TotalCostOfRepairCPH = obj.TOT_COST_REPAIR_CPH.ToString();
                WorkOrder.TotalCostOfCPHSuppliedPartsInUSD = obj.TOT_MAERSK_PARTS_CPH.ToString();
                WorkOrder.TotalToBePaidToAgentFromCPHInUSD = obj.TOT_COST_CPH.ToString();
                WorkOrder.SHOP_CD = obj.SHOP_TYPE_CD;
                //WorkOrder.ExchangeRate = Convert.ToString((100 / (obj.EXCHANGE_RATE * .01)));
                if (obj.SHOP_TYPE_CD == PartSupplierCode.ToString())
                {
                    WorkOrder.PartSupplier = "Agent";
                }
                else
                {
                    WorkOrder.PartSupplier = "Shop";
                }
                WorkOrder.TotalToBePaidToAgentFromCPHInLocalCurrency = ((obj.TOT_COST_CPH * 100) / obj.EXCHANGE_RATE).ToString();
                WorkOrderList.Add(WorkOrder);
            }

            #endregion MercA02

            return WorkOrderList;
        }
        #endregion GetReportsDetailsMercA02

        #region GetReportsDetailsMercA03
        public List<WorkOrder> GetReportsDetailsMercA03(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual)//, string MercA01, string MERCB01, string MERCC01,
        //string MERCD03, string MERCD05, string MERCE01)
        {
            objContext = new ManageReportsServiceEntities();
            //Get the List of countries
            List<WorkOrder> WorkOrderList = new List<WorkOrder>();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            string shopCode = string.Empty;
            DateTime startDate = Convert.ToDateTime(DateFrom);
            string dateTo = DateTo + " " + "23:59:59";
            DateTime endDate = Convert.ToDateTime(dateTo);
            short startStatusCode = 600;
            short endStatusCode = 9000;
            string a = string.Empty;
            #region MercA03
            
            var WOrderMERCA03 = (from WO in objContext.MESC1TS_WO
                                    from S in objContext.MESC1TS_SHOP
                                    from C in objContext.MESC1TS_CURRENCY
                                    from V in objContext.MESC1TS_VENDOR
                                    where WO.SHOP_CD == S.SHOP_CD &&
                                        WO.VENDOR_CD == V.VENDOR_CD &&
                                        WO.CUCDN == C.CUCDN &&
                                        WO.RKRP_XMIT_DTE >= startDate &&
                                        WO.RKRP_XMIT_DTE <= endDate &&
                                        WO.STATUS_CODE >= startStatusCode &&
                                        WO.STATUS_CODE < endStatusCode &&
                                        WO.SHOP_CD == ShopCode &&
                                        (CustomerCode == null ? a == a : WO.CUSTOMER_CD == CustomerCode) &&
                                        (Manual == null ? a == a : WO.MANUAL_CD == Manual) &&
                                        (Mode == null ? a == a : WO.MODE == Mode)
                                        orderby WO.MODE//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                                    select new
                                    {
                                        WO.MODE,
                                        WO.REPAIR_DTE,
                                        WO.EQPNO,
                                        WO.WO_ID,
                                        WO.VENDOR_REF_NO,
                                        WO.TOT_MANH_REG,
                                        WO.TOT_MANH_OT,
                                        WO.TOT_MANH_DT,
                                        WO.TOT_MANH_MISC,
                                        WO.TOT_LABOR_COST,
                                        WO.TOT_REPAIR_MANH,
                                        S.SHOP_TYPE_CD,
                                        WO.TOT_MAN_PARTS,
                                        WO.TOT_SHOP_AMT,
                                        WO.IMPORT_TAX,
                                        WO.SALES_TAX_PARTS,
                                        WO.SALES_TAX_LABOR,
                                        WO.TOT_COST_LOCAL
                                        //wo.TOTAL_COST_LOCAL_USD,
                                        //wo.TOT_COST_REPAIR_CPH,
                                        //wo.TOT_MAERSK_PARTS_CPH,
                                        //wo.TOT_COST_CPH,
                                    });


            foreach (var obj in WOrderMERCA03)
            {
                WorkOrder WorkOrder = new WorkOrder();
                //WorkOrder.Mode = obj.MODE;
                WorkOrder.ActualCompletionDate = obj.REPAIR_DTE.ToString();
                WorkOrder.EquipmentNo = obj.EQPNO;
                //WorkOrder.AgentCode = obj.PAYAGENT_CD;
                WorkOrder.WorkOrderNo = obj.WO_ID.ToString();
                WorkOrder.VendorRefNo = obj.VENDOR_REF_NO;
                //WorkOrder.ExchangeRate = obj.EXCHANGE_RATE.ToString();
                //WorkOrder.VoucherNumber = obj.VOUCHER_NO;
                WorkOrder.OrdinaryManHours = obj.TOT_MANH_REG.ToString();
                WorkOrder.Overtime1ManHours = obj.TOT_MANH_OT.ToString();
                WorkOrder.Overtime2ManHours = obj.TOT_MANH_DT.ToString();
                WorkOrder.Overtime3ManHours = obj.TOT_MANH_MISC.ToString();
                WorkOrder.TotalHours = obj.TOT_REPAIR_MANH.ToString();
                WorkOrder.TotalLabourCost = obj.TOT_LABOR_COST.ToString();
                //If condition to be added for shop/agent
                WorkOrder.TotalCostOfNumberedParts = obj.TOT_MAN_PARTS.ToString();
                WorkOrder.TotalCostOfSuppliedMaterials = obj.TOT_SHOP_AMT.ToString();
                WorkOrder.ImportTax = obj.IMPORT_TAX.ToString();
                WorkOrder.SalesTaxParts = obj.SALES_TAX_PARTS.ToString();
                WorkOrder.SalesTaxLabour = obj.SALES_TAX_LABOR.ToString();
                WorkOrder.TotalToBePaidToShop = obj.TOT_COST_LOCAL.ToString();
               // WorkOrder.TotalCostOfCPHSuppliedParts = obj.TOT_MAERSK_PARTS_CPH.ToString();
                //WorkOrder.TotalCostOfRepair = obj.TOT_COST_REPAIR_CPH.ToString();
                WorkOrderList.Add(WorkOrder);
            }
            #endregion MercA03
            
            //Header
            //"select convert(char(16),getutcdate(),120) date, v.Vendor_cd 'Vendor_cd', v.Vendor_desc 'Vendor_desc'," & _ 
            //   "s.shop_cd 'SHOP_CD', s.shop_desc, c.cucdn, c.currnamc 'Currency'" & _ 
            //   "from mesc1ts_shop s, mesc1ts_vendor v, mesc1ts_currency c " & _  
            //   "where v.vendor_cd = s.vendor_cd " & _  
            //   "and s.cucdn = c.cucdn and s.shop_cd ='"&Shop&"'" 

            var header = (from s in objContext.MESC1TS_SHOP
                          from v in objContext.MESC1TS_VENDOR
                          from c in objContext.MESC1TS_CURRENCY
                          where v.VENDOR_CD == s.VENDOR_CD &&
                                s.CUCDN == c.CUCDN &&
                                s.SHOP_CD == shopCode
                          select new {
                              v.VENDOR_CD,
                              v.VENDOR_DESC,
                              s.SHOP_CD,
                              s.SHOP_DESC,
                              c.CUCDN,
                              c.CURRNAMC
                          });

            foreach (var obj in header)
            {
                WorkOrder WorkOrder = new WorkOrder();
                
            }

            return WorkOrderList;
        }
        #endregion GetReportsDetailsMercA03

        #region GetReportsDetailsMercB01
        public List<WorkOrder> GetReportsDetailsMercB01(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, string RepairCode)//, string MercA01, string MERCB01, string MERCC01,
        //string MERCD03, string MERCD05, string MERCE01)
        {
            objContext = new ManageReportsServiceEntities();
            //Get the List of countries
            List<WorkOrder> WorkOrderList = new List<WorkOrder>();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            string shopCode = string.Empty;
            DateTime startDate = Convert.ToDateTime(DateFrom);
            string dateTo = DateTo + " " + "23:59:59";
            DateTime endDate = Convert.ToDateTime(dateTo);
            short startStatusCode = 600;
            short endStatusCode = 9000;
            string a = string.Empty;

            #region MercB01

            //"Select wo.SHOP_CD  'Shop Code'," & _
            //"wo.MODE 'Mode'," & _
            //"wo.eqpno 'Equipment No'," & _
            //"" & _
            //"wo.REPAIR_DTE  'Completion Date'," & _
            //"wo.wo_id 'Work order no.'," & _
            //"wo.vendor_ref_no 'Vendor Ref No'," & _
            //"wor.REPAIR_CD 'Repair Code'," & _
            //"REPAIR_DESC  'Repair code description'," & _
            //"wop.PART_CD 'Part Number'," & _
            //"PART_DESC  'Part number description'," & _
            //"MANUFACTUR_NAME 'Manufacturer'," & _
            //"QTY_PARTS  'Part Quantity'," & _
            //"COST_LOCAL 'Total cost of parts'," & _
            //"MSL_PART_SW  'Maersk Sealand CPH,supplied part'," & _
            //"CORE_PART_SW  'Core part'," & _
            //"SERIAL_NUMBER  'Core serial no/,TAG number' " & _
            //"from mesc1ts_wo wo, mesc1ts_worepair wor, mesc1ts_wopart wop, mesc1ts_manufactur mfr, " & _
            //"mesc1ts_shop sh, mesc1ts_vendor vr, mesc1ts_repair_code wrc,mesc1ts_master_part mp " & _
            //"where wo.WO_ID=wor.WO_ID " & _
            //"and wo.WO_ID=wop.WO_ID " & _
            //"and wor.REPAIR_CD=wop.REPAIR_CD " & _
            //"and wop.PART_CD=mp.PART_CD " & _
            //"and mp.MANUFCTR=mfr.MANUFCTR " & _
            //"and wo.MANUAL_CD=wrc.MANUAL_CD " & _
            //"and wo.MODE=wrc.MODE " & _
            //"and wor.REPAIR_CD=wrc.REPAIR_CD " & _
            //"and wo.SHOP_CD=sh.SHOP_CD " & _
            //"and sh.VENDOR_CD=vr.VENDOR_CD " & _
            //"and wop.PART_CD!='' " & _
            //"and RKRP_XMIT_DTE> '"&DateFrom&"' and RKRP_XMIT_DTE<'"&DateEnd&"' " & _
            //"and wo.SHOP_CD='"&Shop&"' " & _
            //"and status_code>=600 and status_code<9000 " 

            var WOrderMercB01 = (from wo in objContext.MESC1TS_WO
                                 from wor in objContext.MESC1TS_WOREPAIR
                                 from wop in objContext.MESC1TS_WOPART
                                 from mfr in objContext.MESC1TS_MANUFACTUR
                                 from sh in objContext.MESC1TS_SHOP 
                                 from vr in objContext.MESC1TS_VENDOR
                                 from wrc in objContext.MESC1TS_REPAIR_CODE
                                 from mp in objContext.MESC1TS_MASTER_PART
                                 where wo.WO_ID == wor.WO_ID &&
                                     wo.WO_ID == wop.WO_ID &&
                                     wor.REPAIR_CD == wop.REPAIR_CD &&
                                     wop.PART_CD == mp.PART_CD &&
                                     mp.MANUFCTR == mfr.MANUFCTR &&
                                     wo.MANUAL_CD == wrc.MANUAL_CD &&
                                     wo.MODE == wrc.MODE &&
                                     wor.REPAIR_CD == wrc.REPAIR_CD &&
                                     wo.SHOP_CD == sh.SHOP_CD &&
                                     sh.VENDOR_CD == vr.VENDOR_CD &&
                                     wop.PART_CD != string.Empty &&
                                     wo.RKRP_XMIT_DTE >= startDate &&
                                     wo.RKRP_XMIT_DTE <= endDate &&
                                     wo.STATUS_CODE >= startStatusCode &&
                                     wo.STATUS_CODE < endStatusCode &&
                                     wo.SHOP_CD == ShopCode &&
                                     //(RepairCode == null ? a == a : wor.REPAIR_CD == RepairCode) &&
                                     (Mode == null ? a == a : wo.MODE == Mode)
                                     orderby wo.SHOP_CD, wo.MODE, wo.EQPNO //, year(wo.RKRP_XMIT_DTE),month(wo.RKRP_XMIT_DTE),day(wo.RKRP_XMIT_DTE), year(wo.REPAIR_DTE),month(wo.REPAIR_DTE),day(wo.REPAIR_DTE), wo.wo_id, wo.vendor_ref_no, wor.REPAIR_CD, REPAIR_DESC,wop.PART_CD  " 
                                 select new
                                 {
                                     wo.SHOP_CD,  //'Shop Code',
                                     wo.MODE, //'Mode,
                                     wo.EQPNO, // 'Equipment No'
                                     wo.REPAIR_DTE,  //'Completion Date'
                                     wo.WO_ID, // 'Work order no
                                     wo.VENDOR_REF_NO,
                                     wor.REPAIR_CD, //'Repair Code' 
                                     wrc.REPAIR_DESC, //'Repair code description'
                                     wop.PART_CD, // 'Part Number',
                                     mp.PART_DESC, //Part number description'
                                     mfr.MANUFACTUR_NAME, //Manufacturer
                                     wop.QTY_PARTS, //Part Quantity
                                     wop.COST_LOCAL, //Total cost of parts
                                     mp.MSL_PART_SW, //'Maersk Sealand CPH,supplied part'
                                     mp.CORE_PART_SW, //'Core part'
                                     wop.SERIAL_NUMBER //SERIAL_NUMBER
                                 });


            foreach (var obj in WOrderMercB01)
            {
                WorkOrder WorkOrder = new WorkOrder();
                WorkOrder.Mode = obj.MODE;
                WorkOrder.ActualCompletionDate = obj.REPAIR_DTE.ToString();
                WorkOrder.EquipmentNo = obj.EQPNO;
                WorkOrder.WorkOrderNo = obj.WO_ID.ToString();
                WorkOrder.VendorRefNo = obj.VENDOR_REF_NO;
                WorkOrder.SHOP_CD = obj.SHOP_CD;
                WorkOrder.REPAIR_CD = obj.REPAIR_CD;
                WorkOrder.REPAIR_DESC = obj.REPAIR_DESC;
                WorkOrder.PART_CD = obj.PART_CD;
                WorkOrder.PART_DESC = obj.PART_DESC;
                WorkOrder.ManufacturerName = obj.MANUFACTUR_NAME;
                WorkOrder.QTY_PARTS = obj.QTY_PARTS.ToString();
                WorkOrder.COST_LOCAL = obj.COST_LOCAL.ToString();
                WorkOrder.MSL_PART_SW = obj.MSL_PART_SW;
                WorkOrder.CORE_PART_SW = obj.CORE_PART_SW;
                WorkOrder.SERIAL_NUMBER = obj.SERIAL_NUMBER;
                WorkOrderList.Add(WorkOrder);
            }
            #endregion MercB01
            return WorkOrderList;
        }
        #endregion GetReportsDetailsMercB01

        #region GetReportsDetailsMercC01
        public List<WorkOrder> GetReportsDetailsMercC01()
        //string MERCD03, string MERCD05, string MERCE01)
        {
            objContext = new ManageReportsServiceEntities();
            //Get the List of countries
            List<WorkOrder> WorkOrderList = new List<WorkOrder>();
            
            #region MercA03
            //select a.manual_cd manual,a.mode mode ,a.repair_cd rpr,a.repair_desc rpr_desc,b.exclu_repair_cd ex_rpr,c.repair_desc ex_rpr_desc " &_
                    //"from mesc1ts_repair_code a , mesc1ts_rprcode_exclu b,mesc1ts_repair_code c " &_
                    //"where a.manual_cd = b.manual_cd " &_
                    //"and a.repair_cd=b.repair_cd " &_
                    //"and a.mode=b.mode " &_
                    //"and b.manual_cd = c.manual_cd " &_
                    //"and b.mode=c.mode " &_
                    //"and b.exclu_repair_cd = c.repair_cd " &_
                    //"order by a.manual_cd,a.mode,a.repair_Cd,b.exclu_repair_cd
            var WOrderMERCC01 = (from A in objContext.MESC1TS_REPAIR_CODE
                                 from B in objContext.MESC1TS_RPRCODE_EXCLU
                                 from C in objContext.MESC1TS_REPAIR_CODE
                                 where A.MANUAL_CD == B.MANUAL_CD &&
                                     A.REPAIR_CD == B.REPAIR_CD &&
                                     A.MODE == B.MODE &&
                                     B.MANUAL_CD == C.MANUAL_CD &&
                                     B.MODE == C.MODE &&
                                     B.EXCLU_REPAIR_CD == C.REPAIR_CD
                                 orderby A.MANUAL_CD, A.MODE, A.REPAIR_CD, B.EXCLU_REPAIR_CD
                                 select new
                                 {
                                     A.MANUAL_CD, //manual
                                     A.MODE, //mode
                                     A.REPAIR_CD, //rpr
                                     A.REPAIR_DESC, //rpr_desc
                                     B.EXCLU_REPAIR_CD,//ex_rpr
                                     //Confusion
                                     ExclusionaryRepairCode = C.REPAIR_DESC,  //ex_rpr_desc
                                     ExclusionaryRepairCodeDesc = C.REPAIR_DESC
                                 }).Take(10);


            foreach (var obj in WOrderMERCC01)
            {
                WorkOrder WorkOrder = new WorkOrder();
                WorkOrder.Mode = obj.MODE;
                WorkOrder.Manual = obj.MANUAL_CD;
                WorkOrder.REPAIR_CD = obj.REPAIR_CD;
                WorkOrder.ExclusionaryRepairCode = obj.ExclusionaryRepairCode;
                WorkOrder.ExclusionaryRepairCodeDescription = obj.ExclusionaryRepairCodeDesc;
                WorkOrder.REPAIR_DESC = obj.REPAIR_DESC;
                WorkOrderList.Add(WorkOrder);
            }
            #endregion MercA03

            return WorkOrderList;
        }
        #endregion GetReportsDetailsMercC01

        #region GetReportsDetailsMercC02
        public List<WorkOrder> GetReportsDetailsMercC02()//, string MercA01, string MERCB01, string MERCC01,
        //string MERCD03, string MERCD05, string MERCE01)
        {
            objContext = new ManageReportsServiceEntities();
            //Get the List of countries
            List<WorkOrder> WorkOrderList = new List<WorkOrder>();
            
            #region MercA03
            //"	select a.manual_cd, a.mode, a.repair_cd, b.repair_desc, a.part_cd, c.part_desc " &_
                    //"from mesc1ts_rprcode_part a, mesc1ts_repair_code b, mesc1ts_master_part c " &_
                    //"where a.manual_cd = b.manual_cd and a.mode = b.mode and " &_
                    //"a.repair_cd = b.repair_cd and a.PART_CD = c.part_cd order by a.mode, a.repair_cd, a.part_cd "

            var WOrderMERCC02 = (from A in objContext.MESC1TS_RPRCODE_PART
                                 from B in objContext.MESC1TS_REPAIR_CODE
                                 from C in objContext.MESC1TS_MASTER_PART
                                 where A.MANUAL_CD == B.MANUAL_CD &&
                                     A.MODE == B.MODE &&
                                     A.REPAIR_CD == B.REPAIR_CD &&
                                     A.PART_CD == C.PART_CD
                                 orderby A.MODE, A.REPAIR_CD, A.PART_CD
                                 select new
                                 {
                                     A.MANUAL_CD, //manual
                                     A.MODE, //mode
                                     A.REPAIR_CD, //rpr
                                     B.REPAIR_DESC, //rpr_desc
                                     A.PART_CD,//ex_rpr
                                     C.PART_DESC //ex_rpr_desc
                                 }).Take(10);


            foreach (var obj in WOrderMERCC02)
            {
                WorkOrder WorkOrder = new WorkOrder();
                WorkOrder.Mode = obj.MODE;
                WorkOrder.Manual = obj.MANUAL_CD;
                WorkOrder.REPAIR_CD = obj.REPAIR_CD;
                WorkOrder.REPAIR_DESC = obj.REPAIR_DESC;
                WorkOrder.PART_CD = obj.PART_CD;
                WorkOrder.PART_DESC = obj.PART_DESC;
                WorkOrderList.Add(WorkOrder);
            }
            #endregion MercA03

            return WorkOrderList;
        }
        #endregion GetReportsDetailsMercC01

        #region GetReportsDetailsMercD03
        public List<WorkOrder> GetReportsDetailsMercD03(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string ManualCode, string Mode)//, string MercA01, string MERCB01, string MERCC01,
        //string MERCD03, string MERCD05, string MERCE01)
        {
            objContext = new ManageReportsServiceEntities();
            //Get the List of countries
            List<WorkOrder> WorkOrderList = new List<WorkOrder>();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            string shopCode = string.Empty;
            DateTime startDate = Convert.ToDateTime(DateFrom);
            DateTime endDate = Convert.ToDateTime(DateTo);
            string dateTo = DateTo + "23:59:59";
            //short startStatusCode = 600;
            //short endStatusCode = 9000;
            DateTime CurrentDate = DateTime.Now;
            string a = string.Empty;

            #region MercD03

            var sqlMode = (from S in objContext.MESC1TS_SHOP 
                                 from ST in objContext.MESC1TS_SHOP_CONT
                                 from CR in objContext.MESC1TS_CURRENCY
                                 from V in objContext.MESC1TS_VENDOR
                                 from RP in objContext.MESC1TS_REPAIR_CODE
                                 where S.SHOP_CD == ST.SHOP_CD &&
                                     S.VENDOR_CD == V.VENDOR_CD &&
                                     S.CUCDN == CR.CUCDN &&
                                     ST.MANUAL_CD == RP.MANUAL_CD &&
                                     ST.MODE == RP.MODE  &&
                                     ST.REPAIR_CD == RP.REPAIR_CD &&
                                     ST.SHOP_CD == shopCode &&
                                     ST.MANUAL_CD == ManualCode &&
                                     ST.EFF_DTE <= CurrentDate &&
                                     ST.EXP_DTE >= CurrentDate &&
                                     (Mode == null ? a == a : ST.MODE == Mode)
                                     orderby ST.MODE //asc
                                 select new
                                 {
                                    ST.MODE
                                 }).Distinct();
            if (Mode != null)
            {
                //If strMode<>"***Any***" then
                //    sSQL=sSQL & " and  ST.Mode = '" & strMode & "' "	
                //End if
                //sSQL=sSQL & "order by ST.MODE asc"	
            }

            if (Mode != null)
            {
                var WOrderMERCD03 = (from S in objContext.MESC1TS_SHOP
                                     from ST in objContext.MESC1TS_SHOP_CONT
                                     from CR in objContext.MESC1TS_CURRENCY
                                     from V in objContext.MESC1TS_VENDOR
                                     from RP in objContext.MESC1TS_REPAIR_CODE
                                     where S.SHOP_CD == ST.SHOP_CD &&
                                         S.VENDOR_CD == V.VENDOR_CD &&
                                         S.CUCDN == CR.CUCDN &&
                                         ST.MANUAL_CD == RP.MANUAL_CD &&
                                         ST.MODE == RP.MODE &&
                                         ST.REPAIR_CD == RP.REPAIR_CD &&
                                         ST.SHOP_CD == shopCode &&
                                         ST.MANUAL_CD == ManualCode &&
                                         ST.EFF_DTE <= CurrentDate &&
                                         ST.EXP_DTE >= CurrentDate &&
                                         ST.MODE == Mode
                                         orderby ST.MODE, ST.REPAIR_CD, ST.EFF_DTE
                                     select new
                                     {
                                         ST.MODE,
                                         ST.REPAIR_CD, //'Repair Code'
                                         RP.REPAIR_DESC, //Repair Code Description
                                         ST.CONTRACT_AMOUNT, //Max material amount pr piece
                                         ST.EFF_DTE, //Rate Effective Date
                                         ST.EXP_DTE //Rate expiry date
                                     });
            
            foreach (var obj in WOrderMERCD03)
            {
                WorkOrder WorkOrder = new WorkOrder();
                //WorkOrder.Mode = obj.MODE;
                WorkOrder.Mode = obj.MODE;
                WorkOrder.RepairCode = obj.REPAIR_CD;
                WorkOrder.RepairCodeDesc = obj.REPAIR_DESC;
                WorkOrder.MaxMaterialAmountPrPiece = obj.CONTRACT_AMOUNT.ToString();
                WorkOrder.RateEffectiveDate = obj.EFF_DTE.ToString();
                WorkOrder.RateExpiryDate = obj.EXP_DTE.ToString();
                WorkOrderList.Add(WorkOrder);
            }
            }
            #endregion MercD03

            return WorkOrderList;
        }
        #endregion GetReportsDetailsMercD03

        #region GetReportsDetailsMercD05
        public List<WorkOrder> GetReportsDetailsMercD05(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, string Country)//, string MercA01, string MERCB01, string MERCC01,
        //string MERCD03, string MERCD05, string MERCE01)
        {
            objContext = new ManageReportsServiceEntities();
            //Get the List of countries
            List<WorkOrder> WorkOrderList = new List<WorkOrder>();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            string shopCode = string.Empty;
            DateTime startDate = Convert.ToDateTime(DateFrom);
            string dateTo = DateTo + " " + "23:59:59";
            DateTime endDate = Convert.ToDateTime(dateTo);
            DateTime CurrentDate = DateTime.Now;
            string a = string.Empty;

            //Setting the Header part, we need to know the country, date and the currency type
            List<MESC1TS_COUNTRY> CountryList = new List<MESC1TS_COUNTRY>();
            //CountryList = (from country in objContext.MESC1TS_COUNTRY select country).ToList();
            var country = from ctr in objContext.MESC1TS_COUNTRY
                          where ctr.COUNTRY_CD == Country
                          select new 
                          {
                              ctr.COUNTRY_DESC,
                          };
            //"select CUCDN,CURRNAMC,ROUND(100/EXRATUSD,6) 'Exchange Rate' from mesc1ts_currency 
            //where cucdn in (select cucdn from mesc1ts_country_cont where country_cd='" & strCountry & "')"
            //var currency = from curr in objContext.MESC1TS_CURRENCY
            //               where cucdn in (select cucdn from objContext.MESC1TS_COUNTRY_CONT where cou
            

            #region MercD05
          
            var sqlMode = (from C in objContext.MESC1TS_COUNTRY
                                 from CT in objContext.MESC1TS_COUNTRY_CONT
                                 from CR in objContext.MESC1TS_CURRENCY 
                                 from RC in objContext.MESC1TS_REPAIR_CODE
                                 where C.COUNTRY_CD == CT.COUNTRY_CD &&
                                       CT.CUCDN == CR.CUCDN &&
                                       CT.MANUAL_CD == RC.MANUAL_CD &&
                                       CT.MODE == RC.MODE &&
                                       CT.REPAIR_CD == RC.REPAIR_CD &&
                                       CT.MANUAL_CD == Manual &&
                                       C.COUNTRY_CD == Country &&
                                       CT.EFF_DTE <= CurrentDate &&
                                       CT.EXP_DTE >= CurrentDate &&
                                       (Mode == null ? a == a : CT.MODE == Mode)
                                 orderby CT.MODE//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                                 select new
                                 {
                                    CT.MODE
                                 }).Distinct();

            //mainSQL="select CT.Mode 'Mode', CT.Repair_CD 'Repair Code', REPAIR_DESC 'Repair Code Description',ROUND(CONTRACT_AMOUNT,2) 'Max material amount pr piece', 
            //(ROUND(CONTRACT_AMOUNT * EXRATUSD * .01,2)) 'Max material amount pr piece converted to USD', convert(char(10),EFF_DTE,120) 'Rate Effective Date',
            //convert(char(10),EXP_DTE,120) 'Rate expiry date' from mesc1ts_country C, mesc1ts_country_cont CT, mesc1ts_currency CR, mesc1ts_repair_code RC 
            //where C.COUNTRY_CD=CT.COUNTRY_CD and CT.CUCDN=CR.CUCDN and CT.MANUAL_CD=RC.MANUAL_CD and CT.MODE=RC.MODE and CT.REPAIR_CD=RC.REPAIR_CD and CT.MANUAL_CD='" & strManual & "' 
            //and C.COUNTRY_CD='" & strCountry & "' and EFF_DTE <= getdate() and EXP_DTE>=getdate() "

            var WOrderMercD05 = (from C in objContext.MESC1TS_COUNTRY
                                 from CT in objContext.MESC1TS_COUNTRY_CONT
                                 from CR in objContext.MESC1TS_CURRENCY
                                 from RC in objContext.MESC1TS_REPAIR_CODE
                                 where C.COUNTRY_CD == CT.COUNTRY_CD &&
                                     CT.CUCDN == CR.CUCDN &&
                                     CT.MANUAL_CD == RC.MANUAL_CD &&
                                     CT.MODE == RC.MODE &&
                                     CT.REPAIR_CD == RC.REPAIR_CD &&
                                     CT.MANUAL_CD == Manual &&
                                     C.COUNTRY_CD == Country &&
                                     CT.EFF_DTE <= CurrentDate &&
                                     CT.EXP_DTE >= CurrentDate &&
                                     CT.MODE == Mode
                                 orderby CT.MODE, CT.REPAIR_CD, CT.EFF_DTE //asc
                                 select new
                                 {
                                     CT.MODE,
                                     CT.REPAIR_CD, //'Repair Code'
                                     RC.REPAIR_DESC, //Repair Code Description
                                     CT.CONTRACT_AMOUNT, //Max material amount pr piece
                                     CT.EFF_DTE, //Rate Effective Date
                                     CT.EXP_DTE, //Rate expiry date
                                     CR.EXRATUSD
                                 });

            if (WOrderMercD05 != null && WOrderMercD05.Count() != 0)
            {
                //lambda exp
                foreach (var obj in WOrderMercD05)
                {
                    WorkOrder WorkOrder = new WorkOrder();
                    //WorkOrder.Mode = obj.MODE;
                    WorkOrder.Mode = obj.MODE;
                    WorkOrder.RepairCode = obj.REPAIR_CD;
                    WorkOrder.RepairCodeDesc = obj.REPAIR_DESC;
                    WorkOrder.MaxMaterialAmountPrPiece = Math.Round(obj.CONTRACT_AMOUNT, 2).ToString();  //'Max material amount pr piece'
                    WorkOrder.MaxMaterialAmountPrPieceConvertedToUSD = (obj.EXRATUSD * obj.CONTRACT_AMOUNT * (int).01).ToString();
                    WorkOrder.RateEffectiveDate = obj.EFF_DTE.ToString();
                    WorkOrder.RateExpiryDate = obj.EXP_DTE.ToString();
                    WorkOrderList.Add(WorkOrder);
                }
            }
            //}
            #endregion MercD05

            return WorkOrderList;
        }
        #endregion GetReportsDetailsMercD05

        #region GetReportsDetailsMercE01
        public List<WorkOrder> GetReportsDetailsMercE01(string DateFrom, string DateTo, string ShopCode, string CustomerCode, string Mode, string Manual, string CountryCode,
                                                        string Days, string AreaCode)//, string MercA01, string MERCB01, string MERCC01,
        //string MERCD03, string MERCD05, string MERCE01)
        {
            objContext = new ManageReportsServiceEntities();
            //Get the List of countries
            List<WorkOrder> WorkOrderList = new List<WorkOrder>();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModeList = new List<MESC1TS_CUST_SHOP_MODE>();
            string shopCode = string.Empty;
            DateTime startDate = Convert.ToDateTime(DateFrom);
            string dateTo = DateTo + " " + "23:59:59";
            DateTime endDate = Convert.ToDateTime(dateTo);
            DateTime CurrentDate = DateTime.Now;
            string a = string.Empty;
            bool isShop = false;
            bool isArea = false;
            bool isCountry = false;
            //DateTime CRTS = DateTime.v + (Days);
            short startStatusCode = 100;
            short endStatusCode = 400;
            string ModeCode = string.Empty;
            //"select distinct mode from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd 
            //and S.shop_cd = '" & strShop & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" and W.status_code >= 100 and W.status_code < 400 
            //and ST.status_code = W.status_code and W.manual_cd ='" & strManual &"' "
            if (isShop)
            {
                var sqlShop = (from W in objContext.MESC1TS_WO
                               from C in objContext.MESC1TS_COUNTRY
                               from L in objContext.MESC1TS_LOCATION
                               from S in objContext.MESC1TS_SHOP
                               from ST in objContext.MESC1TS_STATUS_CODE
                               where L.LOC_CD == S.LOC_CD &&
                                     S.SHOP_CD == W.SHOP_CD &&
                                     S.SHOP_CD == shopCode &&
                                     L.COUNTRY_CD == C.COUNTRY_CD &&
                                     W.CRTS < DateTime.Now  && //Pending
                                     W.STATUS_CODE >= startStatusCode &&
                                     W.STATUS_CODE < endStatusCode &&
                                     ST.STATUS_CODE == W.STATUS_CODE &&
                                     W.MANUAL_CD == Manual &&
                                     (Mode == null ? a == a : W.MODE == Mode)
                               orderby Mode//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                               select new
                               {
                                   W.MODE
                               }).Distinct();

                foreach (var obj in sqlShop)
                {
                    ModeCode = obj.MODE;
                }
                
            }
            else if (isCountry)
            {
                //"select distinct mode from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                //where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd and L.country_cd = '" & strCountry & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" 
                //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual &"' "
                var sqlCountry = (from W in objContext.MESC1TS_WO
                                  from C in objContext.MESC1TS_COUNTRY
                                  from L in objContext.MESC1TS_LOCATION
                                  from S in objContext.MESC1TS_SHOP
                                  from ST in objContext.MESC1TS_STATUS_CODE
                                  where L.LOC_CD == S.LOC_CD &&
                                        S.SHOP_CD == W.SHOP_CD &&
                                        L.COUNTRY_CD == CountryCode &&
                                        L.COUNTRY_CD == C.COUNTRY_CD &&
                                        W.CRTS < DateTime.Now && //Pending
                                        W.STATUS_CODE >= startStatusCode &&
                                        W.STATUS_CODE < endStatusCode &&
                                        ST.STATUS_CODE == W.STATUS_CODE &&
                                        W.MANUAL_CD == Manual &&
                                        (Mode == null ? a == a : W.MODE == Mode)
                                  orderby Mode//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                                  select new
                                  {
                                      W.MODE
                                  }).Distinct();

                foreach (var obj in sqlCountry)
                {
                    ModeCode = obj.MODE;
                }
            }
            else if(isArea)
            {
                //select distinct mode from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                //where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd and C.area_cd ='" & strArea & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" 
                //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual &"' "
                var sqlArea = (from W in objContext.MESC1TS_WO
                                  from C in objContext.MESC1TS_COUNTRY
                                  from L in objContext.MESC1TS_LOCATION
                                  from S in objContext.MESC1TS_SHOP
                                  from ST in objContext.MESC1TS_STATUS_CODE
                                  where L.LOC_CD == S.LOC_CD &&
                                        S.SHOP_CD == W.SHOP_CD &&
                                        C.AREA_CD == AreaCode &&
                                        L.COUNTRY_CD == C.COUNTRY_CD &&
                                        W.CRTS < DateTime.Now && //Pending
                                        W.STATUS_CODE >= startStatusCode &&
                                        W.STATUS_CODE < endStatusCode &&
                                        ST.STATUS_CODE == W.STATUS_CODE &&
                                        W.MANUAL_CD == Manual &&
                                        (Mode == null ? a == a : W.MODE == Mode)
                                  orderby Mode//, year(REPAIR_DTE), month(REPAIR_DTE), day(REPAIR_DTE), EQPNO
                                  select new
                                  {
                                      W.MODE
                                  }).Distinct();

                foreach (var obj in sqlArea)
                {
                    ModeCode = obj.MODE;
                }
            }

            //If isShop then
            //mainSQL="select C.area_cd 'Area',C.country_cd 'Country',W.shop_cd 'Repair shop',W.mode as 'Mode',ST.status_dsc 'Status',w.eqpno 'Equipment No.'
            //,replace(convert(char(10),w.crts,120),'-','/') 'Estimate creation date',datediff(DAY,CRTS,getdate()) 'Days since creation',
            //replace(convert(char(10),W.approval_dte,120),'-','/') 'Estimate approval date',datediff(DAY,APPROVAL_DTE,getdate()) 'Days since approval',
            //replace(convert(char(10),W.chts,120),'-','/') 'Last change to estimate',datediff(DAY,W.CHTS,GetDate()) 'Days since last change',
            //ROUND(W.tot_repair_manh,2)'Estimated total hours',ROUND(W.TOT_COST_REPAIR_CPH,2) 'Estimated total cost of repair in USD(incl. CPH supplied parts)'
            //,W.vendor_ref_no 'Vendor Ref. No' 
            //from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
            //where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd and S.shop_cd = '" & strShop & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" 
            //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual & "' and mode='" & mode & "' 
            //order by mode, year(crts),month(crts),day(crts),eqpno asc"
            
            #region MercEO1
            if (isShop)
            {
                var WOrderMercEO1 = (from W in objContext.MESC1TS_WO
                               from C in objContext.MESC1TS_COUNTRY
                               from L in objContext.MESC1TS_LOCATION
                               from S in objContext.MESC1TS_SHOP
                               from ST in objContext.MESC1TS_STATUS_CODE
                               where L.LOC_CD == S.LOC_CD &&
                                     S.SHOP_CD == W.SHOP_CD &&
                                     S.SHOP_CD == ShopCode &&
                                     L.COUNTRY_CD == C.COUNTRY_CD &&
                                     W.CRTS < DateTime.Now &&
                                     W.STATUS_CODE >= startStatusCode &&
                                     W.STATUS_CODE < endStatusCode &&
                                     ST.STATUS_CODE == W.STATUS_CODE &&
                                     W.MANUAL_CD == Manual &&
                                     W.MODE == ModeCode 
                                     orderby W.MODE//,  year(crts),month(crts),day(crts), EQPNO asc
                               select new
                               {
                                   C.AREA_CD, //Area
                                   C.COUNTRY_CD, //Country
                                   W.SHOP_CD, //Repair shop
                                   W.MODE, //as 'Mode'
                                   ST.STATUS_DSC, //'Status'
                                   W.EQPNO, //'Equipment No.
                                   W.CRTS, //Estimate creation date
                                   W.APPROVAL_DTE,
                                   W.CHTS,
                                   W.TOT_REPAIR_MANH,
                                   W.TOT_COST_REPAIR_CPH,
                                   W.VENDOR_REF_NO //Vendor Ref. No
                               });

                if (WOrderMercEO1 != null && WOrderMercEO1.Count() != 0)
                {
                    //lambda exp
                    foreach (var obj in WOrderMercEO1)
                    {
                        WorkOrder WorkOrder = new WorkOrder();
                        //WorkOrder.Mode = obj.MODE;
                        WorkOrder.Mode = obj.MODE;
                        WorkOrder.AreaCode = obj.AREA_CD;
                        WorkOrder.CountryCode = obj.COUNTRY_CD;
                        WorkOrder.SHOP_CD = obj.SHOP_CD;
                        WorkOrder.Status = obj.STATUS_DSC;
                        WorkOrder.EquipmentNo = obj.EQPNO;
                        WorkOrder.EstimateCreationDate = obj.CRTS.ToString();
                        //WorkOrder.DaysSinceCreation = ;
                        WorkOrder.EstimateApprovalDate = obj.APPROVAL_DTE.ToString();
                        //WorkOrder.DaysSinceApproval = ;
                        WorkOrder.LastChangeToEstimate = obj.CHTS.ToString();
                        //WorkOrder.DaysSinceLastChange = ;
                        WorkOrder.EstimatedTotalHours = Math.Round((decimal)obj.TOT_REPAIR_MANH, 2).ToString();
                        WorkOrder.EstimatedTotalCostOfRepairInUSD = Math.Round((decimal)obj.TOT_COST_REPAIR_CPH, 2).ToString();
                        WorkOrder.VendorRefNo = obj.VENDOR_REF_NO;
                        WorkOrderList.Add(WorkOrder);
                    }
                }
            }
            else if (isCountry)
            {
                //select C.area_cd 'Area',C.country_cd 'Country',W.shop_cd 'Repair shop',W.mode as 'Mode',ST.status_dsc 'Status',w.eqpno 'Equipment No.'
                //,replace(convert(char(10),w.crts,120),'-','/') 'Estimate creation date',datediff(DAY,CRTS,getdate()) 'Days since creation',
                //replace(convert(char(10),W.approval_dte,120),'-','/') 'Estimate approval date',datediff(DAY,APPROVAL_DTE,getdate()) 'Days since approval',
                //replace(convert(char(10),W.chts,120),'-','/') 'Last change to estimate',datediff(DAY,W.CHTS,GetDate()) 'Days since last change',
                //ROUND(W.tot_repair_manh,2)'Estimated total hours',ROUND(W.TOT_COST_REPAIR_CPH,2) 'Estimated total cost of repair in USD(incl. CPH supplied parts)'
                //,W.vendor_ref_no 'Vendor Ref. No' from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                //where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd and L.country_cd = '" & strCountry & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" 
                //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual & "' and mode='" & mode & "' 
                //order by mode, year(crts),month(crts),day(crts),eqpno asc"
                var WOrderMercEO1 = (from W in objContext.MESC1TS_WO
                                     from C in objContext.MESC1TS_COUNTRY
                                     from L in objContext.MESC1TS_LOCATION
                                     from S in objContext.MESC1TS_SHOP
                                     from ST in objContext.MESC1TS_STATUS_CODE
                                     where L.LOC_CD == S.LOC_CD &&
                                           S.SHOP_CD == W.SHOP_CD &&
                                           L.COUNTRY_CD == CountryCode &&
                                           L.COUNTRY_CD == C.COUNTRY_CD &&
                                           W.CRTS < DateTime.Now &&
                                           W.STATUS_CODE >= startStatusCode &&
                                           W.STATUS_CODE < endStatusCode &&
                                           ST.STATUS_CODE == W.STATUS_CODE &&
                                           W.MANUAL_CD == Manual &&
                                           W.MODE == ModeCode
                                     orderby W.MODE//,  year(crts),month(crts),day(crts), EQPNO asc
                                     select new
                                     {
                                         C.AREA_CD, //Area
                                         C.COUNTRY_CD, //Country
                                         W.SHOP_CD, //Repair shop
                                         W.MODE, //as 'Mode'
                                         ST.STATUS_DSC, //'Status'
                                         W.EQPNO, //'Equipment No.
                                         W.CRTS, //Estimate creation date
                                         W.APPROVAL_DTE,
                                         W.CHTS,
                                         W.TOT_REPAIR_MANH,
                                         W.TOT_COST_REPAIR_CPH,
                                         W.VENDOR_REF_NO //Vendor Ref. No
                                     });

                if (WOrderMercEO1 != null && WOrderMercEO1.Count() != 0)
                {
                    //lambda exp
                    foreach (var obj in WOrderMercEO1)
                    {
                        WorkOrder WorkOrder = new WorkOrder();
                        //WorkOrder.Mode = obj.MODE;
                        WorkOrder.Mode = obj.MODE;
                        WorkOrder.AreaCode = obj.AREA_CD;
                        WorkOrder.CountryCode = obj.COUNTRY_CD;
                        WorkOrder.SHOP_CD = obj.SHOP_CD;
                        WorkOrder.Status = obj.STATUS_DSC;
                        WorkOrder.EquipmentNo = obj.EQPNO;
                        WorkOrder.EstimateCreationDate = obj.CRTS.ToString();
                        //WorkOrder.DaysSinceCreation = ;
                        WorkOrder.EstimateApprovalDate = obj.APPROVAL_DTE.ToString();
                        //WorkOrder.DaysSinceApproval = ;
                        WorkOrder.LastChangeToEstimate = obj.CHTS.ToString();
                        //WorkOrder.DaysSinceLastChange = ;
                        WorkOrder.EstimatedTotalHours = Math.Round((decimal)obj.TOT_REPAIR_MANH, 2).ToString();
                        WorkOrder.EstimatedTotalCostOfRepairInUSD = Math.Round((decimal)obj.TOT_COST_REPAIR_CPH, 2).ToString();
                        WorkOrder.VendorRefNo = obj.VENDOR_REF_NO;
                        WorkOrderList.Add(WorkOrder);
                    }
                }
            }
            else if (isArea)
            {
                //select C.area_cd 'Area',C.country_cd 'Country',W.shop_cd 'Repair shop',W.mode as 'Mode',ST.status_dsc 'Status',w.eqpno 'Equipment No.'
                //,replace(convert(char(10),w.crts,120),'-','/') 'Estimate creation date',datediff(DAY,CRTS,getdate()) 'Days since creation',
                //replace(convert(char(10),W.approval_dte,120),'-','/') 'Estimate approval date',datediff(DAY,APPROVAL_DTE,getdate()) 'Days since approval'
                //,replace(convert(char(10),W.chts,120),'-','/') 'Last change to estimate',datediff(DAY,W.CHTS,GetDate()) 'Days since last change',
                //ROUND(W.tot_repair_manh,2)'Estimated total hours',ROUND(W.TOT_COST_REPAIR_CPH,2) 'Estimated total cost of repair in USD(incl. CPH supplied parts)'
                //,W.vendor_ref_no 'Vendor Ref. No' from mesc1ts_wo W, mesc1ts_country C, mesc1ts_LOCATION L, mesc1ts_SHOP S, mesc1ts_status_code ST 
                //where L.Loc_cd = S.loc_cd and S.Shop_cd = W.shop_cd and   C.area_cd = '" & strArea & "' and L.country_cd = C.country_cd and crts < getdate() - "& strDays &" 
                //and W.status_code >= 100 and W.status_code < 400 and ST.status_code = W.status_code and W.manual_cd ='" & strManual & "' and mode='" & mode & "' 
                //order by mode, year(crts),month(crts),day(crts),eqpno asc"
                var WOrderMercEO1 = (from W in objContext.MESC1TS_WO
                                     from C in objContext.MESC1TS_COUNTRY
                                     from L in objContext.MESC1TS_LOCATION
                                     from S in objContext.MESC1TS_SHOP
                                     from ST in objContext.MESC1TS_STATUS_CODE
                                     where L.LOC_CD == S.LOC_CD &&
                                           S.SHOP_CD == W.SHOP_CD &&
                                           C.AREA_CD == AreaCode &&
                                           L.COUNTRY_CD == C.COUNTRY_CD &&
                                           W.CRTS < DateTime.Now &&
                                           W.STATUS_CODE >= startStatusCode &&
                                           W.STATUS_CODE < endStatusCode &&
                                           ST.STATUS_CODE == W.STATUS_CODE &&
                                           W.MANUAL_CD == Manual &&
                                           W.MODE == ModeCode
                                     orderby W.MODE//,  year(crts),month(crts),day(crts), EQPNO asc
                                     select new
                                     {
                                         C.AREA_CD, //Area
                                         C.COUNTRY_CD, //Country
                                         W.SHOP_CD, //Repair shop
                                         W.MODE, //as 'Mode'
                                         ST.STATUS_DSC, //'Status'
                                         W.EQPNO, //'Equipment No.
                                         W.CRTS, //Estimate creation date
                                         //datediff(DAY,CRTS,getdate()) 'Days since creation'
                                         W.APPROVAL_DTE, //replace(convert(char(10),w.crts,120),'-','/') Estimate approval date
                                         //datediff(DAY,APPROVAL_DTE,getdate()) 'Days since approval'
                                         W.CHTS, //Last change to estimate
                                         //datediff(DAY,W.CHTS,GetDate()) 'Days since last change'
                                         W.TOT_REPAIR_MANH, //'Estimated total hours
                                         W.TOT_COST_REPAIR_CPH, //Estimated total cost of repair in USD(incl. CPH supplied parts)
                                         W.VENDOR_REF_NO //Vendor Ref. No
                                     });

                if (WOrderMercEO1 != null && WOrderMercEO1.Count() != 0)
                {
                    //lambda exp
                    foreach (var obj in WOrderMercEO1)
                    {
                        WorkOrder WorkOrder = new WorkOrder();
                        //WorkOrder.Mode = obj.MODE;
                        WorkOrder.Mode = obj.MODE;
                        WorkOrder.AreaCode = obj.AREA_CD;
                        WorkOrder.CountryCode = obj.COUNTRY_CD;
                        WorkOrder.SHOP_CD = obj.SHOP_CD;
                        WorkOrder.Status = obj.STATUS_DSC;
                        WorkOrder.EquipmentNo = obj.EQPNO;
                        WorkOrder.EstimateCreationDate = obj.CRTS.ToString();
                        //WorkOrder.DaysSinceCreation = ;
                        WorkOrder.EstimateApprovalDate = obj.APPROVAL_DTE.ToString();
                        //WorkOrder.DaysSinceApproval = ;
                        WorkOrder.LastChangeToEstimate = obj.CHTS.ToString();
                        //WorkOrder.DaysSinceLastChange = ;
                        WorkOrder.EstimatedTotalHours = Math.Round((decimal)obj.TOT_REPAIR_MANH, 2).ToString();
                        WorkOrder.EstimatedTotalCostOfRepairInUSD = Math.Round((decimal)obj.TOT_COST_REPAIR_CPH, 2).ToString();
                        WorkOrder.VendorRefNo = obj.VENDOR_REF_NO;
                        WorkOrderList.Add(WorkOrder);
                    }
                }
            }

            //}
            #endregion MercE01

            return WorkOrderList;
        }
        #endregion GetReportsDetailsMercE01


        private List<WorkOrder> PrepareDataContract(List<MESC1TS_WO> WorkOrderListFromDB)
        {
            //List<IEnumerable> i = new List<IEnumerable>();
            List<WorkOrder> WorkOrderList = new List<WorkOrder>();
            for (int count = 0; count < WorkOrderListFromDB.Count; count++)
            {
                WorkOrder WorkOrder = new WorkOrder();
                WorkOrder.Mode = WorkOrderListFromDB[count].MODE;
                WorkOrder.ActualCompletionDate = WorkOrderListFromDB[count].REPAIR_DTE.ToString();
                WorkOrder.EquipmentNo = WorkOrderListFromDB[count].EQPNO;
                WorkOrder.WorkOrderNo = WorkOrderListFromDB[count].WO_ID.ToString();
                WorkOrder.VendorRefNo = WorkOrderListFromDB[count].VENDOR_REF_NO;
                //WorkOrder.AgentCode = WorkOrderListFromDB[count]
                WorkOrder.VoucherNumber = WorkOrderListFromDB[count].VOUCHER_NO;
                WorkOrder.OrdinaryManHours = WorkOrderListFromDB[count].TOT_MANH_REG.ToString();
                WorkOrder.Overtime1ManHours = WorkOrderListFromDB[count].TOT_MANH_OT.ToString();
                WorkOrder.Overtime2ManHours = WorkOrderListFromDB[count].TOT_MANH_DT.ToString();
                WorkOrder.Overtime3ManHours = WorkOrderListFromDB[count].TOT_MANH_MISC.ToString();
                WorkOrder.TotalHours = WorkOrderListFromDB[count].TOT_REPAIR_MANH.ToString();
                WorkOrder.TotalLabourCostCPH = WorkOrderListFromDB[count].TOT_LABOR_COST_CPH.ToString();
                WorkOrder.TotalCostOfShopSuppliedNumberedParts = WorkOrderListFromDB[count].TOT_MAN_PARTS_CPH.ToString();
                WorkOrder.TotalCostOfShopSuppliedMaterials = WorkOrderListFromDB[count].TOT_SHOP_AMT_CPH.ToString();
                WorkOrder.ImportTax = WorkOrderListFromDB[count].IMPORT_TAX_CPH.ToString();
                WorkOrder.SalesTaxParts = WorkOrderListFromDB[count].SALES_TAX_PARTS_CPH.ToString();
                WorkOrder.SalesTaxLabour = WorkOrderListFromDB[count].SALES_TAX_LABOR_CPH.ToString();
                WorkOrder.TotalToBePaidToShop = WorkOrderListFromDB[count].TOT_COST_CPH.ToString();
                WorkOrder.TotalCostOfCPHSuppliedParts = WorkOrderListFromDB[count].TOT_MAERSK_PARTS_CPH.ToString();
                WorkOrder.TotalCostOfRepairCPH = WorkOrderListFromDB[count].TOT_COST_REPAIR_CPH.ToString();

            }
            return WorkOrderList;
        }

        #region GetCountryList
        public List<Country> GetCountryList()
        {

            //LogEntry logEntry = new LogEntry();
            //logEntry.Message = "Hello World once more again";
            //Logger.Write(logEntry);

            //"SELECT C.COUNTRY_CD, C.COUNTRY_CD + ' - ' + C.COUNTRY_DESC AS DESCRIPTION"
            //strSQL = strSQL & " FROM MESC1TS_COUNTRY C INNER JOIN SEC_AUTHGROUP_USER U"
            //strSQL = strSQL & " ON U.COLUMN_VALUE = C.AREA_CD"
            //strSQL = strSQL & " INNER JOIN SEC_AUTHGROUP A"
            //strSQL = strSQL & " ON A.AUTHGROUP_ID = U.AUTHGROUP_ID"
            //strSQL = strSQL & " WHERE USER_ID = " & sUserID
            //strSQL = strSQL & " ORDER BY C.COUNTRY_CD"

            //Get the List of countries
            List<MESC1TS_COUNTRY> CountryListFromDB = new List<MESC1TS_COUNTRY>();
            List<Country> CountryList = new List<Country>();

            //CountryListFromDB = (from C in objContext.MESC1TS_COUNTRY
            //                     join U in objContext.SEC_AUTHGROUP_USER on C.AREA_CD equals U.COLUMN_VALUE
            //                     join A in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals A.AUTHGROUP_ID
            //                     where U.USER_ID == UserID
            //                     orderby C.COUNTRY_CD
            //                     select new { C.COUNTRY_CD, C.COUNTRY_CD }).ToList();

            string esqlQueryCountry = @"SELECT VALUE country
                    FROM ManageReportsServiceEntities.MESC1TS_COUNTRY as country";


            ObjectQuery<MESC1TS_COUNTRY> queryCountry = new ObjectQuery<MESC1TS_COUNTRY>(esqlQueryCountry, objContext);
            CountryListFromDB = queryCountry.ToList();

            for (int count = 0; count < CountryListFromDB.Count; count++)
            {
                Country Country = new Country();
                Country.CountryCode = CountryListFromDB[count].COUNTRY_CD;
                Country.CountryDescription = CountryListFromDB[count].COUNTRY_DESC;
                CountryList.Add(Country);
            }

            return CountryList;
        }
        #endregion GetCountryList

        #region GetShopList
        public List<Shop> GetShopList()
        {
            #region Get and Prepare Shop List
            //Get the List of countries
            List<MESC1TS_SHOP> ShopListFromDB = new List<MESC1TS_SHOP>();
            List<Shop> ShopList = new List<Shop>();

            ShopListFromDB = (from shop in objContext.MESC1TS_SHOP select shop).ToList();
//            string esqlQueryShop = @"SELECT VALUE shop
//                    FROM ManageReportsServiceEntities.MESC1TS_SHOP as shop";


//            ObjectQuery<MESC1TS_SHOP> queryShop = new ObjectQuery<MESC1TS_SHOP>(esqlQueryShop, objContext);
//            ShopListFromDB = queryShop.ToList();
            #endregion Get and Prepare Shop List

            for (int count = 0; count < ShopListFromDB.Count; count++)
            {
                Shop Shop = new Shop();
                Shop.ShopCode = ShopListFromDB[count].SHOP_CD;
                Shop.ShopDescription = ShopListFromDB[count].SHOP_DESC;
                ShopList.Add(Shop);
            }
            return ShopList;
        }
        #endregion GetShopList

        #region GetModeList
        public List<Mode> GetModeList()
        {
            #region Get and Prepare Mode List
            //Get the List of countries
            List<MESC1TS_MODE> ModeListFromDB = new List<MESC1TS_MODE>();
            List<Mode> ModeList = new List<Mode>();
    //        "SELECT DISTINCT MESC1TS_MODE.MODE, MESC1TS_MODE.MODE + ' - ' + MODE_DESC AS DESCRIPTION"
    //strTables = " FROM MESC1TS_MODE, MESC1TS_MANUAL_MODE, MESC1TS_MANUAL, MESC1TS_CUSTOMER, MESC1VS_CUST_SHOP, MESC1TS_SHOP "
    //strWhere = " WHERE (MESC1TS_MODE.MODE = MESC1TS_MANUAL_MODE.MODE)" _ 
    //    & " AND (MESC1TS_MANUAL_MODE.MANUAL_CD = MESC1TS_MANUAL.MANUAL_CD)" _ 
    //    & " AND (MESC1TS_MANUAL.MANUAL_CD = MESC1TS_CUSTOMER.MANUAL_CD)" _ 
    //    & " AND (MESC1TS_CUSTOMER.CUSTOMER_CD = MESC1VS_CUST_SHOP.CUSTOMER_CD)" _ 
    //    & " AND (MESC1VS_CUST_SHOP.SHOP_CD = MESC1TS_SHOP.SHOP_CD)"

            //(CustomerCode == null ? a == a : WO.CUSTOMER_CD == CustomerCode) &&
            //                            (Manual == null ? a == a : WO.MANUAL_CD == Manual) &&
            //                            (Mode == null ? a == a : WO.MODE == Mode)

            var ModeListFrom = (from m in objContext.MESC1TS_MODE
                                  from mm in objContext.MESC1TS_MANUAL_MODE
                                  from man in objContext.MESC1TS_MANUAL
                                  from cus in objContext.MESC1TS_CUSTOMER
                                  from cs in objContext.MESC1VS_CUST_SHOP
                                  from s in objContext.MESC1TS_SHOP
                                  where m.MODE == mm.MODE &&
                                        mm.MANUAL_CD == man.MANUAL_CD &&
                                        man.MANUAL_CD == cus.MANUAL_CD &&
                                        cus.CUSTOMER_CD == cs.CUSTOMER_CD &&
                                        cs.SHOP_CD == s.SHOP_CD 
                                        orderby m.MODE
                                  select new
                                  {
                                      m.MODE,
                                      m.MODE_DESC
                                  }).Distinct();

            string esqlQueryMode = @"SELECT VALUE mode
                    FROM ManageReportsServiceEntities.MESC1TS_MODE as mode";


            ObjectQuery<MESC1TS_MODE> queryMode = new ObjectQuery<MESC1TS_MODE>(esqlQueryMode, objContext);
            ModeListFromDB = queryMode.ToList();
            #endregion Get and Prepare Mode List

            for (int count = 0; count < ModeListFromDB.Count; count++)
            {
                Mode Mode = new Mode();
                Mode.ModeCode = ModeListFromDB[count].MODE;
                Mode.ModeDescription = ModeListFromDB[count].MODE_DESC;
                ModeList.Add(Mode);
            }
            return ModeList;
        }
        #endregion GetModeList

        #region GetManualList
        public List<Manual> GetManualList()
        {
            #region Get and Prepare Manual List
            //Get the List of countries
            List<MESC1TS_MANUAL> ManualListFromDB = new List<MESC1TS_MANUAL>();
            List<Manual> ManualList = new List<Manual>();

            ManualListFromDB = (from m in objContext.MESC1TS_MANUAL
                                  where m.MANUAL_ACTIVE_SW == "Y"
                                  orderby m.MANUAL_CD
                                  select m).ToList();
//            string esqlQueryManual = @"SELECT VALUE manual
//                    FROM ManageReportsServiceEntities.MESC1TS_MANUAL as manual";


//            ObjectQuery<MESC1TS_MANUAL> queryManual = new ObjectQuery<MESC1TS_MANUAL>(esqlQueryManual, objContext);
//            ManualListFromDB = queryManual.ToList();
            #endregion Get and Prepare Manual List

            for (int count = 0; count < ManualListFromDB.Count; count++)
            {
                Manual Manual = new Manual();
                Manual.ManualCode = ManualListFromDB[count].MANUAL_CD;
                Manual.ManualDescription = ManualListFromDB[count].MANUAL_DESC;
                ManualList.Add(Manual);
            }
            return ManualList;
        }
        #endregion GetManualList

        #region GetCustomerList
        public List<Customer> GetCustomerList()
        {
            #region Get and Prepare Customer List
            //Get the List of countries
            List<MESC1TS_CUSTOMER> CustomerListFromDB = new List<MESC1TS_CUSTOMER>();
            List<Customer> CustomerList = new List<Customer>();

            CustomerListFromDB = (from customer in objContext.MESC1TS_CUSTOMER
                                  where customer.CUSTOMER_ACTIVE_SW == "Y"
                                  orderby customer.CUSTOMER_CD
                                  select customer).ToList();
//            string esqlQueryCustomer = @"SELECT VALUE customer
//                    FROM ManageReportsServiceEntities.MESC1TS_CUSTOMER as customer";


//            ObjectQuery<MESC1TS_CUSTOMER> queryCustomer = new ObjectQuery<MESC1TS_CUSTOMER>(esqlQueryCustomer, objContext);
//            CustomerListFromDB = queryCustomer.ToList();
            #endregion Get and Prepare Customer List

            for (int count = 0; count < CustomerListFromDB.Count; count++)
            {
                Customer Customer = new Customer();
                Customer.CustomerCode = CustomerListFromDB[count].CUSTOMER_CD;
                Customer.CustomerDescription = CustomerListFromDB[count].CUSTOMER_DESC;
                CustomerList.Add(Customer);
            }
            return CustomerList;
        }
        #endregion GetCustomerList

        #region GetAreaList
        public List<Area> GetAreaList()
        {
            #region Get and Prepare GetAreaList List
    //        strSQL = "SELECT DISTINCT A.AREA_CD,A.AREA_CD + ' - ' + A.AREA_DESC AS AREA_DESC "
    //strSQL = strSQL & "FROM MESC1TS_AREA A "
    //strSQL = strSQL & "INNER JOIN MESC1TS_COUNTRY C "
    //strSQL = strSQL & "ON C.AREA_CD = A.AREA_CD "
    //strSQL = strSQL & "INNER JOIN MESC1TS_LOCATION L "
    //strSQL = strSQL & "ON L.COUNTRY_CD = C.COUNTRY_CD "
    //strSQL = strSQL & "INNER JOIN SEC_AUTHGROUP_USER U "
    //strSQL = strSQL & "ON U.COLUMN_VALUE = A.AREA_CD "
    //'strSQL = strSQL & "ON U.COLUMN_VALUE = L.LOC_CD "
    //strSQL = strSQL & "INNER JOIN SEC_AUTHGROUP SA "
    //strSQL = strSQL & "ON SA.AUTHGROUP_ID = U.AUTHGROUP_ID "
    //strSQL = strSQL & "WHERE USER_ID = " & sUserID
    //strSQL = strSQL & " ORDER BY A.AREA_CD"
            //Get the List of countries
            List<MESC1TS_AREA> AreaListFromDB = new List<MESC1TS_AREA>();
            List<Area> AreaList = new List<Area>();

            //var area = (from A in objContext.MESC1TS_AREA
            //            join C in objContext.MESC1TS_COUNTRY on A.AREA_CD equals C.AREA_CD
            //            join L in objContext.MESC1TS_LOCATION on C.COUNTRY_CD equals L.COUNTRY_CD
            //            join U in objContext.SEC_AUTHGROUP_USER on A.AREA_CD equals U.COLUMN_VALUE
            //            join SA in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals SA.AUTHGROUP_ID
            //            where U.USER_ID == UserID
            //            orderby A.AREA_CD
            //            select new { A.AREA_CD, A.AREA_DESC }).ToList();

            string esqlQueryArea = @"SELECT VALUE area
                    FROM ManageReportsServiceEntities.MESC1TS_AREA as area";


            ObjectQuery<MESC1TS_AREA> queryArea = new ObjectQuery<MESC1TS_AREA>(esqlQueryArea, objContext);
            AreaListFromDB = queryArea.ToList();
            #endregion Get and Prepare GetAreaList List

            for (int count = 0; count < AreaListFromDB.Count; count++)
            {
                Area Area = new Area();
                Area.AreaCode = AreaListFromDB[count].AREA_CD;
                Area.AreaDescription = AreaListFromDB[count].AREA_DESC;
                AreaList.Add(Area);
            }
            return AreaList;
        }
        #endregion GetAreaList

        #region GetShopListOnCountryCode
        public List<Shop> GetShopListOnCountryCode(string CountryCode)
        {
            #region Get and Prepare Shop List
            //Get the List of countries
            List<MESC1TS_SHOP> ShopListFromDB = new List<MESC1TS_SHOP>();
            List<Shop> ShopList = new List<Shop>();

            ShopListFromDB = (from s in objContext.MESC1TS_SHOP
                              from l in objContext.MESC1TS_LOCATION
                              where s.LOC_CD == l.LOC_CD &&
                                    l.COUNTRY_CD == CountryCode
                              select s).ToList();
            
            #endregion Get and Prepare Shop List

            for (int count = 0; count < ShopListFromDB.Count; count++)
            {
                Shop Shop = new Shop();
                Shop.ShopCode = ShopListFromDB[count].SHOP_CD;
                Shop.ShopDescription = ShopListFromDB[count].SHOP_DESC;
                ShopList.Add(Shop);
            }
            return ShopList;
        }
        #endregion GetShopListOnCountryCode

        #region GetShopListOnCountryCode
        public List<Shop> GetModeListOnConditions(string ShopCode,string CustomerCode, string ManualCode)
        {
            #region Get and Prepare Shop List
            //Get the List of countries
            List<MESC1TS_SHOP> ShopListFromDB = new List<MESC1TS_SHOP>();
            List<Shop> ShopList = new List<Shop>();
            string a = string.Empty;
            //(Manual == null ? a == a : WO.MANUAL_CD == Manual) &&
            var ModeListFrom = (from m in objContext.MESC1TS_MODE
                                from mm in objContext.MESC1TS_MANUAL_MODE
                                from man in objContext.MESC1TS_MANUAL
                                from cus in objContext.MESC1TS_CUSTOMER
                                from cs in objContext.MESC1VS_CUST_SHOP
                                from s in objContext.MESC1TS_SHOP
                                where m.MODE == mm.MODE &&
                                      mm.MANUAL_CD == man.MANUAL_CD &&
                                      man.MANUAL_CD == cus.MANUAL_CD &&
                                      cus.CUSTOMER_CD == cs.CUSTOMER_CD &&
                                      cs.SHOP_CD == s.SHOP_CD &&
                                      (ShopCode == null ? a==a : s.SHOP_CD == ShopCode) &&
                                      (CustomerCode == null ? a==a : cus.CUSTOMER_CD == CustomerCode) &&
                                      (ManualCode == null ? a == a : man.MANUAL_CD == ManualCode)
                                      orderby m.MODE
                                select new
                                {
                                    m.MODE,
                                    m.MODE_DESC
                                }).Distinct();

            #endregion Get and Prepare Shop List

            for (int count = 0; count < ShopListFromDB.Count; count++)
            {
                Shop Shop = new Shop();
                Shop.ShopCode = ShopListFromDB[count].SHOP_CD;
                Shop.ShopDescription = ShopListFromDB[count].SHOP_DESC;
                ShopList.Add(Shop);
            }
            return ShopList;
        }
        #endregion GetShopListOnCountryCode

        #region IManageReports Members

        public List<ReportsDetails> GetReportsDetails()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
