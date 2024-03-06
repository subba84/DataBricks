using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb
{
    public static class DatabricksExtensions
    {
        public static string ToAppFormat(this DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy");
        }

        public static string ToAppFormat(this DateTime? dt)
        {
            if (dt != null)
                return dt.Value.ToString("dd-MM-yyyy");
            else
                return string.Empty;
        }

        public static string ToAppDateTimeFormat(this DateTime? dt)
        {
            if (dt != null)
                return dt.Value.ToString("dd-MM-yyyy hh:mm");
            else
                return string.Empty;
        }

        public static string ToSqlFormat(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }
    }
}