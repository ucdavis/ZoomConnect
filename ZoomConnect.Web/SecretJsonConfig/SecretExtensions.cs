using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace ZoomConnect.Web.SecretJsonConfig
{
    public static class SecretExtensions
    {
        public static void UseSecretJsonConfig<TSecret>(this IServiceCollection services, string filename) where TSecret : new()
        {
            var fileProvider = new PhysicalFileProvider(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            var secrets = new SecretConfigManager<TSecret> { SecretFile = fileProvider.GetFileInfo(filename) };

            services.AddSingleton<SecretConfigManager<TSecret>>(secrets);
        }
    }
}
