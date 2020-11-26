using Application.Exceptions;
using Autofac;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.SqlServer;
using Infrastructure.Hangfire;
using Infrastructure.Logging;
using Infrastructure.MassTransit;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;

namespace Api
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
            services.AddMassTransitHostedService();
            services.AddHangfire(cfg =>
            {
                cfg.UseSqlServerStorage(Environment.GetEnvironmentVariable("HANGFIRE_CONNECTIONSTRING"))
                    .UseDashboardMetric(SqlServerStorage.ActiveConnections)
                    .UseDashboardMetric(SqlServerStorage.TotalConnections)
                    .UseDashboardMetric(DashboardMetrics.FailedCount);
            });
            services.AddHangfireServer(opt =>
            {
                opt.WorkerCount = Environment.ProcessorCount * 5;
                opt.Queues = new[] { "sample" };
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Sample Service",
                    Description = "Sample description"
                });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //register modules here
            builder.RegisterModule(new LoggingModule(Log.Logger));
            builder.RegisterModule(new MassTransitModule());
            builder.RegisterModule(new HangfireModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/swagger/v1/swagger.json", "Sample");
                });
            }

            app.UseSerilogRequestLogging();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseHangfireDashboard("/SampleDashboard", new DashboardOptions
            {
                Authorization = new[] {
                    new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        LoginCaseSensitive = true,
                        RequireSsl = false,
                        SslRedirect = false,
                        Users = new[]
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = Environment.GetEnvironmentVariable("HANGFIRE_USER"),
                                PasswordClear = Environment.GetEnvironmentVariable("HANGFIRE_PASSWORD")
                            }
                        }
                    })
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
