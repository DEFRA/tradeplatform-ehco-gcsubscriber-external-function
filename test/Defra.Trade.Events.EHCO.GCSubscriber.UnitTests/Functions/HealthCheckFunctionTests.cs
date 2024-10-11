// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Defra.Trade.Common.Function.Health;
using Defra.Trade.Events.EHCO.GCSubscriber.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shouldly;

namespace Defra.Trade.Events.EHCO.GCSubscriber.UnitTests.Functions;

public sealed class HealthCheckFunctionTests
{
    private readonly Mock<HealthCheckService> _healthCheckService;
    private readonly HealthCheckFunction _sut;

    public HealthCheckFunctionTests()
    {
        _healthCheckService = new Mock<HealthCheckService>();
        _sut = new HealthCheckFunction(_healthCheckService.Object);
    }

    [Fact]
    public async Task RunAsync_HealthyCheck_Returns200AndHealthy()
    {
        // arrange
        var ct = CancellationToken.None;
        var entries = new Dictionary<string, HealthReportEntry>();
        var healthReport = new HealthReport(
            entries,
            HealthStatus.Healthy,
            new TimeSpan(0, 0, 1, 31));

        _healthCheckService.Setup(s => s.CheckHealthAsync(null, ct)).ReturnsAsync(healthReport);

        var httpRequest = new DefaultHttpContext();

        // act
        var response = await _sut.RunAsync(httpRequest.Request);

        // assert
        response.ShouldNotBeNull();
        var result = response as OkObjectResult;
        result.StatusCode.ShouldBe(200);
        result.Value.ShouldBe("healthy");
    }

    [Fact]
    public async Task RunAsync_UnhealthyCheck_Returns500AndUnhealthy()
    {
        // arrange
        var ct = CancellationToken.None;
        var entries = new Dictionary<string, HealthReportEntry>()
        {
            {
                "failing check",
                new HealthReportEntry(HealthStatus.Unhealthy,
                "failing check",
                new TimeSpan(0, 0, 1, 31),
                new DivideByZeroException(),
                null)
            },
            {
                "passing check",
                new HealthReportEntry(HealthStatus.Healthy,
                "passing check",
                new TimeSpan(0, 0, 0, 0, 321),
                null,
                null)
            }
        };

        var healthReport = new HealthReport(
            entries,
            HealthStatus.Unhealthy,
            new TimeSpan(0, 0, 1, 31));

        _healthCheckService.Setup(s => s.CheckHealthAsync(null, ct)).ReturnsAsync(healthReport);

        var httpRequest = new DefaultHttpContext();

        // act
        var response = await _sut.RunAsync(httpRequest.Request);

        // assert
        response.ShouldNotBeNull();
        var result = response as ObjectResult;
        result.StatusCode.ShouldBe(500);
        var healthCheck = result.Value as HealthCheckResponse;
        healthCheck.Status.ShouldBe("Unhealthy");
        var healthCheckResults = healthCheck.Results.ToList();
        healthCheckResults[0].Status.ShouldBe("Unhealthy");
        healthCheckResults[1].Status.ShouldBe("Healthy");
    }
}
