using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using csharpWeddings.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace csharpWeddings // CHANGE DEPENDING ON PROJECT
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            //Add our JSON file to the project...
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // CHANGE CONTEXT VARIABLE DEPENDING ON PROJECT
            services.AddDbContext<WeddingContext>(options => options.UseNpgsql(Configuration["DBInfo:ConnectionString"]));
            services.AddMvc();
            services.AddSession();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc();

        }
    }
}