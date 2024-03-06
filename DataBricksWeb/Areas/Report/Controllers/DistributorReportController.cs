﻿using DataBricksWeb.Areas.Report.Data;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.Report.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class DistributorReportController : Controller
    {
        // GET: Report/DistributorReport
        public ActionResult Report()
        {
            using(DbEntities context=new DbEntities())
            {
                int distributorid = Convert.ToInt32(Session["UserId"]);
                var associatedpartners = from p in context.DistributorPartners
                                         where p.IsActive == true
                                         where p.IsDeleted == false
                                         where p.DistributorId == distributorid
                                         select p.PartnerId;
                if(associatedpartners!=null && associatedpartners.Count() > 0)
                {
                    TempData["AssociatedPartners"] = associatedpartners.ToList();
                }
            }
            return View();
        }

        public JsonResult GetDistributorReport(int? partnerid, int? customerid)
        {
            List<AdminReportModel> reports = new List<AdminReportModel>();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"Select * from Po where 1=1");
                if (partnerid != null && partnerid > 0)
                {
                    sb.Append(@" and PartnerId = " + partnerid);
                }
                if (customerid != null && customerid > 0)
                {
                    sb.Append(@" and CustomerId = " + customerid);
                }
                List<Database.Po> pos = null;
                using (DbEntities context = new DbEntities())
                {
                    pos = context.Database.SqlQuery<Database.Po>(sb.ToString()).ToList();
                    if (pos != null && pos.Count > 0)
                    {
                        foreach (var po in pos)
                        {
                            AdminReportModel model = new AdminReportModel();
                            model.PartnerName = po.PartnerName;
                            model.CustomerName = po.CustomerName;
                            model.OrderAmount = Convert.ToString(po.TotalModuleAmount);
                            model.PoId = po.Id;

                            var modules = from m in context.PoModules
                                          where m.IsActive == true
                                          where m.IsDeleted == false
                                          where m.PoId == po.Id
                                          select m.ModuleName;
                            if (modules != null && modules.Count() > 0)
                            {
                                model.ModuleNames = string.Join(",", modules);
                            }
                            reports.Add(model);
                        }
                    }
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Report\Views\Shared\_AdminReport.cshtml", reports)
                }, JsonRequestBehavior.AllowGet);

                jr.MaxJsonLength = int.MaxValue;
                return jr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}