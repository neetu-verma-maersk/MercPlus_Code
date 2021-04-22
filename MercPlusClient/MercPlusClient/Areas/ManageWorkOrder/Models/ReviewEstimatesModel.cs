using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace MercPlusClient.Areas.ManageWorkOrder.Models
{
    public class ReviewEstimatesModel
    {
        #region Control Properties

        public List<SelectListItem> ShopList { get; set; }
        public List<SelectListItem> CustomerList { get; set; }
        public List<SelectListItem> EqpTypeList { get; set; }
        public List<SelectListItem> EqpSizeList { get; set; }
        public List<SelectListItem> EqpSubTypeList { get; set; }
        public List<SelectListItem> ModeList { get; set; }
        public List<SelectListItem> QueryTypeList { get; set; }
        public List<SelectListItem> SortList { get; set; }
        public string WorkOrderIDList { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string EquipmentNo { get; set; }
        public string VendorRefNo { get; set; }
        public string COCL { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }

        #endregion Control Properties
        #region Grid Properties

        public string WO_ID { get; set; }
        public string LOC_CD { get; set; }
        public string SHOP_CD { get; set; }
        public string STATUS_CD { get; set; }
        public string STATUS_DESC { get; set; }
        public string TOTAL_COST_LOCAL { get; set; }
        public string TOTAL_COST_LOCAL_USD { get; set; }
        public string TOTOL_COST_REPAIR_CPH { get; set; }
        public string EQPNO { get; set; }
        public string VENDOR_REF_NO { get; set; }
        public string MODE { get; set; }
        public string WO_RECV_DTE { get; set; }
        public string CHTS { get; set; }
        public string CRTS { get; set; }
        public string REPAIR_DTE { get; set; }
        public string VoucherNo { get; set; }
        public string PayAgent_CD { get; set; }
        public string AgentVouchNo { get; set; }
        public string SHOP_WORKING_SW { get; set; }
        public int initPendingflag { get; set; }
        public int intWorkingflag { get; set; }
        public int intSerialNo { get; set; }
        public List<ReviewEstimatesModel> SearchResults { get; set; }


        #endregion Grid Properties

        public string ErrorMsg { get; set; } /*I want to dynamically change the value of the error message*/

    }
}