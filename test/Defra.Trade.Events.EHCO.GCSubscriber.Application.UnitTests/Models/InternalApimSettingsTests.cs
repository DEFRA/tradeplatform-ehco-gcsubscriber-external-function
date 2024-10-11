// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Infra.Infrastructure;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests.Models;

public class InternalApimSettingsTests
{
    [Fact]
    public void Options_ShouldBe_AsExpected()
    {
        // Act
        var sut = new InternalApimSettings
        {
            BaseUrl = "mocked-base-uri",
            SubscriptionKey = "mocked-key",
            SubscriptionKeyHeaderName = "mocked-header",
            DaeraInternalCertificateStoreApi = "mocked-api",
            Authority = "mocked-authority",
            BaseAddress = "mocked-base-uri",
            DaeraInternalCertificateStoreApiHealthEndpoint = "/api/health"
        };

        // Assert
        sut.BaseUrl.ShouldBe("mocked-base-uri");
        sut.SubscriptionKey.ShouldBe("mocked-key");
        sut.SubscriptionKeyHeaderName.ShouldBe("mocked-header");
        sut.DaeraInternalCertificateStoreApi.ShouldBe("mocked-api");
        sut.Authority.ShouldBe("mocked-authority");
        sut.BaseAddress.ShouldBe("mocked-base-uri");
        sut.DaeraInternalCertificateStoreApiHealthEndpoint.ShouldBe("/api/health");
    }

    [Fact]
    public void Options_SectionName_ShouldBe_AsExpected()
    {
        // Act
        InternalApimSettings.SectionName = "Apim:External";

        // Assert
        InternalApimSettings.SectionName.ShouldBe("Apim:External");
    }
}
