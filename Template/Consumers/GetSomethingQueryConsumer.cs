namespace MassTransitSample.Template.Consumers
{
    using System.Threading.Tasks;
    using Domain.Queries;
    using MassTransit;
    using MassTransit.ConsumeConfigurators;
    using MassTransit.Definition;
    using Messaging;
    using Microsoft.Extensions.Logging;
    using QueryResults;

    public class GetSomethingQueryConsumer : IConsumer<GetSomethingQuery>
    {
        private ILogger<GetSomethingQueryConsumer> _logger;

        public GetSomethingQueryConsumer(ILogger<GetSomethingQueryConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetSomethingQuery> context)
        {
            // Do something with provided query

            await context.RespondAsync<GetSomethingQueryResult>(new
            {
                Text1 = "Test response for a query.",
            });
        }
    }

    public class GetSomethingQueryConsumerDefinition : ConsumerDefinition<GetSomethingQueryConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<GetSomethingQueryConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseCircuitBreaker();
        }
    }
}
