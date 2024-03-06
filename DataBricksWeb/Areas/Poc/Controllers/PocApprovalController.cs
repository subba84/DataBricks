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
    public class PocApprovalController : Controller
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
                    var pocDetails = from l in context.Pocs
                                     where l.Status == Statusses.Submitted
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

        [HttpPost]
        public async Task<ActionResult> Edit(DataBricksWeb.Database.Poc poc)
        {
            try
            {
                LogService.LogActivity("POC Save", "Poc Details - " + poc.ProductName + "  " + poc.Status + " successful..");
                using (DbEntities context = new DbEntities())
                {
                    var pocdetails = from p in context.Pocs
                                     where p.Id == poc.Id
                                     select p;
                    if(pocdetails!=null && pocdetails.Count() > 0)
                    {
                        var pocdata = pocdetails.FirstOrDefault();
                        pocdata.Status = poc.Status;
                        pocdata.ChangedBy = Convert.ToInt32(Session["UserId"]);
                        pocdata.ChangedOn = DateTime.Now;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if(poc.Status == Statusses.Approved)
                TempData["Notification"] = "Poc Details approved successfully";
            else
                TempData["Notification"] = "Poc Details rejected";
            return RedirectToAction("List");
        }

    }
}