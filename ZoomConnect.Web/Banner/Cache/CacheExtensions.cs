using System;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCachedRepositories(this IServiceCollection services)
        {
            services.AddSingleton<SizedCache>();
            services.AddScoped(typeof(CachedRepository<>));

            // add cached repositories as implemented interface
            //services.Scan(scan => scan
            //    .FromAssemblyOf<ICachedRepository>()
            //    .AddClasses(classes => classes.AssignableTo<ICachedRepository>())
            //    .AsImplementedInterfaces()
            //    .WithScopedLifetime());
            services.AddScoped<ICachedRepository, CachedRepository<dual>>();
            services.AddScoped<ICachedRepository, CachedRepository<goremal>>();
            services.AddScoped<ICachedRepository, CachedRepository<sirasgn>>();
            services.AddScoped<ICachedRepository, CachedRepository<sobcald>>();
            services.AddScoped<ICachedRepository, CachedRepository<spriden>>();
            services.AddScoped<ICachedRepository, CachedRepository<spriden_student>>();
            services.AddScoped<ICachedRepository, CachedRepository<ssbsect>>();
            services.AddScoped<ICachedRepository, CachedRepository<ssrmeet>>();
            services.AddScoped<ICachedRepository, CachedRepository<stvsubj>>();
            services.AddScoped<ICachedRepository, CachedRepository<stvterm>>();

            return services;
        }
    }
}
