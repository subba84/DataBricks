using System.Web.Mvc;

namespace DataBricksWeb.Areas.Login
{
    public class LoginAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Login";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null,"login/index",new { controller="Authenticate", action = "Index", id = UrlParameter.Optional });
            context.MapRoute(null, "logout", new { controller = "Authenticate", action = "Logout", id = UrlParameter.Optional });
            context.MapRoute(null, "forgotpassword", new { controller = "Authenticate", action = "ForgotPassword", id = UrlParameter.Optional });
            context.MapRoute(null, "checkforemailexistance", new { controller = "Authenticate", action = "CheckforUserEmail", id = UrlParameter.Optional });
            context.MapRoute(null, "resetpassword", new { controller = "Authenticate", action = "ResetPassword", id = UrlParameter.Optional });
        }
    }
}