using System;
using ZoomConnect.Web.Banner;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BannerExtensions
    {
        public static IServiceCollection AddBanner(this IServiceCollection services)
        {
            services.AddScoped<BannerContext>();

            services.Scan(scan => scan
                .FromAssemblyOf<TestRepository>()
                .AddClasses(classes => classes.AssignableTo(typeof(AbstractRepository<>)))
                .AsSelf()
                .WithScopedLifetime());

            return services;
        }
    }
}
