using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb.Database
{
    public partial class Po
    {
        public List<HttpPostedFileBase> PoFiles { get; set; }

        public List<Attachment> PoAttachments { get; set; }
    }
}