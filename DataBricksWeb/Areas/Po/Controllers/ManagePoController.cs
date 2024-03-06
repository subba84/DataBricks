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
    public class ManagePoController : Controller
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
                    string company = Convert.ToString(Session["Company"]);
                    var poDetails = from l in context.Poes
                                    where l.CompanyName == company
                                    where l.Status == Statusses.Approved
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
        public ActionResult Edit(int? id,int? pocid)
        {
            PoModel model = new PoModel();
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    if(id > 0)
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
                                foreach(var module in modulesofpoc)
                                {
                                    PoModule pomodule = new PoModule();
                                    pomodule.ModuleId = module.ModuleId;
                                    pomodule.ModuleName = module.ModuleName;
                                    pomodule.ModuleCount = module.ModuleCount;
                                    pomodule.ModuleAmount = Convert.ToString(module.ModuleAmount);
                                    pomodule.FromDate = module.FromDate;
                                    pomodule.ToDate = module.ToDate;
                                    model.PoModules.Add(pomodule);
                                }
                            }
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
                using(DbEntities context=new DbEntities())
                {
                    LogService.LogActivity("PO Save", "Po Details - " + model.Po.ProductName + " added successful..");
                    //model.Po.IsDeleted = model.Po.IsActive == true ? false : true;
                    model.Po.IsActive = true;
                    model.Po.IsDeleted = false;
                    var customerDetails = DataCache.Customers.Single(x => x.Id == model.Po.CustomerId);
                    model.Po.CustomerName = customerDetails.CustomerName;
                    model.Po.PartnerId = customerDetails.PartnerId;
                    model.Po.PartnerName = customerDetails.PartnerName;
                    model.Po.ProductName = DataCache.Products.Single(x => x.Id == model.Po.ProductId).ProductName;
                    model.Po.Status = Statusses.Submitted;
                    model.Po.CompanyName = Convert.ToString(Session["Company"]);
                    int totalAmount = model.PoModules.Where(x => x.IsActive == true).Select(x => Convert.ToInt32(x.ModuleAmount) * x.ModuleCount).Sum(x => Convert.ToInt32(x));
                    model.Po.TotalModuleAmount = totalAmount;
                    if (model.Po.Id > 0)
                    {
                        model.Po.ChangedBy = Convert.ToInt32(Session["UserId"]);
                        model.Po.ChangedOn = DateTime.Now;
                        context.Poes.AddOrUpdate(model.Po);
                    }
                    else
                    {
                        model.Po.CreatedBy = Convert.ToInt32(Session["UserId"]);
                        model.Po.CreatedOn = DateTime.Now;
                        context.Poes.Add(model.Po);
                    }
                    await context.SaveChangesAsync();

                    // Check and Update POC Details as Converted to PO
                    var pocDetails = from p in context.Pocs
                                     where p.IsActive == true
                                     where p.IsDeleted == false
                                     where p.Id == model.Po.PocId
                                     select p;
                    if(pocDetails!=null && pocDetails.Count() > 0)
                    {
                        Database.Poc poc = pocDetails.FirstOrDefault();
                        if(poc.IsConvertedtoPo == false || poc.IsConvertedtoPo == null)
                        {
                            poc.IsConvertedtoPo = true;
                            poc.ChangedBy = Convert.ToInt32(Session["UserId"]);
                            poc.ChangedOn = DateTime.Now;
                            context.Pocs.AddOrUpdate(poc);
                            await context.SaveChangesAsync();
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
                        }
                    }

                    if (model.Po.PoFiles != null)
                    {
                        foreach(var fl in model.Po.PoFiles)
                        {
                            string directorypath = System.IO.Path.Combine(DataCache.MapPath, "Uploads/PO", Convert.ToString(model.Po.Id));
                            if (!System.IO.Directory.Exists(directorypath))
                            {
                                System.IO.Directory.CreateDirectory(directorypath);
                            }
                            Attachment attachment = new Attachment();
                            attachment.FileName = fl.FileName;
                            attachment.FileType = fl.ContentType;
                            attachment.RelativePath = System.IO.Path.Combine("/Uploads/PO", Convert.ToString(model.Po.Id),fl.FileName);
                            attachment.IsActive = true;
                            attachment.IsDeleted = false;
                            attachment.FileCategory = "PO";
                            attachment.CreatedBy = Convert.ToInt32(Session["UserId"]);
                            attachment.CreatedOn = DateTime.Now;
                            attachment.ObjectId = model.Po.Id;
                            fl.SaveAs(System.IO.Path.Combine(directorypath, fl.FileName));
                            context.Attachments.Add(attachment);
                            await context.SaveChangesAsync();
                        }                        
                    }
                }
                TempData["Notification"] = "PO Details submitted successfully and Sent for Admin Approval";
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            PoModel model = new PoModel();
            try
            {
                using (DbEntities context = new DbEntities())
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(model);
        }
    }
}