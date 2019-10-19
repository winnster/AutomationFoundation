using System;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;
using AutomationFoundation.Features.ProducerConsumer.Resolvers.TestObjects;
using Moq;
using NUnit.Framework;

namespace AutomationFoundation.Features.ProducerConsumer.Resolvers
{
    [TestFixture]
    public class CallbackConsumerResolverTests
    {
        [Test]
        public void ThrowsAnExceptionWhenTheContextIsNull()
        {
            var target = new CallbackConsumerResolver<object>((context) => null);
            Assert.Throws<ArgumentNullException>(() => target.Resolve(null));
        }

        [Test]
        public void ThrowsAnExceptionWhenTheCallbackIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CallbackConsumerResolver<object>(null));
        }

        [Test]
        public void ReturnsTheResultFromTheCallback()
        {
            var target = new CallbackConsumerResolver<object>(_ => new StubConsumer());
            var result = target.Resolve(new Mock<ProducerConsumerContext<object>>().Object);

            Assert.IsInstanceOf<StubConsumer>(result);
        }

        [Test]
        public void DoesNotThrowAnErrorWhenTheResultIsNull()
        {
            var target = new CallbackConsumerResolver<object>(_ => null);
            var result = target.Resolve(new Mock<ProducerConsumerContext<object>>().Object);

            Assert.IsNull(result);
        }
    }
}