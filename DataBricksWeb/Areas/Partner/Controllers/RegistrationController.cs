using DataBricksWeb.Database;
using SRVTextToImage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.Partner.Controllers
{
    public class RegistrationController : Controller
    {
        public ActionResult Index()
        {
            CaptchaRandomImage CI = new CaptchaRandomImage();
            // GetRandomString function return random text based on the size you provided
            string captchaText = CI.GetRandomString(5);
            Session["CaptchaString"] = captchaText;
            return View();
        }

        public async Task<ActionResult> Edit(User user,HttpPostedFileBase flBusinessCard)
        {
            try
            {
                if (Utility.CheckEmailExistance(user.EmailId))
                {
                    TempData["Notification"] = "Email already existed in the system. Please try to login";
                    return RedirectToAction("Index", "Authenticate", new { area = "Login" });
                }
                LogService.LogActivity("User Save", "User - " + user.FullName + " registration successful..");
                using (DbEntities context=new DbEntities())
                {
                    user.IsActive = true;
                    user.IsDeleted = false;
                    user.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    user.CreatedOn = DateTime.Now;
                    user.RoleId = UserRoleIds.Partner;
                    user.RoleName = DataCache.Roles.Single(x => x.Id == user.RoleId).RoleName;
                    user.CountryName = DataCache.Countries.Single(x => x.Id == user.CountryId).CountryName;
                    user.CityName = DataCache.Cities.Single(x => x.Id == user.CityId).CityName;
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                    if (flBusinessCard != null)
                    {
                        Attachment attachment = new Attachment();
                        attachment.ObjectId = user.Id;
                        attachment.IsActive = true;
                        attachment.IsDeleted = false;
                        attachment.CreatedBy = Convert.ToInt32(Session["UserId"]);
                        attachment.CreatedOn = DateTime.Now;
                        attachment.FileName = flBusinessCard.FileName;
                        attachment.FileType = flBusinessCard.ContentType;
                        attachment.FileCategory = FileCategory.BusinessCard;
                        string directoryPath = DataCache.MapPath + "Uploads/BusinessCards/" + user.Id;
                        if(!System.IO.Directory.Exists(directoryPath))
                        {
                            System.IO.Directory.CreateDirectory(directoryPath);
                        }
                        attachment.RelativePath = "Uploads/BusinessCards/" +user.Id + "/" + flBusinessCard.FileName;
                        string path = DataCache.MapPath + attachment.RelativePath;
                        if (!System.IO.File.Exists(path)) 
                        {
                            flBusinessCard.SaveAs(path);
                        }
                        context.Attachments.Add(attachment);
                        await context.SaveChangesAsync();
                    }
                    TempData["Notification"] = "Registration Successful, You can only login after Admin approval.";
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Index","Authenticate", new { area = "Login" });
        }

        public JsonResult GetCitiesbasedonCountryId(int countryId)
        {
            try
            {
                if(DataCache.Cities!=null && DataCache.Cities.Count > 0)
                {
                    var cities = DataCache.Cities.Where(x => x.CountryId == countryId);
                    if(cities!=null && cities.Count() > 0)
                    {
                        return Json(cities,JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public FileResult GetCaptchaImage()
        {
            try
            {
                CaptchaRandomImage CI = new CaptchaRandomImage();
                string captchaText = Convert.ToString(Session["CaptchaString"]);
                //Prepare Image
                CI.GenerateImage(captchaText, 120, 50, Color.White, Color.DarkGray);
                MemoryStream ms = new MemoryStream();
                CI.Image.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(ms, "image/png");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        
    }
}