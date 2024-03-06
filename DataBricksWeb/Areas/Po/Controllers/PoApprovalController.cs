using DataBricksWeb.Areas.Po.Data;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.Po.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class PoApprovalController : Controller
    {
        [HttpGet]
        public ActionResult List()
        {
            var model = new PoModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(PoModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(PoModel model)
        {
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var poDetails = from l in context.Poes
                                    where l.Status == Statusses.Submitted
                                    select l;
                    if (poDetails != null && poDetails.Count() > 0)
                    {
                        model.Pos = poDetails.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id, int? pocid)
        {
            PoModel model = new PoModel();
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    if (id > 0)
                    {
                        var poDetails = from c in context.Poes
                                        where c.Id == id
                                        select c;
                        if (poDetails != null && poDetails.Count() > 0)
                        {
                            model.Po = poDetails.FirstOrDefault();

                            var poattachment = from a in context.Attachments
                                               where a.IsActive == true
                                               where a.IsDeleted == false
                                               where a.ObjectId == model.Po.Id
                                               where a.FileCategory == "PO"
                                               select a;
                            if (poattachment != null && poattachment.Count() > 0)
                            {
                                model.Po.PoAttachments = poattachment.ToList();
                            }

                            var poModules = from p in context.PoModules
                                            where p.IsActive == true
                                            where p.IsDeleted == false
                                            where p.PoId == model.Po.Id
                                            orderby p.Id
                                            select p;
                            if (poModules != null && poModules.Count() > 0)
                            {
                                model.PoModules = poModules.ToList();
                            }
                        }
                    }
                    else
                    {
                        var pocDetails = from c in context.Pocs
                                         where c.Id == pocid
                                         select c;
                        if (pocDetails != null && pocDetails.Count() > 0)
                        {
                            model.Po = new Database.Po();
                            var poc = pocDetails.FirstOrDefault();

                            model.Po.CustomerId = poc.CustomerId;
                            model.Po.CustomerName = poc.CustomerName;
                            model.Po.ProductId = poc.ProductId;
                            model.Po.ProductName = poc.ProductName;
                            model.Po.PublicIp = poc.PublicIp;
                            model.Po.QualifiedDomainName = poc.QualifiedDomainName;
                            model.Po.PocId = poc.Id;
                            model.Po.PODate = poc.POCDate;


                            var customerDetails = DataCache.Customers.Single(x => x.Id == model.Po.CustomerId);
                            var partnerDetails = from p in context.Users
                                                 where p.Id == customerDetails.CreatedBy
                                                 select p;
                            if (partnerDetails != null && partnerDetails.Count() > 0)
                            {
                                model.Po.PartnerId = partnerDetails.FirstOrDefault().Id;
                                model.Po.PartnerName = partnerDetails.FirstOrDefault().CompanyName;
                            }

                            model.PoModules = new List<PoModule>();

                            var pocModules = from p in context.PocModules
                                             where p.IsActive == true
                                             where p.IsDeleted == false
                                             where p.PocId == poc.Id
                                             orderby p.Id
                                             select p;
                            if (pocModules != null && pocModules.Count() > 0)
                            {
                                var modulesofpoc = pocModules.ToList();
                                foreach (var module in modulesofpoc)
                                {
                                    PoModule pomodule = new PoModule();
                                    pomodule.ModuleId = module.ModuleId;
                                    pomodule.ModuleName = module.ModuleName;
                                    pomodule.ModuleCount = module.ModuleCount;
                                    model.PoModules.Add(pomodule);
                                }
                            }
                        }
                    }

                    if(model.Po.Id > 0)
                    {
                        var licensingDetails = from l in context.Licensings
                                               where l.IsActive == true
                                               where l.IsDeleted == false
                                               where l.CustomerId == model.Po.CustomerId
                                               select l;
                        if(licensingDetails!=null && licensingDetails.Count() > 0)
                        {
                            model.Licensings = licensingDetails.ToList();
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
        public async Task<ActionResult> Edit(PoModel model)
        {
            try
            {
                LogService.LogActivity("PO Save", "Po Details - " + model.Po.ProductName + "  "+ model.Po.Status + " successful..");
                using (DbEntities context = new DbEntities())
                {
                    Database.Po dbpo = new Database.Po();
                    var podetails = from p in context.Poes
                                    where p.Id == model.Po.Id
                                    select p;
                    if(podetails!=null && podetails.Count() > 0)
                    {
                        var po = podetails.FirstOrDefault();
                        dbpo = po;
                        po.Status = model.Po.Status;
                        context.Poes.AddOrUpdate(po);
                        await context.SaveChangesAsync();

                        if (model.Po.Status == Statusses.Rejected)
                        {
                            // Check and Update POC Details as Converted to PO
                            var pocDetails = from p in context.Pocs
                                             where p.IsActive == true
                                             where p.IsDeleted == false
                                             where p.Id == po.PocId
                                             select p;
                            if (pocDetails != null && pocDetails.Count() > 0)
                            {
                                Database.Poc poc = pocDetails.FirstOrDefault();
                                poc.IsConvertedtoPo = false;
                                poc.ChangedBy = Convert.ToInt32(Session["UserId"]);
                                poc.ChangedOn = DateTime.Now;
                                context.Pocs.AddOrUpdate(poc);
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                    
                    

                    if (model.PoModules != null && model.PoModules.Count > 0)
                    {
                        foreach (var module in model.PoModules)
                        {
                            module.PoId = model.Po.Id;
                            module.IsDeleted = module.IsActive == true ? false : true;
                            module.ModuleName = DataCache.Modules.Single(x => x.Id == module.ModuleId).ModuleName;
                            if (module.Id > 0)
                            {
                                module.ChangedBy = Convert.ToInt32(Session["UserId"]);
                                module.ChangedOn = DateTime.Now;
                                context.PoModules.AddOrUpdate(module);
                            }
                            else
                            {
                                module.CreatedBy = Convert.ToInt32(Session["UserId"]);
                                module.CreatedOn = DateTime.Now;
                                context.PoModules.Add(module);
                            }
                            await context.SaveChangesAsync();

                            if (model.Po.Status == Statusses.Approved)
                            {
                                // Populate Licensing Details
                                Licensing licensing = new Licensing();
                                licensing.IsActive = true;
                                licensing.IsDeleted = false;
                                licensing.CreatedBy = Convert.ToInt32(Session["UserId"]);
                                licensing.CreatedOn = DateTime.Now;
                                licensing.PartnerId = dbpo.PartnerId;
                                licensing.PartnerName = dbpo.PartnerName;
                                licensing.PoId = dbpo.Id;
                                licensing.CustomerId = dbpo.CustomerId;
                                licensing.CustomerName = dbpo.CustomerName;
                                licensing.ProductId = dbpo.ProductId;
                                licensing.ProductName = dbpo.ProductName;
                                licensing.ModuleId = module.ModuleId;
                                licensing.ModuleName = module.ModuleName;
                                licensing.ModuleCount = module.ModuleCount;
                                licensing.ModuleAmount = Convert.ToInt32(module.ModuleAmount);
                                licensing.FromDate = module.FromDate;
                                licensing.ToDate = module.ToDate;
                                context.Licensings.Add(licensing);
                                await context.SaveChangesAsync();
                            }
                            
                        }
                    }

                    if (model.Po.Status == Statusses.Approved)
                    {
                        if(model.Licensings!=null && model.Licensings.Count > 0)
                        {
                            foreach(var item in model.Licensings)
                            {
                                var license = context.Licensings.Single(x => x.Id == item.Id);
                                license.ChangedBy = Convert.ToInt32(Session["UserId"]);
                                license.ChangedOn = DateTime.Now;
                                license.FromDate = item.FromDate;
                                license.ToDate = item.ToDate;
                                context.Licensings.AddOrUpdate(license);
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                }
                if(model.Po.Status == Statusses.Approved)
                {
                    TempData["Notification"] = "PO Details approved successfully";
                }
                else
                {
                    TempData["Notification"] = "PO Details rejected";
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("List");
        }
    }
}