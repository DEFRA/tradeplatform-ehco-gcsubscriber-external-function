// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Threading;
using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;

public interface IGeneralCertificateEnrichmentProcessor
{
    Task SendMessageAsync(GeneralCertificateRequest messageRequest, TradeEventMessageHeader messageHeader, CancellationToken cancellationToken = default);
}