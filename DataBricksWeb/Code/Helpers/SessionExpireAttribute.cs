using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace DataBricksWeb
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        #region OnActionExecuting
        /// <summary>
        /// OnActionExecuting
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;

            // check  sessions here
            if (HttpContext.Current.Session["UserId"] == null)
            {
                filterContext.Result = new RedirectResult("~/login/index");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
        #endregion
    }
}