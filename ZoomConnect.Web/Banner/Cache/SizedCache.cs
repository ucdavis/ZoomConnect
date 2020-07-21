using Microsoft.Extensions.Caching.Memory;
using System;

namespace ZoomConnect.Web.Banner.Cache
{
    public class SizedCache
    {
        public MemoryCache Cache { get; set; }

        public SizedCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 5000    // limit of 5,000 rows before cache will stop adding rows.
            });
        }
    }
}
