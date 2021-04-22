using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Web.Security;
using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageMasterData.Models
{
    public class ManageMasterDataCPHModel
    {

        #region CPH ApprovalLevel Properties

        public string UsrRole { get; set; }
        public string UsrId { get; set; }
        public bool IsFlag { get; set; }
        public bool IsAdd { get; set; }
        public bool IsSubmit { get; set; }
        public bool Header { get; set; }
        public bool View { get; set; }
        public bool IsEdit { get; set; }

        [Display(Name = "Eq Size")]
        public string EqSize { get; set; }

        [Display(Name = "Mode List")]
        public string Mode_List { get; set; }


        public string txtEqSize { get; set; }
        public string txtModeList { get; set; }

        public List<SelectListItem> drpEqSize = new List<SelectListItem>();
        public List<SelectListItem> drpMode_List = new List<SelectListItem>();
        public List<SelectListItem> ddlEqSize = new List<SelectListItem>();
        public List<SelectListItem> ddlMode_List = new List<SelectListItem>();

        [Display(Name = "Eq Size")]
        public string Eq_Size { get; set; }

        //[Display(Name = "Mode")]
        //public string Mode { get; set; }

        [Display(Name = "Message")]
        public string strMsg { get; set; }

        public string txtLimitAmt1 { get; set; }

        public string txtLimitAmt2 { get; set; }

        public string txtLimitAmt3 { get; set; }

        public string txtLimitAmt4 { get; set; }

        public string txtLimitAmt5 { get; set; }

        public string txtLimitAmt6 { get; set; }

        public string txtLimitAmt7 { get; set; }

        public string txtLimitAmtAdd1 { get; set; }

        public string txtLimitAmtAdd2 { get; set; }

        public string txtLimitAmtAdd3 { get; set; }

        public string txtLimitAmtAdd4 { get; set; }

        public string txtLimitAmtAdd5 { get; set; }

        public string txtLimitAmtAdd6 { get; set; }

        public string txtLimitAmtAdd7 { get; set; }

        public string txtChangeUser1 { get; set; }

        public string txtChangeUser2 { get; set; }

        public string txtChangeUser3 { get; set; }

        public string txtChangeUser4 { get; set; }

        public string txtChangeUser5 { get; set; }

        public string txtChangeUser6 { get; set; }

        public string txtChangeUser7 { get; set; }

        public string txtChangeTime1 { get; set; }

        public string txtChangeTime2 { get; set; }

        public string txtChangeTime3 { get; set; }

        public string txtChangeTime4 { get; set; }

        public string txtChangeTime5 { get; set; }

        public string txtChangeTime6 { get; set; }

        public string txtChangeTime7 { get; set; }


        #endregion
    }
}