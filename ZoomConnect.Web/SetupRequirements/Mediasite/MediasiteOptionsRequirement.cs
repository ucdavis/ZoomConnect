using SecretJsonConfig;
using System;
using System.Collections.Generic;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.SetupRequirements.Canvas
{
    public class MediasiteOptionsRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private ZoomOptions _options;

        public MediasiteOptionsRequirement(SecretConfigManager<ZoomOptions> optionsManager)
        {
            _options = optionsManager.GetValue().Result;
        }

        public RequirementType Type => RequirementType.Mediasite;

        public string Capabilities => "Needed for connecting to Mediasite API to publish presentations.";

        public string LongDescription => "You will need a Mediasite account with rights to use the API. " +
            "You will also need to generate an Access Token in Mediasite to paste into Setup options for this app. " +
            "For an API Key, visit http://{your server}/mediasite/api/Docs/ApiKeyRegistration.aspx. " +
            "API Documentation is at http://{your server}/mediasite/api/v1/$metadata.";

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
            // if Mediasite is unused just indicate success without checking further
            if (_options == null || _options.MediasiteOptions == null || !_options.MediasiteOptions.UseMediasite)
            {
                return SetStatusAndReturn("");
            }

            // build missing options list
            var missingOptions = new List<string>();
            if (String.IsNullOrEmpty(_options.MediasiteOptions.Endpoint))
            {
                missingOptions.Add("Endpoint");
            }
            if (String.IsNullOrEmpty(_options.MediasiteOptions.ApiKey))
            {
                missingOptions.Add("API Key");
            }
            if (String.IsNullOrEmpty(_options.MediasiteOptions.Username))
            {
                missingOptions.Add("Username");
            }
            if (String.IsNullOrEmpty(_options.MediasiteOptions.Password))
            {
                missingOptions.Add("Password");
            }

            // invalid if any missing options
            if (missingOptions.Count > 0)
            {
                return SetStatusAndReturn($"Missing required options: {String.Join(", ", missingOptions)}");
            }

            return SetStatusAndReturn("");
        }
    }
}
