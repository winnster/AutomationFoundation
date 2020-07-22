using System;
using System.Threading.Tasks;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;
using AutomationFoundation.Runtime.Abstractions;

namespace AutomationFoundation.Features.ProducerConsumer
{
    /// <summary>
    /// Contains extensions for the consumer engine.
    /// </summary>
    public static class ConsumerEngineExtensions
    {
        /// <summary>
        /// Optionally starts the engine if the engine is an asynchronous consumer engine.
        /// </summary>
        /// <param name="engine">The consumer engine.</param>
        /// <returns>The task being used to start the engine, otherwise a completed task if no task was required.</returns>
        public static Task StartIfAsynchronous<TItem>(this IConsumerEngine<TItem> engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            return (engine as IStartable)?.StartAsync() ??
                   Task.CompletedTask;
        }

        /// <summary>
        /// Optionally stops the engine if the engine is an asynchronous consumer engine.
        /// </summary>
        /// <param name="engine">The consumer engine.</param>
        /// <returns>The task being used to stop the engine, otherwise a completed task if no task was required.</returns>
        public static Task StopIfAsynchronous<TItem>(this IConsumerEngine<TItem> engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            return (engine as IStoppable)?.StopAsync() ??
                   Task.CompletedTask;
        }
    }
}