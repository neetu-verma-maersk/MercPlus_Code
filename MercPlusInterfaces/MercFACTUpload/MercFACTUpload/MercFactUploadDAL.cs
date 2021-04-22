using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using IBM.WMQ;
using MercFACTUpload;
using Microsoft.Practices.EnterpriseLibrary.Logging;




namespace MercFactUpload
{
    class MercFactUploadDAL
    {
        public static LogEntry logEntry = new LogEntry();
        MercfactUploadEntities objContext = new MercfactUploadEntities();
        //SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
        public List<MercFactUploadEntity> GetEligibleWorkOrder()
        {


            List<MercFactUploadEntity> WOList = new List<MercFactUploadEntity>();

            try
            {
                var eligibleData = (from W in objContext.MESC1TS_WO
                                    from T in objContext.MESC1TS_TRANSMIT
                                    .Where(m => m.CUSTOMER_CD == W.CUSTOMER_CD && m.MODE == W.MODE)
                                    .DefaultIfEmpty()
                                    from SH in objContext.MESC1TS_SHOP
                                    .Where(s => s.SHOP_CD == W.SHOP_CD)
                                    .DefaultIfEmpty()
                                    from C in objContext.MESC1TS_CUST_SHOP_MODE
                                    .Where(n => n.CUSTOMER_CD == W.CUSTOMER_CD && n.MODE == W.MODE && n.SHOP_CD == W.SHOP_CD)
                                    .DefaultIfEmpty()
                                    from PA in objContext.MESC1TS_PAYAGENT
                                    .Where(o => o.PAYAGENT_CD == C.PAYAGENT_CD)
                                    .DefaultIfEmpty()
                                    from PV in objContext.MESC1TS_PAYAGENT_VENDOR
                                    .Where(p => p.PAYAGENT_CD == C.PAYAGENT_CD && p.VENDOR_CD == SH.VENDOR_CD)
                                    .DefaultIfEmpty()
                                    where (W.STATUS_CODE == 400 || W.STATUS_CODE == 550) //&& W.WO_ID == 11142421
                                   // where W.STATUS_CODE == 400 && W.WO_ID == 53336686
                                    select new
                                    {
                                        W.WO_ID,
                                        W.CUSTOMER_CD,
                                        W.MODE,
                                        W.SHOP_CD,
                                        W.REPAIR_DTE,
                                        W.EXCHANGE_RATE,
                                        W.TOT_SHOP_AMT,
                                        W.TOT_COST_LOCAL,
                                        W.VENDOR_REF_NO,
                                        W.VOUCHER_NO,
                                        W.APPROVAL_DTE,
                                        W.SALES_TAX_LABOR,
                                        W.SALES_TAX_PARTS,
                                        W.SALES_TAX_LABOR_PCT,
                                        W.TOT_LABOR_COST,
                                        W.TOT_MAN_PARTS,
                                        W.IMPORT_TAX_CPH,
                                        W.IMPORT_TAX,
                                        W.EQPNO,
                                        W.CUCDN,
                                        W.CAUSE,
                                        W.MANH_RATE,
                                        W.MANH_RATE_CPH,
                                        W.OT_RATE,
                                        W.OT_RATE_CPH,
                                        W.DT_RATE,
                                        W.DT_RATE_CPH,
                                        W.MISC_RATE,
                                        W.MISC_RATE_CPH,
                                        W.TOT_MANH_OT,
                                        W.TOT_MANH_DT,
                                        W.TOT_MANH_MISC,
                                        W.COUNTRY_CUCDN,
                                        W.COUNTRY_EXCHANGE_RATE,
                                        W.COUNTRY_EXCHANGE_DTE,
                                        XACCOUNT_CD = T.ACCOUNT_CD,
                                        XRRIS_XMIT_SW = T.RRIS_XMIT_SW,
                                        CSM_PAYAGENT_CD = C.PAYAGENT_CD,
                                        CSM_CORP_PAYAGENT_CD = C.CORP_PAYAGENT_CD,
                                        CSM_RRIS_FORMAT = C.RRIS_FORMAT,
                                        CSM_PROFIT_CENTER = C.PROFIT_CENTER,
                                        CSM_SUB_PROFIT_CENTER = C.SUB_PROFIT_CENTER,
                                        CSM_ACCOUNT_CD = C.ACCOUNT_CD,
                                        PA_PAYAGENT_CD = PA.PAYAGENT_CD,
                                        PA_CORP_PAYAGENT_CD = PA.CORP_PAYAGENT_CD,
                                        PA_RRIS_FORMAT = PA.RRIS_FORMAT,
                                        PA_PROFIT_CENTER = PA.PROFIT_CENTER,
                                        PA_SUB_PROFIT_CENTER = PA.SUB_PROFIT_CENTER,
                                        SH_SHOP_TYPE_CD = SH.SHOP_TYPE_CD,
                                        SH_VENDOR_CD = SH.VENDOR_CD,
                                        SH_SHOP_DESC = SH.SHOP_DESC.ToUpper(),
                                        SH_RKRPLOC = SH.RKRPLOC,
                                        SH.DECENTRALIZED,
                                        SH.LOC_CD,
                                        SH.PCT_MATERIAL_FACTOR,
                                        SH_RRIS70_SUFFIX_CD = SH.RRIS70_SUFFIX_CD,
                                        SH_RRIS_XMIT_SW = SH.RRIS_XMIT_SW,
                                        SH_IMPORT_TAX_PCT = SH.IMPORT_TAX,
                                        PV.SUPPLIER_CD,
                                        PV.LOCAL_ACCOUNT_CD
                                    }).ToList();


                foreach (var obj in eligibleData)
                {
                    MercFactUploadEntity objEntity = new MercFactUploadEntity();
                    objEntity.WO_ID = obj.WO_ID.ToString();
                    objEntity.CUSTOMER_CD = obj.CUSTOMER_CD == null ? "" : obj.CUSTOMER_CD.ToString();
                    objEntity.MODE = obj.MODE == null ? "" : obj.MODE.ToString();
                    objEntity.SHOP_CD = obj.SHOP_CD == null ? "" : obj.SHOP_CD.ToString();
                    objEntity.REPAIR_DTE = obj.REPAIR_DTE == null ? "" : obj.REPAIR_DTE.ToString();
                    objEntity.EXCHANGE_RATE = obj.EXCHANGE_RATE == null ? 0 : (double)obj.EXCHANGE_RATE;
                    objEntity.TOT_SHOP_AMT = obj.TOT_SHOP_AMT == null ? 0: (double)obj.TOT_SHOP_AMT;
                    objEntity.TOT_COST_LOCAL = obj.TOT_COST_LOCAL == null ? 0 : (double)obj.TOT_COST_LOCAL;
                    objEntity.VENDOR_REF_NO = obj.VENDOR_REF_NO == null ? "" : obj.VENDOR_REF_NO.ToString();
                    objEntity.VOUCHER_NO = obj.VOUCHER_NO == null ? "" : obj.VOUCHER_NO.ToString();
                    objEntity.APPROVAL_DTE = obj.APPROVAL_DTE == null ? "" : obj.APPROVAL_DTE.ToString();
                    objEntity.SALES_TAX_LABOR = obj.SALES_TAX_LABOR == null ? 0 : (double)obj.SALES_TAX_LABOR;
                    objEntity.SALES_TAX_PARTS = obj.SALES_TAX_PARTS == null ? 0 : (double)obj.SALES_TAX_PARTS;
                    objEntity.SALES_TAX_LABOR_PCT = obj.SALES_TAX_LABOR_PCT == null ? "" : obj.SALES_TAX_LABOR_PCT.ToString();
                    objEntity.TOT_LABOR_COST = obj.TOT_LABOR_COST == null ? 0 : (double)obj.TOT_LABOR_COST;
                    objEntity.TOT_MAN_PARTS = obj.TOT_MAN_PARTS == null ? 0 : (double)obj.TOT_MAN_PARTS;
                    objEntity.IMPORT_TAX_CPH = obj.IMPORT_TAX_CPH == null ? 0 : (double)obj.IMPORT_TAX_CPH;
                    objEntity.IMPORT_TAX = obj.IMPORT_TAX == null ? 0 : (double)obj.IMPORT_TAX;
                    objEntity.EQPNO = obj.EQPNO == null ? "" : obj.EQPNO.ToString();
                    objEntity.CUCDN = obj.CUCDN == null ? "" : obj.CUCDN.ToString();
                    objEntity.CAUSE = obj.CAUSE == null ? "" : obj.CAUSE.ToString();
                    objEntity.MANH_RATE = obj.MANH_RATE == null ? 0 : (double)obj.MANH_RATE;
                    objEntity.MANH_RATE_CPH = obj.MANH_RATE_CPH == null ? 0 : (double)obj.MANH_RATE_CPH;
                    objEntity.OT_RATE = obj.OT_RATE == null ? 0 : (double)obj.OT_RATE;
                    objEntity.OT_RATE_CPH = obj.OT_RATE_CPH == null ? 0 : (double)obj.OT_RATE_CPH;
                    objEntity.DT_RATE = obj.DT_RATE == null ? 0 : (double)obj.DT_RATE;
                    objEntity.DT_RATE_CPH = obj.DT_RATE_CPH == null ? 0 : (double)obj.DT_RATE_CPH;
                    objEntity.MISC_RATE = obj.MISC_RATE == null ? 0 : (double)obj.MISC_RATE;
                    objEntity.MISC_RATE_CPH = obj.MISC_RATE_CPH == null ? 0 : (double)obj.MISC_RATE_CPH;
                    objEntity.TOT_MANH_OT = obj.TOT_MANH_OT == null ? 0 : (double)obj.TOT_MANH_OT;
                    objEntity.TOT_MANH_DT = obj.TOT_MANH_DT == null ? 0 : (double)obj.TOT_MANH_DT;
                    objEntity.TOT_MANH_MISC = obj.TOT_MANH_MISC == null ? 0 : (double)obj.TOT_MANH_MISC;
                    objEntity.COUNTRY_CUCDN = obj.COUNTRY_CUCDN == null ? "" : obj.COUNTRY_CUCDN.ToString();
                    objEntity.COUNTRY_EXCHANGE_RATE = obj.COUNTRY_EXCHANGE_RATE == null ? 0 : (double)obj.COUNTRY_EXCHANGE_RATE;
                    objEntity.COUNTRY_EXCHANGE_RATE *= 0.01;
                    objEntity.COUNTRY_EXCHANGE_DTE = obj.COUNTRY_EXCHANGE_DTE == null ? "" : obj.COUNTRY_EXCHANGE_DTE.ToString();
                    objEntity.XACCOUNT_CD = obj.XACCOUNT_CD == null ? "" : obj.XACCOUNT_CD.ToString();
                    objEntity.XRRIS_XMIT_SW = obj.XRRIS_XMIT_SW == null ? "" : obj.XRRIS_XMIT_SW.ToString();
                    objEntity.CSM_PAYAGENT_CD = obj.CSM_PAYAGENT_CD == null ? "" : obj.CSM_PAYAGENT_CD.ToString();
                    objEntity.CSM_CORP_PAYAGENT_CD = obj.CSM_CORP_PAYAGENT_CD == null ? "" : obj.CSM_CORP_PAYAGENT_CD.ToString();
                    objEntity.CSM_RRIS_FORMAT = obj.CSM_RRIS_FORMAT == null ? "" : obj.CSM_RRIS_FORMAT.ToString();
                    objEntity.CSM_PROFIT_CENTER = obj.CSM_PROFIT_CENTER == null ? "" : obj.CSM_PROFIT_CENTER.ToString();
                    objEntity.CSM_SUB_PROFIT_CENTER = obj.CSM_SUB_PROFIT_CENTER == null ? "" : obj.CSM_SUB_PROFIT_CENTER.ToString();
                    objEntity.CSM_ACCOUNT_CD = obj.CSM_ACCOUNT_CD == null ? "" : obj.CSM_ACCOUNT_CD.ToString();
                    objEntity.PA_PAYAGENT_CD = obj.PA_PAYAGENT_CD == null ? "" : obj.PA_PAYAGENT_CD.ToString();
                    objEntity.PA_CORP_PAYAGENT_CD = obj.PA_CORP_PAYAGENT_CD == null ? "" : obj.PA_CORP_PAYAGENT_CD.ToString();
                    objEntity.PA_RRIS_FORMAT = obj.PA_RRIS_FORMAT == null ? "" : obj.PA_RRIS_FORMAT.ToString();
                    objEntity.PA_PROFIT_CENTER = obj.PA_PROFIT_CENTER == null ? "" : obj.PA_PROFIT_CENTER.ToString();
                    objEntity.PA_SUB_PROFIT_CENTER = obj.PA_SUB_PROFIT_CENTER == null ? "" : obj.PA_SUB_PROFIT_CENTER.ToString();
                    objEntity.SH_SHOP_TYPE_CD = obj.SH_SHOP_TYPE_CD == null ? "" : obj.SH_SHOP_TYPE_CD.ToString();
                    objEntity.SH_VENDOR_CD = obj.SH_VENDOR_CD == null ? "" : obj.SH_VENDOR_CD.ToString();
                    objEntity.SH_SHOP_DESC = obj.SH_SHOP_DESC == null ? "" : obj.SH_SHOP_DESC.ToString();
                    objEntity.SH_RKRPLOC = obj.SH_RKRPLOC == null ? "" : obj.SH_RKRPLOC.ToString();
                    objEntity.DECENTRALIZED = obj.DECENTRALIZED == null ? "" : obj.DECENTRALIZED.ToString();
                    objEntity.LOC_CD = obj.LOC_CD == null ? "" : obj.LOC_CD.ToString();
                    objEntity.PCT_MATERIAL_FACTOR = obj.PCT_MATERIAL_FACTOR == null ? 0 : (double)obj.PCT_MATERIAL_FACTOR;
                    objEntity.SH_RRIS70_SUFFIX_CD = obj.SH_RRIS70_SUFFIX_CD == null ? "" : obj.SH_RRIS70_SUFFIX_CD.ToString();
                    objEntity.SH_RRIS_XMIT_SW = obj.SH_RRIS_XMIT_SW == null ? "" : obj.SH_RRIS_XMIT_SW.ToString();
                    objEntity.SH_IMPORT_TAX_PCT = obj.SH_IMPORT_TAX_PCT == null ? 0 : (double)obj.SH_IMPORT_TAX_PCT;
                    objEntity.SUPPLIER_CD = obj.SUPPLIER_CD == null ? "" : obj.SUPPLIER_CD.ToString();
                    objEntity.LOCAL_ACCOUNT_CD = obj.LOCAL_ACCOUNT_CD == null ? "" : obj.LOCAL_ACCOUNT_CD.ToString();
                    WOList.Add(objEntity);

                }

            }
            catch (SqlException ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }

            return WOList;
        }

        public string GetLastVoucherNo()
        {

            string VoucherNo = "";
            try
            {

                var eligiblVendor = (from MPI in objContext.MESC1TS_PROCESS_IDENTIFIER
                                     where MPI.PROCESS_DESC == "RRIS_XMIT" && MPI.PARAM_NAME == "LAST_VOUCHER_NO"
                                     select new
                                     {
                                         VOUCHER_NO = MPI.PARAM_VALUE

                                     }).ToList();

                foreach (var VenNo in eligiblVendor)
                {
                    VoucherNo = VenNo.VOUCHER_NO;

                }

            }
            catch (SqlException ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return VoucherNo;

        }

        public List<RepairEntities> GetRepairs(string WO_ID)
        {

            List<RepairEntities> RepairList = new List<RepairEntities>();

            try
            {
                int WOID = Convert.ToInt32(WO_ID);
                var RepairData = (from WR in objContext.MESC1TS_WOREPAIR
                                  from RC in objContext.MESC1TS_REPAIR_CODE
                                  .Where(m => m.MANUAL_CD == WR.MANUAL_CD && m.MODE == WR.MODE && m.REPAIR_CD == WR.REPAIR_CD)
                                  .DefaultIfEmpty()
                                  where WR.WO_ID == WOID
                                  select new
                                  {
                                      WR.REPAIR_CD,
                                      WR.QTY_REPAIRS,
                                      WR.SHOP_MATERIAL_AMT,
                                      WR.CPH_MATERIAL_AMT,
                                      WR.ACTUAL_MANH,
                                      RC.REPAIR_DESC,
                                      RC.TAX_APPLIED_SW
                                      ,WR.REPAIR_LOC_CD//Kasturee XML-12-06-18

                                  }).ToList();


                foreach (var objRepCode in RepairData)
                {
                    RepairEntities objEntity = new RepairEntities();
                    //obj.PA_PROFIT_CENTER == null ? "" : obj.PA_PROFIT_CENTER.ToString();
                    //objEntity.SH_SHOP_DESC = obj.SH_SHOP_DESC == null ? "" : obj.SH_SHOP_DESC.ToString();
                    objEntity.REPAIR_CD = objRepCode.REPAIR_CD == null ? "" : objRepCode.REPAIR_CD.ToString();
                    objEntity.QTY_REPAIRS = objRepCode.QTY_REPAIRS == null ? 0 : (long)objRepCode.QTY_REPAIRS;
                    objEntity.SHOP_MATERIAL_AMT = objRepCode.SHOP_MATERIAL_AMT == null ? 0 : (double)objRepCode.SHOP_MATERIAL_AMT;
                    objEntity.CPH_MATERIAL_AMT = objRepCode.CPH_MATERIAL_AMT == null ? 0 : (double)objRepCode.CPH_MATERIAL_AMT;
                    objEntity.ACTUAL_MANH = objRepCode.ACTUAL_MANH == null ? 0 : (double)objRepCode.ACTUAL_MANH;
                    objEntity.REPAIR_DESC = objRepCode.REPAIR_DESC == null ? "" : objRepCode.REPAIR_DESC.ToString();
                    objEntity.TAX_APPLIED_SW = objRepCode.TAX_APPLIED_SW == null ? "" : objRepCode.TAX_APPLIED_SW.ToString();
                    objEntity.REPAIR_LOC_CD = objRepCode.REPAIR_LOC_CD == null ? "" : objRepCode.REPAIR_LOC_CD.ToString();//Kasturee XML-12-06-18

                    RepairList.Add(objEntity);

                }

            }
            catch (SqlException ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return RepairList;
        }

        public List<CurrencyEntities> GetCountryCurrency(string WO_ID, string SHOP_CD, string MODE)
        {


            List<CurrencyEntities> CurrencyList = new List<CurrencyEntities>();

            try
            {
                var repCode = (from rd in objContext.MESC1TS_WOREPAIR where rd.WO_ID == Convert.ToInt32(WO_ID) select rd.REPAIR_CD.ToString());

                var CurrencyData = (from t1 in objContext.MESC1TS_COUNTRY_CONT
                                    join t2 in objContext.MESC1TS_LOCATION on t1.COUNTRY_CD equals t2.COUNTRY_CD
                                    //into t1t2 from x in t1t2.DefaultIfEmpty()
                                    join t3 in objContext.MESC1TS_SHOP on t2.LOC_CD equals t3.LOC_CD
                                    //into t1t3 from y in t1t3.DefaultIfEmpty()
                                    join t4 in objContext.MESC1TS_WO on t3.SHOP_CD equals t4.SHOP_CD
                                    //into t1t4 from Z in t1t4.DefaultIfEmpty()
                                    join t5 in objContext.MESC1TS_CURRENCY on t1.CUCDN equals t5.CUCDN
                                    //into t1t5 from AB in t1t5.DefaultIfEmpty()
                                    where t4.MODE == MODE && t1.MANUAL_CD == t4.MANUAL_CD && t4.SHOP_CD == SHOP_CD && t1.EFF_DTE < DateTime.Now && t1.EXP_DTE > DateTime.Now

                                    select new
                                    {
                                        CNTRY_CRNCY = t1.CUCDN,
                                        EXG_RATE = t5.EXRATUSD,
                                        t1.REPAIR_CD




                                    }).Distinct().ToList();
                CurrencyData.Where(s => repCode.Contains(s.REPAIR_CD));



                foreach (var obj in CurrencyData)
                {
                    CurrencyEntities objEntity = new CurrencyEntities();
                    objEntity.CNTRY_CRNCY = obj.CNTRY_CRNCY == null ? "" : obj.CNTRY_CRNCY.ToString();
                    objEntity.EXG_RATE = obj.EXG_RATE == null ? "" : obj.EXG_RATE.ToString();
                    CurrencyList.Add(objEntity);

                }

            }
            catch (SqlException ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return CurrencyList;
        }

        public List<RepairPartsEntities> GetParts(string WO_ID, string REPAIR_CD, string REPAIR_LOC_CODE)
        {

            List<RepairPartsEntities> PartsList = new List<RepairPartsEntities>();

            try
            {
                var woid = Convert.ToInt32(WO_ID);
                var Location_code = (from WR in objContext.MESC1TS_WOREPAIR
                                     where WR.WO_ID == woid
                                     && WR.REPAIR_CD == REPAIR_CD


                                     select new
                                     {
                                         WR.WO_ID,
                                         WR.REPAIR_CD,
                                         WR.REPAIR_LOC_CD

                                     }).FirstOrDefault();//Kasturee_DuplicatePartTax_Bugfix_01-12-18


                int WOID = Convert.ToInt32(WO_ID);

                if (REPAIR_LOC_CODE == Location_code.REPAIR_LOC_CD)
                {
                    var RepairData = (from WP in objContext.MESC1TS_WOPART
                                      from PC in objContext.MESC1TS_MASTER_PART
                                      .Where(m => m.PART_CD == WP.PART_CD)
                                      .DefaultIfEmpty()
                                      from MF in objContext.MESC1TS_MANUFACTUR
                                     .Where(n => n.MANUFCTR == PC.MANUFCTR)
                                     .DefaultIfEmpty()
                                      where WP.WO_ID == WOID && WP.REPAIR_CD == REPAIR_CD
                                      select new
                                      {
                                          WP.PART_CD,
                                          WP.COST_CPH,
                                          WP.COST_LOCAL,
                                          WP.QTY_PARTS,
                                          PC.MANUFCTR,
                                          PC.PART_PRICE,

                                          PC.QUANTITY,
                                          PC.CORE_VALUE,
                                          PC.DEDUCT_CORE,
                                          PC.MSL_PART_SW,
                                          MF.DISCOUNT_PERCENT,
                                          PC.PART_DESC//Kasturee_part_desc_26_03_19


                                      }).ToList();


                    foreach (var obj in RepairData)
                    {
                        RepairPartsEntities objEntity = new RepairPartsEntities();
                        objEntity.PART_CD = obj.PART_CD == null ? "" : obj.PART_CD.ToString();
                       
                        objEntity.COST_CPH = Convert.ToDouble(obj.COST_CPH);
                        objEntity.COST_LOCAL = Convert.ToDouble(obj.COST_LOCAL);
                        objEntity.QTY_PARTS = obj.QTY_PARTS == null ? 0 : (double)obj.QTY_PARTS;
                        objEntity.MANUFCTR = obj.MANUFCTR == null ? "" : obj.MANUFCTR.ToString();
                        objEntity.PART_PRICE = Convert.ToDouble(obj.PART_PRICE);
                        objEntity.QUANTITY = obj.QUANTITY == null ? 0 : (long)obj.QUANTITY;
                        objEntity.CORE_VALUE = obj.CORE_VALUE == null ? 0 : (double)obj.CORE_VALUE;
                        objEntity.DEDUCT_CORE = obj.DEDUCT_CORE == null ? "" : obj.DEDUCT_CORE.ToString();
                        objEntity.MSL_PART_SW = obj.MSL_PART_SW == null ? "" : obj.MSL_PART_SW.ToString();
                        objEntity.DISCOUNT_PERCENT = obj.DISCOUNT_PERCENT == null ? 0 : (double)obj.DISCOUNT_PERCENT;
                        objEntity.PART_DESC = obj.PART_DESC == null ? "" : obj.PART_DESC.ToString();//Kasturee_part_desc_26_03_19
                        PartsList.Add(objEntity);

                    }
                }
            }
            catch (SqlException ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            //finally
            //{
            //    if (sqlcon.State == ConnectionState.Open)
            //        //Closing connection
            //        sqlcon.Close();
            //}

            return PartsList;
        }

        public void GetUpdateWOProcessed(int Woid)
        {

            MercfactUploadEntities objContext = new MercfactUploadEntities();


            try
            {
                var WOUpdate = (from W in objContext.MESC1TS_WO
                                where W.WO_ID == Woid
                                select W).ToList();
                if (WOUpdate.Count > 0)
                {
                    WOUpdate[0].STATUS_CODE = 900;
                    WOUpdate[0].CHTS = DateTime.Now; //KastureeCHTS_18_04_2018
                    objContext.SaveChanges();

                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

        }

        public void GetUpdateWOTransmitted(int Woid, string VoucherNo, string AmountPaid)
        {
            //logEntry.Message = "MercFactUploadService : GetUpdateWOTransmitted Started for WorkId :  " + Woid + " & VoucherNo : " + VoucherNo + " & AmountPaid : " + AmountPaid + "";
            //Logger.Write(logEntry);
            MercfactUploadEntities objContext = new MercfactUploadEntities();


            try
            {
                var WOUpdate = (from W in objContext.MESC1TS_WO
                                where W.WO_ID == Woid
                                select W).ToList();
                if (WOUpdate.Count > 0)
                {
                    WOUpdate[0].STATUS_CODE = 600;
                    WOUpdate[0].VOUCHER_NO = string.IsNullOrEmpty(VoucherNo) ? "" : VoucherNo;
                    WOUpdate[0].AMOUNT_PAID = Convert.ToDecimal(AmountPaid);
                    WOUpdate[0].INVOICE_DTE = DateTime.Now;
                    WOUpdate[0].CHTS = DateTime.Now;   //KastureeCHTS_18_04_2018
                    int result = objContext.SaveChanges();
                    if (result == 1)
                    {
                        MESC1TS_WOAUDIT objAudit = new MESC1TS_WOAUDIT();
                        objAudit.WO_ID = Woid;
                        objAudit.AUDIT_TEXT = "RRIS/FACT Transmitted Successfully";
                        objAudit.CHUSER = "MercFACTUpload";
                        objAudit.CHTS = DateTime.Now;
                        objContext.MESC1TS_WOAUDIT.Add(objAudit);
                        objContext.SaveChanges();

                    }

                }
                //logEntry.Message = "MercFactUploadService : GetUpdateWOTransmitted End for WorkId :  " + Woid + " & VoucherNo : " + VoucherNo + " & AmountPaid : " + AmountPaid + "";
                //Logger.Write(logEntry);
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

        }

        public void SaveLastVoucher(string VoucherNo)
        {

            MercfactUploadEntities objContext = new MercfactUploadEntities();


            try
            {
                var MESC1TS_PROCESS_IDENTIFIERUpdate = (from MPI in objContext.MESC1TS_PROCESS_IDENTIFIER
                                                        where MPI.PROCESS_DESC == "RRIS_XMIT" && MPI.PARAM_NAME == "LAST_VOUCHER_NO"
                                                        select MPI).ToList();
                if (MESC1TS_PROCESS_IDENTIFIERUpdate.Count > 0)
                {
                    if (MESC1TS_PROCESS_IDENTIFIERUpdate[0].PARAM_VALUE.Length == 7)//Kasturee_Voucher_reset_22_04_19
                    {
                        MESC1TS_PROCESS_IDENTIFIERUpdate[0].PARAM_VALUE = "1";

                        int result = objContext.SaveChanges();
                        if (result > 1)
                        {
                            logEntry.Message = "MercFactUploadService : Voucher Reset  in database is  successful. Last value was :  " + VoucherNo + "";
                            Logger.Write(logEntry);

                        }
                    }//Kasturee_Voucher_reset_22_04_19
                    else
                    {
                    MESC1TS_PROCESS_IDENTIFIERUpdate[0].PARAM_VALUE = VoucherNo.Substring(1, VoucherNo.Length - 1);

                    int result = objContext.SaveChanges();
                    if (result > 1)
                        {
                        logEntry.Message = "MercFactUploadService : SaveLastVoucher Saved in database successfully :  " + VoucherNo + "";
                        Logger.Write(logEntry);

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

        }
        public void SendEventTableAudit(int Woid, string msg)
        {

            MercfactUploadEntities objContext = new MercfactUploadEntities();


            try
            {

                MESC1TS_WOAUDIT objAudit = new MESC1TS_WOAUDIT();
                objAudit.WO_ID = Woid;
                objAudit.AUDIT_TEXT = msg;
                objAudit.CHUSER = "MercFACTUpload";
                objAudit.CHTS = DateTime.Now;
                objContext.MESC1TS_WOAUDIT.Add(objAudit);
                objContext.SaveChanges();

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

        }
    }
}
