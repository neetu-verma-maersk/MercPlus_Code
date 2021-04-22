using System.Web.Mvc;

namespace MercPlusClient.Areas.ManageWorkOrder
{
    public class ManageWorkOrderAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ManageWorkOrder";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ManageWorkOrder_default",
                "ManageWorkOrder/{controller}/{action}/{id}",
                new { controller = "ManageWorkOrderController", action = "ManageWorkOrder", id = UrlParameter.Optional }
            ) ;
        }
    }
}
