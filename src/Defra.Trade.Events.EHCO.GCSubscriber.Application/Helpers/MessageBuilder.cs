// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Text;
using Azure.Messaging.ServiceBus;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Helpers;

public class MessageBuilder
{
    private readonly ServiceBusMessage _message = new();

    public MessageBuilder WithUserProperty(string key, object value)
    {
        _message.ApplicationProperties.Add(key, value);

        return this;
    }

    public MessageBuilder WithBody(string body)
    {
        _message.Body = BinaryData.FromBytes(Encoding.UTF8.GetBytes(body));

        return this;
    }

    public ServiceBusMessage Build()
    {
        return _message;
    }

    public MessageBuilder MessageId(string messageId)
    {
        _message.MessageId = messageId;

        return this;
    }

    public MessageBuilder CorrelationId(string correlationId)
    {
        _message.CorrelationId = correlationId;

        return this;
    }

    public MessageBuilder ContentType(string contentType)
    {
        _message.ContentType = contentType;

        return this;
    }

    public MessageBuilder Label(string label)
    {
        _message.Subject = label;

        return this;
    }
}
