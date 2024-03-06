using System.Web.Mvc;

namespace DataBricksWeb.Areas.WebApi
{
    public class WebApiAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WebApi";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "webapi/moduledetails", new { controller = "PoWebApi", action = "Get", id = UrlParameter.Optional });
            context.MapRoute(null, "webapi/savecustomeractivationlog", new { controller = "PoWebApi", action = "savecustomeractivationlog", id = UrlParameter.Optional });
        }
    }
}