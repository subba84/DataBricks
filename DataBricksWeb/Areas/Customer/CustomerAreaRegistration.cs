using System.Web.Mvc;

namespace DataBricksWeb.Areas.Customer
{
    public class CustomerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Customer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "customer/list", new { controller = "ManageCustomer", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "customer/edit", new { controller = "ManageCustomer", action = "Edit", id = UrlParameter.Optional });
        }
    }
}