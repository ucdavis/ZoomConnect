using System.Collections.Generic;

namespace ZoomConnect.Web.Banner.Cache
{
    public interface ICachedRepository
    {
        /// <summary>
        /// Gets Banner connection test result from cache if available, otherwise pulls from Banner and caches result
        /// </summary>
        /// <returns></returns>
        public bool TestConnection();

        /// <summary>
        /// Lists the tables the repository accesses.
        /// </summary>
        public List<string> Tables { get; }
    }
}
