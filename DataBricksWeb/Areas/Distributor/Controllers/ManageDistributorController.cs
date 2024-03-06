using DataBricksWeb.Areas.Distributor.Model;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.Distributor.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class ManageDistributorController : Controller
    {      
        [HttpGet]
        public ActionResult List()
        {
            var model = new DistributorModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DistributorModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(DistributorModel model)
        {
            try
            {
                using(DbEntities context=new DbEntities())
                {
                    var distributors = from d in context.Users
                                       where d.IsActive == true
                                       where d.IsDeleted == false
                                       where d.RoleId == UserRoleIds.Distributor
                                       orderby d.FullName
                                       select d;
                    if(distributors!=null && distributors.Count() > 0)
                    {
                        model.Distributors = distributors.ToList();
                    }

                    var distributorpartners = from d in context.DistributorPartners
                                       where d.IsActive == true
                                       where d.IsDeleted == false
                                       orderby d.PartnerName
                                       select d;
                    if (distributorpartners != null && distributorpartners.Count() > 0)
                    {
                        model.DistributorPartners = distributorpartners.ToList();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            Database.User user = new User();
            if (id != null && id > 0)
            {
                try
                {
                    using (DbEntities context = new DbEntities())
                    {
                        user = context.Users.Single(x => x.Id == id);
                        var distributorPartners = from dp in context.DistributorPartners
                                                  where dp.IsActive == true
                                                  where dp.IsDeleted == false
                                                  where dp.DistributorId == user.Id
                                                  select dp;
                        if (distributorPartners != null && distributorPartners.Count() > 0)
                        {
                            TempData["AssociatedPartners"] = distributorPartners.ToList();
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {

            }
            
           
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Database.User model,string partnersstring)
        {
            try
            {
                if (Utility.CheckEmailExistance(model.EmailId))
                {
                    TempData["Notification"] = "Email already existed in the system.";
                    return RedirectToAction("List");
                }
                List<DistributorPartner> partners = new List<DistributorPartner>();
                string[] partnersArray = partnersstring.Split(',');
                for (int i = 0; i < partnersArray.Length; i++)
                {
                    DistributorPartner distributorPartner = new DistributorPartner();
                    distributorPartner.PartnerId = Convert.ToInt32(partnersArray[i]);
                    partners.Add(distributorPartner);
                }
                using (DbEntities context=new DbEntities())
                {
                    model.RoleId = UserRoleIds.Distributor;
                    model.RoleName = "Distributor";
                    model.IsActive = true;
                    model.IsDeleted = false;
                    model.IsApproved = true;
                    model.CompanyName = "ABC";// Default Company Name
                    if (model.Id > 0)
                    {
                        model.ChangedBy = Convert.ToInt32(Session["UserId"]);
                        model.ChangedOn = DateTime.Now;
                        context.Users.AddOrUpdate(model);
                    }
                    else
                    {
                        model.CreatedBy = Convert.ToInt32(Session["UserId"]);
                        model.CreatedOn = DateTime.Now;
                        context.Users.Add(model);
                    }
                    await context.SaveChangesAsync();

                    // Delete already exists associated partners
                    context.Database.ExecuteSqlCommand("Delete from DistributorPartner where DistributorId=" + model.Id);
                    context.SaveChanges();
                    if (partners!=null && partners.Count > 0)
                    {
                        foreach(var partner in partners)
                        {
                            partner.DistributorId = model.Id;
                            partner.IsActive = true;
                            partner.IsDeleted = false;
                            partner.DistributorName = model.FullName;
                            partner.PartnerName = DataCache.Partners.Single(x => x.Id == partner.PartnerId).CompanyName;
                            if (partner.Id > 0)
                            {
                                partner.ChangedBy = Convert.ToInt32(Session["UserId"]);
                                partner.ChangedOn = DateTime.Now;
                                context.DistributorPartners.AddOrUpdate(partner);
                                await context.SaveChangesAsync();
                            }
                            else
                            {
                                partner.CreatedBy = Convert.ToInt32(Session["UserId"]);
                                partner.CreatedOn = DateTime.Now;
                                context.DistributorPartners.Add(partner);
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            new Thread(() => DataCache.RefreshPartner()).Start();
            TempData["Notification"] = "DIstributor Details Saved Successful";
            return RedirectToAction("List");
        }

        
        public ActionResult Delete(int? id)
        {
            try
            {
                using(DbEntities context=new DbEntities())
                {
                    var distributor = context.Users.Single(x => x.Id == id);
                    distributor.IsActive = false;
                    distributor.IsDeleted = true;
                    context.Users.AddOrUpdate(distributor);
                    context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            TempData["Notification"] = "DIstributor Deleted Successfully";
            return RedirectToAction("List");
        }
    }
}