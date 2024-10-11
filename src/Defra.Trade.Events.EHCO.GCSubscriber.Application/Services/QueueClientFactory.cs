// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Infrastructure;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;

[ExcludeFromCodeCoverage]
public class QueueClientFactory(IOptions<ServiceBusQueuesSettings> serviceBusQueuesSettings) : IQueueClientFactory
{
    private readonly IOptions<ServiceBusQueuesSettings> _serviceBusQueuesSettings = serviceBusQueuesSettings
        ?? throw new ArgumentNullException(nameof(serviceBusQueuesSettings));

    private readonly ConcurrentDictionary<string, IQueueClient> _queueClients = new();

    public IQueueClient CreateQueueClient()
    {
        return _queueClients.GetOrAdd(_serviceBusQueuesSettings.Value.QueueNameEhcoRemosEnrichment, (key) =>
        {
            return new QueueClient(_serviceBusQueuesSettings.Value.ConnectionString, key, ReceiveMode.PeekLock, RetryExponential.Default);
        });
    }
}