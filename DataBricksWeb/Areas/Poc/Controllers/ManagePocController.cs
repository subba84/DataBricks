using DataBricksWeb.Areas.Poc.Model;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.Poc.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class ManagePocController : Controller
    {
        [HttpGet]
        public ActionResult List()
        {
            var model = new PocModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(PocModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(PocModel model)
        {
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    string company = Convert.ToString(Session["Company"]);
                    var pocDetails = from l in context.Pocs
                                     where l.CompanyName == company
                                     where (l.IsConvertedtoPo == null || l.IsConvertedtoPo == false)
                                     select l;
                    if (pocDetails != null && pocDetails.Count() > 0)
                    {
                        model.Pocs = pocDetails.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetModulesbasedonLead(int id)
        {
            try
            {
                using(DbEntities context=new DbEntities())
                {
                    var modules = from m in context.LeadModules
                                  where m.IsActive == true
                                  where m.IsDeleted == false
                                  where m.LeadId == id
                                  select m;
                    if(modules!=null && modules.Count() > 0)
                    {
                        JsonResult jr = Json(new
                        {
                            HTML = this.RenderPartialView(@"~\Areas\Poc\Views\ManagePoc\_leadmodules.cshtml", modules.ToList())
                        }, JsonRequestBehavior.AllowGet);

                        jr.MaxJsonLength = int.MaxValue;
                        return jr;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json("",JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            DataBricksWeb.Database.Poc model = new DataBricksWeb.Database.Poc();
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var pocDetails = from c in context.Pocs
                                     where c.Id == id
                                     select c;
                    if (pocDetails != null && pocDetails.Count() > 0)
                    {
                        model = pocDetails.FirstOrDefault();

                        var pocModules = from p in context.PocModules
                                         where p.IsActive == true
                                         where p.IsDeleted == false
                                         where p.PocId == model.Id
                                         orderby p.Id
                                         select p;
                        if(pocModules!=null && pocModules.Count() > 0)
                        {
                            TempData["PocModule"] = pocModules.ToList();
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
        public async Task<ActionResult> Edit(DataBricksWeb.Database.Poc poc,List<PocModule> model)
        {
            try
            {
                LogService.LogActivity("POC Save", "Poc Details - " + poc.ProductName + " saved successful..");
                using (DbEntities context = new DbEntities())
                {
                    //poc.IsDeleted = poc.IsActive == true ? false : true;
                    poc.IsActive = true;
                    poc.IsDeleted = false;
                    var customerDetails = DataCache.Customers.Single(x => x.Id == poc.CustomerId);
                    poc.CustomerName = customerDetails.CustomerName;
                    poc.PartnerId = customerDetails.PartnerId;
                    poc.PartnerName = customerDetails.PartnerName;
                    poc.ProductName = DataCache.Products.Single(x => x.Id == poc.ProductId).ProductName;
                    poc.LeadName = context.Leads.Single(x => x.Id == poc.LeadId).LeadDescription;
                    poc.Status = Statusses.Submitted;
                    poc.CompanyName = Convert.ToString(Session["Company"]);
                    if (model!=null && model.Count > 0)
                    {
                        int totalAmount = 0;
                        foreach(var item in model)
                        {
                            if(item.ModuleId > 0)
                            {
                                var moduleDetails = DataCache.Modules.Single(x => x.Id == item.ModuleId);
                                item.ModuleName = moduleDetails.ModuleName;
                                item.ModuleAmount = moduleDetails.ModulePrice * item.ModuleCount;
                                totalAmount += Convert.ToInt32(item.ModuleAmount);
                            }
                            
                        }
                        poc.TotalModuleAmount = totalAmount;
                    }

                    if (poc.Id > 0)
                    {
                        poc.ChangedBy = Convert.ToInt32(Session["UserId"]);
                        poc.ChangedOn = DateTime.Now;
                        context.Pocs.AddOrUpdate(poc);
                    }
                    else
                    {
                        poc.CreatedBy = Convert.ToInt32(Session["UserId"]);
                        poc.CreatedOn = DateTime.Now;
                        context.Pocs.Add(poc);
                    }
                    await context.SaveChangesAsync();
                    if(model!=null && model.Count > 0)
                    {
                        foreach(var module in model)
                        {
                            module.PocId = poc.Id;
                            module.IsDeleted = module.IsActive == true ? false : true;
                            
                            if (module.Id > 0)
                            {
                                module.ChangedBy = Convert.ToInt32(Session["UserId"]);
                                module.ChangedOn = DateTime.Now;
                                context.PocModules.AddOrUpdate(module);
                            }
                            else
                            {
                                module.CreatedBy = Convert.ToInt32(Session["UserId"]);
                                module.CreatedOn = DateTime.Now;
                                context.PocModules.Add(module);
                            }
                            await context.SaveChangesAsync();
                        }
                    }

                    // Update Lead to be converted to Poc
                    var leadDetails = from l in context.Leads
                                      where l.IsActive == true
                                      where l.IsDeleted == false
                                      where l.Id == poc.LeadId
                                      select l;
                    if(leadDetails!=null && leadDetails.Count() > 0)
                    {
                        Database.Lead ld = leadDetails.FirstOrDefault();
                        ld.IsConvertedtoPoc = true;
                        context.Leads.AddOrUpdate(ld);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            TempData["Notification"] = "Poc Details Submitted Successfully and Sent for Admin Approval";
            return RedirectToAction("List");
        }


        [HttpGet]
        public ActionResult Details(int? id)
        {
            DataBricksWeb.Database.Poc model = new DataBricksWeb.Database.Poc();
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var pocDetails = from c in context.Pocs
                                     where c.Id == id
                                     select c;
                    if (pocDetails != null && pocDetails.Count() > 0)
                    {
                        model = pocDetails.FirstOrDefault();

                        var pocModules = from p in context.PocModules
                                         where p.IsActive == true
                                         where p.IsDeleted == false
                                         where p.PocId == model.Id
                                         orderby p.Id
                                         select p;
                        if (pocModules != null && pocModules.Count() > 0)
                        {
                            TempData["PocModule"] = pocModules.ToList();
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