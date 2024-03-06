
using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataBricksWeb.Controllers
{
    public class PoApiController : ApiController
    {
        // GET: api/PoApi
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
                                module.ModuleName = item.ModuleName;
                                module.ModuleCount = item.ModuleCount;
                                module.FromDate = EncryptDecrypt.Encrypt(Convert.ToString(item.FromDate), publicKey, secretkey);
                                module.ToDate = EncryptDecrypt.Encrypt(Convert.ToString(item.ToDate), publicKey, secretkey);
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

        public class WebApiModel
        {
            public string ModuleName { get; set; }
            public int? ModuleCount { get; set; }
            public string LicenseStatus { get; set; }
            public string FromDate { get; set; }
            public string ToDate { get; set; }
        }
    }
}
