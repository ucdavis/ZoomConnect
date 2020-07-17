using System;

namespace ZoomConnect.Web.Banner
{
    public abstract class AbstractRepository
    {
        protected BannerContext Context { get; set; }

        public AbstractRepository(BannerContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Tests connection to Banner appropriate to the derived Repository class and returns true for success.
        /// </summary>
        /// <returns>true if successful</returns>
        public abstract bool TestConnection();
    }
}
