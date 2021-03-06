using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Karma.Data;
using Karma.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Karma.Middleware;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Karma.Interceptor;
using Karma.Services;

namespace Karma
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            LoadItemTypes(Path.Combine(ItemTypes.ItemTypesPath, ItemTypes.ItemTypesFileName));

            var fileWatcher = new FileWatcher(env);
        }

        public static void LoadItemTypes(string path)
        {
            string types = System.IO.File.ReadAllText(path);
            Models.ItemTypes.Types = JsonConvert.DeserializeObject<Dictionary<int, Models.ItemType>>(types);
        }

        public static void SaveItemTypes(string path)
        {
            string types = JsonConvert.SerializeObject(Models.ItemTypes.Types);
            System.IO.File.WriteAllText(path, types);
        }

        public ILifetimeScope AutofacContainer { get; private set; }
        public IConfiguration Configuration { get; }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac here. Don't
            // call builder.Populate(), that happens in AutofacServiceProviderFactory
            // for you.

            builder.RegisterType<MessageService>().As<IMessageService>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(MethodLog))
                .InstancePerDependency();

            builder.Register(c => new MethodLog());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
             .WriteTo.File("logs/Log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            services.AddSingleton(x => Log.Logger);

            // Adding localization services
            services.AddLocalization(option => { option.ResourcesPath = "Resources"; });
            services.AddMvc().AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.AddControllersWithViews();
            services.AddRazorPages();

            // Adding database context with sqlite option
            services.AddDbContext<KarmaContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            // Adding singleton for Google Maps API
            services.AddSingleton<GoogleMaps>(ConfigureGoogleMaps());

            // Adding identity
            services.AddIdentity<Karma.Models.User, IdentityRole>()
                .AddDefaultUI()
                .AddUserManager<Karma.Models.UserManage>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<KarmaContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._+";
            });

            // Adding authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminAccess", policy => policy.RequireRole("Admin"));

                options.AddPolicy("CharityManagerAccess", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Admin")
                        || context.User.IsInRole("Charity manager")));

                options.AddPolicy("UserAccess", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("User")
                        || context.User.IsInRole("Admin")
                        || context.User.IsInRole("Charity manager")));
            });
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
                app.UseStatusCodePagesWithRedirects("Error/{0}");
                app.UseHsts();
            }

            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Supported cultures for localization
            var supportedCultures = new[] { "en", "lt" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);
            app.UseAnalyticsMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            AddDirectories(env);

        }

        private void AddDirectories(IWebHostEnvironment env)
        {
            // Create necessary directories if they do not exist.
            System.IO.Directory.CreateDirectory(Path.Combine(env.WebRootPath, Karma.Models.Post.ImagesDirName));
            System.IO.Directory.CreateDirectory(Path.Combine(env.WebRootPath, Karma.Models.Advert.ImagesDirName));
            System.IO.Directory.CreateDirectory(Path.Combine(env.WebRootPath, Karma.Models.Charity.ImagesDirName));
        }

        private GoogleMaps ConfigureGoogleMaps()
        {
            GoogleMaps maps = new GoogleMaps();
            maps.ServiceApiKey = Configuration.GetValue<string>("GoogleMaps:ServiceApiKey");

            return maps;
        }
    }
}
