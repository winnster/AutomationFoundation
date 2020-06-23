﻿using System;
using System.Threading;

namespace ConsoleRunner.Infrastructure.WorkProcessors
{
    public class Monitor
    {
        /// <summary>
        /// Defines the span of time for thirty seconds.
        /// </summary>
        private static readonly TimeSpan ThirtySeconds = new TimeSpan(0, 0, 0, 30);

        private readonly Timer timer;
        private readonly string name;
        private long count;

        public Monitor(string name)
        {
            timer = new Timer(OnTimerCallback);

            this.name = name;
        }

        public void Start()
        {
            timer.Change(ThirtySeconds, ThirtySeconds);
        }

        public void Stop()
        {
            timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);        
        }

        private void OnTimerCallback(object state)
        {
            var current = Interlocked.Exchange(ref count, 0);

            var itemsPerSecond = current / ThirtySeconds.TotalSeconds;
            Console.Out.WriteLine($"{name}@{DateTime.Now:hh:mm:ss}: {itemsPerSecond}/sec");
        }

        public void Increment()
        {
            Interlocked.Increment(ref count);
        }
    }
}