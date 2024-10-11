// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;

public class GcSubscriberSettings
{
    public static string GcSubscriberSettingsSection => GcSubscriberSettingsName;

    public const string GcSubscriberSettingsName = "EhcoGcSubscriber";

    public const string AppName = "Defra.Trade.Events.EHCO.GCSubscriber";

#if DEBUG

    // In 'Debug' (locally) use connection string
    public const string ConnectionStringConfigurationKey = "ServiceBus:ConnectionString";

#else
    // Assumes that this is 'Release' and uses Managed Identity rather than connection string
    // ie it will actually bind to ServiceBus:FullyQualifiedNamespace !
    public const string ConnectionStringConfigurationKey = "ServiceBus";
#endif

    public string GcCreatedQueue { get; set; } = DefaultQueueName;

    public const string PublisherId = "EHCO";

    public const string AppConfigSentinelName = "Sentinel";

#if DEBUG
    public const string DefaultQueueName = "defra.trade.ehco.remos.create.dev";
#else
    public const string DefaultQueueName = "defra.trade.ehco.remos.create";
#endif

    public const string TradeEventInfo = Common.Functions.Constants.QueueName.DefaultEventsInfoQueueName;

    public static class MessageRetry
    {
        public const int EnqueueTimeSeconds = 30;
        public const int RetryWindowSeconds = 300;
    }
}