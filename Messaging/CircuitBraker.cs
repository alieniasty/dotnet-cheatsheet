namespace MassTransitSample.Messaging
{
    using System;
    using GreenPipes.Configurators;
    using MassTransit;

    public static class CircuitBreaker
    {
        /// <summary>
        ///     Configure circuit breaker for endpoint.
        /// </summary>
        /// <param name="endpointConfigurator"></param>
        /// <param name="configurator">custom configurator for variables used by circuit breaker.</param>
        public static void UseCircuitBreaker(this IReceiveEndpointConfigurator endpointConfigurator, Action<ICircuitBreakerConfigurator<ConsumeContext>> configurator = null)
        {
            var circuitBreakerConfigurator = configurator ?? (options =>
            {
                options.TrackingPeriod = TimeSpan.FromMinutes(1);
                options.TripThreshold = 15;
                options.ActiveThreshold = 10;
                options.ResetInterval = TimeSpan.FromMinutes(5);
            });
            
            endpointConfigurator.UseCircuitBreaker(circuitBreakerConfigurator);
        }
    }
}