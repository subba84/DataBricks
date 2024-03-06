using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb
{
    public class CurrentContext
    {
        private static CurrentUser m_context;
        public static CurrentUser User
        {
            get
            {
                if (m_context != null)
                {
                    return m_context;
                }
                else
                {
                    return new CurrentUser();
                }
            }
        }

        public static void SetCurrentContext(CurrentUser context)
        {
            m_context = context;
        }
    }
}