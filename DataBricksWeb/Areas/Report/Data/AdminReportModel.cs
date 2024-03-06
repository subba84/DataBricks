using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb.Areas.Report.Data
{
    public class AdminReportModel
    {
        public int PoId { get; set; }
        public string PartnerName { get; set; }
        public string CustomerName { get; set; }
        public string ModuleNames { get; set; }
        public string OrderAmount { get; set; }
    }
}