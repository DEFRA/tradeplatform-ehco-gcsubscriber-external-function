// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.Function.Health.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Functions;

/// <summary>
/// A http function that checks the health status of the function app.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HealthCheckFunction"/> class.
/// </remarks>
/// <param name="healthCheckService">Health check service instance.</param>
public class HealthCheckFunction(HealthCheckService healthCheckService)
{
    private readonly HealthCheckService _healthCheckService = healthCheckService;

    /// <summary>
    /// Runs the http health check function.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    [FunctionName(nameof(HealthCheckFunction))]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameter required")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest request)
    {
        var healthReport = await _healthCheckService.CheckHealthAsync();

        if (healthReport.Status == HealthStatus.Healthy)
        {
            return new OkObjectResult("healthy");
        }

        var objectResult = new ObjectResult(healthReport.ToResponse())
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        return objectResult;
    }
}
