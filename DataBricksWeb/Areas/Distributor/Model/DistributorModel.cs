using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb.Areas.Distributor.Model
{
    public class DistributorModel
    {
        public List<Database.User> Distributors { get; set; }

        public List<Database.DistributorPartner> DistributorPartners { get; set; }
    }
}