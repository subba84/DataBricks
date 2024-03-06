using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb.Areas.WebApi.Data
{
    public class WebApiModel
    {
        public string ModuleName { get; set; }
        public string ModuleCount { get; set; }
        public string LicenseStatus { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}