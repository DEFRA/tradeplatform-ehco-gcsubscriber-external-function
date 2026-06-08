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
using Google.Protobuf.WellKnownTypes;
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
            .Bind(configuration);


        var serviceProvider = services.BuildServiceProvider();
        var internalApimSettings = serviceProvider.GetRequiredService<IOptions<InternalApimSettings>>().Value;
        var serviceBusSettings = serviceProvider.GetRequiredService<IOptions<ServiceBusSettings>>().Value;

        services
            .AddHealthChecks()
            .AddCheck<AppSettingHealthCheck>("ServiceBus:ConnectionString")
            .AddCheck<AppSettingHealthCheck>("Apim:Internal:BaseUrl")
            .AddAzureServiceBusQueueCheck(serviceBusSettings, GcSubscriberSettings.DefaultQueueName)
            .AddTradeInternalApiCheck<InternalApimSettings>(serviceProvider,
                $"{internalApimSettings.DaeraInternalCertificateStoreApi}{internalApimSettings.DaeraInternalCertificateStoreApiHealthEndpoint}"
            );

        var assembly = AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName).ToList();
        services.AddAutoMapper(assembly);
        services.AddAutoMapper(typeof(GeneralCertificateRequestProfile).Assembly);
    })
    .Build();

await host.RunAsync();
