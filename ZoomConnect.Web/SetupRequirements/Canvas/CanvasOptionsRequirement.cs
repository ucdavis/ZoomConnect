using SecretJsonConfig;
using System;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.SetupRequirements.Canvas
{
    public class CanvasOptionsRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private ZoomOptions _options;

        public CanvasOptionsRequirement(SecretConfigManager<ZoomOptions> optionsManager)
        {
            _options = optionsManager.GetValue().Result;
        }

        public RequirementType Type => RequirementType.Canvas;

        public string Capabilities => "Needed for connecting to Canvas API to retrieve course data and update calendars.";

        public string LongDescription => "You will need an admin-level Canvas account to exercise the API over all your courses. " +
            "You will need to generate an Access Token in Canvas to paste into Setup options for this app. " +
            "In Canvas, go to Account : Profile : Settings : Approved Integrations and create a New Access Token. " +
            "This app does not perform full OAuth2 Flow so just the Access Token is needed, and it may need to be regenerated occasionally. " +
            "For more info see the Canvas API docs at https://canvas.instructure.com/doc/api/file.oauth.html#storing-access-tokens.";

        public EnforcementType Enforcement => EnforcementType.Optional;

        public int Priority => 1;

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
            // if Canvas is unused just indicate success without checking further
            if (_options == null || _options.CanvasApi == null || !_options.CanvasApi.UseCanvas)
            {
                return SetStatusAndReturn("");
            }

            // only invalid if no Token is provided
            if (String.IsNullOrEmpty(_options.CanvasApi.ApiAccessToken.Value))
            {
                return SetStatusAndReturn("Canvas Access Token is required.");
            }

            return SetStatusAndReturn("");
        }
    }
}
