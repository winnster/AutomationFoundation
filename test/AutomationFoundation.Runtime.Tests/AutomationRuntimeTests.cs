using System;
using System.Linq;
using AutomationFoundation.Runtime.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace AutomationFoundation.Runtime
{
    [TestFixture]
    public class AutomationRuntimeTests
    {
        private Mock<ILogger<AutomationRuntime>> logger;
        private Mock<IProcessor> processor;

        [SetUp]
        public void Setup()
        {
            processor = new Mock<IProcessor>();
            logger = new Mock<ILogger<AutomationRuntime>>();
        }

        [Test]
        public void ThrowsAnExceptionWhenLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new AutomationRuntime(null));
        }

        [Test]
        public void ReturnsTheProcessorsThatHaveBeenAdded()
        {
            using var target = new AutomationRuntime(logger.Object);
            target.Add(processor.Object);

            Assert.True(target.Processors.Contains(processor.Object));
        }

        [Test]
        public void IdentifiesTrueIfTheProcessorIsStarted()
        {
            processor.Setup(o => o.State).Returns(ProcessorState.Started);

            using var target = new AutomationRuntime(logger.Object);
            target.Add(processor.Object);

            Assert.True(target.IsActive);
        }

        [Test]
        public void IdentifiesTrueIfTheProcessorIsBusy()
        {
            processor.Setup(o => o.State).Returns(ProcessorState.Busy);

            using var target = new AutomationRuntime(logger.Object);
            target.Add(processor.Object);

            Assert.True(target.IsActive);
        }

        [Test]
        public void ReturnsTrueWhenTheProcessorHasBeenAdded()
        {
            using var target = new AutomationRuntime(logger.Object);
            var result = target.Add(processor.Object);

            Assert.True(result);
        }

        [Test]
        public void ReturnsFalseWhenTheProcessorHasAlreadyBeenAdded()
        {
            using var target = new AutomationRuntime(logger.Object);
            target.Add(processor.Object);

            var result = target.Add(processor.Object);
            Assert.False(result);
        }

        [Test]
        public void ReturnsTrueWhenTheProcessorHasBeenRemoved()
        {
            using var target = new AutomationRuntime(logger.Object);
            target.Add(processor.Object);

            var result = target.Remove(processor.Object);
            Assert.True(result);
        }

        [Test]
        public void ReturnsFalseWhenTheProcessorHasNotBeenAdded()
        {
            using var target = new AutomationRuntime(logger.Object);
            var result = target.Remove(processor.Object);

            Assert.False(result);
        }

        [Test]
        public void ThrowsAnExceptionIfTheProcessorIsNullWhenAdded()
        {
            using var target = new AutomationRuntime(logger.Object);
            Assert.Throws<ArgumentNullException>(() => target.Add(null));
        }

        [Test]
        public void ThrowsAnExceptionIfTheProcessorIsNullWhenRemoved()
        {
            using var target = new AutomationRuntime(logger.Object);
            Assert.Throws<ArgumentNullException>(() => target.Remove(null));
        }

        [Test]
        public void StartsTheProcessor()
        {
            using var target = new AutomationRuntime(logger.Object);
            target.Add(processor.Object);

            target.Start();

            processor.Verify(o => o.Start(), Times.Once);
        }

        [Test]
        public void StopsTheProcessor()
        {
            using var target = new AutomationRuntime(logger.Object);
            target.Add(processor.Object);

            target.Stop();

            processor.Verify(o => o.Stop(), Times.Once);
        }

        [Test]
        public void ThrowAnExceptionWhenStartedAfterDisposed()
        {
            var target = new AutomationRuntime(logger.Object);
            target.Dispose();

            Assert.Throws<ObjectDisposedException>(() => target.Start());
        }

        [Test]
        public void ThrowAnExceptionWhenStoppedAfterDisposed()
        {
            var target = new AutomationRuntime(logger.Object);
            target.Dispose();

            Assert.Throws<ObjectDisposedException>(() => target.Stop());
        }
    }
}