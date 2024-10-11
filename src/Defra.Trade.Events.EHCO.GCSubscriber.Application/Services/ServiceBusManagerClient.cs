// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;
using Microsoft.Azure.ServiceBus;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;

public class ServiceBusManagerClient(IQueueClientFactory queueClientFactory) : IServiceBusManagerClient
{
    private readonly IQueueClientFactory _queueClientFactory = queueClientFactory ?? throw new ArgumentNullException(nameof(queueClientFactory));

    public async Task SendMessageAsync(Message message)
    {
        var queueClient = _queueClientFactory.CreateQueueClient();

        await queueClient.SendAsync(message);
    }
}