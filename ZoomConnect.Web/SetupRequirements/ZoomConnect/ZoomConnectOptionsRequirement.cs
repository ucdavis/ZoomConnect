using SecretJsonConfig;
using System;
using System.Linq;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;

namespace ZoomConnect.Web.SetupRequirements.Banner
{
    /// <summary>
    /// Checks that all required ZoomConnect options are selected.  Runs after all Banner rules.
    /// </summary>
    public class ZoomConnectOptionsRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private ZoomOptions _options;
        private CachedRepository<stvterm> _bannerTerms;

        public ZoomConnectOptionsRequirement(SecretConfigManager<ZoomOptions> optionsManager, CachedRepository<stvterm> bannerTerms)
        {
            _options = optionsManager.GetValue().Result;
            _bannerTerms = bannerTerms;
        }

        public RequirementType Type => RequirementType.Zoom;

        public string Capabilities => "Checks that a valid Term and Subject are selected.";

        public string LongDescription => "Term and Subject are required to filter Banner data down " +
            "to a limited number of courses.";

        public EnforcementType Enforcement => EnforcementType.Required;

        public int Priority => 4;

        public RequirementStatus Status => _status;

        public string StatusDescription => _statusDescription;

        /// <summary>
        /// Sets status and description with passed in ErrorMessage.
        /// Returns success if ErrorMessage is null or empty.
        /// </summary>
        /// <param name="ErrorDescription">Error message to set in StatusDescription and set Status.</param>
        /// <returns>True if ErrorDescription is null or empty, otherwise failure.</returns>
        private bool SetStatusAndReturn(string ErrorDescription)
        {
            var success = String.IsNullOrEmpty(ErrorDescription);

            _status = success ? RequirementStatus.Completed : RequirementStatus.Missing;
            _statusDescription = ErrorDescription ?? "";

            return success;
        }

        public bool Evaluate()
        {
            if (_options == null || String.IsNullOrEmpty(_options.CurrentTerm) || String.IsNullOrEmpty(_options.CurrentSubject))
            {
                return SetStatusAndReturn("Please select both a Term and Subject.");
            }

            if (!_bannerTerms.GetAll().Any(t => t.code == _options.CurrentTerm))
            {
                return SetStatusAndReturn($"Selected Term '{_options.CurrentTerm}' not found in Banner.");
            }

            return SetStatusAndReturn("");
        }
    }
}
