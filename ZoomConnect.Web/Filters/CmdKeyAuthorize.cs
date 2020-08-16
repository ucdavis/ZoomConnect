using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SecretJsonConfig;
using System;
using System.Net;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.Filters
{
    public class CmdKeyAuthorize : IActionFilter
    {
        private ZoomOptions _options;

        public CmdKeyAuthorize(SecretConfigManager<ZoomOptions> optionsManager)
        {
            _options = optionsManager.GetValue().Result;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var cmdKeySubmitted = context.HttpContext.Request.Headers["X-Cmd-Key"];
            var cmdKeyCurrent = _options?.CmdKeyOptions?.CmdKey;

            if (cmdKeyCurrent == null ||
                String.IsNullOrEmpty(cmdKeyCurrent.Value) ||
                cmdKeySubmitted.Count != 1 ||
                cmdKeySubmitted.ToString() != cmdKeyCurrent)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
