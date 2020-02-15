using System;
using AutomationFoundation.Runtime.Abstractions.Threading.Internal;
using AutomationFoundation.Runtime.Abstractions.Threading.Primitives;
using AutomationFoundation.Runtime.TestObjects;
using AutomationFoundation.Runtime.Threading.Internal;
using Moq;
using NUnit.Framework;

#pragma warning disable S3626 // False positive

namespace AutomationFoundation.Runtime.Threading
{
    [TestFixture]
    public class WorkerPoolTests
    {
        private Mock<IRuntimeWorker> worker;
        private Mock<IWorkerCacheMonitor> cacheMonitor;
        private Mock<IWorkerCache> cache;

        [SetUp]
        public void Setup()
        {
            worker = new Mock<IRuntimeWorker>();
            cacheMonitor = new Mock<IWorkerCacheMonitor>();
            cache = new Mock<IWorkerCache>();
        }

        [Test]
        public void ThrowsAnExceptionWhenTheCacheIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new WorkerPool(null, cacheMonitor.Object))
                {
                    // This code block intentionally left blank.
                }
            });
        }

        [Test]
        public void ThrowsAnExceptionWhenTheCacheMonitorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new WorkerPool(cache.Object, null))
                {
                    // This code block intentionally left blank.
                }
            });
        }

        [Test]
        public void CreatesThePoolAsExpected()
        {
            using var target = WorkerPool.Create();
            Assert.IsNotNull(target);
        }

        [Test]
        public void GetsTheWorkerAsExpected()
        {
            cache.Setup(o => o.Get()).Returns(worker.Object);

            using var target = new WorkerPool(cache.Object, cacheMonitor.Object);
            var result = target.Get(OnRun, OnCompleted);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PooledWorker>(result);
        }

        [Test]
        public void ThrowsAnExceptionWhenTheOnRunCallbackIsNull()
        {
            using var target = new WorkerPool(cache.Object, cacheMonitor.Object);
            Assert.Throws<ArgumentNullException>(() => target.Get(null, OnCompleted));
        }

        [Test]
        public void DoesNotThrowAnExceptionWhenThePostCompletedCallbackIsNull()
        {
            using var target = new WorkerPool(cache.Object, cacheMonitor.Object);
            Assert.DoesNotThrow(() => target.Get(OnRun, null));
        }

        [Test]
        public void ReleasesTheWorkerAsExpectedIfAnErrorOccurs()
        {
            cache.Setup(o => o.Get()).Returns(worker.Object);

            using var target = new TestableWorkerPool(cache.Object, cacheMonitor.Object)
            {
                OnCreatePooledWorkerCallback = o => throw new ApplicationException("Test exception")
            };

            Assert.Throws<ApplicationException>(() => target.Get(OnRun, OnCompleted));
            cache.Verify(o => o.Release(worker.Object), Times.Once);
        }

        [Test]
        public void ThrowsAnExceptionWhenTheWorkerIsNull()
        {
            cache.Setup(o => o.Get()).Returns(worker.Object);

            using var target = new TestableWorkerPool(cache.Object, cacheMonitor.Object)
            {
                OnBaseCreatePooledWorkerCallback = o => null
            };

            Assert.Throws<ArgumentNullException>(() => target.Get(OnRun, OnCompleted));
            cache.Verify(o => o.Release(worker.Object), Times.Once);
        }

        [Test]
        public void ThrowsAnExceptionWhenAfterDispose()
        {
            var target = new TestableWorkerPool(cache.Object, cacheMonitor.Object);
            target.Dispose();

            Assert.Throws<ObjectDisposedException>(() => target.Get(OnRun, OnCompleted));
        }

        private static void OnRun()
        {
            // This method intentionally left blank.
        }

        private static void OnCompleted()
        {
            // This method intentionally left blank.
        }
    }
}
