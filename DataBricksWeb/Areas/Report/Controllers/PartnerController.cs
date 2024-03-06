using DataBricksWeb.Areas.Partner.Model;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.Report.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class PartnerController : Controller
    {
        [HttpGet]
        public ActionResult List()
        {
            var model = new UserModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(UserModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(UserModel model)
        {
            try
            {
                int loggedInUser = Convert.ToInt32(Session["UserId"]);
                using (DbEntities context = new DbEntities())
                {
                    var users = from u in context.Users
                                where u.IsActive == true
                                where u.RoleId == UserRoleIds.Partner
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

        public ActionResult Details(int id)
        {
            User user = new Database.User();
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var users = from u in context.Users
                                where u.Id == id
                                select u;
                    if (users != null && users.Count() > 0)
                    {
                        user = users.FirstOrDefault();
                    }

                    var attachments = from a in context.Attachments
                                      where a.IsActive == true
                                      where a.IsDeleted == false
                                      where a.ObjectId == user.Id
                                      where a.FileCategory == FileCategory.BusinessCard
                                      select a;
                    if (attachments != null && attachments.Count() > 0)
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
    }
}