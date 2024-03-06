using System.Web.Mvc;

namespace DataBricksWeb.Areas.UploadDownload
{
    public class UploadDownloadAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "UploadDownload";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "zip/list", new { controller = "ZipFile", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "zip/edit", new { controller = "ZipFile", action = "Edit", id = UrlParameter.Optional });

            context.MapRoute(null, "doc/list", new { controller = "Document", action = "List", id = UrlParameter.Optional });
            context.MapRoute(null, "doc/edit", new { controller = "Document", action = "Edit", id = UrlParameter.Optional });
        }
    }
}