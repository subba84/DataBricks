using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb.Database
{
    public partial class Lead
    {
        public List<LeadModule> LeadModules { get; set; }
    }
}