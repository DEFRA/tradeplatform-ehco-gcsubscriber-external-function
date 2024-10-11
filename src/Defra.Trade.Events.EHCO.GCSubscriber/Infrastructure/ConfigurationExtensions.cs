// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Infrastructure;

[ExcludeFromCodeCoverage]
public static class ConfigurationExtensions
{
    public static IConfiguration Configuration { get; private set; }

    public static void AddConfigurations(this IFunctionsHostBuilder hostBuilder)
    {
        Configuration = hostBuilder.GetContext().Configuration;
    }
}