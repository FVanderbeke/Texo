using System.Data.Common;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MicroElements.Swashbuckle.NodaTime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Serilog;
using Texo.Domain.Model.Service;
using Texo.Domain.Module;
using Texo.Infrastructure.Db.Module;
using Texo.Infrastructure.Db.Service;

namespace Texo.Application.Rest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddApplicationPart(Assembly.Load("Texo.Application.Rest")).AddControllersAsServices();
            
            services.AddControllers().AddJsonOptions(options =>
            {               
                options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Texo.Application.Rest", Version = "v1"});

                JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
                ConfigureSystemTextJsonSerializerSettings(jsonSerializerOptions);
                c.ConfigureForNodaTimeWithSystemTextJson(jsonSerializerOptions, shouldGenerateExamples: true);
            });
        }
        
        void ConfigureSystemTextJsonSerializerSettings(JsonSerializerOptions serializerOptions)
        {
            // Configures JsonSerializer to properly serialize NodaTime types.
            serializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        }
        
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Instantiating the database connection (mandatory for SQLite).
            var connection = new SqliteConnection(@"Data Source=:memory:");
            connection.Open();

            // Declaring the logger
            var logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            // Now, creating the IOC container.
            
            builder.RegisterInstance(logger).As<ILogger>();
            builder.RegisterInstance(connection).As<DbConnection>();
            builder.RegisterInstance(TexoUtils.DefaultClock).As<IClock>();
            builder.RegisterInstance(TexoUtils.DefaultIdGenerator).As<IIdGenerator>();
            builder.RegisterModule(new DomainModule());
            builder.RegisterModule(new DbModule());
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Texo.Application.Rest v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}