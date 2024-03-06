using System.Web.Mvc;

namespace DataBricksWeb.Areas.Partner
{
    public class PartnerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Partner";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null,"partner/registration",new { controller = "Registration", action = "Index", id = UrlParameter.Optional });
            context.MapRoute(null, "partner/saveregistration", new { controller = "Registration", action = "Edit", id = UrlParameter.Optional });
            context.MapRoute(null, "partner/getcitiesbasedoncountryid", new { controller = "Registration", action = "GetCitiesbasedonCountryId", id = UrlParameter.Optional });


            context.MapRoute(null, "user/list", new { controller = "ManageUser", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "user/edit", new { controller = "ManageUser", action = "Edit", id = UrlParameter.Optional });

            context.MapRoute(null, "partner/getcaptcha", new { controller = "Registration", action = "GetCaptchaImage", id = UrlParameter.Optional });
        }
    }
}