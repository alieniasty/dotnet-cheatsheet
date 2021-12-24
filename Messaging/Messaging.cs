namespace MassTransitSample.Messaging
{
    using System;
    using System.Collections.Generic;
    using MassTransit;
    using MassTransit.ActiveMqTransport;
    using Microsoft.Extensions.DependencyInjection;

    public static class Messaging
    {
        private static Dictionary<Type, Type> _consumersWithDefinitions;

        /// <summary>
        ///     Pass dictionary of types (consumers and definitions) to be registered as message consumers.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="consumersWithDefinitions">
        ///     Dictionary of types (consumers and definitions) that will be registered as
        ///     message consumers.
        /// </param>
        public static void AddConsumersWithDefinitions(this IServiceCollection services,
            Dictionary<Type, Type> consumersWithDefinitions)
        {
            _consumersWithDefinitions = consumersWithDefinitions;
        }

        /// <summary>
        ///     Configures the application as a consumer of the messages.
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureMessagingTopology(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                if (_consumersWithDefinitions is not null)
                {
                    foreach (var consumersWithDefinition in _consumersWithDefinitions)
                    {
                        x.AddConsumer(consumersWithDefinition.Key, consumersWithDefinition.Value);
                    }
                }

                x.UsingActiveMq((context, cfg) =>
                {
                    cfg.Host(BusConsts.Hostname, h =>
                    {
                        h.Username(BusConsts.Username);
                        h.Password(BusConsts.Password);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
            services.AddGenericRequestClient();
            services.AddMassTransitHostedService();
        }
    }
}
