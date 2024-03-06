using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb.Areas.Dashboard.Models
{
    public class ForecastModel
    {
        public int TotalSaleAmount { get; set; }
        public int PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
    }
}