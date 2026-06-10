// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Azure.Messaging.ServiceBus;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Infrastructure;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;

[ExcludeFromCodeCoverage]
public class QueueClientFactory(IOptions<ServiceBusQueuesSettings> serviceBusQueuesSettings) : IQueueClientFactory
{
    private readonly IOptions<ServiceBusQueuesSettings> _serviceBusQueuesSettings = serviceBusQueuesSettings
        ?? throw new ArgumentNullException(nameof(serviceBusQueuesSettings));

    private readonly ConcurrentDictionary<string, ServiceBusSender> _senders = new();

    public ServiceBusSender CreateQueueClient()
    {
        var settings = _serviceBusQueuesSettings.Value;
        return _senders.GetOrAdd(settings.QueueNameEhcoRemosEnrichment, (queueName) =>
        {
            var client = new ServiceBusClient(settings.ConnectionString);
            return client.CreateSender(queueName);
        });
    }
}
