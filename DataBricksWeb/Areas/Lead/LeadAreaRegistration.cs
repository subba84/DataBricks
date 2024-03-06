using System.Web.Mvc;

namespace DataBricksWeb.Areas.Lead
{
    public class LeadAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Lead";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "lead/list", new { controller = "LeadManage", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "lead/edit", new { controller = "LeadManage", action = "Edit", id = UrlParameter.Optional });
            context.MapRoute(null, "lead/details", new { controller = "LeadManage", action = "Details", id = UrlParameter.Optional });
        }
    }
}