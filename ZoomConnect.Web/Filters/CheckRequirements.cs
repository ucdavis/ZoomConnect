using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using ZoomConnect.Web.SetupRequirements;

namespace ZoomConnect.Web.Filters
{
    public class CheckRequirements : IActionFilter
    {
        private RequirementManager _requirementManager;

        public CheckRequirements(RequirementManager requirementManager)
        {
            _requirementManager = requirementManager;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_requirementManager.CheckAllRequirements())
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "Setup" }
                });
            }
        }
    }
}
