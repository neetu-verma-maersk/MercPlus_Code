
using System.Web.Mvc;

namespace MercPlusClient.Areas.HSUDData
{
    public class HSUDDataAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HSUDData";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HSUDData_default",
                "HSUDData/{controller}/{action}/{id}",
                new { action = "HSUDData", id = UrlParameter.Optional },
                namespaces: new[] { "MercPlusClient.Areas.HSUDData.Controllers" }
            );
        }
    }
}
