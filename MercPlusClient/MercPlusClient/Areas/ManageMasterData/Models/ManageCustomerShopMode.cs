using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageCustomerShopMode
    {
        public List<ManageCustomerShopMode> SearchData { get; set; }


        #region Control Propeties

        [Display(Name = "Customer")]
        public string lstCustomerList { get; set; }
        public List<SelectListItem> CustomerList = new List<SelectListItem>();

        [Display(Name = "Shop")]
        public string lstShopList { get; set; }
        public List<SelectListItem> ShopList = new List<SelectListItem>();

        [Display(Name = "Mode")]
        public string lstModeList { get; set; }
        public List<SelectListItem> ModeList = new List<SelectListItem>();

        #endregion

        #region Grid Properties

        public string COCUSTOMER { get; set; }
        public string COSHOP { get; set; }
        public string COMODE { get; set; }
        public string COPAYAGENT { get; set; }
        public string COCORPPAYAGENT { get; set; }
        public string CORRIS { get; set; }
        public string COCPC { get; set; }
        public string COPROFITCENTER { get; set; }
        public string COEXPCODE { get; set; }
        public string CO_CSM_CD { get; set; }
        public string CO_EDIT { get; set; }
        public string CO_DELETE { get; set; }


        #endregion Grid Properties

        #region Add/Edit Properties

        public bool IsAdd { get; set; }
        public bool IsView { get; set; }
        public bool Flag { get; set; }

        [Display(Name = "CsmCode")]
        public string txtCSMCode { get; set; }


        [Display(Name = "PayAgent")]
        public string txtPayAgent { get; set; }
        public List<SelectListItem> ddlPayAgent = new List<SelectListItem>();

        [Display(Name = "CPC")]
        public string txtCPC { get; set; }

        [Display(Name = "CorpPayagent")]
        public string txtCorpPayagent { get; set; }

        [Display(Name = "ProfitCenter")]
        public string txtProfitCenter { get; set; }

        [Display(Name = "ExpenseCode")]
        public string txtExpenseCode { get; set; }

        [Display(Name = "RRIS")]
        public string txtRRIS { get; set; }
        public List<SelectListItem> ddlRRIS = new List<SelectListItem>();

        [Display(Name = "ChangeUserName")]
        public string txtChangedUserName { get; set; }

        [Display(Name = "ChangeTime")]
        public string txtChangeTime { get; set; }

        #endregion
    }
}