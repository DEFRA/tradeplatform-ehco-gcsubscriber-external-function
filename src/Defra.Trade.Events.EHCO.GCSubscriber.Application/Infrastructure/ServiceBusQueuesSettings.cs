// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Config;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Infrastructure;

public class ServiceBusQueuesSettings : ServiceBusSettings
{
    public string QueueNameEhcoRemosEnrichment { get; set; }
    public string QueueNameEhcoRemosCreate { get; set; }
}