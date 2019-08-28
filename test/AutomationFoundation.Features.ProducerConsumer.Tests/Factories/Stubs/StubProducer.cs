﻿using System.Threading;
using System.Threading.Tasks;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;

namespace AutomationFoundation.Features.ProducerConsumer.Factories.Stubs
{
    public class StubProducer : IProducer<object>
    {
        public Task<object> ProduceAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}