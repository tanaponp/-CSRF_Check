using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using _CSRF_Library.Utilities;

namespace _CSRF_Check.Attributes
{
    public class CheckSecretKeyActionFilterAttribute: System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.Request.CheckSecretKey();
        }
    }
}