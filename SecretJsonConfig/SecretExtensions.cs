using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

namespace SecretJsonConfig
{
    public static class SecretExtensions
    {
        public static void UseSecretJsonConfig<TSecret>(this IServiceCollection services, string filename) where TSecret : new()
        {
            services.AddDataProtection()
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
                });

            services.AddTransient<Crypt>();

            services.TryAddSingleton<SecretConfigManager<TSecret>>(sp =>
            {
                var fileProvider = new PhysicalFileProvider(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                var crypt = sp.GetRequiredService<Crypt>();
                return new SecretConfigManager<TSecret>(crypt) { SecretFile = fileProvider.GetFileInfo(filename) };
            });
        }
    }
}
