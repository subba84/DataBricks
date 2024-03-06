using System.Web.Mvc;

namespace DataBricksWeb.Areas.Po
{
    public class PoAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Po";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "po/list", new { controller = "ManagePo", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "po/edit", new { controller = "ManagePo", action = "Edit", id = UrlParameter.Optional });
            context.MapRoute(null, "po/details", new { controller = "ManagePo", action = "Details", id = UrlParameter.Optional });
            context.MapRoute(null, "poapproval/list", new { controller = "PoApproval", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "poapproval/edit", new { controller = "PoApproval", action = "Edit", id = UrlParameter.Optional });
        }
    }
}