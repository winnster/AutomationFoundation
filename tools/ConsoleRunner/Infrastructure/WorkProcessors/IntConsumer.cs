using System.Threading.Tasks;
using AutomationFoundation.Features.ProducerConsumer;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;
using ConsoleRunner.Infrastructure.IO;

namespace ConsoleRunner.Infrastructure.WorkProcessors
{
    public class IntConsumer : IConsumer<int>
    {
        private static readonly Monitor Monitor = new Monitor("Consumed", new ConsoleWriter());

        static IntConsumer()
        {
            Monitor.Start();
        }

        public Task ConsumeAsync(ProducerConsumerContext<int> context)
        {
            Monitor.Increment();
            
            return Task.CompletedTask;
        }
    }
}