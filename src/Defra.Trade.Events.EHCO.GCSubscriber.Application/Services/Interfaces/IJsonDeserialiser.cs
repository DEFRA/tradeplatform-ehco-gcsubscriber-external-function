// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;

/// <summary>
/// Deserialises string to object.
/// </summary>
public interface IJsonDeserialiser
{
    /// <summary>
    /// Method to deserialise json string to object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jsonContent"></param>
    /// <returns>deserialised object.</returns>
    T Deserialise<T>(string jsonContent)
        where T : class, new();
}