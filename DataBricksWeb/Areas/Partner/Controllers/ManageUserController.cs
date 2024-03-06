using DataBricksWeb.Areas.Partner.Model;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.Partner.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class ManageUserController : Controller
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
                using(DbEntities context =new DbEntities())
                {
                    var users = from u in context.Users
                                where u.CreatedBy == loggedInUser
                                where u.IsActive == true
                                orderby u.FullName
                                select u;
                    if(users!=null && users.Count() > 0)
                    {
                        model.Users = users.ToList();
                    }
                }
            }
            catch(Exception ex)
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
                
                using (DbEntities context=new DbEntities())
                {
                    var userDetails = from u in context.Users
                                      where u.Id == id
                                      select u;
                    if(userDetails!=null && userDetails.Count() > 0)
                    {
                        user = userDetails.FirstOrDefault();
                    }
                    else
                    {
                        int partnerId = Convert.ToInt32(Session["UserId"]);
                        userDetails = from u in context.Users
                                      where u.Id == partnerId
                                      select u;
                        if (userDetails != null && userDetails.Count() > 0)
                        {
                            user = userDetails.FirstOrDefault();
                            // Make nulls of existing customer values to new user
                            user.FullName = string.Empty;
                            user.PhoneNumber = string.Empty;
                            user.EmailId = string.Empty;
                            user.Password = string.Empty;
                            user.Id = 0;
                        }
                    }
                }
            }
            catch(Exception ex)
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
                if (Utility.CheckEmailExistance(user.EmailId))
                {
                    TempData["Notification"] = "Email already existed in the system.";
                    return RedirectToAction("List");
                }
                user.RoleName = DataCache.Roles.Single(x => x.Id == user.RoleId).RoleName;
                LogService.LogActivity("User Save", "User - " + user.FullName + " with role " + user.RoleName + " added successful..");
                using (DbEntities context = new DbEntities())
                {
                    user.IsActive = true;
                    user.IsDeleted = false;
                    user.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    user.CreatedOn = DateTime.Now;
                    user.IsApproved = true;
                    user.ApprovedBy = Convert.ToInt32(Session["UserId"]);
                    user.ApprovedOn = DateTime.Now;
                    
                    user.CountryName = DataCache.Countries.Single(x => x.Id == user.CountryId).CountryName;
                    user.CityName = DataCache.Cities.Single(x => x.Id == user.CityId).CityName;
                    context.Users.AddOrUpdate(user);
                    await context.SaveChangesAsync();                    
                    TempData["Notification"] = "User Details Saved Successfully";
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