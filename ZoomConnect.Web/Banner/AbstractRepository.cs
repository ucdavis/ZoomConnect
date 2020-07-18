using SecretJsonConfig;
using System;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Banner
{
    public abstract class AbstractRepository
    {
        protected BannerContext Context { get; set; }
        protected ZoomOptions Options { get; set; }

        public AbstractRepository(BannerContext context, SecretConfigManager<ZoomOptions> options)
        {
            Context = context;
            Options = options == null ? null : options.GetValue().Result;
        }

        /// <summary>
        /// Tests connection to Banner appropriate to the derived Repository class and returns true for success.
        /// </summary>
        /// <returns>true if successful</returns>
        public abstract bool TestConnection();
    }
}
