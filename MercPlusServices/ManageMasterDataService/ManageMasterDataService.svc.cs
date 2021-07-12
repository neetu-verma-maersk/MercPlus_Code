using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MercPlusLibrary;
using System.Globalization;
using System.Data.SqlClient;
using System.Configuration;
using MercPlusServiceLibrary.BusinessObjects;
using System.Data;

namespace ManageMasterDataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class ManageMasterData : IManageMasterData
    {
        ManageMasterData()
        {

        }
        ManageMasterDataServiceEntities objContext = new ManageMasterDataServiceEntities();
        LogEntry logEntry = new LogEntry();
        public List<PayAgent> GetPayAgent()
        {
            List<MESC1TS_PAYAGENT> payAgent = new List<MESC1TS_PAYAGENT>();
            try
            {
                objContext = new ManageMasterDataServiceEntities();

                payAgent = (from pay in objContext.MESC1TS_PAYAGENT
                            select pay).ToList();

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return PrepareDataContract(payAgent);
        }

        private List<PayAgent> PrepareDataContract(List<MESC1TS_PAYAGENT> payAgentFromDB)
        {
            List<PayAgent> payAgentList = new List<PayAgent>();
            try
            {
                for (int count = 0; count < payAgentFromDB.Count; count++)
                {
                    PayAgent payAgent = new PayAgent();
                    payAgent.PayAgentCode = payAgentFromDB[count].PAYAGENT_CD;
                    payAgent.CorpPayAgentCode = payAgentFromDB[count].CORP_PAYAGENT_CD;
                    payAgent.ProfitCenter = payAgentFromDB[count].PROFIT_CENTER;
                    payAgent.SubProfitCenter = payAgentFromDB[count].SUB_PROFIT_CENTER;
                    payAgent.RRISFormat = payAgentFromDB[count].RRIS_FORMAT;
                    payAgent.ChangeUser = payAgentFromDB[count].CHUSER;
                    payAgent.ChangeTime = payAgentFromDB[count].CHTS;
                    payAgentList.Add(payAgent);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return payAgentList;
        }
        private List<Shop> PrepareDataContract(List<MESC1TS_SHOP> shopFromDB)
        {
            List<Shop> shopList = new List<Shop>();
            for (int count = 0; count < shopFromDB.Count; count++)
            {
                Shop shop = new Shop();
                shop.ShopCode = shopFromDB[count].SHOP_CD;
                shop.ShopDescription = shopFromDB[count].SHOP_CD + '-' + shopFromDB[count].SHOP_DESC;

                shopList.Add(shop);
            }
            return shopList;
        }
        private List<RepairLocationCode> PrepareDataContract(List<MESC1TS_REPAIR_LOC> RepairCodeFromDB)
        {
            List<RepairLocationCode> RepairLocationCodeList = new List<RepairLocationCode>();
            for (int count = 0; count < RepairCodeFromDB.Count; count++)
            {
                RepairLocationCode repairlocation = new RepairLocationCode();
                repairlocation.RepairCod = RepairCodeFromDB[count].cedex_code;
                repairlocation.RepairDescription = RepairCodeFromDB[count].description;
                repairlocation.Description = RepairCodeFromDB[count].cedex_code + '-' + RepairCodeFromDB[count].description;

                RepairLocationCodeList.Add(repairlocation);
            }
            return RepairLocationCodeList;
        }
        private List<EqType> PrepareDataContract(List<MESC1TS_EQTYPE> eqTypeFromDB)
        {
            List<EqType> equipmentTypeList = new List<EqType>();
            for (int count = 0; count < eqTypeFromDB.Count; count++)
            {
                EqType eq = new EqType();
                eq.EqpType = eqTypeFromDB[count].EQTYPE;
                eq.EqTypeDesc = eqTypeFromDB[count].EQTYPE_DESC;
                eq.ChangeUser = eqTypeFromDB[count].CHUSER;
                eq.ChangeTime = eqTypeFromDB[count].CHTS;
                equipmentTypeList.Add(eq);
            }
            return equipmentTypeList;
        }
        private List<ModeEntry> PrepareDataContract(List<MESC1TS_EQMODE> mdTypeFromDB)
        {
            List<ModeEntry> modeEntryList = new List<ModeEntry>();
            for (int count = 0; count < mdTypeFromDB.Count; count++)
            {
                ModeEntry md = new ModeEntry();
                md.EqType = mdTypeFromDB[count].COTYPE;
                md.SubType = mdTypeFromDB[count].EQSTYPE;
                md.Size = mdTypeFromDB[count].EQSIZE;
                md.Aluminum = mdTypeFromDB[count].ALUMINIUM_SW;
                modeEntryList.Add(md);
            }
            return modeEntryList;
        }
        private List<Damage> PrepareDataContract(List<MESC1TS_DAMAGE> DamageCodeFromDB, bool IsComboLoad)
        {
            List<Damage> DamageCodeList = new List<Damage>();
            for (int count = 0; count < DamageCodeFromDB.Count; count++)
            {
                Damage Damage = new Damage();
                if (IsComboLoad)
                {
                    Damage.DamageCedexCode = DamageCodeFromDB[count].cedex_code;
                    Damage.DamageFullDescription = DamageCodeFromDB[count].cedex_code + '-' + DamageCodeFromDB[count].name;
                }
                else
                {
                    Damage.DamageCedexCode = DamageCodeFromDB[count].cedex_code;
                    Damage.DamageName = DamageCodeFromDB[count].name;
                    Damage.DamageDescription = DamageCodeFromDB[count].description;
                    Damage.DamageNumericalCode = DamageCodeFromDB[count].numerical_code;
                    Damage.ChangeUser = GetUserNamePTI(DamageCodeFromDB[count].CHUSER);
                    Damage.ChangeTime = DamageCodeFromDB[count].CHTS;
                    Damage.DamageFullDescription = DamageCodeFromDB[count].cedex_code + '-' + DamageCodeFromDB[count].name;
                }
                DamageCodeList.Add(Damage);
            }
            return DamageCodeList;
        }
        public bool UpdatePayAgent(PayAgent PayAgentToBeUpdated)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_PAYAGENT> payAgentDBObject = new List<MESC1TS_PAYAGENT>();
            try
            {
                payAgentDBObject = (from pay in objContext.MESC1TS_PAYAGENT
                                    where pay.PAYAGENT_CD == PayAgentToBeUpdated.PayAgentCode
                                    select pay).ToList();

                payAgentDBObject[0].CORP_PAYAGENT_CD = PayAgentToBeUpdated.CorpPayAgentCode;
                payAgentDBObject[0].PROFIT_CENTER = PayAgentToBeUpdated.ProfitCenter;
                payAgentDBObject[0].SUB_PROFIT_CENTER = PayAgentToBeUpdated.SubProfitCenter;
                payAgentDBObject[0].RRIS_FORMAT = PayAgentToBeUpdated.RRISFormat;
                payAgentDBObject[0].CHUSER = PayAgentToBeUpdated.ChangeUser;
                payAgentDBObject[0].CHTS = DateTime.Now;

                try
                {
                    objContext.SaveChanges();
                    success = true;
                }
                catch (Exception e)
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

        public bool DeletePayAgent(string RRISPayAgentCode, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_PAYAGENT_VENDOR> PayAgntVenObject = new List<MESC1TS_PAYAGENT_VENDOR>();
            try
            {
                PayAgntVenObject = (from pav in objContext.MESC1TS_PAYAGENT_VENDOR
                                    where pav.PAYAGENT_CD == RRISPayAgentCode
                                    select pav).ToList();
                List<MESC1TS_CUST_SHOP_MODE> PayAgntCustObject = new List<MESC1TS_CUST_SHOP_MODE>();
                PayAgntCustObject = (from pc in objContext.MESC1TS_CUST_SHOP_MODE
                                     where pc.PAYAGENT_CD == RRISPayAgentCode
                                     select pc).ToList();
                if (PayAgntVenObject.Count > 0 || PayAgntCustObject.Count > 0)
                {
                    Msg = "RRIS Pay Agent Code " + RRISPayAgentCode + " exists in another table - Not Deleted";
                    return success;
                }
                else
                {
                    List<MESC1TS_PAYAGENT> payAgentDBObject = new List<MESC1TS_PAYAGENT>();
                    payAgentDBObject = (from pay in objContext.MESC1TS_PAYAGENT
                                        where pay.PAYAGENT_CD == RRISPayAgentCode
                                        select pay).ToList();
                    if (payAgentDBObject.Count > 0)
                    {
                        objContext.MESC1TS_PAYAGENT.Remove(payAgentDBObject.First());
                        objContext.SaveChanges();
                        success = true;
                    }
                    else
                    {
                        Msg = "PayAgent " + RRISPayAgentCode + " not found in the table";
                        return success;

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

        public bool CreatePayAgent(PayAgent PayAgentFromClient, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_PAYAGENT> PayAgnt = new List<MESC1TS_PAYAGENT>();
            try
            {
                PayAgnt = (from pa in objContext.MESC1TS_PAYAGENT
                           where pa.PAYAGENT_CD == PayAgentFromClient.PayAgentCode
                           select pa).ToList();

                if (PayAgnt.Count > 0)
                {
                    Msg = "PayAgent Code " + PayAgentFromClient.PayAgentCode + " already exists - Not Added";
                    return success;
                }
                else
                {

                    MESC1TS_PAYAGENT PayAgentToBeInserted = new MESC1TS_PAYAGENT();
                    PayAgentToBeInserted.PAYAGENT_CD = PayAgentFromClient.PayAgentCode;
                    PayAgentToBeInserted.CORP_PAYAGENT_CD = PayAgentFromClient.CorpPayAgentCode;
                    PayAgentToBeInserted.PROFIT_CENTER = PayAgentFromClient.ProfitCenter;
                    PayAgentToBeInserted.SUB_PROFIT_CENTER = PayAgentFromClient.ProfitCenter;
                    PayAgentToBeInserted.RRIS_FORMAT = PayAgentFromClient.RRISFormat;
                    PayAgentToBeInserted.CHUSER = PayAgentFromClient.ChangeUser;
                    PayAgentToBeInserted.CHTS = DateTime.Now;
                    objContext.MESC1TS_PAYAGENT.Add(PayAgentToBeInserted);
                    try
                    {
                        objContext.SaveChanges();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        success = false;
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

        //private List<Mode> PrepareDataContract(List<MESC1TS_MODE> modeFromDB)
        //{
        //    List<Mode> modeList = new List<Mode>();
        //    for (int count = 0; count < modeFromDB.Count; count++)
        //    {
        //        Mode mode = new Mode();
        //        mode.ModeCode = modeFromDB[count].MODE;
        //        mode.ModeDescription = modeFromDB[count].MODE + '-' + modeFromDB[count].MODE_DESC;

        //        modeList.Add(mode);
        //    }
        //    return modeList;
        //}

        //public List<Mode> GetMode()
        //{
        //    objContext = new ManageMasterDataServiceEntities();
        //    List<MESC1TS_MODE> Mode = new List<MESC1TS_MODE>();
        //    Mode = (from mode in objContext.MESC1TS_MODE
        //            select mode).Take(200).ToList();

        //    //            string esqlQuery = @"SELECT VALUE pay
        //    //                    FROM ManageMasterDataServiceEntities.MESC1TS_PAYAGENT as pay";


        //    //            ObjectQuery<MESC1TS_PAYAGENT> query = new ObjectQuery<MESC1TS_PAYAGENT>(esqlQuery, objContext);
        //    //            payAgent = query.ToList();
        //    return PrepareDataContract(Mode);

        //}



        //public List<EqType> GetEquipmentList()
        //{

        //    objContext = new ManageMasterDataServiceEntities();
        //    List<MESC1TS_EQTYPE> EqTypeListFromDB = new List<MESC1TS_EQTYPE>();
        //    List<EqType> EqtypeList = new List<EqType>();

        //    try
        //    {
        //        EqTypeListFromDB = (from C in objContext.MESC1TS_EQTYPE
        //                            orderby C.EQTYPE descending
        //                            select C).ToList();

        //        for (int count = 0; count < EqTypeListFromDB.Count; count++)
        //        {
        //            EqType eqtype = new EqType();
        //            eqtype.EqpType = EqTypeListFromDB[count].EQTYPE;
        //            eqtype.EqTypeDesc = EqTypeListFromDB[count].EQTYPE_DESC;
        //            EqtypeList.Add(eqtype);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;

        //    }

        //    return EqtypeList;

        //}

        public List<Customer> GetCustomerDetailsList()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_CUSTOMER> CustomerListFromDB = new List<MESC1TS_CUSTOMER>();
            List<MESC1TS_MANUAL> ManualListFromDB = new List<MESC1TS_MANUAL>();
            List<Customer> CustomerList = new List<Customer>();
            try
            {
                CustomerListFromDB = (from C in objContext.MESC1TS_CUSTOMER
                                      from M in objContext.MESC1TS_MANUAL
                                      where C.MANUAL_CD == M.MANUAL_CD
                                      orderby C.CUSTOMER_CD
                                      select C).ToList();


                ManualListFromDB = (from C in objContext.MESC1TS_CUSTOMER
                                    from M in objContext.MESC1TS_MANUAL
                                    where C.MANUAL_CD == M.MANUAL_CD
                                    orderby C.CUSTOMER_CD
                                    select M).ToList();

                for (int count = 0; count < CustomerListFromDB.Count; count++)
                {
                    Customer customer = new Customer();
                    customer.CustomerCode = CustomerListFromDB[count].CUSTOMER_CD;
                    customer.CustomerDesc = CustomerListFromDB[count].CUSTOMER_DESC;
                    customer.ManualCode = CustomerListFromDB[count].MANUAL_CD;
                    customer.CustomerActiveSw = CustomerListFromDB[count].CUSTOMER_ACTIVE_SW;
                    customer.ChangeUser = CustomerListFromDB[count].CHUSER;
                    customer.ChangeTime = CustomerListFromDB[count].CHTS;
                    CustomerList.Add(customer);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return CustomerList;
        }

        public List<Manual> GetManualList()
        {
            List<Manual> ManualCodeList = new List<Manual>();
            try
            {
                var ManualListFromDB = (from M in objContext.MESC1TS_MANUAL
                                        orderby M.MANUAL_CD
                                        select M).ToList();
                for (int count = 0; count < ManualListFromDB.Count; count++)
                {
                    Manual manual = new Manual();
                    manual.ManualCode = ManualListFromDB[count].MANUAL_CD;
                    manual.ManualDesc = ManualListFromDB[count].MANUAL_DESC;
                    ManualCodeList.Add(manual);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ManualCodeList;
        }

        public List<Manual> GetManualCodeList()
        {
            List<Manual> ManualCodeList = new List<Manual>();
            try
            {
                var ManualListFromDB = (from M in objContext.MESC1TS_MANUAL
                                        join Mm in objContext.MESC1TS_MANUAL_MODE on
                                         M.MANUAL_CD equals Mm.MANUAL_CD
                                        orderby M.MANUAL_CD
                                        select M).Distinct().ToList();
                for (int count = 0; count < ManualListFromDB.Count; count++)
                {
                    Manual manual = new Manual();
                    manual.ManualCode = ManualListFromDB[count].MANUAL_CD;
                    manual.ManualDesc = ManualListFromDB[count].MANUAL_DESC;
                    ManualCodeList.Add(manual);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ManualCodeList;
        }


        public List<Shop> GetRSByShop(string ShopCode)
        {
            List<Shop> ShopList = new List<Shop>();
            try
            {


                objContext = new ManageMasterDataServiceEntities();


                var Query = (from S in objContext.MESC1TS_SHOP
                             from U in objContext.SEC_USER
                           .Where(U => U.LOGIN == S.CHUSER)
                           .DefaultIfEmpty()
                             where S.SHOP_CD == ShopCode
                             select new { S, U.FIRSTNAME, U.LASTNAME }).ToList();

                foreach (var obj in Query)
                {
                    Shop objShop = new Shop();

                    objShop.ShopCode = obj.S.SHOP_CD;
                    objShop.ShopActiveSW = obj.S.SHOP_ACTIVE_SW;
                    objShop.ShopDescription = obj.S.SHOP_DESC;
                    objShop.ShopTypeCode = obj.S.SHOP_TYPE_CD;
                    objShop.VendorCode = obj.S.VENDOR_CD;
                    objShop.LocationCode = obj.S.LOC_CD;
                    objShop.RKRPloc = obj.S.RKRPLOC;
                    objShop.SalesTaxPartCont = obj.S.SALES_TAX_PART_CONT;
                    objShop.SalesTaxLaborCon = obj.S.SALES_TAX_LABOR_CON;
                    objShop.CUCDN = obj.S.CUCDN;
                    objShop.SalesTaxPartGen = obj.S.SALES_TAX_PART_GEN;
                    objShop.SalesTaxLaborGen = obj.S.SALES_TAX_LABOR_GEN;
                    objShop.EmailAdress = obj.S.EMAIL_ADR;
                    objShop.ImportTax = obj.S.IMPORT_TAX;
                    objShop.Phone = obj.S.PHONE;
                    objShop.PCTMaterialFactor = obj.S.PCT_MATERIAL_FACTOR;
                    objShop.RRISXmitSW = obj.S.RRIS_XMIT_SW;
                    objShop.AcepSW = obj.S.ACEP_SW;
                    objShop.RRIS70SuffixCode = obj.S.RRIS70_SUFFIX_CD;
                    objShop.OvertimeSuspSW = obj.S.OVERTIME_SUSP_SW;
                    objShop.ChangeUser = obj.FIRSTNAME + " " + obj.LASTNAME;
                    objShop.PreptimeSW = obj.S.PREPTIME_SW;
                    objShop.ChangeTime = obj.S.CHTS;
                    objShop.Decentralized = obj.S.DECENTRALIZED;
                    objShop.BypassLeaseRules = obj.S.BYPASS_LEASE_RULES;
                    objShop.AutoCompleteSW = obj.S.AUTO_COMPLETE_SW;
                    ShopList.Add(objShop);
                };

            }
            catch (Exception ex)
            {
                throw;
            }
            return ShopList;
        }

        #region Afroz

        public List<Shop> GetShopByUserId(int UserId)
        {
            List<Shop> ShopList = new List<Shop>();

            //List<MESC1TS_SHOP> ShopListFromDBOnAuth = new List<MESC1TS_SHOP>();
            //List<MESC1TS_SHOP> ShopListFinal = new List<MESC1TS_SHOP>();

            try
            {
                //SELECT U.AUTHGROUP_ID, U.COLUMN_VALUE as COLUMN_VALUE, G.TABLE_NAME, G.COLUMN_NAME as COLUMN_NAME
                //FROM SEC_AUTHGROUP_USER  U,	SEC_AUTHGROUP  G
                //WHERE U.USER_ID = <User_Id>
                //AND U.AUTHGROUP_ID = G.AUTHGROUP_ID ORDER BY U.AUTHGROUP_ID

                //ShopListFromDBOnAuth = (from S in objContext.MESC1TS_SHOP
                //                        join G in objContext.SEC_AUTHGROUP_USER on S.SHOP_CD equals G.COLUMN_VALUE
                //                        join A in objContext.SEC_AUTHGROUP on G.AUTHGROUP_ID equals A.AUTHGROUP_ID
                //                        where G.USER_ID == UserID &&
                //                              S.SHOP_ACTIVE_SW == "Y"
                //                        orderby S.SHOP_CD
                //                        select S).ToList();

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
                        string ShopCode = item.COLUMN_VALUE;
                        ShopCodeList.Add(ShopCode);
                    }
                }

                var ShopListFromDBOnAuth = (from shop in objContext.MESC1VS_SHOP_LOCATION
                                            where
                                                //shop.SHOP_ACTIVE_SW == "Y" &&
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

                //var x = x.FindAll(cd => ShopListFromDBOnAuth1.Any(acd => acd.COLUMN_NAME

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
                                              //S.SHOP_ACTIVE_SW == "Y" &&
                                                S.CUCDN == CY.CUCDN &&
                                                CU.CUSTOMER_CD == SM.CUSTOMER_CD
                                          orderby S.SHOP_CD
                                          select S).ToList();


                    var ShopListFinal = ShopListFromDBOnAuth.FindAll(a => ShopListFromDB.Any(ab => ab.SHOP_CD == a.SHOP_CD));

                    MESC1TS_CURRENCY Currency = new MESC1TS_CURRENCY();
                    List<MESC1TS_CURRENCY> CurrencyFromDB = new List<MESC1TS_CURRENCY>();

                    ////Get the Currency detail on the basis of the ShopCode
                    CurrencyFromDB = (from C in objContext.MESC1TS_CURRENCY
                                      join S in objContext.MESC1TS_SHOP on C.CUCDN equals S.CUCDN
                                      //where S.SHOP_CD == ShopCode
                                      select C).ToList();
                    //CurrencyName = CurrencyFromDB[0].CURRNAMC;

                    List<Customer> Customerlist = new List<Customer>();
                    List<MESC1TS_CUSTOMER> CustomerFromDB = new List<MESC1TS_CUSTOMER>();
                    Customer Customer = new Customer();
                    List<string> shopCodes = ShopList.Select(sc => { return sc.ShopCode; }).ToList();
                    //SELECT DISTINCT C.CUSTOMER_CD 
                    //FROM MESC1VS_CUST_SHOP CS, MESC1TS_CUSTOMER C 
                    //WHERE C.CUSTOMER_CD = CS.CUSTOMER_CD 
                    // AND CS.SHOP_CD IN ( ShopList )

                    foreach (var item in ShopListFinal)
                    {
                        Shop shop = new Shop();
                        shop.ShopCode = item.SHOP_CD;
                        shop.ShopDescription = item.SHOP_DESC;
                        shop.CUCDN = item.CUCDN;
                        ShopList.Add(shop);
                    }
                    if (ShopList != null && ShopList.Count > 0)
                    {
                        PopulateShopWithCustAndCurrency(ShopList[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ShopList;
        }

        public List<Shop> GetShopProfileByUserId(int UserId)
        {
            List<Shop> ShopList = new List<Shop>();

            //List<MESC1TS_SHOP> ShopListFromDBOnAuth = new List<MESC1TS_SHOP>();
            //List<MESC1TS_SHOP> ShopListFinal = new List<MESC1TS_SHOP>();

            try
            {
                //SELECT U.AUTHGROUP_ID, U.COLUMN_VALUE as COLUMN_VALUE, G.TABLE_NAME, G.COLUMN_NAME as COLUMN_NAME
                //FROM SEC_AUTHGROUP_USER  U,	SEC_AUTHGROUP  G
                //WHERE U.USER_ID = <User_Id>
                //AND U.AUTHGROUP_ID = G.AUTHGROUP_ID ORDER BY U.AUTHGROUP_ID

                //ShopListFromDBOnAuth = (from S in objContext.MESC1TS_SHOP
                //                        join G in objContext.SEC_AUTHGROUP_USER on S.SHOP_CD equals G.COLUMN_VALUE
                //                        join A in objContext.SEC_AUTHGROUP on G.AUTHGROUP_ID equals A.AUTHGROUP_ID
                //                        where G.USER_ID == UserID &&
                //                              S.SHOP_ACTIVE_SW == "Y"
                //                        orderby S.SHOP_CD
                //                        select S).ToList();

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
                        string ShopCode = item.COLUMN_VALUE;
                        ShopCodeList.Add(ShopCode);
                    }
                }

                var ShopListFromDBOnAuth = (from shop in objContext.MESC1VS_SHOP_LOCATION
                                            where
                                                //shop.SHOP_ACTIVE_SW == "Y" &&
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

                //var x = x.FindAll(cd => ShopListFromDBOnAuth1.Any(acd => acd.COLUMN_NAME

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
                                          //from SM in objContext.MESC1TS_CUST_SHOP_MODE
                                          //from CY in objContext.MESC1TS_CURRENCY
                                          //from CU in objContext.MESC1TS_CUSTOMER
                                          //where S.SHOP_CD == SM.SHOP_CD &&
                                          //    //S.SHOP_ACTIVE_SW == "Y" &&
                                          //      S.CUCDN == CY.CUCDN &&
                                          //      CU.CUSTOMER_CD == SM.CUSTOMER_CD
                                          orderby S.SHOP_CD
                                          select S).ToList();


                    var ShopListFinal = ShopListFromDBOnAuth.FindAll(a => ShopListFromDB.Any(ab => ab.SHOP_CD == a.SHOP_CD));

                    MESC1TS_CURRENCY Currency = new MESC1TS_CURRENCY();
                    List<MESC1TS_CURRENCY> CurrencyFromDB = new List<MESC1TS_CURRENCY>();

                    ////Get the Currency detail on the basis of the ShopCode
                    CurrencyFromDB = (from C in objContext.MESC1TS_CURRENCY
                                      join S in objContext.MESC1TS_SHOP on C.CUCDN equals S.CUCDN
                                      //where S.SHOP_CD == ShopCode
                                      select C).ToList();
                    //CurrencyName = CurrencyFromDB[0].CURRNAMC;

                    List<Customer> Customerlist = new List<Customer>();
                    List<MESC1TS_CUSTOMER> CustomerFromDB = new List<MESC1TS_CUSTOMER>();
                    Customer Customer = new Customer();
                    List<string> shopCodes = ShopList.Select(sc => { return sc.ShopCode; }).ToList();
                    //SELECT DISTINCT C.CUSTOMER_CD 
                    //FROM MESC1VS_CUST_SHOP CS, MESC1TS_CUSTOMER C 
                    //WHERE C.CUSTOMER_CD = CS.CUSTOMER_CD 
                    // AND CS.SHOP_CD IN ( ShopList )

                    foreach (var item in ShopListFinal)
                    {
                        Shop shop = new Shop();
                        shop.ShopCode = item.SHOP_CD;
                        shop.ShopDescription = item.SHOP_DESC;
                        shop.CUCDN = item.CUCDN;
                        ShopList.Add(shop);
                    }
                    if (ShopList != null && ShopList.Count > 0)
                    {
                        PopulateShopWithCustAndCurrency(ShopList[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ShopList;
        }

        private void PopulateShopWithCustAndCurrency(Shop Shop)
        {
            MESC1TS_CURRENCY Currency = new MESC1TS_CURRENCY();
            List<MESC1TS_CURRENCY> CurrencyFromDB = new List<MESC1TS_CURRENCY>();

            try
            {
                ////Get the Currency detail on the basis of the ShopCode
                CurrencyFromDB = (from C in objContext.MESC1TS_CURRENCY
                                  join S in objContext.MESC1TS_SHOP on C.CUCDN equals S.CUCDN
                                  where S.SHOP_CD == Shop.ShopCode
                                  select C).ToList();
                //CurrencyName = CurrencyFromDB[0].CURRNAMC;

                List<Customer> Customerlist = new List<Customer>();
                List<MESC1TS_CUSTOMER> CustomerFromDB = new List<MESC1TS_CUSTOMER>();
                Customer Customer = new Customer();
                //List<string> shopCodes = ShopList.Select(sc => { return sc.ShopCode; }).ToList();
                //SELECT DISTINCT C.CUSTOMER_CD 
                //FROM MESC1VS_CUST_SHOP CS, MESC1TS_CUSTOMER C 
                //WHERE C.CUSTOMER_CD = CS.CUSTOMER_CD 
                // AND CS.SHOP_CD IN ( ShopList )


                Currency = CurrencyFromDB.Find(curr => curr.CUCDN == Shop.CUCDN);
                //shop.Currency = (Currency)Currency;
                Shop.Currency = new Currency();
                Shop.Currency.Cucdn = Currency.CUCDN;
                Shop.Currency.CurCode = Currency.CURCD;
                Shop.Currency.CurrName = Currency.CURRNAMC;

                CustomerFromDB = (from CS in objContext.MESC1VS_CUST_SHOP
                                  from C in objContext.MESC1TS_CUSTOMER
                                  where C.CUSTOMER_CD == CS.CUSTOMER_CD &&
                                      CS.SHOP_CD == Shop.ShopCode &&
                                      C.CUSTOMER_ACTIVE_SW == "Y"
                                  select C).Distinct().ToList();
                Shop.Customer = new List<Customer>();
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
        public List<Currency> GetRSAllCurrencies()
        {
            List<Currency> CurrencyList = new List<Currency>();
            try
            {


                objContext = new ManageMasterDataServiceEntities();


                var Query = (from MC in objContext.MESC1TS_CURRENCY

                             orderby MC.CUCDN
                             select new { MC.CUCDN, MC.CURCD, MC.CURRNAMC }).ToList();

                foreach (var obj in Query)
                {
                    Currency objCurrency = new Currency();

                    objCurrency.Cucdn = obj.CUCDN;
                    objCurrency.CurCode = obj.CURCD;
                    objCurrency.CurrName = obj.CURRNAMC;

                    CurrencyList.Add(objCurrency);
                };

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            return CurrencyList;
        }
        #region PayAgent_Vendor
        public List<PayAgentVendor> RSAllCorpPayAgents()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<PayAgentVendor> payAgentList = new List<PayAgentVendor>();
            try
            {
                var payAgent = (from pay in objContext.MESC1TS_PAYAGENT
                                orderby pay.CORP_PAYAGENT_CD
                                select pay).ToList();


                foreach (var obj in payAgent)
                {
                    PayAgentVendor payagent = new PayAgentVendor();
                    payagent.PayAgentCode = obj.CORP_PAYAGENT_CD;
                    payAgentList.Add(payagent);

                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return payAgentList;
        }
        public List<Vendor> RSVendorsByPayAgent(string PayAgent_CD)
        {

            objContext = new ManageMasterDataServiceEntities();
            List<Vendor> VendorList = new List<Vendor>();
            try
            {

                var payAgent_Vendor = (from pay in objContext.MESC1TS_PAYAGENT_VENDOR
                                       join E in objContext.MESC1TS_VENDOR
                                       on pay.VENDOR_CD equals E.VENDOR_CD
                                       where pay.PAYAGENT_CD == PayAgent_CD
                                       select new { pay.VENDOR_CD, E.VENDOR_DESC }).ToList();

                foreach (var obj in payAgent_Vendor)
                {
                    Vendor vendor = new Vendor();
                    vendor.VendorCode = obj.VENDOR_CD;
                    vendor.VendorDesc = obj.VENDOR_DESC;
                    VendorList.Add(vendor);
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            return VendorList;

        }
        public List<Vendor> GetRSAllVendors()
        {
            List<Vendor> VendorList = new List<Vendor>();
            try
            {


                objContext = new ManageMasterDataServiceEntities();

                // List<MESC1TS_EQTYPE> DamageFromDB = new List<MESC1TS_EQTYPE>();
                var Query = (from MV in objContext.MESC1TS_VENDOR

                             orderby MV.VENDOR_CD
                             select new { MV.VENDOR_CD, MV.VENDOR_DESC, MV.VENDOR_ACTIVE_SW }).ToList();




                foreach (var obj in Query)
                {
                    Vendor objVendor = new Vendor();

                    objVendor.VendorCode = obj.VENDOR_CD;
                    objVendor.VendorDesc = obj.VENDOR_DESC;
                    objVendor.VendorActiveSw = obj.VENDOR_ACTIVE_SW;

                    VendorList.Add(objVendor);
                };

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            return VendorList;
        }
        public List<PayAgentVendor> RSByPayAgentVendor(string PayAgent_CD, string Vendor_CD)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<PayAgentVendor> payAgentVendorList = new List<PayAgentVendor>();
            try
            {

                var payAgent_Vendor = (from pay in objContext.MESC1TS_PAYAGENT_VENDOR
                                       from U in objContext.SEC_USER
                                       .Where(U => U.LOGIN == pay.CHUSER)
                                       .DefaultIfEmpty()
                                       where pay.PAYAGENT_CD == PayAgent_CD && pay.VENDOR_CD == Vendor_CD
                                       select new { pay, U.FIRSTNAME, U.LASTNAME }).ToList();
                foreach (var obj in payAgent_Vendor)
                {
                    PayAgentVendor payAgentVendor = new PayAgentVendor();
                    payAgentVendor.PayAgentCode = obj.pay.PAYAGENT_CD;
                    payAgentVendor.VendorCode = obj.pay.VENDOR_CD;
                    payAgentVendor.LocalAccountCode = obj.pay.LOCAL_ACCOUNT_CD;
                    payAgentVendor.SupplierCode = obj.pay.SUPPLIER_CD;
                    payAgentVendor.PaymentMethod = obj.pay.PAYMENT_METHOD;
                    //UserInfo User = new UserInfo();
                    //User.FirstName = obj.FIRSTNAME;
                    //User.LastName = obj.LASTNAME;
                    //payAgentVendor.UserInfo = User;
                    payAgentVendor.ChangeUser = obj.FIRSTNAME + " " + obj.LASTNAME;
                    payAgentVendor.ChangeTime = obj.pay.CHTS;
                    payAgentVendorList.Add(payAgentVendor);
                }

            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            return payAgentVendorList;

        }
        public string CreatePayAgentVendor(PayAgentVendor PayAgentFromClient)
        {
            string result = "";
            objContext = new ManageMasterDataServiceEntities();
            try
            {
                var payAgentVendor = (from pay in objContext.MESC1TS_PAYAGENT_VENDOR
                                      where pay.PAYAGENT_CD == PayAgentFromClient.PayAgentCode && pay.VENDOR_CD == PayAgentFromClient.VendorCode
                                      select pay).ToList();
                if (payAgentVendor.Count > 0)
                {
                    result = "Exist";
                }
                else
                {

                    MESC1TS_PAYAGENT_VENDOR PayAgentToBeInserted = new MESC1TS_PAYAGENT_VENDOR();
                    PayAgentToBeInserted.PAYAGENT_CD = PayAgentFromClient.PayAgentCode;
                    PayAgentToBeInserted.VENDOR_CD = PayAgentFromClient.VendorCode;
                    PayAgentToBeInserted.LOCAL_ACCOUNT_CD = PayAgentFromClient.LocalAccountCode;
                    PayAgentToBeInserted.SUPPLIER_CD = PayAgentFromClient.SupplierCode;
                    PayAgentToBeInserted.PAYMENT_METHOD = PayAgentFromClient.PaymentMethod;
                    PayAgentToBeInserted.CHUSER = PayAgentFromClient.ChangeUser;
                    PayAgentToBeInserted.CHTS = DateTime.Now;
                    objContext.MESC1TS_PAYAGENT_VENDOR.Add(PayAgentToBeInserted);
                    int i = objContext.SaveChanges();
                    if (i == 1)
                    {
                        result = "Success";
                    }
                    else
                    {
                        result = "Error";
                    }
                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return "Error";
            }

            return result;
        }
        public string DeletePayAgentVendor(string PayAgent_CD, string Vendor_CD)
        {
            string result = "";
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_PAYAGENT_VENDOR> payAgentDBObject = new List<MESC1TS_PAYAGENT_VENDOR>();
            try
            {

                payAgentDBObject = (from pay in objContext.MESC1TS_PAYAGENT_VENDOR
                                    where pay.PAYAGENT_CD == PayAgent_CD && pay.VENDOR_CD == Vendor_CD
                                    select pay).ToList();

                objContext.MESC1TS_PAYAGENT_VENDOR.Remove(payAgentDBObject.First());
                int i = objContext.SaveChanges();
                if (i == 1)
                {
                    result = "Success";
                }
                else
                {
                    result = "Error";
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return "Error";
            }
            return result;
        }
        public string UpdatePayAgentVendor(PayAgentVendor PayAgentToBeUpdated)
        {
            string result = "";
            objContext = new ManageMasterDataServiceEntities();
            try
            {
                List<MESC1TS_PAYAGENT_VENDOR> payAgentDBObject = new List<MESC1TS_PAYAGENT_VENDOR>();
                payAgentDBObject = (from pay in objContext.MESC1TS_PAYAGENT_VENDOR
                                    where pay.PAYAGENT_CD == PayAgentToBeUpdated.PayAgentCode && pay.VENDOR_CD == PayAgentToBeUpdated.VendorCode
                                    select pay).ToList();
                if (payAgentDBObject.Count > 0)
                {

                    payAgentDBObject[0].PAYAGENT_CD = PayAgentToBeUpdated.PayAgentCode;
                    payAgentDBObject[0].VENDOR_CD = PayAgentToBeUpdated.VendorCode;
                    payAgentDBObject[0].LOCAL_ACCOUNT_CD = PayAgentToBeUpdated.LocalAccountCode;
                    //payAgentDBObject[0].PAYMENT_METHOD = PayAgentToBeUpdated.PaymentMethod;
                    payAgentDBObject[0].SUPPLIER_CD = PayAgentToBeUpdated.SupplierCode;
                    payAgentDBObject[0].CHUSER = PayAgentToBeUpdated.ChangeUser;
                    payAgentDBObject[0].CHTS = DateTime.Now;
                    int i = objContext.SaveChanges();
                    if (i == 1)
                    {
                        result = "Success";
                    }
                    else
                    {
                        result = "Error";
                    }

                }


            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return "Error";
            }
            return result;
        }
        #endregion payagent_Vendor
        public List<ShopCont> GetRSAllContracts(string ShopCode, string RepairCode, string Mode)
        {
            List<ShopCont> ShopContList = new List<ShopCont>();
            try
            {


                objContext = new ManageMasterDataServiceEntities();

                // List<MESC1TS_EQTYPE> DamageFromDB = new List<MESC1TS_EQTYPE>();
                var Query = from C in objContext.MESC1TS_SHOP_CONT
                            from S in objContext.MESC1TS_SHOP
                            .Where(S => S.SHOP_CD == C.SHOP_CD)
                            .DefaultIfEmpty()
                            from R in objContext.MESC1TS_CURRENCY
                            .Where(R => R.CUCDN == S.CUCDN)
                            .DefaultIfEmpty()
                            orderby C.REPAIR_CD, C.EXP_DTE descending, C.MANUAL_CD, C.MODE
                            select new { C, S, R };

                //now we can apply filters on ANY of the joined tables
                if (!string.IsNullOrEmpty(ShopCode))
                    Query = Query.Where(q => q.C.SHOP_CD == ShopCode);
                if (!string.IsNullOrEmpty(RepairCode))
                    Query = Query.Where(q => q.C.REPAIR_CD == RepairCode);
                if (!string.IsNullOrEmpty(Mode))
                    Query = Query.Where(q => q.C.MODE == Mode);

                //finally select the columns we needed
                var DamageFromDB = (from q in Query
                                    select new
                                    {
                                        q.C.SHOP_CONT_ID,
                                        q.C.SHOP_CD,
                                        q.C.EFF_DTE,
                                        q.C.EXP_DTE,
                                        q.C.MANUAL_CD,
                                        q.C.REPAIR_CD,
                                        q.C.CONTRACT_AMOUNT,
                                        q.C.MODE,
                                        q.S.SHOP_ACTIVE_SW,
                                        q.S.CUCDN,
                                        q.R.CURCD,
                                        q.S.DECENTRALIZED
                                    }).ToList();

                foreach (var obj in DamageFromDB)
                {
                    ShopCont objShopCont = new ShopCont();

                    objShopCont.ShopContID = obj.SHOP_CONT_ID;
                    objShopCont.ShopCode = obj.SHOP_CD.ToString();
                    objShopCont.EffDate = obj.EFF_DTE;
                    objShopCont.ExpDate = obj.EXP_DTE;
                    objShopCont.ManualCode = obj.MANUAL_CD;
                    objShopCont.RepairCode = obj.REPAIR_CD;
                    objShopCont.ContractAmount = obj.CONTRACT_AMOUNT;
                    objShopCont.Mode = obj.MODE;
                    Shop objShop = new Shop();
                    objShop.ShopActiveSW = obj.SHOP_ACTIVE_SW;
                    objShop.CUCDN = obj.CUCDN;
                    objShop.Decentralized = obj.DECENTRALIZED;
                    Currency objCur = new Currency();
                    objCur.CurCode = obj.CURCD;
                    objShop.Currency = objCur;
                    objShopCont.Shop = objShop;


                    ShopContList.Add(objShopCont);
                };

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            return ShopContList;
        }
        public List<Mode> GetModesByShop(string Shop_CD)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<Mode> ModeList = new List<Mode>();
            // List<MESC1TS_EQTYPE> DamageFromDB = new List<MESC1TS_EQTYPE>();
            try
            {

                var ModeLst = (from M in objContext.MESC1TS_SHOP_LIMITS
                               join E in objContext.MESC1TS_MODE
                               on M.MODE equals E.MODE
                               where M.SHOP_CD == Shop_CD
                               orderby M.MODE
                               select new { M.MODE, E.MODE_DESC });
                foreach (var obj in ModeLst)
                {
                    Mode listMode = new Mode();
                    listMode.ModeCode = obj.MODE;
                    listMode.ModeDescription = obj.MODE_DESC;
                    ModeList.Add(listMode);
                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ModeList;
        }
        public List<ShopLimits> GetRSByShopModes(string Shop_CD, string Mode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<ShopLimits> ShopLimitList = new List<ShopLimits>();
            try
            {

                var LimitLst = (from L in objContext.MESC1TS_SHOP_LIMITS
                                from U in objContext.SEC_USER
                                .Where(U => U.LOGIN == L.CHUSER)
                                .DefaultIfEmpty()
                                join M in objContext.MESC1TS_MODE
                                on L.MODE equals M.MODE
                                where L.SHOP_CD == Shop_CD & L.MODE == Mode
                                select new { L.SHOP_CD, L.MODE, M.MODE_DESC, L.REPAIR_AMT_LIMIT, L.SHOP_MATERIAL_LIMIT, L.AUTO_APPROVE_LIMIT, L.CHTS, U.FIRSTNAME, U.LASTNAME });

                foreach (var obj in LimitLst)
                {
                    ShopLimits listLimit = new ShopLimits();
                    listLimit.ShopCode = obj.SHOP_CD;
                    listLimit.Mode = obj.MODE;
                    listLimit.ModeDesc = obj.MODE_DESC;
                    listLimit.RepairAmtLimit = obj.REPAIR_AMT_LIMIT;
                    listLimit.ShopMaterialLimit = obj.SHOP_MATERIAL_LIMIT;
                    listLimit.AutoApproveLimit = obj.AUTO_APPROVE_LIMIT;
                    listLimit.ChangeTime = obj.CHTS;
                    listLimit.ChangeUser = obj.FIRSTNAME + " " + obj.LASTNAME;
                    listLimit.FName = obj.FIRSTNAME;
                    listLimit.LName = obj.LASTNAME;
                    ShopLimitList.Add(listLimit);
                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ShopLimitList;
        }

        public string InsertShopContract(ShopCont ShopContList)
        {
            string result = "";
            objContext = new ManageMasterDataServiceEntities();
            var RepairCode = (from reprcode in objContext.MESC1TS_REPAIR_CODE
                              where reprcode.MODE == ShopContList.Mode && reprcode.MANUAL_CD == ShopContList.ManualCode && reprcode.REPAIR_CD == ShopContList.RepairCode
                              select reprcode).ToList();
            if (RepairCode.Count() == 0)
            {
                result = "Manual / Mode / Repair Code is not valid in Repair Code table";
            }
            else
            {

                var Existdata = (from shpLimit in objContext.MESC1TS_SHOP_CONT
                                 where shpLimit.SHOP_CD == ShopContList.ShopCode && shpLimit.MANUAL_CD == ShopContList.ManualCode && shpLimit.MODE == ShopContList.Mode && shpLimit.REPAIR_CD == ShopContList.RepairCode &&
                                 shpLimit.EXP_DTE > ShopContList.EffDate
                                 select shpLimit).ToList();
                if (Existdata.Count() > 0)
                {
                    result = ShopContList.ShopCode + "," + ShopContList.ManualCode + "," + ShopContList.Mode + "," + ShopContList.RepairCode + " already exists.";
                }
                else
                {
                    MESC1TS_SHOP_CONT ShopContToBeInserted = new MESC1TS_SHOP_CONT();
                    ShopContToBeInserted.SHOP_CD = ShopContList.ShopCode;
                    ShopContToBeInserted.MODE = ShopContList.Mode;
                    ShopContToBeInserted.MANUAL_CD = ShopContList.ManualCode;
                    ShopContToBeInserted.REPAIR_CD = ShopContList.RepairCode;
                    ShopContToBeInserted.CONTRACT_AMOUNT = Convert.ToDecimal(ShopContList.ContractAmount);
                    if (ShopContList.EffDate.ToShortDateString() == "1/1/0001" || ShopContList.EffDate == null)
                    {
                        ShopContToBeInserted.EFF_DTE = Convert.ToDateTime("1/1/1753 12:00:00 AM");
                    }
                    else
                    {
                        ShopContToBeInserted.EFF_DTE = Convert.ToDateTime(ShopContList.EffDate);
                    }
                    if (ShopContList.ExpDate.ToShortDateString() == "1/1/0001" || ShopContList.ExpDate == null)
                    {
                        ShopContToBeInserted.EXP_DTE = Convert.ToDateTime("12/31/9999 11:59:59 PM");
                    }
                    else
                    {
                        ShopContToBeInserted.EXP_DTE = (ShopContList.ExpDate).AddHours(23).AddMinutes(59);
                    }
                    //ShopContToBeInserted.EXP_DTE = Convert.ToDateTime("01/01/0001 12:00:00 AM");
                    ShopContToBeInserted.CHUSER = ShopContList.ChangeUser;
                    ShopContToBeInserted.CHTS = DateTime.Now;

                    objContext.MESC1TS_SHOP_CONT.Add(ShopContToBeInserted);
                    try
                    {
                        objContext.SaveChanges();
                        result = "Success";
                    }

                    catch (Exception ex)
                    {
                        logEntry.Message = ex.ToString();
                        Logger.Write(logEntry);
                        result = "Failed to insert.";
                    }
                }
            }
            return result;

        } 
        public string UpdateShopContract(ShopCont ShopList)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_SHOP_CONT> ShopContData = new List<MESC1TS_SHOP_CONT>();
            string message = "";
            var RepairCode = (from reprcode in objContext.MESC1TS_REPAIR_CODE
                              where reprcode.MODE == ShopList.Mode && reprcode.MANUAL_CD == ShopList.ManualCode && reprcode.REPAIR_CD == ShopList.RepairCode
                              select reprcode).ToList();
            if (RepairCode.Count() == 0)
            {
                message = "Manual / Mode / Repair Code is not valid in Repair Code table";
            }
            else
            {
                try
                {
                    ShopContData = (from Cont in objContext.MESC1TS_SHOP_CONT
                                    where Cont.SHOP_CONT_ID == ShopList.ShopContID
                                    //where Cont.SHOP_CD == ShopList.ShopCode && Cont.MODE == ShopList.Mode && Cont.MANUAL_CD == ShopList.ManualCode
                                    //&& Cont.REPAIR_CD == ShopList.RepairCode
                                    select Cont).ToList();
                    if (ShopContData.Count() > 0)
                    {
                        if (ShopContData[0].CONTRACT_AMOUNT != ShopList.ContractAmount)
                        {
                            MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                            RefAudit.UNIQUE_ID = ShopContData[0].SHOP_CD + ShopContData[0].REPAIR_CD;
                            RefAudit.TAB_NAME = "MESC1TS_SHOP_CONT";
                            RefAudit.COL_NAME = "CONTRACT_AMOUNT";
                            RefAudit.OLD_VALUE = ShopContData[0].CONTRACT_AMOUNT.ToString();
                            RefAudit.NEW_VALUE = ShopList.ContractAmount.ToString();
                            RefAudit.CHTS = DateTime.Now;
                            RefAudit.CHUSER = ShopList.ChangeUser;
                            objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                            objContext.SaveChanges();
                        }
                        if (ShopList.ExpDate.ToShortDateString() != "1/1/0001")
                        {
                            if (ShopContData[0].EXP_DTE != ShopList.ExpDate)
                            {
                                MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                                RefAudit.UNIQUE_ID = ShopContData[0].SHOP_CD + ShopContData[0].REPAIR_CD;
                                RefAudit.TAB_NAME = "MESC1TS_SHOP_CONT";
                                RefAudit.COL_NAME = "EXP_DTE";
                                RefAudit.OLD_VALUE = ShopContData[0].EXP_DTE.ToString();
                                RefAudit.NEW_VALUE = (ShopList.ExpDate.AddHours(23).AddMinutes(59)).ToString();//Kasturee_shop_cont_29-01-2019
                                //RefAudit.NEW_VALUE = ShopList.ExpDate.ToString();
                                RefAudit.CHTS = DateTime.Now;
                                RefAudit.CHUSER = ShopList.ChangeUser;
                                objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                                objContext.SaveChanges();
                            }
                        }
                        ShopContData[0].CONTRACT_AMOUNT = ShopList.ContractAmount;

                        if (ShopList.ExpDate.ToShortDateString() == "1/1/0001" || ShopList.ExpDate == null)
                        {
                            ShopContData[0].EXP_DTE = Convert.ToDateTime("12/31/9999 11:59:59 PM");
                        }
                        else
                        {
                            //ShopContData[0].EXP_DTE = Convert.ToDateTime(ShopList.ExpDate);
                            ShopContData[0].EXP_DTE = (Convert.ToDateTime(ShopList.ExpDate)).AddHours(23).AddMinutes(59); //Kasturee_shop_cont_29-01-2019
                        }
                        ShopContData[0].CHUSER = ShopList.ChangeUser;
                        ShopContData[0].CHTS = DateTime.Now;
                        objContext.SaveChanges();
                        message = "Success";
                    }
                    else
                    {
                        message = "Not Exist";
                    }

                }
                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                }
            }
            return message;
        }
        public ShopCont FillShopContractEdit(int ShopContIDs)
        {
            objContext = new ManageMasterDataServiceEntities();
            ShopCont objShopCont = null;
            try
            {

                var ShopContData = (from Cont in objContext.MESC1TS_SHOP_CONT
                                    where Cont.SHOP_CONT_ID == ShopContIDs
                                    select Cont).ToList();
                if (ShopContData.Count > 0)
                {
                    objShopCont = new ShopCont();
                    objShopCont.ShopContID = ShopContData[0].SHOP_CONT_ID;
                    objShopCont.ShopCode = ShopContData[0].SHOP_CD;
                    objShopCont.ManualCode = ShopContData[0].MANUAL_CD;
                    objShopCont.Mode = ShopContData[0].MODE;
                    objShopCont.RepairCode = ShopContData[0].REPAIR_CD;
                    objShopCont.ContractAmount = ShopContData[0].CONTRACT_AMOUNT;
                    objShopCont.EffDate = ShopContData[0].EFF_DTE;
                    objShopCont.ExpDate = ShopContData[0].EXP_DTE;
                    objShopCont.ChangeUser = ShopContData[0].CHUSER;
                    objShopCont.ChangeTime = ShopContData[0].CHTS;


                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return objShopCont;
        }
        public string DeleteShopContract(string gridData)
        {
            objContext = new ManageMasterDataServiceEntities();
            string[] strArray = gridData.Split(',');
            int[] ShopContIDs = Array.ConvertAll(strArray, int.Parse);
            
            string message = "";
            try
            {

                var ShopContData = (from Cont in objContext.MESC1TS_SHOP_CONT
                                    where ShopContIDs.Contains(Cont.SHOP_CONT_ID)
                                    select Cont).ToList();
                if (ShopContData.Count() > 0)
                {
                    foreach (var obj in ShopContData)
                    {
                        objContext.MESC1TS_SHOP_CONT.Remove(obj);
                        objContext.SaveChanges();
                    }

                    message = "Success";
                }
                else
                {
                    message = "Error in deleting the record...";
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                message = "Error in deleting...";
            }

            return message;
        }
       


            public string UpdateExpDateForShopContract(string gridData, DateTime expDate, string CHUser)
        {
            objContext = new ManageMasterDataServiceEntities();
            string[] strArray = gridData.Split(',');
            int[] ShopContIDs = Array.ConvertAll(strArray, int.Parse);
            string message = "";
            try
            {

                var ShopContData = (from Cont in objContext.MESC1TS_SHOP_CONT
                                    where ShopContIDs.Contains(Cont.SHOP_CONT_ID)
                                    select Cont).ToList();

                if (ShopContData.Count > 0)
                {

                    foreach (MESC1TS_SHOP_CONT p in ShopContData)
                    {
                        if (expDate.ToShortDateString() != "1/1/0001")
                        {
                            if (ShopContData[0].EXP_DTE != expDate)
                            {
                                MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                                RefAudit.UNIQUE_ID = ShopContData[0].SHOP_CD + ShopContData[0].REPAIR_CD;
                                RefAudit.TAB_NAME = "MESC1TS_SHOP_CONT";
                                RefAudit.COL_NAME = "EXP_DTE";
                                RefAudit.OLD_VALUE = ShopContData[0].EXP_DTE.ToString();
                                //RefAudit.NEW_VALUE = expDate.ToString();
                                RefAudit.NEW_VALUE = (expDate.AddHours(23).AddMinutes(59)).ToString();//Kasturee_shop_cont_29-01-2019;
                                RefAudit.CHTS = DateTime.Now;
                                RefAudit.CHUSER = CHUser;
                                objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                                objContext.SaveChanges();
                            }
                        }
                        if (expDate.ToShortDateString() == "1/1/0001" || expDate == null)
                        {
                            p.EXP_DTE = Convert.ToDateTime("1/1/1753 12:00:00 AM");
                        }
                        else
                        {
                            //p.EXP_DTE = Convert.ToDateTime(expDate);
                            p.EXP_DTE = (Convert.ToDateTime(expDate)).AddHours(23).AddMinutes(59); //Kasturee_shop_cont_29-01-2019;
                        }
                        p.CHUSER = CHUser;
                        p.CHTS = DateTime.Now;
                    }
                    objContext.SaveChanges();
                    message = "Success";

                }
                else
                {
                    message = "Data is not exist to update... ";
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                message = "Error in updating...";
            }

            return message;
        }

        public string InsertShopLimit(ShopLimits ShopLimit)
        {
            string result = "";
            objContext = new ManageMasterDataServiceEntities();
            var Existdata = (from shpLimit in objContext.MESC1TS_SHOP_LIMITS
                             where shpLimit.SHOP_CD == ShopLimit.ShopCode && shpLimit.MODE == ShopLimit.Mode
                             select shpLimit).ToList();
            if (Existdata.Count() > 0)
            {
                result = "Same shoplimit already exist for ShopCode= " + ShopLimit.ShopCode + " and Mode= " + ShopLimit.Mode;
            }
            else
            {
                MESC1TS_SHOP_LIMITS ShopLimitToBeInserted = new MESC1TS_SHOP_LIMITS();
                ShopLimitToBeInserted.SHOP_CD = ShopLimit.ShopCode;
                ShopLimitToBeInserted.MODE = ShopLimit.Mode;
                if (ShopLimit.RepairAmtLimit > 0)
                {
                    ShopLimitToBeInserted.REPAIR_AMT_LIMIT = ShopLimit.RepairAmtLimit;
                }
                else
                {
                    ShopLimitToBeInserted.REPAIR_AMT_LIMIT = 0;
                }
                if (ShopLimit.ShopMaterialLimit > 0)
                {
                    ShopLimitToBeInserted.SHOP_MATERIAL_LIMIT = ShopLimit.ShopMaterialLimit;
                }
                else
                {
                    ShopLimitToBeInserted.SHOP_MATERIAL_LIMIT = 0;
                }
                if (ShopLimit.AutoApproveLimit > 0)
                {
                    ShopLimitToBeInserted.AUTO_APPROVE_LIMIT = ShopLimit.AutoApproveLimit;
                }
                else
                {
                    ShopLimitToBeInserted.AUTO_APPROVE_LIMIT = 0;
                }
                ShopLimitToBeInserted.CHUSER = ShopLimit.ChangeUser;
                ShopLimitToBeInserted.CHTS = DateTime.Now;

                objContext.MESC1TS_SHOP_LIMITS.Add(ShopLimitToBeInserted);
                try
                {
                    objContext.SaveChanges();
                    result = "Success";
                }

                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    result = "Failed: " + ex.ToString();
                }
            }
            return result;

        }
        public string UpdateShopLimit(ShopLimits ShopLimit)
        {
            objContext = new ManageMasterDataServiceEntities();
            string message = "";
            List<MESC1TS_SHOP_LIMITS> ShopLimitData = new List<MESC1TS_SHOP_LIMITS>();
            try
            {
                ShopLimitData = (from Limit in objContext.MESC1TS_SHOP_LIMITS
                                 where Limit.SHOP_CD == ShopLimit.ShopCode && Limit.MODE == ShopLimit.Mode
                                 select Limit).ToList();
                if (ShopLimitData.Count() > 0)
                {
                    if (ShopLimitData[0].REPAIR_AMT_LIMIT != ShopLimit.RepairAmtLimit)
                    {
                        MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                        RefAudit.UNIQUE_ID = ShopLimitData[0].SHOP_CD + ShopLimitData[0].MODE;
                        RefAudit.TAB_NAME = "MESC1TS_SHOP_LIMITS";
                        RefAudit.COL_NAME = "REPAIR_AMT_LIMIT";
                        RefAudit.OLD_VALUE = ShopLimitData[0].REPAIR_AMT_LIMIT.ToString();
                        RefAudit.NEW_VALUE = ShopLimit.RepairAmtLimit.ToString();
                        RefAudit.CHTS = DateTime.Now;
                        RefAudit.CHUSER = ShopLimit.ChangeUser;
                        objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                        objContext.SaveChanges();
                    }
                    if (ShopLimitData[0].AUTO_APPROVE_LIMIT != ShopLimit.AutoApproveLimit)
                    {
                        MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                        RefAudit.UNIQUE_ID = ShopLimitData[0].SHOP_CD + ShopLimitData[0].MODE;
                        RefAudit.TAB_NAME = "MESC1TS_SHOP_LIMITS";
                        RefAudit.COL_NAME = "AUTO_APPROVE_LIMIT";
                        RefAudit.OLD_VALUE = ShopLimitData[0].AUTO_APPROVE_LIMIT.ToString();
                        RefAudit.NEW_VALUE = ShopLimit.AutoApproveLimit.ToString();
                        RefAudit.CHTS = DateTime.Now;
                        RefAudit.CHUSER = ShopLimit.ChangeUser;
                        objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                        objContext.SaveChanges();

                    }
                    if (ShopLimitData[0].SHOP_MATERIAL_LIMIT != ShopLimit.ShopMaterialLimit)
                    {
                        MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                        RefAudit.UNIQUE_ID = ShopLimitData[0].SHOP_CD + ShopLimitData[0].MODE;
                        RefAudit.TAB_NAME = "MESC1TS_SHOP_LIMITS";
                        RefAudit.COL_NAME = "SHOP_MATERIAL_LIMIT";
                        RefAudit.OLD_VALUE = ShopLimitData[0].SHOP_MATERIAL_LIMIT.ToString();
                        RefAudit.NEW_VALUE = ShopLimit.ShopMaterialLimit.ToString();
                        RefAudit.CHTS = DateTime.Now;
                        RefAudit.CHUSER = ShopLimit.ChangeUser;
                        objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                        objContext.SaveChanges();
                    }


                    //////////////
                    if (ShopLimit.RepairAmtLimit > 0)
                    {
                        ShopLimitData[0].REPAIR_AMT_LIMIT = ShopLimit.RepairAmtLimit;
                    }
                    else
                    {
                        ShopLimitData[0].REPAIR_AMT_LIMIT = 0;
                    }
                    if (ShopLimit.ShopMaterialLimit > 0)
                    {
                        ShopLimitData[0].SHOP_MATERIAL_LIMIT = ShopLimit.ShopMaterialLimit;
                    }
                    else
                    {
                        ShopLimitData[0].SHOP_MATERIAL_LIMIT = 0;
                    }
                    if (ShopLimit.AutoApproveLimit > 0)
                    {
                        ShopLimitData[0].AUTO_APPROVE_LIMIT = ShopLimit.AutoApproveLimit;
                    }
                    else
                    {
                        ShopLimitData[0].AUTO_APPROVE_LIMIT = 0;
                    }
                    ShopLimitData[0].CHUSER = ShopLimit.ChangeUser;
                    ShopLimitData[0].CHTS = DateTime.Now;

                    int result = objContext.SaveChanges();
                    if (result == 1)
                    {
                        message = "Success";
                    }
                    else
                    {
                        message = "Fail";
                    }
                }
                else
                {
                    message = "Same shoplimit already exist for ShopCode= " + ShopLimit.ShopCode + " and Mode= " + ShopLimit.Mode;
                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                message = "Failed: " + ex.ToString();
            }
            return message;

        }

        public string InsertShopProfile(Shop ShopList)
        {
            string message = "";
            objContext = new ManageMasterDataServiceEntities();
            var Existdata = (from shop in objContext.MESC1TS_SHOP
                             where shop.SHOP_CD == ShopList.ShopCode
                             select shop).ToList();
            if (Existdata.Count() > 0)
            {
                message = "Shop Code " + ShopList.ShopCode + " already exists. Not added.";
            }
            else
            {
                MESC1TS_SHOP ShopData = new MESC1TS_SHOP();

                if (ShopList.ShopCode != null)
                {
                    ShopData.SHOP_CD = ShopList.ShopCode;
                }
                if (ShopList.ShopActiveSW != null)
                {
                    ShopData.SHOP_ACTIVE_SW = ShopList.ShopActiveSW;
                }

                if (ShopList.ShopDescription != null)
                {
                    ShopData.SHOP_DESC = ShopList.ShopDescription;
                }

                if (ShopList.ShopTypeCode != null)
                {
                    ShopData.SHOP_TYPE_CD = ShopList.ShopTypeCode;
                }
                if (ShopList.VendorCode != null)
                {
                    ShopData.VENDOR_CD = ShopList.VendorCode;
                }

                if (ShopList.LocationCode != null)
                {
                    ShopData.LOC_CD = ShopList.LocationCode;
                }

                if (ShopList.RKRPloc != null)
                {
                    ShopData.RKRPLOC = ShopList.RKRPloc;
                }
                if (ShopList.SalesTaxPartCont != null)
                {
                    ShopData.SALES_TAX_PART_CONT = ShopList.SalesTaxPartCont;
                }
                else
                {
                    ShopData.SALES_TAX_PART_CONT = 0;
                }

                if (ShopList.SalesTaxLaborCon != null)
                {
                    ShopData.SALES_TAX_LABOR_CON = ShopList.SalesTaxLaborCon;
                }
                else
                {
                    ShopData.SALES_TAX_LABOR_CON = 0;
                }
                if (ShopList.CUCDN != null)
                {
                    ShopData.CUCDN = ShopList.CUCDN;
                }
                if (ShopList.SalesTaxPartGen != null)
                {
                    ShopData.SALES_TAX_PART_GEN = ShopList.SalesTaxPartGen;
                }
                else
                {
                    ShopData.SALES_TAX_PART_GEN = 0;
                }

                if (ShopList.SalesTaxLaborGen != null)
                {
                    ShopData.SALES_TAX_LABOR_GEN = ShopList.SalesTaxLaborGen;
                }
                else
                {
                    ShopData.SALES_TAX_LABOR_GEN = 0;
                }
                if (ShopList.EmailAdress != "")
                {
                    ShopData.EMAIL_ADR = ShopList.EmailAdress;
                }
                if (ShopList.ImportTax != null)
                {
                    ShopData.IMPORT_TAX = ShopList.ImportTax;
                }
                else
                {
                    ShopData.IMPORT_TAX = 0;
                }
                if (ShopList.Phone.Trim() != "")
                {
                    ShopData.PHONE = ShopList.Phone.Trim();
                }
                if (ShopList.PCTMaterialFactor != null)
                {
                    ShopData.PCT_MATERIAL_FACTOR = ShopList.PCTMaterialFactor;
                }
                else
                {
                    ShopData.PCT_MATERIAL_FACTOR = 0;
                }
                if (ShopList.RRISXmitSW != null)
                {
                    ShopData.RRIS_XMIT_SW = ShopList.RRISXmitSW;
                }
                if (ShopList.AcepSW != null)
                {
                    ShopData.ACEP_SW = ShopList.AcepSW;
                }
                if (ShopList.RRIS70SuffixCode != null)
                {
                    ShopData.RRIS70_SUFFIX_CD = ShopList.RRIS70SuffixCode;
                }
                if (ShopList.OvertimeSuspSW != null)
                {
                    ShopData.OVERTIME_SUSP_SW = ShopList.OvertimeSuspSW;
                }
                if (ShopList.PreptimeSW != null)
                {
                    ShopData.PREPTIME_SW = ShopList.PreptimeSW;
                }
                if (ShopList.Decentralized != null)
                {
                    ShopData.DECENTRALIZED = ShopList.Decentralized;
                }
                if (ShopList.BypassLeaseRules != null)
                {
                    ShopData.BYPASS_LEASE_RULES = ShopList.BypassLeaseRules;
                }
                if (ShopList.AutoCompleteSW != null)
                {
                    ShopData.AUTO_COMPLETE_SW = ShopList.AutoCompleteSW;
                }
                if (ShopList.ChangeUser != null)
                {
                    ShopData.CHUSER = ShopList.ChangeUser;
                }

                ShopData.CHTS = DateTime.Now;

                try
                {
                    objContext.MESC1TS_SHOP.Add(ShopData);
                    int result = objContext.SaveChanges();
                    if (result == 1)
                    {
                        message = "Success";
                    }
                    else
                    {
                        message = "Error in adding the data. Please contact the System Administrator.";
                    }
                }
                catch (Exception ex)
                {
                    logEntry.Message = ex.ToString();
                    Logger.Write(logEntry);
                    return "Error in adding the data. Please contact the System Administrator.";
                }
            }

            return message;
        }
        public string UpdateShopProfile(Shop ShopList)
        {
            objContext = new ManageMasterDataServiceEntities();
            string result = "";
            List<MESC1TS_SHOP> ShopData = new List<MESC1TS_SHOP>();
            ShopData = (from shop in objContext.MESC1TS_SHOP
                        where shop.SHOP_CD == ShopList.ShopCode
                        select shop).ToList();
            if (ShopData.Count() > 0)
            {
                if ((ShopData[0].SHOP_ACTIVE_SW == null ? "" : ShopData[0].SHOP_ACTIVE_SW.Trim()) != ShopList.ShopActiveSW.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "SHOP_ACTIVE_SW";
                    RefAudit.OLD_VALUE = ShopData[0].SHOP_ACTIVE_SW == null ? "" : ShopData[0].SHOP_ACTIVE_SW;
                    RefAudit.NEW_VALUE = ShopList.ShopActiveSW.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].SHOP_DESC == null ? "" : ShopData[0].SHOP_DESC.Trim()) != ShopList.ShopDescription.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "SHOP_DESC";
                    RefAudit.OLD_VALUE = ShopData[0].SHOP_DESC == null ? "" : ShopData[0].SHOP_DESC;
                    RefAudit.NEW_VALUE = ShopList.ShopDescription.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].SHOP_TYPE_CD == null ? "" : ShopData[0].SHOP_TYPE_CD.Trim()) != ShopList.ShopTypeCode.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "SHOP_TYPE_CD";
                    RefAudit.OLD_VALUE = ShopData[0].SHOP_TYPE_CD == null ? "" : ShopData[0].SHOP_TYPE_CD.Trim();
                    RefAudit.NEW_VALUE = ShopList.ShopTypeCode.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].VENDOR_CD == null ? "" : ShopData[0].VENDOR_CD.Trim()) != ShopList.VendorCode.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "VENDOR_CD";
                    RefAudit.OLD_VALUE = ShopData[0].VENDOR_CD == null ? "" : ShopData[0].VENDOR_CD.Trim();
                    RefAudit.NEW_VALUE = ShopList.VendorCode.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].LOC_CD == null ? "" : ShopData[0].LOC_CD.Trim()) != ShopList.LocationCode.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "LOC_CD";
                    RefAudit.OLD_VALUE = ShopData[0].LOC_CD == null ? "" : ShopData[0].LOC_CD.Trim();
                    RefAudit.NEW_VALUE = ShopList.LocationCode.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].RKRPLOC == null ? "" : ShopData[0].RKRPLOC.Trim()) != ShopList.RKRPloc.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "RKRPLOC";
                    RefAudit.OLD_VALUE = ShopData[0].RKRPLOC == null ? "" : ShopData[0].RKRPLOC.Trim();
                    RefAudit.NEW_VALUE = ShopList.RKRPloc.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].SALES_TAX_PART_CONT == null ? 0.0 : ShopData[0].SALES_TAX_PART_CONT) != ShopList.SalesTaxPartCont)
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "SALES_TAX_PART_CONT";
                    RefAudit.OLD_VALUE = ShopData[0].SALES_TAX_PART_CONT == null ? "" : ShopData[0].SALES_TAX_PART_CONT.ToString();
                    RefAudit.NEW_VALUE = ShopList.SalesTaxPartCont.ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].SALES_TAX_LABOR_CON == null ? 0.0 : ShopData[0].SALES_TAX_LABOR_CON) != ShopList.SalesTaxLaborCon)
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "SALES_TAX_LABOR_CON";
                    RefAudit.OLD_VALUE = ShopData[0].SALES_TAX_LABOR_CON == null ? "" : ShopData[0].SALES_TAX_LABOR_CON.ToString();
                    RefAudit.NEW_VALUE = ShopList.SalesTaxLaborCon.ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].CUCDN == null ? "" : ShopData[0].CUCDN) != ShopList.CUCDN.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "CUCDN";
                    RefAudit.OLD_VALUE = ShopData[0].CUCDN == null ? "" : ShopData[0].CUCDN.ToString();
                    RefAudit.NEW_VALUE = ShopList.CUCDN.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].SALES_TAX_PART_GEN == null ? 0.0 : ShopData[0].SALES_TAX_PART_GEN) != ShopList.SalesTaxPartGen)
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "SALES_TAX_PART_GEN";
                    RefAudit.OLD_VALUE = ShopData[0].SALES_TAX_PART_GEN == null ? "" : ShopData[0].SALES_TAX_PART_GEN.ToString();
                    RefAudit.NEW_VALUE = ShopList.SalesTaxPartGen.ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].SALES_TAX_LABOR_GEN == null ? 0.0 : ShopData[0].SALES_TAX_LABOR_GEN) != ShopList.SalesTaxLaborGen)
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "SALES_TAX_LABOR_GEN";
                    RefAudit.OLD_VALUE = ShopData[0].SALES_TAX_LABOR_GEN == null ? "" : ShopData[0].SALES_TAX_LABOR_GEN.ToString();
                    RefAudit.NEW_VALUE = ShopList.SalesTaxLaborGen.ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].EMAIL_ADR == null ? "" : ShopData[0].EMAIL_ADR.Trim()) != ShopList.EmailAdress.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "EMAIL_ADR";
                    RefAudit.OLD_VALUE = ShopData[0].EMAIL_ADR == null ? "" : ShopData[0].EMAIL_ADR.Trim().ToString();
                    RefAudit.NEW_VALUE = ShopList.EmailAdress.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].IMPORT_TAX == null ? 0.0 : ShopData[0].IMPORT_TAX) != ShopList.ImportTax)
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "IMPORT_TAX";
                    RefAudit.OLD_VALUE = ShopData[0].IMPORT_TAX == null ? "" : ShopData[0].IMPORT_TAX.ToString();
                    RefAudit.NEW_VALUE = ShopList.ImportTax.ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].PHONE == null ? "" : ShopData[0].PHONE.Trim()) != ShopList.Phone.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "PHONE";
                    RefAudit.OLD_VALUE = ShopData[0].PHONE == null ? "" : ShopData[0].PHONE.Trim().ToString();
                    RefAudit.NEW_VALUE = ShopList.Phone.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].PCT_MATERIAL_FACTOR == null ? 0.0 : ShopData[0].PCT_MATERIAL_FACTOR) != ShopList.PCTMaterialFactor)
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "PCT_MATERIAL_FACTOR";
                    RefAudit.OLD_VALUE = ShopData[0].PCT_MATERIAL_FACTOR == null ? "" : ShopData[0].PCT_MATERIAL_FACTOR.ToString();
                    RefAudit.NEW_VALUE = ShopList.PCTMaterialFactor.ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].RRIS_XMIT_SW == null ? "" : ShopData[0].RRIS_XMIT_SW.Trim()) != ShopList.RRISXmitSW.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "RRIS_XMIT_SW";
                    RefAudit.OLD_VALUE = ShopData[0].RRIS_XMIT_SW == null ? "" : ShopData[0].RRIS_XMIT_SW.ToString();
                    RefAudit.NEW_VALUE = ShopList.RRISXmitSW.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].ACEP_SW == null ? "" : ShopData[0].ACEP_SW.Trim()) != ShopList.AcepSW.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "ACEP_SW";
                    RefAudit.OLD_VALUE = ShopData[0].ACEP_SW == null ? "" : ShopData[0].ACEP_SW.ToString();
                    RefAudit.NEW_VALUE = ShopList.AcepSW.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].RRIS70_SUFFIX_CD == null ? "" : ShopData[0].RRIS70_SUFFIX_CD.Trim()) != ShopList.RRIS70SuffixCode.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "RRIS70_SUFFIX_CD";
                    RefAudit.OLD_VALUE = ShopData[0].RRIS70_SUFFIX_CD == null ? "" : ShopData[0].RRIS70_SUFFIX_CD;
                    RefAudit.NEW_VALUE = ShopList.RRIS70SuffixCode.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].OVERTIME_SUSP_SW == null ? "" : ShopData[0].OVERTIME_SUSP_SW.Trim()) != ShopList.OvertimeSuspSW.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "OVERTIME_SUSP_SW";
                    RefAudit.OLD_VALUE = ShopData[0].OVERTIME_SUSP_SW == null ? "" : ShopData[0].OVERTIME_SUSP_SW;
                    RefAudit.NEW_VALUE = ShopList.OvertimeSuspSW.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].PREPTIME_SW == null ? "" : ShopData[0].PREPTIME_SW.Trim()) != ShopList.PreptimeSW.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "PREPTIME_SW";
                    RefAudit.OLD_VALUE = ShopData[0].PREPTIME_SW == null ? "" : ShopData[0].PREPTIME_SW;
                    RefAudit.NEW_VALUE = ShopList.PreptimeSW.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].DECENTRALIZED == null ? "" : ShopData[0].DECENTRALIZED.Trim()) != ShopList.Decentralized.Trim())
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "DECENTRALIZED";
                    RefAudit.OLD_VALUE = ShopData[0].DECENTRALIZED == null ? "" : ShopData[0].DECENTRALIZED;
                    RefAudit.NEW_VALUE = ShopList.Decentralized.Trim().ToString();
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].BYPASS_LEASE_RULES == null ? "" : ShopData[0].BYPASS_LEASE_RULES) != ShopList.BypassLeaseRules)
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "BYPASS_LEASE_RULES";
                    RefAudit.OLD_VALUE = ShopData[0].BYPASS_LEASE_RULES == null ? "" : ShopData[0].BYPASS_LEASE_RULES;
                    RefAudit.NEW_VALUE = ShopList.BypassLeaseRules;
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }
                if ((ShopData[0].AUTO_COMPLETE_SW == null ? "" : ShopData[0].AUTO_COMPLETE_SW) != ShopList.AutoCompleteSW)
                {
                    MESC1TS_REFAUDIT RefAudit = new MESC1TS_REFAUDIT();
                    RefAudit.UNIQUE_ID = ShopData[0].SHOP_CD;
                    RefAudit.TAB_NAME = "MESC1TS_SHOP";
                    RefAudit.COL_NAME = "AUTO_COMPLETE_SW";
                    RefAudit.OLD_VALUE = ShopData[0].AUTO_COMPLETE_SW == null ? "" : ShopData[0].AUTO_COMPLETE_SW;
                    RefAudit.NEW_VALUE = ShopList.AutoCompleteSW;
                    RefAudit.CHTS = DateTime.Now;
                    RefAudit.CHUSER = ShopList.ChangeUser;
                    objContext.MESC1TS_REFAUDIT.Add(RefAudit);
                    objContext.SaveChanges();
                }


                if (ShopList.ShopActiveSW != null)
                {
                    ShopData[0].SHOP_ACTIVE_SW = ShopList.ShopActiveSW;
                }

                if (ShopList.ShopDescription != null)
                {
                    ShopData[0].SHOP_DESC = ShopList.ShopDescription;
                }

                if (ShopList.ShopTypeCode != null)
                {
                    ShopData[0].SHOP_TYPE_CD = ShopList.ShopTypeCode;
                }
                if (ShopList.VendorCode != null)
                {
                    ShopData[0].VENDOR_CD = ShopList.VendorCode;
                }

                if (ShopList.LocationCode != null)
                {
                    ShopData[0].LOC_CD = ShopList.LocationCode;
                }

                if (ShopList.RKRPloc != null)
                {
                    ShopData[0].RKRPLOC = ShopList.RKRPloc;
                }
                if (ShopList.SalesTaxPartCont != null)
                {
                    ShopData[0].SALES_TAX_PART_CONT = ShopList.SalesTaxPartCont;
                }
                else
                {
                    ShopData[0].SALES_TAX_PART_CONT = 0;
                }

                if (ShopList.SalesTaxLaborCon != null)
                {
                    ShopData[0].SALES_TAX_LABOR_CON = ShopList.SalesTaxLaborCon;
                }
                else
                {
                    ShopData[0].SALES_TAX_LABOR_CON = 0;
                }
                if (ShopList.CUCDN != null)
                {
                    ShopData[0].CUCDN = ShopList.CUCDN;
                }
                if (ShopList.SalesTaxPartGen != null)
                {
                    ShopData[0].SALES_TAX_PART_GEN = ShopList.SalesTaxPartGen;
                }
                else
                {
                    ShopData[0].SALES_TAX_PART_GEN = 0;
                }

                if (ShopList.SalesTaxLaborGen != null)
                {
                    ShopData[0].SALES_TAX_LABOR_GEN = ShopList.SalesTaxLaborGen;
                }
                else
                {
                    ShopData[0].SALES_TAX_LABOR_GEN = 0;
                }

                ShopData[0].EMAIL_ADR = ShopList.EmailAdress;

                if (ShopList.ImportTax != null)
                {
                    ShopData[0].IMPORT_TAX = ShopList.ImportTax;
                }
                else
                {
                    ShopData[0].IMPORT_TAX = 0;
                }

                ShopData[0].PHONE = ShopList.Phone.Trim();
                if (ShopList.PCTMaterialFactor != null)
                {
                    ShopData[0].PCT_MATERIAL_FACTOR = ShopList.PCTMaterialFactor;
                }
                else
                {
                    ShopData[0].PCT_MATERIAL_FACTOR = 0;
                }
                if (ShopList.RRISXmitSW != null)
                {
                    ShopData[0].RRIS_XMIT_SW = ShopList.RRISXmitSW;
                }
                if (ShopList.AcepSW != null)
                {
                    ShopData[0].ACEP_SW = ShopList.AcepSW;
                }
                if (ShopList.RRIS70SuffixCode != null)
                {
                    ShopData[0].RRIS70_SUFFIX_CD = ShopList.RRIS70SuffixCode;
                }
                if (ShopList.OvertimeSuspSW != null)
                {
                    ShopData[0].OVERTIME_SUSP_SW = ShopList.OvertimeSuspSW;
                }
                if (ShopList.PreptimeSW != null)
                {
                    ShopData[0].PREPTIME_SW = ShopList.PreptimeSW;
                }
                if (ShopList.Decentralized != null)
                {
                    ShopData[0].DECENTRALIZED = ShopList.Decentralized;
                }
                if (!string.IsNullOrEmpty(ShopList.BypassLeaseRules))
                {
                    ShopData[0].BYPASS_LEASE_RULES = ShopList.BypassLeaseRules;
                }
                else
                {
                    ShopData[0].BYPASS_LEASE_RULES = "";
                }
                if (!string.IsNullOrEmpty(ShopList.AutoCompleteSW))
                {
                    ShopData[0].AUTO_COMPLETE_SW = ShopList.AutoCompleteSW;
                }
                else
                {
                    ShopData[0].AUTO_COMPLETE_SW = "";
                }
                if (ShopList.ChangeUser != null)
                {
                    ShopData[0].CHUSER = ShopList.ChangeUser;
                }

                ShopData[0].CHTS = DateTime.Now;
            }

            try
            {
                int ExecResult = objContext.SaveChanges();
                if (ExecResult == 1)
                {
                    result = "Success";
                }
                else
                {
                    result = "Error in updating the data. Please contact the System Administrator.";
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return "Error in updating the data. Please contact the System Administrator.";
            }

            return result;
        }



        public List<RefAudit> GetAuditTrailShop(string RecordId, string TableName)
        {
            List<RefAudit> RefAuditList = new List<RefAudit>();
            try
            {

                dynamic Query = "";
                objContext = new ManageMasterDataServiceEntities();
                string TabName = string.Empty;

                if (TableName == "ShopLimits")
                {
                    TabName = ("MESC1TS_SHOP_LIMITS");
                    //Query = (from MR in objContext.MESC1TS_REFAUDIT
                    //         from U in objContext.SEC_USER
                    //                 .Where(U => SqlFunctions.StringConvert((double)U.USER_ID).Trim() == MR.CHUSER)
                    //                   .DefaultIfEmpty()
                    //         where MR.TAB_NAME == "MESC1TS_SHOP_LIMITS" && MR.UNIQUE_ID == RecordId
                    //         select new { MR.TAB_NAME, MR.UNIQUE_ID, MR.COL_NAME, MR.OLD_VALUE, MR.NEW_VALUE, MR.CHUSER, U.LASTNAME, U.FIRSTNAME, MR.CHTS }).ToList();

                    List<SqlDataClass> cls = new List<SqlDataClass>();
                    Query = objContext.Database.SqlQuery<SqlDataClass>("SELECT R.TAB_NAME,R.UNIQUE_ID,R.COL_NAME,R.OLD_VALUE,R.NEW_VALUE,R.CHUSER,U.LASTNAME,U.FIRSTNAME,R.CHTS FROM MESC1TS_REFAUDIT R LEFT OUTER JOIN SEC_USER U ON U.LOGIN=R.CHUSER WHERE R.TAB_NAME = {0} AND R.UNIQUE_ID = {1} ORDER BY R.CHTS DESC", TabName, RecordId).ToList();

                }
                if (TableName == "ShopProfile")
                {
                    TabName = ("MESC1TS_SHOP");
                    //Query = (from MR in objContext.MESC1TS_REFAUDIT
                    //         from U in objContext.SEC_USER
                    //                   .Where(U => SqlFunctions.StringConvert((double)U.USER_ID).Trim() == MR.CHUSER)
                    //                   .DefaultIfEmpty()
                    //         where MR.TAB_NAME == TabName && MR.UNIQUE_ID == RecordId
                    //         select new { MR.TAB_NAME, MR.UNIQUE_ID, MR.COL_NAME, MR.OLD_VALUE, MR.NEW_VALUE, MR.CHUSER, U.LASTNAME, U.FIRSTNAME, MR.CHTS }).ToList();


                    //TabName = ("MESC1TS_SHOP");
                    List<SqlDataClass> cls = new List<SqlDataClass>();
                    Query = objContext.Database.SqlQuery<SqlDataClass>("SELECT R.TAB_NAME,R.UNIQUE_ID,R.COL_NAME,R.OLD_VALUE,R.NEW_VALUE,R.CHUSER,U.LASTNAME,U.FIRSTNAME,R.CHTS FROM MESC1TS_REFAUDIT R LEFT OUTER JOIN SEC_USER U ON U.LOGIN=R.CHUSER WHERE R.TAB_NAME = {0} AND R.UNIQUE_ID = {1} ORDER BY R.CHTS DESC", TabName, RecordId).ToList();

                }
                if (TableName == "ShopContract")
                {
                    Query = (from MR in objContext.MESC1TS_REFAUDIT
                             from U in objContext.SEC_USER
                                       .Where(U => U.LOGIN == MR.CHUSER)
                                       .DefaultIfEmpty()
                             where MR.TAB_NAME == "MESC1TS_SHOP_CONT" && MR.UNIQUE_ID == RecordId
                             select new { MR.TAB_NAME, MR.UNIQUE_ID, MR.COL_NAME, MR.OLD_VALUE, MR.NEW_VALUE, MR.CHUSER, U.LASTNAME, U.FIRSTNAME, MR.CHTS }).ToList();

                }
                if (TableName == "User")
                {
                    Query = (from MR in objContext.MESC1TS_REFAUDIT
                             from U in objContext.SEC_USER
                                       .Where(U => SqlFunctions.StringConvert((double)U.USER_ID) == MR.UNIQUE_ID)
                                       .DefaultIfEmpty()
                             where MR.TAB_NAME == "SEC_USER" && MR.UNIQUE_ID == RecordId
                             orderby MR.CHTS descending
                             select new { MR.TAB_NAME, MR.UNIQUE_ID, MR.COL_NAME, MR.OLD_VALUE, MR.NEW_VALUE, MR.CHUSER, U.LASTNAME, U.FIRSTNAME, MR.CHTS }).ToList();

                }
                foreach (var obj in Query)
                {
                    RefAudit objRef = new RefAudit();
                    objRef.TabName = obj.TAB_NAME;
                    objRef.UniqueID = obj.UNIQUE_ID;
                    objRef.ColName = obj.COL_NAME;
                    objRef.OldValue = obj.OLD_VALUE;
                    objRef.NewValue = obj.NEW_VALUE;
                    objRef.ChangeUser = obj.CHUSER;
                    objRef.FirstName = obj.FIRSTNAME;
                    objRef.LastName = obj.LASTNAME;
                    objRef.ChangeTime = obj.CHTS;
                    RefAuditList.Add(objRef);
                };
                RefAuditList = RefAuditList.OrderByDescending(m => m.ChangeTime).ToList();
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            return RefAuditList;
        }


        public class SqlDataClass
        {
            public string TAB_NAME { get; set; }
            public string UNIQUE_ID { get; set; }
            public string COL_NAME { get; set; }
            public string OLD_VALUE { get; set; }
            public string NEW_VALUE { get; set; }
            public string CHUSER { get; set; }
            public string LASTNAME { get; set; }
            public string FIRSTNAME { get; set; }
            public DateTime CHTS { get; set; }
        }
        public List<Manufactur> RSAllManufacturers()
        {
            List<Manufactur> ManufacturList = new List<Manufactur>();
            try
            {


                objContext = new ManageMasterDataServiceEntities();


                var Query = (from MM in objContext.MESC1TS_MANUFACTUR

                             select new { MM.MANUFCTR, MM.MANUFACTUR_NAME }).ToList();
                Manufactur objManufacturer = new Manufactur();
                objManufacturer.ManufacturCd = "";
                ManufacturList.Add(objManufacturer);

                foreach (var obj in Query)
                {
                    Manufactur objManufacturer1 = new Manufactur();
                    objManufacturer1.ManufacturCd = obj.MANUFCTR;
                    ManufacturList.Add(objManufacturer1);
                };
                ManufacturList.OrderBy(m => m.ManufacturCd);
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            return ManufacturList;
        }

        public List<Discount> GetRSDiscount(string ShopCD)
        {
            List<Discount> DiscountList = new List<Discount>();

            try
            {
                objContext = new ManageMasterDataServiceEntities();
                var Query = (from dis in objContext.MESC1TS_DISCOUNT
                             where dis.SHOP_CD == ShopCD
                             select new { dis.MANUFCTR, dis.DISCOUNT }).ToList();
                foreach (var obj in Query)
                {
                    Discount objDis = new Discount();
                    objDis.Manufctr = obj.MANUFCTR;
                    objDis.MarkDiscount = obj.DISCOUNT;
                    DiscountList.Add(objDis);
                };
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);

            }
            return DiscountList;
        }

        public string InsertUpdateShopDiscount(string ShopCode, string DiscountAll, string ManufactCode, string ManufactDis, string UserName)
        {

            string result = "";
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_SHOP> ShopData = new List<MESC1TS_SHOP>();
            List<MESC1TS_DISCOUNT> ShopDiscount = new List<MESC1TS_DISCOUNT>();
            try
            {
                ShopData = (from m in objContext.MESC1TS_SHOP
                            where m.SHOP_CD == ShopCode
                            select m).ToList();

                if (ShopData.Count() == 0)
                {
                    result = "Entry not found in table, update does not apply.";
                }
                else
                {
                    if (DiscountAll != null)
                    {
                        int DisValue = Convert.ToInt32(DiscountAll);
                        ShopData[0].PCT_MATERIAL_FACTOR = 0;
                        objContext.SaveChanges();
                        ShopData[0].PCT_MATERIAL_FACTOR = DisValue;

                    }
                    else
                    {
                        ShopData[0].PCT_MATERIAL_FACTOR = null;
                    }
                }
                int UpdateRecord = objContext.SaveChanges();
                if (UpdateRecord == 1)
                {
                    ShopDiscount = (from m in objContext.MESC1TS_DISCOUNT
                                    where m.SHOP_CD == ShopCode
                                    select m).ToList();


                    if (ShopDiscount.Count() > 0)
                    {

                        objContext.MESC1TS_DISCOUNT.Remove(ShopDiscount.First());
                        int DeleteRecord = objContext.SaveChanges();
                        if (DeleteRecord == 0)
                        {
                            result = "Database error encountered while attempting to delete DiscountTable record.";

                        }
                    }

                    string[] MafCD = ManufactCode.Split(',');
                    string[] MafDis = ManufactDis.Split(',');


                    string mancode = "";
                    for (int i = 0; i < MafCD.Count(); i++)
                    {

                        mancode = MafCD[i];
                        var ShopDiscount1 = (from m in objContext.MESC1TS_DISCOUNT
                                             where m.SHOP_CD == ShopCode && m.MANUFCTR == mancode // Shop_Cd and Manu fact code exists
                                             select m).ToList();

                        if (ShopDiscount1.Count > 0 && ShopDiscount1 != null)
                        {
                            if (MafCD[i] != "")
                            {

                                // objContext = new ManageMasterDataServiceEntities();
                                // MESC1TS_DISCOUNT dis = new MESC1TS_DISCOUNT();

                                if (MafDis[i] != "")
                                {
                                    ShopDiscount1[0].DISCOUNT = Convert.ToDouble(MafDis[i]);
                                }
                                else
                                {
                                    ShopDiscount1[0].DISCOUNT = 0;
                                }

                                ShopDiscount1[0].CHUSER = UserName;
                                ShopDiscount1[0].CHTS = DateTime.Now;

                                // objContext.MESC1TS_DISCOUNT.Add(dis);
                                objContext.SaveChanges();
                                result = "Success";
                                //-----------

                            }

                        }

                        else
                        {

                            if (MafCD[i] != "")
                            {
                                objContext = new ManageMasterDataServiceEntities();
                                MESC1TS_DISCOUNT dis = new MESC1TS_DISCOUNT();
                                dis.SHOP_CD = ShopCode;
                                dis.MANUFCTR = MafCD[i];
                                if (MafDis[i] != "")
                                {
                                    dis.DISCOUNT = Convert.ToDouble(MafDis[i]);
                                }
                                else
                                {
                                    dis.DISCOUNT = 0;
                                }

                                dis.CHUSER = UserName;
                                dis.CHTS = DateTime.Now;

                                objContext.MESC1TS_DISCOUNT.Add(dis);
                                objContext.SaveChanges();
                                result = "Success";

                            }

                        }

                    }

                }
                else
                {
                    result = "Database error while attempting to update the ShopTable.";
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                return "Database error, Please contact to system administrator.";
            }
            return result;

        } //Bug_Fix_Discount_Debadrita



        public string RetransmitWorkOrderStatus(string[] WOIDS, string Userid)
        {
            string Result = "";
            string strWOIS = "";
            string chUser = Userid.Substring(Userid.IndexOf('[') + 1, Userid.IndexOf(']') - Userid.IndexOf('[') - 1);

            try
            {
                objContext = new ManageMasterDataServiceEntities();


                foreach (var WOID in WOIDS)
                {
                    if (WOID != "")
                    {
                        List<MESC1TS_WO> WOData = new List<MESC1TS_WO>();

                        //string WorkOrderId = WOID.TrimStart('0');
                        int intWOID = Convert.ToInt32(WOID.Trim());



                        WOData = (from m in objContext.MESC1TS_WO
                                  where m.WO_ID == intWOID && (m.STATUS_CODE == 600 || m.STATUS_CODE == 900)
                                  select m).ToList();
                        if (WOData != null)
                        {
                            if (WOData.Count > 0)
                            {
                                WOData[0].STATUS_CODE = 400;
                                WOData[0].CHUSER = chUser;
                                WOData[0].CHTS = DateTime.Now;
                                int val = objContext.SaveChanges();

                                if (val > 0)
                                {
                                    MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
                                    WOAudit.WO_ID = WOData[0].WO_ID;
                                    WOAudit.AUDIT_TEXT = "Retransmitted by " + Userid;
                                    WOAudit.CHUSER = chUser;
                                    WOAudit.CHTS = DateTime.Now;
                                    objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                                    int Audit = objContext.SaveChanges();

                                }
                                else
                                {
                                    strWOIS = strWOIS + intWOID + ",";
                                    //Result = "Can't retransmit, either status didnot match or you have entered wrong format workorder : " + strWOIS + "";
                                }
                            }
                            else
                            {
                                strWOIS = strWOIS + intWOID + ",";
                                //Result = "Can't retransmit, either status didnot match or you have entered wrong format workorder : " + strWOIS + "";
                            }
                        }


                        else
                        {
                            strWOIS = strWOIS + intWOID + ",";
                            //Result = "" + RejWorkOrder + " are not available for update";
                        }
                    }
                    else
                    {
                        Result = "There is no valid record to update";
                        return Result;
                    }
                }
            }
            catch (Exception ex)
            {

                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
                throw;
            }
            if (strWOIS != "")
            {
                Result = "Work Order(s) " + strWOIS + " - not available for update";
            }
            else
            {
                Result = "Success";
            }
            return Result;

        }
        public List<Mode> GetRSAllManualModes(string ManualCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<Mode> ModeList = new List<Mode>();
            try
            {

                var ModeData = (from MM in objContext.MESC1TS_MANUAL_MODE
                                join M in objContext.MESC1TS_MANUAL
                                on MM.MANUAL_CD equals M.MANUAL_CD
                                join E in objContext.MESC1TS_MODE
                                on MM.MODE equals E.MODE
                                where MM.MANUAL_CD == ManualCode
                                orderby MM.MANUAL_CD, E.MODE_IND, MM.MODE
                                select new { MM.MANUAL_CD, MM.MODE, MM.ACTIVE_SW, M.MANUAL_DESC, E.MODE_DESC, E.MODE_IND }).Distinct().ToList();

                foreach (var obj in ModeData)
                {

                    Mode objMode = new Mode();
                    objMode.ModeCode = obj.MODE;
                    objMode.ModeDescription = obj.MODE_DESC;

                    ModeList.Add(objMode);
                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ModeList;
        }
        public List<Manual> GetRSAllManual()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<Manual> ManualList = new List<Manual>();
            try
            {

                var ManualData = (from MM in objContext.MESC1TS_MANUAL
                                  where MM.MANUAL_ACTIVE_SW == "Y"
                                  orderby MM.MANUAL_DESC
                                  select new { MM.MANUAL_CD, MM.MANUAL_DESC }).Distinct().ToList();

                foreach (var obj in ManualData)
                {
                    Manual objManual = new Manual();
                    objManual.ManualCode = obj.MANUAL_CD;
                    objManual.ManualDesc = obj.MANUAL_DESC;
                    ManualList.Add(objManual);
                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ManualList;
        }
        public Currency GetShopContCurrencyCode(string shopcode)
        {
            objContext = new ManageMasterDataServiceEntities();
            Currency objCurrency = new Currency();
            try
            {

                var CurrencuData = (from C in objContext.MESC1TS_CURRENCY
                                    join S in objContext.MESC1TS_SHOP on C.CUCDN equals S.CUCDN
                                    where S.SHOP_CD == shopcode
                                    select new { C.CUCDN, C.CURRNAMC }).Distinct().ToList();

                foreach (var obj in CurrencuData)
                {

                    objCurrency.Cucdn = obj.CUCDN;
                    objCurrency.CurrName = obj.CURRNAMC;

                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return objCurrency;
        }
        public string GetShopTypeByShopCode(string ShopCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            string SHOP_TYPE_CD = "";
            try
            {

                var ShopData = (from Shop in objContext.MESC1TS_SHOP
                                where Shop.SHOP_CD == ShopCode
                                select new { Shop.SHOP_TYPE_CD }).Distinct().ToList();

                foreach (var obj in ShopData)
                {

                    SHOP_TYPE_CD = obj.SHOP_TYPE_CD;

                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return SHOP_TYPE_CD;
        }

        #endregion Afroz


        public bool UpdateCustomer(Customer CustomerToBeUpdated)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_CUSTOMER> CustomerListFromDB = new List<MESC1TS_CUSTOMER>();
            List<MESC1TS_MANUAL> ManualListFromDB = new List<MESC1TS_MANUAL>();
            try
            {
                CustomerListFromDB = (from cust in objContext.MESC1TS_CUSTOMER
                                      from M in objContext.MESC1TS_MANUAL
                                      where cust.CUSTOMER_CD == CustomerToBeUpdated.CustomerCode &&
                                      cust.MANUAL_CD == M.MANUAL_CD
                                      select cust).ToList();

                ManualListFromDB = (from C in objContext.MESC1TS_CUSTOMER
                                    from M in objContext.MESC1TS_MANUAL
                                    where M.MANUAL_CD == CustomerToBeUpdated.CustomerCode &&
                                    C.MANUAL_CD == M.MANUAL_CD
                                    orderby C.CUSTOMER_CD
                                    select M).ToList();

                CustomerListFromDB[0].CUSTOMER_DESC = CustomerToBeUpdated.CustomerDesc;
                CustomerListFromDB[0].MANUAL_CD = CustomerToBeUpdated.ManualCode;
                CustomerListFromDB[0].CUSTOMER_ACTIVE_SW = CustomerToBeUpdated.CustomerActiveSw;
                CustomerListFromDB[0].CHUSER = CustomerToBeUpdated.ChangeUser;
                CustomerListFromDB[0].CHTS = DateTime.Now;
                try
                {
                    objContext.SaveChanges();
                    success = true;
                }
                catch (Exception ex)
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

        public bool CreateCustomer(Customer CustomerFromClient, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_CUSTOMER CustomerToBeInserted = new MESC1TS_CUSTOMER();
            List<MESC1TS_CUSTOMER> CustList = new List<MESC1TS_CUSTOMER>();
            try
            {
                CustList = (from cust in objContext.MESC1TS_CUSTOMER
                            where cust.CUSTOMER_CD == CustomerFromClient.CustomerCode
                            select cust).ToList();

                if (CustList.Count > 0)
                {

                    Msg = "Customer Code " + CustomerFromClient.CustomerCode + " already exists - Not Added";
                    return success;

                }
                else
                {

                    CustomerToBeInserted.CUSTOMER_CD = CustomerFromClient.CustomerCode;
                    CustomerToBeInserted.CUSTOMER_DESC = CustomerFromClient.CustomerDesc;
                    CustomerToBeInserted.CUSTOMER_ACTIVE_SW = CustomerFromClient.CustomerActiveSw;
                    CustomerToBeInserted.MANUAL_CD = CustomerFromClient.ManualCode;
                    CustomerToBeInserted.CHUSER = CustomerFromClient.ChangeUser;
                    CustomerToBeInserted.CHTS = DateTime.Now;
                    objContext.MESC1TS_CUSTOMER.Add(CustomerToBeInserted);

                    try
                    {
                        //objContext.AcceptAllChanges();
                        objContext.SaveChanges();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        success = false;
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


        public List<Location> GetLocationList()
        {
            objContext = new ManageMasterDataServiceEntities();
            //List<MESC1TS_LOCATION> LocationListFromDB = new List<MESC1TS_LOCATION>();
            List<Location> LocationList = new List<Location>();
            try
            {
                LocationList = objContext.Database.SqlQuery<Location>("select LOC_CD as 'LocCode', LOC_CD +'-' + LOC_Desc as 'LocDesc'  from  dbo.MESC1TS_LOCATION order by LOC_CD").ToList();

                //LocationListFromDB = (from L in objContext.MESC1TS_LOCATION
                //                      orderby L.LOC_CD
                //                      select L).ToList();

                //for (int count = 0; count < LocationListFromDB.Count; count++)
                //{

                //    Location loc = new Location();
                //    loc.LocCode = LocationListFromDB[count].LOC_CD;
                //    loc.LocDesc = LocationListFromDB[count].LOC_DESC;
                //    loc.CountryCode = LocationListFromDB[count].COUNTRY_CD;
                //    loc.RegionCode = LocationListFromDB[count].REGION_CD;
                //    loc.ContactEqsalSW = LocationListFromDB[count].CONTACT_EQSAL_SW;
                //    loc.ChangeUserLoc = LocationListFromDB[count].CHUSER;
                //    loc.ChangeTimeLoc = LocationListFromDB[count].CHTS;
                //    LocationList.Add(loc);
                //}
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return LocationList;
        }

        public bool UpdateLocation(Location LocationToBeUpdated)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_LOCATION> LocationListFromDB = new List<MESC1TS_LOCATION>();
            List<Location> LocationList = new List<Location>();

            try
            {
                LocationListFromDB = (from loc in objContext.MESC1TS_LOCATION
                                      where loc.LOC_CD.Trim() == LocationToBeUpdated.LocCode
                                      select loc).ToList();


                LocationListFromDB[0].CONTACT_EQSAL_SW = LocationToBeUpdated.ContactEqsalSW;
                LocationListFromDB[0].REGION_CD = LocationToBeUpdated.RegionCode;
                LocationListFromDB[0].CHUSER = LocationToBeUpdated.ChangeUserLoc;
                LocationListFromDB[0].CHTS = DateTime.Now;

                try
                {
                    objContext.SaveChanges();
                    success = true;
                }
                catch (Exception ex)
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

        public List<Region> GetRegionList()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_REGION> RegionListFromDB = new List<MESC1TS_REGION>();
            List<Region> RegionList = new List<Region>();
            try
            {
                RegionListFromDB = (from reg in objContext.MESC1TS_REGION
                                    orderby reg.REGION_CD
                                    select reg).Distinct().ToList();

                for (int count = 0; count < RegionListFromDB.Count; count++)
                {
                    Region region = new Region();
                    region.RegionCd = RegionListFromDB[count].REGION_CD;
                    region.RegionDesc = RegionListFromDB[count].REGION_DESC;
                    RegionList.Add(region);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return RegionList;

        }


        public List<Country> GetCountryList()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_COUNTRY> CountryListFromDB = new List<MESC1TS_COUNTRY>();
            List<Country> CountryList = new List<Country>();

            try
            {
                CountryListFromDB = (from C in objContext.MESC1TS_COUNTRY
                                     orderby C.COUNTRY_CD
                                     select C).ToList();


                for (int count = 0; count < CountryListFromDB.Count; count++)
                {

                    Country country = new Country();

                    country.CountryCode = CountryListFromDB[count].COUNTRY_CD;
                    country.CountryDescription = CountryListFromDB[count].COUNTRY_DESC;
                    country.AreaCode = CountryListFromDB[count].AREA_CD;
                    country.RepairLimitAdjFactor = CountryListFromDB[count].REPAIR_LIMIT_ADJ_FACTOR;
                    country.ChangeUserCon = CountryListFromDB[count].CHUSER;
                    country.ChangeTimeCon = CountryListFromDB[count].CHTS;
                    CountryList.Add(country);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return CountryList;
        }

        public List<Country> GetRegionByCountryCode(string CountryCode)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<Country> CountryCodeList = new List<Country>();
            try
            {

                var CountryAreaCodeListFromDB = (from A in objContext.MESC1TS_AREA
                                                 join C in objContext.MESC1TS_COUNTRY
                                                    on A.AREA_CD equals C.AREA_CD
                                                 where C.COUNTRY_CD == CountryCode
                                                 orderby C.COUNTRY_CD
                                                 select new
                                                 {
                                                     C.COUNTRY_CD,
                                                     C.COUNTRY_DESC,
                                                     A.AREA_CD,
                                                     A.AREA_DESC,
                                                     C.REPAIR_LIMIT_ADJ_FACTOR,
                                                     C.CHUSER,
                                                     C.CHTS
                                                 }).ToList();


                for (int count = 0; count < CountryAreaCodeListFromDB.Count; count++)
                {

                    Country country = new Country();
                    country.CountryCode = CountryAreaCodeListFromDB[count].COUNTRY_CD;
                    country.CountryDescription = CountryAreaCodeListFromDB[count].COUNTRY_DESC;
                    country.AreaCode = CountryAreaCodeListFromDB[count].AREA_CD + "-" + CountryAreaCodeListFromDB[count].AREA_DESC;
                    country.RepairLimitAdjFactor = CountryAreaCodeListFromDB[count].REPAIR_LIMIT_ADJ_FACTOR;
                    country.ChangeUserCon = CountryAreaCodeListFromDB[count].CHUSER;
                    country.ChangeTimeCon = CountryAreaCodeListFromDB[count].CHTS;
                    CountryCodeList.Add(country);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return CountryCodeList;
        }


        public bool UpdateCountry(Country CountryToBeUpdated)
        {
            bool success = false;
            bool result = false;
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_COUNTRY> CountryListFromDB = new List<MESC1TS_COUNTRY>();
            List<Country> CountryList = new List<Country>();
            MESC1TS_REFAUDIT AuditDBObject = new MESC1TS_REFAUDIT();
            try
            {
                CountryListFromDB = (from Con in objContext.MESC1TS_COUNTRY
                                     where Con.COUNTRY_CD == CountryToBeUpdated.CountryCode
                                     select Con).ToList();
                Nullable<double> OldValue = CountryListFromDB[0].REPAIR_LIMIT_ADJ_FACTOR;
                CountryListFromDB[0].REPAIR_LIMIT_ADJ_FACTOR = CountryToBeUpdated.RepairLimitAdjFactor;
                CountryListFromDB[0].CHUSER = CountryToBeUpdated.ChangeUserCon;
                CountryListFromDB[0].CHTS = DateTime.Now;

                try
                {
                    objContext.SaveChanges();
                    success = true;

                }
                catch (Exception e)
                {
                    success = false;
                }
                if (success)
                {
                    AuditDBObject.TAB_NAME = "MESC1TS_COUNTRY";
                    AuditDBObject.UNIQUE_ID = CountryToBeUpdated.CountryCode;
                    AuditDBObject.COL_NAME = "REPAIR_LIMIT_ADJ_FACTOR";
                    AuditDBObject.OLD_VALUE = OldValue.ToString();
                    AuditDBObject.NEW_VALUE = (CountryToBeUpdated.RepairLimitAdjFactor).ToString();
                    AuditDBObject.CHUSER = CountryToBeUpdated.ChangeUserCon;
                    AuditDBObject.CHTS = DateTime.Now;
                    objContext.MESC1TS_REFAUDIT.Add(AuditDBObject);
                    try
                    {
                        objContext.SaveChanges();
                        result = true;
                    }
                    catch (Exception e)
                    {
                        result = false;
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


        public List<CustShopMode> GetCustShopModeList(string CustomerCode, string ShopCode, string Mode, int SortType)
        {
            try
            {

                objContext = new ManageMasterDataServiceEntities();

                List<MESC1TS_CUST_SHOP_MODE> CSMList = new List<MESC1TS_CUST_SHOP_MODE>();

                List<CustShopMode> CustShopModeList = new List<CustShopMode>();

                var Query = from CSM in objContext.MESC1TS_CUST_SHOP_MODE
                            from C in objContext.MESC1TS_CUSTOMER
                            .Where(C => C.CUSTOMER_CD == CSM.CUSTOMER_CD)
                            .DefaultIfEmpty()
                            from SC in objContext.MESC1TS_SHOP
                            .Where(SC => SC.SHOP_CD == CSM.SHOP_CD)
                            .DefaultIfEmpty()
                            from M in objContext.MESC1TS_MODE
                            .Where(M => M.MODE == CSM.MODE)
                            .DefaultIfEmpty()


                            select new { CSM, C, SC, M };

                //now we can apply filters on ANY of the joined tables
                if (!string.IsNullOrEmpty(ShopCode))
                    Query = Query.Where(q => q.CSM.SHOP_CD == ShopCode);

                if (!string.IsNullOrEmpty(CustomerCode))
                    Query = Query.Where(q => q.CSM.CUSTOMER_CD == CustomerCode);

                if (!string.IsNullOrEmpty(Mode))
                    Query = Query.Where(q => q.CSM.MODE == Mode);


                var CustShopModeFromDB = (from q in Query
                                          select new
                                          {
                                              q.CSM.CUSTOMER_CD,
                                              q.CSM.SHOP_CD,
                                              q.CSM.MODE,
                                              q.CSM.PAYAGENT_CD,
                                              q.CSM.CORP_PAYAGENT_CD,
                                              q.CSM.RRIS_FORMAT,
                                              q.CSM.PROFIT_CENTER,
                                              q.CSM.SUB_PROFIT_CENTER,
                                              q.CSM.ACCOUNT_CD

                                          }).ToList();


                foreach (var obj in CustShopModeFromDB)
                {
                    CustShopMode csm = new CustShopMode();
                    csm.CustomerCode = obj.CUSTOMER_CD.ToString();
                    csm.ShopCode = obj.SHOP_CD.ToString();
                    csm.ModeCode = obj.MODE;
                    csm.PayAgentCode = obj.PAYAGENT_CD;
                    csm.CorpPayAgentCode = obj.CORP_PAYAGENT_CD;
                    csm.RRISFormat = obj.RRIS_FORMAT;
                    csm.SubProfitCenter = obj.SUB_PROFIT_CENTER;
                    csm.ProfitCenter = obj.PROFIT_CENTER;
                    csm.AccountCode = obj.ACCOUNT_CD;
                    CustShopModeList.Add(csm);



                };

                if (SortType == 1)
                {

                    CustShopModeList.OrderBy(li => li.CustomerCode);
                }
                else if (SortType == 2)
                {
                    CustShopModeList.OrderBy(li => li.ShopCode);
                }
                else if (SortType == 3)
                {
                    CustShopModeList.OrderBy(li => li.ModeCode);
                }
                return CustShopModeList;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public List<Customer> GetCustomerCode()
        {
            List<Customer> Customerlist = new List<Customer>();
            List<MESC1TS_CUSTOMER> CustomerFromDB = new List<MESC1TS_CUSTOMER>();

            try
            {
                CustomerFromDB = (from C in objContext.MESC1TS_CUSTOMER
                                  orderby C.CUSTOMER_CD
                                  select C).Distinct().ToList();

                foreach (var item in CustomerFromDB)
                {
                    Customer customer = new Customer();
                    customer.CustomerCode = item.CUSTOMER_CD;
                    customer.CustomerDesc = item.CUSTOMER_DESC;
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

        public List<Shop> GetShopCode()
        {
            List<Shop> ShopList = new List<Shop>();
            List<MESC1TS_SHOP> ShopListFromDB = new List<MESC1TS_SHOP>();

            ShopListFromDB = (from CS in objContext.MESC1TS_CUST_SHOP_MODE
                              from S in objContext.MESC1TS_SHOP
                              where S.SHOP_CD == CS.SHOP_CD
                              orderby S.SHOP_CD
                              select S).Distinct().ToList();

            foreach (var item in ShopListFromDB)
            {
                Shop shop = new Shop();
                shop.ShopCode = item.SHOP_CD;
                shop.ShopDescription = item.SHOP_DESC;
                ShopList.Add(shop);
            }
            return ShopList;
        }

        public List<Mode> GetModeList()
        {
            List<MESC1TS_MODE> ModeListFromDB = new List<MESC1TS_MODE>();
            List<Mode> ModeList = new List<Mode>();
            try
            {
                ModeListFromDB = (from m in objContext.MESC1TS_MODE

                                  orderby m.MODE
                                  select m).Distinct().ToList();

                for (int count = 0; count < ModeListFromDB.Count; count++)
                {
                    Mode Mode = new Mode();
                    Mode.ModeCode = ModeListFromDB[count].MODE;
                    Mode.ModeDescription = ModeListFromDB[count].MODE_DESC;
                    ModeList.Add(Mode);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ModeList;
        }

        public List<Mode> GetModeForSuspend(string Manual)
        {
            List<MESC1TS_MODE> ModeListFromDB = new List<MESC1TS_MODE>();
            List<Mode> ModeList = new List<Mode>();
            try
            {
                ModeListFromDB = (from m in objContext.MESC1TS_MODE
                                  join mm in objContext.MESC1TS_MANUAL_MODE
                                  on m.MODE equals mm.MODE
                                  where mm.MANUAL_CD == Manual
                                  orderby m.MODE
                                  select m).Distinct().ToList();

                for (int count = 0; count < ModeListFromDB.Count; count++)
                {
                    Mode Mode = new Mode();
                    Mode.ModeCode = ModeListFromDB[count].MODE;
                    Mode.ModeDescription = ModeListFromDB[count].MODE_DESC;
                    ModeList.Add(Mode);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ModeList;
        }

        public List<Mode> GetSuspedModeList(string ManualCode)
        {
            List<MESC1TS_MODE> ModeListFromDB = new List<MESC1TS_MODE>();
            List<Mode> ModeList = new List<Mode>();
            try
            {
                ModeListFromDB = (from m in objContext.MESC1TS_MODE
                                  join mm in objContext.MESC1TS_MANUAL_MODE
                                  on m.MODE equals mm.MODE
                                  where mm.MANUAL_CD == ManualCode
                                  orderby m.MODE
                                  select m).Distinct().ToList();

                for (int count = 0; count < ModeListFromDB.Count; count++)
                {
                    Mode Mode = new Mode();
                    Mode.ModeCode = ModeListFromDB[count].MODE;
                    Mode.ModeDescription = ModeListFromDB[count].MODE_DESC;
                    ModeList.Add(Mode);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ModeList;
        }

        public List<Mode> GetModeByCustomerCode(string CustomerCode)
        {

            List<Mode> ModeCodeList = new List<Mode>();

            try
            {
                var ModeCodeListFromDB = from cs in objContext.MESC1TS_TRANSMIT
                                         from m in objContext.MESC1TS_MODE
                                         where cs.CUSTOMER_CD == CustomerCode
                                         && cs.MODE == m.MODE
                                         orderby m.MODE
                                         select new { m.MODE, m.MODE_DESC };


                foreach (var item in ModeCodeListFromDB)
                {
                    ModeCodeList.Add(new Mode()
                    {
                        ModeCode = item.MODE,
                        ModeDescription = item.MODE_DESC
                    });
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ModeCodeList;
        }

        public List<Transmit> GetTransmitDetails()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_TRANSMIT> TransmitDetailsfromDB = new List<MESC1TS_TRANSMIT>();
            List<Transmit> TransmitList = new List<Transmit>();

            try
            {
                TransmitDetailsfromDB = (from T in objContext.MESC1TS_TRANSMIT
                                         select T).ToList();

                for (int count = 0; count < TransmitDetailsfromDB.Count; count++)
                {

                    Transmit transmit = new Transmit();
                    transmit.CustomerCode = TransmitDetailsfromDB[count].CUSTOMER_CD;
                    transmit.ModeCode = TransmitDetailsfromDB[count].MODE;
                    transmit.RRISXMITSwitch = TransmitDetailsfromDB[count].RRIS_XMIT_SW;
                    transmit.RKRPXMITSwitch = TransmitDetailsfromDB[count].RKRP_XMIT_SW;
                    transmit.AccountCode = TransmitDetailsfromDB[count].ACCOUNT_CD;
                    transmit.ChangeUserTransmit = TransmitDetailsfromDB[count].CHUSER;
                    transmit.ChangeTimeTransmit = TransmitDetailsfromDB[count].CHTS;
                    TransmitList.Add(transmit);


                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return TransmitList;
        }
        public List<Transmit> GetTransmitDetailsFromCustomerMode(string CustomrCode, string ModeCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            //List<MESC1TS_TRANSMIT> TransmitDetailsfromDB = new List<MESC1TS_TRANSMIT>();
            List<Transmit> TransmitList = new List<Transmit>();
            try
            {
                var TransmitDetailsfromDB = (from T in objContext.MESC1TS_TRANSMIT
                                             where (T.CUSTOMER_CD == CustomrCode) && (T.MODE == ModeCode)
                                             select T).ToList();

                for (int count = 0; count < TransmitDetailsfromDB.Count; count++)
                {

                    Transmit transmit = new Transmit();
                    transmit.CustomerCode = TransmitDetailsfromDB[count].CUSTOMER_CD;
                    transmit.ModeCode = TransmitDetailsfromDB[count].MODE;
                    transmit.RRISXMITSwitch = TransmitDetailsfromDB[count].RRIS_XMIT_SW;
                    transmit.RKRPXMITSwitch = TransmitDetailsfromDB[count].RKRP_XMIT_SW;
                    transmit.AccountCode = TransmitDetailsfromDB[count].ACCOUNT_CD;
                    transmit.ChangeUserTransmit = TransmitDetailsfromDB[count].CHUSER;
                    transmit.ChangeTimeTransmit = TransmitDetailsfromDB[count].CHTS;
                    TransmitList.Add(transmit);


                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return TransmitList;
        }


        public bool UpdateTransmit(Transmit TransmitToBeUpdated)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_TRANSMIT> TransmitListFromDB = new List<MESC1TS_TRANSMIT>();
            List<Transmit> TransmitList = new List<Transmit>();

            try
            {
                TransmitListFromDB = (from tns in objContext.MESC1TS_TRANSMIT
                                      where tns.CUSTOMER_CD.Trim() == TransmitToBeUpdated.CustomerCode &&
                                      tns.MODE == TransmitToBeUpdated.ModeCode
                                      select tns).ToList();

                TransmitListFromDB[0].RRIS_XMIT_SW = TransmitToBeUpdated.RRISXMITSwitch;
                TransmitListFromDB[0].RKRP_XMIT_SW = null;
                TransmitListFromDB[0].ACCOUNT_CD = TransmitToBeUpdated.AccountCode;
                TransmitListFromDB[0].CHUSER = TransmitToBeUpdated.ChangeUserTransmit;
                TransmitListFromDB[0].CHTS = DateTime.Now;

                try
                {
                    objContext.SaveChanges();
                    success = true;
                }
                catch (Exception ex)
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


        public bool CreateTransmit(Transmit TransmitFromClient, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_TRANSMIT> Transmitlist = new List<MESC1TS_TRANSMIT>();
            try
            {
                Transmitlist = (from t in objContext.MESC1TS_TRANSMIT
                                where t.CUSTOMER_CD == TransmitFromClient.CustomerCode &&
                                t.MODE == TransmitFromClient.ModeCode
                                select t).ToList();
                if (Transmitlist.Count > 0)
                {
                    Msg = "Transmit " + TransmitFromClient.CustomerCode + "/" + TransmitFromClient.ModeCode + " already exists - Not Added";
                    return success;
                }
                else
                {
                    MESC1TS_TRANSMIT TransmitToBeInserted = new MESC1TS_TRANSMIT();
                    TransmitToBeInserted.CUSTOMER_CD = TransmitFromClient.CustomerCode;
                    TransmitToBeInserted.MODE = TransmitFromClient.ModeCode;
                    TransmitToBeInserted.RRIS_XMIT_SW = TransmitFromClient.RRISXMITSwitch;
                    TransmitToBeInserted.ACCOUNT_CD = TransmitFromClient.AccountCode;
                    TransmitToBeInserted.CHUSER = TransmitFromClient.ChangeUserTransmit;
                    TransmitToBeInserted.CHTS = DateTime.Now;

                    objContext.MESC1TS_TRANSMIT.Add(TransmitToBeInserted);

                    try
                    {
                        //objContext.AcceptAllChanges();
                        objContext.SaveChanges();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        success = false;
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

        public List<Shop> GetShop(int UserID)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<Shop> ShopList = new List<Shop>();
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
                        string ShopCode = item.COLUMN_VALUE;
                        ShopCodeList.Add(ShopCode);
                    }
                }

                var ShopListFromDBOnAuth = (from shop in objContext.MESC1VS_SHOP_LOCATION
                                            where shop.SHOP_ACTIVE_SW == "Y" &&
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
                    var ShopSuspendListFromDB = (from S in objContext.MESC1TS_SUSPEND
                                                 select S).Distinct().OrderBy(cd => cd.SHOP_CD).ToList();


                    var ShopListFinal = ShopListFromDBOnAuth.FindAll(a => ShopSuspendListFromDB.Any(ab => ab.SHOP_CD == a.SHOP_CD));

                    foreach (var item in ShopListFinal)
                    {
                        Shop shop = new Shop();
                        shop.ShopCode = item.SHOP_CD;
                        shop.ShopDescription = item.SHOP_DESC;
                        shop.CUCDN = item.CUCDN;
                        ShopList.Add(shop);
                    }

                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ShopList;
        }

        public bool UpdateEquipmentType(EqType EquipmentTypeToBeUpdated, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_EQTYPE> EqTypeDBObject = new List<MESC1TS_EQTYPE>();
            try
            {
                EqTypeDBObject = (from eqty in objContext.MESC1TS_EQTYPE
                                  where eqty.EQTYPE == EquipmentTypeToBeUpdated.EqpType &&
                                  eqty.EQTYPE_DESC == EquipmentTypeToBeUpdated.EqTypeDesc
                                  select eqty).ToList();
                if (EqTypeDBObject.Count > 0)
                {
                    Msg = "Equipment " + EquipmentTypeToBeUpdated.EqpType + " description already exists - Not Updated";
                    return success;
                }
                else
                {
                    EqTypeDBObject = (from eqty in objContext.MESC1TS_EQTYPE
                                      where eqty.EQTYPE == EquipmentTypeToBeUpdated.EqpType
                                      select eqty).ToList();
                    EqTypeDBObject[0].EQTYPE = EquipmentTypeToBeUpdated.EqpType;
                    EqTypeDBObject[0].EQTYPE_DESC = EquipmentTypeToBeUpdated.EqTypeDesc;
                    EqTypeDBObject[0].CHUSER = EquipmentTypeToBeUpdated.ChangeUser;
                    EqTypeDBObject[0].CHTS = DateTime.Now;
                    try
                    {
                        objContext.SaveChanges();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        success = false;
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

        public bool CreateEquipmentTypeEntry(EqType EquipmentTypeFromClient, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_EQTYPE> EqTypeList = new List<MESC1TS_EQTYPE>();
            MESC1TS_EQTYPE EqTypeToBeInserted = new MESC1TS_EQTYPE();
            try
            {
                EqTypeList = (from eq in objContext.MESC1TS_EQTYPE
                              where eq.EQTYPE == EquipmentTypeFromClient.EqpType
                              select eq).ToList();
                if (EqTypeList.Count > 0)
                {
                    Msg = "Equipment Type " + EquipmentTypeFromClient.EqpType + " already exists - Not Added";
                    return success;
                }
                else
                {
                    EqTypeList = (from eq in objContext.MESC1TS_EQTYPE
                                  where eq.EQTYPE == EquipmentTypeFromClient.EqpType
                                  select eq).ToList();

                    EqTypeToBeInserted.EQTYPE = EquipmentTypeFromClient.EqpType;
                    EqTypeToBeInserted.EQTYPE_DESC = EquipmentTypeFromClient.EqTypeDesc;
                    EqTypeToBeInserted.CHUSER = EquipmentTypeFromClient.ChangeUser;
                    EqTypeToBeInserted.CHTS = DateTime.Now;

                    objContext.MESC1TS_EQTYPE.Add(EqTypeToBeInserted);
                    try
                    {
                        objContext.SaveChanges();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        success = false;
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

        public List<RepairLocationCode> GetRepairLocationCodes()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_REPAIR_LOC> RepairLocationCodeList = new List<MESC1TS_REPAIR_LOC>();
            RepairLocationCodeList = (from LocationCode in objContext.MESC1TS_REPAIR_LOC
                                      select LocationCode).ToList();

            return PrepareDataContract(RepairLocationCodeList);

        }

        public List<RepairLocationCode> GetRepairLocationCode(string code)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_REPAIR_LOC> RepairLocationCodeList = new List<MESC1TS_REPAIR_LOC>();
            RepairLocationCodeList = (from LocationCode in objContext.MESC1TS_REPAIR_LOC
                                      where LocationCode.cedex_code == code
                                      select LocationCode).ToList();

            return PrepareDataContract(RepairLocationCodeList);

        }

        //public List<Damage> GetDamageCode()
        //{
        //    objContext = new ManageMasterDataServiceEntities();
        //    List<MESC1TS_DAMAGE> DamageCodeList = new List<MESC1TS_DAMAGE>();
        //    DamageCodeList = (from damageCode in objContext.MESC1TS_DAMAGE
        //                      select damageCode).ToList();

        //    return PrepareDataContract(DamageCodeList);

        //}

        public List<ModeEntry> GetModeEntryList()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<ModeEntry> modeEntryList = new List<ModeEntry>();
            List<MESC1TS_EQMODE> modeEntryListFromDB = new List<MESC1TS_EQMODE>();
            try
            {
                modeEntryListFromDB = (from mdtype in objContext.MESC1TS_EQMODE
                                       select mdtype).ToList();


                for (int count = 0; count < modeEntryListFromDB.Count; count++)
                {
                    ModeEntry md = new ModeEntry();
                    md.EqType = modeEntryListFromDB[count].COTYPE;
                    md.SubType = modeEntryListFromDB[count].EQSTYPE;
                    md.Size = modeEntryListFromDB[count].EQSIZE;
                    md.Aluminum = modeEntryListFromDB[count].ALUMINIUM_SW;
                    modeEntryList.Add(md);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return modeEntryList;

        }

        public bool AddModeEntry(ModeEntry ModeEntryToBeAdded)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_EQMODE modeEntryToBeInserted = new MESC1TS_EQMODE();
            // EqTypeToBeInserted.EQTYPE = EquipmentTypeFromClient.EqpmentType;
            // EqTypeToBeInserted.EQTYPE_DESC = EquipmentTypeFromClient.EqpmentDesc;

            objContext.MESC1TS_EQMODE.Add(modeEntryToBeInserted);
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }


        public List<Vendor> GetVendorList()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_VENDOR> VendorListFromDB = new List<MESC1TS_VENDOR>();
            List<Vendor> VendorList = new List<Vendor>();
            try
            {
                VendorListFromDB = (from Ven in objContext.MESC1TS_VENDOR
                                    orderby Ven.VENDOR_CD
                                    select Ven).ToList();

                for (int count = 0; count < VendorListFromDB.Count; count++)
                {

                    Vendor ven = new Vendor();
                    ven.VendorCode = VendorListFromDB[count].VENDOR_CD;
                    ven.VendorDesc = VendorListFromDB[count].VENDOR_DESC;
                    ven.VenCountryCode = VendorListFromDB[count].COUNTRY_CD;
                    ven.VendorActiveSw = VendorListFromDB[count].VENDOR_ACTIVE_SW;
                    ven.ChangeUserVendor = VendorListFromDB[count].CHUSER;
                    ven.ChangeTimeVendor = VendorListFromDB[count].CHTS;
                    VendorList.Add(ven);

                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return VendorList;
        }


        public bool UpdateVendor(Vendor VendorToBeUpdated)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_VENDOR> VenDBObject = new List<MESC1TS_VENDOR>();
            try
            {
                VenDBObject = (from ven in objContext.MESC1TS_VENDOR
                               where ven.VENDOR_CD == VendorToBeUpdated.VendorCode
                               select ven).ToList();

                VenDBObject[0].VENDOR_DESC = VendorToBeUpdated.VendorDesc;
                VenDBObject[0].VENDOR_ACTIVE_SW = VendorToBeUpdated.VendorActiveSw;
                VenDBObject[0].COUNTRY_CD = VendorToBeUpdated.VenCountryCode;
                VenDBObject[0].CHUSER = VendorToBeUpdated.ChangeUserVendor;
                VenDBObject[0].CHTS = DateTime.Now;
                try
                {
                    objContext.SaveChanges();
                    success = true;
                }
                catch (Exception e)
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

        public bool CreateVendor(Vendor VendorFromClient, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_VENDOR> VendorList = new List<MESC1TS_VENDOR>();
            try
            {
                VendorList = (from ven in objContext.MESC1TS_VENDOR
                              where ven.VENDOR_CD == VendorFromClient.VendorCode
                              select ven).ToList();
                if (VendorList.Count > 0)
                {
                    Msg = "Vendor " + VendorFromClient.VendorCode + " already exists - Not Added";
                    return success;
                }
                else
                {
                    MESC1TS_VENDOR vendorToBeInserted = new MESC1TS_VENDOR();
                    vendorToBeInserted.VENDOR_CD = VendorFromClient.VendorCode;
                    vendorToBeInserted.VENDOR_DESC = VendorFromClient.VendorDesc;
                    vendorToBeInserted.COUNTRY_CD = VendorFromClient.VenCountryCode;
                    vendorToBeInserted.VENDOR_ACTIVE_SW = VendorFromClient.VendorActiveSw;
                    vendorToBeInserted.DISCOUNT_DAYS = 0;
                    vendorToBeInserted.DISCOUNT_PCT = 0.0;
                    vendorToBeInserted.CHUSER = VendorFromClient.ChangeUserVendor;
                    vendorToBeInserted.CHTS = DateTime.Now;


                    objContext.MESC1TS_VENDOR.Add(vendorToBeInserted);
                    try
                    {
                        objContext.SaveChanges();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        success = false;
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


        public List<SuspendCat> GetSuspendCatList()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_SUSPEND_CAT> SuspendCatListFromDB = new List<MESC1TS_SUSPEND_CAT>();
            List<SuspendCat> SuspendCatList = new List<SuspendCat>();
            try
            {
                SuspendCatListFromDB = (from sus in objContext.MESC1TS_SUSPEND_CAT
                                        orderby sus.SUSPCAT_ID
                                        select sus).ToList();



                for (int count = 0; count < SuspendCatListFromDB.Count; count++)
                {


                    SuspendCat suspend = new SuspendCat();
                    suspend.SuspcatID = SuspendCatListFromDB[count].SUSPCAT_ID;
                    suspend.SuspcatDesc = SuspendCatListFromDB[count].SUSPCAT_DESC;
                    suspend.ChangeUserSus = SuspendCatListFromDB[count].CHUSER;
                    suspend.ChangeTimeSus = SuspendCatListFromDB[count].CHTS;

                    SuspendCatList.Add(suspend);


                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return SuspendCatList;
        }

        public bool UpdateSuspendCat(SuspendCat SuspendCatToBeUpdated, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_SUSPEND_CAT> SusDBObject = new List<MESC1TS_SUSPEND_CAT>();
            List<MESC1TS_VENDOR> VenDBObject = new List<MESC1TS_VENDOR>();
            try
            {
                SusDBObject = (from sus in objContext.MESC1TS_SUSPEND_CAT
                               where sus.SUSPCAT_ID == SuspendCatToBeUpdated.SuspcatID &&
                               sus.SUSPCAT_DESC == SuspendCatToBeUpdated.SuspcatDesc
                               orderby sus.SUSPCAT_ID
                               select sus).ToList();
                if (SusDBObject.Count > 0)
                {
                    Msg = "Suspend Category ID " + SuspendCatToBeUpdated.SuspcatID + " description already exists - Not Updated";
                    return success;
                }
                else
                {
                    SusDBObject = (from sus in objContext.MESC1TS_SUSPEND_CAT
                                   where sus.SUSPCAT_ID == SuspendCatToBeUpdated.SuspcatID
                                   select sus).ToList();
                    SusDBObject[0].SUSPCAT_ID = SuspendCatToBeUpdated.SuspcatID;
                    SusDBObject[0].SUSPCAT_DESC = SuspendCatToBeUpdated.SuspcatDesc;
                    SusDBObject[0].CHUSER = SuspendCatToBeUpdated.ChangeUserSus;
                    SusDBObject[0].CHTS = DateTime.Now;
                }

                try
                {
                    objContext.SaveChanges();
                    success = true;
                }
                catch (Exception e)
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

        public bool CreateSuspendCat(SuspendCat SuspendCatFromClient, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            try
            {
                var SuspendDBObject = (from sus in objContext.MESC1TS_SUSPEND_CAT
                                       where sus.SUSPCAT_DESC == SuspendCatFromClient.SuspcatDesc
                                       select sus).ToList();
                if (SuspendDBObject.Count > 0)
                {
                    Msg = "Suspend Category already exist";
                    return success;
                }

                MESC1TS_SUSPEND_CAT SuspendCatListToBeInserted = new MESC1TS_SUSPEND_CAT();
                SuspendCatListToBeInserted.SUSPCAT_DESC = SuspendCatFromClient.SuspcatDesc;
                SuspendCatListToBeInserted.CHUSER = SuspendCatFromClient.ChangeUserSus;
                SuspendCatListToBeInserted.CHTS = DateTime.Now;



                objContext.MESC1TS_SUSPEND_CAT.Add(SuspendCatListToBeInserted);
                try
                {

                    objContext.MESC1TS_SUSPEND_CAT.Add(SuspendCatListToBeInserted);
                    objContext.SaveChanges();
                    success = true;
                }
                catch (Exception ex)
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

        public bool DeleteSuspendCat(string SuspendCatCode)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            Int32 SuspendId = Convert.ToInt32(SuspendCatCode);
            List<MESC1TS_SUSPEND_CAT> SuspendDBObject = new List<MESC1TS_SUSPEND_CAT>();
            try
            {
                SuspendDBObject = (from sus in objContext.MESC1TS_SUSPEND_CAT
                                   where sus.SUSPCAT_ID == SuspendId
                                   select sus).ToList();
                if (SuspendDBObject.Count > 0)
                {


                    objContext.MESC1TS_SUSPEND_CAT.Remove(SuspendDBObject.First());

                    objContext.SaveChanges();
                    success = true;

                }
                else
                {
                    return success;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }


        public bool DeleteSuspend(Suspend SuspendCode)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_SUSPEND> SuspendDBObject = new List<MESC1TS_SUSPEND>();
            try
            {
                SuspendDBObject = (from sus in objContext.MESC1TS_SUSPEND
                                   where sus.SHOP_CD == SuspendCode.ShopCode &&
                                   sus.MODE == SuspendCode.Mode &&
                                   sus.MANUAL_CD == SuspendCode.ManualCode &&
                                   sus.REPAIR_CD == SuspendCode.RepairCode
                                   select sus).ToList();
                if (SuspendDBObject.Count > 0)
                {
                    objContext.MESC1TS_SUSPEND.Remove(SuspendDBObject.First());

                    objContext.SaveChanges();
                    success = true;
                    return success;
                }
                else
                {
                    return success;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }



        public List<Suspend> GetSuspendList(string ShopCode, string ManualCode, string ModeCode, string RepairCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_SUSPEND> SuspendListFromDB = new List<MESC1TS_SUSPEND>();
            List<Suspend> SuspendList = new List<Suspend>();
            try
            {
                SuspendListFromDB = (from sus in objContext.MESC1TS_SUSPEND
                                     where (sus.SHOP_CD == ShopCode)
                                     && (sus.MANUAL_CD == ManualCode)
                                     && (sus.MODE == ModeCode)
                                     && (sus.REPAIR_CD == RepairCode)
                                     select sus).ToList();



                for (int count = 0; count < SuspendListFromDB.Count; count++)
                {


                    Suspend suspend = new Suspend();
                    suspend.ShopCode = SuspendListFromDB[count].SHOP_CD;
                    suspend.ManualCode = SuspendListFromDB[count].MANUAL_CD;
                    suspend.Mode = SuspendListFromDB[count].MODE;
                    suspend.RepairCode = SuspendListFromDB[count].REPAIR_CD;
                    suspend.SuspcatID = Convert.ToString(SuspendListFromDB[count].SUSPCAT_ID);
                    suspend.ChangeUserSp = SuspendListFromDB[count].CHUSER;
                    suspend.ChangeTimeSp = SuspendListFromDB[count].CHTS;

                    SuspendList.Add(suspend);

                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return SuspendList;
        }



        public List<Manual> GetManualCodeFromShopCode(string ShopCode)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<Manual> ManualList = new List<Manual>();
            try
            {

                var ManualCodefromDB = (from S in objContext.MESC1TS_SUSPEND
                                        where S.SHOP_CD == ShopCode
                                        orderby S.MANUAL_CD
                                        select new { S.MANUAL_CD }).Distinct().ToList();

                for (int count = 0; count < ManualCodefromDB.Count; count++)
                {
                    string ManualCode = ManualCodefromDB[count].MANUAL_CD;

                    var ManualDetailsfromDB = (from M in objContext.MESC1TS_MANUAL
                                               where M.MANUAL_CD == ManualCode
                                               orderby M.MANUAL_CD
                                               select new { M.MANUAL_CD, M.MANUAL_DESC }).ToList();

                    foreach (var item in ManualDetailsfromDB)
                    {
                        ManualList.Add(new Manual()
                        {
                            ManualCode = item.MANUAL_CD,
                            ManualDesc = item.MANUAL_DESC

                        });
                    }

                }




            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ManualList;
        }

        public List<Mode> GetModeFromShopManual(string ShopCode, string ManualCode)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<Mode> ModeList = new List<Mode>();
            try
            {
                var ModefromDB = (from S in objContext.MESC1TS_SUSPEND
                                  where (S.SHOP_CD == ShopCode) && (S.MANUAL_CD == ManualCode)
                                  orderby S.MODE
                                  select new { S.MODE }).Distinct().ToList();

                for (int count = 0; count < ModefromDB.Count; count++)
                {
                    string Mode = ModefromDB[count].MODE;
                    var ModeDetailsfromDB = (from Md in objContext.MESC1TS_MODE
                                             where Md.MODE == Mode
                                             orderby Md.MODE
                                             select new { Md.MODE, Md.MODE_DESC }).ToList();

                    foreach (var item in ModeDetailsfromDB)
                    {
                        ModeList.Add(new Mode()
                        {
                            ModeCode = item.MODE,
                            ModeDescription = item.MODE_DESC

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ModeList;
        }

        public List<RepairCode> GetRepairCodeFromShopManualMode(string ShopCode, string ManualCode, string ModeCode)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<RepairCode> RepairCodeList = new List<RepairCode>();
            try
            {
                var RepairdetailsfromDB = (from S in objContext.MESC1TS_SUSPEND
                                           where (S.SHOP_CD == ShopCode) && (S.MANUAL_CD == ManualCode) && (S.MODE == ModeCode)
                                           select new { S.REPAIR_CD }).Distinct().ToList();

                for (int count = 0; count < RepairdetailsfromDB.Count; count++)
                {
                    string Repair = RepairdetailsfromDB[count].REPAIR_CD;
                    var RepairDetailsfromDB = (from R in objContext.MESC1TS_REPAIR_CODE
                                               where (R.REPAIR_CD == Repair) && (R.MANUAL_CD == ManualCode) && (R.MODE == ModeCode)
                                               select new { R.REPAIR_CD, R.REPAIR_DESC }).ToList();

                    foreach (var item in RepairDetailsfromDB)
                    {
                        RepairCodeList.Add(new RepairCode()
                        {
                            RepairCod = item.REPAIR_CD,
                            RepairDesc = item.REPAIR_DESC

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return RepairCodeList;
        }

        public List<RepairCode> GetRepairCode()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<RepairCode> RepairCodeList = new List<RepairCode>();
            try
            {
                var RepairDetailsfromDB = (from R in objContext.MESC1TS_REPAIR_CODE
                                           select new { R.REPAIR_CD, R.REPAIR_DESC }).Distinct().ToList();

                foreach (var item in RepairDetailsfromDB)
                {
                    RepairCodeList.Add(new RepairCode()
                    {
                        RepairCod = item.REPAIR_CD,
                        RepairDesc = item.REPAIR_DESC

                    });
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return RepairCodeList;
        }


        public List<SuspendCat> GetSuspendCategory()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<SuspendCat> SuspendCatList = new List<SuspendCat>();
            try
            {
                var SuspendCatfromDB = (from S in objContext.MESC1TS_SUSPEND_CAT
                                        select new { S.SUSPCAT_ID, S.SUSPCAT_DESC }).Distinct().ToList();

                foreach (var item in SuspendCatfromDB)
                {
                    SuspendCatList.Add(new SuspendCat()
                    {
                        SuspcatID = item.SUSPCAT_ID,
                        SuspcatDesc = item.SUSPCAT_DESC

                    });
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return SuspendCatList;
        }
        public List<SuspendCat> GetSuspendCategoryID(SuspendCat SuspendCatDBObject)
        {

            objContext = new ManageMasterDataServiceEntities();
            List<SuspendCat> SuspendCatList = new List<SuspendCat>();
            List<MESC1TS_SUSPEND_CAT> SuspendCatfromDB = new List<MESC1TS_SUSPEND_CAT>();
            try
            {
                SuspendCatfromDB = (from S in objContext.MESC1TS_SUSPEND_CAT
                                    where S.SUSPCAT_DESC == SuspendCatDBObject.SuspcatDesc
                                    select S).ToList();
                for (int count = 0; count < SuspendCatfromDB.Count; count++)
                {
                    SuspendCat SuspendCategory = new SuspendCat();

                    SuspendCategory.SuspcatID = SuspendCatfromDB[count].SUSPCAT_ID;
                    SuspendCategory.SuspcatDesc = SuspendCatfromDB[count].SUSPCAT_DESC;
                    SuspendCategory.ChangeUserSus = SuspendCatfromDB[count].CHUSER;
                    SuspendCategory.ChangeTimeSus = SuspendCatfromDB[count].CHTS;

                    SuspendCatList.Add(SuspendCategory);


                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return SuspendCatList;
        }

        public bool CreateSuspend(Suspend SuspendFromClient, string ShopCode, string ManualCode, string ModeCode, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_SUSPEND> SuspendList = new List<MESC1TS_SUSPEND>();
            try
            {
                SuspendList = (from s in objContext.MESC1TS_SUSPEND
                               where s.SHOP_CD == ShopCode &&
                               s.MANUAL_CD == ManualCode &&
                               s.MODE == ModeCode
                               select s).ToList();
                if (SuspendList.Count > 0)
                {
                    Msg = "Suspend " + SuspendFromClient.ShopCode + " already exists - Not Added";
                    return success;
                }
                else
                {
                    MESC1TS_SUSPEND SuspendListToBeInserted = new MESC1TS_SUSPEND();
                    SuspendListToBeInserted.SHOP_CD = ShopCode;
                    SuspendListToBeInserted.MANUAL_CD = ManualCode;
                    SuspendListToBeInserted.MODE = ModeCode;
                    SuspendListToBeInserted.REPAIR_CD = SuspendFromClient.RepairCode;
                    SuspendListToBeInserted.SUSPCAT_ID = Convert.ToInt32(SuspendFromClient.SuspcatID);
                    SuspendListToBeInserted.CHUSER = SuspendFromClient.ChangeUserSp;
                    SuspendListToBeInserted.CHTS = DateTime.Now;


                    objContext.MESC1TS_SUSPEND.Add(SuspendListToBeInserted);

                    try
                    {
                        objContext.SaveChanges();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        success = false;
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

        public List<Manual> GetManualDetails()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<Manual> ManualCodeList = new List<Manual>();
            try
            {
                var ManualListFromDB = (from R in objContext.MESC1TS_REPAIR_CODE
                                        join M in objContext.MESC1TS_MANUAL
                                         on R.MANUAL_CD equals M.MANUAL_CD
                                        orderby M.MANUAL_CD
                                        select M).Distinct().ToList();

                for (int count = 0; count < ManualListFromDB.Count; count++)
                {
                    Manual manual = new Manual();
                    manual.ManualCode = ManualListFromDB[count].MANUAL_CD;
                    manual.ManualDesc = ManualListFromDB[count].MANUAL_DESC;
                    ManualCodeList.Add(manual);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ManualCodeList;

        }

        public List<Mode> GetModeFromManual(string ManualCode)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<Mode> ModeList = new List<Mode>();
            try
            {
                var ModeDetailsfromDB = (from R in objContext.MESC1TS_REPAIR_CODE
                                         join Md in objContext.MESC1TS_MODE
                                         on R.MODE equals Md.MODE
                                         where R.MANUAL_CD == ManualCode
                                         orderby Md.MODE
                                         select new { Md.MODE, Md.MODE_DESC }).Distinct().ToList();

                foreach (var item in ModeDetailsfromDB)
                {
                    ModeList.Add(new Mode()
                    {
                        ModeCode = item.MODE,
                        ModeDescription = item.MODE_DESC

                    });
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ModeList;
        }

        public List<RepairCode> GetRepairCodeFromManualMode(string ManualCode, string ModeCode)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<RepairCode> RepairCodeList = new List<RepairCode>();
            try
            {
                var RepairDetailsfromDB = (from R in objContext.MESC1TS_REPAIR_CODE
                                           where (R.MANUAL_CD == ManualCode) && (R.MODE == ModeCode)
                                           orderby R.REPAIR_CD
                                           select R).ToList();

                foreach (var item in RepairDetailsfromDB)
                {
                    RepairCodeList.Add(new RepairCode()
                    {
                        RepairCod = item.REPAIR_CD,
                        RepairDesc = item.REPAIR_DESC

                    });
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return RepairCodeList;
        }


        public List<RprcodeExclu> GetExRprCodeFromManualModeRepairCode(string ManualCode, string ModeCode, string RepairCode, string ExRprCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<RprcodeExclu> ExRprCodeList = new List<RprcodeExclu>();
            try
            {
                var ExRprCodeFromDB = (from ExRpr in objContext.MESC1TS_RPRCODE_EXCLU
                                       where (ExRpr.MANUAL_CD == ManualCode)
                                       && (ExRpr.MODE == ModeCode)
                                       && (ExRpr.REPAIR_CD == RepairCode)
                                       && (ExRpr.EXCLU_REPAIR_CD == ExRprCode)
                                       orderby ExRpr.MANUAL_CD, ExRpr.MODE, ExRpr.REPAIR_CD, ExRpr.EXCLU_REPAIR_CD
                                       select ExRpr).ToList();

                for (int count = 0; count < ExRprCodeList.Count; count++)
                {
                    RprcodeExclu rpr = new RprcodeExclu();
                    rpr.ManualCode = ExRprCodeFromDB[count].MANUAL_CD;
                    rpr.Mode = ExRprCodeFromDB[count].MODE;
                    rpr.RepairCod = ExRprCodeFromDB[count].REPAIR_CD;
                    rpr.ExcluRepairCode = ExRprCodeFromDB[count].EXCLU_REPAIR_CD;
                    rpr.ChangeUser = ExRprCodeFromDB[count].CHUSER;
                    rpr.ChangeTime = ExRprCodeFromDB[count].CHTS;

                    ExRprCodeList.Add(rpr);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ExRprCodeList;
        }

        public bool AddExRepairCode(RprcodeExclu ExRprCodeFromClient, string RepairCode, string ModeCode, string ManualCode, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            try
            {
                if (RepairCode.TrimEnd() == ExRprCodeFromClient.ExcluRepairCode)
                {
                    Msg = "Please create Exclusionary Codes with different repair codes";
                    return success;
                }
                else if (Convert.ToInt32(RepairCode) > Convert.ToInt32(ExRprCodeFromClient.ExcluRepairCode))
                {
                    Msg = "Please create Exclusionary Codes with Lower Repair Code first";
                    return success;

                }
                else
                {
                    List<MESC1TS_REPAIR_CODE> RepairList = new List<MESC1TS_REPAIR_CODE>();
                    RepairList = (from rpr in objContext.MESC1TS_REPAIR_CODE
                                  where rpr.MANUAL_CD == ManualCode &&
                                  rpr.MODE == ModeCode &&
                                  rpr.REPAIR_CD == ExRprCodeFromClient.ExcluRepairCode
                                  select rpr).ToList();
                    if (RepairList.Count == 0)
                    {
                        Msg = "Exclusive Repair Code " + ExRprCodeFromClient.ExcluRepairCode + " is not Valid";
                        return success;
                    }

                    else
                    {
                        List<MESC1TS_RPRCODE_EXCLU> RprCodelist = new List<MESC1TS_RPRCODE_EXCLU>();
                        RprCodelist = (from exrpr in objContext.MESC1TS_RPRCODE_EXCLU
                                       where exrpr.MANUAL_CD == ManualCode &&
                                       exrpr.MODE == ModeCode &&
                                       exrpr.REPAIR_CD == RepairCode &&
                                       exrpr.EXCLU_REPAIR_CD == ExRprCodeFromClient.ExcluRepairCode
                                       select exrpr).ToList();
                        if (RprCodelist.Count > 0)
                        {
                            Msg = "Repair code already exist";
                            return success;
                        }
                        else
                        {

                            MESC1TS_RPRCODE_EXCLU RprCodeListToBeInserted = new MESC1TS_RPRCODE_EXCLU();
                            RprCodeListToBeInserted.REPAIR_CD = RepairCode;
                            RprCodeListToBeInserted.EXCLU_REPAIR_CD = ExRprCodeFromClient.ExcluRepairCode;
                            RprCodeListToBeInserted.MODE = ModeCode;
                            RprCodeListToBeInserted.MANUAL_CD = ManualCode;
                            RprCodeListToBeInserted.CHUSER = ExRprCodeFromClient.ChangeUser;
                            RprCodeListToBeInserted.CHTS = DateTime.Now;


                            objContext.MESC1TS_RPRCODE_EXCLU.Add(RprCodeListToBeInserted);

                            try
                            {
                                objContext.SaveChanges();
                                success = true;
                            }
                            catch (Exception ex)
                            {
                                success = false;
                            }
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


        public bool DeleteExRprCode(string ManualCode, string ModeCode, string RepairCode, string ExRprCode, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_RPRCODE_EXCLU> ExRprCodeFromDB = new List<MESC1TS_RPRCODE_EXCLU>();
            try
            {
                ExRprCodeFromDB = (from ExRpr in objContext.MESC1TS_RPRCODE_EXCLU
                                   where (ExRpr.MANUAL_CD == ManualCode)
                                   && (ExRpr.MODE == ModeCode)
                                   && (ExRpr.REPAIR_CD == RepairCode)
                                   && (ExRpr.EXCLU_REPAIR_CD == ExRprCode)
                                   orderby ExRpr.MANUAL_CD, ExRpr.MODE, ExRpr.REPAIR_CD, ExRpr.EXCLU_REPAIR_CD
                                   select ExRpr).ToList();
                if (ExRprCodeFromDB.Count > 0)
                {


                    objContext.MESC1TS_RPRCODE_EXCLU.Remove(ExRprCodeFromDB.First());
                    objContext.SaveChanges();
                    success = true;
                }
                else
                {
                    Msg = "Exclusive Repair Code " + ExRprCode + " does not exist-Not Deleted";
                    return success;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }


        public List<WO> GetWOListOnCountry(DateTime DateTo, DateTime DateFrom, string CountryCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<WO> WorkOrderList = new List<WO>();
            double GateDt = 0.0;
            double CreationDate1 = 0.0;
            double CreationDate2 = 0.0;
            double ApprovalDate1 = 0.0;
            double ApprovalDate2 = 0.0;
            double RepairDate = 0.0;
            double AvgEstimate = 0.0;
            double AvgAuthorise = 0.0;
            double AvgRepair = 0.0;
            try
            {
                var CountryDBObject = (from loc in objContext.MESC1TS_LOCATION
                                       where loc.COUNTRY_CD == CountryCode
                                       select loc).Distinct().ToList();
                for (int countcont = 0; countcont < CountryDBObject.Count; countcont++)
                {
                    string LocCode = CountryDBObject[countcont].LOC_CD;
                    var WOListDBObject = (from shop in objContext.MESC1TS_SHOP
                                          where shop.LOC_CD == LocCode
                                          select shop).Distinct().ToList();

                    for (int count = 0; count < WOListDBObject.Count; count++)
                    {
                        string ShopCode = WOListDBObject[count].SHOP_CD;
                        var WOListDBObject1 = (from wo in objContext.MESC1TS_WO
                                               where (wo.GATEINDTE > DateFrom) && (wo.GATEINDTE < DateTo) &&
                                               (wo.GATEINDTE != null) &&
                                               (wo.SHOP_CD == ShopCode)
                                               select wo).ToList();

                        int length1 = WOListDBObject1.Count();
                        if (WOListDBObject1.Count > 0)
                        {
                            for (int count1 = 0; count1 < WOListDBObject1.Count; count1++)
                            {
                                WO WoListItem = new WO();
                                WoListItem.GateInDate = WOListDBObject1[count1].GATEINDTE;
                                WoListItem.CreateTime = WOListDBObject1[count1].CRTS;
                                DateTime GateDate = Convert.ToDateTime(WoListItem.GateInDate);
                                DateTime CrTime = Convert.ToDateTime(WoListItem.CreateTime);
                                GateDt += Convert.ToDouble(GateDate.ToOADate());
                                CreationDate1 += Convert.ToDouble(CrTime.ToOADate());

                            }
                            if (CreationDate1 > GateDt)
                            {
                                AvgEstimate = (CreationDate1 - GateDt) / length1;
                                AvgEstimate = Math.Round(AvgEstimate, 2);
                            }
                            else
                            {
                                AvgEstimate = (GateDt - CreationDate1) / length1;
                                AvgEstimate = Math.Round(AvgEstimate, 2);
                            }
                        }
                        else
                        {
                            AvgEstimate = 0.0;
                        }



                        var WOListDBObject2 = (from wo in objContext.MESC1TS_WO
                                               where (wo.CRTS > DateFrom) && (wo.CRTS < DateTo) &&
                                               (wo.CRTS != null) && (wo.SHOP_CD == ShopCode)
                                               select wo).ToList();
                        int length2 = WOListDBObject2.Count();
                        if (WOListDBObject2.Count > 0)
                        {
                            for (int count2 = 0; count2 < WOListDBObject2.Count; count2++)
                            {
                                WO WoListItem = new WO();
                                WoListItem.CreateTime = WOListDBObject2[count2].CRTS;
                                WoListItem.ApprovalDate = WOListDBObject2[count2].APPROVAL_DTE;
                                DateTime CrTime = Convert.ToDateTime(WoListItem.CreateTime);
                                DateTime AppDate = Convert.ToDateTime(WoListItem.ApprovalDate);
                                CreationDate2 += Convert.ToDouble(CrTime.ToOADate());
                                ApprovalDate1 += Convert.ToDouble(AppDate.ToOADate());

                            }
                            if (ApprovalDate1 > CreationDate2)
                            {
                                AvgAuthorise = (ApprovalDate1 - CreationDate2) / length2;
                                AvgAuthorise = Math.Round(AvgAuthorise, 2);
                            }
                            else
                            {
                                AvgAuthorise = (CreationDate2 - ApprovalDate1) / length2;
                                AvgAuthorise = Math.Round(AvgAuthorise, 2);
                            }
                        }
                        else
                        {
                            AvgAuthorise = 0.0;
                        }

                        var WOListDBObject3 = (from wo in objContext.MESC1TS_WO
                                               where (wo.APPROVAL_DTE > DateFrom) && (wo.APPROVAL_DTE < DateTo) &&
                                               (wo.APPROVAL_DTE != null) && (wo.SHOP_CD == ShopCode)
                                               select wo).ToList();
                        int length3 = WOListDBObject3.Count();
                        if (WOListDBObject3.Count > 0)
                        {
                            for (int count3 = 0; count3 < WOListDBObject3.Count; count3++)
                            {
                                WO WoListItem = new WO();
                                WoListItem.ApprovalDate = WOListDBObject3[count3].APPROVAL_DTE;
                                WoListItem.RepairDate = WOListDBObject3[count3].REPAIR_DTE;
                                DateTime AppDate = Convert.ToDateTime(WoListItem.ApprovalDate);
                                DateTime RprDate = Convert.ToDateTime(WoListItem.RepairDate);
                                ApprovalDate2 += Convert.ToDouble(AppDate.ToOADate());
                                RepairDate += Convert.ToDouble(RprDate.ToOADate());
                            }

                            if (RepairDate > ApprovalDate2)
                            {
                                AvgRepair = (RepairDate - ApprovalDate2) / length3;
                                AvgRepair = Math.Round(AvgRepair, 2);
                            }
                            else
                            {
                                AvgRepair = (ApprovalDate2 - RepairDate) / length3;
                                AvgRepair = Math.Round(AvgRepair, 2);

                            }
                        }
                        else
                        {

                            AvgRepair = 0.0;
                        }
                        WO WoList = new WO();
                        WoList.LocCode = LocCode;
                        WoList.ShopCode = ShopCode;
                        WoList.AvgtimeToEstimate = AvgEstimate;
                        WoList.AvgTimeToAuthorise = AvgAuthorise;
                        WoList.AvgTimeToRepair = AvgRepair;
                        WorkOrderList.Add(WoList);
                    }

                }

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return WorkOrderList;
        }

        public List<WO> GetWOListOnLocation(DateTime DateTo, DateTime DateFrom, string LocationCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<WO> WorkOrderList = new List<WO>();

            double GateDt = 0.0;
            double CreationDate1 = 0.0;
            double CreationDate2 = 0.0;
            double ApprovalDate1 = 0.0;
            double ApprovalDate2 = 0.0;
            double RepairDate = 0.0;
            double AvgEstimate = 0.0;
            double AvgAuthorise = 0.0;
            double AvgRepair = 0.0;
            try
            {
                var WOListDBObject = (from shop in objContext.MESC1TS_SHOP
                                      where shop.LOC_CD == LocationCode
                                      select shop).Distinct().ToList();

                for (int count = 0; count < WOListDBObject.Count; count++)
                {
                    string ShopCode = WOListDBObject[count].SHOP_CD;
                    var WOListDBObject1 = (from wo in objContext.MESC1TS_WO
                                           where (wo.GATEINDTE > DateFrom) && (wo.GATEINDTE < DateTo) &&
                                           (wo.GATEINDTE != null) &&
                                           (wo.SHOP_CD == ShopCode)
                                           select wo).ToList();

                    int length1 = WOListDBObject1.Count();
                    if (WOListDBObject1.Count > 0)
                    {
                        for (int count1 = 0; count1 < WOListDBObject1.Count; count1++)
                        {
                            WO WoListItem = new WO();
                            WoListItem.GateInDate = WOListDBObject1[count1].GATEINDTE;
                            WoListItem.CreateTime = WOListDBObject1[count1].CRTS;
                            DateTime GateDate = Convert.ToDateTime(WoListItem.GateInDate);
                            DateTime CrTime = Convert.ToDateTime(WoListItem.CreateTime);
                            GateDt += Convert.ToDouble(GateDate.ToOADate());
                            CreationDate1 += Convert.ToDouble(CrTime.ToOADate());

                        }
                        if (CreationDate1 > GateDt)
                        {
                            AvgEstimate = (CreationDate1 - GateDt) / length1;
                            AvgEstimate = Math.Round(AvgEstimate, 2);
                        }
                        else
                        {
                            AvgEstimate = (GateDt - CreationDate1) / length1;
                            AvgEstimate = Math.Round(AvgEstimate, 2);
                        }
                    }
                    else
                    {
                        AvgEstimate = 0.0;
                    }



                    var WOListDBObject2 = (from wo in objContext.MESC1TS_WO
                                           where (wo.CRTS > DateFrom) && (wo.CRTS < DateTo) &&
                                           (wo.CRTS != null) && (wo.SHOP_CD == ShopCode)
                                           select wo).ToList();
                    int length2 = WOListDBObject2.Count();
                    if (WOListDBObject2.Count > 0)
                    {
                        for (int count2 = 0; count2 < WOListDBObject2.Count; count2++)
                        {
                            WO WoListItem = new WO();
                            WoListItem.CreateTime = WOListDBObject2[count2].CRTS;
                            WoListItem.ApprovalDate = WOListDBObject2[count2].APPROVAL_DTE;
                            DateTime CrTime = Convert.ToDateTime(WoListItem.CreateTime);
                            DateTime AppDate = Convert.ToDateTime(WoListItem.ApprovalDate);
                            CreationDate2 += Convert.ToDouble(CrTime.ToOADate());
                            ApprovalDate1 += Convert.ToDouble(AppDate.ToOADate());

                        }
                        if (ApprovalDate1 > CreationDate2)
                        {
                            AvgAuthorise = (ApprovalDate1 - CreationDate2) / length2;
                            AvgAuthorise = Math.Round(AvgAuthorise, 2);
                        }
                        else
                        {
                            AvgAuthorise = (CreationDate2 - ApprovalDate1) / length2;
                            AvgAuthorise = Math.Round(AvgAuthorise, 2);
                        }
                    }
                    else
                    {
                        AvgAuthorise = 0.0;
                    }

                    var WOListDBObject3 = (from wo in objContext.MESC1TS_WO
                                           where (wo.APPROVAL_DTE > DateFrom) && (wo.APPROVAL_DTE < DateTo) &&
                                           (wo.APPROVAL_DTE != null) && (wo.SHOP_CD == ShopCode)
                                           select wo).ToList();
                    int length3 = WOListDBObject3.Count();
                    if (WOListDBObject3.Count > 0)
                    {
                        for (int count3 = 0; count3 < WOListDBObject3.Count; count3++)
                        {
                            WO WoListItem = new WO();
                            WoListItem.ApprovalDate = WOListDBObject3[count3].APPROVAL_DTE;
                            WoListItem.RepairDate = WOListDBObject3[count3].REPAIR_DTE;
                            DateTime AppDate = Convert.ToDateTime(WoListItem.ApprovalDate);
                            DateTime RprDate = Convert.ToDateTime(WoListItem.RepairDate);
                            ApprovalDate2 += Convert.ToDouble(AppDate.ToOADate());
                            RepairDate += Convert.ToDouble(RprDate.ToOADate());
                        }

                        if (RepairDate > ApprovalDate2)
                        {
                            AvgRepair = (RepairDate - ApprovalDate2) / length3;
                            AvgRepair = Math.Round(AvgRepair, 2);
                        }
                        else
                        {
                            AvgRepair = (ApprovalDate2 - RepairDate) / length3;
                            AvgRepair = Math.Round(AvgRepair, 2);

                        }
                    }
                    else
                    {

                        AvgRepair = 0.0;
                    }
                    WO WoList = new WO();
                    WoList.LocCode = LocationCode;
                    WoList.ShopCode = ShopCode;
                    WoList.AvgtimeToEstimate = AvgEstimate;
                    WoList.AvgTimeToAuthorise = AvgAuthorise;
                    WoList.AvgTimeToRepair = AvgRepair;
                    WorkOrderList.Add(WoList);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return WorkOrderList;
        }
        public List<WO> GetWOListOnShop(DateTime DateTo, DateTime DateFrom, string ShopCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<WO> WorkOrderList = new List<WO>();

            double GateDt = 0.0;
            double CreationDate1 = 0.0;
            double CreationDate2 = 0.0;
            double ApprovalDate1 = 0.0;
            double ApprovalDate2 = 0.0;
            double RepairDate = 0.0;
            double AvgEstimate = 0.0;
            double AvgAuthorise = 0.0;
            double AvgRepair = 0.0;
            try
            {
                var WOListDBObject1 = (from wo in objContext.MESC1TS_WO
                                       where (wo.GATEINDTE > DateFrom) && (wo.GATEINDTE < DateTo) &&
                                       (wo.GATEINDTE != null) &&
                                       (wo.SHOP_CD == ShopCode)
                                       select wo).ToList();

                int length1 = WOListDBObject1.Count();
                if (WOListDBObject1.Count > 0)
                {
                    for (int count = 0; count < WOListDBObject1.Count; count++)
                    {
                        WO WoListItem = new WO();
                        WoListItem.GateInDate = WOListDBObject1[count].GATEINDTE;
                        WoListItem.CreateTime = WOListDBObject1[count].CRTS;
                        DateTime GateDate = Convert.ToDateTime(WoListItem.GateInDate);
                        DateTime CrTime = Convert.ToDateTime(WoListItem.CreateTime);
                        GateDt += Convert.ToDouble(GateDate.ToOADate());
                        CreationDate1 += Convert.ToDouble(CrTime.ToOADate());

                    }
                    AvgEstimate = (CreationDate1 - GateDt) / length1;
                    AvgEstimate = Math.Round(AvgEstimate, 2);
                }
                else
                {
                    AvgEstimate = 0.0;
                }

                var WOListDBObject2 = (from wo in objContext.MESC1TS_WO
                                       where (wo.CRTS > DateFrom) && (wo.CRTS < DateTo) &&
                                       (wo.CRTS != null) && (wo.SHOP_CD == ShopCode)
                                       select wo).ToList();
                int length2 = WOListDBObject2.Count();
                if (WOListDBObject2.Count > 0)
                {
                    for (int count = 0; count < WOListDBObject2.Count; count++)
                    {
                        WO WoListItem = new WO();
                        WoListItem.CreateTime = WOListDBObject2[count].CRTS;
                        WoListItem.ApprovalDate = WOListDBObject2[count].APPROVAL_DTE;
                        DateTime CrTime = Convert.ToDateTime(WoListItem.CreateTime);
                        DateTime AppDate = Convert.ToDateTime(WoListItem.ApprovalDate);
                        CreationDate2 += Convert.ToDouble(CrTime.ToOADate());
                        ApprovalDate1 += Convert.ToDouble(AppDate.ToOADate());

                    }
                    AvgAuthorise = (ApprovalDate1 - CreationDate2) / length2;
                    AvgAuthorise = Math.Round(AvgAuthorise, 2);
                }
                else
                {
                    AvgAuthorise = 0.0;
                }

                var WOListDBObject3 = (from wo in objContext.MESC1TS_WO
                                       where (wo.APPROVAL_DTE > DateFrom) && (wo.APPROVAL_DTE < DateTo) &&
                                       (wo.APPROVAL_DTE != null) && (wo.SHOP_CD == ShopCode)
                                       select wo).ToList();
                int length3 = WOListDBObject3.Count();
                if (WOListDBObject3.Count > 0)
                {
                    for (int count = 0; count < WOListDBObject3.Count; count++)
                    {
                        WO WoListItem = new WO();
                        WoListItem.ApprovalDate = WOListDBObject3[count].APPROVAL_DTE;
                        WoListItem.RepairDate = WOListDBObject3[count].REPAIR_DTE;
                        DateTime AppDate = Convert.ToDateTime(WoListItem.ApprovalDate);
                        DateTime RprDate = Convert.ToDateTime(WoListItem.RepairDate);
                        ApprovalDate2 += Convert.ToDouble(AppDate.ToOADate());
                        RepairDate += Convert.ToDouble(RprDate.ToOADate());
                    }

                    if (RepairDate > ApprovalDate2)
                    {
                        AvgRepair = (RepairDate - ApprovalDate2) / length3;
                        AvgRepair = Math.Round(AvgRepair, 2);
                    }
                    else
                    {
                        AvgRepair = (ApprovalDate2 - RepairDate) / length3;
                        AvgRepair = Math.Round(AvgRepair, 2);

                    }
                }
                else
                {

                    AvgRepair = 0.0;
                }

                var LocListDBObject = (from shop in objContext.MESC1TS_SHOP
                                       where shop.SHOP_CD == ShopCode
                                       select shop).ToList();
                WO WoList = new WO();
                WoList.LocCode = LocListDBObject[0].LOC_CD;
                WoList.ShopCode = ShopCode;
                WoList.AvgtimeToEstimate = AvgEstimate;
                WoList.AvgTimeToAuthorise = AvgAuthorise;
                WoList.AvgTimeToRepair = AvgRepair;
                WorkOrderList.Add(WoList);
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return WorkOrderList;
        }

        #region IManageMasterData Members





        #endregion

        #region IManageMasterData Members


        public List<SubType> GetSubTypeList()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Index
        public List<Index> GetIndexes()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_INDEX> indexList = new List<MESC1TS_INDEX>();
            indexList = (from index in objContext.MESC1TS_INDEX
                         select index).ToList();

            return PrepareDataContract(indexList);

        }

        public List<Index> GetIndex(int IndexID, string Manual_CD, string Mode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_INDEX> indexList = new List<MESC1TS_INDEX>();
            indexList = (from index in objContext.MESC1TS_INDEX
                         where index.INDEX_ID == IndexID && index.MODE == Mode && index.MANUAL_CD == Manual_CD
                         select index).ToList();

            return PrepareDataContract(indexList);

        }

        private List<Index> PrepareDataContract(List<MESC1TS_INDEX> IndexFromDB)
        {
            List<Index> IndexList = new List<Index>();
            for (int count = 0; count < IndexFromDB.Count; count++)
            {
                Index index = new Index();
                index.IndexID = IndexFromDB[count].INDEX_ID;
                index.IndexDesc = IndexFromDB[count].INDEX_DESC;
                index.IndexPriority = IndexFromDB[count].INDEX_PRIORITY;
                index.ManualCode = IndexFromDB[count].MANUAL_CD;
                index.Mode = IndexFromDB[count].MODE;
                index.ChangeUser = GetUserNamePTI(IndexFromDB[count].CHUSER);
                index.ChangeTime = IndexFromDB[count].CHTS;
                IndexList.Add(index);
            }
            return IndexList;
        }

        public List<Index> GetIndexesForDropDown(string Manual_CD, string Mode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_INDEX> indexList = new List<MESC1TS_INDEX>();
            indexList = (from index in objContext.MESC1TS_INDEX
                         where index.MANUAL_CD == Manual_CD && index.MODE == Mode
                         select index).ToList();
            List<Index> IndexCodeList = new List<Index>();
            for (int count = 0; count < indexList.Count; count++)
            {
                Index index = new Index();
                index.IndexID = indexList[count].INDEX_ID;
                index.IndexDesc = indexList[count].INDEX_ID + " - " + indexList[count].INDEX_DESC;
                IndexCodeList.Add(index);
            }
            return IndexCodeList;
        }

        public List<Manual> GetIndexManual()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUAL> manualList = new List<MESC1TS_MANUAL>();
            manualList = (from nd in objContext.MESC1TS_INDEX
                          join ma in objContext.MESC1TS_MANUAL
                              on nd.MANUAL_CD equals ma.MANUAL_CD
                          orderby ma.MANUAL_DESC
                          select ma).Distinct().ToList();
            List<Manual> ManualCodeList = new List<Manual>();
            for (int count = 0; count < manualList.Count; count++)
            {
                Manual manual = new Manual();
                manual.ManualCode = manualList[count].MANUAL_CD;
                manual.ManualDesc = manualList[count].MANUAL_CD + " - " + manualList[count].MANUAL_DESC;
                ManualCodeList.Add(manual);
            }

            return ManualCodeList;
        }

        public List<Mode> GetIndexMode(string manual_cd)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> modeList = new List<MESC1TS_MODE>();
            modeList = (from nd in objContext.MESC1TS_INDEX
                        join ma in objContext.MESC1TS_MODE
                        on nd.MODE equals ma.MODE
                        where nd.MANUAL_CD == manual_cd
                        select ma).Distinct().ToList();
            List<Mode> ModeCodeList = new List<Mode>();
            for (int count = 0; count < modeList.Count; count++)
            {
                Mode mode = new Mode();
                mode.ModeCode = modeList[count].MODE;
                mode.ModeDescription = modeList[count].MODE + " - " + modeList[count].MODE_DESC;
                ModeCodeList.Add(mode);
            }

            return ModeCodeList;
        }

        public bool UpdateIndex(Index updatedIndex, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_INDEX> indexDBObj = new List<MESC1TS_INDEX>();
            indexDBObj = (from index in objContext.MESC1TS_INDEX
                          where index.INDEX_ID == updatedIndex.IndexID &&
                          index.MANUAL_CD == updatedIndex.ManualCode &&
                          index.MODE == updatedIndex.Mode
                          select index).ToList();
            indexDBObj[0].INDEX_DESC = updatedIndex.IndexDesc;
            //indexDBObj[0].INDEX_PRIORITY = updatedIndex.IndexPriority;
            //indexDBObj[0].MANUAL_CD = updatedIndex.ManualCode;
            //indexDBObj[0].MODE = updatedIndex.Mode;
            indexDBObj[0].CHUSER = updatedIndex.ChangeUser;
            indexDBObj[0].CHTS = DateTime.Now;
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
            }
            return success;
        }

        public bool DeleteIndex(int indexID, string manualCode, string mode, ref string Msg)
        {
            bool success = false;
            List<RepairCode> repairCodes = GetRepairCodeByIndex(manualCode, mode, indexID);
            if (repairCodes.Count > 0)
            {
                Msg = "Manual Code " + manualCode + " Mode " + mode + " Index ID " + indexID.ToString() + " Exists on " + repairCodes[0].RepairCod + " Repair Code Records - Not Deleted";
                return success;
            }
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_INDEX> indexDBObj = new List<MESC1TS_INDEX>();
            indexDBObj = (from index in objContext.MESC1TS_INDEX
                          where ((index.INDEX_ID == indexID) && (index.MODE == mode) && (index.MANUAL_CD == manualCode))
                          select index).ToList();
            try
            {
                objContext.MESC1TS_INDEX.Remove(indexDBObj.First());
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex) { Msg = ex.Message; }
            return success;
        }

        public bool CreateIndex(Index insertIndex, ref string Msg)
        {
            bool success = false;
            List<Index> oldIndex = GetIndex(insertIndex.IndexID, insertIndex.ManualCode, insertIndex.Mode);
            if (oldIndex.Count > 0)
            {
                Msg = "Index ID " + insertIndex.IndexID + " Already Exists - Not Added";
                return success;
            }
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_INDEX indexDBObj = new MESC1TS_INDEX();
            indexDBObj.INDEX_ID = insertIndex.IndexID;
            indexDBObj.INDEX_DESC = insertIndex.IndexDesc;
            indexDBObj.INDEX_PRIORITY = insertIndex.IndexPriority;
            indexDBObj.MANUAL_CD = insertIndex.ManualCode;
            indexDBObj.MODE = insertIndex.Mode;
            indexDBObj.CHUSER = insertIndex.ChangeUser;
            indexDBObj.CHTS = DateTime.Now;
            try
            {
                objContext.MESC1TS_INDEX.Add(indexDBObj);
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex) { Msg = ex.Message; }
            return success;
        }

        public List<Manual> GetIndexAllActiveManual()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUAL> manualList = new List<MESC1TS_MANUAL>();
            manualList = (from m in objContext.MESC1TS_MANUAL_MODE
                          join mo in objContext.MESC1TS_MANUAL
                              on m.MANUAL_CD equals mo.MANUAL_CD
                          where m.ACTIVE_SW == "Y"
                          orderby mo.MANUAL_CD
                          select mo).Distinct().ToList();
            List<Manual> ManualCodeList = new List<Manual>();
            for (int count = 0; count < manualList.Count; count++)
            {
                Manual manual = new Manual();
                manual.ManualCode = manualList[count].MANUAL_CD;
                manual.ManualDesc = manualList[count].MANUAL_CD + " - " + manualList[count].MANUAL_DESC;
                ManualCodeList.Add(manual);
            }

            return ManualCodeList;
        }

        public List<Mode> GetIndexAllActiveMode(string manual_cd)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> modeList = new List<MESC1TS_MODE>();
            modeList = (from m in objContext.MESC1TS_MANUAL_MODE
                        join ma in objContext.MESC1TS_MODE
                        on m.MODE equals ma.MODE
                        orderby ma.MODE
                        where m.MANUAL_CD == manual_cd &&
                        m.ACTIVE_SW == "Y"
                        select ma).Distinct().ToList();
            List<Mode> ModeCodeList = new List<Mode>();
            for (int count = 0; count < modeList.Count; count++)
            {
                Mode mode = new Mode();
                mode.ModeCode = modeList[count].MODE;
                mode.ModeDescription = modeList[count].MODE + " - " + modeList[count].MODE_DESC;
                ModeCodeList.Add(mode);
            }

            return ModeCodeList;
        }

        #endregion Index

        //----------------------pinaki---------------------------------------------
        public List<Manufactur> GetManufacturerDiscountList()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUFACTUR> MDiscount = new List<MESC1TS_MANUFACTUR>();
            MDiscount = (from pay in objContext.MESC1TS_MANUFACTUR
                         select pay).ToList();

            return PrepareDataContract_MDiscount(MDiscount);

        }
        private List<Manufactur> PrepareDataContract_MDiscount(List<MESC1TS_MANUFACTUR> modeFromDB)
        {
            List<Manufactur> modeList = new List<Manufactur>();
            for (int count = 0; count < modeFromDB.Count; count++)
            {
                Manufactur mode = new Manufactur();
                mode.ManufacturCd = modeFromDB[count].MANUFCTR;
                //  mode.ManufacturName = modeFromDB[count].MANUFCTR + '-' + modeFromDB[count].MANUFACTUR_NAME;
                mode.ManufacturName = modeFromDB[count].MANUFACTUR_NAME;
                mode.DiscountPercent = modeFromDB[count].DISCOUNT_PERCENT;
                mode.ChangeUser = (GetUserNamePTI(modeFromDB[count].CHUSER.ToString())).Replace("|", " ");
                mode.ChangeTime = modeFromDB[count].CHTS;
                modeList.Add(mode);
            }
            return modeList;
        }

        public List<Manufactur> GetManufacturerDiscountListCode(string code)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUFACTUR> MDiscount = new List<MESC1TS_MANUFACTUR>();
            MDiscount = (from pay in objContext.MESC1TS_MANUFACTUR
                         where pay.MANUFCTR == code
                         select pay).ToList();

            return PrepareDataContract_MDiscountCode(MDiscount);

        }



        private List<Manufactur> PrepareDataContract_MDiscountCode(List<MESC1TS_MANUFACTUR> modeFromDB)
        {
            List<Manufactur> modeList = new List<Manufactur>();
            for (int count = 0; count < modeFromDB.Count; count++)
            {
                Manufactur mode = new Manufactur();
                mode.ManufacturCd = modeFromDB[count].MANUFCTR;
                //  mode.ManufacturName = modeFromDB[count].MANUFCTR + '-' + modeFromDB[count].MANUFACTUR_NAME;
                mode.ManufacturName = modeFromDB[count].MANUFACTUR_NAME;
                mode.DiscountPercent = modeFromDB[count].DISCOUNT_PERCENT;
                mode.ChangeUser = (GetUserNamePTI(modeFromDB[count].CHUSER.ToString())).Replace("|", " ");
                mode.ChangeTime = modeFromDB[count].CHTS;
                modeList.Add(mode);
            }
            return modeList;
        }

        public Manufactur UpdateManufacturerDiscount(Manufactur MDiscountToBeUpdated)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<Manufactur> oldDisCount = GetManufacturerDiscountListCode(MDiscountToBeUpdated.ManufacturCd);

            List<MESC1TS_MANUFACTUR> MDiscountObject = new List<MESC1TS_MANUFACTUR>();
            MDiscountObject = (from pay in objContext.MESC1TS_MANUFACTUR
                               where pay.MANUFCTR == MDiscountToBeUpdated.ManufacturCd
                               select pay).ToList();


            MDiscountObject[0].MANUFCTR = MDiscountToBeUpdated.ManufacturCd;
            MDiscountObject[0].MANUFACTUR_NAME = MDiscountToBeUpdated.ManufacturName;
            MDiscountObject[0].DISCOUNT_PERCENT = MDiscountToBeUpdated.DiscountPercent;
            MDiscountObject[0].CHUSER = MDiscountToBeUpdated.ChangeUser;
            MDiscountObject[0].CHTS = MDiscountToBeUpdated.ChangeTime;
            MDiscountObject[0].CHTS = DateTime.Now;
            try
            {
                objContext.SaveChanges();
                MDiscountToBeUpdated.ChangeTime = MDiscountObject[0].CHTS;
                InsertMDiscountAuditTrail(MDiscountToBeUpdated, oldDisCount);
            }
            catch
            {
            }
            return MDiscountToBeUpdated;
        }
        public string CreateManufacturerDiscount(Manufactur MDiscountToBeCreated)
        {
            string Status = "Failure";
            string Msg = "";
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUFACTUR> MDis = new List<MESC1TS_MANUFACTUR>();
            MDis = (from pay in objContext.MESC1TS_MANUFACTUR
                    where pay.MANUFCTR == MDiscountToBeCreated.ManufacturCd
                    select pay).ToList();

            if (MDis.Count > 0)
            {
                Msg = "Manufacturer Discount Code  " + MDiscountToBeCreated.ManufacturCd + " Already Exists - Not Added";
                Status = "Duplicate";
                return Status;
            }

            MESC1TS_MANUFACTUR MDiscountToBeInserted = new MESC1TS_MANUFACTUR();
            MDiscountToBeInserted.MANUFCTR = MDiscountToBeCreated.ManufacturCd;
            MDiscountToBeInserted.MANUFACTUR_NAME = MDiscountToBeCreated.ManufacturName;
            MDiscountToBeInserted.DISCOUNT_PERCENT = MDiscountToBeCreated.DiscountPercent;
            MDiscountToBeInserted.CHUSER = MDiscountToBeCreated.ChangeUser;
            MDiscountToBeInserted.CHTS = MDiscountToBeCreated.ChangeTime;
            MDiscountToBeInserted.CHTS = DateTime.Now;
            objContext.MESC1TS_MANUFACTUR.Add(MDiscountToBeInserted);
            try
            {
                objContext.SaveChanges();
                Status = "Success";

            }
            catch (Exception ex)
            {
                Status = "Failed";

            }

            return Status;
        }

        public bool DeleteManufacturerDiscount(string MDiscountCode)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUFACTUR> MDiscountObject = new List<MESC1TS_MANUFACTUR>();
            MDiscountObject = (from pay in objContext.MESC1TS_MANUFACTUR
                               where pay.MANUFCTR == MDiscountCode
                               select pay).ToList();

            objContext.MESC1TS_MANUFACTUR.Remove(MDiscountObject.First());

            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }


        private bool InsertMDiscountAuditTrail(Manufactur MDiscountToBeUpdated, List<Manufactur> oldDiscount)
        {
            bool success = false;
            // List<Damage> oldDamages = GetDamageCode(DamageToBeUpdated.DamageCedexCode);
            MESC1TS_REFAUDIT record;
            objContext = new ManageMasterDataServiceEntities();

            var ChangeUser = "";
            var ChangeUser1 = GetUserNamePTI(MDiscountToBeUpdated.ChangeUser);
            if (ChangeUser1.Contains('|'))
            {
                ChangeUser = ChangeUser1.Split('|')[0];
            }

            if (oldDiscount[0].DiscountPercent != MDiscountToBeUpdated.DiscountPercent)
            {
                record = BuildMDiscountAuditRecordSet("MESC1TS_MANUFACTUR", "DISCOUNT PERCENT", oldDiscount[0].DiscountPercent.ToString(), MDiscountToBeUpdated.DiscountPercent.ToString(), ChangeUser, MDiscountToBeUpdated.ChangeTime, MDiscountToBeUpdated.ManufacturCd);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }

            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        private MESC1TS_REFAUDIT BuildMDiscountAuditRecordSet(string TableName, string ColName, string OldValue, string NewValue, string ChangeUser, DateTime ChangeTime, string ID)
        {
            MESC1TS_REFAUDIT record = new MESC1TS_REFAUDIT();
            record.UNIQUE_ID = ID;
            record.TAB_NAME = TableName;
            record.COL_NAME = ColName;
            record.OLD_VALUE = OldValue;
            record.NEW_VALUE = NewValue;
            record.CHUSER = ChangeUser;
            record.CHTS = ChangeTime;
            return record;
        }

        public List<RefAudit> GetAuditTrailMDiscount(string RecordId, string TableName)
        {
            List<RefAudit> RefAuditList = new List<RefAudit>();
            try
            {

                dynamic Query = "";
                objContext = new ManageMasterDataServiceEntities();

                if (TableName == "ManufacturerDiscount")
                {
                    Query = (from MR in objContext.MESC1TS_REFAUDIT
                             from U in objContext.SEC_USER
                                       .Where(U => U.LOGIN == MR.CHUSER)
                                       .DefaultIfEmpty()
                             where MR.TAB_NAME == "MESC1TS_MANUFACTUR" && MR.UNIQUE_ID == RecordId
                             select new { MR.TAB_NAME, MR.UNIQUE_ID, MR.COL_NAME, MR.OLD_VALUE, MR.NEW_VALUE, MR.CHUSER, U.LASTNAME, U.FIRSTNAME, MR.CHTS }).ToList();

                }
                foreach (var obj in Query)
                {
                    RefAudit objRef = new RefAudit();
                    objRef.TabName = obj.TAB_NAME;
                    objRef.UniqueID = obj.UNIQUE_ID;
                    objRef.ColName = obj.COL_NAME;
                    objRef.OldValue = obj.OLD_VALUE;
                    objRef.NewValue = obj.NEW_VALUE;
                    objRef.ChangeUser = obj.CHUSER;
                    objRef.FirstName = obj.FIRSTNAME;
                    objRef.LastName = obj.LASTNAME;
                    objRef.ChangeTime = obj.CHTS;
                    RefAuditList.Add(objRef);
                };
                RefAuditList.OrderByDescending(m => m.ChangeTime);
            }

            catch (Exception ex)
            {
                string ss = ex.Message;

            }
            return RefAuditList;
        }
        //-------------------------------------------------------------------------
        public List<Model> GetManufacturerModelList(string ManufacturerCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODEL> MMmodel = new List<MESC1TS_MODEL>();
            MMmodel = (from pay in objContext.MESC1TS_MODEL
                       where pay.MANUFCTR == ManufacturerCode
                       select pay).ToList();

            return PrepareDataContract_Model(MMmodel);

        }
        private List<Model> PrepareDataContract_Model(List<MESC1TS_MODEL> modelFromDB)
        {
            List<Model> modeList = new List<Model>();
            for (int count = 0; count < modelFromDB.Count; count++)
            {
                Model mode = new Model();
                mode.ManufacturCd = modelFromDB[count].MANUFCTR;
                //  mode.ManufacturName = modeFromDB[count].MANUFCTR + '-' + modeFromDB[count].MANUFACTUR_NAME;
                mode.ModelNo = modelFromDB[count].MODEL_NO;
                mode.IndicatorCd = modelFromDB[count].INDICATOR_CD;
                mode.ChangeUser = (GetUserNamePTI(modelFromDB[count].CHUSER.ToString())).Replace("|", " ");
                mode.ChangeTime = modelFromDB[count].CHTS;
                modeList.Add(mode);
            }
            return modeList;
        }
        public Model UpdateManufacturerModel(Model ModelToBeUpdated)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_MODEL> ModelObject = new List<MESC1TS_MODEL>();
            ModelObject = (from pay in objContext.MESC1TS_MODEL
                           where pay.MODEL_NO == ModelToBeUpdated.ModelNo
                            && pay.MANUFCTR == ModelToBeUpdated.ManufacturCd
                           select pay).ToList();


            ModelObject[0].MANUFCTR = ModelToBeUpdated.ManufacturCd;
            ModelObject[0].MODEL_NO = ModelToBeUpdated.ModelNo;
            ModelObject[0].INDICATOR_CD = ModelToBeUpdated.IndicatorCd;
            ModelObject[0].CHUSER = ModelToBeUpdated.ChangeUser;
            ModelObject[0].CHTS = ModelToBeUpdated.ChangeTime;
            ModelObject[0].CHTS = DateTime.Now;
            objContext.SaveChanges();
            return ModelToBeUpdated;
        }
        public bool DeleteManufacturerModel(string MDiscountCode, string MmodelNo)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODEL> MDiscountObject = new List<MESC1TS_MODEL>();
            MDiscountObject = (from pay in objContext.MESC1TS_MODEL
                               where pay.MANUFCTR == MDiscountCode
                               && pay.MODEL_NO == MmodelNo
                               select pay).ToList();

            objContext.MESC1TS_MODEL.Remove(MDiscountObject.First());
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }


        public bool CreateManufacturerModel(Model MModelToBeCreated)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_MODEL MModelToBeInserted = new MESC1TS_MODEL();
            MModelToBeInserted.MANUFCTR = MModelToBeCreated.ManufacturCd;
            MModelToBeInserted.MODEL_NO = MModelToBeCreated.ModelNo;
            MModelToBeInserted.INDICATOR_CD = MModelToBeCreated.IndicatorCd;
            MModelToBeInserted.CHUSER = MModelToBeCreated.ChangeUser;
            MModelToBeInserted.CHTS = MModelToBeCreated.ChangeTime;
            MModelToBeInserted.CHTS = DateTime.Now;
            objContext.MESC1TS_MODEL.Add(MModelToBeInserted);
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }
        public List<Mode> GetPrepModes(bool p)
        {

            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> Mode = new List<MESC1TS_MODE>();

            if (p == true)
            {
                Mode = (from nd in objContext.MESC1TS_PREPTIME
                        join ma in objContext.MESC1TS_MODE
                        on nd.MODE equals ma.MODE

                        orderby ma.MODE
                        select ma).Distinct().ToList();
            }
            else
            {
                Mode = (from mode in objContext.MESC1TS_MODE
                        select mode).ToList();
            }

            // return PrepareDataContract(Mode, true);
            return PrepareDataContract_Mode(Mode);

        }
        public List<PrepTime> GetPrepTimeDetails(string ModeCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_PREPTIME> MMmodel = new List<MESC1TS_PREPTIME>();
            MMmodel = (from pay in objContext.MESC1TS_PREPTIME
                       where pay.MODE == ModeCode
                       select pay).ToList();

            return PrepareDataContract_PrepTime(MMmodel);

        }
        private List<PrepTime> PrepareDataContract_PrepTime(List<MESC1TS_PREPTIME> modelFromDB)
        {
            List<PrepTime> modeList = new List<PrepTime>();
            for (int count = 0; count < modelFromDB.Count; count++)
            {
                PrepTime pt = new PrepTime();
                pt.ModeCode = modelFromDB[count].MODE;
                //  mode.ManufacturName = modeFromDB[count].MANUFCTR + '-' + modeFromDB[count].MANUFACTUR_NAME;
                pt.PrepCd = modelFromDB[count].PREP_CD;
                pt.PrepTimeMax = modelFromDB[count].PREP_TIME_MAX;
                pt.PrepHrs = modelFromDB[count].PREP_HRS;
                //pt.ChangeUser = (GetUserNamePTI(modelFromDB[count].CHUSER.ToString())).Replace("|", " ");
                pt.ChangeUser = (GetUserNamePTI(modelFromDB[count].CHUSER.ToString()));
                //pt.ChangeTime = modelFromDB[count].CHTS;
                pt.ChangeTime_Display = Convert.ToDateTime(modelFromDB[count].CHTS.ToString()).ToString("yyyy-MM-dd hh:mm:ss tt");
                modeList.Add(pt);
            }

            return modeList;
        }
        public string UpdatePrepTime(PrepTime PrepToBeUpdated)
        {
            objContext = new ManageMasterDataServiceEntities();
            string success = "";
            List<MESC1TS_PREPTIME> ModelObject = new List<MESC1TS_PREPTIME>();
            ModelObject = (from pay in objContext.MESC1TS_PREPTIME
                           where pay.MODE == PrepToBeUpdated.ModeCode
                            && pay.PREP_TIME_MAX == PrepToBeUpdated.PrepTimeMax
                           select pay).ToList();
            List<MESC1TS_REPAIR_CODE> MRep = new List<MESC1TS_REPAIR_CODE>();

            MRep = (from pay in objContext.MESC1TS_REPAIR_CODE
                    where pay.REPAIR_CD == PrepToBeUpdated.PrepCd
                    && pay.MODE == PrepToBeUpdated.ModeCode
                    select pay).ToList();

            if (MRep.Count == 0)
            {
                success = "Repair Code is not valid for selected Mode ";
                return success;
            }

            ModelObject[0].MODE = PrepToBeUpdated.ModeCode;
            ModelObject[0].PREP_TIME_MAX = PrepToBeUpdated.PrepTimeMax;
            ModelObject[0].PREP_HRS = PrepToBeUpdated.PrepHrs;
            ModelObject[0].PREP_CD = PrepToBeUpdated.PrepCd;
            ModelObject[0].CHUSER = PrepToBeUpdated.ChangeUser;
            ModelObject[0].CHTS = DateTime.Now;
            try
            {
                objContext.SaveChanges();
                success = "Modification Completed";
            }
            catch (Exception ex)
            {
                success = "Modification Failed";
            }

            return success;// PrepToBeUpdated;
        }

        public string CreatePrepTime(PrepTime preplToBeCreated)
        {
            string success = "";
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_PREPTIME> MDis = new List<MESC1TS_PREPTIME>();

            MDis = (from pay in objContext.MESC1TS_PREPTIME
                    where pay.MODE == preplToBeCreated.ModeCode
                    && pay.PREP_TIME_MAX == preplToBeCreated.PrepTimeMax
                    select pay).ToList();

            if (MDis.Count > 0)
            {
                success = "Duplicate Record Exist";
                return success;
            }

            List<MESC1TS_REPAIR_CODE> MRep = new List<MESC1TS_REPAIR_CODE>();

            MRep = (from pay in objContext.MESC1TS_REPAIR_CODE
                    where pay.REPAIR_CD == preplToBeCreated.PrepCd
                    && pay.MODE == preplToBeCreated.ModeCode
                    select pay).ToList();

            if (MRep.Count == 0)
            {
                success = "Repair Code is not valid for selected Mode ";
                return success;
            }

            MESC1TS_PREPTIME MModelToBeInserted = new MESC1TS_PREPTIME();
            MModelToBeInserted.MODE = preplToBeCreated.ModeCode;
            MModelToBeInserted.PREP_TIME_MAX = preplToBeCreated.PrepTimeMax;
            MModelToBeInserted.PREP_HRS = preplToBeCreated.PrepHrs;
            MModelToBeInserted.PREP_CD = preplToBeCreated.PrepCd;
            MModelToBeInserted.CHUSER = preplToBeCreated.ChangeUser;
            MModelToBeInserted.CHTS = DateTime.Now;





            objContext.MESC1TS_PREPTIME.Add(MModelToBeInserted);
            try
            {
                objContext.SaveChanges();
                success = "**PrepTimeMax: " + preplToBeCreated.PrepTimeMax.ToString() + " / " + preplToBeCreated.ModeCode.ToString() + " Added**";
            }
            catch (Exception ex)
            {
                success = "Prep Time / Already Exist - Not Added ";
            }

            return success;
        }

        public bool DeletePrepTime(string ModeCode, double PreptimeDigit)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_PREPTIME> MDiscountObject = new List<MESC1TS_PREPTIME>();
            MDiscountObject = (from pay in objContext.MESC1TS_PREPTIME
                               where pay.MODE == ModeCode
                               && pay.PREP_TIME_MAX == PreptimeDigit
                               select pay).ToList();
            try
            {
                objContext.MESC1TS_PREPTIME.Remove(MDiscountObject.First());
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        //----------------labor Rate-----------------
        #region GetCustomerList
        public List<Customer> GetCustomerList()
        {
            List<MESC1TS_CUSTOMER> CustomerListFromDB = new List<MESC1TS_CUSTOMER>();
            List<Customer> CustomerList = new List<Customer>();

            try
            {
                CustomerListFromDB = (from customer in objContext.MESC1TS_CUSTOMER
                                      where customer.CUSTOMER_ACTIVE_SW == "Y"
                                      orderby customer.CUSTOMER_CD
                                      select customer).ToList();


                for (int count = 0; count < CustomerListFromDB.Count; count++)
                {
                    Customer Customer = new Customer();
                    Customer.CustomerCode = CustomerListFromDB[count].CUSTOMER_CD;
                    Customer.CustomerDesc = CustomerListFromDB[count].CUSTOMER_DESC;
                    CustomerList.Add(Customer);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return CustomerList;
        }
        #endregion GetCustomerList

        #region GetShopList
        public List<Shop> GetShopList()
        {
            List<MESC1TS_SHOP> ShopListFromDB = new List<MESC1TS_SHOP>();
            List<Shop> ShopList = new List<Shop>();
            try
            {

                ShopListFromDB = (from s in objContext.MESC1TS_SHOP

                                  select s).ToList();


                for (int count = 0; count < ShopListFromDB.Count; count++)
                {
                    Shop Shop = new Shop();
                    Shop.ShopCode = ShopListFromDB[count].SHOP_CD;
                    Shop.ShopDescription = ShopListFromDB[count].SHOP_DESC;// ShopListFromDB[count].CUCDN;
                    Shop.CUCDN = ShopListFromDB[count].CUCDN;
                    ShopList.Add(Shop);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return ShopList;
        }
        public List<Shop> GetNonActiveShopList()
        {

            List<MESC1TS_SHOP> ShopListFromDB = new List<MESC1TS_SHOP>();
            List<Shop> ShopList = new List<Shop>();
            try
            {
                #region Get and Prepare Shop List
                ShopListFromDB = (from S in objContext.MESC1TS_SHOP

                                  where S.SHOP_ACTIVE_SW == "N"
                                  orderby S.SHOP_CD
                                  select S).Distinct().ToList();
                #endregion Get and Prepare Shop List

                for (int count = 0; count < ShopListFromDB.Count; count++)
                {
                    Shop Shop = new Shop();
                    Shop.ShopCode = ShopListFromDB[count].SHOP_CD;
                    Shop.ShopDescription = ShopListFromDB[count].SHOP_DESC;
                    Shop.CUCDN = ShopListFromDB[count].CUCDN;
                    Shop.ShopActiveSW = ShopListFromDB[count].SHOP_ACTIVE_SW;
                    ShopList.Add(Shop);
                }
            }
            catch (Exception ex)
            {
                //logEntry.Message = ex.ToString();
                //Logger.Write(logEntry);
            }
            return ShopList;
        }

        public List<Shop> GetUserShopList()
        {
            int UserID = 10022; //shop
            List<MESC1TS_SHOP> ShopListFromDB = new List<MESC1TS_SHOP>();
            List<Shop> ShopList = new List<Shop>();
            try
            {
                #region Get and Prepare Shop List
                ShopListFromDB = (from S in objContext.MESC1TS_SHOP
                                  //join G in objContext.SEC_AUTHGROUP_USER on S.SHOP_CD equals G.COLUMN_VALUE
                                  //join A in objContext.SEC_AUTHGROUP on G.AUTHGROUP_ID equals A.AUTHGROUP_ID
                                  //where 
                                  //G.USER_ID == UserID &&
                                  //  S.SHOP_ACTIVE_SW == "Y"
                                  orderby S.SHOP_CD
                                  select S).Distinct().ToList();
                #endregion Get and Prepare Shop List

                for (int count = 0; count < ShopListFromDB.Count; count++)
                {
                    Shop Shop = new Shop();
                    Shop.ShopCode = ShopListFromDB[count].SHOP_CD;
                    Shop.ShopDescription = ShopListFromDB[count].SHOP_DESC;
                    Shop.CUCDN = ShopListFromDB[count].CUCDN;
                    Shop.ShopActiveSW = ShopListFromDB[count].SHOP_ACTIVE_SW;
                    ShopList.Add(Shop);
                }
            }
            catch (Exception ex)
            {
                //logEntry.Message = ex.ToString();
                //Logger.Write(logEntry);
            }
            return ShopList;
        }
        #endregion GetShopList

        public List<EqType> GetEquipmentList()
        {
            List<MESC1TS_EQTYPE> EqpListFromDB = new List<MESC1TS_EQTYPE>();
            List<EqType> EqpList = new List<EqType>();
            try
            {


                EqpListFromDB = (from s in objContext.MESC1TS_EQTYPE

                                 select s).ToList();



                for (int count = 0; count < EqpListFromDB.Count; count++)
                {
                    EqType Ep = new EqType();
                    Ep.EqpType = EqpListFromDB[count].EQTYPE;
                    Ep.EqTypeDesc = EqpListFromDB[count].EQTYPE_DESC;
                    EqpList.Add(Ep);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return EqpList;
        }

        public List<LaborRate> GetLaborRateDetail(string ShopCode, String CustCode, String eqtypeCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_LABOR_RATE> MLrate = new List<MESC1TS_LABOR_RATE>();
            string b = string.Empty;
            //MLrate = (from pay in objContext.MESC1TS_LABOR_RATE
            //          where pay.SHOP_CD == ShopCode
            //          && pay.CUSTOMER_CD == CustCode
            //          && pay.EQTYPE == eqtypeCode
            //          select pay).ToList();

            MLrate = (from pay in objContext.MESC1TS_LABOR_RATE
                      where 1 == 1
                       && (ShopCode == null ? b == b : pay.SHOP_CD == ShopCode)
                       && (CustCode == null ? b == b : pay.CUSTOMER_CD == CustCode)
                       && (eqtypeCode == null ? b == b : pay.EQTYPE == eqtypeCode)

                      select pay).ToList();

            return PrepareDataContract_LaborRate(MLrate);

        }
        private List<LaborRate> PrepareDataContract_LaborRate(List<MESC1TS_LABOR_RATE> LrateFromDB)
        {
            List<LaborRate> LaborList = new List<LaborRate>();
            for (int count = 0; count < LrateFromDB.Count; count++)
            {
                LaborRate mode = new LaborRate();
                mode.LaborRateID = LrateFromDB[count].LABOR_RATE_ID;

                mode.ShopCode = LrateFromDB[count].SHOP_CD;
                mode.CustomerCode = LrateFromDB[count].CUSTOMER_CD;
                mode.EqpType = LrateFromDB[count].EQTYPE;
                mode.EffDate = LrateFromDB[count].EFF_DTE;
                mode.ExpDate = LrateFromDB[count].EXP_DTE;
                mode.RegularRT = LrateFromDB[count].REGULAR_RT;
                mode.OvertimeRT = LrateFromDB[count].OVERTIME_RT;
                mode.DoubleTimeRT = LrateFromDB[count].DOUBLETIME_RT;
                mode.MiscRT = LrateFromDB[count].MISC_RT;
                //mode.ChangeUser = (GetUserNamePTI(LrateFromDB[count].CHUSER.ToString())).Replace("|", " ");
                mode.ChangeUser = LrateFromDB[count].CHUSER;
                mode.ChangeTime = LrateFromDB[count].CHTS;
                LaborList.Add(mode);
            }
            return LaborList;
        }


        public string CreateLaborRate(LaborRate LRateToBeCreated)
        {
            string Result = "NO";

            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_LABOR_RATE> MLrate = new List<MESC1TS_LABOR_RATE>();
            MLrate = (from pay in objContext.MESC1TS_LABOR_RATE
                      where pay.SHOP_CD == LRateToBeCreated.ShopCode
                      && pay.CUSTOMER_CD == LRateToBeCreated.CustomerCode
                      && pay.EQTYPE == LRateToBeCreated.EqpType
                      && pay.EXP_DTE > LRateToBeCreated.EffDate
                      select pay).ToList();

            if (MLrate.Count > 0)
            {
                Result = "Duplicate";
            }
            else
            {
                // objContext = new ManageMasterDataServiceEntities();
                MESC1TS_LABOR_RATE LaborRtaeToBeInserted = new MESC1TS_LABOR_RATE();
                LaborRtaeToBeInserted.SHOP_CD = LRateToBeCreated.ShopCode;
                LaborRtaeToBeInserted.CUSTOMER_CD = LRateToBeCreated.CustomerCode;
                LaborRtaeToBeInserted.EQTYPE = LRateToBeCreated.EqpType;
                LaborRtaeToBeInserted.EFF_DTE = LRateToBeCreated.EffDate;
                //LaborRtaeToBeInserted.EXP_DTE = LRateToBeCreated.ExpDate;
                LaborRtaeToBeInserted.EXP_DTE = LRateToBeCreated.ExpDate.AddHours(23).AddMinutes(59);//Kasturee_labor_rate_08-03-19
                LaborRtaeToBeInserted.REGULAR_RT = LRateToBeCreated.RegularRT;
                LaborRtaeToBeInserted.OVERTIME_RT = LRateToBeCreated.OvertimeRT;
                LaborRtaeToBeInserted.DOUBLETIME_RT = LRateToBeCreated.DoubleTimeRT;
                LaborRtaeToBeInserted.MISC_RT = LRateToBeCreated.MiscRT;
                LaborRtaeToBeInserted.CHUSER = LRateToBeCreated.ChangeUser;
                LaborRtaeToBeInserted.CHTS = DateTime.Now;
                objContext.MESC1TS_LABOR_RATE.Add(LaborRtaeToBeInserted);
                try
                {
                    objContext.SaveChanges();
                    Result = "SUCCESS";
                }
                catch (Exception ex)
                {
                    Result = "FAILED";
                }
            }
            return Result;
        }

        public List<LaborRate> GetLaborRateEditDetail(int LID)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_LABOR_RATE> MLrate = new List<MESC1TS_LABOR_RATE>();
            string b = string.Empty;


            MLrate = (from pay in objContext.MESC1TS_LABOR_RATE

                      where pay.LABOR_RATE_ID == LID
                      select pay).ToList();

            return PrepareDataContract_LaborRate(MLrate);

        }

        public string ModifyLaborRate(LaborRate LRateToBeCreated)
        {
            string Result = "NO";
            int flag = 0;
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_LABOR_RATE> ExistLabourRate = new List<MESC1TS_LABOR_RATE>();
            ExistLabourRate = (from lr in objContext.MESC1TS_LABOR_RATE
                               where lr.SHOP_CD == LRateToBeCreated.ShopCode
                               && lr.CUSTOMER_CD == LRateToBeCreated.CustomerCode
                               && lr.EQTYPE == LRateToBeCreated.EqpType
                               && lr.LABOR_RATE_ID != LRateToBeCreated.LaborRateID
                               select lr).ToList();

            foreach (var obj in ExistLabourRate)
            {
                if ((LRateToBeCreated.EffDate >= obj.EFF_DTE && LRateToBeCreated.EffDate <= obj.EXP_DTE) || (LRateToBeCreated.ExpDate >= obj.EFF_DTE && LRateToBeCreated.ExpDate <= obj.EXP_DTE))
                {
                    flag = 1;
                }
            }

            if (flag != 0)
            {
                Result = "DUPLICATE";
            }
            else
            {

                List<MESC1TS_LABOR_RATE> MLrate = new List<MESC1TS_LABOR_RATE>();
                MLrate = (from pay in objContext.MESC1TS_LABOR_RATE
                          where pay.LABOR_RATE_ID == LRateToBeCreated.LaborRateID

                          select pay).ToList();


                var OLdvertimeRT = MLrate[0].OVERTIME_RT.Value.ToString();
                var OldRegularRT = MLrate[0].REGULAR_RT.Value.ToString();
                var OldDoubleTimeRT = MLrate[0].DOUBLETIME_RT.Value.ToString();
                var OldMiscRT = MLrate[0].MISC_RT.Value.ToString();
                MLrate[0].EFF_DTE = LRateToBeCreated.EffDate;
                //MLrate[0].EXP_DTE = LRateToBeCreated.ExpDate;
                MLrate[0].EXP_DTE = LRateToBeCreated.ExpDate.AddHours(23).AddMinutes(59);//Kasturee_labor_rate_08-03-19
                MLrate[0].REGULAR_RT = LRateToBeCreated.RegularRT;
                MLrate[0].OVERTIME_RT = LRateToBeCreated.OvertimeRT;
                MLrate[0].DOUBLETIME_RT = LRateToBeCreated.DoubleTimeRT;
                MLrate[0].MISC_RT = LRateToBeCreated.MiscRT;
                MLrate[0].CHUSER = LRateToBeCreated.ChangeUser;
                MLrate[0].CHTS = DateTime.Now;
                //objContext.MESC1TS_LABOR_RATE.Add(LaborRtaeToBeInserted);
                try
                {
                    objContext.SaveChanges();
                    Result = "SUCCESS";
                    bool a = false;
                    //decimal firstVal; decimal secVal;
                    decimal out_firstVal = 0; decimal out_secVal = 1;
                    decimal.TryParse(LRateToBeCreated.OvertimeRT.Value.ToString(), out out_firstVal);
                    decimal.TryParse(OLdvertimeRT, out out_secVal);
                    var ChangeUser = LRateToBeCreated.ChangeUser;
                    //var ChangeUser1 = GetUserNamePTI(LRateToBeCreated.ChangeUser);
                    //if (ChangeUser1.Contains('|'))
                    //{
                    //    ChangeUser = ChangeUser1.Split('|')[0];
                    //}
                    if (LRateToBeCreated.OvertimeRT.Value.ToString() != OLdvertimeRT && out_firstVal != out_secVal)
                    {
                        a = EditAuditLogData("MESC1TS_LABOR_RATE", LRateToBeCreated.LaborRateID.ToString(), OLdvertimeRT, LRateToBeCreated.OvertimeRT.Value.ToString(), ChangeUser, "OVERTIME_RT", DateTime.Now.ToString());
                    }
                    decimal.TryParse(LRateToBeCreated.RegularRT.Value.ToString(), out out_firstVal);
                    decimal.TryParse(OldRegularRT, out out_secVal);

                    if (LRateToBeCreated.RegularRT.Value.ToString() != OldRegularRT && out_firstVal != out_secVal)
                    {
                        a = EditAuditLogData("MESC1TS_LABOR_RATE", LRateToBeCreated.LaborRateID.ToString(), OldRegularRT, LRateToBeCreated.RegularRT.Value.ToString(), ChangeUser, "REGULAR_RT", DateTime.Now.ToString());
                    }
                    decimal.TryParse(LRateToBeCreated.DoubleTimeRT.Value.ToString(), out out_firstVal);
                    decimal.TryParse(OldDoubleTimeRT, out out_secVal);

                    if (LRateToBeCreated.DoubleTimeRT.Value.ToString() != OldDoubleTimeRT && out_firstVal != out_secVal)
                    {
                        a = EditAuditLogData("MESC1TS_LABOR_RATE", LRateToBeCreated.LaborRateID.ToString(), OldDoubleTimeRT, LRateToBeCreated.DoubleTimeRT.Value.ToString(), ChangeUser, "DOUBLETIME_RT", DateTime.Now.ToString());
                    }

                    decimal.TryParse(LRateToBeCreated.MiscRT.Value.ToString(), out out_firstVal);
                    decimal.TryParse(OldMiscRT, out out_secVal);


                    if (LRateToBeCreated.MiscRT.Value.ToString() != OldMiscRT && out_firstVal != out_secVal)
                    {
                        a = EditAuditLogData("MESC1TS_LABOR_RATE", LRateToBeCreated.LaborRateID.ToString(), OldMiscRT, LRateToBeCreated.MiscRT.Value.ToString(), ChangeUser, "MISC_RT", DateTime.Now.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Result = "FAILED";
                }
            }
            return Result;
        } 

        public List<RefAudit> GetAuditTrailLabourRate(string RecordId, string TableName)
        {
            List<RefAudit> RefAuditList = new List<RefAudit>();
            try
            {

                dynamic Query = "";
                objContext = new ManageMasterDataServiceEntities();

                if (TableName == "MESC1TS_LABOR_RATE")
                {
                    Query = (from MR in objContext.MESC1TS_REFAUDIT
                             from U in objContext.SEC_USER
                                       .Where(U => U.LOGIN == MR.CHUSER)
                                       .DefaultIfEmpty()
                             where MR.TAB_NAME == "MESC1TS_LABOR_RATE" && MR.UNIQUE_ID == RecordId
                             select new { MR.TAB_NAME, MR.UNIQUE_ID, MR.COL_NAME, MR.OLD_VALUE, MR.NEW_VALUE, MR.CHUSER, U.LASTNAME, U.FIRSTNAME, MR.CHTS }).ToList();

                }
                foreach (var obj in Query)
                {
                    RefAudit objRef = new RefAudit();
                    objRef.TabName = obj.TAB_NAME;
                    objRef.UniqueID = obj.UNIQUE_ID;
                    objRef.ColName = obj.COL_NAME;
                    objRef.OldValue = obj.OLD_VALUE;
                    objRef.NewValue = obj.NEW_VALUE;
                    objRef.ChangeUser = obj.CHUSER;
                    objRef.FirstName = obj.FIRSTNAME;
                    objRef.LastName = obj.LASTNAME;
                    objRef.ChangeTime = obj.CHTS;
                    RefAuditList.Add(objRef);
                };
                RefAuditList = RefAuditList.OrderByDescending(m => m.ChangeTime).ToList();
            }

            catch (Exception ex)
            {
                string ss = ex.Message;

            }
            return RefAuditList;
        }


        //------------------------- End pinaki

        #region Amlan - Master Parts

        public List<RefAudit> GetAuditTrail_MasterParts(string RecordId, string TableName)
        {
            List<RefAudit> RefAuditList = new List<RefAudit>();
            try
            {

                dynamic Query = "";
                objContext = new ManageMasterDataServiceEntities();

                if (TableName == "MESC1TS_MASTER_PART")
                {
                    Query = (from MR in objContext.MESC1TS_REFAUDIT
                             from U in objContext.SEC_USER
                                       .Where(U => U.LOGIN == MR.CHUSER)
                                       .DefaultIfEmpty()
                             where MR.TAB_NAME == "MESC1TS_MASTER_PART" && MR.UNIQUE_ID == RecordId
                             select new { MR.TAB_NAME, MR.UNIQUE_ID, MR.COL_NAME, MR.OLD_VALUE, MR.NEW_VALUE, MR.CHUSER, U.LASTNAME, U.FIRSTNAME, MR.CHTS }).ToList();

                }
                foreach (var obj in Query)
                {
                    RefAudit objRef = new RefAudit();
                    objRef.TabName = obj.TAB_NAME;
                    objRef.UniqueID = obj.UNIQUE_ID;
                    objRef.ColName = obj.COL_NAME;
                    objRef.OldValue = obj.OLD_VALUE;
                    objRef.NewValue = obj.NEW_VALUE;
                    objRef.ChangeUser = obj.CHUSER;
                    objRef.FirstName = obj.FIRSTNAME;
                    objRef.LastName = obj.LASTNAME;
                    objRef.ChangeTime = obj.CHTS;
                    RefAuditList.Add(objRef);
                };
                RefAuditList.OrderByDescending(m => m.ChangeTime);
            }

            catch (Exception ex)
            {
                string ss = ex.Message;

            }
            return RefAuditList;
        }

        private bool EditAuditLogData(string tableName, string partNumber, string oldValue, string newValue,
         string user, string colName, string time)
        {
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_REFAUDIT auditToBeInserted = new MESC1TS_REFAUDIT();

            auditToBeInserted.UNIQUE_ID = partNumber;
            auditToBeInserted.TAB_NAME = tableName;
            auditToBeInserted.NEW_VALUE = newValue;
            auditToBeInserted.OLD_VALUE = oldValue;
            auditToBeInserted.COL_NAME = colName;
            auditToBeInserted.CHTS = DateTime.Parse(time);
            auditToBeInserted.CHUSER = user;

            objContext.MESC1TS_REFAUDIT.Add(auditToBeInserted);

            try
            {
                objContext.SaveChanges();

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;

        }

        public List<PartsGroup> GetAllPartsGroups()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_PARTS_GROUP> partsGroupList = new List<MESC1TS_PARTS_GROUP>();
            partsGroupList = (from partsGroup in objContext.MESC1TS_PARTS_GROUP
                              orderby partsGroup.PARTS_GROUP_CD
                              select partsGroup).ToList();

            return PrepareDataContract(partsGroupList);
        }

        private List<PartsGroup> PrepareDataContract(List<MESC1TS_PARTS_GROUP> partsGroupListFromDB)
        {
            List<PartsGroup> partsGroupList = new List<PartsGroup>();
            for (int count = 0; count < partsGroupListFromDB.Count; count++)
            {
                PartsGroup partsGroup = new PartsGroup();
                partsGroup.PartsGroupCd = partsGroupListFromDB[count].PARTS_GROUP_CD;
                //partsGroup.PartsGroupActiveSW = partsGroupListFromDB[count].PARTS_GROUP_ACTIVE_SW;
                partsGroup.PartsGroupDesc = partsGroupListFromDB[count].PARTS_GROUP_DESC;
                //partsGroup.Remarks = partsGroupListFromDB[count].REMARKS;
                //partsGroup.ChangeUser = partsGroupListFromDB[count].CHUSER;
                //partsGroup.ChangeTime = partsGroupListFromDB[count].CHTS;
                partsGroupList.Add(partsGroup);

            }
            return partsGroupList;
        }


        public List<Manufactur> GetAllManufacturers()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUFACTUR> manufacturerList = new List<MESC1TS_MANUFACTUR>();
            manufacturerList = (from manufacturer in objContext.MESC1TS_MANUFACTUR
                                orderby manufacturer.MANUFCTR
                                select manufacturer).ToList();
            return PrepareDataContract(manufacturerList);

        }

        private List<Manufactur> PrepareDataContract(List<MESC1TS_MANUFACTUR> manufacturerListFromDB)
        {
            List<Manufactur> manufacturerList = new List<Manufactur>();
            for (int count = 0; count < manufacturerListFromDB.Count; count++)
            {
                Manufactur manufacturer = new Manufactur();
                manufacturer.ManufacturCd = manufacturerListFromDB[count].MANUFCTR;
                //manufacturer.DiscountPercent = manufacturerListFromDB[count].DISCOUNT_PERCENT;
                manufacturer.ManufacturName = manufacturerListFromDB[count].MANUFACTUR_NAME;
                //manufacturer.Model = manufacturerListFromDB[count].MESC1TS_MODEL;

                manufacturerList.Add(manufacturer);

            }
            return manufacturerList;


        }

        private bool IsMasterPartExist(string partCode)
        {
            bool isPartExist = false;
            objContext = new ManageMasterDataServiceEntities();
            try
            {
                List<MESC1TS_MASTER_PART> partDBObject = new List<MESC1TS_MASTER_PART>();
                partDBObject = (from masterPart in objContext.MESC1TS_MASTER_PART
                                where masterPart.PART_CD == partCode
                                select masterPart).ToList();

                if (partDBObject.Count > 0)
                {
                    isPartExist = true;
                }
            }
            catch (Exception e)
            {

            }

            return isPartExist;
        }

        public MasterPart AddMasterPart(MasterPart masterPartFromClient)
        {
            bool isPartExist = false;
            MasterPart masterPartResult = new MasterPart();


            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_MASTER_PART masterPartToBeInserted = new MESC1TS_MASTER_PART();

            masterPartResult.IsMasterPartExist = IsMasterPartExist(masterPartFromClient.PartCd);
            masterPartResult.PartCd = masterPartFromClient.PartCd;

            if (masterPartResult.IsMasterPartExist)
            {

                return masterPartResult; // do not go for save
            }

            /// saving data

            masterPartToBeInserted.PART_CD = masterPartFromClient.PartCd;
            masterPartToBeInserted.PARTS_GROUP_CD = masterPartFromClient.PartsGroupCd;
            masterPartToBeInserted.MANUFCTR = masterPartFromClient.Manufactur;
            masterPartToBeInserted.PART_DESIGNATION_1 = masterPartFromClient.PartDesignation1;
            masterPartToBeInserted.PART_DESIGNATION_2 = masterPartFromClient.PartDesignation2;
            masterPartToBeInserted.PART_DESIGNATION_3 = masterPartFromClient.PartDesignation3;

            masterPartToBeInserted.QUANTITY = masterPartFromClient.Quantity;
            masterPartToBeInserted.PART_DESC = masterPartFromClient.PartDesc;
            masterPartToBeInserted.PART_PRICE = masterPartFromClient.PartPrice;
            masterPartToBeInserted.CORE_VALUE = masterPartFromClient.CoreValue;

            masterPartToBeInserted.DEDUCT_CORE = masterPartFromClient.DeductCoreSW;
            masterPartToBeInserted.MSL_PART_SW = masterPartFromClient.MslPartSW;
            masterPartToBeInserted.PART_ACTIVE_SW = masterPartFromClient.PartActiveSW;
            masterPartToBeInserted.CORE_PART_SW = masterPartFromClient.SerialTagSW;

            masterPartToBeInserted.REMARKS = masterPartFromClient.Remarks;
            masterPartToBeInserted.CHTS = DateTime.Now;
            masterPartToBeInserted.CHUSER = masterPartFromClient.ChangeUser;

            objContext.MESC1TS_MASTER_PART.Add(masterPartToBeInserted);

            try
            {
                objContext.SaveChanges();

                masterPartResult.IsMasterPartAddSuccess = true;
            }
            catch (Exception ex)
            {
                masterPartResult.IsMasterPartAddSuccess = false;
            }

            return masterPartResult;
        }


        public List<MasterPart> GetMasterPartsByQuery(string partGroupCode, string manufacturerCode, string designation,
            string partNumber, string description, string isActive, string isCore, string isDeductCoreValue)
        {


            string partNumber_SW = string.Empty;
            string partNumber_EW = string.Empty;
            string partNumber_SW1 = string.Empty;
            string partNumber_EW1 = string.Empty;
            string partNumber_CON = string.Empty;
            string partNumber_AS_IS = string.Empty;

            List<MasterPart> masterPartList = new List<MasterPart>();

            /*
           aa*bb   = StartsWith(aa) and EndsWith(bb)
           *aa = EndsWith(aa)
           aa* = StartsWith(aa)
           *101* = Contains(01)            
            aa*01*bb = pass as is
           * - all 
           */

            int firstIndx;
            int lastIndx;
            int partLen;

            designation = string.IsNullOrEmpty(designation) ? designation : designation.Trim();
            description = string.IsNullOrEmpty(description) ? description : description.Trim();
            isActive = string.IsNullOrEmpty(isActive) ? isActive : isActive.Trim();
            isCore = string.IsNullOrEmpty(isCore) ? isCore : isCore.Trim();
            isDeductCoreValue = string.IsNullOrEmpty(isDeductCoreValue) ? isDeductCoreValue : isDeductCoreValue.Trim();

            try
            {
                if (!string.IsNullOrEmpty(partNumber))
                {
                    partNumber = partNumber.Trim();
                    if (partNumber.Contains("*"))
                    {
                        firstIndx = partNumber.IndexOf('*');
                        lastIndx = partNumber.LastIndexOf('*');
                        partLen = partNumber.Length;

                        if (firstIndx == lastIndx) // only one * is present
                        {
                            if (firstIndx == 0)
                            {
                                // *ab
                                partNumber_EW = partNumber.Substring(firstIndx + 1);
                            }
                            if (firstIndx > 0 && firstIndx == partLen - 1)
                            {
                                //abc*
                                partNumber_SW = partNumber.Substring(0, firstIndx);
                            }
                            if (firstIndx > 0 && firstIndx < partLen - 1)
                            {
                                //aac*bbcd , aa*bb
                                partNumber_SW1 = partNumber.Substring(0, firstIndx); // aac, aa
                                partNumber_EW1 = partNumber.Substring(firstIndx + 1, partLen - firstIndx - 1); // bbcd , bb
                            }
                        }
                        else // more than one * is present
                        //*101* = Contains(101)            
                        //aa*01*bb = pass as is
                        {

                            if (firstIndx == 0 && lastIndx == partLen - 1) //*101* 
                            {
                                partNumber_CON = partNumber.Replace("*", "");
                                partNumber_CON = partNumber_CON.Trim(); // 101
                            }
                            else
                            {
                                partNumber_AS_IS = partNumber; //aa*01*bb = pass as is
                            }
                        }
                    }
                    else
                    {
                        partNumber_AS_IS = partNumber;
                    }
                }

                objContext = new ManageMasterDataServiceEntities();


                List<MESC1TS_MASTER_PART> partListFromDB = new List<MESC1TS_MASTER_PART>();
                partListFromDB = (from masterPart in objContext.MESC1TS_MASTER_PART
                                  where
    masterPart.PARTS_GROUP_CD == (string.IsNullOrEmpty(partGroupCode) ? masterPart.PARTS_GROUP_CD : partGroupCode.Trim())
    &&
    masterPart.MANUFCTR == (string.IsNullOrEmpty(manufacturerCode) ? masterPart.MANUFCTR : manufacturerCode.Trim())
    &&
    ( // PART_DESIGNATION_1, PART_DESIGNATION_2, PART_DESIGNATION_3 Can be NULL
    (masterPart.PART_DESIGNATION_1.Contains(designation) || designation == null)
    ||
    (masterPart.PART_DESIGNATION_2.Contains(designation) || designation == null)
    ||
    (masterPart.PART_DESIGNATION_3.Contains(designation) || designation == null)
    )
    &&
                                      //abcd
    masterPart.PART_CD == (string.IsNullOrEmpty(partNumber_AS_IS) ? masterPart.PART_CD : partNumber_AS_IS.Trim())
    &&
                                      //*101*
    masterPart.PART_CD.Contains((string.IsNullOrEmpty(partNumber_CON) ? masterPart.PART_CD : partNumber_CON.Trim()))
    &&
                                      //aa*
    masterPart.PART_CD.StartsWith((string.IsNullOrEmpty(partNumber_SW) ? masterPart.PART_CD : partNumber_SW.Trim()))
    &&
                                      ////*aa
    masterPart.PART_CD.EndsWith((string.IsNullOrEmpty(partNumber_EW) ? masterPart.PART_CD : partNumber_EW.Trim()))
    &&
                                      ////aa*bb
    masterPart.PART_CD.StartsWith((string.IsNullOrEmpty(partNumber_SW1) ? masterPart.PART_CD : partNumber_SW1.Trim()))
    &&
    masterPart.PART_CD.EndsWith((string.IsNullOrEmpty(partNumber_EW1) ? masterPart.PART_CD : partNumber_EW1.Trim()))
    &&

    (masterPart.PART_DESC.Contains(description) || description == null)
    &&
    (masterPart.PART_ACTIVE_SW == isActive || isActive == null)
    &&
    (masterPart.CORE_PART_SW == isCore || isCore == null)
    &&
    (masterPart.DEDUCT_CORE == isDeductCoreValue || isDeductCoreValue == null)


                                  orderby masterPart.PARTS_GROUP_CD, masterPart.PART_CD ascending

                                  select masterPart).Take(100).ToList();


                masterPartList = PrepareDataContract(partListFromDB);

            }
            catch (Exception e)
            {
                logEntry.Message = e.ToString();
                Logger.Write(logEntry);
            }

            return masterPartList;
        }

        public List<MasterPart> GetMasterPartByPartCode(string partCode)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_MASTER_PART> partListFromDB = new List<MESC1TS_MASTER_PART>();

            dynamic partListFromDBquery = "";
            partListFromDBquery = (from P in objContext.MESC1TS_MASTER_PART
                                   from U in objContext.SEC_USER
                                             .Where(U => U.LOGIN == P.CHUSER)
                                             .DefaultIfEmpty()
                                   where P.PART_CD == partCode.Trim()
                                   select new { P.PART_CD, P.QUANTITY, P.PART_PRICE, P.PART_DESC, P.PARTS_GROUP_CD, P.PART_DESIGNATION_1, P.PART_DESIGNATION_2, P.PART_DESIGNATION_3, P.MANUFCTR, P.PART_ACTIVE_SW, P.CORE_VALUE, P.DEDUCT_CORE, P.CORE_PART_SW, P.MSL_PART_SW, P.REMARKS, U.LASTNAME, U.FIRSTNAME, P.CHTS }).ToList();

            if (partListFromDBquery.Count == 0) return new List<MasterPart>();

            partListFromDBquery = partListFromDBquery[0];
            MasterPart masterPart = new MasterPart();

            masterPart.PartCd = partListFromDBquery.PART_CD;
            masterPart.PartsGroupCd = partListFromDBquery.PARTS_GROUP_CD;
            masterPart.Manufactur = partListFromDBquery.MANUFCTR;
            masterPart.PartDesc = partListFromDBquery.PART_DESC;

            masterPart.PartDesignation1 = partListFromDBquery.PART_DESIGNATION_1;
            masterPart.PartDesignation2 = partListFromDBquery.PART_DESIGNATION_2;
            masterPart.PartDesignation3 = partListFromDBquery.PART_DESIGNATION_3;

            masterPart.CoreValue = partListFromDBquery.CORE_VALUE;
            masterPart.PartPrice = partListFromDBquery.PART_PRICE;
            masterPart.Quantity = partListFromDBquery.QUANTITY;

            masterPart.CoreValueSW = partListFromDBquery.CORE_PART_SW;
            masterPart.DeductCoreSW = partListFromDBquery.DEDUCT_CORE;
            masterPart.PartActiveSW = partListFromDBquery.PART_ACTIVE_SW;
            masterPart.SerialTagSW = partListFromDBquery.CORE_PART_SW;
            masterPart.MslPartSW = partListFromDBquery.MSL_PART_SW;

            masterPart.Remarks = partListFromDBquery.REMARKS;
            masterPart.ChangeTime = partListFromDBquery.CHTS;
            masterPart.ChangeUser = (partListFromDBquery.FIRSTNAME == null ? "" : partListFromDBquery.FIRSTNAME) + " " + (partListFromDBquery.LASTNAME == null ? "" : partListFromDBquery.LASTNAME);


            List<MasterPart> masterPartList = new List<MasterPart>();
            masterPartList.Add(masterPart);

            return masterPartList;

        }


        private List<MasterPart> PrepareDataContract(List<MESC1TS_MASTER_PART> masterPartsListFromDB)
        {
            List<MasterPart> masterpartsList = new List<MasterPart>();
            MasterPart masterPart;
            for (int count = 0; count < masterPartsListFromDB.Count; count++)
            {
                masterPart = new MasterPart();

                masterPart.PartCd = masterPartsListFromDB[count].PART_CD;
                masterPart.PartsGroupCd = masterPartsListFromDB[count].PARTS_GROUP_CD;
                masterPart.Manufactur = masterPartsListFromDB[count].MANUFCTR;
                masterPart.PartDesc = masterPartsListFromDB[count].PART_DESC;

                masterPart.PartDesignation1 = masterPartsListFromDB[count].PART_DESIGNATION_1;
                masterPart.PartDesignation2 = masterPartsListFromDB[count].PART_DESIGNATION_2;
                masterPart.PartDesignation3 = masterPartsListFromDB[count].PART_DESIGNATION_3;

                masterPart.CoreValue = masterPartsListFromDB[count].CORE_VALUE;
                masterPart.PartPrice = masterPartsListFromDB[count].PART_PRICE;
                masterPart.Quantity = masterPartsListFromDB[count].QUANTITY;

                masterPart.CoreValueSW = masterPartsListFromDB[count].CORE_PART_SW;
                masterPart.DeductCoreSW = masterPartsListFromDB[count].DEDUCT_CORE;
                masterPart.PartActiveSW = masterPartsListFromDB[count].PART_ACTIVE_SW;
                masterPart.SerialTagSW = masterPartsListFromDB[count].CORE_PART_SW;
                masterPart.MslPartSW = masterPartsListFromDB[count].MSL_PART_SW;

                masterPart.Remarks = masterPartsListFromDB[count].REMARKS;
                masterPart.ChangeTime = masterPartsListFromDB[count].CHTS;
                masterPart.ChangeUser = masterPartsListFromDB[count].CHUSER;

                masterpartsList.Add(masterPart);

            }
            return masterpartsList;
        }

        public MasterPart UpdateMasterPart(MasterPart masterPartFromClient)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MASTER_PART> masterPartDBObject = new List<MESC1TS_MASTER_PART>();
            MasterPart mastPart = new MasterPart();
            mastPart.PartCd = masterPartFromClient.PartCd;
            decimal oldPrice = 0;
            decimal newPrice = 0;


            if (IsMasterPartExist(masterPartFromClient.PartCd))
            {

                masterPartDBObject = (from masterPart in objContext.MESC1TS_MASTER_PART
                                      where masterPart.PART_CD == masterPartFromClient.PartCd
                                      select masterPart).ToList();
                try
                {
                    masterPartDBObject[0].PARTS_GROUP_CD = masterPartFromClient.PartsGroupCd;
                    masterPartDBObject[0].MANUFCTR = masterPartFromClient.Manufactur;

                    masterPartDBObject[0].PART_DESIGNATION_1 = masterPartFromClient.PartDesignation1;
                    masterPartDBObject[0].PART_DESIGNATION_2 = masterPartFromClient.PartDesignation2;
                    masterPartDBObject[0].PART_DESIGNATION_3 = masterPartFromClient.PartDesignation3;

                    masterPartDBObject[0].PART_DESC = masterPartFromClient.PartDesc;

                    masterPartDBObject[0].QUANTITY = masterPartFromClient.Quantity;

                    newPrice = masterPartFromClient.PartPrice.Value;
                    oldPrice = masterPartDBObject[0].PART_PRICE.Value;

                    masterPartDBObject[0].PART_PRICE = masterPartFromClient.PartPrice.Value;
                    masterPartDBObject[0].CORE_VALUE = masterPartFromClient.CoreValue.Value;

                    masterPartDBObject[0].CORE_PART_SW = masterPartFromClient.SerialTagSW;
                    masterPartDBObject[0].DEDUCT_CORE = masterPartFromClient.DeductCoreSW;
                    masterPartDBObject[0].MSL_PART_SW = masterPartFromClient.MslPartSW;
                    masterPartDBObject[0].PART_ACTIVE_SW = masterPartFromClient.PartActiveSW;

                    masterPartDBObject[0].REMARKS = masterPartFromClient.Remarks;
                    masterPartDBObject[0].CHTS = DateTime.Now;
                    masterPartDBObject[0].CHUSER = masterPartFromClient.ChangeUser;

                    objContext.SaveChanges();
                    mastPart.IsMasterPartEditSuccess = true;
                    mastPart.IsMasterPartExist = true;

                }
                catch (Exception exp)
                {
                    mastPart.IsMasterPartEditSuccess = false;
                }
            }
            else
            {
                mastPart.IsMasterPartEditSuccess = false;
                mastPart.IsMasterPartExist = false;
            }

            //"yyyyMMdd HH:mm:ss"


            if (mastPart.IsMasterPartEditSuccess)
            {
                if (newPrice != oldPrice)
                {
                    EditAuditLogData("MESC1TS_MASTER_PART", masterPartFromClient.PartCd, oldPrice.ToString("F"), newPrice.ToString("F"), masterPartFromClient.ChangeUser, "PART_PRICE", DateTime.Now.ToString());
                }
                else
                {
                    // no audit log entry
                }
            }

            return mastPart;
        }



        public bool DeleteMasterPart(string masterPartCode)
        {
            bool isSuccess = false;
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_MASTER_PART masterPartDBObject = new MESC1TS_MASTER_PART();

            masterPartDBObject = (MESC1TS_MASTER_PART)(from masterPart in objContext.MESC1TS_MASTER_PART
                                                       where masterPart.PART_CD == masterPartCode
                                                       select masterPart);
            try
            {
                objContext.MESC1TS_MASTER_PART.Remove(masterPartDBObject);
                objContext.SaveChanges();
                isSuccess = true;
            }
            catch
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public List<RefAudit> GetAuditTrail_MasterPart(string recordId, string tableName)
        {
            List<RefAudit> RefAuditList = new List<RefAudit>();
            try
            {

                dynamic res = "";
                objContext = new ManageMasterDataServiceEntities();

                /*
                sSQL = "SELECT R.TAB_NAME,R.UNIQUE_ID,R.COL_NAME,R.OLD_VALUE,";
                sSQL += "R.NEW_VALUE,R.CHUSER,U.LASTNAME,U.FIRSTNAME,R.CHTS";
                sSQL += " FROM MESC1TS_REFAUDIT R LEFT OUTER JOIN SEC_USER U ON U.LOGIN=R.CHUSER";
                sSQL += " WHERE R.TAB_NAME="; sSQL += (char*)FormatText(Trim("MESC1TS_MASTER_PART"));
                sSQL += " AND R.UNIQUE_ID="; sSQL += (char*)FormatText(Trim(sUniqueID));
                sSQL += " ORDER BY R.CHTS DESC";                 
                */

                if (tableName == "MESC1TS_MASTER_PART")
                {
                    res = (from MR in objContext.MESC1TS_REFAUDIT
                           from U in objContext.SEC_USER
                                     .Where(U => U.LOGIN == MR.CHUSER)
                                     .DefaultIfEmpty()
                           where MR.TAB_NAME == tableName && MR.UNIQUE_ID == recordId
                           orderby MR.CHTS descending
                           select new { MR.TAB_NAME, MR.UNIQUE_ID, MR.COL_NAME, MR.OLD_VALUE, MR.NEW_VALUE, MR.CHUSER, U.LASTNAME, U.FIRSTNAME, MR.CHTS }).ToList();

                }

                foreach (var obj in res)
                {
                    RefAudit objRef = new RefAudit();
                    objRef.TabName = obj.TAB_NAME;
                    objRef.UniqueID = obj.UNIQUE_ID;
                    objRef.ColName = obj.COL_NAME;
                    objRef.OldValue = obj.OLD_VALUE;
                    objRef.NewValue = obj.NEW_VALUE;
                    objRef.ChangeUser = obj.CHUSER;
                    objRef.FirstName = obj.FIRSTNAME;
                    objRef.LastName = obj.LASTNAME;
                    objRef.ChangeTime = obj.CHTS;
                    RefAuditList.Add(objRef);
                };

            }

            catch (Exception ex)
            {
                string mg = ex.Message;

            }
            return RefAuditList;
        }


        #endregion Amlan - Master Parts

        #region Amlan - Parts Group

        public PartsGroup GetPartsGroupByQuery(string partGroupCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_PARTS_GROUP partGroupFromDB = new MESC1TS_PARTS_GROUP();
            List<MESC1TS_PARTS_GROUP> lstPartGroupFromDB = new List<MESC1TS_PARTS_GROUP>();
            PartsGroup partsGroup = new PartsGroup();

            try
            {
                dynamic lstPartGroupFromDBquery = "";
                lstPartGroupFromDBquery = (from P in objContext.MESC1TS_PARTS_GROUP
                                           from U in objContext.SEC_USER
                                       .Where(U => U.LOGIN == P.CHUSER)
                                       .DefaultIfEmpty()
                                           where P.PARTS_GROUP_CD == partGroupCode.Trim()
                                           select new { P, U.LASTNAME, U.FIRSTNAME }).ToList();




                lstPartGroupFromDBquery = lstPartGroupFromDBquery[0];

                partsGroup.PartsGroupCd = lstPartGroupFromDBquery.P.PARTS_GROUP_CD;
                partsGroup.PartsGroupDesc = lstPartGroupFromDBquery.P.PARTS_GROUP_DESC;
                partsGroup.PartsGroupActiveSW = lstPartGroupFromDBquery.P.PARTS_GROUP_ACTIVE_SW;
                partsGroup.Remarks = lstPartGroupFromDBquery.P.REMARKS;
                partsGroup.ChangeTime = String.Format("{0:yyyy-MM-dd hh:mm:ss tt}", lstPartGroupFromDBquery.P.CHTS);

                partsGroup.ChangeUser = (lstPartGroupFromDBquery.FIRSTNAME == null ? "" : lstPartGroupFromDBquery.FIRSTNAME) + " " +
                 (lstPartGroupFromDBquery.LASTNAME == null ? "" : lstPartGroupFromDBquery.LASTNAME);
            }
            catch (Exception e)
            {

            }

            return partsGroup;

        }



        public PartsGroup AddPartsGroup(PartsGroup partGroupFromClient)
        {

            PartsGroup partsGroupResult = new PartsGroup();
            partsGroupResult.IsPartsGroupCodeExists = false;
            partsGroupResult.IsPartsGroupAddUpdateSuccess = false;

            try
            {

                partsGroupResult.IsPartsGroupCodeExists = IsPartsGroupExist(partGroupFromClient.PartsGroupCd);

                if (partsGroupResult.IsPartsGroupCodeExists)
                {
                    return partsGroupResult; // do not go for save
                }

                // saving data
                objContext = new ManageMasterDataServiceEntities();
                MESC1TS_PARTS_GROUP partGroupToBeInserted = new MESC1TS_PARTS_GROUP();

                partGroupToBeInserted.PARTS_GROUP_CD = partGroupFromClient.PartsGroupCd;
                partGroupToBeInserted.PARTS_GROUP_DESC = partGroupFromClient.PartsGroupDesc;
                partGroupToBeInserted.REMARKS = partGroupFromClient.Remarks;
                partGroupToBeInserted.PARTS_GROUP_ACTIVE_SW = partGroupFromClient.PartsGroupActiveSW;
                partGroupToBeInserted.CHUSER = partGroupFromClient.ChangeUser;
                partGroupToBeInserted.CHTS = DateTime.Now;

                objContext.MESC1TS_PARTS_GROUP.Add(partGroupToBeInserted);


                objContext.SaveChanges();
                partsGroupResult.IsPartsGroupAddUpdateSuccess = true;
            }
            catch (Exception ex)
            {
                partsGroupResult.IsPartsGroupAddUpdateSuccess = false;
            }

            return partsGroupResult;

        }

        public bool UpdatePartsGroup(PartsGroup partGroupFromClient)
        {
            bool isSuccess = false;
            objContext = new ManageMasterDataServiceEntities();

            try
            {
                List<MESC1TS_PARTS_GROUP> partGroupDBObject = new List<MESC1TS_PARTS_GROUP>();
                partGroupDBObject = (from partGroup in objContext.MESC1TS_PARTS_GROUP
                                     where partGroup.PARTS_GROUP_CD == partGroupFromClient.PartsGroupCd
                                     select partGroup).ToList();

                partGroupDBObject[0].PARTS_GROUP_DESC = partGroupFromClient.PartsGroupDesc;
                partGroupDBObject[0].REMARKS = partGroupFromClient.Remarks;
                partGroupDBObject[0].PARTS_GROUP_ACTIVE_SW = partGroupFromClient.PartsGroupActiveSW;
                partGroupDBObject[0].CHUSER = partGroupFromClient.ChangeUser;
                partGroupDBObject[0].CHTS = DateTime.Now;



                objContext.SaveChanges();
                isSuccess = true;
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return isSuccess;


        }

        public bool IsPartsGroupExist(string partGroupCode)
        {
            bool isPartsGroupExist = false;
            objContext = new ManageMasterDataServiceEntities();
            try
            {
                List<MESC1TS_PARTS_GROUP> partGroupDBObject = new List<MESC1TS_PARTS_GROUP>();
                partGroupDBObject = (from partGroup in objContext.MESC1TS_PARTS_GROUP
                                     where partGroup.PARTS_GROUP_CD == partGroupCode
                                     select partGroup).ToList();

                if (partGroupDBObject.Count > 0)
                {
                    isPartsGroupExist = true;
                }

            }
            catch (Exception e)
            {

            }


            return isPartsGroupExist;
        }


        public List<Country> GetAllCountries()
        {
            List<MESC1TS_COUNTRY> countryList = new List<MESC1TS_COUNTRY>();
            try
            {
                objContext = new ManageMasterDataServiceEntities();

                countryList = (from country in objContext.MESC1TS_COUNTRY
                               orderby country.COUNTRY_CD ascending
                               select country).ToList();
            }
            catch (Exception e)
            {

                logEntry.Message = e.ToString();
                Logger.Write(logEntry);
            }


            return PrepareDataContract(countryList);
        }

        private List<Country> PrepareDataContract(List<MESC1TS_COUNTRY> countryListFromDB)
        {
            List<Country> countryList = new List<Country>();
            for (int count = 0; count < countryListFromDB.Count; count++)
            {
                Country country = new Country();
                country.CountryCode = countryListFromDB[count].COUNTRY_CD;
                country.CountryDescription = countryListFromDB[count].COUNTRY_DESC;

                countryList.Add(country);

            }
            return countryList;
        }

        public List<Mode> GetAllModes()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> modeList = new List<MESC1TS_MODE>();
            try
            {

                modeList = (from mode in objContext.MESC1TS_MODE
                            orderby mode.MODE ascending
                            select mode).ToList();
            }
            catch (Exception e)
            {
                logEntry.Message = e.ToString();
                Logger.Write(logEntry);
            }

            return PrepareDataContract_Mode(modeList);
        }

        private List<Mode> PrepareDataContract_Mode(List<MESC1TS_MODE> modeFromDB)
        {
            List<Mode> modeList = new List<Mode>();
            Mode mode;
            for (int count = 0; count < modeFromDB.Count; count++)
            {
                mode = new Mode();
                mode.ModeCode = modeFromDB[count].MODE;
                mode.ModeDescription = modeFromDB[count].MODE + '-' + modeFromDB[count].MODE_DESC;

                modeList.Add(mode);
            }
            return modeList;
        }

        public List<CountryCont> GetCountryContractByQuery(string countryCode, string repairCode, string mode)
        {
            List<CountryCont> countryContList = new List<CountryCont>();
            try
            {
                objContext = new ManageMasterDataServiceEntities();

                List<MESC1TS_COUNTRY_CONT> countryContFromDB = new List<MESC1TS_COUNTRY_CONT>();
                countryContFromDB = (from countryCont in objContext.MESC1TS_COUNTRY_CONT
                                     where
                   countryCont.COUNTRY_CD == (string.IsNullOrEmpty(countryCode) ? countryCont.COUNTRY_CD : countryCode.Trim())
                   &&
                   countryCont.REPAIR_CD == (string.IsNullOrEmpty(repairCode) ? countryCont.REPAIR_CD : repairCode.Trim())
                   &&
                   countryCont.MODE == (string.IsNullOrEmpty(mode) ? countryCont.MODE : mode.Trim())
                                     orderby countryCont.COUNTRY_CD, countryCont.MODE, countryCont.REPAIR_CD ascending
                                     select countryCont).ToList();


                countryContList = PrepareDataContract(countryContFromDB);

            }
            catch (Exception e)
            {
                logEntry.Message = e.ToString();
                Logger.Write(logEntry);
            }

            return countryContList;

        }

        private List<CountryCont> PrepareDataContract(List<MESC1TS_COUNTRY_CONT> countryContListFromDB)
        {
            List<CountryCont> countryContList = new List<CountryCont>();
            CountryCont countryCont;
            for (int count = 0; count < countryContListFromDB.Count; count++)
            {
                countryCont = new CountryCont();

                countryCont.CountryCode = countryContListFromDB[count].COUNTRY_CD;
                countryCont.RepairCod = countryContListFromDB[count].REPAIR_CD;
                countryCont.ContractAmount = countryContListFromDB[count].CONTRACT_AMOUNT;
                countryCont.CUCDN = countryContListFromDB[count].CUCDN;
                countryCont.ManualCode = countryContListFromDB[count].MANUAL_CD;
                countryCont.Mode = countryContListFromDB[count].MODE;
                countryCont.ExpiryDate = countryContListFromDB[count].EXP_DTE;
                countryCont.EffectiveDate = countryContListFromDB[count].EFF_DTE;

                countryContList.Add(countryCont);

            }
            return countryContList;
        }

        #endregion Amlan - Parts Group

        #region Amlan - Repair Code/Part Association

        public List<Mode> GetAllActiveModes()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> modeList = new List<MESC1TS_MODE>();
            try
            {
                modeList = (from mode in objContext.MESC1TS_MODE
                            where mode.MODE_ACTIVE_SW == "Y"
                            orderby mode.MODE ascending
                            select mode).ToList();
            }
            catch (Exception e)
            {
                logEntry.Message = e.ToString();
                Logger.Write(logEntry);
            }

            return PrepareDataContract(modeList, false);
        }

        public List<Manual> GetAllActiveManuals()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUAL> manualList = new List<MESC1TS_MANUAL>();
            try
            {
                manualList = (from manual in objContext.MESC1TS_MANUAL
                              //where manual.MANUAL_ACTIVE_SW == "Y"
                              orderby manual.MANUAL_CD ascending
                              select manual).ToList();
            }
            catch (Exception e)
            {
                logEntry.Message = e.ToString();
                Logger.Write(logEntry);
            }

            return PrepareDataContract(manualList);
        }

        private List<Manual> PrepareDataContract(List<MESC1TS_MANUAL> manualListFromDB)
        {
            List<Manual> manualList = new List<Manual>();
            Manual manual;
            for (int count = 0; count < manualListFromDB.Count; count++)
            {
                manual = new Manual();
                manual.ManualCode = manualListFromDB[count].MANUAL_CD;
                manual.ManualDesc = manualListFromDB[count].MANUAL_CD + '-' + manualListFromDB[count].MANUAL_DESC;

                manualList.Add(manual);
            }
            return manualList;
        }

        public List<RepairCodePart> GetRepairCode_PartAssociation(string mode, string manual, string repairCode, string partNumber)
        {
            objContext = new ManageMasterDataServiceEntities();

            /*"Select distinct R.*, RC.REPAIR_DESC, P.PART_DESC, P.PART_PRICE ";
            "From MESC1TS_RPRCODE_PART R, MESC1TS_REPAIR_CODE RC, MESC1TS_MASTER_PART P Where ";
            "R.MANUAL_CD = ";
            FormatText(sManual_Cd);
            " and R.MODE = ";
            FormatText(sMode);
            " and RC.MODE = R.MODE";
            " and R.REPAIR_CD = RC.REPAIR_CD and R.PART_CD = P.PART_CD ";
            " and R.REPAIR_CD = ";
            FormatText(sRepair_Cd);
            " and R.PART_CD = ";
            FormatText(sPart_Cd);
            " Order by R.MANUAL_CD, R.MODE, R.REPAIR_CD, R.PART_CD";
            */

            List<RepairCodePart> rpaList = new List<RepairCodePart>();

            try
            {

                var srchResult = (from R in objContext.MESC1TS_RPRCODE_PART
                                  join RC in objContext.MESC1TS_REPAIR_CODE
                                  on new { R.MODE, R.REPAIR_CD } equals
                                     new { RC.MODE, RC.REPAIR_CD }
                                  join P in objContext.MESC1TS_MASTER_PART on R.PART_CD equals P.PART_CD
                                  from U in objContext.SEC_USER
                                       .Where(U => U.LOGIN == P.CHUSER)
                                       .DefaultIfEmpty()
                                  where R.REPAIR_CD == (string.IsNullOrEmpty(repairCode) ? R.REPAIR_CD : repairCode.Trim())
                                  && R.PART_CD == (string.IsNullOrEmpty(partNumber) ? R.PART_CD : partNumber.Trim())
                                  && R.MANUAL_CD == (string.IsNullOrEmpty(manual) ? R.MANUAL_CD : manual.Trim())
                                  && R.MODE == (string.IsNullOrEmpty(mode) ? R.MODE : mode.Trim())
                                  orderby R.MANUAL_CD, R.MODE, R.REPAIR_CD, R.PART_CD
                                  select new { R, RC.REPAIR_DESC, P.PART_DESC, P.PART_PRICE, U.LASTNAME, U.FIRSTNAME }).Distinct();

                RepairCodePart rpa;

                foreach (var item in srchResult)
                {
                    rpa = new RepairCodePart();
                    rpa.ManualCode = item.R.MANUAL_CD;
                    rpa.ModeCode = item.R.MODE;
                    rpa.PartNumber = item.R.PART_CD;
                    rpa.RepairCod = item.R.REPAIR_CD;
                    rpa.PartDesc = item.PART_DESC.Trim();
                    rpa.RepairDesc = item.REPAIR_DESC.Trim();
                    rpa.MaxPartQty = item.R.MAX_PART_QTY.ToString();
                    rpa.ChangeUser = item.R.CHUSER;
                    //rpa.ChangeUser = (item.FIRSTNAME == null ? "" : item.FIRSTNAME) + " " +
                    //(item.LASTNAME == null ? "" : item.LASTNAME);
                    //rpa.ChangeTime = item.R.CHTS.ToShortDateString();
                    rpa.ChangeTime = String.Format("{0:yyyy-MM-dd hh:mm:ss tt}", item.R.CHTS);
                    rpaList.Add(rpa);
                }

            }
            catch (Exception e)
            {
                logEntry.Message = e.ToString();
                Logger.Write(logEntry);
            }

            return rpaList;
        }


        private List<RepairCodePart> PrepareDataContract(List<MESC1TS_RPRCODE_PART> lstRprCodePartFromDB)
        {
            List<RepairCodePart> rpaList = new List<RepairCodePart>();
            RepairCodePart rpa;
            for (int count = 0; count < lstRprCodePartFromDB.Count; count++)
            {
                rpa = new RepairCodePart();
                rpa.ManualCode = lstRprCodePartFromDB[count].MANUAL_CD;
                rpa.ModeCode = lstRprCodePartFromDB[count].MODE;
                rpa.PartNumber = lstRprCodePartFromDB[count].PART_CD;
                rpa.RepairCod = lstRprCodePartFromDB[count].REPAIR_CD;
                rpa.MaxPartQty = lstRprCodePartFromDB[count].MAX_PART_QTY.ToString();
                rpa.ChangeUser = lstRprCodePartFromDB[count].CHUSER;
                rpa.ChangeTime = lstRprCodePartFromDB[count].CHTS.ToShortDateString();
                rpaList.Add(rpa);
            }
            return rpaList;
        }

        public List<RepairCodePart> GetRPAssnByCode(string manCode, string modeCode, string repCode)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_RPRCODE_PART> lstRPCFromDB = new List<MESC1TS_RPRCODE_PART>();
            lstRPCFromDB = (from var in objContext.MESC1TS_RPRCODE_PART
                            where
                            var.MANUAL_CD == manCode
                            && var.MODE == modeCode
                            && var.REPAIR_CD == repCode
                            select var).ToList();


            List<RepairCodePart> lstRepairCodePart = new List<RepairCodePart>();
            lstRepairCodePart = PrepareDataContract(lstRPCFromDB);

            return lstRepairCodePart;

        }

        public List<Mode> GetModesByManuals(string manCode)
        {
            List<Mode> modeList = new List<Mode>();
            var res = from MM in objContext.MESC1TS_MANUAL_MODE
                      join MAN in objContext.MESC1TS_MANUAL on MM.MANUAL_CD equals MAN.MANUAL_CD
                      join MOD in objContext.MESC1TS_MODE on MM.MODE equals MOD.MODE
                      where MM.MANUAL_CD == manCode
                      select new { MOD.MODE, MOD.MODE_DESC };

            Mode mode;
            foreach (var item in res.ToList())
            {
                mode = new Mode();
                mode.ModeCode = item.MODE;
                mode.ModeDescription = item.MODE_DESC;
                mode.ModeFullDescription = item.MODE + "-" + item.MODE_DESC;
                modeList.Add(mode);
            }

            return modeList;
        }


        private string ValidateRepairPart(string mode, string manual, string partNumber, string repairCode)
        {
            string sError = string.Empty;
            if (String.IsNullOrEmpty(manual))
                sError = "Manual Code required. ";

            if (String.IsNullOrEmpty(mode))
                sError += "Mode required. ";

            if (String.IsNullOrEmpty(repairCode))
                sError += " Repair Code required";

            if (String.IsNullOrEmpty(partNumber))
                sError += " Part Code required";

            return sError.Trim();

        }

        private string ValidateRprCode_PartNum(string mode, string manual, string repairCode, string partNumber)
        {
            string msg = string.Empty;

            // check if valid repair code entered.

            //Select REPAIR_ACTIVE_SW from MESC1TS_REPAIR_CODE Where MANUAL_CD = Manual_Cd
            //and MODE = sMode and REPAIR_CD = sRepair_Cd

            var res = from REPAIR_CODE in objContext.MESC1TS_REPAIR_CODE
                      where REPAIR_CODE.MANUAL_CD == manual && REPAIR_CODE.MODE == mode && REPAIR_CODE.REPAIR_CD == repairCode
                      select new { REPAIR_CODE.REPAIR_ACTIVE_SW };

            if (res != null && res.ToList().Count == 0)
            {
                msg = "Repair Code Entered is Not Listed on Repair Code Table for the Selected Manual/Mode";
                return msg;
            }


            var act = res.ToList()[0].REPAIR_ACTIVE_SW;

            if (act.ToString().ToUpper() == "N")
            {
                msg = "Repair Code is not Active on Repair Code Table";
                return msg;
            }


            // check if valid part number entered.

            //Select PART_PRICE, PART_DESC, PART_ACTIVE_SW from MESC1TS_MASTER_PART Where PART_CD = ";

            var reslt = from MASTER_PART in objContext.MESC1TS_MASTER_PART
                        where MASTER_PART.PART_CD == partNumber
                        select new { MASTER_PART.PART_PRICE, MASTER_PART.PART_DESC, MASTER_PART.PART_ACTIVE_SW };

            if (reslt != null && reslt.ToList().Count == 0)
            {
                msg = "Part Number Entered is Not Listed on Master Part List";
                return msg;
            }

            var actv = reslt.ToList()[0].PART_ACTIVE_SW;

            if (actv.ToString().ToUpper() == "N")
            {
                msg = "Part Number is not Active on Master Part List";
                return msg;
            }

            var pr = reslt.ToList()[0].PART_PRICE;

            if (pr != null && (int)pr == 0)
            {
                msg = "Part Number has been Superceded on Master Part List - Please check for Current Active Part";
                return msg;
            }

            return msg;


        }


        public ServiceResult UpdateRPA(string orgManualCode, string orgModeCode, string orgPartNumber,
          string orgRepairCod, string maxPartQty, string userName)
        {
            objContext = new ManageMasterDataServiceEntities();
            string msg;
            bool isSuccess;

            try
            {
                List<MESC1TS_RPRCODE_PART> rprDBObject = new List<MESC1TS_RPRCODE_PART>();
                rprDBObject = (from RPRCODE_PART in objContext.MESC1TS_RPRCODE_PART
                               where RPRCODE_PART.MANUAL_CD == orgManualCode && RPRCODE_PART.MODE == orgModeCode
                                     && RPRCODE_PART.PART_CD == orgPartNumber && RPRCODE_PART.REPAIR_CD == orgRepairCod
                               select RPRCODE_PART).ToList();

                int res_maxPartQty = 0;
                int? db_maxPartQty = null;
                if (int.TryParse(maxPartQty, out res_maxPartQty) == false)
                {
                    db_maxPartQty = null;
                }
                else
                {
                    db_maxPartQty = res_maxPartQty;
                }

                rprDBObject[0].MAX_PART_QTY = db_maxPartQty;
                rprDBObject[0].CHUSER = userName;
                rprDBObject[0].CHTS = DateTime.Now;

                objContext.SaveChanges();
                msg = "Association Repair Code/Part " + orgRepairCod + " / " + orgPartNumber + " Updated";
                isSuccess = true;
            }
            catch (Exception exp)
            {
                msg = exp.Message;
                isSuccess = false;
            }

            ServiceResult result = new ServiceResult();
            result.Message = msg;
            result.IsSuccess = isSuccess;
            return result;

        }

        public ServiceResult Add_Edit_RPA(RepairCodePart repairCodePartFromClient, string OprMode)
        {
            //RepairCodePart rcp = new RepairCodePart();
            string msg = string.Empty;

            string mode = repairCodePartFromClient.ModeCode;
            string manual = repairCodePartFromClient.ManualCode;
            string partNumber = repairCodePartFromClient.PartNumber;
            string repairCode = repairCodePartFromClient.RepairCod;
            string maxQty = repairCodePartFromClient.MaxPartQty;
            string user = repairCodePartFromClient.ChangeUser;
            string orgMode = repairCodePartFromClient.OrgModeCode;
            string orgManual = repairCodePartFromClient.OrgManualCode;
            string orgPartNumber = repairCodePartFromClient.OrgPartNumber;
            string orgRepairCode = repairCodePartFromClient.OrgRepairCod;
            string orgMaxQty = repairCodePartFromClient.OrgMaxPartQty;

            ServiceResult result = new ServiceResult();

            msg = ValidateRepairPart(mode, manual, partNumber, repairCode);
            if (!String.IsNullOrEmpty(msg))
            {
                result.Message = msg;
                result.IsSuccess = false;
                return result;
            }

            msg = ValidateRprCode_PartNum(mode, manual, repairCode, partNumber);
            if (!String.IsNullOrEmpty(msg))
            {
                result.Message = msg;
                result.IsSuccess = false;
                return result;
            }

            if (OprMode == "ADD") //Add
            {
                bool isInsertSuccess = false;
                msg = InsertRPRCODE_PART(repairCode, partNumber, mode, manual, maxQty, user, out isInsertSuccess);
                result.Message = msg;
                result.IsSuccess = isInsertSuccess;
                return result;
            }
            else if (OprMode == "EDIT")
            {

                //Select distinct R.*, RC.REPAIR_DESC, P.PART_DESC, P.PART_PRICE From MESC1TS_RPRCODE_PART R,                                    MESC1TS_REPAIR_CODE RC, MESC1TS_MASTER_PART P Where R.MANUAL_CD = manual  and R.MODE = mode
                //and RC.MODE = R.MODE and R.REPAIR_CD = RC.REPAIR_CD and R.PART_CD = P.PART_CD //and R.REPAIR_CD =                              repairCode and R.PART_CD = partNumber  Order by R.MANUAL_CD, R.MODE, R.REPAIR_CD, R.PART_CD

                //List<MESC1TS_RPRCODE_PART> lstRprCodePart = new List<MESC1TS_RPRCODE_PART>();

                try
                {

                    var res = (from R in objContext.MESC1TS_RPRCODE_PART
                               join RC in objContext.MESC1TS_REPAIR_CODE
                               on new { R.MODE, R.REPAIR_CD } equals
                                  new { RC.MODE, RC.REPAIR_CD }
                               join P in objContext.MESC1TS_MASTER_PART on R.PART_CD equals P.PART_CD
                               where R.REPAIR_CD == repairCode && R.PART_CD == partNumber
                               && R.MANUAL_CD == manual && R.MODE == mode
                               select new { R, RC.REPAIR_DESC, P.PART_DESC, P.PART_PRICE }).Distinct();

                    if (res != null && res.ToList().Count > 0)
                    {
                        // new record already exists, so no edit allowed.
                        msg = "Associatio Code/Part " + repairCode + "/" + partNumber + " already exists - Not updated";
                        result.Message = msg;
                        result.IsSuccess = false;
                        return result;
                    }
                    else // record does not exists
                    {
                        // Delete the old record
                        bool isDeleteSuccess = false;
                        msg = DeleteRPRCODE_PART(orgRepairCode, orgPartNumber, orgMode, orgManual, out isDeleteSuccess);
                        if (isDeleteSuccess == false)
                        {
                            result.Message = msg;
                            result.IsSuccess = false;
                            return result;
                        }
                        else // delete success
                        {
                            //insert the new record
                            bool isInsertSuccess = false;
                            msg = InsertRPRCODE_PART(repairCode, partNumber, mode, manual, maxQty, user, out isInsertSuccess);
                            if (isInsertSuccess)
                            {
                                msg = "Association Repair Code/Part " + repairCode + " / " + partNumber + " Updated";
                            }

                            result.Message = msg;
                            result.IsSuccess = isInsertSuccess;
                            return result;
                        }
                    }
                }
                catch (Exception e)
                {
                    result.Message = e.Message;
                    result.IsSuccess = false;
                    return result;
                }
            }
            else
            {
                //do nothing
                return result;
            }


        }


        public ServiceResult DeleteRPRCODE_PART(string repairCod, string partNumber, string modeCode, string manualCode)
        {
            string msg = string.Empty;
            bool isDeleteSuccess;
            try
            {
                msg = DeleteRPRCODE_PART(repairCod, partNumber, modeCode, manualCode, out isDeleteSuccess);

                if (String.IsNullOrEmpty(msg))
                {
                    if (isDeleteSuccess)
                    {
                        msg = "Association Repair Code/Part " + repairCod + " / " + partNumber + " Deleted";
                    }
                }

            }
            catch (Exception ex)
            {
                isDeleteSuccess = false;
                msg = "Exception while Deleting";
            }

            ServiceResult result = new ServiceResult();
            result.IsSuccess = isDeleteSuccess;
            result.Message = msg;
            return result;
        }


        private string DeleteRPRCODE_PART(string repairCod, string partNumber, string modeCode, string manualCode, out bool isDeleteSuccess)
        {
            string msg = string.Empty;

            //sSQL = "select PART_CD, PART_DESC from [dbo].[MESC1TS_MASTER_PART] where PART_PRICE = 0";
            objContext = new ManageMasterDataServiceEntities();
            List<string> lstPartDesc = new List<string>();
            List<string> lstpartCD = new List<string>();
            //StringBuilder descBuilder = new StringBuilder();

            try
            {
                var res = from MASTER_PART in objContext.MESC1TS_MASTER_PART
                          where MASTER_PART.PART_CD == partNumber && MASTER_PART.PART_PRICE == 0
                          select new { MASTER_PART.PART_CD, MASTER_PART.PART_DESC };

                foreach (var item in res.ToList())
                {
                    //descBuilder.Append("'" + item.PART_DESC + "'" + ",");
                    lstPartDesc.Add(item.PART_DESC);
                }

                var result = from MASTER_PART in objContext.MESC1TS_MASTER_PART
                             where lstPartDesc.Contains(MASTER_PART.PART_DESC)
                             select new { MASTER_PART.PART_CD };

                foreach (var item in result.ToList())
                {
                    lstpartCD.Add(item.PART_CD);
                }

                lstpartCD.Add(partNumber); // Finally, adding the part number from UI to the list.

                List<MESC1TS_RPRCODE_PART> lstRprCodeDBObject = new List<MESC1TS_RPRCODE_PART>();
                lstRprCodeDBObject = (from RPRCODE_PART in objContext.MESC1TS_RPRCODE_PART
                                      where lstpartCD.Contains(RPRCODE_PART.PART_CD)
                                      && RPRCODE_PART.REPAIR_CD == repairCod
                                      && RPRCODE_PART.MODE == modeCode
                                      && RPRCODE_PART.MANUAL_CD == manualCode
                                      select RPRCODE_PART).ToList();
                foreach (var item in lstRprCodeDBObject)
                {
                    objContext.MESC1TS_RPRCODE_PART.Remove(item);
                }

                objContext.SaveChanges();
                isDeleteSuccess = true;
            }
            catch (Exception ex)
            {
                isDeleteSuccess = false;
                msg = ex.Message;
            }

            return msg;

        }

        private string InsertRPRCODE_PART(string repairCode, string partNumber, string mode, string manual,
    string maxQty, string user, out bool isInsertSuccess)
        {
            string msg = string.Empty;

            if (!Does_RP_AssnExist(repairCode, partNumber, manual, mode))
            {

                // if user does not enter max qty, set to null.
                int result = 0;
                int.TryParse(maxQty, out result);

                MESC1TS_RPRCODE_PART repair_part_ToBeInserted = new MESC1TS_RPRCODE_PART();
                repair_part_ToBeInserted.CHTS = DateTime.Now;
                repair_part_ToBeInserted.CHUSER = user;
                repair_part_ToBeInserted.MANUAL_CD = manual;
                repair_part_ToBeInserted.MODE = mode;
                repair_part_ToBeInserted.PART_CD = partNumber;
                repair_part_ToBeInserted.REPAIR_CD = repairCode;
                repair_part_ToBeInserted.MAX_PART_QTY = result;

                //insert
                objContext.MESC1TS_RPRCODE_PART.Add(repair_part_ToBeInserted);

                try
                {
                    objContext.SaveChanges();
                    isInsertSuccess = true;
                    msg = "Association Repair Code/Part " + repairCode + " / " + partNumber + " Added";
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    isInsertSuccess = false;
                }
            }
            else
            {
                //record is a duplicate
                msg = "Association Repair Code/Part " + repairCode + " / " + partNumber + " Already Exists - Not Added";
                isInsertSuccess = false;
            }

            return msg;


        }

        private bool Does_RP_AssnExist(string repairCod, string partNumber, string manualCode, string modeCode)
        {
            bool does_RP_AssnExist = false;
            objContext = new ManageMasterDataServiceEntities();
            List<string> lstMp = new List<string>();

            //Select  R.*,  U.LASTNAME, U.FIRSTNAME From MESC1TS_RPRCODE_PART R left outer join SEC_USER U on                 U.LOGIN = R.CHUSER Where MANUAL_CD = FormatText(sManual_Cd)
            //and MODE = FormatText(sMode) and REPAIR_CD = FormatText(sRepair_Cd) and PART_CD = FormatText                    (sPart_Cd);
            try
            {
                lstMp = (from mrp in objContext.MESC1TS_RPRCODE_PART
                         where mrp.PART_CD == partNumber
                         && mrp.MANUAL_CD == manualCode
                         && mrp.MODE == modeCode
                         && mrp.REPAIR_CD == repairCod
                         select mrp.PART_CD).ToList();

                if (lstMp.Count > 0)
                {
                    does_RP_AssnExist = true;
                }
                else
                {
                    does_RP_AssnExist = false;
                }
            }
            catch (Exception e)
            {

                does_RP_AssnExist = false;
                logEntry.Message = e.ToString();
                Logger.Write(logEntry);
            }

            return does_RP_AssnExist;

        }

        #endregion  Amlan - Repair Code/Part Association

        //------------------------- Ashiqur --------------------------------------//

        #region Country Labour Rate

        public List<Country> GetCountryLabourList()
        {

            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_COUNTRY> CountryListFromDB = new List<MESC1TS_COUNTRY>();
            List<Country> CountryList = new List<Country>();

            try
            {
                CountryListFromDB = (from C in objContext.MESC1TS_COUNTRY
                                     orderby C.COUNTRY_CD ascending
                                     select C).ToList();

                for (int count = 0; count < CountryListFromDB.Count; count++)
                {

                    Country country = new Country();
                    country.CountryCode = CountryListFromDB[count].COUNTRY_CD;
                    country.CountryDescription = CountryListFromDB[count].COUNTRY_DESC;
                    CountryList.Add(country);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return CountryList;

        }

        public List<EqType> GetEquipmentTypeList()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_EQTYPE> equipmentTypeList = new List<MESC1TS_EQTYPE>();
            equipmentTypeList = (from eqtype in objContext.MESC1TS_EQTYPE
                                 select eqtype).ToList();

            return PrepareDataContract(equipmentTypeList);

        }

        public List<CountryLabor> GetLabourRateDetails(string Country, string Eqtype)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_COUNTRY_LABOR> LabourRateListfromDB = new List<MESC1TS_COUNTRY_LABOR>();
            List<CountryLabor> CountryLabourList = new List<CountryLabor>();

            bool IsCountry = false;

            var Query = (from CL in objContext.MESC1TS_COUNTRY_LABOR
                         select CL).Distinct().ToList();



            if (!string.IsNullOrEmpty(Country) || !string.IsNullOrEmpty(Eqtype))
            {
                if (!string.IsNullOrEmpty(Country))
                {
                    Query.Clear();
                    Query = (from CL in objContext.MESC1TS_COUNTRY_LABOR
                             where CL.COUNTRY_CD == Country
                             orderby CL.COUNTRY_CD, CL.EXP_DTE descending
                             select CL).Distinct().ToList();

                    IsCountry = true;
                }



                if (!string.IsNullOrEmpty(Eqtype))
                {

                    Query.Clear();

                    if (IsCountry == true)
                    {
                        Query = (from CL in objContext.MESC1TS_COUNTRY_LABOR
                                 where CL.EQTYPE == Eqtype
                                 && CL.COUNTRY_CD == Country
                                 orderby CL.COUNTRY_CD, CL.EQTYPE, CL.EXP_DTE descending
                                 select CL).Distinct().ToList();
                    }
                    else
                    {
                        Query = (from CL in objContext.MESC1TS_COUNTRY_LABOR
                                 where CL.EQTYPE == Eqtype
                                 orderby CL.EQTYPE, CL.EXP_DTE descending
                                 select CL).Distinct().ToList();

                    }



                }


            }

            if (string.IsNullOrEmpty(Eqtype) && string.IsNullOrEmpty(Country))
            {
                CountryLabourList.Clear();
            }
            else
            {

                foreach (var q in Query)
                {
                    CountryLabor CountryLabour = new CountryLabor();
                    CountryLabour.CountryCode = q.COUNTRY_CD;
                    CountryLabour.EqType = q.EQTYPE;
                    CountryLabour.EffDate = q.EFF_DTE;
                    CountryLabour.ExpDate = q.EXP_DTE;
                    CountryLabour.RegularRT = q.REGULAR_RT;
                    CountryLabour.DoubleTimeRT = q.DOUBLETIME_RT;
                    CountryLabour.OvertimeRT = q.OVERTIME_RT;
                    CountryLabour.MiscRT = q.MISC_RT;
                    CountryLabourList.Add(CountryLabour);
                }

            }



            return CountryLabourList;
        }

        #endregion

        #region CPH Approval Level


        public List<Mode> GetCphEquLimitModeList()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> EqTypeListFromDB = new List<MESC1TS_MODE>();
            List<Mode> ModeList = new List<Mode>();

            try
            {
                var ModeListFrom = (from m in objContext.MESC1TS_MODE

                                    orderby m.MODE, m.MODE_IND
                                    select new
                                    {
                                        m.MODE,
                                        m.MODE_DESC,
                                        m.MODE_ACTIVE_SW,
                                        m.MODE_IND
                                    }).Distinct();



                foreach (var obj in ModeListFrom)
                {
                    Mode Mode = new Mode();
                    Mode.ModeCode = Convert.ToString(obj.MODE);
                    Mode.ModeDescription = Convert.ToString(obj.MODE_DESC);
                    ModeList.Add(Mode);
                }

            }
            catch (Exception ex)
            {

                throw ex;

            }
            return ModeList;

        }

        public List<CphEqpLimit> GetRSAllLimits(string Eq, string Mode)
        {

            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> EqTypeListFromDB = new List<MESC1TS_MODE>();
            List<CphEqpLimit> CphEqpLimitList = new List<CphEqpLimit>();

            try
            {


                //var Query = from CPH in objContext.MESC1TS_CPH_EQP_LIMIT
                //            from SU in objContext.SEC_USER
                //            .Where(SU => SU.LOGIN == CPH.CHUSER)
                //            .DefaultIfEmpty()
                //            select new { CPH, SU };


                var Query = from CPH in objContext.MESC1TS_CPH_EQP_LIMIT
                            join innerOD in objContext.SEC_USER on CPH.CHUSER equals innerOD.LOGIN into Inners
                            from SU in Inners.DefaultIfEmpty()
                            select new { CPH, SU };




                if (!string.IsNullOrEmpty(Eq))
                    Query = Query.Where(q => q.CPH.EQSIZE == Eq);

                if (!string.IsNullOrEmpty(Mode))
                    Query = Query.Where(q => q.CPH.MODE == Mode);


                int val = Query.Count();

                var CPHDetailsFromDB = (from q in Query
                                        select new
                                        {
                                            q.CPH.MODE,
                                            q.CPH.EQSIZE,
                                            q.CPH.AGE_FROM,
                                            q.CPH.AGE_TO,
                                            q.CPH.LIMIT_AMOUNT,
                                            q.CPH.CHUSER,
                                            q.CPH.CHTS,
                                            q.SU.FIRSTNAME,
                                            q.SU.LASTNAME

                                        }).ToList();

                int x = CPHDetailsFromDB.Count();



                foreach (var obj in CPHDetailsFromDB)
                {
                    CphEqpLimit cph = new CphEqpLimit();
                    cph.EqSize = Convert.ToString(obj.EQSIZE);
                    cph.ModeCode = Convert.ToString(obj.MODE);
                    cph.AgeFrom = Convert.ToInt16(obj.AGE_FROM);
                    cph.AgeTo = Convert.ToInt16(obj.AGE_TO);
                    cph.LimitAmount = obj.LIMIT_AMOUNT;

                    DateTimeFormatInfo format = new DateTimeFormatInfo();
                    format.ShortDatePattern = "yyyy-MM-dd hh:mm:ss tt";
                    format.DateSeparator = "-";
                    cph.ChTime = Convert.ToDateTime(obj.CHTS, format);

                    cph.ChangeUser = obj.CHUSER;
                    CphEqpLimitList.Add(cph);

                };






            }
            catch (Exception ex)
            {
                throw ex;
            }



            return CphEqpLimitList;
        }

        public List<CphEqpLimit> SubmitCPHApprovalDetails(CphEqpLimit cphEqu, string UserLogin, string LimitAmt1, string LimitAmt2, string LimitAmt3, string LimitAmt4, string LimitAmt5, string LimitAmt6, string LimitAmt7)
        {

            objContext = new ManageMasterDataServiceEntities();

            //     errorMessageList = new List<ErrMessage>();
            //     ErrMessage errormessage = new ErrMessage();


            List<CphEqpLimit> cphEqyList = new List<CphEqpLimit>();
            // List<MESC1TS_CPH_EQP_LIMIT> cphEquFromDB = new List<MESC1TS_CPH_EQP_LIMIT>();

            MESC1TS_CPH_EQP_LIMIT cphEquFromDB = new MESC1TS_CPH_EQP_LIMIT();

            try
            {

                bool valid = false;
                valid = CheckDuplicate(Convert.ToString(cphEqu.ModeCode), cphEqu.EqSize);

                if (valid == false)
                {

                    for (int i = 1; i <= 7; i++)
                    {

                        cphEquFromDB = new MESC1TS_CPH_EQP_LIMIT();

                        //    cphEquFromDB[i].MODE = Convert.ToString(cphEqu.ModeCode);
                        //    cphEquFromDB[i].EQSIZE = Convert.ToString(cphEqu.EqSize);

                        cphEquFromDB.MODE = Convert.ToString(cphEqu.ModeCode);
                        cphEquFromDB.EQSIZE = Convert.ToString(cphEqu.EqSize);

                        if (i == 1)
                        {

                            //          cphEquFromDB[i].AGE_FROM = 0;
                            //          cphEquFromDB[i].AGE_TO = 2;
                            //          cphEquFromDB[i].LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt1);

                            cphEquFromDB.AGE_FROM = 0;
                            cphEquFromDB.AGE_TO = 2;
                            cphEquFromDB.LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt1);
                        }
                        else if (i == 2)
                        {
                            //    cphEquFromDB[i].AGE_FROM = 3;
                            //    cphEquFromDB[i].AGE_TO = 4;
                            //    cphEquFromDB[i].LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt2);

                            cphEquFromDB.AGE_FROM = 3;
                            cphEquFromDB.AGE_TO = 4;
                            cphEquFromDB.LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt2);
                        }
                        else if (i == 3)
                        {
                            //     cphEquFromDB[i].AGE_FROM = 5;
                            //     cphEquFromDB[i].AGE_TO = 6;
                            //     cphEquFromDB[i].LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt3);

                            cphEquFromDB.AGE_FROM = 5;
                            cphEquFromDB.AGE_TO = 6;
                            cphEquFromDB.LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt3);
                        }
                        else if (i == 4)
                        {
                            //      cphEquFromDB[i].AGE_FROM = 7;
                            //      cphEquFromDB[i].AGE_TO = 8;
                            //      cphEquFromDB[i].LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt4);

                            cphEquFromDB.AGE_FROM = 7;
                            cphEquFromDB.AGE_TO = 8;
                            cphEquFromDB.LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt4);
                        }
                        else if (i == 5)
                        {
                            //     cphEquFromDB[i].AGE_FROM = 9;
                            //     cphEquFromDB[i].AGE_TO = 10;
                            //     cphEquFromDB[i].LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt5);

                            cphEquFromDB.AGE_FROM = 9;
                            cphEquFromDB.AGE_TO = 10;
                            cphEquFromDB.LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt5);
                        }
                        else if (i == 6)
                        {


                            //     cphEquFromDB[i].AGE_FROM = 11;
                            //     cphEquFromDB[i].AGE_TO = 12;
                            //     cphEquFromDB[i].LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt7);

                            cphEquFromDB.AGE_FROM = 11;
                            cphEquFromDB.AGE_TO = 12;
                            cphEquFromDB.LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt6);
                        }
                        else if (i == 7)
                        {
                            cphEquFromDB.AGE_FROM = 12;
                            cphEquFromDB.AGE_TO = 99;
                            cphEquFromDB.LIMIT_AMOUNT = Convert.ToDecimal(LimitAmt7);
                        }



                        cphEquFromDB.CHUSER = UserLogin;
                        cphEquFromDB.CHTS = DateTime.Now;

                        objContext.MESC1TS_CPH_EQP_LIMIT.Add(cphEquFromDB);
                        objContext.SaveChanges();



                    }


                }
                else
                {




                }



                //    List<CphEqpLimit> data = GetRSAllLimits(cphEqu.EqSize, cphEqu.ModeCode).ToList();
                //    if (data.Count == 0)
                //    {
                //        //     errormessage.ErrorMsg = "Approval Limit Data is Unavailable";
                //        //     errorMessageList.Add(errormessage);

                //    }
                //    else if (data.Count < 7)
                //    {

                //        //      errormessage.ErrorMsg = "Insert did not generate all 7 limit records";
                //        //     errorMessageList.Add(errormessage);
                //    }

                //}
            }
            catch (Exception ex)
            {
                //    errormessage.ErrorMsg = " ";
                //    errorMessageList.Add(errormessage);

                throw ex;

            }



            return cphEqyList;
        }

        public List<CphEqpLimit> UpdateCPHApprovalDetails(string EqSize, string ModeCode, string Age, string amt, string UserLogin)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_CPH_EQP_LIMIT> CphEqpListFromDB = new List<MESC1TS_CPH_EQP_LIMIT>();
            List<CphEqpLimit> CphEqpList = new List<CphEqpLimit>();

            //    errorMessageList = new List<ErrMessage>();
            //    ErrMessage errormessage = new ErrMessage();


            try
            {

                bool valid = false;
                valid = CheckDuplicate(Convert.ToString(ModeCode), EqSize);

                if (valid == true)
                {

                    short cphAge = Convert.ToInt16(Age);

                    CphEqpListFromDB = (from cphEqu in objContext.MESC1TS_CPH_EQP_LIMIT
                                        where cphEqu.MODE.Trim() == ModeCode
                                        && cphEqu.AGE_FROM == cphAge
                                        && cphEqu.EQSIZE == EqSize
                                        select cphEqu).ToList();

                    CphEqpListFromDB[0].LIMIT_AMOUNT = Convert.ToDecimal(amt);
                    CphEqpListFromDB[0].CHUSER = UserLogin;
                    CphEqpListFromDB[0].CHTS = DateTime.Now;
                    objContext.SaveChanges();

                }
                else
                {

                }




            }
            catch (Exception ex)
            {



            }







            return CphEqpList;
        }

        public bool CheckDuplicate(string Mode, string EqSize)
        {

            List<MESC1TS_CPH_EQP_LIMIT> CphEqpListFromDB = new List<MESC1TS_CPH_EQP_LIMIT>();

            CphEqpListFromDB = (from Cph in objContext.MESC1TS_CPH_EQP_LIMIT
                                where Cph.MODE == Mode
                                && Cph.EQSIZE == EqSize
                                select Cph).ToList();

            if (CphEqpListFromDB.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public bool IsCheckDuplicate(string EqSize, string Mode)
        {

            List<MESC1TS_CPH_EQP_LIMIT> CphEqpListFromDB = new List<MESC1TS_CPH_EQP_LIMIT>();

            CphEqpListFromDB = (from Cph in objContext.MESC1TS_CPH_EQP_LIMIT
                                where Cph.MODE == Mode
                                && Cph.EQSIZE == EqSize
                                select Cph).ToList();

            if (CphEqpListFromDB.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        #endregion

        #region EqTypeModeEntry

        public List<EqsType> GetSubType()
        {
            objContext = new ManageMasterDataServiceEntities();
            //  List<MESC1TS_EQMODE> EqModeFromDB = new List<MESC1TS_EQMODE>();
            List<EqsType> EqModeList = new List<EqsType>();

            try
            {
                var EqModeFromDB = (from C in objContext.MESC1TS_EQSTYPE
                                    orderby C.EQSTYPE
                                    select C).Distinct().ToList();

                for (int count = 0; count < EqModeFromDB.Count; count++)
                {

                    EqsType eqmode = new EqsType();
                    eqmode.EqSType = EqModeFromDB[count].EQSTYPE;
                    EqModeList.Add(eqmode);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return EqModeList;

        }

        public List<EqsType> GetEqType()
        {

            objContext = new ManageMasterDataServiceEntities();
            //  List<MESC1TS_EQMODE> EqModeFromDB = new List<MESC1TS_EQMODE>();
            List<EqsType> EqModeList = new List<EqsType>();

            try
            {
                var EqModeFromDB = (from C in objContext.MESC1TS_EQSTYPE
                                    orderby C.COTYPE
                                    select new { C.COTYPE }).Distinct().ToList();

                for (int count = 0; count < EqModeFromDB.Count; count++)
                {

                    EqsType eqmode = new EqsType();
                    eqmode.CoType = EqModeFromDB[count].COTYPE;
                    EqModeList.Add(eqmode);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return EqModeList;

        }

        public List<EqMode> GetRSAllEquModes(string EqType, string SubType, string Size, string Aluminium)
        {

            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_EQMODE> EqModeFromDB = new List<MESC1TS_EQMODE>();
            List<EqMode> EqModeList = new List<EqMode>();


            var Result = (from CL in objContext.MESC1TS_EQMODE
                          orderby CL.EQSTYPE, CL.EQSIZE
                          select CL).Distinct().ToList();

            bool IsEqType = false;
            bool IsSubType = false;
            bool IsSize = false;
            bool IsAluminium = false;

            try
            {

                if (!string.IsNullOrEmpty(EqType) || !string.IsNullOrEmpty(SubType) || !string.IsNullOrEmpty(Size) || !string.IsNullOrEmpty(Aluminium))
                {
                    if (EqType.Length > 0)
                    {



                        Result = (from CL in objContext.MESC1TS_EQMODE
                                  where CL.COTYPE == EqType
                                  orderby CL.EQSTYPE, CL.EQSIZE
                                  select CL).Distinct().ToList();


                        IsEqType = true;
                    }

                    if (SubType.Length > 0)
                    {
                        Result.Clear();

                        if (IsEqType == true)
                        {

                            Result = (from CL in objContext.MESC1TS_EQMODE
                                      where CL.EQSTYPE == SubType
                                      && CL.COTYPE == EqType
                                      orderby CL.EQSTYPE, CL.EQSIZE
                                      select CL).Distinct().ToList();




                        }
                        else
                        {
                            Result = (from CL in objContext.MESC1TS_EQMODE
                                      where CL.EQSTYPE == SubType
                                      orderby CL.EQSTYPE, CL.EQSIZE
                                      select CL).Distinct().ToList();
                        }

                        IsSubType = true;
                    }



                    if (Size.Length > 0)
                    {

                        Result.Clear();

                        if (IsSubType == true && IsEqType == true)
                        {
                            Result = (from CL in objContext.MESC1TS_EQMODE
                                      where CL.EQSTYPE == SubType
                                      && CL.COTYPE == EqType
                                      && CL.EQSIZE == Size
                                      orderby CL.EQSTYPE, CL.EQSIZE
                                      select CL).Distinct().ToList();
                            IsSize = true;

                        }
                        else if (IsSubType == false && IsEqType == false)
                        {
                            Result = (from CL in objContext.MESC1TS_EQMODE
                                      where CL.EQSIZE == Size
                                      orderby CL.EQSTYPE, CL.EQSIZE
                                      select CL).Distinct().ToList();

                            IsSize = true;
                        }
                        else if (IsSubType == true && IsEqType == false)
                        {
                            Result = (from CL in objContext.MESC1TS_EQMODE
                                      where CL.EQSIZE == Size
                                      && CL.EQSTYPE == SubType
                                      orderby CL.EQSTYPE, CL.EQSIZE
                                      select CL).Distinct().ToList();

                            IsSize = true;

                        }
                        else if (IsSubType == false && IsEqType == true)
                        {
                            Result = (from CL in objContext.MESC1TS_EQMODE
                                      where CL.EQSIZE == Size
                                      && CL.COTYPE == EqType
                                      orderby CL.EQSTYPE, CL.EQSIZE
                                      select CL).Distinct().ToList();

                            IsSize = true;
                        }
                        else
                        { }
                    }


                    if (Aluminium.Length > 0)
                    {
                        Result = Result.Where(q => q.ALUMINIUM_SW == Aluminium).ToList();
                    }


                    foreach (var obj in Result)
                    {
                        EqMode eqmode = new EqMode();
                        eqmode.EqsType = obj.EQSTYPE;
                        eqmode.CoType = obj.COTYPE;
                        eqmode.EqSize = obj.EQSIZE;
                        eqmode.EqModeID = obj.EQMODE_ID;

                        if (obj.ALUMINIUM_SW == "Y")
                        {
                            eqmode.AluminiumSW = "ALUMINIUM";
                        }
                        else
                        {
                            eqmode.AluminiumSW = "NON-ALUMINIUM";
                        }


                        eqmode.ModeCode = obj.MODE;
                        EqModeList.Add(eqmode);

                    }


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return EqModeList;






        }

        public List<Mode> GetRSAllModes()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> ModeListFromDB = new List<MESC1TS_MODE>();
            List<Mode> ModeList = new List<Mode>();



            try
            {
                ModeListFromDB = (from C in objContext.MESC1TS_MODE
                                  orderby C.MODE_IND, C.MODE
                                  select C).ToList();

                for (int count = 0; count < ModeListFromDB.Count; count++)
                {
                    Mode mode = new Mode();
                    mode.ModeCode = ModeListFromDB[count].MODE;
                    mode.ModeDescription = ModeListFromDB[count].MODE_DESC;
                    mode.ModeActiveSW = ModeListFromDB[count].MODE_ACTIVE_SW;
                    mode.ModeInd = ModeListFromDB[count].MODE_IND;
                    ModeList.Add(mode);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return ModeList;
        }

        public List<EqMode> GetRSByEqMode(string EqModeId)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_EQMODE> EqModeFromDB = new List<MESC1TS_EQMODE>();
            List<EqMode> EqModeList = new List<EqMode>();



            int getEqModeId = Convert.ToInt32(EqModeId);

            var EqModeFromDB1 = (from CL in objContext.MESC1TS_EQMODE
                                 where CL.EQMODE_ID == getEqModeId
                                 orderby CL.EQSTYPE, CL.EQSIZE
                                 select new { CL });

            if (EqModeFromDB1.Count() > 0)
            {
                foreach (var obj in EqModeFromDB1)
                {
                    EqMode eqmode = new EqMode();
                    eqmode.ChangeUser = obj.CL.CHUSER;
                    eqmode.ChangeTime = obj.CL.CHTS;
                    EqModeList.Add(eqmode);

                }
            }

            //for (int i = 0; i < EqModeFromDB1.Count; i++)
            //{
            //    EqMode eqmode = new EqMode();
            //    eqmode.ChangeUser = EqModeFromDB[0].CHUSER;
            //    eqmode.ChangeTime = EqModeFromDB[0].CHTS;
            //    EqModeList.Add(eqmode);
            //}

            return EqModeList;

        }

        public bool CreateEqTypeModeEntry(string CoType, string EqType, string EqSize, string Mode, string Aluminium, string Chuser)
        {
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_EQMODE EqModefromDB = new MESC1TS_EQMODE();


            try
            {

                EqModefromDB.COTYPE = CoType;
                EqModefromDB.EQSTYPE = EqType;
                EqModefromDB.EQSIZE = EqSize;
                EqModefromDB.MODE = Mode;
                EqModefromDB.ALUMINIUM_SW = Aluminium;
                EqModefromDB.CHUSER = Chuser;
                EqModefromDB.CHTS = DateTime.Now;
                objContext.MESC1TS_EQMODE.Add(EqModefromDB);
                objContext.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;

            }

            return true;
        }

        public bool UpdateEqTypeModeEntry(string EqModeId, string Mode, string Chuser)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_EQMODE> EqModeListFromDB = new List<MESC1TS_EQMODE>();
            List<EqMode> EqModeList = new List<EqMode>();

            int EqMode_Id = Convert.ToInt32(EqModeId);

            EqModeListFromDB = (from Con in objContext.MESC1TS_EQMODE
                                where Con.EQMODE_ID == EqMode_Id
                                select Con).ToList();

            EqModeListFromDB[0].MODE = Mode;
            EqModeListFromDB[0].CHUSER = Chuser;
            EqModeListFromDB[0].CHTS = DateTime.Now;

            objContext.SaveChanges();
            return true;
        }

        public List<EqMode> GetRSByAltKey(string CoType, string EqType, string EqSize, string Mode, string Aluminium)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_EQMODE> EqModeFromDB = new List<MESC1TS_EQMODE>();
            List<EqMode> EqModeList = new List<EqMode>();

            try
            {


                if (string.IsNullOrEmpty(EqSize))
                {
                    EqModeFromDB = (from CL in objContext.MESC1TS_EQMODE
                                    where CL.COTYPE == CoType
                                    && CL.EQSTYPE == EqType
                                    && CL.EQSIZE == null
                                    && CL.MODE == Mode
                                    && CL.ALUMINIUM_SW == Aluminium
                                    select CL).Distinct().ToList();
                }
                else
                {
                    EqModeFromDB = (from CL in objContext.MESC1TS_EQMODE
                                    where CL.COTYPE == CoType
                                    && CL.EQSTYPE == EqType
                                    && CL.EQSIZE == EqSize
                                    && CL.MODE == Mode
                                    && CL.ALUMINIUM_SW == Aluminium
                                    select CL).Distinct().ToList();
                }

                string getMode = string.Empty;
                if (EqModeFromDB.Count > 0)
                {
                    getMode = EqModeFromDB[0].MODE;

                }


                EqModeFromDB = new List<MESC1TS_EQMODE>();
                var Query = (from CL in objContext.MESC1TS_EQMODE
                             join innerOD in objContext.SEC_USER on CL.CHUSER equals innerOD.LOGIN into Inners
                             where CL.MODE == getMode
                             from SU in Inners.DefaultIfEmpty()
                             orderby CL.EQSTYPE, CL.EQSIZE
                             select new { CL, SU });

                var EqDataFromDB = (from q in Query
                                    select new
                                    {
                                        q.CL.COTYPE,
                                        q.CL.EQSTYPE,
                                        q.CL.EQSIZE,
                                        q.CL.MODE,
                                        q.CL.ALUMINIUM_SW,
                                        q.CL.CHTS,
                                        q.CL.CHUSER,
                                        q.SU.FIRSTNAME,
                                        q.SU.LASTNAME

                                    }).ToList();

                EqModeList.Clear();

                for (int count = 0; count < EqDataFromDB.Count; count++)
                {
                    EqMode eqmode = new EqMode();
                    eqmode.CoType = EqDataFromDB[count].COTYPE;
                    eqmode.EqsType = EqDataFromDB[count].EQSTYPE;
                    eqmode.EqSize = EqDataFromDB[count].EQSIZE;
                    eqmode.ModeCode = EqDataFromDB[count].MODE;
                    eqmode.AluminiumSW = EqDataFromDB[count].ALUMINIUM_SW;
                    eqmode.ChangeTime = EqDataFromDB[count].CHTS;
                    eqmode.ChangeUser = EqDataFromDB[count].FIRSTNAME + " " + EqDataFromDB[count].LASTNAME;
                    EqModeList.Add(eqmode);
                }

            }
            catch (Exception ex)
            {
                throw;
            }


            return EqModeList;
        }

        public bool EQCheckDuplicate(string CoType, string EqType, string EqSize, string Mode, string Aluminium)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_EQMODE> EqModeFromDB = new List<MESC1TS_EQMODE>();
            List<EqMode> EqModeList = new List<EqMode>();

            try
            {
                var Result = (from CL in objContext.MESC1TS_EQMODE
                              orderby CL.EQSTYPE, CL.EQSIZE
                              select CL).Distinct().ToList();


                if (CoType.Length > 0)
                {
                    Result = Result.Where(q => q.COTYPE == CoType).ToList();
                }

                if (EqType.Length > 0)
                {
                    Result = Result.Where(q => q.EQSTYPE == EqType).ToList();
                }

                if (EqSize.Length > 0)
                {
                    Result = Result.Where(q => q.EQSIZE == EqSize).ToList();
                }

                if (Mode.Length > 0)
                {
                    Result = Result.Where(q => q.MODE == Mode).ToList();
                }

                if (Aluminium.Length > 0)
                {
                    Result = Result.Where(q => q.ALUMINIUM_SW == Aluminium).ToList();
                }

                if (Result.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }



        }

        public bool EQCheckDuplicateByType(string CoType, string EqType)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_EQMODE> EqModeFromDB = new List<MESC1TS_EQMODE>();
            List<EqMode> EqModeList = new List<EqMode>();

            try
            {
                var Result = (from CL in objContext.MESC1TS_EQMODE
                              orderby CL.EQSTYPE, CL.EQSIZE
                              select CL).Distinct().ToList();


                if (CoType.Length > 0)
                {
                    Result = Result.Where(q => q.COTYPE == CoType).ToList();
                }

                if (EqType.Length > 0)
                {
                    Result = Result.Where(q => q.EQSTYPE == EqType).ToList();
                }



                if (Result.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool EQCheckDuplicateByMode(string Mode)
        {

            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_EQMODE> EqModeFromDB = new List<MESC1TS_EQMODE>();
            List<EqMode> EqModeList = new List<EqMode>();

            try
            {
                var Result = (from CL in objContext.MESC1TS_EQMODE
                              orderby CL.EQSTYPE, CL.EQSIZE
                              select CL).Distinct().ToList();




                if (Mode.Length > 0)
                {
                    Result = Result.Where(q => q.MODE == Mode).ToList();
                }


                if (Result.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool CheckDuplicateEqId(string EqId)
        {


            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_EQMODE> EqModeFromDB = new List<MESC1TS_EQMODE>();
            List<EqMode> EqModeList = new List<EqMode>();

            try
            {
                var Result = (from CL in objContext.MESC1TS_EQMODE
                              orderby CL.EQSTYPE, CL.EQSIZE
                              select CL).Distinct().ToList();

                int Eq_Id = Convert.ToInt32(EqId);


                if (EqId.Length > 0)
                {
                    Result = Result.Where(q => q.EQMODE_ID == Eq_Id).ToList();
                }


                if (Result.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<EqsType> GetSubTypeDetail(string SubType)
        {
            objContext = new ManageMasterDataServiceEntities();
            //  List<MESC1TS_EQMODE> EqModeFromDB = new List<MESC1TS_EQMODE>();
            List<EqsType> EqModeList = new List<EqsType>();

            try
            {
                var EqModeFromDB = (from C in objContext.MESC1TS_EQSTYPE
                                    where C.COTYPE == SubType
                                    orderby C.EQSTYPE, C.COTYPE
                                    select new { C.EQSTYPE }).Distinct().ToList();

                for (int count = 0; count < EqModeFromDB.Count; count++)
                {

                    EqsType eqmode = new EqsType();
                    eqmode.EqSType = EqModeFromDB[count].EQSTYPE;
                    EqModeList.Add(eqmode);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return EqModeList;


        }




        #endregion

        #region CustShopMode

        public List<Customer> Get_CustomerCode()
        {
            List<Customer> Customerlist = new List<Customer>();
            List<MESC1TS_CUSTOMER> CustomerFromDB = new List<MESC1TS_CUSTOMER>();


            CustomerFromDB = (from C in objContext.MESC1TS_CUSTOMER
                              orderby C.CUSTOMER_CD
                              select C).Distinct().ToList();

            foreach (var item in CustomerFromDB)
            {
                Customer customer = new Customer();
                customer.CustomerCode = item.CUSTOMER_CD;
                customer.CustomerDesc = item.CUSTOMER_DESC;
                Customerlist.Add(customer);
            }
            return Customerlist;
        }

        public List<Shop> Get_ShopCode()
        {
            List<Shop> ShopList = new List<Shop>();
            List<MESC1TS_SHOP> ShopListFromDB = new List<MESC1TS_SHOP>();

            ShopListFromDB = (from CS in objContext.MESC1TS_CUST_SHOP_MODE
                              from S in objContext.MESC1TS_SHOP
                              where S.SHOP_CD == CS.SHOP_CD
                              orderby S.SHOP_CD
                              select S).Distinct().ToList();

            foreach (var item in ShopListFromDB)
            {
                Shop shop = new Shop();
                shop.ShopCode = item.SHOP_CD;
                shop.ShopDescription = item.SHOP_DESC;
                ShopList.Add(shop);
            }
            return ShopList;
        }

        public List<Mode> Get_ModeList()
        {
            #region Get and Prepare Mode List

            List<MESC1TS_MODE> ModeListFromDB = new List<MESC1TS_MODE>();
            List<Mode> ModeList = new List<Mode>();

            ModeListFromDB = (from C in objContext.MESC1TS_MODE
                              orderby C.MODE_IND, C.MODE
                              select C).ToList();


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

        public List<CustShopMode> GetCSMList(string CustomerCode, string ShopCode, string Mode)
        {
            try
            {


                objContext = new ManageMasterDataServiceEntities();
                List<MESC1TS_CUST_SHOP_MODE> CSMList = new List<MESC1TS_CUST_SHOP_MODE>();
                List<CustShopMode> CustShopModeList = new List<CustShopMode>();


                var Query = (from csm in objContext.MESC1TS_CUST_SHOP_MODE
                             select csm).ToList();

                //now we can apply filters on ANY of the joined tables
                if (!string.IsNullOrEmpty(ShopCode))
                    Query = Query.Where(q => q.SHOP_CD == ShopCode).ToList();

                if (!string.IsNullOrEmpty(CustomerCode))
                    Query = Query.Where(q => q.CUSTOMER_CD == CustomerCode).ToList();

                if (!string.IsNullOrEmpty(Mode))
                    Query = Query.Where(q => q.MODE == Mode).ToList();


                var CustShopModeFromDB = (from q in Query
                                          select new
                                          {
                                              q.CUSTOMER_CD,
                                              q.SHOP_CD,
                                              q.MODE,
                                              q.PAYAGENT_CD,
                                              q.CORP_PAYAGENT_CD,
                                              q.RRIS_FORMAT,
                                              q.PROFIT_CENTER,
                                              q.SUB_PROFIT_CENTER,
                                              q.ACCOUNT_CD,
                                              q.CSM_CD

                                          }).ToList();


                foreach (var obj in CustShopModeFromDB)
                {
                    CustShopMode csm = new CustShopMode();
                    csm.CustomerCode = obj.CUSTOMER_CD.ToString();
                    csm.ShopCode = obj.SHOP_CD.ToString();
                    csm.ModeCode = obj.MODE;
                    csm.PayAgentCode = obj.PAYAGENT_CD;
                    csm.CorpPayAgentCode = obj.CORP_PAYAGENT_CD;
                    csm.RRISFormat = obj.RRIS_FORMAT;
                    csm.SubProfitCenter = obj.SUB_PROFIT_CENTER;
                    csm.ProfitCenter = obj.PROFIT_CENTER;
                    csm.AccountCode = obj.ACCOUNT_CD;
                    csm.CSMCode = obj.CSM_CD;

                    CustShopModeList.Add(csm);
                };


                return CustShopModeList;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public List<PayAgent> GetAllPayAgents()
        {
            List<PayAgent> PayAgentList = new List<PayAgent>();
            List<MESC1TS_PAYAGENT> PayAgentFromDB = new List<MESC1TS_PAYAGENT>();
            try
            {
                PayAgentFromDB = (from CS in objContext.MESC1TS_PAYAGENT
                                  orderby CS.PAYAGENT_CD
                                  select CS).Distinct().ToList();

                foreach (var item in PayAgentFromDB)
                {
                    PayAgent payagent = new PayAgent();
                    payagent.PayAgentCode = item.PAYAGENT_CD;
                    PayAgentList.Add(payagent);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return PayAgentList;
        }

        public List<CustShopMode> GetRsByCSM(string CSMCD)
        {

            try
            {
                objContext = new ManageMasterDataServiceEntities();

                List<CustShopMode> CustShopList = new List<CustShopMode>();
                var LimitLst = (from csm in objContext.MESC1TS_CUST_SHOP_MODE
                                where csm.CSM_CD == CSMCD
                                select new { csm }).ToList();

                foreach (var obj in LimitLst)
                {
                    CustShopMode CustShopMode = new CustShopMode();
                    CustShopMode.CSMCode = obj.csm.CSM_CD;
                    CustShopMode.ShopCode = obj.csm.SHOP_CD;
                    CustShopMode.ModeCode = obj.csm.MODE;
                    CustShopMode.PayAgentCode = obj.csm.PAYAGENT_CD;
                    CustShopMode.CorpPayAgentCode = obj.csm.CORP_PAYAGENT_CD;
                    CustShopMode.RRISFormat = obj.csm.RRIS_FORMAT;
                    CustShopMode.ProfitCenter = obj.csm.PROFIT_CENTER;
                    CustShopMode.SubProfitCenter = obj.csm.SUB_PROFIT_CENTER;
                    CustShopMode.AccountCode = obj.csm.ACCOUNT_CD;
                    CustShopMode.CustomerCode = obj.csm.CUSTOMER_CD;
                    CustShopMode.ChangeUser = obj.csm.CHUSER;
                    CustShopMode.ChangeTime = obj.csm.CHTS;
                    CustShopList.Add(CustShopMode);
                }
                return CustShopList;
            }
            catch (Exception)
            {

                throw;
            }



        }

        public bool ValidateProfitCenterByProfit(string strPayAgent, string strProfit)
        {
            try
            {

                objContext = new ManageMasterDataServiceEntities();
                List<CustShopMode> CustShopList = new List<CustShopMode>();



                return true;

            }
            catch (Exception ex)
            {
                throw;

            }

        }

        public bool ValidateProfitCenterBySubProfit(string strCorpPayAgent, string strSubProfit)
        {

            objContext = new ManageMasterDataServiceEntities();
            List<CustShopMode> CustShopList = new List<CustShopMode>();



            return true;
        }

        public bool InsertCustShopMode(string sCSMCd, string sCustomerCd, string sShopCd, string sMode, string sPayagentCd, string sCorpPayagentCd, string sRRISFormat, string sProfitCenter, string sSubProfitCenter, string sAccountCd, string sUser, ref string Msg)
        {
            objContext = new ManageMasterDataServiceEntities();
            CustShopMode CustShopList = new CustShopMode();
            MESC1TS_CUST_SHOP_MODE CustModefromDB = new MESC1TS_CUST_SHOP_MODE();
            bool IsSucess = false;

            try
            {
                var CSMListFromDB = (from csm in objContext.MESC1TS_CUST_SHOP_MODE
                                     where csm.CUSTOMER_CD == sCustomerCd &&
                                     csm.SHOP_CD == sShopCd &&
                                     csm.MODE == sMode
                                     select csm).ToList();
                if (CSMListFromDB.Count > 0)
                {
                    Msg = "Association " + sCustomerCd.Trim() + "/ " + sShopCd.Trim() + "/ " + sMode.Trim() + " already exists-Not Added";
                    return IsSucess;
                }
                else
                {
                    CustModefromDB.CSM_CD = sCSMCd;
                    CustModefromDB.CUSTOMER_CD = sCustomerCd;
                    CustModefromDB.MODE = sMode;
                    CustModefromDB.SHOP_CD = sShopCd;
                    CustModefromDB.PAYAGENT_CD = sPayagentCd;
                    CustModefromDB.CORP_PAYAGENT_CD = sCorpPayagentCd;
                    CustModefromDB.RRIS_FORMAT = sRRISFormat;
                    CustModefromDB.SUB_PROFIT_CENTER = sSubProfitCenter;
                    CustModefromDB.PROFIT_CENTER = sProfitCenter;
                    CustModefromDB.ACCOUNT_CD = sAccountCd;
                    CustModefromDB.CHUSER = sUser;
                    CustModefromDB.CHTS = DateTime.Now;
                    objContext.MESC1TS_CUST_SHOP_MODE.Add(CustModefromDB);
                    objContext.SaveChanges();
                    IsSucess = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return IsSucess;
        }

        public bool UpdateCustShopMode(string sCSMCd, string sCustomerCd, string sShopCd, string sMode, string sPayagentCd, string sCorpPayagentCd, string sRRISFormat, string sProfitCenter, string sSubProfitCenter, string sAccountCd, string sUser)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<CustShopMode> CustShopModeList = new List<CustShopMode>();
            List<MESC1TS_CUST_SHOP_MODE> CustShopModefromDB = new List<MESC1TS_CUST_SHOP_MODE>();
            bool IsSucess = false;

            try
            {

                CustShopModefromDB = (from CS in objContext.MESC1TS_CUST_SHOP_MODE
                                      where CS.CUSTOMER_CD == sCustomerCd &&
                                      CS.MODE == sMode &&
                                      CS.SHOP_CD == sShopCd
                                      select CS).Distinct().ToList();
                if (CustShopModefromDB.Count > 0)
                {
                    CustShopModefromDB[0].CUSTOMER_CD = sCustomerCd;
                    CustShopModefromDB[0].SHOP_CD = sShopCd;
                    CustShopModefromDB[0].MODE = sMode;
                    CustShopModefromDB[0].PAYAGENT_CD = sPayagentCd;
                    CustShopModefromDB[0].CORP_PAYAGENT_CD = sCorpPayagentCd;
                    CustShopModefromDB[0].RRIS_FORMAT = sRRISFormat;
                    CustShopModefromDB[0].PROFIT_CENTER = sProfitCenter;
                    CustShopModefromDB[0].SUB_PROFIT_CENTER = sSubProfitCenter;
                    CustShopModefromDB[0].ACCOUNT_CD = sAccountCd;
                    CustShopModefromDB[0].CHUSER = sUser;
                    CustShopModefromDB[0].CHTS = DateTime.Now;
                    objContext.SaveChanges();
                    IsSucess = true;
                }
                else
                {
                    IsSucess = false;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return IsSucess;


        }

        public bool CheckDuplicate(string sCustomerCd, string sShopCd, string sMode)
        {
            bool IsValid = false;

            try
            {
                List<CustShopMode> ShopModeList = new List<CustShopMode>();
                List<MESC1TS_CUST_SHOP_MODE> ShopModeListFromDB = new List<MESC1TS_CUST_SHOP_MODE>();

                ShopModeListFromDB = (from CS in objContext.MESC1TS_CUST_SHOP_MODE
                                      where CS.CUSTOMER_CD == sCustomerCd &&
                                      CS.MODE == sMode &&
                                      CS.SHOP_CD == sShopCd
                                      select CS).Distinct().ToList();

                if (ShopModeListFromDB.Count > 0)
                {

                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return IsValid;
        }

        public bool DeleteCsmCode(string CSMCD)
        {


            bool IsDelete = false;

            try
            {
                List<CustShopMode> CustShopModeList = new List<CustShopMode>();
                List<MESC1TS_CUST_SHOP_MODE> CustShopModeListFromDB = new List<MESC1TS_CUST_SHOP_MODE>();

                CustShopModeListFromDB = (from pay in objContext.MESC1TS_CUST_SHOP_MODE
                                          where pay.CSM_CD == CSMCD
                                          select pay).ToList();

                objContext.MESC1TS_CUST_SHOP_MODE.Remove(CustShopModeListFromDB.First());
                objContext.SaveChanges();
                IsDelete = true;



            }
            catch (Exception ex)
            {
                throw ex;

            }

            return IsDelete;
        }

        #endregion

        // ----------------------------End Ashiqur----------------------------------//





        #region PTI Periods
        public List<PTIPeriod> GetPTIPeriods(string SerialFrom)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_PTI> ptiList = new List<MESC1TS_PTI>();
            ptiList = (from pti in objContext.MESC1TS_PTI
                       where pti.SERIAL_NO_FROM.StartsWith(SerialFrom)
                       select pti).ToList();

            return PrepareDataContract(ptiList);

        }

        public PTIPeriod GetPTIPeriod(string SerialFrom, string SerialTo)
        {
            List<MESC1TS_PTI> ptiPeriods = GetPTIPeriods(SerialFrom, SerialTo);
            return PrepareDataContract(ptiPeriods, true)[0];
        }

        private List<MESC1TS_PTI> GetPTIPeriods(string SerialFrom, string SerialTo)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_PTI> ptiList = new List<MESC1TS_PTI>();
            ptiList = (from pti in objContext.MESC1TS_PTI
                       where pti.SERIAL_NO_FROM == SerialFrom && pti.SERIAL_NO_TO == SerialTo
                       select pti).ToList();

            return ptiList;

        }

        private List<PTIPeriod> PrepareDataContract(List<MESC1TS_PTI> ptiFromDB)
        {
            List<PTIPeriod> ptiList = new List<PTIPeriod>();
            for (int count = 0; count < ptiFromDB.Count; count++)
            {
                PTIPeriod pti = new PTIPeriod();
                pti.EqpNoFrom = ptiFromDB[count].SERIAL_NO_FROM;
                pti.EqpNoTo = ptiFromDB[count].SERIAL_NO_TO;
                pti.ExceptionDays = Convert.ToInt64(ptiFromDB[count].NO_OF_DAYS);

                pti.PTIChUser = ptiFromDB[count].CHUSER;
                pti.PTIChTime = ptiFromDB[count].CHTS;

                ptiList.Add(pti);
            }
            return ptiList;
        }

        private List<PTIPeriod> PrepareDataContract(List<MESC1TS_PTI> ptiFromDB, bool ShowUser)
        {
            List<PTIPeriod> ptiList = new List<PTIPeriod>();
            for (int count = 0; count < ptiFromDB.Count; count++)
            {
                PTIPeriod pti = new PTIPeriod();
                pti.EqpNoFrom = ptiFromDB[count].SERIAL_NO_FROM;
                pti.EqpNoTo = ptiFromDB[count].SERIAL_NO_TO;
                pti.ExceptionDays = Convert.ToInt64(ptiFromDB[count].NO_OF_DAYS);
                if (ShowUser)
                    pti.PTIChUser = GetUserNamePTI(ptiFromDB[count].CHUSER);
                else
                    pti.PTIChUser = ptiFromDB[count].CHUSER;
                pti.PTIChTime = ptiFromDB[count].CHTS;

                ptiList.Add(pti);
            }
            return ptiList;
        }

        public bool CreatePTIPeriod(PTIPeriod PTIPeriodFromClient, ref string msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_PTI> oldPti = GetPTIPeriods(PTIPeriodFromClient.EqpNoFrom, PTIPeriodFromClient.EqpNoTo);
            if (oldPti.Count > 0)
            {
                msg = "PTI Exception Period " + PTIPeriodFromClient.EqpNoFrom + " / " + PTIPeriodFromClient.EqpNoTo + " Already Exists - Not Added";
                return success;
            }
            MESC1TS_PTI ptiToBeInserted = new MESC1TS_PTI();
            ptiToBeInserted.SERIAL_NO_FROM = PTIPeriodFromClient.EqpNoFrom;
            ptiToBeInserted.SERIAL_NO_TO = PTIPeriodFromClient.EqpNoTo;
            ptiToBeInserted.NO_OF_DAYS = Convert.ToInt32(PTIPeriodFromClient.ExceptionDays);
            ptiToBeInserted.CHUSER = PTIPeriodFromClient.PTIChUser;
            ptiToBeInserted.CHTS = DateTime.Now;

            objContext.MESC1TS_PTI.Add(ptiToBeInserted);
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                msg = ex.Message;
            }

            return success;
        }

        public bool DeletePTIPeriod(PTIPeriod PTIPeriodFromClient, ref string msg)
        {
            bool sucess = false;
            objContext = new ManageMasterDataServiceEntities();
            // MESC1TS_PTI ptiToBeDeleted = new MESC1TS_PTI();
            List<MESC1TS_PTI> pTIDBObj = new List<MESC1TS_PTI>();
            pTIDBObj = (from pti in objContext.MESC1TS_PTI
                        where pti.SERIAL_NO_FROM == PTIPeriodFromClient.EqpNoFrom
                        && pti.SERIAL_NO_TO == PTIPeriodFromClient.EqpNoTo
                        select pti).ToList();
            objContext.MESC1TS_PTI.Remove(pTIDBObj.First());
            try
            {
                objContext.SaveChanges();
                sucess = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return sucess;
        }

        public bool ModifyPTIPeriod(PTIPeriod PTIPeriodFromClient, ref string msg)
        {
            bool sucess = false;
            objContext = new ManageMasterDataServiceEntities();
            //MESC1TS_PTI ptiToBeInserted = new MESC1TS_PTI();
            List<MESC1TS_PTI> pTIDBObj = new List<MESC1TS_PTI>();
            pTIDBObj = (from pti in objContext.MESC1TS_PTI
                        where pti.SERIAL_NO_FROM == PTIPeriodFromClient.EqpNoFrom
                        && pti.SERIAL_NO_TO == PTIPeriodFromClient.EqpNoTo
                        select pti).ToList();
            pTIDBObj[0].NO_OF_DAYS = Convert.ToInt16(PTIPeriodFromClient.ExceptionDays);
            pTIDBObj[0].CHUSER = PTIPeriodFromClient.PTIChUser;
            pTIDBObj[0].CHTS = DateTime.Now;
            try
            {
                objContext.SaveChanges();
                sucess = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return sucess;
        }

        public int GetPTIDefaultPeriod()
        {
            List<MESC1TS_PTI> pTIDBObj = new List<MESC1TS_PTI>();
            pTIDBObj = (from pti in objContext.MESC1TS_PTI
                        where pti.SERIAL_NO_FROM == "0"
                        && pti.SERIAL_NO_TO == "0"
                        select pti).ToList();
            if (pTIDBObj.Count > 0)
                return (int)pTIDBObj[0].NO_OF_DAYS;
            else
                return 0;
        }

        public PTIPeriod GetPTIDefaultPeriodRecord()
        {
            List<MESC1TS_PTI> pTIDBObj = new List<MESC1TS_PTI>();
            pTIDBObj = (from pti in objContext.MESC1TS_PTI
                        where pti.SERIAL_NO_FROM == "0"
                        && pti.SERIAL_NO_TO == "0"
                        select pti).ToList();
            if (pTIDBObj.Count > 0)
            {
                PTIPeriod period = PrepareDataContract(pTIDBObj)[0];
                string username = GetUserNamePTI(period.PTIChUser.ToString());
                if (username.Trim() != "")
                    period.PTIChUser = username;
                return period;
            }
            else
                return null;
        }

        public bool ModifyPTIDefaultPeriod(int NoOfDays, string UserID, ref string msg)
        {
            bool sucess = false;
            List<MESC1TS_PTI> pTIDBObj = new List<MESC1TS_PTI>();
            pTIDBObj = (from pti in objContext.MESC1TS_PTI
                        where pti.SERIAL_NO_FROM == "0"
                        && pti.SERIAL_NO_TO == "0"
                        select pti).ToList();
            pTIDBObj[0].NO_OF_DAYS = Convert.ToInt16(NoOfDays);
            pTIDBObj[0].CHUSER = UserID;
            pTIDBObj[0].CHTS = DateTime.Now;
            try
            {
                objContext.SaveChanges();
                sucess = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return sucess;
        }

        private string GetUserNamePTI(string UserID)
        {
            UserID = UserID == null ? "0" : UserID;
            try
            {
                UserID = int.Parse(UserID).ToString();
            }
            catch (Exception ex)
            {
                UserID = "0";
            }
            int id = int.Parse(UserID);
            List<SEC_USER> pTIDBObj = new List<SEC_USER>();
            pTIDBObj = (from user in objContext.SEC_USER
                        where user.USER_ID == id
                        select user).ToList();
            if (pTIDBObj.Count > 0)
                return pTIDBObj[0].FIRSTNAME + "|" + pTIDBObj[0].LASTNAME;
            else
                return "";
        }
        #endregion PTI Periods

        #region Special Remarks
        public bool InsertSpecialRemarks(SpecialRemarks SpecialRemarksFromClient, ref int ID, ref string msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            if (IsSpecialRemarksExists(SpecialRemarksFromClient.RKEMProfile, SpecialRemarksFromClient.SerialNoFrom, SpecialRemarksFromClient.SerialNoTo))
            {
                msg = "Special Remarks Already Exist, not Added";
                return success;
            }

            MESC1TS_SPECIAL_REMARKS sRemarksToBeInserted = new MESC1TS_SPECIAL_REMARKS();
            sRemarksToBeInserted.RKEM_PROFILE = SpecialRemarksFromClient.RKEMProfile;
            sRemarksToBeInserted.SERIAL_NO_TO = SpecialRemarksFromClient.SerialNoTo;
            sRemarksToBeInserted.SERIAL_NO_FROM = SpecialRemarksFromClient.SerialNoFrom;
            sRemarksToBeInserted.DISPLAY_SW = SpecialRemarksFromClient.DisplaySW;
            sRemarksToBeInserted.REMARKS = SpecialRemarksFromClient.Remarks;
            sRemarksToBeInserted.REPAIR_CEILING = SpecialRemarksFromClient.RepairCeiling;
            sRemarksToBeInserted.CHUSER = SpecialRemarksFromClient.ChangeUser;
            sRemarksToBeInserted.CHTS = DateTime.Now;

            objContext.MESC1TS_SPECIAL_REMARKS.Add(sRemarksToBeInserted);
            try
            {
                objContext.SaveChanges();
                success = true;
                ID = sRemarksToBeInserted.REMARKS_ID;
            }
            catch (Exception ex)
            {
                success = false;
                msg = ex.Message;
            }

            return success;
        }

        public bool ModifySpecialRemarks(SpecialRemarks SpecialRemarksFromClient, ref string msg)
        {
            bool sucess = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_SPECIAL_REMARKS> remarksDBObj = new List<MESC1TS_SPECIAL_REMARKS>();
            remarksDBObj = (from remarks in objContext.MESC1TS_SPECIAL_REMARKS
                            where remarks.REMARKS_ID == SpecialRemarksFromClient.RemarksID
                            select remarks).ToList();
            remarksDBObj[0].DISPLAY_SW = SpecialRemarksFromClient.DisplaySW.Trim();
            remarksDBObj[0].REMARKS = SpecialRemarksFromClient.Remarks.Trim();
            remarksDBObj[0].REPAIR_CEILING = SpecialRemarksFromClient.RepairCeiling;
            remarksDBObj[0].CHUSER = SpecialRemarksFromClient.ChangeUser;
            remarksDBObj[0].CHTS = DateTime.Now;
            try
            {
                objContext.SaveChanges();
                sucess = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return sucess;
        }

        public bool DeleteSpecialRemarks(SpecialRemarks SpecialRemarksFromClient, ref string msg)
        {
            bool sucess = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_SPECIAL_REMARKS> remarksDBObj = new List<MESC1TS_SPECIAL_REMARKS>();
            remarksDBObj = (from remarks in objContext.MESC1TS_SPECIAL_REMARKS
                            where remarks.REMARKS_ID == SpecialRemarksFromClient.RemarksID
                            select remarks).ToList();
            objContext.MESC1TS_SPECIAL_REMARKS.Remove(remarksDBObj.First());
            try
            {
                objContext.SaveChanges();
                sucess = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return sucess;
        }

        public SpecialRemarks GetSpecialRemarks(int RemarksID)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_SPECIAL_REMARKS> remarksList = new List<MESC1TS_SPECIAL_REMARKS>();
            remarksList = (from remarks in objContext.MESC1TS_SPECIAL_REMARKS
                           where remarks.REMARKS_ID == RemarksID
                           select remarks).ToList();

            SpecialRemarks specialRemarks = new SpecialRemarks();
            if (remarksList.Count > 0)
            {
                specialRemarks.RemarksID = remarksList[0].REMARKS_ID;
                specialRemarks.RKEMProfile = remarksList[0].RKEM_PROFILE;
                specialRemarks.SerialNoFrom = remarksList[0].SERIAL_NO_FROM;
                specialRemarks.SerialNoTo = remarksList[0].SERIAL_NO_TO;
                specialRemarks.DisplaySW = remarksList[0].DISPLAY_SW;
                specialRemarks.Remarks = remarksList[0].REMARKS;
                specialRemarks.RepairCeiling = remarksList[0].REPAIR_CEILING;
                specialRemarks.ChangeUser = GetUserNamePTI(remarksList[0].CHUSER);
                specialRemarks.ChangeTime = remarksList[0].CHTS;
            }

            return specialRemarks;
        }

        private bool IsSpecialRemarksExists(string ProfileID, string SerialFrom, string SerialTo)
        {
            bool recordExists = true;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_SPECIAL_REMARKS> remarksList = new List<MESC1TS_SPECIAL_REMARKS>();
            if (ProfileID != null && ProfileID.Trim() != "")
            {
                remarksList = (from remarks in objContext.MESC1TS_SPECIAL_REMARKS
                               where remarks.RKEM_PROFILE == ProfileID
                               select remarks).ToList();
                if (remarksList.Count > 0)
                    recordExists = true;
                else
                    recordExists = false;
            }
            else if ((SerialFrom != null && SerialFrom.Trim() != "") && (SerialTo != null && SerialTo.Trim() != ""))
            {
                remarksList = (from remarks in objContext.MESC1TS_SPECIAL_REMARKS
                               where remarks.SERIAL_NO_FROM == SerialFrom &&
                               remarks.SERIAL_NO_TO == SerialTo
                               select remarks).ToList();
                if (remarksList.Count > 0)
                    recordExists = true;
                else
                    recordExists = false;
            }
            return recordExists;
        }

        private List<MESC1TS_SPECIAL_REMARKS> GetAllSpecialRemarksSerials()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_SPECIAL_REMARKS> remarksList = new List<MESC1TS_SPECIAL_REMARKS>();
            remarksList = (from remarks in objContext.MESC1TS_SPECIAL_REMARKS
                           where remarks.SERIAL_NO_FROM != null
                           orderby remarks.SERIAL_NO_FROM
                           select remarks).ToList();
            return remarksList;
        }

        private List<MESC1TS_SPECIAL_REMARKS> GetAllSpecialRemarksProfiles()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_SPECIAL_REMARKS> remarksList = new List<MESC1TS_SPECIAL_REMARKS>();
            remarksList = (from remarks in objContext.MESC1TS_SPECIAL_REMARKS
                           where remarks.RKEM_PROFILE != null
                           orderby remarks.RKEM_PROFILE
                           select remarks).ToList();
            return remarksList;
        }

        public List<List<SpecialRemarks>> GetSpecialRemarksComboValue()
        {
            List<List<SpecialRemarks>> remarks = new List<List<SpecialRemarks>>();
            List<MESC1TS_SPECIAL_REMARKS> profileDB = GetAllSpecialRemarksProfiles();
            List<SpecialRemarks> profile = new List<SpecialRemarks>();
            foreach (MESC1TS_SPECIAL_REMARKS sRemarks in profileDB)
            {
                SpecialRemarks r = new SpecialRemarks();
                r.RemarksID = sRemarks.REMARKS_ID;
                r.RKEMProfile = sRemarks.RKEM_PROFILE;
                profile.Add(r);
            }
            remarks.Add(profile);

            profileDB = GetAllSpecialRemarksSerials();
            profile = new List<SpecialRemarks>();
            foreach (MESC1TS_SPECIAL_REMARKS sRemarks in profileDB)
            {
                SpecialRemarks r = new SpecialRemarks();
                r.RemarksID = sRemarks.REMARKS_ID;
                r.RKEMProfile = sRemarks.SERIAL_NO_FROM + " - " + sRemarks.SERIAL_NO_TO;
                profile.Add(r);
            }
            remarks.Add(profile);
            return remarks;
        }
        #endregion Special Remarks

        #region Repair STS Code
        public List<Manual> GetRepairCodeActiveIndexManual()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUAL> manualList = new List<MESC1TS_MANUAL>();
            manualList = (from ma in objContext.MESC1TS_MANUAL
                          join i in objContext.MESC1TS_INDEX
                          on ma.MANUAL_CD equals i.MANUAL_CD
                          where ma.MANUAL_ACTIVE_SW == "Y"
                          orderby ma.MANUAL_DESC
                          select ma).Distinct().ToList();
            List<Manual> ManualCodeList = new List<Manual>();
            for (int count = 0; count < manualList.Count; count++)
            {
                Manual manual = new Manual();
                manual.ManualCode = manualList[count].MANUAL_CD;
                manual.ManualDesc = manualList[count].MANUAL_CD + " - " + manualList[count].MANUAL_DESC;
                ManualCodeList.Add(manual);
            }

            return ManualCodeList;
        }
        public List<Manual> GetRepairCodeManual()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUAL> manualList = new List<MESC1TS_MANUAL>();
            manualList = (from nd in objContext.MESC1TS_REPAIR_CODE
                          join ma in objContext.MESC1TS_MANUAL
                              on nd.MANUAL_CD equals ma.MANUAL_CD
                          orderby ma.MANUAL_DESC
                          select ma).Distinct().ToList();
            List<Manual> ManualCodeList = new List<Manual>();
            for (int count = 0; count < manualList.Count; count++)
            {
                Manual manual = new Manual();
                manual.ManualCode = manualList[count].MANUAL_CD;
                manual.ManualDesc = manualList[count].MANUAL_CD + " - " + manualList[count].MANUAL_DESC;
                ManualCodeList.Add(manual);
            }

            return ManualCodeList;
        }

        public List<Mode> GetRepairCodeMode(string ManualCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> modeList = new List<MESC1TS_MODE>();
            modeList = (from nd in objContext.MESC1TS_REPAIR_CODE
                        join ma in objContext.MESC1TS_MODE
                        on nd.MODE equals ma.MODE
                        where nd.MANUAL_CD == ManualCode
                        select ma).Distinct().ToList();
            List<Mode> ModeCodeList = new List<Mode>();
            for (int count = 0; count < modeList.Count; count++)
            {
                Mode mode = new Mode();
                mode.ModeCode = modeList[count].MODE;
                mode.ModeDescription = modeList[count].MODE + " - " + modeList[count].MODE_DESC;
                ModeCodeList.Add(mode);
            }

            return ModeCodeList;
        }

        public List<Mode> GetRepairCodeIndexMode(string ManualCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> modeList = new List<MESC1TS_MODE>();
            modeList = (from nd in objContext.MESC1TS_INDEX
                        join ma in objContext.MESC1TS_MODE
                        on nd.MODE equals ma.MODE
                        where nd.MANUAL_CD == ManualCode
                        select ma).Distinct().ToList();
            List<Mode> ModeCodeList = new List<Mode>();
            for (int count = 0; count < modeList.Count; count++)
            {
                Mode mode = new Mode();
                mode.ModeCode = modeList[count].MODE;
                mode.ModeDescription = modeList[count].MODE + " - " + modeList[count].MODE_DESC;
                ModeCodeList.Add(mode);
            }

            return ModeCodeList;
        }

        public List<RepairCode> GetRepairCodeByIndex(string ManualCode, string ModeCode, int IndexId)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_REPAIR_CODE> repairCodes = new List<MESC1TS_REPAIR_CODE>();
            repairCodes = (from rc in objContext.MESC1TS_REPAIR_CODE
                           where rc.MANUAL_CD == ManualCode &&
                           rc.MODE == ModeCode &&
                           rc.INDEX_ID == IndexId
                           select rc).ToList();
            return PrepareRepairCodeDataContract(repairCodes, true);
        }

        public List<RepairCode> GetRepairCodeByMode(string ManualCode, string ModeCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_REPAIR_CODE> repairCodes = new List<MESC1TS_REPAIR_CODE>();
            repairCodes = (from rc in objContext.MESC1TS_REPAIR_CODE
                           where rc.MANUAL_CD == ManualCode &&
                           rc.MODE == ModeCode
                           select rc).ToList();

            List<RepairCode> RepairCodeLists = new List<RepairCode>();
            for (int count = 0; count < repairCodes.Count; count++)
            {
                RepairCode rc = new RepairCode();
                rc.RepairCod = repairCodes[count].REPAIR_CD;
                rc.RepairDesc = repairCodes[count].REPAIR_CD.Trim() + " - " + repairCodes[count].REPAIR_DESC;
                RepairCodeLists.Add(rc);
            }
            return RepairCodeLists;
        }

        public List<RepairCode> GetRepairCodeByRepairCode(string ManualCode, string ModeCode, string RepairCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_REPAIR_CODE> repairCodes = new List<MESC1TS_REPAIR_CODE>();
            repairCodes = (from rc in objContext.MESC1TS_REPAIR_CODE
                           where rc.MANUAL_CD == ManualCode &&
                           rc.MODE == ModeCode &&
                           rc.REPAIR_CD == RepairCode
                           select rc).ToList();
            return PrepareRepairCodeDataContract(repairCodes, true);
        }

        public bool InsertRepairCode(RepairCode RepairCodeFromClient, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<RepairCode> oldRC = GetRepairCodeByRepairCode(RepairCodeFromClient.ManualCode, RepairCodeFromClient.ModeCode, RepairCodeFromClient.RepairCod);
            if (oldRC.Count > 0)
            {
                Msg = "Repair Code already exists - Not Added";
                return success;
            }
            MESC1TS_REPAIR_CODE rCToBeInserted = new MESC1TS_REPAIR_CODE();
            rCToBeInserted.MANUAL_CD = RepairCodeFromClient.ManualCode;
            rCToBeInserted.MODE = RepairCodeFromClient.ModeCode;
            rCToBeInserted.REPAIR_CD = RepairCodeFromClient.RepairCod;
            rCToBeInserted.INDEX_ID = RepairCodeFromClient.IndexID;
            rCToBeInserted.REPAIR_DESC = RepairCodeFromClient.RepairDesc;
            rCToBeInserted.RKRP_REPAIR_CD = RepairCodeFromClient.RkrpRepairCode;
            rCToBeInserted.REPAIR_PRIORITY = RepairCodeFromClient.RepairPriority;
            rCToBeInserted.MAX_QUANTITY = RepairCodeFromClient.MaxQuantity;
            rCToBeInserted.SHOP_MATERIAL_CEILING = RepairCodeFromClient.ShopMaterialCeiling;
            rCToBeInserted.REPAIR_IND = RepairCodeFromClient.RepairInd;
            rCToBeInserted.MAN_HOUR = RepairCodeFromClient.ManHour;
            rCToBeInserted.REPAIR_ACTIVE_SW = RepairCodeFromClient.RepairActiveSW;
            rCToBeInserted.MULTIPLE_UPDATE_SW = RepairCodeFromClient.MultipleUpdateSW;
            rCToBeInserted.WARRANTY_PERIOD = RepairCodeFromClient.WarrantyPeriod;
            rCToBeInserted.TAX_APPLIED_SW = RepairCodeFromClient.TaxAppliedSW;
            rCToBeInserted.ALLOW_PARTS_SW = RepairCodeFromClient.AllowPartsSW;
            rCToBeInserted.CHUSER = RepairCodeFromClient.ChangeUser;
            rCToBeInserted.CHTS = DateTime.Now;

            objContext.MESC1TS_REPAIR_CODE.Add(rCToBeInserted);
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                Msg = ex.Message;
            }

            return success;
        }

        public bool ModifyRepairCode(RepairCode RepairCodeFromClient, ref string Msg)
        {
            bool success = false;
            List<RepairCode> oldData = GetRepairCodeByRepairCode(RepairCodeFromClient.ManualCode, RepairCodeFromClient.ModeCode, RepairCodeFromClient.RepairCod);
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_REPAIR_CODE> rCDBObj = new List<MESC1TS_REPAIR_CODE>();
            rCDBObj = (from rc in objContext.MESC1TS_REPAIR_CODE
                       where rc.MANUAL_CD == RepairCodeFromClient.ManualCode &&
                       rc.MODE == RepairCodeFromClient.ModeCode &&
                       rc.REPAIR_CD == RepairCodeFromClient.RepairCod
                       select rc).ToList();
            //MESC1TS_REPAIR_CODE rCToBeInserted = new MESC1TS_REPAIR_CODE();
            //rCDBObj[0].MANUAL_CD = RepairCodeFromClient.ManualCode;
            //rCDBObj[0].MODE = RepairCodeFromClient.ModeCode;
            //rCDBObj[0].REPAIR_CD = RepairCodeFromClient.RepairCod;
            rCDBObj[0].INDEX_ID = RepairCodeFromClient.IndexID;
            rCDBObj[0].REPAIR_DESC = RepairCodeFromClient.RepairDesc;
            rCDBObj[0].RKRP_REPAIR_CD = RepairCodeFromClient.RkrpRepairCode;
            rCDBObj[0].REPAIR_PRIORITY = RepairCodeFromClient.RepairPriority;
            rCDBObj[0].MAX_QUANTITY = RepairCodeFromClient.MaxQuantity;
            rCDBObj[0].SHOP_MATERIAL_CEILING = RepairCodeFromClient.ShopMaterialCeiling;
            rCDBObj[0].REPAIR_IND = RepairCodeFromClient.RepairInd;
            rCDBObj[0].MAN_HOUR = RepairCodeFromClient.ManHour;
            rCDBObj[0].REPAIR_ACTIVE_SW = RepairCodeFromClient.RepairActiveSW;
            rCDBObj[0].MULTIPLE_UPDATE_SW = RepairCodeFromClient.MultipleUpdateSW;
            rCDBObj[0].WARRANTY_PERIOD = RepairCodeFromClient.WarrantyPeriod;
            rCDBObj[0].TAX_APPLIED_SW = RepairCodeFromClient.TaxAppliedSW;
            rCDBObj[0].ALLOW_PARTS_SW = RepairCodeFromClient.AllowPartsSW;
            rCDBObj[0].CHUSER = RepairCodeFromClient.ChangeUser;
            rCDBObj[0].CHTS = DateTime.Now;
            try
            {
                objContext.SaveChanges();
                RepairCodeFromClient.ChangeTime = rCDBObj[0].CHTS;
                InsertRepairCodeAuditTrail(RepairCodeFromClient, oldData);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                Msg = ex.Message;
            }

            return success;

        }

        public bool DeleteRepairCode(RepairCode RepairCodeFromClient, ref string msg)
        {
            bool sucess = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_REPAIR_CODE> rCDBObj = new List<MESC1TS_REPAIR_CODE>();
            rCDBObj = (from rc in objContext.MESC1TS_REPAIR_CODE
                       where rc.MANUAL_CD == RepairCodeFromClient.ManualCode &&
                       rc.MODE == RepairCodeFromClient.ModeCode &&
                       rc.REPAIR_CD == RepairCodeFromClient.RepairCod
                       select rc).ToList();
            objContext.MESC1TS_REPAIR_CODE.Remove(rCDBObj.First());
            try
            {
                objContext.SaveChanges();
                sucess = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return sucess;
        }

        private List<RepairCode> PrepareRepairCodeDataContract(List<MESC1TS_REPAIR_CODE> RepairCodes)
        {
            List<RepairCode> rCodes = new List<RepairCode>();
            RepairCode rCode;
            foreach (MESC1TS_REPAIR_CODE dbRC in RepairCodes)
            {
                rCode = new RepairCode();
                rCode.AllowPartsSW = dbRC.ALLOW_PARTS_SW;
                rCode.ChangeTime = dbRC.CHTS;
                rCode.ChangeUser = dbRC.CHUSER;
                rCode.IndexID = dbRC.INDEX_ID;
                rCode.ManHour = dbRC.MAN_HOUR;
                rCode.ManualCode = dbRC.MANUAL_CD;
                rCode.MaxQuantity = dbRC.MAX_QUANTITY;
                //rCode.Manual = dbRC.MESC1TS_MANUAL;
                //rCode.Mode = dbRC.MODE;
                rCode.MaxQuantity = dbRC.MAX_QUANTITY;
                rCode.ModeCode = dbRC.MODE;
                rCode.MultipleUpdateSW = dbRC.MULTIPLE_UPDATE_SW;
                rCode.RepairActiveSW = dbRC.REPAIR_ACTIVE_SW;
                rCode.RepairCod = dbRC.REPAIR_CD;
                rCode.RepairDesc = dbRC.REPAIR_DESC;
                rCode.RepairInd = dbRC.REPAIR_IND;
                rCode.RepairPriority = dbRC.REPAIR_PRIORITY;
                rCode.RkrpRepairCode = dbRC.RKRP_REPAIR_CD;
                rCode.ShopMaterialCeiling = dbRC.SHOP_MATERIAL_CEILING;
                rCode.TaxAppliedSW = dbRC.TAX_APPLIED_SW;
                rCode.WarrantyPeriod = dbRC.WARRANTY_PERIOD;
                rCodes.Add(rCode);

            }
            return rCodes;
        }

        private List<RepairCode> PrepareRepairCodeDataContract(List<MESC1TS_REPAIR_CODE> RepairCodes, bool ShowUser)
        {
            List<RepairCode> rCodes = new List<RepairCode>();
            RepairCode rCode;
            foreach (MESC1TS_REPAIR_CODE dbRC in RepairCodes)
            {
                rCode = new RepairCode();
                rCode.AllowPartsSW = dbRC.ALLOW_PARTS_SW;
                rCode.ChangeTime = dbRC.CHTS;
                rCode.ChangeUser = ShowUser ? GetUserNamePTI(dbRC.CHUSER) : dbRC.CHUSER;
                rCode.IndexID = dbRC.INDEX_ID;
                rCode.ManHour = dbRC.MAN_HOUR;
                rCode.ManualCode = dbRC.MANUAL_CD;
                rCode.MaxQuantity = dbRC.MAX_QUANTITY;
                //rCode.Manual = dbRC.MESC1TS_MANUAL;
                //rCode.Mode = dbRC.MODE;
                rCode.MaxQuantity = dbRC.MAX_QUANTITY;
                rCode.ModeCode = dbRC.MODE;
                rCode.MultipleUpdateSW = dbRC.MULTIPLE_UPDATE_SW;
                rCode.RepairActiveSW = dbRC.REPAIR_ACTIVE_SW;
                rCode.RepairCod = dbRC.REPAIR_CD;
                rCode.RepairDesc = dbRC.REPAIR_DESC;
                rCode.RepairInd = dbRC.REPAIR_IND;
                rCode.RepairPriority = dbRC.REPAIR_PRIORITY;
                rCode.RkrpRepairCode = dbRC.RKRP_REPAIR_CD;
                rCode.ShopMaterialCeiling = dbRC.SHOP_MATERIAL_CEILING;
                rCode.TaxAppliedSW = dbRC.TAX_APPLIED_SW;
                rCode.WarrantyPeriod = dbRC.WARRANTY_PERIOD;

                rCodes.Add(rCode);

            }
            return rCodes;
        }

        private bool InsertRepairCodeAuditTrail(RepairCode NewRecord, List<RepairCode> OldRecord)
        {
            bool success = false;
            string UniqueKey = NewRecord.ManualCode + NewRecord.ModeCode + NewRecord.RepairCod;
            MESC1TS_REFAUDIT record;
            objContext = new ManageMasterDataServiceEntities();
            if (OldRecord[0].RepairDesc != NewRecord.RepairDesc)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_REPAIR_CODE", "REPAIR_DESC", OldRecord[0].RepairDesc, NewRecord.RepairDesc, NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].RkrpRepairCode != NewRecord.RkrpRepairCode)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_REPAIR_CODE", "RKRP_REPAIR_CD", OldRecord[0].RkrpRepairCode, NewRecord.RkrpRepairCode, NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].MaxQuantity != NewRecord.MaxQuantity)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_REPAIR_CODE", "MAX_QUANTITY", OldRecord[0].MaxQuantity.ToString(), NewRecord.MaxQuantity.ToString(), NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].ShopMaterialCeiling != NewRecord.ShopMaterialCeiling)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_REPAIR_CODE", "SHOP_MATERIAL_CEILING", OldRecord[0].ShopMaterialCeiling.ToString(), NewRecord.ShopMaterialCeiling.ToString(), NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].RepairInd != NewRecord.RepairInd)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_REPAIR_CODE", "REPAIR_IND", OldRecord[0].RepairInd, NewRecord.RepairInd, NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].ManHour != NewRecord.ManHour)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_REPAIR_CODE", "MAN_HOUR", OldRecord[0].ManHour.ToString(), NewRecord.ManHour.ToString(), NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].RepairActiveSW != NewRecord.RepairActiveSW)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_REPAIR_CODE", "REPAIR_ACTIVE_SW", OldRecord[0].RepairActiveSW, NewRecord.RepairActiveSW, NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].MultipleUpdateSW != NewRecord.MultipleUpdateSW)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_REPAIR_CODE", "MULTIPLE_UPDATE_SW", OldRecord[0].MultipleUpdateSW, NewRecord.MultipleUpdateSW, NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].WarrantyPeriod != NewRecord.WarrantyPeriod)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_REPAIR_CODE", "WARRANTY_PERIOD", OldRecord[0].WarrantyPeriod.ToString(), NewRecord.WarrantyPeriod.ToString(), NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].TaxAppliedSW != NewRecord.TaxAppliedSW)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_REPAIR_CODE", "TAX_APPLIED_SW", OldRecord[0].TaxAppliedSW, NewRecord.TaxAppliedSW, NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].RepairPriority != NewRecord.RepairPriority)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_REPAIR_CODE", "REPAIR_PRIORITY", OldRecord[0].RepairPriority.ToString(), NewRecord.RepairPriority.ToString(), NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].AllowPartsSW != NewRecord.AllowPartsSW)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_REPAIR_CODE", "ALLOW_PARTS_SW", OldRecord[0].AllowPartsSW, NewRecord.AllowPartsSW, NewRecord.ChangeUser, NewRecord.ChangeTime, UniqueKey);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }
        #endregion Repair STS Code

        #region ManualMode
        public bool InsertManualMode(ManualMode ManualModeFromClient, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<ManualMode> oldRC = GetManualMode(ManualModeFromClient.ManualCode, ManualModeFromClient.ModeCode);
            if (oldRC.Count > 0)
            {
                Msg = "Manual Mode " + ManualModeFromClient.ManualCode + " / " + ManualModeFromClient.ModeCode + " already exists - Not Added";
                return success;
            }
            MESC1TS_MANUAL_MODE mmToBeInserted = new MESC1TS_MANUAL_MODE();
            mmToBeInserted.MANUAL_CD = ManualModeFromClient.ManualCode;
            mmToBeInserted.MODE = ManualModeFromClient.ModeCode;
            mmToBeInserted.ACTIVE_SW = ManualModeFromClient.ActiveSw;
            mmToBeInserted.CHUSER = ManualModeFromClient.ChangeUser;
            mmToBeInserted.CHTS = DateTime.Now;

            objContext.MESC1TS_MANUAL_MODE.Add(mmToBeInserted);
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                Msg = ex.Message;
            }

            return success;
        }

        public bool DeleteManualMode(ManualMode ManualModeFromClient, ref string msg)
        {
            bool sucess = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUAL_MODE> mmDBObj = new List<MESC1TS_MANUAL_MODE>();
            mmDBObj = (from mm in objContext.MESC1TS_MANUAL_MODE
                       where mm.MANUAL_CD == ManualModeFromClient.ManualCode &&
                       mm.MODE == ManualModeFromClient.ModeCode
                       select mm).ToList();
            objContext.MESC1TS_MANUAL_MODE.Remove(mmDBObj.First());
            try
            {
                objContext.SaveChanges();
                sucess = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return sucess;
        }

        public bool ModifyManualMode(ManualMode ManualModeFromClient, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUAL_MODE> mmDBObj = new List<MESC1TS_MANUAL_MODE>();
            mmDBObj = (from mm in objContext.MESC1TS_MANUAL_MODE
                       where mm.MANUAL_CD == ManualModeFromClient.ManualCode &&
                       mm.MODE == ManualModeFromClient.ModeCode
                       select mm).ToList();

            mmDBObj[0].ACTIVE_SW = ManualModeFromClient.ActiveSw;
            mmDBObj[0].CHUSER = ManualModeFromClient.ChangeUser;
            mmDBObj[0].CHTS = DateTime.Now;
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                Msg = ex.Message;
            }

            return success;

        }

        public List<ManualMode> GetManualMode(string ManualCode, string ModeCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUAL_MODE> manualModes = new List<MESC1TS_MANUAL_MODE>();
            manualModes = (from mm in objContext.MESC1TS_MANUAL_MODE
                           where mm.MANUAL_CD == ManualCode &&
                           mm.MODE == ModeCode
                           select mm).ToList();
            return PrepareManualModeDataContract(manualModes);
        }

        public List<Manual> GetManualModeManual()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUAL> manualList = new List<MESC1TS_MANUAL>();
            manualList = (from nd in objContext.MESC1TS_MANUAL_MODE
                          join ma in objContext.MESC1TS_MANUAL
                              on nd.MANUAL_CD equals ma.MANUAL_CD
                          orderby ma.MANUAL_DESC
                          select ma).Distinct().ToList();
            List<Manual> ManualCodeList = new List<Manual>();
            for (int count = 0; count < manualList.Count; count++)
            {
                Manual manual = new Manual();
                manual.ManualCode = manualList[count].MANUAL_CD;
                manual.ManualDesc = manualList[count].MANUAL_CD + " - " + manualList[count].MANUAL_DESC;
                ManualCodeList.Add(manual);
            }

            return ManualCodeList;
        }

        public List<Manual> GetAllManual()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUAL> manualList = new List<MESC1TS_MANUAL>();
            manualList = (from nd in objContext.MESC1TS_MANUAL
                          orderby nd.MANUAL_CD
                          select nd).Distinct().ToList();
            List<Manual> ManualCodeList = new List<Manual>();
            for (int count = 0; count < manualList.Count; count++)
            {
                if (manualList[count].MANUAL_ACTIVE_SW == "Y")
                {
                    Manual manual = new Manual();
                    manual.ManualCode = manualList[count].MANUAL_CD;
                    manual.ManualDesc = manualList[count].MANUAL_CD + " - " + manualList[count].MANUAL_DESC;
                    ManualCodeList.Add(manual);
                }
            }

            return ManualCodeList;
        }

        public List<Mode> GetManualModeMode(string ManualCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> modeList = new List<MESC1TS_MODE>();
            modeList = (from nd in objContext.MESC1TS_MANUAL_MODE
                        join ma in objContext.MESC1TS_MODE
                        on nd.MODE equals ma.MODE
                        where nd.MANUAL_CD == ManualCode
                        orderby ma.MODE
                        select ma).Distinct().ToList();
            List<Mode> ModeCodeList = new List<Mode>();
            for (int count = 0; count < modeList.Count; count++)
            {
                if (modeList[count].MODE_ACTIVE_SW == "Y")
                {
                    Mode mode = new Mode();
                    mode.ModeCode = modeList[count].MODE;
                    mode.ModeDescription = modeList[count].MODE + " - " + modeList[count].MODE_DESC;
                    ModeCodeList.Add(mode);
                }
            }

            return ModeCodeList;
        }

        public List<Mode> GetAllMode()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> modeList = new List<MESC1TS_MODE>();
            modeList = (from ma in objContext.MESC1TS_MODE
                        where ma.MODE_ACTIVE_SW == "Y"
                        select ma).Distinct().ToList();
            List<Mode> ModeCodeList = new List<Mode>();
            for (int count = 0; count < modeList.Count; count++)
            {
                Mode mode = new Mode();
                mode.ModeCode = modeList[count].MODE;
                mode.ModeDescription = modeList[count].MODE + " - " + modeList[count].MODE_DESC;
                ModeCodeList.Add(mode);
            }

            return ModeCodeList;
        }

        private List<ManualMode> PrepareManualModeDataContract(List<MESC1TS_MANUAL_MODE> ManualModes)
        {
            List<ManualMode> manualModes = new List<ManualMode>();
            ManualMode mm;
            foreach (MESC1TS_MANUAL_MODE dbMM in ManualModes)
            {
                mm = new ManualMode();
                mm.ActiveSw = dbMM.ACTIVE_SW;
                mm.ChangeTime = dbMM.CHTS;
                mm.ChangeUser = GetUserNamePTI(dbMM.CHUSER);
                mm.ManualCode = dbMM.MANUAL_CD;
                mm.ModeCode = dbMM.MODE;

                manualModes.Add(mm);

            }
            return manualModes;
        }
        #endregion ManualMode

        #region Mode
        private List<Mode> PrepareDataContract(List<MESC1TS_MODE> modeFromDB, bool IsComboLoad)
        {
            List<Mode> modeList = new List<Mode>();
            for (int count = 0; count < modeFromDB.Count; count++)
            {
                Mode mode = new Mode();
                if (IsComboLoad)
                {
                    mode.ModeCode = modeFromDB[count].MODE;
                    mode.ModeFullDescription = modeFromDB[count].MODE + '-' + modeFromDB[count].MODE_DESC;
                }
                else
                {
                    mode.ModeCode = modeFromDB[count].MODE;
                    mode.ModeFullDescription = modeFromDB[count].MODE + '-' + modeFromDB[count].MODE_DESC;
                    mode.ModeDescription = modeFromDB[count].MODE_DESC;
                    mode.ChangeTime = modeFromDB[count].CHTS;
                    mode.ModeActiveSW = modeFromDB[count].MODE_ACTIVE_SW;
                    mode.ModeInd = modeFromDB[count].MODE_IND;
                    mode.ChangeUser = GetUserNamePTI(modeFromDB[count].CHUSER);
                }
                modeList.Add(mode);
            }
            return modeList;
        }
        public List<Mode> GetModes()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> Mode = new List<MESC1TS_MODE>();
            Mode = (from mode in objContext.MESC1TS_MODE
                    where mode.MODE_ACTIVE_SW == "Y"
                    select mode).ToList();


            return PrepareDataContract(Mode, true);

        }

        public List<Mode> GetAllActiveInActiveModes()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> Mode = new List<MESC1TS_MODE>();
            Mode = (from mode in objContext.MESC1TS_MODE
                    select mode).ToList();

            return PrepareDataContract(Mode, true);
        }

        public List<Mode> GetMode(string ModeCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MODE> Mode = new List<MESC1TS_MODE>();
            Mode = (from mode in objContext.MESC1TS_MODE
                    where mode.MODE == ModeCode
                    select mode).ToList();


            return PrepareDataContract(Mode, false);

        }
        public bool UpdateMode(Mode ModeToBeUpdated, ref string Msg)
        {
            bool sucess = false;
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_MODE> modeDBObject = new List<MESC1TS_MODE>();
            modeDBObject = (from mode in objContext.MESC1TS_MODE
                            where mode.MODE == ModeToBeUpdated.ModeCode
                            select mode).ToList();

            modeDBObject[0].MODE_DESC = ModeToBeUpdated.ModeDescription;
            modeDBObject[0].MODE_ACTIVE_SW = ModeToBeUpdated.ModeActiveSW;
            modeDBObject[0].MODE_IND = ModeToBeUpdated.ModeInd;
            modeDBObject[0].CHUSER = ModeToBeUpdated.ChangeUser;

            modeDBObject[0].CHTS = DateTime.Now;
            try
            {
                objContext.SaveChanges();
                sucess = true;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Msg += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Msg += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

            }
            catch (Exception ex)
            {
                Msg = ex.Message;
            }
            return sucess;
        }
        public bool CreateMode(Mode ModeFromClient, ref string Msg)
        {
            bool success = false;
            List<Mode> modeDs = GetMode(ModeFromClient.ModeCode);
            if (modeDs.Count > 0)
            {
                Msg = "Mode " + ModeFromClient.ModeCode + " Already Exists - Not Added";
                return success;
            }
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_MODE ModeToBeInserted = new MESC1TS_MODE();
            ModeToBeInserted.MODE = ModeFromClient.ModeCode;
            ModeToBeInserted.MODE_DESC = ModeFromClient.ModeDescription;
            ModeToBeInserted.MODE_ACTIVE_SW = ModeFromClient.ModeActiveSW;
            ModeToBeInserted.MODE_IND = ModeFromClient.ModeInd;
            ModeToBeInserted.CHUSER = ModeFromClient.ChangeUser;
            ModeToBeInserted.CHTS = DateTime.Now;
            ModeToBeInserted.STANDARD_TIME_SW = "Y";
            ModeToBeInserted.VALIDATION_SW = "Y";

            objContext.MESC1TS_MODE.Add(ModeToBeInserted);
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                if (ex.InnerException != null)
                    Msg = ex.InnerException.InnerException.Message;
                else
                    Msg = ex.Message;
            }

            return success;
        }
        #endregion Mode

        #region Manual
        public List<Manual> GetManual()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUAL> Manual = new List<MESC1TS_MANUAL>();
            Manual = (from manual in objContext.MESC1TS_MANUAL
                      select manual).ToList();


            return PrepareDataContractManual(Manual);

        }
        public List<Manual> GetSingleManual(string ManualCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_MANUAL> Manual = new List<MESC1TS_MANUAL>();
            Manual = (from manual in objContext.MESC1TS_MANUAL
                      where manual.MANUAL_CD == ManualCode
                      select manual).ToList();


            return PrepareDataContractManual(Manual, true);

        }
        private List<Manual> PrepareDataContractManual(List<MESC1TS_MANUAL> manualFromDB, bool showUser = false)
        {
            List<Manual> manualList = new List<Manual>();
            for (int count = 0; count < manualFromDB.Count; count++)
            {
                Manual manual = new Manual();
                manual.ManualCode = manualFromDB[count].MANUAL_CD;
                // manual.ManualFullDesc = manualFromDB[count].MANUAL_CD + '-' + manualFromDB[count].MANUAL_DESC;
                manual.ManualDesc = manualFromDB[count].MANUAL_DESC;
                manual.ManualActiveSW = manualFromDB[count].MANUAL_ACTIVE_SW;
                manual.ChTime = manualFromDB[count].CHTS;
                manual.ChangeUser = manualFromDB[count].CHUSER;
                if (showUser)
                    manual.ChangeUser = GetUserNamePTI(manualFromDB[count].CHUSER);
                manualList.Add(manual);
            }
            return manualList;
        }
        public bool UpdateManualDescription(Manual ManualDescriptionToBeUpdated, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_MANUAL> ManualDBObject = new List<MESC1TS_MANUAL>();
            ManualDBObject = (from manual in objContext.MESC1TS_MANUAL
                              where manual.MANUAL_CD == ManualDescriptionToBeUpdated.ManualCode
                              select manual).ToList();

            ManualDBObject[0].MANUAL_DESC = ManualDescriptionToBeUpdated.ManualDesc;
            ManualDBObject[0].CHUSER = ManualDescriptionToBeUpdated.ChangeUser;
            ManualDBObject[0].CHTS = DateTime.Now;
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Msg = ex.InnerException.Message;
                else
                    Msg = ex.Message;
            }
            return success;
        }
        public bool UpdateManualActiveSwitch(Manual ManualActiveSwitchToBeUpdated, ref string Msg)
        {
            objContext = new ManageMasterDataServiceEntities();
            bool success = false;
            List<MESC1TS_MANUAL> ManualDBObject = new List<MESC1TS_MANUAL>();
            ManualDBObject = (from manual in objContext.MESC1TS_MANUAL
                              where manual.MANUAL_CD == ManualActiveSwitchToBeUpdated.ManualCode
                              select manual).ToList();

            ManualDBObject[0].MANUAL_ACTIVE_SW = ManualActiveSwitchToBeUpdated.ManualActiveSW;
            ManualDBObject[0].CHUSER = ManualActiveSwitchToBeUpdated.ChangeUser;
            ManualDBObject[0].CHTS = DateTime.Now;

            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Msg = ex.InnerException.Message;
                else
                    Msg = ex.Message;
            }
            return success;
        }
        public bool CreateManual(Manual ManualFromClient, ref string Msg)
        {
            bool success = false;
            List<Manual> manuals = GetSingleManual(ManualFromClient.ManualCode);
            if (manuals.Count > 0)
            {
                Msg = "Manual " + ManualFromClient.ManualCode + " Already Exists or Manual Description Not Unique";
                return success;
            }
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_MANUAL ManualToBeInserted = new MESC1TS_MANUAL();
            ManualToBeInserted.MANUAL_CD = ManualFromClient.ManualCode;
            ManualToBeInserted.MANUAL_DESC = ManualFromClient.ManualDesc;
            ManualToBeInserted.MANUAL_ACTIVE_SW = ManualFromClient.ManualActiveSW;
            ManualToBeInserted.CHUSER = ManualFromClient.ChangeUser;
            ManualToBeInserted.CHTS = ManualFromClient.ChTime;

            objContext.MESC1TS_MANUAL.Add(ManualToBeInserted);
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                if (ex.InnerException != null)
                    Msg = ex.InnerException.Message;
                else
                    Msg = ex.Message;
            }

            return success;
        }
        #endregion Manual

        #region Damage Codes
        public List<Damage> GetDamageCodes()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_DAMAGE> DamageCodeList = new List<MESC1TS_DAMAGE>();
            DamageCodeList = (from damageCode in objContext.MESC1TS_DAMAGE
                              select damageCode).ToList();

            return PrepareDataContract(DamageCodeList, true);

        }

        public List<Damage> GetDamageCode(string code)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_DAMAGE> DamageCodeList = new List<MESC1TS_DAMAGE>();
            DamageCodeList = (from damageCode in objContext.MESC1TS_DAMAGE
                              where damageCode.cedex_code == code
                              select damageCode).ToList();

            return PrepareDataContract(DamageCodeList, false);

        }

        public Damage UpdateDamageCode(Damage DamageToBeUpdated)
        {
            List<Damage> oldDamages = GetDamageCode(DamageToBeUpdated.DamageCedexCode);
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_DAMAGE> damageDBObject = new List<MESC1TS_DAMAGE>();
            damageDBObject = (from damage in objContext.MESC1TS_DAMAGE
                              where damage.cedex_code == DamageToBeUpdated.DamageCedexCode
                              select damage).ToList();
            damageDBObject[0].name = DamageToBeUpdated.DamageName;
            damageDBObject[0].description = DamageToBeUpdated.DamageDescription;
            damageDBObject[0].numerical_code = DamageToBeUpdated.DamageNumericalCode;
            damageDBObject[0].CHUSER = DamageToBeUpdated.ChangeUser;

            damageDBObject[0].CHTS = DateTime.Now;

            try
            {
                objContext.SaveChanges();
                DamageToBeUpdated.ChangeTime = damageDBObject[0].CHTS;
                InsertDamageCodeAuditTrail(DamageToBeUpdated, oldDamages);
            }
            catch (Exception ex)
            {
            }
            return DamageToBeUpdated;
        }

        public bool DeleteDamageCode(string DamageCode)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_DAMAGE> damageDBObject = new List<MESC1TS_DAMAGE>();
            damageDBObject = (from damage in objContext.MESC1TS_DAMAGE
                              where damage.cedex_code == DamageCode
                              select damage).ToList();

            objContext.MESC1TS_DAMAGE.Remove(damageDBObject.First());
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public bool CreateDamageCode(Damage DamageCodeFromClient, ref string Msg)
        {
            bool success = false;
            List<Damage> existingDamage = GetDamageCode(DamageCodeFromClient.DamageCedexCode);
            if (existingDamage.Count > 0)
            {
                Msg = "Damage Code " + DamageCodeFromClient.DamageCedexCode + " Already Exists - Not Added";
                return false;
            }
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_DAMAGE damageToBeInserted = new MESC1TS_DAMAGE();
            damageToBeInserted.cedex_code = DamageCodeFromClient.DamageCedexCode;
            damageToBeInserted.name = DamageCodeFromClient.DamageName;
            damageToBeInserted.description = DamageCodeFromClient.DamageDescription;
            damageToBeInserted.numerical_code = DamageCodeFromClient.DamageNumericalCode;
            damageToBeInserted.CHUSER = DamageCodeFromClient.ChangeUser;
            damageToBeInserted.CHTS = DateTime.Now; //Convert.ToDateTime(DamageCodeFromClient.DamageCHTS);

            objContext.MESC1TS_DAMAGE.Add(damageToBeInserted);
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }

        private bool InsertDamageCodeAuditTrail(Damage DamageToBeUpdated, List<Damage> oldDamages)
        {
            bool success = false;
            // List<Damage> oldDamages = GetDamageCode(DamageToBeUpdated.DamageCedexCode);
            MESC1TS_REFAUDIT record;
            objContext = new ManageMasterDataServiceEntities();
            if (oldDamages[0].DamageDescription != DamageToBeUpdated.DamageDescription)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_DAMAGE", "DESCRIPTION", oldDamages[0].DamageDescription, DamageToBeUpdated.DamageDescription, DamageToBeUpdated.ChangeUser, DamageToBeUpdated.ChangeTime, DamageToBeUpdated.DamageCedexCode);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (oldDamages[0].DamageName != DamageToBeUpdated.DamageName)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_DAMAGE", "NAME", oldDamages[0].DamageName, DamageToBeUpdated.DamageName, DamageToBeUpdated.ChangeUser, DamageToBeUpdated.ChangeTime, DamageToBeUpdated.DamageCedexCode);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (oldDamages[0].DamageNumericalCode != DamageToBeUpdated.DamageNumericalCode)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_DAMAGE", "NUMERICAL_CODE", oldDamages[0].DamageNumericalCode, DamageToBeUpdated.DamageNumericalCode, DamageToBeUpdated.ChangeUser, DamageToBeUpdated.ChangeTime, DamageToBeUpdated.DamageCedexCode);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        private MESC1TS_REFAUDIT BuildDamageCodeAuditRecordSet(string TableName, string ColName, string OldValue, string NewValue, string ChangeUser, DateTime ChangeTime, string ID)
        {
            MESC1TS_REFAUDIT record = new MESC1TS_REFAUDIT();
            record.UNIQUE_ID = ID;
            record.TAB_NAME = TableName;
            record.COL_NAME = ColName;
            record.OLD_VALUE = OldValue;
            record.NEW_VALUE = NewValue;
            record.CHUSER = ChangeUser;
            record.CHTS = ChangeTime;
            return record;
        }

        public List<AuditTrail> GetAuditTrailData(string TableName, string UniqueID)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<Mode> ModeList = new List<Mode>();
            var data = (from AUDIT in objContext.MESC1TS_REFAUDIT
                        from USER in objContext.SEC_USER
                        .Where(USER => SqlFunctions.StringConvert((double)USER.USER_ID).Trim() == AUDIT.CHUSER)
                        .DefaultIfEmpty()
                        where AUDIT.TAB_NAME == TableName && AUDIT.UNIQUE_ID == UniqueID
                        orderby AUDIT.CHTS descending
                        select new
                        {
                            AUDIT.COL_NAME,
                            AUDIT.OLD_VALUE,
                            AUDIT.NEW_VALUE,
                            USER.LOGIN,
                            USER.FIRSTNAME,
                            USER.LASTNAME,
                            AUDIT.CHTS
                        }).ToList();
            List<AuditTrail> records = new List<AuditTrail>();
            foreach (var obj in data)
            {
                AuditTrail record = new AuditTrail();
                record.ChangeTime = Convert.ToDateTime(obj.CHTS).ToString("yyyy-MM-dd hh:mm:ss tt");
                record.ChangeUser = obj.LOGIN;
                record.ColName = obj.COL_NAME;
                record.FirstName = obj.FIRSTNAME;
                record.LastName = obj.LASTNAME;
                record.NewValue = obj.NEW_VALUE;
                record.OldValue = obj.OLD_VALUE;
                records.Add(record);
            }

            return records;
        }
        #endregion Damage Codes

        #region TPI Indicator
        public List<TPIIndicator> GetTPIIndicators()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_TPI> tpiList = new List<MESC1TS_TPI>();
            tpiList = (from tpi in objContext.MESC1TS_TPI
                       select tpi).ToList();

            return PrepareDataContractForCombo(tpiList);

        }

        public List<TPIIndicator> GetTPIIndicator(string Code)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_TPI> tpiList = new List<MESC1TS_TPI>();
            tpiList = (from tpi in objContext.MESC1TS_TPI
                       where tpi.cedex_code == Code
                       select tpi).ToList();

            return PrepareDataContract(tpiList);

        }

        private List<TPIIndicator> PrepareDataContractForCombo(List<MESC1TS_TPI> tpiFromDB)
        {
            List<TPIIndicator> tpiList = new List<TPIIndicator>();
            for (int count = 0; count < tpiFromDB.Count; count++)
            {
                TPIIndicator tpi = new TPIIndicator();
                tpi.TPICedexCode = tpiFromDB[count].cedex_code;
                tpi.TPIName = tpiFromDB[count].name;
                tpi.TPIDescription = tpiFromDB[count].description;
                tpi.TPINumericalCode = tpiFromDB[count].numerical_code;
                tpi.Category = tpiFromDB[count].category;
                tpi.TPICHUser = GetUserNamePTI(tpiFromDB[count].CHUSER);
                tpi.TPICHTS = tpiFromDB[count].CHTS;
                tpi.TPIFullDescription = tpiFromDB[count].cedex_code + '-' + tpiFromDB[count].name;
                tpiList.Add(tpi);
            }
            return tpiList;
        }

        private List<TPIIndicator> PrepareDataContract(List<MESC1TS_TPI> tpiFromDB)
        {
            List<TPIIndicator> tpiList = new List<TPIIndicator>();
            for (int count = 0; count < tpiFromDB.Count; count++)
            {
                TPIIndicator tpi = new TPIIndicator();
                tpi.TPICedexCode = tpiFromDB[count].cedex_code;
                tpi.TPIName = tpiFromDB[count].name;
                tpi.TPIDescription = tpiFromDB[count].description;
                tpi.TPINumericalCode = tpiFromDB[count].numerical_code;
                tpi.Category = tpiFromDB[count].category;
                tpi.TPICHUser = GetUserNamePTI(tpiFromDB[count].CHUSER);
                tpi.TPICHTS = tpiFromDB[count].CHTS;
                tpi.TPIFullDescription = tpiFromDB[count].cedex_code + '-' + tpiFromDB[count].name;
                tpiList.Add(tpi);
            }
            return tpiList;
        }

        public bool UpdateTPIIndicator(TPIIndicator TPIIndicatorToBeUpdated, ref string Msg)
        {
            bool sucess = false;
            List<TPIIndicator> oldTPI = GetTPIIndicator(TPIIndicatorToBeUpdated.TPICedexCode);
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_TPI> tpiDBObject = new List<MESC1TS_TPI>();
            tpiDBObject = (from tpi in objContext.MESC1TS_TPI
                           where tpi.cedex_code == TPIIndicatorToBeUpdated.TPICedexCode
                           select tpi).ToList();
            tpiDBObject[0].name = TPIIndicatorToBeUpdated.TPIName;
            tpiDBObject[0].description = TPIIndicatorToBeUpdated.TPIDescription;
            tpiDBObject[0].numerical_code = TPIIndicatorToBeUpdated.TPINumericalCode;
            tpiDBObject[0].CHUSER = TPIIndicatorToBeUpdated.TPICHUser;
            tpiDBObject[0].category = TPIIndicatorToBeUpdated.Category;
            tpiDBObject[0].CHTS = DateTime.Now;
            try
            {
                objContext.SaveChanges();
                TPIIndicatorToBeUpdated.TPICHTS = tpiDBObject[0].CHTS;
                InsertTPIIndicatorAuditTrail(TPIIndicatorToBeUpdated, oldTPI);
                sucess = true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Msg = ex.InnerException.Message;
                else
                    Msg = ex.Message;
            }
            return sucess;
        }

        public bool DeleteTPIIndicator(string TPICedexCode, ref string Msg)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_TPI> TPIDBObject = new List<MESC1TS_TPI>();
            TPIDBObject = (from tpi in objContext.MESC1TS_TPI
                           where tpi.cedex_code == TPICedexCode
                           select tpi).ToList();

            objContext.MESC1TS_TPI.Remove(TPIDBObject.First());
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Msg = ex.InnerException.Message;
                else
                    Msg = ex.Message;
            }
            return success;
        }

        public bool CreateTPIIndicator(TPIIndicator TPIIndicatorFromClient, ref string Msg)
        {
            bool success = false;
            List<TPIIndicator> tpiRecords = GetTPIIndicator(TPIIndicatorFromClient.TPICedexCode);
            if (tpiRecords.Count > 0)
            {
                Msg = "TPI " + TPIIndicatorFromClient.TPICedexCode + " Already Exists - Not Added";
                return success;
            }
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_TPI tpiToBeInserted = new MESC1TS_TPI();
            tpiToBeInserted.cedex_code = TPIIndicatorFromClient.TPICedexCode;
            tpiToBeInserted.name = TPIIndicatorFromClient.TPIName;
            tpiToBeInserted.description = TPIIndicatorFromClient.TPIDescription;
            tpiToBeInserted.numerical_code = TPIIndicatorFromClient.TPINumericalCode;
            tpiToBeInserted.CHUSER = TPIIndicatorFromClient.TPICHUser;
            tpiToBeInserted.CHTS = Convert.ToDateTime(TPIIndicatorFromClient.TPICHTS);
            tpiToBeInserted.category = TPIIndicatorFromClient.Category;

            objContext.MESC1TS_TPI.Add(tpiToBeInserted);
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Msg = ex.InnerException.Message;
                else
                    Msg = ex.Message;
            }

            return success;
        }

        private bool InsertTPIIndicatorAuditTrail(TPIIndicator NewRecord, List<TPIIndicator> OldRecord)
        {
            bool success = false;
            // List<Damage> oldDamages = GetDamageCode(DamageToBeUpdated.DamageCedexCode);
            MESC1TS_REFAUDIT record;
            objContext = new ManageMasterDataServiceEntities();
            if (OldRecord[0].TPIName != NewRecord.TPIName)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_TPI", "NAME", OldRecord[0].TPIName, NewRecord.TPIName, NewRecord.TPICHUser, NewRecord.TPICHTS, NewRecord.TPICedexCode);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].TPIDescription != NewRecord.TPIDescription)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_TPI", "DESCRIPTION", OldRecord[0].TPIDescription, NewRecord.TPIDescription, NewRecord.TPICHUser, NewRecord.TPICHTS, NewRecord.TPICedexCode);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].TPINumericalCode != NewRecord.TPINumericalCode)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_TPI", "NUMERICAL_CODE", OldRecord[0].TPINumericalCode, NewRecord.TPINumericalCode, NewRecord.TPICHUser, NewRecord.TPICHTS, NewRecord.TPICedexCode);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            if (OldRecord[0].Category != NewRecord.Category)
            {
                record = BuildDamageCodeAuditRecordSet("MESC1TS_TPI", "CATEGORY", OldRecord[0].Category, NewRecord.Category, NewRecord.TPICHUser, NewRecord.TPICHTS, NewRecord.TPICedexCode);
                objContext.MESC1TS_REFAUDIT.Add(record);
            }
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }
        #endregion TPI Indicator


        public List<RefAudit> GetAuditTrailCountry(string RecordId, string TableName)
        {
            List<RefAudit> RefAuditList = new List<RefAudit>();
            try
            {

                // dynamic Query = "";
                objContext = new ManageMasterDataServiceEntities();

                if (TableName == "Country")
                {
                    var Query = (from MR in objContext.MESC1TS_REFAUDIT
                                 where MR.TAB_NAME == "MESC1TS_COUNTRY" && MR.UNIQUE_ID == RecordId
                                 orderby MR.CHTS descending
                                 select new { MR.TAB_NAME, MR.UNIQUE_ID, MR.COL_NAME, MR.OLD_VALUE, MR.NEW_VALUE, MR.CHUSER, MR.CHTS }).ToList();


                    foreach (var obj in Query)
                    {
                        RefAudit objRef = new RefAudit();
                        objRef.TabName = obj.TAB_NAME;
                        objRef.UniqueID = obj.UNIQUE_ID;
                        objRef.ColName = obj.COL_NAME;
                        objRef.OldValue = obj.OLD_VALUE;
                        objRef.NewValue = obj.NEW_VALUE;
                        objRef.ChangeUser = obj.CHUSER;
                        string userName = GetUserName(obj.CHUSER);
                        if (userName != null)
                        {
                            if (userName.Contains('|'))
                            {
                                objRef.FirstName = userName.Split('|')[0];
                                objRef.LastName = userName.Split('|')[1];
                            }
                            else
                            {
                                objRef.FirstName = userName.Split('|')[0];
                                objRef.LastName = "";
                            }
                        }
                        else
                        {
                            objRef.FirstName = "";
                            objRef.LastName = "";
                        }
                        objRef.ChangeTime = obj.CHTS;
                        RefAuditList.Add(objRef);
                    };
                    RefAuditList.OrderByDescending(m => m.ChangeTime);
                }
            }

            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return RefAuditList;
        }


        public string GetUserName(string UserID)
        {
            UserID = UserID == null ? "0" : UserID;
            //try
            //{
            //    UserID = int.Parse(UserID).ToString();
            //}
            //catch (Exception ex)
            //{
            //    UserID = "0";
            //}
            //int id = int.Parse(UserID);
            List<SEC_USER> userDBObj = new List<SEC_USER>();
            userDBObj = (from user in objContext.SEC_USER
                         where user.LOGIN == UserID
                         select user).ToList();
            if (userDBObj.Count > 0)
                return userDBObj[0].FIRSTNAME + "|" + userDBObj[0].LASTNAME;
            else
                return UserID == null ? "" : UserID;
        }

        public List<Shop> GetShopCodeForSuspend(int UserID)
        {
            //UserID = 10022;

            List<Shop> ShopList = new List<Shop>();
            List<WorkOrderDetail> WODetailList = new List<WorkOrderDetail>();
            //List<MESC1TS_SHOP> ShopListFromDBOnAuth = new List<MESC1TS_SHOP>();
            //List<MESC1TS_SHOP> ShopListFinal = new List<MESC1TS_SHOP>();

            try
            {
                //SELECT U.AUTHGROUP_ID, U.COLUMN_VALUE as COLUMN_VALUE, G.TABLE_NAME, G.COLUMN_NAME as COLUMN_NAME
                //FROM SEC_AUTHGROUP_USER  U,	SEC_AUTHGROUP  G
                //WHERE U.USER_ID = <User_Id>
                //AND U.AUTHGROUP_ID = G.AUTHGROUP_ID ORDER BY U.AUTHGROUP_ID

                //ShopListFromDBOnAuth = (from S in objContext.MESC1TS_SHOP
                //                        join G in objContext.SEC_AUTHGROUP_USER on S.SHOP_CD equals G.COLUMN_VALUE
                //                        join A in objContext.SEC_AUTHGROUP on G.AUTHGROUP_ID equals A.AUTHGROUP_ID
                //                        where G.USER_ID == UserID &&
                //                              S.SHOP_ACTIVE_SW == "Y"
                //                        orderby S.SHOP_CD
                //                        select S).ToList();

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
                        string ShopCode = item.COLUMN_VALUE;
                        ShopCodeList.Add(ShopCode);
                    }
                }

                var ShopListFromDBOnAuth = (from shop in objContext.MESC1VS_SHOP_LOCATION
                                            where shop.SHOP_ACTIVE_SW == "Y" &&
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

                //var x = x.FindAll(cd => ShopListFromDBOnAuth1.Any(acd => acd.COLUMN_NAME

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
                                          select S).ToList();


                    var ShopListFinal = ShopListFromDBOnAuth.FindAll(a => ShopListFromDB.Any(ab => ab.SHOP_CD == a.SHOP_CD));

                    foreach (var item in ShopListFinal)
                    {
                        Shop shop = new Shop();
                        shop.ShopCode = item.SHOP_CD;
                        shop.ShopDescription = item.SHOP_DESC;
                        shop.CUCDN = item.CUCDN;
                        ShopList.Add(shop);
                    }

                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ShopList;
        }

        public List<Shop> GetShopCodeByCustomer(string customer)
        {

            List<Shop> ShopList = new List<Shop>();
            //List<MESC1TS_SHOP> ShopListFromDB = new List<MESC1TS_SHOP>();

            var ShopListFromDB = (from CS in objContext.MESC1TS_CUST_SHOP_MODE
                                  from S in objContext.MESC1TS_SHOP
                                  where S.SHOP_CD == CS.SHOP_CD &&
                                  CS.CUSTOMER_CD == customer
                                  orderby S.SHOP_CD
                                  select S).Distinct().ToList();

            foreach (var item in ShopListFromDB)
            {
                Shop shop = new Shop();
                shop.ShopCode = item.SHOP_CD;
                shop.ShopDescription = item.SHOP_DESC;
                ShopList.Add(shop);
            }
            return ShopList;
        }

        public List<Location> GetLocationListByLocationCode(string LocCode)
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_LOCATION> LocationListFromDB = new List<MESC1TS_LOCATION>();
            List<Location> LocationList = new List<Location>();
            try
            {
                LocationListFromDB = (from L in objContext.MESC1TS_LOCATION
                                      where L.LOC_CD.Trim().ToLower() == LocCode.Trim().ToLower()
                                      orderby L.LOC_CD
                                      select L).ToList();

                for (int count = 0; count < LocationListFromDB.Count; count++)
                {

                    Location loc = new Location();
                    loc.LocCode = LocationListFromDB[count].LOC_CD;
                    loc.LocDesc = LocationListFromDB[count].LOC_DESC;
                    loc.CountryCode = LocationListFromDB[count].COUNTRY_CD;
                    loc.RegionCode = LocationListFromDB[count].REGION_CD;
                    loc.ContactEqsalSW = LocationListFromDB[count].CONTACT_EQSAL_SW;
                    loc.ChangeUserLoc = LocationListFromDB[count].CHUSER;
                    loc.ChangeTimeLoc = LocationListFromDB[count].CHTS;
                    LocationList.Add(loc);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return LocationList;
        }

        #region GetCountryList
        public List<Country> GetCountryByID(int UserID, string Role)
        {
            List<Country> CountryList = new List<Country>();
            List<MESC1TS_COUNTRY> CountryListFromDB = new List<MESC1TS_COUNTRY>();

            try
            {
                //var RSAuthGroupByUID = (from u in objContext.SEC_AUTHGROUP_USER
                //                                where u.USER_ID == UserID
                //                                select new
                //                                {
                //                                    u.AUTHGROUP_ID
                //                                }).FirstOrDefault();

                //if (RSAuthGroupByUID != null)
                //{
                //    var RSAuthgroupByAuthgroupId = (from 
                //}

                //    SELECT DISTINCT C.COUNTRY_CD, C.COUNTRY_CD + ' - ' + C.COUNTRY_DESC AS DESCRIPTION"
                //strSQL = strSQL & " FROM MESC1TS_COUNTRY C INNER JOIN MESC1TS_LOCATION L"
                //strSQL = strSQL & " ON C.COUNTRY_CD = L.COUNTRY_CD"
                //strSQL = strSQL & " INNER JOIN SEC_AUTHGROUP_USER U"
                //strSQL = strSQL & " ON U.COLUMN_VALUE = L.LOC_CD"
                //strSQL = strSQL & " INNER JOIN SEC_AUTHGROUP A"
                //strSQL = strSQL & " ON A.AUTHGROUP_ID = U.AUTHGROUP_ID"
                //strSQL = strSQL & " WHERE USER_ID = " & sUserID
                //strSQL = strSQL & " ORDER BY C.COUNTRY_CD"

                if (Role == "MSL")
                {
                    CountryListFromDB = (from C in objContext.MESC1TS_COUNTRY
                                         join L in objContext.MESC1TS_LOCATION on C.COUNTRY_CD equals L.COUNTRY_CD
                                         join U in objContext.SEC_AUTHGROUP_USER on L.LOC_CD equals U.COLUMN_VALUE
                                         join A in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals A.AUTHGROUP_ID
                                         where U.USER_ID == UserID
                                         //orderby C.COUNTRY_CD
                                         select C).OrderBy(c => c.COUNTRY_CD).ToList();
                }
                else if (Role == "AREA" || Role == "CPH" || Role == "ADMIN"   )
                {
                    CountryListFromDB = (from C in objContext.MESC1TS_COUNTRY
                                         join L in objContext.MESC1TS_AREA on C.AREA_CD equals L.AREA_CD
                                         join U in objContext.SEC_AUTHGROUP_USER on L.AREA_CD equals U.COLUMN_VALUE
                                         join A in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals A.AUTHGROUP_ID
                                         where U.USER_ID == UserID
                                         //orderby C.COUNTRY_CD
                                         select C).OrderBy(c => c.COUNTRY_CD).ToList();
                }

                else if (Role == "COUNTRY" || Role == "EMR_APPROVER_COUNTRY" || Role == "EMR_SPECIALIST_COUNTRY" )
                {
                    CountryListFromDB = (from C in objContext.MESC1TS_COUNTRY
                                         join U in objContext.SEC_AUTHGROUP_USER on C.COUNTRY_CD equals U.COLUMN_VALUE
                                         join A in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals A.AUTHGROUP_ID
                                         where U.USER_ID == UserID
                                         //orderby C.COUNTRY_CD
                                         select C).OrderBy(c => c.COUNTRY_CD).ToList();
                }

                for (int count = 0; count < CountryListFromDB.Count; count++)
                {
                    Country Country = new Country();
                    Country.CountryCode = CountryListFromDB[count].COUNTRY_CD;
                    Country.CountryDescription = CountryListFromDB[count].COUNTRY_DESC;
                    CountryList.Add(Country);
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return CountryList;
        }
        #endregion GetCountryList

        public List<Grade> GetAllGradeCode()
        {
            List<Grade> GradeLists = new List<Grade>();
            try
            {
                var gradeCodeDet = (from G in objContext.MESC1TS_GRADE

                                    select new
                                    {
                                        G.GRADE_ID,
                                        G.GRADECODE,
                                        G.GRADE_DESC
                                    }).Distinct().ToList();

                if (gradeCodeDet != null && gradeCodeDet.Count > 0)
                {
                    foreach (var item in gradeCodeDet)
                    {
                        Grade GradeList = new Grade();

                        GradeList.GradeId = item.GRADE_ID;
                        GradeList.GradeCode = item.GRADECODE;
                        GradeList.GradeDescription = item.GRADE_DESC;


                        GradeLists.Add(GradeList);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }


            return GradeLists;
        }
        public bool UpdateGradeContainer(GradeContainer GradeCont)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_GRADECONTAINER GradedConetls = new MESC1TS_GRADECONTAINER();
            try
            {
                GradedConetls.GRADECODE = GradeCont.GRADECODE.ToUpper().Trim();
                GradedConetls.EQPNO = GradeCont.EQPNO.ToUpper();
                GradedConetls.WO_ID = GradeCont.WO_ID;
                GradedConetls.CURRENTLOC = GradeCont.CURRENTLOC;
                GradedConetls.GRADECODE_NEW = GradeCont.GRADECODE_NEW;
                GradedConetls.MODIFIEDBY = GradeCont.MODIFIEDBY;
                GradedConetls.MODIFIEDON = GradeCont.MODIFIEDON;
                objContext.MESC1TS_GRADECONTAINER.Add(GradedConetls);
                objContext.SaveChanges();
                success = true;

            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }
        public List<Shop> GetInactiveShopByUserId(int UserId)
        {
            List<Shop> ShopList = new List<Shop>();

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
                        string ShopCode = item.COLUMN_VALUE;
                        ShopCodeList.Add(ShopCode);
                    }
                }

                var ShopListFromDBOnAuth = (from shop in objContext.MESC1VS_SHOP_LOCATION
                                            where
                                             shop.SHOP_ACTIVE_SW == "N" &&
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


                    foreach (var item in ShopListFromDBOnAuth)
                    {
                        Shop shop = new Shop();
                        shop.ShopCode = item.SHOP_CD;
                        shop.ShopDescription = item.SHOP_DESC;
                        shop.CUCDN = item.CUCDN;
                        ShopList.Add(shop);
                    }
                    if (ShopList != null && ShopList.Count > 0)
                    {
                        PopulateShopWithCustAndCurrency(ShopList[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ShopList;
        }

        public List<Shop> GetShopCodeByUserId(int UserId)
        {
            List<Shop> ShopList = new List<Shop>();

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
                        string ShopCode = item.COLUMN_VALUE;
                        ShopCodeList.Add(ShopCode);
                    }
                }

                var ShopListFromDBOnAuth = (from shop in objContext.MESC1VS_SHOP_LOCATION
                                            where
                                            //shop.SHOP_ACTIVE_SW == "Y" &&
                                            (VendorCodeList.Contains(shop.VENDOR_CD) ||
                                            LocCodeList.Contains(shop.LOC_CD) ||
                                            CountryCodeList.Contains(shop.COUNTRY_CD) ||
                                            AreaCodeList.Contains(shop.AREA_CD) ||
                                            ShopCodeList.Contains(shop.SHOP_CD))
                                            select new
                                            {
                                                shop.SHOP_CD,
                                                shop.CUCDN,
                                                shop.SHOP_DESC,
                                                shop.SHOP_ACTIVE_SW
                                            }).OrderBy(code => code.SHOP_CD).ToList();

                if (ShopListFromDBOnAuth != null && ShopListFromDBOnAuth.Count > 0)
                {
                    foreach (var item in ShopListFromDBOnAuth)
                    {
                        Shop shop = new Shop();
                        shop.ShopCode = item.SHOP_CD;
                        shop.ShopDescription = item.SHOP_DESC;
                        shop.CUCDN = item.CUCDN;
                        shop.ShopActiveSW = item.SHOP_ACTIVE_SW;
                        ShopList.Add(shop);
                    }
                    if (ShopList != null && ShopList.Count > 0)
                    {
                        PopulateShopWithCustAndCurrency(ShopList[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return ShopList;
        }

        public List<string> GetAllGradeNames()
        {
            List<string> gradeNames = new List<string>();
            //string NO = "N";
            try
            {
                //  gradeNames = objContext.MESC1TS_GRADE.Select(x => x.GRADECODE ).Distinct().ToList();   
                var gradeCodeDet = (from G in objContext.MESC1TS_GRADE
                                    where G.GRADECODE != "N"

                                    select new
                                    {
                                        G.GRADECODE

                                    }).Distinct().ToList();

                if (gradeCodeDet != null && gradeCodeDet.Count > 0)
                {
                    foreach (var item in gradeCodeDet)
                    {

                        gradeNames.Add(item.GRADECODE);

                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return gradeNames;
        }
        public List<GradeRelation> GetAllGradeRelations()
        {
            List<GradeRelation> gradeRelations = new List<GradeRelation>();

            try
            {
                var GradeRelations = (from GR in objContext.MESC1TS_GRADERELATION
                                      from G in objContext.MESC1TS_GRADE
                                      where GR.GRADE_ID == G.GRADE_ID
                                      select new
                                      {
                                          GR.GR_ID,
                                          G.GRADE_ID,
                                          G.GRADECODE,
                                          G.GRADE_DESC,
                                          GR.UPGRADED_GRADE,
                                          GR.DOWNGRADED_GRADE,
                                          GR.CREATEDBY,
                                          GR.CREATEDON,
                                          GR.MODIFIEDBY,
                                          GR.MODIFIEDON
                                      }).OrderBy(id => id.GR_ID).ToList();

                if (GradeRelations != null && GradeRelations.Count > 0)
                {
                    foreach (var item in GradeRelations)
                    {
                        GradeRelation gradeRelation = new GradeRelation();

                        gradeRelation.GradeRelationId = item.GR_ID;
                        gradeRelation.GradeId = item.GRADE_ID;
                        gradeRelation.GradeCode = item.GRADECODE;
                        gradeRelation.GradeDescription = item.GRADE_DESC;
                        gradeRelation.UpgradedGrade = item.UPGRADED_GRADE;
                        gradeRelation.DowngradedGrade = item.DOWNGRADED_GRADE;
                        gradeRelation.CreatedBy = item.CREATEDBY;
                        gradeRelation.CreatedOn = item.CREATEDON;
                        gradeRelation.ModifiedBy = item.MODIFIEDBY;
                        gradeRelation.ModifiedOn = item.MODIFIEDON;

                        gradeRelations.Add(gradeRelation);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return gradeRelations;
        }

        public GradeRelation GetGradeRelationById(int graderelationid)
        {
            GradeRelation gradeRelation = null;            

            try
            {
                var GradeRelation = objContext.MESC1TS_GRADERELATION.FirstOrDefault(x => x.GR_ID == graderelationid);                
            
                if (GradeRelation != null)
                {
                    var Grade = objContext.MESC1TS_GRADE.FirstOrDefault(x => x.GRADE_ID == GradeRelation.GRADE_ID);

                    if (Grade != null)
                    {
                        gradeRelation = new GradeRelation();

                        gradeRelation.GradeRelationId = GradeRelation.GR_ID;
                        gradeRelation.GradeId = GradeRelation.GRADE_ID;
                        gradeRelation.GradeCode = Grade.GRADECODE;
                        gradeRelation.GradeDescription = Grade.GRADE_DESC;
                        gradeRelation.UpgradedGrade = GradeRelation.UPGRADED_GRADE;
                        gradeRelation.DowngradedGrade = GradeRelation.DOWNGRADED_GRADE;
                        gradeRelation.CreatedBy = GradeRelation.CREATEDBY;
                        gradeRelation.CreatedOn = GradeRelation.CREATEDON;
                        gradeRelation.ModifiedBy = GradeRelation.MODIFIEDBY;
                        gradeRelation.ModifiedOn = GradeRelation.MODIFIEDON;
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return gradeRelation;
        }

        public bool DeleteGradeRelation(int graderelationid)
        {
            bool isSuccess = false;            
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_GRADERELATION gradeRelation = null;
            MESC1TS_GRADE grade = null;            
           
            try
            {
                gradeRelation = objContext.MESC1TS_GRADERELATION.FirstOrDefault(x => x.GR_ID == graderelationid);                
                if (gradeRelation != null)
                {                   
                    grade = objContext.MESC1TS_GRADE.FirstOrDefault(x => x.GRADE_ID == gradeRelation.GRADE_ID);

                    objContext.MESC1TS_GRADERELATION.Remove(gradeRelation);
                    objContext.SaveChanges();

                    if (grade != null)
                    {
                        var gradeSTSData = (from GS in objContext.MESC1TS_GRADESTS
                                            where GS.GRADE_ID  == grade.GRADE_ID 
                                            select GS).ToList();
                        if (gradeSTSData.Count() > 0)
                        {
                            foreach (var obj in gradeSTSData)
                            {
                                objContext.MESC1TS_GRADESTS.Remove(obj);
                                objContext.SaveChanges();
                            }

                            isSuccess = true; ;
                        }
                         

                        //--------------------
                        objContext.MESC1TS_GRADE.Remove(grade);
                        objContext.SaveChanges();
                    }

                    isSuccess = true;
                }
            }
            catch
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public bool UpdateGradeRelation(GradeRelation graderelation)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_GRADERELATION gradeRelation = null;

            try
            {
                gradeRelation = objContext.MESC1TS_GRADERELATION.FirstOrDefault(x => x.GR_ID == graderelation.GradeRelationId);
                if (gradeRelation != null)
                {
                    gradeRelation.UPGRADED_GRADE = graderelation.UpgradedGrade;
                    gradeRelation.DOWNGRADED_GRADE = graderelation.DowngradedGrade;
                    gradeRelation.MODIFIEDBY = graderelation.ModifiedBy;
                    gradeRelation.MODIFIEDON = DateTime.Now;

                    objContext.SaveChanges();
                    success = true;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }

        public bool AddGradeRelation(GradeRelation graderelation)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_GRADE grade = new MESC1TS_GRADE();
            MESC1TS_GRADERELATION gradeRelation = new MESC1TS_GRADERELATION();
            
            try
            {
                if (objContext.MESC1TS_GRADE.FirstOrDefault(x => x.GRADECODE == graderelation.GradeCode) != null)
                {
                    return false;
                }

                grade.GRADECODE = graderelation.GradeCode.ToUpper();
                grade.GRADE_DESC = graderelation.GradeDescription;
                grade.CREATEDBY = graderelation.CreatedBy;
                grade.CREATEDON = DateTime.Now;
                var newGrade = objContext.MESC1TS_GRADE.Add(grade);
                objContext.SaveChanges();

                gradeRelation.GRADE_ID = newGrade.GRADE_ID;
                gradeRelation.UPGRADED_GRADE = graderelation.UpgradedGrade;
                gradeRelation.DOWNGRADED_GRADE = graderelation.DowngradedGrade;
                gradeRelation.CREATEDBY = graderelation.CreatedBy;
                gradeRelation.CREATEDON = DateTime.Now;
                objContext.MESC1TS_GRADERELATION.Add(gradeRelation);
                objContext.SaveChanges();

                success = true;
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }

        public List<GradeSTS> GetAllGradeSTSByMode(List<string> modes, string manualcd)
        {
            List<GradeSTS> gradeSTSList = new List<GradeSTS>();

            try
            {
                var GradeSTSList = (from GS in objContext.MESC1TS_GRADESTS
                                      from RC in objContext.MESC1TS_REPAIR_CODE
                                      where GS.STS_CODE == RC.REPAIR_CD
                                            && GS.MODE ==RC.MODE
                                            && RC.MANUAL_CD=="MAER"
                                      select new
                                      {
                                          GS.GSTS_ID,
                                          GS.GRADE_ID,
                                          GS.GRADECODE,
                                          GS.STS_CODE,
                                          RC.REPAIR_DESC,
                                          GS.MODE,
                                          GS.FLAG,
                                          GS.MANUAL_CD,
                                          GS.IsApplicable,
                                          GS.CREATEDBY,
                                          GS.CREATEDON,
                                          GS.MODIFIEDBY,
                                          GS.MODIFIEDON
                                      }).OrderBy(id => id.STS_CODE).ToList();             

                if (GradeSTSList != null && GradeSTSList.Count > 0)
                {
                    foreach (var item in GradeSTSList)
                    {
                        GradeSTS gradeSTS = new GradeSTS();

                        gradeSTS.GradeSTSId = item.GSTS_ID;
                        gradeSTS.GradeId = item.GRADE_ID;
                        gradeSTS.GradeCode = item.GRADECODE;
                        gradeSTS.STSCode = item.STS_CODE;
                        gradeSTS.STSDescription = item.REPAIR_DESC;
                        gradeSTS.Mode = item.MODE;
                        gradeSTS.FLAG = item.FLAG == null ? false : item.FLAG.Value;
                        gradeSTS.ManualCD = item.MANUAL_CD;
                        gradeSTS.IsApplicable = item.IsApplicable == null ? false : item.IsApplicable.Value;
                        gradeSTS.CreatedBy = item.CREATEDBY;
                        gradeSTS.CreatedOn = item.CREATEDON;
                        gradeSTS.ModifiedBy = item.MODIFIEDBY;
                        gradeSTS.ModifiedOn = item.MODIFIEDON;

                        gradeSTSList.Add(gradeSTS);
                    }
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return gradeSTSList;
        }

        public bool DeleteGradeSTSMapping(string stscode, string mode, string manualcd)
        {
            bool isSuccess = false;
            objContext = new ManageMasterDataServiceEntities();
          //  MESC1TS_GRADESTS gradeSTS = null;            

            try
            {
                //gradeSTS = objContext.MESC1TS_GRADESTS
                //           .FirstOrDefault(x => 
                //                x.STS_CODE.Trim() == stscode.Trim()
                //                && x.MODE.Trim() == mode.Trim()
                //                && x.MANUAL_CD.Trim().ToUpper() == manualcd.Trim().ToUpper()
                //           );
                //if (gradeSTS != null)
                //{
                //    objContext.MESC1TS_GRADESTS.Remove(gradeSTS);
                //    objContext.SaveChanges();
                //    isSuccess = true;
                //}

                //pinaki


                var gradeSTSData = (from Cont in objContext.MESC1TS_GRADESTS
                                    where Cont.STS_CODE.Trim() == stscode.Trim() && Cont.MODE.Trim() == mode.Trim() && Cont.MANUAL_CD.Trim().ToUpper() == manualcd.Trim().ToUpper()
                                    select Cont).ToList();
                if (gradeSTSData.Count() > 0)
                {
                    foreach (var obj in gradeSTSData)
                    {
                        objContext.MESC1TS_GRADESTS.Remove(obj);
                        objContext.SaveChanges();
                    }

                    isSuccess = true; ;
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public bool UpdateGradeSTSMapping(List<GradeSTS> gradeSTSList)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_GRADESTS> gradeSTSDataList = null;
            bool isUpdated = false;

            string stsCode = gradeSTSList[0].STSCode.Trim();
            string mode = gradeSTSList[0].Mode.Trim();
            string manualCD = gradeSTSList[0].ManualCD.Trim();

            try
            {
                gradeSTSDataList = objContext.MESC1TS_GRADESTS
                               .Where(x =>
                                    x.STS_CODE.Trim() == stsCode
                                    && x.MODE.Trim() == mode
                                    && x.MANUAL_CD.Trim().ToUpper() == manualCD.ToUpper()
                                ).ToList();

                if (gradeSTSDataList != null && gradeSTSDataList.Count > 0)
                {
                    foreach(GradeSTS gradeSTS in gradeSTSList)
                    {
                        string gradeCode = gradeSTS.GradeCode;
                        var gradeSTSData = gradeSTSDataList.FirstOrDefault(x => x.GRADECODE == gradeCode);

                        if (gradeSTSData != null)
                        {
                            gradeSTSData.IsApplicable = gradeSTS.IsApplicable;
                            gradeSTSData.FLAG = gradeSTS.FLAG;
                            gradeSTSData.MODIFIEDBY = gradeSTS.ModifiedBy;
                            gradeSTSData.MODIFIEDON = DateTime.Now;
                        }
                        else
                        {
                            MESC1TS_GRADESTS newGradeSTS = new MESC1TS_GRADESTS();

                            int gradeId = 0;
                            var tempGradeSTS = objContext.MESC1TS_GRADE.FirstOrDefault(x => x.GRADECODE == gradeCode);
                            if (tempGradeSTS != null)
                            {
                                gradeId = tempGradeSTS.GRADE_ID;
                            }

                            newGradeSTS.GRADE_ID = gradeId;
                            newGradeSTS.GRADECODE = gradeCode;
                            newGradeSTS.STS_CODE = stsCode;
                            newGradeSTS.MODE = mode;
                            newGradeSTS.MANUAL_CD = manualCD;
                            newGradeSTS.IsApplicable = gradeSTS.IsApplicable;
                            newGradeSTS.FLAG = gradeSTS.FLAG;
                            newGradeSTS.CREATEDBY = gradeSTS.ModifiedBy;
                            newGradeSTS.CREATEDON = DateTime.Now;

                            objContext.MESC1TS_GRADESTS.Add(newGradeSTS);
                        }
                    }
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    objContext.SaveChanges();
                    success = true;
                }                
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }

        public bool AddGradeSTSMapping(List<GradeSTS> gradeSTSList)
        {
            bool success = false;
            bool Isduplicate = true;
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_GRADESTS gradeSTSData = null;
            bool isAdded = false;
            if (gradeSTSList != null && gradeSTSList.Count > 0)
            {
                foreach (GradeSTS gradeSTS in gradeSTSList)
                {
                    var st = objContext.MESC1TS_GRADESTS.FirstOrDefault(x => x.STS_CODE == gradeSTS.STSCode && x.MODE == gradeSTS.Mode);
                    if (st != null)
                    {
                        Isduplicate = true;
                        break;
                    }
                    else
                    {
                        Isduplicate = false;
                        break;
                    }
                }
            }

            try
            {
                if (gradeSTSList != null && gradeSTSList.Count > 0)
                {
                    if (Isduplicate == false)  //pinaki added
                    {   
                        foreach (GradeSTS gradeSTS in gradeSTSList)
                         {
                             gradeSTSData = new MESC1TS_GRADESTS();

                      
                            var grade = objContext.MESC1TS_GRADE.FirstOrDefault(x => x.GRADECODE == gradeSTS.GradeCode);
                            if (grade != null)
                            {
                              
                                gradeSTSData.GRADE_ID = grade.GRADE_ID;
                                gradeSTSData.GRADECODE = gradeSTS.GradeCode.ToString();
                                gradeSTSData.STS_CODE = gradeSTS.STSCode;
                                gradeSTSData.MODE = gradeSTS.Mode;
                                gradeSTSData.MANUAL_CD = gradeSTS.ManualCD;
                                gradeSTSData.IsApplicable = gradeSTS.IsApplicable;
                                gradeSTSData.FLAG = gradeSTS.FLAG;
                                gradeSTSData.CREATEDBY = gradeSTS.CreatedBy;
                                gradeSTSData.CREATEDON = DateTime.Now;

                                objContext.MESC1TS_GRADESTS.Add(gradeSTSData);
                                isAdded = true;
                            }
                        }
                    }
                }

                if (isAdded)
                {
                    objContext.SaveChanges();
                    success = true;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }
            return success;
        }

        public string GetSTSDescription(string stscode, string mode, string manualcd)
        {            
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_REPAIR_CODE repairCode = null;
            string stsDescription = string.Empty;

            try
            {
                repairCode = objContext.MESC1TS_REPAIR_CODE
                             .FirstOrDefault(x =>
                                x.REPAIR_CD.Trim() == stscode.Trim()
                                && x.MODE.Trim() == mode.Trim()
                                && x.MANUAL_CD.Trim().ToUpper() == manualcd.Trim().ToUpper()
                           );
                if (repairCode != null)
                {
                    stsDescription = repairCode.REPAIR_DESC;
                }
            }
            catch (Exception ex)
            {
                logEntry.Message = ex.ToString();
                Logger.Write(logEntry);
            }

            return stsDescription;
        }
    }
}
