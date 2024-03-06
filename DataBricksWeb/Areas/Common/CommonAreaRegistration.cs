using System.Web.Mvc;

namespace DataBricksWeb.Areas.Common
{
    public class CommonAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Common";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "common/getleadsbasedoncustomer", new { controller = "ManageCommon", action = "GetLeadsBasedonCustomer", id = UrlParameter.Optional });
            context.MapRoute(null, "common/getmodulesbasedonproduct", new { controller = "ManageCommon", action = "GetModulesbasedonProduct", id = UrlParameter.Optional });
            context.MapRoute(null, "common/downloadfile", new { controller = "ManageCommon", action = "DownloadFile", id = UrlParameter.Optional });
            context.MapRoute(null, "common/adddaystodate", new { controller = "ManageCommon", action = "AddDaystoDate", id = UrlParameter.Optional });
            context.MapRoute(null, "common/getcustomersbasedonpartner", new { controller = "ManageCommon", action = "GetCustomersbasedonPartner", id = UrlParameter.Optional });
        }
    }
}