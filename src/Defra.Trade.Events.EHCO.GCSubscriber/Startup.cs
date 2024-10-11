// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Function.Health.HealthChecks;
using Defra.Trade.Common.Infra.Infrastructure;
using Defra.Trade.Common.Security.Authentication.Infrastructure;
using Defra.Trade.Events.EHCO.GCSubscriber.Application;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Infrastructure;
using FunctionHealthCheck;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Defra.Trade.Events.EHCO.GCSubscriber.Startup))]

namespace Defra.Trade.Events.EHCO.GCSubscriber;

[ExcludeFromCodeCoverage]
public class Startup : FunctionsStartup
{
    public static IConfiguration Configuration { get; private set; }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        Configuration = builder.GetContext().Configuration;

        builder.AddConfigurations();

        builder.Services
            .AddTradeAppConfiguration(Configuration)
            .AddServiceRegistrations(Configuration)
            .AddApplication()
            .AddApimAuthentication(Configuration.GetSection(InternalApimSettings.SectionName));

        builder.Services.AddOptions<InternalApimSettings>()
            .Bind(Configuration);

        var healthChecksBuilder = builder.Services.AddFunctionHealthChecks();

        RegisterHealthChecks(healthChecksBuilder, builder.Services, Configuration);
        builder.ConfigureMapper();
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        builder.ConfigurationBuilder
            .ConfigureTradeAppConfiguration(config =>
            {
                config.UseKeyVaultSecrets = true;
                config.RefreshKeys.Add($"{GcSubscriberSettings.GcSubscriberSettingsName}:{GcSubscriberSettings.AppConfigSentinelName}");
            });
    }

    protected static void RegisterHealthChecks(
        IHealthChecksBuilder builder,
        IServiceCollection services,
        IConfiguration configuration)
    {
        builder.AddCheck<AppSettingHealthCheck>("ServiceBus:ConnectionString")
            .AddCheck<AppSettingHealthCheck>("Apim:Internal:BaseUrl");

        var internalApimSettings = services.BuildServiceProvider().GetRequiredService<IOptions<InternalApimSettings>>();

        builder.AddTradeInternalApiCheck<InternalApimSettings>(
            services.BuildServiceProvider(),
            $"{internalApimSettings.Value.DaeraInternalCertificateStoreApi}{internalApimSettings.Value.DaeraInternalCertificateStoreApiHealthEndpoint}");

        builder.AddAzureServiceBusCheck(configuration, "ServiceBus:ConnectionString", GcSubscriberSettings.DefaultQueueName);
    }
}
