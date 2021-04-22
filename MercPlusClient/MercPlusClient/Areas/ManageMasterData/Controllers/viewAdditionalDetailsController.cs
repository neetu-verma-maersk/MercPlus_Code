using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageWorkOrder.Controllers
{
    public class viewAdditionalDetailsController : Controller
    {
        //
        // GET: /ManageWorkOrder/viewAdditionalDetails/

        public ActionResult viewAdditionalDetails()
        {
            return View();
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session.IsNewSession || Session["UserSec"] == null || Session["UserSec"] == "")
            {
                filterContext.Result = new RedirectResult("/ManageSecurity/ManageSecurity/SessionExpire");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
