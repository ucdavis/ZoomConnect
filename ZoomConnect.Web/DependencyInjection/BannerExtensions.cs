using ZoomConnect.Web.Banner;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Banner.Repository;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BannerExtensions
    {
        public static IServiceCollection AddBanner(this IServiceCollection services)
        {
            services.AddScoped<BannerContext>();

            // add repositories when requested as concrete types, and as implemented interfaces
            services.Scan(scan => scan
                .FromAssemblyOf<TestRepository>()
                    .AddClasses(classes => classes.AssignableTo(typeof(AbstractRepository<>)))
                        .AsSelf()
                        .WithScopedLifetime()
                    .AddClasses(classes => classes.AssignableTo<IRepository>())
                        .AsImplementedInterfaces()
                        .WithScopedLifetime());

            // add repositories when requested as abstract generics
            services.AddScoped<AbstractRepository<dual>, TestRepository>();
            services.AddScoped<AbstractRepository<goremal>, GoremalRepository>();
            services.AddScoped<AbstractRepository<sirasgn>, SirasgnRepository>();
            services.AddScoped<AbstractRepository<sobcald>, SobcaldRepository>();
            services.AddScoped<AbstractRepository<spriden>, SpridenRepository>();
            services.AddScoped<AbstractRepository<spriden_student>, StudentRepository>();
            services.AddScoped<AbstractRepository<ssbsect>, SsbsectRepository>();
            services.AddScoped<AbstractRepository<ssrmeet>, SsrmeetRepository>();
            services.AddScoped<AbstractRepository<stvsubj>, StvsubjRepository>();
            services.AddScoped<AbstractRepository<stvterm>, StvtermRepository>();

            return services;
        }
    }
}
