using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthorization();

            var host = app.ApplicationServices.GetRequiredService<IWorkflowHost>();
            host.RegisterWorkflow<MultipleWorkflow>();
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