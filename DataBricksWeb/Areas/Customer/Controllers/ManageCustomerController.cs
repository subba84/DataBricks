using DataBricksWeb.Areas.Customer.Model;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using db = DataBricksWeb.Database;

namespace DataBricksWeb.Areas.Customer.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class ManageCustomerController : Controller
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
                    int createdUser = Convert.ToInt32(Session["UserId"]);
                    var customerDetails = from c in context.Customers
                                          where c.CreatedBy == createdUser
                                          select c;
                    if(customerDetails!=null && customerDetails.Count() > 0)
                    {
                        model.Customers = customerDetails.ToList();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            db.Customer model = new db.Customer();
            try
            {
                using(DbEntities context=new DbEntities())
                {
                    var customerDetails = from c in context.Customers
                                          where c.Id == id
                                          select c;
                    if(customerDetails!=null && customerDetails.Count() > 0)
                    {
                        model = customerDetails.FirstOrDefault();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(db.Customer model)
        {
            try
            {
                LogService.LogActivity("Customer Save", "Customer - " + model.CustomerName + " details saving..");
                using(DbEntities context=new DbEntities())
                {
                    //model.IsDeleted = model.IsActive == true ? false : true;
                    model.IsActive = true;
                    model.IsDeleted = false;
                    model.CountryName = DataCache.Countries.Single(x => x.Id == model.CountryId).CountryName;
                    model.CityName = DataCache.Cities.Single(x => x.Id == model.CityId).CityName;
                    model.PartnerId = Convert.ToInt32(Session["UserId"]);
                    model.PartnerName = Convert.ToString(Session["Company"]);
                    if (model.Id > 0)
                    {
                        model.ChangedBy = Convert.ToInt32(Session["UserId"]);
                        model.ChangedOn = DateTime.Now;
                        context.Customers.AddOrUpdate(model);
                    }
                    else
                    {
                        model.CreatedBy = Convert.ToInt32(Session["UserId"]);
                        model.CreatedOn = DateTime.Now;
                        context.Customers.Add(model);
                    }
                    await context.SaveChangesAsync();
                }
                new Thread(() => DataCache.RefreshCustomer()).Start();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            TempData["Notification"] = "Customer Details Saved Successfully";
            return RedirectToAction("List");
        }
    }
}