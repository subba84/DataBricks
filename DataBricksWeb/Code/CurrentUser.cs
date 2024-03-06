using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb
{
    public class CurrentUser
    {
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string EmailId { get; set; }
    }
}