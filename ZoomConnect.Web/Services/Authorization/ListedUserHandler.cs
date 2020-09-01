using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZoomConnect.Web.Services.Authorization
{
    public class ListedUserHandler : AuthorizationHandler<ListedUserRequirement>
    {
        IConfiguration _config;

        public ListedUserHandler(IConfiguration configuration)
        {
            _config = configuration;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ListedUserRequirement requirement)
        {
            // we need a requirement with a valid configuration list of users to proceed
            if (requirement == null || String.IsNullOrWhiteSpace(requirement.ConfigLists))
            {
                return Task.CompletedTask;
            }

            var authorizedUsers = new List<string>();

            var listNames = requirement.ConfigLists.Split(",");

            foreach (var listName in listNames)
            {
                var listUsers = _config[listName];
                if (listUsers != null)
                {
                    authorizedUsers.AddRange(listUsers.Split(",").ToList());
                }
            }

            if (authorizedUsers.Contains(context.User.Identity.Name, StringComparer.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
