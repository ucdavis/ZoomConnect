using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZoomConnect.Core.Config;
using SecretJsonConfig;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using AspNetCore.Security.CAS;
using Serilog;
using ZoomConnect.Web.Services;
using System.Collections.Generic;
using ZoomConnect.Web.Services.Mediasite;
using MediasiteUtil;
using ZoomConnect.Web.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using ZoomConnect.Web.DependencyInjection;
using ZoomConnect.Web.Services.ZoomConnect;

namespace ZoomConnect.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // add secrets file so credentials stay encrypted on disk
            services.UseSecretJsonConfig<ZoomOptions>("ZoomSecrets.json");
            // add writable file for mediasite jobs pending
            services.UseSecretJsonConfig<List<MediasiteJob>>("MediasiteJobs.json");

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/login");
                })
                .AddCAS(options =>
                {
                    options.CasServerUrlBase = Configuration["CasBaseUrl"];   // Set in `appsettings.json` file.
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.ServiceForceHTTPS = true;
                });

            services.AddSingleton<IAuthorizationHandler, ListedUserHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ConfiguredAdmins", policy => policy.Requirements.Add(new ListedUserRequirement("AdminUsers")));
            });

            services.AddBanner();
            services.AddCachedRepositories();
            services.AddZoomServices();
            services.AddCanvasZoomServices();
            services.AddSetupRequirements();
            services.AddCommandKey();
            services.AddScoped<EmailService>();
            services.AddMediasiteServices();
            services.AddScoped<DirectoryManager>();
            services.AddScoped<RecordingRouter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
