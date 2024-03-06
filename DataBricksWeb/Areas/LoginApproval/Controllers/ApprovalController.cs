using DataBricksWeb.Areas.LoginApproval.Model;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.LoginApproval.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class ApprovalController : Controller
    {
        [HttpGet]
        public ActionResult List()
        {
            var model = new ApprovalModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(ApprovalModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(ApprovalModel model)
        {
            try
            {
                int loggedInUser = Convert.ToInt32(Session["UserId"]);
                using (DbEntities context = new DbEntities())
                {
                    var users = from u in context.Users
                                where (u.IsApproved == false || u.IsApproved == null)
                                where u.IsActive == true
                                orderby u.FullName
                                select u;
                    if (users != null && users.Count() > 0)
                    {
                        model.Users = users.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            User user = new User();
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var userDetails = from u in context.Users
                                      where u.Id == id
                                      select u;
                    if (userDetails != null && userDetails.Count() > 0)
                    {
                        user = userDetails.FirstOrDefault();
                    }

                    var attachments = from a in context.Attachments
                                      where a.IsActive == true
                                      where a.IsDeleted == false
                                      where a.ObjectId == user.Id
                                      where a.FileCategory == FileCategory.BusinessCard
                                      select a;
                    if(attachments!=null && attachments.Count() > 0)
                    {
                        user.BusinessCard = attachments.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(User user)
        {
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var usr = context.Users.Single(x => x.Id == user.Id);
                    LogService.LogActivity("Login Approval", "User - " + usr.FullName + " login is "+(user.IsApproved == true ? "approved" : "rejected") +" out successful..");
                    usr.ApprovedBy = Convert.ToInt32(Session["UserId"]);
                    usr.ApprovedOn = DateTime.Now;
                    usr.IsApproved = user.IsApproved;
                    context.Users.AddOrUpdate(usr);
                    await context.SaveChangesAsync();
                    if(user.IsApproved == true)
                    {
                        TempData["Notification"] = "User Login Approved Successfully";
                    }
                    else
                    {
                        TempData["Notification"] = "User Login Rejected";
                    }
                    
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