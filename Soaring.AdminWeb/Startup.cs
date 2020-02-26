using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soaring.AdminWeb.Services;
using Soaring.AdminWeb.Wrappers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Soaring.AdminWeb {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.Configure<CookiePolicyOptions> (options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthorization (options => {
                //声明一个名为Administrator的授权，判断角色的claimType为Role的值是否equal Administrator
                options.AddPolicy ("Administrator", policy => policy.RequireClaim ("Role", "Administrator"));
            });
            services.AddAuthentication (CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie (options => {
                    options.AccessDeniedPath = new PathString ("/Sign/NotPermission");
                    options.LoginPath = new PathString ("/Sign/Login");
                    options.ExpireTimeSpan = TimeSpan.FromDays (15);
                });
            services.AddSingleton<MongoWrapper> (new MongoWrapper ("mongodb://192.168.150.129:27017", "CollectXomlTask"));
            services.AddSingleton<XomlTaskService> ();
            services.AddControllersWithViews ();
            services.AddRazorPages ();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseExceptionHandler ("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }
            app.UseStaticFiles ();

            app.UseAuthentication ();
            app.UseRouting ();

            app.UseAuthorization ();
            app.UseCookiePolicy ();

            app.UseEndpoints (endpoints => {
                endpoints.MapControllerRoute (
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages ();
            });
        }
    }
}