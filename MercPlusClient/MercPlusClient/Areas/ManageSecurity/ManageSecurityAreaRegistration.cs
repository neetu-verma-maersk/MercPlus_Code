using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageSecurity
{
    public class ManageSecurityAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ManageSecurity";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ManageSecurity_default",
                "ManageSecurity/{controller}/{action}/{id}",
                new { action = "ManageSecurity", id = UrlParameter.Optional },
                namespaces: new[] { "MercPlusClient.Areas.ManageSecurity.Controllers" }
            );
        }
    }
}
