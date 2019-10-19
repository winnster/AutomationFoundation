using System;
using System.Threading;
using AutomationFoundation.Runtime.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AutomationFoundation.Runtime
{
    /// <summary>
    /// Provides contextual information of work being processed by the runtime.
    /// </summary>
    public abstract class ProcessingContext : IDisposable
    {
        private static readonly AsyncLocal<ProcessingContext> Local = new AsyncLocal<ProcessingContext>();

        /// <summary>
        /// Gets the current processing context.
        /// </summary>
        public static ProcessingContext Current
        {
            get
            {
                lock (Local)
                {
                    return Local.Value;
                }
            }
        }

        /// <summary>
        /// Clears the current processing context.
        /// </summary>
        public static void Clear()
        {
            lock (Local)
            {
                Local.Value = null;
            }
        }

        /// <summary>
        /// Sets the current processing context.
        /// </summary>
        /// <param name="context">The context which is being processed.</param>
        public static void SetCurrent(ProcessingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            lock (Local)
            {
                Local.Value = context;
            }
        }

        /// <summary>
        /// Gets the identifier of the item being processed.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the services scoped to the lifetime of the item being processed.
        /// </summary>
        public IServiceProvider LifetimeServices => LifetimeScope?.ServiceProvider;

        /// <summary>
        /// Gets the lifetime scope.
        /// </summary>
        protected IServiceScope LifetimeScope { get; }

        /// <summary>
        /// Gets or sets the cancellation token to monitor for cancellation requests.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets or sets the processor which is processing the data.
        /// </summary>
        public IProcessor Processor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingContext"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the context.</param>
        /// <param name="serviceScope">The scope of the request used for dependency injection.</param>
        protected ProcessingContext(Guid id, IServiceScope serviceScope)
        {
            Id = id;
            LifetimeScope = serviceScope;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ProcessingContext"/> class.
        /// </summary>
        ~ProcessingContext()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources, otherwise false to release unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                LifetimeScope.Dispose();
            }
        }
    }
}