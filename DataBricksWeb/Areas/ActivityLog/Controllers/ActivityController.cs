using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.ActivityLog.Controllers
{
    [CustomAuthorize]
    public class ActivityController : Controller
    {
        [HttpGet]
        public ActionResult List()
        {
            var model = new List<Database.ActivityLog>();
            model = PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(List<Database.ActivityLog> model)
        {
            model = PrepareList(model);
            return View(model);
        }

        public List<Database.ActivityLog> PrepareList(List<Database.ActivityLog> model)
        {
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var customerDetails = from c in context.ActivityLogs
                                          select c;
                    if (customerDetails != null && customerDetails.Count() > 0)
                    {
                        if(Convert.ToInt32(Session["RoleId"]) != UserRoleIds.Admin)
                        {
                            string company = Convert.ToString(Session["Company"]);
                            customerDetails = customerDetails.Where(x => x.CompanyName == company);
                        }
                        model = (customerDetails!=null && customerDetails.Count() > 0) ? customerDetails.ToList() : new List<Database.ActivityLog>();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }
    }
}