using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using AgentReferralSystem.Api.Data.Config;
using AgentReferralSystem.Api.Data.DataAccess;
using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using AgentReferralSystem.Api.Data.Services;
using AgentReferralSystem.Api.Data.Services.Interfaces;
using AgentReferralSystem.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;

namespace AgentReferralSystem.Api
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
            // set value connection string
            var connectionStrings = new ConnectionStrings();
            Configuration.Bind("ConnectionStrings", connectionStrings);
            services.AddSingleton(connectionStrings);

            var logfilePathConfig = new LogFilePath();
            Configuration.Bind("LogFilePath", logfilePathConfig);
            services.AddSingleton(logfilePathConfig);

            var logfileNameConfig = new LogFileName();
            Configuration.Bind("LogFileName", logfileNameConfig);
            services.AddSingleton(logfileNameConfig);

            services.AddTransient<ICacheDataAccess, CacheDataAccess>();
            services.AddTransient<ISqlServerDataAccess, SqlServerDataAccess>();

            services.AddTransient<IAgentService, AgentService>();
            services.AddTransient<IRewardService, RewardService>();
            services.AddTransient<IScheduleService, ScheduleService>();


            services.AddSwaggerDocumentation();

            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwaggerDocumentation();
            }

            // use cors
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseMvc();

            app.UseStaticFiles(); // For the wwwroot folder

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")),
                RequestPath = "/Images"
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")),
                RequestPath = "/Images"
            });
        }
    }
}
