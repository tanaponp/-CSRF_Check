﻿using System.Web;
using System.Web.Mvc;

namespace _CSRF_Check
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
