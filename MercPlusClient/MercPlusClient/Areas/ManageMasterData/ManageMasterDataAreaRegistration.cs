using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageMasterData
{
    public class ManageMasterDataAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ManageMasterData";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ManageMasterData_default",
                "ManageMasterData/{controller}/{action}/{id}",
                new { action = "ManageMasterData", id = UrlParameter.Optional },
                namespaces: new[] { "MercPlusClient.Areas.ManageMasterData.Controllers"}
            );
        }
    }
}
