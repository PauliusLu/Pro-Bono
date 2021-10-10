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
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace Karma
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LoadItemTypes(@"Data/ItemTypes.txt");
        }

        private void LoadItemTypes(string path)
        {
            string types = System.IO.File.ReadAllText(path);
            Models.ItemType.Types = JsonConvert.DeserializeObject<Dictionary<int, Models.ItemType>>(types);
        }

        private void SaveItemTypes(string path)
        {
            string types = JsonConvert.SerializeObject(Models.ItemType.Types);
            System.IO.File.WriteAllText(path, types);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding localization services
            services.AddLocalization(option => { option.ResourcesPath = "Resources"; });
            services.AddMvc().AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
            
            services.AddControllersWithViews();
            services.AddRazorPages();

            // Adding database context with sqlite option
            services.AddDbContext<KarmaContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            // Adding identity
            services.AddIdentity<Karma.Models.User, Karma.Models.UserRole>()
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<KarmaContext>();
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
            System.IO.Directory.CreateDirectory(Path.Combine(env.WebRootPath, Karma.Models.Charity.ImagesDirName));
            System.IO.Directory.CreateDirectory(Path.Combine(env.ContentRootPath, Karma.Models.Charity.ItemTypesDirName));
            System.IO.Directory.CreateDirectory(Path.Combine(env.ContentRootPath, Karma.Models.Charity.AdressDirName));
        }
    }
}
