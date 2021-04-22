using MercPlusClient.UtilityClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageSecurity.Models
{
    public class ManageSecurityModel
    {
        public IList<SelectListItem> UserRoleList = Enum.GetNames(typeof(UtilMethods.USERROLE)).Select(x => new SelectListItem() { Text = x, Value = x }).ToArray();
    }
}