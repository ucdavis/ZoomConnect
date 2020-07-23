using SecretJsonConfig;
using System;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.SetupRequirements.Banner
{
    public class BannerOptionsRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private ZoomOptions _options;

        public BannerOptionsRequirement(SecretConfigManager<ZoomOptions> optionsManager)
        {
            _options = optionsManager.GetValue().Result;
        }

        public RequirementType Type => RequirementType.Banner;

        public string Capabilities => "Needed for connecting to Banner database to retrieve courses and related data.";

        public string LongDescription => "You will need a Banner application account that can access the database directly." +
            "Tables you will need read access to include SCBCRSE (course titles), SIRASGN (instructor assignment), " +
            "SOBCALD (holidays), SPRIDEN (people), SSBSECT (course sections), SSRMEET (course meetings), STVTERM (terms), " +
            "STVSUBJ (course subjects), GOREMAL (email).  For access see https://kb.ucdavis.edu/?id=0640";

        public EnforcementType Enforcement => EnforcementType.Required;

        public int Priority => 1;

        public RequirementStatus Status => _status;

        public string StatusDescription => _statusDescription;

        public bool Evaluate()
        {
            if (_options == null || _options.Banner == null)
            {
                _status = RequirementStatus.Missing;
                _statusDescription = "Banner options are not complete.";

                return false;
            }

            if (String.IsNullOrEmpty(_options.Banner.Instance) ||
                String.IsNullOrEmpty(_options.Banner.Username) ||
                String.IsNullOrEmpty(_options.Banner.Password))
            {
                _status = RequirementStatus.Missing;
                _statusDescription = "Banner connection options are not filled out.";

                return false;
            }

            _status = RequirementStatus.Completed;
            _statusDescription = "";

            return true;
        }
    }
}
