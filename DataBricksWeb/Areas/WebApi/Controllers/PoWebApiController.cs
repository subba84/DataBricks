using DataBricksWeb.Areas.WebApi.Data;
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataBricksWeb.Areas.WebApi.Controllers
{
    public class PoWebApiController : ApiController
    {
        [HttpGet]
        [Route("api/powebapi/get")]
        public HttpResponseMessage Get()
        {
            try
            {
                string domainName = Request.Headers.GetValues("DomainName").FirstOrDefault();
                string publicip = Request.Headers.GetValues("PublicIp").FirstOrDefault();
                string publicKey = Convert.ToString(ConfigurationManager.AppSettings["PublicKey"]);
                string secretkey = Convert.ToString(ConfigurationManager.AppSettings["SecretKey"]);
                var Licensings = new List<WebApiModel>();
                using (DbEntities context = new DbEntities())
                {
                    var podetails = from p in context.Poes
                                    where p.QualifiedDomainName == domainName
                                    where p.Status == Statusses.Approved
                                    where p.PublicIp == publicip
                                    where p.IsActive == true
                                    where p.IsDeleted == false
                                    select p;
                    if (podetails != null && podetails.Count() > 0)
                    {
                        var po = podetails.FirstOrDefault();
                        var pomodules = from m in context.Licensings
                                        where m.CustomerId == po.CustomerId
                                        where m.IsActive == true
                                        where m.IsDeleted == false
                                        select m;
                        if (pomodules != null && pomodules.Count() > 0)
                        {
                            
                            foreach (var item in pomodules.ToList())
                            {
                                WebApiModel module = new WebApiModel();
                                module.ModuleName = EncryptDecrypt.Encrypt(Convert.ToString(item.ModuleName), secretkey);
                                module.ModuleCount = EncryptDecrypt.Encrypt(Convert.ToString(item.ModuleCount), secretkey);
                                module.FromDate = EncryptDecrypt.Encrypt(Convert.ToString(item.FromDate), secretkey);
                                module.ToDate = EncryptDecrypt.Encrypt(Convert.ToString(item.ToDate), secretkey);
                                if (item.ToDate.Value.Date < DateTime.Now.Date)
                                {
                                    module.LicenseStatus = "Expired";
                                }
                                else
                                {
                                    module.LicenseStatus = "Active";
                                }
                                Licensings.Add(module);
                            }
                        }

                    }
                }
                if (Licensings != null && Licensings.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Licensings);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("api/powebapi/savecustomeractivationlog")]
        public HttpResponseMessage SaveCustomerActivationLog(string email)
        {
            try
            {
                var customer = DataCache.Customers.Where(x => x.EmailId.ToLower() == email.ToLower()).FirstOrDefault();
                DataBricksWeb.Database.DistributorPartner distributor = new DistributorPartner();
                using (DbEntities context = new DbEntities())
                {
                    if (customer != null)
                    {
                        var distributordetails = from d in context.DistributorPartners
                                                 where d.IsActive == true
                                                 where d.IsDeleted == false
                                                 where d.PartnerId == customer.PartnerId
                                                 select d;
                        if (distributordetails != null && distributordetails.Count() > 0)
                        {
                            distributor = distributordetails.FirstOrDefault();
                        }
                    }
                    CustomerActivationLog customerActivationLog = new CustomerActivationLog();
                    customerActivationLog.IsActive = true;
                    customerActivationLog.IsDeleted = false;
                    customerActivationLog.CreatedOn = DateTime.Now;
                    customerActivationLog.ActivationDate = DateTime.Now;
                    customerActivationLog.CustomerId = customer.Id;
                    customerActivationLog.CustomerName = customer.CustomerName;
                    customerActivationLog.PartnerId = customer.PartnerId;
                    customerActivationLog.PartnerName = customer.PartnerName;
                    customerActivationLog.DistributorId = distributor.DistributorId;
                    customerActivationLog.DistributorName = distributor.DistributorName;
                    context.CustomerActivationLogs.Add(customerActivationLog);
                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
