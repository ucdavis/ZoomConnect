using System;
using ZoomClient;

namespace ZoomConnect.Web.SetupRequirements.Banner
{
    public class ZoomConnectionRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private ZoomClient.Zoom _zoomClient;

        public ZoomConnectionRequirement(ZoomClient.Zoom zoomClient)
        {
            _zoomClient = zoomClient;
        }

        public RequirementType Type => RequirementType.Zoom;

        public string Capabilities => "Checks that your Zoom API credentials are working.";

        public string LongDescription => "If your Zoom API credentials are not working, please first log into your Zoom instance, " +
            "visit Zoom Marketplace at https://marketplace.zoom.us/, click Manage, click the name of your app, then click App Credentials. " +
            "Copy the API Key and API Secret into this app's settings.";

        public EnforcementType Enforcement => EnforcementType.Required;

        public int Priority => 2;

        public RequirementStatus Status => _status;

        public string StatusDescription => _statusDescription;

        public bool Evaluate()
        {
            var user = _zoomClient.GetUser("me");

            bool resultOk = user != null;
            _status = resultOk ? RequirementStatus.Completed : RequirementStatus.Missing;
            _statusDescription = resultOk ? "" : "Test connection failed, please check your Zoom API credentials.";

            return resultOk;
        }
    }
}
