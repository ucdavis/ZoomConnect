using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.SetupRequirements.Zoom
{
    /// <summary>
    /// Checks for a download directory for Zoom cloud recordings to be written to.
    /// Download is optional so this requirement passes if no directory is present in options.
    /// Requirement fails if a directory option is present but the directory does not exist.
    /// </summary>
    public class DownloadDirectoryRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private ZoomOptions _options;

        public DownloadDirectoryRequirement(SecretConfigManager<ZoomOptions> optionsManager)
        {
            _options = optionsManager.GetValue().Result;
        }

        public RequirementType Type => RequirementType.Zoom;

        public string Capabilities => "Output directory for Zoom recordings downloaded from Cloud.";

        public string LongDescription => "Web App identity will need write access to this directory.";

        public EnforcementType Enforcement => EnforcementType.Optional;

        public int Priority => 3;

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
            // pass if options is missing
            if (_options == null || String.IsNullOrWhiteSpace(_options.DownloadDirectory))
            {
                return SetStatusAndReturn("");
            }

            // fail if directory does not exist
            if (!Directory.Exists(_options.DownloadDirectory))
            {
                return SetStatusAndReturn("Download Directory does not exist");
            }

            //// fail if no read/write rights to directory
            //var ac = new DirectoryInfo(_options.DownloadDirectory)
            //    .GetAccessControl();

            return SetStatusAndReturn("");
        }
    }
}
