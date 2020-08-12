using System;
using ZoomConnect.Web.Services;

namespace ZoomConnect.Web.SetupRequirements.Email
{
    public class EmailConnectionRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private EmailService _emailService;

        public EmailConnectionRequirement(EmailService emailService)
        {
            _emailService = emailService;
        }

        public RequirementType Type => RequirementType.Email;

        public string Capabilities => "Checks that your Email credentials are working.";

        public string LongDescription => "Connection defaults to port 587.";

        public EnforcementType Enforcement => EnforcementType.Required;

        public int Priority => 2;

        public RequirementStatus Status => _status;

        public string StatusDescription => _statusDescription;

        public bool Evaluate()
        {
            bool testResult = _emailService.Test();

            _status = testResult ? RequirementStatus.Completed : RequirementStatus.Missing;
            _statusDescription = testResult ? "" : "Test connection failed, please check your smtp host and credentials.";

            return testResult;
        }
    }
}
