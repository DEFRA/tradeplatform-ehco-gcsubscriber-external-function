// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.API.CertificatesStore.V1.ApiClient.Client;
using Defra.Trade.Common.Functions.Extensions;
using Defra.Trade.Common.Functions.Interfaces;
using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Extensions;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;

/// <summary>
/// Object initializer.
/// </summary>
/// <param name="gcMessageProcessor"></param>
/// <param name="logger"></param>
/// <exception cref="ArgumentNullException"></exception>
public class SbMessageProcessor(
    ILogger<SbMessageProcessor> logger,
    IGcMessageProcessor gcMessageProcessor,
    IGeneralCertificateEnrichmentProcessor gcEnrichmentProcessor,
    IMessageRetryContextAccessor retry) : IMessageProcessor<GeneralCertificateRequest, TradeEventMessageHeader>
{
    private readonly IGeneralCertificateEnrichmentProcessor _gcEnrichmentProcessor = gcEnrichmentProcessor ?? throw new ArgumentNullException(nameof(gcEnrichmentProcessor));
    private readonly IGcMessageProcessor _gcMessageProcessor = gcMessageProcessor ?? throw new ArgumentNullException(nameof(gcMessageProcessor));
    private readonly ILogger<SbMessageProcessor> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly TimeSpan _messageRetryEnqueueTime = new(0, 0, 0, GcSubscriberSettings.MessageRetry.EnqueueTimeSeconds);
    private readonly TimeSpan _messageRetryWindow = new(0, 0, 0, GcSubscriberSettings.MessageRetry.RetryWindowSeconds);
    private readonly IMessageRetryContextAccessor _retry = retry ?? throw new ArgumentNullException(nameof(retry));

    public Task<CustomMessageHeader> BuildCustomMessageHeaderAsync()
    {
        return Task.FromResult(new CustomMessageHeader());
    }

    public Task<string> GetSchemaAsync(TradeEventMessageHeader messageHeader)
    {
        return Task.FromResult(string.Empty);
    }

    public async Task<StatusResponse<GeneralCertificateRequest>> ProcessAsync(GeneralCertificateRequest messageRequest, TradeEventMessageHeader messageHeader)
    {
        try
        {
            await _gcMessageProcessor.ProcessMessage(messageRequest);

            await _gcEnrichmentProcessor.SendMessageAsync(messageRequest, messageHeader);
        }
        catch (ApiException ex) when (ex.ErrorCode is 0 or (>= 500 and <= 599) && _retry.Context is { } context)
        {
            _logger.SendingMessageToCertificateStoreFailure(
                messageRequest.ExchangedDocument.Id,
                context.Message.MessageId,
                context.Message.RetryCount());

            await context.RetryMessage(_messageRetryWindow, _messageRetryEnqueueTime, ex);
        }
        catch (ServiceBusCommunicationException ex) when (_retry.Context is { } context)
        {
            _logger.SendingMessageToEnrichmentQueueFailure(
                messageRequest.ExchangedDocument.Id,
                context.Message.MessageId,
                context.Message.RetryCount());

            await context.RetryMessage(_messageRetryWindow, _messageRetryEnqueueTime, ex);
        }

        return new StatusResponse<GeneralCertificateRequest> { ForwardMessage = false, Response = messageRequest };
    }

    public Task<bool> ValidateMessageLabelAsync(TradeEventMessageHeader messageHeader)
        => Task.FromResult(messageHeader.Label.Equals(GcMessageConstants.BrokerLabel, StringComparison.OrdinalIgnoreCase));
}