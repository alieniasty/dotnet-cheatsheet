﻿namespace MassTransitSample.Domain.Commands
{
    using System;
    using MassTransit;

    // PLEASE NOTICE THE NAMESPACE NAME. IT IS IMPORTANT THAT ALL MESSAGES HAVE THE SAME NAMESPACE NAME, OTHERWISE THEY WILL NOT BE RECEIVED.
    public interface DoSomethingCommand : CorrelatedBy<Guid>
    {
        public string TextDomainInfo { get; set; }
        public int NumberDomainInfo { get; set; }
        public string SomeObject { get; set; }
    }
}
