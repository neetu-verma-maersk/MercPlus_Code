using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CreateWorkOrderService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ICreateWorkOrder
    {
        
        [OperationContract]
        List<Damage> GetDamageCodeAll();

        [OperationContract]
        List<Shop> GetShopCode(int UserID);

        [OperationContract]
        List<Customer> GetCustomerCode(string ShopCode);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class Damage
    {
        [DataMember]
        public string DamageCode { get; set; }
        [DataMember]
        public string DamageCodeDescription { get; set; }
        [DataMember]
        public string DamageName { get; set; }
    }

    [DataContract]
    public class Shop
    {
        [DataMember]
        public string ShopCode { get; set; }
        [DataMember]
        public string ShopDescription { get; set; }
    }

    [DataContract]
    public class Customer
    {
        [DataMember]
        public string CustomerCode { get; set; }
        [DataMember]
        public string CustomerDescription { get; set; }
    }

}
