namespace MassTransitSample.Template.HealthChecks
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public class ExampleReadinessHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            // Here check things that are required by service to process requests i.e. database connections, other services availability, data sets load etc. 
            bool importantCheckSucceeded;

            importantCheckSucceeded = true;
            //importantCheckSucceeded = new Random().Next(0, 6) >= 5; Uncomment this to simulate ready check fail.

            if (importantCheckSucceeded == false)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(description: "Important check failed"));
            }
            
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
