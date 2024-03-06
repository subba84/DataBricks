using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.Login.Controllers
{
    public class AuthenticateController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string email, string password)
        {
            try
            {
               
                using (DbEntities context = new DbEntities())
                {
                    var userDetails = from u in context.Users
                                      where u.IsActive == true
                                      where u.IsDeleted == false
                                      where u.EmailId == email.Trim()
                                      where u.Password == password.Trim()
                                      where u.IsApproved == true
                                      select u;
                    if (userDetails != null && userDetails.Count() > 0)
                    {
                        User user = userDetails.FirstOrDefault();
                        InitializeContext(user);
                        LogService.LogActivity("Login", "User - " + user.FullName + " logged in successful..");
                        if (Convert.ToInt32(Session["RoleId"]) == UserRoleIds.Admin)
                            return RedirectToAction("LandingPage", "Dashboard", new { area = "Dashboard" });
                        else if(Convert.ToInt32(Session["RoleId"]) == UserRoleIds.Distributor)
                            return RedirectToAction("DistributorDashboard", "Dashboard", new { area = "Dashboard" });
                        else
                            return RedirectToAction("PartnerDashboard", "Dashboard", new { area = "Dashboard" });
                    }
                    else
                    {
                        TempData["Notification"] = "Invalid Credentials/Your Login is not approved yet.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Index");
        }

        public void InitializeContext(User user)
        {
            try
            {
                Session["UserId"] = user.Id;
                Session["FullName"] = user.FullName;
                Session["Email"] = user.EmailId;
                Session["RoleId"] = user.RoleId;
                Session["RoleName"] = user.RoleName;
                Session["Company"] = user.CompanyName.ToLower().Trim();
                //CurrentContext.SetCurrentContext(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Logout()
        {
            LogService.LogActivity("Logout", "User - " + Convert.ToString(Session["FullName"]) + " logged out successful..");
            Session.Clear();
            Session.Abandon();            
            return RedirectToAction("Index");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public JsonResult CheckforUserEmail(string email)
        {
            if (Utility.CheckEmailExistance(email))
            {
                return Json("Success",JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failue", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ResetPassword(string email,string password)
        {
            try
            {
                using(DbEntities context=new DbEntities())
                {
                    var users = from c in context.Users
                                where c.EmailId.ToLower().Trim() == email.ToLower().Trim()
                                where c.IsActive == true
                                where c.IsDeleted == false
                                select c;
                    if(users!=null && users.Count() > 0)
                    {
                        User user = users.FirstOrDefault();
                        user.Password = password;
                        context.Users.AddOrUpdate(user);
                        context.SaveChanges();
                    }
                } 
            }
            catch(Exception ex)
            {
                throw ex;
            }
            TempData["Notification"] = "Password Reset Successful";
            return RedirectToAction("Index");
        }
    }
}