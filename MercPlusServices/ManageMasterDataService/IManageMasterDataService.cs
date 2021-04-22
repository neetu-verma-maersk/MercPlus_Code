using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MercPlusLibrary;

namespace ManageMasterDataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IManageMasterData
    {
        [OperationContract]
        List<PayAgent> GetPayAgent();

        [OperationContract]
        List<Shop> GetShop(int UserID);


        [OperationContract]
        bool UpdatePayAgent(PayAgent PayAgentToBeUpdated);

        [OperationContract]
        bool DeletePayAgent(string RRISPayAgentCode,ref string Msg);

        [OperationContract]
        bool CreatePayAgent(PayAgent PayAgentListToBeUpdated, ref string Msg);


        [OperationContract]
        bool UpdateEquipmentType(EqType EquipmentTypeToBeUpdated, ref string Msg);

        [OperationContract]
        bool CreateEquipmentTypeEntry(EqType EquipmentTypeFromClient, ref string Msg);

        [OperationContract]
        List<RepairLocationCode> GetRepairLocationCodes();

        [OperationContract]
        List<RepairLocationCode> GetRepairLocationCode(string code);

        //[OperationContract]
        //List<Damage> GetDamageCode();

        [OperationContract]
        List<ModeEntry> GetModeEntryList();

        [OperationContract]
        bool AddModeEntry(ModeEntry ModeEntryToBeAdded);

        [OperationContract]
        List<SubType> GetSubTypeList();

        [OperationContract]
        //List<Mode> GetMode();
        List<Mode> GetPrepModes(bool p);

        [OperationContract]
        List<Mode> GetAllModes(); //@Soumik



        [OperationContract]
        List<Customer> GetCustomerDetailsList();

        [OperationContract]
        List<Manual> GetManualList();

        [OperationContract]
        List<Manual> GetManualCodeList();

        [OperationContract]
        bool UpdateCustomer(Customer CustomerToBeUpdated);

        [OperationContract]
        bool CreateCustomer(Customer CustomerFromClient, ref string Msg);

        [OperationContract]
        List<Location> GetLocationList();

        [OperationContract]
        bool UpdateLocation(Location LocationToBeUpdated);

        [OperationContract]
        List<Country> GetCountryList();

        [OperationContract]
        List<Country> GetRegionByCountryCode(string CountryCode);

        [OperationContract]
        bool UpdateCountry(Country CountryToBeUpdated);

        [OperationContract]
        List<CustShopMode> GetCustShopModeList(string CustomerCode, string ShopCode, string Mode, int SortType);

        [OperationContract]
        List<Customer> GetCustomerCode();

        [OperationContract]
        List<Shop> GetShopCode();

        [OperationContract]
        List<Mode> GetModeList();

        [OperationContract]
        List<Mode> GetSuspedModeList(string ManualCode);

        [OperationContract]
        List<Region> GetRegionList();

        [OperationContract]
        List<Transmit> GetTransmitDetails();

        [OperationContract]
        List<RepairCode> GetRepairCode();

        [OperationContract]
        List<SuspendCat> GetSuspendCategory();

        [OperationContract]
        bool UpdateTransmit(Transmit TransmitToBeUpdated);

        [OperationContract]
        bool CreateTransmit(Transmit TransmitFromClient, ref string Msg);

        [OperationContract]
        List<Mode> GetModeByCustomerCode(string CustomerCode);

        [OperationContract]
        List<Transmit> GetTransmitDetailsFromCustomerMode(string CustomrCode, string ModeCode);

        [OperationContract]
        List<Vendor> GetVendorList();

        [OperationContract]
        bool UpdateVendor(Vendor VendorToBeUpdated);

        [OperationContract]
        bool CreateVendor(Vendor VendorFromClient,ref string Msg);

        [OperationContract]
        List<SuspendCat> GetSuspendCatList();

        [OperationContract]
        List<SuspendCat> GetSuspendCategoryID(SuspendCat SuspendCatDBObject);

        [OperationContract]
        bool UpdateSuspendCat(SuspendCat SuspendCatToBeUpdated,ref string Msg);

        [OperationContract]
        bool DeleteSuspendCat(string SuspendCatCode);

        [OperationContract]
        bool CreateSuspendCat(SuspendCat SuspendCatFromClient,ref string Msg);

        [OperationContract]
        List<Suspend> GetSuspendList(string ShopCode, string ManualCode, string ModeCode, string RepairCode);

        [OperationContract]
        List<Manual> GetManualCodeFromShopCode(string ShopCode);

        [OperationContract]
        List<Mode> GetModeFromShopManual(string ShopCode, string ManualCode);

        [OperationContract]
        List<RepairCode> GetRepairCodeFromShopManualMode(string ShopCode, string ManualCode, string ModeCode);

        [OperationContract]
        bool CreateSuspend(Suspend SuspendFromClient, string ShopCode, string ManualCode, string ModeCode, ref string Msg);

        [OperationContract]
        bool DeleteSuspend(Suspend SuspendCode);

        [OperationContract]
        List<Manual> GetManualDetails();

        [OperationContract]
        List<Mode> GetModeFromManual(string ManualCode);

        [OperationContract]
        List<RepairCode> GetRepairCodeFromManualMode(string ManualCode, string ModeCode);

        [OperationContract]
        List<RprcodeExclu> GetExRprCodeFromManualModeRepairCode(string ManualCode, string ModeCode, string RepairCode, string ExRprCode);

        [OperationContract]
        bool AddExRepairCode(RprcodeExclu ExRprCodeFromClient, string RepairCode, string ManualCode, string ModeCode, ref string Msg);

        [OperationContract]
        bool DeleteExRprCode(string ManualCode, string ModeCode, string RepairCode, string ExRprCode, ref string Msg);

        [OperationContract]
        List<WO> GetWOListOnCountry(DateTime DateTo, DateTime DateFrom, string CountryCode);

        [OperationContract]
        List<WO> GetWOListOnLocation(DateTime DateTo, DateTime DateFrom, string LocationCode);

        [OperationContract]
        List<WO> GetWOListOnShop(DateTime DateTo, DateTime DateFrom, string ShopCode);

        [OperationContract]
        List<Manufactur> GetManufacturerDiscountList();

        [OperationContract]
        Manufactur UpdateManufacturerDiscount(Manufactur MDiscountToBeUpdated);

        [OperationContract]
        bool DeleteManufacturerDiscount(string MDiscountCode);

        [OperationContract]
        string CreateManufacturerDiscount(Manufactur MDiscountToBeCreated);


        [OperationContract]
        List<RefAudit> GetAuditTrailMDiscount(string RecordId, string TableName);

         
        [OperationContract]
        List<Model> GetManufacturerModelList(string ManufacturerCode);

        [OperationContract]
        Model UpdateManufacturerModel(Model ModelToBeUpdated);

        [OperationContract]
        List<Shop> GetRSByShop(string ShopCode);
                
        [OperationContract]
        bool DeleteManufacturerModel(string MDiscountCode, string MmodelNo);

        #region Afroz

        [OperationContract]
        List<Shop> GetShopByUserId(int UserId);

        [OperationContract]
        List<Shop> GetShopProfileByUserId(int UserId);

        [OperationContract]
        List<Currency> GetRSAllCurrencies();


        [OperationContract]
        List<PayAgentVendor> RSAllCorpPayAgents();

        [OperationContract]
        List<Vendor> RSVendorsByPayAgent(string PayAgent_CD);

        [OperationContract]
        List<Vendor> GetRSAllVendors();

        [OperationContract]
        List<PayAgentVendor> RSByPayAgentVendor(string PayAgent_CD, string Vendor_CD);

        [OperationContract]
        string CreatePayAgentVendor(PayAgentVendor PayAgentFromClient);

        [OperationContract]
        string DeletePayAgentVendor(string PayAgent_CD, string Vendor_CD);

        [OperationContract]
        string UpdatePayAgentVendor(PayAgentVendor PayAgentToBeUpdated);

        [OperationContract]
        List<ShopCont> GetRSAllContracts(string ShopCode, string RepairCode, string ModeCode);

        [OperationContract]
        List<Mode> GetModesByShop(string Shop_CD);

        [OperationContract]
        List<ShopLimits> GetRSByShopModes(string Shop_CD, string Mode);

        [OperationContract]
        string InsertShopContract(ShopCont ShopContList);

        [OperationContract]
        string UpdateShopContract(ShopCont ShopContList);

        [OperationContract]
        ShopCont FillShopContractEdit(int ContID);

        [OperationContract]
        string DeleteShopContract(string gridData);

        [OperationContract]
        string UpdateExpDateForShopContract(string gridData, DateTime expDate, string CHUser);

        [OperationContract]
        string InsertShopLimit(ShopLimits ShopLimit);

        [OperationContract]
        string UpdateShopLimit(ShopLimits ShopLimit);

        [OperationContract]
        List<RefAudit> GetAuditTrailShop(string RecordId, string TableName);

        [OperationContract]
        List<Manufactur> RSAllManufacturers();

        [OperationContract]
        List<Discount> GetRSDiscount(string ShopCD);

        [OperationContract]
        string InsertUpdateShopDiscount(string ShopCode, string DiscountAll, string ManufactCode, string ManufactDis, string UserName);

        [OperationContract]
        string UpdateShopProfile(Shop ShopList);

        [OperationContract]
        string InsertShopProfile(Shop ShopList);

        [OperationContract]
        string RetransmitWorkOrderStatus(string[] WOIDS, string UserId);

        [OperationContract]
        List<Mode> GetRSAllManualModes(string ManaualCode);

        [OperationContract]
        List<Manual> GetRSAllManual();

        [OperationContract]
        Currency GetShopContCurrencyCode(string shopcode);

        [OperationContract]
        string GetShopTypeByShopCode(string ShopCode); 

        #endregion Afroz

        [OperationContract]
        bool CreateManufacturerModel(Model MModelToBeCreated);
        [OperationContract]
        List<PrepTime> GetPrepTimeDetails(string ModeCode);


        [OperationContract]
        string UpdatePrepTime(PrepTime PrepToBeUpdated);


        [OperationContract]
        bool DeletePrepTime(string ModeCode, double PreptimeDigit);

        [OperationContract]
        string CreatePrepTime(PrepTime preplToBeCreated);
        [OperationContract]
        List<Customer> GetCustomerList();
        [OperationContract]
        List<Shop> GetShopList();
        [OperationContract]
        List<Shop> GetUserShopList();
        [OperationContract]
        List<Shop> GetNonActiveShopList();
        //[OperationContract]
        //List<EqType> GetEquipmentList();

        [OperationContract]
        List<LaborRate> GetLaborRateDetail(string ShopCode, String CustCode, String eqtypeCode);

        [OperationContract]
        List<LaborRate> GetLaborRateEditDetail(int LID);


        [OperationContract]
        string CreateLaborRate(LaborRate LRateToBeCreated);

        [OperationContract]
        string ModifyLaborRate(LaborRate LRateToBeCreated);
        [OperationContract]
        List<RefAudit> GetAuditTrailLabourRate(string RecordId, string TableName);
        

        #region Amlan - Master Parts

        [OperationContract]
        List<PartsGroup> GetAllPartsGroups();

        [OperationContract]
        List<Manufactur> GetAllManufacturers();

        [OperationContract]
        MasterPart AddMasterPart(MasterPart masterPartFromClient);

        [OperationContract]
        List<MasterPart> GetMasterPartsByQuery(string partGroupCode, string manufacturerCode, string designation,
            string partNumber, string description, string isActive, string isCore, string isDeductCoreValue);

        [OperationContract]
        List<MasterPart> GetMasterPartByPartCode(string partGroupCode);

        [OperationContract]
        MasterPart UpdateMasterPart(MasterPart masterPartFromClient);

        [OperationContract]
        bool DeleteMasterPart(string masterPartCode);

        [OperationContract]
        List<Country> GetAllCountries();

        [OperationContract]
        List<CountryCont> GetCountryContractByQuery(string countryCode, string repairCode, string mode);

        [OperationContract]
        List<RefAudit> GetAuditTrail_MasterPart(string RecordId, string TableName);

        #endregion

        #region Amlan - Parts Group

        [OperationContract]
        PartsGroup AddPartsGroup(PartsGroup masterPartFromClient);

        [OperationContract]
        bool UpdatePartsGroup(PartsGroup PartsGroupToBeUpdated);

        [OperationContract]
        PartsGroup GetPartsGroupByQuery(string partGroupCode);

        #endregion

        #region Amlan - Repair Code/Part Association

        [OperationContract]
        List<Mode> GetAllActiveModes();
        [OperationContract]
        List<Manual> GetAllActiveManuals();
        [OperationContract]
        List<RepairCodePart> GetRepairCode_PartAssociation(string mode, string manual, string repairCode, string partNumber);
        [OperationContract]
        List<Mode> GetModesByManuals(string manCode);
        [OperationContract]
        ServiceResult Add_Edit_RPA(RepairCodePart repairCodePartFromClient, string OprMode);
        [OperationContract]
        ServiceResult UpdateRPA(string orgManualCode, string orgModeCode, string orgPartNumber, string orgRepairCod, string maxPartQty,
        string UserName);
        [OperationContract]
        ServiceResult DeleteRPRCODE_PART(string repairCod, string partNumber, string modeCode, string manualCode);
        

        #endregion Amlan - Repair Code/Part Association

   

        #region PTI Periods
        [OperationContract]
        List<PTIPeriod> GetPTIPeriods(string serialFrom);
        [OperationContract]
        PTIPeriod GetPTIPeriod(string SerialFrom, string SerialTo);
        [OperationContract]
        bool CreatePTIPeriod(PTIPeriod PTIPeriodFromClient, ref string msg);
        [OperationContract]
        bool DeletePTIPeriod(PTIPeriod PTIPeriodFromClient, ref string msg);
        [OperationContract]
        bool ModifyPTIPeriod(PTIPeriod PTIPeriodFromClient, ref string msg);
        [OperationContract]
        int GetPTIDefaultPeriod();
        [OperationContract]
        bool ModifyPTIDefaultPeriod(int NoOfDays, string UserID, ref string msg);
        [OperationContract]
        PTIPeriod GetPTIDefaultPeriodRecord();
        #endregion PTI Periods

        #region Special Remarks
        [OperationContract]
        bool InsertSpecialRemarks(SpecialRemarks SpecialRemarksFromClient, ref int ID, ref string msg);
        [OperationContract]
        bool ModifySpecialRemarks(SpecialRemarks SpecialRemarksFromClient, ref string msg);
        [OperationContract]
        bool DeleteSpecialRemarks(SpecialRemarks SpecialRemarksFromClient, ref string msg);
        [OperationContract]
        SpecialRemarks GetSpecialRemarks(int RemarksID);
        #endregion Special Remarks

        #region Index
        [OperationContract]
        List<Index> GetIndexes();
        [OperationContract]
        List<Index> GetIndex(int IndexID, string Manual_CD, string Mode);
        [OperationContract]
        List<Manual> GetIndexManual();
        [OperationContract]
        List<Mode> GetIndexMode(string manual_cd);
        [OperationContract]
        bool UpdateIndex(Index updatedIndex, ref string Msg);
        [OperationContract]
        bool DeleteIndex(int indexID, string manualCode, string mode, ref string Msg);
        [OperationContract]
        bool CreateIndex(Index insertIndex, ref string Msg);
        [OperationContract]
        List<Index> GetIndexesForDropDown(string Manual_CD, string Mode);
        [OperationContract]
        List<List<SpecialRemarks>> GetSpecialRemarksComboValue();
        [OperationContract]
        List<Manual> GetIndexAllActiveManual();
        [OperationContract]
        List<Mode> GetIndexAllActiveMode(string manual_cd);
        #endregion Index

        #region Repair STS Code
        [OperationContract]
        List<Manual> GetRepairCodeManual();
        [OperationContract]
        List<Mode> GetRepairCodeMode(string ManualCode);
        [OperationContract]
        List<RepairCode> GetRepairCodeByMode(string ManualCode, string ModeCode);
        [OperationContract]
        List<RepairCode> GetRepairCodeByIndex(string ManualCode, string ModeCode, int IndexId);
        [OperationContract]
        List<RepairCode> GetRepairCodeByRepairCode(string ManualCode, string ModeCode, string RepairCode);
        [OperationContract]
        bool InsertRepairCode(RepairCode RepairCodeFromClient, ref string Msg);
        [OperationContract]
        bool ModifyRepairCode(RepairCode RepairCodeFromClient, ref string Msg);
        [OperationContract]
        bool DeleteRepairCode(RepairCode RepairCodeFromClient, ref string msg);
        [OperationContract]
        List<Manual> GetRepairCodeActiveIndexManual();
        [OperationContract]
        List<Mode> GetRepairCodeIndexMode(string ManualCode);
        #endregion Repair STS Code

        #region ManualMode
        [OperationContract]
        bool InsertManualMode(ManualMode ManualModeFromClient, ref string Msg);
        [OperationContract]
        bool DeleteManualMode(ManualMode ManualModeFromClient, ref string msg);
        [OperationContract]
        bool ModifyManualMode(ManualMode ManualModeFromClient, ref string Msg);
        [OperationContract]
        List<ManualMode> GetManualMode(string ManualCode, string ModeCode);
        [OperationContract]
        List<Manual> GetManualModeManual();
        [OperationContract]
        List<Manual> GetAllManual();
        [OperationContract]
        List<Mode> GetManualModeMode(string ManualCode);
        [OperationContract]
        List<Mode> GetAllMode();
        #endregion ManualMode

        #region Damage Code
        [OperationContract]
        List<Damage> GetDamageCodes();
        [OperationContract]
        List<Damage> GetDamageCode(string code);
        [OperationContract]
        bool CreateDamageCode(Damage DamageCodeFromClient, ref string Msg);
        [OperationContract]
        Damage UpdateDamageCode(Damage DamageToBeUpdated);
        [OperationContract]
        bool DeleteDamageCode(string DamageCode);
        [OperationContract]
        List<AuditTrail> GetAuditTrailData(string TableName, string UniqueID);
        #endregion Damage Code

        #region Mode
        [OperationContract]
        List<Mode> GetModes();
        [OperationContract]
        List<Mode> GetAllActiveInActiveModes();
        [OperationContract]
        List<Mode> GetMode(string ModeCode);
        [OperationContract]
        bool UpdateMode(Mode ModeToBeUpdated, ref string Msg);
        [OperationContract]
        bool CreateMode(Mode ModeFromClient, ref string Msg);
        #endregion Mode

        #region TPI Indicator
        [OperationContract]
        List<TPIIndicator> GetTPIIndicators();
        [OperationContract]
        List<TPIIndicator> GetTPIIndicator(string Code);
        [OperationContract]
        bool UpdateTPIIndicator(TPIIndicator TPIIndicatorToBeUpdated, ref string Msg);
        [OperationContract]
        bool DeleteTPIIndicator(string TPICedexCode, ref string Msg);
        [OperationContract]
        bool CreateTPIIndicator(TPIIndicator TPIIndicatorListToBeUpdated, ref string Msg);
        #endregion TPI Indicator

        #region Manual
        [OperationContract]
        List<Manual> GetManual();
        [OperationContract]
        bool UpdateManualDescription(Manual ManualDescriptionToBeUpdated, ref string Msg);
        [OperationContract]
        bool UpdateManualActiveSwitch(Manual ManualActiveSwitchToBeUpdated, ref string Msg);
        [OperationContract]
        bool CreateManual(Manual ManualFromClient, ref string Msg);
        [OperationContract]
        List<Manual> GetSingleManual(string ManualCode);
        #endregion Manual


        #region Ashiqur

        [OperationContract]
        List<CountryLabor> GetLabourRateDetails(string Country, string Eqtype);

        [OperationContract]
        List<Country> GetCountryLabourList();

        [OperationContract]
        List<EqType> GetEquipmentList();

        [OperationContract]
        List<EqType> GetEquipmentTypeList();

        [OperationContract]
        List<Mode> GetCphEquLimitModeList();

        [OperationContract]
        List<CphEqpLimit> GetRSAllLimits(string Eq, string Mode);

        [OperationContract]
        List<CphEqpLimit> SubmitCPHApprovalDetails(CphEqpLimit cphEqp, string UserLogin, string LimitAmt1, string LimitAmt2, string LimitAmt3, string LimitAmt4, string LimitAmt5, string LimitAmt6, string LimitAmt7);

        [OperationContract]
        List<CphEqpLimit> UpdateCPHApprovalDetails(string EqSize, string ModeCode, string Age, string amt, string UserLogin);

        [OperationContract]
        List<EqsType> GetSubType();

        [OperationContract]
        List<EqsType> GetEqType();

        [OperationContract]
        List<EqMode> GetRSAllEquModes(string EqType, string SubType, string Size, string Aluminium);

        [OperationContract]
        List<Mode> GetRSAllModes();

        [OperationContract]
        bool CreateEqTypeModeEntry(string CoType, string EqType, string EqSize, string Mode, string Aluminium, string Chuser);

        [OperationContract]
        bool UpdateEqTypeModeEntry(string EqModeId, string Mode, string Chuser);

        [OperationContract]
        List<EqMode> GetRSByEqMode(string EqModeId);

        [OperationContract]
        List<EqMode> GetRSByAltKey(string CoType, string EqType, string EqSize, string Mode, string Aluminium);

        [OperationContract]
        bool EQCheckDuplicate(string CoType, string EqType, string EqSize, string Mode, string Aluminium);

        [OperationContract]
        bool EQCheckDuplicateByType(string CoType, string EqType);

        [OperationContract]
        bool EQCheckDuplicateByMode(string Mode);

        [OperationContract]
        bool CheckDuplicateEqId(string EqId);

        [OperationContract]
        List<EqsType> GetSubTypeDetail(string SubType);

        [OperationContract]
        List<CustShopMode> GetCSMList(string CustomerCode, string ShopCode, string Mode);

        [OperationContract]
        List<PayAgent> GetAllPayAgents();

        [OperationContract]
        List<CustShopMode> GetRsByCSM(string CSMCD);

        [OperationContract]
        bool ValidateProfitCenterByProfit(string strPayAgent, string strProfit);

        [OperationContract]
        bool ValidateProfitCenterBySubProfit(string strCorpPayAgent, string strSubProfit);

        [OperationContract]
        bool InsertCustShopMode(string sCSMCd, string sCustomerCd, string sShopCd, string sMode, string sPayagentCd, string sCorpPayagentCd, string sRRISFormat, string sProfitCenter, string sSubProfitCenter, string sAccountCd, string sUser,ref string Msg);

        [OperationContract]
        bool UpdateCustShopMode(string sCSMCd, string sCustomerCd, string sShopCd, string sMode, string sPayagentCd, string sCorpPayagentCd, string sRRISFormat, string sProfitCenter, string sSubProfitCenter, string sAccountCd, string sUser);

        [OperationContract]
        bool CheckDuplicate(string sCustomerCd, string sShopCd, string sMode);

        [OperationContract]
        bool DeleteCsmCode(string CSMCD);

        [OperationContract]
        List<Customer> Get_CustomerCode();

        [OperationContract]
        List<Shop> Get_ShopCode();

        [OperationContract]
        List<Mode> Get_ModeList();

        [OperationContract]
        bool IsCheckDuplicate(string Mode, string EqSize);


        #endregion

        [OperationContract]
        List<RefAudit> GetAuditTrailCountry(string RecordId, string TableName);

        [OperationContract]
        string GetUserName(string UserID);

        [OperationContract]
        List<Shop> GetShopCodeForSuspend(int UserID);

        [OperationContract]
        List<Mode> GetModeForSuspend(string Manual);

        [OperationContract]
        List<Shop> GetShopCodeByCustomer(string customer);

        [OperationContract]
        List<Location> GetLocationListByLocationCode(string LocCode);

        [OperationContract]
        List<Country> GetCountryByID(int UserID, string Role);

        [OperationContract]
        List<Shop> GetInactiveShopByUserId(int UserId);

        [OperationContract]
        List<Shop> GetShopCodeByUserId(int UserId);
        [OperationContract]
        List<Grade> GetAllGradeCode();
        [OperationContract]
        bool UpdateGradeContainer(GradeContainer GradeCont);

        [OperationContract]
        List<string> GetAllGradeNames();

        [OperationContract]
        List<GradeRelation> GetAllGradeRelations();

        [OperationContract]
        bool DeleteGradeRelation(int graderelationid);

        [OperationContract]
        bool UpdateGradeRelation(GradeRelation graderelation);

        [OperationContract]
        bool AddGradeRelation(GradeRelation graderelation);

        [OperationContract]
        GradeRelation GetGradeRelationById(int graderelationid);

        [OperationContract]
        List<GradeSTS> GetAllGradeSTSByMode(List<string> modes, string manualcd);

        [OperationContract]
        bool DeleteGradeSTSMapping(string stscode, string mode, string manualcd);

        [OperationContract]
        bool UpdateGradeSTSMapping(List<GradeSTS> gradeSTSList);

        [OperationContract]
        bool AddGradeSTSMapping(List<GradeSTS> gradeSTSList);

        [OperationContract]
        string GetSTSDescription(string stscode, string mode, string manualcd);
    } 
    
}
