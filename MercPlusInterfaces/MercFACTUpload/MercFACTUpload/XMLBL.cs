using System;
using System.Collections.Generic;
using System.Linq;
using IBM.WMQ;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Configuration;
using MercFACTUpload;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MercFactUpload;
using MercPlusLibrary;

namespace MercFACTUpload
{
    class XMLBL
    {
        #region Variables declaration
        public static LogEntry logEntry = new LogEntry();
        byte[] MessageID;
        string message = "";
        string VoucherNo = "";
        string strFileName = "";
        // final rounded country and local import tax from database
        // Note DB store these values as a total for all manufacturer parts
        double fCountryTax = 0;
        double fLocalTax = 0;

        // total part price amounts which are taxable
        double fTotLocalTaxableAmt = 0;
        double fTotCountryTaxableAmt = 0;

        // total distributed tax calculated proportionately accross taxable parts.
        double fTotDistributedCountryTax = 0;
        double fTotDistributedLocalTax = 0;

        // misc work
        double fPrice = 0.0;
        double fTax = 0.0;
        double fTaxWorkPct;

      /*  double m_fTAX_LOCAL = 0.0;
        double m_fTAX_CPH = 0.0;
        double m_fCOST_COUNTRY = 0.0;*/
        double m_fCOST_LOCAL = 0.0;

        double fCountryTaxVariation = 0;
        double fLocalTaxVariation = 0;


        // Declare MQ Variables
        MQManager MQmgr = new MQManager();
        MQQueueManager Qmgr = null;
        MQQueue Qqueue = null;
        string strQmsg = "";
        // End MQ

        MercFactUploadDAL objDal = new MercFactUploadDAL();
        #endregion
        public virtual void StartProcessWorkOrder()
        {

            try
            {

                List<MercFactUploadEntity> Wolist = objDal.GetEligibleWorkOrder();

                if (Wolist.Count() > 0)
                {

                    ProcessWorkOrder(Wolist);

                }

            }

            catch (Exception ex)
            {
                message = "Failed read of elligible work orders for FACT upload:";
                logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
            }


        }

        public void ProcessWorkOrder(List<MercFactUploadEntity> Wolist)
        {
            try
            {

                MercFactUploadDAL objDal = new MercFactUploadDAL();
                string strFolderPath = Path.Combine((ConfigurationManager.AppSettings["MessageRepository"]).ToString(), DateTime.UtcNow.ToString("dd-MM-yyyy") + "/");
                //Check if directory exists, create if not.
                if (!Directory.Exists(strFolderPath))
                    Directory.CreateDirectory(strFolderPath);

                for (int i = 0; i < Wolist.Count(); i++)
                {

                    if (IsElligible(Wolist[i]))
                    {
                        VoucherNo = "";
                        VoucherNo = objDal.GetLastVoucherNo();

                        List<RepairEntities> RepairList = objDal.GetRepairs(Wolist[i].WO_ID.ToString());

                        if (RepairList.Count() == 0)
                            continue;

                        strFileName = Wolist[i].WO_ID.ToString() + ".xml";
                        // Append filename with folder path.
                        string strFilePath = Path.Combine(strFolderPath, strFileName);
                        if (File.Exists(strFilePath)) //Check if same file already exists, if yes append _1 with the file name
                        {
                            string[] strArr = strFileName.Split('.');//extract the file name and remove .txt
                            strFileName = strArr[0] + "_" + i + ".xml";
                            strFilePath = Path.Combine(strFolderPath, strFileName);
                        }

                        List<CurrencyEntities> CurrencyList = objDal.GetCountryCurrency(Wolist[i].WO_ID.ToString(), Wolist[i].SHOP_CD.ToString(), Wolist[i].MODE.ToString());

                        if (RepairList.Count() > 0)
                        {

                            for (int k = 0; k < RepairList.Count(); k++)
                            {
                                RepairList[k].pPart = objDal.GetParts(Wolist[i].WO_ID, RepairList[k].REPAIR_CD, RepairList[k].REPAIR_LOC_CD);
                                //BuildRepairParts(Wolist, RepairList, writer, i, k);
                            }

                           // BuildOvertime(Wolist, writer, i);
                        }

                        XmlTextWriter writer = new XmlTextWriter(strFilePath, System.Text.Encoding.UTF8);
                        writer.WriteStartElement("Root");

                        // Build message Broker header
                        BuildBrokerHeader(Wolist[i], writer);
                        // End Build message Broker header


                        // Build Header (all fields including RRIS fields.)
                        // NOTE: if problem in creating data, move on to next work order
                        writer.WriteStartElement("MessageBody");
                        writer.WriteStartElement("HEADER");

                        BuildHeader(Wolist[i], writer);

                        // End Build Header (all fields including RRIS fields.)

                        // calculate CPH prices to country curreny code and distribute import tax on part costs.
                        SetPartPrices(Wolist[i], RepairList);

                        for (int k = 0; k < RepairList.Count(); k++)
                        {
                            RepairEntities pRepair = RepairList[k];
                            BuildRepairParts(Wolist[i], pRepair, writer);
                        }

                        // add overtime repair codes. ITEM A`s RprCode OT01, OT02, OT03
                        BuildOvertime(Wolist[i], writer);

                        writer.WriteEndElement(); // End Header
                        writer.WriteEndElement(); // End MessageBody
                        writer.WriteEndElement(); // End Root
                        writer.Close();

                        string xmlString = System.IO.File.ReadAllText(strFilePath);


                        //Send message to MQ Channnel
                        try
                        {

                            Qmgr = MQmgr.OpenQueueManager(ConfigurationManager.AppSettings["QueueManagerName"]); //"MERCDEV1"
                            Qqueue = MQmgr.OpenQ("W", Qmgr, ConfigurationManager.AppSettings["AccessQueue"]); //BROKER.MERC.IN.FEED
                            MQmgr.PutMessage(Qqueue, xmlString);

                            objDal.GetUpdateWOTransmitted(Convert.ToInt32(Wolist[i].WO_ID), VoucherNo, (Wolist[i].TOT_COST_LOCAL).ToString());
                            objDal.SaveLastVoucher(VoucherNo);
                        }

                        catch (MQException mqException)
                        {

                            message = "Write log - A problem occured while retrieving the MQ message: " + mqException.Message.ToString();
                            logEntry.Message = message;
                            Logger.Write(logEntry);

                        }
                        Qqueue.Close();
                        Qmgr.Close();
                        // End sending to MQ
                    }

                }

            }

            catch (Exception ex)
            {
                message = "Error to fail process WorkOrder";
                logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
            }

        }

        void BuildBrokerHeader(MercFactUploadEntity pWO, XmlTextWriter writer)
        {
            try
            {

                writer.WriteStartElement("MessageHeader");
                writer.WriteStartElement("MessageID");
                writer.WriteElementString("MessageIDSystem", ConfigurationManager.AppSettings["MessageIDSystem"]);
                writer.WriteElementString("MessageIDType", ConfigurationManager.AppSettings["MessageIDType"]);
                writer.WriteElementString("MessageIDIdentifier", ConfigurationManager.AppSettings["MessageIDIdentifier"]);
                writer.WriteEndElement();

                writer.WriteElementString("MessageReleaseNo", ConfigurationManager.AppSettings["MessageReleaseNo"]);
                writer.WriteElementString("MessageVersionNo", ConfigurationManager.AppSettings["MessageVersionNo"]);
                writer.WriteElementString("MessageOrigAppID", ConfigurationManager.AppSettings["MessageOrigAppID"]);
                writer.WriteElementString("MessageDestAppID", ConfigurationManager.AppSettings["MessageDestAppID"]);

                writer.WriteStartElement("RoutingInfo");
                writer.WriteElementString("RoutingValue", pWO.LOC_CD.Substring(0, 2));
                writer.WriteElementString("SpecificDestAppID", "");
                writer.WriteEndElement();
                writer.WriteEndElement();

            }
            catch (Exception ex)
            {
                message = "Error in adding Build Broker Header";
                logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
            }
        }
        void BuildHeader(MercFactUploadEntity pWO, XmlTextWriter writer)
        {
            try
            {

                writer.WriteElementString("WO_ID", pWO.WO_ID);
                writer.WriteElementString("CEN_INDC", pWO.DECENTRALIZED.CompareTo("Y") == 0 ? "D" : "C");
                writer.WriteElementString("REPAIR_DTE", string.IsNullOrEmpty(pWO.REPAIR_DTE.ToString()) ? string.Empty : (Convert.ToDateTime(pWO.REPAIR_DTE.ToString())).ToString("yyyyMMdd"));
                writer.WriteElementString("SHOP_CD", pWO.SHOP_CD);
                writer.WriteElementString("LOC_CD", pWO.LOC_CD);
                writer.WriteElementString("CUSTOMER_CD", pWO.CUSTOMER_CD);
                writer.WriteElementString("CAUSE", pWO.CAUSE);
                writer.WriteElementString("EQPNO", pWO.EQPNO);
                writer.WriteElementString("MODE", pWO.MODE);
                writer.WriteElementString("VENDOR_REF_NO", pWO.VENDOR_REF_NO);
                writer.WriteElementString("PAY_AGT", pWO.CSM_PAYAGENT_CD);
                string sRRIS = FormatRRISPage1(pWO);
                writer.WriteStartElement("RRIS_P01");
                writer.WriteCData(sRRIS);
                writer.WriteEndElement();
                writer.WriteStartElement("RRIS_P02");
                writer.WriteCData("");
                writer.WriteEndElement();
                //writer.WriteElementString("RRIS_P01", "<![CDATA[" + sRRIS + "]]>");
                //writer.WriteElementString("RRIS_P02", "<![CDATA[]]>");
                double tempCOUNTRY_EXCHANGE_RATE = pWO.COUNTRY_EXCHANGE_RATE * 100;
                writer.WriteElementString("CNTY_EXCHANGE_RATE", tempCOUNTRY_EXCHANGE_RATE.ToString());
                writer.WriteElementString("CNTY_EXCHANGE_DATE", string.IsNullOrEmpty(pWO.COUNTRY_EXCHANGE_DTE.ToString()) ? string.Empty : (Convert.ToDateTime(pWO.COUNTRY_EXCHANGE_DTE.ToString())).ToString("yyyyMMdd"));

            }
            catch (Exception ex)
            {
                message = "Error in adding Build Header";
                logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
            }
        }

        void SetPartPrices(MercFactUploadEntity pWO,List<RepairEntities> pRepair)
        {
            try
            {

                // final rounded country and local import tax from database
                // Note DB store these values as a total for all manufacturer parts
                double fCountryTax = 0;
                double fLocalTax = 0;

                // total part price amounts which are taxable
                double fTotLocalTaxableAmt = 0;
                double fTotCountryTaxableAmt = 0;

                // total distributed tax calculated proportionately accross taxable parts.
                double fTotDistributedCountryTax = 0;
                double fTotDistributedLocalTax = 0;

                // misc work
                double fPrice = 0.0;
                double fTax = 0.0;
                double fTaxWorkPct;

                double fCountryTaxVariation = 0;
                double fLocalTaxVariation = 0;
              /*  m_fCOST_LOCAL = 0.0;
                m_fCOST_COUNTRY = 0.0;*/


                // convert IMPORT_TAX_CPH to country currency; and 2 decimal places 
                fCountryTax = pWO.IMPORT_TAX_CPH / (pWO.COUNTRY_EXCHANGE_RATE);
                fLocalTax = pWO.IMPORT_TAX;

                // Step 1
                // iterate thru each part - convert CPH cost to country currency. (note: may be same as local shop)
                // Total both taxable amount local and country for manufacturer's parts for taxable repairs.
                for (int k = 0; k < pRepair.Count() ; k++)
                {
                    RepairEntities pRepairEntities = pRepair[k] ;

                    // iterate thru each part - check if tax required etc and convert CPH to local country currency.
                    for (int i = 0; i < pRepairEntities.pPart.Count() ; i++)
                    {
                        RepairPartsEntities pPart = pRepairEntities.pPart[i] ;
                        // convert CPH (US$) value to country currency
                        pPart.COST_COUNTRY = Math.Round(pPart.COST_CPH / pWO.COUNTRY_EXCHANGE_RATE, 2);
                        // ensure 2 decimals on local price as well.
                        pPart.COST_LOCAL = Math.Round(pPart.COST_LOCAL,2);

                        // if not a maersk part, then part is taxable  if tax applied = 'Y'
                        if (pPart.MSL_PART_SW.CompareTo("Y") != 0)
                        {
                            // if not shop type 2, total taxable amounts
                            //	double fLocalTaxableAmt=0;
                            //	double fCountryTaxableAmt=0;
                            if (pWO.SH_SHOP_TYPE_CD.CompareTo("2") != 0)
                            {
                                if (pRepairEntities.TAX_APPLIED_SW.CompareTo("Y") == 0)
                                {
                                    // total both local amount and country taxable amounts.
                                    fTotCountryTaxableAmt += pPart.COST_COUNTRY;
                                    fTotLocalTaxableAmt += pPart.COST_LOCAL;
                                }
                            }
                        }
                    }
                }

                // We now have total amounts of taxable part costs...

                // Step 2
                // Calculate distributed taxes and total the calculated taxes for check against actual import tax totals.
                for (int k = 0; k < pRepair.Count(); k++)  
                {
                    RepairEntities pRepairEntities = pRepair[k];

                    // iterate thru each part - check if tax required etc
                    if (pRepairEntities.TAX_APPLIED_SW.CompareTo("Y") == 0)
                    {
                        for (int i = 0; i < pRepairEntities.pPart.Count(); i++)
                        {
                            RepairPartsEntities pPart = pRepairEntities.pPart[i];
                            // if not a maersk part, then part is taxable 
                            if (pPart.MSL_PART_SW.CompareTo("Y") != 0)
                            {
                                // for each taxable part ie. manufactures part for shops 1,3 and 4
                                if (pWO.SH_SHOP_TYPE_CD.CompareTo("2") != 0)
                                {
                                    // calculate country portion of tax.
                                    // get percent of total and round calculation
                                    if (fTotCountryTaxableAmt > 0)
                                    {
                                        fTaxWorkPct = pPart.COST_COUNTRY / fTotCountryTaxableAmt;
                                        pPart.TAX_CPH = Math.Round(fTaxWorkPct * fCountryTax,2);
                                        // add to total for later comparison.
                                        fTotDistributedCountryTax += pPart.TAX_CPH;
                                    }

                                    // calculate local portion of tax.
                                    // get percent of total and round calculation
                                    if (fTotLocalTaxableAmt > 0)
                                    {
                                        fTaxWorkPct = pPart.COST_LOCAL / fTotLocalTaxableAmt;
                                        pPart.TAX_LOCAL = Math.Round(fTaxWorkPct * fLocalTax,2);
                                        // add to total for later comparison.
                                        fTotDistributedLocalTax += pPart.TAX_LOCAL;
                                    }
                                }
                            }
                        }
                    }
                }

                // Step 3
                // check calculated total taxes vs. original DB - get variation and apply to first taxable part.
                // sum part prices with tax for final cost for reporting to FACT.
                fCountryTaxVariation = fTotDistributedCountryTax - fCountryTax;
                fLocalTaxVariation = fTotDistributedLocalTax - fLocalTax;

                for (int k = 0; k < pRepair.Count(); k++)  
                {
                    RepairEntities pRepairEntities = pRepair[k];

                    // iterate thru each part - check if tax required etc
                    if (pRepairEntities.TAX_APPLIED_SW.CompareTo("Y") == 0)
                    {
                        for (int i = 0; i < pRepairEntities.pPart.Count(); i++)
                        {
                            RepairPartsEntities pPart = pRepairEntities.pPart[i];
                            // if not a maersk part, then part is taxable 
                            if (pPart.MSL_PART_SW.CompareTo("Y") != 0)
                            {
                                // for each taxable part ie. manufactures part for shops 1,3 and 4
                                if (pWO.SH_SHOP_TYPE_CD.CompareTo("2") != 0)
                                {
                                    // handle variation if != 0;
                                    if (fCountryTaxVariation != 0.0)
                                    {	// add variation to this tax
                                        pPart.TAX_CPH += fCountryTaxVariation;
                                        // set to 0 as only one cost needs to be adjusted.
                                        fCountryTaxVariation = 0.0;
                                    }
                                    // final country cost of part. Add calculated tax to the country cost.
                                    pPart.COST_COUNTRY += pPart.TAX_CPH;

                                    if (fLocalTaxVariation != 0.0)
                                    {
                                        pPart.TAX_LOCAL += fLocalTaxVariation;
                                        // set to 0 as only one cost needs to be adjusted.
                                        fLocalTaxVariation = 0.0;
                                    }
                                    // final local cost of part. Add calculated tax to the local cost.
                                    pPart.COST_LOCAL += pPart.TAX_LOCAL;

                                    // Note: if same currency code, then set country value to local value.
                                    // This solves SQL server issues on rounding numeric values from extract from DB.
                                    // if different currency than shop, value will likely be different in any case.
                                    if (pWO.COUNTRY_CUCDN.CompareTo(pWO.CUCDN) == 0)
                                    {
                                        pPart.COST_COUNTRY = pPart.COST_LOCAL;
                                    }
                                }
                            }
                        }
                    }
                }

                // VJP - 2006-06-09- noted that part costs are stored as totals in wopart table
                // So values must be reduced to unit costs before send to FACT
                for (int k = 0; k < pRepair.Count(); k++)
                {
                    RepairEntities pRepairEntities = pRepair[k];

                    for (int i = 0; i < pRepairEntities.pPart.Count(); i++)
                    {
                        RepairPartsEntities pPart = pRepairEntities.pPart[i];

                        // if qty > 1 : calculate unit cost by dividing part cost by qty of parts.
                        if (pPart.QTY_PARTS > 1)
                        {
                            pPart.COST_COUNTRY = pPart.COST_COUNTRY / pPart.QTY_PARTS;
                            pPart.COST_LOCAL = pPart.COST_LOCAL / pPart.QTY_PARTS;
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                message = "Error to set the part price";
                logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
            }
        }


        void BuildRepairParts(MercFactUploadEntity pWO, RepairEntities pRepairEntities, XmlTextWriter writer)
        {
            try
            {
               
                bool bReportRepair = true;

                // check if this repair code has reportable items. i.e. either labor costs or Type D manufacturer's parts
                // or Type E agency parts to be reported.
                // check first if there is a labor rate.
                if (Convert.ToDouble(pRepairEntities.ACTUAL_MANH) == 0 || Convert.ToDouble(pWO.MANH_RATE_CPH) == 0 || Convert.ToDouble(pRepairEntities.QTY_REPAIRS) == 0)
                {
                    // set report to false unless we find D cost parts that are reportable
                    bReportRepair = false;

                    // here, there are no labor costs, check to see if Type D shop parts are being reported
                    // or if manufacture parts for type 4 shop as well.
                    // if not, exit this routine, do not report this repair code
                  //  List<RepairPartsEntities> pPart = objDal.GetParts(pWO.WO_ID, pRepair.REPAIR_CD);
                    for (int i = 0; i < pRepairEntities.pPart.Count(); i++)
                    {
                        RepairPartsEntities pPart = pRepairEntities.pPart[i];
                        // if Manufacturers Part (ie, NON MSL supplied parts)
                        if (pPart.MSL_PART_SW.CompareTo("Y") != 0)
                        {	// if shop is 1, 3 or 4  and NOT 2  !!!
                            if (pWO.SH_SHOP_TYPE_CD.CompareTo("2") != 0)
                            {	// if part has a cost (likely since zero priced parts should always be superceded)
                                if ((pPart.QTY_PARTS > 0) && (pPart.COST_CPH > 0))
                                    bReportRepair = true;
                            }
                        }
                    }

                    /* FP 6522 Changes by Supriya on 02/11/2007  BEGIN */
                    if (bReportRepair == false)
                    {
                        double tempShopMatAmt = pRepairEntities.SHOP_MATERIAL_AMT;
                        double tempCPHMatAmt = pRepairEntities.CPH_MATERIAL_AMT;
                        //long x = Int64.TryParse(pRepair[k].CPH_MATERIAL_AMT, out

                        if (tempShopMatAmt > 0 || tempCPHMatAmt > 0)
                            bReportRepair = true;
                    }
                    /* FP 6522 Changes by Supriya on 02/11/2007  BEGIN */

                }

                // if reportable items, I.e. either > 0 labor and/or > 0 on manufacturer's parts.
                if (bReportRepair)
                {
                    writer.WriteStartElement("REPAIR");

                    writer.WriteElementString("REPAIR_CD", pRepairEntities.REPAIR_CD);


                    writer.WriteElementString("REPAIR_DESC", pRepairEntities.REPAIR_DESC);



                    // build sub tags, labor costs, items, subconmats(sub contract materials) etc.
                    BuildParts(pWO, pRepairEntities, writer);

                    // Close the repair tag.
                    writer.WriteEndElement();
                }

            }
            catch (Exception ex)
            {
                message = "Error in writing the tag with BuildRepairParts";
                logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
            }
        }
        void BuildParts(MercFactUploadEntity pWO, RepairEntities pRepairEntities, XmlTextWriter writer)
        {
            try
            {
                double f = 0.0;
                bool bCreateZeroLabor = false;
                double dWork = 0;
                double m_fCountry_MANH_RATE_CPH = 0.0;
                //List<RepairPartsEntities> pPart = objDal.GetParts(pWO[i].WO_ID, pRepair[k].REPAIR_CD);

                // Labor Cost - Item A 
                ////////////////////////////////////////////////////////////////
                // build a labor cost for every repair record: ITEM, type A
                // only send labor cost and SUBCONMATs if labor cost > 0

                // NOTE New check for existing labor cost
                // currently, if no labor cost, then no subconmats sent as well - TBD
                // problem for agents - won't get paid for parts.
                if ((pRepairEntities.ACTUAL_MANH > 0) && (pWO.MANH_RATE_CPH > 0) && (pRepairEntities.QTY_REPAIRS > 0))
                {
                    writer.WriteStartElement("ITEM");
                    writer.WriteElementString("COST_TYPE", "A");
                    // qty repairs x manhours
                    f = pRepairEntities.QTY_REPAIRS * pRepairEntities.ACTUAL_MANH;
                    writer.WriteElementString("QTY", Math.Round(f, 2).ToString());
                    writer.WriteElementString("MPN_NO", "");
                    writer.WriteElementString("UOM", "HOUR");

                    // convert rate back to local currency - divide amount by country exchange rate.
                    // VJP (2005-09-22)
                    // safety (test if rate = 0 ) if 0 user current amount.

                    //dWork =Convert.ToDouble(pWO[i].MANH_RATE_CPH) / (Convert.ToDouble(pWO[i].COUNTRY_EXCHANGE_RATE) * 0.01);
                    //dWork = dWork + Math.Round(Convert.ToDouble(pWO[i].SALES_TAX_LABOR), 2);
                   /* m_fCountry_MANH_RATE_CPH = pWO.MANH_RATE * (pWO.COUNTRY_EXCHANGE_RATE) * .01);
                    dWork = m_fCountry_MANH_RATE_CPH / (Convert.ToDouble(pWO[i].COUNTRY_EXCHANGE_RATE) * 0.01);
                    dWork = dWork + (dWork * .01 * Convert.ToDouble(pWO[i].SALES_TAX_LABOR_PCT)); */

                    dWork = pWO.MANH_RATE_CPH  / pWO.COUNTRY_EXCHANGE_RATE;

                    writer.WriteElementString("CPH_AGT_RATE", Math.Round(dWork, 2).ToString());
                    writer.WriteElementString("CPH_AGT_CRCY", pWO.COUNTRY_CUCDN.ToString());
                    writer.WriteElementString("AGT_SHOP_RATE", Math.Round(pWO.MANH_RATE, 2).ToString());
                    writer.WriteElementString("AGT_SHOP_CRCY", pWO.CUCDN.ToString());


                    // check for subconmats, Types: C (Maersk Owned Parts) and E (Agency Owned Parts)
                    // Build type C and E as sub-tags under the ITEM A labor cost Tag
                    ////////////////////////////////////////////////////////////////
                    // check first if there is a labor rate.

                    for (int i = 0; i < pRepairEntities.pPart.Count(); i++)
                    {
                        RepairPartsEntities pPart = pRepairEntities.pPart[i];
                        // Start Calculation
                        //SetPartPrices(pWO, i, pRepair, k, pPart, j);
                        // End Calculation
                        // if MSL part or shop type 2(all parts are automatically maersk supplied parts)
                        if ((pPart.MSL_PART_SW.CompareTo("Y") == 0) || (pWO.SH_SHOP_TYPE_CD.CompareTo("2") == 0))
                        {
                            // if Decentralized shop - don't report MSL parts - will be handled manually.
                            if (pWO.DECENTRALIZED.CompareTo("Y") == 0)
                            {
                                continue;
                            }
                            else
                            {	// is a centralized shop - report part as TYPE C
                                // Build C Cost
                                if ((pPart.COST_CPH > 0) && (pPart.QTY_PARTS) > 0)
                                {
                                    writer.WriteStartElement("SUBCONMAT");
                                    writer.WriteElementString("MPN_NO", pPart.PART_CD);
                                    writer.WriteElementString("MPN_DESC", pPart.PART_DESC);//Kasturee_Part_desc_26_03_19
                                    writer.WriteElementString("COST_TYPE", "C");
                                    writer.WriteElementString("MANUFACT", pPart.MANUFCTR);
                                    writer.WriteElementString("QTY", Math.Round(Convert.ToDecimal(pPart.QTY_PARTS), 2).ToString());
                                    writer.WriteElementString("UOM", "PC");
                                    // calculate part to country currency
                                    // don't recalculate if same exchange rate for parts - use local value.
                                    writer.WriteElementString("CPH_AGT_RATE", Math.Round(pPart.COST_COUNTRY, 2).ToString());
                                    writer.WriteElementString("CPH_AGT_CRCY", pWO.COUNTRY_CUCDN.ToString());
                                    writer.WriteElementString("AGT_SHOP_RATE", Math.Round(pPart.COST_LOCAL, 2).ToString());
                                    writer.WriteElementString("AGT_SHOP_CRCY", pWO.CUCDN.ToString());

                                    writer.WriteEndElement();

                                }
                            }
                        }
                        else  // if decentralized shop
                        {	// if shop type 4 and NOT a maersk part (agency supplied parts) TYPE E
                            if ((pWO.SH_SHOP_TYPE_CD.CompareTo("4") == 0) && (pPart.MSL_PART_SW.CompareTo("Y") != 0))
                            {
                                // if decentralized shop (3rd party agency supplied parts)
                                if (pWO.DECENTRALIZED.CompareTo("Y") == 0)
                                {
                                    // is a shop type 4 with numbered part TYPE E
                                    // Build E Cost
                                    if ((pPart.COST_CPH > 0) && (pPart.QTY_PARTS > 0))
                                    {
                                        writer.WriteStartElement("SUBCONMAT");
                                        writer.WriteElementString("MPN_NO", pPart.PART_CD);
                                        writer.WriteElementString("MPN_DESC", pPart.PART_DESC);//Kasturee_Part_desc_26_03_19
                                        writer.WriteElementString("COST_TYPE", "E");
                                        writer.WriteElementString("MANUFACT", pPart.MANUFCTR);
                                        writer.WriteElementString("QTY", Math.Round(pPart.QTY_PARTS, 2).ToString());
                                        writer.WriteElementString("UOM", "PC");

                                        // calculate part to country currency
                                        // don't recalculate if same exchange rate for parts - use local value.
                                        writer.WriteElementString("CPH_AGT_RATE", Math.Round(pPart.COST_COUNTRY, 2).ToString());
                                        writer.WriteElementString("CPH_AGT_CRCY", pWO.COUNTRY_CUCDN.ToString());
                                        writer.WriteElementString("AGT_SHOP_RATE", Math.Round(pPart.COST_LOCAL, 2).ToString());
                                        writer.WriteElementString("AGT_SHOP_CRCY", pWO.CUCDN.ToString());

                                        writer.WriteEndElement();

                                    }
                                }
                                else
                                {   // handle below to build D Cost (as separate Items)
                                    // At this point, code loop is still in the Item A parent. 
                                    // D Types are Items as well and must be added after Item A
                                    // and subConmats are completed..i.e. outside of this Tag
                                    continue;
                                }
                            }
                        }
                    }
                    // close the labor cost TYPE A ITEM 
                    writer.WriteEndElement();
                }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Awaiting response from CPH.
                // Issue: How do we handle maersk parts w/ costs for centralized shops with zero labor costs?
                // Expect that it will also be a special case as is with decentralized type 4's with manufacturer's
                // parts. Check for MSL parts with associated cost for decentralized shop.
                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                // INCOMPLETE 
                bCreateZeroLabor = false;
                // if zero labor cost.
                if (pRepairEntities.ACTUAL_MANH == 0 || pWO.MANH_RATE_CPH == 0 || pRepairEntities.QTY_REPAIRS == 0)
                {
                    // if shop is CENTRALIZED i.e. not decentralized
                    if (pWO.DECENTRALIZED.CompareTo("Y") != 0)
                    {
                        // iterate thru repair list.
                        for (int i = 0; i < pRepairEntities.pPart.Count(); i++)
                        {
                            RepairPartsEntities pPart = pRepairEntities.pPart[i];

                            // if an MSL part or shop type 2
                            if ((pPart.MSL_PART_SW.CompareTo("Y") == 0) || (pWO.SH_SHOP_TYPE_CD.CompareTo("2") == 0))
                            {
                                // is there a part cost
                                if (Convert.ToDouble(pPart.COST_CPH) > 0 && Convert.ToDouble(pPart.QTY_PARTS) > 0)
                                {
                                    bCreateZeroLabor = true;
                                }
                            }
                        }
                    }
                }

                if (bCreateZeroLabor)
                {
                    writer.WriteStartElement("ITEM");

                    writer.WriteElementString("COST_TYPE", "A");

                    // qty repairs x manhours
                    f = pRepairEntities.QTY_REPAIRS * pRepairEntities.ACTUAL_MANH;
                    writer.WriteElementString("QTY", Math.Round(f, 2).ToString());

                    writer.WriteElementString("MPN_NO", "");

                    writer.WriteElementString("UOM", "HOUR");

                    // convert rate back to local currency - divide amount by country exchange rate.
                    // VJP (2005-09-22)
                    // safety (test if rate = 0 ) if 0 user current amount.
                   // dWork = Convert.ToDouble(pWO[i].MANH_RATE_CPH) / (Convert.ToDouble(pWO[i].COUNTRY_EXCHANGE_RATE) * 0.01);
                   // dWork = dWork + Math.Round(Convert.ToDouble(pWO[i].SALES_TAX_LABOR), 2);
                   /* m_fCountry_MANH_RATE_CPH = Convert.ToDouble(pWO.MANH_RATE) * (Convert.ToDouble(pWO[i].COUNTRY_EXCHANGE_RATE) * .01);
                    dWork = m_fCountry_MANH_RATE_CPH / (Convert.ToDouble(pWO[i].COUNTRY_EXCHANGE_RATE) * 0.01);
                    dWork = dWork + (dWork * .01 * Convert.ToDouble(pWO[i].SALES_TAX_LABOR_PCT)); */

                    dWork = pWO.MANH_RATE_CPH / pWO.COUNTRY_EXCHANGE_RATE;
                    writer.WriteElementString("CPH_AGT_RATE", Math.Round(dWork, 2).ToString());
                    writer.WriteElementString("CPH_AGT_CRCY", pWO.COUNTRY_CUCDN.ToString());
                    writer.WriteElementString("AGT_SHOP_RATE", Math.Round(pWO.MANH_RATE, 2).ToString());
                    writer.WriteElementString("AGT_SHOP_CRCY", pWO.CUCDN.ToString());


                    for (int i = 0; i < pRepairEntities.pPart.Count(); i++)
                    {
                        RepairPartsEntities pPart = pRepairEntities.pPart[i];
                        //SetPartPrices(pWO, i, pRepair, k, pPart, j);
                        // if MSL part or shop type 2.
                        if (pPart.MSL_PART_SW.CompareTo("Y") == 0 || pWO.SH_SHOP_TYPE_CD.CompareTo("2") == 0)
                        {
                            if ((pPart.COST_CPH) > 0 && (pPart.QTY_PARTS) > 0)
                            {
                                // is a shop type 4 with numbered part TYPE E
                                // Build E Cost
                                writer.WriteStartElement("SUBCONMAT");
                                writer.WriteElementString("MPN_NO", pPart.PART_CD);
                                writer.WriteElementString("MPN_DESC", pPart.PART_DESC);//Kasturee_Part_desc_26_03_19
                                writer.WriteElementString("COST_TYPE", "C");
                                writer.WriteElementString("MANUFACT", pPart.MANUFCTR);
                                writer.WriteElementString("QTY", Math.Round(pPart.QTY_PARTS, 2).ToString());
                                writer.WriteElementString("UOM", "PC");
                                // convert rate back to local currency - divide amount by country exchange rate.
                                // VJP (2005-09-22)
                                // safety (test if rate = 0 ) if 0 user current amount.
                                //					dWork = pPart->m_fCOST_CPH / pWO->m_fCOUNTRY_EXCHANGE_RATE;
                                //					strcat( cBuffer, StringUtil.FormatFloatToString( dWork ).c_str() );
                                writer.WriteElementString("CPH_AGT_RATE", Math.Round(pPart.COST_COUNTRY, 2).ToString());
                                writer.WriteElementString("CPH_AGT_CRCY", pWO.COUNTRY_CUCDN.ToString());
                                writer.WriteElementString("AGT_SHOP_RATE", Math.Round(pPart.COST_LOCAL, 2).ToString());
                                writer.WriteElementString("AGT_SHOP_CRCY", pWO.CUCDN.ToString());


                                writer.WriteEndElement();

                            }
                        }
                    }
                    // close ITEM tag
                    writer.WriteEndElement();
                }



                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                // IMCOMPLETE (Waiting on response from CPH) - It is possible that there are no labor costs, 
                // but we will still need to report
                // decentralized parts as SUBCONMATS under a zero labor cost for Shop type 4 only.
                // so we need to check for no labor cost with a decentralized shop type 4 
                // with manufacturer's parts
                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Determine if we need to build special case shop type 4 with zero labor costs.
                // if no labor cost
                bCreateZeroLabor = false;
                if (pRepairEntities.ACTUAL_MANH == 0 || pWO.MANH_RATE_CPH == 0 || pRepairEntities.QTY_REPAIRS == 0)
                {	// if shop type 4 and is decentralized
                    if ((pWO.SH_SHOP_TYPE_CD.CompareTo("4") == 0) && (pWO.DECENTRALIZED.CompareTo("Y") == 0))
                    {
                        // iterate through parts and check for manufacturers parts.
                        for (int i = 0; i < pRepairEntities.pPart.Count(); i++)
                        {
                            RepairPartsEntities pPart = pRepairEntities.pPart[i];
                            // if manufacturer's parts exist, then set switch to build special case.
                            if (pPart.MSL_PART_SW.CompareTo("Y") != 0)
                            {
                                // check if part has a cost > 0.
                                if ((pPart.COST_CPH) > 0 && (pPart.QTY_PARTS) > 0)
                                {
                                    bCreateZeroLabor = true;
                                }
                            }
                        }
                    }
                }
                // So... if  CreateZeroLabor is true, build for shop 4, decentralized with Manufacturer parts and zero labor cost
                if (bCreateZeroLabor)
                {
                    writer.WriteStartElement("ITEM");

                    writer.WriteElementString("COST_TYPE", "A");

                    // qty repairs x manhours
                    f = pRepairEntities.QTY_REPAIRS * pRepairEntities.ACTUAL_MANH;
                    writer.WriteElementString("QTY", Math.Round(f, 2).ToString());

                    writer.WriteElementString("MPN_NO", "");

                    writer.WriteElementString("UOM", "HOUR");

                    // convert rate back to local currency - divide amount by country exchange rate.
                    // VJP (2005-09-22)
                    // safety (test if rate = 0 ) if 0 user current amount.
                  /*  m_fCountry_MANH_RATE_CPH = Convert.ToDouble(pWO[i].MANH_RATE) * (Convert.ToDouble(pWO[i].COUNTRY_EXCHANGE_RATE) * .01);
                    dWork = m_fCountry_MANH_RATE_CPH / (Convert.ToDouble(pWO[i].COUNTRY_EXCHANGE_RATE) * 0.01);
                    dWork = dWork + (dWork * .01 * Convert.ToDouble(pWO[i].SALES_TAX_LABOR_PCT)); */
                    dWork = pWO.MANH_RATE_CPH / pWO.COUNTRY_EXCHANGE_RATE;
                    writer.WriteElementString("CPH_AGT_RATE", Math.Round(dWork, 2).ToString());
                    writer.WriteElementString("CPH_AGT_CRCY", pWO.COUNTRY_CUCDN.ToString());
                    writer.WriteElementString("AGT_SHOP_RATE", Math.Round(pWO.MANH_RATE, 2).ToString());
                    writer.WriteElementString("AGT_SHOP_CRCY", pWO.CUCDN.ToString());


                    // iterate through parts and check for manufacturers parts.
                    for (int i = 0; i < pRepairEntities.pPart.Count(); i++)
                    {
                        RepairPartsEntities pPart = pRepairEntities.pPart[i];
                        //SetPartPrices(pWO, i, pRepair, k, pPart, j);
                        // if non-MSL part, i.e. manufacturer's part, then report it.
                        if (pPart.MSL_PART_SW.CompareTo("Y") != 0)
                        {
                            if ((pPart.COST_CPH) > 0 && (pPart.QTY_PARTS) > 0)
                            {
                                // is a shop type 4 with numbered part TYPE E
                                // Build E Cost
                                writer.WriteStartElement("SUBCONMAT");
                                writer.WriteElementString("MPN_NO", pPart.PART_CD);
                                writer.WriteElementString("MPN_DESC", pPart.PART_DESC);//Kasturee_Part_desc
                                writer.WriteElementString("COST_TYPE", "E");
                                writer.WriteElementString("MANUFACT", pPart.MANUFCTR);
                                writer.WriteElementString("QTY", Math.Round(pPart.QTY_PARTS, 2).ToString());
                                writer.WriteElementString("UOM", "PC");
                                writer.WriteElementString("CPH_AGT_RATE", Math.Round(pPart.COST_COUNTRY, 2).ToString());
                                writer.WriteElementString("CPH_AGT_CRCY", pWO.COUNTRY_CUCDN.ToString());
                                writer.WriteElementString("AGT_SHOP_RATE", Math.Round(pPart.COST_LOCAL, 2).ToString());
                                writer.WriteElementString("AGT_SHOP_CRCY", pWO.CUCDN.ToString());

                                writer.WriteEndElement();


                            }
                        }
                    }

                    // close ITEM tag
                    writer.WriteEndElement();
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                // END special case shop type 4 with no labor costs
                /////////////////////////////////////////////////////////////////////////////////////////////////////////


                // re-iterate thru parts again to check for separate part cost ITEMS - type D. 
                // Shop supplied parts for shop types 1 and 3.
                // D types ITEMS (parts) shops 1 and 3 for non-MSL parts only
                ////////////////////////////////////////////////////////////////

                /* FP 6168 BEGIN*/
                BuildShopMaterials(pWO, pRepairEntities, writer);
                /* FP 6168 END*/

                 // ==========================================kasturee Part XML Start 12-06-18=================================================================
                    /*If any work order have same repair code multipletime with different location code, 
                     * This will add all part_codes to the first repair code in the  XML file and the Second repair will not have any part (To avoid Duplicate_Part)*/

                MercfactUploadEntities objContext = new MercfactUploadEntities();
                var woid = Convert.ToInt32(pWO.WO_ID);
                var Duplicate_Repair_Part = (from WR in objContext.MESC1TS_WOREPAIR
                                             where WR.WO_ID == woid
                                             && WR.REPAIR_CD == pRepairEntities.REPAIR_CD


                                             select new
                                            {
                                                WR.WO_ID,
                                                WR.REPAIR_CD,
                                                WR.QTY_REPAIRS,
                                                WR.SHOP_MATERIAL_AMT,
                                                WR.CPH_MATERIAL_AMT,
                                                WR.CHUSER,
                                                WR.ACTUAL_MANH,
                                                WR.NONS_CD,
                                                WR.MODE,
                                                WR.CHTS,
                                                WR.REPAIR_LOC_CD

                                            }).FirstOrDefault();

                if (Duplicate_Repair_Part.REPAIR_LOC_CD == pRepairEntities.REPAIR_LOC_CD)
                {

                    //========================================kasturee Part XML ends 12-06-18=============================================================

                    for (int i = 0; i < pRepairEntities.pPart.Count(); i++)
                    {
                        RepairPartsEntities pPart = pRepairEntities.pPart[i];

                        //SetPartPrices(pWO, i, pRepair, k, pPart, j);
                        // if manufacturer's parts exist, then set switch to build special case.
                        if (pPart.MSL_PART_SW.CompareTo("Y") != 0)
                        {
                            // if shop is 1 or 3
                            if (pWO.SH_SHOP_TYPE_CD.CompareTo("1") == 0 || pWO.SH_SHOP_TYPE_CD.CompareTo("3") == 0)
                            {	// ensure that this part has a cost, else don't report it.
                                double tempCostCPH = Convert.ToDouble(pPart.COST_CPH);
                                if (Convert.ToInt32(pPart.QTY_PARTS) > 0 && tempCostCPH > 0)
                                {
                                    // Build ITEM D Shop Supplied Parts
                                    writer.WriteStartElement("ITEM");
                                    writer.WriteElementString("MPN_NO", pPart.PART_CD);
                                    writer.WriteElementString("MPN_DESC", pPart.PART_DESC);//Kasturee_Part_desc_26_03_19
                                    writer.WriteElementString("COST_TYPE", "D");
                                    writer.WriteElementString("QTY", Math.Round(pPart.QTY_PARTS, 2).ToString());
                                    writer.WriteElementString("UOM", "PC");
                                    //dWork = Convert.ToDouble(pWO[i].MANH_RATE_CPH) / Convert.ToDouble(pWO[i].COUNTRY_EXCHANGE_RATE);
                                    writer.WriteElementString("CPH_AGT_RATE", Math.Round(pPart.COST_COUNTRY, 2).ToString());
                                    writer.WriteElementString("CPH_AGT_CRCY", pWO.COUNTRY_CUCDN.ToString());
                                    writer.WriteElementString("AGT_SHOP_RATE", Math.Round(pPart.COST_LOCAL, 2).ToString());
                                    writer.WriteElementString("AGT_SHOP_CRCY", pWO.CUCDN.ToString());

                                    writer.WriteEndElement();


                                }
                            }
                        }
                    }
                }//==========================Kasturee Part XML  12-06-18===========
            }
            catch (Exception ex)
            {
                message = "Error in writing the tag for BuildParts";
                logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
            }
        }
        void BuildShopMaterials(MercFactUploadEntity pWO, RepairEntities RepairEntities, XmlTextWriter writer)
        {

            try
            {
                //logEntry.Message = "MercFactUploadService : BuildShopMaterials Started  ";
                //Logger.Write(logEntry);
                //FP 6835 -- IF checking has been added.
                if ((RepairEntities.SHOP_MATERIAL_AMT) > 0 && (RepairEntities.CPH_MATERIAL_AMT) > 0)
                {

                    writer.WriteStartElement("ITEM");

                    writer.WriteElementString("COST_TYPE", "B");
                    writer.WriteElementString("AGT_SHOP_RATE", Math.Round(RepairEntities.SHOP_MATERIAL_AMT, 2).ToString());
                    writer.WriteElementString("AGT_SHOP_CRCY", pWO.CUCDN.ToString());
                    double dblVal = Convert.ToDouble(RepairEntities.CPH_MATERIAL_AMT);
                    dblVal /= (Convert.ToDouble(pWO.EXCHANGE_RATE) * 0.01);
                    writer.WriteElementString("CPH_AGT_RATE", Math.Round(dblVal, 2).ToString());
                    writer.WriteElementString("CPH_AGT_CRCY", pWO.COUNTRY_CUCDN.ToString());
                    writer.WriteElementString("QTY", Math.Round(Convert.ToDecimal(RepairEntities.QTY_REPAIRS), 2).ToString());
                    writer.WriteElementString("UOM", "PC");


                    writer.WriteEndElement();

                }

            }
            catch (Exception ex)
            {
                message = "Error in writing the tag ofBuildShopMaterials";
                logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
            }
        }
        void BuildOvertime(MercFactUploadEntity pWO, XmlTextWriter writer)
        {

            try
            {
                double dWork = 0.0;

                // Report overtimes as difference between OT and regular hours.
                if (pWO.TOT_MANH_OT > 0)
                {
                    // Build OT01 Repair
                    // Build B where TAX=Y

                    writer.WriteStartElement("REPAIR");

                    writer.WriteElementString("REPAIR_CD", "OT01");
                    writer.WriteElementString("REPAIR_DESC", "OVERTIME HOURS 1");

                    writer.WriteStartElement("ITEM");

                    // Item A Overtime	
                    writer.WriteElementString("COST_TYPE", "A");

                    writer.WriteElementString("QTY", Math.Round(pWO.TOT_MANH_OT, 2).ToString());

                    writer.WriteElementString("MPN_NO", "");

                    writer.WriteElementString("UOM", "HOUR");

                   // dWork = (Convert.ToDouble(pWO[i].OT_RATE_CPH) - Convert.ToDouble(pWO[i].MANH_RATE_CPH)) / ((Convert.ToDouble(pWO[i].COUNTRY_EXCHANGE_RATE) * 0.01));
                    dWork = (pWO.OT_RATE_CPH - pWO.MANH_RATE_CPH) / pWO.COUNTRY_EXCHANGE_RATE;
                    
                    writer.WriteElementString("CPH_AGT_RATE", Math.Round(dWork, 2).ToString());

                    writer.WriteElementString("AGT_SHOP_RATE", (Math.Round(pWO.OT_RATE - pWO.MANH_RATE, 2)).ToString());

                    writer.WriteElementString("CPH_AGT_CRCY", pWO.COUNTRY_CUCDN.ToString());

                    writer.WriteElementString("AGT_SHOP_CRCY", pWO.CUCDN.ToString());

                    //Close tag for Item
                    writer.WriteEndElement();

                    //Close tag for Repair
                    writer.WriteEndElement();

                }

                if (Convert.ToDouble(pWO.TOT_MANH_DT) > 0)
                {
                    // Build OT02 Repair
                    writer.WriteStartElement("REPAIR");

                    writer.WriteElementString("REPAIR_CD", "OT02");

                    writer.WriteElementString("REPAIR_DESC", "OVERTIME HOURS 2");

                    writer.WriteStartElement("ITEM");

                    // Item A Overtime	
                    writer.WriteElementString("COST_TYPE", "A");

                    writer.WriteElementString("QTY", Math.Round(pWO.TOT_MANH_DT, 2).ToString());

                    writer.WriteElementString("MPN_NO", "");

                    writer.WriteElementString("UOM", "HOUR");

                    dWork = (pWO.DT_RATE_CPH - pWO.MANH_RATE_CPH) / (pWO.COUNTRY_EXCHANGE_RATE);
                    writer.WriteElementString("CPH_AGT_RATE", Math.Round(dWork, 2).ToString());

                    writer.WriteElementString("AGT_SHOP_RATE", (Math.Round(pWO.DT_RATE - pWO.MANH_RATE, 2)).ToString());

                    writer.WriteElementString("CPH_AGT_CRCY", pWO.COUNTRY_CUCDN.ToString());

                    writer.WriteElementString("AGT_SHOP_CRCY", pWO.CUCDN.ToString());


                    //Close tag for Item
                    writer.WriteEndElement();

                    //Close tag for Repair
                    writer.WriteEndElement();

                }

                if (Convert.ToDouble(pWO.TOT_MANH_MISC) > 0)
                {
                    // Build OT03 Repair

                    writer.WriteStartElement("REPAIR");

                    writer.WriteElementString("REPAIR_CD", "OT03");

                    writer.WriteElementString("REPAIR_DESC", "OVERTIME HOURS 3");

                    writer.WriteStartElement("ITEM");

                    // Item A Overtime	
                    writer.WriteElementString("COST_TYPE", "A");

                    writer.WriteElementString("QTY", Math.Round(Convert.ToDecimal(pWO.TOT_MANH_MISC), 2).ToString());

                    writer.WriteElementString("MPN_NO", "");

                    writer.WriteElementString("UOM", "HOUR");

                    dWork = (pWO.MISC_RATE_CPH - pWO.MANH_RATE_CPH) / (pWO.COUNTRY_EXCHANGE_RATE);
                    writer.WriteElementString("CPH_AGT_RATE", Math.Round(dWork, 2).ToString());

                    writer.WriteElementString("AGT_SHOP_RATE", Math.Round(pWO.MISC_RATE- pWO.MANH_RATE, 2).ToString());

                    writer.WriteElementString("CPH_AGT_CRCY", pWO.COUNTRY_CUCDN.ToString());

                    writer.WriteElementString("AGT_SHOP_CRCY", pWO.CUCDN.ToString());


                    //Close tag for Item
                    writer.WriteEndElement();

                    //Close tag for Repair
                    writer.WriteEndElement();
                }

            }
            catch (Exception ex)
            {
                message = "Error in writingg the tag of BuilOverTimes";
                logEntry.Message = logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
            }
        }


        public string FormatRRISPage1(MercFactUploadEntity pWO)
        {

            string sRRISMsg = "";
            /////////////////////////////////////////////////////////
            // data extraction
            /////////////////////////////////////////////////////////

            string sNullStr = "";
            string sUnTrimmedFormat, sTrimmedCSMFormat, sTrimmedPAFormat;

            // load next voucher number and payagent/vendor values.
            string sSupplierCd = pWO.SUPPLIER_CD;
            string sLocAccCd = pWO.LOCAL_ACCOUNT_CD;

            string sVoucherNo = GetNextVoucher();
            //pWO[i].VOUCHER_NO = sVoucherNo;
            /////////////////////////////////////////////////////////
            // message formatting
            /////////////////////////////////////////////////////////
            string sTemp = "";
            int iRRIS_Format = 0;
            string sRRIS_Format;
            string sHdr;
            string sRec100;
            string sRec101;
            string sRec102;
            string sAgent = "";
            string sPCCode = "";
            string sAPMGroup = "";
            string sApmAcc = "";
            string sActPort = "";
            string sSuffixCode101 = "";
            string sSuffixCode102 = "";
            string sLocAcc = "";
            string sLocAccTemp;
            string sAgent1;
            string sPCCode102;
            string sRepDte;
            string sRepMonth;
            string sRepYear;
            string sFiller = "  ";
            string sTruncDesc;
            const string sMsgStart = "MERCRRIS01000001";
            const string sUserID = "BNYCWMES";
            const string sEnd = "E";
            //////////////////////////
            //Ticket # 2510
            string sExchangeRate;

            try
            {
                sUnTrimmedFormat = pWO.CSM_RRIS_FORMAT;
                sTrimmedCSMFormat = sUnTrimmedFormat.Trim();

                if (sTrimmedCSMFormat == "")
                {
                    sUnTrimmedFormat = pWO.PA_RRIS_FORMAT;
                    sTrimmedPAFormat = sUnTrimmedFormat.Trim();
                    sRRIS_Format = sTrimmedPAFormat;
                }
                else
                    sRRIS_Format = sTrimmedCSMFormat;

                /////////////////////////////////////////////////////////
                if (sRRIS_Format.CompareTo("52") == 0)
                    iRRIS_Format = 52;
                if (sRRIS_Format.CompareTo("70") == 0)
                    iRRIS_Format = 70;
                if (sRRIS_Format.CompareTo("72") == 0)
                    iRRIS_Format = 72;
                /////////////////////////////////////////////////////////

                switch (iRRIS_Format)
                {
                    case 52:
                        {
                            sTemp = pWO.CSM_PAYAGENT_CD.ToString();
                            if (sTemp == "")
                                sTemp = pWO.PA_PAYAGENT_CD;
                            sAgent = sTemp;
                            sTemp = pWO.CSM_PROFIT_CENTER;
                            if (sTemp == "")
                                sTemp = pWO.PA_PROFIT_CENTER;
                            sPCCode = sTemp;
                            sLocAcc = sLocAccCd;
                            sSuffixCode101 = "   ";
                            //				sAPMGroup	="01";
                            sAPMGroup = "02";
                            sActPort = pWO.SH_RKRPLOC;
                            sTemp = pWO.CSM_ACCOUNT_CD;
                            if (sTemp == "")
                                sTemp = pWO.XACCOUNT_CD;
                            sApmAcc = sTemp;
                            sSuffixCode102 = "   ";
                        }
                        break;

                    case 70:
                        {
                            sTemp = pWO.CSM_CORP_PAYAGENT_CD;
                            if (sTemp == "")
                                sTemp = pWO.PA_CORP_PAYAGENT_CD;
                            sAgent = sTemp;
                            sTemp = pWO.CSM_SUB_PROFIT_CENTER;
                            if (sTemp == "")
                                sTemp = pWO.PA_SUB_PROFIT_CENTER;
                            sPCCode = sTemp;
                            sSuffixCode101 = pWO.SH_RRIS70_SUFFIX_CD;
                            //sAPMGroup.Resize(2, ' ');
                            //sActPort.Resize(3, ' ');
                            sLocAcc = sLocAccCd;

                            sTemp = pWO.CSM_ACCOUNT_CD;
                            if (sTemp == "")
                                sTemp = pWO.XACCOUNT_CD;
                            sApmAcc = sTemp;
                            sSuffixCode102 = pWO.SH_RRIS70_SUFFIX_CD;
                        }
                        break;

                    case 72:
                        {
                            sTemp = pWO.CSM_CORP_PAYAGENT_CD;
                            if (sTemp == "")
                                sTemp = pWO.PA_CORP_PAYAGENT_CD;
                            sAgent = sTemp;
                            sTemp = pWO.CSM_SUB_PROFIT_CENTER;
                            if (sTemp == "")
                                sTemp = pWO.PA_SUB_PROFIT_CENTER;
                            sPCCode = sTemp;
                            sSuffixCode101 = "   ";
                            sAPMGroup = "  ";
                            sActPort = "   ";
                            sTemp = pWO.CSM_PAYAGENT_CD;
                            if (sTemp == "")
                                sTemp = pWO.PA_PAYAGENT_CD;
                            sAgent1 = sTemp;
                            sTemp = pWO.CSM_PROFIT_CENTER;
                            if (sTemp == "")
                                sTemp = pWO.PA_PROFIT_CENTER;
                            sPCCode102 = sTemp;
                            sLocAcc = sLocAccCd;

                            sTemp = pWO.CSM_ACCOUNT_CD;
                            if (sTemp == "")
                                sTemp = pWO.XACCOUNT_CD;
                            sApmAcc = sTemp;
                            sSuffixCode102 = pWO.SH_RRIS70_SUFFIX_CD;
                        }
                        break;
                    default:
                        break;
                }

                // date value YYYYMMDD. extract YYMMDD.
                string sTmp = pWO.REPAIR_DTE.Substring(2, 6);
                sRepDte = sTmp;
                sRepYear = sRepDte.Substring(0, 2);
                sRepMonth = sRepDte.Substring(2, 2);

                /////////////////////////////////////////////////////////
                //Determinig total amount
                /**********************************************************************
                //Initial implementation required the calculation of total amount
                //fr update into the WO table. New requirement - the same amount 
                //that is sent to RRIS is to be used to populate the total amount field
                **********************************************************************/

                /////////////////////////////////////////////////////////
                // Header
                /////////////////////////////////////////////////////////
                sHdr = sMsgStart;
                sHdr += string.IsNullOrEmpty(DateTime.Now.ToString()) ? string.Empty : (Convert.ToDateTime(DateTime.Now.ToString())).ToString("yyyy-MM-dd-HH.mm.ss.ffffff");
                /////////////////////////////////////////////////////////
                // Record 100 
                /////////////////////////////////////////////////////////
                sRec100 = "100";
                sRec100 += sUserID;
                sRec100 += sEnd;
                /////////////////////////////////////////////////////////
                // Record 101 
                /////////////////////////////////////////////////////////
                sRec101 = "101";
                sRec101 += FormatString(sAgent, 3, true);
                sRec101 += ("  ");//Accounting year
                sRec101 += sVoucherNo;
                sRec101 += sRRIS_Format; // PICNO
                sRec101 += "  ";//Accounting month
                sRec101 += string.IsNullOrEmpty(pWO.REPAIR_DTE.ToString()) ? string.Empty : (Convert.ToDateTime(pWO.REPAIR_DTE.ToString())).ToString("yyMMdd");
                sRec101 += FormatString(sPCCode, 2, true);
                sRec101 += FormatString(sLocAcc, 6, true);
                sRec101 += pWO.WO_ID.ToString();
                // Total cost - negative amount
                sRec101 += "  -";

                sTemp = (Math.Round(Convert.ToDecimal(pWO.TOT_COST_LOCAL), 2)).ToString();
                // confirm whether false or true
                string sCost = sTemp.PadLeft(14, '0');



                sRec101 += sCost; // Invoice amt
                sTemp = "";
                sRec101 += "-     ";
                // New addition - 032904 - formatstring invoked for all append
                sRec101 += string.IsNullOrEmpty(pWO.REPAIR_DTE.ToString()) ? string.Empty : (Convert.ToDateTime(pWO.REPAIR_DTE.ToString())).ToString("yyMM");
                //sRec101 += FormatString(sRepYear, 2, true);
                sRec101 += (FormatString(sSuffixCode101, 2, true));
                sRec101 += (FormatString(pWO.CUCDN, 3, true));
                //////////////////////////////////////////////////////////////////

                // if format is 72, do not include the exchange rate.
                if (iRRIS_Format == 72)
                {	// space fill to 8 characters.
                    sExchangeRate = " ";
                    sRec101 += "        ";
                }
                else
                {

                    sExchangeRate = (Math.Round(Convert.ToDecimal(pWO.EXCHANGE_RATE), 2)).ToString();

                    sExchangeRate = sExchangeRate.PadLeft(8, '0');
                    sRec101 += sExchangeRate;
                }

                //////////////////////////////////////////////////////////////////
                sRec101 += (FormatString(pWO.VENDOR_REF_NO, 10, true));
                sRec101 += (FormatString(sSupplierCd, 6, true));
                sRec101 += (sAPMGroup);
                sRec101 += (FormatString(sActPort, 3, true));
                sRec101 += string.IsNullOrEmpty(pWO.APPROVAL_DTE.ToString()) ? string.Empty : (Convert.ToDateTime(pWO.APPROVAL_DTE.ToString())).ToString("yyMMdd");
                sRec101 += ("   "); // Batch
                sRec101 += ("       ");//Filler
                sRec101 += (sEnd);
                /////////////////////////////////////////////////////////
                // Record 102 
                /////////////////////////////////////////////////////////
                sRec102 = "102";
                sRec102 += (FormatString(sAgent, 3, true));
                sRec102 += ("  ");//Accounting year
                sRec102 += (sVoucherNo);
                sRec102 += (sRRIS_Format); // PICNO
                string sTempCost = (Math.Round(Convert.ToDecimal(pWO.TOT_COST_LOCAL), 2)).ToString();

                sTempCost = sTempCost.PadLeft(15, '0');
                sRec102 += sTempCost;

                if (iRRIS_Format == 52)
                {
                    sTruncDesc = FormatString((pWO.EQPNO + " " + pWO.SHOP_CD + " " + pWO.SH_SHOP_DESC), 33, true);
                    sRec102 += (FormatString(sApmAcc, 7, true));
                    sRec102 += ("99");
                    sRec102 += string.IsNullOrEmpty(pWO.REPAIR_DTE.ToString()) ? string.Empty : (Convert.ToDateTime(pWO.REPAIR_DTE.ToString())).ToString("yyMM");
                    //sRec102 += (FormatString(sRepYear, 2, true));
                    sRec102 += (sSuffixCode102);//suffix
                    sRec102 += (sTruncDesc);
                    //sFiller.resize(68, ' ');
                    sRec102 += (sFiller);
                }
                else
                {
                    if (iRRIS_Format == 72)
                        sRec102 += (FormatString(sAgent, 3, true));//FormatString newly added 032904			sRec102.append(FormatString(sApmAcc, 6, true));
                    if (iRRIS_Format == 70)
                    {
                        sTruncDesc = FormatString((" " + pWO.EQPNO + " " + pWO.SHOP_CD + " " + pWO.SH_SHOP_DESC), 40, true);
                        //sFiller.resize(62, ' ');

                    }
                    else
                    {
                        sTruncDesc = FormatString((pWO.EQPNO + " " + pWO.SHOP_CD + " " + pWO.SH_SHOP_DESC), 34, true);
                        //sFiller.resize(65, ' ');
                    }
                    sRec102 += (sTruncDesc);
                    sRec102 = string.IsNullOrEmpty(pWO.REPAIR_DTE.ToString()) ? string.Empty : (Convert.ToDateTime(pWO.REPAIR_DTE.ToString())).ToString("yyMM");
                    //sRec102 += (FormatString(sRepMonth, 2, true));
                    //sRec102 += (FormatString(sRepYear, 2, true));
                    sRec102 += (sSuffixCode102);//suffix
                    sRec102 += (sFiller);
                }
                sRec102 += (sEnd);

                sRRISMsg = sHdr;
                sRRISMsg += (sRec100);
                sRRISMsg += (sRec101);
                sRRISMsg += (sRec102);

            }
            catch (Exception ex)
            {
                objDal.SendEventTableAudit(Convert.ToInt32(pWO.WO_ID), "Fail on build of RRIS01 message");
                message = "Fail on build of RRIS01 message";
                logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
                return (sNullStr);
                //return(sRRISMsg);
            }

            return (sRRISMsg);
        }

        /////////////////////////////////////////////////////////////////////////////
        // CVoucherGen member functions
        public string GetNextVoucher()
        {
            char[] cNum = new char[8];

            // Start with leading 'Q' prefix + 3 Char Julian day + 4 alpha/digit ascending value
            string sNextVoucher = "Q";
            try
            {

                // if no voucher, start new.
                if (VoucherNo.Length == 0)
                    VoucherNo = "";
                // default true if all final 4 characters are digits.i.e. < 9999  else could be A001 thru Z999
                bool bNumber = true;

                // Get last 4 characters. if first char is alpha, set bNumber to false.
                string sNum = VoucherNo;



                // convert to int and increment 
                int iNum = StringToInt(sNum.ToCharArray());
                iNum++;
                VoucherNo = "";
                VoucherNo = iNum.ToString();


            }
            catch (Exception ex)
            {
                message = "Error in getting the next voucher number";
                logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
            }

            // finally append Next Voucher number to 'Q' prefix.

            VoucherNo = sNextVoucher + VoucherNo;
            sNextVoucher = VoucherNo;
            return (sNextVoucher);
        }

        public static int StringToInt(char[] ch)
        {
            int length = ch.Length;
            int i = 0;
            int lastNumber = 0;
            int returnNumber = 0;
            bool numberNegative = false;
            int startPoint = 0;

            if (ch[0] == '-')
            {
                numberNegative = true;
                startPoint = 1;
            }

            for (i = startPoint; i < length; i++)
            {
                if (ch[i] == ' ')
                {
                    continue;
                }
                else
                {
                    if ((ch[i] >= '0') && ch[i] <= '9')
                    {
                        returnNumber = ch[i] - '0';
                        if (i > 0)
                            lastNumber = lastNumber * 10;
                        lastNumber = lastNumber + returnNumber;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (numberNegative)
                lastNumber = -1 * lastNumber;

            return lastNumber;
        }


        public string FormatString(string sString, int iSize, bool bCharTrue)
        {
            string sTrimString = sString.Trim();
            string sFormatString = "";
            int iStrSize = sTrimString.Count();
            try
            {

                if (iStrSize < iSize)
                    // alpha left justified, numeric right justified
                    if (bCharTrue)
                    {
                        //sString.Resize(iSize, ' ');
                        sFormatString = sString;
                    }
                    else
                    {
                        // changed to left pad numbers with '0'
                        //sFormatString.resize((iSize - iStrSize), '0');
                        sFormatString += sTrimString;
                    }

                else
                    if (iStrSize > iSize)
                        sFormatString = sTrimString.Substring(0, iSize);
                    else
                        sFormatString = sTrimString;

            }
            catch (Exception ex)
            {
                message = "Error in formatting the string";
                logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
            }
            return (sFormatString);
        }

        bool IsElligible(MercFactUploadEntity pRecord)
        {
            //logEntry.Message = "MercFactUploadService : Verifying the eligible WorkOrder " + WO_ID + "";
            //Logger.Write(logEntry);
            try
            {
                bool bOK = true;
                double dMaterials = 0.0;
                MercFactUploadDAL objDal = new MercFactUploadDAL();
                List<RepairEntities> RepairList = objDal.GetRepairs(pRecord.WO_ID.ToString());
                if ((pRecord.SH_SHOP_TYPE_CD.CompareTo("4") == 0) && (pRecord.TOT_COST_LOCAL == 0.00))
                {

                    dMaterials = 0.0;

                    for (int k = 0; k < RepairList.Count(); k++)
                    {
                        dMaterials += RepairList[k].SHOP_MATERIAL_AMT;
                        List<RepairPartsEntities> PartList = objDal.GetParts(pRecord.WO_ID.ToString(), RepairList[k].REPAIR_CD.ToString(), RepairList[k].REPAIR_LOC_CD);
                        for (int n = 0; n < PartList.Count(); n++)
                        {	// Check for manufacturer's part with costs.

                            if (PartList[n].MSL_PART_SW.CompareTo("Y") != 0)
                            {
                                double tempCostCPH = PartList[n].COST_CPH;
                                if ((PartList[n].QTY_PARTS > 0) && (tempCostCPH > 0))
                                {
                                    dMaterials += tempCostCPH;
                                }
                            }
                        }
                        // if no shop materials, labor costs or Manufacturer's parts, then bypass this work order.
                        if (dMaterials == 0.0)
                        {	// 900 Processed status for non-elligible work orders
                            // bypass this record and exit - no point in continuing.
                            objDal.GetUpdateWOProcessed(Convert.ToInt32(pRecord.WO_ID));
                            objDal.SendEventTableAudit(Convert.ToInt32(pRecord.WO_ID), "WO not eligible for FACT transmission - Shop switch = %s, Transmit switch = %s -WO status code ted to 900");
                            return false;
                        }
                    }
                }




                // additional zero cost check needed - 
                // check if any charges exist on this work order to be reported i.e. zero charges etc.
                // if zero cost work order, not parts, labor or material costs, then set to 900 and bypass.
                // if shoptype 1,2 or 3
                if ((pRecord.SH_SHOP_TYPE_CD.CompareTo("4") != 0) && (pRecord.TOT_COST_LOCAL == 0.00))
                {
                    dMaterials = 0.0;

                    // check if any shop materials exist
                    for (int k = 0; k < RepairList.Count(); k++)
                    {

                        dMaterials += Convert.ToDouble(RepairList[k].SHOP_MATERIAL_AMT);
                    }

                    if (dMaterials == 0.0)
                    {	// 900 Processed status for non-elligible work orders
                        // bypass this record and exit - no point in continuing.
                        objDal.GetUpdateWOProcessed(Convert.ToInt32(pRecord.WO_ID));
                        objDal.SendEventTableAudit(Convert.ToInt32(pRecord.WO_ID), "WO not eligible for FACT transmission - Shop switch = %s, Transmit switch = %s -WO status code updated to 900");
                        return false;
                    }
                }


                // Check if record is set up to be sent to FACT/RRIS
                // Ticket 4798 - VJP - 2006-08-23
                // RRIS XMIT Sw can = 'F(FACT)', 'Y(RRIS)' or 'N(None)' If none selected, then not elligible.
                // if ((pRecord->m_sSH_RRIS_XMIT_SW.compare("Y")!=0) || (pRecord->m_sXRRIS_XMIT_SW.compare("Y")!=0))		
                if ((pRecord.SH_RRIS_XMIT_SW.CompareTo("N") == 0) || (pRecord.XRRIS_XMIT_SW.CompareTo("Y") != 0))
                {	// 900 Processed status for non-elligible work orders
                    // not elligible for FACT/RRIS - sent event log and update record to 900 (processed)
                    objDal.GetUpdateWOProcessed(Convert.ToInt32(pRecord.WO_ID));
                    objDal.SendEventTableAudit(Convert.ToInt32(pRecord.WO_ID), "WO not eligible for FACT transmission - Shop switch = %s, Transmit switch = %s -WO status code updated to 900");
                    return false;
                }

                // if no pay agent on Customer/Shop/Mode table, is an error.
                if (bOK)
                {
                    if (pRecord.CSM_PAYAGENT_CD == "")
                    {
                        //sprintf(msg, "FACT Processing - PAYAGENT data not available for Work Order : %s ",pRecord->m_sWO_ID.c_str());
                        objDal.SendEventTableAudit(Convert.ToInt32(pRecord.WO_ID), "FACT Processing - PAYAGENT data not available for Work Order");
                        //nWoError++;
                        bOK = false;
                    }
                }

                // check payagent/Vendor both codes must exist.
                if (bOK)
                {
                    if (pRecord.SUPPLIER_CD == "" && pRecord.LOCAL_ACCOUNT_CD == "")
                    {
                        //sprintf(msg, "FACT Processing - PAYAGENT_VENDOR data not available for Work Order : %s ",pRecord->m_sWO_ID.c_str());
                        objDal.SendEventTableAudit(Convert.ToInt32(pRecord.WO_ID), "FACT Processing - PAYAGENT_VENDOR data not available for Work Order");
                        //nWoError++;
                        bOK = false;
                    }
                }

                // check RRIS format
                if (bOK)
                {
                    string sUnTrimmedFormat = pRecord.CSM_RRIS_FORMAT;
                    string sTrimmedCSMFormat = sUnTrimmedFormat.Trim();


                    if (sTrimmedCSMFormat == "")
                    {
                        sUnTrimmedFormat = pRecord.PA_RRIS_FORMAT;
                        string sTrimmedPAFormat = sUnTrimmedFormat.Trim();
                        if (sTrimmedPAFormat == "")
                        {
                            //sprintf(msg, "FACT Processing - RRIS_FORMAT data not available for Work Order : %s  in MESC1TS_CUSTOMER_SHOP_MODE or MESC1TS_TRANSMIT ",pRecord->m_sWO_ID.c_str());
                            objDal.SendEventTableAudit(Convert.ToInt32(pRecord.WO_ID), "FACT Processing - RRIS_FORMAT data not available for Work Order :");
                            //nWoError++;
                            bOK = false;
                        }
                    }
                }

                // Check that country exchange information
                if (bOK)
                {
                    if (pRecord.COUNTRY_CUCDN == "" || pRecord.COUNTRY_EXCHANGE_DTE == "")
                    {
                        //sprintf(msg, "FACT Processing - Country exchange data not available for Work Order : %s",pRecord->m_sWO_ID.c_str());
                        objDal.SendEventTableAudit(Convert.ToInt32(pRecord.WO_ID), "FACT Processing - Country exchange data not available for Work Order :");
                        //nWoError++;
                        bOK = false;
                    }
                }
                if (bOK)
                {
                    if (pRecord.COUNTRY_EXCHANGE_RATE == 0.00)
                    {
                        //sprintf(msg, "FACT Processing - Country exchange rate not available for Work Order : %s",pRecord->m_sWO_ID.c_str());
                        objDal.SendEventTableAudit(Convert.ToInt32(pRecord.WO_ID), "FACT Processing - Country exchange rate not available for Work Order :" + pRecord.WO_ID);
                        //nWoError++;
                        bOK = false;
                    }
                    /*else
                    {
                        bOK = false;
                    }*/
                }

                return (bOK);
            }
            catch (Exception ex)
            {
                message = "Error in finding the eligible WorkOrder";
                logEntry.Message = (message) + "  " + ex.Message;
                Logger.Write(logEntry);
                return false;
            }
        }


    }

}
