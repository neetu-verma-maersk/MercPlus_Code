using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using MercPlusLibrary;
using MercWorkOrderWebService.ManageWorkOrderServiceReference;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace MercWorkOrderWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class MercWorkOrderWeb : IMercWorkOrderWeb
    {
        MESC2DSEntities objContext = new MESC2DSEntities();
        ManageWorkOrderClient WorkOrderClient = new ManageWorkOrderClient();
        LogEntry logEntry = new LogEntry();

        public string ProcessRequest(string XMLString)
        {
            string Status = "200 OK";
            bool bOK = true;
            bool bLogXML = true;
            bool VarOut = true;
            bool success = false;
            // Create the XmlDocument.
            XmlDocument doc = new XmlDocument();
            XmlNodeList xmlnode = null;
            XmlDataDocument xmldoc = new XmlDataDocument();
            List<MercPlusLibrary.ErrMessage> ErrorMessageList = new List<MercPlusLibrary.ErrMessage>();

            int i = 0;
            int j = 0;
            string WorkOrderString = string.Empty;
            MercPlusLibrary.WorkOrderDetail WOD = new MercPlusLibrary.WorkOrderDetail();
            WOD.RepairsViewList = new List<RepairsView>();
            WOD.SparePartsViewList = new List<SparePartsView>();
            //doc.LoadXml("<item><name>wrench</name></item>");
            string tCustomer = string.Empty;
            string tShop = string.Empty;
            string tLocation = string.Empty;
            string tType = string.Empty;
            string tDate = string.Empty;
            string tVendorRef = string.Empty;


            // equipment
            string tNumber = string.Empty;
            string tMode = string.Empty;
            string tCause = string.Empty;

            //<totalhours straighttime="1.0" overtime="" doubletime="" miscellaneoustime=""/> 
            string tStraighttime = string.Empty;
            string tOvertime = string.Empty;
            string tDoubletime = string.Empty;
            string tMiscellaneoustime = string.Empty;

            // Total EDI cost
            string tTotalCost = string.Empty;

            // Repairs
            string tDamageCode = string.Empty;
            string tCode = string.Empty;
            string tRepairLocCode = string.Empty;
            string tQty = string.Empty;
            string tCost = string.Empty;
            string tManHours = string.Empty;
            string tTpi = string.Empty;

            // Parts
            string tPartCode = string.Empty;
            string tPartQty = string.Empty;

            // Remark
            string tRemark = string.Empty;
            //string XMLMessage = string.Empty;

            try
            {
                //doc.LoadXml(XMLString);
                //FileStream fs = new FileStream("msxml2.domdocument", FileMode.Open, FileAccess.Read);
                //.Load(doc);
                xmldoc.LoadXml(XMLString);
                if (!bOK)
                {
                    //WorkOrderString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                    //WorkOrderString += "<workorderresponse accept=\"NO\">";
                    //WorkOrderString += "<errorgroup>";
                    //WorkOrderString += "<error>\"XML format incorrect - please correct and resubmit\"</error>";
                    //WorkOrderString += "</errorgroup></workorderresponse>";


                    // finally free up components.
                    WOD = null;

                    // log XML. Don't log if from web service active check module.
                    if (bLogXML) InsertXML(XMLString);

                    return WorkOrderString;
                }

                // get root element <workorder>
                //<workorder shop="098" customer="MAER" location="NWK" type="W" Date=""> 

                XmlNode WOxmlNode = xmldoc.SelectSingleNode("workorder"); //@Fixbysoumik
                tShop = WOxmlNode.Attributes.GetNamedItem("shop").Value;
                tCustomer = WOxmlNode.Attributes.GetNamedItem("customer").Value;
                tLocation = WOxmlNode.Attributes.GetNamedItem("location").Value;
                tType = WOxmlNode.Attributes.GetNamedItem("type").Value;
                tDate = WOxmlNode.Attributes.GetNamedItem("date").Value;
                try
                {
                    tVendorRef = WOxmlNode.Attributes.GetNamedItem("vendorref").Value; //@Fixbysoumik
                }
                catch
                {
                    tVendorRef = "";
                }

            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                bOK = false;
            }

            if (bOK)
            {
                try
                {
                    XmlNodeList EquipxmlNode = xmldoc.SelectNodes("workorder/equipment"); //@Fixbysoumik
                    //xmlnode = xmldoc.GetElementsByTagName("equipment");
                    for (i = 0; i < EquipxmlNode.Count; i++)
                    {
                        tNumber = EquipxmlNode[i].Attributes.GetNamedItem("number").Value;
                        tMode = EquipxmlNode[i].Attributes.GetNamedItem("mode").Value;
                        tCause = EquipxmlNode[i].Attributes.GetNamedItem("cause").Value;
                    }
                }
                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    bOK = false;
                }
            }

            if (bOK)
            {
                try
                {
                    XmlNodeList TotalHoursxmlNode = xmldoc.SelectNodes("workorder/totalhours"); //@Fixbysoumik
                    //xmlnode = xmldoc.GetElementsByTagName("totalhours");
                    for (i = 0; i < TotalHoursxmlNode.Count; i++)
                    {
                        tStraighttime = TotalHoursxmlNode[i].Attributes.GetNamedItem("straighttime").Value;
                        tOvertime = TotalHoursxmlNode[i].Attributes.GetNamedItem("overtime").Value; ;
                        tDoubletime = TotalHoursxmlNode[i].Attributes.GetNamedItem("doubletime").Value; ;
                        tMiscellaneoustime = TotalHoursxmlNode[i].Attributes.GetNamedItem("miscellaneoustime").Value;
                    }
                }

                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    bOK = false;
                }
            }

            if (bOK)
            {
                try
                {
                    XmlNodeList CostxmlNode = xmldoc.SelectNodes("workorder/cost"); //@Fixbysoumik
                    //xmlnode = xmldoc.GetElementsByTagName("cost");
                    for (i = 0; i < CostxmlNode.Count; i++)
                    {
                        tTotalCost = CostxmlNode[i].Attributes.GetNamedItem("total").Value;
                    }
                }

                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    bOK = false;
                }
            }

            if (bOK)
            {
                //WorkOrderString = tCustomer; WorkOrderString += "~";
                //WorkOrderString += tVendorRef; WorkOrderString += "~";
                //WorkOrderString += tShop; WorkOrderString += "~";
                //WorkOrderString += tDate; WorkOrderString += "~";
                //WorkOrderString += tNumber; WorkOrderString += "~";
                //WorkOrderString += tMode; WorkOrderString += "~";
                //WorkOrderString += tCause; WorkOrderString += "~";
                //WorkOrderString += tLocation; WorkOrderString += "~";
                //WorkOrderString += tType; WorkOrderString += "~";

                //WorkOrderString += tStraighttime; WorkOrderString += "~";
                //WorkOrderString += tDoubletime; WorkOrderString += "~";
                //WorkOrderString += tOvertime; WorkOrderString += "~";
                //WorkOrderString += tMiscellaneoustime; WorkOrderString += "~";
                //WorkOrderString += tTotalCost; WorkOrderString += "~";

                // setup estimate and get RKEM data. if no parse errors and equipemnt and shop received
                if ((bOK) && (!string.IsNullOrEmpty(tNumber)) && (!string.IsNullOrEmpty(tShop)))
                {
                    WOD.Shop = new Shop();
                    WOD.Shop.Customer = new List<Customer>();//@Soumik, shop will hav multiple customer
                    WOD.EquipmentList = new List<Equipment>();
                    WOD.Shop.Customer.Insert(0, new Customer());     //@Fixbysoumik
                    WOD.Shop.Customer[0].CustomerCode = tCustomer;   //@Fixbysoumik
                    WOD.EquipmentList.Insert(0, new Equipment());    //@Fixbysoumik
                    WOD.EquipmentList[0].VendorRefNo = tVendorRef;
                    WOD.Shop.ShopCode = tShop;
                    WOD.IsSingle = true;

                    //WOD.ChangeTime = Convert.ToDateTime(tDate);      
                    try
                    {
                        //@Fixbysoumik
                        WOD.ChangeTime = DateTime.ParseExact(tDate, "ddmmyyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (FormatException ex)
                    {
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        bOK = false;
                    }

                    WOD.EquipmentList[0].EquipmentNo = tNumber;  //@Fixbysoumik
                    WOD.EquipmentList[0].SelectedMode = tMode;
                    WOD.Mode = tMode;
                    WOD.Cause = tCause;
                    WOD.Shop.LocationCode = tLocation;
                    WOD.WorkOrderType = tType;
                    if (tStraighttime != "")
                        WOD.TotalManHourReg = Convert.ToDouble(tStraighttime);
                    if (tDoubletime != "")
                        WOD.TotalManHourDoubleTime = Convert.ToDouble(tDoubletime);
                    if (tOvertime != "")
                        WOD.TotalManHourOverTime = Convert.ToDouble(tOvertime);
                    if (tMiscellaneoustime != "")
                        WOD.TotalManHourMisc = Convert.ToDouble(tMiscellaneoustime);
                    if (tTotalCost != "")
                        WOD.TotalCostOfRepair = Convert.ToDecimal(tTotalCost);
                    WOD.ChangeUser = "Merc+WebService";
                    //pWO->SetWorkOrder(tWork);
                    //pWO->SetUser("Merc+WebService");
                }
            }

            if (bOK)
            {
                try
                {
                    XmlNodeList RepairgrpxmlNode = xmldoc.SelectNodes("workorder/repairgroup"); //@Fixbysoumik
                    //xmlnode = xmldoc.GetElementsByTagName("repairgroup");
                    for (i = 0; i < RepairgrpxmlNode.Count; i++) //@Fixbysoumik
                    {
                        //how to get the child nodes
                        XmlNodeList RepairChildNodeList = RepairgrpxmlNode[i].ChildNodes;
                        for (j = 0; j < RepairChildNodeList.Count; j++)
                        {
                            tDamageCode = "";
                            tCode = "";
                            tRepairLocCode = "";
                            tQty = "";
                            tCost = "";
                            tManHours = "";
                            tTpi = "";

                            tDamageCode = RepairChildNodeList[j].Attributes.GetNamedItem("damagecode").Value;
                            tCode = RepairChildNodeList[j].Attributes.GetNamedItem("code").Value; ;
                            tRepairLocCode = RepairChildNodeList[j].Attributes.GetNamedItem("repairlocation").Value; ;
                            tQty = RepairChildNodeList[j].Attributes.GetNamedItem("quantity").Value;
                            tCost = RepairChildNodeList[j].Attributes.GetNamedItem("cost").Value; ;
                            tManHours = RepairChildNodeList[j].Attributes.GetNamedItem("manhours").Value;
                            tTpi = RepairChildNodeList[j].Attributes.GetNamedItem("tpi").Value;

                            // here AddRepair cost.
                            //WorkOrderString = tDamageCode; WorkOrderString += "~";
                            //WorkOrderString += tCode; WorkOrderString += "~";
                            //WorkOrderString += tRepairLocCode; WorkOrderString += "~";
                            //WorkOrderString += tQty; WorkOrderString += "~";
                            //WorkOrderString += tCost; WorkOrderString += "~";
                            //WorkOrderString += tManHours; WorkOrderString += "~";
                            //WorkOrderString += tTpi; WorkOrderString += "~";

                            if (bOK)
                            {
                                //WOD.RepairsViewList = new List<RepairsView>();
                                RepairsView RV = new RepairsView();
                                RV.RepairLocationCode = new RepairLoc();
                                RV.RepairCode = new RepairCode();
                                RV.Tpi = new Tpi();
                                RV.Damage = new Damage();
                                RV.Damage.DamageCedexCode = tDamageCode;
                                RV.RepairCode.RepairCod = tCode;
                                RV.RepairLocationCode.CedexCode = tRepairLocCode;
                                RV.Pieces = Convert.ToInt32(tQty);
                                RV.MaterialCost = Convert.ToDecimal(tCost);
                                RV.ManHoursPerPiece = Convert.ToDouble(tManHours);
                                RV.Tpi.CedexCode = tTpi;
                                WOD.RepairsViewList.Add(RV);
                            }

                            XmlNodeList PartChildNodeList = RepairChildNodeList[j].ChildNodes;
                            for (int p = 0; p < PartChildNodeList.Count; p++)
                            {
                                // get part attributes
                                tPartCode = "";
                                tPartQty = "";

                                tPartCode = PartChildNodeList[p].Attributes.GetNamedItem("number").Value;
                                tPartQty = PartChildNodeList[p].Attributes.GetNamedItem("quantity").Value;

                                // add spare part etc... Use tCode to fill in the repair code
                                WorkOrderString = tCode; WorkOrderString += "~";
                                WorkOrderString += tPartQty; WorkOrderString += "~";
                                WorkOrderString += tPartCode; WorkOrderString += "~";
                                if (bOK)
                                {
                                    SparePartsView SPV = new SparePartsView();
                                    SPV.RepairCode = new RepairCode();
                                    SPV.OwnerSuppliedPartsNumber = tPartCode; ;
                                    string t = string.Empty;
                                    for (i = 0; i < tPartQty.Length; i++)
                                    {
                                        var c = tPartQty[i];
                                        if (c != '.')
                                        {
                                            t += c;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(tPartQty))
                                    {
                                        //string tempInt = 
                                        SPV.Pieces = Convert.ToInt32(t);
                                    }
                                    SPV.RepairCode.RepairCod = tCode;
                                    WOD.SparePartsViewList.Add(SPV);
                                }
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    bOK = false;
                }
            }

            if (bOK)
            {
                try
                {
                    XmlNodeList RemarkxmlNode = xmldoc.SelectNodes("workorder/remark"); //@Fixbysoumik
                    //xmlnode = xmldoc.GetElementsByTagName("remark");
                    for (i = 0; i < RemarkxmlNode.Count; i++)
                    {
                        tRemark = RemarkxmlNode[i].Attributes.GetNamedItem("text").Value;
                    }
                }

                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    bOK = false;
                }
            }

            if (bOK && !string.IsNullOrEmpty(tRemark))
            {
                WOD.RemarksList = new List<RemarkEntry>();
                RemarkEntry RE = new RemarkEntry();
                RE.Remark = tRemark;
                WOD.RemarksList.Add(RE);
            }

            if (bOK)
            {
                try
                {
                    success = WorkOrderClient.CallValidateMethod(ref WOD, WOD.EquipmentList[0], out ErrorMessageList); //@Fixbysoumik
                    //ErrorMessageList = WorkOrderClient.Review(ref WOD, WOD.EquipmentList, false).ToList(); //@Fixbysoumik
                    if (success && ErrorMessageList.Count == 0)
                    {
                        success = WorkOrderClient.CallSaveMethod(WOD, out ErrorMessageList); //@Fixbysoumik
                    }

                    if (success)
                    {
                        WorkOrderString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                        WorkOrderString += "<workorderresponse accept=\"YES\">";
                        WorkOrderString += "<errorgroup><error></error></errorgroup></workorderresponse>";
                    }
                    else
                    {
                        WorkOrderString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

                        WorkOrderString += "<workorderresponse accept=\"NO\">";
                        WorkOrderString += "<errorgroup>";
                        foreach (var err in ErrorMessageList)
                        {
                            WorkOrderString += "<error>\"";
                            WorkOrderString += err.Message;
                            WorkOrderString += "\"</error>";
                        }
                        WorkOrderString += "</errorgroup></workorderresponse>";
                    }
                }

                catch (Exception ex)
                {
                    WorkOrderString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                    WorkOrderString += "<workorderresponse accept=\"NO\">";
                    WorkOrderString += "<errorgroup>";
                    WorkOrderString += "<error>\"General system error - Unable to validate estimate.\"</error>";
                    WorkOrderString += "</errorgroup></workorderresponse>";
                }
            }
            else // failed parse - send invalid XML message.
            {
                WorkOrderString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                WorkOrderString += "<workorderresponse accept=\"NO\">";
                WorkOrderString += "<errorgroup>";
                WorkOrderString += "<error>\"Missing required field(s) - correct and resubmit\"</error>";
                WorkOrderString += "</errorgroup></workorderresponse>";
            }

            // and send return XML
            //Send(pECB, (char*)tWork);

            // if error log XML
            if (!bOK)
            {
                InsertXML(XMLString);
            }

            return WorkOrderString;
        }

        private void InsertXML(string XMLMessage)
        {
            string RepairedXMLMsg = string.Empty;
            MESC1TS_XML_LOG XMLLog = new MESC1TS_XML_LOG();

            try
            {
                RepairedXMLMsg = RepairText(XMLMessage);
                XMLLog.CRTS = DateTime.Now;
                XMLLog.XML_TEXT = RepairedXMLMsg;
                objContext.MESC1TS_XML_LOG.Add(XMLLog);
                objContext.SaveChanges();
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        private string RepairText(string MSG)
        {
            string newstr = "";
            char ctest;

            // fix apostrophe for SQL server.
            for (int i = 0; i < MSG.Length; i++)
            {
                ctest = MSG[i];
                if (ctest == '\'') newstr += '\'';
                newstr += ctest;
            }

            return (newstr);
        }
    }
}
