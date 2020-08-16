using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ZoomConnect.Web.Filters
{
    public class CorsAllowAll : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            base.OnActionExecuting(filterContext);
        }
    }
}
