using System.Web.Mvc;

namespace DataBricksWeb.Areas.Dashboard
{
    public class DashboardAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Dashboard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null,"dashboard/landingpage",new { controller="Dashboard",action = "LandingPage", id = UrlParameter.Optional });
            context.MapRoute(null, "dashboard/forecastchart", new { controller = "Dashboard", action = "GetForecast", id = UrlParameter.Optional });
            context.MapRoute(null, "dashboard/getexpirypos", new { controller = "Dashboard", action = "GetExpiryPo", id = UrlParameter.Optional });
            context.MapRoute(null, "partnerdashboard", new { controller = "Dashboard", action = "PartnerDashboard", id = UrlParameter.Optional });
            context.MapRoute(null, "distributordashboard", new { controller = "Dashboard", action = "DistributorDashboard", id = UrlParameter.Optional });
        }
    }
}