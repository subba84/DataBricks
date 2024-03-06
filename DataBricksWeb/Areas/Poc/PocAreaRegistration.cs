using System.Web.Mvc;

namespace DataBricksWeb.Areas.Poc
{
    public class PocAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Poc";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "poc/list", new { controller = "ManagePoc", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "poc/edit", new { controller = "ManagePoc", action = "Edit", id = UrlParameter.Optional });
            context.MapRoute(null, "poc/getleadmodules", new { controller = "ManagePoc", action = "GetModulesbasedonLead", id = UrlParameter.Optional });
            context.MapRoute(null, "poc/details", new { controller = "ManagePoc", action = "Details", id = UrlParameter.Optional });
            context.MapRoute(null, "pocapproval/list", new { controller = "PocApproval", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "pocapproval/edit", new { controller = "PocApproval", action = "Edit", id = UrlParameter.Optional });
        }
    }
}