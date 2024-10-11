// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IJsonDeserialiser, JsonDeserialiser>();

        return services;
    }
}