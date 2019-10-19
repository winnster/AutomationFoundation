using System.Threading.Tasks;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;

namespace AutomationFoundation.Features.ProducerConsumer.Strategies.TestObjects
{
    public class StubConsumer<TItem> : IConsumer<TItem>
    {
        public Task ConsumeAsync(ProducerConsumerContext<TItem> context)
        {
            return Task.CompletedTask;
        }
    }
}