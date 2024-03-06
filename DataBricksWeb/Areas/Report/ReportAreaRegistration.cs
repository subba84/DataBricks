using System.Web.Mvc;

namespace DataBricksWeb.Areas.Report
{
    public class ReportAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Report";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "admin/report", new { controller = "AdminReport", action = "Report", id = UrlParameter.Optional });
            context.MapRoute(null, "admin/getadminreport", new { controller = "AdminReport", action = "GetAdminReport", id = UrlParameter.Optional });
            context.MapRoute(null, "partner/report", new { controller = "PartnerReport", action = "Report", id = UrlParameter.Optional });
            context.MapRoute(null, "partner/getpartnerreport", new { controller = "PartnerReport", action = "GetPartnerReport", id = UrlParameter.Optional });
            context.MapRoute(null, "distributor/report", new { controller = "DistributorReport", action = "Report", id = UrlParameter.Optional });
            context.MapRoute(null, "distributor/getdistributorreport", new { controller = "DistributorReport", action = "GetDistributorReport", id = UrlParameter.Optional });
            context.MapRoute(null, "admin/customeractivationreport", new { controller = "CustomerActivationReport", action = "Index", id = UrlParameter.Optional });
            context.MapRoute(null, "admin/getactivationreport", new { controller = "CustomerActivationReport", action = "GetCustomerActivationReport", id = UrlParameter.Optional });
            context.MapRoute(null, "partners/list", new { controller = "Partner", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "partners/details", new { controller = "Partner", action = "Details", id = UrlParameter.Optional });
            context.MapRoute(null, "customers/list", new { controller = "Customer", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "customers/details", new { controller = "Customer", action = "Details", id = UrlParameter.Optional });
        }
    }
}