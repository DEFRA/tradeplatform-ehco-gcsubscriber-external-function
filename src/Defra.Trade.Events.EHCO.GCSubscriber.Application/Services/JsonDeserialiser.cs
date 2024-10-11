// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Text.Json;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;

/// <inheritdoc />
public class JsonDeserialiser : IJsonDeserialiser
{
    /// <inheritdoc />
    public T Deserialise<T>(string jsonContent)
        where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            throw new ArgumentNullException(nameof(jsonContent));
        }

        var deserializedObject = JsonSerializer.Deserialize<T>(jsonContent);
        return deserializedObject!;
    }
}