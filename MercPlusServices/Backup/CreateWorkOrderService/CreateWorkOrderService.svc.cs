using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.Objects;

namespace CreateWorkOrderService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class CreateWorkOrder : ICreateWorkOrder
    {
        CreateWorkOrderServiceEntities objContext = new CreateWorkOrderServiceEntities();
        public List<Damage> GetDamageCodeAll()
        {
            objContext = new CreateWorkOrderServiceEntities();
            List<Damage> DamageList = new List<Damage>();
            List<MESC1TS_DAMAGE> DamageFromDB = new List<MESC1TS_DAMAGE>();
            DamageFromDB = (from damage in objContext.MESC1TS_DAMAGE
                      select damage).ToList();

            foreach (var obj in DamageFromDB)
            {
                Damage Damage = new Damage();
                Damage.DamageCode = obj.cedex_code;
                Damage.DamageCodeDescription = obj.description;
                Damage.DamageName = obj.name;
                DamageList.Add(Damage);
            }
            return DamageList;
        }

        public List<Shop> GetShopCode(int UserID)
        {
            UserID = 10022;
            objContext = new CreateWorkOrderServiceEntities();
            List<Shop> ShopList = new List<Shop>();
            List<MESC1TS_SHOP> ShopListFromDB = new List<MESC1TS_SHOP>();

            //(from C in objContext.MESC1TS_COUNTRY
            //                     join U in objContext.SEC_AUTHGROUP_USER on C.AREA_CD equals U.COLUMN_VALUE
            //                     join A in objContext.SEC_AUTHGROUP on U.AUTHGROUP_ID equals A.AUTHGROUP_ID
            //                     where U.USER_ID == UserID
            //                     orderby C.COUNTRY_CD
            //                     select new { C.COUNTRY_CD, C.COUNTRY_CD }).ToList();

            ShopListFromDB = (from S in objContext.MESC1TS_SHOP
                        join G in objContext.SEC_AUTHGROUP_USER on S.SHOP_CD equals G.COLUMN_VALUE
                        join A in objContext.SEC_AUTHGROUP on G.AUTHGROUP_ID equals A.AUTHGROUP_ID
                        where G.USER_ID == UserID &&
                              S.SHOP_ACTIVE_SW == "Y"
                        orderby S.SHOP_CD
                        select S).ToList();

            foreach (var item in ShopListFromDB)
            {
                Shop shop = new Shop();
                shop.ShopCode = item.SHOP_CD;
                shop.ShopDescription = item.SHOP_DESC;
                ShopList.Add(shop);
            }
            return ShopList;
        }

        public List<Customer> GetCustomerCode(string ShopCode)
        {
            objContext = new CreateWorkOrderServiceEntities();
            List<Customer> Customerlist = new List<Customer>();
            List<MESC1TS_CUSTOMER> CustomerFromDB = new List<MESC1TS_CUSTOMER>();
            List<Shop> ShopList = new List<Shop>();
            List<string> shopCodes = ShopList.Select(sc => { return sc.ShopCode ;}).ToList();
            //SELECT DISTINCT C.CUSTOMER_CD 
            //FROM MESC1VS_CUST_SHOP CS, MESC1TS_CUSTOMER C 
            //WHERE C.CUSTOMER_CD = CS.CUSTOMER_CD 
            // AND CS.SHOP_CD IN ( ShopList )

            CustomerFromDB = (from CS in objContext.MESC1VS_CUST_SHOP
                              from C in objContext.MESC1TS_CUSTOMER
                              where C.CUSTOMER_CD == CS.CUSTOMER_CD &&
                                    CS.SHOP_CD == ShopCode
                              select C).Distinct().ToList();

            foreach (var item in CustomerFromDB)
            {
                Customer customer = new Customer();
                customer.CustomerCode = item.CUSTOMER_CD;
                Customerlist.Add(customer);
            }
            return Customerlist;
        }
    }
}
