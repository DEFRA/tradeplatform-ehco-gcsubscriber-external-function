// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Microsoft.Azure.ServiceBus;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;

public interface IServiceBusManagerClient
{
    Task SendMessageAsync(Message message);
}