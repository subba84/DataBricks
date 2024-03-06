using DataBricksWeb.Areas.Customer.Model;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.Report.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class CustomerController : Controller
    {
        [HttpGet]
        public ActionResult List()
        {
            var model = new CustomerModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(CustomerModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(CustomerModel model)
        {
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var users = from u in context.Customers
                                where u.IsActive == true
                                orderby u.CustomerName
                                select u;
                    if (users != null && users.Count() > 0)
                    {
                        model.Customers = users.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Details(int id)
        {
            Database.Customer user = new Database.Customer();
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var users = from u in context.Customers
                                where u.Id == id
                                select u;
                    if (users != null && users.Count() > 0)
                    {
                        user = users.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(user);
        }
    }
}