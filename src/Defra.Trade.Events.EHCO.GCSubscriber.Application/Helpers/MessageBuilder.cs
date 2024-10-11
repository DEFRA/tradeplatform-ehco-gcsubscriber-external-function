// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Text;
using Microsoft.Azure.ServiceBus;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Helpers;

public class MessageBuilder
{
    private readonly Message _message = new();

    public MessageBuilder WithUserProperty(string key, object value)
    {
        _message.UserProperties.Add(key, value);

        return this;
    }

    public MessageBuilder WithBody(string body)
    {
        _message.Body = Encoding.UTF8.GetBytes(body);

        return this;
    }

    public Message Build()
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
        _message.Label = label;

        return this;
    }
}