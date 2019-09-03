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
using testWorkflow.Controllers;
using WorkflowCore.Interface;

namespace testWorkflow
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddLogging();
            
            services.AddWorkflow();

            services.AddTransient<HelloWorld>();
            services.AddTransient<GoodbyeWorld>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            var host = app.ApplicationServices.GetRequiredService<IWorkflowHost>();
            host.RegisterWorkflow<HelloWorldWorkflow>();
            host.Start();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}