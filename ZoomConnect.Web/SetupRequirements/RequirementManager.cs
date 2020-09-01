using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoomConnect.Web.SetupRequirements
{
    public class RequirementManager
    {
        private IEnumerable<ISetupRequirement> _requirements;

        public RequirementManager(IEnumerable<ISetupRequirement> requirements)
        {
            _requirements = requirements;
        }

        public bool CheckAllRequirements()
        {
            return CheckAllRequirements(_requirements);
        }

        public bool CheckAllRequirements(RequirementType type)
        {
            var filteredRequirements = _requirements.Where(r => r.Type == type);

            return CheckAllRequirements(filteredRequirements);
        }

        public IEnumerable<ISetupRequirement> MissingRequirements()
        {
            return _requirements.Where(r => r.Status == RequirementStatus.Missing);
        }

        private bool CheckAllRequirements(IEnumerable<ISetupRequirement> requirements)
        {
            var success = true;

            // group and sort requirements by priority
            var groupedRequirements = requirements
                .GroupBy(r => r.Priority, r => r)
                .OrderBy(p => p.Key)
                .ToList();

            foreach (var priority in groupedRequirements)
            {
                foreach (var requirement in priority)
                {
                    success &= requirement.Evaluate();
                }

                // short-circuit on failure so we don't run lower priority requirements
                if (!success)
                {
                    break;
                }
            }

            return success;
        }
    }
}
