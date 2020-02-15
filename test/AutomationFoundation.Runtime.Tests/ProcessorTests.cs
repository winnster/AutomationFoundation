using System;
using AutomationFoundation.Runtime.TestObjects;
using NUnit.Framework;

#pragma warning disable S3626 // False positive

namespace AutomationFoundation.Runtime
{
    [TestFixture]
    public class ProcessorTests
    {
        [Test]
        public void ThrowsAnExceptionWhenStartedAfterDisposed()
        {
            var target = new StubProcessor();
            target.Dispose();

            Assert.Throws<ObjectDisposedException>(() => target.Start());
        }

        [Test]
        public void ThrowsAnExceptionWhenStoppedAfterDisposed()
        {
            var target = new StubProcessor();
            target.Dispose();

            Assert.Throws<ObjectDisposedException>(() => target.Stop());
        }

        [Test]
        public void ThrowsAnExceptionWhenStartingWhileInTheErrorState()
        {
            using var target = new StubProcessor();
            target.SetState(ProcessorState.Error);

            Assert.Throws<RuntimeException>(() => target.Start());
        }

        [Test]
        public void ThrowsAnExceptionWhenStoppingWhileInTheErrorState()
        {
            using var target = new StubProcessor();
            target.SetState(ProcessorState.Error);

            Assert.Throws<RuntimeException>(() => target.Stop());
        }

        [Test]
        public void ThrowsAnExceptionWhenNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new StubProcessor(null))
                {
                    // This code block intentionally left blank.
                }
            }, "name");
        }

        [Test]
        public void ThrowsAnExceptionWhenNameIsWhitespace()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new StubProcessor("   "))
                {
                    // This code block intentionally left blank.
                }
            }, "name");
        }

        [Test]
        public void ReturnsTheCreatedStateUponNew()
        {
            using var target = new StubProcessor();
            Assert.AreEqual(ProcessorState.Created, target.State);
        }

        [Test]
        public void StoppingTwiceThrowsAnException()
        {
            using var target = new StubProcessor();
            target.Start();
            target.Stop();

            Assert.Throws<RuntimeException>(() => target.Stop());
        }

        [Test]
        public void SetsTheNamePropertyDuringInitialization()
        {
            using var target = new StubProcessor("Test");
            Assert.AreEqual("Test", target.Name);
        }

        [Test]
        public void ChangeToAnErrorStateWhenExceptionThrownDuringStart()
        {
            using var target = new StubProcessor();
            target.SetupCallbacks(() => throw new Exception("This is a test exception"));

            Assert.Throws<Exception>(() => target.Start());
            Assert.AreEqual(ProcessorState.Error, target.State);
        }

        [Test]
        public void ChangeToAnErrorStateWhenExceptionThrownDuringStop()
        {
            using var target = new StubProcessor();
            target.SetupCallbacks(onStopCallback: () => throw new Exception("This is a test exception"));
            target.Start();

            Assert.Throws<Exception>(() => target.Stop());
            Assert.AreEqual(ProcessorState.Error, target.State);
        }

        [Test]
        public void ChangeTheProcessorStatesDuringStart()
        {
            var tested = false;

            using var target = new StubProcessor();
            target.SetupCallbacks(() =>
            {
                Assert.AreEqual(ProcessorState.Starting, target.State);
                tested = true;
            });

            target.Start();

            Assert.True(tested);
            Assert.AreEqual(ProcessorState.Started, target.State);
        }

        [Test]
        public void ChangeTheProcessorStatesDuringStop()
        {
            var tested = false;

            using var target = new StubProcessor();
            target.SetupCallbacks(onStopCallback: () =>
            {
                Assert.AreEqual(ProcessorState.Stopping, target.State);
                tested = true;
            });

            target.Start();
            target.Stop();

            Assert.True(tested);
            Assert.AreEqual(ProcessorState.Stopped, target.State);
        }

        [Test]
        public void ThrowAnExceptionWhenTheProcessorIsAlreadyStarted()
        {
            using var target = new StubProcessor();
            target.Start();

            Assert.AreEqual(ProcessorState.Started, target.State);
            Assert.Throws<RuntimeException>(() => target.ExecuteGuardMustNotAlreadyBeStarted());
        }

        [Test]
        public void ThrowAnExceptionWhenTheProcessorIsAlreadyStopped()
        {
            using var target = new StubProcessor();
            target.Start();
            Assert.AreEqual(ProcessorState.Started, target.State);

            target.Stop();
            Assert.AreEqual(ProcessorState.Stopped, target.State);

            Assert.Throws<RuntimeException>(() => target.ExecuteGuardMustNotAlreadyBeStopped());
        }

        [Test]
        public void DisposeOfAProcessorThatIsNeverStarted()
        {
            var called = false;

            var target = new StubProcessor();
            target.SetupCallbacks(onDispose: () => called = true);

            target.Dispose();

            Assert.IsTrue(called);
        }
    }
}
