namespace MassTransitSample.Template
{
    using System;
    using System.Collections.Generic;
    using Consumers;
    using HealthChecks;
    using Mappings;
    using Messaging;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using Services;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAutoMapper(typeof(MappingProfile));

            var consumersWithDefinitions = new Dictionary<Type, Type>
            {
                { typeof(DoSomethingCommandConsumer), null },
                { typeof(GetSomethingQueryConsumer), typeof(GetSomethingQueryConsumerDefinition) },
                { typeof(ImportantProcessingFailedConsumer), null },
                { typeof(ImportantProcessingSucceededConsumer), null },
            };

            services.AddConsumersWithDefinitions(consumersWithDefinitions);
            services.ConfigureMessagingTopology();

            // Add multiple checks here if required and group them between live check and ready checks with tags
            // You can use packages like AspNetCore.HealthChecks.SqlServer to check health of many popular services.
            services.AddHealthChecks()
                .AddCheck("LivenessCheck", tags: new[] { "liveness" }, check: () => HealthCheckResult.Healthy())
                // word "ready" can't be used as tag for unknown reason
                .AddCheck<ExampleReadinessHealthCheck>("ReadinessCheck", tags: new[] { "readiness" });

            services.AddTransient<ITestService, TestService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Input API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // Use these endpoints in livenessProbe and readinessProbe sections of deployment YAML file
                // in container template for this this service.
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("liveness"),
                });

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("readiness"),
                });

                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("v1/swagger.json", "Input API V1"); });
        }
    }
}
