// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;

/// <summary>
/// GC subscriber - GC payload processor
/// </summary>
public interface IGcMessageProcessor
{
    /// <summary>
    /// Process GC message from SB queue.
    /// </summary>
    /// <param name="gcMessageRequest"></param>
    Task ProcessMessage(GeneralCertificateRequest gcMessageRequest);
}