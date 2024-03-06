using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb.Areas.Po.Data
{
    public class PoModel
    {
        public List<DataBricksWeb.Database.Po> Pos { get; set; }
        public DataBricksWeb.Database.Po Po { get; set; }
        public List<Database.PoModule> PoModules { get; set; }
        public List<Database.Licensing> Licensings { get; set; }
    }
}