// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions;
using Defra.Trade.Common.Functions.Interfaces;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Extensions;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceBusMessage = Azure.Messaging.ServiceBus.ServiceBusMessage;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Functions;

public class GcCreatedServiceBusTriggerFunction
{
    private readonly IBaseMessageProcessorService<GeneralCertificateInbound> _baseMessageProcessorService;
    private readonly IMessageRetryService _messageRetryService;

    public GcCreatedServiceBusTriggerFunction(
        IBaseMessageProcessorService<GeneralCertificateInbound> baseMessageProcessorService,
        IMessageRetryService messageRetryService)
    {
        ArgumentNullException.ThrowIfNull(baseMessageProcessorService);
        ArgumentNullException.ThrowIfNull(messageRetryService);
        _baseMessageProcessorService = baseMessageProcessorService;
        _messageRetryService = messageRetryService;
    }

    [ServiceBusAccount(GcSubscriberSettings.ConnectionStringConfigurationKey)]
    [FunctionName(nameof(GcCreatedServiceBusTriggerFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger(queueName: GcSubscriberSettings.DefaultQueueName, IsSessionsEnabled = false)] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        ExecutionContext executionContext,
        [ServiceBus(GcSubscriberSettings.TradeEventInfo)] IAsyncCollector<ServiceBusMessage> eventStoreCollector,
        [ServiceBus(GcSubscriberSettings.DefaultQueueName)] IAsyncCollector<ServiceBusMessage> retryQueue,
        ILogger logger)
    {
        _messageRetryService.SetContext(message, retryQueue);

        await RunInternalAsync(message, messageActions, eventStoreCollector, executionContext, logger);
    }

    private static string GetGcId(BinaryData messageBody)
    {
        var gcInbound = JsonConvert.DeserializeObject<dynamic>(messageBody.ToString());

        return gcInbound?.exchangedDocument?.id ?? string.Empty;
    }

    private async Task RunInternalAsync(
            ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageReceiver,
        IAsyncCollector<ServiceBusMessage> eventStoreCollector,
        ExecutionContext executionContext, ILogger logger)
    {
        try
        {
            string gcId = GetGcId(message.Body);

            logger.MessageReceived(message.MessageId, executionContext.FunctionName, gcId);

            await _baseMessageProcessorService.ProcessAsync(executionContext.InvocationId.ToString(),
                GcSubscriberSettings.DefaultQueueName,
                GcSubscriberSettings.PublisherId,
                message,
                messageReceiver,
                eventStoreCollector,
                originalCrmPublisherId: GcSubscriberSettings.PublisherId,
                originalSource: GcSubscriberSettings.DefaultQueueName,
                originalRequestName: "Create");

            logger.ProcessMessageSuccess(message.MessageId, executionContext.FunctionName, gcId);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, ex.Message);
        }
    }
}