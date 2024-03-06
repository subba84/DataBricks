using DataBricksWeb.Areas.UploadDownload.Model;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.UploadDownload.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class DocumentController : Controller
    {
        [HttpGet]
        public ActionResult List()
        {
            var model = new DocumentModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DocumentModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(DocumentModel model)
        {
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var uploadsModel = from u in context.Documents
                                       where u.IsActive == true
                                       where u.IsDeleted == false
                                       orderby u.Version descending
                                       select u;
                    if (uploadsModel != null && uploadsModel.Count() > 0)
                    {
                        model.Documents = uploadsModel.ToList();
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
            try
            {               
                using (DbEntities context = new DbEntities())
                {
                    var documentModel = from u in context.Documents
                                       where u.Id == id
                                       select u;
                    if (documentModel != null && documentModel.Count() > 0)
                    {
                        return View(documentModel.FirstOrDefault());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(new Document());
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Document model, HttpPostedFileBase doc)
        {
            try
            {
                LogService.LogActivity("Document Upload", "Document Details - " + (doc != null ? doc.FileName : "") + " uploaded successful..");
                using (DbEntities context = new DbEntities())
                {
                    model.IsDeleted = model.IsActive == true ? false : true;
                    model.DocumentName = doc != null ? doc.FileName : string.Empty;
                    if (doc != null)
                    {
                        model.DocumentType = doc.ContentType;
                    }
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
                    context.Documents.AddOrUpdate(model);
                    await context.SaveChangesAsync();
                    if (doc != null)
                    {
                        Database.Attachment attachment = new Database.Attachment();
                        attachment.FileName = doc.FileName;
                        attachment.FileType = doc.ContentType;
                        attachment.RelativePath = "Uploads/" + FileCategory.Document + "/" + model.Id + "/" + attachment.FileName;
                        string directorypath = System.IO.Path.Combine(DataCache.MapPath, "Uploads/" + FileCategory.Document + "/" + model.Id);
                        if (!System.IO.Directory.Exists(directorypath))
                        {
                            System.IO.Directory.CreateDirectory(directorypath);
                        }
                        string filepath = System.IO.Path.Combine(directorypath, attachment.FileName);
                        doc.SaveAs(filepath);
                        attachment.ObjectId = model.Id;
                        attachment.FileCategory = FileCategory.Document;
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
                    TempData["Notification"] = "File upaloded successfully";
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