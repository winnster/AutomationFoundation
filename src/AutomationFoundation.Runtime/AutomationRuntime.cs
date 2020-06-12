﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutomationFoundation.Runtime.Abstractions;

namespace AutomationFoundation.Runtime
{
    /// <summary>
    /// Provides a runtime for the Automation Foundation framework.
    /// </summary>
    public sealed class AutomationRuntime : IRuntime
    {
        private readonly IList<IProcessor> processors = new List<IProcessor>();

        private bool disposed;

        /// <summary>
        /// Gets the collection of processors.
        /// </summary>
        public IEnumerable<IProcessor> Processors => new ReadOnlyCollection<IProcessor>(processors);

        /// <inheritdoc />
        public bool IsRunning => processors.Any(o => o.State >= ProcessorState.Started);

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

            if (!processors.Contains(processor))
            {
                processors.Add(processor);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public bool Remove(IProcessor processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }

            GuardMustNotBeDisposed();

            if (processors.Contains(processor))
            {
                processors.Remove(processor);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            GuardMustNotBeDisposed();

            foreach (var processor in processors)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await processor.StartAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            GuardMustNotBeDisposed();

            foreach (var processor in processors)
            {
                await processor.StopAsync(cancellationToken);
            }
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
                foreach (var processor in Processors)
                {
                    processor.Dispose();
                }

                disposed = true;
            }
        }

        private void GuardMustNotBeDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(AutomationRuntime));
            }
        }
    }
}