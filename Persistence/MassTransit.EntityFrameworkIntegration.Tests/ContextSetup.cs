﻿namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using Log4NetIntegration.Logging;
    using NUnit.Framework;

    [SetUpFixture]
    public class ContextSetup
    {
        [OneTimeSetUp]
        public void Before_any()
        {
            Log4NetLogger.Use("test.log4net.xml");
        }
    }
}