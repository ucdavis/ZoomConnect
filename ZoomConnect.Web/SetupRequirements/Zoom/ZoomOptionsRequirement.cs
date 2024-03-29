﻿using SecretJsonConfig;
using System;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.SetupRequirements.Banner
{
    public class ZoomOptionsRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private ZoomOptions _options;

        public ZoomOptionsRequirement(SecretConfigManager<ZoomOptions> optionsManager)
        {
            _options = optionsManager.GetValue().Result;
        }

        public RequirementType Type => RequirementType.Zoom;

        public string Capabilities => "Needed for connecting to Zoom API to read and write Zoom Meetings and related data.";

        public string LongDescription => "To connect to the Zoom API you will need to create or have access to " +
            "an OAuth2 Server to Server app in the Zoom Marketplace.  Creating one requires account-level " +
            "Developer Role Permission.  Once you have an OAuth2 Server to Server app, return here to tell us the " +
            "Account Id, Client Key and Client Secret so we can make calls for you. " +
            "See https://marketplace.zoom.us/docs/guides/build/server-to-server-oauth-app/ for more info.";

        public EnforcementType Enforcement => EnforcementType.Required;

        public int Priority => 1;

        public RequirementStatus Status => _status;

        public string StatusDescription => _statusDescription;

        public bool Evaluate()
        {
            if (_options == null || _options.ZoomApi == null)
            {
                _status = RequirementStatus.Missing;
                _statusDescription = "Zoom API options are not complete.";

                return false;
            }

            if (String.IsNullOrEmpty(_options.ZoomApi.AccountId.Value) ||
                String.IsNullOrEmpty(_options.ZoomApi.ClientId.Value) ||
                String.IsNullOrEmpty(_options.ZoomApi.ClientSecret.Value))
            {
                _status = RequirementStatus.Missing;
                _statusDescription = "Zoom API options are not filled out.";

                return false;
            }

            _status = RequirementStatus.Completed;
            _statusDescription = "";

            return true;
        }
    }
}
