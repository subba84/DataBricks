using DataBricksWeb.Areas.UploadDownload.Model;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.UploadDownload.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class ZipFileController : Controller
    {        
        [HttpGet]
        public ActionResult List()
        {
            var model = new ZipFileModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(ZipFileModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(ZipFileModel model)
        {
            try
            {
                using(DbEntities context=new DbEntities())
                {
                    var uploadsModel = from u in context.ZipUploads
                                       where u.IsActive == true
                                       where u.IsDeleted == false
                                       orderby u.VersionNumber descending
                                       select u;
                    if(uploadsModel!=null && uploadsModel.Count() > 0)
                    {
                        model.ZipUploads = uploadsModel.ToList();
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
            try
            {
                using(DbEntities context=new DbEntities())
                {
                    var zipFileModel = from u in context.ZipUploads
                                       where u.Id == id
                                       select u;
                    if(zipFileModel!=null && zipFileModel.Count() > 0)
                    {
                        return View(zipFileModel.FirstOrDefault());
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return View(new ZipUpload());
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ZipUpload model,HttpPostedFileBase zip)
        {
            try
            {
                LogService.LogActivity("Zip File Upload", "Document Details - " + (zip!=null ? zip.FileName : "") + " uploaded successful..");
                using (DbEntities context=new DbEntities())
                {
                    model.IsDeleted = model.IsActive == true ? false : true;
                    model.ZipFileName = zip != null ? zip.FileName : string.Empty;
                    if (model.Id > 0)
                    {
                        model.ChangedBy = Convert.ToInt32(Session["UserId"]);
                        model.ChangedOn = DateTime.Now;
                    }
                    else
                    {
                        model.CreatedBy = Convert.ToInt32(Session["UserID"]);
                        model.CreatedOn = DateTime.Now;
                    }
                    context.ZipUploads.AddOrUpdate(model);
                    await context.SaveChangesAsync();
                    if(zip!=null)
                    {

                        

                        Attachment attachment = new Attachment();
                        attachment.FileName = zip.FileName;
                        attachment.FileType = zip.ContentType;
                        attachment.RelativePath = "Uploads/" + FileCategory.ZipFile + "/" + model.Id + "/" + attachment.FileName;
                        string directorypath = System.IO.Path.Combine(DataCache.MapPath, "Uploads/" + FileCategory.ZipFile + "/" + model.Id);
                        if (!System.IO.Directory.Exists(directorypath))
                        {
                            System.IO.Directory.CreateDirectory(directorypath);
                        }
                        string filepath = System.IO.Path.Combine(directorypath, attachment.FileName);
                        zip.SaveAs(filepath);
                        attachment.ObjectId = model.Id;
                        attachment.FileCategory = FileCategory.ZipFile;
                        attachment.IsActive = true;
                        attachment.IsDeleted = false;

                        //using (ZipArchive archive = new ZipArchive(zip.InputStream))
                        //{
                        //    foreach (ZipArchiveEntry entry in archive.Entries)
                        //    {
                        //        if (!string.IsNullOrEmpty(System.IO.Path.GetExtension(filepath))) //make sure it's not a folder
                        //        {
                        //            entry.extra(filepath);
                        //        }
                        //        else
                        //        {
                        //            System.IO.Directory.CreateDirectory(filepath);
                        //        }
                        //    }
                        //}


                        context.Attachments.Add(attachment);
                        await context.SaveChangesAsync();
                    }
                    new Thread(() => DataCache.RefreshVersion()).Start();
                    TempData["Notification"] = "File upaloded successfully";
                }                 
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("List");
        }
    }
}