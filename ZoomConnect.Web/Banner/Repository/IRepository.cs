using System.Collections.Generic;

namespace ZoomConnect.Web.Banner.Repository
{
    public interface IRepository
    {
        /// <summary>
        /// Tests connection to Banner appropriate to the derived Repository class and returns true for success.
        /// </summary>
        /// <returns>true if successful</returns>
        public bool TestConnection();

        /// <summary>
        /// Lists the tables this repository accesses.
        /// </summary>
        public List<string> Tables { get; }
    }
}
