// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Azure.Messaging.ServiceBus;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;

public class ServiceBusManagerClient(IQueueClientFactory queueClientFactory) : IServiceBusManagerClient
{
    private readonly IQueueClientFactory _queueClientFactory = queueClientFactory ?? throw new ArgumentNullException(nameof(queueClientFactory));

    public async Task SendMessageAsync(ServiceBusMessage message)
    {
        var sender = _queueClientFactory.CreateQueueClient();

        await sender.SendMessageAsync(message);
    }
}
