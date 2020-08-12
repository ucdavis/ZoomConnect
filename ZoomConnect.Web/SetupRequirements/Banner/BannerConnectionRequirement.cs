using System;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;

namespace ZoomConnect.Web.SetupRequirements.Banner
{
    public class BannerConnectionRequirement : ISetupRequirement
    {
        private RequirementStatus _status = RequirementStatus.Unchecked;
        private string _statusDescription = "";
        private CachedRepository<dual> _testRepository;

        public BannerConnectionRequirement(CachedRepository<dual> testRepository)
        {
            _testRepository = testRepository;
        }

        public RequirementType Type => RequirementType.Banner;

        public string Capabilities => "Checks that your Banner credentials are working.";

        public string LongDescription => "You must specify the instance in TNSNAMES format since LDAP connection is not working currently. " +
            "For Banner Prod, use '(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(Host=tyche.ucdavis.edu)(Port=1525))(CONNECT_DATA=(SID=prod)))'. " +
            "Check Oracle LDAP or your TNSNAMES file for other instances if you want test data.";

        public EnforcementType Enforcement => EnforcementType.Required;

        public int Priority => 2;

        public RequirementStatus Status => _status;

        public string StatusDescription => _statusDescription;

        public bool Evaluate()
        {
            bool testResult = _testRepository.TestConnection();

            _status = testResult ? RequirementStatus.Completed : RequirementStatus.Missing;
            _statusDescription = testResult ? "" : "Test connection failed, please check your instance and credentials.";

            return testResult;
        }
    }
}
