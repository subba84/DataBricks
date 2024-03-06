using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb
{
    public static class LogService
    {
        public static int? LogActivity(string category,string details)
        {
            int? activityid = null;
            using (DbEntities context = new DbEntities())
            {
                ActivityLog log = new ActivityLog();
                log.CreatedBy = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
                log.CreatedOn = DateTime.Now;
                log.RoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]);
                log.RoleName = Convert.ToString(System.Web.HttpContext.Current.Session["RoleName"]);
                log.UserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
                log.FullName = Convert.ToString(System.Web.HttpContext.Current.Session["FullName"]);
                log.CompanyName = Convert.ToString(System.Web.HttpContext.Current.Session["Company"]);
                log.ActivityDetails = details;
                log.ActivityCategory = category;
                context.ActivityLogs.Add(log);
                context.SaveChanges();
                activityid = log.Id;
            }
            return activityid;
        }
    }
}