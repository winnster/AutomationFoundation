﻿using System;
using NUnit.Framework;

namespace AutomationFoundation.Hosting
{
    [TestFixture]
    public class HostingExceptionTests
    {
        [Test]
        public void SetsTheMessageAsExpected1()
        {
            var target = new HostingException("This is a test message!");
            Assert.AreEqual(target.Message, "This is a test message!");
        }

        [Test]
        public void SetsTheMessageAndInnerExceptionAsExpected1()
        {
            var innerEx = new Exception("This is a test exception!");

            var target = new HostingException("This is a test message!", innerEx);

            Assert.AreEqual(target.Message, "This is a test message!");
            Assert.AreEqual(target.InnerException, innerEx);
        }

    }
}