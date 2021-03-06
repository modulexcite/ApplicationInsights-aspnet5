﻿namespace Microsoft.ApplicationInsights.AspNet.Tests.TelemetryInitializers
{
    using System;
    using Microsoft.ApplicationInsights.AspNet.TelemetryInitializers;
    using Microsoft.ApplicationInsights.AspNet.Tests.Helpers;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.AspNet.Hosting;
    using Xunit;
    using Microsoft.AspNet.Http.Internal;

    public class OperationIdTelemetryInitializerTest
    {
        [Fact]
        public void InitializeThrowIfHttpContextAccessorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => { var initializer = new OperationIdTelemetryInitializer(null); });
        }

        [Fact]
        public void InitializeDoesNotThrowIfHttpContextIsUnavailable()
        {
            var ac = new HttpContextAccessor() { HttpContext = null };

            var initializer = new OperationIdTelemetryInitializer(ac);

            initializer.Initialize(new RequestTelemetry());
        }

        [Fact]
        public void InitializeDoesNotThrowIfRequestTelemetryIsUnavailable()
        {
            var ac = new HttpContextAccessor() { HttpContext = new DefaultHttpContext() };

            var initializer = new OperationIdTelemetryInitializer(ac);

            initializer.Initialize(new RequestTelemetry());
        }

        [Fact]
        public void InitializeDoesNotOverrideOperationIdProvidedInline()
        {
            var telemetry = new EventTelemetry();
            telemetry.Context.Operation.Id = "123";
            var contextAccessor = HttpContextAccessorHelper.CreateHttpContextAccessor();

            var initializer = new OperationIdTelemetryInitializer(contextAccessor);

            initializer.Initialize(telemetry);

            Assert.Equal("123", telemetry.Context.Operation.Id);
        }

        [Fact]
        public void InitializeSetsTelemetryOperationIdToRequestId()
        {
            var telemetry = new EventTelemetry();
            var requestTelemetry = new RequestTelemetry();
            var contextAccessor = HttpContextAccessorHelper.CreateHttpContextAccessor(requestTelemetry);

            var initializer = new OperationIdTelemetryInitializer(contextAccessor);

            initializer.Initialize(telemetry);

            Assert.Equal(requestTelemetry.Id, telemetry.Context.Operation.Id);
        }
    }
}