using ZoomConnect.Web.SetupRequirements;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RequirementsExtensions
    {
        public static IServiceCollection AddSetupRequirements(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<ISetupRequirement>()
                .AddClasses(classes => classes.AssignableTo<ISetupRequirement>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddScoped<RequirementManager>();

            return services;
        }
    }
}
