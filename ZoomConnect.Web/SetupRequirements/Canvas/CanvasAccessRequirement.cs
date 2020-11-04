using SecretJsonConfig;
using System;
using CanvasClient;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Services.Canvas;

namespace ZoomConnect.Web.SetupRequirements.Canvas
{
    public class CanvasAccessRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private ZoomOptions _options;
        private CanvasApi _canvasApi;

        public CanvasAccessRequirement(SecretConfigManager<ZoomOptions> optionsManager, CanvasApi canvasApi)
        {
            _options = optionsManager.GetValue().Result;
            _canvasApi = canvasApi;

            _canvasApi.Options = _options.CanvasApi.CreateCanvasOptions();
        }

        public RequirementType Type => RequirementType.Canvas;

        public string Capabilities => "Checking to make sure your Access Token is working.";

        public string LongDescription => "Checks if your Access Token is expired or otherwise not working. " +
            "If expiration is indicated, you can refresh it in Canvas where you created it initially, at " +
            "Account : Profile : Settings : Approved Integrations. " +
            "Copy the new Access Token into app settings and refresh.";

        public EnforcementType Enforcement => EnforcementType.Optional;

        public int Priority => 2;

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

            // try the access token on a simple call and see if it works.
            return SetStatusAndReturn(_canvasApi.TokenErrorCheck());
        }
    }
}
