using SecretJsonConfig;
using System;
using System.Collections.Generic;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.Banner.Repository
{
    public abstract class AbstractRepository<TBannerTable> : IRepository where TBannerTable : IBannerTable
    {
        protected BannerContext Context { get; set; }
        protected ZoomOptions Options { get; set; }

        public virtual List<string> Tables => new List<string>() { typeof(TBannerTable).Name };

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

        /// <summary>
        /// Get all rows from this table, filtered by current options.
        /// </summary>
        /// <returns></returns>
        public abstract List<TBannerTable> GetAll();
    }
}
