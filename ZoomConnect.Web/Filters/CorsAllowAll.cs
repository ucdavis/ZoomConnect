using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace ZoomConnect.Web.Filters
{
    public class CorsAllowAll : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var header = new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", new StringValues("*"));
            filterContext.HttpContext.Response.Headers.Append(header);
            base.OnActionExecuting(filterContext);
        }
    }
}
