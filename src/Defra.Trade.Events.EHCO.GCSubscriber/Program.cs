// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.API.CertificatesStore.V1.ApiClient.Client;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Config;
using Defra.Trade.Common.Function.Health.Extensions;
using Defra.Trade.Common.Function.Health.HealthChecks;
using Defra.Trade.Common.Infra.Infrastructure;
using Defra.Trade.Common.Security.Authentication.Infrastructure;
using Defra.Trade.Events.EHCO.GCSubscriber.Application;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Mappers;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: ExcludeFromCodeCoverage]

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.ConfigureTradeAppConfiguration(config =>
        {
            config.UseKeyVaultSecrets = true;
            config.RefreshKeys.Add($"{GcSubscriberSettings.GcSubscriberSettingsName}:{GcSubscriberSettings.AppConfigSentinelName}");
        });
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services
            .AddTradeAppConfiguration(configuration)
            .AddServiceRegistrations(configuration)
            .AddApplication()
            .AddApimAuthentication(configuration.GetSection(InternalApimSettings.SectionName));
            
        services.AddOptions<InternalApimSettings>()
            .Bind(configuration.GetSection(InternalApimSettings.SectionName));

        services.AddOptions<ApimInternalSettings>()
            .Bind(configuration.GetSection(InternalApimSettings.SectionName));


        var serviceProvider = services.BuildServiceProvider();
        var internalApimSettings = serviceProvider.GetRequiredService<IOptions<InternalApimSettings>>().Value;
        var serviceBusSettings = serviceProvider.GetRequiredService<IOptions<ServiceBusSettings>>().Value;

        var healthEndpoint = internalApimSettings.DaeraInternalCertificateStoreApiHealthEndpoint ?? string.Empty;
        // Defra.Trade.Common.Function.Health 4.1.0+ appends "/health" itself, so strip it if already present.
        var trimmedEndpoint = healthEndpoint.EndsWith("/health", StringComparison.OrdinalIgnoreCase)
            ? healthEndpoint[..^"/health".Length]
            : healthEndpoint;
        var certificateStoreApiPath = $"{internalApimSettings.BaseUrl}{internalApimSettings.DaeraInternalCertificateStoreApi}{trimmedEndpoint}";

        services
            .AddHealthChecks()
            .AddCheck<AppSettingHealthCheck>("ServiceBus:ConnectionString")
            .AddCheck<AppSettingHealthCheck>("Apim:Internal:BaseUrl")
            .AddAzureServiceBusQueueCheck(serviceBusSettings, GcSubscriberSettings.DefaultQueueName)
            .AddTradeApiHealthCheck(certificateStoreApiPath, "CertificateStoreApi");

        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(GeneralCertificateRequestProfile).Assembly));
    })
    .Build();

await host.RunAsync();
