namespace MassTransitSample.Domain.Queries
{
    using System;
    using MassTransit;

    // PLEASE NOTICE THE NAMESPACE NAME. IT IS IMPORTANT THAT ALL MESSAGES HAVE THE SAME NAMESPACE NAME, OTHERWISE THEY WILL NOT BE RECEIVED.
    public interface GetSomethingQuery: CorrelatedBy<Guid>
    {
        public string Id { get; set; }
    }
}
