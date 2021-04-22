using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageUser
{
    public class ManageUserAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ManageUser";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ManageUser_default",
                "ManageUser/{controller}/{action}/{id}",
                new { action = "ManageUser", id = UrlParameter.Optional },
                namespaces: new[] { "MercPlusClient.Areas.ManageUser.Controllers" }
            );
        }
    }
}
