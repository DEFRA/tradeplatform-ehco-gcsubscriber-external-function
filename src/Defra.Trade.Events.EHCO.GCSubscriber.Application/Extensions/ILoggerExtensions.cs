// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Extensions;

public static partial class ILoggerExtensions
{
    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Messages ID : {MessageId} received on {FunctionName}, GC with ID : {GcId}")]
    public static partial void MessageReceived(this ILogger logger, string messageId, string functionName, string gcId);

    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "Processing failed for messageId: {MessageId}. Retry count: {RetryCount}")]
    public static partial void ProcessingFailed(this ILogger logger, string messageId, int retryCount);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "Successfully processed Message with ID : {MessageId} received on {FunctionName}, GC with ID : {GcId}")]
    public static partial void ProcessMessageSuccess(this ILogger logger, string messageId, string functionName, string gcId);

    [LoggerMessage(EventId = 10, Level = LogLevel.Information, Message = "Sending GC with ID : {GcId} to certificate store")]
    public static partial void SendingMessageToCertificateStore(this ILogger logger, string gcId);

    [LoggerMessage(EventId = 11, Level = LogLevel.Information, Message = "Failed sent GC with ID : {GcId} to certificate store. Message ID : {MessageId}. Retry count : {RetryCount}")]
    public static partial void SendingMessageToCertificateStoreFailure(this ILogger logger, string gcId, string messageId, int retryCount);

    [LoggerMessage(EventId = 12, Level = LogLevel.Information, Message = "Successfully sent GC with ID : {GcId} to certificate store")]
    public static partial void SendingMessageToCertificateStoreSuccess(this ILogger logger, string gcId);

    [LoggerMessage(EventId = 15, Level = LogLevel.Information, Message = "Sending GC with ID : {GcId} to enrichment queue")]
    public static partial void SendingMessageToEnrichmentQueue(this ILogger logger, string gcId);

    [LoggerMessage(EventId = 16, Level = LogLevel.Information, Message = "Failed to send GC with ID : {GcId} to enrichment queue. Message ID : {MessageId}. Retry count : {RetryCount}")]
    public static partial void SendingMessageToEnrichmentQueueFailure(this ILogger logger, string gcId, string messageId, int retryCount);

    [LoggerMessage(EventId = 17, Level = LogLevel.Information, Message = "Successfully sent GC with ID : {GcId} to enrichment queue")]
    public static partial void SendingMessageToEnrichmentQueueSuccess(this ILogger logger, string gcId);
}
