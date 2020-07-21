using System;
using ZoomConnect.Web.Banner.Cache;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCachedRepositories(this IServiceCollection services)
        {
            services.AddSingleton<SizedCache>();
            services.AddScoped(typeof(CachedRepository<>));

            return services;
        }
    }
}
