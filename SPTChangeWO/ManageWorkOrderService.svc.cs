using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.Objects;
using MercPlusLibrary;
using System.Data.SqlClient;
using System.Configuration;
using MercPlusServiceLibrary.BusinessObjects;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Data;
using IBM.WMQ;
using System.Data.Entity.Validation;
namespace ManageWorkOrderService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class ManageWorkOrder : IManageWorkOrder
    {

        #region Declare Variables
        LogEntry logEntry = new LogEntry();
        ManageWorkOrderServiceEntities objContext;
        private const string INFO_MSG = "No Changes to Save";
        //ManageWorkOrderDAL ManageWorkOrderDAL = new ManageWorkOrderDAL();
        bool m_bCPHRepairCostExceeded = true;
        bool m_bNULLRegHours;
        bool m_bNULLDblHours;
        bool m_bNULLOTHours;
        bool m_bNULLMiscHours;
        decimal? m_fDiscountPercent = 0;
        decimal? m_fMarkUp = 0;
        decimal? m_fTotRepairMaterialAmt = 0;
        decimal? m_fTotRepairMaterialAmtCPH = 0;
        decimal? m_fTotWMaterialAmt;
        decimal? m_fTotWMaterialAmtCPH;
        decimal? m_fTotTMaterialAmt;
        decimal? m_fTotTMaterialAmtCPH;
        decimal? m_fRemarksRepairCeiling;
        string SpecialRemarks = string.Empty;
        string m_sDisplaySw;
        bool m_bSpecialRemarkFound;
        decimal? m_fAutoAppovalLimit = 0;
        double? m_fTotRepairAmtLimit = 0;
        decimal? m_fShopMaterialLimit = 0;
        bool m_bBypassLease = false;

        private const string IMPORTTAXCODE = "0975";
        private const string SALESTAXPARTCODE = "0976";
        private const string SALESTAXLABORCODE = "0977";
        private const string PTICODE = "0940";
        private const string PTIMODE1 = "43";
        private const string PTIMODE2 = "45";
        private const short APPROVEDSTATUS = 390;
        private const short COMPLETEDWO = 400;
        private const int NONSCODELEN = 6;
        private const int CHUSERLEN = 32;

        #region RKEM Constants
        // Equipment length/offsets (return from RKEM get equipment call.
        // ErrorMessageTypes
        private const int SUCCESS = 0;
        private const int NOTFOUND = -1;
        private const int INVALID = -2;
        private const int INVALIDSHOP = -3;
        private const int SYSTEMERROR = -99;
        private const int RTNCODEPOS = 42;
        private const int RTNCODELEN = 1;
        private const int ERRPGMPOS = 43;
        private const int ERRPGMLEN = 8;
        private const int ERRMSGPOS = 55;
        private const int ERRMSGLEN = 50;
        private const int EQUIPPOS = 111;
        private const int EQUIPLEN = 11;
        private const int COSIZEPOS = 122;
        private const int COSIZELEN = 2;
        private const int EQOUTHGUPOS = 124;
        private const int EQOUTHGULEN = 7;
        private const int COTYPEPOS = 131;
        private const int COTYPELEN = 4;
        private const int EQSTYPPOS = 135;
        private const int EQSTYPLEN = 4;
        private const int EQOWNTPPOS = 139;
        private const int EQOWNTPLEN = 3;
        private const int EQMATRPOS = 142;
        private const int EQMATRLEN = 3;
        private const int DELDATSHPOS = 145;
        private const int DELDATSHLEN = 10;
        private const int STEMPTYPOS = 155;
        private const int STEMPTYLEN = 1;
        private const int STREFURBPOS = 156;
        private const int STREFURBLEN = 1;
        private const int REFRBDATPOS = 157;
        private const int REFRBDATLEN = 10;
        private const int STREDELPOS = 167;
        private const int STREDELLEN = 1;
        private const int FIXCOVERPOS = 168;
        private const int FIXCOVERLEN = 7;
        private const int DPPPOS = 175;
        private const int DPPLEN = 7;
        private const int OFFHIREPOS = 182;
        private const int OFFHIRELEN = 1;
        private const int STSELSCRPOS = 183;
        private const int STSELSCRLEN = 1;
        private const int EQMANCDPOS = 184;
        private const int EQMANCDLEN = 3;
        private const int EQPROFILPOS = 187;
        private const int EQPROFILLEN = 11;
        private const int EQINDATPOS = 198;
        private const int EQINDATLEN = 10;
        private const int EQRUMANPOS = 208;
        private const int EQRUMANLEN = 15;
        private const int EQRUTYPPOS = 223;
        private const int EQRUTYPLEN = 15;
        private const int EQIOFLTPOS = 238;
        private const int EQIOFLTLEN = 1;
        private const int DAMAGEPOS = 239;
        private const int DAMAGELEN = 1;
        private const int PLAWHOPOS = 240;
        private const int PLAWHOLEN = 30;
        private const int LSCOMPPOS = 270;
        private const int LSCOMPLEN = 3;
        private const int LSCONTRPOS = 273;
        private const int LSCONTRLEN = 10;
        private const int PRESENTLOCPOS = 283;
        private const int PRESENTLOCLEN = 8;
        private const int GATEINDTEPOS = 291;
        private const int GATEINDTELEN = 10;
        private const string MQ_READ_MODE = "R";
        private const string MQ_WRITE_MODE = "W";

        #endregion RKEM Constants

        ErrMessage Message = new ErrMessage();
        RemarkEntry RemarksEntry = new RemarkEntry();
        List<string> lstRepCodeLocPart = new List<string>();
        #endregion Declare Variables


        ManageWorkOrder()
        {
            objContext = new ManageWorkOrderServiceEntities();
        }

        public List<Damage> GetDamageCodeAll(string DamageCode)
        {
            //List<Damage> DamageList = ManageWorkOrderDAL.GetDamageCode();
            string a = string.Empty;
            List<Damage> DamageList = new List<Damage>();
            //List<MESC1TS_DAMAGE> DamageFromDB = new List<MESC1TS_DAMAGE>();
            try
            {
                var DamageFromDB = (from damage in objContext.MESC1TS_DAMAGE
                                    where (DamageCode == null ? a == a : damage.cedex_code == DamageCode)
                                    select new
                                    {
                                        damage.cedex_code,
                                        damage.description,
                                        damage.name
                                    }).ToList();

                foreach (var obj in DamageFromDB)
                {
                    Damage Damage = new Damage();
                    Damage.DamageCedexCode = obj.cedex_code;
                    Damage.DamageDescription = obj.description;
                    Damage.DamageName = obj.name;
                    DamageList.Add(Damage);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return DamageList;
        }

        private void PopulateShopWithCustAndCurrency(Shop Shop)
        {
            List<Customer> Customerlist = new List<Customer>();
            Customer Customer = new Customer();
            Shop.Currency = new Currency();
            Shop.Customer = new List<Customer>();
            try
            {
                ////Get the Currency detail on the basis of the ShopCode
                var CurrencyFromDB = (from C in objContext.MESC1TS_CURRENCY
                                      join S in objContext.MESC1TS_SHOP on C.CUCDN equals S.CUCDN
                                      where S.SHOP_CD == Shop.ShopCode
                                      select new
                                      {
                                          C.CUCDN,
                                          C.CURCD,
                                          C.CURRNAMC
                                      }).ToList();

                var Currency = CurrencyFromDB.Find(curr => curr.CUCDN == Shop.CUCDN);
                Shop.Currency.Cucdn = Currency.CUCDN;
                Shop.Currency.CurCode = Currency.CURCD;
                Shop.Currency.CurrName = Currency.CURRNAMC;

                var CustomerFromDB = (from CS in objContext.MESC1VS_CUST_SHOP
                                      from C in objContext.MESC1TS_CUSTOMER
                                      where C.CUSTOMER_CD == CS.CUSTOMER_CD &&
                                          CS.SHOP_CD == Shop.ShopCode &&
                                          C.CUSTOMER_ACTIVE_SW == "Y"
                                      select new
                                      {
                                          C.CUSTOMER_CD,
                                          C.CUSTOMER_DESC,
                                          C.CUSTOMER_ACTIVE_SW,
                                          C.CHUSER,
                                          C.CHTS
                                      }).Distinct().ToList();
                foreach (var cust in CustomerFromDB)
                {
                    Customer Cust = new Customer();
                    Cust.CustomerCode = cust.CUSTOMER_CD;
                    Cust.CustomerDesc = cust.CUSTOMER_DESC;
                    Cust.CustomerActiveSw = cust.CUSTOMER_ACTIVE_SW;
                    Cust.ChangeTime = cust.CHTS;
                    Cust.ChangeUser = cust.CHUSER;
                    Shop.Customer.Add(Cust);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            //ShopList.Add(shop);
            //}
            //return Shop;
        }

        public Shop GetShopDetailsOnShopCode(string ShopCode)
        {
            Shop Shop = new Shop();

            try
            {
                var ShopDetailsFromDB = (from shop in objContext.MESC1TS_SHOP
                                         where shop.SHOP_CD == ShopCode
                                         select new
                                         {
                                             shop.SHOP_CD,
                                             shop.CUCDN,
                                             shop.SHOP_DESC,
                                             shop.IMPORT_TAX
                                         }).FirstOrDefault();

                if (ShopDetailsFromDB != null)
                {
                    Shop.ShopCode = ShopDetailsFromDB.SHOP_CD;
                    Shop.ShopDescription = ShopDetailsFromDB.SHOP_DESC;
                    Shop.CUCDN = ShopDetailsFromDB.CUCDN;
                    Shop.ImportTax = ShopDetailsFromDB.IMPORT_TAX;
                    PopulateShopWithCustAndCurrency(Shop);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Shop;
        }

        public List<Shop> GetShopCode(int UserID)
        {
            //UserID = 10022;
            logEntry.Message = "Method Name : GetShopCode(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);

            
            List<WorkOrderDetail> WODetailList = new List<WorkOrderDetail>();
            List<MESC1VS_SHOP_LOCATION> ShopListFromDBOnAuth = new List<MESC1VS_SHOP_LOCATION>();


            try
            {
                //SELECT U.AUTHGROUP_ID, U.COLUMN_VALUE as COLUMN_VALUE, G.TABLE_NAME, G.COLUMN_NAME as COLUMN_NAME
                //FROM SEC_AUTHGROUP_USER  U,	SEC_AUTHGROUP  G
                //WHERE U.USER_ID = <User_Id>
                //AND U.AUTHGROUP_ID = G.AUTHGROUP_ID ORDER BY U.AUTHGROUP_ID

                ShopListFromDBOnAuth = GenerateShopList(UserID);

                //select distinct S.SHOP_CD, S.SHOP_ACTIVE_SW, S.CUCDN, SM.CUSTOMER_CD, CY.CURRNAMC, S.IMPORT_TAX, S.SALES_TAX_PART_CONT, S.SALES_TAX_PART_GEN, S.SALES_TAX_LABOR_CON, S.SALES_TAX_LABOR_GEN, S.OVERTIME_SUSP_SW, CU.MAERSK_SW 
                //from MESC1TS_SHOP S, MESC1TS_CUST_SHOP_MODE SM, MESC1TS_CURRENCY CY, MESC1TS_CUSTOMER CU 
                //where S.SHOP_CD = SM.SHOP_CD 
                //and S.SHOP_ACTIVE_SW = 'Y' 
                //and S.CUCDN = CY.CUCDN 
                //and CU.CUSTOMER_CD = SM.CUSTOMER_CD 
                //and S.SHOP_CD in ( ShopList ) 
                //order by S.SHOP_CD asc, CU.MAERSK_SW desc";	
                if (ShopListFromDBOnAuth != null && ShopListFromDBOnAuth.Count > 0)
                {
                    var ShopListFromDB = (from S in objContext.MESC1TS_SHOP
                                          from SM in objContext.MESC1TS_CUST_SHOP_MODE
                                          from CY in objContext.MESC1TS_CURRENCY
                                          from CU in objContext.MESC1TS_CUSTOMER
                                          where S.SHOP_CD == SM.SHOP_CD &&
                                                S.SHOP_ACTIVE_SW == "Y" &&
                                                S.CUCDN == CY.CUCDN &&
                                                CU.CUSTOMER_CD == SM.CUSTOMER_CD
                                          orderby S.SHOP_CD
                                          select new
                                          {
                                              S.SHOP_CD
                                          }).ToList();


                    var ShopListFinal = ShopListFromDBOnAuth.FindAll(a => ShopListFromDB.Any(ab => ab.SHOP_CD == a.SHOP_CD));

                    MESC1TS_CURRENCY Currency = new MESC1TS_CURRENCY();
                    List<MESC1TS_CURRENCY> CurrencyFromDB = new List<MESC1TS_CURRENCY>();

                    //SELECT DISTINCT C.CUSTOMER_CD 
                    //FROM MESC1VS_CUST_SHOP CS, MESC1TS_CUSTOMER C 
                    //WHERE C.CUSTOMER_CD = CS.CUSTOMER_CD 
                    // AND CS.SHOP_CD IN ( ShopList )
                    List<Shop> ShopList = new List<Shop>();
                    foreach (var item in ShopListFinal)
                    {
                        Shop shop = new Shop();
                        shop.ShopCode = item.SHOP_CD;
                        shop.ShopDescription = item.SHOP_DESC;
                        shop.CUCDN = item.CUCDN;
                        shop.ImportTax = item.IMPORT_TAX;
                        ShopList.Add(shop);
                    }
                    if (ShopList != null && ShopList.Count > 0)
                    {
                        IndexedShop = ShopList[0];
                        PopulateShopWithCustAndCurrency(IndexedShop);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            logEntry.Message = "Method Name : GetShopCode(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);

            return ShopList;
        }

        private List<MESC1VS_SHOP_LOCATION> GenerateShopList(int UserID)
        {
            logEntry.Message = "Method Name : GenerateShopList(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            List<string> VendorCodeList = new List<string>();
            List<string> LocCodeList = new List<string>();
            List<string> CountryCodeList = new List<string>();
            List<string> ShopCodeList = new List<string>();
            List<string> AreaCodeList = new List<string>();
            List<MESC1VS_SHOP_LOCATION> ShopListFromDBOnAuth = null;

            try
            {
                var ShopOnAuth = (from U in objContext.SEC_AUTHGROUP_USER
                                  from G in objContext.SEC_AUTHGROUP
                                  where U.USER_ID == UserID &&
                                  U.AUTHGROUP_ID == G.AUTHGROUP_ID
                                  select new
                                  {
                                      U.AUTHGROUP_ID,
                                      COLUMN_VALUE = U.COLUMN_VALUE,
                                      G.TABLE_NAME,
                                      COLUMN_NAME = G.COLUMN_NAME
                                  }).OrderBy(id => id.AUTHGROUP_ID).ToList();



                foreach (var item in ShopOnAuth)
                {
                    if (item.COLUMN_NAME == "VENDOR_CD")
                    {
                        string VendorCode = item.COLUMN_VALUE;
                        VendorCodeList.Add(VendorCode);
                    }
                    if (item.COLUMN_NAME == "LOC_CD")
                    {
                        string LocCode = item.COLUMN_VALUE;
                        LocCodeList.Add(LocCode);
                    }
                    if (item.COLUMN_NAME == "COUNTRY_CD")
                    {
                        string CountryCode = item.COLUMN_VALUE;
                        CountryCodeList.Add(CountryCode);
                    }
                    if (item.COLUMN_NAME == "AREA_CD")
                    {
                        string AreaCode = item.COLUMN_VALUE;
                        AreaCodeList.Add(AreaCode);
                    }
                    if (item.COLUMN_NAME == "SHOP_CD")
                    {
                        string ShopCode = item.COLUMN_VALUE;
                        ShopCodeList.Add(ShopCode);
                    }
                }
                ShopListFromDBOnAuth = new List<MESC1VS_SHOP_LOCATION>();
                ShopListFromDBOnAuth = (from shop in objContext.MESC1VS_SHOP_LOCATION
                                        where shop.SHOP_ACTIVE_SW == "Y" &&
                                        (VendorCodeList.Contains(shop.VENDOR_CD) ||
                                        LocCodeList.Contains(shop.LOC_CD) ||
                                        CountryCodeList.Contains(shop.COUNTRY_CD) ||
                                        AreaCodeList.Contains(shop.AREA_CD) ||
                                        ShopCodeList.Contains(shop.SHOP_CD))
                                        select shop).OrderBy(s => s.SHOP_CD).ToList();
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            logEntry.Message = "Method Name : GenerateShopList(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            return ShopListFromDBOnAuth;
        }

        public List<Customer> GetCustomerCode(string ShopCode)
        {
            List<Customer> Customerlist = new List<Customer>();
            List<Shop> ShopList = new List<Shop>();
            List<string> shopCodes = ShopList.Select(sc => { return sc.ShopCode; }).ToList();
            try
            {
                var CustomerFromDB = (from CS in objContext.MESC1VS_CUST_SHOP
                                      from C in objContext.MESC1TS_CUSTOMER
                                      where C.CUSTOMER_CD == CS.CUSTOMER_CD &&
                                            CS.SHOP_CD == ShopCode
                                      select new
                                      {
                                          C.CUSTOMER_CD
                                      }).Distinct().ToList();

                foreach (var item in CustomerFromDB)
                {
                    Customer customer = new Customer();
                    customer.CustomerCode = item.CUSTOMER_CD;
                    Customerlist.Add(customer);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return Customerlist;
        }

        public string GetCurrency(string ShopCode)
        {
            List<Currency> Currency = new List<Currency>();
            string CurrencyName = string.Empty;

            try
            {
                //Get the Currency detail on the basis of the ShopCode
                var CurrencyFromDB = (from C in objContext.MESC1TS_CURRENCY
                                      join S in objContext.MESC1TS_SHOP on C.CUCDN equals S.CUCDN
                                      where S.SHOP_CD == ShopCode
                                      select new
                                      {
                                          C.CURRNAMC
                                      }).FirstOrDefault();
                CurrencyName = CurrencyFromDB.CURRNAMC;
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return CurrencyName;
        }

        public List<RepairLoc> GetRepairLocCode(string RepairLocCode)
        {
            string a = string.Empty;
            List<RepairLoc> RepairLocList = new List<RepairLoc>();

            try
            {
                var RepairLocFromDB = (from R in objContext.MESC1TS_REPAIR_LOC
                                       where (RepairLocCode == null ? a == a : R.cedex_code == RepairLocCode)
                                       select new
                                       {
                                           R.cedex_code,
                                           R.description
                                       }).OrderBy(code => code.cedex_code).ToList();

                foreach (var item in RepairLocFromDB)
                {
                    RepairLoc RepairLoc = new RepairLoc();
                    RepairLoc.CedexCode = item.cedex_code;
                    RepairLoc.Description = item.description;
                    RepairLocList.Add(RepairLoc);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return RepairLocList;
        }

        public List<RepairCode> GetRepairCode(string ModeCode)
        {
            string a = string.Empty;
            List<RepairCode> RepairCodeList = new List<RepairCode>();
            try
            {
                var RepairCodeFromDB = (from R in objContext.MESC1TS_REPAIR_CODE
                                        where R.MODE == ModeCode
                                        select new
                                        {
                                            R.REPAIR_CD,
                                            R.REPAIR_DESC,
                                            R.MAX_QUANTITY,
                                            R.MANUAL_CD
                                        }).OrderBy(code => code.REPAIR_CD).ToList();

                foreach (var item in RepairCodeFromDB)
                {
                    RepairCode RepairCode = new RepairCode();
                    RepairCode.RepairCod = item.REPAIR_CD.Trim();
                    RepairCode.RepairDesc = item.REPAIR_DESC;
                    RepairCode.MaxQuantity = item.MAX_QUANTITY;
                    RepairCode.ManualCode = item.MANUAL_CD;
                    RepairCodeList.Add(RepairCode);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return RepairCodeList;
        }

        private List<RepairCode> CheckRepairCode(string ModeCode, string RepCode)
        {
            string a = string.Empty;
            List<RepairCode> RepairCodeList = new List<RepairCode>();
            try
            {
                var RepairCodeFromDB = (from R in objContext.MESC1TS_REPAIR_CODE
                                        where R.MODE == ModeCode &&
                                        R.REPAIR_CD == RepCode.Trim()
                                        select new
                                        {
                                            R.REPAIR_CD,
                                            R.REPAIR_DESC,
                                            R.MAX_QUANTITY,
                                            R.MANUAL_CD
                                        }).OrderBy(code => code.REPAIR_CD.Trim()).ToList();

                foreach (var item in RepairCodeFromDB)
                {
                    RepairCode RepairCode = new RepairCode();
                    RepairCode.RepairCod = item.REPAIR_CD.Trim();
                    RepairCode.RepairDesc = item.REPAIR_DESC;
                    RepairCode.MaxQuantity = item.MAX_QUANTITY;
                    RepairCode.ManualCode = item.MANUAL_CD;
                    RepairCodeList.Add(RepairCode);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return RepairCodeList;
        }

        public List<Tpi> GetTpiCode(string TPICode)
        {
            List<Tpi> TpiList = new List<Tpi>();
            string a = string.Empty;
            try
            {
                var TpiFromDB = (from T in objContext.MESC1TS_TPI
                                 where (TPICode == null ? a == a : T.cedex_code == TPICode)
                                 select new
                                 {
                                     T.cedex_code,
                                     T.name,
                                     T.description
                                 }).ToList();

                foreach (var item in TpiFromDB)
                {
                    Tpi Tpi = new Tpi();
                    Tpi.CedexCode = item.cedex_code;
                    Tpi.Name = item.name;
                    Tpi.Description = item.description;
                    TpiList.Add(Tpi);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return TpiList;
        }

        public Equipment GetEquipmentDetailsFromRKEM(string EqpNo, string ShopCode, string VendorRefNo)
        {
            logEntry.Message = "Method Name : GetEquipmentDetailsFromRKEM(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            //Get the eqp details from RKEM
            List<Equipment> EqpDetailsList = new List<Equipment>();
            Equipment RKEMEquipment = null;
            string MaterialSwitch = string.Empty;
            string a = string.Empty;
            int ErrorCount = 0;


            try
            {
                ErrorCount = LoadEquipmentRefNo(EqpNo, ShopCode, VendorRefNo, ref RKEMEquipment);

                if (RKEMEquipment == null || ErrorCount < 0)
                {
                    RKEMEquipment = new Equipment();
                    RKEMEquipment.EquipmentNo = EqpNo;
                    RKEMEquipment.EqpNotFound = "No RKEM Data found";
                }
                else if (RKEMEquipment != null && ErrorCount == 0)
                {
                    if (RKEMEquipment.Eqmatr == "ALU")
                    {
                        MaterialSwitch = "Y";
                    }
                    else
                    {
                        MaterialSwitch = "N";
                    }

                    //SELECT M.MODE, M.MODE_DESC, M.MODE_ACTIVE_SW from MESC1TS_EQMODE EM, MESC1TS_MODE M  
                    //WHERE COTYPE = 'CoType' AND EQSTYPE = 'EqSType' 
                    //AND ALUMINIUM_SW = 'Material' AND EQSIZE = 'Size' AND M.MODE = EM.MODE
                    var ModeList = (from M in objContext.MESC1TS_MODE
                                    from EM in objContext.MESC1TS_EQMODE
                                    where EM.COTYPE == RKEMEquipment.COType &&
                                    EM.EQSTYPE == RKEMEquipment.Eqstype &&
                                    EM.ALUMINIUM_SW == MaterialSwitch &&
                                    (!string.IsNullOrEmpty(RKEMEquipment.Size) ? EM.EQSIZE == RKEMEquipment.Size : a == a) &&
                                    M.MODE == EM.MODE
                                    select new
                                    {
                                        M.MODE,
                                        M.MODE_DESC
                                    }).ToList();


                    if (ModeList != null && ModeList.Count > 0)
                    {
                        RKEMEquipment.SelectedMode = ModeList[0].MODE;
                        RKEMEquipment.ModeList = new List<Mode>();
                        //DummyEqp.ModeList = EqpList;
                        foreach (var item in ModeList)
                        {
                            //Equipment Equipment = new Equipment();
                            Mode Mode = new Mode();
                            Mode.ModeCode = item.MODE;
                            Mode.ModeDescription = item.MODE_DESC;
                            RKEMEquipment.ModeList.Add(Mode);
                        }
                    }
                }
                if (ErrorCount == 0)
                {
                    switch (RKEMEquipment.Damage)
                    {
                        case "N": RKEMEquipment.Damage = "No damage"; break;
                        case "2": RKEMEquipment.Damage = "Yes, 3rd party unknown"; break;
                        case "3": RKEMEquipment.Damage = "Yes, 3rd party known"; break;
                        case "4": RKEMEquipment.Damage = "Yes, Machinery damaged"; break;
                        default: RKEMEquipment.Damage = "not available"; break;
                    }

                    switch (RKEMEquipment.LeaseComp)
                    {
                        case "001": RKEMEquipment.LeaseComp = string.Empty; break;
                        case "003": RKEMEquipment.LeaseComp = string.Empty; break;
                        case "NA": RKEMEquipment.LeaseComp = "not available"; break;
                        default: RKEMEquipment.LeaseComp = RKEMEquipment.LeaseComp; break;
                    }

                    if (RKEMEquipment.LeaseContract == "NA")
                        RKEMEquipment.LeaseContract = "not available";
                }


            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                ErrorCount = 99;

            }
            //if (ErrorCount != 0)
            //{
            //    Message.Message = "EQPNO: " + EqpNo + " " + GetEQErrorMsg(ErrorCount);
            //    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
            //    //to add the errormessagelist
            //    //m_ErrorList.Insert(msg);
            //}
            logEntry.Message = "Method Name : GetEquipmentDetailsFromRKEM(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);

            return RKEMEquipment;
        }

        public string GetEQErrorMsg(long nError)
        {
            string sMsg;

            switch (nError)
            {
                case SUCCESS: sMsg = "Equipment found in RKEM"; break;
                case NOTFOUND: sMsg = "Equipment not found in RKEM"; break;
                case INVALID: sMsg = "Equipment number is invalid"; break;
                case INVALIDSHOP: sMsg = "Shop not found"; break;
                case SYSTEMERROR: sMsg = "No response received from RKEM"; break;
                default: sMsg = "Unknown system failure attempting to find equipment in RKEM"; break;
            }

            return (sMsg);
        }

        private WorkOrderDetail LoadWorkOrderDetails(int WorkOrderID) // return workOrderDetail
        {

            logEntry.Message = "Method Name : LoadWorkOrderDetails(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            WorkOrderDetail OriginalWO = new WorkOrderDetail();
            try
            {
                #region Load Header
                //OriginalWO = ManageWorkOrderDAL.LoadHeaderDetails(OriginalWO.WorkOrderID);
                //OriginalWO.RepairsViewList = ManageWorkOrderDAL.LoadRepairsDetails(OriginalWO.WorkOrderID);
                //OriginalWO.SparePartsViewList = ManageWorkOrderDAL.LoadPartsDetails(OriginalWO.WorkOrderID);
                //OriginalWO.RemarksList = ManageWorkOrderDAL.LoadRemarksDetails(OriginalWO.WorkOrderID);

                OriginalWO = LoadHeaderDetails(WorkOrderID);
                OriginalWO.RepairsViewList = LoadRepairsDetails(WorkOrderID);
                OriginalWO.SparePartsViewList = LoadPartsDetails(WorkOrderID);
                OriginalWO.RemarksList = LoadRemarksDetails(WorkOrderID);
                #endregion Load Header
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            logEntry.Message = "Method Name : LoadWorkOrderDetails(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);

            return OriginalWO;
        }

        public WorkOrderDetail LoadHeaderDetails(int WOID)
        {
            WorkOrderDetail WorkOrder = new WorkOrderDetail();
            WorkOrder.Shop = new Shop();
            WorkOrder.Shop.Customer = new List<Customer>();
            WorkOrder.Shop.Currency = new Currency();
            WorkOrder.EquipmentList = new List<Equipment>();
            WorkOrder.Shop.Location = new Location();
            double d = 0;
            try
            {
                //List<MESC1TS_WO> WorkOrderList = GetWorkOrderHeaderDetails(WOID);

                var WorkOrderList = (from w in objContext.MESC1TS_WO
                                     join shop in objContext.MESC1TS_SHOP on w.SHOP_CD equals shop.SHOP_CD
                                     join curr in objContext.MESC1TS_CURRENCY on w.CUCDN equals curr.CUCDN
                                     join mode in objContext.MESC1TS_MODE on w.MODE equals mode.MODE
                                     join cust in objContext.MESC1TS_CUSTOMER on w.CUSTOMER_CD equals cust.CUSTOMER_CD
                                     join loc in objContext.MESC1TS_LOCATION on shop.LOC_CD equals loc.LOC_CD
                                     where w.WO_ID == WOID
                                     select new
                                     {
                                         shop.SHOP_DESC,
                                         shop.SHOP_TYPE_CD,
                                         shop.IMPORT_TAX,
                                         curr.CURRNAMC,
                                         mode.MODE_DESC,
                                         cust.CUSTOMER_DESC,
                                         loc.CONTACT_EQSAL_SW,
                                         w
                                     }).FirstOrDefault();

                #region Data Mapper
                if (WorkOrderList != null)
                {
                    Customer cust = new Customer();
                    WorkOrder.woState = (int)Validation.STATE.EXISTING;
                    WorkOrder.WorkOrderID = WorkOrderList.w.WO_ID;
                    cust.CustomerCode = WorkOrderList.w.CUSTOMER_CD;
                    cust.CustomerDesc = WorkOrderList.CUSTOMER_DESC;
                    WorkOrder.Shop.Currency.Cucdn = WorkOrderList.w.CUCDN;
                    WorkOrder.Shop.Currency.CurrName = WorkOrderList.CURRNAMC;
                    cust.ManualCode = WorkOrderList.w.MANUAL_CD;
                    WorkOrder.Shop.ShopDescription = WorkOrderList.SHOP_DESC;
                    WorkOrder.Shop.ImportTax = WorkOrderList.IMPORT_TAX;
                    WorkOrder.Mode = WorkOrderList.w.MODE;
                    WorkOrder.ModeDescription = WorkOrderList.MODE_DESC;
                    WorkOrder.WorkOrderType = WorkOrderList.w.WOTYPE;
                    WorkOrder.VendorCode = WorkOrderList.w.VENDOR_CD;
                    WorkOrder.Shop.ShopCode = WorkOrderList.w.SHOP_CD;
                    WorkOrder.Shop.ShopTypeCode = WorkOrderList.SHOP_TYPE_CD;
                    WorkOrder.RepairDate = WorkOrderList.w.REPAIR_DTE;
                    WorkOrder.WorkOrderStatus = WorkOrderList.w.STATUS_CODE;
                    WorkOrder.Cause = WorkOrderList.w.CAUSE;
                    WorkOrder.Shop.Customer.Add(cust);
                    WorkOrder.ThirdPartyPort = WorkOrderList.w.THIRD_PARTY;
                    //WorkOrder.THIRD_PARTY= " ";
                    WorkOrder.Shop.Location.ContactEqsalSW = WorkOrderList.CONTACT_EQSAL_SW;
                    WorkOrder.ManHourRate = WorkOrderList.w.MANH_RATE;
                    WorkOrder.ManHourRateCPH = WorkOrderList.w.MANH_RATE_CPH;
                    WorkOrder.ExchangeRate = WorkOrderList.w.EXCHANGE_RATE;
                    WorkOrder.Shop.CUCDN = WorkOrderList.w.CUCDN;

                    if (WorkOrderList.w.TOT_REPAIR_MANH != null)
                    {
                        WorkOrder.TotalRepairManHour = Math.Round(WorkOrderList.w.TOT_REPAIR_MANH.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalRepairManHour = 0.0;
                    }


                    if (WorkOrderList.w.TOT_MANH_REG != null)
                    {
                        WorkOrder.TotalManHourReg = Math.Round(WorkOrderList.w.TOT_MANH_REG.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalManHourReg = 0.0;
                    }

                    //	   		WorkOrder.TOT_MANH_OT=  WorkOrderList.w.TOT_MANH_OT;
                    if (WorkOrderList.w.TOT_MANH_OT != null)
                    {
                        WorkOrder.TotalManHourOverTime = Math.Round(WorkOrderList.w.TOT_MANH_OT.Value, 4); ;
                    }
                    else
                    {
                        WorkOrder.TotalManHourOverTime = 0.0;
                    }

                    //	   		WorkOrder.TOT_MANH_DT=  WorkOrderList.w.TOT_MANH_DT;
                    if (WorkOrderList.w.TOT_MANH_DT != null)
                    {
                        WorkOrder.TotalManHourDoubleTime = Math.Round(WorkOrderList.w.TOT_MANH_DT.Value, 4); ;
                    }
                    else
                    {
                        WorkOrder.TotalManHourDoubleTime = 0.0;
                    }

                    //	   		WorkOrder.TOT_MANH_MISC=  WorkOrderList.w.TOT_MANH_MISC;
                    if (WorkOrderList.w.TOT_MANH_MISC != null)
                    {
                        WorkOrder.TotalManHourMisc = Math.Round(WorkOrderList.w.TOT_MANH_MISC.Value, 4); ;
                    }
                    else
                    {
                        WorkOrder.TotalManHourMisc = 0.0;
                    }


                    //	   		WorkOrder.TOT_PREP_HRS=  WorkOrderList.w.TOT_PREP_HRS;
                    if (WorkOrderList.w.TOT_PREP_HRS != null)
                    {
                        d = (double)WorkOrderList.w.TOT_PREP_HRS;


                        WorkOrder.TotalPrepHours = Math.Round(WorkOrderList.w.TOT_PREP_HRS.Value, 4); ;
                    }
                    else
                    {
                        WorkOrder.TotalPrepHours = 0.0;
                    }

                    //	   		WorkOrder.TOT_LABOR_HRS=  WorkOrderList.w.TOT_LABOR_HRS;
                    if (WorkOrderList.w.TOT_LABOR_HRS != null)
                    {
                        d = (double)WorkOrderList.w.TOT_LABOR_HRS;


                        WorkOrder.TotalLaborHours = Math.Round(WorkOrderList.w.TOT_LABOR_HRS.Value, 4); ;
                    }
                    else
                    {
                        WorkOrder.TotalLaborHours = 0.0;
                    }

                    //	   		WorkOrder.TOT_LABOR_COST=  WorkOrderList.w.TOT_LABOR_COST;
                    if (WorkOrderList.w.TOT_LABOR_COST != null)
                    {
                        WorkOrder.TotalLabourCost = Math.Round(WorkOrderList.w.TOT_LABOR_COST.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalLabourCost = 0;
                    }

                    //	   		WorkOrder.TOT_LABOR_COST_CPH=  WorkOrderList.w.TOT_LABOR_COST_CPH;
                    if (WorkOrderList.w.TOT_LABOR_COST_CPH != null)
                    {
                        WorkOrder.TotalLabourCostCPH = Math.Round(WorkOrderList.w.TOT_LABOR_COST_CPH.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalLabourCostCPH = 0;
                    }

                    //	   		WorkOrder.TOT_SHOP_AMT=  WorkOrderList.w.TOT_SHOP_AMT;
                    if (WorkOrderList.w.TOT_SHOP_AMT != null)
                    {
                        WorkOrder.TotalShopAmount = Math.Round(WorkOrderList.w.TOT_SHOP_AMT.Value, 4);

                    }
                    else
                    {
                        WorkOrder.TotalShopAmount = 0;
                    }

                    //Mangal Release 3 RQ6344 retrieving the field value

                    if (WorkOrderList.w.TOT_W_MATERIAL_AMT != null)
                    {
                        WorkOrder.TotalWMaterialAmount = Math.Round(WorkOrderList.w.TOT_W_MATERIAL_AMT.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalWMaterialAmount = 0;
                    }

                    if (WorkOrderList.w.TOT_T_MATERIAL_AMT != null)
                    {
                        WorkOrder.TotalTMaterialAmount = Math.Round(WorkOrderList.w.TOT_T_MATERIAL_AMT.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalTMaterialAmount = 0;
                    }

                    if (WorkOrderList.w.TOT_W_MATERIAL_AMT_CPH != null)
                    {
                        WorkOrder.TotalWMaterialAmountCPH = Math.Round(WorkOrderList.w.TOT_W_MATERIAL_AMT_CPH.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalWMaterialAmountCPH = 0;
                    }

                    if (WorkOrderList.w.TOT_T_MATERIAL_AMT_CPH != null)
                    {
                        WorkOrder.TotalWMaterialAmountCPH = Math.Round(WorkOrderList.w.TOT_T_MATERIAL_AMT_CPH.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalWMaterialAmountCPH = 0;
                    }

                    if (WorkOrderList.w.TOT_W_MATERIAL_AMT_USD != null)
                    {
                        WorkOrder.TotalWMaterialAmountUSD = Math.Round(WorkOrderList.w.TOT_W_MATERIAL_AMT_USD.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalWMaterialAmountUSD = 0;
                    }

                    if (WorkOrderList.w.TOT_T_MATERIAL_AMT_USD != null)
                    {
                        WorkOrder.TotalTMaterialAmountUSD = Math.Round(WorkOrderList.w.TOT_T_MATERIAL_AMT_USD.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalTMaterialAmountUSD = 0;
                    }

                    if (WorkOrderList.w.TOT_W_MATERIAL_AMT_CPH_USD != null)
                    {
                        WorkOrder.TotalWMaterialAmountCPHUSD = Math.Round(WorkOrderList.w.TOT_W_MATERIAL_AMT_CPH_USD.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalWMaterialAmountCPHUSD = 0;
                    }

                    if (WorkOrderList.w.TOT_T_MATERIAL_AMT_CPH_USD != null)
                    {
                        WorkOrder.TotalTMaterialAmountCPHUSD = Math.Round(WorkOrderList.w.TOT_T_MATERIAL_AMT_CPH_USD.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalTMaterialAmountCPHUSD = 0;
                    }




                    //mangal

                    //	   		WorkOrder.TOT_SHOP_AMT_CPH=  WorkOrderList.w.TOT_SHOP_AMT_CPH;
                    if (WorkOrderList.w.TOT_SHOP_AMT_CPH != null)
                    {
                        WorkOrder.TotalShopAmountCPH = Math.Round(WorkOrderList.w.TOT_SHOP_AMT_CPH.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalShopAmountCPH = 0;
                    }

                    //	   		WorkOrder.TOT_COST_LOCAL=  WorkOrderList.w.TOT_COST_LOCAL;
                    if (WorkOrderList.w.TOT_COST_LOCAL != null)
                    {
                        WorkOrder.TotalCostLocal = Math.Round(WorkOrderList.w.TOT_COST_LOCAL.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalCostLocal = 0;
                    }

                    //	   		WorkOrder.TOT_COST_CPH=  WorkOrderList.w.TOT_COST_CPH;
                    if (WorkOrderList.w.TOT_COST_CPH != null)
                    {
                        WorkOrder.TotalCostCPH = Math.Round(WorkOrderList.w.TOT_COST_CPH.Value, 4);
                    }
                    else
                    {
                        WorkOrder.TotalCostCPH = 0;
                    }


                    WorkOrder.OverTimeRate = WorkOrderList.w.OT_RATE;
                    WorkOrder.OverTimeRateCPH = WorkOrderList.w.OT_RATE_CPH;
                    WorkOrder.DoubleTimeRate = WorkOrderList.w.DT_RATE;
                    WorkOrder.DoubleTimeRateCPH = WorkOrderList.w.DT_RATE_CPH;
                    WorkOrder.MiscRate = WorkOrderList.w.MISC_RATE;
                    WorkOrder.MiscRateCPH = WorkOrderList.w.MISC_RATE_CPH;
                    WorkOrder.TotalCostLocalUSD = WorkOrderList.w.TOTAL_COST_LOCAL_USD;
                    WorkOrder.TotalCostOfRepair = WorkOrderList.w.TOT_COST_REPAIR;
                    WorkOrder.TotalCostOfRepairCPH = WorkOrderList.w.TOT_COST_REPAIR_CPH;
                    WorkOrder.SalesTaxLabour = WorkOrderList.w.SALES_TAX_LABOR;
                    WorkOrder.SalesTaxLabourCPH = WorkOrderList.w.SALES_TAX_LABOR_CPH;
                    WorkOrder.SalesTaxParts = WorkOrderList.w.SALES_TAX_PARTS;
                    WorkOrder.SalesTaxPartsCPH = WorkOrderList.w.SALES_TAX_PARTS_CPH;
                    WorkOrder.TotalMaerksParts = WorkOrderList.w.TOT_MAERSK_PARTS;
                    WorkOrder.TotalMaerksPartsCPH = WorkOrderList.w.TOT_MAERSK_PARTS_CPH;
                    WorkOrder.TotalManParts = WorkOrderList.w.TOT_MAN_PARTS;

                    WorkOrder.TotalManPartsCPH = WorkOrderList.w.TOT_MAN_PARTS_CPH;
                    //WorkOrder.VendorRefNo=  WorkOrderList.w.VENDOR_REF_NO;
                    WorkOrder.VoucherNumber = WorkOrderList.w.VOUCHER_NO;

                    //	   		WorkOrder.INVOICE_DTE=  WorkOrderList.w.INVOICE_DTE;
                    if (WorkOrderList.w.INVOICE_DTE != null)
                    {
                        WorkOrder.InvoiceDate = WorkOrderList.w.INVOICE_DTE;
                    }
                    //else
                    //{
                    //    WorkOrder.InvoiceDate = "";
                    //}


                    WorkOrder.CheckNo = WorkOrderList.w.CHECK_NO;

                    //	   		WorkOrder.PAID_DTE=  WorkOrderList.w.PAID_DTE;
                    if (WorkOrderList.w.PAID_DTE != null)
                    {
                        WorkOrder.PaidDate = WorkOrderList.w.PAID_DTE;
                    }
                    //else
                    //{
                    //    WorkOrder.PAID_DTE= "";
                    //}


                    WorkOrder.AmountPaid = WorkOrderList.w.AMOUNT_PAID;

                    //	   		WorkOrder.RKRP_REPAIR_DTE=  WorkOrderList.w.RKRP_REPAIR_DTE;
                    if (WorkOrderList.w.RKRP_REPAIR_DTE != null)
                    {
                        WorkOrder.RKRPRepairDate = WorkOrderList.w.RKRP_REPAIR_DTE;
                    }

                    //	   		WorkOrder.RKRP_XMIT_DTE=  WorkOrderList.w.RKRP_XMIT_DTE;
                    if (WorkOrderList.w.RKRP_XMIT_DTE != null)
                    {
                        WorkOrder.RKRPXMITDate = WorkOrderList.w.RKRP_XMIT_DTE;
                    }
                    //else
                    //{
                    //    WorkOrder.RKRP_XMIT_DTE= "";
                    //}


                    WorkOrder.RKRPXMITSW = WorkOrderList.w.RKRP_XMIT_SW;

                    //	   		WorkOrder.WO_RECV_DTE=  WorkOrderList.w.WO_RECV_DTE;
                    if (WorkOrderList.w.WO_RECV_DTE != null)
                    {
                        WorkOrder.WorkOrderReceiveDate = WorkOrderList.w.WO_RECV_DTE;
                    }
                    //else
                    //{
                    //    WorkOrder.WO_RECV_DTE= "";
                    //}


                    WorkOrder.ApprovedBy = WorkOrderList.w.APPROVED_BY;
                    WorkOrder.ShopWorkingSW = WorkOrderList.w.SHOP_WORKING_SW;

                    //	   		WorkOrder.APPROVAL_DTE=  WorkOrderList.w.APPROVAL_DTE;
                    if (WorkOrderList.w.APPROVAL_DTE != null)
                    {
                        WorkOrder.ApprovalDate = WorkOrderList.w.APPROVAL_DTE;
                    }
                    //else
                    //{
                    //    WorkOrder.APPROVAL_DTE= "";
                    //}


                    WorkOrder.ImportTax = WorkOrderList.w.IMPORT_TAX;
                    WorkOrder.ImportTaxCPH = WorkOrderList.w.IMPORT_TAX_CPH;
                    WorkOrder.ChangeUser = WorkOrderList.w.CHUSER;
                    if (WorkOrderList.w.CRTS != null)
                    {
                        WorkOrder.ChangeTime = (DateTime)WorkOrderList.w.CRTS;
                    }

                    // !FIX - not getting CHTS from DB.
                    //			WorkOrder.CHTS=  WorkOrderList.w.CHTS;
                    if (WorkOrderList.w.CHTS != null)
                    {
                        WorkOrder.ChangeTime = WorkOrderList.w.CHTS;
                    }
                    //else
                    //{
                    //    WorkOrder.CHTS= "";
                    //}

                    Equipment Equipment = new Equipment();
                    Equipment.EquipmentNo = WorkOrderList.w.EQPNO;
                    Equipment.Size = WorkOrderList.w.EQSIZE;
                    Equipment.Type = WorkOrderList.w.EQTYPE;
                    Equipment.VendorRefNo = WorkOrderList.w.VENDOR_REF_NO;
                    Equipment.Eqouthgu = WorkOrderList.w.EQOUTHGU;

                    Equipment.COType = WorkOrderList.w.COTYPE;
                    Equipment.Eqstype = WorkOrderList.w.EQSTYPE;
                    Equipment.Eqowntp = WorkOrderList.w.EQOWNTP;
                    Equipment.Eqmatr = WorkOrderList.w.EQMATR;
                    Equipment.EqMancd = WorkOrderList.w.EQMANCD;
                    Equipment.EQProfile = WorkOrderList.w.EQPROFIL;
                    Equipment.EQInDate = WorkOrderList.w.EQINDAT;

                    Equipment.EQRuman = WorkOrderList.w.EQRUMAN;
                    Equipment.EQRutyp = WorkOrderList.w.EQRUTYP;
                    Equipment.EQIoflt = WorkOrderList.w.EQIOFLT;
                    Equipment.ReqRemarkSW = WorkOrderList.w.REQ_REMARK_SW;
                    Equipment.StEmptyFullInd = WorkOrderList.w.STEMPTY; ;
                    Equipment.Strefurb = WorkOrderList.w.STREFURB;
                    Equipment.Stredel = WorkOrderList.w.STREDEL;
                    Equipment.STSELSCR = WorkOrderList.w.STSELSCR;
                    Equipment.Fixcover = WorkOrderList.w.FIXCOVER;
                    Equipment.OffhirLocationSW = WorkOrderList.w.OFFHIR_LOCATION_SW;
                    if (WorkOrderList.w.GATEINDTE != null)
                    {
                        //COleDateTime t = pRs->Fields->GetItem("GATEINDTE")->Value;
                        Equipment.GateInDate = WorkOrderList.w.GATEINDTE;
                    }
                    if (WorkOrderList.w.REFRBDAT != null)
                    {
                        Equipment.RefurbishmnentDate = WorkOrderList.w.REFRBDAT;
                    }

                    if (WorkOrderList.w.DELDATSH != null)
                    {
                        Equipment.Deldatsh = WorkOrderList.w.DELDATSH;
                    }

                    if (WorkOrderList.w.FIXCOVER != null)
                    {
                        Equipment.Fixcover = Math.Round(WorkOrderList.w.FIXCOVER.Value, 4); ;
                    }
                    else
                    {
                        Equipment.Fixcover = 0.0;
                    }


                    //	   		WorkOrder.DPP=  WorkOrderList.w.DPP;
                    if (WorkOrderList.w.DPP != null)
                    {
                        WorkOrderList.w.DPP = Math.Round(WorkOrderList.w.DPP.Value, 4);
                        Equipment.Dpp = WorkOrderList.w.DPP;
                    }
                    else
                    {
                        Equipment.Dpp = 0.0;
                    }

                    WorkOrder.EquipmentList.Add(Equipment);



                    // ADO tweak...This is not working from the string utility call. 

                    //	   		WorkOrder.SALES_TAX_LABOR_PCT=  WorkOrderList.w.SALES_TAX_LABOR_PCT;
                    if (WorkOrderList.w.SALES_TAX_LABOR_PCT != null)
                    {
                        WorkOrder.SalesTaxLaborPCT = Math.Round(WorkOrderList.w.SALES_TAX_LABOR_PCT.Value, 4); ;
                    }
                    else
                    {
                        WorkOrder.SalesTaxLaborPCT = 0.0;
                    }


                    //	   		WorkOrder.SALES_TAX_PARTS_PCT=  WorkOrderList.w.SALES_TAX_PARTS_PCT;
                    if (WorkOrderList.w.SALES_TAX_PARTS_PCT != null)
                    {
                        WorkOrder.SalesTaxPartsPCT = Math.Round(WorkOrderList.w.SALES_TAX_PARTS_PCT.Value, 4); ;
                    }
                    else
                    {
                        WorkOrder.SalesTaxPartsPCT = 0.0;
                    }


                    //			WorkOrder.IMPORT_TAX_PCT=  WorkOrderList.w.IMPORT_TAX_PCT;

                    //			d = pRs->Fields->GetItem("IMPORT_TAX_PCT")->Value.fltVal;
                    // ADO tweak...This is not working from the string utility call. 
                    if (WorkOrderList.w.IMPORT_TAX_PCT != null)
                    {
                        WorkOrder.ImportTaxPCT = Math.Round(WorkOrderList.w.IMPORT_TAX_PCT.Value, 4); ;
                    }
                    else
                    {
                        WorkOrder.ImportTaxPCT = 0.0;
                    }

                    // Agent tax on parts
                    if (WorkOrderList.w.AGENT_PARTS_TAX != null)
                    {
                        WorkOrder.AgentPartsTax = Math.Round(WorkOrderList.w.AGENT_PARTS_TAX.Value, 4);
                    }
                    else
                    {
                        WorkOrder.AgentPartsTax = 0;
                    }

                    if (WorkOrderList.w.AGENT_PARTS_TAX_CPH != null)
                    {
                        WorkOrder.AgentPartsTaxCPH = Math.Round(WorkOrderList.w.AGENT_PARTS_TAX_CPH.Value, 4);
                    }
                    else
                    {
                        WorkOrder.AgentPartsTaxCPH = 0;
                    }

                    // Get FACT interface values (2005-09-12)
                    WorkOrder.CountryCUCDN = WorkOrderList.w.COUNTRY_CUCDN;

                    // call below failing.
                    WorkOrder.CountryExchangeRate = WorkOrderList.w.COUNTRY_EXCHANGE_RATE;

                    WorkOrder.CountryExchangeDate = WorkOrderList.w.COUNTRY_EXCHANGE_DTE;

                    // New RKEM damage value - VJP 2006-07-17
                    //WorkOrder.DAMAGE=  WorkOrderList.w.DAMAGE;
                    // VJP - added RKEM fields. 2006-07-25
                    WorkOrder.LeaseComp = WorkOrderList.w.LSCOMP;
                    WorkOrder.LeaseContract = WorkOrderList.w.LSCONTR;

                    //RQ 6361 & RQ 6362 (satadal)
                    // Added for two new RKEM fields

                    WorkOrder.PresentLoc = WorkOrderList.w.PRESENTLOC;




                    if (WorkOrderList.w.prev_wo_id != null)
                    {
                        WorkOrder.PrevWorkOrderID = WorkOrderList.w.prev_wo_id;
                    }
                    //else
                    //{
                    //    WorkOrder.PrevWorkOrderID = "";
                    //}
                    if (WorkOrderList.w.PREV_STATUS != null)
                    {
                        WorkOrder.PrevStatus = WorkOrderList.w.PREV_STATUS;
                        //if (WorkOrder.PrevStatus != null)
                        //    WorkOrder.PrevStatus = null;
                    }
                    else
                    {
                        WorkOrder.PrevStatus = null;
                    }

                    if (WorkOrderList.w.PREV_LOC_CD != null)
                    {
                        WorkOrder.PrevLocCode = WorkOrderList.w.PREV_LOC_CD;
                    }
                    else
                    {
                        WorkOrder.PrevLocCode = string.Empty;
                    }

                    if (WorkOrderList.w.PREV_DATE != null)
                    {
                        WorkOrder.PrevDate = WorkOrderList.w.PREV_DATE;
                    }
                    else
                    {
                        //WorkOrder.PrevDate = "";
                    }

                    WorkOrder.PayAgentCode = RSAllCSM(WorkOrder.Shop.Customer[0].CustomerCode, WorkOrder.Shop.ShopCode, WorkOrder.Mode);

                }
                #endregion Data Mapper
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return WorkOrder;
        }

        public List<RepairsView> LoadRepairsDetails(int WOID)
        {
            List<RepairsView> RepairsViewList = new List<RepairsView>();
            try
            {
                //List<MESC1TS_WOREPAIR> RepairListFromDB = GetRepairListDetails(WOID);

                var RepairListFromDB = (from R in objContext.MESC1TS_WOREPAIR
                                        join Rep in objContext.MESC1TS_REPAIR_CODE on R.REPAIR_CD equals Rep.REPAIR_CD
                                        where R.WO_ID == WOID &&
                                        R.MODE == Rep.MODE
                                        select new
                                        {
                                            Rep.REPAIR_DESC,
                                            Rep.MAX_QUANTITY,
                                            R
                                        }).ToList();

                #region Data Mapper
                if (RepairListFromDB != null && RepairListFromDB.Count > 0)
                {
                    foreach (var item in RepairListFromDB)
                    {
                        RepairsView Repairs = new RepairsView();
                        Repairs.RepairCode = new RepairCode();
                        Repairs.RepairLocationCode = new RepairLoc();
                        Repairs.Damage = new Damage();
                        Repairs.Tpi = new Tpi();
                        Repairs.NonsCode = new NonsCode();
                        Repairs.RepairCode.Mode = new Mode();
                        Repairs.rState = (int)Validation.STATE.EXISTING;
                        Repairs.WorkOrderID = item.R.WO_ID;
                        Repairs.RepairCode.RepairCod = item.R.REPAIR_CD.Trim();
                        Repairs.RepairCode.ManualCode = item.R.MANUAL_CD;
                        Repairs.RepairCode.Mode.ModeCode = item.R.MODE;
                        Repairs.RepairCode.RepairDesc = item.REPAIR_DESC;
                        Repairs.RepairCode.MaxQuantity = item.MAX_QUANTITY;
                        Repairs.IsRepairTaxCode = IsRepairTaxCode(Repairs.RepairCode);
                        //			Repairs.Pieces = item.QTY_REPAIRS;
                        if (item.R.QTY_REPAIRS != null)
                        {
                            Repairs.Pieces = (int)item.R.QTY_REPAIRS; ;
                        }
                        else
                        {
                            Repairs.Pieces = 0;
                        }


                        //			Repairs.MaterialAmt = item.R..SHOP_MATERIAL_AMT;
                        if (item.R.SHOP_MATERIAL_AMT != null)
                        {
                            Repairs.MaterialCost = Math.Round(item.R.SHOP_MATERIAL_AMT.Value, 4);
                        }
                        else
                        {
                            Repairs.MaterialCost = 0;
                        }


                        //			Repairs.MaterialAmtCPH = item.R..CPH_MATERIAL_AMT;
                        if (item.R.CPH_MATERIAL_AMT != null)
                        {
                            Repairs.MaterialCostCPH = Math.Round(item.R.CPH_MATERIAL_AMT.Value, 4);
                        }
                        else
                        {
                            Repairs.MaterialCostCPH = 0;
                        }

                        //			Repairs.ManHrs = item.R..ACTUAL_MANH;
                        if (item.R.ACTUAL_MANH != null)
                        {
                            Repairs.ManHoursPerPiece = Math.Round(item.R.ACTUAL_MANH.Value, 4);
                        }
                        else
                        {
                            Repairs.ManHoursPerPiece = 0;
                        }


                        //Mangal Release 3 Loading damage code, repair location code and Tpi code
                        Repairs.Damage.DamageCedexCode = item.R.DAMAGE_CD;
                        Repairs.RepairLocationCode.CedexCode = item.R.REPAIR_LOC_CD;
                        Repairs.Tpi.CedexCode = item.R.TPI_CD;

                        //Mangal
                        Repairs.NonsCode.NonsCd = item.R.NONS_CD;
                        Repairs.RepairCode.ChangeUser = item.R.CHUSER;
                        RepairsViewList.Add(Repairs);
                    }
                }
                #endregion Data Mapper
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return RepairsViewList;
        }

        public List<SparePartsView> LoadPartsDetails(int WOID)
        {
            List<SparePartsView> SparePartsList = new List<SparePartsView>();

            try
            {
                //List<MESC1TS_WOPART> SparePartsListFromDB = GetSparePartListDetails(WOID);
                var SparePartsListFromDB = (from wp in objContext.MESC1TS_WOPART
                                            from p in objContext.MESC1TS_MASTER_PART
                                            where //(wp.SERIAL_NUMBER == null || wp.SERIAL_NUMBER == "") &&


                                            wp.PART_CD == p.PART_CD &&
                                            wp.WO_ID == WOID
                                            select new
                                            {
                                                p.CORE_VALUE,
                                                p.PART_DESC,
                                                p.MSL_PART_SW,
                                                p.CORE_PART_SW,
                                                wp
                                            }).ToList();
                #region DataMapper
                if (SparePartsListFromDB != null && SparePartsListFromDB.Count > 0)
                {
                    foreach (var item in SparePartsListFromDB)
                    {
                        SparePartsView SpareParts = new SparePartsView();
                        SpareParts.RepairCode = new RepairCode();
                        SpareParts.pState = (int)Validation.STATE.EXISTING;
                        SpareParts.WorkOrderID = item.wp.WO_ID;
                        SpareParts.RepairCode.RepairCod = item.wp.REPAIR_CD.Trim();
                        SpareParts.OwnerSuppliedPartsNumber = item.wp.PART_CD;
                        SpareParts.CostLocal = item.wp.COST_LOCAL;
                        SpareParts.CostLocalCPH = item.wp.COST_CPH;
                        SpareParts.CoreValue = item.CORE_VALUE;
                        SpareParts.PartDescription = item.PART_DESC;
                        SpareParts.MslPartSW = item.MSL_PART_SW;
                        SpareParts.CORE_PART_SW = Char.Parse(item.CORE_PART_SW);
                        //SpareParts.SerialNumber = item.wp.SERIAL_NUMBER;
                        if (!string.IsNullOrEmpty(item.wp.SERIAL_NUMBER))
                        {
                            SpareParts.SerialNumber = item.wp.SERIAL_NUMBER;
                        }
                        else
                        {
                            SpareParts.SerialNumber = string.Empty;
                        }
                        if (item.wp.QTY_PARTS != null)
                        {
                            //d = (double)item.wp.QTY_PARTS;
                            //sprintf(cNum,"%.2f", d);
                            SpareParts.Pieces = (int)item.wp.QTY_PARTS;
                        }
                        else
                        {
                            SpareParts.Pieces = 0;
                        }
                        //SpareParts.CHUser = item.wp.CHUSER;

                        SparePartsList.Add(SpareParts);
                    }
                }
                #endregion DataMapper
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return SparePartsList;
        }

        public List<RemarkEntry> LoadRemarksDetails(int WOID)
        {
            List<RemarkEntry> RemarksList = new List<RemarkEntry>();
            try
            {
                List<MESC1TS_WOREMARK> RemarksListFromDB = GetRemarksListDetails(WOID);
                #region DataMapper
                if (RemarksListFromDB != null && RemarksListFromDB.Count > 0)
                {
                    foreach (var item in RemarksListFromDB)
                    {
                        RemarkEntry RemarkEntry = new RemarkEntry();
                        RemarkEntry.RemarkID = item.WOREMARK_ID;
                        RemarkEntry.WorkOrderID = item.WO_ID;
                        RemarkEntry.RemarkType = item.REMARK_TYPE;
                        RemarkEntry.SuspendCatID = item.SUSPCAT_ID;
                        RemarkEntry.Remark = item.REMARK;

                        if (item.XMIT_DTE != null)
                        {
                            RemarkEntry.XMITDate = item.XMIT_DTE;
                        }
                        else
                        {
                            RemarkEntry.XMITDate = DateTime.MinValue;
                        }


                        RemarkEntry.ChangeUser = item.CHUSER;
                        RemarkEntry.CRTSDate = item.CRTS.ToString();
                        RemarkEntry.rState = (int)Validation.STATE.EXISTING;
                        RemarksList.Add(RemarkEntry);
                    }
                }
                #endregion DataMapper
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return RemarksList;
        }

        public List<Equipment> GetEquipmentDetailsOnWOID(int WOID, string Mode)
        {
            List<Equipment> EquipmentList = new List<Equipment>();
            try
            {
                var WorkOrder = (from wo in objContext.MESC1TS_WO
                                 where wo.WO_ID == WOID
                                 select wo).FirstOrDefault();

                Equipment Eqp = new Equipment();
                Eqp.EquipmentNo = WorkOrder.EQPNO;
                Eqp.Size = WorkOrder.EQSIZE;
                Eqp.Type = WorkOrder.EQTYPE;
                Eqp.VendorRefNo = WorkOrder.VENDOR_REF_NO;
                Eqp.Eqouthgu = WorkOrder.EQOUTHGU;

                Eqp.COType = WorkOrder.COTYPE;
                Eqp.Eqstype = WorkOrder.EQSTYPE;
                Eqp.Eqowntp = WorkOrder.EQOWNTP;
                Eqp.Eqmatr = WorkOrder.EQMATR;
                Eqp.EqMancd = WorkOrder.EQMANCD;
                Eqp.EQProfile = WorkOrder.EQPROFIL;
                Eqp.EQInDate = WorkOrder.EQINDAT;

                Eqp.EQRuman = WorkOrder.EQRUMAN;
                Eqp.EQRutyp = WorkOrder.EQRUTYP;
                Eqp.EQIoflt = WorkOrder.EQIOFLT;
                Eqp.ReqRemarkSW = WorkOrder.REQ_REMARK_SW;
                Eqp.StEmptyFullInd = WorkOrder.STEMPTY; ;
                Eqp.Strefurb = WorkOrder.STREFURB;
                Eqp.Stredel = WorkOrder.STREDEL;
                Eqp.STSELSCR = WorkOrder.STSELSCR;
                Eqp.Fixcover = WorkOrder.FIXCOVER;
                Eqp.OffhirLocationSW = WorkOrder.OFFHIR_LOCATION_SW;
                if (WorkOrder.GATEINDTE != null)
                {
                    //COleDateTime t = pRs->Fields->GetItem("GATEINDTE")->Value;
                    Eqp.GateInDate = WorkOrder.GATEINDTE;
                }
                if (WorkOrder.REFRBDAT != null)
                {
                    Eqp.RefurbishmnentDate = WorkOrder.REFRBDAT;
                }

                if (WorkOrder.DELDATSH != null)
                {
                    Eqp.Deldatsh = WorkOrder.DELDATSH;
                }

                if (WorkOrder.FIXCOVER != null)
                {
                    Eqp.Fixcover = Math.Round(WorkOrder.FIXCOVER.Value, 4); ;
                }
                else
                {
                    Eqp.Fixcover = 0.0;
                }


                //	   		WorkOrder.DPP=  WorkOrder.DPP;
                if (WorkOrder.DPP != null)
                {
                    WorkOrder.DPP = Math.Round(WorkOrder.DPP.Value, 4);
                    Eqp.Dpp = WorkOrder.DPP;
                }
                else
                {
                    Eqp.Dpp = 0.0;
                }
                Eqp.SelectedMode = Mode;
                EquipmentList.Add(Eqp);
            }
            catch
            {
            }

            return EquipmentList;
        }

        private List<MESC1TS_WO> GetWorkOrderHeaderDetails(int WOID)
        {
            List<MESC1TS_WO> WorkOrder = new List<MESC1TS_WO>();

            try
            {
                var WorkOrder1 = (from w in objContext.MESC1TS_WO
                                  join shop in objContext.MESC1TS_SHOP on w.SHOP_CD equals shop.SHOP_CD
                                  where w.WO_ID == WOID
                                  select new
                                  {
                                      shop.SHOP_CD,
                                      shop.SHOP_DESC,
                                      w
                                  }).ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return WorkOrder;
        }

        private List<MESC1TS_WOREPAIR> GetRepairListDetails(int WOID)
        {
            List<MESC1TS_WOREPAIR> RepairList = new List<MESC1TS_WOREPAIR>();
            try
            {
                RepairList = (from R in objContext.MESC1TS_WOREPAIR
                              where R.WO_ID == WOID
                              select R).ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return RepairList;
        }

        private List<MESC1TS_WOPART> GetSparePartListDetails(int WOID)
        {
            List<MESC1TS_WOPART> PartsList = new List<MESC1TS_WOPART>();
            try
            {
                PartsList = (from P in objContext.MESC1TS_WOPART
                             where P.WO_ID == WOID
                             select P).ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return PartsList;
        }

        private List<MESC1TS_WOREMARK> GetRemarksListDetails(int WOID)
        {
            List<MESC1TS_WOREMARK> RemarksList = new List<MESC1TS_WOREMARK>();
            try
            {
                RemarksList = (from R in objContext.MESC1TS_WOREMARK
                               where R.WO_ID == WOID
                               select R).OrderByDescending(remarks => remarks.CRTS).ToList();
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return RemarksList;
        }

        public List<RepairsView> GetHours(WorkOrderDetail WorkOrder, out List<ErrMessage> ErrorMessageList)
        {
            logEntry.Message = "Method Name : GetHours(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            //string ManualCode = string.Empty;
            double ManHours = 0.0;
            ErrorMessageList = new List<ErrMessage>();
            bool success = true;
            string CustCode = WorkOrder.Shop.Customer[0].CustomerCode;


            //First load the manual details on the basis of shop and customer
            //"SELECT DISTINCT CS.SHOP_CD,	C.CUSTOMER_CD, C.MANUAL_CD, C.CUSTOMER_DESC, C.MAERSK_SW ";
            //sSQL += "FROM MESC1VS_CUST_SHOP CS, MESC1TS_CUSTOMER C ";
            //sSQL += "WHERE C.CUSTOMER_CD = CS.CUSTOMER_CD AND	C.CUSTOMER_ACTIVE_SW =  'Y'";
            //sSQL += " AND CS.SHOP_CD = '";
            //sSQL += tShop;
            //sSQL += "' AND C.CUSTOMER_CD = '";
            //sSQL += tCustomerCode;
            //sSQL += "'";
            try
            {
                var LoadByShopAndCustomer = (from CS in objContext.MESC1VS_CUST_SHOP
                                             from C in objContext.MESC1TS_CUSTOMER
                                             where C.CUSTOMER_CD == CS.CUSTOMER_CD &&
                                             C.CUSTOMER_ACTIVE_SW == "Y" &&
                                             CS.SHOP_CD == WorkOrder.Shop.ShopCode &&
                                             C.CUSTOMER_CD == CustCode
                                             select new
                                             {
                                                 CS.SHOP_CD,
                                                 C.CUSTOMER_CD,
                                                 C.MANUAL_CD,
                                                 C.CUSTOMER_DESC,
                                                 C.MAERSK_SW
                                             }).Distinct().FirstOrDefault();

                if (LoadByShopAndCustomer != null)
                {
                    WorkOrder.Shop.Customer[0].ManualCode = LoadByShopAndCustomer.MANUAL_CD;
                    WorkOrder.Shop.Customer[0].MaerskSw = LoadByShopAndCustomer.MAERSK_SW;
                }
                else
                {
                    Message = new ErrMessage();
                    Message.Message = GetErrorMessage(20025);
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrorMessageList.Add(Message);
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System Error on getting ManHours";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                ErrorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;

            }

            //Den get the man hours on the basis of the mode and the manual
            //"SELECT RC.MANUAL_CD,RC.MODE,RC.REPAIR_CD,RC.REPAIR_DESC,RE.EXCLU_REPAIR_CD,";
            //tSQL += "RC.MAX_QUANTITY,RC.MAN_HOUR,RC.REPAIR_IND,RC.SHOP_MATERIAL_CEILING, RC.TAX_APPLIED_SW, RC.ALLOW_PARTS_SW ";
            //tSQL += "FROM MESC1TS_REPAIR_CODE RC left outer join MESC1TS_RPRCODE_EXCLU RE on RE.MANUAL_CD = RC.MANUAL_CD ";
            //tSQL += "and RE.MODE = RC.MODE AND RE.REPAIR_CD = RC.REPAIR_CD ";
            //tSQL += "WHERE RC.REPAIR_ACTIVE_SW = 'Y' AND ";
            //tSQL += "RC.MANUAL_CD = '"; tSQL += tManualCode; tSQL += "' AND ";
            //tSQL += "RC.MODE = '"; tSQL += tModeCode; tSQL += "' ORDER BY RC.MANUAL_CD, RC.MODE";
            if (success)
            {
                try
                {
                    string Mode = WorkOrder.Mode;
                    string ManualCode = WorkOrder.Shop.Customer[0].ManualCode;
                    var LoadByManualAndMode = (from RC in objContext.MESC1TS_REPAIR_CODE
                                               join RE in objContext.MESC1TS_RPRCODE_EXCLU on new { RC1 = RC.MANUAL_CD, RC2 = RC.MODE, RC3 = RC.REPAIR_CD.Trim() }
                                               equals new { RC1 = RE.MANUAL_CD, RC2 = RE.MODE, RC3 = RE.REPAIR_CD.Trim() } into Inner
                                               from O in Inner.DefaultIfEmpty()
                                               where RC.REPAIR_ACTIVE_SW == "Y" &&
                                               RC.MANUAL_CD == ManualCode &&
                                               RC.MODE == Mode
                                               select new
                                               {
                                                   RC.MANUAL_CD,
                                                   RC.MODE,
                                                   RC.REPAIR_CD,
                                                   RC.REPAIR_DESC,
                                                   O.EXCLU_REPAIR_CD,
                                                   RC.MAX_QUANTITY,
                                                   RC.MAN_HOUR,
                                                   RC.REPAIR_IND,
                                                   RC.SHOP_MATERIAL_CEILING,
                                                   RC.TAX_APPLIED_SW,
                                                   RC.ALLOW_PARTS_SW
                                               });

                    //WorkOrder.RepairsViewList = new List<RepairsView>();
                    //RepairsView rv = new RepairsView();
                    //rv.RepairCode = new RepairCode();
                    //rv.RepairCode.RepairCod = "1110";
                    //WorkOrder.RepairsViewList.Add(rv);
                    if (LoadByManualAndMode != null) // if table is not empty...// iterate through collection of repair codes.			                                    	
                    {
                        foreach (var rItem in WorkOrder.RepairsViewList)
                        {
                            if (rItem.rState == (int)Validation.STATE.DELETED) continue;
                            if (!IsRepairTaxCode(rItem.RepairCode))
                            {
                                if (rItem.rState != (int)Validation.STATE.DISCARDED)
                                {
                                    //Check if the repair code is present in the list retrieved on the basis of mode and manual
                                    if (LoadByManualAndMode.Any(RepCode => RepCode.REPAIR_CD.Trim() == rItem.RepairCode.RepairCod.Trim()))
                                    {
                                        var SelectedRepairCodeFromDB = LoadByManualAndMode.FirstOrDefault(RepCode => RepCode.REPAIR_CD.Trim() == rItem.RepairCode.RepairCod.Trim());
                                        if (SelectedRepairCodeFromDB.MAN_HOUR == null)
                                        {
                                            ManHours = (float)0.0;
                                        }
                                        else
                                        {
                                            ManHours = (double)(SelectedRepairCodeFromDB.MAN_HOUR.Value);
                                        }
                                        rItem.ManHoursPerPiece = ManHours;
                                        rItem.ManHoursPerPiece = Math.Round(rItem.ManHoursPerPiece.Value, 2);
                                    }
                                    else
                                    {
                                        rItem.RepairCode.RepairCodeNotFound = "Repair Code not found";
                                    }
                                }
                            }
                        }
                    }
                    else // no records in table, therefore all Repair codes are not active...	
                    {
                        foreach (var ritem in WorkOrder.RepairsViewList)
                        {
                            if (ritem.rState == (int)Validation.STATE.DELETED) continue;
                            if (!IsRepairTaxCode(ritem.RepairCode))
                            {
                                Message = new ErrMessage();
                                Message.Message = GetErrorMessage(20065) + " " + ritem.RepairCode.RepairCod.Trim();
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                ErrorMessageList.Add(Message);
                            }
                        }
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                    throw new DbEntityValidationException(errorMessages);
                }
                catch (Exception ex)
                {
                    Message = new ErrMessage();
                    Message.Message = "System Error on getting ManHours";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrorMessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    success = false;
                }
            }
            logEntry.Message = "Method Name : GetHours(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            return WorkOrder.RepairsViewList;
        }

        private bool CheckThirdPartyPort(string thirdPartyPort, out List<ErrMessage> errorMessageList)
        {
            bool success = false;
            errorMessageList = new List<ErrMessage>();

            try
            {
                //SELECT LOC_CD FROM MESC1TS_LOCATION WHERE LOC_CD = 'ThirdPP'
                var locationList = (from loc in objContext.MESC1TS_LOCATION
                                    where loc.LOC_CD == thirdPartyPort
                                    select new
                                    {
                                        loc.LOC_CD
                                    }).ToList();

                if (locationList != null && locationList.Count > 0)
                {
                    success = true;
                }
                else
                {
                    Message = new ErrMessage();
                    Message.Message = "Invalid Third Party Port";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }

        private bool IsValidNumericSize(string value, int wholeNum, int Decimal)
        {
            //string c = new string(new char[5]);
            string s = value;
            int i = 0;

            int nDecCtr = 0;
            for (i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (!char.IsDigit(c))
                {
                    if (c == '.')
                    {
                        nDecCtr++;
                    }
                    else
                    { // if is a sign value and is first character
                        if ((c == '-') && (i != 0))
                        {
                            return (false);
                        }
                    }
                }
            }
            // fail if multiple decimals entered.
            if (nDecCtr > 1)
            {
                return (false);
            }

            i = s.IndexOf(".", 0);
            // assume entered as whole number.
            // if decimal found
            if (i != -1)
            {
                if (i > wholeNum)
                {
                    return (false);
                }
            }
            else // no decimal - check total size.
            {
                if (s.Length > wholeNum)
                {
                    return (false);
                }
            }

            // get length of decimal, if decimal found
            if (i != -1)
            {
                string s1 = s.Substring(i + 1);
                if (s1.Length > Decimal)
                {
                    return (false);
                }
            }
            return (true);

        }

        private bool GetDiscountPercent(string Manufacturer)
        {
            bool success = true;
            m_fDiscountPercent = 0;
            //DiscPercent = 0;

            try
            {
                var DiscountByMfg = (from m in objContext.MESC1TS_MANUFACTUR
                                     where m.MANUFCTR == Manufacturer
                                     select new
                                     {
                                         m.DISCOUNT_PERCENT
                                     }).FirstOrDefault();

                if (DiscountByMfg != null)
                {
                    // Get discount percent for part price calculations. Entered as whole percentage, translate into decimal format
                    m_fDiscountPercent = (DiscountByMfg.DISCOUNT_PERCENT == null ? 0 : (decimal)DiscountByMfg.DISCOUNT_PERCENT);
                    m_fDiscountPercent = m_fDiscountPercent * ((decimal)0.01);
                    //DiscPercent = m_fDiscountPercent;
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "Failed open of Manufacturer recordset. System Error on validating Manufacturer REFU and GENS";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }
            return success;
        }

        private bool GetDiscountPercent1(string ShopCode, string Manufacturer)
        {
            m_fMarkUp = 0;
            bool success = false;
            try
            {
                var Discount = (from disc in objContext.MESC1TS_DISCOUNT
                                where disc.SHOP_CD == ShopCode &&
                                disc.MANUFCTR == Manufacturer
                                select new
                                {
                                    disc.DISCOUNT
                                }).FirstOrDefault();
                //decimal? disc = 0;
                if (Discount != null)
                {
                    m_fMarkUp = (decimal)(Discount.DISCOUNT == null ? 0 : Discount.DISCOUNT);
                    m_fMarkUp = m_fMarkUp * ((decimal)0.01);
                    success = true;
                }
            }

            catch
            {
                Message = new ErrMessage();
                Message.Message = " m_pShopManager->RSByDiscount1: Failed open of Discount recordset. ";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                success = false;
            }

            return success;
        }

        //private string BuildPrepTimeFilter(string repCode, string Mode)
        //{
        //    string sFIlter = string.Empty;

        //    string sPrepCode = repCode.TrimEnd();
        //    if (string.IsNullOrEmpty(sPrepCode))
        //    {
        //        sFIlter = "(MODE = \'" + Mode + " \')";
        //    }
        //    else
        //    {
        //        sFIlter = "(MODE = \'" + Mode + "\') and (PREP_CD = \'" + repCode + "\')";
        //    }

        //    return sFIlter;
        //}

        private void DistributePrepTime(WorkOrderDetail workOrder, double? fPrepTime)
        {
            double? fBalance = 0.0;
            // get hourly values in float format
            double? fRegHours = workOrder.TotalManHourReg;
            double? fOtHours = workOrder.TotalManHourOverTime;
            double? fDbHours = workOrder.TotalManHourDoubleTime;
            double? fMsHours = workOrder.TotalManHourMisc;

            try
            {
                // add up to size of hours field and subtract balance, continue adding to each additional hourly type
                // in same manner. Do not add to zero hourly values.
                if (fRegHours > 0.0)
                {
                    if (fPrepTime < fRegHours)
                    {
                        fRegHours += fPrepTime;
                        fPrepTime = 0;
                    }
                    else
                    {	// decrement prep time by Reg hours. This will double regular hours, and leave remaining prep time.	
                        fBalance = fPrepTime - fRegHours;
                        fRegHours += fRegHours;
                        fPrepTime = fBalance;
                    }
                }

                if (fOtHours > 0.0)
                {
                    if (fPrepTime < fOtHours)
                    {
                        fOtHours += fPrepTime;
                        fPrepTime = 0;
                    }
                    else
                    {	// decrement prep time by Reg hours. This will double regular hours, and leave remaining prep time.	
                        fBalance = fPrepTime - fOtHours;
                        fOtHours += fOtHours;
                        fPrepTime = fBalance;
                    }
                }

                if (fDbHours > 0.0)
                {
                    if (fPrepTime < fDbHours)
                    {
                        fDbHours += fPrepTime;
                        fPrepTime = 0;
                    }
                    else
                    {	// decrement prep time by Reg hours. This will double regular hours, and leave remaining prep time.	
                        fBalance = fPrepTime - fDbHours;
                        fDbHours += fDbHours;
                        fPrepTime = fBalance;
                    }
                }

                if (fMsHours > 0.0)
                {
                    if (fPrepTime < fMsHours)
                    {
                        fMsHours += fPrepTime;
                        fPrepTime = 0;
                    }
                    else
                    {	// decrement prep time by Reg hours. This will double regular hours, and leave remaining prep time.	
                        fBalance = fPrepTime - fMsHours;
                        fMsHours += fMsHours;
                        fPrepTime = fBalance;
                    }
                }

                // any remaining prep time add to first non-zero value. Unlikely, but possible (safe code)
                if (fPrepTime > 0.0)
                {
                    if (fRegHours > 0.0)
                    {
                        fRegHours += fPrepTime;
                        fPrepTime = 0;
                    }
                    if (fOtHours > 0.0)
                    {
                        fOtHours += fPrepTime;
                        fPrepTime = 0;
                    }
                    if (fDbHours > 0.0)
                    {
                        fDbHours += fPrepTime;
                        fPrepTime = 0;
                    }
                    if (fMsHours > 0.0)
                    {
                        fMsHours += fPrepTime;
                        fPrepTime = 0;
                    }
                }

                // re-format values in rhsRecord.
                workOrder.TotalManHourReg = Math.Round(fRegHours.Value, 2);
                workOrder.TotalManHourOverTime = Math.Round(fOtHours.Value, 2);
                workOrder.TotalManHourDoubleTime = Math.Round(fDbHours.Value, 2);
                workOrder.TotalManHourMisc = Math.Round(fMsHours.Value, 2);
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }


        private bool GetFindDuplicateRepairCodeSQL(WorkOrderDetail workOrder)
        {
            List<string> FILTERREPAIRCODES = new List<string>();

            try
            {

            }

            catch
            {
            }

            return true;
        }

        public List<string> GetFilterRepairCodes()
        {
            List<string> FilteredRepairCodes = new List<string>();
            FilteredRepairCodes.Add("0003");
            FilteredRepairCodes.Add("0005");
            FilteredRepairCodes.Add("0008");
            FilteredRepairCodes.Add("0011");
            FilteredRepairCodes.Add("0014");
            FilteredRepairCodes.Add("0017");
            FilteredRepairCodes.Add("0075");
            FilteredRepairCodes.Add("0076");
            FilteredRepairCodes.Add("0077");

            return FilteredRepairCodes;
        }

        // Created By Afroz
        public List<Customer> GetCustomerCodeByShopCode(string ShopCode, int UserId)
        {
            try
            {
                //objContext = new CreateWorkOrderServiceEntities();
                List<Customer> Customerlist = new List<Customer>();
                List<MESC1TS_CUSTOMER> CustomerFromDB = new List<MESC1TS_CUSTOMER>();
                if (string.IsNullOrEmpty(ShopCode))
                {
                    CustomerFromDB = (from C in objContext.MESC1TS_CUSTOMER
                                      join CS in objContext.MESC1VS_CUST_SHOP on C.CUSTOMER_CD equals CS.CUSTOMER_CD
                                      //join A in objContext.SEC_AUTHGROUP_USER on CS.SHOP_CD equals A.COLUMN_VALUE
                                      //where A.USER_ID == UserId
                                      select C).Distinct().ToList();
                }

                else
                {
                    CustomerFromDB = (from C in objContext.MESC1TS_CUSTOMER
                                      join CS in objContext.MESC1VS_CUST_SHOP on C.CUSTOMER_CD equals CS.CUSTOMER_CD
                                      join A in objContext.SEC_AUTHGROUP_USER on CS.SHOP_CD equals A.COLUMN_VALUE
                                      where A.USER_ID == UserId && CS.SHOP_CD == ShopCode
                                      select C).Distinct().ToList();
                }

                foreach (var item in CustomerFromDB)
                {
                    Customer customer = new Customer();
                    customer.CustomerCode = item.CUSTOMER_CD;
                    Customerlist.Add(customer);
                }
                return Customerlist;
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                throw;
            }

        }
        public List<EqType> GetEquipmentType()
        {
            try
            {
                //objContext = new CreateWorkOrderServiceEntities();
                List<EqType> EquipmentTypeList = new List<EqType>();
                // List<MESC1TS_EQTYPE> DamageFromDB = new List<MESC1TS_EQTYPE>();
                var DamageFromDB = (from E in objContext.MESC1TS_EQTYPE
                                    from U in objContext.SEC_USER
                                    .Where(U => U.LOGIN == E.CHUSER)
                                    .DefaultIfEmpty()
                                    select new { E }).Distinct().ToList();

                foreach (var obj in DamageFromDB)
                {
                    EqType EqpType = new EqType();
                    EqpType.EqpType = obj.E.EQTYPE;
                    EqpType.EqTypeDesc = obj.E.EQTYPE_DESC;
                    EquipmentTypeList.Add(EqpType);
                }
                return EquipmentTypeList;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public string GetVenRefNoByWOID(int WOID)
        {
            string venrefno = "";
            try
            {
                objContext = new ManageWorkOrderServiceEntities();


                var DamageFromDB = (from W in objContext.MESC1TS_WO
                                    where W.WO_ID == WOID

                                    select new { W.VENDOR_REF_NO }).ToList();

                foreach (var obj in DamageFromDB)
                {
                    venrefno = obj.VENDOR_REF_NO;
                }
                return venrefno;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public List<EqsType> GetEquipmentSubType(string EQPType)
        {
            try
            {
                //objContext = new CreateWorkOrderServiceEntities();
                List<EqsType> EquipmentSubTypeList = new List<EqsType>();
                // List<MESC1TS_EQTYPE> DamageFromDB = new List<MESC1TS_EQTYPE>();
                var DamageFromDB = (from ES in objContext.MESC1TS_EQSTYPE
                                    join E in objContext.MESC1TS_EQTYPE
                                    on ES.COTYPE equals E.EQTYPE into D
                                    from U in D.DefaultIfEmpty()
                                    join F in objContext.SEC_USER
                                    on ES.CHUSER equals F.LOGIN
                                    where ES.COTYPE == EQPType
                                    select new { ES }).Distinct().ToList();

                foreach (var obj in DamageFromDB)
                {
                    EqsType EqpType = new EqsType();
                    EqpType.CoType = obj.ES.COTYPE;
                    EqpType.EqSType = obj.ES.EQSTYPE;
                    EqpType.TypeDesc = obj.ES.TYPE_DESC;
                    EquipmentSubTypeList.Add(EqpType);
                }
                return EquipmentSubTypeList;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public List<WorkOrderDetail> GetAuditRecord(string WOID)
        {
            //objContext = new CreateWorkOrderServiceEntities();
            List<WorkOrderDetail> AuditList = new List<WorkOrderDetail>();
            int workid = Convert.ToInt32(WOID);
            try
            {

                if (workid != 0)
                {
                    var WOList = (from C in objContext.MESC1TS_WOAUDIT

                                  where C.WO_ID == workid
                                  select new
                                  {
                                      C.WO_ID,
                                      C.AUDIT_TEXT,
                                      C.CHTS,
                                      C.CHUSER
                                  }).ToList();


                    foreach (var obj in WOList)
                    {
                        WorkOrderDetail WO = new WorkOrderDetail();
                        WO.WorkOrderID = obj.WO_ID;
                        WO.Description = obj.AUDIT_TEXT.ToString();
                        WO.ChangeUser = obj.CHUSER;
                        WO.ChangeTime = obj.CHTS;
                        AuditList.Add(WO);
                    };
                }
                return AuditList;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public WorkOrderDetail GetWOAdditionalDetails(string orderNo)
        {
            int WOId = Convert.ToInt32(orderNo);
            //objContext = new CreateWorkOrderServiceEntities();
            WorkOrderDetail OrderDetails = new WorkOrderDetail();
            List<MESC1TS_WO> WO = new List<MESC1TS_WO>();
            OrderDetails.EquipmentList = new List<Equipment>();
            Equipment eqp = new Equipment();
            OrderDetails.Shop = new Shop();
            OrderDetails.Shop.Currency = new Currency();
            try
            {
                WO = (from T in objContext.MESC1TS_WO
                      where T.WO_ID == WOId
                      select T).ToList();

                foreach (var item in WO)
                {
                    OrderDetails.WorkOrderID = item.WO_ID;
                    //OrderDetails.EquipmentList[0].VendorRefNo = item.VENDOR_REF_NO;
                    OrderDetails.ExchangeRate = item.EXCHANGE_RATE;
                    OrderDetails.TotalPrepHours = item.TOT_PREP_HRS;
                    OrderDetails.Shop.CUCDN = item.CUCDN;
                    OrderDetails.Shop.Currency.Cucdn = item.CUCDN;
                    OrderDetails.ManHourRateCPH = item.MANH_RATE_CPH;
                    OrderDetails.ManHourRate = item.MANH_RATE;
                    OrderDetails.TotalManHourReg = item.TOT_MANH_REG;
                    OrderDetails.OverTimeRateCPH = item.OT_RATE_CPH;
                    OrderDetails.OverTimeRate = item.OT_RATE;
                    OrderDetails.TotalManHourOverTime = item.TOT_MANH_OT;
                    OrderDetails.DoubleTimeRateCPH = item.DT_RATE_CPH;
                    OrderDetails.DoubleTimeRate = item.DT_RATE;
                    OrderDetails.TotalManHourDoubleTime = item.TOT_MANH_DT;
                    OrderDetails.MiscRateCPH = item.MISC_RATE_CPH;
                    OrderDetails.MiscRate = item.MISC_RATE;
                    OrderDetails.TotalManHourMisc = item.TOT_MANH_MISC;
                    OrderDetails.TotalLabourCostCPH = item.TOT_LABOR_COST_CPH;
                    OrderDetails.TotalLabourCost = item.TOT_LABOR_COST;
                    OrderDetails.TotalRepairManHour = item.TOT_REPAIR_MANH;
                    OrderDetails.AgentPartsTaxCPH = item.AGENT_PARTS_TAX_CPH;
                    OrderDetails.AgentPartsTax = item.AGENT_PARTS_TAX;
                    OrderDetails.TotalManPartsCPH = item.TOT_MAN_PARTS_CPH;
                    OrderDetails.TotalManParts = item.TOT_MAN_PARTS;
                    OrderDetails.TotalShopAmountCPH = item.TOT_SHOP_AMT_CPH;
                    OrderDetails.TotalShopAmount = item.TOT_SHOP_AMT;
                    OrderDetails.TotalMaerksPartsCPH = item.TOT_MAERSK_PARTS_CPH;
                    OrderDetails.TotalMaerksParts = item.TOT_MAERSK_PARTS;
                    OrderDetails.Shop.ShopCode = item.SHOP_CD;
                    OrderDetails.SalesTaxLaborPCT = item.SALES_TAX_LABOR_PCT;
                    OrderDetails.SalesTaxLabourCPH = item.SALES_TAX_LABOR_CPH;
                    OrderDetails.SalesTaxLabour = item.SALES_TAX_LABOR;
                    OrderDetails.SalesTaxPartsPCT = item.SALES_TAX_PARTS_PCT;
                    OrderDetails.SalesTaxPartsCPH = item.SALES_TAX_PARTS_CPH;
                    OrderDetails.SalesTaxParts = item.SALES_TAX_PARTS;
                    OrderDetails.ImportTaxPCT = item.IMPORT_TAX_PCT;
                    OrderDetails.ImportTaxCPH = item.IMPORT_TAX_CPH;
                    OrderDetails.ImportTax = item.IMPORT_TAX;
                    eqp.VendorRefNo = item.VENDOR_REF_NO;
                    OrderDetails.EquipmentList.Add(eqp);

                }
            }
            catch (Exception)
            {

                throw;
            }

            return OrderDetails;
        }

        public decimal? RSUserByUserId(int Userid)
        {
            //objContext = new CreateWorkOrderServiceEntities();

            decimal? ApprovalAmount = 0;
            try
            {

                var SecUser = (from SU in objContext.SEC_USER
                               where SU.USER_ID == Userid
                               select new { SU.USER_ID, SU.LOGIN, SU.FIRSTNAME, SU.LASTNAME, SU.COMPANY, SU.ACTIVE_STATUS, SU.APPROVAL_AMOUNT, SU.LOC_CD }).ToList();
                if (SecUser.Count() > 0)
                {


                    ApprovalAmount = SecUser[0].APPROVAL_AMOUNT;


                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            return ApprovalAmount;

        }
        public List<ErrMessage> SetWorkingSwitchByWOID(int WOID, string Switch, string ChangeUser)
        {
            List<ErrMessage> errorMessageList = new List<ErrMessage>();
            MESC1TS_WO WO = new MESC1TS_WO();

            try
            {
                // if swithch not = N or Y then return false;
                if (!Switch.Equals("Y", StringComparison.CurrentCultureIgnoreCase) && !Switch.Equals("N", StringComparison.CurrentCultureIgnoreCase))
                {
                    Message = new ErrMessage();
                    Message.Message = "";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    return errorMessageList;
                }
                WO = (from wo in objContext.MESC1TS_WO
                      where wo.WO_ID == WOID
                      select wo).FirstOrDefault();

                if (WO != null)
                {
                    //OldRepairDate = WO.REPAIR_DTE;
                    //if (NewRepairDate != OldRepairDate)
                    {
                        string AuditComment = string.Empty;
                        MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();

                        WO.SHOP_WORKING_SW = Switch;
                        WO.CHUSER = ChangeUser;
                        WO.CHTS = DateTime.Now;
                        objContext.SaveChanges();

                        //if(OldRepairDate == null) 
                        AuditComment = "Work Order: " + WOID + "<b> working switch set to " + Switch + "</b> by " + ChangeUser;
                        WOAudit.WO_ID = WOID;
                        WOAudit.CHTS = DateTime.Now;
                        WOAudit.AUDIT_TEXT = AuditComment;
                        WOAudit.CHUSER = ChangeUser;
                        objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                        int i = objContext.SaveChanges();

                    }
                }


            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "Not all selected estimates were approved; please try again, or approve them manually: <br>";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return errorMessageList;
            }
            return errorMessageList;

        }
        public List<ErrMessage> UpdateCompleteApprovedWO(int WOID, DateTime? NewRepairDate, string ChangeUser)
        {
            List<ErrMessage> errorMessageList = new List<ErrMessage>();
            MESC1TS_WO WO = null;


            try
            {
                // safety - no wo_id - just exit - nothing to do
                //if (WOID == 0) return;

                // check if date is > current date
                // IsExpired returns 0 if equal, 1 is currentdate is > and -1 if current date is <
                if (NewRepairDate > DateTime.Now)
                {
                    Message = new ErrMessage();
                    Message.Message = " Unable to Complete Estimate. Repair date must not be older than 1st day of previous month, nor later than the current date (UTC)";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    return errorMessageList;
                }

                // date is valid and not > than current date, now ensure that date is not less than 1st day of previous month
                // Ensure that date is not < 1st day of previous month.
                // build oldest allowed date.
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                if (month > 1)
                {
                    month -= 1;
                }
                else
                {
                    month = 12;
                    year -= 1;
                }

                DateTime setDate = (DateTime.Now.Date);
                DateTime dt = (DateTime)NewRepairDate;
                //string tempRepDate = dt.ToString();
                //Incomplete

                if (dt.Date < setDate)
                {
                    Message = new ErrMessage();
                    Message.Message = " Unable to Complete Estimate. Repair date must not be older than 1st day of previous month, nor later than the current date (UTC)";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    return errorMessageList;

                }

                DateTime? OldRepairDate;

                WO = (from wo in objContext.MESC1TS_WO
                      where wo.WO_ID == WOID
                      select wo).FirstOrDefault();

                if (WO != null)
                {
                    OldRepairDate = WO.REPAIR_DTE;
                    if (NewRepairDate != OldRepairDate)
                    {
                        string AuditComment = string.Empty;
                        MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();

                        WO.REPAIR_DTE = NewRepairDate;
                        WO.CHUSER = ChangeUser;
                        objContext.SaveChanges();

                        //if(OldRepairDate == null) 
                        AuditComment = "Work Order: " + WOID + "<b> RepairDate changed from " + OldRepairDate + " to " + NewRepairDate + "</b> by " + ChangeUser;
                        WOAudit.WO_ID = WOID;
                        WOAudit.CHTS = DateTime.Now;
                        WOAudit.AUDIT_TEXT = AuditComment;
                        WOAudit.CHUSER = ChangeUser;
                        objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                        objContext.SaveChanges();
                    }
                }



                //if (m_WOList.Size() > 0)
                //{
                //    for (int i = 0; i < m_WOList.Size(); i++)
                //    {
                //        pWO = (CWorkOrderRecord*)m_WOList.GetAt(i);
                //        if (strcmp((char*)tWOID, pWO->GetWO_ID().c_str()) == 0)
                //        {
                //            STATE stat = pWO->GetObjectState();
                //            pWO->SetREPAIR_DTE((char*)tRepairDte);
                //            pWO->SetObjectState(stat);
                //            break;
                //        }
                //    }
                //}
            }

            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "Failed update on work order - UpdateRepairDate.";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return errorMessageList;

            }

            return errorMessageList;
        }
        public string UpdateApproveWorkOrder(int WOID, string User, string OldStatusOrRemark, string VendorRefNo)
        {
            MESC1TS_WO PrevWODataFromDB = new MESC1TS_WO();
            string LocCode = string.Empty;
            int? PrevID = 0;
            string Date = string.Empty;
            short? StatusCodeTemp = 0;
            short? Status = 390;
            string message = "";
            //var PrevWOID;
            MESC1TS_WOAUDIT WOAudit = null;
            MESC1TS_WOREMARK WORemark = null;
            List<ErrMessage> errorMessageList = new List<ErrMessage>();
            try
            {
                //if (Status == APPROVEDSTATUS)
                //{
                var PrevWOData = (from W in objContext.MESC1TS_WO
                                  where W.WO_ID == WOID
                                  select W).FirstOrDefault();

                if (PrevWOData != null)
                {
                    PrevID = PrevWOData.prev_wo_id;
                }
                //}

                if (PrevID == null || PrevID == 0)
                {
                    var PrevWOID = (from w1 in objContext.MESC1TS_WO
                                    join w2 in objContext.MESC1TS_WO on w1.EQPNO equals w2.EQPNO
                                    where w1.WO_ID == WOID &&
                                    w1.CRTS > w2.CRTS
                                    select w2).OrderByDescending(w2 => w2.CRTS).FirstOrDefault();

                    if (PrevWOID != null && PrevWOID.Count > 0) //if previous wo_id found, buffer it else change it to -1
                    {
                        PrevID = PrevWOID.WO_ID;
                    }
                    else
                    {
                        PrevID = -1;
                    }
                }
                if (PrevID == -1) //-1 for work orders without prev data
                {
                    LocCode = string.Empty;
                    Date = "9999-12-31";
                    StatusCodeTemp = null;
                }
                else
                {
                    var WOData = (from wo in objContext.MESC1TS_WO
                                  from s in objContext.MESC1TS_SHOP
                                  where wo.WO_ID == PrevID &&
                                  wo.SHOP_CD == s.SHOP_CD
                                  select new { wo, s }).ToList();

                    if (WOData != null && WOData.Count > 0)
                    {
                        LocCode = WOData[0].s.LOC_CD;
                        Date = WOData[0].wo.CHTS.ToString();
                        StatusCodeTemp = WOData[0].wo.STATUS_CODE;
                    }
                }

                var WOFromDB = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == WOID
                                select wo).FirstOrDefault();

                if (WOFromDB != null)
                {
                    if (390 > WOFromDB.STATUS_CODE)
                    {
                        //"'"; rhsRecord.m_sPREV_STATUS += _bstr_t(cStatus); rhsRecord.m_sPREV_STATUS += "'";
                        WOFromDB.STATUS_CODE = 390;
                        WOFromDB.CHUSER = User;
                        WOFromDB.CHTS = DateTime.Now;
                        WOFromDB.APPROVAL_DTE = DateTime.Now;
                        WOFromDB.PREV_STATUS = Status;
                        WOFromDB.PREV_DATE = Convert.ToDateTime(Date);
                        WOFromDB.PREV_LOC_CD = LocCode;
                        objContext.SaveChanges();
                    }
                }
                WOAudit = new MESC1TS_WOAUDIT()
                WOAudit.WO_ID = WOID;
                WOAudit.CHUSER = User;
                WOAudit.CHTS = DateTime.Now;
                WOAudit.AUDIT_TEXT = "Approved by " + User;
                objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                objContext.SaveChanges();

                WORemark = new MESC1TS_WOREMARK()
                WORemark.WO_ID = WOID;
                WORemark.REMARK_TYPE = "S";
                WORemark.SUSPCAT_ID = null;
                WORemark.REMARK = OldStatusOrRemark;
                WORemark.CHUSER = User;
                WORemark.CRTS = DateTime.Now;
                objContext.SaveChanges();

            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                message = "Not all selected estimates were approved; please try again, or approve them manually: <br>";
            }

            return message;
        }
        // By Afroz
        public string UpdateApproveWorkOrderByReview(int WOID, string User, string OldStatusOrRemark, string VendorRefNo)
        {
            MESC1TS_WO PrevWODataFromDB = new MESC1TS_WO();
            string LocCode = string.Empty;
            int? PrevID = 0;
            string Date = string.Empty;
            short? StatusCodeTemp = 0;
            short? Status = 390;
            string message = "";
            //var PrevWOID;
            MESC1TS_WOAUDIT WOAudit = null;
            MESC1TS_WOREMARK WORemark = null;
            List<ErrMessage> errorMessageList = new List<ErrMessage>();
            try
            {
                //if (Status == APPROVEDSTATUS)
                //{
                var PrevWOData = (from W in objContext.MESC1TS_WO
                                  where W.WO_ID == WOID
                                  select W).FirstOrDefault();

                if (PrevWOData != null)
                {
                    PrevID = PrevWOData.prev_wo_id;
                }
                //}

                if (PrevID == null || PrevID == 0)
                {
                    var PrevWOID = (from w1 in objContext.MESC1TS_WO
                                    join w2 in objContext.MESC1TS_WO on w1.EQPNO equals w2.EQPNO
                                    where w1.WO_ID == WOID &&
                                    w1.CRTS > w2.CRTS
                                    select w2).OrderByDescending(w2 => w2.CRTS).FirstOrDefault();

                    if (PrevWOID != null && PrevWOID.Count > 0) //if previous wo_id found, buffer it else change it to -1
                    {
                        PrevID = PrevWOID.WO_ID;
                    }
                    else
                    {
                        PrevID = -1;
                    }
                }
                if (PrevID == -1) //-1 for work orders without prev data
                {
                    LocCode = string.Empty;
                    Date = "9999-12-31";
                    StatusCodeTemp = null;
                }
                else
                {
                    var WOData = (from wo in objContext.MESC1TS_WO
                                  from s in objContext.MESC1TS_SHOP
                                  where wo.WO_ID == PrevID &&
                                  wo.SHOP_CD == s.SHOP_CD
                                  select new { wo, s }).ToList();

                    if (WOData != null && WOData.Count > 0)
                    {
                        LocCode = WOData[0].s.LOC_CD;
                        Date = WOData[0].wo.CHTS.ToString();
                        StatusCodeTemp = WOData[0].wo.STATUS_CODE;
                    }
                }

                var WOFromDB = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == WOID
                                select wo).FirstOrDefault();

                if (WOFromDB != null)
                {
                    if (390 > WOFromDB.STATUS_CODE)
                    {
                        //"'"; rhsRecord.m_sPREV_STATUS += _bstr_t(cStatus); rhsRecord.m_sPREV_STATUS += "'";
                        WOFromDB.STATUS_CODE = 390;
                        WOFromDB.CHUSER = User;
                        WOFromDB.CHTS = DateTime.Now;
                        WOFromDB.APPROVAL_DTE = DateTime.Now;
                        WOFromDB.PREV_STATUS = Status;
                        WOFromDB.PREV_DATE = Convert.ToDateTime(Date);
                        WOFromDB.PREV_LOC_CD = LocCode;
                        int k = objContext.SaveChanges();

                        if (k == 1)
                        {
                            WOAudit = new MESC1TS_WOAUDIT()
                            WOAudit.WO_ID = WOID;
                            WOAudit.CHUSER = User;
                            WOAudit.CHTS = DateTime.Now;
                            WOAudit.AUDIT_TEXT = "Approved by " + User;
                            objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                            int n = objContext.SaveChanges();
                            if (n == 1)
                            {

                                WORemark = new MESC1TS_WOREMARK()
                                WORemark.WO_ID = WOID;
                                WORemark.REMARK_TYPE = "S";
                                WORemark.SUSPCAT_ID = null;
                                WORemark.REMARK = OldStatusOrRemark;
                                WORemark.CHUSER = User;
                                WORemark.CRTS = DateTime.Now;
                                objContext.MESC1TS_WOREMARK.Add(WORemark);
                                int m = objContext.SaveChanges();
                                if (m == 1)
                                {
                                    message = "Success";
                                }

                            }
                        }
                        else
                        {
                            message = "Failed";
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                message = "Not all selected estimates were approved; please try again, or approve them manually: <br>";
            }

            return message;
        }
        public string SetWorkingSwitchByWOIDByReview(int WOID, string Switch, string ChangeUser)
        {

            MESC1TS_WO WO = new MESC1TS_WO();
            string message = "";
            try
            {
                // if swithch not = N or Y then return false;
                if (!Switch.Equals("Y", StringComparison.CurrentCultureIgnoreCase) && !Switch.Equals("N", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "failed";
                }
                WO = (from wo in objContext.MESC1TS_WO
                      where wo.WO_ID == WOID
                      select wo).FirstOrDefault();

                if (WO != null)
                {
                    //OldRepairDate = WO.REPAIR_DTE;
                    //if (NewRepairDate != OldRepairDate)
                    {
                        string AuditComment = string.Empty;
                        MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();

                        WO.SHOP_WORKING_SW = Switch;
                        WO.CHUSER = ChangeUser;
                        WO.CHTS = DateTime.Now;
                        int k = objContext.SaveChanges();
                        if (k == 1)
                        {

                            //if(OldRepairDate == null) 
                            AuditComment = "Work Order: " + WOID + "<b> working switch set to " + Switch + "</b> by " + ChangeUser;
                            WOAudit.WO_ID = WOID;
                            WOAudit.CHTS = DateTime.Now;
                            WOAudit.AUDIT_TEXT = AuditComment;
                            WOAudit.CHUSER = ChangeUser;
                            objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                            int i = objContext.SaveChanges();
                            if (i == 1)
                            {
                                message = "Success";
                            }

                        }
                        else
                        {
                            message = "Failed";
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return "Failed";
            }
            return message;

        }
        public string UpdateCompleteApprovedWOByReview(int WOID, DateTime? NewRepairDate, string ChangeUser)
        {
            string message = "";
            MESC1TS_WO WO = null;


            try
            {
                // safety - no wo_id - just exit - nothing to do
                //if (WOID == 0) return;

                // check if date is > current date
                // IsExpired returns 0 if equal, 1 is currentdate is > and -1 if current date is <
                if (NewRepairDate > DateTime.Now)
                {
                    return "Failed";
                }

                // date is valid and not > than current date, now ensure that date is not less than 1st day of previous month
                // Ensure that date is not < 1st day of previous month.
                // build oldest allowed date.
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                if (month > 1)
                {
                    month -= 1;
                }
                else
                {
                    month = 12;
                    year -= 1;
                }

                DateTime setDate = (DateTime.Now.Date);
                DateTime dt = (DateTime)NewRepairDate;
                //string tempRepDate = dt.ToString();
                //Incomplete

                if (dt.Date < setDate)
                {
                    return "Failed";

                }

                DateTime? OldRepairDate;

                WO = (from wo in objContext.MESC1TS_WO
                      where wo.WO_ID == WOID
                      select wo).FirstOrDefault();

                if (WO != null)
                {
                    OldRepairDate = WO.REPAIR_DTE;
                    if (NewRepairDate != OldRepairDate)
                    {
                        string AuditComment = string.Empty;
                        MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();

                        WO.REPAIR_DTE = NewRepairDate;
                        WO.CHUSER = ChangeUser;
                        WO.STATUS_CODE = 400;
                        int k = objContext.SaveChanges();
                        if (k == 1)
                        {


                            //if(OldRepairDate == null) 
                            AuditComment = "Work Order: " + WOID + "<b> RepairDate changed from " + OldRepairDate + " to " + NewRepairDate + "</b> by " + ChangeUser;
                            WOAudit.WO_ID = WOID;
                            WOAudit.CHTS = DateTime.Now;
                            WOAudit.AUDIT_TEXT = AuditComment;
                            WOAudit.CHUSER = ChangeUser;
                            objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                            int m = objContext.SaveChanges();
                            if (m == 1)
                            {
                                message = "Success";
                            }
                        }
                        else
                        {
                            message = "Failed";
                        }
                    }
                }



                //if (m_WOList.Size() > 0)
                //{
                //    for (int i = 0; i < m_WOList.Size(); i++)
                //    {
                //        pWO = (CWorkOrderRecord*)m_WOList.GetAt(i);
                //        if (strcmp((char*)tWOID, pWO->GetWO_ID().c_str()) == 0)
                //        {
                //            STATE stat = pWO->GetObjectState();
                //            pWO->SetREPAIR_DTE((char*)tRepairDte);
                //            pWO->SetObjectState(stat);
                //            break;
                //        }
                //    }
                //}
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return "failed";

            }

            return message;
        }
        // End Afroz
        public string UpdateWorkOrder(string WO_ID, int Status)
        {
            //objContext = new CreateWorkOrderServiceEntities();
            string result = "";
            List<MESC1TS_WO> WOOrder = new List<MESC1TS_WO>();
            try
            {

                WOOrder = (from WOs in objContext.MESC1TS_WO
                           where WOs.WO_ID == Convert.ToInt32(WO_ID)
                           select WOs).ToList();
                if (WOOrder.Count() > 0)
                {

                    WOOrder[0].STATUS_CODE = Convert.ToInt16(Status);

                    int i = objContext.SaveChanges();

                    if (i == 1)
                    {
                        result = "Success";

                    }
                    else
                    {
                        result = "Failed";

                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                result = "Failed";

            }
            return result;

        }

        public List<WorkOrderDetail> GetWorkOrder(string ShopCode, string FromDate, string ToDate, string CustomerCD, string EqpSize, string EqpType, string EqpSType, string Mode, string EquipmentNo, string VenRefNo, string Cocl, string Country, string Location, string QueryType, int SortType, int UserId)
        {
            //////////////Shop List/////////////////////////
            List<Shop> ShopList = new List<Shop>();

            List<WorkOrderDetail> WOList = null;
            try
            {

                var ShopOnAuth = (from U in objContext.SEC_AUTHGROUP_USER
                                  from G in objContext.SEC_AUTHGROUP
                                  where U.USER_ID == UserId &&
                                  U.AUTHGROUP_ID == G.AUTHGROUP_ID
                                  select new
                                  {
                                      U.AUTHGROUP_ID,
                                      COLUMN_VALUE = U.COLUMN_VALUE,
                                      G.TABLE_NAME,
                                      COLUMN_NAME = G.COLUMN_NAME
                                  }).OrderBy(id => id.AUTHGROUP_ID).ToList();

                List<string> VendorCodeList = new List<string>();
                List<string> LocCodeList = new List<string>();
                List<string> CountryCodeList = new List<string>();
                List<string> ShopCodeList = new List<string>();
                List<string> AreaCodeList = new List<string>();

                foreach (var item in ShopOnAuth)
                {
                    if (item.COLUMN_NAME == "VENDOR_CD")
                    {
                        string VendorCode = item.COLUMN_VALUE;
                        VendorCodeList.Add(VendorCode);
                    }
                    if (item.COLUMN_NAME == "LOC_CD")
                    {
                        string LocCode = item.COLUMN_VALUE;
                        LocCodeList.Add(LocCode);
                    }
                    if (item.COLUMN_NAME == "COUNTRY_CD")
                    {
                        string CountryCode = item.COLUMN_VALUE;
                        CountryCodeList.Add(CountryCode);
                    }
                    if (item.COLUMN_NAME == "AREA_CD")
                    {
                        string AreaCode = item.COLUMN_VALUE;
                        AreaCodeList.Add(AreaCode);
                    }
                    if (item.COLUMN_NAME == "SHOP_CD")
                    {
                        string ShopCd = item.COLUMN_VALUE;
                        ShopCodeList.Add(ShopCd);
                    }
                }

                var ShopListFromDBOnAuth = (from shop in objContext.MESC1VS_SHOP_LOCATION
                                            where
                                                shop.SHOP_ACTIVE_SW == "Y" &&
                                            (VendorCodeList.Contains(shop.VENDOR_CD) ||
                                            LocCodeList.Contains(shop.LOC_CD) ||
                                            CountryCodeList.Contains(shop.COUNTRY_CD) ||
                                            AreaCodeList.Contains(shop.AREA_CD) ||
                                            ShopCodeList.Contains(shop.SHOP_CD))
                                            select new
                                            {
                                                shop.SHOP_CD,
                                                shop.CUCDN,
                                                shop.SHOP_DESC
                                            }).OrderBy(code => code.SHOP_CD).ToList();


                if (ShopListFromDBOnAuth != null && ShopListFromDBOnAuth.Count > 0)
                {
                    var ShopListFromDB = (from S in objContext.MESC1TS_SHOP
                                          from SM in objContext.MESC1TS_CUST_SHOP_MODE
                                          from CY in objContext.MESC1TS_CURRENCY
                                          from CU in objContext.MESC1TS_CUSTOMER
                                          where S.SHOP_CD == SM.SHOP_CD &&
                                              S.SHOP_ACTIVE_SW == "Y" &&
                                                S.CUCDN == CY.CUCDN &&
                                                CU.CUSTOMER_CD == SM.CUSTOMER_CD
                                          orderby S.SHOP_CD
                                          select new
                                          {
                                              S.SHOP_CD
                                          }).ToList();


                    var ShopListFinal = ShopListFromDBOnAuth.FindAll(a => ShopListFromDB.Any(ab => ab.SHOP_CD == a.SHOP_CD));


                    ////////////End Shop List///////////////////////
                    //objContext = new CreateWorkOrderServiceEntities();


                    DateTime? StartDate = null;
                    DateTime? EndDate = null;
                    if (!string.IsNullOrEmpty(FromDate))
                    {
                        StartDate = Convert.ToDateTime(FromDate);
                    }
                    if (!string.IsNullOrEmpty(ToDate))
                    {
                        EndDate = Convert.ToDateTime(ToDate).AddDays(1);
                    }

                    DateTime? MinDate = System.DateTime.Now.AddYears(-1);
                    // List<MESC1TS_EQTYPE> DamageFromDB = new List<MESC1TS_EQTYPE>();
                    var Query = from W in objContext.MESC1TS_WO
                                from S in objContext.MESC1TS_SHOP
                                .Where(s => s.SHOP_CD == W.SHOP_CD).DefaultIfEmpty()
                                where S.SHOP_ACTIVE_SW == "Y"
                                from SC in objContext.MESC1TS_STATUS_CODE
                               .Where(sc => sc.STATUS_CODE == W.STATUS_CODE).DefaultIfEmpty()
                                from L in objContext.MESC1TS_LOCATION
                               .Where(L => L.LOC_CD == S.LOC_CD).DefaultIfEmpty()
                                from C in objContext.MESC1TS_COUNTRY
                                .Where(C => C.COUNTRY_CD == L.COUNTRY_CD)
                                .DefaultIfEmpty()
                                from M in objContext.MESC1TS_CUST_SHOP_MODE
                               .Where(M => M.SHOP_CD == W.SHOP_CD && M.MODE == W.MODE && M.CUSTOMER_CD == W.CUSTOMER_CD)
                               .DefaultIfEmpty()



                                select new
                                {
                                    W.WO_ID,
                                    W.SHOP_CD,
                                    W.STATUS_CODE,
                                    SC.STATUS_DSC,
                                    W.TOT_COST_LOCAL,
                                    W.TOTAL_COST_LOCAL_USD,
                                    W.EQPNO,
                                    W.VENDOR_REF_NO,
                                    W.MODE,
                                    W.SHOP_WORKING_SW,
                                    W.CUSTOMER_CD,
                                    W.EQSIZE,
                                    W.EQOUTHGU,
                                    W.EQTYPE,
                                    W.EQSTYPE,
                                    W.COTYPE,
                                    S.LOC_CD,
                                    W.REPAIR_DTE,
                                    W.WO_RECV_DTE,
                                    W.CRTS,
                                    W.TOT_COST_REPAIR_CPH,
                                    M.PAYAGENT_CD,
                                    W.VOUCHER_NO,
                                    C.COUNTRY_CD,
                                    W.TOT_REPAIR_MANH
                                };

                    //now we can apply filters on ANY of the joined tables
                    if (ShopCode != "0")
                        Query = Query.Where(q => q.SHOP_CD == ShopCode);
                    if (!string.IsNullOrEmpty(FromDate))
                        Query = Query.Where(q => q.CRTS >= StartDate);
                    if (!string.IsNullOrEmpty(ToDate))
                        Query = Query.Where(q => q.CRTS < EndDate);
                    if (!string.IsNullOrEmpty(CustomerCD))
                    {
                        if (CustomerCD != "0")
                            Query = Query.Where(q => q.CUSTOMER_CD == CustomerCD);
                    }
                    if (EqpSize != "0")
                        Query = Query.Where(q => q.EQSIZE == EqpSize);
                    if (EqpType != "0")
                        Query = Query.Where(q => q.EQTYPE == EqpType);
                    if (!string.IsNullOrEmpty(EqpSType))
                        Query = Query.Where(q => q.EQSTYPE == EqpSType);
                    if (!string.IsNullOrEmpty(Mode))
                        Query = Query.Where(q => q.MODE == Mode);
                    if (!string.IsNullOrEmpty(EquipmentNo))
                        Query = Query.Where(q => q.EQPNO == EquipmentNo);
                    if (!string.IsNullOrEmpty(VenRefNo))
                        Query = Query.Where(q => q.VENDOR_REF_NO == VenRefNo);
                    if (!string.IsNullOrEmpty(Cocl))
                        Query = Query.Where(q => q.COTYPE == Cocl);
                    if (!string.IsNullOrEmpty(Country))
                        Query = Query.Where(q => q.COUNTRY_CD == Country);
                    if (!string.IsNullOrEmpty(Location))
                        Query = Query.Where(q => q.LOC_CD == Location);
                    if (QueryType.Contains(","))
                    {
                        string[] strArray1 = QueryType.Split(',');
                        List<short?> arr = new List<short?>();
                        foreach (var val in strArray1)
                        {
                            arr.Add(Convert.ToInt16(val));
                        }

                        Query = Query.Where(a => arr.Contains(a.STATUS_CODE ?? -1));

                    }
                    else
                    {
                        if (QueryType != "")
                        {
                            if (QueryType == "-390")
                            {
                                Query = Query.Where(q => q.STATUS_CODE == 390 && q.SHOP_WORKING_SW == "Y");
                            }
                            else
                            {
                                int Status_Code = Convert.ToInt32(QueryType);
                                Query = Query.Where(q => q.STATUS_CODE == Status_Code);
                            }
                        }
                    }

                    //finally select the columns we needed
                    var DamageFromDB = (from q in Query
                                        orderby q.CRTS descending
                                        select new
                                        {
                                            q.WO_ID,
                                            q.SHOP_CD,
                                            q.STATUS_CODE,
                                            q.STATUS_DSC,
                                            q.TOT_COST_LOCAL,
                                            q.TOTAL_COST_LOCAL_USD,
                                            q.EQPNO,
                                            q.VENDOR_REF_NO,
                                            q.MODE,
                                            q.SHOP_WORKING_SW,
                                            q.CUSTOMER_CD,
                                            q.EQSIZE,
                                            q.EQOUTHGU,
                                            q.EQTYPE,
                                            q.EQSTYPE,
                                            q.COTYPE,
                                            q.LOC_CD,
                                            q.REPAIR_DTE,
                                            q.WO_RECV_DTE,
                                            q.CRTS,
                                            q.TOT_COST_REPAIR_CPH,
                                            q.PAYAGENT_CD,
                                            q.VOUCHER_NO,
                                            q.COUNTRY_CD,
                                            q.TOT_REPAIR_MANH
                                        }).Take(100).ToList();

                    DamageFromDB = DamageFromDB.Where(a => a.CRTS > DateTime.Now.AddYears(-1)).ToList(); ;

                    DamageFromDB = DamageFromDB.FindAll(a => ShopListFinal.Any(ab => ab.SHOP_CD == a.SHOP_CD)).ToList();


                    if (SortType == 1)
                    {
                        DamageFromDB = DamageFromDB.OrderByDescending(li => li.TOTAL_COST_LOCAL_USD).ToList();
                    }
                    else if (SortType == 2)
                    {
                        DamageFromDB = DamageFromDB.OrderBy(li => li.EQPNO).ToList();
                    }
                    else if (SortType == 3)
                    {
                        DamageFromDB = DamageFromDB.OrderBy(li => li.TOT_REPAIR_MANH).ToList();
                    }
                    else if (SortType == 4)
                    {
                        DamageFromDB = DamageFromDB.OrderBy(li => li.EQSIZE).ToList();
                        DamageFromDB = DamageFromDB.OrderBy(li => li.EQOUTHGU).ToList();
                        DamageFromDB = DamageFromDB.OrderBy(li => li.EQTYPE).ToList();
                        DamageFromDB = DamageFromDB.OrderBy(li => li.EQSTYPE).ToList();
                        DamageFromDB = DamageFromDB.OrderByDescending(li => li.CRTS).ToList();
                        DamageFromDB = DamageFromDB.OrderBy(li => li.EQPNO).ToList();
                    }
                    else if (SortType == 5)
                    {
                        DamageFromDB = DamageFromDB.OrderBy(li => li.MODE).ToList();
                        DamageFromDB = DamageFromDB.OrderByDescending(li => li.CRTS).ToList();
                        DamageFromDB = DamageFromDB.OrderBy(li => li.EQPNO).ToList();
                    }
                    else if (SortType == 6)
                    {
                        DamageFromDB = DamageFromDB.OrderByDescending(li => li.VENDOR_REF_NO).ToList();
                    }
                    WOList = new List<WorkOrderDetail>()
                    foreach (var obj in DamageFromDB)
                    {
                        WorkOrderDetail WO = new WorkOrderDetail();
                        WO.WorkOrderID = obj.WO_ID;
                        WO.AreaCode = obj.LOC_CD.ToString();
                        WO.Shop = new Shop();
                        WO.Shop.ShopCode = obj.SHOP_CD.ToString();

                        if (obj.STATUS_CODE == null)
                        {
                            WO.StatusCode = "";
                        }
                        else
                        {
                            WO.StatusCode = obj.STATUS_CODE.ToString();
                        }
                        if (obj.STATUS_DSC == null)
                        {
                            WO.Status = "";
                        }
                        else
                        {
                            WO.Status = obj.STATUS_DSC.ToString();
                        }

                        if (obj.TOT_COST_LOCAL.ToString() == null || obj.TOT_COST_LOCAL.ToString() == "")
                        {
                            WO.TotalCostLocal = 0;
                        }
                        else
                        {
                            WO.TotalCostLocal = obj.TOT_COST_LOCAL;
                        }
                        if (obj.TOTAL_COST_LOCAL_USD.ToString() == null || obj.TOTAL_COST_LOCAL_USD.ToString() == "")
                        {
                            WO.TotalCostLocalUSD = 0;
                        }
                        else
                        {
                            WO.TotalCostLocalUSD = obj.TOTAL_COST_LOCAL_USD;
                        }
                        if (obj.TOT_COST_REPAIR_CPH.ToString() == null || obj.TOT_COST_REPAIR_CPH.ToString() == "")
                        {
                            WO.TotalCostOfRepairCPH = 0;
                        }
                        else
                        {
                            WO.TotalCostOfRepairCPH = obj.TOT_COST_REPAIR_CPH;
                        }
                        Equipment Equipment = new Equipment();
                        Equipment.EquipmentNo = obj.EQPNO;
                        Equipment.VendorRefNo = obj.VENDOR_REF_NO;
                        WO.Mode = obj.MODE;
                        WO.RepairDate = obj.REPAIR_DTE;
                        WO.WorkOrderReceiveDate = obj.WO_RECV_DTE;
                        WO.ShopWorkingSW = obj.SHOP_WORKING_SW;
                        WO.VoucherNumber = obj.VOUCHER_NO;
                        WO.PayAgentCode = obj.PAYAGENT_CD;
                        WO.TotalRepairManHour = obj.TOT_REPAIR_MANH;
                        WO.ChangeTime = Convert.ToDateTime(obj.CRTS);
                        Equipment.Size = obj.EQSIZE;
                        WO.EquipmentList = new List<MercPlusLibrary.Equipment>();
                        WO.EquipmentList.Add(Equipment);
                        WOList.Add(WO);
                    };
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                throw;
            }
            return WOList;
        }

        // End Afroz
        public WorkOrderDetail GetWorkOrderDetails(int WorkOrderID)
        {
            WorkOrderDetail WOD = new WorkOrderDetail();

            WOD = LoadWorkOrderDetails(WorkOrderID);

            return WOD;
        }

        public void Save(WorkOrderDetail workOrder, out List<ErrMessage> errorMessageList)
        {
            errorMessageList = new List<ErrMessage>();
            bool success = true;
            try
            {
                // Safe code - possible update from review screen in UI without validation of 
                // repair date.
                // check most basic fields for content.
                // partial validations performed to protect the database.
                //if (ValidationBasics())
                {
                    //if (LoadErrors())
                    {
                        if (GetShopData(ref workOrder, true, out errorMessageList))
                        {
                            workOrder.RepairDate = DateTime.Now;
                            //if (CheckRepairDate(workOrder, out errorMessageList))
                            {
                                success = SaveWorkOrder(workOrder, out errorMessageList);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "Error saving WorkOrder to database";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        #region    Submit  Work  Order     (Ashiqur Rahaman -  14/9/2015  )

        public void Save2(WorkOrderDetail workorderDetail, out List<ErrMessage> errorMessageList)
        {
            bool result = false;

            errorMessageList = new List<ErrMessage>();
            ErrMessage Message = new ErrMessage();

            //  if - else logic //

            try
            {

                result = SaveWorkOrder(workorderDetail, out errorMessageList);

            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error while saving work order - work order not saved";
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
        }

        private DateTime? GetDateTimeNullable(DateTime? dt)
        {
            return ((dt == null || dt <= DateTime.MinValue) ? null : dt);
        }

        public bool InsertHeader(WorkOrderDetail workorderDetail, out List<ErrMessage> errorMessageList)
        {
            bool success = false;

            errorMessageList = new List<ErrMessage>();
            ErrMessage Message = new ErrMessage();
            //objContext = new CreateWorkOrderServiceEntities();
            MESC1TS_WO workOrder = new MESC1TS_WO();
            Equipment Eqp = workorderDetail.EquipmentList[0];
            Customer customer = workorderDetail.Shop.Customer[0];
            LaborRate Labor = workorderDetail.Shop.LaborRate[0];

            try
            {
                workOrder.COUNTRY_EXCHANGE_DTE = GetDateTimeNullable(workorderDetail.CountryExchangeDate);
                workOrder.CHTS = DateTime.Now;
                workOrder.DELDATSH = GetDateTimeNullable(Eqp.Deldatsh);
                workOrder.REFRBDAT = GetDateTimeNullable(Eqp.RefurbishmnentDate);
                workOrder.EQINDAT = GetDateTimeNullable(Eqp.EQInDate);
                workOrder.PREV_DATE = GetDateTimeNullable(workorderDetail.PrevDate);
                workOrder.GATEINDTE = GetDateTimeNullable(Eqp.GateInDate);
                workOrder.WO_RECV_DTE = DateTime.Now;

                workOrder.WO_ID = workorderDetail.WorkOrderID;
                workOrder.CUSTOMER_CD = (customer.CustomerCode);
                workOrder.MANUAL_CD = (customer.ManualCode);
                workOrder.MODE = (workorderDetail.Mode);
                workOrder.WOTYPE = (workorderDetail.WorkOrderType);
                workOrder.VENDOR_CD = (workorderDetail.Shop.VendorCode);
                workOrder.SHOP_CD = (workorderDetail.Shop.ShopCode);
                workOrder.REPAIR_DTE = DateTime.UtcNow;  //((workorderDetail.RepairDate));
                workOrder.STATUS_CODE = workorderDetail.WorkOrderStatus;
                workOrder.CAUSE = (workorderDetail.Cause);
                workOrder.THIRD_PARTY = ((workorderDetail.ThirdPartyPort));
                workOrder.MANH_RATE = (((workorderDetail.ManHourRate)));
                workOrder.MANH_RATE_CPH = (((workorderDetail.ManHourRateCPH)));
                workOrder.EXCHANGE_RATE = (workorderDetail.ExchangeRate * 100);
                workOrder.CUCDN = ((workorderDetail.Shop.CUCDN));
                workOrder.TOT_REPAIR_MANH = (((workorderDetail.TotalRepairManHour)));
                workOrder.TOT_MANH_REG = (((workorderDetail.TotalManHourReg)));
                workOrder.TOT_MANH_OT = (((workorderDetail.TotalManHourOverTime)));
                workOrder.TOT_MANH_DT = (((workorderDetail.TotalManHourDoubleTime)));
                workOrder.TOT_MANH_MISC = (((workorderDetail.TotalManHourMisc)));

                workOrder.TOT_PREP_HRS = (((workorderDetail.TotalPrepHours)));
                workOrder.TOT_LABOR_HRS = (((workorderDetail.TotalLaborHours)));
                workOrder.TOT_LABOR_COST = (((workorderDetail.TotalLabourCost)));
                workOrder.TOT_LABOR_COST_CPH = (((workorderDetail.TotalLabourCostCPH)));
                workOrder.TOT_SHOP_AMT = (((workorderDetail.TotalShopAmount)));
                workOrder.TOT_SHOP_AMT_CPH = (((workorderDetail.TotalShopAmountCPH)));
                workOrder.TOT_COST_LOCAL = (((workorderDetail.TotalCostLocal)));
                workOrder.TOT_COST_CPH = (((workorderDetail.TotalCostCPH)));
                workOrder.OT_RATE = (((workorderDetail.OverTimeRate)));
                workOrder.OT_RATE_CPH = (((workorderDetail.OverTimeRateCPH)));
                workOrder.DT_RATE = (((workorderDetail.DoubleTimeRate)));
                workOrder.DT_RATE_CPH = (((workorderDetail.DoubleTimeRateCPH)));
                workOrder.MISC_RATE = (((Labor.MiscRT)));
                workOrder.MISC_RATE_CPH = (((workorderDetail.MiscRateCPH)));
                workOrder.TOTAL_COST_LOCAL_USD = (((workorderDetail.TotalCostLocalUSD)));
                workOrder.TOT_COST_REPAIR = (((workorderDetail.TotalCostOfRepair)));
                workOrder.TOT_COST_REPAIR_CPH = (((workorderDetail.TotalCostOfRepairCPH)));
                workOrder.SALES_TAX_LABOR = (((workorderDetail.SalesTaxLabour)));
                workOrder.SALES_TAX_LABOR_CPH = (((workorderDetail.SalesTaxLabourCPH)));
                workOrder.SALES_TAX_PARTS = (((workorderDetail.SalesTaxParts)));
                workOrder.SALES_TAX_PARTS_CPH = (((workorderDetail.SalesTaxPartsCPH)));

                workOrder.TOT_MAERSK_PARTS = (((workorderDetail.TotalMaerksParts)));
                workOrder.TOT_MAERSK_PARTS_CPH = (((workorderDetail.TotalMaerksPartsCPH)));
                workOrder.TOT_MAN_PARTS = (((workorderDetail.TotalManParts)));
                workOrder.TOT_MAN_PARTS_CPH = (((workorderDetail.TotalManPartsCPH)));
                workOrder.VENDOR_REF_NO = (((Eqp.VendorRefNo)));
                workOrder.IMPORT_TAX = (((workorderDetail.ImportTax)));
                workOrder.IMPORT_TAX_CPH = (((workorderDetail.ImportTaxCPH)));
                workOrder.CHUSER = ((workorderDetail.ChangeUser));
                workOrder.CRTS = DateTime.Now;
                workOrder.EQPNO = (((Eqp.EquipmentNo)));
                workOrder.EQSIZE = ((Eqp.Size));
                workOrder.EQTYPE = ((Eqp.Type));
                workOrder.EQOUTHGU = ((Eqp.Eqouthgu));
                workOrder.COTYPE = ((Eqp.COType));
                workOrder.EQSTYPE = ((Eqp.Eqstype));
                workOrder.EQOWNTP = ((Eqp.Eqowntp));
                workOrder.EQMATR = ((Eqp.Eqmatr));
                //workOrder.DELDATSH = DateTime.UtcNow;  //(((workorderDetail.Deldatsh)));
                workOrder.STEMPTY = ((Eqp.StEmptyFullInd));
                workOrder.STREFURB = ((Eqp.Strefurb));
                //workOrder.REFRBDAT = DateTime.UtcNow;  //(((workorderDetail.RefurbishmnentDate)));
                workOrder.STREDEL = ((Eqp.Stredel));
                workOrder.FIXCOVER = (((Eqp.Fixcover)));
                workOrder.DPP = (((Eqp.Dpp)));
                workOrder.OFFHIR_LOCATION_SW = ((Eqp.OffhirLocationSW));
                workOrder.STSELSCR = ((Eqp.STSELSCR));
                workOrder.EQMANCD = ((Eqp.EqMancd));
                workOrder.EQPROFIL = ((Eqp.EQProfile));
                //workOrder.EQINDAT = DateTime.UtcNow; //(((Eqp.EQINDAT)));
                workOrder.EQRUMAN = ((Eqp.EQRuman));
                workOrder.EQRUTYP = ((Eqp.EQRutyp));
                workOrder.EQIOFLT = ((Eqp.EQIoflt));

                workOrder.SALES_TAX_LABOR_PCT = (((workorderDetail.SalesTaxLaborPCT)));
                workOrder.SALES_TAX_PARTS_PCT = (((workorderDetail.SalesTaxPartsPCT)));
                workOrder.IMPORT_TAX_PCT = (((workorderDetail.ImportTaxPCT)));
                workOrder.AGENT_PARTS_TAX = (((workorderDetail.AgentPartsTax)));
                workOrder.AGENT_PARTS_TAX_CPH = (((workorderDetail.AgentPartsTaxCPH)));
                workOrder.RKRP_XMIT_SW = "0";
                workOrder.REQ_REMARK_SW = "Y"; // Convert.ToString(workorderDetail.ReqdRemarkSW);
                workOrder.COUNTRY_CUCDN = ((workorderDetail.CountryCUCDN));
                workOrder.COUNTRY_EXCHANGE_DTE = DateTime.UtcNow;  //(((workorderDetail.CountryExchangeDate)));


                if (string.IsNullOrEmpty(Eqp.Damage))
                {
                    workOrder.DAMAGE = "N";
                }
                if (!string.IsNullOrEmpty(Eqp.Damage) && Eqp.Damage.Length == 0)
                {
                    workOrder.DAMAGE = "9";
                }

                //workOrder.DAMAGE = (Eqp.Damage);
                workOrder.LSCOMP = (workorderDetail.LeaseComp);
                workOrder.LSCONTR = (workorderDetail.LeaseContract);
                workOrder.PREV_STATUS = workorderDetail.PrevStatus;
                workOrder.PREV_LOC_CD = workorderDetail.PrevLocCode;
                //workOrder.PREV_DATE = DateTime.UtcNow; // workorderDetail.PrevDate;
                workOrder.prev_wo_id = workorderDetail.PrevWorkOrderID;
                workOrder.REVISION_NO = 00;
                workOrder.PRESENTLOC = workorderDetail.PresentLoc;
                //workOrder.GATEINDTE = DateTime.Now; // workorderDetail.GateInDate;

                /*Rohit Mallick added as these were missing : starts*/

                workOrder.TOT_W_MATERIAL_AMT = workorderDetail.TotalWMaterialAmount;
                workOrder.TOT_T_MATERIAL_AMT = workorderDetail.TotalTMaterialAmount;
                workOrder.TOT_W_MATERIAL_AMT_CPH = workorderDetail.TotalWMaterialAmountCPH;
                workOrder.TOT_T_MATERIAL_AMT_CPH = workorderDetail.TotalWMaterialAmountCPH;
                workOrder.TOT_W_MATERIAL_AMT_USD = workorderDetail.TotalWMaterialAmountUSD;
                workOrder.TOT_T_MATERIAL_AMT_USD = workorderDetail.TotalTMaterialAmountUSD;
                workOrder.TOT_W_MATERIAL_AMT_CPH_USD = workorderDetail.TotalWMaterialAmountCPHUSD;
                workOrder.TOT_T_MATERIAL_AMT_CPH_USD = workorderDetail.TotalTMaterialAmountCPHUSD;

                /*Rohit Mallick added as these were missing : ends*/

                objContext.MESC1TS_WO.Add(workOrder);
                //objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Exception e = ex.InnerException;
                Message.Message = "System error while saving WorkOrder ot Database";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }

            return success;
        }

        public bool GetIdentity(ref WorkOrderDetail workorderDtl, out List<ErrMessage> errorMessageList)
        {

            bool success = true;

            errorMessageList = new List<ErrMessage>();
            ErrMessage Message = new ErrMessage();
            try
            {

                int WoId = objContext.MESC1TS_WO.Max(u => u.WO_ID);
                WoId = WoId + 1;
                workorderDtl.WorkOrderID = WoId;
                //if (workorderDtl.VendorRefNo > 0)
                if (string.IsNullOrEmpty(workorderDtl.EquipmentList[0].VendorRefNo))
                {

                    workorderDtl.EquipmentList[0].VendorRefNo = (workorderDtl.WorkOrderID.ToString().PadLeft(10, '0'));


                    /////// need to set the update query  ////////

                    //   objContext = new ManageMasterDataServiceEntities();

                    //List<MESC1TS_WO> WOdtl = new List<MESC1TS_WO>();
                    //WOdtl = (from eqty in objContext.MESC1TS_WO
                    //         where eqty.WO_ID == workorderDtl.WorkOrderID
                    //         select eqty).ToList();
                    //WOdtl[0].VENDOR_REF_NO = Convert.ToString(workorderDtl.EquipmentList[0].VendorRefNo);


                    //objContext.SaveChanges();

                    //success = true;
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "Database error encountered in SaveWorkOrder call.";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return false;
            }


            return success;

        }

        public bool UpdateHeader(WorkOrderDetail workorderDetail, out List<ErrMessage> errorMessageList)
        {
            bool success = true;

            errorMessageList = new List<ErrMessage>();
            ErrMessage Message = new ErrMessage();
            success = false;
            Equipment Eqp = workorderDetail.EquipmentList[0];
            Customer customer = workorderDetail.Shop.Customer[0];
            LaborRate Labor = workorderDetail.Shop.LaborRate[0];
            //workorderDetail = new WorkOrderDetail();
            MESC1TS_WO workOrder = new MESC1TS_WO();
            //objContext = new CreateWorkOrderServiceEntities();

            try
            {
                workOrder = (from wo in objContext.MESC1TS_WO
                             where wo.WO_ID == workorderDetail.WorkOrderID
                             select wo).FirstOrDefault();

                if (workOrder != null)
                {
                    workOrder.COUNTRY_EXCHANGE_DTE = GetDateTimeNullable(workorderDetail.CountryExchangeDate);
                    workOrder.CHTS = DateTime.Now;
                    workOrder.DELDATSH = GetDateTimeNullable(Eqp.Deldatsh);
                    workOrder.REFRBDAT = GetDateTimeNullable(Eqp.RefurbishmnentDate);
                    workOrder.EQINDAT = GetDateTimeNullable(Eqp.EQInDate);
                    workOrder.PREV_DATE = GetDateTimeNullable(workorderDetail.PrevDate);
                    workOrder.GATEINDTE = GetDateTimeNullable(Eqp.GateInDate);

                    //workOrder.CUSTOMER_CD = workorderDetail.Shop.Customer[0].CustomerCode;
                    workOrder.MANUAL_CD = customer.ManualCode;
                    //workOrder.MODE = workorderDetail.Mode;
                    workOrder.WOTYPE = (workorderDetail.WorkOrderType);
                    workOrder.VENDOR_CD = (workorderDetail.Shop.VendorCode);
                    //workOrder.SHOP_CD = (workorderDetail.Shop.ShopCode);
                    workOrder.REPAIR_DTE = ((workorderDetail.RepairDate));
                    workOrder.STATUS_CODE = workorderDetail.WorkOrderStatus;
                    workOrder.CAUSE = (workorderDetail.Cause);
                    workOrder.THIRD_PARTY = ((workorderDetail.ThirdPartyPort));
                    workOrder.MANH_RATE = (((workorderDetail.ManHourRate)));
                    workOrder.MANH_RATE_CPH = (((workorderDetail.ManHourRateCPH)));
                    workOrder.EXCHANGE_RATE = (((workorderDetail.ExchangeRate)));
                    //workOrder.CUCDN = ((workorderDetail.Shop.Currency.Cucdn));
                    workOrder.TOT_REPAIR_MANH = (((workorderDetail.TotalRepairManHour)));
                    workOrder.TOT_MANH_REG = (((workorderDetail.TotalManHourReg)));
                    workOrder.TOT_MANH_OT = (((workorderDetail.TotalManHourOverTime)));
                    workOrder.TOT_MANH_DT = (((workorderDetail.TotalManHourDoubleTime)));
                    workOrder.TOT_MANH_MISC = (((workorderDetail.TotalManHourMisc)));
                    workOrder.TOT_PREP_HRS = (((workorderDetail.TotalPrepHours)));
                    workOrder.TOT_LABOR_HRS = (((workorderDetail.TotalLaborHours)));
                    workOrder.TOT_LABOR_COST = (((workorderDetail.TotalLabourCost)));
                    workOrder.TOT_LABOR_COST_CPH = (((workorderDetail.TotalLabourCostCPH)));
                    workOrder.TOT_SHOP_AMT = (((workorderDetail.TotalShopAmount)));
                    workOrder.TOT_SHOP_AMT_CPH = (((workorderDetail.TotalShopAmountCPH)));
                    workOrder.TOT_COST_LOCAL = (((workorderDetail.TotalCostLocal)));
                    workOrder.TOT_COST_CPH = (((workorderDetail.TotalCostCPH)));
                    workOrder.OT_RATE = (((workorderDetail.OverTimeRate)));
                    workOrder.OT_RATE_CPH = (((workorderDetail.OverTimeRateCPH)));
                    workOrder.DT_RATE = (((workorderDetail.DoubleTimeRate)));
                    workOrder.DT_RATE_CPH = (((workorderDetail.DoubleTimeRateCPH)));
                    workOrder.MISC_RATE = (((workorderDetail.MiscRate)));
                    workOrder.MISC_RATE_CPH = (((workorderDetail.MiscRateCPH)));
                    workOrder.TOTAL_COST_LOCAL_USD = (((workorderDetail.TotalCostLocalUSD)));
                    workOrder.TOT_COST_REPAIR = (((workorderDetail.TotalCostOfRepair)));
                    workOrder.TOT_COST_REPAIR_CPH = (((workorderDetail.TotalCostOfRepairCPH)));
                    workOrder.SALES_TAX_LABOR = (((workorderDetail.SalesTaxLabour)));
                    workOrder.SALES_TAX_LABOR_CPH = (((workorderDetail.SalesTaxLabourCPH)));
                    workOrder.SALES_TAX_PARTS = (((workorderDetail.SalesTaxParts)));
                    workOrder.SALES_TAX_PARTS_CPH = (((workorderDetail.SalesTaxPartsCPH)));
                    workOrder.TOT_MAERSK_PARTS = (((workorderDetail.TotalMaerksParts)));
                    workOrder.TOT_MAERSK_PARTS_CPH = (((workorderDetail.TotalMaerksPartsCPH)));
                    workOrder.TOT_MAN_PARTS = (((workorderDetail.TotalManParts)));
                    workOrder.TOT_MAN_PARTS_CPH = (((workorderDetail.TotalManPartsCPH)));
                    //workOrder.VENDOR_REF_NO = (((Eqp.VendorRefNo)));
                    workOrder.IMPORT_TAX = (((workorderDetail.ImportTax)));
                    workOrder.IMPORT_TAX_CPH = (((workorderDetail.ImportTaxCPH)));
                    workOrder.CHUSER = ((workorderDetail.ChangeUser));
                    workOrder.CHTS = DateTime.Now;
                    //workOrder.EQPNO = (((Eqp.EquipmentNo)));
                    workOrder.EQSIZE = ((Eqp.Size));
                    workOrder.EQTYPE = ((Eqp.Type));
                    workOrder.EQOUTHGU = ((Eqp.Eqouthgu));
                    workOrder.COTYPE = ((Eqp.COType));
                    workOrder.EQSTYPE = ((Eqp.Eqstype));
                    workOrder.EQOWNTP = ((Eqp.Eqowntp));
                    workOrder.EQMATR = ((Eqp.Eqmatr));
                    workOrder.DELDATSH = (Eqp.Deldatsh);
                    workOrder.STEMPTY = ((Eqp.StEmptyFullInd));
                    workOrder.STREFURB = ((Eqp.Strefurb));
                    workOrder.REFRBDAT = (((Eqp.RefurbishmnentDate)));
                    workOrder.STREDEL = ((Eqp.Stredel));
                    workOrder.FIXCOVER = (((Eqp.Fixcover)));
                    workOrder.DPP = (((Eqp.Dpp)));
                    workOrder.OFFHIR_LOCATION_SW = ((Eqp.OffhirLocationSW));
                    workOrder.STSELSCR = ((Eqp.STSELSCR));
                    workOrder.EQMANCD = ((Eqp.EqMancd));
                    workOrder.EQPROFIL = ((Eqp.EQProfile));
                    workOrder.EQINDAT = (((Eqp.EQInDate)));
                    workOrder.EQRUMAN = ((Eqp.EQRuman));
                    workOrder.EQRUTYP = ((Eqp.EQRutyp));
                    workOrder.EQIOFLT = ((Eqp.EQIoflt));
                    workOrder.SALES_TAX_LABOR_PCT = (((workorderDetail.SalesTaxLaborPCT)));
                    workOrder.SALES_TAX_PARTS_PCT = (((workorderDetail.SalesTaxPartsPCT)));
                    workOrder.IMPORT_TAX_PCT = (((workorderDetail.ImportTaxPCT)));
                    workOrder.AGENT_PARTS_TAX = (((workorderDetail.AgentPartsTax)));
                    workOrder.AGENT_PARTS_TAX_CPH = (((workorderDetail.AgentPartsTaxCPH)));
                    workOrder.RKRP_XMIT_SW = "0";
                    workOrder.REQ_REMARK_SW = "Y"; // Convert.ToString(workorderDetail.ReqdRemarkSW);
                    workOrder.COUNTRY_CUCDN = ((workorderDetail.CountryCUCDN));
                    workOrder.COUNTRY_EXCHANGE_DTE = (((workorderDetail.CountryExchangeDate)));
                    workOrder.DAMAGE = UpdateRkemData(Eqp.Damage);
                    workOrder.LSCOMP = (workorderDetail.LeaseComp);
                    workOrder.LSCONTR = (workorderDetail.LeaseContract);
                    workOrder.TOT_W_MATERIAL_AMT = workorderDetail.TotalWMaterialAmount;
                    workOrder.TOT_T_MATERIAL_AMT = workorderDetail.TotalTMaterialAmount;
                    workOrder.TOT_W_MATERIAL_AMT_CPH = workorderDetail.TotalWMaterialAmountCPH;
                    workOrder.TOT_T_MATERIAL_AMT_CPH = workorderDetail.TotalWMaterialAmountCPH;
                    workOrder.TOT_W_MATERIAL_AMT_USD = workorderDetail.TotalWMaterialAmountUSD;
                    workOrder.TOT_T_MATERIAL_AMT_USD = workorderDetail.TotalTMaterialAmountUSD;
                    workOrder.TOT_W_MATERIAL_AMT_CPH_USD = workorderDetail.TotalWMaterialAmountCPHUSD;
                    workOrder.TOT_T_MATERIAL_AMT_CPH_USD = workorderDetail.TotalTMaterialAmountCPHUSD;
                    if (workOrder.REVISION_NO != null)
                    {
                        short? NewRevNo = (short)(workOrder.REVISION_NO + 1);
                        workOrder.REVISION_NO = NewRevNo;
                    }

                    //objContext.MESC1TS_WO.AddObject(workOrder);
                    objContext.SaveChanges();
                    success = true;
                }
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
            catch (Exception ex)
            {

                //    lRtn = pError->WriteApplicationLog("MercWorkOrderTable", "Database error encountered in SaveWorkOrder call.");
                //    m_nRtn = 99;
                //    return (false);
                Message = new ErrMessage();
                Message.Message = "Error Update in Database.";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }

            return success;
        }

        //public bool AddWOID(WorkOrderDetail workorderDetail)
        //{
        //    bool success = false;

        //    try
        //    {



        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }


        //    return success;
        //}     //  Not Required. //

        public bool SaveRepair(RepairsView RepairsView, int WOID, string ModeCode, string ManualCode, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : SaveRepair(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            bool success = true;

            errorMessageList = new List<ErrMessage>();
            ErrMessage Message = new ErrMessage();
            MESC1TS_WOREPAIR WORepair = new MESC1TS_WOREPAIR();


            try
            {
                if (RepairsView.rState == (int)Validation.STATE.NEW)
                {
                    //success = ManageWorkOrderDAL.InsertWORepair(RepairsView, ModeCode, ManualCode);


                    //WORepair = (from wor in objContext.MESC1TS_WOREPAIR
                    //            where wor.REPAIR_CD == RepairsView.RepairCode.RepairCod
                    WORepair.WO_ID = WOID;
                    WORepair.REPAIR_CD = RepairsView.RepairCode.RepairCod.Trim();
                    WORepair.MANUAL_CD = ManualCode;
                    WORepair.MODE = ModeCode;
                    WORepair.QTY_REPAIRS = (short)RepairsView.Pieces;
                    WORepair.SHOP_MATERIAL_AMT = RepairsView.MaterialCost;
                    WORepair.CPH_MATERIAL_AMT = RepairsView.MaterialCostCPH;
                    WORepair.ACTUAL_MANH = RepairsView.ManHoursPerPiece;
                    if (RepairsView.NonsCode == null)
                    {
                        WORepair.NONS_CD = null;
                    }
                    else
                    {
                        WORepair.NONS_CD = RepairsView.NonsCode.NonsCd;
                    }
                    WORepair.CHUSER = RepairsView.RepairCode.ChangeUser;
                    WORepair.CHTS = DateTime.Now;
                    WORepair.DAMAGE_CD = RepairsView.Damage.DamageCedexCode;
                    WORepair.REPAIR_LOC_CD = RepairsView.RepairLocationCode.CedexCode;
                    WORepair.TPI_CD = RepairsView.Tpi.CedexCode;
                    objContext.MESC1TS_WOREPAIR.Add(WORepair);
                    //objContext.SaveChanges();
                }
                else if (RepairsView.rState == (int)Validation.STATE.UPDATED)
                {
                    //success = ManageWorkOrderDAL.UpdateWORepair(RepairsView);

                    WORepair = (from wor in objContext.MESC1TS_WOREPAIR
                                where wor.WO_ID == WOID &&
                                wor.REPAIR_CD == RepairsView.RepairCode.RepairCod.Trim() &&
                                wor.REPAIR_LOC_CD == RepairsView.RepairLocationCode.CedexCode
                                select wor).FirstOrDefault();

                    //WORepair.WO_ID = RepairsView.WorkOrderID;
                    //WORepair.REPAIR_CD = RepairsView.RepairCode.RepairCod;
                    WORepair.MANUAL_CD = ManualCode;
                    WORepair.MODE = ModeCode;
                    WORepair.QTY_REPAIRS = (short)RepairsView.Pieces;
                    WORepair.SHOP_MATERIAL_AMT = RepairsView.MaterialCost;
                    WORepair.CPH_MATERIAL_AMT = RepairsView.MaterialCostCPH;
                    WORepair.ACTUAL_MANH = RepairsView.ManHoursPerPiece;
                    if (RepairsView.NonsCode == null)
                    {
                        WORepair.NONS_CD = null;
                    }
                    else
                    {
                        WORepair.NONS_CD = RepairsView.NonsCode.NonsCd;
                    }
                    WORepair.CHUSER = RepairsView.RepairCode.ChangeUser;
                    WORepair.CHTS = DateTime.Now;
                    WORepair.DAMAGE_CD = RepairsView.Damage.DamageCedexCode;
                    WORepair.REPAIR_LOC_CD = RepairsView.RepairLocationCode.CedexCode;
                    WORepair.TPI_CD = RepairsView.Tpi.CedexCode;
                    //objContext.MESC1TS_WOREPAIR.Add(WORepair);
                    //objContext.SaveChanges();
                }
                else if (RepairsView.rState == (int)Validation.STATE.DELETED)
                {
                    if (DeleteParts(RepairsView, out errorMessageList))
                    {
                        //success = ManageWorkOrderDAL.DeleteWORepair(RepairsView);

                        WORepair = (from wor in objContext.MESC1TS_WOREPAIR
                                    where wor.WO_ID == WOID &&
                                    wor.REPAIR_CD == RepairsView.RepairCode.RepairCod.Trim() &&
                                    wor.REPAIR_LOC_CD == RepairsView.RepairLocationCode.CedexCode
                                    select wor).FirstOrDefault();

                        objContext.MESC1TS_WOREPAIR.Remove(WORepair);
                        //objContext.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
                //if (RepairsView.rState != (int)Validation.STATE.DELETED)
                //    /* FP 5943 END */
                //    RepairsView.rState = (int)Validation.STATE.EXISTING;
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error while saving WorkOrder to Database";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }
            logEntry.Message = "Method Name : SaveRepair(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            return success;
        }

        public bool SaveParts(SparePartsView SparePartsView, int WOID, out List<ErrMessage> errorMessageList)
        {
            bool success = true;

            errorMessageList = new List<ErrMessage>();
            ErrMessage Message = new ErrMessage();
            MESC1TS_WOPART WOPart = new MESC1TS_WOPART();
            try
            {
                if (SparePartsView.pState == (int)Validation.STATE.NEW)
                {
                    WOPart.WO_ID = WOID;
                    WOPart.REPAIR_CD = SparePartsView.RepairCode.RepairCod.Trim();
                    //WOPart.PART_CD = SparePartsView.PartCode.ToString();
                    WOPart.PART_CD = SparePartsView.OwnerSuppliedPartsNumber;
                    WOPart.COST_LOCAL = (decimal)SparePartsView.CostLocal;
                    WOPart.COST_CPH = (decimal)SparePartsView.CostLocalCPH;
                    WOPart.SERIAL_NUMBER = SparePartsView.SerialNumber;
                    WOPart.QTY_PARTS = SparePartsView.Pieces;
                    WOPart.CHUSER = SparePartsView.RepairCode.ChangeUser;
                    WOPart.CHTS = DateTime.Now;
                    objContext.MESC1TS_WOPART.Add(WOPart);
                    //objContext.SaveChanges();

                }
                else if (SparePartsView.pState == (int)Validation.STATE.UPDATED)
                {
                    //success = ManageWorkOrderDAL.UpdateWOPart(SparePartsView);

                    WOPart = (from wop in objContext.MESC1TS_WOPART
                              where wop.WO_ID == SparePartsView.WorkOrderID &&
                              wop.REPAIR_CD.Trim() == SparePartsView.RepairCode.RepairCod.Trim() &&
                              wop.PART_CD == SparePartsView.OwnerSuppliedPartsNumber.ToString()
                              select wop).FirstOrDefault();

                    WOPart.WO_ID = WOID;
                    WOPart.REPAIR_CD = SparePartsView.RepairCode.RepairCod.Trim();
                    WOPart.PART_CD = SparePartsView.OwnerSuppliedPartsNumber.ToString();
                    WOPart.COST_LOCAL = (decimal)SparePartsView.CostLocal;
                    WOPart.COST_CPH = (decimal)SparePartsView.CostLocalCPH;
                    WOPart.SERIAL_NUMBER = SparePartsView.SerialNumber;
                    WOPart.QTY_PARTS = SparePartsView.Pieces;
                    WOPart.CHUSER = SparePartsView.RepairCode.ChangeUser;
                    WOPart.CHTS = DateTime.Now;
                    objContext.MESC1TS_WOPART.Add(WOPart);
                    //objContext.SaveChanges();
                }
                else if (SparePartsView.pState == (int)Validation.STATE.DELETED)
                {
                    //success = ManageWorkOrderDAL.DeleteWOPart(SparePartsView);
                    WOPart = (from wor in objContext.MESC1TS_WOPART
                              where wor.WO_ID == SparePartsView.WorkOrderID &&
                              wor.REPAIR_CD == SparePartsView.RepairCode.RepairCod.Trim() &&
                              wor.PART_CD == SparePartsView.OwnerSuppliedPartsNumber.ToString()
                              select wor).FirstOrDefault();

                    objContext.MESC1TS_WOPART.Remove(WOPart);
                    //objContext.SaveChanges();
                }
                if (SparePartsView.pState != (int)Validation.STATE.DELETED)
                    /* FP 5943 END */
                    SparePartsView.pState = (int)Validation.STATE.EXISTING;
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error while saving WorkOrder to Database";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }

            return success;
        }
        // Delete all parts for the repair code entered
        public bool DeleteParts(RepairsView repairView, out List<ErrMessage> errorMessageList)
        {
            bool success = false;

            errorMessageList = new List<ErrMessage>();
            ErrMessage Message = new ErrMessage();

            MESC1TS_WOREPAIR woRepair;

            try
            {

                woRepair = new MESC1TS_WOREPAIR();


                //     _bstr_t sSQL = " DELETE MESC1TS_WOREPAIR WHERE WO_ID= ";
                //      sSQL += rhsRecord.m_sWOID.c_str();
                //      sSQL += " AND REPAIR_CD = ";
                //      sSQL += m_StringUtil.FormatTextString(rhsRecord.m_sRepairCd, REPAIRCODELEN).c_str();

                //                List<MESC1TS_WOREPAIR> woRepLst = new List<MESC1TS_WOREPAIR>;
                //            woRepLst = (from woRep in objContext.MESC1TS_WOREPAIR 
                //                     where woRep.WO_ID == 
                //                           && woRep.REPAIR_CD == 
                //                              select woRep)


                //   bool success = false;
                //    objContext = new ManageMasterDataServiceEntities();
                //    List<MESC1TS_PAYAGENT> payAgentDBObject = new List<MESC1TS_PAYAGENT>();
                //    payAgentDBObject = (from pay in objContext.MESC1TS_PAYAGENT
                //     where pay.PAYAGENT_CD == RRISPayAgentCode
                //     select pay).ToList();
                //            string esqlQuery = @"SELECT VALUE pay
                //                FROM ManageMasterDataServiceEntities.MESC1TS_PAYAGENT as pay where pay.PAYAGENT_CD = @ln";
                //ObjectQuery<MESC1TS_PAYAGENT> query = new ObjectQuery<MESC1TS_PAYAGENT>(esqlQuery, objContext);
                //query.Parameters.Add(new ObjectParameter("ln", RRISPayAgentCode));
                //payAgentDBObject = query.ToList();
                //     objContext.DeleteObject(payAgentDBObject.First());
                //     objContext.SaveCha

                //            _bstr_t sSQL = "DELETE MESC1TS_WOPART WHERE WO_ID= ";
                //             sSQL += rhsRecord.m_sWOID.c_str();
                //             sSQL += " AND REPAIR_CD = ";
                //             sSQL += m_StringUtil.FormatTextString(rhsRecord.m_sRepairCd, REPAIRCODELEN).c_str();



            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error while deleting WorkOrder to Database";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;

            }

            return success;
        }

        public bool SaveRemark(RemarkEntry Remarks, int woID, string Chuser, DateTime? crts, out List<ErrMessage> errorMessageList)
        {
            bool success = true;
            MESC1TS_WOREMARK woRemark;


            errorMessageList = new List<ErrMessage>();
            ErrMessage Message = new ErrMessage();



            try
            {
                if (Remarks.rState == (int)Validation.STATE.NEW)
                {

                    woRemark = new MESC1TS_WOREMARK();
                    woRemark.WO_ID = Convert.ToInt32(woID);
                    woRemark.REMARK_TYPE = Remarks.RemarkType;
                    if (Remarks.SuspendCatID == 0)
                    {
                        woRemark.SUSPCAT_ID = null;
                    }
                    woRemark.REMARK = Remarks.Remark;
                    woRemark.CHUSER = Chuser;
                    woRemark.CRTS = DateTime.Now;
                    objContext.MESC1TS_WOREMARK.Add(woRemark);
                    objContext.SaveChanges();

                }
                else if (Remarks.rState == (int)Validation.STATE.UPDATED)
                {

                    woRemark = new MESC1TS_WOREMARK();


                    woRemark.REMARK_TYPE = Remarks.RemarkType;
                    if (Remarks.SuspendCatID == 0)
                    {
                        woRemark.SUSPCAT_ID = null;
                    }
                    woRemark.REMARK = Remarks.Remark;

                    //   woRemark.XMIT_DTE = Convert.ToDateTime(Remarks.)

                    woRemark.CHUSER = Chuser;
                    woRemark.CRTS = DateTime.Now;

                    objContext.SaveChanges();

                }


            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error while saving WorkOrder to Database";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

                return false;
            }

            return success;
        }

        #endregion

        private bool SaveWorkOrder(WorkOrderDetail workOrder, out List<ErrMessage> errorMessageList)
        {
            bool success = true;
            bool bnew = false;
            MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
            errorMessageList = new List<ErrMessage>();
            WorkOrderDetail OriginalWorkOrder = null;
            string auditComment = string.Empty;
            List<string> auditCommentList = new List<string>();
            string Mode = workOrder.EquipmentList[0].SelectedMode;
            string Manual = workOrder.Shop.Customer[0].ManualCode;
            // if this is not a new work order, and changes are detected
            // get a copy of existing original work order (needed for audit trail entries)
            // hold changes in audit trail helper for later save
            // ensure that status is allowed based on status in persisted record

            // Need to re-check statuses. It is possible that the original is
            // now in a deleted state (by another user at the same time.in a different location.
            try
            {
                if (workOrder.woState != (int)Validation.STATE.NEW)
                {
                    if (AuditRequired(workOrder))
                    {
                        OriginalWorkOrder = LoadWorkOrderDetails(workOrder.WorkOrderID);
                        if (OriginalWorkOrder != null) //It basically loads the entire WorkOrder object
                        {
                            if (!CheckWOStatus(workOrder, OriginalWorkOrder, out errorMessageList))
                            {
                                OriginalWorkOrder = new WorkOrderDetail();
                                return false;
                            }

                            //This does a comparison between the original and the new copy to see whether any change is done or not.
                            CheckForChanges(workOrder, OriginalWorkOrder, out auditCommentList);
                        }
                    }
                    else
                    {
                        Message = new ErrMessage();
                        Message.Message = "System error attempting to retrieve original estimate for audit trail comparison";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        errorMessageList.Add(Message);
                    }
                }

                if (workOrder.woState == (int)Validation.STATE.NEW)
                {
                    bnew = true;
                }

                //if (OriginalWorkOrder != null)
                //{
                bool CheckInspectionRequired = CheckIfInspectionsRequired(workOrder, OriginalWorkOrder);
                //}

                if (success)
                {
                    // Save Origianl state of all objects in collections. i.e., NEW,UPDATED etc.
                    //rhsRecord.SaveOriginalState();

                    if (workOrder.woState == (int)Validation.STATE.NEW)
                    {
                        try
                        {
                            success = GetIdentity(ref workOrder, out errorMessageList);
                            if (success)
                            {
                                success = InsertHeader(workOrder, out errorMessageList);
                            }

                        }

                        catch (Exception ex)
                        {
                            Message = new ErrMessage();
                            Message.Message = "System error while saving WorkOrder to Database";
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            logEntry.Message = ex.ToString();
                            Logger.Write(logEntry);
                            success = false;
                        }
                    }
                    else
                    {
                        if (workOrder.woState == (int)Validation.STATE.UPDATED)
                        {
                            success = UpdateHeader(workOrder, out errorMessageList);
                            //UpdateRevNo;
                        }
                        // Populate WO_ID throughout collections.
                        //AddWOID(workOrder);
                    }

                    // SAVE REPAIRS
                    foreach (var rItem in workOrder.RepairsViewList)
                    {
                        if (!IsRepairTaxCode(rItem.RepairCode))
                        {
                            if (rItem.rState != (int)Validation.STATE.EXISTING)
                            {
                                success = SaveRepair(rItem, workOrder.WorkOrderID, Mode, Manual, out errorMessageList);
                            }
                        }
                        if (workOrder.SparePartsViewList != null && workOrder.SparePartsViewList.Count > 0)
                        {
                            List<SparePartsView> SpareViewList = workOrder.SparePartsViewList.FindAll(spCode => spCode.RepairCode.RepairCod.Trim() == rItem.RepairCode.RepairCod.Trim());
                            if (rItem.rState == (int)Validation.STATE.EXISTING
                            || rItem.rState != (int)Validation.STATE.NEW
                                || rItem.rState != (int)Validation.STATE.UPDATED)
                            {
                                foreach (var spareParts in SpareViewList)
                                {
                                    success = SaveParts(spareParts, workOrder.WorkOrderID, out errorMessageList);
                                }

                            }
                        }
                    }

                    if (success)
                    {
                        foreach (var remItem in workOrder.RemarksList)
                        {
                            success = SaveRemark(remItem, workOrder.WorkOrderID, workOrder.ChangeUser, workOrder.ChangeTime, out errorMessageList);
                        }
                    }

                    //-------------------------------------------------------------------------------------------------------------------------

                    // Perform chassis inspection logic after successful save of WO and related collections
                    // bool set before WO saved - inspections only added for 'new' inspection repair cd records.
                    // Nothing to add for existing repaircodes. Of course after above save, all repair codes now 'exist'

                    if (success)
                    {
                        try
                        {
                            if (CheckInspectionRequired)
                            {
                                InsertChassisInspection(workOrder, out errorMessageList);
                            }
                        }

                        catch (Exception ex)
                        {
                            Message = new ErrMessage();
                            Message.Message = "Fail Chassis inspecition insert: %s";
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            errorMessageList.Add(Message);
                            logEntry.Message = ex.ToString();
                            Logger.Write(logEntry);
                            return false;
                        }
                    }

                }

                if (success)
                {
                    if (bnew)
                    {
                        auditComment = "Work Order: " + workOrder.WorkOrderID + " created on " + DateTime.Now + " by " + workOrder.ChangeUser;
                        //Call a new WCF method
                        WOAudit.WO_ID = workOrder.WorkOrderID;
                        WOAudit.CHTS = DateTime.Now;
                        WOAudit.AUDIT_TEXT = auditComment;
                        WOAudit.CHUSER = workOrder.ChangeUser;
                        objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                        objContext.SaveChanges();

                    }
                    else
                    {
                        if (auditCommentList != null && auditCommentList.Count > 0)
                        {
                            foreach (var comment in auditCommentList)
                            {
                                auditComment = "Work Order: " + workOrder.WorkOrderID + " updated on " + DateTime.Now + " by " + workOrder.ChangeUser;
                                WOAudit.WO_ID = workOrder.WorkOrderID;
                                WOAudit.CHTS = DateTime.Now;
                                WOAudit.AUDIT_TEXT = comment;
                                WOAudit.CHUSER = workOrder.ChangeUser;
                                objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                                objContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "Error saving to the audit trail";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                errorMessageList.Add(Message);
            }

            return success;
        }

        private bool AuditRequired(WorkOrderDetail WorkOrder)
        {
            if (WorkOrder.woState != (int)Validation.STATE.EXISTING) return true;
            foreach (var rItem in WorkOrder.RepairsViewList)
            {
                if (rItem.rState != (int)Validation.STATE.EXISTING) return true;
                foreach (var pItem in WorkOrder.SparePartsViewList)
                {
                    if (pItem.pState != (int)Validation.STATE.EXISTING) return true;
                }
            }
            return false;
        }

        private bool CheckForChangesForEdit(WorkOrderDetail NewWorkOrder, WorkOrderDetail OriginalWorkOrder)
        {

            bool isChanged = false;
            logEntry.Message = "Method Name : CheckForChangesForEdit(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            string Manual = NewWorkOrder.Shop.Customer[0].ManualCode;
            try
            {
                isChanged = CheckHeaderChangesForEdit(NewWorkOrder, OriginalWorkOrder, true);
                if (!isChanged)
                {
                    foreach (var rItem in NewWorkOrder.RepairsViewList)
                    {
                        RepairsView RepairsView = rItem;
                        RepairsView.RepairCode.ManualCode = Manual;
                        if (RepairsView.NonsCode == null)
                        {
                            RepairsView.NonsCode = new NonsCode();
                        }
                        RepairsView.RepairCode.ModeCode = NewWorkOrder.Mode;
                        //isChanged = CheckRepairChangesForEdit(RepairsView, OriginalWorkOrder, true);
                        CheckRepairChangesForEdit(ref RepairsView, OriginalWorkOrder, true);
                    }
                    if (NewWorkOrder.SparePartsViewList != null && NewWorkOrder.SparePartsViewList.Count > 0)
                    {
                        foreach (var pItem in NewWorkOrder.SparePartsViewList)
                        {
                            SparePartsView PartsView = pItem;
                            CheckPartChangesForEdit(pItem, OriginalWorkOrder, true);
                        }
                    }
                }


                foreach (var rItem in NewWorkOrder.RepairsViewList)
                {
                    if (rItem.rState == (int)Validation.STATE.UPDATED)
                    {
                        isChanged = true;
                    }
                }

                if (!isChanged)
                {
                    if (NewWorkOrder.SparePartsViewList != null && NewWorkOrder.SparePartsViewList.Count > 0)
                    {
                        foreach (var rItem in NewWorkOrder.SparePartsViewList)
                        {
                            if (rItem.pState == (int)Validation.STATE.UPDATED)
                            {
                                isChanged = true;
                            }
                        }
                    }
                }
                //isChanged = SetDeletedState(NewWorkOrder, OriginalWorkOrder);
            }

            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error while checking changes";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return false;
            }
            logEntry.Message = "Method Name : CheckForChangesForEdit(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            return isChanged;
        }

        private bool CheckForChanges(WorkOrderDetail NewWorkOrder, WorkOrderDetail OriginalWorkOrder, out List<string> Remarks)
        {
            Remarks = new List<string>();
            bool isChanged = false;
            try
            {
                isChanged = CheckHeaderChanges(NewWorkOrder, OriginalWorkOrder, true, out Remarks);
                foreach (var rItem in NewWorkOrder.RepairsViewList)
                {
                    RepairsView RepairsView = rItem;
                    RepairsView.RepairCode.ManualCode = NewWorkOrder.Shop.Customer[0].ManualCode;
                    if (RepairsView.NonsCode == null)
                    {
                        RepairsView.NonsCode = new NonsCode();
                    }
                    RepairsView.RepairCode.ModeCode = NewWorkOrder.Mode;

                    isChanged = CheckRepairChanges(RepairsView, OriginalWorkOrder, true, out Remarks);
                    foreach (var pItem in NewWorkOrder.SparePartsViewList)
                    {
                        SparePartsView PartsView = pItem;
                        isChanged = CheckPartChanges(pItem, OriginalWorkOrder, true, out Remarks);
                    }
                }
                isChanged = SetDeletedState(NewWorkOrder, OriginalWorkOrder);
            }

            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error while checking changes";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return false;
            }
            return isChanged;
        }

        private bool CheckHeaderChanges(WorkOrderDetail NewWorkOrder, WorkOrderDetail OriginalWorkOrder, bool RemarksReqd, out List<string> Remarks)
        {
            Remarks = new List<string>();
            string msg = string.Empty;
            bool isChanged = false;
            try
            {
                if (NewWorkOrder.Shop.Customer[0].ManualCode != OriginalWorkOrder.Shop.Customer[0].ManualCode)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Manual code changed from %s to %s </b><br>";//, FormatEmpty(OriginalWorkOrder.Shop.Customer[0].ManualCode).c_str(), FormatEmpty(rhsNewRecord.m_sMANUAL_CD);
                        Remarks.Add(msg);
                    }
                }

                if (!NewWorkOrder.Mode.Equals(OriginalWorkOrder.Mode, StringComparison.CurrentCultureIgnoreCase))
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Mode changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sMODE).c_str(), FormatEmpty(rhsNewRecord.m_sMODE);
                        Remarks.Add(msg);
                    }
                }
                if (!NewWorkOrder.WorkOrderType.Equals(OriginalWorkOrder.WorkOrderType, StringComparison.CurrentCultureIgnoreCase))
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Work order type changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sWOTYPE).c_str(), FormatEmpty(rhsNewRecord.m_sWOTYPE);
                        Remarks.Add(msg);
                    }
                }
                if (NewWorkOrder.VendorCode != OriginalWorkOrder.VendorCode)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Vendor code changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sVENDOR_CD).c_str(), FormatEmpty(rhsNewRecord.m_sVENDOR_CD);
                        Remarks.Add(msg);
                    }
                }
                if (!NewWorkOrder.Shop.ShopCode.Equals(OriginalWorkOrder.Shop.ShopCode, StringComparison.CurrentCultureIgnoreCase))
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Shop code changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sSHOP_CD).c_str(), FormatEmpty(rhsNewRecord.m_sSHOP_CD);
                        Remarks.Add(msg);
                    }
                }
                if (NewWorkOrder.RepairDate != OriginalWorkOrder.RepairDate)
                {
                    // Satadal(RQ 6355)
                    msg = "<b>Repair date changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sREPAIR_DTE).c_str(), FormatEmpty(rhsNewRecord.m_sREPAIR_DTE);
                    Remarks.Add(msg);
                }
                //	fputs(rhsNewRecord.m_sSTATUS_CODE.c_str(),fp);
                //	fputs("\n",fp);
                //	fputs(OriginalWorkOrder.m_sSTATUS_CODE.c_str(),fp);	
                if (NewWorkOrder.WorkOrderStatus != OriginalWorkOrder.WorkOrderStatus)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Status code changed from %s to %s </b><br>";// (char*)GetStatusDescription(OriginalWorkOrder.m_sSTATUS_CODE, (char*)GetStatusDescription(rhsNewRecord.m_sSTATUS_CODE); 
                        Remarks.Add(msg);
                    }
                }

                if (!NewWorkOrder.Cause.Equals(OriginalWorkOrder.Cause, StringComparison.CurrentCultureIgnoreCase))
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Cause changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sCAUSE).c_str(), FormatEmpty(rhsNewRecord.m_sCAUSE);
                        Remarks.Add(msg);
                    }
                }
                if (!NewWorkOrder.ThirdPartyPort.Equals(OriginalWorkOrder.ThirdPartyPort, StringComparison.CurrentCultureIgnoreCase))
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Third Party changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sTHIRD_PARTY).c_str(), FormatEmpty(rhsNewRecord.m_sTHIRD_PARTY);
                        Remarks.Add(msg);
                    }
                }

                // 
                NewWorkOrder.ManHourRate = Decimal.Round(NewWorkOrder.ManHourRate.Value, 2);
                OriginalWorkOrder.ManHourRate = Decimal.Round(OriginalWorkOrder.ManHourRate.Value, 2);
                if (NewWorkOrder.ManHourRate != OriginalWorkOrder.ManHourRate)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Man hour rate has been changed </b><br>";
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.ManHourRateCPH = Decimal.Round(NewWorkOrder.ManHourRateCPH.Value, 2);
                OriginalWorkOrder.ManHourRateCPH = Decimal.Round(OriginalWorkOrder.ManHourRateCPH.Value, 2); ;
                if (NewWorkOrder.ManHourRateCPH != OriginalWorkOrder.ManHourRateCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Man hour rate CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.ManHourRateCPH + "" + NewWorkOrder.ManHourRateCPH;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.ExchangeRate = Decimal.Round(NewWorkOrder.ExchangeRate.Value, 4);
                OriginalWorkOrder.ExchangeRate = Decimal.Round(OriginalWorkOrder.ExchangeRate.Value, 4);
                if (NewWorkOrder.ExchangeRate != OriginalWorkOrder.ExchangeRate)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Exchange rate changed from %s to %s </b><br>" + "" + OriginalWorkOrder.ExchangeRate + "" + NewWorkOrder.ExchangeRate;
                        Remarks.Add(msg);
                    }
                }


                if (NewWorkOrder.Shop.CUCDN != OriginalWorkOrder.Shop.CUCDN)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Currency code changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sCUCDN).c_str(), FormatEmpty(rhsNewRecord.m_sCUCDN);
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalRepairManHour = Math.Round(NewWorkOrder.TotalRepairManHour.Value, 2);
                OriginalWorkOrder.TotalRepairManHour = Math.Round(NewWorkOrder.TotalRepairManHour.Value, 2);
                if (NewWorkOrder.TotalRepairManHour != OriginalWorkOrder.TotalRepairManHour)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total repair man hours changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalRepairManHour + "" + NewWorkOrder.TotalRepairManHour;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalManHourReg = Math.Round(NewWorkOrder.TotalManHourReg.Value, 2);
                OriginalWorkOrder.TotalManHourReg = Math.Round(NewWorkOrder.TotalManHourReg.Value, 2);
                if (NewWorkOrder.TotalManHourReg != OriginalWorkOrder.TotalManHourReg)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total ordinary hours changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalManHourReg + "" + NewWorkOrder.TotalManHourReg;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalManHourOverTime = Math.Round(NewWorkOrder.TotalManHourOverTime.Value, 2);
                OriginalWorkOrder.TotalManHourOverTime = Math.Round(NewWorkOrder.TotalManHourOverTime.Value, 2);
                if (NewWorkOrder.TotalManHourOverTime != OriginalWorkOrder.TotalManHourOverTime)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total overtime 1 hours changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalManHourOverTime + "" + NewWorkOrder.TotalManHourOverTime;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalManHourDoubleTime = Math.Round(NewWorkOrder.TotalManHourDoubleTime.Value, 2);
                OriginalWorkOrder.TotalManHourDoubleTime = Math.Round(NewWorkOrder.TotalManHourDoubleTime.Value, 2);

                if (NewWorkOrder.TotalManHourDoubleTime != OriginalWorkOrder.TotalManHourDoubleTime)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total overtime 2 hours changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalManHourDoubleTime + "" + NewWorkOrder.TotalManHourDoubleTime;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalManHourMisc = Math.Round(NewWorkOrder.TotalManHourMisc.Value, 2);
                OriginalWorkOrder.TotalManHourMisc = Math.Round(NewWorkOrder.TotalManHourMisc.Value, 2);
                if (NewWorkOrder.TotalManHourMisc != OriginalWorkOrder.TotalManHourMisc)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total overtime 3 hours from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalManHourMisc + "" + NewWorkOrder.TotalManHourMisc;
                        Remarks.Add(msg);
                    }
                }

                //NewWorkOrder.TotalPrepHours = Math.Round(NewWorkOrder.TotalPrepHours.Value, 2);
                //OriginalWorkOrder.TotalPrepHours = Math.Round(NewWorkOrder.TotalPrepHours.Value, 2);
                if (NewWorkOrder.TotalPrepHours != OriginalWorkOrder.TotalPrepHours)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total preparation hours from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalPrepHours + "" + NewWorkOrder.TotalPrepHours;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalLaborHours = Math.Round(NewWorkOrder.TotalLaborHours.Value, 2);
                OriginalWorkOrder.TotalLaborHours = Math.Round(NewWorkOrder.TotalLaborHours.Value, 2);
                if (NewWorkOrder.TotalLaborHours != OriginalWorkOrder.TotalLaborHours)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total labor hours changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalLaborHours + "" + NewWorkOrder.TotalLaborHours;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalLabourCost = Math.Round(NewWorkOrder.TotalLabourCost.Value, 2);
                OriginalWorkOrder.TotalLabourCost = Math.Round(NewWorkOrder.TotalLabourCost.Value, 2);
                if (NewWorkOrder.TotalLabourCost != OriginalWorkOrder.TotalLabourCost)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total labor cost has changed </b><br>";
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalLabourCostCPH = Math.Round(NewWorkOrder.TotalLabourCostCPH.Value, 2);
                OriginalWorkOrder.TotalLabourCostCPH = Math.Round(NewWorkOrder.TotalLabourCostCPH.Value, 2);
                if (NewWorkOrder.TotalLabourCostCPH != OriginalWorkOrder.TotalLabourCostCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total labor cost CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalLabourCostCPH + "" + NewWorkOrder.TotalLabourCostCPH;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalShopAmount = Math.Round(NewWorkOrder.TotalShopAmount.Value, 2);
                OriginalWorkOrder.TotalShopAmount = Math.Round(NewWorkOrder.TotalShopAmount.Value, 2);
                if (NewWorkOrder.TotalShopAmount != OriginalWorkOrder.TotalShopAmount)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total shop amount has changed </b><br>";
                        Remarks.Add(msg);
                    }
                }

                //Mangal Release 3 RQ6344 checking the audit trail changes
                NewWorkOrder.TotalWMaterialAmount = Math.Round(NewWorkOrder.TotalWMaterialAmount.Value, 2);
                OriginalWorkOrder.TotalWMaterialAmount = Math.Round(NewWorkOrder.TotalWMaterialAmount.Value, 2);
                if (NewWorkOrder.TotalWMaterialAmount != OriginalWorkOrder.TotalWMaterialAmount)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total Wear and Tear material amount changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalWMaterialAmount + "" + NewWorkOrder.TotalWMaterialAmount;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalTMaterialAmount = Math.Round(NewWorkOrder.TotalTMaterialAmount.Value, 2);
                OriginalWorkOrder.TotalTMaterialAmount = Math.Round(NewWorkOrder.TotalTMaterialAmount.Value, 2);

                if (NewWorkOrder.TotalTMaterialAmount != OriginalWorkOrder.TotalTMaterialAmount)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total third party material amount changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalTMaterialAmount + "" + NewWorkOrder.TotalTMaterialAmount;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalWMaterialAmountCPHUSD = Math.Round(NewWorkOrder.TotalWMaterialAmountCPHUSD.Value, 2);
                OriginalWorkOrder.TotalWMaterialAmountCPHUSD = Math.Round(NewWorkOrder.TotalWMaterialAmountCPHUSD.Value, 2);
                if (NewWorkOrder.TotalWMaterialAmountCPHUSD != OriginalWorkOrder.TotalWMaterialAmountCPHUSD)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total Wear and Tear material amount CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalWMaterialAmountCPHUSD + "" + NewWorkOrder.TotalWMaterialAmountCPHUSD;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalTMaterialAmountCPHUSD = Math.Round(NewWorkOrder.TotalTMaterialAmountCPHUSD.Value, 2);
                OriginalWorkOrder.TotalTMaterialAmountCPHUSD = Math.Round(NewWorkOrder.TotalTMaterialAmountCPHUSD.Value, 2);
                if (NewWorkOrder.TotalTMaterialAmountCPHUSD != OriginalWorkOrder.TotalTMaterialAmountCPHUSD)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total third party material amount CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalTMaterialAmountCPHUSD + "" + NewWorkOrder.TotalTMaterialAmountCPHUSD;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalWMaterialAmountUSD = Math.Round(NewWorkOrder.TotalWMaterialAmountUSD.Value, 2);
                OriginalWorkOrder.TotalWMaterialAmountUSD = Math.Round(NewWorkOrder.TotalWMaterialAmountUSD.Value, 2);
                if (NewWorkOrder.TotalWMaterialAmountUSD != OriginalWorkOrder.TotalWMaterialAmountUSD)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total Wear and Tear material amount USD changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalWMaterialAmountUSD + "" + NewWorkOrder.TotalWMaterialAmountUSD;
                        Remarks.Add(msg);
                    }
                }


                NewWorkOrder.TotalTMaterialAmountUSD = Math.Round(NewWorkOrder.TotalTMaterialAmountUSD.Value, 2);
                OriginalWorkOrder.TotalTMaterialAmountUSD = Math.Round(NewWorkOrder.TotalTMaterialAmountUSD.Value, 2);
                if (NewWorkOrder.TotalTMaterialAmountUSD != OriginalWorkOrder.TotalTMaterialAmountUSD)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total third party material amount USD changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalTMaterialAmountUSD + "" + NewWorkOrder.TotalTMaterialAmountUSD;
                        Remarks.Add(msg);
                    }
                }


                //NewWorkOrder.TotalTMaterialAmountCPH = Math.Round(NewWorkOrder.TotalTMaterialAmountCPH.Value, 2);
                //OriginalWorkOrder.TotalTMaterialAmountCPH = Math.Round(NewWorkOrder.TotalTMaterialAmountCPH.Value, 2);
                //if (NewWorkOrder.TotalTMaterialAmountCPH != OriginalWorkOrder.TotalTMaterialAmountCPH)
                //{
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Total Wear and Tear material amount CPH USD changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalTMaterialAmountCPH + "" + NewWorkOrder.TotalTMaterialAmountCPH;
                //        Remarks.Add(msg);
                //    }
                //}

                NewWorkOrder.TotalWMaterialAmountCPH = Math.Round(NewWorkOrder.TotalWMaterialAmountCPH.Value, 2);
                OriginalWorkOrder.TotalWMaterialAmountCPH = Math.Round(NewWorkOrder.TotalWMaterialAmountCPH.Value, 2);
                if (NewWorkOrder.TotalWMaterialAmountCPH != OriginalWorkOrder.TotalWMaterialAmountCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total third party material amount CPH USD changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalWMaterialAmountCPH + "" + NewWorkOrder.TotalWMaterialAmountCPH;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalShopAmountCPH = Math.Round(NewWorkOrder.TotalShopAmountCPH.Value, 2);
                OriginalWorkOrder.TotalShopAmountCPH = Math.Round(NewWorkOrder.TotalShopAmountCPH.Value, 2);
                if (NewWorkOrder.TotalShopAmountCPH != OriginalWorkOrder.TotalShopAmountCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total shop amount CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalShopAmountCPH + "" + NewWorkOrder.TotalShopAmountCPH;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalCostLocal = Math.Round(NewWorkOrder.TotalCostLocal.Value, 2);
                OriginalWorkOrder.TotalCostLocal = Math.Round(NewWorkOrder.TotalCostLocal.Value, 2);
                if (NewWorkOrder.TotalCostLocal != OriginalWorkOrder.TotalCostLocal)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total cost local has changed  </b><br>";
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalCostCPH = Math.Round(NewWorkOrder.TotalCostCPH.Value, 2);
                OriginalWorkOrder.TotalCostCPH = Math.Round(NewWorkOrder.TotalCostCPH.Value, 2);
                if (NewWorkOrder.TotalCostCPH != OriginalWorkOrder.TotalCostCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total cost CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalCostCPH + "" + NewWorkOrder.TotalCostCPH;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.OverTimeRate = Math.Round(NewWorkOrder.OverTimeRate.Value, 2);
                OriginalWorkOrder.OverTimeRate = Math.Round(NewWorkOrder.OverTimeRate.Value, 2);
                if (NewWorkOrder.OverTimeRate != OriginalWorkOrder.OverTimeRate)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Overtime 1 rate has been changed </b><br>";
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.OverTimeRateCPH = Math.Round(NewWorkOrder.OverTimeRateCPH.Value, 2);
                OriginalWorkOrder.OverTimeRateCPH = Math.Round(NewWorkOrder.OverTimeRateCPH.Value, 2);
                if (NewWorkOrder.OverTimeRateCPH != OriginalWorkOrder.OverTimeRateCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Overtime 1 rate CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.OverTimeRateCPH + "" + NewWorkOrder.OverTimeRateCPH;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.DoubleTimeRate = Math.Round(NewWorkOrder.DoubleTimeRate.Value, 2);
                OriginalWorkOrder.DoubleTimeRate = Math.Round(NewWorkOrder.DoubleTimeRate.Value, 2);
                if (NewWorkOrder.DoubleTimeRate != OriginalWorkOrder.DoubleTimeRate)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Overtime 2 rate has changed </b><br>";
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.DoubleTimeRateCPH = Math.Round(NewWorkOrder.DoubleTimeRateCPH.Value, 2);
                OriginalWorkOrder.DoubleTimeRateCPH = Math.Round(NewWorkOrder.DoubleTimeRateCPH.Value, 2);
                if (NewWorkOrder.DoubleTimeRateCPH != OriginalWorkOrder.DoubleTimeRateCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Overtime 2 rate CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.DoubleTimeRateCPH + "" + NewWorkOrder.DoubleTimeRateCPH;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.MiscRate = Math.Round(NewWorkOrder.MiscRate.Value, 2);
                OriginalWorkOrder.MiscRate = Math.Round(NewWorkOrder.MiscRate.Value, 2);
                if (NewWorkOrder.MiscRate != OriginalWorkOrder.MiscRate)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Overtime 3 rate has changed </b><br>";
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.MiscRateCPH = Math.Round(NewWorkOrder.MiscRateCPH.Value, 2);
                OriginalWorkOrder.MiscRateCPH = Math.Round(NewWorkOrder.MiscRateCPH.Value, 2);
                if (NewWorkOrder.MiscRateCPH != OriginalWorkOrder.MiscRateCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Overtime 3 rate CHP changed from %s to %s </b><br>" + "" + OriginalWorkOrder.MiscRateCPH + "" + NewWorkOrder.MiscRateCPH;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalCostLocalUSD = Math.Round(NewWorkOrder.TotalCostLocalUSD.Value, 2);
                OriginalWorkOrder.TotalCostLocalUSD = Math.Round(NewWorkOrder.TotalCostLocalUSD.Value, 2);

                if (NewWorkOrder.TotalCostLocalUSD != OriginalWorkOrder.TotalCostLocalUSD)
                {
                    // do not show values
                    //		sprintf( msg, "Total cost local USD changed from %s to %s <br>";// szOld, szNew );
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total cost local USD has changed </b><br>";
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalCostOfRepair = Math.Round(NewWorkOrder.TotalCostOfRepair.Value, 2);
                OriginalWorkOrder.TotalCostOfRepair = Math.Round(NewWorkOrder.TotalCostOfRepair.Value, 2);
                if (NewWorkOrder.TotalCostOfRepair != OriginalWorkOrder.TotalCostOfRepair)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total repair cost has changed </b><br>";
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalCostOfRepairCPH = Math.Round(NewWorkOrder.TotalCostOfRepairCPH.Value, 2);
                OriginalWorkOrder.TotalCostOfRepairCPH = Math.Round(NewWorkOrder.TotalCostOfRepairCPH.Value, 2);

                if (NewWorkOrder.TotalCostOfRepairCPH != OriginalWorkOrder.TotalCostOfRepairCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total repair cost CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalCostOfRepairCPH + "" + NewWorkOrder.TotalCostOfRepairCPH;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.SalesTaxLabour = Math.Round(NewWorkOrder.SalesTaxLabour.Value, 2);
                OriginalWorkOrder.SalesTaxLabour = Math.Round(NewWorkOrder.SalesTaxLabour.Value, 2);

                if (NewWorkOrder.SalesTaxLabour != OriginalWorkOrder.SalesTaxLabour)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Sales tax labor has changed </b><br>";
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.SalesTaxLabourCPH = Math.Round(NewWorkOrder.SalesTaxLabourCPH.Value, 2);
                OriginalWorkOrder.SalesTaxLabourCPH = Math.Round(NewWorkOrder.SalesTaxLabourCPH.Value, 2);
                if (NewWorkOrder.SalesTaxLabourCPH != OriginalWorkOrder.SalesTaxLabourCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Sales tax labor CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.SalesTaxLabourCPH + "" + NewWorkOrder.SalesTaxLabourCPH;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.SalesTaxParts = Math.Round(NewWorkOrder.SalesTaxParts.Value, 2);
                OriginalWorkOrder.SalesTaxParts = Math.Round(NewWorkOrder.SalesTaxParts.Value, 2);
                if (NewWorkOrder.SalesTaxParts != OriginalWorkOrder.SalesTaxParts)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Sales tax parts has changed </b><br>";
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.SalesTaxPartsCPH = Math.Round(NewWorkOrder.SalesTaxPartsCPH.Value, 2);
                OriginalWorkOrder.SalesTaxPartsCPH = Math.Round(NewWorkOrder.SalesTaxPartsCPH.Value, 2);
                if (NewWorkOrder.SalesTaxPartsCPH != OriginalWorkOrder.SalesTaxPartsCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Sales tax parts CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.SalesTaxPartsCPH + "" + NewWorkOrder.SalesTaxPartsCPH;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalMaerksParts = Math.Round(NewWorkOrder.TotalMaerksParts.Value, 2);
                OriginalWorkOrder.TotalMaerksParts = Math.Round(NewWorkOrder.TotalMaerksParts.Value, 2);
                if (NewWorkOrder.TotalMaerksParts != OriginalWorkOrder.TotalMaerksParts)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total maersk parts changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalMaerksParts + "" + NewWorkOrder.TotalMaerksParts;
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.TotalMaerksPartsCPH = Math.Round(NewWorkOrder.TotalMaerksPartsCPH.Value, 2);
                OriginalWorkOrder.TotalMaerksPartsCPH = Math.Round(NewWorkOrder.TotalMaerksPartsCPH.Value, 2);
                if (NewWorkOrder.TotalMaerksPartsCPH != OriginalWorkOrder.TotalMaerksPartsCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Total maersk parts CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalMaerksPartsCPH + "" + NewWorkOrder.TotalMaerksPartsCPH;
                        Remarks.Add(msg);
                    }
                }



                //if (!NewWorkOrder.EquipmentList[0].VendorRefNo.Equals(OriginalWorkOrder.EquipmentList[0].VendorRefNo, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Vendor reference number changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sVENDOR_REF_NO).c_str(), FormatEmpty(rhsNewRecord.m_sVENDOR_REF_NO);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (!NewWorkOrder.ApprovedBy.Equals(OriginalWorkOrder.ApprovedBy, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Approved by changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sAPPROVED_BY).c_str(), FormatEmpty(rhsNewRecord.m_sAPPROVED_BY);
                //        Remarks.Add(msg);
                //    }
                //}
                if (NewWorkOrder.ShopWorkingSW != null && OriginalWorkOrder.ShopWorkingSW != null)
                {
                    if (!NewWorkOrder.ShopWorkingSW.Equals(OriginalWorkOrder.ShopWorkingSW, StringComparison.CurrentCultureIgnoreCase))
                    {
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Shop working switch changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sSHOP_WORKING_SW).c_str(), FormatEmpty(rhsNewRecord.m_sSHOP_WORKING_SW);
                            Remarks.Add(msg);
                        }
                    }
                }
                //if (NewWorkOrder.ApprovalDate != OriginalWorkOrder.ApprovalDate)
                //{
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Approval date changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sAPPROVAL_DTE).c_str(), FormatEmpty(rhsNewRecord.m_sAPPROVAL_DTE);
                //        Remarks.Add(msg);
                //    }
                //}

                NewWorkOrder.ImportTax = Math.Round(NewWorkOrder.ImportTax.Value, 2);
                OriginalWorkOrder.ImportTax = Math.Round(NewWorkOrder.ImportTax.Value, 2);
                if (NewWorkOrder.ImportTax != OriginalWorkOrder.ImportTax)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Import tax has changed </b><br>";
                        Remarks.Add(msg);
                    }
                }

                NewWorkOrder.ImportTaxCPH = Math.Round(NewWorkOrder.ImportTaxCPH.Value, 2);
                OriginalWorkOrder.ImportTaxCPH = Math.Round(NewWorkOrder.ImportTaxCPH.Value, 2);

                if (NewWorkOrder.ImportTaxCPH != OriginalWorkOrder.ImportTaxCPH)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Import tax CPH changed from %s to %s </b><br>" + "" + OriginalWorkOrder.ImportTaxCPH + "" + NewWorkOrder.ImportTaxCPH;
                        Remarks.Add(msg);
                    }
                }

                if (NewWorkOrder.ChangeUser != null && OriginalWorkOrder.ChangeUser != null)
                {
                    if (!NewWorkOrder.ChangeUser.Equals(OriginalWorkOrder.ChangeUser, StringComparison.CurrentCultureIgnoreCase))
                    {
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Last user changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sCHUSER).c_str(), FormatEmpty(rhsNewRecord.m_sCHUSER);
                            Remarks.Add(msg);
                        }
                    }
                }
                if (NewWorkOrder.ChangeTime != OriginalWorkOrder.ChangeTime)
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Change timestamp changed from %s to %s </b><br>";// OriginalWorkOrder.m_sCHTS.c_str(), rhsNewRecord.m_sCHTS;
                        Remarks.Add(msg);
                    }
                }
                if (!NewWorkOrder.EquipmentList[0].EquipmentNo.Equals(OriginalWorkOrder.EquipmentList[0].EquipmentNo, StringComparison.CurrentCultureIgnoreCase))
                {
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Equipment number changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQPNO).c_str(), FormatEmpty(rhsNewRecord.m_sEQPNO);
                        Remarks.Add(msg);
                    }
                }
                if (NewWorkOrder.SalesTaxLaborPCT != OriginalWorkOrder.SalesTaxLaborPCT)
                {
                    // Satadal(RQ 6355)
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Sales tax labor percent changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sSALES_TAX_LABOR_PCT).c_str(), FormatEmpty(rhsNewRecord.m_sSALES_TAX_LABOR_PCT);
                        Remarks.Add(msg);
                    }
                }
                if (NewWorkOrder.SalesTaxPartsPCT != OriginalWorkOrder.SalesTaxPartsPCT)
                {
                    // Satadal(RQ 6355)
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Sales tax parts percent changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sSALES_TAX_PARTS_PCT).c_str(), FormatEmpty(rhsNewRecord.m_sSALES_TAX_PARTS_PCT);
                        Remarks.Add(msg);
                    }
                }
                if (NewWorkOrder.ImportTaxPCT != OriginalWorkOrder.ImportTaxPCT)
                {
                    // Satadal(RQ 6355)
                    isChanged = true;
                    if (RemarksReqd)
                    {
                        msg = "<b>Import tax percent changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sIMPORT_TAX_PCT).c_str(), FormatEmpty(rhsNewRecord.m_sIMPORT_TAX_PCT);
                        Remarks.Add(msg);
                    }
                }

                if (isChanged)
                {
                    NewWorkOrder.woState = (int)Validation.STATE.UPDATED;
                }
                else
                {
                    NewWorkOrder.woState = (int)Validation.STATE.EXISTING;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return isChanged;

        }

        private bool CheckHeaderChangesForEdit(WorkOrderDetail NewWorkOrder, WorkOrderDetail OriginalWorkOrder, bool RemarksReqd)
        {

            bool isChanged = false;

            try
            {
                if (!NewWorkOrder.Mode.Equals(OriginalWorkOrder.Mode, StringComparison.CurrentCultureIgnoreCase))
                {
                    isChanged = true;

                }
                //if (!NewWorkOrder.VendorCode.Equals(OriginalWorkOrder.VendorCode, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Vendor code changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sVENDOR_CD).c_str(), FormatEmpty(rhsNewRecord.m_sVENDOR_CD);
                //        Remarks.Add(msg);
                //    }
                //}

                if (!NewWorkOrder.Cause.Equals(OriginalWorkOrder.Cause, StringComparison.CurrentCultureIgnoreCase))
                {
                    isChanged = true;

                }
                if (!NewWorkOrder.ThirdPartyPort.Equals(OriginalWorkOrder.ThirdPartyPort, StringComparison.CurrentCultureIgnoreCase))
                {
                    isChanged = true;

                }


                if (!NewWorkOrder.Shop.Currency.Cucdn.Equals(OriginalWorkOrder.Shop.Currency.Cucdn, StringComparison.CurrentCultureIgnoreCase))
                {
                    isChanged = true;

                }

                //NewWorkOrder.TotalRepairManHour = Math.Round(NewWorkOrder.TotalRepairManHour.Value, 2);
                //OriginalWorkOrder.TotalRepairManHour = Math.Round(NewWorkOrder.TotalRepairManHour.Value, 2);
                //if (NewWorkOrder.TotalRepairManHour != OriginalWorkOrder.TotalRepairManHour)
                //{
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Total repair man hours changed from %s to %s </b><br>" + "" + OriginalWorkOrder.TotalRepairManHour + "" + NewWorkOrder.TotalRepairManHour;
                //        Remarks.Add(msg);
                //    }
                //}

                NewWorkOrder.TotalManHourReg = Math.Round(NewWorkOrder.TotalManHourReg.Value, 2);
                OriginalWorkOrder.TotalManHourReg = Math.Round(NewWorkOrder.TotalManHourReg.Value, 2);
                if (NewWorkOrder.TotalManHourReg != OriginalWorkOrder.TotalManHourReg)
                {
                    isChanged = true;

                }

                NewWorkOrder.TotalManHourOverTime = Math.Round(NewWorkOrder.TotalManHourOverTime.Value, 2);
                OriginalWorkOrder.TotalManHourOverTime = Math.Round(NewWorkOrder.TotalManHourOverTime.Value, 2);
                if (NewWorkOrder.TotalManHourOverTime != OriginalWorkOrder.TotalManHourOverTime)
                {
                    isChanged = true;

                }

                NewWorkOrder.TotalManHourDoubleTime = Math.Round(NewWorkOrder.TotalManHourDoubleTime.Value, 2);
                OriginalWorkOrder.TotalManHourDoubleTime = Math.Round(NewWorkOrder.TotalManHourDoubleTime.Value, 2);

                if (NewWorkOrder.TotalManHourDoubleTime != OriginalWorkOrder.TotalManHourDoubleTime)
                {
                    isChanged = true;

                }

                NewWorkOrder.TotalManHourMisc = Math.Round(NewWorkOrder.TotalManHourMisc.Value, 2);
                OriginalWorkOrder.TotalManHourMisc = Math.Round(NewWorkOrder.TotalManHourMisc.Value, 2);
                if (NewWorkOrder.TotalManHourMisc != OriginalWorkOrder.TotalManHourMisc)
                {
                    isChanged = true;

                }


                if (NewWorkOrder.SalesTaxLabour != null)
                {
                    NewWorkOrder.SalesTaxLabour = Math.Round(NewWorkOrder.SalesTaxLabour.Value, 2);
                    OriginalWorkOrder.SalesTaxLabour = Math.Round(NewWorkOrder.SalesTaxLabour.Value, 2);

                    if (NewWorkOrder.SalesTaxLabour != OriginalWorkOrder.SalesTaxLabour)
                    {
                        isChanged = true;

                    }
                }


                if (NewWorkOrder.SalesTaxParts != null)
                {
                    NewWorkOrder.SalesTaxParts = Math.Round(NewWorkOrder.SalesTaxParts.Value, 2);
                    OriginalWorkOrder.SalesTaxParts = Math.Round(NewWorkOrder.SalesTaxParts.Value, 2);
                    if (NewWorkOrder.SalesTaxParts != OriginalWorkOrder.SalesTaxParts)
                    {
                        isChanged = true;

                    }
                }


                if (!string.IsNullOrEmpty(NewWorkOrder.EquipmentList[0].VendorRefNo) && !NewWorkOrder.EquipmentList[0].VendorRefNo.Equals(OriginalWorkOrder.EquipmentList[0].VendorRefNo, StringComparison.CurrentCultureIgnoreCase))
                {
                    isChanged = true;

                }

                if (NewWorkOrder.ImportTax != null)
                {
                    NewWorkOrder.ImportTax = Math.Round(NewWorkOrder.ImportTax.Value, 2);
                    OriginalWorkOrder.ImportTax = Math.Round(NewWorkOrder.ImportTax.Value, 2);
                    if (NewWorkOrder.ImportTax != OriginalWorkOrder.ImportTax)
                    {
                        isChanged = true;

                    }
                }




                //if (!NewWorkOrder.ChangeUser.Equals(OriginalWorkOrder.ChangeUser, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Last user changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sCHUSER).c_str(), FormatEmpty(rhsNewRecord.m_sCHUSER);
                //        Remarks.Add(msg);
                //    }
                //}

                //if (!NewWorkOrder.EquipmentList[0].EquipmentNo.Equals(OriginalWorkOrder.EquipmentList[0].EquipmentNo, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment number changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQPNO).c_str(), FormatEmpty(rhsNewRecord.m_sEQPNO);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (!NewWorkOrder.EquipmentList[0].Size.Equals(OriginalWorkOrder.EquipmentList[0].Size, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment size changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQSIZE).c_str(), FormatEmpty(rhsNewRecord.m_sEQSIZE);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (!NewWorkOrder.EquipmentList[0].Type.Equals(OriginalWorkOrder.EquipmentList[0].Type, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment type changed from %s to %s </b><br>";// OriginalWorkOrder.m_sEQTYPE.c_str(), rhsNewRecord.m_sEQTYPE;
                //        Remarks.Add(msg);
                //    }
                //}
                //if (!NewWorkOrder.EquipmentList[0].Eqouthgu.Equals(OriginalWorkOrder.EquipmentList[0].Eqouthgu, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    // Satadal(RQ 6355)
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Eqipment height changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQOUTHGU).c_str(), FormatEmpty(rhsNewRecord.m_sEQOUTHGU);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (!NewWorkOrder.EquipmentList[0].COType.Equals(OriginalWorkOrder.EquipmentList[0].COType, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    // Satadal(RQ 6355)
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment cotype changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sCOTYPE).c_str(), FormatEmpty(rhsNewRecord.m_sCOTYPE);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (!NewWorkOrder.EquipmentList[0].SubType.Equals(OriginalWorkOrder.EquipmentList[0].SubType, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment sub type changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQSTYPE).c_str(), FormatEmpty(rhsNewRecord.m_sEQSTYPE);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (!NewWorkOrder.EquipmentList[0].Eqowntp.Equals(OriginalWorkOrder.EquipmentList[0].Eqowntp, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    // Satadal(RQ 6355)
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment owner type changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQOWNTP).c_str(), FormatEmpty(rhsNewRecord.m_sEQOWNTP);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (!NewWorkOrder.EquipmentList[0].Eqmatr.Equals(OriginalWorkOrder.EquipmentList[0].Eqmatr, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment material changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQMATR).c_str(), FormatEmpty(rhsNewRecord.m_sEQMATR);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (NewWorkOrder.Deldatsh != OriginalWorkOrder.Deldatsh)
                //{
                //    // Satadal(RQ 6355)
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment PTI date changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sDELDATSH).c_str(), FormatEmpty(rhsNewRecord.m_sDELDATSH);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (NewWorkOrder.StEmptyFullInd != OriginalWorkOrder.StEmptyFullInd)
                //{
                //    // Satadal(RQ 6355)
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment full-empty status changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sSTEMPTY).c_str(), FormatEmpty(rhsNewRecord.m_sSTEMPTY);
                //        Remarks.Add(msg);
                //    }
                //}

                //if (!NewWorkOrder.EquipmentList[0].EqMancd.Equals(OriginalWorkOrder.EquipmentList[0].EqMancd, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    // Satadal(RQ 6355)
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment manufacturer code changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQMANCD).c_str(), FormatEmpty(rhsNewRecord.m_sEQMANCD);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (!NewWorkOrder.EquipmentList[0].EQProfile.Equals(OriginalWorkOrder.EquipmentList[0].EQProfile, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    // Satadal(RQ 6355)
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment profile changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQPROFIL).c_str(), FormatEmpty(rhsNewRecord.m_sEQPROFIL);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (NewWorkOrder.EquipmentList[0].EQInDate != OriginalWorkOrder.EquipmentList[0].EQInDate)
                //{
                //    // Satadal(RQ 6355)
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment production date changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQINDAT).c_str(), FormatEmpty(rhsNewRecord.m_sEQINDAT);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (!NewWorkOrder.EquipmentList[0].EQRuman.Equals(OriginalWorkOrder.EquipmentList[0].EQRuman, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    // Satadal(RQ 6355)
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment make changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQRUMAN).c_str(), FormatEmpty(rhsNewRecord.m_sEQRUMAN);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (!NewWorkOrder.EquipmentList[0].EQRutyp.Equals(OriginalWorkOrder.EquipmentList[0].EQRutyp, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    // Satadal(RQ 6355)
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment model changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQRUTYP).c_str(), FormatEmpty(rhsNewRecord.m_sEQRUTYP);
                //        Remarks.Add(msg);
                //    }
                //}
                //if (!NewWorkOrder.EquipmentList[0].EQIoflt.Equals(OriginalWorkOrder.EquipmentList[0].EQIoflt, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    // Satadal(RQ 6355)
                //    isChanged = true;
                //    if (RemarksReqd)
                //    {
                //        msg = "<b>Equipment inactive switcyh changed from %s to %s </b><br>";// FormatEmpty(OriginalWorkOrder.m_sEQIOFLT).c_str(), FormatEmpty(rhsNewRecord.m_sEQIOFLT);
                //        Remarks.Add(msg);
                //    }
                //}


                if (isChanged)
                {
                    NewWorkOrder.woState = (int)Validation.STATE.UPDATED;
                }
                else
                {
                    NewWorkOrder.woState = (int)Validation.STATE.EXISTING;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return isChanged;

        }

        private bool CheckRepairChanges(RepairsView RepairsView, WorkOrderDetail OriginalWorkOrder, bool RemarksReqd, out List<string> Remarks)
        {
            Remarks = new List<string>();
            string msg = string.Empty;
            bool isChanged = false;

            // iterate through repair collection and associated parts collections.
            try
            {
                // ignore tax code repair records.
                if (IsRepairTaxCode(RepairsView.RepairCode)) return isChanged;

                // no changes, exit
                if (RepairsView.rState == (int)Validation.STATE.EXISTING) return isChanged; ;

                if (RepairsView.rState == (int)Validation.STATE.DELETED)
                {
                    // VJP - 2005-07-14 - no need to show CHUSER here - will be shown in audit trail anyway from current user
                    //		sprintf(msg, "Repair code %s deleted by %s. <br>";// pRepair->m_sRepairCd.c_str(), FormatEmpty( pRepair->m_sCHUser ).c_str() );
                    msg = "<b>Repair code %s deleted</b><br>" + "" + RepairsView.RepairCode.RepairCod.Trim();
                    Remarks.Add(msg);
                    return isChanged;
                }

                if (RepairsView.rState == (int)Validation.STATE.NEW)
                {
                    // VJP - 2005-07-14 - no need to show CHUSER here - will be shown in audit trail anyway from current user
                    //		sprintf(msg, "Repair code %s added by %s. <br>";// pRepair->m_sRepairCd.c_str(), FormatEmpty( pRepair->m_sCHUser ).c_str() );
                    msg = "<b>Repair code %s added</b><br>" + "" + RepairsView.RepairCode.RepairCod.Trim();
                    Remarks.Add(msg);
                    return isChanged;
                }

                bool bFound = true;
                RepairsView oldRepairsView = new RepairsView();
                for (int i = 0; i < OriginalWorkOrder.RepairsViewList.Count; i++)
                {
                    oldRepairsView = OriginalWorkOrder.RepairsViewList[i];

                    if (RepairsView.RepairCode.RepairCod.Equals(oldRepairsView.RepairCode.RepairCod.TrimEnd(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        bFound = true;
                        break;
                    }
                }

                // if record is updated and original repair record found, enter changes
                if (RepairsView.rState == (int)Validation.STATE.UPDATED && bFound)
                {
                    if (RepairsView.RepairCode.ManualCode != oldRepairsView.RepairCode.ManualCode)
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Manual code changed from %s to %s for repair: %s </b><br>";// oldRepairsView.RepairCode.ManualCd.c_str(), pRepair->m_sManualCd.c_str(), pRepair->m_sRepairCd.c_str());
                            Remarks.Add(msg);
                        }
                    }
                    if (RepairsView.RepairCode.ModeCode != oldRepairsView.RepairCode.ModeCode)
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Mode changed from %s to %s for repair: %s </b><br>";// oldRepairsView.RepairCode.WOMode.c_str(), pRepair->m_sWOMode.c_str(), pRepair->m_sRepairCd.c_str());
                            Remarks.Add(msg);
                        }
                    }
                    if (RepairsView.Pieces != oldRepairsView.Pieces)
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Quantity pieces changed from %s to %s for repair: %s </b><br>";// oldRepairsView.RepairCode.Pieces.c_str(), pRepair->m_sPieces.c_str(), pRepair->m_sRepairCd.c_str());
                            Remarks.Add(msg);
                        }
                    }

                    RepairsView.MaterialCost = Math.Round(RepairsView.MaterialCost.Value, 2);
                    oldRepairsView.MaterialCost = Math.Round(oldRepairsView.MaterialCost.Value, 2);

                    if (RepairsView.MaterialCost != oldRepairsView.MaterialCost)
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Material amount changed from %s to %s for repair: %s </b>" + "" + RepairsView.MaterialCost + "" + oldRepairsView.MaterialCost + "" + RepairsView.RepairCode.RepairCod.Trim();
                            Remarks.Add(msg);
                        }
                    }

                    RepairsView.MaterialCostCPH = Math.Round(RepairsView.MaterialCostCPH.Value, 2);
                    oldRepairsView.MaterialCostCPH = Math.Round(oldRepairsView.MaterialCostCPH.Value, 2);
                    if (RepairsView.MaterialCostCPH != oldRepairsView.MaterialCostCPH)
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Material amount CPH changed from %s to %s for repair: %s </b>" + "" + oldRepairsView.MaterialCostCPH + "" + RepairsView.MaterialCostCPH + "" + RepairsView.RepairCode.RepairCod.Trim();
                            Remarks.Add(msg);
                        }
                    }

                    RepairsView.ManHoursPerPiece = Math.Round(RepairsView.ManHoursPerPiece.Value, 2);
                    oldRepairsView.ManHoursPerPiece = Math.Round(oldRepairsView.ManHoursPerPiece.Value, 2);
                    if (RepairsView.ManHoursPerPiece != oldRepairsView.ManHoursPerPiece)
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Man hours changed from %s to %s for repair: %s </b>" + "" + oldRepairsView.ManHoursPerPiece + "" + RepairsView.ManHoursPerPiece + "" + RepairsView.RepairCode.RepairCod.Trim();
                            Remarks.Add(msg);
                        }
                    }



                    if (!RepairsView.NonsCode.NonsCd.Equals(oldRepairsView.NonsCode.NonsCd, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Nons Code changed from %s to %s for repair: %s </b><br>";// pRepair->m_sNonsCd.c_str(), oldRepairsView.RepairCode.NonsCd.c_str(), pRepair->m_sRepairCd.c_str());
                            Remarks.Add(msg);
                        }
                    }

                }

                if (isChanged)
                {
                    RepairsView.rState = (int)Validation.STATE.UPDATED;
                }
                else
                {
                    RepairsView.rState = (int)Validation.STATE.EXISTING;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return isChanged;
        }

        private void CheckRepairChangesForEdit(ref RepairsView RepairsView, WorkOrderDetail OriginalWorkOrder, bool RemarksReqd)
        {

            bool isChanged = false;

            // iterate through repair collection and associated parts collections.

            // ignore tax code repair records.
            try
            {
                if (IsRepairTaxCode(RepairsView.RepairCode)) return; // isChanged;

                // no changes, exit
                if (RepairsView.rState == (int)Validation.STATE.EXISTING) return; // isChanged; ;

                //if (RepairsView.rState == (int)Validation.STATE.DELETED)
                //{
                //    // VJP - 2005-07-14 - no need to show CHUSER here - will be shown in audit trail anyway from current user
                //    //		sprintf(msg, "Repair code %s deleted by %s. <br>";// pRepair->m_sRepairCd.c_str(), FormatEmpty( pRepair->m_sCHUser ).c_str() );
                //    //msg = "<b>Repair code %s deleted</b><br>" + "" + RepairsView.RepairCode.RepairCod;
                //    //Remarks.Add(msg);
                //    return isChanged;
                //}

                //if (RepairsView.rState == (int)Validation.STATE.NEW)
                //{
                //    // VJP - 2005-07-14 - no need to show CHUSER here - will be shown in audit trail anyway from current user
                //    //		sprintf(msg, "Repair code %s added by %s. <br>";// pRepair->m_sRepairCd.c_str(), FormatEmpty( pRepair->m_sCHUser ).c_str() );
                //    //msg = "<b>Repair code %s added</b><br>" + "" + RepairsView.RepairCode.RepairCod;
                //    //Remarks.Add(msg);
                //    return isChanged;
                //}

                bool bFound = false;
                RepairsView oldRepairsView = new RepairsView();
                for (int i = 0; i < OriginalWorkOrder.RepairsViewList.Count; i++)
                {
                    oldRepairsView = OriginalWorkOrder.RepairsViewList[i];

                    if (RepairsView.RepairCode.RepairCod.Equals(oldRepairsView.RepairCode.RepairCod.TrimEnd(), StringComparison.CurrentCultureIgnoreCase)
                        && (RepairsView.RepairLocationCode.CedexCode.Equals(oldRepairsView.RepairLocationCode.CedexCode.TrimEnd(), StringComparison.CurrentCultureIgnoreCase)))
                    {
                        bFound = true;
                        break;
                    }
                }
                if (bFound)
                {
                    //if (RepairsView.RepairCode.ManualCode.Equals(oldRepairsView.RepairCode.ManualCode, StringComparison.CurrentCultureIgnoreCase))
                    //{
                    //    // Satadal(RQ 6355)
                    //    isChanged = true;

                    //}
                    //if (!RepairsView.RepairCode.ModeCode.Equals(oldRepairsView.RepairCode.ModeCode, StringComparison.CurrentCultureIgnoreCase))
                    //{
                    //    // Satadal(RQ 6355)
                    //    isChanged = true;

                    //}
                    //if(
                    if (RepairsView.Damage.DamageCedexCode != oldRepairsView.Damage.DamageCedexCode)
                    {
                        isChanged = true;
                    }
                    if (RepairsView.Pieces != oldRepairsView.Pieces)
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;

                    }

                    if (RepairsView.MaterialCost != null)
                    {
                        RepairsView.MaterialCost = Math.Round(RepairsView.MaterialCost.Value, 2);
                    }
                    if (oldRepairsView.MaterialCost != null)
                    {
                        oldRepairsView.MaterialCost = Math.Round(oldRepairsView.MaterialCost.Value, 2);
                    }

                    if (RepairsView.MaterialCost != oldRepairsView.MaterialCost)
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;
                    }

                    if (RepairsView.ManHoursPerPiece != null)
                    {
                        RepairsView.ManHoursPerPiece = Math.Round(RepairsView.ManHoursPerPiece.Value, 2);
                    }
                    if (oldRepairsView.ManHoursPerPiece != null)
                    {
                        oldRepairsView.ManHoursPerPiece = Math.Round(oldRepairsView.ManHoursPerPiece.Value, 2);
                    }
                    if (RepairsView.ManHoursPerPiece != oldRepairsView.ManHoursPerPiece)
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;

                    }
                    if (RepairsView.Tpi.CedexCode != oldRepairsView.Tpi.CedexCode)
                    {
                        isChanged = true;
                    }
                }


                if (isChanged)
                {
                    RepairsView.rState = (int)Validation.STATE.UPDATED;
                }
                else
                {
                    RepairsView.rState = (int)Validation.STATE.EXISTING;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            //return isChanged;
        }

        private bool CheckPartChanges(SparePartsView PartsView, WorkOrderDetail OriginalWorkOrder, bool RemarksReqd, out List<string> Remarks)
        {
            Remarks = new List<string>();
            string msg = string.Empty;

            bool isChanged = false;

            try
            {
                // no changes, exit
                if (PartsView.pState == (int)Validation.STATE.EXISTING) return isChanged;

                //if (PartsView.pState == (int)Validation.STATE.DELETED)
                //{
                //    // VJP - 2005-07-14 - no need to show CHUSER here - will be shown in audit trail anyway from current user
                //    //		sprintf(msg, "Repair code %s deleted by %s. <br>";// pRepair->m_sRepairCd.c_str(), FormatEmpty( pRepair->m_sCHUser ).c_str() );
                //    msg = "<b>Part %s deleted for repair: %s. </b><br>" + "" + PartsView.RepairCode.RepairCod;
                //    Remarks.Add(msg);
                //    return;
                //}

                //if (PartsView.pState == (int)Validation.STATE.NEW)
                //{
                //    // VJP - 2005-07-14 - no need to show CHUSER here - will be shown in audit trail anyway from current user
                //    //		sprintf(msg, "Repair code %s added by %s. <br>";// pRepair->m_sRepairCd.c_str(), FormatEmpty( pRepair->m_sCHUser ).c_str() );
                //    msg = "<b>Part %s added for repair code %s</b><br>" + "" + PartsView.RepairCode.RepairCod;
                //    Remarks.Add(msg);
                //    return;
                //}

                SparePartsView oldPartsView = new SparePartsView();
                for (int i = 0; i < OriginalWorkOrder.SparePartsViewList.Count; i++)
                {
                    oldPartsView = OriginalWorkOrder.SparePartsViewList[i];

                    //if (oldPartsView.PartCode != oldPartsView.PartCode)
                    //{
                    //    bFound = true;
                    //    break;
                    //}
                }


                // if record is updated and original repair record found, enter changes
                if (PartsView.pState == (int)Validation.STATE.UPDATED) // && bFound)
                {
                    if (oldPartsView.OwnerSuppliedPartsNumber != oldPartsView.OwnerSuppliedPartsNumber)
                    {
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Part Code changed from %s to %s for part: %s </b><br>" + " " + oldPartsView.OwnerSuppliedPartsNumber + " " + PartsView.OwnerSuppliedPartsNumber;
                            Remarks.Add(msg);
                        }
                    }

                    PartsView.CostLocal = Math.Round(PartsView.CostLocal.Value, 2);
                    oldPartsView.CostLocal = Math.Round(oldPartsView.CostLocal.Value, 2);
                    if (PartsView.CostLocal != oldPartsView.CostLocal)
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Cost changed from %s to %s for part: %s </b><br>" + " " + oldPartsView.CostLocal + " " + PartsView.CostLocal;
                            Remarks.Add(msg);
                        }
                    }

                    PartsView.CostLocalCPH = Math.Round(PartsView.CostLocalCPH.Value, 2);
                    oldPartsView.CostLocalCPH = Math.Round(oldPartsView.CostLocalCPH.Value, 2);

                    if (PartsView.CostLocalCPH != oldPartsView.CostLocalCPH)
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Cost CPH changed from %s to %s for part: %s </b><br>" + " " + oldPartsView.CostLocalCPH + " " + PartsView.CostLocalCPH;
                            Remarks.Add(msg);
                        }
                    }

                    if (PartsView.SerialNumber.Equals(oldPartsView.SerialNumber, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Serial number changed from %s to %s for part: %s </b><br>";// FormatEmpty(pOldPart->m_SerialNumber).c_str(), FormatEmpty(pPart->m_SerialNumber).c_str(), FormatEmpty(pPart->m_sPartCode).c_str());
                            Remarks.Add(msg);
                        }
                    }

                    if (PartsView.Pieces != oldPartsView.Pieces)
                    {
                        // Satadal(RQ 6355)
                        isChanged = true;
                        if (RemarksReqd)
                        {
                            msg = "<b>Quantity of pieces changed from %s to %s for part: %s </b><br>";// pPart->m_sPieces.c_str(), pOldPart->m_sPieces.c_str(), pPart->m_sPartCode.c_str());
                            Remarks.Add(msg);
                        }
                    }

                }
                if (isChanged)
                {
                    PartsView.pState = (int)Validation.STATE.UPDATED;
                }
                else
                {
                    PartsView.pState = (int)Validation.STATE.EXISTING;
                }

            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error while checking changes";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return false;
            }
            return isChanged;
        }

        private void CheckPartChangesForEdit(SparePartsView PartsView, WorkOrderDetail OriginalWorkOrder, bool RemarksReqd)
        {

            bool isChanged = false;

            // no changes, exit
            try
            {
                if (PartsView.pState == (int)Validation.STATE.EXISTING) return; // isChanged;
                SparePartsView oldPartsView = new SparePartsView();
                for (int i = 0; i < OriginalWorkOrder.SparePartsViewList.Count; i++)
                {
                    oldPartsView = OriginalWorkOrder.SparePartsViewList[i];
                }


                if (oldPartsView.OwnerSuppliedPartsNumber != oldPartsView.OwnerSuppliedPartsNumber)
                {
                    isChanged = true;

                }


                if (PartsView.Pieces != oldPartsView.Pieces)
                {
                    // Satadal(RQ 6355)
                    isChanged = true;

                }

                if (isChanged)
                {
                    PartsView.pState = (int)Validation.STATE.UPDATED;
                }
                else
                {
                    PartsView.pState = (int)Validation.STATE.EXISTING;
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error while checking changes";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return;
            }
            //return isChanged;
        }

        private bool CheckWOStatus(WorkOrderDetail UpdatedWorkOrder, WorkOrderDetail OriginalWorkOrder, out List<ErrMessage> errorMessageList)
        {
            bool success = true;
            int oldStatus = (int)OriginalWorkOrder.WorkOrderStatus;
            int newStatus = (int)UpdatedWorkOrder.WorkOrderStatus;

            errorMessageList = new List<ErrMessage>();
            try
            {
                if ((oldStatus >= 400) && (oldStatus < 9900))
                {
                    if (newStatus < oldStatus)
                    {
                        Message = new ErrMessage();
                        Message.Message = "Work order updated by a different user. Unable to apply changes.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        errorMessageList.Add(Message);
                        success = false;
                    }
                }

                if (oldStatus == 9999)
                {
                    Message = new ErrMessage();
                    Message.Message = "Work order has been deleted by another user - no changes permitted";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    success = false;
                }
                if (oldStatus == 9998)
                {
                    Message = new ErrMessage();
                    Message.Message = "Work order has been voided by another user - no changes permitted";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    success = false;
                }
                if (oldStatus == 9997)
                {
                    Message = new ErrMessage();
                    Message.Message = "Work order has been marked for sale by another user - no changes permitted";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    success = false;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }

        // this check must be performed before the WO and associated collections are saved.
        // New inspection only if repair code of 0940(inspection) is part of a 'completed' RepairCd
        // record. This is the first call in the Save() routine - need to know before collections
        // are saved and marked as 'EXISTING'.
        private bool CheckIfInspectionsRequired(WorkOrderDetail WorkOrder, WorkOrderDetail OriginalWorkOrder)
        {
            bool success = true;
            try
            {
                // check if this work order is in process of being completed.
                if (WorkOrder.WorkOrderStatus == 400 && OriginalWorkOrder.WorkOrderStatus != 400)
                {
                    foreach (var item in WorkOrder.RepairsViewList)
                    {
                        // if repair code = 0940 PTICODE then inspection insert is required
                        if (item.RepairCode.RepairCod.Trim() == PTICODE)
                        {
                            success = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return success;
        }

        // already determined that this WO had a new 0940 Repair record in collection
        // so... check if there is a header for chassis equipment
        // if not, add one - done in MESC_usp_addChassis stored proc
        // get count of inspection records in inspection table, for equipment number
        // if < 2 simply insert this new inspection
        // else delete all but the highest INSP_DTE, then insert new inspection.
        private bool InsertChassisInspection(WorkOrderDetail WorkOrder, out List<ErrMessage> errorMessageList)
        {
            bool success = false;
            string tempEQ = string.Empty;

            errorMessageList = new List<ErrMessage>();
            // Inspection list - need count 
            // if Count > 1 then MoveLast, get INSP_DTE, delete all not = INSP_DTE
            // Basically leave only the last inspection record in the table.
            string eqpNo = WorkOrder.EquipmentList[0].EquipmentNo;
            var InspectionList = (from ins in objContext.MESC1TS_INSPECTION
                                  where ins.CHAS_EQPNO == eqpNo
                                  select new
                                  {
                                      ins.INSP_DTE
                                  }).OrderBy(insp => insp.INSP_DTE).ToList();

            try
            {
                if (InspectionList.Count > 0)
                {
                    foreach (var item in InspectionList)
                    {
                        // delete all inspection records not containing the highest inspection date.
                        var findInspDateRecords = (from ins in objContext.MESC1TS_INSPECTION
                                                   where ins.INSP_DTE != item.INSP_DTE &&
                                                   ins.CHAS_EQPNO == eqpNo
                                                   select ins).ToList();

                        foreach (var ins in findInspDateRecords)
                        {
                            objContext.MESC1TS_INSPECTION.Remove(ins);
                            objContext.SaveChanges();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "Fail delete of inspection records:";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }

            // Need location code from the shop for insert in the inspection table.
            if (string.IsNullOrEmpty(WorkOrder.Shop.LocationCode))
            {
                try
                {
                    var shop = (from s in objContext.MESC1TS_SHOP
                                where s.SHOP_CD == WorkOrder.Shop.ShopCode
                                select new
                                {
                                    s.LOC_CD
                                }).FirstOrDefault();
                    if (shop != null)
                    {
                        WorkOrder.Shop.LocationCode = shop.LOC_CD;
                    }
                }

                catch (Exception ex)
                {
                    Message = new ErrMessage();
                    Message.Message = ": System Error reading Location Code: ";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    success = false;
                }
            }

            try
            {
                //check if not exists clause
                var getIfExists = (from ins in objContext.MESC1TS_INSPECTION
                                   where ins.CHAS_EQPNO == eqpNo &&
                                   ins.INSP_DTE == WorkOrder.RepairDate &&
                                   ins.INSP_BY == WorkOrder.Shop.ShopCode &&
                                   ins.RKEMLOC == WorkOrder.Shop.LocationCode
                                   select ins).ToList();

                if (getIfExists == null || getIfExists.Count == 0)
                {
                    MESC1TS_INSPECTION inspection = new MESC1TS_INSPECTION();
                    inspection.CHAS_EQPNO = WorkOrder.EquipmentList[0].EquipmentNo;
                    inspection.INSP_DTE = (DateTime)WorkOrder.RepairDate;
                    inspection.INSP_BY = WorkOrder.Shop.ShopCode;
                    inspection.RKEMLOC = WorkOrder.Shop.LocationCode;

                    objContext.MESC1TS_INSPECTION.Add(inspection);
                }

            }

            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = ": Fail to insert in inspection table: ";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }
            return success;
        }

        private bool CheckRepairDate(WorkOrderDetail workOrder, out List<ErrMessage> errorMessageList)
        {

            errorMessageList = new List<ErrMessage>();
            DateTime tempRep = (DateTime)workOrder.RepairDate;
            try
            {
                if (workOrder.RepairDate == null)
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO:  " + workOrder.EquipmentList[0].EquipmentNo + " " + " Repair date is required";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    return false;
                }
                // Verify date - ensure that date is valid and is in proper format.
                //Is it necessary to do this check? we are using a DT picker
                //_bstr_t sError = pDate->Validate(sDate.c_str(), &i);WorkOrder.RepairDate

                // check if date is > current date
                // IsExpired returns 0 if equal, 1 is currentdate is > and -1 if current date is <
                if (tempRep.Date > DateTime.Now.Date)
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO:  " + workOrder.EquipmentList[0].EquipmentNo + " " + " Unable to Complete Estimate. Repair date must not be older than 1st day of previous month, nor later than the current date (UTC)";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    return false;
                }

                // date is valid and not > than current date, now ensure that date is not less than 1st day of previous month
                // Ensure that date is not < 1st day of previous month.
                // build oldest allowed date.
                //COleDateTime tOldestDate;
                //tOldestDate.SetDate(year, month, 1);


                //COleDateTime tRepair;
                //tRepair.SetDate(atoi(sDate.substr(0, 4).c_str()), atoi(sDate.substr(5, 2).c_str()), atoi(sDate.substr(8, 2).c_str()));

                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                if (month > 1)
                {
                    month -= 1;
                }
                else
                {
                    month = 12;
                    year -= 1;
                }

                DateTime setDate = new DateTime(year, month, 1);
                ////Incomplete
                //setDate.AddYears(year);
                //setDate.AddMonths(month);
                //setDate.AddDays(1);


                if (tempRep.Date < setDate.Date)
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO:  " + workOrder.EquipmentList[0].EquipmentNo + " " + " Unable to Complete Estimate. Repair date must not be older than 1st day of previous month, nor later than the current date (UTC)";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return true;
        }

        public List<ErrMessage> ApproveWorkOrder(int WOID, string User, string OldStatusOrRemark, string VendorRefNo)
        {
            MESC1TS_WO PrevWODataFromDB = new MESC1TS_WO();
            string LocCode = string.Empty;
            int? PrevID = 0;
            string Date = string.Empty;
            short? StatusCodeTemp = 0;
            short? Status = 390;
            //var PrevWOID;
            MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
            MESC1TS_WOREMARK WORemark = new MESC1TS_WOREMARK();
            List<ErrMessage> errorMessageList = new List<ErrMessage>();
            try
            {
                //if (Status == APPROVEDSTATUS)
                //{
                var PrevWOData = (from W in objContext.MESC1TS_WO
                                  where W.WO_ID == WOID
                                  select new
                                  {
                                      W.prev_wo_id
                                  }).FirstOrDefault();

                if (PrevWOData != null)
                {
                    PrevID = PrevWOData.prev_wo_id;
                }
                //}

                if (PrevID == null || PrevID == 0)
                {
                    var PrevWOID = (from w1 in objContext.MESC1TS_WO
                                    join w2 in objContext.MESC1TS_WO on w1.EQPNO equals w2.EQPNO
                                    where w1.WO_ID == WOID &&
                                    w1.CRTS > w2.CRTS
                                    select new
                                    {
                                        w2.WO_ID,
                                        w2.CRTS
                                    }).OrderByDescending(w2 => w2.CRTS).Take(1).ToList();

                    if (PrevWOID != null) //if previous wo_id found, buffer it else change it to -1
                    {
                        PrevID = PrevWOID[0].WO_ID;
                    }
                    else
                    {
                        PrevID = -1;
                    }
                }
                if (PrevID == -1) //-1 for work orders without prev data
                {
                    LocCode = string.Empty;
                    Date = "9999-12-31";
                    StatusCodeTemp = null;
                }
                else
                {
                    var WOData = (from wo in objContext.MESC1TS_WO
                                  from s in objContext.MESC1TS_SHOP
                                  where wo.WO_ID == PrevID &&
                                  wo.SHOP_CD == s.SHOP_CD
                                  select new
                                  {
                                      wo.STATUS_CODE,
                                      wo.CHTS,
                                      s.LOC_CD
                                  }).ToList();

                    if (WOData != null && WOData.Count > 0)
                    {
                        LocCode = WOData[0].LOC_CD;
                        Date = WOData[0].CHTS.ToString();
                        StatusCodeTemp = WOData[0].STATUS_CODE;
                    }
                }

                var WOFromDB = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == WOID
                                select wo).FirstOrDefault();

                if (WOFromDB != null)
                {
                    if (390 > WOFromDB.STATUS_CODE)
                    {
                        //"'"; rhsRecord.m_sPREV_STATUS += _bstr_t(cStatus); rhsRecord.m_sPREV_STATUS += "'";
                        WOFromDB.STATUS_CODE = 390;
                        WOFromDB.CHUSER = User;
                        WOFromDB.CHTS = DateTime.Now;
                        WOFromDB.APPROVAL_DTE = DateTime.Now;
                        WOFromDB.PREV_STATUS = Status;
                        WOFromDB.PREV_DATE = Convert.ToDateTime(Date);
                        WOFromDB.PREV_LOC_CD = LocCode;
                        objContext.SaveChanges();
                    }
                }

                WOAudit.WO_ID = WOID;
                WOAudit.CHUSER = User;
                WOAudit.CHTS = DateTime.Now;
                WOAudit.AUDIT_TEXT = "Approved by " + User;
                objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                objContext.SaveChanges();

                WORemark.WO_ID = WOID;
                WORemark.REMARK_TYPE = "S";
                WORemark.SUSPCAT_ID = null;
                WORemark.REMARK = OldStatusOrRemark;
                WORemark.CHUSER = User;
                WORemark.CRTS = DateTime.Now;
            }

            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "Not all selected estimates were approved; please try again, or approve them manually: <br>";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                //success = false;
            }

            return errorMessageList;
        }


        public short? GetRevNo(int WOID)
        {
            List<WorkOrderDetail> WorkOrder = new List<WorkOrderDetail>();
            short? RevNo = 0;
            List<WorkOrderDetail> WorkOrderDetails = new List<WorkOrderDetail>();
            WorkOrderDetail WOD = new WorkOrderDetail();
            try
            {
                var WorkOrderFromDB = (from wo in objContext.MESC1TS_WO
                                       where wo.WO_ID == WOID
                                       select new
                                       {
                                           wo.REVISION_NO
                                       }).ToList();

                if ((WorkOrderFromDB != null && WorkOrderFromDB.Count > 0) && (WorkOrderFromDB[0].REVISION_NO != null))
                {
                    RevNo = WorkOrderFromDB[0].REVISION_NO;
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return RevNo;
        }

        private string ChangeStatusOfWorkOrder(int WOID, short? Status, string User)
        {
            string sResult = "";
            bool success = true;
            //CAuditHelper audit;

            short? OldStatus = 0;
            short? NewStatus = Status;

            int nNewStatus = 0;
            int nOldStatus = 0;
            try
            {
                // if UI accidentally attempts to approve , explain correct use via CompleteWorkOrder methods 
                if (Status == COMPLETEDWO)
                {
                    success = false;
                    sResult = "Cannot set work order to completed. Module must use the CompleteWorkOrder method";
                }

                // if UI accidentally attempts to approve , explain correct use via CompleteWorkOrder methods 
                if (Status == APPROVEDSTATUS)
                {
                    success = false;
                    sResult = "Cannot set work order to approved. Module must use the ApproveWorkOrder method";
                }

                // Check if WO exists - get current status
                if (success)
                {
                    OldStatus = GetWorkOrderStatus(WOID);
                    if (OldStatus == null)
                    {
                        sResult = "Work Order ID: %s not found in table, unable to set status" + "" + WOID;
                        success = false;
                    }
                }

                // if yes check value of status
                // i.e., status chages above 400 ok if wo status >= 400 etc.

                if (success)
                {
                    nNewStatus = (int)Status;
                    nOldStatus = (int)OldStatus;

                    if ((nOldStatus >= 400) && (nOldStatus < 9900))
                    {
                        if (nNewStatus < nOldStatus)
                        {
                            sResult = "Unable to set status below current status in work order";
                            success = false;
                        }
                    }

                    if (nOldStatus == 9999)
                    {
                        sResult = "Work order has been deleted - unable to changes status";
                        success = false;
                    }

                    if (nOldStatus == 9998)
                    {
                        sResult = "Work order has been voided - unable to changes status";
                        success = false;
                    }

                    if (nOldStatus == 9997)
                    {
                        sResult = "Work order has been marked for sale - unable to changes status";
                        success = false;
                    }
                }

                // Hord code test of status codes.- is it a valid status code.
                bool bValid;
                switch (nNewStatus)
                {
                    case 000: bValid = true; break;
                    case 100: bValid = true; break;
                    case 130: bValid = true; break;
                    case 140: bValid = true; break;
                    // RQ7407 (Satadal)
                    // Date April, 2009
                    // Here 150 corresponds to new status "Total Loss"
                    case 150: bValid = true; break;
                    // End
                    case 200: bValid = true; break;
                    case 310: bValid = true; break;
                    case 320: bValid = true; break;
                    case 330: bValid = true; break;
                    case 340: bValid = true; break;
                    case 390: bValid = true; break;
                    case 400: bValid = true; break;
                    case 500: bValid = true; break;
                    case 550: bValid = true; break;
                    case 600: bValid = true; break;
                    case 800: bValid = true; break;
                    case 900: bValid = true; break;
                    case 9997: bValid = true; break;
                    case 9998: bValid = true; break;
                    case 9999: bValid = true; break;
                    default: bValid = false; break;
                }

                if (!bValid)
                {
                    success = false;
                    string msg = "Unknown status code: " + Status + " " + ", unable to set status." + "";
                    sResult = msg;
                }


                // Special case for drafts. We will physically remove the draft estimate from the
                // database.
                if ((nOldStatus == 0) && (nNewStatus == 9999))
                {
                    success = RemoveDraft(WOID);
                    if (!success)
                    {
                        sResult = "Failed delete of draft estimate - system error";
                    }
                    else
                    {
                        // we are done with a draft, do not insert into the audit trail
                        success = false;
                    }
                }

                // Set work order status
                if (success)
                {
                    success = UpdateStatus(WOID, Status, User, OldStatus);
                    if (!success)
                    {
                        sResult = "Failed status change to database - system error";
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return sResult;

        }

        public short? GetWorkOrderStatus(int WOID)
        {
            short? ID = 0;

            try
            {
                var WorkOrderStatus = (from wo in objContext.MESC1TS_WO
                                       where wo.WO_ID == WOID
                                       select new
                                       {
                                           wo.STATUS_CODE
                                       }).ToList();
                if (WorkOrderStatus != null && WorkOrderStatus.Count > 0)
                {
                    ID = WorkOrderStatus[0].STATUS_CODE;
                }
            }

            catch
            {
            }

            return ID;
        }

        public bool RemoveDraft(int WOID)
        {
            bool success = false;
            try
            {
                var DeleteRemark = (from rem in objContext.MESC1TS_WOREMARK
                                    where rem.WO_ID == WOID
                                    select rem).ToList();

                foreach (MESC1TS_WOREMARK remark in DeleteRemark)
                {
                    objContext.MESC1TS_WOREMARK.Remove(remark);
                    objContext.SaveChanges();
                }
                //objContext.DeleteObject(DeleteRemark.First());


                var DeletePart = (from part in objContext.MESC1TS_WOPART
                                  where part.WO_ID == WOID
                                  select part).ToList();

                foreach (MESC1TS_WOPART part in DeletePart)
                {
                    objContext.MESC1TS_WOPART.Remove(part);
                    objContext.SaveChanges();
                }

                var DeleteRepair = (from rep in objContext.MESC1TS_WOREPAIR
                                    where rep.WO_ID == WOID
                                    select rep).ToList();

                foreach (MESC1TS_WOREPAIR repair in DeleteRepair)
                {
                    objContext.MESC1TS_WOREPAIR.Remove(repair);
                    objContext.SaveChanges();
                }

                var DeleteAudit = (from audit in objContext.MESC1TS_WOAUDIT
                                   where audit.WO_ID == WOID
                                   select audit).ToList();

                foreach (MESC1TS_WOAUDIT woAudit in DeleteAudit)
                {
                    objContext.MESC1TS_WOAUDIT.Remove(woAudit);
                    objContext.SaveChanges();
                }

                var DeleteWO = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == WOID
                                select wo).ToList();

                foreach (MESC1TS_WO wo in DeleteWO)
                {
                    objContext.MESC1TS_WO.Remove(wo);
                    objContext.SaveChanges();
                }
                success = true;
            }
            catch
            {
                success = false;
            }

            return success;
        }

        public bool UpdateStatus(int WOID, short? Status, string ChangeUser, short? oldStatus)
        {
            bool success = true;
            int? prevWOID = 0;
            string loc = string.Empty;
            short? statusCode = null;
            DateTime date;
            MESC1TS_WO workOrderDB = new MESC1TS_WO();
            MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
            try
            {
                if (Status == APPROVEDSTATUS)
                {
                    //Pick up previous WO ID
                    workOrderDB = (from wo in objContext.MESC1TS_WO
                                   where wo.WO_ID == WOID
                                   select wo).FirstOrDefault();

                    prevWOID = workOrderDB.prev_wo_id;

                    //For unbuffed(before prev id was introduced), unapproved WO's
                    if (prevWOID == null || prevWOID == 0)
                    {
                        workOrderDB = (from wo1 in objContext.MESC1TS_WO
                                       join wo2 in objContext.MESC1TS_WO on new { WO1 = wo1.EQPNO }
                                       equals new { WO1 = wo2.EQPNO } into INNER
                                       from O in INNER.DefaultIfEmpty()
                                       where wo1.WO_ID == WOID &&
                                       wo1.CRTS > O.CRTS
                                       select O).OrderByDescending(crts => crts.CRTS).FirstOrDefault();
                    }

                    //if previous WO does not exist, set prev_id to -1(means no prev id)
                    if (prevWOID == null || prevWOID == 0)
                    {
                        prevWOID = -1;
                    }

                    //Prev id is -1, i.e., no previous WO exist
                    if (prevWOID == -1)
                    {
                        loc = string.Empty;
                        date = DateTime.MinValue;
                        statusCode = null;
                    }
                    else
                    {
                        var WO = (from wo in objContext.MESC1TS_WO
                                  from s in objContext.MESC1TS_SHOP
                                  where wo.WO_ID == WOID &&
                                  wo.SHOP_CD == s.SHOP_CD
                                  select new
                                  {
                                      wo.WO_ID,
                                      wo.STATUS_CODE,
                                      wo.CHTS,
                                      s.LOC_CD
                                  }).FirstOrDefault();

                        statusCode = WO.STATUS_CODE;
                        date = WO.CHTS;
                        loc = WO.LOC_CD;
                    }

                    //sSQL += "if exists (select WO_ID from mesc1ts_wo where wo_id = ";
                    //sSQL += result;
                    //sSQL += ") begin ";
                    //sSQL += "if 390 > (select status_code from mesc1ts_wo where wo_id = ";
                    //sSQL += result;
                    //sSQL += ") begin ";

                    workOrderDB = (from wo in objContext.MESC1TS_WO
                                   where wo.WO_ID == WOID
                                   select wo).FirstOrDefault();

                    workOrderDB.STATUS_CODE = 390;
                    workOrderDB.CHUSER = ChangeUser;
                    workOrderDB.CHTS = DateTime.Now;
                    workOrderDB.APPROVAL_DTE = DateTime.Now;
                    workOrderDB.PREV_STATUS = statusCode;
                    workOrderDB.PREV_DATE = date;
                    workOrderDB.PREV_LOC_CD = loc;
                    objContext.SaveChanges();

                    WOAudit.WO_ID = WOID;
                    WOAudit.CHTS = DateTime.Now;
                    WOAudit.AUDIT_TEXT = "Approved by: " + ChangeUser;
                    WOAudit.CHUSER = ChangeUser;
                    objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                    objContext.SaveChanges();
                }
                else
                {
                    workOrderDB = (from wo in objContext.MESC1TS_WO
                                   where wo.WO_ID == WOID
                                   select wo).FirstOrDefault();

                    workOrderDB.STATUS_CODE = Status;
                    workOrderDB.CHUSER = ChangeUser;
                    workOrderDB.CHTS = DateTime.Now;
                    objContext.SaveChanges();
                    //workOrderDB[0].APPROVAL_DTE = DateTime.Now;
                    //workOrderDB[0].PREV_STATUS = statusCode;
                    //workOrderDB[0].PREV_DATE = date;
                    //workOrderDB[0].PREV_LOC_CD = loc;

                    WOAudit.WO_ID = WOID;
                    WOAudit.CHTS = DateTime.Now;
                    WOAudit.AUDIT_TEXT = "Work Order: " + WOID + " status changed from " + oldStatus + " to " + Status + " by " + ChangeUser;
                    WOAudit.CHUSER = ChangeUser;
                    objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                    objContext.SaveChanges();
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return success;
        }

        private void CheckForWarnings(WorkOrderDetail WorkOrder, out List<ErrMessage> MessageList)
        {
            MessageList = new List<ErrMessage>();
            ErrMessage WarningMessage = new ErrMessage();
            try
            {
                // check part collections for required serial numbers for core deductible parts
                if (WorkOrder.SparePartsViewList != null && WorkOrder.SparePartsViewList.Count > 0)
                {
                    foreach (var pItem in WorkOrder.SparePartsViewList)
                    {
                        if (pItem.SerialNumber != null && pItem.SerialNumber.Equals("<enter serial number>", StringComparison.CurrentCultureIgnoreCase))
                        {
                            pItem.SerialNumber = "";
                            WarningMessage.Message = "Serial number required for part code: " + pItem.OwnerSuppliedPartsNumber;
                            WarningMessage.ErrorType = Validation.MESSAGETYPE.WARNING.ToString();
                            MessageList.Add(WarningMessage);
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

        public bool CallSaveMethod(WorkOrderDetail WorkOrder, out List<ErrMessage> ErrorMessageList)
        {
            bool success = true;
            ErrorMessageList = new List<ErrMessage>();


            try
            {
                if (WorkOrder != null)
                {
                    Save(WorkOrder, out ErrorMessageList);
                }
                if (ErrorMessageList.Count > 0)
                {
                    success = false;
                }
                else
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error while attempting to save the work order.";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                ErrorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }
            return success;
        }

        private string GetCategory(string TpiCode)
        {
            string category = string.Empty;

            var rpView = (from tpi in objContext.MESC1TS_TPI
                          where tpi.cedex_code == TpiCode
                          select new
                          {
                              tpi.category
                          }).FirstOrDefault();


            if (rpView != null)
            {
                category = rpView.category;
            }
            return category;
        }

        // weak numeric check...
        private bool IsNumericString(string s)
        {
            char c;
            if (s.Length == 0) return (false);

            for (int i = 0; i < s.Length; i++)
            {
                c = s[i];
                if ((c != '-') && (c != '+') && (!char.IsDigit(c)) && (c != '.')) return (false);
            }
            return (true);
        }

        private string GetErrorMessage(int ErrorCode)
        {
            string errMsg = string.Empty;
            try
            {
                var msg = (from err in objContext.MESC1TS_ERR_MESSAGE
                           where err.ERROR_NO == ErrorCode
                           select new
                           {
                               err.ERROR_MSG
                           }).FirstOrDefault();

                if (string.IsNullOrEmpty(msg.ERROR_MSG))
                {
                    errMsg = "Error code received: %d, error description not available or not found in database" + ErrorCode;
                }
                else
                {
                    errMsg = msg.ERROR_MSG;
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error while getting error messages";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return (errMsg);

            /* use below as reference to error codes
	            switch( nCode )
	            {
		            case 0	   : return(""); break;	
		            case 20010 : return("SHOP CODE IS INVALID OR INACTIVE."); break;
		            case 20015 : return("WO ALREADY EXISTS WITH REFERENCE NUMBER"); break;
		            case 20020 : return("WO REPAIR ALREADY EXISTS IN DATABASE"); break;
		            case 20025 : return("CUSTOMER CODE IS INVALID OR INACTIVE"); break;
		            case 20030 : return("EQUIPMENT NUMBER NOT FOUND IN RKEM."); break;
		            case 20035 : return("Warning : CONTAINER NUMBER IS INVALID."); break;
		            case 20036 : return("MANUAL / MODE IS NOT VALID OR INACTIVE."); break;
		            case 20040 : return("INVALID MODE FOR EQUIPMENT NUMBER."); break;
		            case 20046 : return("INVALID REPAIR CODE FOR REFU MANUFACTURER."); break;
		            case 20050 : return("WO REPAIR DATE CANNOT EXCEED DATE OF WO RECEIPT."); break;
		            case 20055 : return("LABOR RATES DO NOT EXIST IN SYSTEM FOR REPAIR DATE."); break;
		            case 20060 : return("SHOP CURRENCY CODE NOT FOUND FOR EXCHANGE RATE ."); break;
		            case 20065 : return("REPAIR CODE IS INVALID OR INACTIVE."); break;
		            case 20070 : return("QUANTITY OF REPAIR CODE EXCEEDS MAX QUANTITY."); break;
		            case 20075 : return("MAN HOURS on REPAIR CODE EXCEEDS MAX  ."); break;
		            case 20080 : return("EXCLUSIONARY REPAIR CODE EXISTS ON WO."); break;
		            case 20085 : return("RKRP PART NUMBER DOES NOT EXIST."); break;
		            case 20090 : return("TOTAL WO AMOUNT INVOICED DOES NOT MATCH SYSTEM CALCULATIONS."); break;
		            case 20100 : return("MAN HOUR CALCULATIONS DO NOT TOTAL CORRECTLY."); break;

		            default	   : tmp.Format("Error code: %d", nCode); return( tmp ); break;
	            }
            */
        }

        #region TAX_CODE_HANDLING
        private bool IsRepairTaxCode(RepairCode RepairCode)
        {
            if (RepairCode.RepairCod.Equals(IMPORTTAXCODE, StringComparison.CurrentCultureIgnoreCase)
                || RepairCode.RepairCod.Equals(SALESTAXPARTCODE, StringComparison.CurrentCultureIgnoreCase)
                    || RepairCode.RepairCod.Equals(SALESTAXLABORCODE, StringComparison.CurrentCultureIgnoreCase))
                return true;
            else return false;
        }
        #endregion TAX_CODE_HANDLING

        #region Exposed Methods
        /// <summary>
        /// The Validate method that is exposed to the client
        /// </summary>
        /// <param name="WorkOrderDetail"></param>
        /// <param name="EquipmentList"></param>
        /// <returns></returns>
        public List<ErrMessage> Review(ref WorkOrderDetail WorkOrderDetail, List<Equipment> EquipmentList, bool ClientCall = true)
        {
            //MercPlusLibrary.obj
            bool success = true;
            List<ErrMessage> ErrorMessageList = new List<ErrMessage>();
            ErrMessage errmsg = new ErrMessage();
            List<ErrMessage> MercPlusMessageList = new List<ErrMessage>();
            WorkOrderDetail DeepCopyWOD = new WorkOrderDetail();
            try
            {
                if (ClientCall && !string.IsNullOrEmpty(WorkOrderDetail.ThirdPartyPort))
                {
                    success = CheckThirdPartyPort(WorkOrderDetail.ThirdPartyPort, out ErrorMessageList);
                    MercPlusMessageList.AddRange(ErrorMessageList);
                }

                DeepCopyWOD = ObjectCopy<WorkOrderDetail>.DeepCopyEntireObject(WorkOrderDetail);

                foreach (var eqp in EquipmentList)
                {
                    success = true;
                    WorkOrderDetail = null;
                    WorkOrderDetail = ObjectCopy<WorkOrderDetail>.DeepCopyEntireObject(DeepCopyWOD);
                    //Check for duplicate Vendor ref no
                    if (!string.IsNullOrEmpty(eqp.VendorRefNo))
                    {
                        //if()
                        //{
                        //    Message.Message = "Vendor reference number not unique: " + WorkOrderDetail.EquipmentList[0].VendorRefNo;
                        //    ErrorMessageList.Add(ErrorMessage);
                        //    success = false;
                        //}
                    }

                    if (success)
                    {
                        try
                        {
                            //CheckNumericHours(WorkOrderDetail, out ErrorMessageList);

                            // if no hour numeric errors, continue with normal validation
                            //if (ErrorMessageList.Count == 0 || (ErrorMessageList.Count > 0 && ErrorMessageList.Any(cd => cd.ErrorType == Validation.MESSAGETYPE.SUCCESS.ToString())))
                            {
                                success = CallValidateMethod(ref WorkOrderDetail, eqp, out ErrorMessageList);
                                MercPlusMessageList.AddRange(ErrorMessageList);
                                if (success)
                                {
                                    // Check for warnings on Parts - core deductible with no serial numbers entered
                                    CheckForWarnings(WorkOrderDetail, out ErrorMessageList);
                                    MercPlusMessageList.AddRange(ErrorMessageList);
                                    if (success)
                                    {
                                        Message = new ErrMessage();
                                        ErrorMessageList = new List<ErrMessage>();
                                        Message.Message = "Review completed successfully for EQP NO.: " + eqp.EquipmentNo;
                                        Message.ErrorType = Validation.MESSAGETYPE.SUCCESS.ToString();
                                        ErrorMessageList.Add(Message);
                                        MercPlusMessageList.AddRange(ErrorMessageList);
                                    }
                                }
                                else if (ErrorMessageList.Any(msg => msg.ErrorType == Validation.MESSAGETYPE.INFO.ToString() && msg.Message == INFO_MSG))
                                {
                                    WorkOrderDetail = LoadWorkOrderDetails(WorkOrderDetail.WorkOrderID);
                                    Message = new ErrMessage();
                                    ErrorMessageList = new List<ErrMessage>();
                                    MercPlusMessageList = new List<ErrMessage>();
                                    Message.Message = "Review completed successfully for EQP NO.: " + eqp.EquipmentNo;
                                    Message.ErrorType = Validation.MESSAGETYPE.SUCCESS.ToString();
                                    ErrorMessageList.Add(Message);
                                    MercPlusMessageList.AddRange(ErrorMessageList);
                                }
                            }
                        }

                        catch (Exception ex)
                        {
                            Message = new ErrMessage();
                            Message.Message = "System error while review";
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            logEntry.Message = ex.ToString();
                            Logger.Write(logEntry);
                        }
                    }
                }
                MercPlusMessageList = GetMessageList(MercPlusMessageList);
                WorkOrderDetail.ExchangeRate = WorkOrderDetail.ExchangeRate * 100;
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return MercPlusMessageList;
        }

        private bool CheckEquipmentList(WorkOrderDetail WorkOrderDetail, Equipment eqp, out List<ErrMessage> ErrorMessageList)
        {
            // force special repair code test for multiple work order entry;
            ErrorMessageList = new List<ErrMessage>();
            bool hasError = false;

            try
            {
                //foreach (var eqp in EquipmentList)
                //{
                WorkOrderDetail.EquipmentList = new List<Equipment>();
                Equipment eqRKEM = new Equipment();
                eqRKEM = GetEquipmentDetailsFromRKEM(eqp.EquipmentNo, WorkOrderDetail.Shop.ShopCode, ""); //WorkOrderDetail.EquipmentList[0].VendorRefNo);

                if (!string.IsNullOrEmpty(eqRKEM.EqpNotFound))
                {
                    //Error throw;
                    WorkOrderDetail.EquipmentList.Add(eqRKEM);
                    Message = new ErrMessage();
                    Message.Message = "Container type not received from RKEM or RKEM call not performed for EQP NO.: " + eqp.EquipmentNo;
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrorMessageList.Add(Message);
                    hasError = true;
                }
                else
                {
                    eqRKEM.SelectedMode = WorkOrderDetail.Mode;

                    if (eqRKEM.ModeList == null || !eqRKEM.ModeList.Any(modeCode => modeCode.ModeCode == eqRKEM.SelectedMode))
                    {
                        //Error throw;
                        Message = new ErrMessage();
                        Message.Message = eqRKEM.SelectedMode + " is not valid for " + eqp.EquipmentNo;
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        // eqpModeMismatch = true;
                        hasError = true;
                    }
                    else
                        WorkOrderDetail.EquipmentList.Add(eqRKEM);
                }
                //}
            }
            catch (Exception ex)
            {
                hasError = true;
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return hasError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrderDetail"></param>
        /// <param name="EquipmentList"></param>
        /// <returns></returns>
        public List<ErrMessage> SaveAsDraft(ref WorkOrderDetail WorkOrderDetail, List<Equipment> EquipmentList)
        {
            int temp = 0;
            bool success = true;
            List<object> WorkOrderObject = new List<object>();
            List<ErrMessage> ErrorMessageList = new List<ErrMessage>();
            List<ErrMessage> MercPlusMessageList = new List<ErrMessage>();
            WorkOrderDetail DeepCopyWOD = new WorkOrderDetail();

            try
            {
                // iterate through wo list - validate each WO - call stateless WO manager component
                DeepCopyWOD = ObjectCopy<WorkOrderDetail>.DeepCopyEntireObject(WorkOrderDetail);

                foreach (var eqp in EquipmentList)
                {
                    success = true;
                    WorkOrderDetail = null;
                    WorkOrderDetail = ObjectCopy<WorkOrderDetail>.DeepCopyEntireObject(DeepCopyWOD);

                    WorkOrderDetail.EquipmentList = new List<Equipment>();
                    WorkOrderDetail.EquipmentList.Add(eqp);

                    // if no status code or status code = 000, then can save as draft.
                    // if empty status code, set to -1 
                    if (!string.IsNullOrEmpty(WorkOrderDetail.ThirdPartyPort))
                    {
                        success = CheckThirdPartyPort(WorkOrderDetail.ThirdPartyPort, out ErrorMessageList);
                        MercPlusMessageList.AddRange(ErrorMessageList);
                    }
                    if (success)
                    {
                        success = CallValidateMethod(ref WorkOrderDetail, eqp, out ErrorMessageList);
                        MercPlusMessageList.AddRange(ErrorMessageList);
                    }
                    if (success && WorkOrderDetail.WorkOrderStatus != null)
                    {
                        temp = (int)WorkOrderDetail.WorkOrderStatus;
                    }

                    //if (tmp.length() == 0)
                    //    pRecord->SetSTATUS_CODE("0");

                    if (success)
                    {
                        if (temp < 400)
                        {
                            WorkOrderDetail.WorkOrderStatus = 0;
                            // Check for warnings on Parts - core deductible with no serial numbers entered
                            success = CallSaveMethod(WorkOrderDetail, out ErrorMessageList);
                            MercPlusMessageList.AddRange(ErrorMessageList);

                            if (success)
                            {
                                // Check for warnings on Parts - core deductible with no serial numbers entered
                                CheckForWarnings(WorkOrderDetail, out ErrorMessageList);
                                if (success)
                                {
                                    Message = new ErrMessage();
                                    Message.Message = "Estimate Received: " + WorkOrderDetail.WorkOrderID + " for EQP NO.: " + eqp.EquipmentNo;
                                    Message.ErrorType = Validation.MESSAGETYPE.SUCCESS.ToString();
                                    ErrorMessageList.Add(Message);
                                    MercPlusMessageList.AddRange(ErrorMessageList);
                                }
                            }
                        }
                        else
                            success = true;
                    }

                }
                MercPlusMessageList = GetMessageList(MercPlusMessageList);
                WorkOrderDetail.ExchangeRate = WorkOrderDetail.ExchangeRate * 100;
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return MercPlusMessageList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrderDetailsList"></param>
        /// <returns></returns>
        public List<ErrMessage> SubmitWorkOrder(ref WorkOrderDetail WorkOrderDetail, List<Equipment> EquipmentList)
        {
            bool success = true;
            List<object> WorkOrderObject = new List<object>();
            List<ErrMessage> ErrorMessageList = new List<ErrMessage>();
            List<ErrMessage> MercPlusMessageList = new List<ErrMessage>();
            WorkOrderDetail DeepCopyWOD = new WorkOrderDetail();

            try
            {
                DeepCopyWOD = ObjectCopy<WorkOrderDetail>.DeepCopyEntireObject(WorkOrderDetail);

                // iterate through wo list - validate each WO - call stateless WO manager component
                foreach (var eqp in EquipmentList)
                {
                    success = true;
                    WorkOrderDetail = null;
                    WorkOrderDetail = ObjectCopy<WorkOrderDetail>.DeepCopyEntireObject(DeepCopyWOD); ;

                    if (success)
                    {
                        if (!string.IsNullOrEmpty(WorkOrderDetail.ThirdPartyPort))
                        {
                            success = CheckThirdPartyPort(WorkOrderDetail.ThirdPartyPort, out ErrorMessageList);
                            MercPlusMessageList.AddRange(ErrorMessageList);
                        }
                        if (success)
                        {
                            success = CallValidateMethod(ref WorkOrderDetail, eqp, out ErrorMessageList);
                            MercPlusMessageList.AddRange(ErrorMessageList);
                        }
                        // Check for warnings on Parts - core deductible with no serial numbers entered
                        if (success)
                        {
                            success = GetPrevData(WorkOrderDetail, out ErrorMessageList);
                            if (success)
                            {
                                success = CallSaveMethod(WorkOrderDetail, out ErrorMessageList);
                            }
                            MercPlusMessageList.AddRange(ErrorMessageList);
                        }

                        if (success)
                        {
                            Message = new ErrMessage();
                            ErrorMessageList = new List<ErrMessage>();
                            Message.Message = "Estimate(s) Received:  " + WorkOrderDetail.WorkOrderID + " for EQP NO.: " + eqp.EquipmentNo;
                            Message.ErrorType = Validation.MESSAGETYPE.SUCCESS.ToString();
                            ErrorMessageList.Add(Message);
                            MercPlusMessageList.AddRange(ErrorMessageList);
                        }
                        //else
                        //{
                        //    MercPlusMessageList.AddRange(ErrorMessageList);
                        //}
                        //if (success == false)
                        //{
                        //    for (int i = 0; i < WorkOrderDetailsList.Count; i++)
                        //    {
                        //        for(int j = 0; 
                        //    }
                        //}
                    }
                }
                MercPlusMessageList = GetMessageList(MercPlusMessageList);
                WorkOrderDetail.ExchangeRate = WorkOrderDetail.ExchangeRate * 100;
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return MercPlusMessageList;
        }

        private bool GetPrevData(WorkOrderDetail WorkOrder, out List<ErrMessage> ErrorMessageList)
        {
            bool success = true;
            string eqpNO = WorkOrder.EquipmentList[0].EquipmentNo;
            string PrevLocCode = string.Empty;
            string StatusDesc = string.Empty;
            string PrevStatusDesc = string.Empty;
            string PrevWOID = string.Empty;
            ErrorMessageList = new List<ErrMessage>();
            try
            {
                if (WorkOrder.WorkOrderID == 0)
                {
                    var tempWO1 = (from W in objContext.MESC1TS_WO
                                   from S in objContext.MESC1TS_SHOP
                                   where W.EQPNO == eqpNO &&
                                   W.SHOP_CD == S.SHOP_CD
                                   select new
                                   {
                                       W.WO_ID,
                                       W.STATUS_CODE,
                                       W.CHTS,
                                       W.CRTS,
                                       S.LOC_CD
                                   }).OrderByDescending(crts => crts.CRTS).FirstOrDefault();

                    if (tempWO1 != null)
                    {
                        WorkOrder.PrevWorkOrderID = tempWO1.WO_ID;
                    }
                }
                else
                {
                    if (WorkOrder.PrevWorkOrderID == null || WorkOrder.PrevWorkOrderID == 0)
                    {
                        //sSQL += " select top 1 w2.wo_id from mesc1ts_wo w1 left outer join mesc1ts_wo w2 ";
                        //sSQL += " on w1.eqpno=w2.eqpno where w1.wo_id = '";
                        //sSQL += sWOID.c_str();
                        //sSQL += "' and w1.crts>w2.crts order by w2.crts desc";

                        //(from RC in objContext.MESC1TS_REPAIR_CODE
                        //                       join RE in objContext.MESC1TS_RPRCODE_EXCLU on new { RC1 = RC.MANUAL_CD, RC2 = RC.MODE, RC3 = RC.REPAIR_CD.Trim() }
                        //                       equals new { RC1 = RE.MANUAL_CD, RC2 = RE.MODE, RC3 = RE.REPAIR_CD.Trim() } into Inner
                        //                       from O in Inner.DefaultIfEmpty()
                        //                       where RC.REPAIR_ACTIVE_SW == "Y" &&
                        var TempWOID = (from w1 in objContext.MESC1TS_WO
                                        join w2 in objContext.MESC1TS_WO on new { w11 = w1.EQPNO }
                                        equals new { w11 = w2.EQPNO } into InnerTable
                                        from O in InnerTable.DefaultIfEmpty()
                                        where w1.WO_ID == WorkOrder.WorkOrderID &&
                                        w1.CRTS > O.CRTS
                                        select new
                                        {
                                            O.WO_ID,
                                            O.CRTS
                                        }).OrderByDescending(crts => crts.CRTS).FirstOrDefault();

                        //TempPrevWOID = TempWOID.WO_ID;
                        if (TempWOID != null)
                        {
                            WorkOrder.PrevWorkOrderID = TempWOID.WO_ID;

                            var tempWO2 = (from W in objContext.MESC1TS_WO
                                           from S in objContext.MESC1TS_SHOP
                                           where W.WO_ID == WorkOrder.WorkOrderID &&
                                           W.SHOP_CD == S.SHOP_CD
                                           select new
                                           {
                                               W.STATUS_CODE,
                                               W.CHTS,
                                               S.LOC_CD
                                           }).FirstOrDefault();

                            if (tempWO2 != null)
                            {
                                if (WorkOrder.WorkOrderStatus >= 390)
                                {
                                    WorkOrder.PrevStatus = tempWO2.STATUS_CODE;
                                    WorkOrder.PrevDate = tempWO2.CHTS;
                                    WorkOrder.PrevLocCode = tempWO2.LOC_CD;
                                }
                                else
                                {
                                    WorkOrder.PrevStatus = null;
                                    WorkOrder.PrevDate = DateTime.MinValue;
                                    WorkOrder.PrevLocCode = string.Empty;
                                }
                            }
                            else
                            {
                                WorkOrder.PrevStatus = null;
                                WorkOrder.PrevDate = DateTime.MinValue;
                                WorkOrder.PrevLocCode = string.Empty;
                            }
                        }

                    }
                }
            }

            catch (Exception ex)
            {
                Message.Message = "System Error on retrieving previous work order id ";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                ErrorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WOID"></param>
        /// <param name="WOStatus"></param>
        /// <param name="ChangeUser"></param>
        /// <returns></returns>
        public List<ErrMessage> ChangeStatus(int WOID, short? WOStatus, string ChangeUser) //Same as ChangeStatus()
        {
            bool success = true;
            List<object> WorkOrderObject = new List<object>();
            List<ErrMessage> ErrorMessageList = new List<ErrMessage>();
            //ErrorMessageList.Add(new ErrMessage
            //{
            //    Message = "Completed successfully",
            //    ErrorType = Validation.MESSAGETYPE.SUCCESS.ToString().ToString()

            //});
            //ErrorMessageList.Add(new ErrMessage
            //{
            //    Message = "Warning received",
            //    ErrorType = Validation.MESSAGETYPE.WARNING.ToString().ToString()

            //});
            //ErrorMessageList.Add(new ErrMessage
            //{
            //    Message = "There are errors",
            //    ErrorType = Validation.MESSAGETYPE.ERROR.ToString().ToString()

            //});
            try
            {
                success = CallChangeStatusMethod(WOID, WOStatus, ChangeUser, out ErrorMessageList);


                if (success && ErrorMessageList.Count == 0)
                {
                    Message = new ErrMessage();
                    Message.Message = "Status Changed successfully";
                    Message.ErrorType = Validation.MESSAGETYPE.SUCCESS.ToString();
                    ErrorMessageList.Add(Message);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ErrorMessageList;
        }

        #endregion Exposed Methods

        public bool CallChangeStatusMethod(int WOID, short? WOStatus, string ChangeUser, out List<ErrMessage> ErrorMessageList)
        {
            bool success = true;
            ErrorMessageList = new List<ErrMessage>();
            //ErrMessage Message = new ErrMessage();

            try
            {
                Message = new ErrMessage();
                Message.Message = ChangeStatusOfWorkOrder(WOID, WOStatus, ChangeUser);

                if (!string.IsNullOrEmpty(Message.Message))
                {
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrorMessageList.Add(Message);
                    success = false;
                }
                else
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error while attempting to save the work order.";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                ErrorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }
            return success;
        }

        #region Validate

        /// <summary>
        /// The main private Validate method that is used to validate a WO.
        /// Called while Review, Save AsDraft, Submit and Update
        /// </summary>
        /// <param name="WorkOrderDetail"></param>
        /// <param name="ErrorMessageList"></param>
        /// <returns></returns>
        public bool CallValidateMethod(ref WorkOrderDetail WorkOrderDetail, Equipment Equipment, out List<ErrMessage> ErrorMessageList)
        {
            logEntry.Message = "Method Name : CallValidateMethod(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            bool success = true;
            ErrorMessageList = new List<ErrMessage>();
            WorkOrderDetail g = null;
            bool isChanged = true, isDeleted = false;
            List<SparePartsView> PartsList = null;

            try
            {
                if (WorkOrderDetail.WorkOrderID == 0 && CheckEquipmentList(WorkOrderDetail, Equipment, out ErrorMessageList))
                {
                    return false;
                }
                if (WorkOrderDetail.WorkOrderID != 0)
                {
                    string selectedMode = WorkOrderDetail.EquipmentList[0].SelectedMode;
                    WorkOrderDetail.EquipmentList = GetEquipmentDetailsOnWOID(WorkOrderDetail.WorkOrderID, selectedMode);
                }

                //foreach (var  sItem in WorkOrderDetail.SparePartsViewList)
                //{
                //    if (WorkOrderDetail.SparePartsViewList != null && WorkOrderDetail.SparePartsViewList.Count > 0)
                //    {
                //        PartsList = WorkOrderDetail.RepairsViewList.FindAll(rCd => rCd.RepairCode.RepairCod.Trim() == sItem.RepairCode.RepairCod.Trim());
                //        //PartsList = 
                //    }
                //}
                if (WorkOrderDetail.SparePartsViewList != null && WorkOrderDetail.SparePartsViewList.Count > 0)
                {
                    PartsList = new List<SparePartsView>();
                    List<RepairsView> TempRV = WorkOrderDetail.RepairsViewList;
                    PartsList = WorkOrderDetail.SparePartsViewList.FindAll(rCd => TempRV.Any(Cd => rCd.RepairCode.RepairCod.Trim() == Cd.RepairCode.RepairCod.Trim()));
                    WorkOrderDetail.SparePartsViewList = PartsList;
                }


                if (WorkOrderDetail.WorkOrderID != 0)
                {
                    OriginalWorkOderFromDB = new WorkOrderDetail();
                    OriginalWorkOderFromDB = LoadWorkOrderDetails(WorkOrderDetail.WorkOrderID);
                    isChanged = CheckForChangesForEdit(WorkOrderDetail, OriginalWorkOderFromDB);
                    isDeleted = SetDeletedState(WorkOrderDetail, OriginalWorkOderFromDB);
                }
                // don't save a disgarded record
                if (WorkOrderDetail.woState == (int)Validation.STATE.DISCARDED) return false;


                if (isChanged || isDeleted)
                {
                    //string eqpNo = WorkOrderDetail.EquipmentList[0].EquipmentNo;
                    ////Check if an estimate is already created that is not finished.
                    //var ExistingWO = (from wo in objContext.MESC1TS_WO
                    //                  where wo.EQPNO == eqpNo &&
                    //                  wo.MODE == WorkOrderDetail.Mode //&&
                    //                  //wo.CHTS.Date == date
                    //                  select wo).ToList();

                    //if (ExistingWO != null && ExistingWO.Count > 0)
                    //{
                    //    ExistingWO = ExistingWO.FindAll(date => date.CHTS.Date == DateTime.Now.Date);
                    //    if (ExistingWO != null && ExistingWO.Count > 0)
                    //    {
                    //        if (ExistingWO[0].STATUS_CODE != 400)
                    //        {
                    //            Message.Message = "EQPNO: " + WorkOrderDetail.EquipmentList[0].EquipmentNo + "." + GetErrorMessage(20020);
                    //            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    //            ErrorMessageList.Add(Message);
                    //            success = false;
                    //        }
                    //    }
                    //}


                    //if (!RequiresValidation(WorkOrderDetail)) return true;

                    //Validate basic field requirements - trim etc.
                    try
                    {
                        success = ValidateBasics(WorkOrderDetail, out ErrorMessageList);
                        if (ErrorMessageList.Count > 0)
                            success = false;
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while validating basics - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }
                    // Load mercplus system defined messages into hash table.
                    //if (success)
                    //    LoadErrors();
                    try
                    {
                        if (success)
                        {
                            success = GetShopData(ref WorkOrderDetail, false, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while retrieving shop data - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }

                    // Shop exists, get shop limits and special remarks limits - return if system error 
                    try
                    {
                        if (success)
                        {
                            success = GetShopLimitsData(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while retrieving shop limits data - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }
                    // if tax repair codes exist, fill in appropriate values.
                    if (success)
                    {

                        success = ExtractRepairTax(ref WorkOrderDetail, out ErrorMessageList);
                        if (ErrorMessageList.Count > 0)
                            success = false;
                    }

                    // Get customer data. Ensure customer exists  - get manual-cd and maersk-sw as well
                    try
                    {
                        if (success)
                        {
                            success = GetCustomerData(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while validating customer - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }

                    // Force wo type to "S"
                    WorkOrderDetail.WorkOrderType = "S";

                    // This code was from the original mesc system. 
                    // Verify Manual_mode exists for Manual/womode and activesw= 'Y'
                    // HOLD OFF - May no longer be needed - 
                    //		if (success)
                    //			success = CheckManualCode( WorkOrderDetail ); 



                    // NOTE: THIS MUST BE UN-COMMENTED FOR PRODUCTION!!!
                    // TEST TEST TEST  HOLD OFF FOR TESTING ONLY
                    // !FIX - put back on release
                    // Ensure that there are no duplicate WO's already in table

                    // TEST TEST TEST THIS IS COMMENTED OUT FOR TESTING ONLY.  ENABLE FOR PRODUCTION

                    try
                    {
                        if (success)
                        {
                            success = CheckDuplicate(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                            success = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while validating duplicate estimates - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        success = false;
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                    }

                    // ticket 11039 - VJP 2006-09-27 - check dup repair codes for equipment/shop for past 60 days.
                    try
                    {
                        if (success)
                        {
                            success = CheckDuplicateRepairs(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0 && ErrorMessageList.Any(msg => msg.ErrorType == Validation.MESSAGETYPE.ERROR.ToString()))
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while validating duplicate repairs - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }

                    // Check eqmode table for mode match by Alum sw, COTYPE and STYPE
                    try
                    {
                        if (success)
                        {
                            success = CheckEquipmentMode(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while validating equipment-mode - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }

                    // Check EQ type if REFU or GENS read manufacturer table
                    // Set MERC equipment type based on COTYPE etc. needed for additional validations.below.
                    // check indicator code against first char of repair codes for match.

                    try
                    {
                        if (success)
                        {
                            success = CheckManufacturer(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while validating manufacture - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }

                    // Labor rate tables by CustCd and ShopCD and EQType, REpairDate, Overtime
                    // Get country labor rates if available as well.
                    // if no record, then is error

                    try
                    {
                        if (success)
                        {
                            success = CheckLaborRate(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while validating labor rates - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }

                    // Check all repair codes. Read Repair code by manualcd ,repair mode and
                    // repair code where repair active sw = "Y"
                    // possible errors???
                    // Verify that Repair qty !> max allowed Repair qty
                    // Verify that Actual Man hous !> Repair code man hours.

                    try
                    {
                        if (success)
                        {
                            success = CheckExclusiveRepair(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while validating Repair codes  - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }

                    // Parts validation.
                    // read thru WOPart by part number 
                    // if not exist, then is error
                    try
                    {
                        // else get part's cost
                        if (success)
                        {
                            success = CheckParts(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while validating parts - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }


                    // PREP time is one of the repair codes passed to pgm.
                    // needed for WO calculations. - Preptime etc
                    // Get total prep time hours		(m_sTotPrepManHrs);
                    // Get prep time if any. complete calculations on remaining totals.
                    // Get sum of repair man hours		(m_sTotRepairManHrs)
                    // Get total repair material amount (m_sTotShopAmt)
                    try
                    {
                        if (success)
                        {
                            success = CheckPrepTime(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while validating prep-time - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }


                    // Check if part of multiple work orders - if so check that repair codes are alloweed
                    // depends on CheckPrepTime to find and mark Prep records
                    try
                    {
                        if (success)
                        {
                            success = CheckMultiRepairSwitch(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while validating multi-repair switch - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }

                    // Enhancement: (VJP 07-06-2004) Check maximum parts allowed by part,repaircode, mode, manualcode.
                    try
                    {
                        if (success)
                        {
                            success = CheckMaxPart(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while validating max Repair qty - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }


                    // Perform all calculations, including tax etc.
                    try
                    {
                        if (success)
                        {
                            PerformCalculations(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while performing system calculations - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }

                    // Perform equipment age check limit - TOT_COST_REPAIR_CPH (depends on PerformCalculations
                    try
                    {
                        if (success)
                        {
                            success = CheckEquipmemtAge(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0)
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while performing equipment age check - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }

                    // set wo status based on business rules -  some status codes
                    // will be set from the UI via special updateStatus call. 

                    try
                    {
                        if (success)
                        {
                            SetWorkOrderStatus(ref WorkOrderDetail, out ErrorMessageList);
                            if (ErrorMessageList.Count > 0 && ErrorMessageList.Any(msg => msg.ErrorType == Validation.MESSAGETYPE.ERROR.ToString()))
                                success = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error encountered while setting estimate status - unable to continue.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        success = false;
                    }
                }
                else
                {
                    Message = new ErrMessage();
                    Message.Message = "No Changes to Save";
                    Message.ErrorType = Validation.MESSAGETYPE.INFO.ToString();
                    ErrorMessageList.Add(Message);
                    success = false;
                }

            }

            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System Error encountered while validating work order estimate - unable to continue.";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                ErrorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
                //success = false;
            }
            logEntry.Message = "Method Name : CallValidateMethod(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrderDetail"></param>
        /// <returns></returns>
        private bool RequiresValidation(WorkOrderDetail WorkOrderDetail)
        {
            bool updatable = false;
            if (WorkOrderDetail.woState != (int)Validation.STATE.EXISTING) updatable = true;

            foreach (var rItem in WorkOrderDetail.RepairsViewList)
            {
                if (rItem.rState != (int)Validation.STATE.EXISTING) updatable = true;
            }

            foreach (var pItem in WorkOrderDetail.SparePartsViewList)
            {
                if (pItem.pState != (int)Validation.STATE.EXISTING) updatable = true;
            }

            // finaly, if this is a draft work order, we must force a revalidation so that
            // the status code can be reset.
            if (WorkOrderDetail.woState != (int)Validation.STATE.NEW)
            {
                if (WorkOrderDetail.WorkOrderStatus == 0)
                {
                    updatable = true;
                }
            }

            return updatable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="exchangeOnly"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        public bool GetShopData(ref WorkOrderDetail WorkOrder, bool exchangeOnly, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : GetShopData(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            //ExchangeOnly bool value is set to true while saving and set to false while reviewing
            List<MESC1TS_SHOP> shopListFromDB = new List<MESC1TS_SHOP>();
            errorMessageList = new List<ErrMessage>();
            string shopCode = WorkOrder.Shop.ShopCode;
            string a = string.Empty;
            bool success = true;


            if (!exchangeOnly)
            {
                try
                {
                    shopListFromDB = (from shop in objContext.MESC1TS_SHOP
                                      where shop.SHOP_CD == shopCode
                                      select shop).ToList();

                    if (shopListFromDB.Count > 0)
                    {
                        foreach (var item in shopListFromDB)
                        {
                            Shop shopItem = new Shop();
                            //WorkOrder.Shop = new Shop();
                            WorkOrder.Shop.CUCDN = item.CUCDN;
                            WorkOrder.CountryCUCDN = item.CUCDN;
                            WorkOrder.Shop.PreptimeSW = item.PREPTIME_SW;
                            WorkOrder.Shop.LocationCode = item.LOC_CD;
                            WorkOrder.Shop.OvertimeSuspSW = item.OVERTIME_SUSP_SW;
                            WorkOrder.Shop.VendorCode = item.VENDOR_CD;
                            WorkOrder.Shop.PCTMaterialFactor = item.PCT_MATERIAL_FACTOR;
                            if (WorkOrder.Shop.PCTMaterialFactor == 0.0)
                            {
                                WorkOrder.Shop.PCTMaterialFactor = 100;
                            }
                            WorkOrder.Shop.PCTMaterialFactor = WorkOrder.Shop.PCTMaterialFactor * 0.01;
                            if (item.IMPORT_TAX == null)
                            {
                                WorkOrder.Shop.ImportTax = (0.0);
                            }
                            else
                            {
                                WorkOrder.Shop.ImportTax = (float)item.IMPORT_TAX;
                            }
                            WorkOrder.Shop.ImportTax *= 0.01;
                            if (item.SALES_TAX_PART_GEN == null)
                            {
                                WorkOrder.Shop.SalesTaxPartGen = (0.0);
                            }
                            else
                            {
                                WorkOrder.Shop.SalesTaxPartGen = item.SALES_TAX_PART_GEN;
                            }
                            WorkOrder.Shop.SalesTaxPartGen *= 0.01;
                            if (item.SALES_TAX_PART_CONT == null)
                            {
                                WorkOrder.Shop.SalesTaxPartCont = (0.0);
                            }
                            else
                            {
                                WorkOrder.Shop.SalesTaxPartCont = item.SALES_TAX_PART_CONT;
                            }
                            WorkOrder.Shop.SalesTaxPartCont *= 0.01;
                            if (item.SALES_TAX_LABOR_GEN == null)
                            {
                                WorkOrder.Shop.SalesTaxLaborGen = (0.0);
                            }
                            else
                            {
                                WorkOrder.Shop.SalesTaxLaborGen = item.SALES_TAX_LABOR_GEN;
                                WorkOrder.Shop.SalesTaxLaborGen *= 0.01;
                            }
                            if (item.SALES_TAX_LABOR_CON == null)
                            {
                                WorkOrder.Shop.SalesTaxLaborCon = (0.0);
                            }
                            else
                            {
                                WorkOrder.Shop.SalesTaxLaborCon = item.SALES_TAX_LABOR_CON;
                            }
                            WorkOrder.Shop.SalesTaxLaborCon *= 0.01;
                            WorkOrder.Shop.ShopTypeCode = item.SHOP_TYPE_CD;
                            WorkOrder.Shop.AutoCompleteSW = item.AUTO_COMPLETE_SW;
                            if (item.BYPASS_LEASE_RULES == "Y")
                            {
                                WorkOrder.Shop.BypassLeaseRules = "Y";
                                m_bBypassLease = true;
                            }
                            else
                            {
                                WorkOrder.Shop.BypassLeaseRules = "N";
                                m_bBypassLease = false;
                            }
                        }
                    }
                    else
                    {
                        Message = new ErrMessage();
                        Message.Message = GetErrorMessage(20010);
                        errorMessageList.Add(Message);
                    }
                }
                catch (Exception ex)
                {
                    Message = new ErrMessage();
                    Message.Message = "System Error on validating Shopcode";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    success = false;
                }
                try
                {
                    string CUCDN = WorkOrder.Shop.CUCDN;
                    //Get exchange rate, Note, exchange rate is in percent format, so multiply by .01
                    //         "SELECT CURRNAMC, EXRATUSD, QUOTEDAT FROM MESC1TS_CURRENCY WHERE CUCDN = NULL";
                    //else
                    //{
                    //    sSQL = "SELECT CURRNAMC, EXRATUSD, QUOTEDAT FROM MESC1TS_CURRENCY WHERE CUCDN = '";
                    //    sSQL+= tCurrencyCode; sSQL+= "'";
                    var exchangeCurrency = (from curr in objContext.MESC1TS_CURRENCY
                                            where CUCDN == null ? curr.CUCDN == null : curr.CUCDN == CUCDN
                                            select new
                                            {
                                                curr.CURRNAMC,
                                                curr.EXRATUSD,
                                                curr.QUOTEDAT,
                                                curr.EXRATDKK
                                            });

                    if (exchangeCurrency != null)
                    {
                        foreach (var item in exchangeCurrency)
                        {
                            WorkOrder.Shop.Currency = new Currency();
                            WorkOrder.Shop.Currency.CurrName = item.CURRNAMC;
                            if (item.EXRATUSD != null)
                            {
                                WorkOrder.Shop.Currency.ExtratUsd = item.EXRATUSD;
                            }
                            else
                            {
                                WorkOrder.Shop.Currency.ExtratUsd = 100;
                            }
                            if (item.QUOTEDAT != null)
                            {
                                WorkOrder.Shop.Currency.QuoteDat = item.QUOTEDAT;
                                WorkOrder.CountryExchangeDate = item.QUOTEDAT;
                            }
                            if (WorkOrder.Shop.Currency.ExtratUsd != null)
                            {
                                WorkOrder.ExchangeRate = Math.Round(WorkOrder.Shop.Currency.ExtratUsd.Value, 6);
                            }
                            else
                            {
                                WorkOrder.ExchangeRate = 100;
                            }
                            // FACT changes - hold shop exchange as country exchange - might be replaced if country contracts exist
                            WorkOrder.CountryExchangeRate = Math.Round(WorkOrder.Shop.Currency.ExtratUsd.Value, 6);
                            WorkOrder.ExchangeRate *= (decimal)0.01;


                        }
                    }
                    else
                    {
                        Message = new ErrMessage();
                        Message.Message = GetErrorMessage(20010);
                        errorMessageList.Add(Message);
                    }
                }
                catch (Exception ex)
                {
                    Message = new ErrMessage();
                    Message.Message = "System Error on retrieving exchange rate";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    success = false;
                    throw;
                }
            }
            logEntry.Message = "Method Name : GetShopData(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        public bool GetShopLimitsData(ref WorkOrderDetail WorkOrder, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : GetShopLimitsData(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            bool success = true;
            errorMessageList = new List<ErrMessage>();
            ErrMessage Message = new ErrMessage();
            string ShopCode = WorkOrder.Shop.ShopCode;
            string Mode = WorkOrder.Mode;

            try
            {
                var RSLimitsByShopAndMode = (from shop in objContext.MESC1TS_SHOP_LIMITS
                                             where shop.SHOP_CD == ShopCode &&
                                             shop.MODE == Mode
                                             select new
                                             {
                                                 shop.AUTO_APPROVE_LIMIT,
                                                 shop.SHOP_MATERIAL_LIMIT,
                                                 shop.REPAIR_AMT_LIMIT
                                             }).ToList();

                if (RSLimitsByShopAndMode.Count > 0)
                {
                    foreach (var item in RSLimitsByShopAndMode)
                    {
                        if (item.AUTO_APPROVE_LIMIT != null)
                        {
                            m_fAutoAppovalLimit = item.AUTO_APPROVE_LIMIT;
                        }
                        else
                        {
                            m_fAutoAppovalLimit = 0;
                        }

                        if (item.SHOP_MATERIAL_LIMIT != null)
                        {
                            m_fShopMaterialLimit = item.SHOP_MATERIAL_LIMIT;
                        }
                        else
                        {
                            m_fShopMaterialLimit = 0;
                        }

                        if (item.REPAIR_AMT_LIMIT != null)
                        {
                            m_fTotRepairAmtLimit = (double)item.REPAIR_AMT_LIMIT;
                        }
                        else
                        {
                            //m_fRepairAmtLimit = 0.0;
                            m_fTotRepairAmtLimit = 0.0;
                        }
                    }
                }
                else
                {
                    m_fAutoAppovalLimit = 0;
                    m_fShopMaterialLimit = 0;
                    m_fTotRepairAmtLimit = 0.0;
                    Message = new ErrMessage();
                    //success = false;
                    Message.Message = "Shop Limits not created for this mode";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }

            }

            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System Error on validating Shop Limits";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }

            try
            {
                List<MESC1TS_SPECIAL_REMARKS> SpecialRemarksList = new List<MESC1TS_SPECIAL_REMARKS>();



                if (WorkOrder.EquipmentList != null && WorkOrder.EquipmentList.Count > 0)
                {
                    string eqpNo = WorkOrder.EquipmentList[0].EquipmentNo;
                    string eqpProfile = WorkOrder.EquipmentList[0].EQProfile;

                    SpecialRemarksList = (from remarks in objContext.MESC1TS_SPECIAL_REMARKS
                                          where remarks.SERIAL_NO_FROM == eqpNo //WorkOrder.EquipmentList[0].EquipmentNo
                                          select remarks).ToList();

                    if (SpecialRemarksList.Count == 0)
                    {
                        SpecialRemarksList = (from remarks in objContext.MESC1TS_SPECIAL_REMARKS
                                              where remarks.RKEM_PROFILE == eqpProfile //WorkOrder.EquipmentList[0].EQProfile
                                              select remarks).ToList();
                    }

                    if (SpecialRemarksList.Count > 0)
                    {
                        bool bRemarkFound = false;
                        bool bDisplayFound = false;
                        m_bSpecialRemarkFound = false;

                        foreach (var item in SpecialRemarksList)
                        {
                            if (!bRemarkFound)
                            {
                                try
                                {
                                    m_fRemarksRepairCeiling = item.REPAIR_CEILING;
                                    bRemarkFound = true;
                                }
                                catch
                                {
                                    // value is empty or null - ADO bug - cannot use VT_ERROR,
                                    m_fRemarksRepairCeiling = -1;
                                    bRemarkFound = true;
                                }
                            }
                            SpecialRemarks += "<br>";
                            SpecialRemarks += item.REMARKS;

                            if (!bDisplayFound)
                            {
                                m_sDisplaySw = item.DISPLAY_SW;
                                bDisplayFound = true;
                            }
                            m_bSpecialRemarkFound = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System Error on validating Shopcode";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }
            logEntry.Message = "Method Name : GetShopLimitsData(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool GetCustomerData(ref WorkOrderDetail WorkOrder, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : GetCustomerData(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            bool success = true;
            errorMessageList = new List<ErrMessage>();
            string ShopCode = WorkOrder.Shop.ShopCode;
            string CustomerCode = WorkOrder.Shop.Customer[0].CustomerCode;
            string a = string.Empty;


            try
            {
                var LoadByShopAndCustomer = (from cs in objContext.MESC1VS_CUST_SHOP
                                             from c in objContext.MESC1TS_CUSTOMER
                                             where c.CUSTOMER_CD == cs.CUSTOMER_CD &&
                                             c.CUSTOMER_ACTIVE_SW == "Y" &&
                                             cs.SHOP_CD == ShopCode &&
                                             (CustomerCode != null ? c.CUSTOMER_CD == CustomerCode : a == a)
                                             select new
                                             {
                                                 cs.SHOP_CD,
                                                 c.CUSTOMER_CD,
                                                 c.MANUAL_CD,
                                                 c.CUSTOMER_DESC,
                                                 c.MAERSK_SW
                                             }).Distinct().ToList();

                if (LoadByShopAndCustomer.Count > 0)
                {
                    //WorkOrder.Shop.Customer = new Customer();
                    foreach (var item in LoadByShopAndCustomer)
                    {
                        if (WorkOrder.Shop.Customer[0].CustomerCode.Equals(item.CUSTOMER_CD, StringComparison.InvariantCultureIgnoreCase))
                        {
                            WorkOrder.Shop.Customer[0].MaerskSw = item.MAERSK_SW;
                            WorkOrder.Shop.Customer[0].ManualCode = item.MANUAL_CD;
                        }
                        else
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + " " + GetErrorMessage(20025);
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            errorMessageList.Add(Message);
                        }
                    }
                }
                else
                {
                    Message = new ErrMessage();
                    Message.Message = GetErrorMessage(20025);
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "Unknown System Error on validating customer";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }
            logEntry.Message = "Method Name : GetCustomerData(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool ExtractRepairTax(ref WorkOrderDetail WorkOrder, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : ExtractRepairTax(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            bool success = true;
            decimal amountCPH = 0;
            errorMessageList = new List<ErrMessage>();
            ErrMessage Message = new ErrMessage();

            try
            {
                if (WorkOrder.ImportTax == null)
                {
                    WorkOrder.ImportTax = 0;
                }
                if (WorkOrder.SalesTaxLabour == null)
                {
                    WorkOrder.SalesTaxLabour = 0;
                }
                if (WorkOrder.SalesTaxParts == null)
                {
                    WorkOrder.SalesTaxParts = 0;
                }
                if (WorkOrder.ImportTaxCPH == null)
                {
                    WorkOrder.ImportTaxCPH = 0;
                }
                //else
                //{
                //    WorkOrder.ImportTaxCPH = 0;
                //}

                if (WorkOrder.SalesTaxPartsCPH == null)
                {
                    WorkOrder.SalesTaxPartsCPH = 0;
                }

                if (WorkOrder.SalesTaxLabourCPH == null)
                {
                    WorkOrder.SalesTaxLabourCPH = 0;
                }

                WorkOrder.ImportTaxCPH = (WorkOrder.ImportTax * WorkOrder.ExchangeRate);
                WorkOrder.ImportTaxCPH = Math.Round(WorkOrder.ImportTaxCPH.Value, 4);

                WorkOrder.SalesTaxPartsCPH = WorkOrder.SalesTaxParts * WorkOrder.ExchangeRate;
                WorkOrder.SalesTaxPartsCPH = Math.Round(WorkOrder.SalesTaxPartsCPH.Value, 2);

                WorkOrder.SalesTaxLabourCPH = WorkOrder.SalesTaxLabour * WorkOrder.ExchangeRate;
                WorkOrder.SalesTaxLabourCPH = Math.Round(WorkOrder.SalesTaxLabourCPH.Value, 2);

                // force save of record since we are changing values
                if (WorkOrder.woState == (int)Validation.STATE.EXISTING)
                    WorkOrder.woState = (int)Validation.STATE.UPDATED;

                // Iterate through repair collection. extract material amount as tax amount 
                // and fill appropriate tax amount field in work order header.
                foreach (var repairItem in WorkOrder.RepairsViewList)
                {
                    decimal? cph = 0;
                    if (repairItem.rState == (int)Validation.STATE.DELETED) continue;

                    if (IsRepairTaxCode(repairItem.RepairCode))
                    {
                        //m_sMaterialAmt = repairItem.MaterialCostCPH.ToString();
                        if (repairItem.RepairCode.RepairCod.Trim() == IMPORTTAXCODE.Trim())
                        {
                            if (!IsValidNumericSize(repairItem.MaterialCost.ToString(), 8, 4))
                            {
                                success = false;
                                Message = new ErrMessage();
                                Message.Message = "Import tax must be numeric. Format: n(8).n(4)";
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                errorMessageList.Add(Message);
                            }
                            else
                            {
                                WorkOrder.ImportTax = (decimal)repairItem.MaterialCost;
                                cph = repairItem.MaterialCost * WorkOrder.ExchangeRate;
                                cph = Math.Round(amountCPH, 4);
                                WorkOrder.ImportTaxCPH = cph;
                            }
                        }
                        if (repairItem.RepairCode.RepairCod.Trim() == SALESTAXPARTCODE.Trim())
                        {
                            if (!IsValidNumericSize(repairItem.MaterialCost.ToString(), 8, 4))
                            {
                                success = false;
                                Message = new ErrMessage();
                                Message.Message = "Sales tax for parts must be numeric. Format: n(8).n(4)";
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                errorMessageList.Add(Message);
                            }
                            else
                            {
                                WorkOrder.SalesTaxParts = repairItem.MaterialCost;
                                cph = repairItem.MaterialCost * WorkOrder.ExchangeRate;
                                cph = Math.Round(amountCPH, 2);
                                WorkOrder.SalesTaxPartsCPH = cph;
                            }
                        }
                        if (repairItem.RepairCode.RepairCod.Trim() == SALESTAXLABORCODE.Trim())
                        {
                            if (!IsValidNumericSize(repairItem.MaterialCost.ToString(), 8, 4))
                            {
                                success = false;
                                Message = new ErrMessage();
                                Message.Message = "Sales tax for labour must be numeric. Format: n(8).n(4)";
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                errorMessageList.Add(Message);
                            }
                            else
                            {
                                WorkOrder.SalesTaxLabour = (decimal)repairItem.MaterialCost; ;
                                cph = repairItem.MaterialCost * WorkOrder.ExchangeRate;
                                cph = Math.Round(amountCPH, 2);
                                WorkOrder.SalesTaxLabourCPH = cph;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error encounterd while extracting Repair Tax";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }
            logEntry.Message = "Method Name : ExtractRepairTax(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool CheckDuplicate(ref WorkOrderDetail WorkOrder, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : CheckDuplicate(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            bool success = true;
            errorMessageList = new List<ErrMessage>();
            int WOID = WorkOrder.WorkOrderID;

            //List<MESC1TS_WO> WOList = new List<MESC1TS_WO>();
            int WorkOrderCount;
            string ShopCode = WorkOrder.Shop.ShopCode;
            string Mode = WorkOrder.Mode;

            try
            {
                // MercPlus not handling non-sts work orders. Set error message and exit.
                if (WorkOrder.WorkOrderType.Equals("N", StringComparison.CurrentCultureIgnoreCase))
                {
                    Message = new ErrMessage();
                    Message.Message = "Non-STS work orders are not accepted by this system";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    return (false);
                }

                // MercPlus only allowing work orders with equipment number.
                if (WorkOrder.EquipmentList.Count == 0)
                {
                    Message = new ErrMessage();
                    Message.Message = "Equipment number is required.";
                    errorMessageList.Add(Message);
                    return (false);
                }

                for (int count = 0; count < WorkOrder.EquipmentList.Count; count++)
                {
                    string VendorRefNo = WorkOrder.EquipmentList[count].VendorRefNo;
                    if (!string.IsNullOrEmpty(WorkOrder.EquipmentList[count].VendorRefNo))
                    {
                        if (WorkOrder.woState == (int)Validation.STATE.NEW)
                        {
                            WorkOrderCount = (from wo in objContext.MESC1TS_WO
                                              where wo.VENDOR_REF_NO == VendorRefNo &&
                                              wo.STATUS_CODE > 99 &&
                                              wo.WOTYPE == "S" &&
                                              wo.STATUS_CODE < 9997
                                              select wo).Count();
                        }
                        else
                        {
                            WorkOrderCount = (from wo in objContext.MESC1TS_WO
                                              where wo.VENDOR_REF_NO == VendorRefNo &&
                                              wo.STATUS_CODE > 99 &&
                                              wo.WOTYPE == "S" &&
                                              wo.STATUS_CODE < 9997 &&
                                              wo.WO_ID != WOID
                                              select wo).Count();
                        }

                        if (WorkOrderCount > 0)
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO: " + WorkOrder.EquipmentList[count].EquipmentNo + " " + GetErrorMessage(20015);
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            errorMessageList.Add(Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System Error on validating vendor code and Vendor Reference number";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }

            bool edit = true;
            string eqp = string.Empty;
            if (WorkOrder.woState != (int)Validation.STATE.NEW)
            {
                edit = false;
            }

            if (edit)
            {
                try
                {
                    for (int count = 0; count < WorkOrder.EquipmentList.Count; count++)
                    {
                        eqp = WorkOrder.EquipmentList[count].EquipmentNo;
                        WorkOrderCount = (from wo in objContext.MESC1TS_WO
                                          where wo.SHOP_CD == ShopCode &&
                                          wo.MODE == Mode &&
                                          wo.EQPNO == eqp &&
                                          wo.STATUS_CODE > 99 &&
                                          wo.STATUS_CODE < 9997 &&
                                          wo.WOTYPE == "S"
                                          select wo).Count();

                        if (WorkOrderCount > 0)
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO: " + WorkOrder.EquipmentList[count].EquipmentNo + " " + GetErrorMessage(20020);
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            errorMessageList.Add(Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Message = new ErrMessage();
                    Message.Message = "System error: ValidateUniqueWO: Failed on validating unique WO";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    success = false;
                }
            }

            //if (WorkOrder.WorkOrderStatus == 0 && WorkOrder.woState != (int)Validation.STATE.NEW)
            //{
            //    edit = true;
            //}

            //if (edit) // check duplicate  status > 99 and < 400 by shop eqno and mode
            //{
            //    try
            //    {
            //        for (int count = 0; count < WorkOrder.EquipmentList.Count; count++)
            //        {
            //            eqp = WorkOrder.EquipmentList[count].EquipmentNo;
            //            WorkOrderCount = (from wo in objContext.MESC1TS_WO
            //                              where wo.SHOP_CD == ShopCode &&
            //                              wo.MODE == Mode &&
            //                              wo.EQPNO == eqp &&
            //                              wo.STATUS_CODE > 99 &&
            //                              wo.STATUS_CODE < 400 &&
            //                              wo.WOTYPE == "S"
            //                              select wo).Count();

            //            if (WorkOrderCount > 0)
            //            {
            //                Message.Message = "EQPNO: " + WorkOrder.EquipmentList[count].EquipmentNo + " An open estimate exists for this unit in this mode - Please revise the estimate";
            //                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
            //                errorMessageList.Add(Message);
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Message = new ErrMessage();
            //        Message.Message = "System error: ValidateUniqueWO: Failed on validating unique WO";
            //        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
            //        errorMessageList.Add(Message);
            //        logEntry.Message = ex.ToString();
            //        Logger.Write(logEntry);
            //        success = false;
            //    }
            //}
            logEntry.Message = "Method Name : CheckDuplicate(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool CheckDuplicateRepairs(ref WorkOrderDetail WorkOrder, out List<ErrMessage> errorMessageList)
        {
            bool success = true;
            bool bFound = false;
            errorMessageList = new List<ErrMessage>();
            string ShopCode = WorkOrder.Shop.ShopCode;
            string Mode = WorkOrder.Mode;
            string sRemarkHeader = "Duplicate repairs found within 60 days for the same equipment number.";
            string sRepairCodes = string.Empty;
            string sWarningTail = "Verify estimate is not a duplicate and submit for approval<br>";
            string sVendorRefNoHold = string.Empty;
            string sNewVendorRefNo = string.Empty;
            string sDateHold = string.Empty;
            string sWOData = string.Empty;
            string sRemark = string.Empty;
            string sRemarkTail = "Verify estimate is not a duplicate prior approval";
            //List<string> DuplicateRepairCode = new List<string>();
            List<string> tempRepairCode = new List<string>();
            DateTime repairDate = DateTime.Now;
            string sWarning = "</br>";
            sWarning += sRemarkHeader;

            //if (WorkOrder.Mode.Equals("33", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    sRemarkHeader = "Duplicate repairs found within 30 days for the same equipment number.";
            //}
            //else
            //{
            //    sRemarkHeader = "Duplicate repairs found within 60 days for the same equipment number.";
            //}
            sRemarkTail = "Verify estimate is not a duplicate prior approval";
            sWarningTail = "Verify estimate is not a duplicate and submit for approval<br>";

            sWarning = "<br>";
            sWarning += sRemarkHeader;

            // perform if work order is new or is a draft.
            //Big confusion here
            //int n= atoi( rhsRecord.GetWO_ID();
            //if (n>0)
            //return( true );
            if (WorkOrder.woState != (int)Validation.STATE.NEW)
            {
                if (WorkOrder.WorkOrderID > 0)
                    return true;
            }

            try
            {

                // get SQL to find repairs - if empty, then none, return.
                // The call will filter out repair codes (listed in constants.asp).
                List<string> FilteredRepairCodes = GetFilterRepairCodes();
                foreach (var rItem in WorkOrder.RepairsViewList)
                {
                    if (FilteredRepairCodes.Any(filteredCds => filteredCds == rItem.RepairCode.RepairCod.TrimEnd()))
                    {
                        continue;
                    }
                    else
                    {
                        tempRepairCode.Add(rItem.RepairCode.RepairCod.Trim());
                    }
                }

                //sSQL = sql.GetFindDuplicateRepairCodeSQL( rhsRecord );

                //if(sSQL == 0)
                //{
                //    return true;
                //}
                if (tempRepairCode.Count == 0) return true;

                if (WorkOrder.Mode.Equals("33", StringComparison.CurrentCultureIgnoreCase))
                {
                    repairDate = repairDate.AddDays(-30);
                }
                else
                {
                    repairDate = repairDate.AddDays(-60);
                }

                string eqp = WorkOrder.EquipmentList[0].EquipmentNo;
                var DuplicateRepairCode = (from w in objContext.MESC1TS_WO
                                           from r in objContext.MESC1TS_WOREPAIR
                                           where w.EQPNO == eqp &&
                                           w.SHOP_CD == ShopCode &&
                                           w.MODE == Mode &&
                                           w.REPAIR_DTE >= repairDate &&
                                           w.WO_ID == r.WO_ID &&
                                           w.STATUS_CODE > 0 &&
                                           w.STATUS_CODE < 9000 &&
                                           tempRepairCode.Contains(r.REPAIR_CD)
                                           orderby w.VENDOR_REF_NO
                                           select new
                                           {
                                               r.REPAIR_CD,
                                               w.REPAIR_DTE,
                                               w.VENDOR_REF_NO
                                           }).ToList();


                //}
                //catch
                //{
                //    //System error: CheckDuplicateRepair: Failed on getting SQL
                //    errorMessageList.Add(Message);
                //    return (false);
                //}

                sVendorRefNoHold = "tempnonum";
                sRepairCodes = string.Empty;

                //try
                //{
                foreach (var item in DuplicateRepairCode)
                {
                    // check vendor reference number if different
                    sNewVendorRefNo = item.VENDOR_REF_NO;
                    // if diff vendor num create new remark and append to warnings as well.

                    if (!sNewVendorRefNo.Equals(sVendorRefNoHold, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // if != then if RepairCodes lenght > 0 add new remark and append to sWarning.
                        if (!string.IsNullOrEmpty(sRepairCodes))
                        {
                            // add WOData to remark

                            sRemark = sRemarkHeader;

                            sWOData = "<br>";
                            sWOData += sDateHold;
                            sWOData += "";
                            sWOData += sVendorRefNoHold;
                            sWOData += "";
                            sWOData += sRepairCodes;

                            sRemark += sWOData;
                            sRemark += sRemarkTail;

                            // safe - remark limited to 255 bytes;
                            if (sRemark.Length > 255) sRemark = sRemark.Substring(0, 255);
                            //rhsRecord.m_RemarkList.AddTail(new CRemarkRecord(m_StringUtil.GetCurrentTimeString(), "S", "", sRemark));

                            // reset holds to new record.
                            sVendorRefNoHold = item.VENDOR_REF_NO;
                            sDateHold = item.REPAIR_DTE.ToString();

                            // add this estimate to warning
                            sWarning += sWOData;
                        }
                        else
                        {
                            // is first record - set holds
                            sVendorRefNoHold = item.VENDOR_REF_NO;
                            sDateHold = item.REPAIR_DTE.ToString();
                            sRepairCodes = "";
                            sRepairCodes += item.REPAIR_CD.Trim();
                        }
                    }
                    else
                    {
                        // same estimate, so add this repair code to reapair list

                        sRepairCodes += "";
                        sRepairCodes += item.REPAIR_CD.Trim();
                    }
                    bFound = true; // set switch to save remark and create warnings on work order.
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "Fail on duplicate repair check in last 60 days";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return false;
            }

            // Create final warning and remarks if data found
            if (bFound)
            {

                sWOData += "<br>";
                sWOData += (sDateHold);
                sWOData += ("");
                sWOData += (sVendorRefNoHold);
                sWOData += ("");
                sWOData += (sRepairCodes);

                // append to warnings
                sWarning += (sWOData);
                sWarning += ("<br>");
                sWarning += (sWarningTail);

                // build remark string and create final remark for this record.
                sRemark = sRemarkHeader;
                sRemark += (sWOData);
                sRemark += (sRemarkTail);

                // safe - remark limited to 255 bytes;
                if (sRemark.Length > 255) sRemark = sRemark.Substring(0, 255);
                //rhsRecord.m_RemarkList.AddTail(new CRemarkRecord(m_StringUtil.GetCurrentTimeString(), "S", "", sRemark));

                if (WorkOrder.RemarksList == null)
                    WorkOrder.RemarksList = new List<RemarkEntry>();
                WorkOrder.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, sRemark));

                // add to warning list.for display to user on Work order entry page
                //ADD WARNING HANDLER
                //rhsRecord.m_sError += sWarning;
                Message.Message = sWarning;
                Message.ErrorType = Validation.MESSAGETYPE.WARNING.ToString();
                errorMessageList.Add(Message);
            }
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool CheckEquipmentMode(ref WorkOrderDetail WorkOrder, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : CheckEquipmentMode(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            bool success = true;
            errorMessageList = new List<ErrMessage>();
            string ShopCode = WorkOrder.Shop.ShopCode;
            string ModeCode = WorkOrder.Mode;
            string sMode = string.Empty;
            string sActiveSw = string.Empty;
            string sAluminumSw = string.Empty;
            string sEQSIZE = string.Empty;
            string EqpNo = WorkOrder.EquipmentList[0].EquipmentNo;

            try
            {
                for (int count = 0; count < WorkOrder.EquipmentList.Count; count++)
                {
                    string EquipmentNo = WorkOrder.EquipmentList[count].EquipmentNo;
                    // set aluminum switch to Y if EQMATR = ALU - for search on record set.
                    if (WorkOrder.EquipmentList[count].Eqmatr.Equals("ALU", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sAluminumSw = "Y";
                    }
                    else
                    {
                        sAluminumSw = "N";
                    }
                    // if m_sEQSIZE is empty, force aluminSW to 'N'
                    if (string.IsNullOrEmpty(WorkOrder.EquipmentList[count].Size))
                    {
                        sAluminumSw = "N";
                    }

                    // check size if we have a GENS - no sizes - use NA for Not Applicable
                    sEQSIZE = WorkOrder.EquipmentList[count].Size;
                    if (WorkOrder.EquipmentList[count].COType.Equals("GENS", StringComparison.CurrentCultureIgnoreCase) && string.IsNullOrEmpty(sEQSIZE))
                    {
                        sEQSIZE = "NA";
                    }
                    //Found = false;

                    string eqpCO = WorkOrder.EquipmentList[count].COType;
                    string eqpSType = WorkOrder.EquipmentList[count].Eqstype;

                    var Mode = (from m in objContext.MESC1TS_MODE
                                from em in objContext.MESC1TS_EQMODE
                                where em.COTYPE == eqpCO &&
                                em.EQSTYPE == eqpSType &&
                                em.ALUMINIUM_SW == sAluminumSw &&
                                (!string.IsNullOrEmpty(sEQSIZE) ? em.EQSIZE == sEQSIZE : em.EQSIZE == null) &&
                                    //em.EQSIZE == WorkOrder.EquipmentList[count].Size && 
                                m.MODE == em.MODE &&
                                m.MODE == ModeCode
                                select new
                                {
                                    m.MODE,
                                    m.MODE_ACTIVE_SW
                                }).ToList();

                    if (Mode.Count > 0)
                    {
                        foreach (var item in Mode)
                        {
                            sMode = item.MODE;
                            if (!sMode.Equals(WorkOrder.Mode, StringComparison.CurrentCultureIgnoreCase))
                            {
                                Message = new ErrMessage();
                                Message.Message = "EQPNO: " + WorkOrder.EquipmentList[count].EquipmentNo + " " + GetErrorMessage(20040);
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                errorMessageList.Add(Message);
                            }
                            else
                            {
                                sActiveSw = item.MODE_ACTIVE_SW;
                                if (!sActiveSw.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Message = new ErrMessage();
                                    Message.Message = "EQPNO: " + WorkOrder.EquipmentList[count].EquipmentNo + " " + GetErrorMessage(20040);
                                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                    errorMessageList.Add(Message);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Found = false;
                    }

                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System error: ValidateUniqueWO: Failed on validating unique WO";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return false;
            }

            //this is  the same check as above without the eqsize
            //if (!Found)
            //{
            //    try
            //    {
            //    }
            //    catch
            //    {
            //    }
            //}

            // Pre-trip inspection check.
            // check to see if pre-trip inspection is in the repair collection
            // i.e., PTICODE=0940 repair code with modes 43 or 45.

            //Found = false;
            int? Days = 60; // 60=default days before pre-trip-inspection allowed

            for (int count = 0; count < WorkOrder.RepairsViewList.Count; count++)
            {
                if (WorkOrder.RepairsViewList[count].rState == (int)Validation.STATE.DELETED) continue;

                if (!IsRepairTaxCode(WorkOrder.RepairsViewList[count].RepairCode))
                {
                    if (WorkOrder.RepairsViewList[count].RepairCode.RepairCod.Equals(PTICODE, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (WorkOrder.Mode.Equals(PTIMODE1, StringComparison.CurrentCultureIgnoreCase) || WorkOrder.Mode.Equals(PTIMODE2, StringComparison.CurrentCultureIgnoreCase))
                        {
                            // found a PTI repair code.
                            // search for PTI
                            var PTI = (from pti in objContext.MESC1TS_PTI
                                       where pti.SERIAL_NO_FROM == EqpNo
                                       select new
                                       {
                                           pti.NO_OF_DAYS
                                       }).ToList();

                            foreach (var item in PTI)
                            {
                                Days = item.NO_OF_DAYS;
                                // now check if PTI date falls within number of days.
                                if (!m_bBypassLease)
                                {
                                    success = CheckPTIDate(WorkOrder, Days, out errorMessageList);
                                }
                            }
                        }
                    }
                }
            }
            logEntry.Message = "Method Name : CheckEquipmentMode(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            return success;
        }

        /// <summary>
        /// Assuming call to RKEM already done. Else will not be able to determine the equipment type
        /// determine equipment type for MERC plus
        /// Check the indicator code.
        /// #9 Check REFU and GENS
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool CheckManufacturer(ref WorkOrderDetail WO, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : CheckManufacturer(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            errorMessageList = new List<ErrMessage>();
            bool bOK = true;
            string sEquipType;
            Equipment Eqp = WO.EquipmentList[0];
            // values should be coming in from RKEM - below code is from original MERC
            // default to COTYPE
            sEquipType = Eqp.COType;


            // Check only valid for WO type of REFU or GENS, and WOType not = "N".
            if (string.Equals(Eqp.COType, Equipment.CONTAINERTYPE.GENS.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                sEquipType = Equipment.CONTAINERTYPE.GENS.ToString();
            }
            else	// special case - for Maersk.
            {
                // (VJP - 12-13-2004) Subtype RFHV - labor rates calculated same as for subtype 'REEF'
                if ((string.Equals(Eqp.COType, Equipment.CONTAINERTYPE.CONT.ToString(), StringComparison.OrdinalIgnoreCase))
                    && ((string.Equals(Eqp.Eqstype, Equipment.CONTAINERTYPE.REEF.ToString(), StringComparison.OrdinalIgnoreCase))
                    || (string.Equals(Eqp.Eqstype, Equipment.CONTAINERTYPE.RFHV.ToString(), StringComparison.OrdinalIgnoreCase))))
                // original - did not include RFHV subtype
                {
                    if ((WO.Mode.StartsWith("4")) || (string.Equals(WO.Mode, "80", StringComparison.OrdinalIgnoreCase)))
                    {
                        sEquipType = Equipment.CONTAINERTYPE.REFU.ToString();
                    }
                    else
                    {
                        sEquipType = Equipment.CONTAINERTYPE.REEF.ToString();
                    }
                }
            }

            // Set record MERC equipment type.
            WO.EquipmentList[0].Type = sEquipType;

            string sIndicatorCd;
            m_fDiscountPercent = 0;


            // Only do this check if the mode is a 4x mode.  (VJP 02-25-2004)
            if ((string.Equals(Equipment.CONTAINERTYPE.GENS.ToString(), sEquipType, StringComparison.OrdinalIgnoreCase)) ||
                (string.Equals(Equipment.CONTAINERTYPE.REFU.ToString(), sEquipType, StringComparison.OrdinalIgnoreCase)))
            {

                try
                {
                    // NOTE: the Manufacturer SQL is no longer using the EQMANCD, just the eq type EQRUTYP 
                    //pRs = m_pManufacturer->RSByMfgAndModel(rhsRecord.m_sEQMANCD.c_str(), rhsRecord.m_sEQRUTYP.c_str(), &hr);
                    string eqpRUType = Eqp.EQRutyp;

                    var sInCode = (from model in objContext.MESC1TS_MODEL
                                   where model.MODEL_NO == eqpRUType
                                   select new
                                   {
                                       model.INDICATOR_CD
                                   }).FirstOrDefault();

                    if (sInCode != null)
                    {
                        sIndicatorCd = sInCode.INDICATOR_CD;

                        // Getting discount percent based on part manufacturer code by part
                        // Get discount percent for part price calculations. Entered as whole percentage, translate into decimal format
                        // iterate through repair list - check indicator code against first char in repair codes.
                        foreach (var repV in WO.RepairsViewList)
                        {
                            if (repV.rState == (int)Validation.STATE.DELETED)
                                continue;

                            // RepairCd[0] must equal IndicatorCd[0]; else is error.
                            // unless first digit is a zero... Added zero check 05-03-01 - VJP
                            // Ensure is not a tax repair code
                            if (!IsRepairTaxCode(repV.RepairCode))
                            {
                                if (!repV.RepairCode.RepairCod.StartsWith("0"))
                                {
                                    if (!string.Equals(sIndicatorCd.Substring(0, 1), repV.RepairCode.RepairCod.Substring(0, 1)))
                                    {
                                        // Changed error msg format 05-29-01 - VJP
                                        //INVALID CODE - CODES FOR THIS UNIT SHOULD START WITH x-. PLS CHANGE yzzz TO xzzz.
                                        // build what repair code should be
                                        string temp = repV.RepairCode.RepairCod;

                                        StringBuilder sb = new StringBuilder(temp);
                                        sb[0] = char.Parse(sIndicatorCd);
                                        temp = sb.ToString();
                                        Message = new ErrMessage();
                                        Message.Message = "EQPNO: " + Eqp.EquipmentNo + ", Invalid code - codes for this unit should start with " + sIndicatorCd.Substring(0, 1) + ": Please change " + repV.RepairCode.RepairCod + " to " + temp + "";
                                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                        errorMessageList.Add(Message);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (NullReferenceException ex)	// Catch ADO data errors.
                {
                    Message = new ErrMessage();
                    Message.Message = "System Error on validating Manufacturer REFU and GENS";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    bOK = false;
                }
                catch (ArgumentException ex)
                {
                    //strcpy( cmsg, "System Error on validating Manufacturer REFU and GENS" );
                    //pLog.CreateInstance(__uuidof(SystemErrorManager));
                    //pLog->WriteApplicationLog(PROGRAMNAME, cmsg);
                    //pLog = NULL;
                    Message = new ErrMessage();
                    Message.Message = "m_pManufacturer->RSByEQTypeNumber: Failed open of Manufacturer recordset";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    bOK = false;
                }
            }
            logEntry.Message = "Method Name : CheckManufacturer(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            return bOK;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool CheckLaborRate(ref WorkOrderDetail WorkOrder, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : CheckLaborRate(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            bool success = true;
            errorMessageList = new List<ErrMessage>();
            string ShopCode = WorkOrder.Shop.ShopCode;
            string ModeCode = WorkOrder.Mode;
            string CustomerCode = WorkOrder.Shop.Customer[0].CustomerCode;
            WorkOrder.Shop.LaborRate = new List<LaborRate>();
            LaborRate LaborRate = new MercPlusLibrary.LaborRate();
            double fCountryLaborExchangRate = 1.0;
            string Type = WorkOrder.EquipmentList[0].COType;


            if (WorkOrder.Shop.Customer[0].MaerskSw.Equals("Y", StringComparison.CurrentCultureIgnoreCase) && string.IsNullOrEmpty(WorkOrder.EquipmentList[0].EquipmentNo)) return false;

            // switches - required to flag errors if any of these rates are in DB as null or empty
            m_bNULLRegHours = false;
            m_bNULLDblHours = false;
            m_bNULLOTHours = false;
            m_bNULLMiscHours = false;

            LaborRate.RegularRT = 0;
            LaborRate.RegularRTCPH = 0;
            LaborRate.MiscRT = 0;
            LaborRate.MiscRTCPH = 0;
            LaborRate.DoubleTimeRT = 0;
            LaborRate.DoubleTimeRTCPH = 0;
            LaborRate.OvertimeRT = 0;
            LaborRate.OvertimeRTCPH = 0;

            // :Original Merc - No labor rates for non-standard work order.
            if (WorkOrder.WorkOrderType.Equals("N", StringComparison.CurrentCultureIgnoreCase)) return false;

            if (string.IsNullOrEmpty(Type))
            {
                Message = new ErrMessage();
                Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + "No Equipment type found - unable to check labour rates by shop.";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
            }
            else
            {
                try
                {
                    var LaborRateByCustomerShopList = (from rate in objContext.MESC1TS_LABOR_RATE
                                                   where rate.CUSTOMER_CD == CustomerCode &&
                                                   rate.SHOP_CD == ShopCode
                                                   select new
                                                   {
                                                       rate.CUSTOMER_CD,
                                                       rate.SHOP_CD,
                                                       rate.EQTYPE,
                                                       rate.EFF_DTE,
                                                       rate.EXP_DTE,
                                                       rate.REGULAR_RT,
                                                       rate.OVERTIME_RT,
                                                       rate.DOUBLETIME_RT,
                                                       rate.MISC_RT
                                                   }).ToList();



                    if (LaborRateByCustomerShopList != null && LaborRateByCustomerShopList.Count > 0)
                    {
                        DateTime? RepairDate = WorkOrder.RepairDate;
                        var LaborRateByCustomerShop = LaborRateByCustomerShopList.Find(filter => filter.EQTYPE == Type
                            && filter.EFF_DTE <= RepairDate && filter.EXP_DTE >= RepairDate);
                        // set null switches to true on misc rates  - for later validations.
                        // ADO failing n recognizing null field values.- the following code is necessary
                        if (LaborRateByCustomerShop != null) // && LaborRateByCustomerShop.Count > 0)
                        {
                            try
                            {
                                LaborRate.RegularRT = LaborRateByCustomerShop.REGULAR_RT;
                            }
                            catch
                            {
                                LaborRate.RegularRT = (decimal)0.0;
                                m_bNULLRegHours = true;
                            }
                            try
                            {
                                LaborRate.DoubleTimeRT = LaborRateByCustomerShop.DOUBLETIME_RT;
                            }
                            catch
                            {
                                LaborRate.DoubleTimeRT = 0;
                                m_bNULLDblHours = true;
                            }
                            try
                            {
                                LaborRate.MiscRT = LaborRateByCustomerShop.MISC_RT;
                            }
                            catch
                            {
                                LaborRate.MiscRT = 0;
                                m_bNULLMiscHours = true;
                            }

                            try
                            {
                                LaborRate.OvertimeRT = LaborRateByCustomerShop.OVERTIME_RT;
                            }
                            catch
                            {
                                LaborRate.OvertimeRT = 0;
                                m_bNULLOTHours = true;
                            }

                            // New 02-02-2004 - if any value is 0, then rates are invalid.
                            if (LaborRate.RegularRT == 0) m_bNULLRegHours = true;
                            if (LaborRate.DoubleTimeRT == 0) m_bNULLDblHours = true;
                            if (LaborRate.MiscRT == 0) m_bNULLMiscHours = true;
                            if (LaborRate.OvertimeRT == 0) m_bNULLOTHours = true;
                            WorkOrder.Shop.LaborRate.Add(LaborRate);

                            // place in record
                            WorkOrder.ManHourRate = Math.Round(LaborRate.RegularRT.Value, 2);
                            WorkOrder.OverTimeRate = Math.Round(LaborRate.OvertimeRT.Value, 2);
                            WorkOrder.DoubleTimeRate = Math.Round(LaborRate.DoubleTimeRT.Value, 2);
                            WorkOrder.MiscRate = Math.Round(LaborRate.MiscRT.Value, 2);

                        }
                        else
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + " Labour rate not created for this equipment type.";
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            errorMessageList.Add(Message);
                            WorkOrder.ManHourRate = 0;
                            WorkOrder.OverTimeRate = 0;
                            WorkOrder.DoubleTimeRate = 0;
                            WorkOrder.MiscRate = 0;
                        }
                    }
                    else
                    {
                        Message = new ErrMessage();
                        Message.Message = "Rate schedule not found for Customer: " + WorkOrder.Shop.Customer[0].CustomerCode + ", Shop Code: " + WorkOrder.Shop.ShopCode;
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        errorMessageList.Add(Message);
                    }
                }
                catch (Exception ex)
                {
                    Message = new ErrMessage();
                    Message.Message = "System Error on validating Labour Rates";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    success = false;
                }
            }

            try
            {
                //FInd the country code first

                var locCode = (from s in objContext.MESC1TS_SHOP
                               where s.SHOP_CD == ShopCode
                               select new
                               {
                                   s.LOC_CD
                               }).FirstOrDefault();

                var countryCode = (from c in objContext.MESC1TS_LOCATION
                                   where c.LOC_CD == locCode.LOC_CD
                                   select new
                                   {
                                       c.COUNTRY_CD
                                   }).FirstOrDefault();

                // CPH Rates
                // Get country labor rates as well if existing. If no country contract, use local shop labor rates.
                //string eqp = WorkOrder.EquipmentList[0].COType;
                var CountryRateByShopTypeDate = (from COUN in objContext.MESC1TS_COUNTRY_LABOR
                                                 from CURR in objContext.MESC1TS_CURRENCY
                                                 where COUN.COUNTRY_CD == countryCode.COUNTRY_CD &&
                                                 COUN.EQTYPE == Type &&
                                                 DateTime.Now >= COUN.EFF_DTE &&
                                                 DateTime.Now <= COUN.EXP_DTE &&
                                                 COUN.CUCDN == CURR.CUCDN
                                                 select new
                                                 {
                                                     COUN.REGULAR_RT,
                                                     COUN.DOUBLETIME_RT,
                                                     COUN.MISC_RT,
                                                     COUN.OVERTIME_RT,
                                                     COUN.CUCDN,
                                                     CURR.EXRATUSD,
                                                     CURR.QUOTEDAT
                                                 }).ToList();

                if (CountryRateByShopTypeDate != null && CountryRateByShopTypeDate.Count > 0)
                {
                    foreach (var item in CountryRateByShopTypeDate)
                    {
                        fCountryLaborExchangRate = (double)item.EXRATUSD;
                        //fCountryLaborExchangRate = Math.Round(fCountryLaborExchangRate, 6);
                        WorkOrder.CountryExchangeRate = (decimal)fCountryLaborExchangRate;
                        WorkOrder.CountryCUCDN = item.CUCDN;
                        //WorkOrder.Shop.CUCDN = item.CUCDN;
                        if (item.QUOTEDAT != null)
                        {
                            try
                            {
                                WorkOrder.CountryExchangeDate = item.QUOTEDAT;
                            }
                            catch
                            {
                                WorkOrder.CountryExchangeDate = null;
                            }
                            // fianlly - convert to decimal value for calculations
                            fCountryLaborExchangRate *= 0.01;
                            LaborRate.RegularRTCPH = (item.REGULAR_RT == null ? 0 : item.REGULAR_RT * ((decimal)fCountryLaborExchangRate));
                            LaborRate.OvertimeRTCPH = (item.OVERTIME_RT == null ? 0 : (item.OVERTIME_RT) * (decimal)fCountryLaborExchangRate);
                            LaborRate.DoubleTimeRTCPH = (item.DOUBLETIME_RT == null ? 0 : (item.DOUBLETIME_RT) * (decimal)fCountryLaborExchangRate);
                            LaborRate.MiscRTCPH = (item.MISC_RT == null ? 0 : (item.MISC_RT) * (decimal)fCountryLaborExchangRate);
                            //WorkOrder.Shop.LaborRate.Add(LaborRate);

                        }
                    }
                }
                else // No Country/Local MSL contract, so use existing shop contract values.
                {
                    LaborRate.RegularRTCPH = LaborRate.RegularRT;
                    LaborRate.OvertimeRTCPH = LaborRate.OvertimeRT;
                    LaborRate.DoubleTimeRTCPH = LaborRate.DoubleTimeRT;
                    LaborRate.MiscRTCPH = LaborRate.MiscRT;

                    // Convert local shop values to US$ at this point for CPH.
                    LaborRate.RegularRTCPH *= WorkOrder.ExchangeRate;
                    LaborRate.OvertimeRTCPH *= WorkOrder.ExchangeRate;
                    LaborRate.DoubleTimeRTCPH *= WorkOrder.ExchangeRate;
                    LaborRate.MiscRTCPH *= WorkOrder.ExchangeRate;
                    //WorkOrder.Shop.LaborRate.Add(LaborRate);
                }
                WorkOrder.ManHourRateCPH = (LaborRate.RegularRTCPH == null ? 0 : Math.Round(LaborRate.RegularRTCPH.Value, 4));
                WorkOrder.OverTimeRateCPH = (LaborRate.OvertimeRTCPH == null ? 0 : Math.Round(LaborRate.OvertimeRTCPH.Value, 4));
                WorkOrder.DoubleTimeRateCPH = (LaborRate.DoubleTimeRTCPH == null ? 0 : Math.Round(LaborRate.DoubleTimeRTCPH.Value, 4));
                WorkOrder.MiscRateCPH = (LaborRate.MiscRTCPH == null ? 0 : Math.Round(LaborRate.MiscRTCPH.Value, 4));
                WorkOrder.Shop.LaborRate.Add(LaborRate);
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System Error on validating Country Labor Rates";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                success = false;
            }
            logEntry.Message = "Method Name : CheckLaborRate(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool CheckExclusiveRepair(ref WorkOrderDetail WorkOrder, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : CheckExclusiveRepair(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            bool success = true;
            errorMessageList = new List<ErrMessage>();

            List<SparePartsView> sparePartsList = null;
            double? dRecordHours = 0.0;
            double dTableHours = 0.0;
            decimal? dTmp = 0;
            decimal? dUSDAmt = 0;
            decimal tempVar = 0;
            string ManualCode = WorkOrder.Shop.Customer[0].ManualCode;
            string Mode = WorkOrder.Mode;


            if (!WorkOrder.WorkOrderType.Equals("N", StringComparison.CurrentCultureIgnoreCase))
            {
                try
                {
                    var LoadByManualAndMode = (from RC in objContext.MESC1TS_REPAIR_CODE
                                               join RE in objContext.MESC1TS_RPRCODE_EXCLU on new { RC1 = RC.MANUAL_CD, RC2 = RC.MODE, RC3 = RC.REPAIR_CD.Trim() }
                                               equals new { RC1 = RE.MANUAL_CD, RC2 = RE.MODE, RC3 = RE.REPAIR_CD.Trim() } into Inner
                                               from O in Inner.DefaultIfEmpty()
                                               where RC.REPAIR_ACTIVE_SW == "Y" &&
                                               RC.MANUAL_CD == ManualCode &&
                                               RC.MODE == Mode
                                               select new
                                               {
                                                   RC.MANUAL_CD,
                                                   RC.MODE,
                                                   RC.REPAIR_CD,
                                                   RC.REPAIR_DESC,
                                                   O.EXCLU_REPAIR_CD,
                                                   RC.MAX_QUANTITY,
                                                   RC.MAN_HOUR,
                                                   RC.REPAIR_IND,
                                                   RC.SHOP_MATERIAL_CEILING,
                                                   RC.TAX_APPLIED_SW,
                                                   RC.ALLOW_PARTS_SW
                                               }).ToList();

                    //if (LoadByManualAndMode != null && LoadByManualAndMode.Count > 0)
                    //{
                    if (LoadByManualAndMode != null && LoadByManualAndMode.Count > 0)
                    {
                        // if table is not empty...
                        // iterate through collection of repair codes.
                        foreach (var repairItem in WorkOrder.RepairsViewList)
                        {
                            // Check if marked for deletion
                            if (repairItem.rState == (int)Validation.STATE.DELETED) continue;

                            if (!IsRepairTaxCode(repairItem.RepairCode))
                            {
                                //First check that Repair code exists in table,
                                var LoadManualMode = LoadByManualAndMode.Find(rCode => rCode.REPAIR_CD.TrimEnd() == repairItem.RepairCode.RepairCod.Trim());
                                if (LoadByManualAndMode.Any(repCode => repCode.REPAIR_CD.TrimEnd() == repairItem.RepairCode.RepairCod.Trim()))
                                {
                                    // Check if repair contains parts - if so check if allowed to contain parts
                                    if (WorkOrder.SparePartsViewList != null && WorkOrder.SparePartsViewList.Count > 0)
                                    {
                                        sparePartsList = new List<SparePartsView>();
                                        sparePartsList = WorkOrder.SparePartsViewList.FindAll(parts => parts.RepairCode.RepairCod.Trim() == repairItem.RepairCode.RepairCod.Trim());
                                        if (sparePartsList != null && sparePartsList.Count > 0)
                                        {
                                            if (LoadManualMode.ALLOW_PARTS_SW != null)
                                            {
                                                if (LoadManualMode.ALLOW_PARTS_SW.Equals("N", StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    Message = new ErrMessage();
                                                    Message.Message = "No parts allowed for repair code" + " " + LoadManualMode.REPAIR_CD.Trim();
                                                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                                    errorMessageList.Add(Message);
                                                    continue;
                                                }
                                            }
                                        }
                                    }

                                    decimal? dCPHAmount = repairItem.MaterialCost;
                                    dCPHAmount *= WorkOrder.ExchangeRate;
                                    repairItem.MaterialCostCPH = dCPHAmount;

                                    if (LoadManualMode.MAX_QUANTITY != null)
                                    {
                                        if (repairItem.Pieces > LoadManualMode.MAX_QUANTITY)
                                        {
                                            Message = new ErrMessage();
                                            Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + " " + GetErrorMessage(20070) + " code: " + repairItem.RepairCode.RepairCod + " Max pieces allowed:" + LoadManualMode.MAX_QUANTITY;
                                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                            errorMessageList.Add(Message);
                                        }
                                    }

                                    // Get Tax applied switch
                                    repairItem.RepairCode.TaxAppliedSW = LoadManualMode.TAX_APPLIED_SW;

                                    // Check Man Hours not exceeding REPAIR_CODE maximum man hours.
                                    //FP7410(4) checking modified for max Manhours - checking against summation of manhrs for same repair code
                                    dRecordHours = repairItem.ManHoursPerPiece;
                                    dRecordHours = Math.Round(dRecordHours.Value, 2);
                                    if (LoadManualMode.MAN_HOUR == null)
                                    {
                                        dTableHours = (float)0.0;
                                    }
                                    else
                                    {
                                        dTableHours = (double)LoadManualMode.MAN_HOUR;
                                    }

                                    if (dRecordHours > dTableHours)
                                    {
                                        Message = new ErrMessage();
                                        Message.Message = " Repair code: " + repairItem.RepairCode.RepairCod + " " + " Repair Location Code: " + repairItem.RepairLocationCode.CedexCode + " " + GetErrorMessage(20075);
                                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                        errorMessageList.Add(Message);
                                    }

                                    //// Add repair code description here.
                                    //if (!pRecord->IsExisting()) // don't change this value to an existing record
                                    if (repairItem.rState != (int)Validation.STATE.EXISTING)
                                    {
                                        repairItem.RepairCode.RepairDesc = LoadManualMode.REPAIR_DESC;
                                        repairItem.RepairCode.RepairInd = LoadManualMode.REPAIR_IND;
                                        if (repairItem.rState != (int)Validation.STATE.NEW)
                                        {
                                            repairItem.rState = (int)Validation.STATE.UPDATED;
                                        }
                                    }

                                    dTmp = LoadManualMode.SHOP_MATERIAL_CEILING / WorkOrder.ExchangeRate;

                                    repairItem.RepairCode.ShopMaterialCeiling = (decimal)-1.0;
                                    //TO BE DONE
                                    GetContractAmounts(WorkOrder, repairItem, dTmp);

                                    // if we have a shop contract ceiling
                                    // check 0 ceiling
                                    if (repairItem.RepairCode.ShopMaterialCeiling >= 0)
                                    {
                                        if (repairItem.MaterialCost > repairItem.RepairCode.ShopMaterialCeiling)
                                        {
                                            tempVar = (decimal)repairItem.RepairCode.ShopMaterialCeiling;
                                            tempVar = Math.Round(tempVar, 4);
                                            Message = new ErrMessage();
                                            Message.Message = WorkOrder.EquipmentList[0].EquipmentNo + ": Shop Materials for code " + repairItem.RepairCode.RepairCod + " are limited to " + tempVar + " - please correct and resubmit";
                                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                            errorMessageList.Add(Message);
                                        }
                                    }
                                    else
                                    {
                                        if (LoadManualMode.SHOP_MATERIAL_CEILING != null)
                                        {
                                            try
                                            {
                                                //dTmp = LoadManualMode.SHOP_MATERIAL_CEILING;
                                                repairItem.RepairCode.ShopMaterialCeiling = LoadManualMode.SHOP_MATERIAL_CEILING;
                                                if (repairItem.rState == (int)Validation.STATE.EXISTING || repairItem.rState == (int)Validation.STATE.UPDATED)
                                                {
                                                    repairItem.rState = (int)Validation.STATE.UPDATED;
                                                }
                                                dUSDAmt = repairItem.MaterialCost;
                                                dUSDAmt *= WorkOrder.ExchangeRate;
                                                if (dUSDAmt > repairItem.RepairCode.ShopMaterialCeiling)
                                                {
                                                    Message = new ErrMessage();
                                                    Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + ", Repair material amount exceeds Repair Material Ceiling for repair code: " + repairItem.RepairCode.RepairCod;
                                                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                                    errorMessageList.Add(Message);
                                                }
                                                else
                                                {
                                                    if (repairItem.MaterialCostCPH >= 0)
                                                    {
                                                        dUSDAmt = Math.Round(dUSDAmt.Value, 4);
                                                        if (dUSDAmt > repairItem.MaterialCostCPH)
                                                        {
                                                            if (WorkOrder.RemarksList == null)
                                                                WorkOrder.RemarksList = new List<RemarkEntry>();
                                                            WorkOrder.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "Shop materials too high according to contract between country/CENEMR for repair code:" + repairItem.RepairCode.RepairCod));
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Message = new ErrMessage();
                                                Message.Message = "System error: While checking Exclusive repair.";
                                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                                errorMessageList.Add(Message);
                                                logEntry.Message = ex.ToString();
                                                Logger.Write(logEntry);
                                                success = false;
                                            }
                                        }
                                    }

                                    //while(!LoadByManualAndMode)
                                    // Check exclusive repair code value for this repair code against current collection of repair codes.
                                    string temp = string.Empty;
                                    if (LoadManualMode.EXCLU_REPAIR_CD != null)
                                    {
                                        temp = LoadManualMode.EXCLU_REPAIR_CD.TrimEnd();
                                    }
                                    // exclusive code found for this repair code, check if exists in collection.
                                    if (!string.IsNullOrEmpty(temp))
                                    {
                                        // Search Repair collection for any record with this repair code in collection
                                        var found = WorkOrder.RepairsViewList.Find(excluRCode => excluRCode.RepairCode.RepairCod.Trim() == temp.Trim());
                                        if (found != null && found.rState != (int)Validation.STATE.DELETED)
                                        {
                                            Message = new ErrMessage();
                                            Message.Message = "EQPNO : " + WorkOrder.EquipmentList[0].EquipmentNo + " " + GetErrorMessage(20080) + ".";
                                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                            errorMessageList.Add(Message);
                                        }
                                    }
                                }
                                else
                                {
                                    Message = new ErrMessage();
                                    Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + " " + GetErrorMessage(20065) + " " + repairItem.RepairCode.RepairCod;
                                    //sprintf( cmsg,"EQPNO: %s, %s, %s",rhsRecord.m_sEQPNO.c_str(),GetErrorMessage( 20065 ).c_str(), pRecord->GetRepairCd().c_str() );
                                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                    errorMessageList.Add(Message);
                                    success = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        //end if for count > 0
                        foreach (var item in WorkOrder.RepairsViewList)
                        {
                            if (item.rState == (int)Validation.STATE.DELETED) continue;
                            if (!IsRepairTaxCode(item.RepairCode))
                            {
                                Message = new ErrMessage();
                                Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + " " + GetErrorMessage(20065) + " " + item.RepairCode.RepairCod;
                                //sprintf( cmsg,"EQPNO: %s, %s, %s",rhsRecord.m_sEQPNO.c_str(),GetErrorMessage( 20065 ).c_str(), pRecord->GetRepairCd().c_str() );
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                errorMessageList.Add(Message);
                            }
                        }
                    }
                    //}

                    //End if for null
                    //else
                    //{
                    //    //what happens if no repairs for manual mode.
                    //    Message.Message = "Unable to verify repair codes - system error";
                    //    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    //    errorMessageList.Add(Message);
                    //}
                }
                catch (Exception ex)
                {
                    Message = new ErrMessage();
                    Message.Message = "Unable to verify repair codes - system error";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    success = false;
                }
            }
            // Check if WOType == "N"
            // Check NONSCodes for each Repair record. Original MERC code - leave in for non sts wo's
            else
            {
                try
                {
                    foreach (var item in WorkOrder.RepairsViewList)
                    {
                        if (item.rState == (int)Validation.STATE.DELETED) continue;

                        // if not a special tax repair record...
                        if (!IsRepairTaxCode(item.RepairCode))
                        {
                            var NONSCode = (from nons in objContext.MESC1TS_NONSCODE
                                            where nons.MODE == Mode &&
                                            nons.NONS_CD == item.RepairCode.RepairCod.Trim() &&
                                            nons.NONS_ACTIVE_SW == "Y"
                                            select nons).ToList();

                            if (NONSCode == null)
                            {
                                Message = new ErrMessage();
                                Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + " " + GetErrorMessage(20065) + " " + item.RepairCode.RepairCod;
                                //sprintf( cmsg,"EQPNO: %s, %s, %s",rhsRecord.m_sEQPNO.c_str(),GetErrorMessage( 20065 ).c_str(), pRecord->GetRepairCd().c_str() );
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                errorMessageList.Add(Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Message = new ErrMessage();
                    Message.Message = "System Error on validating NONSCode";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    success = false;
                }
            }

            if (errorMessageList.Count > 0)
            {
                success = false;
            }
            logEntry.Message = "Method Name : CheckExclusiveRepair(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool CheckParts(ref WorkOrderDetail WorkOrder, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : CheckParts(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            bool success = true;
            errorMessageList = new List<ErrMessage>();


            //float fPartPrice;
            decimal? fPartPrice = 0; //test
            //long nQtyPerBox;
            int? nQtyPerBox;
            string sActiveSw = string.Empty;
            string sPartCode = string.Empty;
            string sDeductCoreSw = string.Empty;
            string sCorePartSw = string.Empty;

            // calculation work
            bool bMaerskPart = false;
            bool bDeductCore = false;
            bool bFound = false;

            string tmp = string.Empty; ;
            string sManufacturer = string.Empty; ;
            string sShopCd = WorkOrder.Shop.ShopCode;
            List<SparePartsView> PartsList = null;
            SparePartsView spView = null;


            foreach (var repairItem in WorkOrder.RepairsViewList)
            {
                // Check if repair code is deleted
                if (repairItem.rState == (int)Validation.STATE.DELETED) continue;
                //if (repairItem.IsDeleted())
                // continue;

                if (WorkOrder.SparePartsViewList != null && WorkOrder.SparePartsViewList.Count > 0)
                {
                    PartsList = new List<SparePartsView>();
                    PartsList = WorkOrder.SparePartsViewList.FindAll(rCd => rCd.RepairCode.RepairCod.Trim() == repairItem.RepairCode.RepairCod.Trim());
                    foreach (var partsItem in PartsList) //pPartsRecord
                    {
                        if (partsItem.pState == (int)Validation.STATE.DELETED)
                            continue;

                        try
                        {
                            if (partsItem.pState != (int)Validation.STATE.DELETED)
                            {
                                sPartCode = partsItem.OwnerSuppliedPartsNumber; //(i.e. the partCode)
                                bFound = false;
                                success = true;

                                spView = GetSpareDetails(sPartCode);
                                while (!bFound)
                                {
                                    if (spView != null)
                                    {
                                        sActiveSw = spView.PART_ACTIVE_SW.ToString();
                                        if (sActiveSw.Equals("N", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            success = false;
                                            bFound = true;
                                            Message = new ErrMessage();
                                            //sprintf(cmsg, "EQPNO: %s, Part Code is inactive: %s", rhsRecord.m_sEQPNO.c_str(),pPartsRecord->GetPartCode().c_str() );
                                            Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + ",Part Code is inactive: " + partsItem.OwnerSuppliedPartsNumber;
                                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                            errorMessageList.Add(Message);
                                        }
                                        else // check if part price == 0. if yes, repeat getting superceded part in PART_DESC
                                        {
                                            fPartPrice = (spView.PART_PRICE == null ? 0 : spView.PART_PRICE);
                                            // NOTE: Parts pricing is in US Dollars - it must be converted to the local currency
                                            // So... divide the value by the exchange rate to convert to local currency.
                                            if (WorkOrder.ExchangeRate != 0)
                                            {
                                                fPartPrice = fPartPrice / WorkOrder.ExchangeRate;
                                            }

                                            // if price == 0 then part must be superceded. New part number is in the part_desc field
                                            // Extract and re-query recordset with new data.
                                            if (fPartPrice == 0)
                                            {
                                                sPartCode = spView.PartDescription;
                                                sPartCode = sPartCode.Trim();

                                                spView = GetSpareDetails(sPartCode);
                                            }
                                            else // FINALLY - we have a legitimate part record
                                            {
                                                // Need to get the manufacture discount percent based on the manufacturer name in the part table.
                                                sManufacturer = spView.MANUFCTR;
                                                // populate m_fDiscountPercent by manufacturer code
                                                GetDiscountPercent(sManufacturer);
                                                //m_fDiscountPercent = Math.Round(m_fDiscountPercent.Value, 1);
                                                // msg1 IS NEVER USED? WHATS THE POINT?
                                                //sprintf(msg1,"%f",m_fDiscountPercent); 
                                                //m_fDiscountPercent = Math.Round(m_fDiscountPercent.Value);

                                                GetDiscountPercent1(sShopCd, sManufacturer);
                                                //m_fMarkUp = Math.Round(m_fMarkUp.Value, 1);
                                                //msg2 IS NEVER USED? WHATS THE POINT?
                                                //sprintf(msg2,"%f",m_fMarkUp);

                                                bFound = true;
                                                success = true;

                                                // set part priced.
                                                partsItem.CostLocal = fPartPrice;
                                                partsItem.CostLocal = Math.Round(fPartPrice.Value, 4);

                                                nQtyPerBox = spView.Pieces;

                                                partsItem.CoreValue = (spView.CoreValue == null ? 0 : spView.CoreValue);

                                                if (WorkOrder.ExchangeRate != 0)
                                                {
                                                    partsItem.CoreValue = (partsItem.CoreValue) / WorkOrder.ExchangeRate;
                                                }

                                                sDeductCoreSw = spView.DEDUCT_CORE.ToString();
                                                sCorePartSw = spView.CORE_PART_SW.ToString();

                                                // if deduct core = 'Y' and serial number is empty, fill serial number with "<enter serial number>"
                                                if (sCorePartSw.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    if (string.IsNullOrEmpty(partsItem.SerialNumber))
                                                    {
                                                        // if byPass lease rules not in effect, force core part serial message in part code.
                                                        if (!m_bBypassLease)
                                                        {
                                                            partsItem.SerialNumber = "<enter serial number>";
                                                        }
                                                    }
                                                }

                                                // get core price quantity etc.
                                                // shop markup						rhsRecord.m_fPctMaterialFactor
                                                // manufacturer's discount percent  

                                                partsItem.MslPartSW = spView.MslPartSW;

                                                bMaerskPart = (partsItem.MslPartSW.Equals("Y", StringComparison.CurrentCultureIgnoreCase) ? true : false);
                                                bDeductCore = (sDeductCoreSw.Equals("Y", StringComparison.CurrentCultureIgnoreCase) ? true : false);

                                                decimal dQtyParts = partsItem.Pieces;

                                                if (WorkOrder.Shop.ShopTypeCode.Equals("1", StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    if (nQtyPerBox > 0)
                                                    {
                                                        if ((bMaerskPart) && (bDeductCore))
                                                        {
                                                            partsItem.CostLocal = (((fPartPrice - partsItem.CoreValue) / nQtyPerBox)) * dQtyParts;
                                                            partsItem.CostLocal -= (partsItem.CostLocal * m_fDiscountPercent);
                                                            partsItem.fCostNoMarkup = partsItem.CostLocal;
                                                        }
                                                        if ((bMaerskPart) && (!bDeductCore))
                                                        {
                                                            partsItem.CostLocal = (fPartPrice / nQtyPerBox) * dQtyParts;
                                                            partsItem.CostLocal -= (partsItem.CostLocal * m_fDiscountPercent);
                                                            partsItem.fCostNoMarkup = partsItem.CostLocal;
                                                        }
                                                        if ((!bMaerskPart) && (bDeductCore))
                                                        {
                                                            partsItem.CostLocal = ((fPartPrice - partsItem.CoreValue) / nQtyPerBox);
                                                            partsItem.CostLocal -= (partsItem.CostLocal * m_fDiscountPercent);
                                                            partsItem.fCostNoMarkup = partsItem.CostLocal;

                                                            partsItem.CostLocal *= (decimal)WorkOrder.Shop.PCTMaterialFactor;
                                                            partsItem.CostLocal *= dQtyParts;
                                                            partsItem.fCostNoMarkup *= dQtyParts;
                                                        }
                                                        if ((!bMaerskPart) && (!bDeductCore))
                                                        {
                                                            partsItem.CostLocal = (fPartPrice / nQtyPerBox);
                                                            partsItem.CostLocal -= (partsItem.CostLocal * m_fDiscountPercent);
                                                            partsItem.fCostNoMarkup = partsItem.CostLocal;

                                                            partsItem.CostLocal *= (decimal)WorkOrder.Shop.PCTMaterialFactor;
                                                            partsItem.CostLocal *= dQtyParts;
                                                            partsItem.fCostNoMarkup *= dQtyParts;
                                                        }
                                                    }
                                                }

                                                // part price for type 2 shops
                                                if (WorkOrder.Shop.ShopTypeCode.Equals("2", StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    if (nQtyPerBox > 0)
                                                    {
                                                        if (bDeductCore)
                                                        {
                                                            partsItem.CostLocal = (((fPartPrice - partsItem.CoreValue) / nQtyPerBox) * dQtyParts);
                                                            partsItem.CostLocal -= (partsItem.CostLocal * m_fDiscountPercent);
                                                            partsItem.fCostNoMarkup = partsItem.CostLocal;
                                                        }
                                                        else
                                                        {
                                                            partsItem.CostLocal = (fPartPrice / nQtyPerBox) * dQtyParts;
                                                            partsItem.CostLocal -= (partsItem.CostLocal * m_fDiscountPercent);
                                                            partsItem.fCostNoMarkup = partsItem.CostLocal;
                                                        }
                                                    }
                                                }

                                                // part price for type 3 shops
                                                if (WorkOrder.Shop.ShopTypeCode.Equals("3", StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    decimal? tmpPartCost = 0;
                                                    if (nQtyPerBox > 0)
                                                    {
                                                        if ((bMaerskPart) && (bDeductCore))
                                                        {
                                                            partsItem.CostLocal = ((fPartPrice - partsItem.CoreValue) / nQtyPerBox) * dQtyParts;
                                                            if (!GetDiscountPercent1(sShopCd, sManufacturer))
                                                            {
                                                                tmpPartCost = partsItem.CostLocal * (decimal)WorkOrder.Shop.PCTMaterialFactor;
                                                            }
                                                            else
                                                            {
                                                                if (m_fMarkUp == 0)
                                                                {
                                                                    m_fMarkUp = 1;
                                                                }
                                                                tmpPartCost = partsItem.CostLocal * m_fMarkUp;
                                                            }

                                                            if (tmpPartCost > partsItem.CostLocal)
                                                            {
                                                                partsItem.fCostNoMarkup = partsItem.CostLocal;
                                                            }
                                                            else
                                                            {
                                                                partsItem.fCostNoMarkup = tmpPartCost;
                                                            }
                                                            partsItem.CostLocal = tmpPartCost;
                                                        }

                                                        if ((bMaerskPart) && (!bDeductCore))
                                                        {
                                                            partsItem.CostLocal = (fPartPrice / nQtyPerBox) * dQtyParts;
                                                            if (!GetDiscountPercent1(sShopCd, sManufacturer))
                                                            {
                                                                tmpPartCost = partsItem.CostLocal * (decimal)WorkOrder.Shop.PCTMaterialFactor;
                                                            }
                                                            else
                                                            {
                                                                if (m_fMarkUp == 0)
                                                                {
                                                                    m_fMarkUp = 1;
                                                                }
                                                                tmpPartCost = partsItem.CostLocal * m_fMarkUp;
                                                            }

                                                            if (tmpPartCost > partsItem.CostLocal)
                                                            {
                                                                partsItem.fCostNoMarkup = partsItem.CostLocal;
                                                            }
                                                            else
                                                            {
                                                                partsItem.fCostNoMarkup = tmpPartCost;
                                                            }
                                                            partsItem.CostLocal = tmpPartCost;
                                                        }

                                                        if ((!bMaerskPart) && (bDeductCore))
                                                        {
                                                            partsItem.CostLocal = ((fPartPrice - partsItem.CoreValue) / nQtyPerBox);
                                                            if (!GetDiscountPercent1(sShopCd, sManufacturer))
                                                            {
                                                                tmpPartCost = partsItem.CostLocal * (decimal)WorkOrder.Shop.PCTMaterialFactor;
                                                            }
                                                            else
                                                            {
                                                                if (m_fMarkUp == 0)
                                                                {
                                                                    m_fMarkUp = 1;
                                                                }
                                                                tmpPartCost = partsItem.CostLocal * m_fMarkUp;
                                                            }

                                                            if (tmpPartCost > partsItem.CostLocal)
                                                            {
                                                                partsItem.fCostNoMarkup = partsItem.CostLocal;
                                                            }
                                                            else
                                                            {
                                                                partsItem.fCostNoMarkup = tmpPartCost;
                                                            }
                                                            partsItem.CostLocal = tmpPartCost;
                                                            partsItem.CostLocal *= dQtyParts;
                                                            partsItem.fCostNoMarkup *= dQtyParts;
                                                        }

                                                        if ((!bMaerskPart) && (!bDeductCore))
                                                        {
                                                            partsItem.CostLocal = (fPartPrice / nQtyPerBox);
                                                            if (!GetDiscountPercent1(sShopCd, sManufacturer))
                                                            {
                                                                tmpPartCost = partsItem.CostLocal * (decimal)WorkOrder.Shop.PCTMaterialFactor;
                                                            }
                                                            else
                                                            {
                                                                if (m_fMarkUp == 0)
                                                                {
                                                                    m_fMarkUp = 1;
                                                                }
                                                                tmpPartCost = partsItem.CostLocal * m_fMarkUp;
                                                            }

                                                            if (tmpPartCost > partsItem.CostLocal)
                                                            {
                                                                partsItem.fCostNoMarkup = partsItem.CostLocal;
                                                            }
                                                            else
                                                            {
                                                                partsItem.fCostNoMarkup = tmpPartCost;
                                                            }
                                                            partsItem.CostLocal = tmpPartCost;
                                                            partsItem.CostLocal *= dQtyParts;
                                                            partsItem.fCostNoMarkup *= dQtyParts;
                                                        }
                                                    }
                                                }

                                                //                                        // Set string value of cost for later save to database.
                                                //                                        sprintf(cmsg, "%.2f", pPartsRecord->GetFloatCost() );
                                                //                                        pPartsRecord->SetCost( cmsg );
                                                partsItem.CostLocal = Math.Round(partsItem.CostLocal.Value, 2);

                                                partsItem.CostLocalCPH = partsItem.CostLocal * WorkOrder.ExchangeRate;
                                                partsItem.CostLocal = Math.Round(partsItem.CostLocal.Value, 4);
                                                partsItem.fCostNoMarkup = Math.Round(partsItem.fCostNoMarkup.Value, 2);
                                                partsItem.fCostCPHNoMarkup = partsItem.fCostNoMarkup * WorkOrder.ExchangeRate;
                                                partsItem.PartDescription = spView.PartDescription;
                                                partsItem.CORE_PART_SW = spView.CORE_PART_SW;
                                                partsItem.DEDUCT_CORE = spView.DEDUCT_CORE;

                                                string ManualCode = WorkOrder.Shop.Customer[0].ManualCode;
                                                string Mode = WorkOrder.Mode;
                                                var GetQtyRepairManualMode = (from part in objContext.MESC1TS_RPRCODE_PART
                                                                              where part.REPAIR_CD.Trim() == partsItem.RepairCode.RepairCod.Trim() &&
                                                                              part.MODE == Mode &&
                                                                              part.MANUAL_CD == ManualCode &&
                                                                              part.PART_CD == partsItem.OwnerSuppliedPartsNumber
                                                                              select part).Count();

                                                if (GetQtyRepairManualMode < 1)
                                                {
                                                    Message = new ErrMessage();
                                                    Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + " Part code: " + partsItem.OwnerSuppliedPartsNumber + " is a valid part code, but not valid for repair code: " + partsItem.RepairCode.RepairCod + ". <br>Please recheck and if you judge part is correct - please contact local MSL agency for instruction.";
                                                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                                    errorMessageList.Add(Message);
                                                    success = false;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //throw error
                                        bFound = true;
                                        success = false;
                                        Message = new ErrMessage();
                                        Message.Message = "Part number " + partsItem.OwnerSuppliedPartsNumber + " used with repair code " + partsItem.RepairCode.RepairCod + " does not exist, Please check part number.";
                                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                        errorMessageList.Add(Message);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Message = new ErrMessage();
                            Message.Message = "System Error on validating Parts";
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            errorMessageList.Add(Message);
                            logEntry.Message = ex.ToString();
                            Logger.Write(logEntry);
                            success = false;
                            bFound = true;
                        }
                    }
                }
            }
            logEntry.Message = "Method Name : CheckParts(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool CheckPrepTime(ref WorkOrderDetail WorkOrder, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : CheckPrepTime(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            errorMessageList = new List<ErrMessage>();

            string Mode = WorkOrder.Mode;
            string tmp = string.Empty;
            string tmp1 = string.Empty; ;
            bool m_bPrepAllowed = false;
            double? fNonPrepHoursHold = 0.0;
            double fMaxPrep = 0.0;
            double? fPrepHrs = 0.0;
            int nPrepCtr = 0;
            List<MESC1TS_PREPTIME> RSPrepTime = new List<MESC1TS_PREPTIME>();
            List<MESC1TS_PREPTIME> RSPrepTimePrepCode = null;
            List<MESC1TS_PREPTIME> RSPrepTimeMode = new List<MESC1TS_PREPTIME>();
            RepairsView tempRepair = null;

            try
            {
                RSPrepTime = (from PR in objContext.MESC1TS_PREPTIME
                              //orderby PR.MODE, PR.PREP_CD, PR.PREP_TIME_MAX
                              select PR).OrderBy(pr => pr.MODE).ThenBy(pr => pr.PREP_CD).ThenBy(pr => pr.PREP_TIME_MAX)
                                  .ToList();

                if (RSPrepTime != null && RSPrepTime.Count > 0)
                {
                    foreach (var repairItem in WorkOrder.RepairsViewList)
                    {
                        if (repairItem.rState == (int)Validation.STATE.DELETED)
                        {
                            continue;
                        }
                        // ensure that this isn't a tax repair record
                        if (!IsRepairTaxCode(repairItem.RepairCode))
                        {
                            // check if in prep code table. - if yes
                            // incre ctr, set WO prep hours, mark repair record as a prep-time repair-Cd
                            RSPrepTimePrepCode = new List<MESC1TS_PREPTIME>();
                            RSPrepTimePrepCode = RSPrepTime.FindAll(pCode => pCode.PREP_CD.Trim() == repairItem.RepairCode.RepairCod.Trim() && pCode.MODE == Mode);
                            if (RSPrepTimePrepCode != null && RSPrepTimePrepCode.Count > 0) // ie, record found and therefore is a prep code...
                            {
                                // set WO prep time
                                WorkOrder.TotalPrepHours = repairItem.ManHoursPerPiece;
                                // set repairRec prep sw to true
                                repairItem.RepairCode.PrepTime = true;
                                // increment prep counter - later check if > 1 - only 1 allowed per WO
                                nPrepCtr++;
                            }
                            else // is a non-prep-code repair code, sum hours for non-prep codes. for later prep-code max check
                            {
                                if ((repairItem.RepairCode.RepairInd != null) && !repairItem.RepairCode.RepairInd.Equals("X", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    m_bPrepAllowed = true;
                                    // add hours to calculate prep-time.
                                    //fNonPrepHoursHold+= ((atof( pRecord->GetManHrs().c_str() )) * (atoi( pRecord->GetPieces().c_str() )));
                                    fNonPrepHoursHold += repairItem.ManHoursPerPiece * repairItem.Pieces;
                                }
                                // this is a non-prep-time record.
                                repairItem.RepairCode.PrepTime = false;
                            }
                        }
                    }
                }
                else
                {
                    //strcpy( cmsg, "System error: Failed open of PrepTime recordset - cannot validate PrepTime" );
                    Message = new ErrMessage();
                    Message.Message = "System error: Failed open of PrepTime recordset - cannot validate PrepTime";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    return (false);
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System Error on validating Prep Time - Failed open of PrepTime recordset";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return (false);
            }

            // check prep counter, if > 1 then is error - only one prep code allowed for a WO.
            if (nPrepCtr > 1)
            {
                Message = new ErrMessage();
                Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + ", Only one prep-code allowed per WO";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                return (false);
            }

            // It is possible that 'X' check above allows prep-time, but the shop doesn't, so...
            // Check if SHOP allows prep-time, if not, set switch to false and then flag error if prep-codes entered
            if (!WorkOrder.Shop.PreptimeSW.Equals("Y", StringComparison.CurrentCultureIgnoreCase)) m_bPrepAllowed = false;
            else m_bPrepAllowed = true;

            // If prep-time not allowed, and prep codes entered, is an error.
            if ((!m_bPrepAllowed) && nPrepCtr > 0)
            {
                foreach (var repairItem in WorkOrder.RepairsViewList)
                {
                    if (repairItem.rState == (int)Validation.STATE.DELETED)
                    {
                        continue;
                    }

                    if (!IsRepairTaxCode(repairItem.RepairCode))
                    {
                        if (repairItem.RepairCode.PrepTime)
                        {
                            string errorMsg = GetErrorMessage(20105);
                            Message = new ErrMessage();
                            Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + " " + errorMsg + " " + repairItem.RepairCode.RepairCod;
                            //sprintf(cmsg, "EQPNO: %s, %s: %s", rhsRecord.m_sEQPNO.c_str(), GetErrorMessage( 20105 ).c_str(), pRecord->m_sRepairCd.c_str() );
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            errorMessageList.Add(Message);
                            return false;
                        }
                    }
                }
            }

            // Fix floating point math issue with fNonPrepHoursHold variable. (VJP - 12-16-2004)
            // Limit 'dangling insignificant decimal values'
            double temp = (double)fNonPrepHoursHold;
            fNonPrepHoursHold = Math.Round(temp, 2);

            // UI may no longer be manually entering prep code and hours. so if prep time allowed and no prep code
            // found, then find prep code and add it in.
            // If prep time allowed and prepctr == 0 then get appropriate prep code and distribute
            // hours among st,ot,db, and misc hours.
            // if prep-hours <= reg hours, add prep to regular hours
            // if < reg-hours then, prep-time = prep-time - reg hours, reghours = reg hours * 2;
            // next non zero time, example OT apply same rule and continue with other times till prep-time is used up.
            // add only if repair hours > 0.
            if ((m_bPrepAllowed) && (nPrepCtr == 0) && (fNonPrepHoursHold > 0))
            {
                try
                {
                    //string prepCode = BuildPrepTimeFilter(string.Empty, WorkOrder.Mode);
                    RSPrepTimeMode = RSPrepTime.FindAll(pCode => pCode.MODE == Mode);
                    if (RSPrepTimeMode != null && RSPrepTimeMode.Count > 0)
                    {
                        RSPrepTimeMode = RSPrepTimeMode.OrderBy(p => p.PREP_TIME_MAX).ToList();
                        //pRs->Sort = "PREP_TIME_MAX ASC";
                        foreach (var item in RSPrepTimeMode)
                        {
                            fMaxPrep = item.PREP_TIME_MAX;
                            // If hours are within the maximum allowed hours, then check if the repair-code =
                            // this prep-code.
                            if (fNonPrepHoursHold <= fMaxPrep)
                            {
                                fPrepHrs = (item.PREP_HRS == null ? 0.0 : (double)item.PREP_HRS);

                                // check if prep code already exists as 'deleted' in repair collection
                                // if yes, mark as updated and change values.
                                tempRepair = WorkOrder.RepairsViewList.Find(pCode => pCode.RepairCode.RepairCod.Trim() == item.PREP_CD.Trim());
                                if (tempRepair != null)
                                {
                                    if (tempRepair.rState == (int)Validation.STATE.DELETED)
                                    {
                                        tempRepair.rState = (int)Validation.STATE.UPDATED;
                                    }
                                }
                                else
                                {
                                    // else create a new repair record
                                    // add new repair code with this prep code.
                                    // Add to repair list
                                    tempRepair = new RepairsView();
                                    WorkOrder.RepairsViewList.Add(tempRepair);
                                }
                                tempRepair.RepairCode = new RepairCode();
                                tempRepair.RepairCode.RepairCod = item.PREP_CD;
                                //find the rep desc
                                var desc = (from rep in objContext.MESC1TS_REPAIR_CODE
                                            where rep.REPAIR_CD == tempRepair.RepairCode.RepairCod &&
                                            rep.MODE == Mode
                                            select new
                                            {
                                                rep.REPAIR_DESC
                                            }).FirstOrDefault();

                                tempRepair.RepairCode.RepairDesc = desc.REPAIR_DESC;
                                //float tempPrepHrs = (float)fPrepHrs;
                                tempRepair.ManHoursPerPiece = Math.Round(fPrepHrs.Value, 2);
                                tempRepair.Pieces = 1;
                                tempRepair.MaterialCost = 0;
                                tempRepair.MaterialCostCPH = 0;
                                tempRepair.RepairCode.PrepTime = true;
                                tempRepair.Damage = new Damage();
                                tempRepair.Damage.DamageCedexCode = "OU";
                                tempRepair.RepairLocationCode = new RepairLoc();
                                tempRepair.RepairLocationCode.CedexCode = "XXNN";
                                tempRepair.Tpi = new Tpi();
                                tempRepair.Tpi.CedexCode = "O";
                                //tempRepair.RepairCode

                                // distribute hours among hourly types. Since it may not be added in from the UI
                                DistributePrepTime(WorkOrder, fPrepHrs);

                                //// set Total prep hours on wo
                                //rhsRecord.m_sTOT_PREP_HRS = pRecord->m_sManHrs;
                                WorkOrder.TotalPrepHours = tempRepair.ManHoursPerPiece;

                                //// set manual and mode for GetDescription call.for UI
                                //pRecord->m_sManualCd = rhsRecord.m_sMANUAL_CD;
                                //pRecord->m_sWOMode = rhsRecord.m_sMODE;


                                // record found, so exit loop to avoid getting subsequent prep codes with greater prep time hours.
                                // Set prep counter to 1 to force check
                                nPrepCtr = 1;
                                break;
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    Message = new ErrMessage();
                    Message.Message = "System Error on validating Prep Time (System Add of Prep record)";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    return false;
                }

            }

            // New logic - if prep-time allowed, Then get prep record - compare it to a prep table filtered by mode
            // and sorted by MAx Prep hours. First record found where fNonPrepHours
            try
            {
                if ((m_bPrepAllowed) && (nPrepCtr == 1))
                {
                    // Locate the actual prep-code record
                    foreach (var rItem in WorkOrder.RepairsViewList)
                    {
                        if (rItem.rState == (int)Validation.STATE.DELETED)
                        {
                            continue;
                        }
                        if (!IsRepairTaxCode(rItem.RepairCode))
                        {
                            if (rItem.RepairCode.PrepTime) break;
                        }

                        // get list of prep codes by mode only.
                        //string prepCode = BuildPrepTimeFilter(string.Empty, WorkOrder.Mode);
                        RSPrepTimeMode = RSPrepTime.FindAll(pCode => pCode.MODE == Mode);
                        if (RSPrepTimeMode == null && RSPrepTimeMode.Count > 0)
                        {
                            // Sort by prep-time max.
                            //pRs->Sort = "PREP_TIME_MAX ASC";
                            RSPrepTimeMode = RSPrepTimeMode.OrderBy(p => p.PREP_TIME_MAX).ToList();
                            foreach (var item in RSPrepTimeMode)
                            {
                                fMaxPrep = item.PREP_TIME_MAX; // got prep-code: the hours are within maximum allowed
                                // If hours are within the maximum allowed hours, then check if the repair-code =
                                // this prep-code.
                                if (fNonPrepHoursHold <= fMaxPrep) // got prep-code: the hours are within maximum allowed
                                {
                                    // compare prep-code to repair-code, should be the same, else is error
                                    tmp = item.PREP_CD;
                                    tmp = tmp.TrimEnd();
                                    tmp1 = rItem.RepairCode.RepairCod.Trim();
                                    tmp1 = tmp1.TrimEnd();
                                    if (tmp.Equals(tmp1, StringComparison.CurrentCultureIgnoreCase)) // ie. codes match, then is OK, break
                                    {
                                        fPrepHrs = (item.PREP_HRS == null ? 0.0 : (double)item.PREP_HRS);
                                        if (rItem.ManHoursPerPiece > fPrepHrs)
                                        {
                                            //sprintf(cmsg, "EQPNO: %s, Prep-time hours exceed maximum prep-time hours.: %s.", rhsRecord.m_sEQPNO.c_str(), pRecord->m_sRepairCd.c_str() );
                                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                            errorMessageList.Add(Message);
                                        }
                                        break;
                                    }
                                }
                                else // Got the permitted prep-code and it doesn't match what was entered - error.
                                {
                                    //"EQPNO: %s, Incorrect Prep-code entered: %s.", rhsRecord.m_sEQPNO.c_str(), pRecord->m_sRepairCd.c_str() );
                                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                    errorMessageList.Add(Message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "System Error on validating Prep Time (compare preptime record)";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return false;
            }

            // Total hours only after we have determined that prep-time is OK. because Prep-hours cannot be included
            // in determining which prep-code can be used as done in above step....
            // Sum total man hours - needed to check sum of all hours on WO header, ie reg,dbl,misc and 1/2 hours
            // sum total material amount for all repairs.
            WorkOrder.TotalRepairManHour = 0;
            foreach (var rItem in WorkOrder.RepairsViewList)
            {
                if (rItem.rState == (int)Validation.STATE.DELETED)
                {
                    continue;
                }
                if (!IsRepairTaxCode(rItem.RepairCode))
                {
                    try
                    {
                        // Total repair Man hours is man hours times the repair record piece count.
                        WorkOrder.TotalRepairManHour += (rItem.ManHoursPerPiece * rItem.Pieces);
                        //rhsRecord.m_fTotRepairManHrs += ((atof(pRecord->GetManHrs()) * (atoi(pRecord->GetPieces()));
                        // CHANGED 2-10-04 - VJP
                        //	m_fTotRepairMaterialAmt+= atof( pRecord->GetMaterialAmt().c_str() );
                        m_fTotRepairMaterialAmt += (rItem.MaterialCost * rItem.Pieces);
                    }

                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "System Error on validating Prep Time (compare preptime record)";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        errorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        return false;
                    }
                }

            }

            //no difference in total repair hours and total labor hours
            // calculations are identical.  Not sure why 2 values in db.
            WorkOrder.TotalRepairManHour = Math.Round(WorkOrder.TotalRepairManHour.Value, 2);
            WorkOrder.TotalLaborHours = WorkOrder.TotalRepairManHour; //, 2);
            //double tempAmt = (double)m_fTotRepairMaterialAmt;
            m_fTotRepairMaterialAmt = Math.Round(m_fTotRepairMaterialAmt.Value, 4);
            WorkOrder.TotalShopAmount = m_fTotRepairMaterialAmt;
            logEntry.Message = "Method Name : CheckPrepTime(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool CheckMultiRepairSwitch(ref WorkOrderDetail WorkOrder, out List<ErrMessage> errorMessageList)
        {
            bool success = true;
            errorMessageList = new List<ErrMessage>();
            string ManualCode = WorkOrder.Shop.Customer[0].ManualCode;

            if (WorkOrder.IsSingle) return true;

            foreach (var item in WorkOrder.RepairsViewList)
            {
                if (!IsRepairTaxCode(item.RepairCode) && !item.RepairCode.PrepTime)
                {
                    string mode = WorkOrder.EquipmentList[0].SelectedMode;
                    var GetRepairActiveSW = (from rep in objContext.MESC1TS_REPAIR_CODE
                                             where rep.REPAIR_CD == item.RepairCode.RepairCod.Trim() &&
                                             rep.MANUAL_CD == ManualCode &&
                                             rep.MODE == mode
                                             select new { rep.MULTIPLE_UPDATE_SW }).FirstOrDefault();

                    if (GetRepairActiveSW != null)// && GetRepairActiveSW.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(GetRepairActiveSW.MULTIPLE_UPDATE_SW) && !GetRepairActiveSW.MULTIPLE_UPDATE_SW.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                        {
                            Message = new ErrMessage();
                            Message.Message = "Repair record not valid for multiple estimate entry: Repair code: " + item.RepairCode.RepairCod;
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            errorMessageList.Add(Message);
                        }
                    }
                }
            }

            return success;
        }

        // Check if part/repair is on the repairCode/part table with a maximum qty value
        /// <summary>
        /// 
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool CheckMaxPart(ref WorkOrderDetail WO, out List<ErrMessage> errorMessageList)
        {
            errorMessageList = new List<ErrMessage>();
            bool bOK = true;
            int? nQty = 0;

            foreach (var repV in WO.RepairsViewList)
            {
                if ((!IsRepairTaxCode(repV.RepairCode)) && (!repV.RepairCode.PrepTime))
                {
                    if (WO.SparePartsViewList != null && WO.SparePartsViewList.Count > 0)
                    {
                        List<SparePartsView> spvList = WO.SparePartsViewList.FindAll(a => a.RepairCode.RepairCod.Trim() == repV.RepairCode.RepairCod.Trim());
                        if (spvList != null)
                        {
                            // iterate through part collection - check qty for each.
                            foreach (var spv in spvList)
                            {
                                try
                                {
                                    nQty = GetMaxPartQty(spv.RepairCode.RepairCod, WO.Shop.Customer[0].ManualCode, WO.EquipmentList[0].SelectedMode, spv.OwnerSuppliedPartsNumber);
                                    // if > 0 then check against part qty entered.
                                    if (nQty > 0)
                                    {
                                        if (spv.Pieces > nQty)
                                        {
                                            // format error message if part qty entered > amount allowed.
                                            Message = new ErrMessage();
                                            Message.Message = "EQPNO: " + WO.EquipmentList[0].EquipmentNo + ", Part Code: " + spv.OwnerSuppliedPartsNumber + ". Maximum part quantity allowed is: " + nQty + ".";
                                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                            errorMessageList.Add(Message);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Message = new ErrMessage();
                                    Message.Message = "System Error on validating Parts";
                                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                    errorMessageList.Add(Message);
                                    logEntry.Message = ex.ToString();
                                    Logger.Write(logEntry);
                                    bOK = false;
                                }

                                //ROHIT : logging task below
                                //}
                                //catch(Exception ex)( _com_error &e )	// catch(Exception ex) ADO data errors.
                                //{
                                //    strcpy( cmsg, ": m_pPart->GetMaxPartQty: Failed read of RPRCODE_PART table" );
                                //    strcat( cmsg, e.ErrorMessage() );
                                //    pLog.CreateInstance(__uuidof(SystemErrorManager));
                                //    pLog->WriteApplicationLog(PROGRAMNAME, cmsg);
                                //    pLog = NULL;
                                //    rhsRecord.m_aErrorList.Insert("System Error on validating Parts");
                                //    bOK=false;
                                //}
                                //catch(Exception ex)(...)
                                //{
                                //    strcpy( cmsg, "m_pPart->GetMaxPartQty: Failed read of RPRCODE_PART table" );
                                //    pLog.CreateInstance(__uuidof(SystemErrorManager));
                                //    pLog->WriteApplicationLog(PROGRAMNAME, cmsg);
                                //    pLog = NULL;
                                //    rhsRecord.m_aErrorList.Insert("System Error on validating Parts");
                                //    bOK= false;
                                //}
                            }
                        }
                    }
                }
            }
            return bOK;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkOrderDetail"></param>
        /// <param name="errorMessageList"></param>
        private void PerformCalculations(ref WorkOrderDetail WorkOrderDetail, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : PerformCalculations(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            // #16 CALCULATIONS:
            // LEGEND: 
            // Total Shop Amount = total cost of materials ( sum of all repair record material amounts )
            // Total Repair Man Hours ( sum of all repair record 'ManHrs' )
            // Total Labor Cost = sum of each hour type(reg,dbl,misc etc) multiplied by associated rate type.
            // Total Cost Local = Sum of Total Shop Amount + Total Labor Cost (Also known as TotEDIAmt - passed in from EDI batch)

            // Total Cost Local  m_sTotCostLocal
            //	add total shop amount(Total material amount to Total Labor Cost

            // - Note: all values for DB are stored as strings, so conversions to doubles/ints etc. necessary for calculations

            // Get Total Labor Cost.
            double? TotalSum = 0;
            double? TotalSumCPH = 0;
            double tmp = 0.0;
            errorMessageList = new List<ErrMessage>();
            ErrMessage Message = new ErrMessage();
            List<RemarkEntry> RemarksEntryList = new List<RemarkEntry>();
            WorkOrderDetail.AgentPartsTax = 0;
            WorkOrderDetail.AgentPartsTaxCPH = 0;
            string LaborRate = WorkOrderDetail.Shop.LaborRate[0];
            string COType = WorkOrderDetail.EquipmentList[0].COType;

            //Convert.ToDouble(Model.dbWOD.TotalCostLocal) - dImportTax - Convert.ToDouble(Model.dbWOD.SalesTaxParts) - Convert.ToDouble(Model.dbWOD.SalesTaxLabour);
            try
            {
                //WorkOrderDetail.TotalManHourReg = 2.0;
                //WorkOrderDetail.TotalManHourOverTime = 1.1;
                //WorkOrderDetail.TotalManHourDoubleTime = 2.3;
                //WorkOrderDetail.TotalManHourMisc = 1.6;
                // Check if total RepairManHrs = sum of WO hours.
                TotalSum = (WorkOrderDetail.TotalManHourReg +
                            WorkOrderDetail.TotalManHourOverTime +
                            WorkOrderDetail.TotalManHourDoubleTime +
                            WorkOrderDetail.TotalManHourMisc);

                // Is necessary for Web side not to pass any hours. If sum of hours = 0 then set RegHours to TotRepair Hours
                // ie.  Calculations already performed for Web pgm.

                if (TotalSum == 0.00) // ie no hours entered, then assign them.
                {
                    WorkOrderDetail.TotalManHourReg = WorkOrderDetail.TotalRepairManHour; // set regular hours to calculated total repair hours
                    TotalSum = WorkOrderDetail.TotalRepairManHour; // re-set fTotSum
                }


                // Hold off till client pays... ?  Re-implemented till further notice. - VJP
                // Check rates entered Vs. Rates from DB. If rates entered and rate is null, then flag errors.
                if (((WorkOrderDetail.TotalManHourReg) > 0) && m_bNULLRegHours)
                {
                    Message = new ErrMessage();
                    Message.Message = " Rate schedule for ordinary hours is invalid."; // Abu - Add Equipment No. to Message
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }


                if (((WorkOrderDetail.TotalManHourOverTime) > 0) && m_bNULLOTHours)
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + WorkOrderDetail.EquipmentList[0].EquipmentNo + "  Rate schedule for overtime 1 hours is invalid."; // Abu - Add Equipment No. to Message
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }


                if (((WorkOrderDetail.TotalManHourDoubleTime) > 0) && m_bNULLDblHours)
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + WorkOrderDetail.EquipmentList[0].EquipmentNo + " Rate schedule for overtime 2 hours is invalid. "; // Abu - Add Equipment No. to Message
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }

                if (((WorkOrderDetail.TotalManHourMisc) > 0) && m_bNULLMiscHours)
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + WorkOrderDetail.EquipmentList[0].EquipmentNo + " Rate schedule for overtime 3 hours is invalid."; // Abu - Add Equipment No. to Message
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);

                }

                // Check Overtime suspend swith

                if (WorkOrderDetail.Shop.OvertimeSuspSW.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                {

                    double? dOT = (WorkOrderDetail.TotalManHourOverTime +
                                   WorkOrderDetail.TotalManHourDoubleTime +
                                   WorkOrderDetail.TotalManHourMisc);


                    if (dOT > 0)
                    {

                        RemarksEntry.Remark = DateTime.Now + "S Suspended due to use of overtime"; // Abu - Check logic for RemarkEntry. 'S' signifies special remark
                        RemarksEntryList.Add(RemarksEntry); // Abu - Add DateTime.Now() to the list

                    }

                }

                // Calculate Total Labor Cost
                // sum of each hour type Multiplied by associated charge rate.
                // BUT: if non-standard work order, expect no labor rates in table. so. should be zero
                // so, totalCostLocal will be sum of Repair Material Amounts only.

                if (WorkOrderDetail.WorkOrderType == "N")
                {
                    TotalSum = 0.00;
                    TotalSumCPH = 0.00;
                }
                else
                {

                    TotalSum = (WorkOrderDetail.TotalManHourReg * (double)LaborRate.RegularRT) +
                                         (WorkOrderDetail.TotalManHourOverTime * (double)LaborRate.OvertimeRT) +
                                         (WorkOrderDetail.TotalManHourDoubleTime * (double)LaborRate.DoubleTimeRT) +
                                         (WorkOrderDetail.TotalManHourMisc * (double)LaborRate.MiscRT);

                    TotalSumCPH = (WorkOrderDetail.TotalManHourReg * (double)LaborRate.RegularRTCPH) +
                                         (WorkOrderDetail.TotalManHourOverTime * (double)LaborRate.OvertimeRTCPH) +
                                         (WorkOrderDetail.TotalManHourDoubleTime * (double)LaborRate.DoubleTimeRTCPH) +
                                         (WorkOrderDetail.TotalManHourMisc * (double)LaborRate.MiscRTCPH);
                }

                //This taking of a temp double variable is necessary since math.round do not take nullable doubles.
                double temp = (double)TotalSum;
                temp = Math.Round(temp, 2);
                WorkOrderDetail.TotalLabourCost = (decimal)temp;

                temp = (double)TotalSumCPH;
                temp = Math.Round(temp, 4);
                WorkOrderDetail.TotalLabourCostCPH = (decimal)temp;

                /*  
                I told you wrong regarding the Sales Tax calculation 
                when I said COTYPE = ‘GENS’  
                only  use  the SALES_TAX_LABOR_GEN and SALES_TAX_PARTS_GEN.   
                You also need to include COTYPE = ‘CHAS’   
                in that formula as well -  so ‘CHAS’ and ‘GENS’  
                use the sales tax rates  ending with the underscore GENS, 
                while all other types use the underscore CONS.
                */

                // Labor Sales Tax:  Local and CPH 


                if ((COType.Equals(Equipment.CONTAINERTYPE.GENS.ToString(), StringComparison.InvariantCultureIgnoreCase)) || (COType.Equals(Equipment.CONTAINERTYPE.CHAS.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    if (WorkOrderDetail.Shop.SalesTaxLaborGen > 0)
                    {
                        WorkOrderDetail.SalesTaxLabour = (WorkOrderDetail.TotalLabourCost * (decimal)WorkOrderDetail.Shop.SalesTaxLaborGen);
                        WorkOrderDetail.SalesTaxLabourCPH = (WorkOrderDetail.TotalLabourCostCPH * (decimal)WorkOrderDetail.Shop.SalesTaxLaborGen);
                    }

                }
                else
                {
                    if (WorkOrderDetail.Shop.SalesTaxLaborCon > 0)
                    {

                        WorkOrderDetail.SalesTaxLabour = (WorkOrderDetail.TotalLabourCost * (decimal)WorkOrderDetail.Shop.SalesTaxLaborCon);
                        WorkOrderDetail.SalesTaxLabourCPH = (WorkOrderDetail.TotalLabourCostCPH * (decimal)WorkOrderDetail.Shop.SalesTaxLaborCon);

                    }

                }

                // TOT_SHOP_AMT and CPH amount - Sum of SHOP_MATERIAL_AMT for all repair records.

                decimal? MaterialAmt = 0;
                decimal? MaterialAmtCPH = 0;
                m_fTotRepairMaterialAmt = 0;
                m_fTotRepairMaterialAmtCPH = 0;
                m_fTotWMaterialAmt = 0;
                m_fTotWMaterialAmtCPH = 0;
                m_fTotTMaterialAmt = 0;
                m_fTotTMaterialAmtCPH = 0;
                decimal? TotTMaerskParts = 0;
                decimal? TotTMaerskPartsCPH = 0;
                decimal? TotWMaerskParts = 0;
                decimal? TotWMaerskPartsCPH = 0;
                decimal? TManHrs = 0;
                decimal? WManHrs = 0;

                foreach (RepairsView repView in WorkOrderDetail.RepairsViewList)
                {
                    if (repView.rState == (int)Validation.STATE.DELETED) continue;

                    // do not total tax records
                    if (!IsRepairTaxCode(repView.RepairCode)) // Abu - Need to verify IsRepairTaxCode()
                    {

                        MaterialAmt = (repView.MaterialCost);
                        MaterialAmtCPH = (repView.MaterialCostCPH);
                        m_fTotRepairMaterialAmt += MaterialAmt * repView.Pieces;
                        m_fTotRepairMaterialAmtCPH += MaterialAmtCPH * repView.Pieces;

                        if (repView.Tpi != null)
                            repView.Tpi.category = GetCategory(repView.Tpi.CedexCode); //Abu - Need to verify GetCategory()
                        else
                        {
                            repView.Tpi = new Tpi();
                            repView.Tpi.category = GetCategory(repView.Tpi.CedexCode); //Abu - Need to verify GetCategory()
                        }

                        if (repView.Tpi.category != null)
                        {
                            if (repView.Tpi.category.Equals("W", StringComparison.InvariantCultureIgnoreCase))
                            {
                                m_fTotWMaterialAmt += MaterialAmt * repView.Pieces;
                                m_fTotWMaterialAmtCPH += MaterialAmtCPH * repView.Pieces;
                                WManHrs += ((decimal)repView.ManHoursPerPiece) * repView.Pieces;

                            }

                            if (repView.Tpi.category.Equals("T", StringComparison.InvariantCultureIgnoreCase))
                            {
                                m_fTotTMaterialAmt += MaterialAmt * repView.Pieces;
                                m_fTotTMaterialAmtCPH += MaterialAmtCPH * repView.Pieces;
                                TManHrs += ((decimal)repView.ManHoursPerPiece) * repView.Pieces;

                            }
                        }
                    }
                }

                // finally put sums in WO

                //WorkOrderDetail.TotalShopAmount = (decimal)m_fTotRepairMaterialAmt;
                WorkOrderDetail.TotalShopAmount = Math.Round(m_fTotRepairMaterialAmt.Value, 4);
                WorkOrderDetail.TotalShopAmountCPH = Math.Round(m_fTotRepairMaterialAmtCPH.Value, 4);


                // Total Maersk Parts and Man parts - iterate through all parts for WO.
                //Shop Types (m_sShopTypeCd) 1 & 3 - Sum local-cost if MSLPart == 'M'
                // sum all for shop type 2

                decimal? fTotMaerskParts = 0;
                //decimal? 
                decimal? fTotMaerskPartsCPH = 0;
                decimal? fTotMaerskPartsTaxable = 0;
                decimal? fTotMaerskPartsTaxableCPH = 0;

                // total part costs.
                decimal? fTotManParts = 0;
                decimal? fTotManPartsCPH = 0;

                // no shop markup percent values - for calculating import tax.
                decimal? fTotManPartsNoMarkup = 0;
                decimal? fTotManPartsCPHNoMarkup = 0;

                decimal? fTotManPartsTaxable = 0;
                decimal? fTotManPartsTaxableCPH = 0;
                decimal? fTotShopMaterialAmtTaxable = 0;
                decimal? fTotShopMaterialAmtTaxableCPH = 0;

                int ShopTypeCode = Convert.ToInt32(WorkOrderDetail.Shop.ShopTypeCode);

                foreach (RepairsView rpView in WorkOrderDetail.RepairsViewList)
                {
                    if (rpView.rState == (int)Validation.STATE.DELETED) continue;

                    if (rpView.RepairCode.TaxAppliedSW == "Y")
                    {
                        fTotShopMaterialAmtTaxable += (rpView.MaterialCost) * rpView.Pieces;
                        fTotShopMaterialAmtTaxableCPH += rpView.MaterialCostCPH * rpView.Pieces;
                    }
                }

                if (WorkOrderDetail.SparePartsViewList != null && WorkOrderDetail.SparePartsViewList.Count > 0)
                {
                    foreach (SparePartsView spView in WorkOrderDetail.SparePartsViewList)
                    {
                        //find the corresponding RPCode
                        RepairsView rpView1 = WorkOrderDetail.RepairsViewList.Find(cd => spView.RepairCode.RepairCod == cd.RepairCode.RepairCod);
                        if (spView.pState == (int)Validation.STATE.DELETED) continue;

                        if (ShopTypeCode == 2)
                        {
                            fTotMaerskParts += spView.CostLocal;
                            fTotMaerskPartsCPH += spView.CostLocalCPH;
                        }
                        else
                        {
                            if (spView.MslPartSW != null && spView.MslPartSW.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                            {
                                fTotMaerskParts += spView.CostLocal;
                                fTotMaerskPartsCPH += spView.CostLocalCPH;

                                if (rpView1.RepairCode.TaxAppliedSW.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    fTotMaerskPartsTaxable += spView.CostLocal;
                                    fTotMaerskPartsTaxableCPH += spView.CostLocalCPH;
                                }
                            }
                            else
                            {
                                fTotManParts += spView.CostLocal;
                                fTotManPartsCPH += spView.CostLocalCPH;
                                fTotManPartsNoMarkup += spView.fCostNoMarkup;
                                fTotManPartsCPHNoMarkup += spView.fCostCPHNoMarkup;

                                // sum taxable man parts

                                if (rpView1.RepairCode.TaxAppliedSW.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    fTotManPartsTaxable += spView.CostLocal;
                                    fTotManPartsTaxableCPH += spView.CostLocalCPH;
                                }
                            }
                        }

                        if (string.Equals(rpView1.Tpi.category, "W", StringComparison.InvariantCultureIgnoreCase) == true)
                        {
                            TotWMaerskParts += spView.CostLocal;
                            TotWMaerskPartsCPH += spView.CostLocalCPH;
                        }

                        if (string.Equals(rpView1.Tpi.category, "T", StringComparison.InvariantCultureIgnoreCase) == true)
                        {
                            TotTMaerskParts += spView.CostLocal;
                            TotTMaerskPartsCPH += spView.CostLocalCPH;
                        }
                    }
                }
                //}

                // set record fields with derived values.

                WorkOrderDetail.TotalMaerksParts = fTotMaerskParts;
                WorkOrderDetail.TotalMaerksPartsCPH = fTotMaerskPartsCPH;
                WorkOrderDetail.TotalManParts = fTotManParts;
                WorkOrderDetail.TotalManPartsCPH = fTotManPartsCPH;

                decimal? fTotLabourSum = WorkOrderDetail.ManHourRate;
                decimal? fTotLabourSumCPH = WorkOrderDetail.ManHourRateCPH;
                decimal? m_fTotWMaterialAmt1 = m_fTotWMaterialAmt + TotWMaerskParts + (WManHrs * fTotLabourSum);
                decimal? m_fTotTMaterialAmt1 = m_fTotTMaterialAmt + TotTMaerskParts + (TManHrs * fTotLabourSum);
                decimal? m_fTotWMaterialAmtCPH1 = m_fTotWMaterialAmtCPH + TotWMaerskPartsCPH + (WManHrs * fTotLabourSumCPH);
                decimal? m_fTotTMaterialAmtCPH1 = m_fTotTMaterialAmtCPH + TotTMaerskPartsCPH + (TManHrs * fTotLabourSumCPH);

                WorkOrderDetail.TotalWMaterialAmount = m_fTotWMaterialAmt1;
                WorkOrderDetail.TotalWMaterialAmountCPH = m_fTotWMaterialAmtCPH1;
                WorkOrderDetail.TotalTMaterialAmount = m_fTotTMaterialAmt1;
                WorkOrderDetail.TotalTMaterialAmountCPH = m_fTotTMaterialAmtCPH1;

                WorkOrderDetail.TotalWMaterialAmountUSD = (m_fTotWMaterialAmt1) * WorkOrderDetail.ExchangeRate;
                WorkOrderDetail.TotalWMaterialAmountCPHUSD = (m_fTotWMaterialAmt1) * WorkOrderDetail.ExchangeRate; //Abu - This looks incorrect as it seems it should be m_fTotWMaterialAmtCPH1. This looks like a bug in the original code itself. Need to verify with Ross.
                WorkOrderDetail.TotalTMaterialAmountUSD = (m_fTotTMaterialAmt1) * WorkOrderDetail.ExchangeRate;
                WorkOrderDetail.TotalTMaterialAmountCPHUSD = m_fTotTMaterialAmt1 * WorkOrderDetail.ExchangeRate; //Abu - This looks incorrect as it seems it should be m_fTotTMaterialAmtCPH1. This looks like a bug in the original code itself. Need to verify with Ross.


                // NOTE: import tax gets applied to total parts costs excluding the shop markup percent
                // These part record fields are totaled at the same time the part cost fields are calculated

                // Import tax  
                //(Tot manufacturer parts (excluding shop markup percent) * import tax) + Tot maersk parts * import tax)
                // if Import tax == 0 - possible that import tax was manually filled in from UI
                if (WorkOrderDetail.Shop.ImportTax > 0)
                {
                    WorkOrderDetail.ImportTax = ((decimal)WorkOrderDetail.Shop.ImportTax) * fTotManPartsNoMarkup;
                    WorkOrderDetail.ImportTax = Math.Round(WorkOrderDetail.ImportTax.Value, 2);
                    WorkOrderDetail.ImportTaxCPH = WorkOrderDetail.ImportTax * WorkOrderDetail.ExchangeRate;
                    WorkOrderDetail.ImportTaxCPH = Math.Round(WorkOrderDetail.ImportTaxCPH.Value, 4);
                }

                // Sales Tax Parts
                // need to hold totals for each man and maers parts tax dollars
                // (Tot Parts taxable + Maersk Parts Taxable) * Sales tax parts

                double? fSalesTaxMslPartsCPH = 0.0;
                double? fSalesTaxMslParts = 0.0;
                double? fSalesTaxManParts = 0.0;

                if ((COType.Equals(Equipment.CONTAINERTYPE.GENS.ToString(), StringComparison.InvariantCultureIgnoreCase)) || (COType.Equals(Equipment.CONTAINERTYPE.CHAS.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (WorkOrderDetail.Shop.SalesTaxPartGen > 0)
                    {
                        WorkOrderDetail.SalesTaxParts = ((decimal)(fTotShopMaterialAmtTaxable) + (decimal)(fTotManPartsTaxable) + WorkOrderDetail.ImportTax) * (decimal)(WorkOrderDetail.Shop.SalesTaxPartGen);
                        WorkOrderDetail.SalesTaxParts = Math.Round(WorkOrderDetail.SalesTaxParts.Value, 2);
                        WorkOrderDetail.AgentPartsTax = 0;

                        WorkOrderDetail.SalesTaxPartsCPH = WorkOrderDetail.SalesTaxParts * WorkOrderDetail.ExchangeRate;
                        WorkOrderDetail.SalesTaxPartsCPH = Math.Round(WorkOrderDetail.SalesTaxPartsCPH.Value, 4);
                        WorkOrderDetail.AgentPartsTaxCPH = 0;

                        // Total tax on MSL parts CPH: needed for total cost CPH calculation	
                        fSalesTaxMslPartsCPH = ((double)fTotManPartsTaxableCPH) * WorkOrderDetail.Shop.SalesTaxPartGen;
                        fSalesTaxMslParts = ((double)fTotMaerskParts) * WorkOrderDetail.Shop.SalesTaxPartGen;
                        fSalesTaxManParts = ((double)fTotManPartsTaxable) * WorkOrderDetail.Shop.SalesTaxPartGen;
                    }
                    else
                    {
                        // May not be the case - i.e., UI manually entered dollar amount
                        fSalesTaxMslPartsCPH = (double)(WorkOrderDetail.SalesTaxPartsCPH);
                        fSalesTaxMslParts = (double)(WorkOrderDetail.SalesTaxParts);
                    }
                }
                else
                {
                    if (WorkOrderDetail.Shop.SalesTaxPartCont > 0)
                    {
                        WorkOrderDetail.SalesTaxParts = ((decimal)(fTotShopMaterialAmtTaxable) + (decimal)(fTotManPartsTaxable) + WorkOrderDetail.ImportTax) * (decimal)(WorkOrderDetail.Shop.SalesTaxPartCont);
                        WorkOrderDetail.SalesTaxParts = Math.Round(WorkOrderDetail.SalesTaxParts.Value, 2);
                        WorkOrderDetail.AgentPartsTax = 0;

                        WorkOrderDetail.SalesTaxPartsCPH = WorkOrderDetail.SalesTaxParts * WorkOrderDetail.ExchangeRate;
                        WorkOrderDetail.SalesTaxPartsCPH = Math.Round(WorkOrderDetail.SalesTaxPartsCPH.Value, 4);
                        WorkOrderDetail.AgentPartsTaxCPH = 0;

                        // Total tax on MSL parts CPH: needed for total cost CPH calculation
                        fSalesTaxMslPartsCPH = ((double)fTotManPartsTaxableCPH) * WorkOrderDetail.Shop.SalesTaxPartCont;
                        fSalesTaxMslParts = ((double)fTotMaerskParts) * WorkOrderDetail.Shop.SalesTaxPartCont;
                        fSalesTaxManParts = ((double)fTotManPartsTaxable) * WorkOrderDetail.Shop.SalesTaxPartCont;
                    }
                    else
                    {
                        // May not be the case - i.e., UI manually entered dollar amount
                        fSalesTaxMslPartsCPH = (double)(WorkOrderDetail.SalesTaxPartsCPH);
                        fSalesTaxMslParts = (double)(WorkOrderDetail.SalesTaxParts);
                    }
                }


                // derived IMPORT_MAN_TAX needed for TOT_COST_LOCAL
                double? dImportManTax = ((double)fTotManPartsTaxable) * WorkOrderDetail.Shop.ImportTax;
                //double? fTotCost;
                decimal? fTotCost = 0;

                //fTotCost = (WorkOrderDetail.TotalLabourCost +
                //            (double)WorkOrderDetail.TotalShopAmount +
                //            WorkOrderDetail.TotalManParts +
                //            (double)(WorkOrderDetail.ImportTax) +
                //            (double)(WorkOrderDetail.SalesTaxParts) +
                //            (double)(WorkOrderDetail.SalesTaxLabour));

                fTotCost = (WorkOrderDetail.TotalLabourCost +
                            WorkOrderDetail.TotalShopAmount +
                            WorkOrderDetail.TotalManParts +
                            (WorkOrderDetail.ImportTax) +
                            (WorkOrderDetail.SalesTaxParts) +
                            (WorkOrderDetail.SalesTaxLabour));

                WorkOrderDetail.TotalCostLocal = Math.Round(fTotCost.Value, 2);

                fTotCost = (WorkOrderDetail.TotalLabourCostCPH +
                            WorkOrderDetail.TotalShopAmountCPH +
                            WorkOrderDetail.TotalManPartsCPH +
                            (WorkOrderDetail.ImportTaxCPH) +
                            (WorkOrderDetail.SalesTaxPartsCPH) +
                            (WorkOrderDetail.SalesTaxLabourCPH) +
                            WorkOrderDetail.AgentPartsTaxCPH);

                WorkOrderDetail.TotalCostCPH = Math.Round(fTotCost.Value, 4);

                fTotCost = (WorkOrderDetail.TotalLabourCost + //* (double)WorkOrderDetail.ExchangeRate +
                            WorkOrderDetail.TotalShopAmount + //* (double)WorkOrderDetail.ExchangeRate +
                            WorkOrderDetail.TotalManPartsCPH + //* (double)WorkOrderDetail.ExchangeRate +
                            WorkOrderDetail.ImportTax + //* (double)WorkOrderDetail.ExchangeRate +
                            WorkOrderDetail.SalesTaxParts + //* (double)WorkOrderDetail.ExchangeRate +
                            WorkOrderDetail.SalesTaxLabour); //* (double)WorkOrderDetail.ExchangeRate);

                fTotCost *= WorkOrderDetail.ExchangeRate;
                WorkOrderDetail.TotalCostLocalUSD = Math.Round(fTotCost.Value, 4);

                fTotCost = (WorkOrderDetail.TotalLabourCost +
                            WorkOrderDetail.TotalShopAmount +
                            WorkOrderDetail.TotalManParts +
                            WorkOrderDetail.ImportTax +
                            WorkOrderDetail.SalesTaxParts +
                            WorkOrderDetail.SalesTaxLabour +
                            WorkOrderDetail.TotalMaerksParts +
                            WorkOrderDetail.AgentPartsTax);

                fTotCost *= WorkOrderDetail.ExchangeRate;
                WorkOrderDetail.TotalCostOfRepair = Math.Round(fTotCost.Value, 4);

                fTotCost = (WorkOrderDetail.TotalLabourCostCPH +
                            WorkOrderDetail.TotalShopAmountCPH +
                            WorkOrderDetail.TotalManPartsCPH +
                            WorkOrderDetail.ImportTaxCPH +
                            WorkOrderDetail.SalesTaxPartsCPH +
                            WorkOrderDetail.SalesTaxLabourCPH +
                            WorkOrderDetail.TotalMaerksPartsCPH +
                            WorkOrderDetail.AgentPartsTaxCPH);

                WorkOrderDetail.TotalCostOfRepairCPH = Math.Round(fTotCost.Value, 4);

                // Store labor, parts and import taxes in percent format.
                if ((COType.Equals(Equipment.CONTAINERTYPE.GENS.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    WorkOrderDetail.SalesTaxLaborPCT = (WorkOrderDetail.Shop.SalesTaxLaborGen * 100);
                    WorkOrderDetail.SalesTaxLaborPCT = Math.Round(WorkOrderDetail.SalesTaxLaborPCT.Value, 2);
                    WorkOrderDetail.SalesTaxPartsPCT = (WorkOrderDetail.Shop.SalesTaxPartGen * 100);
                    WorkOrderDetail.SalesTaxPartsPCT = Math.Round(WorkOrderDetail.SalesTaxPartsPCT.Value, 2);
                }
                else // is container type
                {
                    WorkOrderDetail.SalesTaxLaborPCT = (WorkOrderDetail.Shop.SalesTaxLaborCon * 100);
                    WorkOrderDetail.SalesTaxLaborPCT = Math.Round(WorkOrderDetail.SalesTaxLaborPCT.Value, 2);
                    WorkOrderDetail.SalesTaxPartsPCT = (WorkOrderDetail.Shop.SalesTaxPartCont * 100);
                    WorkOrderDetail.SalesTaxPartsPCT = Math.Round(WorkOrderDetail.SalesTaxPartsPCT.Value, 2);
                }

                WorkOrderDetail.ImportTaxPCT = (WorkOrderDetail.Shop.ImportTax * 100);
                WorkOrderDetail.ImportTaxPCT = Math.Round(WorkOrderDetail.ImportTaxPCT.Value, 2);

                //------Added extra checking for 7602 --------------


                //rounding up all the amounts
                //WorkOrderDetail.TotalShopAmount
                if (!IsValidNumericSize(Convert.ToString(WorkOrderDetail.TotalShopAmount), 12, 4))
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + WorkOrderDetail.EquipmentList[0].EquipmentNo + " , Total material amount exceeds:<br> (-)n(0-12).n(0-4) where (-) negative is optional and n = 0-9<br>and brackets denote max number of digits.<br> ";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);

                }
                else if (!IsValidNumericSize(Convert.ToString(WorkOrderDetail.TotalShopAmountCPH), 12, 4))
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO:" + WorkOrderDetail.EquipmentList[0].EquipmentNo + "  Total material amount CPH exceeds:<br> (-)n(0-12).n(0-4) where (-) negative is optional and n = 0-9<br>and brackets denote max number of digits.<br> ";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }
                else if (!IsValidNumericSize(Convert.ToString(WorkOrderDetail.TotalCostOfRepair), 12, 4))
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + WorkOrderDetail.EquipmentList[0].EquipmentNo + " Total cost repair exceeds:<br> (-)n(0-12).n(0-4) where (-) negative is optional and n = 0-9<br>and brackets denote max number of digits.<br> ";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }
                else if (!IsValidNumericSize(Convert.ToString(WorkOrderDetail.TotalCostOfRepairCPH), 12, 4))
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + WorkOrderDetail.EquipmentList[0].EquipmentNo + ", Total cost repair cph exceeds:<br> (-)n(0-12).n(0-4) where (-) negative is optional and n = 0-9<br>and brackets denote max number of digits.<br> ";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }
                else if (!IsValidNumericSize(Convert.ToString(WorkOrderDetail.TotalCostLocal), 12, 4))
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO:" + WorkOrderDetail.EquipmentList[0].EquipmentNo + ", Total cost exceeds:<br> (-)n(0-12).n(0-4) where (-) negative is optional and n = 0-9<br>and brackets denote max number of digits.<br> ";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }
                else if (!IsValidNumericSize(Convert.ToString(WorkOrderDetail.TotalCostLocalUSD), 12, 4))
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + WorkOrderDetail.EquipmentList[0].EquipmentNo + ", Total cost USD exceeds:<br> (-)n(0-12).n(0-4) where (-) negative is optional and n = 0-9<br>and brackets denote max number of digits.<br> ";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }
                else if (!IsValidNumericSize(Convert.ToString(WorkOrderDetail.TotalCostCPH), 12, 4))
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + WorkOrderDetail.EquipmentList[0].EquipmentNo + ", Total cost CPH exceeds:<br> (-)n(0-12).n(0-4) where (-) negative is optional and n = 0-9<br>and brackets denote max number of digits.<br> ";
                    errorMessageList.Add(Message);
                }



                if (string.IsNullOrEmpty(Convert.ToString(WorkOrderDetail.TotalEDIAmount)))
                    WorkOrderDetail.TotalEDIAmount = 0;
                else
                    if (string.IsNullOrEmpty(Convert.ToString(WorkOrderDetail.TotalEDIAmount))) //IsNumericString
                    {
                        WorkOrderDetail.TotalEDIAmount = 0;
                    }

                decimal? fEDIAmount = 0;

                if (WorkOrderDetail.TotalEDIAmount == 0)
                {
                    fEDIAmount = (WorkOrderDetail.TotalLabourCost + WorkOrderDetail.TotalShopAmount);
                    WorkOrderDetail.TotalEDIAmount = Math.Round(fEDIAmount.Value, 2);
                }
                else
                {
                    decimal? fTotCostLocal, fTotAmt;
                    fTotAmt = WorkOrderDetail.TotalEDIAmount; // Total amount passed in from EDI. (Note calculated and assigned for Web)
                    fTotCostLocal = (WorkOrderDetail.TotalLabourCost + WorkOrderDetail.TotalShopAmount);

                    fTotAmt *= WorkOrderDetail.ExchangeRate;
                    fTotCostLocal *= WorkOrderDetail.ExchangeRate;

                    if ((fTotCostLocal - fTotAmt) > 1) //Abu - The original code is checking absolute value. Please revisit.
                    {
                        Message = new ErrMessage();
                        Message.Message = GetErrorMessage(20090);
                        errorMessageList.Add(Message);
                    }
                }

                TotalSum = (WorkOrderDetail.TotalManHourReg +
                            WorkOrderDetail.TotalManHourOverTime +
                            WorkOrderDetail.TotalManHourDoubleTime +
                            WorkOrderDetail.TotalManHourMisc);

                temp = (double)TotalSum;
                tmp = Math.Round(TotalSum.Value, 2);

                // UI may edit existing records - change man hours to match input.

                if (WorkOrderDetail.woState != (int)Validation.STATE.NEW)
                {
                    WorkOrderDetail.TotalRepairManHour = tmp;
                }

                if (tmp != WorkOrderDetail.TotalRepairManHour)
                {
                    Message = new ErrMessage();
                    Message.Message = GetErrorMessage(20100);
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            logEntry.Message = "Method Name : PerformCalculations(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repCode"></param>
        /// <param name="manualCode"></param>
        /// <param name="mode"></param>
        /// <param name="partCode"></param>
        /// <returns></returns>
        private int? GetMaxPartQty(string repCode, string manualCode, string mode, string partCode)
        {
            int? res = 0;
            try
            {
                var qty = (from M in objContext.MESC1TS_RPRCODE_PART
                           where M.REPAIR_CD == repCode.Trim() && M.MANUAL_CD == manualCode && M.MODE == mode && M.PART_CD == partCode
                           select new { M.MAX_PART_QTY }).FirstOrDefault();
                if (qty != null) res = qty.MAX_PART_QTY;
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="errorMessageList"></param>
        /// <returns></returns>
        private bool CheckEquipmemtAge(ref WorkOrderDetail WO, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : CheckEquipmemtAge(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            errorMessageList = new List<ErrMessage>();
            // Attempt to get date from EQINDAT - if empty, assume 5 years
            // if date error, send in error list
            bool bSuccess = true;
            m_bCPHRepairCostExceeded = true;
            int nYr, nDy, nMt;
            string s = string.Empty;
            short szAge = 0;
            string sDate = string.Empty;
            string shopCode = WO.Shop.ShopCode;
            string Mode = WO.Mode;
            float fAge = 0;
            double lDays;
            decimal? fCPHPercent = 0;
            decimal? fCPHLimit = 0;
            Equipment Eqp = WO.EquipmentList[0];

            try
            {
                if (Eqp.EQInDate != null)
                    sDate = Eqp.EQInDate.ToString();//("yyyy-MM-DD", System.Globalization.CultureInfo.InvariantCulture);
                else szAge = 5;

                // Check length - expecting 10 character YYYY-MM-DD format.
                // if INDAT not empty, attempt to extract date and calculate age from current time.
                if (!string.IsNullOrEmpty(sDate) && sDate.Length == 10)
                {
                    nYr = Convert.ToInt32(sDate.Substring(0, 4));
                    nMt = Convert.ToInt32(sDate.Substring(5, 2));
                    nDy = Convert.ToInt32(sDate.Substring(8, 2));

                    // Get current time and calculate age in years
                    try
                    {
                        DateTime tNow = DateTime.Now;
                        DateTime tAge, dt;
                        TimeSpan dtDiff;

                        tAge = new DateTime(nYr, nMt, nDy, 0, 0, 0);

                        // check if valid date - and get age difference.
                        if (DateTime.TryParse(string.Format("{0}-{1}-{2}", tAge.Year, tAge.Month, tAge.Date), out dt))
                        {
                            // get date time differnce
                            dtDiff = tNow - tAge;
                            // Get days and calculate into years.
                            lDays = dtDiff.TotalDays;
                            fAge = (float)(lDays / 365);
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = new ErrMessage();
                        Message.Message = "m_pManufacturer->RSByEQTypeNumber: Failed open of Manufacturer recordset";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        errorMessageList.Add(Message);
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        bSuccess = false;
                    }
                }
                else
                {	// INDAT is empty, so default to 5 years.
                    szAge = 5;
                }
                // Need age mode and size to make limits call.
                //m_pLimitsAccessor.CreateInstance(__uuidof(LimitsAccessor));

                // Get Shop Repair Limit Adjustment factor by Shop - in percent format, so x 0,01 convert to decimal

                string sSize = Eqp.Size;

                var RSRepairLimitAdjByShop = (from C in objContext.MESC1TS_COUNTRY
                                              from L in objContext.MESC1TS_LOCATION
                                              from S in objContext.MESC1TS_SHOP
                                              where C.COUNTRY_CD == L.COUNTRY_CD &&
                                              L.LOC_CD == S.LOC_CD &&
                                              S.SHOP_CD == shopCode
                                              select new
                                              {
                                                  C.REPAIR_LIMIT_ADJ_FACTOR
                                              }).FirstOrDefault();

                //fCPHPercent :: pRs = m_pLimitsAccessor->RSRepairLimitAdjByShop( rhsRecord.m_sSHOP_CD.c_str() );

                if (RSRepairLimitAdjByShop != null)
                {
                    try
                    {
                        fCPHPercent = (decimal)RSRepairLimitAdjByShop.REPAIR_LIMIT_ADJ_FACTOR;
                        fCPHPercent *= (decimal)0.01;
                    }
                    catch
                    {
                        // is null or empty - set to 1 so CPH values not changed.
                        fCPHPercent = 1;
                    }
                }
            }
            catch (Exception ex)	// Catch ADO data errors.
            {
                Message = new ErrMessage();
                Message.Message = "System error: LimitsAcessor : Method RSRepairLimitAdjByShop - Failed to open MESC1TS_CPH_EQP_LIMIT";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                bSuccess = false;
            }


            m_bCPHRepairCostExceeded = false;
            // Get CPH limits by mode, size and age.
            try
            {
                string sSize = Eqp.Size;
                if (Eqp.COType.Equals(Equipment.CONTAINERTYPE.GENS))
                {
                    sSize = "NA";
                }
                //fCPHLimit::pRs = m_pLimitsAccessor->RSCPHLimitByModeSizeAge( rhsRecord.m_sMODE.c_str(), sSize.c_str(), szAge );

                var RSCPHLimitByModeSizeAge = (from l in objContext.MESC1TS_CPH_EQP_LIMIT
                                               where l.MODE == Mode &&
                                               l.EQSIZE == sSize &&
                                               l.AGE_FROM <= szAge &&
                                               l.AGE_TO >= szAge
                                               select new
                                               {
                                                   l.LIMIT_AMOUNT
                                               }).FirstOrDefault();

                if (RSCPHLimitByModeSizeAge != null)
                {
                    try
                    {
                        fCPHLimit = RSCPHLimitByModeSizeAge.LIMIT_AMOUNT;
                        // apply Country adjustment percent
                        fCPHLimit *= fCPHPercent;

                        // Compare if CPH cost more that CPH limit.
                        if (WO.TotalCostOfRepairCPH > fCPHLimit)
                        {
                            // !FIX - set switch for final suspend to MSL for forward to CPH 320
                            // Add remark
                            m_bCPHRepairCostExceeded = true;
                        }
                    }
                    catch
                    {
                        // limit value is null or empty, do not do comparison. - fall through
                    }
                }
            }
            catch (Exception ex)	// Catch ADO data errors.
            {
                Message = new ErrMessage();
                Message.Message = (ex.Message + " System error: m_pLimitsAccessor->RSCPHLimitByModeSizeAge: Failed open of MESC1TS_CPH_EQP_LIMIT recordset");
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                bSuccess = false;
            }
            logEntry.Message = "Method Name : CheckEquipmemtAge(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            return bSuccess;
        }

        // set status based on business rules. (combine with check suspended status from original merc)
        private void SetWorkOrderStatus(ref WorkOrderDetail WO, out List<ErrMessage> errorMessageList)
        {
            logEntry.Message = "Method Name : SetWorkOrderStatus(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            errorMessageList = new List<ErrMessage>();
            bool bDraft = false;
            WO.RemarksList = new List<RemarkEntry>();
            Equipment Eqp = WO.EquipmentList[0];

            // (VJP 01-27-2005) Prime Mover requested equipment numbers be removed from system remarks.

            try
            {
                // Force draft status - if saved as draft
                if (string.Equals(Validation.WOSTATUS.ASDRAFT.ToString(), WO.WorkOrderStatus.ToString()))
                {
                    bDraft = true;
                }

                // VJP 2006-07-17
                // New rules if original EQOWNTP from RKEM = VSA or SO or PLAWHO is not empty, 
                // Throw error to stop estimate with below message:
                // Equipment not owned, leased or operated by Maersk, repair not allowed.
                if (string.Equals(Eqp.RKEM_EQOWNThirdParty, Validation.EQPOWNTHIRDPARTY.VSA.ToString(), StringComparison.OrdinalIgnoreCase) ||
                    (string.Equals(Eqp.RKEM_EQOWNThirdParty, Validation.EQPOWNTHIRDPARTY.SO.ToString(), StringComparison.OrdinalIgnoreCase)) ||

                    ((!string.IsNullOrEmpty(Eqp.PLAWHO))) &&
                     (!string.Equals(Eqp.COType, Equipment.CONTAINERTYPE.GENS.ToString(), StringComparison.OrdinalIgnoreCase)) &&
                     (!string.Equals(Eqp.COType, Equipment.CONTAINERTYPE.CHAS.ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    // add to error list (this equipment isn't owned by Maersk - no work to be done on it.
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + Eqp.EquipmentNo + " not owned, leased or operated by Maersk, repair not allowed.";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }

                // If NOT bypass lease rules Do SUB Lease, ONE way lease and Redelivery checks.
                if (!m_bBypassLease)
                {

                    // Additional owner checks Sublease check SUB                
                    if (string.Equals(Eqp.RKEM_EQOWNThirdParty, Validation.EQPOWNTHIRDPARTY.SUB.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        // add to remark list (this equipment isn't owned by Maersk - no work to be done on it.
                        WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "SUSPENDED: Equipment is subleased. Please investigate if damage caused by MSL. Wear and Tear repair not allowed."));
                    }

                    // Additional owner checks One way lease check  ONE
                    if (string.Equals(Eqp.RKEM_EQOWNThirdParty, Validation.EQPOWNTHIRDPARTY.ONE.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        // add to remark list (this equipment isn't owned by Maersk - no work to be done on it.
                        WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "SUSPENDED: Equipment is oneway lease. Please investigate if damage caused by MSL. Wear and tear repair not allowed."));
                    }


                    // suspend for modes 8X, modes?  Redelivery
                    // Additional  - check redelivery status in m_sEQIOFLT  - redelivery is a suspension
                    if (string.Equals(Eqp.EQIoflt, "R", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(Eqp.SelectedMode))
                        {
                            if (string.Equals(Eqp.SelectedMode.Substring(0, 1), "8", StringComparison.OrdinalIgnoreCase))
                            {
                                // add to remark list (this equipment isn't owned by Maersk - no work to be done on it.
                                // here 8x mode used, this is a suspension.
                                WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "SUSPENDED: Equipment has been redelivered to leasing company."));
                            }
                            else
                            {
                                // !Only 8x modes, if not - error
                                Message = new ErrMessage();
                                Message.Message = "EQPNO: " + Eqp.EquipmentNo + ", Equipment has been redelivered to leasing company. Only 8x modes are valid.";
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                errorMessageList.Add(Message);
                            }
                        }
                    }
                }

                // Additional  - check disposed status in m_sEQIOFLT  - disposed is a failure
                if (string.Equals(Eqp.EQIoflt, "D", StringComparison.OrdinalIgnoreCase))
                {
                    // add to remark list (this equipment isn't owned by Maersk - no work to be done on it.
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + Eqp.EquipmentNo + ", Equipment:  " + Eqp.EquipmentNo + " Disposed, repair not allowed.";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }

                // Additional  - check disposed status in m_sEQIOFLT  - VSA STOP!!!
                if (string.Equals(Eqp.EQIoflt, "V", StringComparison.OrdinalIgnoreCase))
                {
                    // add to remark list (this equipment isn't owned by Maersk - no work to be done on it.
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + Eqp.EquipmentNo + ", VSA/So, repair not allowed.";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }


                // #3 (original specs)  Check Cause.
                if ((WO.Cause == "3") || (WO.Cause == "4"))
                {
                    WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "Suspected 3rd party damage - pls supply location of responsible party."));
                }

                // #5 (original specs) check repair amount limitations - passed in from shop limits.
                //	if (atof( rhsRecord.m_sTOT_COST_LOCAL.c_str() ) > rhsRecord.m_fTotRepairAmtLimit )
                if (WO.TotalCostOfRepair > Convert.ToDecimal(m_fTotRepairAmtLimit))
                {
                    //		sprintf(msg, "EQPNO: %s, Shop suspend limit exceeded", rhsRecord.m_sEQPNO.c_str() );
                    WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", 0, "Shop suspend limit exceeded"));
                }

                // #8  - new interface ????  ISuspend?
                string errMsg = CheckSuspendTable(WO);
                if (!string.IsNullOrEmpty(errMsg))
                {
                    Message = new ErrMessage();
                    Message.Message = errMsg;
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                }

                // Original specs - 05-01-01 VJP - All non-STS work orders are automatically suspended.
                if (WO.WorkOrderType.Equals("N", StringComparison.CurrentCultureIgnoreCase))
                {
                    //		sprintf(msg, "EQPNO: %s, Suspend reason: NON-STS work order", rhsRecord.m_sEQPNO.c_str() );
                    //sprintf(msg, "Suspend reason: NON-STS work order");
                    WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "Suspend reason: NON-STS work order"));
                }

                // if not bypass leased equipment rules check then do the global hunt/offhire checks.
                if (m_bBypassLease)
                {
                    // Global hunt check and Off Hire Location check
                    // if On Global hunt and not on Off Hire, then is automatic suspension to Local msl
                    if (Eqp.Stredel.Equals("Y", StringComparison.CurrentCultureIgnoreCase) && (Eqp.OffhirLocationSW.Equals("N", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        //		sprintf(msg, "EQPNO: %s, Suspend reason: Equipment is due for re-delivery, Off Hire shall be considered: Please note DPP/Fixed Cover", rhsRecord.m_sEQPNO.c_str() );
                        WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "Suspend reason: Equipment is due for re-delivery, Off Hire shall be considered: Please note DPP/Fixed Cover"));
                    }

                    // Global hunt check and Off Hire Location check
                    // if On Global hunt and on Off Hire, then is automatic suspension to Local msl
                    if (Eqp.Stredel.Equals("Y", StringComparison.CurrentCultureIgnoreCase) && (Eqp.OffhirLocationSW.Equals("Y", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        //		sprintf(msg, "EQPNO: %s, Suspend reason: Equipment is due for re-delivery and is at off hire location. Local Off Hire shall be considered. Please note DPP/Fixed Cover", rhsRecord.m_sEQPNO.c_str() );
                        WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "Suspend reason: Equipment is due for re-delivery and is at off hire location. Local Off Hire shall be considered. Please note DPP/Fixed Cover"));
                    }
                }

                // Compare rhsRecord.m_fShopMaterialLimit to
                // rhsRecord.m_sTOT_SHOP_AMT  - force suspension.
                if (WO.TotalShopAmount > m_fShopMaterialLimit)
                {
                    //		sprintf(msg, "EQPNO: %s, Suspend reason: Total shop supplied material limit exceeded", rhsRecord.m_sEQPNO.c_str() );
                    WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "Suspend reason: Total shop supplied material limit exceeded"));
                }


                // FINALLY  set WO suspend Status. Check for any remarks created as "S" system type which forces 'suspension' of WO
                bool bFound = false;
                if (WO.RemarksList != null && WO.RemarksList.Count > 0)
                {
                    foreach (var remarks in WO.RemarksList)
                    {
                        if (remarks.RemarkType.Equals("S", StringComparison.CurrentCultureIgnoreCase)) // && remarks.
                            //if ((m_StringUtil.CompareNoCase(pRecord->GetRemarkType(), "S") == 0) && (pRecord->IsNew()))
                            bFound = true;
                    }
                }

                // check level.
                if (bFound)	// i.e. system generated remarks found ( for suspensions only );
                    WO.WorkOrderStatus = 310;
                else		// check for automatic approval		
                {
                    if (WO.TotalCostLocal <= m_fAutoAppovalLimit)
                    {
                        if (WO.Shop.AutoCompleteSW != null && WO.Shop.AutoCompleteSW.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                        {
                            WO.WorkOrderStatus = 400;
                        }
                        else
                        {
                            WO.WorkOrderStatus = 390;
                        }
                    }
                    else	// set status to 'Pending'
                        WO.WorkOrderStatus = 200;
                }

                // REQ_REMARK_SW - if 'Y' then suspend as 310
                if (WO.ReqdRemarkSW == true)
                {
                    WO.WorkOrderStatus = 310;
                }

                // if bypass lease rules in effect for this shop, skip special remarks suspension checks (VJP 02-14-2005)
                if (m_bBypassLease)
                {
                    // skip special remarks if bypassing leased equipment validation checks.
                    m_bSpecialRemarkFound = false;
                }

                if (m_bSpecialRemarkFound)
                {

                    // if null repair ceiling and displaySw = Y, then display warning only and create shop and system remark.
                    if ((m_fRemarksRepairCeiling == -1) && (m_sDisplaySw.Equals("Y", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO" + Eqp.EquipmentNo + SpecialRemarks;
                        // add to warning list;
                        Message.ErrorType = Validation.MESSAGETYPE.WARNING.ToString();
                        errorMessageList.Add(Message);
                        // remove equipment number from remarks

                        //sprintf(lpszError, "%s", m_sSpecialRemarks.c_str());
                        WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, SpecialRemarks));
                        // (VJP - 12-13-2004) set to suspend - specs require warning only
                        //			rhsRecord.m_sSTATUS_CODE= "310";
                    }

                    // if null repair ceiling and displaySw = N, then create a system remark only
                    if ((m_fRemarksRepairCeiling == -1) && (m_sDisplaySw.Equals("N", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        //			sprintf(lpszError, "EQPNO: %s, %s", rhsRecord.m_sEQPNO.c_str(), m_sSpecialRemarks.c_str() );
                        //sprintf(lpszError, "%s", m_sSpecialRemarks.c_str());
                        WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, SpecialRemarks));
                        WO.WorkOrderStatus = 310;
                    }

                    //---------------------------------------------
                    // Add remark - force fail of work order 
                    // if 0 ceiling and displaySw = Y, create error message and create error message with special remark.
                    if ((m_fRemarksRepairCeiling == 0) && (m_sDisplaySw.Equals("Y", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        //sprintf(lpszError, "EQPNO: %s, %s", rhsRecord.m_sEQPNO.c_str(), m_sSpecialRemarks.c_str());
                        //rhsRecord.m_sError += lpszError;

                        //sprintf(lpszError, "EQPNO: %s, Contact Local MLS Agency before repairing or moving.", rhsRecord.m_sEQPNO.c_str());
                        //rhsRecord.m_aErrorList.Insert(lpszError);
                        Message = new ErrMessage();
                        Message.Message = "EQPNO: " + Eqp.EquipmentNo + ", Contact Local MLS Agency before repairing or moving.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        errorMessageList.Add(Message);
                    }

                    if ((m_fRemarksRepairCeiling == 0) && (m_sDisplaySw.Equals("Y", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO: " + Eqp.EquipmentNo + ", Contact Local MLS Agency before repairing or moving.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        errorMessageList.Add(Message);
                    }


                    // if Total cost local < m_fRemarksRepairCeiling then suspend for CPH
                    if ((m_fRemarksRepairCeiling > 0) && (m_sDisplaySw.Equals("N", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        // if CPH amount > special repair amount limit ceiling, suspend for CPH
                        if (WO.TotalCostOfRepairCPH <= m_fRemarksRepairCeiling)
                        {
                            WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, SpecialRemarks));
                            WO.WorkOrderStatus = 310;
                        }
                    }
                    // if Total cost local > m_fRemarksRepairCeiling then suspend for CPH - show warnings
                    if ((m_fRemarksRepairCeiling > 0) && (m_sDisplaySw.Equals("Y", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        // if CPH amount > special repair amount limit ceiling, suspend for CPH
                        Message = new ErrMessage();
                        Message.Message = "EQPNO: " + Eqp.EquipmentNo + SpecialRemarks;
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        errorMessageList.Add(Message);
                        if (WO.TotalCostOfRepair <= m_fRemarksRepairCeiling)
                        {
                            //sprintf(lpszError, "%s", m_sSpecialRemarks.c_str());
                            WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, SpecialRemarks));
                            WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "V", null, SpecialRemarks));
                            WO.WorkOrderStatus = 310;
                        }
                        //			rhsRecord.m_sError+= lpszError; 
                    }


                    // if Total cost local > m_fRemarksRepairCeiling then suspend for CPH
                    if ((m_fRemarksRepairCeiling > 0) && (m_sDisplaySw.Equals("N", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        // if CPH amount > special repair amount limit ceiling, suspend for CPH
                        if (WO.TotalCostOfRepair > m_fRemarksRepairCeiling)
                        {
                            WO.WorkOrderStatus = 320;
                            //				sprintf(lpszError, "EQPNO: %s, Estimate must be approved by CENEMR.", rhsRecord.m_sEQPNO.c_str() );
                            //sprintf(lpszError, "Estimate must be approved by CENEMR.");
                            WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "Estimate must be approved by CENEMR."));

                            //				sprintf(lpszError, "EQPNO: %s, %s", rhsRecord.m_sEQPNO.c_str(), m_sSpecialRemarks.c_str() );
                            //sprintf(lpszError, "%s", m_sSpecialRemarks.c_str());
                            WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, SpecialRemarks));
                        }
                    }
                    //if Total cost local > m_fRemarksRepairCeiling then suspend for CPH - show warnings
                    if ((m_fRemarksRepairCeiling > 0) && (m_sDisplaySw.Equals("N", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        // if CPH amount > special repair amount limit ceiling, suspend for CPH
                        if (WO.TotalCostOfRepair > m_fRemarksRepairCeiling)
                        {
                            WO.WorkOrderStatus = 320;
                            //				sprintf(lpszError, "EQPNO: %s, Estimate must be approved by CENEMR.", rhsRecord.m_sEQPNO.c_str() );
                            //sprintf(lpszError, "Estimate must be approved by CENEMR.");
                            WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "Estimate must be approved by CENEMR."));


                            //				sprintf(lpszError, "EQPNO: %s, %s", rhsRecord.m_sEQPNO.c_str(), m_sSpecialRemarks.c_str() );
                            //sprintf(lpszError, "%s", m_sSpecialRemarks.c_str());
                            WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, SpecialRemarks));
                            WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "V", null, SpecialRemarks));
                        }
                    }
                }
                if (m_bCPHRepairCostExceeded)
                {
                    WO.WorkOrderStatus = 320;
                    //		sprintf(lpszError, "EQPNO: %s, Total cost repair CPH exceeds CPH equipment limit amount.", rhsRecord.m_sEQPNO.c_str() );
                    //sprintf(lpszError, "Total cost repair CPH exceeds CPH equipment limit amount.");
                    WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "Total cost repair CPH exceeds CPH equipment limit amount."));
                }


                if (bDraft)
                {
                    WO.WorkOrderStatus = 0;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            logEntry.Message = "Method Name : SetWorkOrderStatus(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
        }

        #endregion Validate

        private bool CheckPTIDate(WorkOrderDetail WorkOrder, int? nDays, out List<ErrMessage> ErrormessageList)
        //{ }
        {
            ErrormessageList = new List<ErrMessage>();
            DateTime date1 = DateTime.MinValue;
            DateTime dtCurrentDatetime = DateTime.Now;
            bool success = true;
            int nYr, nDy, nMt, i;

            // get PTI date from RKEM call 
            string sDate = WorkOrder.EquipmentList[0].Deldatsh.ToString();

            // RKEM is returning a 0001 for year value if date not in RKEM
            // so...  if 0001, no problem, return.
            if (WorkOrder.EquipmentList[0].Deldatsh != null)
            {
                string sYear = sDate.ToString();
                sYear = sYear.Substring(0, 4);
                if (sYear.Equals("0001", StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            else
                return true;

            //if(WorkOrder.Deldatsh == DateTime.MinValue)
            //{
            //    //
            //}
            //else if(WorkOrder.Deldatsh == null)
            //{
            //    return;
            //}

            // attempt to create OLE Date from RKEM PTI date.
            i = sDate.Length;
            if (i >= 10)
            {
                // if not numeric date, just return
                if (!char.IsDigit(sDate[0])) return true;


                // Else check if valid date
                if (!char.IsDigit(sDate[2]))	// probably is mm/dd/yyyy
                {
                    nYr = Convert.ToInt32(sDate.Substring(6, 4));
                    nMt = Convert.ToInt32(sDate.Substring(0, 2));
                    nDy = Convert.ToInt32(sDate.Substring(3, 2));
                }
                else							// probably is yyyy/mm/dd							
                {
                    nYr = Convert.ToInt32(sDate.Substring(0, 4));
                    nMt = Convert.ToInt32(sDate.Substring(5, 2));
                    nDy = Convert.ToInt32(sDate.Substring(8, 2));
                }

                try
                {
                    date1 = new DateTime(nYr, nMt, nDy, 0, 0, 0);
                    // check if valid date
                    //if (date1.GetStatus() != 0)
                    //{
                    //    sprintf(lpszError, "EQPNO: %s, Invalid Date PTI date: %s", rhsRecord.m_sEQPNO.c_str(), sDate );
                    //    rhsRecord.m_aErrorList.Insert(lpszError);
                    //    return;
                    //}
                }
                catch (Exception ex)
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + ", Invalid Date PTI date: " + sDate;
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrormessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    return false;
                }
            }
            else	// else is unknown format - fail
            {
                Message = new ErrMessage();
                Message.Message = "EQPNO: " + WorkOrder.EquipmentList[0].EquipmentNo + ", Invalid Date PTI date: " + sDate;
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                ErrormessageList.Add(Message);
                return false;
            }


            // Get days elapsed since last PTI
            TimeSpan tt = DateTime.Now.Subtract(date1);
            // If < days allowed before next inspection, Add system remark.
            if (tt.TotalDays < nDays)
            {
                WorkOrder.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "Last PTI less than " + nDays + " days ago. PTI not due unless a live load has been moved since last PTI."));
            }
            return success;
        }

        private bool ValidateBasics(WorkOrderDetail WorkOrder, out List<ErrMessage> ErrorMessageList)
        {
            logEntry.Message = "Method Name : ValidateBasics(), Start Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            ErrorMessageList = new List<ErrMessage>();
            List<Damage> DamageList = null;
            List<RepairLoc> RepairLocationCodeList = null;
            List<RepairCode> RepairCodeList = null;
            List<Tpi> TpiList = null;
            Equipment Eqp = Eqp;
            try
            {
                if (Eqp.VendorRefNo != null && Eqp.VendorRefNo.Length > 10)
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + Eqp.EquipmentNo + " " + ", Vendor reference number exceeds 10 char, please correct and resubmit";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrorMessageList.Add(Message);

                }

                if (string.IsNullOrEmpty(WorkOrder.Shop.ShopCode))
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Shop code is required.";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrorMessageList.Add(Message);
                }

                if (string.IsNullOrEmpty(WorkOrder.Shop.Customer[0].CustomerCode))
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO: " + Eqp.EquipmentNo + " " + " Customer code is required.";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrorMessageList.Add(Message);
                }

                if (WorkOrder.RepairDate == null)
                {
                    WorkOrder.RepairDate = DateTime.Now;
                }
                else
                {
                    // If repair date entered, do validation.
                    if (WorkOrder.RepairDate != null)
                        CheckRepairDate(WorkOrder, out ErrorMessageList);
                }

                if (string.IsNullOrEmpty(Eqp.SelectedMode))
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " WO Mode is required.";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrorMessageList.Add(Message);
                }


                if (string.IsNullOrEmpty(WorkOrder.Cause))
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " WO Cause is required.";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrorMessageList.Add(Message);
                }
                else
                {	// check cause must be 1, 2, 3 or 4
                    char c = WorkOrder.Cause.ElementAt(0);
                    if ((c != '1') && (c != '2') && (c != '3') && (c != '4'))
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Invalid 3rd party cause. Value must be 1, 2, 3 or 4.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                    }
                }


                // Check 4 hour types entered.
                if (WorkOrder.TotalManHourReg == 0)
                    WorkOrder.TotalManHourReg = 0.0;
                else
                {
                    if (!IsValidNumericSize(WorkOrder.TotalManHourReg.ToString(), 3, 2))
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Ordinary hours must be numeric - n(3).n(1).";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                    }
                    else if (WorkOrder.TotalManHourReg < 0)
                    {
                        // ensure is > 0
                        //double d = atof( WorkOrder.m_sTOT_MANH_REG;
                        //if (d < 0.0)
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Ordinary hours cannot be less than zero.<br> ";
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                        }
                    }
                }

                if (WorkOrder.TotalManHourDoubleTime == 0)
                    WorkOrder.TotalManHourDoubleTime = 0.0;
                else
                {
                    if (!IsValidNumericSize(WorkOrder.TotalManHourDoubleTime.ToString(), 3, 2))
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Overtime2 hours must be numeric - n(3).n(1).";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                    }
                    else if (WorkOrder.TotalManHourDoubleTime < 0)
                    {
                        // ensure is > 0
                        //double d = atof( WorkOrder.m_sTOT_MANH_DT;
                        //if (d < 0.0)
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Overtime2 hours cannot be less than zero.<br> ";
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                        }
                    }
                }

                if (WorkOrder.TotalManHourMisc == 0)
                    WorkOrder.TotalManHourMisc = 0.0;
                else
                {
                    if (!IsValidNumericSize(WorkOrder.TotalManHourMisc.ToString(), 3, 2))
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Overtime3 hours must be numeric - n(3).n(1).";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                    }
                    else if (WorkOrder.TotalManHourMisc < 0)
                    {
                        // ensure is > 0
                        //double d = atof( WorkOrder.m_sTOT_MANH_MISC;
                        //if (d < 0.0)
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Overtime3 hours cannot be less than zero.<br> ";
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                        }
                    }
                }


                if (WorkOrder.TotalManHourOverTime == 0)
                    WorkOrder.TotalManHourOverTime = 0.0;
                else
                {
                    if (!IsValidNumericSize(WorkOrder.TotalManHourOverTime.ToString(), 3, 2))
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Overtime1 hours must be numeric - n(3).n(1).";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                    }
                    else if (WorkOrder.TotalManHourOverTime < 0)
                    {
                        // ensure is > 0
                        //double d = atof( WorkOrder.m_sTOT_MANH_OT;
                        //if (d < 0.0)
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Overtime1 hours cannot be less than zero.<br> ";
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                        }
                    }
                }


                //if (string.IsNullOrEmpty(WorkOrder.ChangeUser))
                //{
                //    Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " User name is required." + Eqp.EquipmentNo;
                //    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                //    ErrorMessageList.Add(Message);
                //}

                // check for cotype and other equipment values
                if (string.IsNullOrEmpty(Eqp.COType))
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Container type not received from RKEM or RKEM call not performed.";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrorMessageList.Add(Message);
                }

                // Continue with repair records.

                // At least one repair must exist on a work order.
                if (WorkOrder.RepairsViewList.Count == 0)
                {
                    Message = new ErrMessage();
                    Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " At least one repair must be entered for this WO.";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrorMessageList.Add(Message);
                    return (false);
                }
                else
                {
                    bool bAllDeleted = true;
                    // make sure that at least one repair record is not deleted.
                    foreach (var rItem in WorkOrder.RepairsViewList)
                    {
                        // check repair code.status if not deleted.
                        // don't include check of repair tax codes.
                        if (!IsRepairTaxCode(rItem.RepairCode))
                        {
                            if (rItem.rState != (int)Validation.STATE.DELETED)
                                bAllDeleted = false;
                        }
                    }

                    // if no new or existing repairs then error out.
                    if (bAllDeleted)
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " At least one repair must be entered for this WO.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                    }
                }
                lstRepCodeLocPart.Clear();
                // Check individual repair record fields.
                foreach (var rItem in WorkOrder.RepairsViewList)
                {
                    lstRepCodeLocPart.Clear();
                    // check if deleted.
                    if (rItem.rState == (int)Validation.STATE.DELETED)
                        continue;

                    // trim string - check if empty.
                    if (rItem.RepairCode.RepairCod.Length == 0)
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Repair code is required.";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        continue;	// no point in checking anything else on this record
                    }

                    // Check amount for tax repair codes
                    if (IsRepairTaxCode(rItem.RepairCode))
                    {
                        // check amount field only for numerics.
                        if (rItem.MaterialCost > 0)
                        {
                            if (!IsNumericString(rItem.MaterialCost.ToString()))
                            {
                                Message = new ErrMessage();
                                Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Tax entered must be numeric.";
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                ErrorMessageList.Add(Message);
                            }
                            else	// check precision.
                            {
                                if (!IsValidNumericSize(rItem.MaterialCost.ToString(), 7, 2))
                                {
                                    Message = new ErrMessage();
                                    Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Enter tax amount as:<br> (-)n(0-7).n(0-2) where (-) negative is optional and n = 0-9<br>and brackets denote max number of digits.<br> ";
                                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                    ErrorMessageList.Add(Message);
                                }
                            }
                        }

                        continue;	// no point in checking other values for this record, only the material/tax amount
                    }

                    if (rItem.Damage.DamageCedexCode.Length != 2)
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Damage Code length must be 2 characters for repair : " + rItem.RepairCode.RepairCod;
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        continue;
                    }
                    else
                    {
                        DamageList = new List<Damage>();
                        DamageList = GetDamageCodeAll(rItem.Damage.DamageCedexCode);
                        if (DamageList == null || DamageList.Count == 0)
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Invalid Damage Code for repair :" + rItem.RepairCode.RepairCod;
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                            continue;
                        }
                    }

                    if (rItem.RepairCode.RepairCod.Length != 4)
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Repair Code length must be 4 characters for repair : " + rItem.RepairCode.RepairCod;
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        continue;
                    }
                    else
                    {
                        RepairCodeList = new List<RepairCode>();
                        RepairCodeList = CheckRepairCode(WorkOrder.Mode, rItem.RepairCode.RepairCod);
                        if (RepairCodeList == null || RepairCodeList.Count == 0)
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Invalid Repair Code for repair : " + rItem.RepairCode.RepairCod;
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                            continue;
                        }
                    }

                    //Release 3 RQ6343 Checking the length of the Repeir location code
                    if (rItem.RepairLocationCode.CedexCode.Length != 4)
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Repair Loc Code length must be 4 characters for repair : " + rItem.RepairLocationCode.CedexCode;
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        continue;
                    }
                    else
                    {
                        string temp = rItem.RepairLocationCode.CedexCode.Substring(0, 2);
                        RepairLocationCodeList = new List<RepairLoc>();
                        RepairLocationCodeList = GetRepairLocCode(temp);
                        if (RepairLocationCodeList == null || RepairLocationCodeList.Count == 0)
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Invalid Repair Loc Code for repair : " + rItem.RepairCode.RepairCod;
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                            continue;
                        }
                    }
                    //Release 3 RQ6343 Checking the length of the Tpi code
                    if (rItem.Tpi.CedexCode.Length != 1)
                    {
                        //			fputs("Tpi error",fp);
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Tpi Code length must be 1 character for repair : " + rItem.RepairCode.RepairCod;
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                        continue;
                    }
                    else
                    {
                        TpiList = new List<Tpi>();
                        TpiList = GetTpiCode(rItem.Tpi.CedexCode);
                        if (TpiList == null || TpiList.Count == 0)
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Invalid Tpi Code for repair : " + rItem.RepairCode.RepairCod;
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                            continue;
                        }
                    }

                    // check piece count
                    if (rItem.Pieces == 0)
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Repair piece count is required for repair code: : " + rItem.RepairCode.RepairCod;
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                    }
                    else // not empty, so check if is numeric
                    {
                        if (!IsNumericString(rItem.Pieces.ToString()))
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Repair piece count must be numeric for repair code: : " + rItem.RepairCode.RepairCod;
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                        }
                        else // ensure that this is a whole number.
                        {
                            // (VJP 04-13-2005) Business rule change - no good reason for zero qty repair - causes confusion for users
                            if (rItem.Pieces < 1)
                            {
                                Message = new ErrMessage();
                                Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Repair piece count must not be less than one for repair code: : " + rItem.RepairCode.RepairCod;
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                ErrorMessageList.Add(Message);
                            }
                        }
                    }

                    // check the material amt.
                    if (rItem.MaterialCost > 0)
                    {
                        if (!IsNumericString(rItem.MaterialCost.ToString()))
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Material Amount must be numeric for repair code: : " + rItem.RepairCode.RepairCod;
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                        }
                        else	// check precision.
                        {
                            /* FP FP5724-5725 */
                            //---------Modified for FP7602 - whole number changed from 7 digits to 12 digits.----------
                            if (!IsValidNumericSize(rItem.MaterialCost.ToString(), 12, 4))
                            {
                                Message = new ErrMessage();
                                Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Enter material amount as:<br> (-)n(0-12).n(0-4) where (-) negative is optional and n = 0-9<br>and brackets denote max number of digits.<br> " + rItem.RepairCode.RepairCod;
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                ErrorMessageList.Add(Message);
                            }
                        }
                    }
                    // Check repair man hours - if entered, check numerics.
                    if (rItem.ManHoursPerPiece != null)
                    {
                        if (!IsNumericString(rItem.ManHoursPerPiece.ToString()))
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Man hours must be entered as:<br> n(0-2).n(0-1) where n = 0-9<br>and brackets denote max number of digits.<br> " + rItem.RepairCode.RepairCod;
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                        }
                        else if (rItem.ManHoursPerPiece < 0)
                        {
                            // is numeric - make sure is >= 0;
                            //double d = atof( rItem->m_sManHrs;
                            //if (d < 0.0)
                            {
                                Message = new ErrMessage();
                                Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Man hours cannot be less than zero.<br> " + rItem.RepairCode.RepairCod;
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                ErrorMessageList.Add(Message);
                            }
                        }
                    }
                    else
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Man hours are required<br> " + rItem.RepairCode.RepairCod;
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                    }

                    List<SparePartsView> PartList = new List<SparePartsView>();
                    if (WorkOrder.SparePartsViewList != null && WorkOrder.SparePartsViewList.Count > 0)
                    {
                        PartList = WorkOrder.SparePartsViewList.FindAll(rCode => rCode.RepairCode.RepairCod == rItem.RepairCode.RepairCod);
                    }

                    // iterate through member parts collection.
                    foreach (var pItem in PartList)
                    {
                        if (pItem.pState == (int)Validation.STATE.DELETED)
                            continue;

                        // check 
                        if (string.IsNullOrEmpty(pItem.OwnerSuppliedPartsNumber))
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Missing part number for repair code: : " + pItem.RepairCode.RepairCod;
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                            continue;	// no point in checking anything else on this record
                        }

                        if (lstRepCodeLocPart.Count > 0)
                        {
                            if (lstRepCodeLocPart.Any(str => str.Contains(pItem.OwnerSuppliedPartsNumber)))
                            {
                                Message = new ErrMessage();
                                Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " ," + " Duplicate Part: " + pItem.OwnerSuppliedPartsNumber + ", for repair code:" + pItem.RepairCode.RepairCod;
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                ErrorMessageList.Add(Message);
                            }
                            else
                                lstRepCodeLocPart.Add(pItem.OwnerSuppliedPartsNumber);
                        }
                        else
                            lstRepCodeLocPart.Add(pItem.OwnerSuppliedPartsNumber);

                        if (pItem.Pieces > 0)
                        {
                            if (!IsNumericString(pItem.Pieces.ToString()))
                            {
                                Message = new ErrMessage();
                                Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Part piece count must be numeric: : " + pItem.RepairCode.RepairCod;
                                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                ErrorMessageList.Add(Message);
                            }
                            else
                            {
                                if (pItem.Pieces <= 0)
                                {
                                    Message = new ErrMessage();
                                    Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Part piece count must be greater than zero: : " + pItem.RepairCode.RepairCod;
                                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                                    ErrorMessageList.Add(Message);
                                }
                            }
                        }
                        else
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Part piece count must be greater than zero : " + pItem.RepairCode.RepairCod;
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                        }
                    }
                }

                lstRepCodeLocPart.Clear();
                // Duplicate repair code check and duplicate part check.
                // Ensure that no duplicate repair codes or duplicate parts by repair code are entered.
                foreach (var rItem in WorkOrder.RepairsViewList)
                {
                    // check if deleted.
                    if (rItem.rState == (int)Validation.STATE.DELETED)
                    {
                        continue;
                    }
                    if (lstRepCodeLocPart.Count > 0)
                    {
                        if (lstRepCodeLocPart.Any(str => str.Contains(rItem.RepairCode.RepairCod.Trim() + rItem.RepairLocationCode.CedexCode)))
                        {
                            Message = new ErrMessage();
                            Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " ," + " Repair code: " + rItem.RepairCode.RepairCod + ",Repair Loc code:" + rItem.RepairLocationCode + ", can only be used once for the same repair code.<br>Please delete one and resubmit.";
                            Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                            ErrorMessageList.Add(Message);
                        }
                        else
                            lstRepCodeLocPart.Add(rItem.RepairCode.RepairCod.Trim() + rItem.RepairLocationCode.CedexCode);
                    }
                    else
                        lstRepCodeLocPart.Add(rItem.RepairCode.RepairCod.Trim() + rItem.RepairLocationCode.CedexCode);


                    // Modified for FP 7410 to check unique combination of repair code, repair Loc code
                    //sprintf(cUniqueRepair,": : ",pRepair->GetRepairCd().c_str(),pRepair->GetRepairLocCd().c_str());

                    //strRepair = cUniqueRepair;
                    ////cUniqueRepair = (char*)pRepair->m_sRepairCd.c_str(); 
                    //string sError;
                    //if (! HashTable.InsertUnique(strRepair, i ))
                    //{
                    //    Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Repair code:  " + Eqp.EquipmentNo + " " + " Repair Loc code:  " + Eqp.EquipmentNo + " " + " can only be used once for the same repair code.<br>Please delete one and resubmit.", Eqp.EquipmentNo,pRepair->m_sRepairCd.c_str(), pRepair->m_sRepairLocCd.c_str());

                    //    sError = lpszError;
                    //    ErrorMessageList.Add(Message);
                    //}
                    ////End Modifying
                    //PartHashTable.Empty();
                    //foreach(j=0;j<pRepair->m_PartList.Size();j++)
                    //{
                    //    pPart = (CPartRecord*)pRepair->m_PartList.GetAt( j );

                    //    if (pPart->IsDeleted())
                    //        continue;

                    //    if (! PartHashTable.InsertUnique( pPart->GetPartCode(), j ))
                    //    {
                    //        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Duplicate Part:  " + Eqp.EquipmentNo + " " + " for repair code: : " + Eqp.EquipmentNo + pPart->GetPartCode().c_str(), pRepair->GetRepairCd().c_str() );
                    //        ErrorMessageList.Add(Message);
                    //    }
                    //}
                }

                if (WorkOrder.ImportTax != 0)
                {
                    //----------whole number limit changed from 6 to 10 for import tax - FP7602--------
                    if (!IsValidNumericSize(WorkOrder.ImportTax.ToString(), 10, 4))
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Enter import tax as:<br> n(0-10).n(0-4) where  n = 0-9<br>and brackets denote max number of digits.<br> ";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                    }
                }
                if (WorkOrder.SalesTaxParts != 0)
                {
                    if (!IsValidNumericSize(WorkOrder.SalesTaxParts.ToString(), 6, 4))
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Enter sales tax parts as:<br> n(0-6).n(0-4) where  n = 0-9<br>and brackets denote max number of digits.<br> ";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                    }
                }
                if (WorkOrder.SalesTaxLabour != 0)
                {
                    if (!IsValidNumericSize(WorkOrder.SalesTaxLabour.ToString(), 6, 4))
                    {
                        Message = new ErrMessage();
                        Message.Message = "EQPNO:  " + Eqp.EquipmentNo + " " + " Enter sales tax labour as:<br> n(0-6).n(0-4) where  n = 0-9<br>and brackets denote max number of digits.<br> ";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        ErrorMessageList.Add(Message);
                    }
                }


                //CheckRepairPart(WorkOrder); // FP 6078


                // Any errors, return false. No point in going any further.
                if (ErrorMessageList.Count > 0) return (false);
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            logEntry.Message = "Method Name : ValidateBasics(), End Time : " + DateTime.Now;
            Logger.Write(logEntry.Message);
            return true;
        }

        private bool SetDeletedState(WorkOrderDetail NewWorkOrder, WorkOrderDetail OriginalWorkOrderFromDB)
        {
            bool NewOrDeleted = false;

            //check to see if a repaircode is there in original but not in the new one. set that code as deleted.
            List<RepairsView> DeletedRepairs = new List<RepairsView>();
            List<RepairsView> NewRepairs = new List<RepairsView>();
            List<SparePartsView> DeletedParts = new List<SparePartsView>();
            try
            {
                //FIND THE NEW REP CODE
                foreach (var nrCode in NewWorkOrder.RepairsViewList)
                {
                    if (!OriginalWorkOrderFromDB.RepairsViewList.Any(rCd => rCd.RepairCode.RepairCod == nrCode.RepairCode.RepairCod))
                    {
                        NewOrDeleted = true;
                        nrCode.rState = (int)Validation.STATE.NEW;
                    }
                }

                //Find the deleted Repair Code
                foreach (var drCode in OriginalWorkOrderFromDB.RepairsViewList)
                {
                    if (!NewWorkOrder.RepairsViewList.Any(rCd => rCd.RepairCode.RepairCod == drCode.RepairCode.RepairCod))
                    {
                        NewOrDeleted = true;
                        drCode.rState = (int)Validation.STATE.DELETED;
                        NewWorkOrder.RepairsViewList.Add(drCode);

                    }
                }
                //DeletedRepairs = OriginalWorkOrderFromDB.RepairsViewList.FindAll(rCode => NewWorkOrder.RepairsViewList.Any(repCode => repCode.RepairCode.RepairCod.Trim() != rCode.RepairCode.RepairCod.Trim()));
                //if (DeletedRepairs != null && DeletedRepairs.Count > 0)
                //{
                //    deleted = true;
                //    foreach (var repairs in DeletedRepairs)
                //    {
                //        repairs.rState = (int)Validation.STATE.DELETED;
                //        NewWorkOrder.RepairsViewList.Add(repairs);
                //    }
                //}

                if (NewWorkOrder.SparePartsViewList != null && NewWorkOrder.SparePartsViewList.Count > 0)
                {
                    foreach (var npCode in NewWorkOrder.SparePartsViewList)
                    {
                        if (!OriginalWorkOrderFromDB.SparePartsViewList.Any(rCd => rCd.RepairCode.RepairCod == npCode.RepairCode.RepairCod))
                        {
                            NewOrDeleted = true;
                            npCode.pState = (int)Validation.STATE.NEW;
                        }
                    }

                    foreach (var dpCode in OriginalWorkOrderFromDB.SparePartsViewList)
                    {
                        if (!NewWorkOrder.SparePartsViewList.Any(rCd => rCd.RepairCode.RepairCod == dpCode.RepairCode.RepairCod))
                        {
                            NewOrDeleted = true;
                            dpCode.pState = (int)Validation.STATE.DELETED;
                            NewWorkOrder.SparePartsViewList.Add(dpCode);
                        }
                    }
                    //DeletedParts = OriginalWorkOrderFromDB.SparePartsViewList.FindAll(sCode => NewWorkOrder.SparePartsViewList.Any(spCode => spCode.RepairCode.RepairCod.Trim() != sCode.RepairCode.RepairCod.Trim()));
                    //if (DeletedParts != null && DeletedParts.Count > 0)
                    //{
                    //    NewOrDeleted = true;
                    //    foreach (var parts in DeletedParts)
                    //    {
                    //        parts.pState = (int)Validation.STATE.DELETED;
                    //        NewWorkOrder.SparePartsViewList.Add(parts);
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                NewOrDeleted = false;
            }
            return NewOrDeleted;

        }

        private bool GetContractAmounts(WorkOrderDetail WorkOrder, RepairsView RepairItem, decimal? dRprCeilingLocCurr)
        {
            bool success = true;
            // hold value entered (possible that no shop contract exists, but country contract exists.
            // without a shop contract, no way to calculate relative amount against a shop contract.
            decimal? sHoldCPHAmount = RepairItem.MaterialCostCPH;
            decimal? dEnteredMaterialAmt = RepairItem.MaterialCost;
            decimal? dUSAmount = -1;
            decimal? fExchange = -1;
            decimal? tExchange = -1;
            string ManualCode = WorkOrder.Shop.Customer[0].ManualCode;
            try
            {
                var GetShopContractAmount = (from cont in objContext.MESC1TS_SHOP_CONT
                                             where cont.SHOP_CD == WorkOrder.Shop.ShopCode &&
                                             cont.MANUAL_CD == ManualCode &&
                                             cont.MODE == WorkOrder.Mode &&
                                             cont.REPAIR_CD == RepairItem.RepairCode.RepairCod.Trim() &&
                                             cont.EFF_DTE <= DateTime.Now &&
                                             cont.EXP_DTE >= DateTime.Now
                                             select new
                                             {
                                                 cont.CONTRACT_AMOUNT
                                             }).FirstOrDefault();

                if (GetShopContractAmount != null && GetShopContractAmount.CONTRACT_AMOUNT >= 0)
                {
                    RepairItem.RepairCode.ShopMaterialCeiling = GetShopContractAmount.CONTRACT_AMOUNT;
                    if (RepairItem.rState == (int)Validation.STATE.EXISTING || RepairItem.rState == (int)Validation.STATE.UPDATED)
                    {
                        RepairItem.rState = (int)Validation.STATE.UPDATED;
                    }
                }

                var GetCountryCodeByLocation = (from loc in objContext.MESC1TS_LOCATION
                                                where loc.LOC_CD == WorkOrder.Shop.LocationCode
                                                select loc).FirstOrDefault();



                var GetCountryContractAmount = (from coun in objContext.MESC1TS_COUNTRY_CONT
                                                where coun.COUNTRY_CD == GetCountryCodeByLocation.COUNTRY_CD &&
                                                coun.MANUAL_CD == ManualCode &&
                                                coun.MODE == WorkOrder.Mode &&
                                                coun.REPAIR_CD == RepairItem.RepairCode.RepairCod.Trim() &&
                                                coun.EFF_DTE <= DateTime.Now &&
                                                coun.EXP_DTE >= DateTime.Now
                                                select new
                                                {
                                                    coun.CONTRACT_AMOUNT
                                                }).FirstOrDefault();

                if (GetCountryContractAmount != null && GetCountryContractAmount.CONTRACT_AMOUNT >= 0)
                {
                    dUSAmount = GetCountryContractAmount.CONTRACT_AMOUNT;
                    //}

                    try
                    {
                        var GetCountryContractExchangeInfo = (from curr in objContext.MESC1TS_CURRENCY
                                                              from coun in objContext.MESC1TS_COUNTRY_CONT
                                                              where curr.CUCDN == coun.CUCDN &&
                                                              coun.COUNTRY_CD == GetCountryCodeByLocation.COUNTRY_CD &&
                                                              coun.MANUAL_CD == ManualCode &&
                                                              coun.MODE == WorkOrder.Mode &&
                                                              coun.REPAIR_CD == RepairItem.RepairCode.RepairCod.Trim() &&
                                                              coun.EFF_DTE <= DateTime.Now &&
                                                              coun.EXP_DTE >= DateTime.Now
                                                              select new
                                                              {
                                                                  curr.EXRATUSD,
                                                                  curr.QUOTEDAT,
                                                                  curr.CUCDN
                                                              }).ToList();

                        if (GetCountryContractExchangeInfo != null && GetCountryContractExchangeInfo.Count > 0)
                        {
                            fExchange = GetCountryContractExchangeInfo[0].EXRATUSD == null ? -1 : GetCountryContractExchangeInfo[0].EXRATUSD;

                            if (GetCountryContractExchangeInfo[0].QUOTEDAT != null)
                            {
                                WorkOrder.CountryExchangeDate = GetCountryContractExchangeInfo[0].QUOTEDAT;
                            }

                            WorkOrder.CountryCUCDN = GetCountryContractExchangeInfo[0].CUCDN;
                            // FACT changes - hold shop exchange as country exchange - might be replaced if country contracts exist
                            WorkOrder.CountryExchangeRate = Math.Round(fExchange.Value, 6);
                            tExchange = WorkOrder.CountryExchangeRate;
                        }
                    }

                    catch (Exception ex)
                    {
                        Message.Message = "Contract->GetCountryContractExchangeInfo: Failed retrieve of country contract info";
                        Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                        success = false;
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);

                    }

                    if (tExchange > 0)
                    {
                        dUSAmount *= (tExchange * (decimal).01);
                    }
                    RepairItem.MaterialCostCPH = Math.Round(dUSAmount.Value, 4);
                }

                if (dUSAmount >= 0)
                {
                    if (RepairItem.RepairCode.ShopMaterialCeiling >= 0)
                    {
                        if (dEnteredMaterialAmt < RepairItem.RepairCode.ShopMaterialCeiling)
                        {
                            if (RepairItem.RepairCode.ShopMaterialCeiling != 0)
                            {
                                decimal? dPercent = dEnteredMaterialAmt / RepairItem.RepairCode.ShopMaterialCeiling;
                                // reduce CPH amount by same percentage.
                                dUSAmount *= dPercent;
                                // re-format CPH amount.
                                //				sprintf( msg, "%.2f", dUSAmount);
                                //sprintf(msg, "%.4f", dUSAmount);
                                RepairItem.MaterialCostCPH = Math.Round(dUSAmount.Value, 4);
                            }
                            else
                            {	// if shop material ceiling is zero then percen of materials entered is 0.
                                RepairItem.MaterialCostCPH = 0;
                            }
                        }
                    }
                    else
                    {
                        RepairItem.MaterialCostCPH = sHoldCPHAmount;
                    }
                }
                else
                {
                    // no shop contract - use original amount entered for CPH amount.
                    RepairItem.MaterialCostCPH = sHoldCPHAmount;

                    /* removed - it was determined that it is better to simply use amount entered as CPH amount
                    // since we cannot really calculate a CPH relative amount against the global repair ceiling.
                    // no shop contract, so calculate relative value against repair code ceiling (already converted to local currency).
                    if (dRprCeilingLocCurr > 0)
                    {
                        if (dEnteredMaterialAmt < dRprCeilingLocCurr)
                        {
                            double dPercent = dEnteredMaterialAmt / dRprCeilingLocCurr;
                            // reduce CPH amount by same percentage.
                            dUSAmount *= dPercent;
                            // re-format CPH amount.
                            sprintf( msg, "%.2f", dUSAmount);
                            pRepair->m_sMaterialAmtCPH = msg;
                        }
                    }
                    */
                }
            }
            catch
            {
            }



            return success;
        }

        private string CheckSuspendTable(WorkOrderDetail WorkOrder)
        {
            //bool success = true;
            string errMsg = "";

            try
            {
                var RSByShopCode = (from SC in objContext.MESC1TS_SUSPEND_CAT
                                    from SU in objContext.MESC1TS_SUSPEND
                                    where SU.SHOP_CD == WorkOrder.Shop.ShopCode &&
                                    SC.SUSPCAT_ID == SU.SUSPCAT_ID
                                    select new
                                    {
                                        SU.MANUAL_CD,
                                        SU.MODE,
                                        SU.REPAIR_CD,
                                        SC.SUSPCAT_DESC
                                    }).ToList();

                if (RSByShopCode != null && RSByShopCode.Count > 0)
                {
                    foreach (var rItem in WorkOrder.RepairsViewList)
                    {
                        RSByShopCode = RSByShopCode.FindAll(susp => susp.REPAIR_CD.Trim() == rItem.RepairCode.RepairCod.Trim() && susp.MANUAL_CD == WorkOrder.Shop.Customer[0].ManualCode
                                                                && susp.MODE == WorkOrder.Mode);

                        if (RSByShopCode != null && RSByShopCode.Count > 0)
                        {
                            WorkOrder.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "Suspended due to use of mode " + WorkOrder.Mode + " and repair code " + rItem.RepairCode.RepairCod));
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                errMsg = "RSByShopCode: Failed read of suspend table";
                //success = false;
            }

            return errMsg;
        }

        #region MgrApproval

        public bool AuthenticateShopCodeByUserID(string ShopCode, int UserID)
        {
            MESC1VS_SHOP_LOCATION SelectedShop = null;
            List<MESC1VS_SHOP_LOCATION> ShopFromDBOnAuth = new List<MESC1VS_SHOP_LOCATION>();
            bool success = true;
            try
            {
                ShopFromDBOnAuth = GenerateShopList(UserID);
                SelectedShop = ShopFromDBOnAuth.Find(sCode => sCode.SHOP_CD == ShopCode);

                if (SelectedShop != null)
                {
                    success = true;
                }
                else
                {
                    success = false;
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }

        public List<ErrMessage> AddRemarkByTypeAndWOID(int WOID, string Remarks, string RemarksType, string ChangeUser)
        {

            RemarkEntry RemarksToBeSaved = new RemarkEntry();
            List<ErrMessage> errorMessageList = new List<ErrMessage>();

            try
            {
                //InsertRemarkByType
                //WO.RemarksList.Add(new RemarkEntry(Validation.GetCurrentTimeString(), "S", null, "Suspected 3rd party damage - pls supply location of responsible party."));
                RemarksToBeSaved.ChangeUser = ChangeUser;
                RemarksToBeSaved.Remark = Remarks;
                RemarksToBeSaved.WorkOrderID = WOID;
                RemarksToBeSaved.RemarkType = RemarksType;
                SaveRemark(RemarksToBeSaved, WOID, ChangeUser, DateTime.Now, out errorMessageList);

                //if (m_WOList.Size() > 0)
                //{	// find work order if existing
                //    for (int i = 0; i < m_WOList.Size(); i++)
                //    {
                //        pWO = (CWorkOrderRecord*)m_WOList.GetAt(i);
                //        if (strcmp((char*)tWOID, pWO->GetWO_ID().c_str()) == 0)
                //        {
                //            CRemarkRecord* pRemark = new CRemarkRecord(m_StringUtil.GetCurrentTimeString(), (char*)sType, "", (char*)tRemark, (char*)tUser);
                //            pRemark->SetObjectState(EXISTING);
                //            pWO->m_RemarkList.AddTail(pRemark);
                //            break;
                //        }
                //    }
                //}
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return errorMessageList;
        }

        public List<ErrMessage> UpdateThirdPartyCause(int WOID, string NewThirdParty, string NewCause, string ChangeUser)
        {
            //bool bFound = false;
            List<ErrMessage> ErrorMessageList = new List<ErrMessage>();

            string OldThirdParty = string.Empty;
            string OldCause = string.Empty;
            MESC1TS_WO WO = null;
            MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
            string WorkOrderID = WOID.ToString();
            string AuditComment = string.Empty;

            try
            {
                // safety - no wo_id - just exit - nothing to do
                //if (WOID == 0) return;

                //first check the third party port
                CheckThirdPartyPort(NewThirdParty, out ErrorMessageList);
                if (ErrorMessageList != null && ErrorMessageList.Count > 0)
                {
                    return ErrorMessageList;
                }
                else
                {
                    WO = (from wo in objContext.MESC1TS_WO
                          where wo.WO_ID == WOID
                          select wo).FirstOrDefault();

                    if (WO != null)
                    {
                        //bFound = true; // we know that the record exists.
                        OldThirdParty = WO.THIRD_PARTY;
                        OldCause = WO.CAUSE;
                    }
                }
            }

            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "DB failure - get thirdParty and cause check: ";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                ErrorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            //if (!bFound)
            //{
            //    return;
            //}

            try
            {
                // if differences found, save and send audit-trail.
                if (OldThirdParty.Equals(NewThirdParty, StringComparison.CurrentCultureIgnoreCase) || OldCause.Equals(NewCause, StringComparison.CurrentCultureIgnoreCase))
                {
                    WO = (from wo in objContext.MESC1TS_WO
                          where wo.WO_ID == WOID
                          select wo).FirstOrDefault();

                    if (WO != null)
                    {
                        WO.THIRD_PARTY = NewThirdParty.TrimEnd();
                        WO.CAUSE = NewCause;
                        WO.CHUSER = ChangeUser;
                        objContext.SaveChanges();
                        if (OldThirdParty.Equals(NewThirdParty, StringComparison.CurrentCultureIgnoreCase))
                        {
                            AuditComment = "Work Order: " + WorkOrderID.PadRight(8) + "<b> Thirdparty changed from  " + OldThirdParty + " to " + NewThirdParty + "</b> by " + ChangeUser;
                        }
                        else if (OldCause.Equals(NewCause, StringComparison.CurrentCultureIgnoreCase))
                        {
                            AuditComment = "Work Order: " + WorkOrderID.PadRight(8) + "<b> Cause changed from  " + OldThirdParty + " to " + NewThirdParty + "</b> by " + ChangeUser;
                        }




                        WOAudit.WO_ID = WOID;
                        WOAudit.CHTS = DateTime.Now;
                        WOAudit.AUDIT_TEXT = AuditComment;
                        WOAudit.CHUSER = ChangeUser;
                        objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                        objContext.SaveChanges();
                    }
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ErrorMessageList;
        }

        public List<ErrMessage> UpdateRepairDateByWOID(int WOID, DateTime? NewRepairDate, string ChangeUser)
        {
            MESC1TS_WO WO = null;
            List<ErrMessage> errorMessageList = new List<ErrMessage>();

            try
            {
                // safety - no wo_id - just exit - nothing to do
                //if (WOID == 0) return;

                // check if date is > current date
                // IsExpired returns 0 if equal, 1 is currentdate is > and -1 if current date is <
                if (NewRepairDate > DateTime.Now)
                {
                    Message = new ErrMessage();
                    Message.Message = "Unable to Complete Estimate. Repair date must not be older than 1st day of previous month, nor later than the current date (UTC)";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    return errorMessageList;
                }

                // date is valid and not > than current date, now ensure that date is not less than 1st day of previous month
                // Ensure that date is not < 1st day of previous month.
                // build oldest allowed date.
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                if (month > 1)
                {
                    month -= 1;
                }
                else
                {
                    month = 12;
                    year -= 1;
                }

                DateTime setDate = (DateTime.Now.Date);
                DateTime dt = (DateTime)NewRepairDate;
                //string tempRepDate = dt.ToString();
                //Incomplete

                if (dt.Date < setDate)
                {
                    Message = new ErrMessage();
                    Message.Message = "Unable to Complete Estimate. Repair date must not be older than 1st day of previous month, nor later than the current date (UTC)";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    errorMessageList.Add(Message);
                    return errorMessageList;
                }

                DateTime? OldRepairDate;

                WO = (from wo in objContext.MESC1TS_WO
                      where wo.WO_ID == WOID
                      select wo).FirstOrDefault();

                if (WO != null)
                {
                    OldRepairDate = WO.REPAIR_DTE;
                    if (NewRepairDate != OldRepairDate)
                    {
                        string AuditComment = string.Empty;
                        MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();

                        WO.REPAIR_DTE = NewRepairDate;
                        WO.CHUSER = ChangeUser;
                        objContext.SaveChanges();

                        //if(OldRepairDate == null) 
                        AuditComment = "Work Order: " + WOID + "<b> RepairDate changed from " + OldRepairDate + " to" + NewRepairDate + "</b> by " + ChangeUser;
                        WOAudit.WO_ID = WOID;
                        WOAudit.CHTS = DateTime.Now;
                        WOAudit.AUDIT_TEXT = AuditComment;
                        WOAudit.CHUSER = ChangeUser;
                        objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                        objContext.SaveChanges();
                    }
                }



                //if (m_WOList.Size() > 0)
                //{
                //    for (int i = 0; i < m_WOList.Size(); i++)
                //    {
                //        pWO = (CWorkOrderRecord*)m_WOList.GetAt(i);
                //        if (strcmp((char*)tWOID, pWO->GetWO_ID().c_str()) == 0)
                //        {
                //            STATE stat = pWO->GetObjectState();
                //            pWO->SetREPAIR_DTE((char*)tRepairDte);
                //            pWO->SetObjectState(stat);
                //            break;
                //        }
                //    }
                //}
            }

            catch (Exception ex)
            {
                Message = new ErrMessage();
                Message.Message = "Failed update on work order - UpdateRepairDate.";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                errorMessageList.Add(Message);
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return errorMessageList;
            }

            return errorMessageList;
        }

        public List<ErrMessage> UpdateSerialNumber(int WOID, string RepairCode, string PartNumber, string SerialNumber, string ChangeUser)
        {
            MESC1TS_WOPART WO = null;
            List<ErrMessage> errorMessageList = new List<ErrMessage>();
            MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
            string AuditComment = string.Empty;

            try
            {
                // safety - no wo_id - just exit - nothing to do
                // if (WOID == 0) return;

                WO = (from wo in objContext.MESC1TS_WOPART
                      where wo.WO_ID == WOID &&
                      wo.REPAIR_CD == RepairCode.Trim() &&
                      wo.PART_CD == PartNumber
                      select wo).FirstOrDefault();

                if (WO != null)
                {
                    WO.SERIAL_NUMBER = SerialNumber;
                    WO.CHUSER = ChangeUser;
                    objContext.SaveChanges();

                    AuditComment = "Work Order: " + WOID + "<b> Core part serial number entered for RepairCode: " + RepairCode + ", PartCode: " + PartNumber + "</b>";
                    WOAudit.WO_ID = WOID;
                    WOAudit.CHTS = DateTime.Now;
                    WOAudit.AUDIT_TEXT = AuditComment;
                    WOAudit.CHUSER = ChangeUser;
                    objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                    objContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return errorMessageList;
        }

        public List<ErrMessage> UpdateRevNo(int WOID)
        {
            MESC1TS_WO WO = null;
            List<ErrMessage> ErrorMessageList = new List<ErrMessage>();
            short? RevNo = 0;
            try
            {
                RevNo = GetRevNo(WOID);

                WO = (from wo in objContext.MESC1TS_WO
                      where wo.WO_ID == WOID
                      select wo).FirstOrDefault();

                if (WO != null)
                {
                    WO.REVISION_NO = RevNo;
                    objContext.SaveChanges();
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ErrorMessageList;
        }

        public List<ErrMessage> UpdateShopWorkingSwitch(int WOID, string Switch, string ChangeUser)
        {
            List<ErrMessage> MercPlusMessageList = new List<ErrMessage>();
            MESC1TS_WO WO = new MESC1TS_WO();
            try
            {
                // if swithch not = N or Y then return false;
                if (!Switch.Equals("Y", StringComparison.CurrentCultureIgnoreCase) && !Switch.Equals("N", StringComparison.CurrentCultureIgnoreCase))
                {
                    Message = new ErrMessage();
                    Message.Message = "";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    MercPlusMessageList.Add(Message);
                    return MercPlusMessageList;
                }
                WO = (from wo in objContext.MESC1TS_WO
                      where wo.WO_ID == WOID
                      select wo).FirstOrDefault();

                if (WO != null)
                {
                    //OldRepairDate = WO.REPAIR_DTE;
                    //if (NewRepairDate != OldRepairDate)
                    {
                        string AuditComment = string.Empty;
                        MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();

                        WO.SHOP_WORKING_SW = Switch;
                        WO.CHUSER = ChangeUser;
                        WO.CHTS = DateTime.Now;
                        objContext.SaveChanges();

                        //if(OldRepairDate == null) 
                        AuditComment = "Work Order: " + WOID + "<b> working switch set to " + Switch + "</b> by " + ChangeUser;
                        WOAudit.WO_ID = WOID;
                        WOAudit.CHTS = DateTime.Now;
                        WOAudit.AUDIT_TEXT = AuditComment;
                        WOAudit.CHUSER = ChangeUser;
                        objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                        objContext.SaveChanges();
                    }
                }


            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return MercPlusMessageList;
        }

        public Dictionary<string, object> GetPrevStatusDateLoc(int WOID, string WOIndicator)
        {
            MESC1TS_WO WO = null;
            WorkOrderDetail WorkOrder = new WorkOrderDetail();
            short? PrevStatus = null;
            DateTime? PrevDate = null;
            string PrevLocCode = string.Empty;
            string StatusDesc = string.Empty;
            string PrevStatusDesc = string.Empty;
            Dictionary<string, object> _data = new Dictionary<string, object>();

            try
            {
                if (WOIndicator.Equals("WO_ID", StringComparison.CurrentCultureIgnoreCase))
                {
                    WO = (from wo in objContext.MESC1TS_WO
                          where wo.WO_ID == WOID
                          select wo).FirstOrDefault();

                    if (WO != null)
                    {
                        if (390 <= WO.STATUS_CODE)
                        {
                            var tempWO1 = (from w in objContext.MESC1TS_WO
                                           from s in objContext.MESC1TS_STATUS_CODE
                                           where w.PREV_STATUS == s.STATUS_CODE &&
                                           w.WO_ID == WOID
                                           select new
                                           {
                                               w.WO_ID,
                                               w.PREV_STATUS,
                                               w.PREV_DATE,
                                               w.PREV_LOC_CD,
                                               s.STATUS_DSC
                                           }).FirstOrDefault();

                            if (tempWO1 != null)
                            {
                                PrevStatus = tempWO1.PREV_STATUS;
                                PrevLocCode = tempWO1.PREV_LOC_CD;
                                PrevDate = tempWO1.PREV_DATE;
                                StatusDesc = tempWO1.STATUS_DSC;
                            }
                        }

                        else
                        {
                            var tempWO2 = (from w in objContext.MESC1TS_WO
                                           from s in objContext.MESC1TS_SHOP
                                           from sc in objContext.MESC1TS_STATUS_CODE
                                           where w.EQPNO == WO.EQPNO &&
                                                 w.CRTS < WO.CRTS &&
                                           sc.STATUS_CODE == w.STATUS_CODE &&
                                           w.SHOP_CD == s.SHOP_CD
                                           select new
                                           {
                                               w.WO_ID,
                                               PREV_STATUS = w.STATUS_CODE,
                                               PREV_DATE = w.CHTS,
                                               PREV_LOC_CD = s.LOC_CD,
                                               sc.STATUS_DSC,
                                               w.CRTS
                                           }).OrderByDescending(wo => wo.CRTS).FirstOrDefault();

                            if (tempWO2 != null)
                            {
                                PrevStatus = tempWO2.PREV_STATUS;
                                PrevLocCode = tempWO2.PREV_LOC_CD;
                                PrevDate = tempWO2.PREV_DATE;
                                StatusDesc = tempWO2.STATUS_DSC;
                            }
                        }
                    }
                }
                else
                {
                    var tempWO3 = (from w in objContext.MESC1TS_WO
                                   from s in objContext.MESC1TS_SHOP
                                   from sc in objContext.MESC1TS_STATUS_CODE
                                   where sc.STATUS_CODE == w.STATUS_CODE &&
                                   w.SHOP_CD == s.SHOP_CD &&
                                   w.WO_ID == WOID
                                   select new
                                   {
                                       PREV_STATUS = w.STATUS_CODE,
                                       PREV_DATE = w.CHTS,
                                       PREV_LOC_CD = s.LOC_CD,
                                       sc.STATUS_DSC
                                   }).FirstOrDefault();

                    if (tempWO3 != null)
                    {
                        PrevStatus = tempWO3.PREV_STATUS;
                        PrevLocCode = tempWO3.PREV_LOC_CD;
                        PrevDate = tempWO3.PREV_DATE;
                        StatusDesc = tempWO3.STATUS_DSC;
                    }

                }

                if (PrevStatus != null)
                {
                    var RS = (from s in objContext.MESC1TS_STATUS_CODE
                              where s.STATUS_CODE == PrevStatus
                              select new
                              {
                                  s.STATUS_DSC
                              }).FirstOrDefault();

                    if (RS != null)
                    {
                        PrevStatusDesc = RS.STATUS_DSC;
                    }
                }
                WorkOrder.PrevDate = PrevDate;
                WorkOrder.PrevLocCode = PrevLocCode;
                WorkOrder.PrevStatus = PrevStatus;
                WorkOrder.Status = StatusDesc;
                //PreviousData.Add(PrevStatus, StatusDesc);
                //PreviousData.Add(PrevLocCode, PrevDate);

                _data.Add("PREVDATE", PrevDate);
                _data.Add("PREVSTATUS", PrevStatus);
                _data.Add("PREVLOCCODE", PrevLocCode);
                _data.Add("STATUSDESC", StatusDesc);
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return _data;
        }

        private string RSAllCSM(string CustomerCode, string ShopCode, string ModeCode)
        {
            string PayAgentCode = string.Empty;
            try
            {
                var CSMFromDB = (from csm in objContext.MESC1TS_CUST_SHOP_MODE
                                 where csm.SHOP_CD == ShopCode &&
                                 csm.CUSTOMER_CD == CustomerCode &&
                                 csm.MODE == ModeCode
                                 select new
                                 {
                                     csm.PAYAGENT_CD
                                 }).FirstOrDefault();

                if (CSMFromDB != null)
                {
                    PayAgentCode = CSMFromDB.PAYAGENT_CD;
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return PayAgentCode;
        }

        public string RSByMfgAndModel(string eqpRUType)
        {
            string IndicatorCode = string.Empty;

            var InCode = (from model in objContext.MESC1TS_MODEL
                          where model.MODEL_NO == eqpRUType
                          select new
                          {
                              model.INDICATOR_CD
                          }).FirstOrDefault();

            if (InCode != null)
            {
                IndicatorCode = InCode.INDICATOR_CD;
            }

            return IndicatorCode;
        }

        public List<ErrMessage> CompleteWorkOrderByID(int WOID, DateTime RepairDate, string ChangeUser)
        {
            bool success = true;
            MESC1TS_WO WorkOrderFromDB = new MESC1TS_WO();
            WorkOrderDetail WorkOrder = new WorkOrderDetail();
            List<ErrMessage> ErrorMessageList = new List<ErrMessage>();

            try
            {
                WorkOrder = GetWorkOrderDetails(WOID);

                if (WorkOrder != null)
                {
                    WorkOrder.RepairDate = RepairDate;
                    WorkOrder.ChangeUser = ChangeUser;
                    WorkOrder.woState = (int)Validation.STATE.EXISTING;

                    success = CompleteWorkOrder(WorkOrder, out ErrorMessageList);
                    if (ErrorMessageList == null || ErrorMessageList.Count == 0)
                    {
                        Message = new ErrMessage();
                        Message.Message = "WorkOrder Completed";
                        Message.ErrorType = Validation.MESSAGETYPE.SUCCESS.ToString();
                        ErrorMessageList.Add(Message);
                    }
                }
            }

            catch
            {
            }

            return ErrorMessageList;
        }

        private bool CompleteWorkOrder(WorkOrderDetail WorkOrder, out List<ErrMessage> ErrorMessageList)
        {
            ErrorMessageList = new List<ErrMessage>();
            bool success = true;

            // Check for changes, if changes, set error  and return.
            if (RequiresValidation(WorkOrder))
            {
                Message = new ErrMessage();
                Message.Message = "Changes detected - work order not completed. Please re-submit work order for approval";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                ErrorMessageList.Add(Message);
                success = false;
                return (success);
            }
            // Ensure that status is 390, i.e., 'approved'. else do not complete this work order.
            if (WorkOrder.WorkOrderStatus != APPROVEDSTATUS)
            {
                Message = new ErrMessage();
                Message.Message = "Estimate must be approved before it can be completed.";
                Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                ErrorMessageList.Add(Message);
                success = false;
                return (success);
            }

            // Do we fill in repair date as current date if not included? - don't think so...UI will provide current date
            // Run repair date through validations before attempting to complete WO.
            if (!CheckRepairDate(WorkOrder, out ErrorMessageList))
            {
                success = false;
                return (success);
            }

            // need to check for core parts.???

            // if no problems, set status code to completed 400 for imminent save.
            if (success)
            {
                // set status to completed and set work order state to updated.
                WorkOrder.WorkOrderStatus = COMPLETEDWO;
                WorkOrder.woState = (int)Validation.STATE.UPDATED;
            }


            // if no problems in preparing Work order - attempt to save changes
            if (success)
            {
                try
                {
                    //			DBPlug.Execute( sql.GetUpdateHeaderSQL( rhsRecord ) );
                    success = SaveWorkOrder(WorkOrder, out ErrorMessageList);
                    if (ErrorMessageList != null && ErrorMessageList.Count > 0)
                    {
                        success = false;
                    }
                }
                catch (Exception ex)
                {
                    Message = new ErrMessage();
                    Message.Message = "System error while saving work order - work order not saved";
                    Message.ErrorType = Validation.MESSAGETYPE.ERROR.ToString();
                    ErrorMessageList.Add(Message);
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }

            }

            return (success);
        }

        //public List<RemarkEntry> RSRemarkView(int WOID, string RemarksType)
        //{
        //    List<RemarkEntry> RemarksList = new List<RemarkEntry>();
        //    List<MESC1TS_WOREMARK> RemarksFromDB = new List<MESC1TS_WOREMARK>();
        //    string a = string.Empty;
        //    try
        //    {
        //        RemarksFromDB = (from rem in objContext.MESC1TS_WOREMARK
        //                         where rem.WO_ID == WOID &&
        //                         (RemarksType == "" ? a == a : rem.REMARK_TYPE == RemarksType)
        //                         select rem).ToList();

        //        if (RemarksFromDB != null && RemarksFromDB.Count > 0)
        //        {
        //            foreach (var item in RemarksFromDB)
        //            {
        //                RemarkEntry Remarks = new RemarkEntry();
        //                Remarks.Remark = item.REMARK;
        //                Remarks.CRTSDate = item.CRTS.ToString();
        //                Remarks.ChangeUser = item.CHUSER;
        //                Remarks.WorkOrderID = item.WO_ID;
        //            }
        //        }
        //    }

        //    catch
        //    {
        //    }
        //}
        #endregion MgrApproval

        #region CreateWOSummary
        public List<SparePartsView> RSSerialNumberParts(int WOID)
        {
            List<SparePartsView> SparePartsList = new List<SparePartsView>();
            //List<MESC1TS_WOPART> SparePartsFromDB = new List<MESC1TS_WOPART>();
            try
            {
                var SparePartsFromDB = (from wp in objContext.MESC1TS_WOPART
                                        from p in objContext.MESC1TS_MASTER_PART
                                        where (wp.SERIAL_NUMBER == null || wp.SERIAL_NUMBER == "") &&
                                        p.CORE_PART_SW == "Y" &&
                                        wp.WO_ID == WOID &&
                                        wp.PART_CD == p.PART_CD
                                        select new { wp.SERIAL_NUMBER, p.CORE_VALUE }).ToList();

                foreach (var item in SparePartsFromDB)
                {
                    SparePartsView SpareParts = new SparePartsView();
                    SpareParts.CoreValue = item.CORE_VALUE;
                    SpareParts.SerialNumber = item.SERIAL_NUMBER;
                    SparePartsList.Add(SpareParts);
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return SparePartsList;
        }

        public string GetRepairDesc(string RepairCode, String Mode, String Manual)
        {
            string repDesc = string.Empty;
            try
            {
                var RepCode = (from r in objContext.MESC1TS_REPAIR_CODE
                               where r.REPAIR_CD == RepairCode.Trim() &&
                                  r.MODE == Mode &&
                                  r.MANUAL_CD == Manual
                               select new
                               {
                                   r.REPAIR_DESC
                               }).FirstOrDefault();
                if (RepCode != null)
                {
                    repDesc = RepCode.REPAIR_DESC;
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return repDesc;
        }

        public string GetPartDescription(string PartCode)
        {
            bool bDone = false;
            decimal? Price = 0;
            string Description = string.Empty;

            try
            {
                while (!bDone)
                {
                    var Part = (from p in objContext.MESC1TS_MASTER_PART
                                where p.PART_CD == PartCode
                                select new
                                {
                                    p.PART_DESC,
                                    p.PART_PRICE
                                }).ToList();

                    if (Part != null)
                    {
                        foreach (var item in Part)
                        {
                            if (item.PART_DESC != null)
                            {
                                Description = item.PART_DESC;
                                Price = (item.PART_PRICE == null ? 0 : item.PART_PRICE);
                                if (Price > 0)
                                {
                                    bDone = true;
                                }
                                else
                                {
                                    PartCode = Description;
                                }
                            }
                            else
                            {
                                Description = string.Empty;
                                bDone = true;
                            }
                        }
                    }
                    else
                    {
                        bDone = true;
                    }
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return Description;
        }
        #endregion CreateWOSummary

        private string GetTrimmedString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            else
                return value.Trim();
        }

        private Equipment ParseRKEMData(string RKEMMessage)
        {
            ///WorkOrder.EquipmentList = new List<Equipment>();
            Equipment RKEMEqp = new Equipment();
            string sWhole = string.Empty;
            string doubleParse = string.Empty;
            try
            {
                RKEMEqp.EquipmentNo = GetTrimmedString(RKEMMessage.Substring(EQUIPPOS, EQUIPLEN));
                RKEMEqp.Size = GetTrimmedString(RKEMMessage.Substring(COSIZEPOS, COSIZELEN));
                RKEMEqp.Eqouthgu = GetTrimmedString(RKEMMessage.Substring(EQOUTHGUPOS, EQOUTHGULEN));
                RKEMEqp.COType = GetTrimmedString(RKEMMessage.Substring(COTYPEPOS, COTYPELEN));
                RKEMEqp.Eqstype = GetTrimmedString(RKEMMessage.Substring(EQSTYPPOS, EQSTYPLEN));
                // Get OWNTYPE and hold for later validation as the EQOWNTP may be changed
                // for certain validations.
                RKEMEqp.Eqowntp = GetTrimmedString(RKEMMessage.Substring(EQOWNTPPOS, EQOWNTPLEN));
                //RKEMEqp.RKEM_EQOWNTP = RKEMMessage.Substring( EQOWNTPPOS, EQOWNTPLEN );		

                //RKEMEqp.Material = RKEMMessage.Substring(EQMATRPOS, EQMATRLEN);
                RKEMEqp.Eqmatr = GetTrimmedString(RKEMMessage.Substring(EQMATRPOS, EQMATRLEN));

                // Note DELDATSH will return with year 0001 - should be NULL in DB
                // Therefore force field to empty.
                string TempDate;
                TempDate = GetTrimmedString(RKEMMessage.Substring(DELDATSHPOS, DELDATSHLEN));
                DateTime date;
                if (!string.IsNullOrEmpty(TempDate))
                {
                    date = DateTime.Parse(TempDate);
                    string yr = date.Year.ToString();
                    if (yr.Equals("0001", StringComparison.CurrentCultureIgnoreCase))
                    {
                        RKEMEqp.Deldatsh = null;
                    }
                    else
                    {
                        RKEMEqp.Deldatsh = DateTime.Parse(TempDate);
                    }
                }

                //char s[20]; memset(s, '\0', 20);
                //strcpy( s, WorkOrder.Deldatsh.c_str() );
                //if (strlen( s ) > 4)
                //    if (strncmp( s, "0001", 4) == 0)
                //        WorkOrder.Deldatsh = null;

                RKEMEqp.StEmptyFullInd = GetTrimmedString(RKEMMessage.Substring(STEMPTYPOS, STEMPTYLEN));
                RKEMEqp.Strefurb = GetTrimmedString(RKEMMessage.Substring(STREFURBPOS, STREFURBLEN));
                TempDate = GetTrimmedString(RKEMMessage.Substring(REFRBDATPOS, REFRBDATLEN));
                RKEMEqp.RefurbishmnentDate = DateTime.Parse(TempDate);
                RKEMEqp.Stredel = GetTrimmedString(RKEMMessage.Substring(STREDELPOS, STREDELLEN));

                // Infix decimal after 5th character - mainframe using pic 9(5)V9(2)
                doubleParse = GetTrimmedString(RKEMMessage.Substring(FIXCOVERPOS, FIXCOVERLEN));
                sWhole = doubleParse.Substring(0, 5);
                sWhole += ".";
                sWhole += doubleParse.Substring(5, 2);
                RKEMEqp.Fixcover = double.Parse(doubleParse, System.Globalization.CultureInfo.InvariantCulture);

                // Infix decimal after 5th character - mainframe using pic 9(5)V9(2)
                doubleParse = GetTrimmedString(RKEMMessage.Substring(DPPPOS, DPPLEN));
                sWhole = doubleParse.Substring(0, 5);
                sWhole += ".";
                sWhole += doubleParse.Substring(5, 2);
                RKEMEqp.Dpp = double.Parse(doubleParse, System.Globalization.CultureInfo.InvariantCulture);

                RKEMEqp.OffhirLocationSW = GetTrimmedString(RKEMMessage.Substring(OFFHIREPOS, OFFHIRELEN));
                RKEMEqp.STSELSCR = GetTrimmedString(RKEMMessage.Substring(STSELSCRPOS, STSELSCRLEN));
                RKEMEqp.EqMancd = GetTrimmedString(RKEMMessage.Substring(EQMANCDPOS, EQMANCDLEN));

                // strip off initial '*' sent in the profile.
                //memset(msg,'\0',256);
                RKEMEqp.EQProfile = GetTrimmedString(RKEMMessage.Substring(EQPROFILPOS, EQPROFILLEN));
                //strcpy(msg, RKEMEqp.EQProfile.c_str());
                if (!string.IsNullOrEmpty(RKEMEqp.EQProfile))
                {
                    if (RKEMEqp.EQProfile.StartsWith("*"))
                    {
                        RKEMEqp.EQProfile = GetTrimmedString(RKEMEqp.EQProfile.Substring(1, RKEMEqp.EQProfile.Length - 1));
                    }
                }

                TempDate = GetTrimmedString(RKEMMessage.Substring(EQINDATPOS, EQINDATLEN));
                if (!string.IsNullOrEmpty(TempDate))
                {
                    RKEMEqp.EQInDate = DateTime.Parse(TempDate);
                    //RKEMEqp.InService = TempDate;
                }
                RKEMEqp.EQRuman = GetTrimmedString(RKEMMessage.Substring(EQRUMANPOS, EQRUMANLEN));
                RKEMEqp.EQRutyp = GetTrimmedString(RKEMMessage.Substring(EQRUTYPPOS, EQRUTYPLEN));
                RKEMEqp.EQIoflt = GetTrimmedString(RKEMMessage.Substring(EQIOFLTPOS, EQIOFLTLEN));

                // new RKEM fields. VJP 2006-07-17
                RKEMEqp.Damage = GetTrimmedString(RKEMMessage.Substring(DAMAGEPOS, DAMAGELEN));
                RKEMEqp.PLAWHO = GetTrimmedString(RKEMMessage.Substring(PLAWHOPOS, PLAWHOLEN));
                // VJP - added 2006-07-25
                RKEMEqp.LeasingCompany = GetTrimmedString(RKEMMessage.Substring(LSCOMPPOS, LSCOMPLEN));
                RKEMEqp.LeasingContract = GetTrimmedString(RKEMMessage.Substring(LSCONTRPOS, LSCONTRLEN));

                // RQ 6361 & RQ 6362 (Satadal)
                // Added for two new rkem fields in release 3.0
                // Parsing of data is done here
                RKEMEqp.PresentLoc = GetTrimmedString(RKEMMessage.Substring(PRESENTLOCPOS, PRESENTLOCLEN));
                TempDate = GetTrimmedString(RKEMMessage.Substring(GATEINDTEPOS, GATEINDTELEN));
                if (!string.IsNullOrEmpty(TempDate))
                {
                    RKEMEqp.GateInDate = DateTime.Parse(TempDate);
                }
                //WorkOrder.EquipmentList.Add(RKEMEqp);
                //End
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return RKEMEqp;

        }

        public int LoadEquipmentRefNo(string EqpNo, string ShopCode, string VendorRefNo, ref Equipment RKEMEqp)
        {
            string RKEMMessage = string.Empty;
            RKEMEqp = new Equipment();
            int iRtn = (int)NOTFOUND;
            //WorkOrder = new WorkOrderDetail();
            MQMessage mqMsg = new MQMessage();
            try
            {
                // Return 0 if Container size contains information , i.e., RKEM performed already
                //if (RKEMEqp.COType != null)
                //{
                //    //WorkOrder.woState = (int)Validation.STATE.NEW;
                //    RKEMEqp.VendorRefNo = VendorRefNo.ToUpper();
                //    return ;
                //}

                var locCode = (from s in objContext.MESC1TS_SHOP
                               where s.SHOP_CD == ShopCode
                               select new
                               {
                                   s.LOC_CD
                               }).FirstOrDefault();

                string LocationCode = locCode.LOC_CD;
                //var countryCode = (from c in objContext.MESC1TS_LOCATION
                //                   where c.LOC_CD == locCode.LOC_CD
                //                   select c).First();

                if (LocationCode.Length != 0)
                {
                    MQManager mMQ = new MQManager();
                    MQQueueManager QMgr = null;
                    MQQueue RequestQueue = null;
                    MQQueue ResponseQueue = null;
                    string messageID = "";
                    string corrID = "";
                    //QMgr = mMQ.OpenQueueManager("GEO.MERC.FEED", "localhost", "MERCDEV", false);
                    //Q = mMQ.OpenQ("R", QMgr, "MERCDEV.EXRT");
                    //int msgCount = mMQ.GetMessageCount(Q);
                    //string message = mMQ.GetMessage(Q, true);
                    //mMQ.PutMessage(Q, "Hi Abu !!! Got ur message.Thanks");
                    //LocationCode = "002";
                    string Request = "MERCRKEM01";
                    Request += ("000001");
                    Request += (DateTime.Now.ToString("yyyy-mm-dd-hh:mm:ss.000000"));
                    Request += ("TAG001");
                    Request += EqpNo.PadRight(11);
                    Request += LocationCode.PadRight(80);
                    //Request += ( PadSpaces( EqpNo, 11 ) );
                    //Request += ( PadSpaces( LocationCode, 80 ) ); 

                    ///MERCRKEM010000012015-11-26-13.44.25.000000TAG001ULCRU4914727002


                    QMgr = mMQ.OpenQueueManager(ConfigurationManager.AppSettings["MQManagerRequestQueueName"]);
                    RequestQueue = mMQ.OpenQ(MQ_WRITE_MODE, QMgr, ConfigurationManager.AppSettings["MQManagerRequestName"]);
                    //mMQ.PutMessage(Q, "MERCRKEM010000012015-11-30-13.44.25.000000TAG001LCRU4914727002                                                                             ",
                    //    "MERCDEV", "RKEM.RKEMME.RESPONSE", ref messageID);
                    mMQ.PutMessage(RequestQueue, Request, ConfigurationManager.AppSettings["MQManagerResponseQueueName"], ConfigurationManager.AppSettings["MQManagerResponseName"], ref messageID);


                    // QMgr = mMQ.OpenQueueManager(ConfigurationSettings.AppSettings["MQManagerQueueName"]);
                    ResponseQueue = mMQ.OpenQ(MQ_READ_MODE, QMgr, ConfigurationManager.AppSettings["MQManagerResponseName"]);
                    int msgCount = mMQ.GetMessageCount(ResponseQueue);
                    RKEMMessage = mMQ.GetMessage(ResponseQueue, false, ref corrID);

                    if (!messageID.Equals(corrID))
                    {
                        iRtn = (int)SYSTEMERROR;
                        logEntry.Message = "Correlation ID not matching the RKEM response";
                        Logger.Write(logEntry);
                    }

                    mMQ.CloseQ(RequestQueue);
                    mMQ.CloseQ(ResponseQueue);
                    mMQ.DisconnectQueueManager(QMgr);

                }
                else
                {
                    iRtn = (int)INVALIDSHOP;
                    return (iRtn);
                }

                if (!string.IsNullOrEmpty(RKEMMessage))	// then RKEM returned data
                {
                    // extract RKEM success value: 0=OK, 1 = not found, 9 = system failure.
                    int i = Convert.ToInt32(RKEMMessage.Substring(RTNCODEPOS, RTNCODELEN));
                    switch (i)
                    {
                        case 0: iRtn = (int)SUCCESS; break;
                        case 1: iRtn = (int)NOTFOUND; break;
                        case 9: iRtn = (int)SYSTEMERROR; break;
                        default: iRtn = (int)NOTFOUND; break;
                    }


                    if (iRtn == SUCCESS)
                    {
                        RKEMEqp = ParseRKEMData(RKEMMessage);

                    }
                }
                else	// error attempting to get RKEM data
                {
                    iRtn = (int)SYSTEMERROR;	// (-99)
                }


            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                iRtn = (int)SYSTEMERROR;
            }

            return iRtn;
        }

        public List<Mode> GetEquipmentModeDropDownFiltered(string CoType, string EqSType, string Material, string Size, string STREFURB, string STREDEL, string EQIOFLT)
        {
            bool bNo9xModes = false;
            List<Mode> ModeList = new List<Mode>();
            string a = string.Empty;
            Material = (Material.Equals("ALU", StringComparison.CurrentCultureIgnoreCase) ? "Y" : "N");

            // if not ready for refurbishment, filter out 9x modes
            STREFURB = STREFURB.Trim();
            // if empty - don't show 9x modes
            if (string.IsNullOrEmpty(STREFURB))
                bNo9xModes = true;
            else
            {	// or is alpha, but not Y - then don't show 9x modes
                var isAlpha = char.IsLetter(STREFURB[0]);
                if (isAlpha)
                {	// alpha char but not = to Y
                    if (STREFURB.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                        bNo9xModes = true;
                }
            }



            var ModeListFromDB = (from EM in objContext.MESC1TS_EQMODE
                                  from M in objContext.MESC1TS_MODE
                                  where EM.COTYPE == CoType &&
                                  EM.EQSTYPE == EqSType &&
                                  EM.ALUMINIUM_SW == Material &&
                                  EM.EQSIZE == Size &&
                                  (!string.IsNullOrEmpty(Size) ? EM.EQSIZE == Size : a == a) &&
                                  M.MODE == EM.MODE
                                  select new
                                  {
                                      M.MODE,
                                      M.MODE_DESC,
                                      M.MODE_ACTIVE_SW
                                  }).ToList();

            if (ModeListFromDB != null && ModeListFromDB.Count > 0)
            {
                if (bNo9xModes)
                {
                    ModeListFromDB = ModeListFromDB.FindAll(mode => !mode.MODE.Contains("9"));
                    //sFilter += " AND M.MODE not like '9%'";
                }


                // Ticket 4829
                // if not on redelivery
                if (EQIOFLT.Equals("R", StringComparison.CurrentCultureIgnoreCase))
                {	// if not on global hunt, filter out 8x modes
                    if (STREDEL.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                    {
                        ModeListFromDB = ModeListFromDB.FindAll(mode => !mode.MODE.Contains("8"));
                        //sFilter += " AND M.MODE not like '8%'";
                    }
                }

                if (EQIOFLT.Equals("R", StringComparison.CurrentCultureIgnoreCase))
                {
                    // if on redelivery, show only 8x modes, hence sFilter = etc.
                    ModeListFromDB = ModeListFromDB.FindAll(mode => mode.MODE.Contains("8"));
                    //sFilter = " AND M.MODE like '8%'";
                }

                foreach (var item in ModeListFromDB)
                {
                    Mode Mode = new Mode();
                    Mode.ModeCode = item.MODE;
                    Mode.ModeDescription = item.MODE_DESC;
                    Mode.ModeActiveSW = item.MODE_ACTIVE_SW;
                    ModeList.Add(Mode);
                }
            }

            return ModeList;
        }

        private SparePartsView GetSpareDetails(string sPartCode)
        {
            // get master part record loop
            var PartListFromDB = (from parts in objContext.MESC1TS_MASTER_PART
                                  where parts.PART_CD == sPartCode
                                  select new
                                  {
                                      parts.CORE_PART_SW,
                                      parts.MSL_PART_SW,
                                      parts.PART_ACTIVE_SW,
                                      parts.MANUFCTR,
                                      parts.PART_PRICE,
                                      parts.PART_DESC,
                                      parts.QUANTITY,
                                      parts.CORE_VALUE,
                                      parts.DEDUCT_CORE
                                  }).FirstOrDefault();

            SparePartsView SpareParts = null;
            if (PartListFromDB != null) // && PartListFromDB.Count > 0)
            {
                SpareParts = new SparePartsView();
                SpareParts.OwnerSuppliedPartsNumber = sPartCode;
                SpareParts.PART_ACTIVE_SW = (string.IsNullOrEmpty(PartListFromDB.PART_ACTIVE_SW) ? 'N' : Convert.ToChar(PartListFromDB.PART_ACTIVE_SW));
                SpareParts.PART_PRICE = (PartListFromDB.PART_PRICE == null ? 0 : (decimal)PartListFromDB.PART_PRICE);
                SpareParts.PartDescription = PartListFromDB.PART_DESC;
                SpareParts.MANUFCTR = PartListFromDB.MANUFCTR;
                SpareParts.Pieces = (PartListFromDB.QUANTITY == null ? 0 : (int)PartListFromDB.QUANTITY);
                SpareParts.CoreValue = (PartListFromDB.CORE_VALUE == null ? 0 : (decimal)PartListFromDB.CORE_VALUE);
                SpareParts.DEDUCT_CORE = (string.IsNullOrEmpty(PartListFromDB.DEDUCT_CORE) ? 'N' : Convert.ToChar(PartListFromDB.DEDUCT_CORE));
                SpareParts.CORE_PART_SW = (string.IsNullOrEmpty(PartListFromDB.CORE_PART_SW) ? 'N' : Convert.ToChar(PartListFromDB.CORE_PART_SW));
                SpareParts.MslPartSW = (string.IsNullOrEmpty(PartListFromDB.MSL_PART_SW) ? "N" : PartListFromDB.MSL_PART_SW);
            }
            return SpareParts;
        }

        private string UpdateRkemData(string damageText)
        {
            string retVal = "0";
            if (!string.IsNullOrEmpty(damageText))
            {
                switch (damageText)
                {
                    case "No damage": retVal = "N"; break;
                    case "Yes, 3rd party unknown": retVal = "2"; break;
                    case "Yes, 3rd party known": retVal = "3"; break;
                    case "Yes, Machinery damaged": retVal = "4"; break;
                    case "not available": retVal = "1"; break;
                    default: retVal = "0"; break;
                }
            }
            return retVal;
        }

        private List<ErrMessage> GetMessageList(List<ErrMessage> MainList)
        {
            if (MainList != null && MainList.Count > 0)
            {
                int index = MainList.FindIndex(item => item.ErrorType == Validation.MESSAGETYPE.WARNING.ToString() || item.ErrorType == Validation.MESSAGETYPE.ERROR.ToString());
                if (index != -1)
                {
                    MainList.RemoveAll(x => x.ErrorType == Validation.MESSAGETYPE.SUCCESS.ToString());
                }
            }
            return MainList;
        }

        
    }
}
