using System.Web.Mvc;

namespace DataBricksWeb.Areas.LoginApproval
{
    public class LoginApprovalAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LoginApproval";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "approval/list", new { controller = "Approval", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "approval/edit", new { controller = "Approval", action = "Edit", id = UrlParameter.Optional });
        }
    }
}