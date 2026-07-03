// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Azure.Messaging.ServiceBus;
using DocStoreApi = Defra.Trade.API.CertificatesStore.V1.ApiClient.Api;
using Defra.Trade.API.CertificatesStore.V1.ApiClient.Client;
using Defra.Trade.Common.Config;
using Defra.Trade.Common.Functions.Isolated;
using Defra.Trade.Common.Functions.Isolated.EventStore;
using Defra.Trade.Common.Functions.Isolated.Interfaces;
using Defra.Trade.Common.Functions.Isolated.Models;
using Defra.Trade.Common.Functions.Isolated.Services;
using Defra.Trade.Common.Functions.Isolated.Validation;
using Defra.Trade.Common.Infra.Infrastructure;
using Defra.Trade.Common.Security.Authentication.Interfaces;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Infrastructure;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Validators;
using Defra.Trade.Events.EHCO.GCSubscriber.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Defra.Trade.Common.Function.Health.HealthChecks.ApiCheck;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Infrastructure;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISchemaValidator, SchemaValidator>();
        services.AddSingleton<ICustomValidatorFactory, CustomValidatorFactory>();
        services.AddSingleton<AbstractValidator<TradeEventMessageHeader>, GCMessageHeaderValidator>();
        services.AddSingleton<AbstractValidator<GeneralCertificateInbound>, GeneralCertificateInboundValidator>();

        services.AddEventStoreConfiguration();

        services.AddTransient<IMessageProcessor<GeneralCertificateRequest, TradeEventMessageHeader>, SbMessageProcessor>();
        services.AddTransient<IInboundMessageValidator<GeneralCertificateInbound, TradeEventMessageHeader>,
            InboundMessageValidator<GeneralCertificateInbound, GeneralCertificateRequest, TradeEventMessageHeader>>();
        services.AddTransient<IBaseMessageProcessorService<GeneralCertificateInbound>,
            BaseMessageProcessorService<GeneralCertificateInbound, GeneralCertificateRequest, GeneralCertificateRequest, TradeEventMessageHeader>>();

        services.AddSingleton<IMessageCollector, EventStoreCollector>();
        services.AddTransient<IGcMessageProcessor, GcMessageProcessor>();

        services.AddOptions<ServiceBusQueuesSettings>().Bind(configuration.GetSection(ServiceBusSettings.OptionsName));
        services.AddScoped<IGeneralCertificateEnrichmentProcessor, GeneralCertificateEnrichmentProcessor>();
        services.AddScoped<IServiceBusManagerClient, ServiceBusManagerClient>();
        services.AddSingleton<IQueueClientFactory, QueueClientFactory>();
        services.AddSingleton(sp =>
        {
#if DEBUG
            string connectionString = configuration.GetValue<string>(GcSubscriberSettings.ConnectionStringConfigurationKey);
            return new ServiceBusClient(connectionString);
#else
            string connectionString = configuration.GetValue<string>($"{GcSubscriberSettings.ConnectionStringConfigurationKey}FQN");
            return new ServiceBusClient(connectionString, new Azure.Identity.DefaultAzureCredential());
#endif
        });

        var gcConfig = configuration.GetSection(GcSubscriberSettings.GcSubscriberSettingsName);
        services.AddOptions<GcSubscriberSettings>().Bind(gcConfig);

        var appConfig = configuration.GetSection(InternalApimSettings.SectionName);
        services.AddOptions<InternalApimSettings>().Bind(appConfig);

        services.Configure<ServiceBusSettings>(configuration.GetSection(ServiceBusSettings.OptionsName));

        services.AddClientApiServices();

        services.AddMessageRetryService();

        return services;
    }

    private static void AddClientApiServices(this IServiceCollection services)
    {
        services
            .AddTransient<DocStoreApi.IEhcoGeneralCertificateApplicationApi>(provider => new DocStoreApi.EhcoGeneralCertificateApplicationApi(CreateConfigurationSettings(provider)))
            .AddTransient<DocStoreApi.IHealthApi>(provider => new DocStoreApi.HealthApi(CreateConfigurationSettings(provider)))
            .AddTransient<IHealthApi, DocStoreHealthApi>();
    }

    private static IServiceCollection AddMessageRetryService(this IServiceCollection services)
    {
        return services
            .AddSingleton<MessageRetryService>()
            .AddSingleton<IMessageRetryService>(provider => provider.GetRequiredService<MessageRetryService>())
            .AddSingleton<IMessageRetryContextAccessor>(provider => provider.GetRequiredService<MessageRetryService>());
    }

    private static Configuration CreateConfigurationSettings(IServiceProvider provider)
    {
        var authService = provider.GetService<IAuthenticationService>();
        var apimSettings = provider.GetService<IOptions<InternalApimSettings>>()!.Value;
        string authToken = authService.GetAuthenticationHeaderAsync().Result.ToString();
        var config = new Configuration
        {
            BasePath = $"{apimSettings.BaseUrl}{apimSettings.DaeraInternalCertificateStoreApi}",
            DefaultHeaders = new Dictionary<string, string>
            {
                { "Authorization", authToken },
                { apimSettings.SubscriptionKeyHeaderName, apimSettings.SubscriptionKey }
            }
        };
        return config;
    }
}
