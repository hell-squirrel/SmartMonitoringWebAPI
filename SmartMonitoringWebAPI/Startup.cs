using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SmartMonitoring.Commons.Configuration;
using SmartMonitoring.Commons.Middlewares;
using SmartMonitoring.Infrastructure.Interfaces;
using SmartMonitoring.Infrastructure.Services;
using SmartMonitoring.Service;
using SmartMonitoring.Service.Interfaces;
using System;

namespace SmartMonitoringWebAPI
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
            services.AddSingleton(new DatabaseConfiguration { Name = Configuration["DatabaseName"] });

            services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
            services.AddSingleton<IAssignmentRepository, AssignmentRepository>();
            services.AddSingleton<ISmartMonitoringService, SmartMonitoringService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Smart Monitoring Assignment",
                    Version = "v1",
                    Description = "Applitation to work with Monitoring Assignments using basic CRUD operation.\n\n" +
                    "Every assignment has it's own label set to provide better maintainability.\n\n" +
                    "Properties can be update individually.\n\n" +
                    "Search provided with pagination, individial search by assignment name or label is available.\n\n"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartMonitoringWebAPI v1"));
            }

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ///Initialize database if not created
            serviceProvider.GetService<IDatabaseInitializer>().Setup();
        }
    }
}
