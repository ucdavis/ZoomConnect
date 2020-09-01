using MediasiteUtil;
using SecretJsonConfig;
using System;
using System.Collections.Generic;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.SetupRequirements.Canvas
{
    //public class MediasiteAuthRequirement : ISetupRequirement
    //{
    //    private RequirementStatus _status = RequirementStatus.Unchecked;
    //    private string _statusDescription = "";
    //    private ZoomOptions _options;
    //    private MediasiteClient _mediasite;

    //    public MediasiteAuthRequirement(SecretConfigManager<ZoomOptions> optionsManager, MediasiteClient mediasiteClient)
    //    {
    //        _options = optionsManager.GetValue().Result;
    //        _mediasite = mediasiteClient;
    //    }

    //    public RequirementType Type => RequirementType.Mediasite;

    //    public string Capabilities => "Checking your authentication to Mediasite API.";

    //    public string LongDescription => "You will need a valid Endpoint, API Key, Username, and Password.";

    //    public EnforcementType Enforcement => EnforcementType.Optional;

    //    public int Priority => 2;

    //    public RequirementStatus Status => _status;

    //    public string StatusDescription => _statusDescription;

    //    /// <summary>
    //    /// Sets status and description with passed in ErrorMessage.
    //    /// Returns success if ErrorMessage is null or empty.
    //    /// </summary>
    //    /// <param name="ErrorDescription">Error message to set in StatusDescription and set Status.</param>
    //    /// <returns>True if ErrorDescription is null or empty, otherwise failure.</returns>
    //    private bool SetStatusAndReturn(string ErrorDescription)
    //    {
    //        var success = String.IsNullOrEmpty(ErrorDescription);

    //        _status = success ? RequirementStatus.Completed : RequirementStatus.Missing;
    //        _statusDescription = ErrorDescription ?? "";

    //        return success;
    //    }

    //    public bool Evaluate()
    //    {
    //        //_mediasite.

    //        return SetStatusAndReturn("");
    //    }
    //}
}
