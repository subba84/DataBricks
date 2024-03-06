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
    public class CustomerActivationReportController : Controller
    {
        // GET: Report/CustomerActivationReport
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCustomerActivationReport(int customerid,string fromdate,string todate)
        {
            try
            {
                using(DbEntities context=new DbEntities())
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("select * from CustomerActivationLog where 1=1 ");
                    if(customerid > 0)
                    {
                        query.Append(" and CustomerId = " + customerid);
                    }
                    if(!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(todate))
                    {
                        query.Append(" and convert(datetime,activationdate,103) >= '" + Convert.ToDateTime(fromdate).ToSqlFormat() + "' and convert(datetime,activationdate,103) <= '" + Convert.ToDateTime(todate).ToSqlFormat() + "'");
                    }
                    var customerActiveLogs = context.Database.SqlQuery<CustomerActivationLog>(query.ToString());
                    if(customerActiveLogs!=null && customerActiveLogs.Count() > 0)
                    {
                        JsonResult jr = Json(new
                        {
                            HTML = this.RenderPartialView(@"~\Areas\Report\Views\CustomerActivationReport\_CustomerActivationReport.cshtml", customerActiveLogs.ToList())
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
    }
}