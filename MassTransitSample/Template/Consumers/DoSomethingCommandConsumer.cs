namespace MassTransitSample.Template.Consumers
{
    using System.Threading.Tasks;
    using Domain.Commands;
    using DomainModels;
    using MassTransit;
    using Microsoft.Extensions.Logging;

    public class DoSomethingCommandConsumer : IConsumer<DoSomethingCommand>
    {
        private readonly ILogger<DoSomethingCommandConsumer> _logger;

        public DoSomethingCommandConsumer(ILogger<DoSomethingCommandConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DoSomethingCommand> context)
        {
            _logger.LogInformation($"{nameof(CommandSomethingModel)} command received.");
            _logger.LogInformation(
                $"{context.Message.TextDomainInfo} - {context.Message.NumberDomainInfo} - {context.Message.SomeObject}");
        }
    }
}
