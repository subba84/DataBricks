using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataBricksWeb.Areas.Common.Controllers
{
    [CustomAuthorize]
    [SessionExpire]
    public class ManageCommonController : Controller
    {
        [HttpGet]
        public JsonResult GetLeadsBasedonCustomer(int customerid)
        {
            try
            {
                using (DbEntities context = new DbEntities())
                {
                    var leadDetails = from l in context.Leads
                                      where l.IsActive == true
                                      where l.IsDeleted == false
                                      where l.CustomerId == customerid
                                      select l;
                    if (leadDetails != null && leadDetails.Count() > 0)
                    {
                        return Json(leadDetails.ToList(), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetModulesbasedonProduct(int productid)
        {
            try
            {
                if (DataCache.Modules != null && DataCache.Modules.Count > 0)
                {
                    var productModules = from m in DataCache.Modules
                                         where m.IsActive == true
                                         where m.IsDeleted == false
                                         where m.ProductId == productid
                                         orderby m.ModuleName
                                         select m;
                    if (productModules != null && productModules.Count() > 0)
                    {
                        return Json(productModules.ToList(), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("");
        }

        [HttpGet]
        public JsonResult AddDaystoDate(string date)
        {
            DateTime dt = Convert.ToDateTime(date).AddYears(1);
            return Json(dt.ToString("dd-MM-yyyy"),JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadFile(int id,string category)
        {
            try
            {
                using(DbEntities context=new DbEntities())
                {
                    var fileDetails = from f in context.Attachments
                                      where f.ObjectId == id
                                      where f.FileCategory == category
                                      select f;
                    if(fileDetails!=null && fileDetails.Count() > 0)
                    {
                        LogService.LogActivity("Document Download", "Document Details - " + fileDetails.FirstOrDefault().FileName + " downloaded successful..");
                        //return File(System.IO.Path.Combine(DataCache.MapPath,fileDetails.FirstOrDefault().RelativePath), fileDetails.FirstOrDefault().FileType);
                        //Download(fileDetails.FirstOrDefault());
                        var file = fileDetails.FirstOrDefault();
                        
                        byte[] byteArray = System.IO.File.ReadAllBytes(DataCache.MapPath + file.RelativePath);
                        return File(byteArray, "application/pdf", file.FileName);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public void Download(Attachment model)
        {
            try
            {
                Response.ContentType = model.FileType;
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + model.FileName);
                Response.WriteFile(System.IO.Path.Combine(DataCache.MapPath,model.RelativePath));
                Response.End();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetCustomersbasedonPartner(int partnerid)
        {
            try
            {
                return Json(DataCache.Customers.Where(x=>x.PartnerId == partnerid).ToList(),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}