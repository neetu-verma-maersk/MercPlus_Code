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
//using System.Data.Entity.Validation;
using System.Threading;
using System.Configuration;

using System.IO;//Kasturee_Geo_10_06_19_ITSOE CSI # 1448 


namespace MercGEOFeed
{
    public partial class MercGEOFeedService : ServiceBase
    {
        #region Declare variables

        private static MESC2DSEntities objContext = new MESC2DSEntities();
        public const int BUFFERSIZE = 65536;
        public const int WAITTIME = 20;

        const int TAGOFF = 0;
        const int TAGLEN = 6;
        const int TRANSOFF = 6;
        const int TRANSLEN = 1;
        const int GEOIDOFF = 7;
        const int GEOIDLEN = 13;

        // TAGA - country record.
        // This is the country code in GEO.
        const int TAGARKSTCODEOFF = 119;
        const int TAGARKSTCODELEN = 8;

        // TAGA - COUNTRY RECORD
        const int TAGACOUNTRYCODEOFF = 30;
        const int TAGACOUNTRYCODELEN = 2;
        const int TAGAUPPERCASENAMEOFF = 78;
        const int TAGAUPPERCASENAMELEN = 30;

        // TAGC  - LOCATION
        // description
        const int TAGCUPPERCASENAMEOFF = 88;
        const int TAGCUPPERCASENAMELEN = 30;
        // location code
        const int TAGCRKSTCODEOFF = 35;
        const int TAGCRKSTCODELEN = 8;
        // RKRPLocation code 
        const int TAGCRKCCLOCATIONCODEOFF = 48;
        const int TAGCRKCCLOCATIONCODELEN = 5;
        // Country code offset
        const int TAGCCOUNTRYCODEOFF = 293;
        const int TAGCCOUNTRYCODELEN = 2;
        // TAGD  - LOCATION
        // description
        const int TAGDUPPERCASENAMEOFF = 88;
        const int TAGDUPPERCASENAMELEN = 30;
        // location code
        const int TAGDRKSTCODEOFF = 35;
        const int TAGDRKSTCODELEN = 8;
        // RKRPLocation code 
        const int TAGDRKCCLOCATIONCODEOFF = 48;
        const int TAGDRKCCLOCATIONCODELEN = 5;
        // Country code offset
        const int TAGDCOUNTRYCODEOFF = 351;
        const int TAGDCOUNTRYCODELEN = 2;
        const int LOCDESCLEN = 30;
        // TAGE  - Area Code
        // description
        const int TAGENAMEOFF = 20;
        const int TAGENAMELEN = 20; // len = 40, but db max=20
        // Area Code
        const int TAGECODEOFF = 60;
        const int TAGECODELEN = 3;
        // BDA_TYPE_CODE
        const int TAGEBDATYPEOFF = 70;
        const int TAGEBDATYPELEN = 10;
        // TAGX3  - Area connector
        const int TAGX3TYPECODEOFF = 60;
        const int TAGX3TYPECODELEN = 10;
        const int TAGX3COUNTRYIDOFF = 90;
        const int TAGX3COUNTRYIDLEN = 13;
        const int AREACODELEN = 3;

        //Country table fields
        public static string m_CountryCode;
        public static string m_CountryDescription;

        // Location table fields.
        public static string m_LocationCode;
        public static string m_LocationDescription;
        public static string m_RKCCLocation;

        // Area fields and connector fields
        public static string m_AreaCode;
        public static string m_AreaDescription;
        // Additional Area Connector Field
        public static string m_CountryGeoId;
        public static string m_AreaTypeCode;
        public static string m_GeoId;

        // public string RecordBuffer;
        public static string Record;
        public static string TransType;

        #endregion Declare variables
        private AutoResetEvent AutoEventInstance { get; set; }
        private MercGEOFeedService TimerInstance { get; set; }
        private Timer StateTimer { get; set; }
        public int TimerInterval { get; set; }
        Thread Worker;
        AutoResetEvent StopRequest = new AutoResetEvent(false);
        public const string MQ_READ_MODE = "R";
        public static LogEntry logEntry = new LogEntry();

        public MercGEOFeedService()
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
                    logEntry.Message = "Merc GEO Feed Service started";
                    Logger.Write(logEntry);
                    MercGEOFeedService MercGeo = new MercGEOFeedService();
                    MercGeo.ProcessQueue();
                   
                }
                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }

            

            autoEvent.Set();
        }

        public void ProcessQueue()
        {
            MQManager MQmgr = new MQManager();
            MQQueueManager Qmgr = null;
            MQQueue Qqueue = null;
            MQMessage MQMessage = null;

            Qmgr = MQmgr.OpenQueueManager(ConfigurationManager.AppSettings["MQManagerQueueName"]);
            Qqueue = MQmgr.OpenQ(MQ_READ_MODE, Qmgr, ConfigurationManager.AppSettings["MQManagerRequestName"]);

            bool blnContinue = true;
            while (blnContinue)
            {
                try
                {
                    MQMessage = new MQMessage();
                    MQmgr.GetMessage(Qqueue, ref MQMessage, false);
                    if (MQMessage.Format.CompareTo(MQC.MQFMT_STRING) == 0)
                    {

                        string GEOMQMessageFile = null;//Kasturee_Geo_10_06_19_ITSOE CSI # 1448 
                        GEOMQMessageFile = ConfigurationManager.AppSettings["GEOMQMessageFile"];
                        string MQmessage_Log = MQMessage.ReadString(MQMessage.MessageLength);
                        DateTime CurrentDateTime = DateTime.Now;
                       
                        GEOMQMessageFile += CurrentDateTime.ToString("dd-MM-yyyy");
                        GEOMQMessageFile += ".TXT";

                        bool Exists = System.IO.File.Exists(GEOMQMessageFile);
                        if (!Exists)
                        {
                            using (FileStream GeoLines = File.Open(GEOMQMessageFile, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                using (System.IO.StreamWriter WOfile = new System.IO.StreamWriter(GeoLines))
                                {
                                    WOfile.WriteLine(MQmessage_Log);

                                    WOfile.Close();
                                }
                                GeoLines.Close();
                            }
                        }
                        else
                        {
                            using (FileStream GeoLines = File.Open(GEOMQMessageFile, FileMode.Append, FileAccess.Write, FileShare.None))
                            {
                                using (System.IO.StreamWriter WOfile = new System.IO.StreamWriter(GeoLines))
                                {
                                    WOfile.WriteLine(MQmessage_Log);

                                    WOfile.Close();
                                }
                                GeoLines.Close();
                            }
                        }
                        //Kasturee_Geo_end
                        ParseAndProcessRecords(MQmessage_Log);
                    }
                    else
                    {
                        logEntry.Message = "Else part of MQMessage.Format.CompareTo(MQC.MQFMT_STRING)";//Kasturee_GEO_Log
                        Logger.Write(logEntry);
                        // Error Message - The MQ message is a Non-text message + mqMessage.MessageSequenceNumber.ToString()
                    }
                }
                catch (MQException mqException)
                {
                    if (mqException.Reason != MQC.MQRC_NO_MSG_AVAILABLE)
                    {
                        //Write log - A problem occured while retrieving the MQ message: " + mqException.Message.ToString()  
                        logEntry.Message = mqException.ToString();
                        Logger.Write(logEntry);
                    }
                    blnContinue = false;
                }

            }

            MQmgr.CloseQ(Qqueue);
            MQmgr.DisconnectQueueManager(Qmgr);
        }

        public static void ParseAndProcessRecords(string Message)
        {
            string szRecord;
            //string Record;
            int idx;
            try
            {
                int size = Message.Length;
                idx = Message.IndexOf("TAG");
                szRecord = Message.Substring(idx, 6);
                bool result;
                Record = Message.Substring(idx, (size - idx));
                while (idx > 0)
                {
                    result = IsTag(szRecord);
                    if (result)
                    {

                        try
                        {
                            ProcessRecord(Record);
                        }
                        catch (Exception ex)
                        {
                            //string ErrorMsg = "MercGEO: Error trapped in ParseAndProcessRecords routine.";
                            logEntry.Message = ex.ToString();
                            Logger.Write(logEntry);
                        }
                    }
                    else
                    {
                        idx += 1;
                    }
                    //int Newidx = idx + 6;
                    int Newsize = Record.Length;
                    string newRecord = Record.Substring(6, (Newsize - 6));    //exception from here plz check
                    idx = newRecord.IndexOf("TAG");
                    if (idx > 0)
                    {
                        size = newRecord.Length;
                        szRecord = newRecord.Substring(idx, 6);
                        Record = newRecord.Substring(idx, (size - idx));
                        result = IsTag(szRecord);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return;
        }

        public static void ProcessRecord(string Record)
        {
            bool IsEmpty = true;
            //bool bTagA = false;
            string Tag = LoadFromText(Record);
            if (Tag == "NOTAG")
            {
                return;
            }
            try
            {
                //if (Tag == "TAGA")
                //{
                //    bTagA = true;
                //}
                if (Tag != "TAGX3")
                {
                    if (Tag != "TAGE")
                    {
                        if (Tag == "TAGA")
                        {
                            List<Country> CountryList = GetSelectCountrySQL();
                            if (CountryList.Count > 0)
                            {
                                IsEmpty = false;                               
                            }

                        }
                        else
                        {
                            List<Location> LocationList = GetSelectLocationSQL();
                            if (LocationList.Count > 0)
                            {
                                IsEmpty = false;                               
                            }
                        }
                    }
                    else
                    {
                        List<Area> AreaList = GetSelectAreaSQL();
                        if (AreaList.Count > 0)
                        {
                            IsEmpty = false;
                        }
                    }
                }

                if (TransType == "GEODELETE")
                {
                    if (!IsEmpty)
                    {
                        if (Tag == "TAGA")
                        {
                            GetUpdateLocationForCountryDelete();
                            GetDeleteSQLTAGA(Tag);
                        }
                        if ((Tag == "TAGD") || (Tag == "TAGC"))
                        {

                            DeactivateShops(m_LocationCode);
                            GetDeleteSQLTAGA(Tag);
                        }
                    }
                }
                if (TransType == "GEOINSERT")
                {
                    if (IsEmpty)
                    {
                        if (Tag == "TAGX3")
                        {
                            if (ExistCountry())
                            {
                                if (FindAreaCode())
                                {
                                    GetAssignAreaSQL();
                                }
                            }
                        }
                        else
                        {
                            GetInsertSQL(Tag);
                        }
                    }
                    else
                    {
                        GetUpdateSQL(Tag);
                    }

                }


                if (TransType == "GEOUPDATE")
                {
                    if (IsEmpty)
                    {
                        if (Tag == "TAGX3")
                        {
                            if (ExistCountry())
                            {

                                if (FindAreaCode())
                                {
                                    GetAssignAreaSQL();
                                }
                            }
                        }
                        else
                        {

                            GetInsertSQL(Tag);
                        }
                    }
                    else
                    {

                        GetUpdateSQL(Tag);
                    }
                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }


        public static string LoadFromText(string Record)
        {
            //int TransType;
            string Tag = ExtractTag(Record);
            try
            {
                char TransactionType = Convert.ToChar(Record.Substring(TRANSOFF, TRANSLEN));
                switch (TransactionType)
                {
                    case 'I': TransType = "GEOINSERT";
                        break;
                    case 'U': TransType = "GEOUPDATE";
                        break;
                    case 'D': TransType = "GEODELETE";
                        break;
                    case 'F': TransType = "GEOUPDATE";
                        break;
                }
                switch (Tag)
                {
                    case "TAGA":
                        m_CountryCode = Record.Substring(TAGACOUNTRYCODEOFF, TAGACOUNTRYCODELEN);
                        m_CountryDescription = Record.Substring(TAGAUPPERCASENAMEOFF, TAGAUPPERCASENAMELEN);
                        m_GeoId = Record.Substring(GEOIDOFF, GEOIDLEN);
                        break;
                    case "TAGC":
                        m_LocationCode = Record.Substring(TAGCRKSTCODEOFF, TAGCRKSTCODELEN);
                        m_LocationDescription = Record.Substring(TAGCUPPERCASENAMEOFF, TAGCUPPERCASENAMELEN);
                        m_RKCCLocation = Record.Substring(TAGCRKCCLOCATIONCODEOFF, TAGCRKCCLOCATIONCODELEN);
                        m_CountryCode = Record.Substring(TAGCCOUNTRYCODEOFF, TAGCCOUNTRYCODELEN);
                        m_GeoId = Record.Substring(GEOIDOFF, GEOIDLEN);
                        break;
                    case "TAGD":
                        m_LocationCode = Record.Substring(TAGDRKSTCODEOFF, TAGDRKSTCODELEN);
                        m_LocationDescription = Record.Substring(TAGDUPPERCASENAMEOFF, TAGDUPPERCASENAMELEN);
                        m_RKCCLocation = Record.Substring(TAGDRKCCLOCATIONCODEOFF, TAGDRKCCLOCATIONCODELEN);
                        m_CountryCode = Record.Substring(TAGDCOUNTRYCODEOFF, TAGDCOUNTRYCODELEN);
                        m_GeoId = Record.Substring(GEOIDOFF, GEOIDLEN);
                        break;
                    case "TAGE":
                        m_AreaCode = Record.Substring(TAGECODEOFF, TAGECODELEN);
                        m_AreaDescription = Record.Substring(TAGENAMEOFF, TAGENAMELEN);
                        m_AreaTypeCode = Record.Substring(TAGEBDATYPEOFF, TAGEBDATYPELEN);
                        m_GeoId = Record.Substring(GEOIDOFF, GEOIDLEN);
                        break;
                    case "TAGX3":
                        m_GeoId = Record.Substring(GEOIDOFF, GEOIDLEN);
                        m_AreaTypeCode = Record.Substring(TAGX3TYPECODEOFF, TAGX3TYPECODELEN);
                        m_CountryGeoId = Record.Substring(TAGX3COUNTRYIDOFF, TAGX3COUNTRYIDLEN);
                        break;

                    case "NOTAG":
                        break;


                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Tag;
        }

        public static string ExtractTag(string Record)
        {
            string sArea;
            string Tag = null;
            try
            {
                string sTag = Record.Substring(TAGOFF, TAGLEN);
                if (sTag.ToUpper() == "TAGA  ")
                {
                    Tag = "TAGA";
                }
                else if (sTag.ToUpper() == "TAGC  ")
                {
                    Tag = "TAGC";
                }
                else if (sTag.ToUpper() == "TAGD  ")
                {
                    Tag = "TAGD";
                }
                else if (sTag.ToUpper() == "TAGE  ")
                {
                    sArea = Record.Substring(TAGEBDATYPEOFF, TAGEBDATYPELEN);
                    string Area = sArea.Substring(0, 4);
                    if (Area.Equals("AREA"))
                    {
                        Tag = "TAGE";
                    }
                    else
                    {
                        Tag = "NOTAG";
                    }
                }
                else if (sTag.ToUpper() == "TAGX3 ")
                {
                    sArea = Record.Substring(TAGX3TYPECODEOFF, TAGX3TYPECODELEN);
                    string Area = sArea.Substring(0, 4);
                    if (Area.Equals("AREA", StringComparison.OrdinalIgnoreCase))
                    {
                        Tag = "TAGX3";
                    }
                    else
                    {
                        Tag = "NOTAG";
                    }
                }
                else
                {
                    Tag = "NOTAG";

                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Tag;
        }

        public static bool IsTag(string sRecord)
        {
            if (sRecord.Equals("TAGA  "))
                return true;
            if (sRecord.Equals("TAGC  "))
                return true;
            if (sRecord.Equals("TAGD  "))
                return true;
            if (sRecord.Equals("TAGE  "))
                return true;
            if (sRecord.Equals("TAGX3 "))
                return true;

            return false;
        }

        public static bool ExistCountry()
        {
            
            int Qty = GetCountryByGeoIdSQL();
            if (Qty > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool FindAreaCode()
        {
            string Area;
            List<Area> AreaList = GetAreaCodeByGeoIdSQL();
            foreach (var item in AreaList)
            {
                Area = item.AreaCode;
            }
            if (AreaList.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<Country> GetSelectCountrySQL()
        {
            List<Country> CountryList = new List<Country>();
            var CountryDBObject = (from country in objContext.MESC1TS_COUNTRY
                                   where country.COUNTRY_CD == m_CountryCode.Trim()
                                   select country).ToList();
            try
            {
                if (CountryDBObject.Count > 0)
                {
                    for (int count = 0; count < CountryDBObject.Count; count++)
                    {
                        Country country = new Country();
                        country.CountryCode = CountryDBObject[count].COUNTRY_CD;
                        country.CountryDescription = CountryDBObject[count].COUNTRY_DESC;
                        country.CountryGeoID = CountryDBObject[count].COUNTRY_GEO_ID;
                        country.AreaCode = CountryDBObject[count].AREA_CD;
                        CountryList.Add(country);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return CountryList;
        }

        public static List<Location> GetSelectLocationSQL()
        {
            List<Location> LocationList = new List<Location>();
            var LocationDBObject = (from loc in objContext.MESC1TS_LOCATION
                                    where loc.LOC_CD == m_LocationCode.Trim()
                                    select loc).ToList();
            try
            {
                if (LocationDBObject.Count > 0)
                {
                    for (int count = 0; count < LocationDBObject.Count; count++)
                    {
                        Location loc = new Location();
                        loc.LocCode = LocationDBObject[count].LOC_CD;
                        loc.LocDesc = LocationDBObject[count].LOC_DESC;
                        loc.CountryCode = LocationDBObject[count].COUNTRY_CD;
                        loc.RegionCode = LocationDBObject[count].REGION_CD;
                        loc.ContactEqsalSW = LocationDBObject[count].CONTACT_EQSAL_SW;
                        loc.RkrpLoc = LocationDBObject[count].RKRPLOC;
                        LocationList.Add(loc);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return LocationList;
        }

        public static int GetCountryByGeoIdSQL()
        {
            int Qty = 0;

            List<Country> CountryListByGEO = new List<Country>();
            var CountryDBObject = (from country in objContext.MESC1TS_COUNTRY
                                   where country.COUNTRY_GEO_ID == m_CountryGeoId.Trim()
                                   select country).ToList();
            try
            {
                if (CountryDBObject.Count > 0)
                {
                    for (int count = 0; count < CountryDBObject.Count; count++)
                    {

                        Country country = new Country();
                        country.CountryCode = CountryDBObject[count].COUNTRY_CD;
                        country.CountryDescription = CountryDBObject[count].COUNTRY_DESC;
                        country.CountryGeoID = CountryDBObject[count].COUNTRY_GEO_ID;
                        country.AreaCode = CountryDBObject[count].AREA_CD;
                        CountryListByGEO.Add(country);
                        Qty++;
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Qty;
        }

        public static List<Area> GetSelectAreaSQL()
        {
            List<Area> AreaList = new List<Area>();
            var AreaDBObject = (from area in objContext.MESC1TS_AREA
                                where area.AREA_CD == m_AreaCode
                                select area).ToList();
            try
            {
                if (AreaDBObject.Count > 0)
                {
                    for (int count = 0; count < AreaDBObject.Count; count++)
                    {
                        Area area = new Area();
                        area.AreaCode = AreaDBObject[count].AREA_CD;
                        area.AreaDescription = AreaDBObject[count].AREA_DESC;
                        area.AreaGeoID = AreaDBObject[count].AREA_GEO_ID;
                        AreaList.Add(area);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return AreaList;
        }
        public static List<Area> GetAreaCodeByGeoIdSQL()
        {
            List<Area> AreaList = new List<Area>();
            var AreaDBObject = (from area in objContext.MESC1TS_AREA
                                where area.AREA_GEO_ID == m_GeoId
                                select area).ToList();
            try
            {
                if (AreaDBObject.Count > 0)
                {
                    foreach (var item in AreaDBObject)
                    {
                        Area area = new Area();
                        area.AreaCode = item.AREA_CD;
                        AreaList.Add(area);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return AreaList;
        }

        public static void GetUpdateLocationForCountryDelete()
        {
            List<Location> Loclist = new List<Location>();
            var LocationListFromDB = (from loc in objContext.MESC1TS_LOCATION
                                      where loc.COUNTRY_CD.Trim() == m_CountryCode.Trim()
                                      select loc).ToList();
            if (LocationListFromDB.Count > 0)
            {
                LocationListFromDB[0].COUNTRY_CD = null;
                LocationListFromDB[0].CHUSER = "MercGEO Feed";
                LocationListFromDB[0].CHTS = DateTime.Now;

                try
                {
                    objContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }
            }
        }

        public static void GetDeleteSQLTAGA(string Tag)
        {

            if (Tag == "TAGA")
            {
                List<Location> Loclist = new List<Location>();
                var LocationListFromDB = (from loc in objContext.MESC1TS_LOCATION
                                          where loc.COUNTRY_CD.Trim() == m_CountryCode.Trim()
                                          select loc).ToList();
                if (LocationListFromDB.Count > 0)
                {
                    LocationListFromDB[0].COUNTRY_CD = null;
                    LocationListFromDB[0].CHUSER = "MercGEO Feed";
                    LocationListFromDB[0].CHTS = DateTime.Now;

                    try
                    {
                        objContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                    }
                }

                var ConDBObject = (from con in objContext.MESC1TS_COUNTRY
                                   where con.COUNTRY_CD == m_CountryCode.Trim()
                                   select con).ToList();
                try
                {
                    if (ConDBObject.Count > 0)
                    {
                        objContext.MESC1TS_COUNTRY.Remove(ConDBObject.First());
                        objContext.SaveChanges();

                    }
                }
                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }
            }
            else
            {

                var LocDBObject = (from loc in objContext.MESC1TS_LOCATION
                                   where loc.LOC_CD == m_LocationCode.Trim()
                                   select loc).ToList();
                try
                {
                    if (LocDBObject.Count > 0)
                    {
                        objContext.MESC1TS_LOCATION.Remove(LocDBObject.First());
                        objContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }
            }
        }

        public static void DeactivateShops(string LocationCode)
        {
            bool success;
            List<Shop> ShopList = new List<Shop>();
            var ShopDBObject = (from shop in objContext.MESC1TS_SHOP
                                where shop.LOC_CD == LocationCode &&
                                shop.SHOP_ACTIVE_SW == "Y"
                                select shop).ToList();
            if (ShopDBObject.Count > 0)
            {
                for (int count = 0; ShopDBObject.Count > 0; count++)
                {
                    Shop shop = new Shop();
                    shop.ShopCode = ShopDBObject[count].SHOP_CD;
                    ShopList.Add(shop);
                }
            }
            var ShopListDBObject = (from s in objContext.MESC1TS_SHOP
                                    select s).ToList();

            var ShopUpdate = ShopListDBObject.FindAll(Sl => ShopList.Any(Sid => Sid.ShopCode == Sl.SHOP_CD));
            if (ShopUpdate.Count > 0)
            {
                for (int count = 0; ShopUpdate.Count > 0; count++)
                {
                    ShopUpdate[count].SHOP_ACTIVE_SW = "N";
                    ShopUpdate[count].LOC_CD = null;
                    ShopUpdate[count].CHUSER = "MercGEO Batch";
                    ShopUpdate[count].CHTS = DateTime.Now;
                }
                try
                {
                    objContext.SaveChanges();
                    success = true;

                }
                catch (Exception ex)
                {
                    success = false;
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }

                if (success)
                {
                    MESC1TS_REFAUDIT auditToBeInserted = new MESC1TS_REFAUDIT();

                    auditToBeInserted.UNIQUE_ID = LocationCode;
                    auditToBeInserted.TAB_NAME = "MESC1TS_SHOP";
                    auditToBeInserted.NEW_VALUE = "N";
                    auditToBeInserted.OLD_VALUE = "Y";
                    auditToBeInserted.COL_NAME = "SHOP_ACTIVE_SW";
                    auditToBeInserted.CHTS = DateTime.Now;
                    auditToBeInserted.CHUSER = "MercGEO Batch Process";
                    try
                    {
                        objContext.MESC1TS_REFAUDIT.Add(auditToBeInserted);
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

        public static void GetAssignAreaSQL()
        {
            var AreaDBObject = (from con in objContext.MESC1TS_COUNTRY
                                where con.COUNTRY_GEO_ID == m_CountryGeoId
                                select con).ToList();
            if (AreaDBObject.Count > 0)
            {
                AreaDBObject[0].AREA_CD = m_AreaCode;
                try
                {
                    objContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }
            }
        }

        public static void GetInsertSQL(string Tag)
        {

            if (Tag == "TAGA")
            {
                 var CountryDBObject = (from con in objContext.MESC1TS_COUNTRY
                                       where con.COUNTRY_CD == m_CountryCode
                                       select con).ToList();
                 if (CountryDBObject.Count > 0)
                 {
                     GetUpdateSQL(Tag);
                 }
                 else
                 {
                     MESC1TS_COUNTRY CountryToBeInserted = new MESC1TS_COUNTRY();
                     CountryToBeInserted.COUNTRY_CD = m_CountryCode.Trim();
                     CountryToBeInserted.COUNTRY_DESC = m_CountryDescription.Trim();
                     CountryToBeInserted.AREA_CD = null;
                     CountryToBeInserted.REPAIR_LIMIT_ADJ_FACTOR = 100.00;
                     CountryToBeInserted.COUNTRY_GEO_ID = m_GeoId.Trim();
                     CountryToBeInserted.CHUSER = "MercGEO Feed";
                     CountryToBeInserted.CHTS = DateTime.Now;

                     try
                     {
                         objContext.MESC1TS_COUNTRY.Add(CountryToBeInserted);
                         objContext.SaveChanges();
                     }
                     catch (Exception ex)
                     {
                        
                         logEntry.Message = ex.ToString();
                         Logger.Write(logEntry);
                     }
                 }

            }
            if ((Tag == "TAGC") || (Tag == "TAGD"))
            {
                var LocationListFromDB = (from loc in objContext.MESC1TS_LOCATION
                                          where loc.LOC_CD.Trim() == m_LocationCode.Trim()
                                          select loc).ToList();
                if (LocationListFromDB.Count > 0)
                {
                    GetUpdateSQL(Tag);
                }
                else
                {
                    MESC1TS_LOCATION LocationtoBeInserted = new MESC1TS_LOCATION();
                    LocationtoBeInserted.LOC_CD = m_LocationCode.Trim();
                    LocationtoBeInserted.LOC_DESC = m_LocationDescription.Trim();
                    LocationtoBeInserted.COUNTRY_CD = m_CountryCode.Trim();
                    LocationtoBeInserted.RKRPLOC = m_RKCCLocation.Trim();
                    LocationtoBeInserted.LOC_GEO_ID = m_GeoId.Trim();
                    LocationtoBeInserted.CONTACT_EQSAL_SW = "N";
                    LocationtoBeInserted.CHUSER = "MercGEO Feed";
                    LocationtoBeInserted.CHTS = DateTime.Now;
                    try
                    {
                        objContext.MESC1TS_LOCATION.Add(LocationtoBeInserted);
                        objContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                    }
                }
            }
            if (Tag == "TAGE")
            {
               var AreaDBObject = (from area in objContext.MESC1TS_AREA
                                    where area.AREA_CD == m_AreaCode
                                    select area).ToList();
               if (AreaDBObject.Count > 0)
               {
                   GetUpdateSQL(Tag);
               }
               else
               {
                   MESC1TS_AREA AreaToBeInserted = new MESC1TS_AREA();
                   AreaToBeInserted.AREA_CD = m_AreaCode.Trim();
                   AreaToBeInserted.AREA_DESC = m_AreaDescription.Trim();
                   AreaToBeInserted.AREA_GEO_ID = m_GeoId.Trim();
                   AreaToBeInserted.CHUSER = "MercGEO Feed";
                   AreaToBeInserted.CHTS = DateTime.Now;
                   try
                   {
                       objContext.MESC1TS_AREA.Add(AreaToBeInserted);
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

        public static void GetUpdateSQL(string Tag)
        {
            if (Tag == "TAGA")
            {
               
                var CountryDBObject = (from con in objContext.MESC1TS_COUNTRY
                                       where con.COUNTRY_CD == m_CountryCode
                                       select con).ToList();
                if (CountryDBObject.Count > 0)
                {
                    CountryDBObject[0].COUNTRY_CD = m_CountryCode.Trim();
                    CountryDBObject[0].COUNTRY_DESC = m_CountryDescription.Trim();
                    CountryDBObject[0].CHUSER = "MercGEO Feed";
                    CountryDBObject[0].CHTS = DateTime.Now;
                    try
                    {
                        objContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                    }
                }
                else
                {
                    GetInsertSQL(Tag);
                }

            }
            if ((Tag == "TAGC") || (Tag == "TAGD"))
            {

                var LocationListFromDB = (from loc in objContext.MESC1TS_LOCATION
                                          where loc.LOC_CD.Trim() == m_LocationCode.Trim()
                                          select loc).ToList();
                if (LocationListFromDB.Count > 0)
                {
                    LocationListFromDB[0].LOC_DESC = m_LocationDescription.Trim();
                    LocationListFromDB[0].COUNTRY_CD = m_CountryCode.Trim();
                    LocationListFromDB[0].RKRPLOC = m_RKCCLocation.Trim();
                    LocationListFromDB[0].CHUSER = "MercGEO Feed";
                    LocationListFromDB[0].CHTS = DateTime.Now;

                    try
                    {
                        objContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                    }
                }
                else
                {
                    GetInsertSQL(Tag);
                }
                

            }
            if (Tag == "TAGE")
            {
               
                var AreaDBObject = (from area in objContext.MESC1TS_AREA
                                    where area.AREA_CD == m_AreaCode
                                    select area).ToList();
                if (AreaDBObject.Count > 0)
                {
                    AreaDBObject[0].AREA_DESC = m_AreaDescription.Trim();
                    AreaDBObject[0].CHUSER = "MercGEO Feed";
                    AreaDBObject[0].CHTS = DateTime.Now;

                    try
                    {
                        objContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                    }
                }
                else
                {
                    GetInsertSQL(Tag);
                }
            }

        }
    
    }
}
