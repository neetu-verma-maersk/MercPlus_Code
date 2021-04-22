using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageReports
{
    public class ManageReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ManageReports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ManageReports_default",
                "ManageReports/{controller}/{action}/{id}",
                new { action = "ManageReports", id = UrlParameter.Optional },
                namespaces: new[] { "MercPlusClient.Areas.ManageReports.Controllers" }
            );
        }
    }
}
