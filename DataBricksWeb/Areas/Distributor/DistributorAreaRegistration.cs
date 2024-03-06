using System.Web.Mvc;

namespace DataBricksWeb.Areas.Distributor
{
    public class DistributorAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Distributor";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "distributor/list", new { controller = "ManageDistributor", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "distributor/edit", new { controller = "ManageDistributor", action = "Edit", id = UrlParameter.Optional });
            context.MapRoute(null, "distributor/delete", new { controller = "ManageDistributor", action = "Delete", id = UrlParameter.Optional });
        }
    }
}