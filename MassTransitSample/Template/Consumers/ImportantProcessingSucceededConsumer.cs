namespace MassTransitSample.Template.Consumers
{
    using System.Threading.Tasks;
    using Domain.Events;
    using MassTransit;
    using Microsoft.Extensions.Logging;

    public class ImportantProcessingSucceededConsumer : IConsumer<ImportantProcessingSucceeded>
    {
        private readonly ILogger<ImportantProcessingSucceededConsumer> _logger;

        public ImportantProcessingSucceededConsumer(ILogger<ImportantProcessingSucceededConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ImportantProcessingSucceeded> context)
        {
            _logger.LogInformation($"{nameof(ImportantProcessingSucceeded)} event received.");
            _logger.LogInformation($"{context.Message.TextDomainInfo} - {context.Message.NumberDomainInfo} - {context.Message.SomeObject}");
        }
    }
}
