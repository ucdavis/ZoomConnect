using System;
using System.Collections.Generic;
using ZoomConnect.Web.Banner.Domain;
using Microsoft.Extensions.Caching.Memory;
using ZoomConnect.Web.Banner.Repository;

namespace ZoomConnect.Web.Banner.Cache
{
    public class CachedRepository<TTable> : ICachedRepository where TTable : IBannerTable
    {
        private AbstractRepository<TTable> _repository;
        private IMemoryCache _cache;
        private string _cacheKeyRows;
        private string _cacheKeyTest;

        public CachedRepository(AbstractRepository<TTable> repository, SizedCache cache)
        {
            _repository = repository;
            _cache = cache.Cache;
            var typename = typeof(TTable).Name;
            _cacheKeyRows = $"{typename}_rows";
            _cacheKeyTest = $"{typename}_test";
        }

        public bool TestConnection()
        {
            // see if test result is cached
            if (_cache.TryGetValue(_cacheKeyTest, out bool cacheEntry))
            {
                return cacheEntry;
            }

            // otherwise get test result from db into cache
            cacheEntry = _repository.TestConnection();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(1)
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(_cacheKeyTest, cacheEntry, cacheEntryOptions);

            return cacheEntry;
        }

        public List<string> Tables => _repository.Tables;

        /// <summary>
        /// Gets Banner rows from cache if available, otherwise pulls from Banner and caches results
        /// </summary>
        /// <returns></returns>
        public List<TTable> GetAll()
        {
            // see if rows are cached
            if (_cache.TryGetValue(_cacheKeyRows, out List<TTable> cacheEntry))
            {
                return cacheEntry;
            }

            // otherwise get rows from db into cache
            cacheEntry = _repository.GetAll();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(cacheEntry.Count)
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(_cacheKeyRows, cacheEntry, cacheEntryOptions);

            return cacheEntry;
        }
    }
}
