// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Collections.Generic;
using Defra.Trade.API.CertificatesStore.V1.ApiClient.Client;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;
using DocStoreAPI = Defra.Trade.API.CertificatesStore.V1.ApiClient.Api;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests.Models;

public class DocStoreHealthApiTests
{
    private readonly Mock<DocStoreAPI.IHealthApi> _mockHealthApi;
    private readonly DocStoreHealthApi _sut;

    public DocStoreHealthApiTests()
    {
        _mockHealthApi = new Mock<DocStoreAPI.IHealthApi>();
        _sut = new DocStoreHealthApi(_mockHealthApi.Object);
    }

    [Fact]
    public void GetBasePath_ReturnsBasePathFromInnerApi()
    {
        // Arrange
        const string expectedBasePath = "https://api.example.com";
        _mockHealthApi.Setup(x => x.GetBasePath()).Returns(expectedBasePath);

        // Act
        var result = _sut.GetBasePath();

        // Assert
        result.ShouldBe(expectedBasePath);
        _mockHealthApi.Verify(x => x.GetBasePath(), Times.Once);
    }

    [Fact]
    public void GetDefaultHeaders_ReturnsDefaultHeadersFromInnerApiConfiguration()
    {
        // Arrange
        var expectedHeaders = new Dictionary<string, string>
        {
            { "x-api-key", "test-key" },
            { "Accept", "application/json" }
        };
        var mockConfiguration = new Mock<IReadableConfiguration>();
        mockConfiguration.Setup(x => x.DefaultHeaders).Returns(expectedHeaders);
        _mockHealthApi.Setup(x => x.Configuration).Returns(mockConfiguration.Object);

        // Act
        var result = _sut.GetDefaultHeaders();

        // Assert
        result.ShouldBeSameAs(expectedHeaders);
    }
}
