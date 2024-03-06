using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataBricksWeb.Database;

namespace DataBricksWeb.Areas.Lead.Model
{
    public class LeadModel
    {
        public List<DataBricksWeb.Database.Lead> Leads { get; set; }
    }
}