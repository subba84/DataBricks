using DataBricksWeb.Areas.Dashboard.Models;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.Dashboard.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class DashboardController : Controller
    {
        public ActionResult LandingPage()
        {
            return View();
        }

        public ActionResult PartnerDashboard()
        {
            return View();
        }

        public ActionResult DistributorDashboard()
        {
            return View();
        }

        public JsonResult GetForecast(int partnerid, int year, string category)
        {
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    if (Convert.ToInt32(Session["RoleId"]) == UserRoleIds.Distributor)
                    {
                        int userid = Convert.ToInt32(Session["UserId"]);
                        var associatedpartnerts = from dp in context.DistributorPartners
                                                  where dp.DistributorId == userid
                                                  select dp.PartnerId;
                        if (associatedpartnerts != null && associatedpartnerts.Count() > 0)
                        {
                            var forecastdetails = context.Database.SqlQuery<ForecastModel>(@"select Sum(TotalModuleAmount) as TotalSaleAmount,PartnerId,PartnerName,CustomerName,Upper(CompanyName) CompanyName from " + category + " where status='Approved' "+ (category == "Lead" ? " and IsConvertedtoPoc is null" : "") + "  " + (category == "Poc" ? " and IsConvertedtoPo is null" : "") + "  and Year(CreatedOn)='" + year + "' and PartnerId in (" + string.Join(",", associatedpartnerts) + ") group by PartnerId,PartnerName,CompanyName,CustomerName");
                            if (forecastdetails != null && forecastdetails.Count() > 0)
                            {
                                return Json(forecastdetails.ToList(), JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    if (partnerid == 0)
                    {
                        var forecastdetails = context.Database.SqlQuery<ForecastModel>(@"select Sum(TotalModuleAmount) as TotalSaleAmount,PartnerId,PartnerName,CustomerName,Upper(CompanyName) CompanyName from " + category + " where status='Approved' " + (category == "Lead" ? " and IsConvertedtoPoc is null" : "") + "  " + (category == "Poc" ? " and IsConvertedtoPo is null" : "") + "   and Year(CreatedOn)='" + year + "' group by PartnerId,PartnerName,CompanyName,CustomerName");
                        if (forecastdetails != null && forecastdetails.Count() > 0)
                        {
                            return Json(forecastdetails.ToList(), JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var forecastdetails = context.Database.SqlQuery<ForecastModel>(@"select Sum(TotalModuleAmount) as TotalSaleAmount,PartnerId,PartnerName,CustomerName,Upper(CompanyName) CompanyName from " + category + " where status='Approved' " + (category == "Lead" ? " and IsConvertedtoPoc is null" : "") + "  " + (category == "Poc" ? " and IsConvertedtoPo is null" : "") + "   and Year(CreatedOn)='" + year + "' and PartnerId='" + partnerid + "' group by PartnerId,PartnerName,CompanyName,CustomerName");
                        if (forecastdetails != null && forecastdetails.Count() > 0)
                        {
                            return Json(forecastdetails.ToList(), JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExpiryPo(int partnerid, int year, int month)
        {
            List<Database.Licensing> model = new List<Database.Licensing>();
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    if (Convert.ToInt32(Session["RoleId"]) == UserRoleIds.Distributor)
                    {
                        int userid = Convert.ToInt32(Session["UserId"]);
                        var associatedpartnerts = from dp in context.DistributorPartners
                                                  where dp.DistributorId == userid
                                                  select dp.PartnerId;
                        if (associatedpartnerts != null && associatedpartnerts.Count() > 0)
                        {
                            var podetails = context.Database.SqlQuery<Database.Licensing>(@"select * from Licensing where Month(ToDate)='" + month + "' and Year(ToDate)='" + year + "'  and PartnerId in (" + string.Join(",", associatedpartnerts) + ")");
                            if (podetails != null && podetails.Count() > 0)
                            {
                                model = podetails.ToList();
                                JsonResult jr = Json(new
                                {
                                    HTML = this.RenderPartialView(@"~\Areas\Dashboard\Views\Shared\_renewalexpiry.cshtml", podetails.ToList())
                                }, JsonRequestBehavior.AllowGet);

                                jr.MaxJsonLength = int.MaxValue;
                                return jr;
                            }
                        }
                    }
                    else if (Convert.ToInt32(Session["RoleId"]) == UserRoleIds.Partner)
                    {
                        var podetails = context.Database.SqlQuery<Database.Licensing>(@"select * from Licensing where Month(ToDate)='" + month + "' and Year(ToDate)='" + year + "' and PartnerId = '"+partnerid+"'");
                        if (podetails != null && podetails.Count() > 0)
                        {
                            model = podetails.ToList();
                            JsonResult jr = Json(new
                            {
                                HTML = this.RenderPartialView(@"~\Areas\Dashboard\Views\Shared\_renewalexpiry.cshtml", podetails.ToList())
                            }, JsonRequestBehavior.AllowGet);

                            jr.MaxJsonLength = int.MaxValue;
                            return jr;
                        }
                    }
                    else
                    {
                        var podetails = context.Database.SqlQuery<Database.Licensing>(@"select * from Licensing where Month(ToDate)='" + month + "' and Year(ToDate)='" + year + "'");
                        if (podetails != null && podetails.Count() > 0)
                        {
                            model = podetails.ToList();
                            JsonResult jr = Json(new
                            {
                                HTML = this.RenderPartialView(@"~\Areas\Dashboard\Views\Shared\_renewalexpiry.cshtml", podetails.ToList())
                            }, JsonRequestBehavior.AllowGet);

                            jr.MaxJsonLength = int.MaxValue;
                            return jr;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}