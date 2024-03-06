using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb
{
    public class Utility
    {
        public static bool CheckEmailExistance(string emailid)
        {
            bool isExists = false;
            try
            {
                using(DbEntities context=new DbEntities())
                {
                    var user = from u in context.Users
                               where u.EmailId.ToLower().Trim() == emailid.ToLower().Trim()
                               where u.IsActive == true
                               where u.IsDeleted == false
                               select u;
                    if(user!=null && user.Count() > 0)
                    {
                        isExists = true;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return isExists;
        }
    }
}