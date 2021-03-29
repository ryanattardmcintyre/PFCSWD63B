using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.DataAccess.Repositories;
using Google.Cloud.SecretManager.V1;
using Newtonsoft.Json;
using Google.Cloud.Diagnostics.AspNetCore;

namespace WebApplication1
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
            //registering an Application Service with a static collection called services which is injected in the runtime pipeline


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IBlogsRepository, FirestoreBlogRepository>();
            services.AddScoped<ICachingService, CachingService>();
            services.AddScoped<IPubSubRepository, PubSubRepository>();
            services.AddScoped<ILog, LogRepository>();


            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                 .AddDefaultUI()
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders();

            services.AddControllersWithViews();
            services.AddRazorPages();


            /*
             * google client id/ secret key
             * mailgun api key
             * connection string (password)
             * cache password
             * 
             */

            string googleClientid = GetPassword("Authentication:Google:ClientId");
            string password = GetPassword("Authentication:Google:ClientSecret");


            services.AddAuthentication()
                   .AddGoogle(options =>
                   {
                       //   IConfigurationSection googleAuthNSection =
                       //     Configuration.GetSection("Authentication:Google");

                       options.ClientId = GetPassword("Authentication:Google:ClientId"); //googleAuthNSection["ClientId"];
                       options.ClientSecret = GetPassword("Authentication:Google:ClientSecret");// googleAuthNSection["ClientSecret"];
                   });



            services.AddGoogleExceptionLogging(options =>
            {
                options.ProjectId = "pfc2021";
                options.ServiceName = "BlogService";
                options.Version = "0.01";
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseGoogleExceptionLogging();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/Identity/Account/Register", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));
                endpoints.MapPost("/Identity/Account/Register", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));

                // Omitted for clarity
            });
        }


        public string GetPassword(string key)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Build the resource name.
            SecretVersionName secretVersionName = new SecretVersionName("pfc2021", "ApiClientId", "2");

            // Call the API.
            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

            // Convert the payload to a string. Payloads are bytes by default.
            String payload = result.Payload.Data.ToStringUtf8();


            dynamic obj = JsonConvert.DeserializeObject(payload);
            string value = obj[key];

            return value;
        }


    }
}
