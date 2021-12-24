namespace MassTransitSample.Template.Services
{
    using System;
    using System.Threading.Tasks;
    using Domain.Commands;
    using Domain.Events;
    using Domain.Queries;
    using DomainModels;
    using MassTransit;
    using QueryResults;

    public interface ITestService
    {
        Task DoStuffAndPublish(ImportantProcessingModel importantProcessingModel);
        Task DoStuffAndSend(CommandSomethingModel commandSomethingModel);
        Task<Response<GetSomethingQueryResult>> DoStuffAndQuery(string id);
    }

    public class TestService : ITestService
    {
        private readonly IBus _bus;
        private readonly IRequestClient<GetSomethingQuery> _client;
        private readonly IPublishEndpoint _publishEndpoint;

        public TestService(IBus bus, IPublishEndpoint publishEndpoint, IRequestClient<GetSomethingQuery> client)
        {
            _bus = bus;
            _publishEndpoint = publishEndpoint;
            _client = client;
        }

        public async Task DoStuffAndPublish(ImportantProcessingModel importantProcessingModel)
        {
            // Do something with importantProcessingModel.

            // Please notice that whether processing was successful or not, we still publish the event. That means we treat failure as part of the business logic.
            // The other scenario is that processing fails and exception is thrown. In this case we DO NOT publish such event.

            var rng = new Random();
            bool successful = rng.Next(0, 2) > 0;

            if (successful is true)
            {
                await _publishEndpoint.Publish<ImportantProcessingSucceeded>(importantProcessingModel);
            }
            else
            {
                await _publishEndpoint.Publish<ImportantProcessingFailed>(importantProcessingModel);
            }
        }

        public async Task DoStuffAndSend(CommandSomethingModel commandSomethingModel)
        {
            ISendEndpoint sendEndpoint = await _bus.GetPublishSendEndpoint<DoSomethingCommand>();

            // Do something with commandSomethingModel.

            await sendEndpoint.Send<DoSomethingCommand>(commandSomethingModel);
        }

        public async Task<Response<GetSomethingQueryResult>> DoStuffAndQuery(string id)
        {
            // Do something with that id.

            Response<GetSomethingQueryResult> response =
                await _client.GetResponse<GetSomethingQueryResult>(new { Id = id });
            return response;
        }
    }
}
