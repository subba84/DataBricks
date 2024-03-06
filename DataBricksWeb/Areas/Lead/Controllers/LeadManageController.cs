using DataBricksWeb.Areas.Lead.Model;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using db = DataBricksWeb.Database;

namespace DataBricksWeb.Areas.Lead.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class LeadManageController : Controller
    {
        [HttpGet]
        public ActionResult List()
        {
            var model = new LeadModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(LeadModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(LeadModel model)
        {
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    string company = Convert.ToString(Session["Company"]);
                    var leadDetails = from l in context.Leads
                                      where l.CompanyName == company
                                          select l;
                    if (leadDetails != null && leadDetails.Count() > 0)
                    {
                        model.Leads = leadDetails.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            db.Lead model = new db.Lead();
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var leadDetails = from c in context.Leads
                                          where c.Id == id
                                          select c;
                    if (leadDetails != null && leadDetails.Count() > 0)
                    {
                        model = leadDetails.FirstOrDefault();

                        var leadmodules = from l in context.LeadModules
                                          where l.IsActive == true
                                          where l.IsDeleted == false
                                          where l.LeadId == model.Id
                                          orderby l.Id
                                          select l;
                        if(leadmodules!=null && leadmodules.Count() > 0)
                        {
                            model.LeadModules = leadmodules.ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(db.Lead model)
        {
            try
            {
                LogService.LogActivity("Lead Save", "Lead - " + model.LeadDescription + " details saving..");
                using (DbEntities context = new DbEntities())
                {
                    model.IsDeleted = model.IsActive == true ? false : true;
                    var customerDetails = DataCache.Customers.Single(x => x.Id == model.CustomerId);
                    model.CustomerName = customerDetails.CustomerName;
                    model.PartnerId = customerDetails.PartnerId;
                    model.PartnerName = customerDetails.PartnerName;
                    model.ProductName = DataCache.Products.Single(x => x.Id == model.ProductId).ProductName;
                    model.Status = Statusses.Approved;
                    model.CompanyName = Convert.ToString(Session["Company"]);
                    if(model.LeadModules!=null && model.LeadModules.Count > 0)
                    {
                        int totalAmount = 0;
                        foreach(var item in model.LeadModules)
                        {
                            if(item.ModuleId > 0)
                            {
                                var moduleDetails = DataCache.Modules.Single(x => x.Id == item.ModuleId);
                                item.ModuleName = moduleDetails.ModuleName;
                                item.ModuleAmount = moduleDetails.ModulePrice * item.ModuleCount;
                                totalAmount += Convert.ToInt32(item.ModuleAmount);
                            }
                        }
                        model.TotalModuleAmount = totalAmount;
                    }
                    if (model.Id > 0)
                    {
                        model.ChangedBy = Convert.ToInt32(Session["UserId"]);
                        model.ChangedOn = DateTime.Now;
                        context.Leads.AddOrUpdate(model);
                    }
                    else
                    {
                        model.CreatedBy = Convert.ToInt32(Session["UserId"]);
                        model.CreatedOn = DateTime.Now;
                        context.Leads.Add(model);
                    }
                    await context.SaveChangesAsync();
                    if(model.LeadModules!=null && model.LeadModules.Count > 0)
                    {
                        foreach(var item in model.LeadModules)
                        {
                            item.LeadId = model.Id;
                            item.IsDeleted = item.IsActive == true ? false : true;
                            if(item.Id > 0)
                            {
                                item.ChangedBy = Convert.ToInt32(Session["UserId"]);
                                item.ChangedOn = DateTime.Now;
                                context.LeadModules.AddOrUpdate(item);
                            }
                            else
                            {
                                item.CreatedBy = Convert.ToInt32(Session["UserId"]);
                                item.CreatedOn = DateTime.Now;
                                context.LeadModules.AddOrUpdate(item);
                            }
                            await context.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            TempData["Notification"] = "Lead Details Saved Successfully";
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            db.Lead model = new db.Lead();
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var leadDetails = from c in context.Leads
                                      where c.Id == id
                                      select c;
                    if (leadDetails != null && leadDetails.Count() > 0)
                    {
                        model = leadDetails.FirstOrDefault();

                        var leadmodules = from l in context.LeadModules
                                          where l.IsActive == true
                                          where l.IsDeleted == false
                                          where l.LeadId == model.Id
                                          orderby l.Id
                                          select l;
                        if (leadmodules != null && leadmodules.Count() > 0)
                        {
                            model.LeadModules = leadmodules.ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(model);
        }
    }
}