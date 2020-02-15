using System;
using NUnit.Framework;
using ManualResetEventSlim = System.Threading.ManualResetEventSlim;

#pragma warning disable S3626 // False positive

namespace AutomationFoundation.Runtime.Threading.Primitives
{
    [TestFixture]
    public class TimerTests
    {
        [Test]
        [Timeout(10000)]
        public void RunsTheCallbackAsExpected()
        {
            var called = false;

            using var waitEvent = new ManualResetEventSlim(false);
            using var target = new Timer();

            target.Start(TimeSpan.FromSeconds(1), () =>
            {
                called = true;
            }, error => { });

            waitEvent.Wait(5000);

            Assert.True(called);
        }

        [Test]
        [Timeout(10000)]
        public void RunsTheErrorCallbackAsExpected()
        {
            var called = false;

            using var waitEvent = new ManualResetEventSlim(false);
            using var target = new Timer();

            target.Start(TimeSpan.FromSeconds(1), () => throw new InvalidOperationException(), error =>
            {
                Assert.IsInstanceOf<InvalidOperationException>(error);

                called = true;
            });

            waitEvent.Wait(5000);

            Assert.True(called);
        }
    }
}