// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Mappers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Infrastructure;

[ExcludeFromCodeCoverage]
public static class ConfigureMapperExtensions
{
    public static void ConfigureMapper(this IFunctionsHostBuilder hostBuilder)
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName).ToList();
        hostBuilder.Services.AddAutoMapper(assembly);

        hostBuilder.Services.AddAutoMapper(typeof(GeneralCertificateRequestProfile).Assembly);
    }
}