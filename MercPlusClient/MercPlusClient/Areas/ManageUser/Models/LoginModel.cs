using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MercPlusClient.UtilityClass;

namespace MercPlusClient.Areas.ManageUser.Models
{
    public class LoginModel
    {
        public IList<SelectListItem> UserRoleList = Enum.GetNames(typeof(UtilMethods.USERROLE)).Select(x => new SelectListItem() { Text = x, Value = x }).ToArray();
    }
}