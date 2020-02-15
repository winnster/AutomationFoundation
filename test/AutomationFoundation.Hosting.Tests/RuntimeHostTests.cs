using System;
using AutomationFoundation.Hosting.Abstractions;
using AutomationFoundation.Runtime.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace AutomationFoundation.Hosting
{
    [TestFixture]
    public class RuntimeHostTests
    {
        private Mock<IRuntime> runtime;
        private Mock<IHostingEnvironment> environment;
        private Mock<IServiceProvider> services;
        private Mock<ILogger<RuntimeHost>> logger;

        [SetUp]
        public void Setup()
        {
            runtime = new Mock<IRuntime>();
            environment = new Mock<IHostingEnvironment>();
            services = new Mock<IServiceProvider>();
            logger = new Mock<ILogger<RuntimeHost>>();
        }

        [Test]
        public void ThrowsAnExceptionWhenRuntimeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RuntimeHost(null, environment.Object, services.Object, logger.Object));
        }

        [Test]
        public void ThrowsAnExceptionWhenEnvironmentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RuntimeHost(runtime.Object, null, services.Object, logger.Object));
        }

        [Test]
        public void DoesNotThrowsAnExceptionWhenServicesAreNull()
        {
            Assert.DoesNotThrow(() => new RuntimeHost(runtime.Object, environment.Object, null, logger.Object));
        }

        [Test]
        public void StartsTheRuntime()
        {
            var target = new RuntimeHost(runtime.Object, environment.Object, services.Object, logger.Object);
            target.Start();

            runtime.Verify(o => o.Start(), Times.Once);
        }

        [Test]
        public void StopsTheRuntime()
        {
            var target = new RuntimeHost(runtime.Object, environment.Object, services.Object, logger.Object);
            target.Stop();

            runtime.Verify(o => o.Stop(), Times.Once);
        }

        [Test]
        public void ReturnsTheServices()
        {
            var target = new RuntimeHost(runtime.Object, environment.Object, services.Object, logger.Object);
            Assert.AreSame(services.Object, target.ApplicationServices);
        }

        [Test]
        public void ReturnsTheEnvironment()
        {
            var target = new RuntimeHost(runtime.Object, environment.Object, services.Object, logger.Object);
            Assert.AreSame(environment.Object, target.Environment);
        }
    }
}