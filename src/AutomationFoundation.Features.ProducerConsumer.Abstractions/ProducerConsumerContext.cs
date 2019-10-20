﻿using System;
using AutomationFoundation.Runtime;
using AutomationFoundation.Runtime.Abstractions.Synchronization;
using Microsoft.Extensions.DependencyInjection;

namespace AutomationFoundation.Features.ProducerConsumer
{
    /// <summary>
    /// Provides contextual information regarding the production and consumption of work being processed by the runtime.
    /// </summary>
    /// <typeparam name="TItem">The type of item which was produced.</typeparam>
    public class ProducerConsumerContext<TItem> : ProcessingContext
    {
        /// <summary>
        /// Gets or sets the item which was produced.
        /// </summary>
        public virtual TItem Item { get; set; }

        /// <summary>
        /// Gets or sets the synchronization lock for the produced item.
        /// </summary>
        public virtual ISynchronizationLock SynchronizationLock { get; set; }

        /// <summary>
        /// Gets the contextual information regarding the production of work.
        /// </summary>
        public virtual ProductionContext<TItem> ProductionContext { get; } = new ProductionContext<TItem>();

        /// <summary>
        /// Gets the contextual information regarding the consumption of work.
        /// </summary>
        public virtual ConsumptionContext<TItem> ConsumptionContext { get; } = new ConsumptionContext<TItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerConsumerContext{TItem}"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the context.</param>
        /// <param name="serviceScope">The scope of the request used for dependency injection.</param>
        public ProducerConsumerContext(Guid id, IServiceScope serviceScope)
            : base(id, serviceScope)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerConsumerContext{TItem}"/> class.
        /// </summary>
        protected ProducerConsumerContext()
        {
        }
    }
}