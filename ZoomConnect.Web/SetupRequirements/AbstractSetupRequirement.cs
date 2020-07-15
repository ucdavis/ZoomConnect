using System;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.SetupRequirements
{
    /// <inheritdoc />
    public class AbstractSetupRequirement : ISetupRequirement
    {
        private SecretConfigurationManager<ZoomOptions> _zoomOptions;

        public AbstractSetupRequirement(SecretConfigurationManager<ZoomOptions> zoomOptions)
        {
            _zoomOptions = zoomOptions;
        }

        public string Capabilities { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string LongDescription { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public EnforcementType Enforcement { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public RequirementStatus Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string StatusDescription { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool Evaluate() => throw new NotImplementedException();
    }
}
