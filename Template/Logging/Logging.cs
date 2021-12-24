namespace MassTransitSample.Template.Logging
{
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Primitives;
    using Serilog;

    /// <summary>
    /// Common logging behavior
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// Enriches the diagnostic context by a few properties.
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigureCommonLogging(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(
                options =>
                {
                    options.MessageTemplate = "[{CorrelationID}] {RequestMethod} {RequestHost}{RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms to {RequestScheme}://{RemoteIpAddress}";
                    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                    {
                        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                        diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                    };
                });
        }
        
        /// <summary>
        /// Sets up Serilog logger to write to console (temporary solution).
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IHostBuilder UseCommonLogging(this IHostBuilder host)
        {
            return host.UseSerilog((_, services, configuration) => configuration
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console());
        }
    }
}
