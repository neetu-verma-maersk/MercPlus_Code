using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MercPlusLibrary
{
    [DataContract]
    public class Shop
    {
        [DataMember]
        public string ShopCode { get; set; }
        [DataMember]
        public string ShopDescription { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public System.DateTime ChangeTime { get; set; }
        [DataMember]
        public string VendorCode { get; set; }
        [DataMember]
        public string RKRPloc { get; set; }
        [DataMember]
        public string LocationCode { get; set; }
        [DataMember]
        public string EmailAdress { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string EDIPartner { get; set; }
        [DataMember]
        public string RRIS70SuffixCode { get; set; }
        [DataMember]
        public string PreptimeSW { get; set; }
        [DataMember]
        public string ShopActiveSW { get; set; }
        [DataMember]
        public Nullable<double> PCTMaterialFactor { get; set; }
        [DataMember]
        public string RRISXmitSW { get; set; }
        [DataMember]
        public string OvertimeSuspSW { get; set; }
        [DataMember]
        public Nullable<double> ImportTax { get; set; }
        [DataMember]
        public string ShopTypeCode { get; set; }
        [DataMember]
        public Nullable<double> SalesTaxPartCont { get; set; }
        [DataMember]
        public Nullable<double> SalesTaxPartGen { get; set; }
        [DataMember]
        public Nullable<double> SalesTaxLaborCon { get; set; }
        [DataMember]
        public string CUCDN { get; set; }
        [DataMember]
        public Nullable<double> SalesTaxLaborGen { get; set; }
        [DataMember]
        public string AcepSW { get; set; }
        [DataMember]
        public string Decentralized { get; set; }
        [DataMember]
        public string AutoCompleteSW { get; set; }
        [DataMember]
        public string BypassLeaseRules { get; set; }
        [DataMember]
        public string ShopNotFound { get; set; }
        
        [DataMember]
        public Currency Currency { get; set; }
        [DataMember]
        public List<Customer> Customer { get; set; }
        [DataMember]
        public List<CustShopMode> CustShopMode { get; set; }
        [DataMember]
        public List<LaborRate> LaborRate { get; set; }
        [DataMember]
        public Location Location { get; set; }
        [DataMember]
        public ErrMessage ErrorMessage { get; set; }
        //public virtual ICollection<MESC1TS_SHOP_CONT> MESC1TS_SHOP_CONT { get; set; }
        //public virtual ICollection<MESC1TS_SHOP_LIMITS> MESC1TS_SHOP_LIMITS { get; set; }
        //public virtual ICollection<MESC1TS_SUSPEND> MESC1TS_SUSPEND { get; set; }
        //public virtual MESC1TS_VENDOR MESC1TS_VENDOR { get; set; }
        //public virtual ICollection<WorkOrder> WorkOrder { get; set; }

    }
}
