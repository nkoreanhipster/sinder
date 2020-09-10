using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using Westwind.AspNetCore.LiveReload;

namespace Sinder
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
            // Live reload config
            //services.AddLiveReload();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Validate connections string and App.config is set
            try
            {
                string connstring = Helper.GetConnectionString();
            }
            catch (NullReferenceException e)
            {
                System.Diagnostics.Debug.Write("Set the upp the App.config plox. See README");
            }

            Dataprovider.Instance.TestConnection();

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

            /// 404 handling
            //app.Use(async (context, next) =>
            //{
            //    await next();
            //    if (context.Response.StatusCode == 404)
            //    {
            //        //context.Request.Path = "/";
            //        //context.Response.Body = "hello";
            //        //await next();
            //    }
            //});

            // Mammas död att vi använder https.
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "login",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "register",
                    pattern: "{controller=Registration}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "search",
                    pattern: "{controller=Search}/{action=Index}/{id?}");
                //endpoints.MapControllerRoute(
                //    name: "api", 
                //    pattern: "{controller=ApiController}/{action=Index}/{id?}");
            });

            // Live reload config
            //app.UseLiveReload();
        }
    }
}
