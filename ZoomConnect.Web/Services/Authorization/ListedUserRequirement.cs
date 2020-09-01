using Microsoft.AspNetCore.Authorization;
using System;

namespace ZoomConnect.Web.Services.Authorization
{
    public class ListedUserRequirement : IAuthorizationRequirement
    {
        public string ConfigLists { get; private set; }

        /// <summary>
        /// Requires that the authenticated user is a member of a named list in the ZoomSecrets.json config file.
        /// </summary>
        /// <param name="configList">Comma-separated list of entry names in the configuration file.
        /// The associated values are comma-separated lists of usernames.</param>
        public ListedUserRequirement(string configLists)
        {
            ConfigLists = configLists;
        }
    }
}
