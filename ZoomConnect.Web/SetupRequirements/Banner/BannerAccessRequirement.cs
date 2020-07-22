using System;
using System.Collections.Generic;
using ZoomConnect.Web.Banner.Cache;

namespace ZoomConnect.Web.SetupRequirements.Banner
{
    public class BannerAccessRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private IEnumerable<ICachedRepository> _repositories;

        public BannerAccessRequirement(IEnumerable<ICachedRepository> repositories)
        {
            _repositories = repositories;
        }

        public RequirementType Type => RequirementType.Banner;

        public string Capabilities => $"Checks that you have access to required Banner tables.";

        public string LongDescription => "For access to Banner tables contact banneraccess@ucdavis.edu.";

        public EnforcementType Enforcement => EnforcementType.Required;

        public int Priority => 3;

        public RequirementStatus Status => _status;

        public string StatusDescription => _statusDescription;

        public bool Evaluate()
        {
            var missingTables = new List<string>();

            foreach (var repository in _repositories)
            {
                if (!repository.TestConnection())
                {
                    missingTables.AddRange(repository.Tables);
                }
            }

            var success = missingTables.Count == 0;
            _status = success ? RequirementStatus.Completed : RequirementStatus.Missing;
            _statusDescription = success
                ? "You have access to all required tables."
                : $"Access to the following tables is not working with your account: {String.Join(", ", missingTables)}";

            return success;
        }
    }
}
