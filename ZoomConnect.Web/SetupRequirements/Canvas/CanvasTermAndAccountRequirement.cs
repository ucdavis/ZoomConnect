using SecretJsonConfig;
using System;
using CanvasClient;
using ZoomConnect.Core.Config;
using System.Linq;
using CanvasClient.Extensions;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Services.Canvas;

namespace ZoomConnect.Web.SetupRequirements.Canvas
{
    /// <summary>
    /// Checks that a Canvas Term can be found matching the selected Banner Term.
    /// Also checks that the selected Canvas Account can be found.
    /// Runs after ZoomConnectOptionsRequirement.
    /// </summary>
    public class CanvasTermAndAccountRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private ZoomOptions _options;
        private CanvasApi _canvasApi;
        private CachedRepository<stvterm> _bannerTerms;

        public CanvasTermAndAccountRequirement(SecretConfigManager<ZoomOptions> optionsManager, CanvasApi canvasApi, CachedRepository<stvterm> bannerTerms)
        {
            _options = optionsManager.GetValue().Result;
            _canvasApi = canvasApi;
            _bannerTerms = bannerTerms;

            _canvasApi.Options = _options.CanvasApi.CreateCanvasOptions();
        }

        public RequirementType Type => RequirementType.Canvas;

        public string Capabilities => "Checks that selected Term is found in Canvas.";

        public string LongDescription => "Canvas term must match selected Banner Year and Term Description.";

        public EnforcementType Enforcement => EnforcementType.Required;

        public int Priority => 5;

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
            var bannerTermRow = _bannerTerms.GetAll().FirstOrDefault(t => t.code == _options.CurrentTerm);
            var canvasTerms = _canvasApi.ListEnrollmentTerms();

            if (!canvasTerms.Any(t => t.MatchesBannerTermDesc(bannerTermRow.description)))
            {
               return SetStatusAndReturn($"Canvas EnrollmentTerm could not be found matching '{bannerTermRow.description}'.");
            }

            var canvasAccounts = _canvasApi.ListAccounts();
            if (!canvasAccounts.Any(a => a.id == _options.CanvasApi.SelectedAccount))
            {
                return SetStatusAndReturn($"Selected Canvas Account could not be found.");
            }

            return SetStatusAndReturn("");
        }
    }
}
