using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace MercPlusLibrary
{
    [DataContract]
    public class Currency
    {
        [DataMember]
        public string CurCode { get; set; }
        [DataMember]
        public string Cucdn { get; set; }
        [DataMember]
        public string CurrName { get; set; }
        [DataMember]
        public decimal? ExtraTdkk { get; set; }
        [DataMember]
        public decimal? ExtratUsd { get; set; }
        [DataMember]
        public decimal? ExtraTyen { get; set; }
        [DataMember]
        public string ChangeUser { get; set; }
        [DataMember]
        public DateTime ChangeTime { get; set; }
        [DataMember]
        public decimal ExtraTeur{ get; set; }
        [DataMember]
        public DateTime? QuoteDat { get; set; }
        [DataMember]
        public string CurrencyNotFound { get; set; }

        [DataMember]
        public List<CountryCont> CountryCont { get; set; }
        [DataMember]
        public List<CountryLabor> CountryLabor { get; set; }
        [DataMember]
        public List<Shop> SHop { get; set; }
        [DataMember]
        public List<WO> WorkOrder { get; set; }
    }
}
