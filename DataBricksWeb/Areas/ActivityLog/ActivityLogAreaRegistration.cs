using System.Web.Mvc;

namespace DataBricksWeb.Areas.ActivityLog
{
    public class ActivityLogAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ActivityLog";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "activity/list", new { controller = "Activity", action = "List", id = UrlParameter.Optional });
        }
    }
}