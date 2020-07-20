using SecretJsonConfig;
using System;
using System.Collections.Generic;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Banner
{
    public abstract class AbstractRepository<TBannerTable> where TBannerTable : IBannerTable
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

        /// <summary>
        /// Get all rows from this table, filtered by current options.
        /// </summary>
        /// <returns></returns>
        public abstract List<TBannerTable> GetAll();
    }
}
