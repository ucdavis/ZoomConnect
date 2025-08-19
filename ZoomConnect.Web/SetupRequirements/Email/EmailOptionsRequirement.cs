using SecretJsonConfig;
using System;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.SetupRequirements.Email
{
    public class EmailOptionsRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private ZoomOptions _options;

        public EmailOptionsRequirement(SecretConfigManager<ZoomOptions> optionsManager)
        {
            _options = optionsManager.GetValue().Result;
        }

        public RequirementType Type => RequirementType.Email;

        public string Capabilities => "Needed for connecting to an email server to send notifications and reports.";

        public string LongDescription => "This would typically be smtp.office365.com with campus AD3 account credentials, " +
            "or potentially unauthenticated.";

        public EnforcementType Enforcement => EnforcementType.Required;

        public int Priority => 1;

        public RequirementStatus Status => _status;

        public string StatusDescription => _statusDescription;

        public bool Evaluate()
        {
            if (_options == null || _options.EmailOptions == null)
            {
                _status = RequirementStatus.Missing;
                _statusDescription = "Email options are not complete.";

                return false;
            }

            if (String.IsNullOrEmpty(_options.EmailOptions.SmtpHost) ||
                String.IsNullOrEmpty(_options.EmailOptions.Username))
            {
                _status = RequirementStatus.Missing;
                _statusDescription = "Email server and/or username are not filled out.";

                return false;
            }

            _status = RequirementStatus.Completed;
            _statusDescription = "";

            return true;
        }
    }
}
