// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests.Models;

public class GcSubscriberSettingsTests
{
    [Fact]
    public void Options_ShouldBe_AsExpected()
    {
        // Act
        var sut = new GcSubscriberSettings
        {
            GcCreatedQueue = "mocked-queue-name"
        };

        // Assert
        sut.GcCreatedQueue.ShouldBe("mocked-queue-name");
    }
}
