using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutomationFoundation.Runtime.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutomationFoundation.Runtime
{
    /// <summary>
    /// Provides a runtime for the Automation Foundation framework.
    /// </summary>
    public sealed class AutomationRuntime : IRuntime
    {
        private readonly object syncRoot = new object();
        private readonly IList<IProcessor> processors = new List<IProcessor>();

        private bool disposed;

        /// <summary>
        /// Gets the collection of processors.
        /// </summary>
        public IEnumerable<IProcessor> Processors => new ReadOnlyCollection<IProcessor>(processors);

        /// <inheritdoc />
        public bool IsActive => processors.Any(o => o.State >= ProcessorState.Started);

        private ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationRuntime"/> class.
        /// </summary>
        /// <param name="logger">The logger used to receive logging events.</param>
        public AutomationRuntime(ILogger<AutomationRuntime> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="AutomationRuntime"/> class.
        /// </summary>
        ~AutomationRuntime()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public bool Add(IProcessor processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }
            
            GuardMustNotBeDisposed();

            lock (syncRoot)
            {
                GuardMustNotBeDisposed();

                if (!processors.Contains(processor))
                {
                    processors.Add(processor);
                    return true;
                }

                return false;
            }
        }

        /// <inheritdoc />
        public bool Remove(IProcessor processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }

            GuardMustNotBeDisposed();

            lock (syncRoot)
            {
                GuardMustNotBeDisposed();

                if (processors.Contains(processor))
                {
                    processors.Remove(processor);
                    return true;
                }

                return false;
            }
        }

        /// <inheritdoc />
        public void Start()
        {
            GuardMustNotBeDisposed();

            Logger.LogInformation(new EventId(2000, "STARTING_RUNTIME"), "Starting the runtime...");

            lock (syncRoot)
            {
                GuardMustNotBeDisposed();

                foreach (var processor in Processors)
                {
                    processor.Start();
                }
            }

            Logger.LogInformation(new EventId(2001, "STARTED_RUNTIME"), "Started the runtime.");
        }

        /// <inheritdoc />
        public void Stop()
        {
            GuardMustNotBeDisposed();

            Logger.LogInformation(new EventId(2002, "STOPPING_RUNTIME"), "Stopping the runtime...");

            lock (syncRoot)
            {
                foreach (var processor in Processors)
                {
                    processor.Stop();
                }
            }

            Logger.LogInformation(new EventId(2002, "STOPPED_RUNTIME"), "Stopped the runtime.");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (syncRoot)
                {
                    foreach (var processor in Processors)
                    {
                        processor.Dispose();
                    }

                    disposed = true;
                }
            }
        }

        private void GuardMustNotBeDisposed()
        {
            lock (syncRoot)
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(nameof(AutomationRuntime));
                }
            }
        }
    }
}