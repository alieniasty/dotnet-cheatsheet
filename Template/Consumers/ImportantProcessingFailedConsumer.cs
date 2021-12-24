namespace MassTransitSample.Template.Consumers
{
    using System.Threading.Tasks;
    using Domain.Events;
    using MassTransit;
    using Microsoft.Extensions.Logging;

    public class ImportantProcessingFailedConsumer : IConsumer<ImportantProcessingFailed>
    {
        private readonly ILogger<ImportantProcessingFailedConsumer> _logger;

        public ImportantProcessingFailedConsumer(ILogger<ImportantProcessingFailedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ImportantProcessingFailed> context)
        {
            _logger.LogInformation($"{nameof(ImportantProcessingFailed)} event received.");
            _logger.LogInformation($"{context.Message.TextDomainInfo} - {context.Message.NumberDomainInfo} - {context.Message.SomeObject}");
        }
    }
}
